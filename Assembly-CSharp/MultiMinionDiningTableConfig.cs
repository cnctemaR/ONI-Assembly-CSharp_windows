using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MultiMinionDiningTableConfig : IBuildingConfig
{
	public static int SeatCount
	{
		get
		{
			return MultiMinionDiningTableConfig.seats.Length;
		}
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "MultiMinionDiningTable";
		int num = 5;
		int num2 = 1;
		string text2 = "multi_dupe_table_kanim";
		int num3 = 10;
		float num4 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] woods = MATERIALS.WOODS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, woods, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.BONUS.TIER2, none, 0.2f);
		buildingDef.WorkTime = 20f;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AddSearchTerms(SEARCH_TERMS.DINING);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.DiningTableType, false);
		go.AddOrGetDef<RocketUsageRestriction.Def>();
		go.AddOrGet<MultiMinionDiningTable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		storage.capacityKg = TableSaltTuning.SALTSHAKERSTORAGEMASS * (float)MultiMinionDiningTableConfig.SeatCount;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.RequestedItemTag = TableSaltConfig.ID.ToTag();
		manualDeliveryKG.capacity = TableSaltTuning.SALTSHAKERSTORAGEMASS * (float)MultiMinionDiningTableConfig.SeatCount;
		manualDeliveryKG.refillMass = TableSaltTuning.CONSUMABLE_RATE * (float)MultiMinionDiningTableConfig.SeatCount;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FoodFetch.IdHash;
		manualDeliveryKG.ShowStatusItem = false;
	}

	public const string ID = "MultiMinionDiningTable";

	public static readonly MultiMinionDiningTableConfig.Seat[] seats = new MultiMinionDiningTableConfig.Seat[]
	{
		new MultiMinionDiningTableConfig.Seat("anim_eat_table_kanim", "anim_bionic_eat_table_kanim", "saltshaker", new CellOffset(0, 0)),
		new MultiMinionDiningTableConfig.Seat("anim_eat_table_L_kanim", "anim_bionic_eat_table_L_kanim", "saltshaker_L", new CellOffset(-1, 0)),
		new MultiMinionDiningTableConfig.Seat("anim_eat_table_R_kanim", "anim_bionic_eat_table_R_kanim", "saltshaker_R", new CellOffset(1, 0))
	};

	public struct Seat
	{
		public readonly HashedString EatAnim
		{
			get
			{
				return this.eatAnim;
			}
		}

		public readonly HashedString ReloadElectrobankAnim
		{
			get
			{
				return this.reloadElectrobankAnim;
			}
		}

		public readonly HashedString SaltSymbol
		{
			get
			{
				return this.saltSymbol;
			}
		}

		public readonly CellOffset TableRelativeLocation
		{
			get
			{
				return this.tableRelativeLocation;
			}
		}

		public Seat(HashedString eatAnim, HashedString reloadElectrobankAnim, HashedString saltSymbol, CellOffset tableRelativeLocation)
		{
			this.eatAnim = eatAnim;
			this.reloadElectrobankAnim = reloadElectrobankAnim;
			this.saltSymbol = saltSymbol;
			this.tableRelativeLocation = tableRelativeLocation;
		}

		private readonly HashedString eatAnim;

		private readonly HashedString reloadElectrobankAnim;

		private readonly HashedString saltSymbol;

		private CellOffset tableRelativeLocation;
	}
}
