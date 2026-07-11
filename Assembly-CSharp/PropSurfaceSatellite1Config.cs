using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropSurfaceSatellite1Config : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "PropSurfaceSatellite1";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE1.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE1.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("satellite1_kanim"), "off", Grid.SceneLayer.Building, 3, 3, tier, tier2, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
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
