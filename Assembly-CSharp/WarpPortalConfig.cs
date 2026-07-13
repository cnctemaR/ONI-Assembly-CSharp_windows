using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class WarpPortalConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "WarpPortal";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.WARPPORTAL.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.WARPPORTAL.DESC;
		float num = 2000f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("warp_portal_sender_kanim"), "idle", Grid.SceneLayer.Building, 3, 3, tier, tier2, SimHashes.Creature, null, 293f);
		gameObject.AddTag(GameTags.NotRoomAssignable);
		gameObject.AddTag(GameTags.WarpTech);
		gameObject.AddTag(GameTags.Gravitas);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium, true);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<Operational>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGet<WarpPortal>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<Ownable>().tintWhenUnassigned = false;
		LoreBearerUtil.AddLoreTo(gameObject, LoreBearerUtil.UnlockSpecificEntry("notes_teleportation", UI.USERMENUACTIONS.READLORE.SEARCH_TELEPORTER_SENDER, false));
		gameObject.AddOrGet<Prioritizable>();
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kbatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<WarpPortal>().workLayer = Grid.SceneLayer.Building;
		inst.GetComponent<Ownable>().slotID = Db.Get().AssignableSlots.WarpPortal.Id;
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		inst.GetComponent<Deconstructable>();
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "WarpPortal";
}
