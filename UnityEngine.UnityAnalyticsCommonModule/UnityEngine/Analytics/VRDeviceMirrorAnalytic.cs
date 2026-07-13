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
	public class VRDeviceMirrorAnalytic : VRDeviceAnalyticBase
	{
		[RequiredByNativeCode]
		internal static VRDeviceMirrorAnalytic CreateVRDeviceMirrorAnalytic()
		{
			return new VRDeviceMirrorAnalytic();
		}

		public bool vr_device_mirror_mode;
	}
}
