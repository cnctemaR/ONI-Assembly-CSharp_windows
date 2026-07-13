using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MaskStation : StateMachineComponent<MaskStation.SMInstance>, IBasicBuilding
{
	private bool isRotated
	{
		get
		{
			return (this.gridFlags & Grid.SuitMarker.Flags.Rotated) > (Grid.SuitMarker.Flags)0;
		}
		set
		{
			this.UpdateGridFlag(Grid.SuitMarker.Flags.Rotated, value);
		}
	}

	private bool isOperational
	{
		get
		{
			return (this.gridFlags & Grid.SuitMarker.Flags.Operational) > (Grid.SuitMarker.Flags)0;
		}
		set
		{
			this.UpdateGridFlag(Grid.SuitMarker.Flags.Operational, value);
		}
	}

	public void UpdateOperational()
	{
		bool flag = this.GetTotalOxygenAmount() >= this.oxygenConsumedPerMask * (float)this.maxUses;
		this.shouldPump = this.IsPumpable();
		if (this.operational.IsOperational && this.shouldPump && !flag)
		{
			this.operational.SetActive(true, false);
		}
		else
		{
			this.operational.SetActive(false, false);
		}
		this.noElementStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidMaskStationConsumptionState, this.noElementStatusGuid, !this.shouldPump, null);
	}

	private bool IsPumpable()
	{
		ElementConsumer[] components = base.GetComponents<ElementConsumer>();
		int num = Grid.PosToCell(base.transform.GetPosition());
		bool flag = false;
		foreach (ElementConsumer elementConsumer in components)
		{
			for (int j = 0; j < (int)elementConsumer.consumptionRadius; j++)
			{
				for (int k = 0; k < (int)elementConsumer.consumptionRadius; k++)
				{
					int num2 = num + k + Grid.WidthInCells * j;
					bool flag2 = Grid.Element[num2].IsState(Element.State.Gas);
					bool flag3 = Grid.Element[num2].id == elementConsumer.elementToConsume;
					if (flag2 && flag3)
					{
						flag = true;
					}
				}
			}
		}
		return flag;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ChoreType choreType = Db.Get().ChoreTypes.Get(this.choreTypeID);
		this.filteredStorage = new FilteredStorage(this, null, null, false, choreType);
	}

	private List<GameObject> GetPossibleMaterials()
	{
		List<GameObject> list = new List<GameObject>();
		this.materialStorage.Find(this.materialTag, list);
		return list;
	}

	private float GetTotalMaterialAmount()
	{
		return this.materialStorage.GetMassAvailable(this.materialTag);
	}

	private float GetTotalOxygenAmount()
	{
		return this.oxygenStorage.GetMassAvailable(this.oxygenTag);
	}

	private void RefreshMeters()
	{
		float num = this.GetTotalMaterialAmount();
		num = Mathf.Clamp01(num / ((float)this.maxUses * this.materialConsumedPerMask));
		float num2 = this.GetTotalOxygenAmount();
		num2 = Mathf.Clamp01(num2 / ((float)this.maxUses * this.oxygenConsumedPerMask));
		this.materialsMeter.SetPositionPercent(num);
		this.oxygenMeter.SetPositionPercent(num2);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.CreateNewReactable();
		this.cell = Grid.PosToCell(this);
		Grid.RegisterSuitMarker(this.cell);
		this.isOperational = base.GetComponent<Operational>().IsOperational;
		base.Subscribe<MaskStation>(-592767678, MaskStation.OnOperationalChangedDelegate);
		this.isRotated = base.GetComponent<Rotatable>().IsRotated;
		base.Subscribe<MaskStation>(-1643076535, MaskStation.OnRotatedDelegate);
		this.materialsMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_resources_target", "meter_resources", this.materialsMeterOffset, Grid.SceneLayer.BuildingBack, new string[] { "meter_resources_target" });
		this.oxygenMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_oxygen_target", "meter_oxygen", this.oxygenMeterOffset, Grid.SceneLayer.BuildingFront, new string[] { "meter_oxygen_target" });
		if (this.filteredStorage != null)
		{
			this.filteredStorage.FilterChanged();
		}
		base.Subscribe<MaskStation>(-1697596308, MaskStation.OnStorageChangeDelegate);
		this.RefreshMeters();
	}

	private void Update()
	{
		float num = this.GetTotalMaterialAmount() / this.materialConsumedPerMask;
		float num2 = this.GetTotalOxygenAmount() / this.oxygenConsumedPerMask;
		int num3 = (int)Mathf.Min(num, num2);
		int num4 = 0;
		Grid.UpdateSuitMarker(this.cell, num3, num4, this.gridFlags, this.PathFlag);
	}

	protected override void OnCleanUp()
	{
		if (this.filteredStorage != null)
		{
			this.filteredStorage.CleanUp();
		}
		if (base.isSpawned)
		{
			Grid.UnregisterSuitMarker(this.cell);
		}
		if (this.reactable != null)
		{
			this.reactable.Cleanup();
		}
		base.OnCleanUp();
	}

	private void OnOperationalChanged(bool isOperational)
	{
		this.isOperational = isOperational;
	}

	private void OnStorageChange(object data)
	{
		this.RefreshMeters();
	}

	private void UpdateGridFlag(Grid.SuitMarker.Flags flag, bool state)
	{
		if (state)
		{
			this.gridFlags |= flag;
			return;
		}
		this.gridFlags &= ~flag;
	}

	private void CreateNewReactable()
	{
		this.reactable = new MaskStation.OxygenMaskReactable(this);
	}

	private static readonly EventSystem.IntraObjectHandler<MaskStation> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<MaskStation>(delegate(MaskStation component, object data)
	{
		component.OnStorageChange(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MaskStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<MaskStation>(delegate(MaskStation component, object data)
	{
		component.OnOperationalChanged(((Boxed<bool>)data).value);
	});

	private static readonly EventSystem.IntraObjectHandler<MaskStation> OnRotatedDelegate = new EventSystem.IntraObjectHandler<MaskStation>(delegate(MaskStation component, object data)
	{
		component.isRotated = ((Rotatable)data).IsRotated;
	});

	public float materialConsumedPerMask = 1f;

	public float oxygenConsumedPerMask = 1f;

	public Tag materialTag = GameTags.Metal;

	public Tag oxygenTag = GameTags.Breathable;

	public int maxUses = 10;

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	public Storage materialStorage;

	public Storage oxygenStorage;

	private bool shouldPump;

	private MaskStation.OxygenMaskReactable reactable;

	private MeterController materialsMeter;

	private MeterController oxygenMeter;

	public Meter.Offset materialsMeterOffset = Meter.Offset.Behind;

	public Meter.Offset oxygenMeterOffset;

	public string choreTypeID;

	protected FilteredStorage filteredStorage;

	public KAnimFile interactAnim = Assets.GetAnim("anim_equip_clothing_kanim");

	private int cell;

	public PathFinder.PotentialPath.Flags PathFlag;

	private Guid noElementStatusGuid;

	private Grid.SuitMarker.Flags gridFlags;

	private class OxygenMaskReactable : Reactable
	{
		public OxygenMaskReactable(MaskStation mask_station)
			: base(mask_station.gameObject, "OxygenMask", Db.Get().ChoreTypes.SuitMarker, 1, 1, false, 0f, 0f, float.PositiveInfinity, 0f, ObjectLayer.NumLayers)
		{
			this.maskStation = mask_station;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (this.reactor != null)
			{
				return false;
			}
			if (this.maskStation == null)
			{
				base.Cleanup();
				return false;
			}
			bool flag = !new_reactor.GetComponent<MinionIdentity>().GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Suit);
			int x = transition.navGridTransition.x;
			if (x == 0)
			{
				return false;
			}
			if (!flag)
			{
				return (x >= 0 || !this.maskStation.isRotated) && (x <= 0 || this.maskStation.isRotated);
			}
			return this.maskStation.smi.IsReady() && (x <= 0 || !this.maskStation.isRotated) && (x >= 0 || this.maskStation.isRotated);
		}

		protected override void InternalBegin()
		{
			this.startTime = Time.time;
			KBatchedAnimController component = this.reactor.GetComponent<KBatchedAnimController>();
			component.AddAnimOverrides(this.maskStation.interactAnim, 1f);
			component.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("working_loop", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			this.maskStation.CreateNewReactable();
		}

		public override void Update(float dt)
		{
			Facing facing = (this.reactor ? this.reactor.GetComponent<Facing>() : null);
			if (facing && this.maskStation)
			{
				facing.SetFacing(this.maskStation.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH);
			}
			if (Time.time - this.startTime > 2.8f)
			{
				this.Run();
				base.Cleanup();
			}
		}

		private void Run()
		{
			GameObject reactor = this.reactor;
			Equipment equipment = reactor.GetComponent<MinionIdentity>().GetEquipment();
			bool flag = !equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit);
			Navigator component = reactor.GetComponent<Navigator>();
			bool flag2 = component != null && (component.flags & this.maskStation.PathFlag) > PathFinder.PotentialPath.Flags.None;
			if (flag)
			{
				if (!this.maskStation.smi.IsReady())
				{
					return;
				}
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("Oxygen_Mask".ToTag()), null, null);
				gameObject.SetActive(true);
				SimHashes elementID = this.maskStation.GetPossibleMaterials()[0].GetComponent<PrimaryElement>().ElementID;
				gameObject.GetComponent<PrimaryElement>().SetElement(elementID, false);
				SuitTank component2 = gameObject.GetComponent<SuitTank>();
				this.maskStation.materialStorage.ConsumeIgnoringDisease(this.maskStation.materialTag, this.maskStation.materialConsumedPerMask);
				this.maskStation.oxygenStorage.Transfer(component2.storage, component2.elementTag, this.maskStation.oxygenConsumedPerMask, false, true);
				Equippable component3 = gameObject.GetComponent<Equippable>();
				component3.Assign(equipment.GetComponent<IAssignableIdentity>());
				component3.isEquipped = true;
			}
			if (!flag)
			{
				Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
				assignable.Unassign();
				if (!flag2)
				{
					Notification notification = new Notification(MISC.NOTIFICATIONS.SUIT_DROPPED.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.SUIT_DROPPED.TOOLTIP, null, true, 0f, null, null, null, true, false, false);
					assignable.GetComponent<Notifier>().Add(notification, "");
				}
			}
		}

		protected override void InternalEnd()
		{
			if (this.reactor != null)
			{
				this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(this.maskStation.interactAnim);
			}
		}

		protected override void InternalCleanup()
		{
		}

		private MaskStation maskStation;

		private float startTime;
	}

	public class SMInstance : GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.GameInstance
	{
		public SMInstance(MaskStation master)
			: base(master)
		{
		}

		private bool HasSufficientMaterials()
		{
			return base.master.GetTotalMaterialAmount() >= base.master.materialConsumedPerMask;
		}

		private bool HasSufficientOxygen()
		{
			return base.master.GetTotalOxygenAmount() >= base.master.oxygenConsumedPerMask;
		}

		public bool OxygenIsFull()
		{
			return base.master.GetTotalOxygenAmount() >= base.master.oxygenConsumedPerMask * (float)base.master.maxUses;
		}

		public bool IsReady()
		{
			return this.HasSufficientMaterials() && this.HasSufficientOxygen();
		}
	}

	public class States : GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.notOperational;
			this.notOperational.PlayAnim("off").TagTransition(GameTags.Operational, this.charging, false);
			this.charging.TagTransition(GameTags.Operational, this.notOperational, true).EventTransition(GameHashes.OnStorageChange, this.notCharging, (MaskStation.SMInstance smi) => smi.OxygenIsFull() || !smi.master.shouldPump).Update(delegate(MaskStation.SMInstance smi, float dt)
			{
				smi.master.UpdateOperational();
			}, UpdateRate.SIM_1000ms, false)
				.Enter(delegate(MaskStation.SMInstance smi)
				{
					if (smi.OxygenIsFull() || !smi.master.shouldPump)
					{
						smi.GoTo(this.notCharging);
						return;
					}
					if (smi.IsReady())
					{
						smi.GoTo(this.charging.openChargingPre);
						return;
					}
					smi.GoTo(this.charging.closedChargingPre);
				});
			this.charging.opening.QueueAnim("opening_charging", false, null).OnAnimQueueComplete(this.charging.open);
			this.charging.open.PlayAnim("open_charging_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OnStorageChange, this.charging.closing, (MaskStation.SMInstance smi) => !smi.IsReady());
			this.charging.closing.QueueAnim("closing_charging", false, null).OnAnimQueueComplete(this.charging.closed);
			this.charging.closed.PlayAnim("closed_charging_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OnStorageChange, this.charging.opening, (MaskStation.SMInstance smi) => smi.IsReady());
			this.charging.openChargingPre.PlayAnim("open_charging_pre").OnAnimQueueComplete(this.charging.open);
			this.charging.closedChargingPre.PlayAnim("closed_charging_pre").OnAnimQueueComplete(this.charging.closed);
			this.notCharging.TagTransition(GameTags.Operational, this.notOperational, true).EventTransition(GameHashes.OnStorageChange, this.charging, (MaskStation.SMInstance smi) => !smi.OxygenIsFull() && smi.master.shouldPump).Update(delegate(MaskStation.SMInstance smi, float dt)
			{
				smi.master.UpdateOperational();
			}, UpdateRate.SIM_1000ms, false)
				.Enter(delegate(MaskStation.SMInstance smi)
				{
					if (!smi.OxygenIsFull() && smi.master.shouldPump)
					{
						smi.GoTo(this.charging);
						return;
					}
					if (smi.IsReady())
					{
						smi.GoTo(this.notCharging.openChargingPst);
						return;
					}
					smi.GoTo(this.notCharging.closedChargingPst);
				});
			this.notCharging.opening.PlayAnim("opening_not_charging").OnAnimQueueComplete(this.notCharging.open);
			this.notCharging.open.PlayAnim("open_not_charging_loop").EventTransition(GameHashes.OnStorageChange, this.notCharging.closing, (MaskStation.SMInstance smi) => !smi.IsReady());
			this.notCharging.closing.PlayAnim("closing_not_charging").OnAnimQueueComplete(this.notCharging.closed);
			this.notCharging.closed.PlayAnim("closed_not_charging_loop").EventTransition(GameHashes.OnStorageChange, this.notCharging.opening, (MaskStation.SMInstance smi) => smi.IsReady());
			this.notCharging.openChargingPst.PlayAnim("open_charging_pst").OnAnimQueueComplete(this.notCharging.open);
			this.notCharging.closedChargingPst.PlayAnim("closed_charging_pst").OnAnimQueueComplete(this.notCharging.closed);
		}

		public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State notOperational;

		public MaskStation.States.ChargingStates charging;

		public MaskStation.States.NotChargingStates notCharging;

		public class ChargingStates : GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State
		{
			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State opening;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State open;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closing;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closed;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State openChargingPre;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closedChargingPre;
		}

		public class NotChargingStates : GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State
		{
			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State opening;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State open;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closing;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closed;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State openChargingPst;

			public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closedChargingPst;
		}
	}
}
