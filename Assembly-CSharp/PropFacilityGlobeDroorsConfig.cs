using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropFacilityGlobeDroorsConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "PropFacilityGlobeDroors";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPFACILITYGLOBEDROORS.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPFACILITYGLOBEDROORS.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_globe_kanim"), "off", Grid.SceneLayer.Building, 1, 3, tier, tier2, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Granite);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<LoreBearer>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		int num = Grid.PosToCell(inst);
		foreach (CellOffset cellOffset in component.OccupiedCellsOffsets)
		{
			Grid.GravitasFacility[Grid.OffsetCell(num, cellOffset)] = true;
		}
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
