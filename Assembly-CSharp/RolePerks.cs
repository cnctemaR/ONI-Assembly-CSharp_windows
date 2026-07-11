using System;
using STRINGS;
using TUNING;

public class RolePerks
{
	public RolePerks()
	{
		this.IncreaseDigSpeedSmall = new RoleAttributePerk("IncreaseDigSpeedSmall", UI.ROLES_SCREEN.PERKS.INCREASED_DIG_SPEED.DESCRIPTION, Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_MINER.NAME);
		this.IncreaseDigSpeedMedium = new RoleAttributePerk("IncreaseDigSpeedMedium", UI.ROLES_SCREEN.PERKS.INCREASED_DIG_SPEED.DESCRIPTION, Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MINER.NAME);
		this.IncreaseDigSpeedLarge = new RoleAttributePerk("IncreaseDigSpeedLarge", UI.ROLES_SCREEN.PERKS.INCREASED_DIG_SPEED.DESCRIPTION, Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_MINER.NAME);
		this.CanDigVeryFirm = new SimpleRolePerk("CanDigVeryFirm", UI.ROLES_SCREEN.PERKS.CAN_DIG_VERY_FIRM.DESCRIPTION);
		this.CanDigNearlyImpenetrable = new SimpleRolePerk("CanDigAbyssalite", UI.ROLES_SCREEN.PERKS.CAN_DIG_NEARLY_IMPENETRABLE.DESCRIPTION);
		this.IncreaseConstructionSmall = new RoleAttributePerk("IncreaseConstructionSmall", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME);
		this.IncreaseConstructionMedium = new RoleAttributePerk("IncreaseConstructionMedium", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.BUILDER.NAME);
		this.IncreaseConstructionLarge = new RoleAttributePerk("IncreaseConstructionLarge", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_BUILDER.NAME);
		this.IncreaseConstructionMechatronics = new RoleAttributePerk("IncreaseConstructionMechatronics", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME);
		this.IncreaseLearningSmall = new RoleAttributePerk("IncreaseLearningSmall", UI.ROLES_SCREEN.PERKS.INCREASED_LEARNING.DESCRIPTION, Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME);
		this.IncreaseLearningMedium = new RoleAttributePerk("IncreaseLearningMedium", UI.ROLES_SCREEN.PERKS.INCREASED_LEARNING.DESCRIPTION, Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.RESEARCHER.NAME);
		this.IncreaseLearningLarge = new RoleAttributePerk("IncreaseLearningLarge", UI.ROLES_SCREEN.PERKS.INCREASED_LEARNING.DESCRIPTION, Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME);
		this.IncreaseBotanySmall = new RoleAttributePerk("IncreaseBotanySmall", UI.ROLES_SCREEN.PERKS.INCREASE_BOTANIST.DESCRIPTION, Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_FARMER.NAME);
		this.IncreaseBotanyMedium = new RoleAttributePerk("IncreaseBotanyMedium", UI.ROLES_SCREEN.PERKS.INCREASE_BOTANIST.DESCRIPTION, Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.FARMER.NAME);
		this.IncreaseBotanyLarge = new RoleAttributePerk("IncreaseBotanyLarge", UI.ROLES_SCREEN.PERKS.INCREASE_BOTANIST.DESCRIPTION, Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_FARMER.NAME);
		this.CanFarmTinker = new SimpleRolePerk("CanFarmTinker", UI.ROLES_SCREEN.PERKS.CAN_FARM_TINKER.DESCRIPTION);
		this.IncreaseRanchingSmall = new RoleAttributePerk("IncreaseRanchingSmall", UI.ROLES_SCREEN.PERKS.INCREASE_RANCHING.DESCRIPTION, Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.RANCHER.NAME);
		this.IncreaseRanchingMedium = new RoleAttributePerk("IncreaseRanchingMedium", UI.ROLES_SCREEN.PERKS.INCREASE_RANCHING.DESCRIPTION, Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.SENIOR_RANCHER.NAME);
		this.CanWrangleCreatures = new SimpleRolePerk("CanWrangleCreatures", UI.ROLES_SCREEN.PERKS.CAN_WRANGLE_CREATURES.DESCRIPTION);
		this.CanUseRanchStation = new SimpleRolePerk("CanUseRanchStation", UI.ROLES_SCREEN.PERKS.CAN_USE_RANCH_STATION.DESCRIPTION);
		this.IncreaseAthleticsSmall = new RoleAttributePerk("IncreaseAthleticsSmall", UI.ROLES_SCREEN.PERKS.INCREASED_ATHLETICS.DESCRIPTION, Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HAULER.NAME);
		this.IncreaseAthleticsMedium = new RoleAttributePerk("IncreaseAthletics", UI.ROLES_SCREEN.PERKS.INCREASED_ATHLETICS.DESCRIPTION, Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.SUIT_EXPERT.NAME);
		this.IncreaseStrengthGofer = new RoleAttributePerk("IncreaseStrengthGofer", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HAULER.NAME);
		this.IncreaseStrengthCourier = new RoleAttributePerk("IncreaseStrengthCourier", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME);
		this.IncreaseStrengthGroundskeeper = new RoleAttributePerk("IncreaseStrengthGroundskeeper", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HANDYMAN.NAME);
		this.IncreaseStrengthPlumber = new RoleAttributePerk("IncreaseStrengthPlumber", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.PLUMBER.NAME);
		this.IncreaseCarryAmountSmall = new RoleAttributePerk("IncreaseCarryAmountSmall", UI.ROLES_SCREEN.PERKS.INCREASED_CARRY_AMOUNT.DESCRIPTION, Db.Get().Attributes.CarryAmount.Id, 400f, DUPLICANTS.ROLES.HAULER.NAME);
		this.IncreaseCarryAmountMedium = new RoleAttributePerk("IncreaseCarryAmountMedium", UI.ROLES_SCREEN.PERKS.INCREASED_CARRY_AMOUNT.DESCRIPTION, Db.Get().Attributes.CarryAmount.Id, 800f, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME);
		this.IncreaseArtSmall = new RoleAttributePerk("IncreaseArtSmall", UI.ROLES_SCREEN.PERKS.INCREASED_ART.DESCRIPTION, Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME);
		this.IncreaseArtMedium = new RoleAttributePerk("IncreaseArt", UI.ROLES_SCREEN.PERKS.INCREASED_ART.DESCRIPTION, Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.ARTIST.NAME);
		this.IncreaseArtLarge = new RoleAttributePerk("IncreaseArtLarge", UI.ROLES_SCREEN.PERKS.INCREASED_ART.DESCRIPTION, Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MASTER_ARTIST.NAME);
		this.CanArt = new SimpleRolePerk("CanArt", UI.ROLES_SCREEN.PERKS.CAN_ART.DESCRIPTION);
		this.CanArtUgly = new SimpleRolePerk("CanArtUgly", UI.ROLES_SCREEN.PERKS.CAN_ART_UGLY.DESCRIPTION);
		this.CanArtOkay = new SimpleRolePerk("CanArtOkay", UI.ROLES_SCREEN.PERKS.CAN_ART_OKAY.DESCRIPTION);
		this.CanArtGreat = new SimpleRolePerk("CanArtGreat", UI.ROLES_SCREEN.PERKS.CAN_ART_GREAT.DESCRIPTION);
		this.IncreaseMachinerySmall = new RoleAttributePerk("IncreaseMachinerySmall", UI.ROLES_SCREEN.PERKS.INCREASED_MACHINERY.DESCRIPTION, Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME);
		this.IncreaseMachineryMedium = new RoleAttributePerk("IncreaseMachineryMedium", UI.ROLES_SCREEN.PERKS.INCREASED_MACHINERY.DESCRIPTION, Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME);
		this.IncreaseMachineryLarge = new RoleAttributePerk("IncreaseMachineryLarge", UI.ROLES_SCREEN.PERKS.INCREASED_MACHINERY.DESCRIPTION, Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME);
		this.ConveyorBuild = new SimpleRolePerk("ConveyorBuild", UI.ROLES_SCREEN.PERKS.CONVEYOR_BUILD.DESCRIPTION);
		this.CanPowerTinker = new SimpleRolePerk("CanPowerTinker", UI.ROLES_SCREEN.PERKS.CAN_POWER_TINKER.DESCRIPTION);
		this.CanElectricGrill = new SimpleRolePerk("CanElectricGrill", UI.ROLES_SCREEN.PERKS.CAN_ELECTRIC_GRILL.DESCRIPTION);
		this.IncreaseCookingSmall = new RoleAttributePerk("IncreaseCookingSmall", UI.ROLES_SCREEN.PERKS.INCREASED_COOKING.DESCRIPTION, Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_COOK.NAME);
		this.IncreaseCookingMedium = new RoleAttributePerk("IncreaseCookingMedium", UI.ROLES_SCREEN.PERKS.INCREASED_COOKING.DESCRIPTION, Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.COOK.NAME);
		this.IncreaseCaringMedium = new RoleAttributePerk("IncreaseCaringMedium", UI.ROLES_SCREEN.PERKS.INCREASED_CARING.DESCRIPTION, Db.Get().Attributes.Caring.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MEDIC.NAME);
		this.ExosuitExpertise = new SimpleRolePerk("ExosuitExpertise", UI.ROLES_SCREEN.PERKS.EXOSUIT_EXPERTISE.DESCRIPTION);
		this.AllowAdvancedResearch = new SimpleRolePerk("AllowAdvancedResearch", UI.ROLES_SCREEN.PERKS.ADVANCED_RESEARCH.DESCRIPTION);
		this.AllowInterstellarResearch = new SimpleRolePerk("AllowInterStellarResearch", UI.ROLES_SCREEN.PERKS.INTERSTELLAR_RESEARCH.DESCRIPTION);
		this.CanStudyWorldObjects = new SimpleRolePerk("CanStudyWorldObjects", UI.ROLES_SCREEN.PERKS.CAN_STUDY_WORLD_OBJECTS.DESCRIPTION);
		this.CanDoPlumbing = new SimpleRolePerk("CanDoPlumbing", UI.ROLES_SCREEN.PERKS.CAN_DO_PLUMBING.DESCRIPTION);
		this.CanUseRockets = new SimpleRolePerk("CanUseRockets", UI.ROLES_SCREEN.PERKS.CAN_USE_ROCKETS.DESCRIPTION);
		this.CanTrainToBeAstronaut = new SimpleRolePerk("CanTrainToBeAstronaut", UI.ROLES_SCREEN.PERKS.CAN_DO_ASTRONAUT_TRAINING.DESCRIPTION);
	}

	public RoleAttributePerk IncreaseDigSpeedSmall;

	public RoleAttributePerk IncreaseDigSpeedMedium;

	public RoleAttributePerk IncreaseDigSpeedLarge;

	public SimpleRolePerk CanDigVeryFirm;

	public SimpleRolePerk CanDigNearlyImpenetrable;

	public RoleAttributePerk IncreaseConstructionSmall;

	public RoleAttributePerk IncreaseConstructionMedium;

	public RoleAttributePerk IncreaseConstructionLarge;

	public RoleAttributePerk IncreaseConstructionMechatronics;

	public RoleAttributePerk IncreaseLearningSmall;

	public RoleAttributePerk IncreaseLearningMedium;

	public RoleAttributePerk IncreaseLearningLarge;

	public RoleAttributePerk IncreaseBotanySmall;

	public RoleAttributePerk IncreaseBotanyMedium;

	public RoleAttributePerk IncreaseBotanyLarge;

	public SimpleRolePerk CanFarmTinker;

	public SimpleRolePerk CanWrangleCreatures;

	public SimpleRolePerk CanUseRanchStation;

	public RoleAttributePerk IncreaseRanchingSmall;

	public RoleAttributePerk IncreaseRanchingMedium;

	public RoleAttributePerk IncreaseAthleticsSmall;

	public RoleAttributePerk IncreaseAthleticsMedium;

	public RoleAttributePerk IncreaseStrengthSmall;

	public RoleAttributePerk IncreaseStrengthMedium;

	public RoleAttributePerk IncreaseStrengthGofer;

	public RoleAttributePerk IncreaseStrengthCourier;

	public RoleAttributePerk IncreaseStrengthGroundskeeper;

	public RoleAttributePerk IncreaseStrengthPlumber;

	public RoleAttributePerk IncreaseCarryAmountSmall;

	public RoleAttributePerk IncreaseCarryAmountMedium;

	public RoleAttributePerk IncreaseArtSmall;

	public RoleAttributePerk IncreaseArtMedium;

	public RoleAttributePerk IncreaseArtLarge;

	public SimpleRolePerk CanArt;

	public SimpleRolePerk CanArtUgly;

	public SimpleRolePerk CanArtOkay;

	public SimpleRolePerk CanArtGreat;

	public RoleAttributePerk IncreaseMachinerySmall;

	public RoleAttributePerk IncreaseMachineryMedium;

	public RoleAttributePerk IncreaseMachineryLarge;

	public SimpleRolePerk ConveyorBuild;

	public SimpleRolePerk CanPowerTinker;

	public SimpleRolePerk CanElectricGrill;

	public RoleAttributePerk IncreaseCookingSmall;

	public RoleAttributePerk IncreaseCookingMedium;

	public RoleAttributePerk IncreaseCaringMedium;

	public SimpleRolePerk ExosuitExpertise;

	public SimpleRolePerk AllowAdvancedResearch;

	public SimpleRolePerk AllowInterstellarResearch;

	public SimpleRolePerk CanStudyWorldObjects;

	public SimpleRolePerk CanDoPlumbing;

	public SimpleRolePerk CanUseRockets;

	public SimpleRolePerk CanTrainToBeAstronaut;
}
