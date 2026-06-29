using System;
using System.Collections.Generic;
using Klei.AI;
using TUNING;

public class RoleConfig : IListableOption
{
	public string id { get; protected set; }

	public string name { get; protected set; }

	public string description { get; protected set; }

	public HashedString roleGroup { get; protected set; }

	public string hat { get; protected set; }

	public int tier { get; protected set; }

	public RolePerk[] perks { get; protected set; }

	public RoleAssignmentRequirement[] requirements { get; protected set; }

	public Expectation[] expectations { get; protected set; }

	public string GetProperName()
	{
		return this.name;
	}

	public void SetTier(int tier)
	{
		this.tier = tier;
		this.experienceRequired = ROLES.BASIC_ROLE_MASTERY_EXPERIENCE_REQUIRED * (float)tier;
	}

	public virtual void InitRequirements()
	{
	}

	public bool HasPerk(RolePerk perk)
	{
		for (int i = 0; i < this.perks.Length; i++)
		{
			if (this.perks[i] == perk)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPerk(HashedString perk_id)
	{
		for (int i = 0; i < this.perks.Length; i++)
		{
			if (this.perks[i].id == perk_id)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void GatherNearbyFetchChores(FetchChore root_chore, Chore.Precondition.Context context, int x, int y, int radius, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts)
	{
		FetchAreaChore.GatherNearbyFetchChores(root_chore, context, x, y, radius, succeeded_contexts, failed_contexts);
	}

	public float experienceRequired = 100f;

	public Klei.AI.Attribute[] relevantAttributes = new Klei.AI.Attribute[0];
}
