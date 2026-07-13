using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/ScreenCapture/Public/CaptureScreenshot.h")]
	public static class ScreenCapture
	{
		public static void CaptureScreenshot(string filename)
		{
			ScreenCapture.CaptureScreenshot(filename, 1, ScreenCapture.StereoScreenCaptureMode.LeftEye);
		}

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

		public static Texture2D CaptureScreenshotAsTexture(int superSize)
		{
			return ScreenCapture.CaptureScreenshotAsTexture(superSize, ScreenCapture.StereoScreenCaptureMode.LeftEye);
		}

		public static Texture2D CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode stereoCaptureMode)
		{
			return ScreenCapture.CaptureScreenshotAsTexture(1, stereoCaptureMode);
		}

		public static void CaptureScreenshotIntoRenderTexture(RenderTexture renderTexture)
		{
			ScreenCapture.CaptureScreenshotIntoRenderTexture_Injected(Object.MarshalledUnityObject.Marshal<RenderTexture>(renderTexture));
		}

		private unsafe static void CaptureScreenshot(string filename, [DefaultValue("1")] int superSize, [DefaultValue("1")] ScreenCapture.StereoScreenCaptureMode CaptureMode)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filename.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ScreenCapture.CaptureScreenshot_Injected(ref managedSpanWrapper, superSize, CaptureMode);
			}
			finally
			{
				char* ptr = null;
			}
		}

		private static Texture2D CaptureScreenshotAsTexture(int superSize, ScreenCapture.StereoScreenCaptureMode stereoScreenCaptureMode)
		{
			return Unmarshal.UnmarshalUnityObject<Texture2D>(ScreenCapture.CaptureScreenshotAsTexture_Injected(superSize, stereoScreenCaptureMode));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CaptureScreenshotIntoRenderTexture_Injected(IntPtr renderTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CaptureScreenshot_Injected(ref ManagedSpanWrapper filename, [DefaultValue("1")] int superSize, [DefaultValue("1")] ScreenCapture.StereoScreenCaptureMode CaptureMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CaptureScreenshotAsTexture_Injected(int superSize, ScreenCapture.StereoScreenCaptureMode stereoScreenCaptureMode);

		public enum StereoScreenCaptureMode
		{
			LeftEye = 1,
			RightEye,
			BothEyes,
			MotionVectors
		}
	}
}
