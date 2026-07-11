using System;
using Klei.AI;
using STRINGS;
using TUNING;

public class SuitExpert : RoleConfig
{
	public SuitExpert()
	{
		base.id = "SuitExpert";
		base.name = DUPLICANTS.ROLES.SUIT_EXPERT.NAME;
		base.description = DUPLICANTS.ROLES.SUIT_EXPERT.DESCRIPTION;
		base.roleGroup = "Hauling";
		base.hat = Game.Instance.roleManager.GetHat("SuitExpert");
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Athletics };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.ExosuitExpertise,
			RoleManager.rolePerks.IncreaseAthleticsMedium
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Haul,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_MaterialsManager
		};
	}

	public const string ID = "SuitExpert";

	public static readonly AttributeModifier AthleticsModifier = new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)(-(float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS), DUPLICANTS.ROLES.SUIT_EXPERT.NAME, false, false, true);
}
