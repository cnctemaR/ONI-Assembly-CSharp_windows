using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropTallPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "PropTallPlant";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPFACILITYTALLPLANT.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPFACILITYTALLPLANT.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_tall_plant_kanim"), "off", Grid.SceneLayer.Building, 1, 3, tier, tier2, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Polypropylene);
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
