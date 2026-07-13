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
	public class VRDeviceUserAnalytic : VRDeviceAnalyticBase
	{
		[RequiredByNativeCode]
		internal static VRDeviceUserAnalytic CreateVRDeviceUserAnalytic()
		{
			return new VRDeviceUserAnalytic();
		}

		public int vr_user_presence;
	}
}
