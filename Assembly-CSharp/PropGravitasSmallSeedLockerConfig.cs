using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropGravitasSmallSeedLockerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "PropGravitasSmallSeedLocker";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASSMALLSEEDLOCKER.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASSMALLSEEDLOCKER.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_medical_locker_kanim"), "on", Grid.SceneLayer.Building, 1, 1, tier, PermittedRotations.R90, Orientation.Neutral, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium, true);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		GravitasLocker.Def def = gameObject.AddOrGetDef<GravitasLocker.Def>();
		def.CanBeClosed = false;
		def.SideScreen_OpenButtonText = UI.USERMENUACTIONS.EMPTYSTORAGE.NAME;
		def.SideScreen_OpenButtonTooltip = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP;
		def.SideScreen_CancelOpenButtonText = UI.USERMENUACTIONS.EMPTYSTORAGE.NAME_OFF;
		def.SideScreen_CancelOpenButtonTooltip = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP_OFF;
		def.SideScreen_CloseButtonText = UI.USERMENUACTIONS.CLOSESTORAGE.NAME;
		def.SideScreen_CloseButtonTooltip = UI.USERMENUACTIONS.CLOSESTORAGE.TOOLTIP;
		def.SideScreen_CancelCloseButtonText = UI.USERMENUACTIONS.CLOSESTORAGE.NAME_OFF;
		def.SideScreen_CancelCloseButtonTooltip = UI.USERMENUACTIONS.CLOSESTORAGE.TOOLTIP_OFF;
		def.ObjectsToSpawn = new string[] { "EvilFlowerSeed", "EvilFlowerSeed" };
		def.LootSymbols = new string[] { "seed1", "seed2" };
		LoreBearerUtil.AddLoreTo(gameObject, LoreBearerUtil.UnlockSpecificEntry("story_trait_morbrover_locker", CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.LOCKER.DESCRIPTION, false));
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		gameObject.AddOrGet<Demolishable>();
		SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.AddOrGet<Workable>().SetOffsets(new CellOffset[]
		{
			new CellOffset(0, 0),
			CellOffset.down
		});
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
