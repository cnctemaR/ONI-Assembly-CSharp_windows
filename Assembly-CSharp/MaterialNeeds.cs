using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MaterialNeeds")]
public static class MaterialNeeds
{
	public static void UpdateNeed(Tag tag, float amount, int worldId)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(worldId);
		if (world != null)
		{
			Dictionary<Tag, float> materialNeeds = world.materialNeeds;
			float num = 0f;
			if (!materialNeeds.TryGetValue(tag, out num))
			{
				materialNeeds[tag] = 0f;
			}
			materialNeeds[tag] = num + amount;
			return;
		}
		global::Debug.LogWarning(string.Format("MaterialNeeds.UpdateNeed called with invalid worldId {0}", worldId));
	}

	public static float GetAmount(Tag tag, int worldId, bool includeRelatedWorlds)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(worldId);
		float num = 0f;
		if (world != null)
		{
			if (!includeRelatedWorlds)
			{
				float num2 = 0f;
				ClusterManager.Instance.GetWorld(worldId).materialNeeds.TryGetValue(tag, out num2);
				num += num2;
			}
			else
			{
				int parentWorldId = world.ParentWorldId;
				foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
				{
					if (worldContainer.ParentWorldId == parentWorldId)
					{
						float num3 = 0f;
						if (worldContainer.materialNeeds.TryGetValue(tag, out num3))
						{
							num += num3;
						}
					}
				}
			}
			return num;
		}
		global::Debug.LogWarning(string.Format("MaterialNeeds.GetAmount called with invalid worldId {0}", worldId));
		return 0f;
	}
}
