using System;
using Klei.AI;
using STRINGS;

public class MasterArtist : RoleConfig
{
	public MasterArtist()
	{
		base.id = MasterArtist.ID;
		base.name = DUPLICANTS.ROLES.MASTER_ARTIST.NAME;
		base.description = DUPLICANTS.ROLES.MASTER_ARTIST.DESCRIPTION;
		base.roleGroup = "Art";
		base.hat = Game.Instance.roleManager.GetHat(MasterArtist.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Art };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.CanArt,
			RoleManager.rolePerks.CanArtGreat,
			RoleManager.rolePerks.IncreaseArtLarge
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Art,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Artist
		};
	}

	public static string ID = "MasterArtist";
}
