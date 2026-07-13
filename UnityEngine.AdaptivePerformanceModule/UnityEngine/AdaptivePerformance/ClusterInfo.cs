using System;

namespace UnityEngine.AdaptivePerformance
{
	public struct ClusterInfo
	{
		public int BigCore { readonly get; set; }

		public int MediumCore { readonly get; set; }

		public int LittleCore { readonly get; set; }
	}
}
