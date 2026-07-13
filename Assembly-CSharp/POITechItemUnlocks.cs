using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class POITechItemUnlocks : GameStateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.locked;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.locked.PlayAnim("on", KAnim.PlayMode.Loop).ParamTransition<bool>(this.isUnlocked, this.unlocked, GameStateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.IsTrue);
		this.unlocked.ParamTransition<bool>(this.seenNotification, this.unlocked.notify, GameStateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.IsFalse).ParamTransition<bool>(this.seenNotification, this.unlocked.done, GameStateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.IsTrue);
		this.unlocked.notify.PlayAnim("notify", KAnim.PlayMode.Loop).ToggleStatusItem(Db.Get().MiscStatusItems.AttentionRequired, null).ToggleNotification(delegate(POITechItemUnlocks.Instance smi)
		{
			smi.notificationReference = EventInfoScreen.CreateNotification(POITechItemUnlocks.GenerateEventPopupData(smi), null);
			smi.notificationReference.Type = NotificationType.MessageImportant;
			return smi.notificationReference;
		});
		this.unlocked.done.PlayAnim("off");
	}

	private static void OnNotificationAknowledged(object o)
	{
		GameObject gameObject = (GameObject)o;
		Game.Instance.Trigger(1633134300, gameObject);
	}

	private static string GetMessageBody(POITechItemUnlocks.Instance smi)
	{
		string text = "";
		foreach (TechItem techItem in smi.unlockTechItems)
		{
			text = text + "\n    • " + techItem.Name;
		}
		return string.Format((smi.def.loreUnlockId != null) ? MISC.NOTIFICATIONS.POIRESEARCHUNLOCKCOMPLETE.MESSAGEBODY : MISC.NOTIFICATIONS.POIRESEARCHUNLOCKCOMPLETE_NOLORE.MESSAGEBODY, text);
	}

	private static EventInfoData GenerateEventPopupData(POITechItemUnlocks.Instance smi)
	{
		EventInfoData eventInfoData = new EventInfoData(MISC.NOTIFICATIONS.POIRESEARCHUNLOCKCOMPLETE.NAME, POITechItemUnlocks.GetMessageBody(smi), smi.def.animName);
		int num = Mathf.Max(2, Components.LiveMinionIdentities.Count);
		GameObject[] array = new GameObject[num];
		using (IEnumerator<MinionIdentity> enumerator = Components.LiveMinionIdentities.Shuffle<MinionIdentity>().GetEnumerator())
		{
			for (int i = 0; i < num; i++)
			{
				if (!enumerator.MoveNext())
				{
					num = 0;
					array = new GameObject[num];
					break;
				}
				array[i] = enumerator.Current.gameObject;
			}
		}
		eventInfoData.minions = array;
		if (smi.def.loreUnlockId != null)
		{
			eventInfoData.AddOption(MISC.NOTIFICATIONS.POIRESEARCHUNLOCKCOMPLETE.BUTTON_VIEW_LORE, null).callback = delegate
			{
				smi.sm.seenNotification.Set(true, smi, false);
				smi.notificationReference = null;
				Game.Instance.unlocks.Unlock(smi.def.loreUnlockId, true);
				ManagementMenu.Instance.OpenCodexToLockId(smi.def.loreUnlockId, false);
				POITechItemUnlocks.OnNotificationAknowledged(smi.gameObject);
			};
		}
		eventInfoData.AddDefaultOption(delegate
		{
			smi.sm.seenNotification.Set(true, smi, false);
			smi.notificationReference = null;
			POITechItemUnlocks.OnNotificationAknowledged(smi.gameObject);
		});
		eventInfoData.clickFocus = smi.gameObject.transform;
		return eventInfoData;
	}

	public GameStateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.State locked;

	public POITechItemUnlocks.UnlockedStates unlocked;

	public StateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.BoolParameter isUnlocked;

	public StateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.BoolParameter pendingChore;

	public StateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.BoolParameter seenNotification;

	public class Def : StateMachine.BaseDef
	{
		public List<string> POITechUnlockIDs;

		public LocString PopUpName;

		public string animName;

		public string loreUnlockId;
	}

	public new class Instance : GameStateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.GameInstance, ISidescreenButtonControl
	{
		public Instance(IStateMachineTarget master, POITechItemUnlocks.Def def)
			: base(master, def)
		{
			this.unlockTechItems = new List<TechItem>(def.POITechUnlockIDs.Count);
			foreach (string text in def.POITechUnlockIDs)
			{
				TechItem techItem = Db.Get().TechItems.TryGet(text);
				if (techItem != null)
				{
					this.unlockTechItems.Add(techItem);
				}
				else
				{
					DebugUtil.DevAssert(false, "Invalid tech item " + text + " for POI Tech Unlock", null);
				}
			}
		}

		public override void StartSM()
		{
			this.onBuildingSelectHandle = base.Subscribe(-1503271301, new Action<object>(this.OnBuildingSelect));
			this.UpdateUnlocked();
			base.StartSM();
			if (base.sm.pendingChore.Get(this) && this.unlockChore == null)
			{
				this.CreateChore();
			}
		}

		public override void StopSM(string reason)
		{
			base.Unsubscribe(ref this.onBuildingSelectHandle);
			base.StopSM(reason);
		}

		public void OnBuildingSelect(object obj)
		{
			if (!((Boxed<bool>)obj).value)
			{
				return;
			}
			if (!base.sm.seenNotification.Get(this) && this.notificationReference != null)
			{
				this.notificationReference.customClickCallback(this.notificationReference.customClickData);
			}
		}

		private void ShowPopup()
		{
		}

		public void UnlockTechItems()
		{
			foreach (TechItem techItem in this.unlockTechItems)
			{
				if (techItem != null)
				{
					techItem.POIUnlocked();
				}
			}
			MusicManager.instance.PlaySong("Stinger_ResearchComplete", false);
			this.UpdateUnlocked();
		}

		private void UpdateUnlocked()
		{
			bool flag = true;
			using (List<TechItem>.Enumerator enumerator = this.unlockTechItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.IsComplete())
					{
						flag = false;
						break;
					}
				}
			}
			base.sm.isUnlocked.Set(flag, base.smi, false);
		}

		public string SidescreenButtonText
		{
			get
			{
				if (base.sm.isUnlocked.Get(base.smi))
				{
					return UI.USERMENUACTIONS.OPEN_TECHUNLOCKS.ALREADY_RUMMAGED;
				}
				if (this.unlockChore != null)
				{
					return UI.USERMENUACTIONS.OPEN_TECHUNLOCKS.NAME_OFF;
				}
				return UI.USERMENUACTIONS.OPEN_TECHUNLOCKS.NAME;
			}
		}

		public string SidescreenButtonTooltip
		{
			get
			{
				if (base.sm.isUnlocked.Get(base.smi))
				{
					return UI.USERMENUACTIONS.OPEN_TECHUNLOCKS.TOOLTIP_ALREADYRUMMAGED;
				}
				if (this.unlockChore != null)
				{
					return UI.USERMENUACTIONS.OPEN_TECHUNLOCKS.TOOLTIP_OFF;
				}
				return UI.USERMENUACTIONS.OPEN_TECHUNLOCKS.TOOLTIP;
			}
		}

		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
			throw new NotImplementedException();
		}

		public bool SidescreenEnabled()
		{
			return base.smi.IsInsideState(base.sm.locked);
		}

		public bool SidescreenButtonInteractable()
		{
			return base.smi.IsInsideState(base.sm.locked);
		}

		public void OnSidescreenButtonPressed()
		{
			if (this.unlockChore == null)
			{
				base.smi.sm.pendingChore.Set(true, base.smi, false);
				base.smi.CreateChore();
				return;
			}
			base.smi.sm.pendingChore.Set(false, base.smi, false);
			base.smi.CancelChore();
		}

		private void CreateChore()
		{
			Workable component = base.smi.master.GetComponent<POITechItemUnlockWorkable>();
			Prioritizable.AddRef(base.gameObject);
			base.Trigger(1980521255, null);
			this.unlockChore = new WorkChore<POITechItemUnlockWorkable>(Db.Get().ChoreTypes.Research, component, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		}

		private void CancelChore()
		{
			this.unlockChore.Cancel("UserCancel");
			this.unlockChore = null;
			Prioritizable.RemoveRef(base.gameObject);
			base.Trigger(1980521255, null);
		}

		public int HorizontalGroupID()
		{
			return -1;
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}

		public List<TechItem> unlockTechItems;

		public Notification notificationReference;

		private int onBuildingSelectHandle = -1;

		private Chore unlockChore;
	}

	public class UnlockedStates : GameStateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.State
	{
		public GameStateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.State notify;

		public GameStateMachine<POITechItemUnlocks, POITechItemUnlocks.Instance, IStateMachineTarget, POITechItemUnlocks.Def>.State done;
	}
}
