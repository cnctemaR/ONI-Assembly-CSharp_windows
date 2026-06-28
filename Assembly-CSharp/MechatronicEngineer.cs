using System;
using Klei.AI;
using STRINGS;

public class MechatronicEngineer : RoleConfig
{
	public MechatronicEngineer()
	{
		base.id = "MechatronicEngineer";
		base.name = DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME;
		base.description = DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat("MechatronicEngineer");
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Fabricating);
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Conveyor);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Machinery };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseMachineryMedium,
			RoleManager.rolePerks.IncreaseConstructionMedium,
			RoleManager.rolePerks.ConveyorBuild
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_MachineTechnician,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_MaterialsManager
		};
	}

	public const string ID = "MechatronicEngineer";
}
