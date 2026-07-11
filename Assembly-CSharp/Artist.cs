using System;
using Klei.AI;
using STRINGS;

public class Artist : RoleConfig
{
	public Artist()
	{
		base.id = Artist.ID;
		base.name = DUPLICANTS.ROLES.ARTIST.NAME;
		base.description = DUPLICANTS.ROLES.ARTIST.DESCRIPTION;
		base.roleGroup = "Art";
		base.hat = Game.Instance.roleManager.GetHat(Artist.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Art };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.CanArt,
			RoleManager.rolePerks.CanArtOkay,
			RoleManager.rolePerks.IncreaseArtMedium
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Art,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorArtist
		};
	}

	public static string ID = "Artist";
}
