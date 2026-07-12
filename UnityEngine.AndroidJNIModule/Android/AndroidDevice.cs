using System;

namespace UnityEngine.Android
{
	public class AndroidDevice
	{
		public static AndroidHardwareType hardwareType
		{
			get
			{
				return AndroidHardwareType.Generic;
			}
		}

		public static void SetSustainedPerformanceMode(bool enabled)
		{
		}
	}
}
