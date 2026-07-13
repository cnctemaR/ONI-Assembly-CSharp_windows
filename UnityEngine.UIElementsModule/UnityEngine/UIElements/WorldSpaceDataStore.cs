using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal static class WorldSpaceDataStore
	{
		public static void SetWorldSpaceData(VisualElement ve, WorldSpaceData data)
		{
			WorldSpaceDataStore.m_WorldSpaceData[ve.controlid] = data;
		}

		public static WorldSpaceData GetWorldSpaceData(VisualElement ve)
		{
			WorldSpaceData worldSpaceData;
			bool flag = WorldSpaceDataStore.m_WorldSpaceData.TryGetValue(ve.controlid, out worldSpaceData);
			WorldSpaceData worldSpaceData2;
			if (flag)
			{
				worldSpaceData2 = worldSpaceData;
			}
			else
			{
				worldSpaceData2 = default(WorldSpaceData);
			}
			return worldSpaceData2;
		}

		public static void ClearWorldSpaceData(VisualElement ve)
		{
			ve.isLocalBounds3DDirty = true;
			ve.needs3DBounds = false;
			WorldSpaceDataStore.m_WorldSpaceData.Remove(ve.controlid);
			for (int i = ve.hierarchy.childCount - 1; i >= 0; i--)
			{
				WorldSpaceDataStore.ClearWorldSpaceData(ve.hierarchy[i]);
			}
		}

		public static void ClearLocalBounds3DData(VisualElement ve)
		{
			WorldSpaceData worldSpaceData = WorldSpaceDataStore.GetWorldSpaceData(ve);
			worldSpaceData.localBounds3D = WorldSpaceData.k_Empty3DBounds;
			worldSpaceData.localBoundsPicking3D = WorldSpaceData.k_Empty3DBounds;
			worldSpaceData.localBoundsWithoutNested3D = WorldSpaceData.k_Empty3DBounds;
			WorldSpaceDataStore.SetWorldSpaceData(ve, worldSpaceData);
		}

		private static Dictionary<uint, WorldSpaceData> m_WorldSpaceData = new Dictionary<uint, WorldSpaceData>();
	}
}
