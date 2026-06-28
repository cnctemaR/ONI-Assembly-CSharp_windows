using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Door : Workable, ISaveLoadable
{
	public Door()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	public Door.ControlState CurrentState
	{
		get
		{
			return this.controlState;
		}
	}

	public Door.ControlState RequestedState
	{
		get
		{
			return this.requestedState;
		}
	}

	public bool IsRotated
	{
		get
		{
			return this.rotatable.IsRotated;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = Door.OVERRIDE_ANIMS;
	}

	private Door.ControlState GetNextState(Door.ControlState wantedState)
	{
		return (wantedState + 1) % Door.ControlState.NumStates;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KPrefabID component = base.GetComponent<KPrefabID>();
		if (component != null)
		{
			this.log = new LoggerFSS("Door");
		}
		this.controller = new Door.Controller.Instance(this);
		this.controller.StartSM();
		if (this.doorType == Door.DoorType.Sealed && !this.hasBeenUnsealed)
		{
			this.Seal();
		}
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.Subscribe(824508782, new Action<object>(this.OnOperationalChanged));
		StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
		HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
		structureTemperatures.Disable(handle);
		Game.Instance.roomProber.AddDoor(this);
		this.requestedState = this.CurrentState;
		this.RefreshControlState();
		this.OnOperationalChanged(null);
		this.collisionCollider = base.GetComponent<BoxCollider2D>();
		this.selectionCollider = new GameObject("selection")
		{
			transform = 
			{
				parent = this.transform,
				localPosition = Vector3.zero
			}
		}.AddComponent<BoxCollider2D>();
		this.selectionCollider.size = new Vector2(1f, 2f);
		if (this.controlState == Door.ControlState.Opened)
		{
			this.controller.sm.isOpen.Set(true, this.controller);
		}
		if (this.rotatable.IsRotated)
		{
			Vector2 vector = new Vector2(2f, 1f);
			Vector2 vector2 = new Vector2(0.5f, 0.5f);
			this.collisionCollider.size = vector;
			this.collisionCollider.offset = vector2;
			this.selectionCollider.size = vector;
			this.selectionCollider.offset = vector2;
			foreach (int num in this.building.PlacementCells)
			{
				Grid.FakeFloor[num] = true;
				Pathfinding.Instance.AddDirtyNavGridCell(num);
			}
		}
		else
		{
			this.selectionCollider.offset = new Vector2(0f, 1f);
		}
		List<int> list = new List<int>();
		foreach (int num2 in this.building.PlacementCells)
		{
			Grid.HasDoor[num2] = true;
			Grid.HasAccessDoor[num2] = base.GetComponent<AccessControl>() != null;
			if (this.rotatable.IsRotated)
			{
				list.Add(Grid.CellAbove(num2));
				list.Add(Grid.CellBelow(num2));
			}
			else
			{
				list.Add(Grid.CellLeft(num2));
				list.Add(Grid.CellRight(num2));
			}
			if (Grid.Solid[num2] && Grid.Element[num2].id != SimHashes.SteelDoor)
			{
				SimMessages.Dig(num2, -1);
			}
			SimMessages.SetCellProperties(num2, 4);
		}
		List<int> list2 = new List<int>(this.building.PlacementCells);
		foreach (int num3 in list)
		{
			if (!list2.Contains(num3))
			{
				Region intersectionRegion = Game.Instance.RegionManager.GetIntersectionRegion(num3);
				if (!(intersectionRegion == null))
				{
					intersectionRegion.AddDoor(this);
				}
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.UpdateDoorState(true);
		List<int> list = new List<int>();
		foreach (int num in this.building.PlacementCells)
		{
			SimMessages.ClearCellProperties(num, 4);
			Grid.RenderedByWorld[num] = Grid.Element[num].substance.renderedByWorld;
			Grid.FakeFloor[num] = false;
			SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.DoorOpen, 0f, -1f, byte.MaxValue, 0, -1);
			Pathfinding.Instance.AddDirtyNavGridCell(num);
			if (this.rotatable.IsRotated)
			{
				list.Add(Grid.CellAbove(num));
				list.Add(Grid.CellBelow(num));
			}
			else
			{
				list.Add(Grid.CellLeft(num));
				list.Add(Grid.CellRight(num));
			}
		}
		List<int> list2 = new List<int>(this.building.PlacementCells);
		foreach (int num2 in list)
		{
			if (!list2.Contains(num2))
			{
				Region intersectionRegion = Game.Instance.RegionManager.GetIntersectionRegion(num2);
				if (!(intersectionRegion == null))
				{
					intersectionRegion.RemoveDoor(this);
				}
			}
		}
		Game.Instance.roomProber.RemoveDoor(this);
		foreach (int num3 in this.building.PlacementCells)
		{
			Grid.HasDoor[num3] = false;
			Grid.HasAccessDoor[num3] = false;
			Game.Instance.SetForceField(num3, false, Grid.Solid[num3]);
			Grid.Impassable[num3] = false;
			Pathfinding.Instance.AddDirtyNavGridCell(num3);
		}
	}

	public bool isSealed
	{
		get
		{
			return this.controller.sm.isSealed.Get(this.controller);
		}
	}

	public void Seal()
	{
		this.controller.sm.isSealed.Set(true, this.controller);
	}

	public void OrderUnseal()
	{
		this.controller.GoTo(this.controller.sm.Sealed.awaiting_unlock);
	}

	private void RefreshControlState()
	{
		switch (this.controlState)
		{
		case Door.ControlState.Auto:
			this.operational.SetActive(false, true);
			this.controller.sm.isLocked.Set(false, this.controller);
			break;
		case Door.ControlState.Opened:
			this.operational.SetActive(true, true);
			this.controller.sm.isLocked.Set(false, this.controller);
			break;
		case Door.ControlState.Closed:
			this.operational.SetActive(false, true);
			this.controller.sm.isLocked.Set(true, this.controller);
			break;
		}
		this.SetWorldState();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CurrentDoorControlState, this);
	}

	private void OnOperationalChanged(object data)
	{
		bool isOperational = this.operational.IsOperational;
		if (isOperational != this.on)
		{
			this.on = isOperational;
			if (isOperational)
			{
				this.animController.PlaySpeedMultiplier = 1f;
				if (this.controlState == Door.ControlState.Auto)
				{
					this.operational.SetActive(false, false);
				}
			}
			else
			{
				this.animController.PlaySpeedMultiplier = this.unpoweredAnimSpeed;
			}
		}
	}

	private void SetWorldState()
	{
		int[] placementCells = this.building.PlacementCells;
		bool flag = this.IsOpen();
		bool flag2 = flag || this.controlState == Door.ControlState.Auto;
		bool flag3 = !flag;
		float num = 0f;
		foreach (int num2 in placementCells)
		{
			num += Grid.Temperature[num2];
			switch (this.doorType)
			{
			case Door.DoorType.Pressure:
			case Door.DoorType.ManualPressure:
			case Door.DoorType.Sealed:
			{
				Game.Instance.SetForceField(num2, flag2, flag3);
				World.Instance.groundRenderer.MarkDirty(num2);
				HandleVector<Game.CallbackInfo>.Handle invalidHandle = HandleVector<Game.CallbackInfo>.InvalidHandle;
				if (flag)
				{
					int num3 = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new global::System.Action(this.OnSimDoorOpened), false)).index;
					SimMessages.ReplaceElement(num2, SimHashes.Vacuum, CellEventLogger.Instance.DoorOpen, 0f, -1f, byte.MaxValue, 0, num3);
				}
				else
				{
					PrimaryElement component = base.GetComponent<PrimaryElement>();
					int num3 = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new global::System.Action(this.OnSimDoorClosed), false)).index;
					SimMessages.ReplaceAndDisplaceElement(num2, SimHashes.SteelDoor, CellEventLogger.Instance.DoorClose, 400f, component.Temperature, byte.MaxValue, 0, num3);
				}
				break;
			}
			case Door.DoorType.Internal:
				Grid.Impassable[num2] = this.controlState != Door.ControlState.Opened;
				Game.Instance.SetForceField(num2, this.controlState != Door.ControlState.Closed, false);
				Pathfinding.Instance.AddDirtyNavGridCell(num2);
				break;
			}
			Grid.RenderedByWorld[num2] = false;
		}
	}

	private void UpdateDoorState(bool cleaningUp)
	{
		foreach (int num in this.building.PlacementCells)
		{
			if (Grid.IsValidCell(num))
			{
				Grid.Foundation[num] = !cleaningUp;
			}
		}
	}

	public void QueueStateChange(Door.ControlState nextState)
	{
		if (this.requestedState != nextState)
		{
			this.requestedState = nextState;
		}
		else
		{
			this.requestedState = this.controlState;
		}
		if (this.requestedState == this.controlState)
		{
			if (this.changeStateChore != null)
			{
				this.changeStateChore.Cancel("Change state");
				this.changeStateChore = null;
				base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, false);
			}
			return;
		}
		if (DebugHandler.InstantBuildMode)
		{
			this.controlState = this.requestedState;
			this.RefreshControlState();
			this.OnOperationalChanged(null);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, false);
		}
		else
		{
			if (this.changeStateChore != null)
			{
				this.changeStateChore.Cancel("Change state");
			}
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, this);
			this.changeStateChore = new WorkChore<Door>(Db.Get().ChoreTypes.Toggle, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true, true);
		}
	}

	private void OnSimDoorOpened()
	{
		if (this == null)
		{
			return;
		}
		StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
		HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
		structureTemperatures.Enable(handle);
	}

	private void OnSimDoorClosed()
	{
		if (this == null)
		{
			return;
		}
		StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
		HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
		structureTemperatures.Disable(handle);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.changeStateChore = null;
		this.controlState = this.requestedState;
		this.RefreshControlState();
		this.OnOperationalChanged(null);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, false);
		this.Trigger(1734268753, this);
		this.Open();
		this.Close();
	}

	public float Open()
	{
		if (this.openCount == 0)
		{
			StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
			HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
			if (handle.IsValid() && !structureTemperatures.IsEnabled(handle))
			{
				int[] placementCells = this.building.PlacementCells;
				float num = 0f;
				foreach (int num2 in placementCells)
				{
					num += Grid.Temperature[num2];
				}
				num /= (float)placementCells.Length;
				PrimaryElement component = base.GetComponent<PrimaryElement>();
				component.Temperature = num;
			}
		}
		this.openCount++;
		float num3 = 1f;
		if (this.consumer != null)
		{
			num3 = ((!this.consumer.IsPowered) ? 0.5f : 1f);
		}
		switch (this.controlState)
		{
		case Door.ControlState.Auto:
		case Door.ControlState.Opened:
			this.collisionCollider.enabled = false;
			this.controller.sm.isOpen.Set(true, this.controller);
			if (this.operational.IsOperational)
			{
				this.operational.SetActive(true, false);
			}
			break;
		}
		return num3;
	}

	public void Close()
	{
		this.openCount = Mathf.Max(0, this.openCount - 1);
		if (this.openCount == 0)
		{
			StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
			HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			if (handle.IsValid() && structureTemperatures.IsEnabled(handle))
			{
				float temperature = structureTemperatures.GetData(handle).Temperature;
				component.Temperature = temperature;
			}
		}
		switch (this.controlState)
		{
		case Door.ControlState.Auto:
			if (this.openCount == 0)
			{
				this.collisionCollider.enabled = true;
				this.operational.SetActive(false, false);
				this.controller.sm.isOpen.Set(false, this.controller);
				this.userMenu.Refresh();
			}
			break;
		case Door.ControlState.Closed:
			this.collisionCollider.enabled = true;
			this.controller.sm.isOpen.Set(false, this.controller);
			break;
		}
	}

	public bool IsOpen()
	{
		return this.controller.IsInsideState(this.controller.sm.open) || this.controller.IsInsideState(this.controller.sm.closedelay) || this.controller.IsInsideState(this.controller.sm.closeblocked);
	}

	[MyCmpReq]
	private UserMenu userMenu;

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpReq]
	private KBatchedAnimController animController;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private EnergyConsumer consumer;

	private Door.Controller.Instance controller;

	private LoggerFSS log;

	private BoxCollider2D collisionCollider;

	private BoxCollider2D selectionCollider;

	[SerializeField]
	public bool hasComplexUserControls;

	[SerializeField]
	public float unpoweredAnimSpeed = 0.25f;

	[Serialize]
	private bool hasBeenUnsealed;

	[Serialize]
	private Door.ControlState controlState;

	private Door.ControlState requestedState;

	private Chore changeStateChore;

	[SerializeField]
	public Door.DoorType doorType;

	private static readonly KAnimFile[] OVERRIDE_ANIMS = new KAnimFile[] { Assets.GetAnim("anim_use_remote_kanim") };

	private bool on = true;

	private int openCount;

	public enum DoorType
	{
		Pressure,
		ManualPressure,
		Internal,
		Sealed
	}

	public enum ControlState
	{
		Auto,
		Opened,
		Closed,
		NumStates
	}

	public class Controller : GameStateMachine<Door.Controller, Door.Controller.Instance, Door>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = true;
			default_state = this.closed;
			this.root.ToggleSchedulePeriodic("RefreshIsBlocked", 0.25f, delegate(Door.Controller.Instance smi)
			{
				smi.RefreshIsBlocked();
			}).ParamTransition<bool>(this.isSealed, this.Sealed.closed, (Door.Controller.Instance smi, bool p) => p);
			this.closeblocked.PlayAnim("open", KAnim.PlayMode.Once, null).ParamTransition<bool>(this.isOpen, this.open, (Door.Controller.Instance smi, bool p) => p).ParamTransition<bool>(this.isBlocked, this.closedelay, (Door.Controller.Instance smi, bool p) => !p);
			this.closedelay.PlayAnim("open", KAnim.PlayMode.Once, null).ScheduleGoTo(0.5f, this.closing).ParamTransition<bool>(this.isOpen, this.open, (Door.Controller.Instance smi, bool p) => p)
				.ParamTransition<bool>(this.isBlocked, this.closeblocked, (Door.Controller.Instance smi, bool p) => p);
			this.closing.PlayAnim("closing", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.closed).ParamTransition<bool>(this.isBlocked, this.closeblocked, (Door.Controller.Instance smi, bool p) => p);
			this.open.PlayAnim("open", KAnim.PlayMode.Once, null).ParamTransition<bool>(this.isOpen, this.closeblocked, (Door.Controller.Instance smi, bool p) => !p).Enter("SetWorldStateOpen", delegate(Door.Controller.Instance smi)
			{
				smi.master.SetWorldState();
			});
			this.closed.PlayAnim("closed", KAnim.PlayMode.Once, null).ParamTransition<bool>(this.isOpen, this.opening, (Door.Controller.Instance smi, bool p) => p).ParamTransition<bool>(this.isLocked, this.locking, (Door.Controller.Instance smi, bool p) => p)
				.Enter("SetWorldStateClosed", delegate(Door.Controller.Instance smi)
				{
					smi.master.SetWorldState();
				});
			this.locking.PlayAnim("locked_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.locked);
			this.locked.PlayAnim("locked", KAnim.PlayMode.Once, null).ParamTransition<bool>(this.isLocked, this.unlocking, (Door.Controller.Instance smi, bool p) => !p);
			this.unlocking.PlayAnim("locked_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.closed);
			this.opening.PlayAnim("opening", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.open);
			this.Sealed.Enter(delegate(Door.Controller.Instance smi)
			{
				OccupyArea component = smi.master.GetComponent<OccupyArea>();
				for (int i = 0; i < component.OccupiedCellsOffsets.Length; i++)
				{
					Grid.PreventFogOfWarReveal[Grid.OffsetCell(Grid.PosToCell(smi.master.gameObject), component.OccupiedCellsOffsets[i])] = false;
				}
				smi.sm.isLocked.Set(true, smi);
				smi.master.controlState = Door.ControlState.Closed;
				smi.master.RefreshControlState();
				if (smi.master.GetComponent<Unsealable>().facingRight)
				{
					KBatchedAnimController component2 = smi.master.GetComponent<KBatchedAnimController>();
					component2.FlipX = true;
				}
			}).Enter("SetWorldStateClosed", delegate(Door.Controller.Instance smi)
			{
				smi.master.SetWorldState();
			}).Exit(delegate(Door.Controller.Instance smi)
			{
				smi.sm.isLocked.Set(false, smi);
				smi.master.GetComponent<AccessControl>().controlEnabled = true;
				smi.master.controlState = Door.ControlState.Opened;
				smi.master.RefreshControlState();
				smi.sm.isOpen.Set(true, smi);
				smi.sm.isLocked.Set(false, smi);
				smi.sm.isSealed.Set(false, smi);
			});
			this.Sealed.closed.PlayAnim("sealed", KAnim.PlayMode.Once, null);
			this.Sealed.awaiting_unlock.ToggleChore((Door.Controller.Instance smi) => this.CreateUnsealChore(smi, true), this.Sealed.chore_pst);
			this.Sealed.chore_pst.Enter(delegate(Door.Controller.Instance smi)
			{
				smi.master.hasBeenUnsealed = true;
				if (smi.master.GetComponent<Unsealable>().unsealed)
				{
					smi.GoTo(this.opening);
				}
				else
				{
					smi.GoTo(this.Sealed.closed);
				}
			});
		}

		private Chore CreateUnsealChore(Door.Controller.Instance smi, bool approach_right)
		{
			return new WorkChore<Unsealable>(Db.Get().ChoreTypes.Toggle, smi.master, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true);
		}

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State open;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State opening;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closed;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closing;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closedelay;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closeblocked;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State locking;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State locked;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State unlocking;

		public Door.Controller.SealedStates Sealed;

		public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter isOpen;

		public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter isLocked;

		public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter isBlocked;

		public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter isSealed;

		public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter sealDirectionRight;

		public class SealedStates : GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State
		{
			public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closed;

			public Door.Controller.SealedStates.AwaitingUnlock awaiting_unlock;

			public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State chore_pst;

			public class AwaitingUnlock : GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State
			{
				public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State awaiting_arrival;

				public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State unlocking;
			}
		}

		public new class Instance : GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.GameInstance
		{
			public Instance(Door door)
				: base(door)
			{
			}

			public void RefreshIsBlocked()
			{
				bool flag = false;
				foreach (int num in base.master.GetComponent<Building>().PlacementCells)
				{
					if (Grid.Objects[num, 0] != null)
					{
						flag = true;
						break;
					}
				}
				base.sm.isBlocked.Set(flag, base.smi);
			}
		}
	}
}
