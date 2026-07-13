using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei.AI;
using Klei.CustomSettings;
using ProcGen;

public class GameplayEventPreconditions
{
	public static GameplayEventPreconditions Instance
	{
		get
		{
			if (GameplayEventPreconditions._instance == null)
			{
				GameplayEventPreconditions._instance = new GameplayEventPreconditions();
			}
			return GameplayEventPreconditions._instance;
		}
	}

	public GameplayEventPrecondition LiveMinions(int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => Components.LiveMinionIdentities.Count >= count,
			description = string.Format("At least {0} dupes alive", count)
		};
	}

	public GameplayEventPrecondition BuildingExists(string buildingId, int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => BuildingInventory.Instance.BuildingCount(new Tag(buildingId)) >= count,
			description = string.Format("{0} {1} has been built", count, buildingId)
		};
	}

	public GameplayEventPrecondition ResearchCompleted(string techName)
	{
		return new GameplayEventPrecondition
		{
			condition = () => Research.Instance.Get(Db.Get().Techs.Get(techName)).IsComplete(),
			description = "Has researched " + techName + "."
		};
	}

	public GameplayEventPrecondition AchievementUnlocked(ColonyAchievement achievement)
	{
		return new GameplayEventPrecondition
		{
			condition = () => SaveGame.Instance.ColonyAchievementTracker.IsAchievementUnlocked(achievement),
			description = "Unlocked the " + achievement.Id + " achievement"
		};
	}

	public GameplayEventPrecondition RoomBuilt(RoomType roomType)
	{
		Predicate<global::Room> <>9__1;
		return new GameplayEventPrecondition
		{
			condition = delegate
			{
				List<global::Room> rooms = Game.Instance.roomProber.rooms;
				Predicate<global::Room> predicate;
				if ((predicate = <>9__1) == null)
				{
					predicate = (<>9__1 = (global::Room match) => match.roomType == roomType);
				}
				return rooms.Exists(predicate);
			},
			description = "Built a " + roomType.Id + " room"
		};
	}

	public GameplayEventPrecondition CycleRestriction(float min = 0f, float max = float.PositiveInfinity)
	{
		return new GameplayEventPrecondition
		{
			condition = () => GameUtil.GetCurrentTimeInCycles() >= min && GameUtil.GetCurrentTimeInCycles() <= max,
			description = string.Format("After cycle {0} and before cycle {1}", min, max)
		};
	}

	public GameplayEventPrecondition MinionsWithEffect(string effectId, int count = 1)
	{
		Func<MinionIdentity, bool> <>9__1;
		return new GameplayEventPrecondition
		{
			condition = delegate
			{
				IEnumerable<MinionIdentity> items = Components.LiveMinionIdentities.Items;
				Func<MinionIdentity, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (MinionIdentity minion) => minion.GetComponent<Effects>().Get(effectId) != null);
				}
				return items.Count<MinionIdentity>(func) >= count;
			},
			description = string.Format("At least {0} dupes have the {1} effect applied", count, effectId)
		};
	}

	public GameplayEventPrecondition MinionsWithStatusItem(StatusItem statusItem, int count = 1)
	{
		Func<MinionIdentity, bool> <>9__1;
		return new GameplayEventPrecondition
		{
			condition = delegate
			{
				IEnumerable<MinionIdentity> items = Components.LiveMinionIdentities.Items;
				Func<MinionIdentity, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (MinionIdentity minion) => minion.GetComponent<KSelectable>().HasStatusItem(statusItem));
				}
				return items.Count<MinionIdentity>(func) >= count;
			},
			description = string.Format("At least {0} dupes have the {1} status item", count, statusItem)
		};
	}

	public GameplayEventPrecondition MinionsWithChoreGroupPriorityOrGreater(ChoreGroup choreGroup, int count, int priority)
	{
		Func<MinionIdentity, bool> <>9__1;
		return new GameplayEventPrecondition
		{
			condition = delegate
			{
				IEnumerable<MinionIdentity> items = Components.LiveMinionIdentities.Items;
				Func<MinionIdentity, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = delegate(MinionIdentity minion)
					{
						ChoreConsumer component = minion.GetComponent<ChoreConsumer>();
						return !component.IsChoreGroupDisabled(choreGroup) && component.GetPersonalPriority(choreGroup) >= priority;
					});
				}
				return items.Count<MinionIdentity>(func) >= count;
			},
			description = string.Format("At least {0} dupes have their {1} set to {2} or higher.", count, choreGroup.Name, priority)
		};
	}

	public GameplayEventPrecondition PastEventCount(string evtId, int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => GameplayEventManager.Instance.NumberOfPastEvents(evtId) >= count,
			description = string.Format("The {0} event has triggered {1} times.", evtId, count)
		};
	}

	public GameplayEventPrecondition DifficultySetting(SettingConfig config, string levelId)
	{
		return new GameplayEventPrecondition
		{
			condition = () => CustomGameSettings.Instance.GetCurrentQualitySetting(config).id == levelId,
			description = string.Concat(new string[] { "The config ", config.id, " is level ", levelId, "." })
		};
	}

	public GameplayEventPrecondition ClusterHasTag(string tag)
	{
		return new GameplayEventPrecondition
		{
			condition = delegate
			{
				ClusterLayout currentClusterLayout = CustomGameSettings.Instance.GetCurrentClusterLayout();
				return currentClusterLayout != null && currentClusterLayout.clusterTags.Contains(tag);
			},
			description = "The cluster is tagged with " + tag + "."
		};
	}

	public GameplayEventPrecondition PastEventCountAndNotActive(GameplayEvent evt, int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => GameplayEventManager.Instance.NumberOfPastEvents(evt.IdHash) >= count && !GameplayEventManager.Instance.IsGameplayEventActive(evt),
			description = string.Format("The {0} event has triggered {1} times and is not active.", evt.Id, count)
		};
	}

	public GameplayEventPrecondition Not(GameplayEventPrecondition precondition)
	{
		return new GameplayEventPrecondition
		{
			condition = () => !precondition.condition(),
			description = "Not[" + precondition.description + "]"
		};
	}

	public GameplayEventPrecondition Or(GameplayEventPrecondition precondition1, GameplayEventPrecondition precondition2)
	{
		return new GameplayEventPrecondition
		{
			condition = () => precondition1.condition() || precondition2.condition(),
			description = string.Concat(new string[] { "[", precondition1.description, "]-OR-[", precondition2.description, "]" })
		};
	}

	private static GameplayEventPreconditions _instance;
}
