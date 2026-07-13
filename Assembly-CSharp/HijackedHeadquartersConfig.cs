using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class HijackedHeadquartersConfig : IBuildingConfig
{
	public static int GetDataBankCost(Tag printableTag, int printCount = 0)
	{
		if (HijackedHeadquartersConfig.PrintableCostOverrides.ContainsKey(printableTag))
		{
			return HijackedHeadquartersConfig.PrintableCostOverrides[printableTag];
		}
		return 25 + Math.Min(printCount, 10) * 25;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "HijackedHeadquarters";
		int num = 5;
		int num2 = 5;
		string text2 = "hijacked_hq_kanim";
		int num3 = 250;
		float num4 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] array = new string[] { SimHashes.Steel.ToString() };
		float num5 = 3200f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, array, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.Floodable = false;
		buildingDef.Entombable = true;
		buildingDef.Overheatable = false;
		buildingDef.ShowInBuildMenu = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "medium";
		buildingDef.ForegroundLayer = Grid.SceneLayer.Ground;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Steel, true);
		component.Temperature = 294.15f;
		BuildingTemplates.ExtendBuildingToGravitas(go);
		Storage storage = go.AddComponent<Storage>();
		storage.capacityKg = 275f;
		Activatable activatable = go.AddComponent<Activatable>();
		activatable.synchronizeAnims = false;
		activatable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_remote_kanim") };
		activatable.SetWorkTime(30f);
		go.AddOrGetDef<HijackedHeadquarters.Def>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.RequestedItemTag = DatabankHelper.ID;
		manualDeliveryKG.MinimumMass = 0f;
		manualDeliveryKG.refillMass = 25f;
		manualDeliveryKG.capacity = storage.capacityKg;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.Fetch.IdHash;
		manualDeliveryKG.operationalRequirement = Operational.State.Operational;
		manualDeliveryKG.ShowStatusItem = false;
		manualDeliveryKG.RoundFetchAmountToInt = true;
		manualDeliveryKG.FillToCapacity = true;
		go.AddComponent<DropToUserCapacity>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			game_object.GetComponent<Activatable>().SetOffsets(OffsetGroups.LeftOrRight);
			StoryManager.Instance.ForceCreateStory(Db.Get().Stories.HijackedHeadquarters, game_object.GetMyWorldId());
		};
	}

	public const string ID = "HijackedHeadquarters";

	private const int WIDTH = 5;

	private const int HEIGHT = 5;

	public const int DEFAULT_DATABANK_PRINT_COST = 25;

	public const int COST_INCREASE_PER_PRINT = 25;

	public const int MAX_COST_INCREASES_PER_PRINT = 10;

	private static Dictionary<Tag, int> PrintableCostOverrides = new Dictionary<Tag, int>();
}
