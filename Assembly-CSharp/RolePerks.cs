using System;
using STRINGS;
using TUNING;

public class RolePerks
{
	public RolePerks()
	{
		this.IncreaseDigSpeedSmall = new RoleAttributePerk("IncreaseDigSpeedSmall", UI.ROLES_SCREEN.PERKS.INCREASED_DIG_SPEED.DESCRIPTION, Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_SMALL, DUPLICANTS.ROLES.JUNIOR_MINER.NAME);
		this.IncreaseDigSpeedMedium = new RoleAttributePerk("IncreaseDigSpeedMedium", UI.ROLES_SCREEN.PERKS.INCREASED_DIG_SPEED.DESCRIPTION, Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.MINER.NAME);
		this.IncreaseDigSpeedLarge = new RoleAttributePerk("IncreaseDigSpeedLarge", UI.ROLES_SCREEN.PERKS.INCREASED_DIG_SPEED.DESCRIPTION, Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_LARGE, DUPLICANTS.ROLES.SENIOR_MINER.NAME);
		this.CanDigVeryFirm = new SimpleRolePerk("CanDigVeryFirm", UI.ROLES_SCREEN.PERKS.CAN_DIG_VERY_FIRM.DESCRIPTION);
		this.CanDigNearlyImpenetrable = new SimpleRolePerk("CanDigAbyssalite", UI.ROLES_SCREEN.PERKS.CAN_DIG_NEARLY_IMPENETRABLE.DESCRIPTION);
		this.IncreaseConstructionSmall = new RoleAttributePerk("IncreaseConstructionSmall", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_SMALL, DUPLICANTS.ROLES.BUILDER.NAME);
		this.IncreaseConstructionMedium = new RoleAttributePerk("IncreaseConstructionMedium", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.BUILDER.NAME);
		this.IncreaseConstructionLarge = new RoleAttributePerk("IncreaseConstructionLarge", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_LARGE, DUPLICANTS.ROLES.BUILDER.NAME);
		this.IncreaseLearningSmall = new RoleAttributePerk("IncreaseLearningSmall", UI.ROLES_SCREEN.PERKS.INCREASED_LEARNING.DESCRIPTION, Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_SMALL, DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME);
		this.IncreaseLearningMedium = new RoleAttributePerk("IncreaseLearningMedium", UI.ROLES_SCREEN.PERKS.INCREASED_LEARNING.DESCRIPTION, Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.RESEARCHER.NAME);
		this.IncreaseLearningLarge = new RoleAttributePerk("IncreaseLearningLarge", UI.ROLES_SCREEN.PERKS.INCREASED_LEARNING.DESCRIPTION, Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_LARGE, DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME);
		this.IncreaseBotanySmall = new RoleAttributePerk("IncreaseBotanySmall", UI.ROLES_SCREEN.PERKS.INCREASE_BOTANIST.DESCRIPTION, Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.JUNIOR_FARMER.NAME);
		this.IncreaseBotanyMedium = new RoleAttributePerk("IncreaseBotanyMedium", UI.ROLES_SCREEN.PERKS.INCREASE_BOTANIST.DESCRIPTION, Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.FARMER.NAME);
		this.IncreaseBotanyLarge = new RoleAttributePerk("IncreaseBotanyLarge", UI.ROLES_SCREEN.PERKS.INCREASE_BOTANIST.DESCRIPTION, Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.SENIOR_FARMER.NAME);
		this.CanFarmTinker = new SimpleRolePerk("CanFarmTinker", UI.ROLES_SCREEN.PERKS.CAN_FARM_TINKER.DESCRIPTION);
		this.IncreaseRanchingSmall = new RoleAttributePerk("IncreaseRanchingSmall", UI.ROLES_SCREEN.PERKS.INCREASE_RANCHING.DESCRIPTION, Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.RANCHER.NAME);
		this.IncreaseRanchingMedium = new RoleAttributePerk("IncreaseRanchingMedium", UI.ROLES_SCREEN.PERKS.INCREASE_RANCHING.DESCRIPTION, Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.SENIOR_RANCHER.NAME);
		this.CanWrangleCreatures = new SimpleRolePerk("CanWrangleCreatures", UI.ROLES_SCREEN.PERKS.CAN_WRANGLE_CREATURES.DESCRIPTION);
		this.CanUseRanchStation = new SimpleRolePerk("CanUseRanchStation", UI.ROLES_SCREEN.PERKS.CAN_USE_RANCH_STATION.DESCRIPTION);
		this.IncreaseAthleticsSmall = new RoleAttributePerk("IncreaseAthleticsSmall", UI.ROLES_SCREEN.PERKS.INCREASED_ATHLETICS.DESCRIPTION, Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_SMALL, DUPLICANTS.ROLES.HAULER.NAME);
		this.IncreaseAthleticsMedium = new RoleAttributePerk("IncreaseAthletics", UI.ROLES_SCREEN.PERKS.INCREASED_ATHLETICS.DESCRIPTION, Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.HAULER.NAME);
		this.IncreaseStrengthSmall = new RoleAttributePerk("IncreaseStrengthSmall", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_SMALL, DUPLICANTS.ROLES.HAULER.NAME);
		this.IncreaseStrengthMedium = new RoleAttributePerk("IncreaseStrength", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME);
		this.IncreaseCarryAmountSmall = new RoleAttributePerk("IncreaseCarryAmountSmall", UI.ROLES_SCREEN.PERKS.INCREASED_CARRY_AMOUNT.DESCRIPTION, Db.Get().Attributes.CarryAmount.Id, 400f, DUPLICANTS.ROLES.HAULER.NAME);
		this.IncreaseCarryAmountMedium = new RoleAttributePerk("IncreaseCarryAmountMedium", UI.ROLES_SCREEN.PERKS.INCREASED_CARRY_AMOUNT.DESCRIPTION, Db.Get().Attributes.CarryAmount.Id, 800f, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME);
		this.IncreaseArtSmall = new RoleAttributePerk("IncreaseArtSmall", UI.ROLES_SCREEN.PERKS.INCREASED_ART.DESCRIPTION, Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_SMALL, DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME);
		this.IncreaseArtMedium = new RoleAttributePerk("IncreaseArt", UI.ROLES_SCREEN.PERKS.INCREASED_ART.DESCRIPTION, Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.ARTIST.NAME);
		this.CanArt = new SimpleRolePerk("CanArt", UI.ROLES_SCREEN.PERKS.CAN_PAINT.DESCRIPTION + "\n    • " + UI.ROLES_SCREEN.PERKS.CAN_SCULPT.DESCRIPTION);
		this.IncreaseMachineryMedium = new RoleAttributePerk("IncreaseMachineryMedium", UI.ROLES_SCREEN.PERKS.INCREASED_MACHINERY.DESCRIPTION, Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME);
		this.ConveyorBuild = new SimpleRolePerk("ConveyorBuild", UI.ROLES_SCREEN.PERKS.CONVEYOR_BUILD.DESCRIPTION);
		this.CanPowerTinker = new SimpleRolePerk("CanPowerTinker", UI.ROLES_SCREEN.PERKS.CAN_POWER_TINKER.DESCRIPTION);
		this.CanElectricGrill = new SimpleRolePerk("CanElectricGrill", UI.ROLES_SCREEN.PERKS.CAN_ELECTRIC_GRILL.DESCRIPTION);
		this.IncreaseCookingSmall = new RoleAttributePerk("IncreaseCookingSmall", UI.ROLES_SCREEN.PERKS.INCREASED_COOKING.DESCRIPTION, Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_SMALL, DUPLICANTS.ROLES.JUNIOR_COOK.NAME);
		this.IncreaseCookingMedium = new RoleAttributePerk("IncreaseCookingMedium", UI.ROLES_SCREEN.PERKS.INCREASED_COOKING.DESCRIPTION, Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.COOK.NAME);
		this.IncreaseCaringMedium = new RoleAttributePerk("IncreaseCaringMedium", UI.ROLES_SCREEN.PERKS.INCREASED_CARING.DESCRIPTION, Db.Get().Attributes.Caring.Id, (float)ROLES.ATTRIBUTE_BONUS_MEDIUM, DUPLICANTS.ROLES.MEDIC.NAME);
		this.ExosuitExpertise = new SimpleRolePerk("ExosuitExpertise", UI.ROLES_SCREEN.PERKS.EXOSUIT_EXPERTISE.DESCRIPTION);
		this.AllowAdvancedResearch = new SimpleRolePerk("AllowAdvancedResearch", UI.ROLES_SCREEN.PERKS.ADVANCED_RESEARCH.DESCRIPTION);
		this.CanStudyWorldObjects = new SimpleRolePerk("CanStudyWorldObjects", UI.ROLES_SCREEN.PERKS.CAN_STUDY_WORLD_OBJECTS.DESCRIPTION);
	}

	public RoleAttributePerk IncreaseDigSpeedSmall;

	public RoleAttributePerk IncreaseDigSpeedMedium;

	public RoleAttributePerk IncreaseDigSpeedLarge;

	public SimpleRolePerk CanDigVeryFirm;

	public SimpleRolePerk CanDigNearlyImpenetrable;

	public RoleAttributePerk IncreaseConstructionSmall;

	public RoleAttributePerk IncreaseConstructionMedium;

	public RoleAttributePerk IncreaseConstructionLarge;

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

	public RoleAttributePerk IncreaseCarryAmountSmall;

	public RoleAttributePerk IncreaseCarryAmountMedium;

	public RoleAttributePerk IncreaseArtSmall;

	public RoleAttributePerk IncreaseArtMedium;

	public SimpleRolePerk CanArt;

	public RoleAttributePerk IncreaseMachineryMedium;

	public SimpleRolePerk ConveyorBuild;

	public SimpleRolePerk CanPowerTinker;

	public SimpleRolePerk CanElectricGrill;

	public RoleAttributePerk IncreaseCookingSmall;

	public RoleAttributePerk IncreaseCookingMedium;

	public RoleAttributePerk IncreaseCaringMedium;

	public SimpleRolePerk ExosuitExpertise;

	public SimpleRolePerk AllowAdvancedResearch;

	public SimpleRolePerk CanStudyWorldObjects;
}
