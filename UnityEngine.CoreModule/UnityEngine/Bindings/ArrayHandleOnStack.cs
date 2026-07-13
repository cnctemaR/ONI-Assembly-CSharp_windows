using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Bindings
{
	internal readonly struct ArrayHandleOnStack
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate void* CreateArrayDelegate(void* targetRef, int size);
	}
}
