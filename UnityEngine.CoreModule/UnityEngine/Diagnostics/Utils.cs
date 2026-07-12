using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Diagnostics
{
	[NativeHeader("Runtime/Export/Diagnostics/DiagnosticsUtils.bindings.h")]
	[NativeHeader("Runtime/Misc/GarbageCollectSharedAssets.h")]
	public static class Utils
	{
		[FreeFunction("DiagnosticsUtils_Bindings::ForceCrash", IsThreadSafe = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ForceCrash(ForcedCrashCategory crashCategory);

		[FreeFunction("DiagnosticsUtils_Bindings::NativeAssert", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void NativeAssert(string message);

		[FreeFunction("DiagnosticsUtils_Bindings::NativeError", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void NativeError(string message);

		[FreeFunction("DiagnosticsUtils_Bindings::NativeWarning", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void NativeWarning(string message);

		[FreeFunction("ValidateHeap")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ValidateHeap();
	}
}
