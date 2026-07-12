using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class LogicCircuitNetwork : UtilityNetwork
{
	public override void AddItem(object item)
	{
		if (item is LogicWire)
		{
			LogicWire logicWire = (LogicWire)item;
			LogicWire.BitDepth maxBitDepth = logicWire.MaxBitDepth;
			List<LogicWire> list = this.wireGroups[(int)maxBitDepth];
			if (list == null)
			{
				list = new List<LogicWire>();
				this.wireGroups[(int)maxBitDepth] = list;
			}
			list.Add(logicWire);
			return;
		}
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = (ILogicEventReceiver)item;
			this.receivers.Add(logicEventReceiver);
			return;
		}
		if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = (ILogicEventSender)item;
			this.senders.Add(logicEventSender);
		}
	}

	public override void RemoveItem(object item)
	{
		if (item is LogicWire)
		{
			LogicWire logicWire = (LogicWire)item;
			this.wireGroups[(int)logicWire.MaxBitDepth].Remove(logicWire);
			return;
		}
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = item as ILogicEventReceiver;
			this.receivers.Remove(logicEventReceiver);
			return;
		}
		if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = (ILogicEventSender)item;
			this.senders.Remove(logicEventSender);
		}
	}

	public override void ConnectItem(object item)
	{
		if (item is ILogicEventReceiver)
		{
			((ILogicEventReceiver)item).OnLogicNetworkConnectionChanged(true);
			return;
		}
		if (item is ILogicEventSender)
		{
			((ILogicEventSender)item).OnLogicNetworkConnectionChanged(true);
		}
	}

	public override void DisconnectItem(object item)
	{
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = item as ILogicEventReceiver;
			logicEventReceiver.ReceiveLogicEvent(0);
			logicEventReceiver.OnLogicNetworkConnectionChanged(false);
			return;
		}
		if (item is ILogicEventSender)
		{
			(item as ILogicEventSender).OnLogicNetworkConnectionChanged(false);
		}
	}

	public override void Reset(UtilityNetworkGridNode[] grid)
	{
		this.resetting = true;
		this.previousValue = -1;
		this.outputValue = 0;
		for (int i = 0; i < 2; i++)
		{
			List<LogicWire> list = this.wireGroups[i];
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					LogicWire logicWire = list[j];
					if (logicWire != null)
					{
						int num = Grid.PosToCell(logicWire.transform.GetPosition());
						UtilityNetworkGridNode utilityNetworkGridNode = grid[num];
						utilityNetworkGridNode.networkIdx = -1;
						grid[num] = utilityNetworkGridNode;
					}
				}
				list.Clear();
			}
		}
		this.senders.Clear();
		this.receivers.Clear();
		this.resetting = false;
		this.RemoveOverloadedNotification();
	}

	public void UpdateLogicValue()
	{
		if (this.resetting)
		{
			return;
		}
		this.previousValue = this.outputValue;
		this.outputValue = 0;
		foreach (ILogicEventSender logicEventSender in this.senders)
		{
			logicEventSender.LogicTick();
		}
		foreach (ILogicEventSender logicEventSender2 in this.senders)
		{
			int logicValue = logicEventSender2.GetLogicValue();
			this.outputValue |= logicValue;
		}
	}

	public int GetBitsUsed()
	{
		int num;
		if (this.outputValue > 1)
		{
			num = 4;
		}
		else
		{
			num = 1;
		}
		return num;
	}

	public bool IsBitActive(int bit)
	{
		return (this.OutputValue & (1 << bit)) > 0;
	}

	public static bool IsBitActive(int bit, int value)
	{
		return (value & (1 << bit)) > 0;
	}

	public static int GetBitValue(int bit, int value)
	{
		return value & (1 << bit);
	}

	public void SendLogicEvents(bool force_send, int id)
	{
		if (this.resetting)
		{
			return;
		}
		if (this.outputValue != this.previousValue || force_send)
		{
			foreach (ILogicEventReceiver logicEventReceiver in this.receivers)
			{
				logicEventReceiver.ReceiveLogicEvent(this.outputValue);
			}
			if (!force_send)
			{
				this.TriggerAudio((this.previousValue >= 0) ? this.previousValue : 0, id);
			}
		}
	}

	private void TriggerAudio(int old_value, int id)
	{
		SpeedControlScreen instance = SpeedControlScreen.Instance;
		if (old_value != this.outputValue && instance != null && !instance.IsPaused)
		{
			int num = 0;
			GridArea visibleArea = GridVisibleArea.GetVisibleArea();
			List<LogicWire> list = new List<LogicWire>();
			for (int i = 0; i < 2; i++)
			{
				List<LogicWire> list2 = this.wireGroups[i];
				if (list2 != null)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						num++;
						if (visibleArea.Min <= list2[j].transform.GetPosition() && list2[j].transform.GetPosition() <= visibleArea.Max)
						{
							list.Add(list2[j]);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				int num2 = Mathf.CeilToInt((float)(list.Count / 2));
				if (list[num2] != null)
				{
					Vector3 position = list[num2].transform.GetPosition();
					position.z = 0f;
					string text = "Logic_Circuit_Toggle";
					LogicCircuitNetwork.LogicSoundPair logicSoundPair = new LogicCircuitNetwork.LogicSoundPair();
					if (!LogicCircuitNetwork.logicSoundRegister.ContainsKey(id))
					{
						LogicCircuitNetwork.logicSoundRegister.Add(id, logicSoundPair);
					}
					else
					{
						logicSoundPair.playedIndex = LogicCircuitNetwork.logicSoundRegister[id].playedIndex;
						logicSoundPair.lastPlayed = LogicCircuitNetwork.logicSoundRegister[id].lastPlayed;
					}
					if (logicSoundPair.playedIndex < 2)
					{
						LogicCircuitNetwork.logicSoundRegister[id].playedIndex = logicSoundPair.playedIndex + 1;
					}
					else
					{
						LogicCircuitNetwork.logicSoundRegister[id].playedIndex = 0;
						LogicCircuitNetwork.logicSoundRegister[id].lastPlayed = Time.time;
					}
					float num3 = (Time.time - logicSoundPair.lastPlayed) / 3f;
					EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound(text, false), position, 1f);
					eventInstance.setParameterByName("logic_volumeModifer", num3, false);
					eventInstance.setParameterByName("wireCount", (float)(num % 24), false);
					eventInstance.setParameterByName("enabled", (float)this.outputValue, false);
					KFMOD.EndOneShot(eventInstance);
				}
			}
		}
	}

	public void UpdateOverloadTime(float dt, int bits_used)
	{
		bool flag = false;
		List<LogicWire> list = null;
		List<LogicUtilityNetworkLink> list2 = null;
		for (int i = 0; i < 2; i++)
		{
			List<LogicWire> list3 = this.wireGroups[i];
			List<LogicUtilityNetworkLink> list4 = this.relevantBridges[i];
			float num = (float)LogicWire.GetBitDepthAsInt((LogicWire.BitDepth)i);
			if ((float)bits_used > num && ((list4 != null && list4.Count > 0) || (list3 != null && list3.Count > 0)))
			{
				flag = true;
				list = list3;
				list2 = list4;
				break;
			}
		}
		if (list != null)
		{
			list.RemoveAll((LogicWire x) => x == null);
		}
		if (list2 != null)
		{
			list2.RemoveAll((LogicUtilityNetworkLink x) => x == null);
		}
		if (flag)
		{
			this.timeOverloaded += dt;
			if (this.timeOverloaded > 6f)
			{
				this.timeOverloaded = 0f;
				if (this.targetOverloadedWire == null)
				{
					if (list2 != null && list2.Count > 0)
					{
						int num2 = global::UnityEngine.Random.Range(0, list2.Count);
						this.targetOverloadedWire = list2[num2].gameObject;
					}
					else if (list != null && list.Count > 0)
					{
						int num3 = global::UnityEngine.Random.Range(0, list.Count);
						this.targetOverloadedWire = list[num3].gameObject;
					}
				}
				if (this.targetOverloadedWire != null)
				{
					this.targetOverloadedWire.Trigger(-794517298, new BuildingHP.DamageSourceInfo
					{
						damage = 1,
						source = BUILDINGS.DAMAGESOURCES.LOGIC_CIRCUIT_OVERLOADED,
						popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LOGIC_CIRCUIT_OVERLOADED,
						takeDamageEffect = SpawnFXHashes.BuildingLogicOverload,
						fullDamageEffectName = "logic_ribbon_damage_kanim",
						statusItemID = Db.Get().BuildingStatusItems.LogicOverloaded.Id
					});
				}
				if (this.overloadedNotification == null)
				{
					this.timeOverloadNotificationDisplayed = 0f;
					this.overloadedNotification = new Notification(MISC.NOTIFICATIONS.LOGIC_CIRCUIT_OVERLOADED.NAME, NotificationType.BadMinor, null, null, true, 0f, null, null, this.targetOverloadedWire.transform, true, false, false);
					Game.Instance.FindOrAdd<Notifier>().Add(this.overloadedNotification, "");
					return;
				}
			}
		}
		else
		{
			this.timeOverloaded = Mathf.Max(0f, this.timeOverloaded - dt * 0.95f);
			this.timeOverloadNotificationDisplayed += dt;
			if (this.timeOverloadNotificationDisplayed > 5f)
			{
				this.RemoveOverloadedNotification();
			}
		}
	}

	private void RemoveOverloadedNotification()
	{
		if (this.overloadedNotification != null)
		{
			Game.Instance.FindOrAdd<Notifier>().Remove(this.overloadedNotification);
			this.overloadedNotification = null;
		}
	}

	public void UpdateRelevantBridges(List<LogicUtilityNetworkLink>[] bridgeGroups)
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		for (int i = 0; i < bridgeGroups.Length; i++)
		{
			if (this.relevantBridges[i] != null)
			{
				this.relevantBridges[i].Clear();
			}
			for (int j = 0; j < bridgeGroups[i].Count; j++)
			{
				if (logicCircuitManager.GetNetworkForCell(bridgeGroups[i][j].cell_one) == this || logicCircuitManager.GetNetworkForCell(bridgeGroups[i][j].cell_two) == this)
				{
					if (this.relevantBridges[i] == null)
					{
						this.relevantBridges[i] = new List<LogicUtilityNetworkLink>();
					}
					this.relevantBridges[i].Add(bridgeGroups[i][j]);
				}
			}
		}
	}

	public int OutputValue
	{
		get
		{
			return this.outputValue;
		}
	}

	public int WireCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < 2; i++)
			{
				if (this.wireGroups[i] != null)
				{
					num += this.wireGroups[i].Count;
				}
			}
			return num;
		}
	}

	public ReadOnlyCollection<ILogicEventSender> Senders
	{
		get
		{
			return this.senders.AsReadOnly();
		}
	}

	public ReadOnlyCollection<ILogicEventReceiver> Receivers
	{
		get
		{
			return this.receivers.AsReadOnly();
		}
	}

	private List<LogicWire>[] wireGroups = new List<LogicWire>[2];

	private List<LogicUtilityNetworkLink>[] relevantBridges = new List<LogicUtilityNetworkLink>[2];

	private List<ILogicEventReceiver> receivers = new List<ILogicEventReceiver>();

	private List<ILogicEventSender> senders = new List<ILogicEventSender>();

	private int previousValue = -1;

	private int outputValue;

	private bool resetting;

	public static float logicSoundLastPlayedTime = 0f;

	private const float MIN_OVERLOAD_TIME_FOR_DAMAGE = 6f;

	private const float MIN_OVERLOAD_NOTIFICATION_DISPLAY_TIME = 5f;

	public const int UNINITIALIZED_LOGIC_STATE = -1;

	private GameObject targetOverloadedWire;

	private float timeOverloaded;

	private float timeOverloadNotificationDisplayed;

	private Notification overloadedNotification;

	public static Dictionary<int, LogicCircuitNetwork.LogicSoundPair> logicSoundRegister = new Dictionary<int, LogicCircuitNetwork.LogicSoundPair>();

	public class LogicSoundPair
	{
		public int playedIndex;

		public float lastPlayed;
	}
}
