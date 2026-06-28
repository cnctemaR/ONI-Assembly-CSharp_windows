using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class RegionLoader : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		for (int i = 0; i < REGIONS.REGIONS_TYPES.Length; i++)
		{
			RegionManager.RegionInfo regionInfo = REGIONS.REGIONS_TYPES[i];
			regionInfo.queryLayer = i;
			GameObject gameObject = new GameObject(regionInfo.name);
			gameObject.SetActive(false);
			gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.Regions).transform;
			Region region = gameObject.AddComponent<Region>();
			gameObject.AddComponent<Modifiers>();
			region.SetInfo(regionInfo);
			KPrefabID kprefabID = gameObject.AddComponent<KPrefabID>();
			kprefabID.PrefabTag = new Tag(regionInfo.prefabID);
			gameObject.AddComponent<SaveLoadRoot>().folder = Folder.Regions;
			gameObject.AddComponent<KSelectable>().entityName = regionInfo.name;
			foreach (REGIONS.RequiredComponent requiredComponent in regionInfo.additionalCmps)
			{
				requiredComponent.AddToObject(gameObject);
			}
			Assets.AddRegionPrefab(kprefabID);
		}
	}
}
