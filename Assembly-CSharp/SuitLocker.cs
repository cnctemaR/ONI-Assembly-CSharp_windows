using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SuitLocker : StateMachineComponent<SuitLocker.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_target", "meter_arrow", "meter_scale" });
		base.smi.StartSM();
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits);
	}

	public KPrefabID GetStoredOutfit()
	{
		foreach (GameObject gameObject in base.GetComponent<Storage>())
		{
			if (!(gameObject == null))
			{
				KPrefabID component = gameObject.GetComponent<KPrefabID>();
				if (!(component == null))
				{
					if (component.HasAnyTags(this.OutfitTags))
					{
						return component;
					}
				}
			}
		}
		return null;
	}

	public KPrefabID GetFullyChargedOutfit()
	{
		KPrefabID storedOutfit = this.GetStoredOutfit();
		KPrefabID kprefabID;
		if (storedOutfit != null && storedOutfit.GetComponent<SuitTank>().PercentFull() >= 1f)
		{
			kprefabID = storedOutfit;
		}
		else
		{
			kprefabID = null;
		}
		return kprefabID;
	}

	private void CreateFetchChore()
	{
		this.fetchChore = new FetchChore(base.GetComponent<Storage>(), 1f, this.OutfitTags, new Tag[] { GameTags.Assigned }, null, true, null, null, null, FetchOrder2.OperationalRequirement.None, 0);
		this.fetchChore.allowMultifetch = false;
	}

	private void CancelFetchChore()
	{
		if (this.fetchChore != null)
		{
			this.fetchChore.Cancel("SuitLocker.CancelFetchChore");
			this.fetchChore = null;
		}
	}

	public bool HasOxygen()
	{
		return null != this.GetOxygen();
	}

	private void RefreshMeter()
	{
		GameObject oxygen = this.GetOxygen();
		float num = 0f;
		if (oxygen != null)
		{
			num = oxygen.GetComponent<PrimaryElement>().Mass / base.GetComponent<ConduitConsumer>().capacityKG;
			num = Math.Min(num, 1f);
		}
		this.meter.SetPositionPercent(num);
	}

	public bool IsSuitFullyCharged()
	{
		KPrefabID storedOutfit = this.GetStoredOutfit();
		bool flag;
		if (storedOutfit != null)
		{
			SuitTank component = storedOutfit.GetComponent<SuitTank>();
			flag = component == null || component.PercentFull() >= 1f;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	private void OnRequestOutfit()
	{
		base.smi.sm.isWaitingForSuit.Set(true, base.smi);
	}

	private void OnCancelRequest()
	{
		base.smi.sm.isWaitingForSuit.Set(false, base.smi);
	}

	public void DropSuit()
	{
		KPrefabID storedOutfit = this.GetStoredOutfit();
		if (!(storedOutfit == null))
		{
			base.GetComponent<Storage>().Drop(storedOutfit.gameObject);
		}
	}

	public void EquipTo(Equipment equipment)
	{
		KPrefabID storedOutfit = this.GetStoredOutfit();
		if (!(storedOutfit == null))
		{
			base.GetComponent<Storage>().Drop(storedOutfit.gameObject);
			storedOutfit.GetComponent<Equippable>().Assign(equipment.GetComponent<IAssignableIdentity>());
			storedOutfit.GetComponent<Equippable>().CancelChore();
			equipment.Equip(storedOutfit.GetComponent<Equippable>());
			this.returnSuitWorkable.CreateChore();
		}
	}

	public void UnequipFrom(Equipment equipment)
	{
		Assignable assignable = equipment.GetAssignable(global::TUNING.EQUIPMENT.SUIT_SLOT);
		assignable.Unassign();
		base.GetComponent<Storage>().Store(assignable.gameObject, false, false, true);
	}

	public void ConfigRequestSuit()
	{
		base.smi.sm.isConfigured.Set(true, base.smi);
		base.smi.sm.isWaitingForSuit.Set(true, base.smi);
	}

	public void ConfigNoSuit()
	{
		base.smi.sm.isConfigured.Set(true, base.smi);
		base.smi.sm.isWaitingForSuit.Set(false, base.smi);
	}

	public bool CanDropOffSuit()
	{
		return base.smi.sm.isConfigured.Get(base.smi) && !base.smi.sm.isWaitingForSuit.Get(base.smi) && this.GetStoredOutfit() == null;
	}

	private SuitLocker.SuitMarkerState GetSuitMarkerState()
	{
		int num = Grid.PosToCell(this);
		SuitMarker suitMarker = null;
		int num2 = 0;
		GameObject gameObject;
		for (;;)
		{
			int num3 = Grid.OffsetCell(num, num2, 0);
			if (!Grid.IsValidCell(num3))
			{
				break;
			}
			gameObject = Grid.Objects[num3, 1];
			if (gameObject == null)
			{
				break;
			}
			if (!(gameObject.GetComponent<SuitLocker>() != null))
			{
				goto IL_0062;
			}
			num2++;
		}
		goto IL_0078;
		IL_0062:
		suitMarker = gameObject.GetComponent<SuitMarker>();
		IL_0078:
		if (suitMarker == null)
		{
			int num4 = 0;
			GameObject gameObject2;
			for (;;)
			{
				int num5 = Grid.OffsetCell(num, num4, 0);
				if (!Grid.IsValidCell(num5))
				{
					break;
				}
				gameObject2 = Grid.Objects[num5, 1];
				if (gameObject2 == null)
				{
					break;
				}
				if (!(gameObject2.GetComponent<SuitLocker>() != null))
				{
					goto IL_00E2;
				}
				num4--;
			}
			goto IL_00FA;
			IL_00E2:
			suitMarker = gameObject2.GetComponent<SuitMarker>();
			IL_00FA:;
		}
		SuitLocker.SuitMarkerState suitMarkerState = SuitLocker.SuitMarkerState.HasMarker;
		if (suitMarker == null)
		{
			suitMarkerState = SuitLocker.SuitMarkerState.NoMarker;
		}
		else if (suitMarker.transform.position.x > base.transform.position.x && suitMarker.GetComponent<Rotatable>().IsRotated)
		{
			suitMarkerState = SuitLocker.SuitMarkerState.WrongSide;
		}
		else if (suitMarker.transform.position.x < base.transform.position.x && !suitMarker.GetComponent<Rotatable>().IsRotated)
		{
			suitMarkerState = SuitLocker.SuitMarkerState.WrongSide;
		}
		return suitMarkerState;
	}

	private GameObject GetOxygen()
	{
		List<GameObject> list = base.GetComponent<Storage>().Find(GameTags.Oxygen);
		GameObject gameObject;
		if (list == null || list.Count == 0)
		{
			gameObject = null;
		}
		else
		{
			gameObject = list[0];
		}
		return gameObject;
	}

	private void ChargeSuit(float dt)
	{
		KPrefabID storedOutfit = this.GetStoredOutfit();
		if (!(storedOutfit == null))
		{
			GameObject oxygen = this.GetOxygen();
			if (!(oxygen == null))
			{
				SuitTank component = storedOutfit.GetComponent<SuitTank>();
				float num = component.capacity * 15f * dt / 600f;
				num = Mathf.Min(num, component.capacity - component.amount);
				num = Mathf.Min(oxygen.GetComponent<PrimaryElement>().Mass, num);
				oxygen.GetComponent<PrimaryElement>().Mass -= num;
				component.amount += num;
			}
		}
	}

	private void Update()
	{
		SuitLocker.SuitMarkerState suitMarkerState = this.GetSuitMarkerState();
		if (suitMarkerState != this.suitMarkerState)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoSuitMarker, false);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerWrongSide, false);
			if (suitMarkerState != SuitLocker.SuitMarkerState.HasMarker)
			{
				if (suitMarkerState != SuitLocker.SuitMarkerState.NoMarker)
				{
					if (suitMarkerState == SuitLocker.SuitMarkerState.WrongSide)
					{
						base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerWrongSide, null);
					}
				}
				else
				{
					base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSuitMarker, null);
				}
			}
			this.suitMarkerState = suitMarkerState;
		}
		this.RefreshMeter();
	}

	public Tag[] OutfitTags;

	private FetchChore fetchChore;

	[MyCmpAdd]
	public SuitLocker.ReturnSuitWorkable returnSuitWorkable;

	private MeterController meter;

	private SuitLocker.SuitMarkerState suitMarkerState = SuitLocker.SuitMarkerState.HasMarker;

	public class ReturnSuitWorkable : Workable
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.resetProgressOnStop = true;
			this.workTime = 0.25f;
			this.synchronizeAnims = false;
		}

		public void CreateChore()
		{
			if (this.urgentChore == null)
			{
				this.urgentChore = new WorkChore<SuitLocker.ReturnSuitWorkable>(Db.Get().ChoreTypes.ReturnSuitUrgent, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
				this.urgentChore.AddPrecondition(SuitLocker.ReturnSuitWorkable.DoesSuitNeedRechargingUrgent, null);
				this.idleChore = new WorkChore<SuitLocker.ReturnSuitWorkable>(Db.Get().ChoreTypes.ReturnSuitIdle, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true, true, PriorityScreen.PriorityClass.basic, -1);
				this.idleChore.AddPrecondition(SuitLocker.ReturnSuitWorkable.DoesSuitNeedRechargingIdle, null);
			}
		}

		public void CancelChore()
		{
			if (this.urgentChore != null)
			{
				this.urgentChore.Cancel("ReturnSuitWorkable.CancelChore");
				this.urgentChore = null;
			}
			if (this.idleChore != null)
			{
				this.idleChore.Cancel("ReturnSuitWorkable.CancelChore");
				this.idleChore = null;
			}
		}

		protected override void OnStartWork(Worker worker)
		{
			base.ShowProgressBar(false);
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			return true;
		}

		protected override void OnCompleteWork(Worker worker)
		{
			Equipment component = worker.GetComponent<Equipment>();
			if (worker.GetComponent<Equipment>().IsSlotOccupied(global::TUNING.EQUIPMENT.SUIT_SLOT))
			{
				SuitLocker component2 = base.GetComponent<SuitLocker>();
				if (component2.CanDropOffSuit())
				{
					base.GetComponent<SuitLocker>().UnequipFrom(component);
				}
				else
				{
					Assignable assignable = worker.GetComponent<Equipment>().GetAssignable(global::TUNING.EQUIPMENT.SUIT_SLOT);
					assignable.Unassign();
				}
			}
			if (this.urgentChore != null)
			{
				this.CancelChore();
				this.CreateChore();
			}
		}

		public override HashedString[] GetWorkAnims(Worker worker)
		{
			return new HashedString[]
			{
				new HashedString("none")
			};
		}

		public static Chore.Precondition DoesSuitNeedRechargingUrgent = new Chore.Precondition
		{
			id = "DoesSuitNeedRechargingUrgent",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Equipment component = context.consumer.GetComponent<Equipment>();
				AssignableSlotInstance slot = component.GetSlot(global::TUNING.EQUIPMENT.SUIT_SLOT);
				bool flag;
				if (slot.assignable == null)
				{
					flag = false;
				}
				else
				{
					SuitTank component2 = slot.assignable.GetComponent<SuitTank>();
					flag = !(component2 == null) && component2.NeedsRecharging();
				}
				return flag;
			}
		};

		public static Chore.Precondition DoesSuitNeedRechargingIdle = new Chore.Precondition
		{
			id = "DoesSuitNeedRechargingIdle",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Equipment component3 = context.consumer.GetComponent<Equipment>();
				AssignableSlotInstance slot2 = component3.GetSlot(global::TUNING.EQUIPMENT.SUIT_SLOT);
				bool flag2;
				if (slot2.assignable == null)
				{
					flag2 = false;
				}
				else
				{
					SuitTank component4 = slot2.assignable.GetComponent<SuitTank>();
					flag2 = !(component4 == null);
				}
				return flag2;
			}
		};

		private WorkChore<SuitLocker.ReturnSuitWorkable> urgentChore;

		private WorkChore<SuitLocker.ReturnSuitWorkable> idleChore;
	}

	public class StatesInstance : GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.GameInstance
	{
		public StatesInstance(SuitLocker suit_locker)
			: base(suit_locker)
		{
		}
	}

	public class States : GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.empty;
			base.serializable = true;
			this.empty.DefaultState(this.empty.notconfigured).EventTransition(GameHashes.OnStorageChange, this.charging, (SuitLocker.StatesInstance smi) => smi.master.GetStoredOutfit() != null).ParamTransition<bool>(this.isWaitingForSuit, this.waitingforsuit, (SuitLocker.StatesInstance smi, bool p) => p)
				.Enter("CreateReturnSuitChore", delegate(SuitLocker.StatesInstance smi)
				{
					smi.master.returnSuitWorkable.CreateChore();
				})
				.RefreshUserMenuOnEnter()
				.Exit("CancelReturnSuitChore", delegate(SuitLocker.StatesInstance smi)
				{
					smi.master.returnSuitWorkable.CancelChore();
				})
				.PlayAnim("no_suit");
			GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State state = this.empty.notconfigured.ParamTransition<bool>(this.isConfigured, this.empty.configured, (SuitLocker.StatesInstance smi, bool p) => p);
			string text = BUILDING.STATUSITEMS.SUIT_LOCKER_NEEDS_CONFIGURATION.NAME;
			string text2 = BUILDING.STATUSITEMS.SUIT_LOCKER_NEEDS_CONFIGURATION.TOOLTIP;
			string text3 = "status_item_no_filter_set";
			StatusItem.IconType iconType = StatusItem.IconType.Custom;
			NotificationType notificationType = NotificationType.BadMinor;
			StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
			state.ToggleStatusItem(text, text2, text3, iconType, notificationType, false, SimViewMode.None, 0, null, null, statusItemCategory);
			GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State state2 = this.empty.configured.RefreshUserMenuOnEnter().ParamTransition<bool>(this.isConfigured, this.empty.configured, (SuitLocker.StatesInstance smi, bool p) => !p);
			text3 = BUILDING.STATUSITEMS.SUIT_LOCKER.READY.NAME;
			text2 = BUILDING.STATUSITEMS.SUIT_LOCKER.READY.TOOLTIP;
			statusItemCategory = Db.Get().StatusItemCategories.Main;
			state2.ToggleStatusItem(text3, text2, "", StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
			GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State state3 = this.waitingforsuit.EventTransition(GameHashes.OnStorageChange, this.charging, (SuitLocker.StatesInstance smi) => smi.master.GetStoredOutfit() != null).Enter("CreateFetchChore", delegate(SuitLocker.StatesInstance smi)
			{
				smi.master.CreateFetchChore();
			}).ParamTransition<bool>(this.isWaitingForSuit, this.empty, (SuitLocker.StatesInstance smi, bool p) => !p)
				.RefreshUserMenuOnEnter()
				.PlayAnim("awaiting_suit")
				.Exit("ClearIsWaitingForSuit", delegate(SuitLocker.StatesInstance smi)
				{
					this.isWaitingForSuit.Set(false, smi);
				})
				.Exit("CancelFetchChore", delegate(SuitLocker.StatesInstance smi)
				{
					smi.master.CancelFetchChore();
				});
			text2 = BUILDING.STATUSITEMS.SUIT_LOCKER.SUIT_REQUESTED.NAME;
			text3 = BUILDING.STATUSITEMS.SUIT_LOCKER.SUIT_REQUESTED.TOOLTIP;
			statusItemCategory = Db.Get().StatusItemCategories.Main;
			state3.ToggleStatusItem(text2, text3, "", StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
			this.charging.DefaultState(this.charging.pre).RefreshUserMenuOnEnter().EventTransition(GameHashes.OnStorageChange, this.empty, (SuitLocker.StatesInstance smi) => smi.master.GetStoredOutfit() == null);
			this.charging.pre.Enter(delegate(SuitLocker.StatesInstance smi)
			{
				if (smi.master.IsSuitFullyCharged())
				{
					smi.GoTo(this.suitfullycharged);
				}
				else if (smi.master.GetComponent<Operational>().IsOperational)
				{
					smi.GetComponent<KBatchedAnimController>().Play("charging_pre", KAnim.PlayMode.Once, 1f, 0f);
				}
				else
				{
					smi.GetComponent<KBatchedAnimController>().Play("not_charging_pre", KAnim.PlayMode.Once, 1f, 0f);
				}
			}).OnAnimQueueComplete(this.charging.operational);
			GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State state4 = this.charging.operational.TagTransition(GameTags.Operational, this.charging.notoperational, true).Transition(this.charging.nooxygen, (SuitLocker.StatesInstance smi) => !smi.master.HasOxygen()).PlayAnim("charging_loop", KAnim.PlayMode.Loop)
				.Enter("SetActive", delegate(SuitLocker.StatesInstance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(true, false);
				})
				.Transition(this.charging.pst_operational, (SuitLocker.StatesInstance smi) => smi.master.IsSuitFullyCharged())
				.Update("ChargeSuit", delegate(SuitLocker.StatesInstance smi)
				{
					smi.master.ChargeSuit(smi.dt);
				})
				.Exit("ClearActive", delegate(SuitLocker.StatesInstance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(false, false);
				});
			text3 = BUILDING.STATUSITEMS.SUIT_LOCKER.CHARGING.NAME;
			text2 = BUILDING.STATUSITEMS.SUIT_LOCKER.CHARGING.TOOLTIP;
			statusItemCategory = Db.Get().StatusItemCategories.Main;
			state4.ToggleStatusItem(text3, text2, "", StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
			GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State state5 = this.charging.nooxygen.TagTransition(GameTags.Operational, this.charging.notoperational, true).Transition(this.charging.operational, (SuitLocker.StatesInstance smi) => smi.master.HasOxygen()).Transition(this.charging.pst_operational, (SuitLocker.StatesInstance smi) => smi.master.IsSuitFullyCharged())
				.PlayAnim("no_o2_loop", KAnim.PlayMode.Loop);
			text2 = BUILDING.STATUSITEMS.SUIT_LOCKER.NO_OXYGEN.NAME;
			text3 = BUILDING.STATUSITEMS.SUIT_LOCKER.NO_OXYGEN.TOOLTIP;
			text = "status_item_suit_locker_no_oxygen";
			iconType = StatusItem.IconType.Custom;
			notificationType = NotificationType.BadMinor;
			statusItemCategory = Db.Get().StatusItemCategories.Main;
			state5.ToggleStatusItem(text2, text3, text, iconType, notificationType, false, SimViewMode.None, 0, null, null, statusItemCategory);
			GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State state6 = this.charging.notoperational.TagTransition(GameTags.Operational, this.charging.operational, false).PlayAnim("not_charging_loop", KAnim.PlayMode.Loop).Transition(this.charging.pst_notoperational, (SuitLocker.StatesInstance smi) => smi.master.IsSuitFullyCharged());
			text = BUILDING.STATUSITEMS.SUIT_LOCKER.NOT_OPERATIONAL.NAME;
			text3 = BUILDING.STATUSITEMS.SUIT_LOCKER.NOT_OPERATIONAL.TOOLTIP;
			statusItemCategory = Db.Get().StatusItemCategories.Main;
			state6.ToggleStatusItem(text, text3, "", StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
			this.charging.pst_operational.PlayAnim("charging_pst").OnAnimQueueComplete(this.suitfullycharged);
			this.charging.pst_notoperational.PlayAnim("not_charging_pst").OnAnimQueueComplete(this.suitfullycharged);
			GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State state7 = this.suitfullycharged.EventTransition(GameHashes.OnStorageChange, this.empty, (SuitLocker.StatesInstance smi) => smi.master.GetStoredOutfit() == null).PlayAnim("has_suit").RefreshUserMenuOnEnter();
			text3 = BUILDING.STATUSITEMS.SUIT_LOCKER.FULLY_CHARGED.NAME;
			text = BUILDING.STATUSITEMS.SUIT_LOCKER.FULLY_CHARGED.TOOLTIP;
			statusItemCategory = Db.Get().StatusItemCategories.Main;
			state7.ToggleStatusItem(text3, text, "", StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		}

		public SuitLocker.States.EmptyStates empty;

		public SuitLocker.States.ChargingStates charging;

		public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State waitingforsuit;

		public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State suitfullycharged;

		public StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.BoolParameter isWaitingForSuit;

		public StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.BoolParameter isConfigured;

		public class ChargingStates : GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State
		{
			public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State pre;

			public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State pst_operational;

			public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State pst_notoperational;

			public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State operational;

			public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State nooxygen;

			public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State notoperational;
		}

		public class EmptyStates : GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State
		{
			public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State configured;

			public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State notconfigured;
		}
	}

	private enum SuitMarkerState
	{
		HasMarker,
		NoMarker,
		WrongSide
	}
}
