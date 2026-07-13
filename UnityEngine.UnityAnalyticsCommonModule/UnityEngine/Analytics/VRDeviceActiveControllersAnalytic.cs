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
	public class VRDeviceActiveControllersAnalytic : VRDeviceAnalyticBase
	{
		[RequiredByNativeCode]
		internal static VRDeviceActiveControllersAnalytic CreateVRDeviceActiveControllersAnalytic()
		{
			return new VRDeviceActiveControllersAnalytic();
		}

		public string[] vr_active_controllers;
	}
}
