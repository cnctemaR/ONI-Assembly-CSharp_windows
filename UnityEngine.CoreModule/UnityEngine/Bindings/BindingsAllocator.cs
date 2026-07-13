using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Bindings
{
	[StaticAccessor("Marshalling::BindingsAllocator", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Scripting/Marshalling/BindingsAllocator.h")]
	[VisibleToOtherModules]
	internal static class BindingsAllocator
	{
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void* Malloc(int size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void Free(void* ptr);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void FreeNativeOwnedMemory(void* ptr);

		public unsafe static void* GetNativeOwnedDataPointer(void* ptr)
		{
			return ((BindingsAllocator.NativeOwnedMemory*)ptr)->data;
		}

		private struct NativeOwnedMemory
		{
			public unsafe void* data;
		}
	}
}
