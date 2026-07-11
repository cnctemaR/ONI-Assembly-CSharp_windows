using System;
using Klei.AI;
using STRINGS;

public class MaterialsManager : RoleConfig
{
	public MaterialsManager()
	{
		base.id = MaterialsManager.ID;
		base.name = DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME;
		base.description = DUPLICANTS.ROLES.MATERIALS_MANAGER.DESCRIPTION;
		base.roleGroup = "Hauling";
		base.hat = Game.Instance.roleManager.GetHat(MaterialsManager.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Athletics };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseStrengthCourier,
			RoleManager.rolePerks.IncreaseCarryAmountMedium
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Haul,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Hauler
		};
	}

	public static string ID = "MaterialsManager";
}
