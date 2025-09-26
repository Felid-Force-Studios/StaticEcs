using System;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

[assembly: InternalsVisibleTo("Test")]
[assembly: InternalsVisibleTo("FFS.StaticEcs.Unity")]
[assembly: InternalsVisibleTo("FFS.StaticEcs.Unity.Editor")]


namespace FFS.Libraries.StaticEcs {
    
    internal class StaticEcsException : Exception {
        public StaticEcsException() { }

        public StaticEcsException(string message) : base(message) { }

        public StaticEcsException(string message, Exception inner) : base(message, inner) { }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    internal static class Const {
        internal const int BITS_PER_LONG = 64;
        internal const int LONG_SHIFT = 6;
        internal const int LONG_OFFSET_MASK = BITS_PER_LONG - 1;
        
        internal const int ENTITIES_IN_BLOCK = BITS_PER_LONG;
        internal const int ENTITIES_IN_BLOCK_SHIFT = LONG_SHIFT;
        internal const int ENTITIES_IN_BLOCK_OFFSET_MASK = ENTITIES_IN_BLOCK - 1;
        
        internal const int BLOCK_IN_CHUNK = 64;
        internal const int BLOCK_IN_CHUNK_SHIFT = 6;
        internal const int BLOCK_IN_CHUNK_OFFSET_MASK = BLOCK_IN_CHUNK - 1;
        
        internal const int ENTITIES_IN_CHUNK = ENTITIES_IN_BLOCK * BLOCK_IN_CHUNK;
        internal const int ENTITIES_IN_CHUNK_SHIFT = ENTITIES_IN_BLOCK_SHIFT + BLOCK_IN_CHUNK_SHIFT;
        internal const int ENTITIES_IN_CHUNK_OFFSET_MASK = ENTITIES_IN_CHUNK - 1;

        #if FFS_ECS_LARGE_WORLDS
        internal const int DATA_BLOCK_SIZE = 4096;
        internal const int DATA_BLOCK_SIZE_FOR_THREADS = 4096;
        internal const int DATA_BLOCK_IN_CHUNK_FOR_THREADS = 1;
        internal const int DATA_QUERY_SHIFT_FOR_THREADS = 6;
        internal const int JOB_SIZE = 64;
        internal const int DATA_SHIFT = 12;
        #else
        internal const int DATA_BLOCK_SIZE = 256;
        internal const int DATA_BLOCK_SIZE_FOR_THREADS = 512;
        internal const int DATA_BLOCK_IN_CHUNK_FOR_THREADS = 8;
        internal const int DATA_QUERY_SHIFT_FOR_THREADS = 3;
        internal const int JOB_SIZE = 8;
        internal const int DATA_SHIFT = 8;
        #endif
        internal const int DATA_ENTITY_MASK = DATA_BLOCK_SIZE - 1;
        internal const int DATA_QUERY_SHIFT = DATA_SHIFT - BLOCK_IN_CHUNK_SHIFT;
        internal const int DATA_BLOCK_MASK = ENTITIES_IN_CHUNK / DATA_BLOCK_SIZE - 1;
        internal const int DATA_BLOCKS_IN_CHUNK = ENTITIES_IN_CHUNK / DATA_BLOCK_SIZE;
        
        internal const int MAX_NESTED_QUERY = 4;
        
        internal static readonly ulong[] DataMasks = CreateDataMasks();

        internal static ulong[] CreateDataMasks() {
            var masks = new ulong[ENTITIES_IN_CHUNK / DATA_BLOCK_SIZE];
            const int range = DATA_BLOCK_SIZE / BLOCK_IN_CHUNK;
            const ulong baseMask = range == 64 
                ? ulong.MaxValue 
                : (1UL << range) - 1;
            for (var i = 0; i < masks.Length; i++) {
                masks[i] = baseMask << (i * range);
            }

            return masks;
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    public static class Utils {
        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        public static Func<EntityGID, string> EntityGidToString = gid => $"GID {gid.Id()} : Version {gid.Version()}";
        #endif

        public static readonly byte[] DeBruijn = {
            0, 1, 17, 2, 18, 50, 3, 57, 47, 19, 22, 51, 29, 4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41, 54, 30, 38, 5, 43, 34, 59, 8,
            63, 16, 49, 56, 46, 21, 28, 32, 14, 26, 24, 40, 53, 37, 42, 7,
            62, 55, 45, 31, 13, 39, 36, 6, 61, 44, 12, 35, 60, 11, 10, 9,
        };
        
        [MethodImpl(AggressiveInlining)]
        public static int ApproximateMSB(this ulong value) {
            return value >= 0x100000000UL 
                ? value >= 0x1000000000000UL 
                    ? 64 
                    : 48 
                : value >= 0x10000UL 
                    ? 32 
                    : 16;
        }

        #region MATH
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckBitDensity(this ulong x, out int idx, out int end) {
            idx = DeBruijn[(int) (((x & (ulong) -(long) x) * 0x37E84A99DAE458FUL) >> 58)];
            end = ApproximateMSB(x);
            var total = PopCnt(x);
            var half = (end - idx) >> 1;
            return total >= half;
        }

        [MethodImpl(AggressiveInlining)]
        public static int PopCnt(this ulong x) {
            x -= (x >> 1) & 0x5555555555555555UL;
            x = (x & 0x3333333333333333UL) + ((x >> 2) & 0x3333333333333333UL);
            x = (x + (x >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
            return (int)((x * 0x0101010101010101UL) >> 56);
        }

        [MethodImpl(AggressiveInlining)]
        internal static int PopLsb(ref ulong v) {
            var val = DeBruijn[(int) (((v & (ulong) -(long) v) * 0x37E84A99DAE458FUL) >> 58)];
            v &= v - 1;
            return val;
        }

        [MethodImpl(AggressiveInlining)]
        internal static int Lsb(ulong v) {
            return DeBruijn[(int) (((v & (ulong) -(long) v) * 0x37E84A99DAE458FUL) >> 58)];
        }
        #endregion
        

        
        [MethodImpl(AggressiveInlining)]
        internal static void SetBitAtomic(this ref long mask, int bitIndex) {
            var bit = 1UL << bitIndex;
            long oldValue, newValue;
            do {
                oldValue = mask;
                newValue = (long)((ulong)oldValue | bit);
            }
            while (Interlocked.CompareExchange(ref mask, newValue, oldValue) != oldValue);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void ClearBitAtomic(this ref long mask, int bitIndex) {
            var bitMask = ~(1UL << bitIndex);
            long oldValue, newValue;
            do {
                oldValue = mask;
                newValue = (long)((ulong)oldValue & bitMask);
            }
            while (Interlocked.CompareExchange(ref mask, newValue, oldValue) != oldValue);
        }

        [MethodImpl(AggressiveInlining)]
        public static uint CalculateSize(uint value) {
            var u = value;
            if (u == 0) {
                return 0;
            }

            u--;
            u |= u >> 1;
            u |= u >> 2;
            u |= u >> 4;
            u |= u >> 8;
            u |= u >> 16;
            u++;

            return u;
        }

        [MethodImpl(AggressiveInlining)]
        public static void NormalizeThis(this ref uint value, uint min) {
            var minMinusOne = min - 1;
            value = (Math.Max(value, min) + minMinusOne) & ~minMinusOne;
        }

        [MethodImpl(AggressiveInlining)]
        public static ushort Normalize(this ushort value, ushort min) {
            var minMinusOne = min - 1;
            return (ushort) ((Math.Max(value, min) + minMinusOne) & ~minMinusOne);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static void LoopFallbackCopy<T>(T[] src, uint srcIdx, T[] dst, uint dstIdx, uint len) {
            if (len > 4) {
                Array.Copy(src, srcIdx, dst, dstIdx, len);
                return;
            }
 
            for (var i = 0; i < len; i++) {
                dst[dstIdx + i] = src[srcIdx + i];
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public static void LoopFallbackCopyReverse<T>(T[] src, uint srcIdx, T[] dst, uint dstIdx, uint len) {
            if (len > 4) {
                Array.Copy(src, srcIdx, dst, dstIdx, len);
                return;
            }
            
            for (var i = (int) (len - 1); i >= 0; i--) {
                dst[dstIdx + i] = src[srcIdx + i];
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public static void LoopFallbackClear<T>(T[] array, int idx, int len) {
            if (len > 4) {
                Array.Clear(array, idx, len);
                return;
            }
            
            for (uint i = 0; i < len; i++) {
                array[idx + i] = default;
            }
        }

        internal static string GetGenericName(this Type type) {
            if (!type.IsGenericType) {
                return type.Name;
            }

            var genericArguments = type.GetGenericArguments();
            var typeName = type.FullName!.Substring(0, type.FullName.IndexOf('`'));
            var genericArgs = string.Join(", ", Array.ConvertAll(genericArguments, GetGenericName));

            return $"{typeName}<{genericArgs}>";
        }
    }

    public interface Stateless { }
    
    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> where WorldType : struct, IWorldType {
        internal static FileLogger<WorldType> FileLogger;
        
        public static void CreateFileLogger(string logsFilePath, OperationType[] excludedOperations = null, ICsvColumnHandler<WorldType>[] columnWriters = null) {
            if (Status != WorldStatus.Created) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateFileLogger, world status not `Created`");
            if (FileLogger != null) throw new StaticEcsException("File logger already added");

            FileLogger = new FileLogger<WorldType>(logsFilePath, excludedOperations, columnWriters);
            FileLogger.Enable();
        }
        
        public static void EnableFileLogger() {
            if (FileLogger == null) {
                throw new StaticEcsException("File logger not added");
            }
            
            FileLogger.Enable();
        }
        
        public static void DisableFileLogger() {
            if (FileLogger == null) {
                throw new StaticEcsException("File logger not added");
            }
            
            FileLogger.Disable();
        }
    }

    public enum OperationType : byte {
        EntityCreate,
        EntityDestroy,
        ComponentRef,
        ComponentAdd,
        ComponentPut,
        ComponentDelete,
        TagAdd,
        TagDelete,
        MaskSet,
        MaskDelete,
        EventAdd,
        EventDelete,
        SystemCallInit,
        SystemCallUpdate,
        SystemCallDestroy,
    }


    internal static class TypeData<T> {
        public static readonly string Name;

        static TypeData() {
            Name = typeof(T).Name;
        }
    }
    
    public interface ICsvWriter {
        public void WriteColumn(StreamWriter writer);
    }

    public interface ICsvColumnHandler<WorldType> where WorldType : struct, IWorldType {
        public string ColumnName();

        public void TryAddColumn(World<WorldType>.Entity entity, StreamWriter writer);
    }

    public struct CsvColumnComponentHandler<WorldType, T> : ICsvColumnHandler<WorldType>
        where WorldType : struct, IWorldType
        where T : struct, IComponent, ICsvWriter {
        public string ColumnName() {
            return typeof(T).Name;
        }

        public void TryAddColumn(World<WorldType>.Entity entity, StreamWriter writer) {
            if (World<WorldType>.Components<T>.Value.IsRegistered() && World<WorldType>.Components<T>.Value.Has(entity)) {
                World<WorldType>.Components<T>.Value.RefInternal(entity).WriteColumn(writer);
            }

            writer.Write(";");
        }
    }

    internal static class FileLogger {
        internal static readonly string[] MethodNames = {
            "CREATE",
            "DESTROY",
            "REF",
            "ADD",
            "PUT",
            "DEL",
            "ADD",
            "DEL",
            "SET",
            "DEL",
            "EVENT_ADD",
            "EVENT_DEL",
            "SYS_INIT",
            "SYS_UPDATE",
            "SYS_DESTROY",
        };
        
        internal static string MethodName(OperationType operationType) {
            return MethodNames[(int) operationType];
        }
    }

    public sealed class FileLogger<WorldType> : World<WorldType>.IWorldDebugEventListener,
                                                World<WorldType>.IComponentsDebugEventListener
                                                #if !FFS_ECS_DISABLE_TAGS
                                                , World<WorldType>.ITagDebugEventListener
                                                #endif
                                                #if !FFS_ECS_DISABLE_EVENTS
                                                , World<WorldType>.IEventsDebugEventListener
                                              #endif
        where WorldType : struct, IWorldType {
        internal static readonly uint EmptyEntity = uint.MaxValue;

        internal readonly string LogsFilePath;
        internal readonly DateTime DateTime;
        internal readonly bool[] ExcludedOperations;
        internal readonly ICsvColumnHandler<WorldType>[] ColumnWriters;

        internal StreamWriter Writer;
        internal int Lines;
        internal int Part;
        internal bool Enabled;

        internal FileLogger(string logsFilePath, OperationType[] excludedOperations = null, ICsvColumnHandler<WorldType>[] columnWriters = null) {
            LogsFilePath = logsFilePath;
            DateTime = DateTime.Now;
            ExcludedOperations = new bool[Enum.GetValues(typeof(OperationType)).Length];
            if (excludedOperations != null) {
                foreach (var operation in excludedOperations) {
                    ExcludedOperations[(int) operation] = true;
                }
            }

            ColumnWriters = columnWriters ?? Array.Empty<ICsvColumnHandler<WorldType>>();
            var files = Directory.GetFiles(logsFilePath);
            foreach (var file in files) {
                if (file.Contains($"entities_log_{typeof(WorldType).Name}")) {
                    try {
                        File.Delete(file);
                    }
                    catch (Exception _) {
                        // ignored
                    }
                }
            }

            CreateWriter();
        }

        public void Enable() {
            if (!Enabled) {
                World<WorldType>.AddWorldDebugEventListener(this);
                World<WorldType>.AddComponentsDebugEventListener(this);
                #if !FFS_ECS_DISABLE_TAGS
                World<WorldType>.AddTagDebugEventListener(this);
                #endif
                #if !FFS_ECS_DISABLE_EVENTS
                World<WorldType>.Events.AddEventsDebugEventListener(this);
                #endif
                Enabled = true;
            }
        }

        public void Disable() {
            if (Enabled) {
                World<WorldType>.RemoveWorldDebugEventListener(this);
                World<WorldType>.RemoveComponentsDebugEventListener(this);
                #if !FFS_ECS_DISABLE_TAGS
                World<WorldType>.RemoveTagDebugEventListener(this);
                #endif
                #if !FFS_ECS_DISABLE_EVENTS
                World<WorldType>.Events.RemoveEventsDebugEventListener(this);
                #endif
                Writer.Flush();
                Enabled = false;
            }
        }

        private void CreateWriter() {
            Writer = new StreamWriter($"{LogsFilePath}/entities_log_{typeof(WorldType).Name}_{Part}_{DateTime:yyyy_MM_dd_HH_mm_ss}.csv", true, Encoding.UTF8);

            Writer.Write("EntityId");
            Writer.Write(";");
            Writer.Write("Version");
            Writer.Write(";");
            foreach (var columnWriter in ColumnWriters) {
                Writer.Write(columnWriter.ColumnName());
                Writer.Write(";");
            }

            Writer.Write("Operation");
            Writer.Write(";");
            Writer.WriteLine("Type");
        }
        
        public void Flush() {
            Writer.Flush();
        }

        public void OnWorldInitialized() {
            Enable();
        }

        public void OnWorldDestroyed() {
            Disable();
            Writer.Dispose();
        }

        public void OnWorldResized(uint capacity) { }
        
        public void Write(OperationType operation, string type) {
            Write(new World<WorldType>.Entity(EmptyEntity), operation, type);
        }

        public void Write(World<WorldType>.Entity entity, OperationType operation, string type) {
            if (ExcludedOperations[(int) operation]) {
                return;
            }
            
            if (entity._id != uint.MaxValue) {
                Writer.Write(entity._id);
            }

            Writer.Write(";");
            
            if (entity._id != uint.MaxValue) {
                Writer.Write(entity.Gid().id);
            }

            Writer.Write(";");

            foreach (var columnWriter in ColumnWriters) {
                if (entity._id != uint.MaxValue) {
                    columnWriter.TryAddColumn(entity, Writer);
                } else {
                    Writer.Write(";");
                }
            }

            Writer.Write(FileLogger.MethodName(operation));
            Writer.Write(";");
            if (type != null) {
                Writer.WriteLine(type);
            } else {
                Writer.WriteLine();
            }

            Lines++;
            if (Lines >= 1_000_000) {
                Lines = 0;
                Part++;
                Writer.Dispose();
                CreateWriter();
            }
        }

        public void OnEntityCreated(World<WorldType>.Entity entity) {
            Write(entity, OperationType.EntityCreate, null);
        }

        public void OnEntityDestroyed(World<WorldType>.Entity entity) {
            Write(entity, OperationType.EntityDestroy, null);
        }

        public void OnComponentRef<T>(World<WorldType>.Entity entity, ref T component) where T : struct, IComponent {
            Write(entity, OperationType.ComponentRef, TypeData<T>.Name);
        }

        public void OnComponentAdd<T>(World<WorldType>.Entity entity, ref T component) where T : struct, IComponent {
            Write(entity, OperationType.ComponentAdd, TypeData<T>.Name);
        }

        public void OnComponentPut<T>(World<WorldType>.Entity entity, ref T component) where T : struct, IComponent {
            Write(entity, OperationType.ComponentPut, TypeData<T>.Name);
        }

        public void OnComponentDelete<T>(World<WorldType>.Entity entity, ref T component) where T : struct, IComponent {
            Write(entity, OperationType.ComponentDelete, TypeData<T>.Name);
        }
        #if !FFS_ECS_DISABLE_TAGS
        public void OnTagAdd<T>(World<WorldType>.Entity entity) where T : struct, ITag {
            Write(entity, OperationType.TagAdd, TypeData<T>.Name);
        }

        public void OnTagDelete<T>(World<WorldType>.Entity entity) where T : struct, ITag {
            Write(entity, OperationType.TagDelete, TypeData<T>.Name);
        }
        #endif

        #if !FFS_ECS_DISABLE_EVENTS
        public void OnEventSent<T>(World<WorldType>.Event<T> value) where T : struct, IEvent {
            Write(new World<WorldType>.Entity(EmptyEntity), OperationType.EventAdd, TypeData<T>.Name);
        }

        public void OnEventReadAll<T>(World<WorldType>.Event<T> value) where T : struct, IEvent {
            Write(new World<WorldType>.Entity(EmptyEntity), OperationType.EventDelete, TypeData<T>.Name);
        }

        public void OnEventSuppress<T>(World<WorldType>.Event<T> value) where T : struct, IEvent {
            Write(new World<WorldType>.Entity(EmptyEntity), OperationType.EventDelete, TypeData<T>.Name);
        }
        #endif
    }
    #endif
}

#if ENABLE_IL2CPP
namespace Unity.IL2CPP.CompilerServices {
    using System;

    internal enum Option {
        NullChecks = 1,
        ArrayBoundsChecks = 2,
        DivideByZeroChecks = 3
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    internal class Il2CppSetOptionAttribute : Attribute {
        public Option Option { get; }
        public object Value { get; }

        public Il2CppSetOptionAttribute(Option option, object value) {
            Option = option;
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    internal class Il2CppEagerStaticClassConstructionAttribute : Attribute { }
}
#endif