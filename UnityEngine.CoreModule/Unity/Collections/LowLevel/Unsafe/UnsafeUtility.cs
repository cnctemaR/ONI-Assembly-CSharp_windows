using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Collections.LowLevel.Unsafe
{
	[NativeHeader("Runtime/Export/Unsafe/UnsafeUtility.bindings.h")]
	[StaticAccessor("UnsafeUtility", StaticAccessorType.DoubleColon)]
	public static class UnsafeUtility
	{
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetFieldOffsetInStruct(FieldInfo field);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetFieldOffsetInClass(FieldInfo field);

		public static int GetFieldOffset(FieldInfo field)
		{
			bool isValueType = field.DeclaringType.IsValueType;
			int num;
			if (isValueType)
			{
				num = UnsafeUtility.GetFieldOffsetInStruct(field);
			}
			else
			{
				bool isClass = field.DeclaringType.IsClass;
				if (isClass)
				{
					num = UnsafeUtility.GetFieldOffsetInClass(field);
				}
				else
				{
					num = -1;
				}
			}
			return num;
		}

		public unsafe static void* PinGCObjectAndGetAddress(object target, out ulong gcHandle)
		{
			return UnsafeUtility.PinSystemObjectAndGetAddress(target, out gcHandle);
		}

		public unsafe static void* PinGCArrayAndGetDataAddress(Array target, out ulong gcHandle)
		{
			return UnsafeUtility.PinSystemArrayAndGetAddress(target, out gcHandle);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* PinSystemArrayAndGetAddress(object target, out ulong gcHandle);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* PinSystemObjectAndGetAddress(object target, out ulong gcHandle);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReleaseGCObject(ulong gcHandle);

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void CopyObjectAddressToPtr(object target, void* dstPtr);

		public static bool IsBlittable<T>() where T : struct
		{
			return UnsafeUtility.IsBlittable(typeof(T));
		}

		[ThreadSafe(ThrowsException = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int CheckForLeaks();

		[ThreadSafe(ThrowsException = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ForgiveLeaks();

		[BurstAuthorizedExternalMethod]
		[ThreadSafe(ThrowsException = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern NativeLeakDetectionMode GetLeakDetectionMode();

		[BurstAuthorizedExternalMethod]
		[ThreadSafe(ThrowsException = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLeakDetectionMode(NativeLeakDetectionMode value);

		[VisibleToOtherModules(new string[] { "UnityEngine.AIModule" })]
		[BurstAuthorizedExternalMethod]
		[ThreadSafe(ThrowsException = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int LeakRecord(IntPtr handle, LeakCategory category, int callstacksToSkip);

		[VisibleToOtherModules(new string[] { "UnityEngine.AIModule" })]
		[BurstAuthorizedExternalMethod]
		[ThreadSafe(ThrowsException = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int LeakErase(IntPtr handle, LeakCategory category);

		public unsafe static void* MallocTracked(long size, int alignment, Allocator allocator, int callstacksToSkip)
		{
			return UnsafeUtility.MallocTracked(size, alignment, allocator, callstacksToSkip + 1, IntPtr.Zero);
		}

		public unsafe static void* MallocTracked(long size, int alignment, MemoryLabel label, int callstacksToSkip)
		{
			return UnsafeUtility.MallocTracked(size, alignment, label.allocator, callstacksToSkip + 1, label.pointer);
		}

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void* MallocTracked(long size, int alignment, Allocator allocator, int callstacksToSkip, IntPtr label);

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void FreeTracked(void* memory, Allocator allocator);

		public unsafe static void FreeTracked(void* memory, MemoryLabel label)
		{
			UnsafeUtility.FreeTracked(memory, label.allocator);
		}

		public unsafe static void* Malloc(long size, int alignment, Allocator allocator)
		{
			return UnsafeUtility.Malloc(size, alignment, allocator, IntPtr.Zero);
		}

		public unsafe static void* Malloc(long size, int alignment, MemoryLabel label)
		{
			return UnsafeUtility.Malloc(size, alignment, label.allocator, label.pointer);
		}

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* Malloc(long size, int alignment, Allocator allocator, IntPtr label);

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void Free(void* memory, Allocator allocator);

		public unsafe static void Free(void* memory, MemoryLabel label)
		{
			UnsafeUtility.Free(memory, label.allocator);
		}

		public static bool IsValidAllocator(Allocator allocator)
		{
			return allocator > Allocator.None;
		}

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemCpy(void* destination, void* source, long size);

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemCpyReplicate(void* destination, void* source, int size, int count);

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemCpyStride(void* destination, int destinationStride, void* source, int sourceStride, int elementSize, int count);

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemMove(void* destination, void* source, long size);

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemSwap(void* ptr1, void* ptr2, long size);

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemSet(void* destination, byte value, long size);

		public unsafe static void MemClear(void* destination, long size)
		{
			UnsafeUtility.MemSet(destination, 0, size);
		}

		[ThreadSafe(ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern int MemCmp(void* ptr1, void* ptr2, long size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int SizeOf(Type type);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsBlittable(Type type);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsUnmanaged(Type type);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValidNativeContainerElementType(Type type);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetScriptingTypeFlags(Type type);

		[ThreadSafe]
		internal unsafe static void LogError(string msg, string filename, int linenumber)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(msg, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = msg.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = filename.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				UnsafeUtility.LogError_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, linenumber);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		private static bool IsBlittableValueType(Type t)
		{
			return t.IsValueType && UnsafeUtility.IsBlittable(t);
		}

		private static string GetReasonForTypeNonBlittableImpl(Type t, string name)
		{
			bool flag = !t.IsValueType;
			string text;
			if (flag)
			{
				text = string.Format("{0} is not blittable because it is not of value type ({1})\n", name, t);
			}
			else
			{
				bool isPrimitive = t.IsPrimitive;
				if (isPrimitive)
				{
					text = string.Format("{0} is not blittable ({1})\n", name, t);
				}
				else
				{
					string text2 = "";
					foreach (FieldInfo fieldInfo in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						bool flag2 = !UnsafeUtility.IsBlittableValueType(fieldInfo.FieldType);
						if (flag2)
						{
							text2 += UnsafeUtility.GetReasonForTypeNonBlittableImpl(fieldInfo.FieldType, string.Format("{0}.{1}", name, fieldInfo.Name));
						}
					}
					text = text2;
				}
			}
			return text;
		}

		internal static bool IsArrayBlittable(Array arr)
		{
			return UnsafeUtility.IsBlittableValueType(arr.GetType().GetElementType());
		}

		internal static bool IsGenericListBlittable<T>() where T : struct
		{
			return UnsafeUtility.IsBlittable<T>();
		}

		internal static string GetReasonForArrayNonBlittable(Array arr)
		{
			Type elementType = arr.GetType().GetElementType();
			return UnsafeUtility.GetReasonForTypeNonBlittableImpl(elementType, elementType.Name);
		}

		internal static string GetReasonForGenericListNonBlittable<T>() where T : struct
		{
			Type typeFromHandle = typeof(T);
			return UnsafeUtility.GetReasonForTypeNonBlittableImpl(typeFromHandle, typeFromHandle.Name);
		}

		internal static string GetReasonForTypeNonBlittable(Type t)
		{
			return UnsafeUtility.GetReasonForTypeNonBlittableImpl(t, t.Name);
		}

		internal static string GetReasonForValueTypeNonBlittable<T>() where T : struct
		{
			Type typeFromHandle = typeof(T);
			return UnsafeUtility.GetReasonForTypeNonBlittableImpl(typeFromHandle, typeFromHandle.Name);
		}

		public static bool IsUnmanaged<T>()
		{
			return (UnsafeUtility.TypeFlagsCache<T>.flags & 1) == 0;
		}

		public static bool IsNativeContainerType<T>()
		{
			return (UnsafeUtility.TypeFlagsCache<T>.flags & 2) != 0;
		}

		public static bool IsValidNativeContainerElementType<T>()
		{
			return UnsafeUtility.TypeFlagsCache<T>.flags == 0;
		}

		public static int AlignOf<T>() where T : struct
		{
			return UnsafeUtility.SizeOf<UnsafeUtility.AlignOfHelper<T>>() - UnsafeUtility.SizeOf<T>();
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.ImageConversionModule" })]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static Span<byte> GetByteSpanFromArray(Array array, int elementSize)
		{
			bool flag = array == null || array.Length == 0;
			Span<byte> span;
			if (flag)
			{
				span = default(Span<byte>);
			}
			else
			{
				byte[] array2 = *UnsafeUtility.As<Array, byte[]>(ref array);
				span = new Span<byte>(UnsafeUtility.AddressOf<byte>(ref array2[0]), array.Length * elementSize);
			}
			return span;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Span<byte> GetByteSpanFromList<T>(List<T> list) where T : struct
		{
			return MemoryMarshal.AsBytes<T>(NoAllocHelpers.ExtractArrayFromList<T>(list).AsSpan<T>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyPtrToStructure<T>(void* ptr, out T output) where T : struct
		{
			UnsafeUtility.InternalCopyPtrToStructure<T>(ptr, out output);
		}

		private unsafe static void InternalCopyPtrToStructure<T>(void* ptr, out T output) where T : struct
		{
			output = *(T*)ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyStructureToPtr<T>(ref T input, void* ptr) where T : struct
		{
			UnsafeUtility.InternalCopyStructureToPtr<T>(ref input, ptr);
		}

		private unsafe static void InternalCopyStructureToPtr<T>(ref T input, void* ptr) where T : struct
		{
			*(T*)ptr = input;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T ReadArrayElement<T>(void* source, int index)
		{
			return *(T*)((byte*)source + (long)index * (long)sizeof(T));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T ReadArrayElementWithStride<T>(void* source, int index, int stride)
		{
			return *(T*)((byte*)source + (long)index * (long)stride);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteArrayElement<T>(void* destination, int index, T value)
		{
			*(T*)((byte*)destination + (long)index * (long)sizeof(T)) = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteArrayElementWithStride<T>(void* destination, int index, int stride, T value)
		{
			*(T*)((byte*)destination + (long)index * (long)stride) = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* AddressOf<T>(ref T output) where T : struct
		{
			return (void*)(&output);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SizeOf<T>() where T : struct
		{
			return sizeof(T);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T As<U, T>(ref U from)
		{
			return ref from;
		}

		internal static T As<T>(object from) where T : class
		{
			return from;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ref T AsRef<T>(void* ptr) where T : struct
		{
			return ref *(T*)ptr;
		}

		internal unsafe static ref T ClassAsRef<T>(void* ptr) where T : class
		{
			return ref *(T*)ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ref T ArrayElementAsRef<T>(void* ptr, int index) where T : struct
		{
			return ref *(T*)((byte*)ptr + (long)index * (long)sizeof(T));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int EnumToInt<T>(T enumValue) where T : struct, IConvertible
		{
			int num = 0;
			UnsafeUtility.InternalEnumToInt<T>(ref enumValue, ref num);
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void InternalEnumToInt<T>(ref T enumValue, ref int intValue)
		{
			intValue = enumValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EnumEquals<T>(T lhs, T rhs) where T : struct, IConvertible
		{
			return lhs == rhs;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ref T Add<[IsUnmanaged] T>(ref T source, int elementOffset) where T : struct, ValueType
		{
			return (ref source) + sizeof(T) * elementOffset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void* AsPointer<T>(ref T output)
		{
			return (void*)(&output);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LogError_Injected(ref ManagedSpanWrapper msg, ref ManagedSpanWrapper filename, int linenumber);

		private const int kIsManaged = 1;

		private const int kIsNativeContainer = 2;

		internal struct TypeFlagsCache<T>
		{
			static TypeFlagsCache()
			{
				UnsafeUtility.TypeFlagsCache<T>.Init(ref UnsafeUtility.TypeFlagsCache<T>.flags);
			}

			[BurstDiscard]
			private static void Init(ref int flags)
			{
				flags = UnsafeUtility.GetScriptingTypeFlags(typeof(T));
			}

			internal static readonly int flags;
		}

		private struct AlignOfHelper<T> where T : struct
		{
			public byte dummy;

			public T data;
		}
	}
}
