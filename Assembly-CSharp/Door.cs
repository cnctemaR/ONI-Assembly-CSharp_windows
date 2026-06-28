using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Door : BuildingWorkable, ISaveLoadableJson
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

	public Door.ControlState CurrentControlState
	{
		get
		{
			return this.controlState;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_remote") };
	}

	private Door.ControlState GetNextState(Door.ControlState wantedState)
	{
		return (wantedState + 1) % Door.ControlState.NumStates;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.controller = new Door.Controller.Instance(this);
		this.controller.StartSM();
		this.Subscribe(-592767678, new EventSystem.EventHandler(this.OnOperationalChanged));
		this.Subscribe(824508782, new EventSystem.EventHandler(this.OnOperationalChanged));
		Components.Doors.Add(this);
		this.nextState = this.GetNextState(this.controlState);
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		Game.Instance.roomProber.AddDoor(this);
		this.open = this.controlState == Door.ControlState.Opened;
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
		Components.Doors.Remove(this);
		this.UpdateDoorState(true);
		List<int> list = new List<int>();
		foreach (int num in this.building.PlacementCells)
		{
			SimMessages.ClearCellProperties(num, 4);
			Grid.RenderedByWorld[num] = Grid.Element[num].substance.renderedByWorld;
			Grid.FakeFloor[num] = false;
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
			Game.Instance.SetForceField(num3, false, Grid.Solid[num3]);
		}
	}

	private void RefreshControlState()
	{
		switch (this.controlState)
		{
		case Door.ControlState.Auto:
			this.operational.SetActive(false, true);
			break;
		case Door.ControlState.Opened:
			this.operational.SetActive(true, true);
			break;
		case Door.ControlState.Closed:
			this.operational.SetActive(false, true);
			break;
		}
		this.SetWorldState(this.open);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CurrentDoorControlState, this);
	}

	private void OnOperationalChanged(object data)
	{
		if (this.consumer != null)
		{
			this.animController.PlaySpeedMultiplier = ((!this.consumer.IsPowered) ? 0.25f : 1f);
		}
		bool isOperational = this.operational.IsOperational;
		bool flag = false;
		switch (this.controlState)
		{
		case Door.ControlState.Auto:
			flag = this.openCount > 0;
			break;
		case Door.ControlState.Opened:
			flag = true;
			break;
		case Door.ControlState.Closed:
			flag = false;
			break;
		}
		if (isOperational != this.on)
		{
			this.on = isOperational;
			if (isOperational)
			{
				this.animController.PlaySpeedMultiplier = 1f;
				if (this.controlState == Door.ControlState.Auto)
				{
					this.operational.SetActive(false, false);
					flag = false;
				}
			}
			else
			{
				this.animController.PlaySpeedMultiplier = this.unpoweredAnimSpeed;
			}
		}
		if (flag != this.open)
		{
			this.open = flag;
		}
	}

	private void SetWorldState(bool is_open)
	{
		foreach (int num in this.building.PlacementCells)
		{
			switch (this.doorType)
			{
			case Door.DoorType.Pressure:
			case Door.DoorType.ManualPressure:
				if (is_open)
				{
					Game.Instance.SetForceField(num, true, false);
					SimMessages.ReplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.DoorOpen, 0f, -1f, -1);
				}
				else
				{
					Game.Instance.SetForceField(num, this.controlState == Door.ControlState.Auto, true);
					SimMessages.ReplaceElement(num, SimHashes.SteelDoor, CellEventLogger.Instance.DoorClose, 400f, base.GetComponent<PrimaryElement>().Temperature, -1);
				}
				World.Instance.groundRenderer.MarkDirty(num);
				break;
			}
			Grid.RenderedByWorld[num] = false;
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

	private void QueueNextState()
	{
		this.requestedState = this.nextState;
		this.nextState = this.GetNextState(this.requestedState);
		this.userMenu.Refresh();
		if (this.requestedState == this.controlState)
		{
			if (this.changeStateChore != null)
			{
				this.changeStateChore.Cancel("Change state");
				this.changeStateChore = null;
				base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState);
			}
			return;
		}
		if (DebugHandler.InstantBuildMode)
		{
			this.controlState = this.requestedState;
			this.RefreshControlState();
			this.OnOperationalChanged(null);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState);
		}
		else
		{
			if (this.changeStateChore != null)
			{
				this.changeStateChore.Cancel("Change state");
			}
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, this);
			this.changeStateChore = new WorkChore<Door>(Db.Get().ChoreTypes.Toggle, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.changeStateChore = null;
		this.controlState = this.requestedState;
		this.RefreshControlState();
		this.OnOperationalChanged(null);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState);
		this.Open();
		this.Close();
	}

	private void OnRefreshUserMenu(object data)
	{
		this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo(Door.controlStateIcon, Strings.Get(Door.controlStateNames[(int)this.nextState]), new global::System.Action(this.QueueNextState), global::Action.ToggleOpen, null, null, null, null, string.Empty));
	}

	public float Open()
	{
		this.openCount++;
		float num = 1f;
		if (this.consumer != null)
		{
			num = ((!this.consumer.IsPowered) ? 0.5f : 1f);
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
			this.open = true;
			break;
		}
		return num;
	}

	public void Close()
	{
		this.openCount = Mathf.Max(0, this.openCount - 1);
		switch (this.controlState)
		{
		case Door.ControlState.Auto:
			if (this.openCount == 0)
			{
				this.collisionCollider.enabled = true;
				this.operational.SetActive(false, false);
				this.controller.sm.isOpen.Set(false, this.controller);
				this.userMenu.Refresh();
				this.open = false;
			}
			break;
		case Door.ControlState.Closed:
			this.collisionCollider.enabled = true;
			this.controller.sm.isOpen.Set(false, this.controller);
			this.open = false;
			break;
		}
	}

	public override string[] GetWorkAnims(Worker worker)
	{
		return this.WorkAnims;
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

	private BoxCollider2D collisionCollider;

	private BoxCollider2D selectionCollider;

	[SerializeField]
	public bool hasComplexUserControls;

	[SerializeField]
	public float unpoweredAnimSpeed = 0.25f;

	[Serialize]
	private Door.ControlState controlState;

	private Door.ControlState nextState;

	private Door.ControlState requestedState;

	private Chore changeStateChore;

	[SerializeField]
	public Door.DoorType doorType;

	private static string[] controlStateNames = new string[] { "STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE.AUTO", "STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE.OPEN", "STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE.CLOSE" };

	private static string controlStateIcon = "status_item_change_door_control_state";

	private bool on = true;

	private bool open = true;

	private int openCount;

	public enum DoorType
	{
		Pressure,
		ManualPressure,
		Internal
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
			default_state = this.closed;
			this.root.ToggleSchedulePeriodic("RefreshIsBlocked", 0.25f, delegate(Door.Controller.Instance smi)
			{
				smi.RefreshIsBlocked();
			});
			this.closeblocked.PlayAnim("open", KAnim.PlayMode.Once, null).ParamTransition<bool>(this.isOpen, this.open, (Door.Controller.Instance smi, bool p) => p).ParamTransition<bool>(this.isBlocked, this.closedelay, (Door.Controller.Instance smi, bool p) => !p);
			this.closedelay.PlayAnim("open", KAnim.PlayMode.Once, null).ScheduleGoTo(0.5f, this.closing).ParamTransition<bool>(this.isOpen, this.open, (Door.Controller.Instance smi, bool p) => p)
				.ParamTransition<bool>(this.isBlocked, this.closeblocked, (Door.Controller.Instance smi, bool p) => p);
			this.closing.PlayAnim("closing", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.closed).ParamTransition<bool>(this.isBlocked, this.closeblocked, (Door.Controller.Instance smi, bool p) => p);
			this.open.PlayAnim("open", KAnim.PlayMode.Once, null).ParamTransition<bool>(this.isOpen, this.closeblocked, (Door.Controller.Instance smi, bool p) => !p);
			this.closed.PlayAnim("closed", KAnim.PlayMode.Once, null).ParamTransition<bool>(this.isOpen, this.opening, (Door.Controller.Instance smi, bool p) => p).Enter("SetWorldStateClosed", delegate(Door.Controller.Instance smi)
			{
				smi.master.SetWorldState(false);
			})
				.Exit("SetWorldStateOpen", delegate(Door.Controller.Instance smi)
				{
					smi.master.SetWorldState(true);
				});
			this.opening.PlayAnim("opening", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.open);
		}

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door>.State open;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door>.State opening;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door>.State closed;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door>.State closing;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door>.State closedelay;

		public GameStateMachine<Door.Controller, Door.Controller.Instance, Door>.State closeblocked;

		public StateMachine<Door.Controller, Door.Controller.Instance, Door>.BoolParameter isOpen;

		public StateMachine<Door.Controller, Door.Controller.Instance, Door>.BoolParameter isBlocked;

		public new class Instance : GameStateMachine<Door.Controller, Door.Controller.Instance, Door>.GameInstance
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
					if (Grid.Objects[num, 1] != null)
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
