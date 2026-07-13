using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[ExcludeFromDocs]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class VRDeviceAnalyticAspect : VRDeviceAnalyticBase
	{
		[RequiredByNativeCode]
		internal static VRDeviceAnalyticAspect CreateVRDeviceAnalyticAspect()
		{
			return new VRDeviceAnalyticAspect();
		}

		public float vr_aspect_ratio;
	}
}
