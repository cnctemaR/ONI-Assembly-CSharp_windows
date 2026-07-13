using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class CryoTankConfig : IEntityConfig, IHasDlcRestrictions
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
		string text = "CryoTank";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.CRYOTANK.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.CRYOTANK.DESC;
		float num = 100f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("cryo_chamber_kanim"), "off", Grid.SceneLayer.Building, 2, 3, tier, tier2, SimHashes.Creature, null, 293f);
		gameObject.GetComponent<KAnimControllerBase>().SetFGLayer(Grid.SceneLayer.BuildingFront);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium, true);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		CryoTank cryoTank = gameObject.AddOrGet<CryoTank>();
		cryoTank.overrideAnim = "anim_interacts_cryo_activation_kanim";
		cryoTank.dropOffset = new CellOffset(1, 0);
		LoreBearerUtil.AddLoreTo(gameObject, LoreBearerUtil.UnlockSpecificEntry("cryotank_warning", UI.USERMENUACTIONS.READLORE.SEARCH_CRYO_TANK, false));
		gameObject.AddOrGet<Demolishable>().allowDemolition = false;
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CryoTank";
}
