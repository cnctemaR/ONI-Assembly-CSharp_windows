using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class POICeresTechUnlockConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC2;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "POICeresTechUnlock";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.DLC2POITECHUNLOCKS.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.DLC2POITECHUNLOCKS.DESC;
		float num = 100f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("research_unlock_kanim"), "on", Grid.SceneLayer.Building, 3, 3, tier, tier2, SimHashes.Creature, new List<Tag>
		{
			GameTags.Gravitas,
			RoomConstraints.ConstraintTags.LightSource,
			GameTags.RoomProberBuilding
		}, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium, true);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		gameObject.AddOrGet<Demolishable>();
		POITechItemUnlockWorkable poitechItemUnlockWorkable = gameObject.AddOrGet<POITechItemUnlockWorkable>();
		poitechItemUnlockWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_research_unlock_kanim") };
		poitechItemUnlockWorkable.workTime = 5f;
		POITechItemUnlocks.Def def = gameObject.AddOrGetDef<POITechItemUnlocks.Def>();
		def.POITechUnlockIDs = new List<string> { "Campfire", "IceKettle", "WoodTile" };
		def.PopUpName = global::STRINGS.BUILDINGS.PREFABS.DLC2POITECHUNLOCKS.NAME;
		def.animName = "ceres_remote_archive_kanim";
		def.loreUnlockId = "notes_welcometoceres";
		Light2D light2D = gameObject.AddComponent<Light2D>();
		light2D.Color = LIGHT2D.POI_TECH_UNLOCK_COLOR;
		light2D.Range = 5f;
		light2D.Angle = 2.6f;
		light2D.Direction = LIGHT2D.POI_TECH_DIRECTION;
		light2D.Offset = LIGHT2D.POI_TECH_UNLOCK_OFFSET;
		light2D.overlayColour = LIGHT2D.POI_TECH_UNLOCK_OVERLAYCOLOR;
		light2D.shape = global::LightShape.Cone;
		light2D.drawOverlay = true;
		light2D.Lux = 1800;
		gameObject.AddOrGet<Prioritizable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
