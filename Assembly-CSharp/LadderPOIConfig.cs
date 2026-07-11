using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class LadderPOIConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		int num = 1;
		int num2 = 1;
		string text = "PropLadder";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPLADDER.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPLADDER.DESC;
		float num3 = 50f;
		int num4 = num;
		int num5 = num2;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num3, Assets.GetAnim("ladder_poi_kanim"), "off", Grid.SceneLayer.Building, num4, num5, tier, tier2, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Polypropylene);
		component.Temperature = 294.15f;
		Ladder ladder = gameObject.AddOrGet<Ladder>();
		ladder.upwardsMovementSpeedMultiplier = 1.5f;
		ladder.downwardsMovementSpeedMultiplier = 1.5f;
		gameObject.AddOrGet<AnimTileable>();
		global::UnityEngine.Object.DestroyImmediate(gameObject.AddOrGet<OccupyArea>());
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(num, num2);
		occupyArea.objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
