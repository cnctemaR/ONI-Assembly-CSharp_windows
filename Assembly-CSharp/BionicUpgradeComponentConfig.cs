using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BionicUpgradeComponentConfig : IMultiEntityConfig
{
	public static string CraftBionicPrefabID(string sufix)
	{
		return "bionic_upgrade_" + sufix.ToLower();
	}

	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_CONSTRUCTION, global::STRINGS.ITEMS.BIONIC_BOOSTERS.CONSTRUCTION_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.CONSTRUCTION_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_CONSTRUCTION), Db.Get().Attributes.Construction.Id, "BionicConstructionBoost", new string[] { "Building1", "Building2" })), DlcManager.DLC3, "upgrade_disc_kanim", "construction", SimHashes.Creature, "PrettyGoodConductors", "DefaultBionicBoostBuilding", BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_EXCAVATION, global::STRINGS.ITEMS.BIONIC_BOOSTERS.EXCAVATION_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.EXCAVATION_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_EXCAVATION), Db.Get().Attributes.Digging.Id, "BionicExcavationBoost", new string[] { "Mining1", "Mining2" })), DlcManager.DLC3, "upgrade_disc_kanim", "excavation", SimHashes.Creature, "RoboticTools", "DefaultBionicBoostDigging", BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_MACHINERY, global::STRINGS.ITEMS.BIONIC_BOOSTERS.MACHINERY_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.MACHINERY_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_MACHINERY), Db.Get().Attributes.Machinery.Id, "BionicMachineryBoost", new string[] { "Technicals1", "Technicals2" })), DlcManager.DLC3, "upgrade_disc_kanim", "machinery", SimHashes.Creature, "Smelting", null, BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_COOKING, global::STRINGS.ITEMS.BIONIC_BOOSTERS.COOKING_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.COOKING_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_COOKING), Db.Get().Attributes.Cooking.Id, "BionicCookingBoost", new string[] { "Cooking1", "Cooking2" })), DlcManager.DLC3, "upgrade_disc_kanim", "cooking", SimHashes.Creature, "FoodRepurposing", "DefaultBionicBoostCooking", BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_MEDICINE, global::STRINGS.ITEMS.BIONIC_BOOSTERS.MEDICINE_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.MEDICINE_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_MEDICINE), Db.Get().Attributes.Caring.Id, "BionicMedicineBoost", new string[] { "Medicine1", "Medicine2" })), DlcManager.DLC3, "upgrade_disc_kanim", "medicine", SimHashes.Creature, "MedicineIV", "DefaultBionicBoostMedicine", BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_CREATIVITY, global::STRINGS.ITEMS.BIONIC_BOOSTERS.CREATIVITY_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.CREATIVITY_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_CREATIVITY), Db.Get().Attributes.Art.Id, "BionicCreativityBoost", new string[] { "Arting1", "Arting2" })), DlcManager.DLC3, "upgrade_disc_kanim", "creativity", SimHashes.Creature, "RefractiveDecor", "DefaultBionicBoostArt", BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_AGRICULTURE, global::STRINGS.ITEMS.BIONIC_BOOSTERS.AGRICULTURE_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.AGRICULTURE_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_AGRICULTURE), Db.Get().Attributes.Botanist.Id, "BionicAgricultureBoost", new string[] { "Farming1", "Farming2" })), DlcManager.DLC3, "upgrade_disc_kanim", "agriculture", SimHashes.Creature, "AnimalControl", "DefaultBionicBoostFarming", BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_HUSBANDRY, global::STRINGS.ITEMS.BIONIC_BOOSTERS.HUSBANDRY_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.HUSBANDRY_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_1, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_HUSBANDRY), Db.Get().Attributes.Ranching.Id, "BionicHusbandryBoost", new string[] { "Ranching1" })), DlcManager.DLC3, "upgrade_disc_kanim", "ranching", SimHashes.Creature, "AnimalControl", "DefaultBionicBoostRanching", BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_SCIENCE, global::STRINGS.ITEMS.BIONIC_BOOSTERS.SCIENCE_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.SCIENCE_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_1, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_SCIENCE), Db.Get().Attributes.Learning.Id, "BionicScienceBoost", new string[] { "Researching1" })), DlcManager.DLC3, "upgrade_disc_kanim", "science", SimHashes.Creature, "ParallelAutomation", null, BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_PILOTING, global::STRINGS.ITEMS.BIONIC_BOOSTERS.PILOTING_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.PILOTING_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_1, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), new BionicUpgrade_SkilledWorker.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_PILOTING), Db.Get().Attributes.SpaceNavigation.Id, "BionicPilotingBoost", new string[] { "RocketPiloting1" })), new string[] { "EXPANSION1_ID", "DLC3_ID" }, "upgrade_disc_kanim", "piloting", SimHashes.Creature, "CrashPlan", null, BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_STRENGTH, global::STRINGS.ITEMS.BIONIC_BOOSTERS.STRENGTH_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.STRENGTH_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_OnGoingEffect.Instance(smi.GetMaster(), new BionicUpgrade_OnGoingEffect.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_STRENGTH), "BionicStrengthBoost", new string[] { "Hauling1", "Hauling2" })), DlcManager.DLC3, "upgrade_disc_kanim", "strength", SimHashes.Creature, "Suits", null, BionicUpgradeComponentConfig.RarityType.Basic));
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_ATHLETICS, global::STRINGS.ITEMS.BIONIC_BOOSTERS.ATHLETICS_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.ATHLETICS_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_OnGoingEffect.Instance(smi.GetMaster(), new BionicUpgrade_OnGoingEffect.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_ATHLETICS), "BionicAthleticsBoost", null)), DlcManager.DLC3, "upgrade_disc_kanim", "athletics", SimHashes.Creature, "SolidTransport", null, BionicUpgradeComponentConfig.RarityType.Basic));
		List<GameObject> list2 = list;
		GameObject gameObject = BionicUpgradeComponentConfig.CreateNewUpgradeComponent(BionicUpgradeComponentConfig.SUFFIX_EXPLORER, global::STRINGS.ITEMS.BIONIC_BOOSTERS.EXPLORER_BOOSTER.NAME, global::STRINGS.ITEMS.BIONIC_BOOSTERS.EXPLORER_BOOSTER.DESC, global::TUNING.ITEMS.BIONIC_UPGRADES.POWER_COST.TIER_2, (StateMachine.Instance smi) => new BionicUpgrade_ExplorerBoosterMonitor.Instance(smi.GetMaster(), new BionicUpgrade_ExplorerBoosterMonitor.Def(BionicUpgradeComponentConfig.CraftBionicPrefabID(BionicUpgradeComponentConfig.SUFFIX_EXPLORER))), DlcManager.DLC3, "upgrade_disc_kanim", "explore", SimHashes.Creature, "HighTempForging", "DefaultBionicBoostExplorer", BionicUpgradeComponentConfig.RarityType.Special);
		if (gameObject != null)
		{
			gameObject.AddOrGetDef<BionicUpgrade_ExplorerBooster.Def>();
			list2.Add(gameObject);
		}
		list2.RemoveAll((GameObject t) => t == null);
		return list2;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static Tag GetBionicUpgradePrefabIDWithTraitID(string traitID)
	{
		foreach (Tag tag in BionicUpgradeComponentConfig.UpgradesData.Keys)
		{
			BionicUpgradeComponentConfig.BionicUpgradeData bionicUpgradeData = BionicUpgradeComponentConfig.UpgradesData[tag];
			if (bionicUpgradeData.relatedTrait != null && bionicUpgradeData.relatedTrait == traitID)
			{
				return tag;
			}
		}
		return Tag.Invalid;
	}

	public static GameObject CreateNewUpgradeComponent(string id, string name, string desc, float wattageCost, Func<StateMachine.Instance, StateMachine.Instance> stateMachine = null, string[] dlcIDs = null, string animFile = "upgrade_disc_kanim", string animStateName = "object", SimHashes element = SimHashes.Creature, string craftTechUnlockID = null, string relatedTrait = null, BionicUpgradeComponentConfig.RarityType rarity = BionicUpgradeComponentConfig.RarityType.Basic)
	{
		if (!DlcManager.IsAllContentSubscribed(dlcIDs))
		{
			return null;
		}
		string ID = BionicUpgradeComponentConfig.CraftBionicPrefabID(id);
		TechItem techItem = new TechItem(ID, Db.Get().TechItems, Strings.Get("STRINGS.RESEARCH.OTHER_TECH_ITEMS." + id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH.OTHER_TECH_ITEMS." + id.ToUpper() + ".DESC"), (string a, bool b) => Def.GetUISprite(Assets.GetPrefab(ID), "ui", false).first, craftTechUnlockID, DlcManager.DLC3, null, false);
		Db.Get().Techs.Get(craftTechUnlockID).AddUnlockedItemIDs(new string[] { techItem.Id });
		GameObject gameObject = EntityTemplates.CreateLooseEntity(ID, name, desc, 25f, true, Assets.GetAnim(animFile), animStateName, Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.45f, true, SORTORDER.ARTIFACTS, element, new List<Tag>
		{
			GameTags.MiscPickupable,
			GameTags.BionicUpgrade,
			GameTags.NotRoomAssignable
		});
		gameObject.AddOrGet<OccupyArea>().SetCellOffsets(EntityTemplates.GenerateOffsets(1, 1));
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.SetValues(DECOR.NONE);
		decorProvider.overrideName = gameObject.GetProperName();
		gameObject.AddOrGet<BionicUpgradeComponent>().slotID = Db.Get().AssignableSlots.BionicUpgrade.Id;
		gameObject.AddOrGet<KSelectable>();
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.PedestalDisplayable, false);
		component.requiredDlcIds = dlcIDs;
		BionicUpgradeComponentConfig.UpgradesData.Add(component.PrefabTag, new BionicUpgradeComponentConfig.BionicUpgradeData(wattageCost, animStateName, relatedTrait, rarity, stateMachine));
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Polypropylene.CreateTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ID.ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ElectrobankConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, array, array2), array, array2)
		{
			time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME,
			description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.BIONIC_COMPONENT_RECIPE_DESC, DlcManager.IsExpansion1Active() ? global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.NAME : global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.NAME, name),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "AdvancedCraftingTable" },
			requiredTech = craftTechUnlockID
		};
		return gameObject;
	}

	public const string DEFAULT_ANIM_FILE_NAME = "upgrade_disc_kanim";

	public const string ID_PREFIX = "bionic_upgrade_";

	public static string SUFFIX_CONSTRUCTION = "ConstructionBooster".ToLower();

	public static string SUFFIX_EXCAVATION = "ExcavationBooster".ToLower();

	public static string SUFFIX_MACHINERY = "MachineryBooster".ToLower();

	public static string SUFFIX_ATHLETICS = "AthleticsBooster".ToLower();

	public static string SUFFIX_COOKING = "CookingBooster".ToLower();

	public static string SUFFIX_MEDICINE = "MedicineBooster".ToLower();

	public static string SUFFIX_STRENGTH = "StrengthBooster".ToLower();

	public static string SUFFIX_CREATIVITY = "CreativityBooster".ToLower();

	public static string SUFFIX_AGRICULTURE = "AgricultureBooster".ToLower();

	public static string SUFFIX_HUSBANDRY = "HusbandryBooster".ToLower();

	public static string SUFFIX_SCIENCE = "ScienceBooster".ToLower();

	public static string SUFFIX_PILOTING = "PilotingBooster".ToLower();

	public static string SUFFIX_EXPLORER = "ExplorerBooster".ToLower();

	public static Dictionary<Tag, BionicUpgradeComponentConfig.BionicUpgradeData> UpgradesData = new Dictionary<Tag, BionicUpgradeComponentConfig.BionicUpgradeData>();

	public enum RarityType
	{
		Basic,
		Special
	}

	public class BionicUpgradeData
	{
		public float WattageCost { get; private set; }

		public Func<StateMachine.Instance, StateMachine.Instance> stateMachine { get; private set; }

		public string uiAnimName
		{
			get
			{
				if (!(this.animStateName == "object"))
				{
					return "ui_" + this.animStateName;
				}
				return "ui";
			}
		}

		public string relatedTrait { get; private set; }

		public BionicUpgradeComponentConfig.RarityType rarity { get; private set; }

		public BionicUpgradeData(float cost, string animStateName, string relatedTrait, BionicUpgradeComponentConfig.RarityType rarity, Func<StateMachine.Instance, StateMachine.Instance> smi)
		{
			this.WattageCost = cost;
			this.stateMachine = smi;
			this.animStateName = animStateName;
			this.relatedTrait = relatedTrait;
			this.rarity = rarity;
		}

		private const string DEFAULT_ANIM_STATE_NAME = "object";

		public string animStateName = "object";
	}
}
