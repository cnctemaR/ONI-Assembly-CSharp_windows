using System;
using STRINGS;
using TUNING;

namespace Database
{
	public class SkillPerks : ResourceSet<SkillPerk>
	{
		public SkillPerks(ResourceSet parent)
			: base("SkillPerks", parent)
		{
			this.IncreaseDigSpeedSmall = base.Add(new SkillAttributePerk("IncreaseDigSpeedSmall", Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_MINER.NAME, false));
			this.IncreaseDigSpeedMedium = base.Add(new SkillAttributePerk("IncreaseDigSpeedMedium", Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MINER.NAME, false));
			this.IncreaseDigSpeedLarge = base.Add(new SkillAttributePerk("IncreaseDigSpeedLarge", Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_MINER.NAME, false));
			this.CanDigVeryFirm = base.Add(new SimpleSkillPerk("CanDigVeryFirm", UI.ROLES_SCREEN.PERKS.CAN_DIG_VERY_FIRM.DESCRIPTION));
			this.CanDigNearlyImpenetrable = base.Add(new SimpleSkillPerk("CanDigAbyssalite", UI.ROLES_SCREEN.PERKS.CAN_DIG_NEARLY_IMPENETRABLE.DESCRIPTION));
			this.CanDigSuperDuperHard = base.Add(new SimpleSkillPerk("CanDigDiamondAndObsidan", UI.ROLES_SCREEN.PERKS.CAN_DIG_SUPER_SUPER_HARD.DESCRIPTION));
			this.CanDigRadioactiveMaterials = base.Add(new SimpleSkillPerk("CanDigCorium", UI.ROLES_SCREEN.PERKS.CAN_DIG_RADIOACTIVE_MATERIALS.DESCRIPTION));
			this.CanDigUnobtanium = base.Add(new SimpleSkillPerk("CanDigUnobtanium", UI.ROLES_SCREEN.PERKS.CAN_DIG_UNOBTANIUM.DESCRIPTION));
			this.IncreaseConstructionSmall = base.Add(new SkillAttributePerk("IncreaseConstructionSmall", Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME, false));
			this.IncreaseConstructionMedium = base.Add(new SkillAttributePerk("IncreaseConstructionMedium", Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.BUILDER.NAME, false));
			this.IncreaseConstructionLarge = base.Add(new SkillAttributePerk("IncreaseConstructionLarge", Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_BUILDER.NAME, false));
			this.IncreaseConstructionMechatronics = base.Add(new SkillAttributePerk("IncreaseConstructionMechatronics", Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME, false));
			this.CanDemolish = base.Add(new SimpleSkillPerk("CanDemonlish", UI.ROLES_SCREEN.PERKS.CAN_DEMOLISH.DESCRIPTION));
			this.IncreaseLearningSmall = base.Add(new SkillAttributePerk("IncreaseLearningSmall", Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME, false));
			this.IncreaseLearningMedium = base.Add(new SkillAttributePerk("IncreaseLearningMedium", Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.RESEARCHER.NAME, false));
			this.IncreaseLearningLarge = base.Add(new SkillAttributePerk("IncreaseLearningLarge", Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME, false));
			this.IncreaseLearningLargeSpace = base.Add(new SkillAttributePerk("IncreaseLearningLargeSpace", Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SPACE_RESEARCHER.NAME, false));
			this.IncreaseBotanySmall = base.Add(new SkillAttributePerk("IncreaseBotanySmall", Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_FARMER.NAME, false));
			this.IncreaseBotanyMedium = base.Add(new SkillAttributePerk("IncreaseBotanyMedium", Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.FARMER.NAME, false));
			this.IncreaseBotanyLarge = base.Add(new SkillAttributePerk("IncreaseBotanyLarge", Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_FARMER.NAME, false));
			this.CanFarmTinker = base.Add(new SimpleSkillPerk("CanFarmTinker", UI.ROLES_SCREEN.PERKS.CAN_FARM_TINKER.DESCRIPTION));
			this.CanIdentifyMutantSeeds = base.Add(new SimpleSkillPerk("CanIdentifyMutantSeeds", UI.ROLES_SCREEN.PERKS.CAN_IDENTIFY_MUTANT_SEEDS.DESCRIPTION));
			this.CanFarmStation = base.Add(new SimpleSkillPerk("CanFarmStation", UI.ROLES_SCREEN.PERKS.CAN_FARM_STATION.DESCRIPTION));
			this.IncreaseRanchingSmall = base.Add(new SkillAttributePerk("IncreaseRanchingSmall", Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.RANCHER.NAME, false));
			this.IncreaseRanchingMedium = base.Add(new SkillAttributePerk("IncreaseRanchingMedium", Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.SENIOR_RANCHER.NAME, false));
			this.CanWrangleCreatures = base.Add(new SimpleSkillPerk("CanWrangleCreatures", UI.ROLES_SCREEN.PERKS.CAN_WRANGLE_CREATURES.DESCRIPTION));
			this.CanUseRanchStation = base.Add(new SimpleSkillPerk("CanUseRanchStation", UI.ROLES_SCREEN.PERKS.CAN_USE_RANCH_STATION.DESCRIPTION));
			this.CanUseMilkingStation = base.Add(new SimpleSkillPerk("CanUseMilkingStation", UI.ROLES_SCREEN.PERKS.CAN_USE_MILKING_STATION.DESCRIPTION));
			this.IncreaseAthleticsSmall = base.Add(new SkillAttributePerk("IncreaseAthleticsSmall", Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HAULER.NAME, false));
			this.IncreaseAthleticsMedium = base.Add(new SkillAttributePerk("IncreaseAthletics", Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.SUIT_EXPERT.NAME, false));
			this.IncreaseAthleticsLarge = base.Add(new SkillAttributePerk("IncreaseAthleticsLarge", Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SUIT_DURABILITY.NAME, false));
			this.IncreaseStrengthGofer = base.Add(new SkillAttributePerk("IncreaseStrengthGofer", Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HAULER.NAME, false));
			this.IncreaseStrengthCourier = base.Add(new SkillAttributePerk("IncreaseStrengthCourier", Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME, false));
			this.IncreaseStrengthGroundskeeper = base.Add(new SkillAttributePerk("IncreaseStrengthGroundskeeper", Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HANDYMAN.NAME, false));
			this.IncreaseStrengthPlumber = base.Add(new SkillAttributePerk("IncreaseStrengthPlumber", Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.PLUMBER.NAME, false));
			this.IncreaseCarryAmountSmall = base.Add(new SkillAttributePerk("IncreaseCarryAmountSmall", Db.Get().Attributes.CarryAmount.Id, 400f, DUPLICANTS.ROLES.HAULER.NAME, false));
			this.IncreaseCarryAmountMedium = base.Add(new SkillAttributePerk("IncreaseCarryAmountMedium", Db.Get().Attributes.CarryAmount.Id, 800f, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME, false));
			this.IncreaseArtSmall = base.Add(new SkillAttributePerk("IncreaseArtSmall", Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME, false));
			this.IncreaseArtMedium = base.Add(new SkillAttributePerk("IncreaseArt", Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.ARTIST.NAME, false));
			this.IncreaseArtLarge = base.Add(new SkillAttributePerk("IncreaseArtLarge", Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MASTER_ARTIST.NAME, false));
			this.CanArt = base.Add(new SimpleSkillPerk("CanArt", UI.ROLES_SCREEN.PERKS.CAN_ART.DESCRIPTION));
			this.CanArtUgly = base.Add(new SimpleSkillPerk("CanArtUgly", UI.ROLES_SCREEN.PERKS.CAN_ART_UGLY.DESCRIPTION));
			this.CanArtOkay = base.Add(new SimpleSkillPerk("CanArtOkay", UI.ROLES_SCREEN.PERKS.CAN_ART_OKAY.DESCRIPTION));
			this.CanArtGreat = base.Add(new SimpleSkillPerk("CanArtGreat", UI.ROLES_SCREEN.PERKS.CAN_ART_GREAT.DESCRIPTION));
			this.CanStudyArtifact = base.Add(new SimpleSkillPerk("CanStudyArtifact", UI.ROLES_SCREEN.PERKS.CAN_STUDY_ARTIFACTS.DESCRIPTION));
			this.CanClothingAlteration = base.Add(new SimpleSkillPerk("CanClothingAlteration", UI.ROLES_SCREEN.PERKS.CAN_CLOTHING_ALTERATION.DESCRIPTION));
			this.IncreaseMachinerySmall = base.Add(new SkillAttributePerk("IncreaseMachinerySmall", Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME, false));
			this.IncreaseMachineryMedium = base.Add(new SkillAttributePerk("IncreaseMachineryMedium", Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME, false));
			this.IncreaseMachineryLarge = base.Add(new SkillAttributePerk("IncreaseMachineryLarge", Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME, false));
			this.ConveyorBuild = base.Add(new SimpleSkillPerk("ConveyorBuild", UI.ROLES_SCREEN.PERKS.CONVEYOR_BUILD.DESCRIPTION));
			this.CanPowerTinker = base.Add(new SimpleSkillPerk("CanPowerTinker", UI.ROLES_SCREEN.PERKS.CAN_POWER_TINKER.DESCRIPTION));
			this.CanMakeMissiles = base.Add(new SimpleSkillPerk("CanMakeMissiles", UI.ROLES_SCREEN.PERKS.CAN_MAKE_MISSILES.DESCRIPTION));
			this.CanCraftElectronics = base.Add(new SimpleSkillPerk("CanCraftElectronics", UI.ROLES_SCREEN.PERKS.CAN_CRAFT_ELECTRONICS.DESCRIPTION, DlcManager.DLC3));
			this.CanElectricGrill = base.Add(new SimpleSkillPerk("CanElectricGrill", UI.ROLES_SCREEN.PERKS.CAN_ELECTRIC_GRILL.DESCRIPTION));
			this.CanGasRange = base.Add(new SimpleSkillPerk("CanGasRange", UI.ROLES_SCREEN.PERKS.CAN_GAS_RANGE.DESCRIPTION));
			this.CanDeepFry = base.Add(new SimpleSkillPerk("CanDeepFry", UI.ROLES_SCREEN.PERKS.CAN_DEEP_FRYER.DESCRIPTION));
			this.IncreaseCookingSmall = base.Add(new SkillAttributePerk("IncreaseCookingSmall", Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_COOK.NAME, false));
			this.IncreaseCookingMedium = base.Add(new SkillAttributePerk("IncreaseCookingMedium", Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.COOK.NAME, false));
			this.CanSpiceGrinder = base.Add(new SimpleSkillPerk("CanSpiceGrinder ", UI.ROLES_SCREEN.PERKS.CAN_SPICE_GRINDER.DESCRIPTION));
			this.IncreaseCaringSmall = base.Add(new SkillAttributePerk("IncreaseCaringSmall", Db.Get().Attributes.Caring.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_MEDIC.NAME, false));
			this.IncreaseCaringMedium = base.Add(new SkillAttributePerk("IncreaseCaringMedium", Db.Get().Attributes.Caring.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MEDIC.NAME, false));
			this.IncreaseCaringLarge = base.Add(new SkillAttributePerk("IncreaseCaringLarge", Db.Get().Attributes.Caring.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_MEDIC.NAME, false));
			this.CanCompound = base.Add(new SimpleSkillPerk("CanCompound", UI.ROLES_SCREEN.PERKS.CAN_COMPOUND.DESCRIPTION));
			this.CanDoctor = base.Add(new SimpleSkillPerk("CanDoctor", UI.ROLES_SCREEN.PERKS.CAN_DOCTOR.DESCRIPTION));
			this.CanAdvancedMedicine = base.Add(new SimpleSkillPerk("CanAdvancedMedicine", UI.ROLES_SCREEN.PERKS.CAN_ADVANCED_MEDICINE.DESCRIPTION));
			this.ExosuitExpertise = base.Add(new SimpleSkillPerk("ExosuitExpertise", UI.ROLES_SCREEN.PERKS.EXOSUIT_EXPERTISE.DESCRIPTION));
			this.ExosuitDurability = base.Add(new SimpleSkillPerk("ExosuitDurability", UI.ROLES_SCREEN.PERKS.EXOSUIT_DURABILITY.DESCRIPTION));
			this.AllowAdvancedResearch = base.Add(new SimpleSkillPerk("AllowAdvancedResearch", UI.ROLES_SCREEN.PERKS.ADVANCED_RESEARCH.DESCRIPTION));
			this.AllowInterstellarResearch = base.Add(new SimpleSkillPerk("AllowInterStellarResearch", UI.ROLES_SCREEN.PERKS.INTERSTELLAR_RESEARCH.DESCRIPTION));
			this.AllowNuclearResearch = base.Add(new SimpleSkillPerk("AllowNuclearResearch", UI.ROLES_SCREEN.PERKS.NUCLEAR_RESEARCH.DESCRIPTION));
			this.AllowOrbitalResearch = base.Add(new SimpleSkillPerk("AllowOrbitalResearch", UI.ROLES_SCREEN.PERKS.ORBITAL_RESEARCH.DESCRIPTION));
			this.AllowGeyserTuning = base.Add(new SimpleSkillPerk("AllowGeyserTuning", UI.ROLES_SCREEN.PERKS.GEYSER_TUNING.DESCRIPTION));
			this.AllowChemistry = base.Add(new SimpleSkillPerk("AllowChemistry", UI.ROLES_SCREEN.PERKS.CHEMISTRY.DESCRIPTION));
			this.CanStudyWorldObjects = base.Add(new SimpleSkillPerk("CanStudyWorldObjects", UI.ROLES_SCREEN.PERKS.CAN_STUDY_WORLD_OBJECTS.DESCRIPTION));
			this.CanUseClusterTelescope = base.Add(new SimpleSkillPerk("CanUseClusterTelescope", UI.ROLES_SCREEN.PERKS.CAN_USE_CLUSTER_TELESCOPE.DESCRIPTION));
			this.CanUseClusterTelescopeEnclosed = base.Add(new SimpleSkillPerk("CanUseClusterTelescopeEnclosed", UI.ROLES_SCREEN.PERKS.CAN_CLUSTERTELESCOPEENCLOSED.DESCRIPTION));
			this.CanDoPlumbing = base.Add(new SimpleSkillPerk("CanDoPlumbing", UI.ROLES_SCREEN.PERKS.CAN_DO_PLUMBING.DESCRIPTION));
			this.CanUseRockets = base.Add(new SimpleSkillPerk("CanUseRockets", UI.ROLES_SCREEN.PERKS.CAN_USE_ROCKETS.DESCRIPTION));
			this.FasterSpaceFlight = base.Add(new SkillAttributePerk("FasterSpaceFlight", Db.Get().Attributes.SpaceNavigation.Id, 0.1f, DUPLICANTS.ROLES.ASTRONAUT.NAME, false));
			this.CanTrainToBeAstronaut = base.Add(new SimpleSkillPerk("CanTrainToBeAstronaut", UI.ROLES_SCREEN.PERKS.CAN_DO_ASTRONAUT_TRAINING.DESCRIPTION));
			this.CanMissionControl = base.Add(new SimpleSkillPerk("CanMissionControl", UI.ROLES_SCREEN.PERKS.CAN_MISSION_CONTROL.DESCRIPTION));
			this.CanUseRocketControlStation = base.Add(new SimpleSkillPerk("CanUseRocketControlStation", UI.ROLES_SCREEN.PERKS.CAN_PILOT_ROCKET.DESCRIPTION));
			this.IncreaseRocketSpeedSmall = base.Add(new SkillAttributePerk("IncreaseRocketSpeedSmall", Db.Get().Attributes.SpaceNavigation.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.ROCKETPILOT.NAME, false));
			if (DlcManager.IsContentSubscribed("DLC3_ID"))
			{
				this.IncreaseCarryAmountBionic = base.Add(new SkillAttributePerk("IncreaseCarryAmountBionic", Db.Get().Attributes.CarryAmount.Id, 600f, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME, false));
				this.ExtraBionicBooster1 = base.Add(new SkillAttributePerk("ExtraBionicBooster1", Db.Get().Attributes.BionicBoosterSlots.Id, 1f, DUPLICANTS.ATTRIBUTES.BIONICBOOSTERSLOTS.DESC, false));
				this.ExtraBionicBooster2 = base.Add(new SkillAttributePerk("ExtraBionicBooster2", Db.Get().Attributes.BionicBoosterSlots.Id, 1f, DUPLICANTS.ATTRIBUTES.BIONICBOOSTERSLOTS.DESC, false));
				this.ExtraBionicBooster3 = base.Add(new SkillAttributePerk("ExtraBionicBooster3", Db.Get().Attributes.BionicBoosterSlots.Id, 2f, DUPLICANTS.ATTRIBUTES.BIONICBOOSTERSLOTS.DESC, false));
				this.ExtraBionicBooster4 = base.Add(new SkillAttributePerk("ExtraBionicBooster4", Db.Get().Attributes.BionicBoosterSlots.Id, 1f, DUPLICANTS.ATTRIBUTES.BIONICBOOSTERSLOTS.DESC, false));
				this.ExtraBionicBooster5 = base.Add(new SkillAttributePerk("ExtraBionicBooster5", Db.Get().Attributes.BionicBoosterSlots.Id, 1f, "", false));
				this.ExtraBionicBooster6 = base.Add(new SkillAttributePerk("ExtraBionicBooster6", Db.Get().Attributes.BionicBoosterSlots.Id, 1f, DUPLICANTS.ATTRIBUTES.BIONICBOOSTERSLOTS.DESC, false));
				this.ExtraBionicBatteries = base.Add(new SkillAttributePerk("ExtraBionicBatteries", Db.Get().Attributes.BionicBatteryCountCapacity.Id, 2f, UI.ROLES_SCREEN.PERKS.EXTRA_BIONIC_BATTERIES.DESCRIPTION, false));
				this.ReducedBionicGunkProduction = base.Add(new SimpleSkillPerk("ReducedBionicGunkProduction", UI.ROLES_SCREEN.PERKS.REDUCED_GUNK_PRODUCTION.DESCRIPTION));
				this.EfficientBionicGears = base.Add(new SimpleSkillPerk("EfficientBionicGears", UI.ROLES_SCREEN.PERKS.EFFICIENT_BIONIC_GEARS.DESCRIPTION));
				this.IncreaseAthleticsBionicsC1 = base.Add(new SkillAttributePerk("IncreaseAthleticsBionicsC1", Db.Get().Attributes.Athletics.Id, 2f, DUPLICANTS.ROLES.BIONICS_C1.NAME, false));
				this.IncreaseAthleticsBionicsC2 = base.Add(new SkillAttributePerk("IncreaseAthleticsBionicsC2", Db.Get().Attributes.Athletics.Id, 2f, DUPLICANTS.ROLES.BIONICS_C2.NAME, false));
				this.IncreaseAthleticsBionicsB2 = base.Add(new SkillAttributePerk("IncreaseAthleticsBionicsB2", Db.Get().Attributes.Athletics.Id, 2f, DUPLICANTS.ROLES.BIONICS_B2.NAME, false));
				this.IncreaseAthleticsBionicsA2 = base.Add(new SkillAttributePerk("IncreaseAthleticsBionicsA2", Db.Get().Attributes.Athletics.Id, 2f, DUPLICANTS.ROLES.BIONICS_A2.NAME, false));
				this.IncreasedCarryBionics = base.Add(new SkillAttributePerk("IncreasedCarryBionics", Db.Get().Attributes.CarryAmount.Id, 400f, global::STRINGS.ITEMS.BIONIC_BOOSTERS.BOOSTER_CARRY1.NAME, true));
			}
		}

		public SkillPerk IncreaseDigSpeedSmall;

		public SkillPerk IncreaseDigSpeedMedium;

		public SkillPerk IncreaseDigSpeedLarge;

		public SkillPerk CanDigVeryFirm;

		public SkillPerk CanDigNearlyImpenetrable;

		public SkillPerk CanDigSuperDuperHard;

		public SkillPerk CanDigRadioactiveMaterials;

		public SkillPerk CanDigUnobtanium;

		public SkillPerk IncreaseConstructionSmall;

		public SkillPerk IncreaseConstructionMedium;

		public SkillPerk IncreaseConstructionLarge;

		public SkillPerk IncreaseConstructionMechatronics;

		public SkillPerk CanDemolish;

		public SkillPerk IncreaseLearningSmall;

		public SkillPerk IncreaseLearningMedium;

		public SkillPerk IncreaseLearningLarge;

		public SkillPerk IncreaseLearningLargeSpace;

		public SkillPerk IncreaseBotanySmall;

		public SkillPerk IncreaseBotanyMedium;

		public SkillPerk IncreaseBotanyLarge;

		public SkillPerk CanFarmTinker;

		public SkillPerk CanIdentifyMutantSeeds;

		public SkillPerk CanFarmStation;

		public SkillPerk CanWrangleCreatures;

		public SkillPerk CanUseRanchStation;

		public SkillPerk CanUseMilkingStation;

		public SkillPerk IncreaseRanchingSmall;

		public SkillPerk IncreaseRanchingMedium;

		public SkillPerk IncreaseAthleticsSmall;

		public SkillPerk IncreaseAthleticsMedium;

		public SkillPerk IncreaseAthleticsLarge;

		public SkillPerk IncreaseStrengthSmall;

		public SkillPerk IncreaseStrengthMedium;

		public SkillPerk IncreaseStrengthGofer;

		public SkillPerk IncreaseStrengthCourier;

		public SkillPerk IncreaseStrengthGroundskeeper;

		public SkillPerk IncreaseStrengthPlumber;

		public SkillPerk IncreaseCarryAmountSmall;

		public SkillPerk IncreaseCarryAmountMedium;

		public SkillPerk IncreaseCarryAmountBionic;

		public SkillPerk IncreaseArtSmall;

		public SkillPerk IncreaseArtMedium;

		public SkillPerk IncreaseArtLarge;

		public SkillPerk CanArt;

		public SkillPerk CanArtUgly;

		public SkillPerk CanArtOkay;

		public SkillPerk CanArtGreat;

		public SkillPerk CanStudyArtifact;

		public SkillPerk CanClothingAlteration;

		public SkillPerk IncreaseMachinerySmall;

		public SkillPerk IncreaseMachineryMedium;

		public SkillPerk IncreaseMachineryLarge;

		public SkillPerk ConveyorBuild;

		public SkillPerk CanMakeMissiles;

		public SkillPerk CanPowerTinker;

		public SkillPerk CanCraftElectronics;

		public SkillPerk CanElectricGrill;

		public SkillPerk CanGasRange;

		public SkillPerk CanDeepFry;

		public SkillPerk IncreaseCookingSmall;

		public SkillPerk IncreaseCookingMedium;

		public SkillPerk CanSpiceGrinder;

		public SkillPerk IncreaseCaringSmall;

		public SkillPerk IncreaseCaringMedium;

		public SkillPerk IncreaseCaringLarge;

		public SkillPerk CanCompound;

		public SkillPerk CanDoctor;

		public SkillPerk CanAdvancedMedicine;

		public SkillPerk ExosuitExpertise;

		public SkillPerk ExosuitDurability;

		public SkillPerk AllowAdvancedResearch;

		public SkillPerk AllowInterstellarResearch;

		public SkillPerk AllowNuclearResearch;

		public SkillPerk AllowOrbitalResearch;

		public SkillPerk AllowGeyserTuning;

		public SkillPerk AllowChemistry;

		public SkillPerk CanStudyWorldObjects;

		public SkillPerk CanUseClusterTelescope;

		public SkillPerk CanUseClusterTelescopeEnclosed;

		public SkillPerk IncreaseRocketSpeedSmall;

		public SkillPerk CanMissionControl;

		public SkillPerk CanDoPlumbing;

		public SkillPerk CanUseRockets;

		public SkillPerk FasterSpaceFlight;

		public SkillPerk CanTrainToBeAstronaut;

		public SkillPerk CanUseRocketControlStation;

		public SkillPerk ExtraBionicBooster1;

		public SkillPerk ExtraBionicBooster2;

		public SkillPerk ExtraBionicBooster3;

		public SkillPerk ExtraBionicBooster4;

		public SkillPerk ExtraBionicBooster5;

		public SkillPerk ExtraBionicBooster6;

		public SkillPerk ReducedBionicGunkProduction;

		public SkillPerk EfficientBionicGears;

		public SkillPerk ExtraBionicBatteries;

		public SkillPerk IncreaseAthleticsBionicsC1;

		public SkillPerk IncreaseAthleticsBionicsC2;

		public SkillPerk IncreaseAthleticsBionicsB2;

		public SkillPerk IncreaseAthleticsBionicsA2;

		public SkillPerk IncreasedCarryBionics;
	}
}
