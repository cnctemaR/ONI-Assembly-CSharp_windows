using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class OilWellConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("OilWell", global::STRINGS.CREATURES.SPECIES.OIL_WELL.NAME, global::STRINGS.CREATURES.SPECIES.OIL_WELL.DESC, 2000f, Assets.GetAnim("geyser_side_oil_kanim"), "off", Grid.SceneLayer.BuildingBack, 4, 2, global::TUNING.BUILDINGS.DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER5, SimHashes.Creature, null, 293f);
		OccupyArea component = gameObject.GetComponent<OccupyArea>();
		component.objectLayer = ObjectLayer.Building;
		PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
		component2.SetElement(SimHashes.SedimentaryRock);
		component2.Temperature = 372.15f;
		BuildingAttachPoint buildingAttachPoint = gameObject.UpdateComponentRequirement<BuildingAttachPoint>(true);
		buildingAttachPoint.allowedAttachType = GameTags.OilWell;
		SoundEventVolumeCache.instance.AddVolume("geyser_side_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
		SoundEventVolumeCache.instance.AddVolume("geyser_side_methane_kanim", "GeyserMethane_erupt_LP", NOISE_POLLUTION.NOISY.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "OilWell";
}
