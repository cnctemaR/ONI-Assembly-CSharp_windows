using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Provides an interface to the Unity splash screen.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/DrawSplashScreenAndWatermarks.h")]
	public class SplashScreen
	{
		/// <summary>
		///   <para>Returns true once the splash screen as finished. This is once all logos have been shown for their specified duration.</para>
		/// </summary>
		public static extern bool isFinished
		{
			[FreeFunction("IsSplashScreenFinished")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Initializes the splash screen so it is ready to begin drawing. Call this before you start calling Rendering.SplashScreen.Draw. Internally this function resets the timer and prepares the logos for drawing.</para>
		/// </summary>
		[FreeFunction("BeginSplashScreen_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Begin();

		/// <summary>
		///   <para>Immediately draws the splash screen. Ensure you have called Rendering.SplashScreen.Begin before you start calling this.</para>
		/// </summary>
		[FreeFunction("DrawSplashScreen_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Draw();
	}
}
