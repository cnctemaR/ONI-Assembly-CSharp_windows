using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Windows.WebCam
{
	[MovedFrom("UnityEngine.XR.WSA.WebCam")]
	[StaticAccessor("WebCam::GetInstance()", StaticAccessorType.Dot)]
	[NativeHeader("PlatformDependent/Win/Webcam/WebCam.h")]
	public class WebCam
	{
		public static extern WebCamMode Mode
		{
			[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
			[NativeName("GetWebCamMode")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
