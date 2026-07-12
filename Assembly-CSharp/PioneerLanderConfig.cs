using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PioneerLanderConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		string text = "PioneerLander";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PIONEERLANDER.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PIONEERLANDER.DESC;
		float num = 400f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("rocket_pioneer_cargo_lander_kanim"), "grounded", Grid.SceneLayer.Building, 3, 3, tier, tier2, SimHashes.Creature, new List<Tag> { GameTags.RoomProberBuilding }, 293f);
		gameObject.AddOrGetDef<CargoLander.Def>().previewTag = "PioneerLander_Preview".ToTag();
		CargoDropperMinion.Def def = gameObject.AddOrGetDef<CargoDropperMinion.Def>();
		def.kAnimName = "anim_interacts_pioneer_cargo_lander_kanim";
		def.animName = "enter";
		gameObject.AddOrGet<MinionStorage>();
		gameObject.AddOrGet<Prioritizable>();
		Prioritizable.AddRef(gameObject);
		gameObject.AddOrGet<Operational>();
		gameObject.AddOrGet<Deconstructable>().audioSize = "large";
		gameObject.AddOrGet<Storable>();
		Placeable placeable = gameObject.AddOrGet<Placeable>();
		placeable.kAnimName = "rocket_pioneer_cargo_lander_kanim";
		placeable.animName = "place";
		placeable.placementRules = new List<Placeable.PlacementRules>
		{
			Placeable.PlacementRules.OnFoundation,
			Placeable.PlacementRules.VisibleToSpace,
			Placeable.PlacementRules.RestrictToWorld
		};
		placeable.checkRootCellOnly = true;
		EntityTemplates.CreateAndRegisterPreview("PioneerLander_Preview", Assets.GetAnim("rocket_pioneer_cargo_lander_kanim"), "place", ObjectLayer.Building, 3, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.ApplyToCells = false;
		component.objectLayers = new ObjectLayer[] { ObjectLayer.Building };
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PioneerLander";

	public const string PREVIEW_ID = "PioneerLander_Preview";

	public const float MASS = 400f;
}
