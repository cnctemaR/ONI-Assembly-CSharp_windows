using System;
using System.Collections.Generic;
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
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Art);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Art };
		this.favoredChoreTypes = new List<ChoreType> { Db.Get().ChoreTypes.Art };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.CanArt,
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
