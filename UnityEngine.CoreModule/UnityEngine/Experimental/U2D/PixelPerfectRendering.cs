using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.U2D
{
	[NativeHeader("Runtime/2D/Common/PixelSnapping.h")]
	public static class PixelPerfectRendering
	{
		public static extern float pixelSnapSpacing
		{
			[FreeFunction("GetPixelSnapSpacing")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("SetPixelSnapSpacing")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
