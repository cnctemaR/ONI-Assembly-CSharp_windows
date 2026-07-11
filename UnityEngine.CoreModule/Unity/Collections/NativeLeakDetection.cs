using System;
using UnityEngine;

namespace Unity.Collections
{
	public static class NativeLeakDetection
	{
		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			NativeLeakDetection.s_NativeLeakDetectionMode = 1;
		}

		public static NativeLeakDetectionMode Mode
		{
			get
			{
				bool flag = NativeLeakDetection.s_NativeLeakDetectionMode == 0;
				if (flag)
				{
					NativeLeakDetection.Initialize();
				}
				return (NativeLeakDetectionMode)NativeLeakDetection.s_NativeLeakDetectionMode;
			}
			set
			{
				bool flag = NativeLeakDetection.s_NativeLeakDetectionMode != (int)value;
				if (flag)
				{
					NativeLeakDetection.s_NativeLeakDetectionMode = (int)value;
				}
			}
		}

		private static int s_NativeLeakDetectionMode;

		private const string kNativeLeakDetectionModePrefsString = "Unity.Colletions.NativeLeakDetection.Mode";
	}
}
