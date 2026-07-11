using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class RoleAssignmentRequirements
{
	public RoleAssignmentRequirements(RoleManager roleManager)
	{
		this.HasExperience_AstronautTrainee = new PreviousRoleAssignmentRequirement(AstronautTrainee.ID);
		this.HasExperience_JuniorMiner = new PreviousRoleAssignmentRequirement(JuniorMiner.ID);
		this.HasExperience_Miner = new PreviousRoleAssignmentRequirement(Miner.ID);
		this.HasExperience_JuniorResearcher = new PreviousRoleAssignmentRequirement(JuniorResearcher.ID);
		this.HasExperience_Researcher = new PreviousRoleAssignmentRequirement(Researcher.ID);
		this.HasExperience_SeniorResearcher = new PreviousRoleAssignmentRequirement(SeniorResearcher.ID);
		this.HasExperience_JuniorBuilder = new PreviousRoleAssignmentRequirement(JuniorBuilder.ID);
		this.HasExperience_Builder = new PreviousRoleAssignmentRequirement(Builder.ID);
		this.HasExperience_JuniorFarmer = new PreviousRoleAssignmentRequirement("JuniorFarmer");
		this.HasExperience_Farmer = new PreviousRoleAssignmentRequirement("Farmer");
		this.HasExperience_Rancher = new PreviousRoleAssignmentRequirement("Rancher");
		this.HasExperience_Hauler = new PreviousRoleAssignmentRequirement("Hauler");
		this.HasExperience_MaterialsManager = new PreviousRoleAssignmentRequirement(MaterialsManager.ID);
		this.HasExperience_SuitExpert = new PreviousRoleAssignmentRequirement("SuitExpert");
		this.HasExperience_JuniorCook = new PreviousRoleAssignmentRequirement(JuniorCook.ID);
		this.HasExperience_MachineTechnician = new PreviousRoleAssignmentRequirement(MachineTechnician.ID);
		this.HasExperience_PowerTechnician = new PreviousRoleAssignmentRequirement("PowerTechnician");
		this.HasExperience_JuniorArtist = new PreviousRoleAssignmentRequirement(JuniorArtist.ID);
		this.HasExperience_Handyman = new PreviousRoleAssignmentRequirement(Handyman.ID);
		this.HasExperience_MechatronicsEngineer = new PreviousRoleAssignmentRequirement("MechatronicEngineer");
		this.HasColonyLeader = new RoleAssignmentRequirement("HasColonyLeader", UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_COLONY_LEADER.DESCRIPTION, (MinionResume resume) => true);
		this.HasAttribute_Learning_Basic = new RoleAssignmentRequirement("HasAttribute_Learning_Basic", string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_ATTRIBUTE_LEARNING_BASIC.DESCRIPTION, 1), (MinionResume resume) => resume.GetAttributes().Get(Db.Get().Attributes.Learning).GetTotalValue() >= 1f);
		this.HasAttribute_Cooking_Basic = new RoleAssignmentRequirement("HasAttribute_Cooking_Basic", string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_ATTRIBUTE_COOKING_BASIC.DESCRIPTION, 1), (MinionResume resume) => resume.GetAttributes().Get(Db.Get().Attributes.Cooking).GetTotalValue() >= 1f);
		this.HasAttribute_Digging_Basic = new RoleAssignmentRequirement("HasAttribute_Digging_Basic", string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_ATTRIBUTE_DIGGING_BASIC.DESCRIPTION, 1), (MinionResume resume) => resume.GetAttributes().Get(Db.Get().Attributes.Digging).GetTotalValue() >= 1f);
		this.HasAttribute_Learning_Medium = new RoleAssignmentRequirement("HasAttribute_Learning_Medium", string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_ATTRIBUTE_LEARNING_MEDIUM.DESCRIPTION, 3), (MinionResume resume) => resume.GetAttributes().Get(Db.Get().Attributes.Learning).GetTotalValue() >= 3f);
		this.CompletedAnyOtherRole = new RoleAssignmentRequirement("CompletedAnyOtherRole", UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_COMPLETED_ANY_OTHER_ROLE.DESCRIPTION, delegate(MinionResume resume)
		{
			foreach (KeyValuePair<string, bool> keyValuePair in resume.MasteryByRoleID)
			{
				if (keyValuePair.Value)
				{
					return true;
				}
			}
			return false;
		});
		this.Can_Cook = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Cook.Id);
		this.Can_Dig = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Dig.Id);
		this.Can_Research = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Research.Id);
		this.Can_Build = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Build.Id);
		this.Can_Basekeep = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Basekeeping.Id);
		this.Can_Art = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Art.Id);
		this.Can_Haul = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Hauling.Id);
		this.Can_Operate = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Operating.Id);
		this.Can_Combat = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Combat.Id);
		this.Can_MedicalAid = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.MedicalAid.Id);
		this.Can_Farming = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Farming.Id);
		this.Can_Ranching = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Ranching.Id);
	}

	public const int SKILL_LEVEL_BASIC = 1;

	public const int SKILL_LEVEL_MEDIUM = 3;

	public PreviousRoleAssignmentRequirement HasExperience_AstronautTrainee;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorMiner;

	public PreviousRoleAssignmentRequirement HasExperience_Miner;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorResearcher;

	public PreviousRoleAssignmentRequirement HasExperience_Researcher;

	public PreviousRoleAssignmentRequirement HasExperience_SeniorResearcher;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorBuilder;

	public PreviousRoleAssignmentRequirement HasExperience_Builder;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorFarmer;

	public PreviousRoleAssignmentRequirement HasExperience_Farmer;

	public PreviousRoleAssignmentRequirement HasExperience_Rancher;

	public RoleAssignmentRequirement HasColonyLeader;

	public RoleAssignmentRequirement HasAttribute_Learning_Basic;

	public RoleAssignmentRequirement HasAttribute_Cooking_Basic;

	public RoleAssignmentRequirement HasAttribute_Digging_Basic;

	public RoleAssignmentRequirement HasAttribute_Learning_Medium;

	public RoleAssignmentRequirement HasExperience_MachineTechnician;

	public RoleAssignmentRequirement HasExperience_PowerTechnician;

	public RoleAssignmentRequirement HasExperience_MaterialsManager;

	public RoleAssignmentRequirement HasExperience_SuitExpert;

	public PreviousRoleAssignmentRequirement HasExperience_Hauler;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorCook;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorArtist;

	public PreviousRoleAssignmentRequirement HasExperience_Handyman;

	public PreviousRoleAssignmentRequirement HasExperience_MechatronicsEngineer;

	public RoleAssignmentRequirement CompletedAnyOtherRole;

	public ChoreGroupEnabledRequirement Can_Cook;

	public ChoreGroupEnabledRequirement Can_Dig;

	public ChoreGroupEnabledRequirement Can_Research;

	public ChoreGroupEnabledRequirement Can_Build;

	public ChoreGroupEnabledRequirement Can_Basekeep;

	public ChoreGroupEnabledRequirement Can_Art;

	public ChoreGroupEnabledRequirement Can_Haul;

	public ChoreGroupEnabledRequirement Can_Operate;

	public ChoreGroupEnabledRequirement Can_Combat;

	public ChoreGroupEnabledRequirement Can_MedicalAid;

	public ChoreGroupEnabledRequirement Can_Farming;

	public ChoreGroupEnabledRequirement Can_Ranching;
}
