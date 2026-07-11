using System;

namespace Unity.Collections
{
	public static class NativeLeakDetection
	{
		public static NativeLeakDetectionMode Mode
		{
			get
			{
				return (NativeLeakDetectionMode)NativeLeakDetection.s_NativeLeakDetectionMode;
			}
			set
			{
				NativeLeakDetection.s_NativeLeakDetectionMode = (int)value;
			}
		}

		private static int s_NativeLeakDetectionMode;
	}
}
