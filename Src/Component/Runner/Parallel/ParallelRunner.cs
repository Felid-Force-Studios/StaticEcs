using System;
using System.Collections.Concurrent;
using System.Threading;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract class AbstractParallelTask {
        public abstract void Run(uint from, uint to);
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal unsafe struct Job {
        internal byte Count;
        internal fixed ulong Masks[Const.JOB_SIZE];
        internal fixed uint GlobalBlockIdx[Const.JOB_SIZE];
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    internal static class ParallelRunner<WorldType> where WorldType : struct, IWorldType {
        private static Worker[] _workers;
        private static AbstractParallelTask _task;
        private static int _threadsCount;
        private static volatile bool _disposing;
        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        private static ConcurrentQueue<(Exception, string)> _exceptions;
        #endif
        
        internal static void Create(WorldConfig cfg) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            _exceptions = new();
            #endif
            if (cfg.ParallelQueryType == ParallelQueryType.Disabled) {
                _threadsCount = -1;
                return;
            }
            #if UNITY_WEBGL
            _threadsCount = 1;
            #else
            if (cfg.ParallelQueryType == ParallelQueryType.MaxThreadsCount) {
                _threadsCount = Environment.ProcessorCount;
            } else {
                _threadsCount = (int) Math.Min(Environment.ProcessorCount, cfg.CustomThreadCount);
            }
            #endif
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
                    ref var worker = ref _workers[i];
                    worker.HasWork.Set();
                    worker.WorkDone.Dispose();
                    worker.HasWork.Dispose();
                    if (!worker.Thread.Join(10000)) {
                        throw new StaticEcsException("One of the workers didn't finish in 10 seconds");
                    }
                }

                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                _exceptions = null;
                #endif
                _workers = null;
                _threadsCount = -1;
                _task = default;
            }
        }

        public static void Run(AbstractParallelTask task, uint count, uint chunkSize, uint workersLimit) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (_task != null) {
                throw new StaticEcsException("The current task is not completed, multiple calls are not supported");
            }

            World<WorldType>.MultiThreadActive = true;
            #endif
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
            for (uint i = 0, iMax = workersCount - 1; i < iMax; i++) {
                ref var worker = ref _workers[i];
                worker.FromIndex = from;
                from += batchSize;
                worker.BeforeIndex = from;
                worker.WorkDone.Reset();
                worker.HasWork.Set();
            }

            _task.Run(from, count);
            for (uint i = 0, iMax = workersCount - 1; i < iMax; i++) {
                _workers[i].WorkDone.WaitOne();
            }
            
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
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
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            World<WorldType>.MultiThreadActive = false;
            #endif
        }

        static void ThreadFunction(object raw) {
            ref var worker = ref _workers[(int) raw];
            while (!_disposing) {
                try {
                    worker.HasWork.WaitOne();
                    if (_disposing) {
                        break;
                    }

                    worker.HasWork.Reset();
                    _task.Run(worker.FromIndex, worker.BeforeIndex);
                    worker.WorkDone.Set();
                }
                catch (Exception ex) {
                    worker.WorkDone.Set();
                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                    if (ex is not ThreadAbortException) {
                        _exceptions.Enqueue((ex, Thread.CurrentThread.Name));
                    }
                    #else
                    _ = ex;
                    #endif
                }
            }
        }

        struct Worker {
            public readonly Thread Thread;
            public readonly ManualResetEvent HasWork;
            public readonly ManualResetEvent WorkDone;
            public uint FromIndex;
            public uint BeforeIndex;

            public Worker(Thread thread) {
                Thread = thread;
                HasWork = new ManualResetEvent(false);
                WorkDone = new ManualResetEvent(true);
                FromIndex = 0;
                BeforeIndex = 0;
            }

            public Worker Start(int workerId) {
                Thread.Start(workerId);
                return this;
            }
        }
    }
}