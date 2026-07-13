using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings
{
	internal static class CreateAndFillCalbacks
	{
		[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
		public unsafe static void CreateAndCallbackPinned1(IntPtr arrayPointer, IntPtr createArrayCb, int size, delegate* unmanaged[Cdecl]<byte*, IntPtr, void> callback, IntPtr arg)
		{
			ref Array ptr = ref UnsafeUtility.ClassAsRef<Array>((void*)arrayPointer);
			ref Array ptr2 = ref ptr;
			delegate*<int, Array> system.Array_u0020(System.Int32) = (void*)createArrayCb;
			ptr2 = calli(System.Array(System.Int32), size, system.Array_u0020(System.Int32));
			byte[] array;
			byte* ptr3;
			if ((array = UnsafeUtility.As<byte[]>(ptr)) == null || array.Length == 0)
			{
				ptr3 = null;
			}
			else
			{
				ptr3 = &array[0];
			}
			calli(System.Void(System.Byte*,System.IntPtr), ptr3, arg, callback);
			array = null;
		}

		[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
		public unsafe static void CreateAndCallbackPinned2(IntPtr arrayPointer, IntPtr createArrayCb, int size1, int size2, delegate* unmanaged[Cdecl]<byte*, IntPtr, void> callback, IntPtr arg)
		{
			ref Array ptr = ref UnsafeUtility.ClassAsRef<Array>((void*)arrayPointer);
			ref Array ptr2 = ref ptr;
			delegate*<int, int, Array> system.Array_u0020(System.Int32,System.Int32) = (void*)createArrayCb;
			ptr2 = calli(System.Array(System.Int32,System.Int32), size1, size2, system.Array_u0020(System.Int32,System.Int32));
			byte[,] array;
			byte* ptr3;
			if ((array = UnsafeUtility.As<byte[,]>(ptr)) == null || array.Length == 0)
			{
				ptr3 = null;
			}
			else
			{
				ptr3 = &array[0, 0];
			}
			calli(System.Void(System.Byte*,System.IntPtr), ptr3, arg, callback);
			array = null;
		}

		[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
		public unsafe static void CreateAndCallbackPinned3(IntPtr arrayPointer, IntPtr createArrayCb, int size1, int size2, int size3, delegate* unmanaged[Cdecl]<byte*, IntPtr, void> callback, IntPtr arg)
		{
			ref Array ptr = ref UnsafeUtility.ClassAsRef<Array>((void*)arrayPointer);
			ref Array ptr2 = ref ptr;
			delegate*<int, int, int, Array> system.Array_u0020(System.Int32,System.Int32,System.Int32) = (void*)createArrayCb;
			ptr2 = calli(System.Array(System.Int32,System.Int32,System.Int32), size1, size2, size2, system.Array_u0020(System.Int32,System.Int32,System.Int32));
			byte[,,] array;
			byte* ptr3;
			if ((array = UnsafeUtility.As<byte[,,]>(ptr)) == null || array.Length == 0)
			{
				ptr3 = null;
			}
			else
			{
				ptr3 = &array[0, 0, 0];
			}
			calli(System.Void(System.Byte*,System.IntPtr), ptr3, arg, callback);
			array = null;
		}
	}
}
