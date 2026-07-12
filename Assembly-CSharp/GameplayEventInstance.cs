using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GameplayEventInstance : ISaveLoadable
{
	public StateMachine.Instance smi { get; private set; }

	public bool seenNotification
	{
		get
		{
			return this._seenNotification;
		}
		set
		{
			this._seenNotification = value;
			this.monitorCallbackObjects.ForEach(delegate(GameObject x)
			{
				x.Trigger(-1122598290, this);
			});
		}
	}

	public GameplayEvent gameplayEvent
	{
		get
		{
			if (this._gameplayEvent == null)
			{
				this._gameplayEvent = Db.Get().GameplayEvents.TryGet(this.eventID);
			}
			return this._gameplayEvent;
		}
	}

	public GameplayEventInstance(GameplayEvent gameplayEvent, int worldId)
	{
		this.eventID = gameplayEvent.Id;
		this.tags = new List<Tag>();
		this.eventStartTime = GameUtil.GetCurrentTimeInCycles();
		this.worldId = worldId;
	}

	public StateMachine.Instance PrepareEvent(GameplayEventManager manager)
	{
		this.smi = this.gameplayEvent.GetSMI(manager, this);
		return this.smi;
	}

	public void StartEvent()
	{
		GameplayEventManager.Instance.Trigger(1491341646, this);
		StateMachine.Instance smi = this.smi;
		smi.OnStop = (Action<string, StateMachine.Status>)Delegate.Combine(smi.OnStop, new Action<string, StateMachine.Status>(this.OnStop));
		this.smi.StartSM();
	}

	public void RegisterMonitorCallback(GameObject go)
	{
		if (this.monitorCallbackObjects == null)
		{
			this.monitorCallbackObjects = new List<GameObject>();
		}
		if (!this.monitorCallbackObjects.Contains(go))
		{
			this.monitorCallbackObjects.Add(go);
		}
	}

	public void UnregisterMonitorCallback(GameObject go)
	{
		if (this.monitorCallbackObjects == null)
		{
			this.monitorCallbackObjects = new List<GameObject>();
		}
		this.monitorCallbackObjects.Remove(go);
	}

	public void OnStop(string reason, StateMachine.Status status)
	{
		GameplayEventManager.Instance.Trigger(1287635015, this);
		if (this.monitorCallbackObjects != null)
		{
			this.monitorCallbackObjects.ForEach(delegate(GameObject x)
			{
				x.Trigger(1287635015, this);
			});
		}
		if (status == StateMachine.Status.Success)
		{
			using (List<HashedString>.Enumerator enumerator = this.gameplayEvent.successEvents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HashedString hashedString = enumerator.Current;
					GameplayEvent gameplayEvent = Db.Get().GameplayEvents.TryGet(hashedString);
					DebugUtil.DevAssert(gameplayEvent != null, string.Format("GameplayEvent {0} is null", hashedString), null);
					if (gameplayEvent != null && gameplayEvent.IsAllowed())
					{
						GameplayEventManager.Instance.StartNewEvent(gameplayEvent, -1);
					}
				}
				return;
			}
		}
		if (status == StateMachine.Status.Failed)
		{
			foreach (HashedString hashedString2 in this.gameplayEvent.failureEvents)
			{
				GameplayEvent gameplayEvent2 = Db.Get().GameplayEvents.TryGet(hashedString2);
				DebugUtil.DevAssert(gameplayEvent2 != null, string.Format("GameplayEvent {0} is null", hashedString2), null);
				if (gameplayEvent2 != null && gameplayEvent2.IsAllowed())
				{
					GameplayEventManager.Instance.StartNewEvent(gameplayEvent2, -1);
				}
			}
		}
	}

	public static GameplayEventInfoScreen ShowEventPopup(GameplayEventPopupData eventPopupData)
	{
		GameplayEventInfoScreen gameplayEventInfoScreen = (GameplayEventInfoScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.GameplayEventInfoScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject);
		gameplayEventInfoScreen.SetEventData(eventPopupData);
		if (eventPopupData.focus != null)
		{
			WorldContainer myWorld = eventPopupData.focus.gameObject.GetMyWorld();
			if (myWorld != null && myWorld.IsDiscovered)
			{
				CameraController.Instance.ActiveWorldStarWipe(myWorld.id, eventPopupData.focus.position, 10f, null);
			}
		}
		return gameplayEventInfoScreen;
	}

	public static Notification CreateStandardEventNotification(GameplayEventPopupData eventPopupData)
	{
		if (eventPopupData == null)
		{
			DebugUtil.LogWarningArgs(new object[] { "eventPopup is null in CreateStandardEventNotification" });
			return null;
		}
		eventPopupData.FinalizeText();
		return new Notification(eventPopupData.title, NotificationType.Event, null, null, false, 0f, null, null, eventPopupData.focus, true)
		{
			customClickCallback = delegate(object data)
			{
				GameplayEventInstance.ShowEventPopup(eventPopupData);
			}
		};
	}

	public static Notification CreateStandardEventChosenNotification(GameplayEventPopupData eventPopupData)
	{
		if (eventPopupData == null)
		{
			DebugUtil.LogWarningArgs(new object[] { "eventPopup is null in CreateStandardEventChosenNotification" });
			return null;
		}
		eventPopupData.FinalizeText();
		return new Notification(eventPopupData.title, NotificationType.Event, null, null, false, 0f, null, null, eventPopupData.focus, true)
		{
			customClickCallback = delegate(object data)
			{
				GameplayEventInstance.ShowEventPopup(eventPopupData);
			}
		};
	}

	public static Notification CreateStandardCancelledNotification(GameplayEventPopupData eventPopupData)
	{
		if (eventPopupData == null)
		{
			DebugUtil.LogWarningArgs(new object[] { "eventPopup is null in CreateStandardCancelledNotification" });
			return null;
		}
		eventPopupData.FinalizeText();
		return new Notification(string.Format(GAMEPLAY_EVENTS.CANCELED, eventPopupData.title), NotificationType.Event, (List<Notification> list, object data) => string.Format(GAMEPLAY_EVENTS.CANCELED_TOOLTIP, eventPopupData.title), null, true, 0f, null, null, null, true);
	}

	public float AgeInCycles()
	{
		return GameUtil.GetCurrentTimeInCycles() - this.eventStartTime;
	}

	[Serialize]
	public readonly HashedString eventID;

	[Serialize]
	public List<Tag> tags;

	[Serialize]
	public float eventStartTime;

	[Serialize]
	public readonly int worldId;

	[Serialize]
	private bool _seenNotification;

	public List<GameObject> monitorCallbackObjects;

	public GameplayEventInstance.GameplayEventPopupDataCallback GetEventPopupData;

	private GameplayEvent _gameplayEvent;

	public delegate GameplayEventPopupData GameplayEventPopupDataCallback();
}
