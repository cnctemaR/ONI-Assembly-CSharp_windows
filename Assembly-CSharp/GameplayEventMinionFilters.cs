using System;
using Database;

public class GameplayEventMinionFilters
{
	public static GameplayEventMinionFilters Instance
	{
		get
		{
			if (GameplayEventMinionFilters._instance == null)
			{
				GameplayEventMinionFilters._instance = new GameplayEventMinionFilters();
			}
			return GameplayEventMinionFilters._instance;
		}
	}

	public GameplayEventMinionFilter HasMasteredSkill(Skill skill)
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => minion.GetComponent<MinionResume>().HasMasteredSkill(skill.Id),
			id = "HasMasteredSkill"
		};
	}

	public GameplayEventMinionFilter HasSkillAptitude(Skill skill)
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => minion.GetComponent<MinionResume>().HasSkillAptitude(skill),
			id = "HasSkillAptitude"
		};
	}

	public GameplayEventMinionFilter HasChoreGroupPriorityOrHigher(ChoreGroup choreGroup, int priority)
	{
		return new GameplayEventMinionFilter
		{
			filter = delegate(MinionIdentity minion)
			{
				ChoreConsumer component = minion.GetComponent<ChoreConsumer>();
				return !component.IsChoreGroupDisabled(choreGroup) && component.GetPersonalPriority(choreGroup) >= priority;
			},
			id = "HasChoreGroupPriorityOrHigher"
		};
	}

	public GameplayEventMinionFilter AgeRange(float min = 0f, float max = float.PositiveInfinity)
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => minion.arrivalTime >= min && minion.arrivalTime <= max,
			id = "AgeRange"
		};
	}

	public GameplayEventMinionFilter PriorityIn()
	{
		GameplayEventMinionFilter gameplayEventMinionFilter = new GameplayEventMinionFilter();
		gameplayEventMinionFilter.filter = (MinionIdentity minion) => true;
		gameplayEventMinionFilter.id = "PriorityIn";
		return gameplayEventMinionFilter;
	}

	public GameplayEventMinionFilter Not(GameplayEventMinionFilter filter)
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => !filter.filter(minion),
			id = "Not[" + filter.id + "]"
		};
	}

	public GameplayEventMinionFilter Or(GameplayEventMinionFilter precondition1, GameplayEventMinionFilter precondition2)
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => precondition1.filter(minion) || precondition2.filter(minion),
			id = string.Concat(new string[] { "[", precondition1.id, "]-OR-[", precondition2.id, "]" })
		};
	}

	private static GameplayEventMinionFilters _instance;
}
