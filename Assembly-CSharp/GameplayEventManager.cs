using System;
using System.Collections.Generic;
using KSerialization;

public class GameplayEventManager : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		GameplayEventManager.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameplayEventManager.Instance = this;
		this.notifier = base.GetComponent<Notifier>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.ScheduleNextFrame("GameplayEventManager", delegate(object obj)
		{
			this.RestoreEvents();
		}, null, null);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameplayEventManager.Instance = null;
	}

	private void RestoreEvents()
	{
		this.activeEvents.RemoveAll((GameplayEventInstance x) => Db.Get().GameplayEvents.TryGet(x.eventID) == null);
		foreach (GameplayEventInstance gameplayEventInstance in this.activeEvents)
		{
			this.StartEventInstance(gameplayEventInstance);
		}
	}

	public bool IsGameplayEventActive(GameplayEvent eventType)
	{
		return this.activeEvents.Find((GameplayEventInstance e) => e.eventID == eventType.IdHash) != null;
	}

	public bool IsGameplayEventRunningWithTag(Tag tag)
	{
		using (List<GameplayEventInstance>.Enumerator enumerator = this.activeEvents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.tags.Contains(tag))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void GetActiveEventsOfType<T>(int worldID, ref List<GameplayEventInstance> results) where T : GameplayEvent
	{
		foreach (GameplayEventInstance gameplayEventInstance in this.activeEvents)
		{
			if (gameplayEventInstance.worldId == worldID && gameplayEventInstance.gameplayEvent is T)
			{
				results.Add(gameplayEventInstance);
			}
		}
	}

	private GameplayEventInstance CreateGameplayEvent(GameplayEvent gameplayEvent, int worldId)
	{
		return gameplayEvent.CreateInstance(worldId);
	}

	public GameplayEventInstance GetGameplayEventInstance(HashedString eventID)
	{
		return this.activeEvents.Find((GameplayEventInstance e) => e.eventID == eventID);
	}

	public GameplayEventInstance StartNewEvent(GameplayEvent eventType, int worldId = -1)
	{
		GameplayEventInstance gameplayEventInstance = this.CreateGameplayEvent(eventType, worldId);
		this.StartEventInstance(gameplayEventInstance);
		this.activeEvents.Add(gameplayEventInstance);
		int num;
		this.pastEvents.TryGetValue(gameplayEventInstance.eventID, out num);
		this.pastEvents[gameplayEventInstance.eventID] = num + 1;
		return gameplayEventInstance;
	}

	private void StartEventInstance(GameplayEventInstance gameplayEventInstance)
	{
		StateMachine.Instance instance = gameplayEventInstance.PrepareEvent(this);
		instance.OnStop = (Action<string, StateMachine.Status>)Delegate.Combine(instance.OnStop, new Action<string, StateMachine.Status>(delegate(string reason, StateMachine.Status status)
		{
			this.activeEvents.Remove(gameplayEventInstance);
		}));
		gameplayEventInstance.StartEvent();
	}

	public int NumberOfPastEvents(HashedString eventID)
	{
		int num;
		this.pastEvents.TryGetValue(eventID, out num);
		return num;
	}

	public static GameplayEventManager Instance;

	public Notifier notifier;

	[Serialize]
	private List<GameplayEventInstance> activeEvents = new List<GameplayEventInstance>();

	[Serialize]
	private Dictionary<HashedString, int> pastEvents = new Dictionary<HashedString, int>();
}
