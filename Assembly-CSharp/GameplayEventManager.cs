using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;

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
		this.RestoreEvents();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameplayEventManager.Instance = null;
	}

	private void RestoreEvents()
	{
		this.activeEvents.RemoveAll((GameplayEventInstance x) => Db.Get().GameplayEvents.TryGet(x.eventID) == null);
		for (int i = this.activeEvents.Count - 1; i >= 0; i--)
		{
			GameplayEventInstance gameplayEventInstance = this.activeEvents[i];
			if (gameplayEventInstance.smi == null)
			{
				this.StartEventInstance(gameplayEventInstance, null);
			}
		}
	}

	public void SetSleepTimerForEvent(GameplayEvent eventType, float time)
	{
		this.sleepTimers[eventType.IdHash] = time;
	}

	public float GetSleepTimer(GameplayEvent eventType)
	{
		float num = 0f;
		this.sleepTimers.TryGetValue(eventType.IdHash, out num);
		this.sleepTimers[eventType.IdHash] = num;
		return num;
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

	public void GetActiveEventsOfType<T>(ref List<GameplayEventInstance> results) where T : GameplayEvent
	{
		foreach (GameplayEventInstance gameplayEventInstance in this.activeEvents)
		{
			if (gameplayEventInstance.gameplayEvent is T)
			{
				results.Add(gameplayEventInstance);
			}
		}
	}

	private GameplayEventInstance CreateGameplayEvent(GameplayEvent gameplayEvent, int worldId)
	{
		return gameplayEvent.CreateInstance(worldId);
	}

	public GameplayEventInstance GetGameplayEventInstance(HashedString eventID, int worldId = -1)
	{
		return this.activeEvents.Find((GameplayEventInstance e) => e.eventID == eventID && (worldId == -1 || e.worldId == worldId));
	}

	public GameplayEventInstance CreateOrGetEventInstance(GameplayEvent eventType, int worldId = -1)
	{
		GameplayEventInstance gameplayEventInstance = this.GetGameplayEventInstance(eventType.Id, worldId);
		if (gameplayEventInstance == null)
		{
			gameplayEventInstance = this.StartNewEvent(eventType, worldId, null);
		}
		return gameplayEventInstance;
	}

	public void RemoveActiveEvent(GameplayEventInstance eventInstance, string reason = "RemoveActiveEvent() called")
	{
		GameplayEventInstance gameplayEventInstance = this.activeEvents.Find((GameplayEventInstance x) => x == eventInstance);
		if (gameplayEventInstance != null)
		{
			if (gameplayEventInstance.smi != null)
			{
				gameplayEventInstance.smi.StopSM(reason);
				return;
			}
			this.activeEvents.Remove(gameplayEventInstance);
		}
	}

	public GameplayEventInstance StartNewEvent(GameplayEvent eventType, int worldId = -1, Action<StateMachine.Instance> setupActionsBeforeStart = null)
	{
		GameplayEventInstance gameplayEventInstance = this.CreateGameplayEvent(eventType, worldId);
		this.StartEventInstance(gameplayEventInstance, setupActionsBeforeStart);
		this.activeEvents.Add(gameplayEventInstance);
		int num;
		this.pastEvents.TryGetValue(gameplayEventInstance.eventID, out num);
		this.pastEvents[gameplayEventInstance.eventID] = num + 1;
		return gameplayEventInstance;
	}

	private void StartEventInstance(GameplayEventInstance gameplayEventInstance, Action<StateMachine.Instance> setupActionsBeforeStart = null)
	{
		StateMachine.Instance instance = gameplayEventInstance.PrepareEvent(this);
		StateMachine.Instance instance2 = instance;
		instance2.OnStop = (Action<string, StateMachine.Status>)Delegate.Combine(instance2.OnStop, new Action<string, StateMachine.Status>(delegate(string reason, StateMachine.Status status)
		{
			this.activeEvents.Remove(gameplayEventInstance);
		}));
		if (setupActionsBeforeStart != null)
		{
			setupActionsBeforeStart(instance);
		}
		gameplayEventInstance.StartEvent();
	}

	public int NumberOfPastEvents(HashedString eventID)
	{
		int num;
		this.pastEvents.TryGetValue(eventID, out num);
		return num;
	}

	public static Notification CreateStandardCancelledNotification(EventInfoData eventInfoData)
	{
		if (eventInfoData == null)
		{
			DebugUtil.LogWarningArgs(new object[] { "eventPopup is null in CreateStandardCancelledNotification" });
			return null;
		}
		eventInfoData.FinalizeText();
		return new Notification(string.Format(GAMEPLAY_EVENTS.CANCELED, eventInfoData.title), NotificationType.Event, (List<Notification> list, object data) => string.Format(GAMEPLAY_EVENTS.CANCELED_TOOLTIP, eventInfoData.title), null, true, 0f, null, null, null, true, false, false);
	}

	public static GameplayEventManager Instance;

	public Notifier notifier;

	[Serialize]
	private List<GameplayEventInstance> activeEvents = new List<GameplayEventInstance>();

	[Serialize]
	private Dictionary<HashedString, int> pastEvents = new Dictionary<HashedString, int>();

	[Serialize]
	private Dictionary<HashedString, float> sleepTimers = new Dictionary<HashedString, float>();
}
