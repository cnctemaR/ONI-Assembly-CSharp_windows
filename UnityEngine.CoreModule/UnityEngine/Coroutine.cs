using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Mono/Coroutine.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Coroutine : YieldInstruction
	{
		private Coroutine()
		{
		}

		~Coroutine()
		{
			Coroutine.ReleaseCoroutine(this.m_Ptr);
		}

		[FreeFunction("Coroutine::CleanupCoroutineGC", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseCoroutine(IntPtr ptr);

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(Coroutine coroutine)
			{
				return coroutine.m_Ptr;
			}
		}
	}
}
