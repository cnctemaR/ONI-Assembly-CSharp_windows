using System;

namespace Unity.Collections
{
	/// <summary>
	///   <para>Static class for native leak detection settings.</para>
	/// </summary>
	public static class NativeLeakDetection
	{
		/// <summary>
		///   <para>Set whether native memory leak detection should be enabled or disabled.</para>
		/// </summary>
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
