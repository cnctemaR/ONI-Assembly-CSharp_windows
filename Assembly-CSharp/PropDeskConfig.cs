using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropDeskConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "PropDesk";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPDESK.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPDESK.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("setpiece_desk_kanim"), "off", Grid.SceneLayer.Building, 3, 2, tier, tier2, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Steel);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<LoreBearer>();
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
