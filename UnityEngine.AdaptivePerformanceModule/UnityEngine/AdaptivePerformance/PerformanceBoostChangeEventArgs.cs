using System;

namespace UnityEngine.AdaptivePerformance
{
	public struct PerformanceBoostChangeEventArgs
	{
		public bool CpuBoost { readonly get; set; }

		public bool GpuBoost { readonly get; set; }
	}
}
