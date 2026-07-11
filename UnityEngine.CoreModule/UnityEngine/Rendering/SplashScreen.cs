using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Graphics/DrawSplashScreenAndWatermarks.h")]
	public class SplashScreen
	{
		public static extern bool isFinished
		{
			[FreeFunction("IsSplashScreenFinished")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("BeginSplashScreen_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Begin();

		[FreeFunction("DrawSplashScreen_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Draw();
	}
}
