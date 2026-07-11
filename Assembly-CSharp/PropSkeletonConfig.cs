using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropSkeletonConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "PropSkeleton";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPSKELETON.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPSKELETON.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER5;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("skeleton_poi_kanim"), "off", Grid.SceneLayer.Building, 1, 2, tier, tier2, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Creature);
		component.Temperature = 294.15f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
