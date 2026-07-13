using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Graphics/DrawSplashScreenAndWatermarks.h")]
	public class Watermark
	{
		[FreeFunction("IsAnyWatermarkVisible")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsVisible();

		[NativeProperty("s_ShowDeveloperWatermark", false, TargetType.Field)]
		public static extern bool showDeveloperWatermark
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
