using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal struct Unmarshal
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T UnmarshalUnityObject<T>(IntPtr gcHandlePtr) where T : Object
		{
			bool flag = gcHandlePtr == IntPtr.Zero;
			T t;
			if (flag)
			{
				t = default(T);
			}
			else
			{
				T t2 = (T)((object)Unmarshal.FromIntPtrUnsafe(gcHandlePtr).Target);
				t = t2;
			}
			return t;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static GCHandle FromIntPtrUnsafe(IntPtr gcHandle)
		{
			return *UnsafeUtility.As<IntPtr, GCHandle>(ref gcHandle);
		}
	}
}
