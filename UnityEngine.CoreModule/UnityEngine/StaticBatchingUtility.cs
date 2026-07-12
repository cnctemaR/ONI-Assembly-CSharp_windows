using System;
using Unity.Profiling;

namespace UnityEngine
{
	public sealed class StaticBatchingUtility
	{
		public static void Combine(GameObject staticBatchRoot)
		{
			using (StaticBatchingUtility.s_CombineMarker.Auto())
			{
				InternalStaticBatchingUtility.CombineRoot(staticBatchRoot, null);
			}
		}

		public static void Combine(GameObject[] gos, GameObject staticBatchRoot)
		{
			using (StaticBatchingUtility.s_CombineMarker.Auto())
			{
				InternalStaticBatchingUtility.CombineGameObjects(gos, staticBatchRoot, false, null);
			}
		}

		internal static ProfilerMarker s_CombineMarker = new ProfilerMarker("StaticBatching.Combine");

		internal static ProfilerMarker s_SortMarker = new ProfilerMarker("StaticBatching.SortObjects");

		internal static ProfilerMarker s_MakeBatchMarker = new ProfilerMarker("StaticBatching.MakeBatch");
	}
}
