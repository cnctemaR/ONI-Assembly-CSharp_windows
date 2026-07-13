using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Diagnostics
{
	[NativeHeader("Runtime/Misc/GarbageCollectSharedAssets.h")]
	[NativeHeader("Runtime/Export/Diagnostics/DiagnosticsUtils.bindings.h")]
	public static class Utils
	{
		[FreeFunction("DiagnosticsUtils_Bindings::ForceCrash", IsThreadSafe = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ForceCrash(ForcedCrashCategory crashCategory);

		[FreeFunction("DiagnosticsUtils_Bindings::NativeAssert", IsThreadSafe = true)]
		public unsafe static void NativeAssert(string message)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = message.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Utils.NativeAssert_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("DiagnosticsUtils_Bindings::NativeError", IsThreadSafe = true)]
		public unsafe static void NativeError(string message)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = message.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Utils.NativeError_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("DiagnosticsUtils_Bindings::NativeWarning", IsThreadSafe = true)]
		public unsafe static void NativeWarning(string message)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = message.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Utils.NativeWarning_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("ValidateHeap")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ValidateHeap();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NativeAssert_Injected(ref ManagedSpanWrapper message);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NativeError_Injected(ref ManagedSpanWrapper message);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NativeWarning_Injected(ref ManagedSpanWrapper message);
	}
}
