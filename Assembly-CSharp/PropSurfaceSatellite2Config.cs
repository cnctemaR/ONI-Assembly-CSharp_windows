using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropSurfaceSatellite2Config : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string id = PropSurfaceSatellite2Config.ID;
		string text = global::STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE2.NAME;
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE2.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, text, text2, num, Assets.GetAnim("satellite2_kanim"), "off", Grid.SceneLayer.Building, 4, 4, tier, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium, true);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		SetLocker setLocker = gameObject.AddOrGet<SetLocker>();
		setLocker.overrideAnim = "anim_interacts_clothingfactory_kanim";
		setLocker.dropOffset = new Vector2I(0, 1);
		setLocker.numDataBanks = new int[] { 4, 9 };
		gameObject.AddOrGet<LoreBearer>();
		gameObject.AddOrGet<Demolishable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		SetLocker component = inst.GetComponent<SetLocker>();
		component.possible_contents_ids = new string[][]
		{
			new string[] { "ResearchDatabank", "ResearchDatabank", "ResearchDatabank" },
			new string[] { "ColdBreatherSeed", "ColdBreatherSeed", "ColdBreatherSeed" },
			new string[] { "Atmo_Suit", "Glom", "Glom", "Glom" }
		};
		component.ChooseContents();
		OccupyArea component2 = inst.GetComponent<OccupyArea>();
		component2.objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		int num = Grid.PosToCell(inst);
		foreach (CellOffset cellOffset in component2.OccupiedCellsOffsets)
		{
			Grid.GravitasFacility[Grid.OffsetCell(num, cellOffset)] = true;
		}
		RadiationEmitter radiationEmitter = inst.AddOrGet<RadiationEmitter>();
		radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
		radiationEmitter.radiusProportionalToRads = false;
		radiationEmitter.emitRadiusX = 12;
		radiationEmitter.emitRadiusY = 12;
		radiationEmitter.emitRads = 240f / ((float)radiationEmitter.emitRadiusX / 6f);
	}

	public void OnSpawn(GameObject inst)
	{
		RadiationEmitter component = inst.GetComponent<RadiationEmitter>();
		if (component != null)
		{
			component.SetEmitting(true);
		}
	}

	public static string ID = "PropSurfaceSatellite2";
}
