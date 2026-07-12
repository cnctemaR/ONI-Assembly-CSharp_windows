using System;
using System.Runtime.InteropServices;
using ObjCRuntimeInternal;

namespace Mono
{
	internal class CFArray : CFObject
	{
		public CFArray(IntPtr handle, bool own)
			: base(handle, own)
		{
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFArrayCreate(IntPtr allocator, IntPtr values, IntPtr numValues, IntPtr callbacks);

		static CFArray()
		{
			IntPtr intPtr = CFObject.dlopen("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", 0);
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			try
			{
				CFArray.kCFTypeArrayCallbacks = CFObject.GetIndirect(intPtr, "kCFTypeArrayCallBacks");
			}
			finally
			{
				CFObject.dlclose(intPtr);
			}
		}

		public static CFArray FromNativeObjects(params INativeObject[] values)
		{
			return new CFArray(CFArray.Create(values), true);
		}

		public unsafe static IntPtr Create(params IntPtr[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			IntPtr* ptr;
			if (values == null || values.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &values[0];
			}
			return CFArray.CFArrayCreate(IntPtr.Zero, (IntPtr)((void*)ptr), (IntPtr)values.Length, CFArray.kCFTypeArrayCallbacks);
		}

		internal unsafe static CFArray CreateArray(params IntPtr[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			IntPtr* ptr;
			if (values == null || values.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &values[0];
			}
			return new CFArray(CFArray.CFArrayCreate(IntPtr.Zero, (IntPtr)((void*)ptr), (IntPtr)values.Length, CFArray.kCFTypeArrayCallbacks), false);
		}

		public static CFArray CreateArray(params INativeObject[] values)
		{
			return new CFArray(CFArray.Create(values), true);
		}

		public static IntPtr Create(params INativeObject[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			IntPtr[] array = new IntPtr[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = values[i].Handle;
			}
			return CFArray.Create(array);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFArrayGetCount(IntPtr handle);

		public int Count
		{
			get
			{
				return (int)CFArray.CFArrayGetCount(base.Handle);
			}
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFArrayGetValueAtIndex(IntPtr handle, IntPtr index);

		public IntPtr this[int index]
		{
			get
			{
				return CFArray.CFArrayGetValueAtIndex(base.Handle, (IntPtr)index);
			}
		}

		public static T[] ArrayFromHandle<T>(IntPtr handle, Func<IntPtr, T> creation) where T : class, INativeObject
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			IntPtr intPtr = CFArray.CFArrayGetCount(handle);
			T[] array = new T[(int)intPtr];
			for (uint num = 0U; num < (uint)(int)intPtr; num += 1U)
			{
				array[(int)num] = creation(CFArray.CFArrayGetValueAtIndex(handle, (IntPtr)((long)((ulong)num))));
			}
			return array;
		}

		private static readonly IntPtr kCFTypeArrayCallbacks;
	}
}
