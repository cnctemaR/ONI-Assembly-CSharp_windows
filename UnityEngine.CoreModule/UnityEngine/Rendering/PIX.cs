using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("PlatformDependent/Win/Profiler/PixBindings.h")]
	[NativeConditional("PLATFORM_WIN && ENABLE_PROFILER")]
	public class PIX
	{
		[FreeFunction("PIX::BeginGPUCapture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginGPUCapture();

		[FreeFunction("PIX::EndGPUCapture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndGPUCapture();

		[FreeFunction("PIX::IsAttached")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsAttached();
	}
}
