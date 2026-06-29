using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class PowerTechnician : RoleConfig
{
	public PowerTechnician()
	{
		base.id = "PowerTechnician";
		base.name = DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME;
		base.description = DUPLICANTS.ROLES.POWER_TECHNICIAN.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat("PowerTechnician");
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Machinery };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseMachineryMedium,
			RoleManager.rolePerks.CanPowerTinker
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_MachineTechnician
		};
	}

	public override void GatherNearbyFetchChores(FetchChore root_chore, Chore.Precondition.Context context, int x, int y, int radius, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts)
	{
		Tag prefabTag = root_chore.gameObject.GetComponent<KPrefabID>().PrefabTag;
		int num = ((!PowerTechnician.GroupedBuildTags.Contains(prefabTag)) ? radius : (radius * 2));
		FetchAreaChore.GatherNearbyFetchChores(root_chore, context, x, y, num, succeeded_contexts, failed_contexts);
	}

	public const string ID = "PowerTechnician";

	private static HashSet<Tag> GroupedBuildTags = new HashSet<Tag>
	{
		TagManager.Create(BuildingConfigManager.GetUnderConstructionName("Wire")),
		TagManager.Create(BuildingConfigManager.GetUnderConstructionName("WireBridge")),
		TagManager.Create(BuildingConfigManager.GetUnderConstructionName("WireRefined")),
		TagManager.Create(BuildingConfigManager.GetUnderConstructionName("WireRefinedBridge")),
		TagManager.Create(BuildingConfigManager.GetUnderConstructionName("HighWattageWire")),
		TagManager.Create(BuildingConfigManager.GetUnderConstructionName("WireBridgeHighWattage")),
		TagManager.Create(BuildingConfigManager.GetUnderConstructionName("LogicWire")),
		TagManager.Create(BuildingConfigManager.GetUnderConstructionName("LogicWireBridge"))
	};
}
