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
				StaticBatchingUtility.CombineRoot(staticBatchRoot);
			}
		}

		public static void Combine(GameObject[] gos, GameObject staticBatchRoot)
		{
			using (StaticBatchingUtility.s_CombineMarker.Auto())
			{
				StaticBatchingHelper.CombineMeshes(gos, staticBatchRoot);
			}
		}

		private static void CombineRoot(GameObject staticBatchRoot)
		{
			bool flag = staticBatchRoot == null;
			MeshFilter[] array;
			if (flag)
			{
				array = (MeshFilter[])Object.FindObjectsByType(typeof(MeshFilter), FindObjectsSortMode.None);
			}
			else
			{
				array = staticBatchRoot.GetComponentsInChildren<MeshFilter>();
			}
			GameObject[] array2 = new GameObject[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].gameObject;
			}
			StaticBatchingHelper.CombineMeshes(array2, staticBatchRoot);
		}

		internal static ProfilerMarker s_CombineMarker = new ProfilerMarker("StaticBatching.Combine");
	}
}
