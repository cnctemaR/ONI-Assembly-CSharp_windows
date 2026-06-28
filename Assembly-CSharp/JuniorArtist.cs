using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class JuniorArtist : RoleConfig
{
	public JuniorArtist()
	{
		base.id = JuniorArtist.ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_ARTIST.DESCRIPTION;
		base.roleGroup = "Art";
		base.hat = Game.Instance.roleManager.GetHat(JuniorArtist.ID);
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Art);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Art };
		this.favoredChoreTypes = new List<ChoreType> { Db.Get().ChoreTypes.Art };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.CanArt,
			RoleManager.rolePerks.IncreaseArtSmall
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.Can_Art };
	}

	public static string ID = "JuniorArtist";
}
