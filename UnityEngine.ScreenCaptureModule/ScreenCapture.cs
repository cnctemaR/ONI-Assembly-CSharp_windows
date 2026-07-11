using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Functionality to take Screenshots.</para>
	/// </summary>
	[NativeHeader("Modules/ScreenCapture/Public/CaptureScreenshot.h")]
	public static class ScreenCapture
	{
		public static void CaptureScreenshot(string filename)
		{
			ScreenCapture.CaptureScreenshot(filename, 1, ScreenCapture.StereoScreenCaptureMode.LeftEye);
		}

		/// <summary>
		///   <para>Captures a screenshot at path filename as a PNG file.</para>
		/// </summary>
		/// <param name="filename">Pathname to save the screenshot file to.</param>
		/// <param name="superSize">Factor by which to increase resolution.</param>
		/// <param name="stereoCaptureMode">Specifies the eye texture to capture when stereo rendering is enabled.</param>
		public static void CaptureScreenshot(string filename, int superSize)
		{
			ScreenCapture.CaptureScreenshot(filename, superSize, ScreenCapture.StereoScreenCaptureMode.LeftEye);
		}

		public static void CaptureScreenshot(string filename, ScreenCapture.StereoScreenCaptureMode stereoCaptureMode)
		{
			ScreenCapture.CaptureScreenshot(filename, 1, stereoCaptureMode);
		}

		public static Texture2D CaptureScreenshotAsTexture()
		{
			return ScreenCapture.CaptureScreenshotAsTexture(1, ScreenCapture.StereoScreenCaptureMode.LeftEye);
		}

		/// <summary>
		///   <para>Captures a screenshot of the game view into a Texture2D object.</para>
		/// </summary>
		/// <param name="superSize">Factor by which to increase resolution.</param>
		/// <param name="stereoCaptureMode">Specifies the eye texture to capture when stereo rendering is enabled.</param>
		public static Texture2D CaptureScreenshotAsTexture(int superSize)
		{
			return ScreenCapture.CaptureScreenshotAsTexture(superSize, ScreenCapture.StereoScreenCaptureMode.LeftEye);
		}

		public static Texture2D CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode stereoCaptureMode)
		{
			return ScreenCapture.CaptureScreenshotAsTexture(1, stereoCaptureMode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CaptureScreenshot(string filename, [DefaultValue("1")] int superSize, [DefaultValue("1")] ScreenCapture.StereoScreenCaptureMode CaptureMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D CaptureScreenshotAsTexture(int superSize, ScreenCapture.StereoScreenCaptureMode stereoScreenCaptureMode);

		/// <summary>
		///   <para>Enumeration specifying the eye texture to capture when using ScreenCapture.CaptureScreenshot and when stereo rendering is enabled.</para>
		/// </summary>
		public enum StereoScreenCaptureMode
		{
			/// <summary>
			///   <para>The Left Eye is captured. This is the default setting for the CaptureScreenshot method.</para>
			/// </summary>
			LeftEye = 1,
			/// <summary>
			///   <para>The Right Eye is captured.</para>
			/// </summary>
			RightEye,
			/// <summary>
			///   <para>Both the left and right eyes are captured and composited into one image.</para>
			/// </summary>
			BothEyes
		}
	}
}
