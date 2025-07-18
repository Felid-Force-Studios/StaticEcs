﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

[assembly: InternalsVisibleTo("Test")]
[assembly: InternalsVisibleTo("FFS.StaticEcs.Unity")]
[assembly: InternalsVisibleTo("FFS.StaticEcs.Unity.Editor")]

internal class StaticEcsException : Exception {
    public StaticEcsException() { }

    public StaticEcsException(string message) : base(message) { }

    public StaticEcsException(string message, Exception inner) : base(message, inner) { }
}

namespace FFS.Libraries.StaticEcs {
    
    public static class Const {
        internal const uint EmptyComponentMask = 1u << 31;
        internal const uint EmptyComponentMaskInv = ~EmptyComponentMask;
        internal const uint DisabledComponentMask = 1u << 30;
        internal const uint DisabledComponentMaskInv = ~DisabledComponentMask;
        internal const uint EmptyAndDisabledComponentMask = EmptyComponentMask | DisabledComponentMask;
        internal const uint EmptyAndDisabledComponentMaskInv = ~EmptyAndDisabledComponentMask;
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
    #endif
    public static class Utils {
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        public static Func<EntityGID, string> EntityGidToString = gid => $"GID {gid.Id()} : Version {gid.Version()}";
        #endif
        
        internal static ushort CalculateMaskLen(ushort count) {
            var len = (ushort) (count >> 6);
            if (count - (len << 6) != 0) {
                len++;
            }

            return len;
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
            var typeName = type.FullName!.Substring(0, type.FullName.IndexOf('`')); // Убираем суффикс `1, `2 и т.д.
            var genericArgs = string.Join(", ", Array.ConvertAll(genericArguments, GetGenericName));

            return $"{typeName}<{genericArgs}>";
        }
    }

    public interface Stateless { }
    
    #if DEBUG || FFS_ECS_ENABLE_DEBUG
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
                                                #if !FFS_ECS_DISABLE_MASKS
                                                , World<WorldType>.IMaskDebugEventListener
                                                #endif
                                                #if !FFS_ECS_DISABLE_EVENTS
                                                , World<WorldType>.IEventsDebugEventListener
                                              #endif
        where WorldType : struct, IWorldType {
        internal static readonly World<WorldType>.Entity EmptyEntity = World<WorldType>.Entity.FromIdx(uint.MaxValue);

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
                #if !FFS_ECS_DISABLE_MASKS
                World<WorldType>.AddMaskDebugEventListener(this);
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
                #if !FFS_ECS_DISABLE_MASKS
                World<WorldType>.RemoveMaskDebugEventListener(this);
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
            Write(EmptyEntity, operation, type);
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
        
        #if !FFS_ECS_DISABLE_MASKS
        public void OnMaskSet<T>(World<WorldType>.Entity entity) where T : struct, IMask {
            Write(entity, OperationType.MaskSet, TypeData<T>.Name);
        }

        public void OnMaskDelete<T>(World<WorldType>.Entity entity) where T : struct, IMask {
            Write(entity, OperationType.MaskDelete, TypeData<T>.Name);
        }
        #endif

        #if !FFS_ECS_DISABLE_EVENTS
        public void OnEventSent<T>(World<WorldType>.Event<T> value) where T : struct, IEvent {
            Write(EmptyEntity, OperationType.EventAdd, TypeData<T>.Name);
        }

        public void OnEventReadAll<T>(World<WorldType>.Event<T> value) where T : struct, IEvent {
            Write(EmptyEntity, OperationType.EventDelete, TypeData<T>.Name);
        }

        public void OnEventSuppress<T>(World<WorldType>.Event<T> value) where T : struct, IEvent {
            Write(EmptyEntity, OperationType.EventDelete, TypeData<T>.Name);
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