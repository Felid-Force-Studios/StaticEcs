#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    public static class ParallelRunner<TWorld> where TWorld : struct, IWorldType {
        private static Worker[] _workers;
        // (Job[] jobs, uint[] jobIndexes, uint from, uint to, int worker)
        private static unsafe delegate*<Job[], uint[], uint, uint, int, void> _task;
        private static Job[] _jobs;
        private static uint[] _jobIndexes;
        private static int _threadsCount;
        private static int _workerSpinCount;
        private static bool _disposing;
        #if FFS_ECS_DEBUG
        private static ConcurrentQueue<(Exception, string)> _exceptions;
        #endif

        internal static Job[] CachedJobs;
        internal static uint[] CachedJobIndexes;
        internal static int CachedSize;

        internal static void Create(ParallelQueryType parallelQueryType, uint customThreadCount, uint workerSpinCount) {
            #if FFS_ECS_DEBUG
            _exceptions = new();
            #endif
            if (parallelQueryType == ParallelQueryType.Disabled) {
                _threadsCount = -1;
                return;
            }
            #if UNITY_WEBGL
            _threadsCount = 1;
            #else
            if (parallelQueryType == ParallelQueryType.MaxThreadsCount) {
                _threadsCount = Environment.ProcessorCount;
            } else {
                _threadsCount = (int) Math.Min(Environment.ProcessorCount, customThreadCount);
            }
            #endif
            _workerSpinCount = (int) workerSpinCount;
            _disposing = false;
            _workers = new Worker[_threadsCount - 1];
            for (var i = 0; i < _workers.Length; i++) {
                _workers[i] = new Worker(new Thread(ThreadFunction) { IsBackground = true });
                _workers[i].Start(i);
            }
        }

        internal static void Destroy() {
            if (_threadsCount > 0) {
                _disposing = true;
                for (var i = 0; i < _workers.Length; i++) {
                    Volatile.Write(ref _workers[i].State, Worker.StateHasWork);
                    _workers[i].Wake.Set();
                }
                for (var i = 0; i < _workers.Length; i++) {
                    if (!_workers[i].Thread.Join(10000)) {
                        throw new StaticEcsException("One of the workers didn't finish in 10 seconds");
                    }
                    _workers[i].Wake.Dispose();
                }

                #if FFS_ECS_DEBUG
                _exceptions = null;
                #endif
                _workers = null;
                _threadsCount = -1;
                _workerSpinCount = 0;
                CachedJobs = null;
                CachedJobIndexes = null;
                CachedSize = 0;
                unsafe {
                    _task = default;
                }
            }
        }

        internal static unsafe void Run(delegate*<Job[], uint[], uint, uint, int, void> task, Job[] jobs, uint[] jobIndexes, uint count, uint chunkSize, uint workersLimit) {
            #if FFS_ECS_DEBUG
            if (_task != null) {
                throw new StaticEcsException("The current task is not completed, multiple calls are not supported");
            }
            #endif
            World<TWorld>.Data.Instance.MultiThreadActive = true;
            if (count == 0 || chunkSize <= 0) {
                return;
            }

            if (workersLimit <= 0 || workersLimit > _threadsCount) {
                workersLimit = (uint) _threadsCount;
            }

            uint from = 0;
            var batchSize = count / workersLimit;
            uint workersCount;
            if (batchSize >= chunkSize) {
                workersCount = workersLimit;
            } else {
                workersCount = count / chunkSize;
                batchSize = chunkSize;
            }

            if (workersCount <= 0) {
                workersCount = 1;
            }

            _task = task;
            _jobs = jobs;
            _jobIndexes = jobIndexes;
            for (uint i = 0, iMax = workersCount - 1; i < iMax; i++) {
                ref var worker = ref _workers[i];

                worker.FromIndex = from;
                from += batchSize;
                worker.BeforeIndex = from;
                Volatile.Write(ref worker.State, Worker.StateHasWork);
                worker.Wake.Set();
            }

            _task(_jobs, _jobIndexes, from, count, _workers.Length);

            for (uint i = 0, iMax = workersCount - 1; i < iMax; i++) {
                ref var worker = ref _workers[i];
                var spinIterations = 0;
                while (Volatile.Read(ref worker.State) != Worker.StateDone) {
                    Thread.SpinWait(1);
                    if (++spinIterations > 100_000_000) {
                        throw new StaticEcsException($"Worker {i} didn't finish in time, possible deadlock");
                    }
                }
                worker.State = Worker.StateIdle;
            }

            #if FFS_ECS_DEBUG
            var error = string.Empty;
            while (!_exceptions.IsEmpty) {
                if (_exceptions.TryDequeue(out var exData)) {
                    error += $"{exData.Item2}: {exData.Item1.Message}, {exData.Item1.StackTrace}\n";
                }
            }

            if (error.Length > 0) {
                throw new StaticEcsException(error);
            }
            #endif

            _task = default;
            _jobs = default;
            _jobIndexes = default;
            World<TWorld>.Data.Instance.MultiThreadActive = false;
        }

        private static unsafe void ThreadFunction(object raw) {
            var workerId = (int) raw;
            ref var worker = ref _workers[workerId];
            while (true) {
                try {
                    var spinCount = 0;
                    while (Volatile.Read(ref worker.State) != Worker.StateHasWork) {
                        if (spinCount < _workerSpinCount) {
                            Thread.SpinWait(1);
                            spinCount++;
                        } else {
                            worker.Wake.Wait();
                            worker.Wake.Reset();
                            spinCount = 0;
                        }
                    }
                    if (_disposing) {
                        break;
                    }

                    _task(_jobs, _jobIndexes, worker.FromIndex, worker.BeforeIndex, workerId);
                    Volatile.Write(ref worker.State, Worker.StateDone);
                }
                catch (Exception ex) {
                    Volatile.Write(ref worker.State, Worker.StateDone);
                    #if FFS_ECS_DEBUG
                    if (ex is not ThreadAbortException) {
                        _exceptions.Enqueue((ex, Thread.CurrentThread.Name));
                    }
                    #else
                    _ = ex;
                    #endif
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 128)]
    struct Worker {
        public const int StateIdle = 0;
        public const int StateHasWork = 1;
        public const int StateDone = 2;
        
        [FieldOffset(0)]  public readonly Thread Thread;
        [FieldOffset(8)]  public readonly ManualResetEventSlim Wake;
        [FieldOffset(16)] public int State;
        [FieldOffset(20)] public uint FromIndex;
        [FieldOffset(24)] public uint BeforeIndex;

        public Worker(Thread thread) {
            Thread = thread;
            Wake = new ManualResetEventSlim(false, 0);
            State = StateIdle;
            FromIndex = 0;
            BeforeIndex = 0;
        }

        public readonly Worker Start(int workerId) {
            Thread.Start(workerId);
            return this;
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    internal struct ParallelData<T> {
        internal T Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParallelData(T value) {
            Value = value;
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    internal struct ParallelData<T1, T2> {
        internal T1 Value1;
        internal T2 Value2;

        public ParallelData(T1 value1, T2 value2) {
            Value1 = value1;
            Value2 = value2;
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    internal unsafe struct Job {
        internal byte Count;
        internal fixed ulong Masks[Const.JOB_SIZE];
        internal fixed uint GlobalBlockIdx[Const.JOB_SIZE];
    }
}