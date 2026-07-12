using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Windows.WebCam
{
	[NativeHeader("PlatformDependent/Win/Webcam/WebCam.h")]
	[MovedFrom("UnityEngine.XR.WSA.WebCam")]
	[StaticAccessor("WebCam::GetInstance()", StaticAccessorType.Dot)]
	public class WebCam
	{
		public static extern WebCamMode Mode
		{
			[NativeName("GetWebCamMode")]
			[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
