using System;
using TUNING;
using UnityEngine;

public class OxysconceConfig : IBuildingConfig
{
	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "Oxysconce";
		int num = 1;
		int num2 = 1;
		string text2 = "oxy_sconce_kanim";
		int num3 = 10;
		float num4 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER0, tier2, 0.2f);
		buildingDef.RequiresPowerInput = false;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.ViewMode = OverlayModes.Oxygen.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.Breakable = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		new CellOffset(0, 0);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 240f;
		storage.showInUI = true;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.RequestedItemTag = SimHashes.OxyRock.CreateTag();
		manualDeliveryKG.capacity = 240f;
		manualDeliveryKG.refillMass = 96f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		go.AddOrGet<StorageMeter>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			Tutorial.Instance.oxygenGenerators.Add(game_object);
		};
	}

	public const string ID = "Oxysconce";

	private const float OXYLITE_STORAGE = 240f;
}
