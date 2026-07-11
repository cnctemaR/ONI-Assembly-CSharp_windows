using System;
using System.Reflection;
using System.Runtime.CompilerServices;
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
			int num;
			if (field.DeclaringType.IsValueType)
			{
				num = UnsafeUtility.GetFieldOffsetInStruct(field);
			}
			else if (field.DeclaringType.IsClass)
			{
				num = UnsafeUtility.GetFieldOffsetInClass(field);
			}
			else
			{
				num = -1;
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

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void CopyObjectAddressToPtr(object target, void* dstPtr);

		public static bool IsBlittable<T>() where T : struct
		{
			return UnsafeUtility.IsBlittable(typeof(T));
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void* Malloc(long size, int alignment, Allocator allocator);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void Free(void* memory, Allocator allocator);

		public static bool IsValidAllocator(Allocator allocator)
		{
			return allocator > Allocator.None;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemCpy(void* destination, void* source, long size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemCpyReplicate(void* destination, void* source, int size, int count);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemCpyStride(void* destination, int destinationStride, void* source, int sourceStride, int elementSize, int count);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemMove(void* destination, void* source, long size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void MemClear(void* destination, long size);

		[ThreadSafe]
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
		internal static extern void LogError(string msg, string filename, int linenumber);

		private static bool IsValueType(Type t)
		{
			return t.IsValueType;
		}

		private static bool IsPrimitive(Type t)
		{
			return t.IsPrimitive;
		}

		private static bool IsBlittableValueType(Type t)
		{
			return UnsafeUtility.IsValueType(t) && UnsafeUtility.IsBlittable(t);
		}

		private static string GetReasonForTypeNonBlittableImpl(Type t, string name)
		{
			string text;
			if (!UnsafeUtility.IsValueType(t))
			{
				text = string.Format("{0} is not blittable because it is not of value type ({1})\n", name, t);
			}
			else if (UnsafeUtility.IsPrimitive(t))
			{
				text = string.Format("{0} is not blittable ({1})\n", name, t);
			}
			else
			{
				string text2 = "";
				foreach (FieldInfo fieldInfo in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (!UnsafeUtility.IsBlittableValueType(fieldInfo.FieldType))
					{
						text2 += UnsafeUtility.GetReasonForTypeNonBlittableImpl(fieldInfo.FieldType, string.Format("{0}.{1}", name, fieldInfo.Name));
					}
				}
				text = text2;
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

		public unsafe static void CopyPtrToStructure<T>(void* ptr, out T output) where T : struct
		{
			output = *(T*)ptr;
		}

		public unsafe static void CopyStructureToPtr<T>(ref T input, void* ptr) where T : struct
		{
			*(T*)ptr = input;
		}

		public unsafe static T ReadArrayElement<T>(void* source, int index)
		{
			return *(T*)((byte*)source + index * sizeof(T));
		}

		public unsafe static T ReadArrayElementWithStride<T>(void* source, int index, int stride)
		{
			return *(T*)((byte*)source + index * stride);
		}

		public unsafe static void WriteArrayElement<T>(void* destination, int index, T value)
		{
			*(T*)((byte*)destination + index * sizeof(T)) = value;
		}

		public unsafe static void WriteArrayElementWithStride<T>(void* destination, int index, int stride, T value)
		{
			*(T*)((byte*)destination + index * stride) = value;
		}

		public unsafe static void* AddressOf<T>(ref T output) where T : struct
		{
			return (void*)(&output);
		}

		public static int SizeOf<T>() where T : struct
		{
			return sizeof(T);
		}

		public static int AlignOf<T>() where T : struct
		{
			return 4;
		}
	}
}
