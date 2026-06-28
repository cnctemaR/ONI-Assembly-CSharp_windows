using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropLadderConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropLadder", SETITEMS.LADDER.NAME, SETITEMS.LADDER.DESC, 50f, Assets.GetAnim("ladder_poi_kanim"), "off", Grid.SceneLayer.Building, 1, 1, global::TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		Ladder ladder = gameObject.AddOrGet<Ladder>();
		ladder.upwardsMovementSpeedMultiplier = 1.5f;
		ladder.downwardsMovementSpeedMultiplier = 1.5f;
		gameObject.AddOrGet<AnimTileable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayer = ObjectLayer.Building;
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
