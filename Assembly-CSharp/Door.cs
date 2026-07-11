using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Door : Workable, ISaveLoadable, ISim200ms
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

	public bool isSealed
	{
		get
		{
			return this.controller.sm.isSealed.Get(this.controller);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = Door.OVERRIDE_ANIMS;
		this.doorClosingSound = GlobalAssets.GetSound(this.doorClosingSoundEventName, false);
		this.doorOpeningSound = GlobalAssets.GetSound(this.doorOpeningSoundEventName, false);
	}

	private Door.ControlState GetNextState(Door.ControlState wantedState)
	{
		return (wantedState + 1) % Door.ControlState.NumStates;
	}

	private static bool DisplacesGas(Door.DoorType type)
	{
		return type != Door.DoorType.Internal;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KPrefabID component = base.GetComponent<KPrefabID>();
		if (component != null)
		{
			this.log = new LoggerFSS("Door", 35);
		}
		if (!this.allowAutoControl && this.controlState == Door.ControlState.Auto)
		{
			this.controlState = Door.ControlState.Closed;
		}
		StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
		HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
		if (Door.DisplacesGas(this.doorType))
		{
			structureTemperatures.Disable(handle);
		}
		this.controller = new Door.Controller.Instance(this);
		this.controller.StartSM();
		if (this.doorType == Door.DoorType.Sealed && !this.hasBeenUnsealed)
		{
			this.Seal();
		}
		this.UpdateDoorSpeed(this.operational.IsOperational);
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		base.Subscribe(824508782, new Action<object>(this.OnOperationalChanged));
		base.Subscribe(-801688580, new Action<object>(this.OnLogicValueChanged));
		this.requestedState = this.CurrentState;
		this.ApplyRequestedControlState(true);
		if (this.rotatable.IsRotated)
		{
			foreach (int num in this.building.PlacementCells)
			{
				Grid.FakeFloor[num] = true;
				Pathfinding.Instance.AddDirtyNavGridCell(num);
			}
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
			SimMessages.SetCellProperties(num2, 8);
			Grid.RenderedByWorld[num2] = false;
		}
		List<int> list2 = new List<int>(this.building.PlacementCells);
		Game.Instance.roomProber.AddDoor(this, list2, list);
	}

	protected override void OnCleanUp()
	{
		this.UpdateDoorState(true);
		List<int> list = new List<int>();
		foreach (int num in this.building.PlacementCells)
		{
			SimMessages.ClearCellProperties(num, 12);
			Grid.RenderedByWorld[num] = Grid.Element[num].substance.renderedByWorld;
			Grid.FakeFloor[num] = false;
			if (Grid.Element[num].IsSolid)
			{
				SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.DoorOpen, 0f, -1f, byte.MaxValue, 0, -1);
			}
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
		foreach (int num2 in this.building.PlacementCells)
		{
			Grid.HasDoor[num2] = false;
			Grid.HasAccessDoor[num2] = false;
			Game.Instance.SetForceField(num2, false, Grid.Solid[num2]);
			Grid.Impassable[num2] = false;
			Pathfinding.Instance.AddDirtyNavGridCell(num2);
		}
		Game.Instance.roomProber.RemoveDoor(this);
		base.OnCleanUp();
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
		Door.ControlState controlState = this.controlState;
		if (controlState != Door.ControlState.Auto)
		{
			if (controlState != Door.ControlState.Opened)
			{
				if (controlState == Door.ControlState.Closed)
				{
					this.controller.sm.isLocked.Set(true, this.controller);
				}
			}
			else
			{
				this.controller.sm.isLocked.Set(false, this.controller);
			}
		}
		else
		{
			this.controller.sm.isLocked.Set(false, this.controller);
		}
		base.Trigger(279163026, this.controlState);
		this.SetWorldState();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CurrentDoorControlState, this);
	}

	private void OnOperationalChanged(object data)
	{
		bool isOperational = this.operational.IsOperational;
		if (isOperational != this.on)
		{
			this.UpdateDoorSpeed(isOperational);
			if (this.on && base.GetComponent<KPrefabID>().HasTag(GameTags.Transition))
			{
				this.SetActive(true);
			}
			else
			{
				this.SetActive(false);
			}
		}
	}

	private void UpdateDoorSpeed(bool powered)
	{
		this.on = powered;
		this.UpdateAnimAndSoundParams(powered);
		float positionPercent = this.animController.GetPositionPercent();
		this.animController.Play(this.animController.CurrentAnim.hash, this.animController.PlayMode, 1f, 0f);
		this.animController.SetPositionPercent(positionPercent);
	}

	private void UpdateAnimAndSoundParams(bool powered)
	{
		if (powered)
		{
			this.animController.PlaySpeedMultiplier = this.poweredAnimSpeed;
			if (this.doorClosingSound != null)
			{
				this.loopingSounds.UpdateFirstParameter(this.doorClosingSound, Door.SOUND_POWERED_PARAMETER, 1f);
			}
			if (this.doorOpeningSound != null)
			{
				this.loopingSounds.UpdateFirstParameter(this.doorOpeningSound, Door.SOUND_POWERED_PARAMETER, 1f);
			}
		}
		else
		{
			this.animController.PlaySpeedMultiplier = this.unpoweredAnimSpeed;
			if (this.doorClosingSound != null)
			{
				this.loopingSounds.UpdateFirstParameter(this.doorClosingSound, Door.SOUND_POWERED_PARAMETER, 0f);
			}
			if (this.doorOpeningSound != null)
			{
				this.loopingSounds.UpdateFirstParameter(this.doorOpeningSound, Door.SOUND_POWERED_PARAMETER, 0f);
			}
		}
	}

	private void SetActive(bool active)
	{
		if (this.operational.IsOperational)
		{
			this.operational.SetActive(active, false);
		}
	}

	private void SetWorldState()
	{
		int[] placementCells = this.building.PlacementCells;
		bool flag = this.IsOpen();
		this.SetForceFieldState(flag, placementCells);
		this.SetSimState(flag, placementCells);
	}

	private void SetForceFieldState(bool is_door_open, IList<int> cells)
	{
		bool flag = !is_door_open;
		bool flag2 = is_door_open || this.controlState == Door.ControlState.Auto;
		for (int i = 0; i < cells.Count; i++)
		{
			int num = cells[i];
			switch (this.doorType)
			{
			case Door.DoorType.Pressure:
			case Door.DoorType.ManualPressure:
			case Door.DoorType.Sealed:
				Game.Instance.SetForceField(num, flag2, flag);
				break;
			case Door.DoorType.Internal:
				Grid.Impassable[num] = this.controlState != Door.ControlState.Opened;
				Game.Instance.SetForceField(num, this.controlState != Door.ControlState.Closed, false);
				Pathfinding.Instance.AddDirtyNavGridCell(num);
				break;
			}
		}
	}

	private void SetSimState(bool is_door_open, IList<int> cells)
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		float num = component.Mass / (float)cells.Count;
		for (int i = 0; i < cells.Count; i++)
		{
			int num2 = cells[i];
			Door.DoorType doorType = this.doorType;
			if (doorType == Door.DoorType.Pressure || doorType == Door.DoorType.Sealed || doorType == Door.DoorType.ManualPressure)
			{
				World.Instance.groundRenderer.MarkDirty(num2);
				if (is_door_open)
				{
					if (Grid.Element[num2].IsSolid)
					{
						HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new global::System.Action(this.OnSimDoorOpened), false));
						int num3 = num2;
						SimHashes simHashes = SimHashes.Vacuum;
						CellElementEvent cellElementEvent = CellEventLogger.Instance.DoorOpen;
						float num4 = 0f;
						float num5 = -1f;
						int num6 = handle.index;
						SimMessages.ReplaceElement(num3, simHashes, cellElementEvent, num4, num5, byte.MaxValue, 0, num6);
						SimMessages.ClearCellProperties(num2, 4);
					}
					else
					{
						this.OnSimDoorOpened();
					}
				}
				else if (Grid.Element[num2].IsSolid)
				{
					this.OnSimDoorClosed();
				}
				else
				{
					HandleVector<Game.CallbackInfo>.Handle handle2 = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new global::System.Action(this.OnSimDoorClosed), false));
					int num6 = num2;
					SimHashes simHashes = component.ElementID;
					CellElementEvent cellElementEvent = CellEventLogger.Instance.DoorClose;
					float num5 = num;
					float num4 = component.Temperature;
					int num3 = handle2.index;
					SimMessages.ReplaceAndDisplaceElement(num6, simHashes, cellElementEvent, num5, num4, byte.MaxValue, 0, num3);
					SimMessages.SetCellProperties(num2, 4);
				}
			}
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
			this.Open();
			this.Close();
		}
		else
		{
			if (this.changeStateChore != null)
			{
				this.changeStateChore.Cancel("Change state");
			}
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, this);
			this.changeStateChore = new WorkChore<Door>(Db.Get().ChoreTypes.Toggle, this, null, null, true, null, null, null, true, null, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		}
	}

	private void OnSimDoorOpened()
	{
		if (this == null || !Door.DisplacesGas(this.doorType))
		{
			return;
		}
		StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
		HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
		structureTemperatures.Enable(handle);
		this.do_melt_check = false;
	}

	private void OnSimDoorClosed()
	{
		if (this == null || !Door.DisplacesGas(this.doorType))
		{
			return;
		}
		StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
		HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
		structureTemperatures.Disable(handle);
		this.do_melt_check = true;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.changeStateChore = null;
		this.ApplyRequestedControlState(false);
	}

	public float Open()
	{
		if (this.openCount == 0 && Door.DisplacesGas(this.doorType))
		{
			StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
			HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
			if (handle.IsValid() && !structureTemperatures.IsEnabled(handle))
			{
				int[] placementCells = this.building.PlacementCells;
				float num = 0f;
				int num2 = 0;
				foreach (int num3 in placementCells)
				{
					if (Grid.Mass[num3] > 0f)
					{
						num2++;
						num += Grid.Temperature[num3];
					}
				}
				if (num2 > 0)
				{
					num /= (float)placementCells.Length;
					PrimaryElement component = base.GetComponent<PrimaryElement>();
					component.Temperature = num;
				}
			}
		}
		this.openCount++;
		float num4 = 1f;
		if (this.consumer != null)
		{
			num4 = ((!this.consumer.IsPowered) ? 0.5f : 1f);
		}
		Door.ControlState controlState = this.controlState;
		if (controlState != Door.ControlState.Auto && controlState != Door.ControlState.Opened)
		{
			if (controlState != Door.ControlState.Closed)
			{
			}
		}
		else
		{
			this.controller.sm.isOpen.Set(true, this.controller);
		}
		return num4;
	}

	public void Close()
	{
		this.openCount = Mathf.Max(0, this.openCount - 1);
		if (this.openCount == 0 && Door.DisplacesGas(this.doorType))
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
		Door.ControlState controlState = this.controlState;
		if (controlState != Door.ControlState.Opened)
		{
			if (controlState != Door.ControlState.Closed)
			{
				if (controlState == Door.ControlState.Auto)
				{
					if (this.openCount == 0)
					{
						this.controller.sm.isOpen.Set(false, this.controller);
						Game.Instance.userMenu.Refresh(base.gameObject);
					}
				}
			}
			else
			{
				this.controller.sm.isOpen.Set(false, this.controller);
			}
		}
	}

	public bool IsOpen()
	{
		return this.controller.IsInsideState(this.controller.sm.open) || this.controller.IsInsideState(this.controller.sm.closedelay) || this.controller.IsInsideState(this.controller.sm.closeblocked);
	}

	private void ApplyRequestedControlState(bool force = false)
	{
		if (this.requestedState == this.controlState && !force)
		{
			return;
		}
		this.controlState = this.requestedState;
		this.RefreshControlState();
		this.OnOperationalChanged(null);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, false);
		base.Trigger(1734268753, this);
		if (!force)
		{
			this.Open();
			this.Close();
		}
	}

	public void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID != Door.OPEN_CLOSE_PORT_ID)
		{
			return;
		}
		int newValue = logicValueChanged.newValue;
		if (this.changeStateChore != null)
		{
			this.changeStateChore.Cancel("Change state");
			this.changeStateChore = null;
		}
		bool flag = newValue == 1;
		this.requestedState = ((!flag) ? Door.ControlState.Closed : Door.ControlState.Opened);
		this.applyLogicChange = true;
	}

	public void Sim200ms(float dt)
	{
		if (this == null)
		{
			return;
		}
		if (this.applyLogicChange)
		{
			this.applyLogicChange = false;
			this.ApplyRequestedControlState(false);
		}
		if (this.do_melt_check)
		{
			StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
			HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
			if (handle.IsValid() && !structureTemperatures.GetData(handle).enabled)
			{
				foreach (int num in this.building.PlacementCells)
				{
					if (!Grid.Solid[num])
					{
						PrimaryElement component = base.GetComponent<PrimaryElement>();
						StructureTemperatureComponents.DoMelt(component);
						break;
					}
				}
			}
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpReq]
	private KBatchedAnimController animController;

	[MyCmpReq]
	public Building building;

	[MyCmpGet]
	private EnergyConsumer consumer;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	[SerializeField]
	public bool hasComplexUserControls;

	[SerializeField]
	public float unpoweredAnimSpeed = 0.25f;

	[SerializeField]
	public float poweredAnimSpeed = 1f;

	[SerializeField]
	public Door.DoorType doorType;

	[SerializeField]
	public bool allowAutoControl = true;

	[SerializeField]
	public string doorClosingSoundEventName;

	[SerializeField]
	public string doorOpeningSoundEventName;

	private string doorClosingSound;

	private string doorOpeningSound;

	private static readonly HashedString SOUND_POWERED_PARAMETER = "doorPowered";

	private static readonly HashedString SOUND_PROGRESS_PARAMETER = "doorProgress";

	[Serialize]
	private bool hasBeenUnsealed;

	[Serialize]
	private Door.ControlState controlState;

	private bool on;

	private bool do_melt_check;

	private int openCount;

	private Door.ControlState requestedState;

	private Chore changeStateChore;

	private Door.Controller.Instance controller;

	private LoggerFSS log;

	public static readonly HashedString OPEN_CLOSE_PORT_ID = new HashedString("DoorOpenClose");

	private static readonly KAnimFile[] OVERRIDE_ANIMS = new KAnimFile[] { Assets.GetAnim("anim_use_remote_kanim") };

	private bool applyLogicChange;

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
			this.root.Update("RefreshIsBlocked", delegate(Door.Controller.Instance smi, float dt)
			{
				smi.RefreshIsBlocked();
			}, UpdateRate.SIM_200ms, false).ParamTransition<bool>(this.isSealed, this.Sealed.closed, (Door.Controller.Instance smi, bool p) => p);
			this.closeblocked.PlayAnim("open").ParamTransition<bool>(this.isOpen, this.open, (Door.Controller.Instance smi, bool p) => p).ParamTransition<bool>(this.isBlocked, this.closedelay, (Door.Controller.Instance smi, bool p) => !p);
			this.closedelay.PlayAnim("open").ScheduleGoTo(0.5f, this.closing).ParamTransition<bool>(this.isOpen, this.open, (Door.Controller.Instance smi, bool p) => p)
				.ParamTransition<bool>(this.isBlocked, this.closeblocked, (Door.Controller.Instance smi, bool p) => p);
			this.closing.ParamTransition<bool>(this.isBlocked, this.closeblocked, (Door.Controller.Instance smi, bool p) => p).ToggleTag(GameTags.Transition).ToggleLoopingSound("Closing loop", (Door.Controller.Instance smi) => smi.master.doorClosingSound, (Door.Controller.Instance smi) => !string.IsNullOrEmpty(smi.master.doorClosingSound))
				.Enter("SetParams", delegate(Door.Controller.Instance smi)
				{
					smi.master.UpdateAnimAndSoundParams(smi.master.on);
				})
				.Update(delegate(Door.Controller.Instance smi, float dt)
				{
					if (smi.master.doorClosingSound != null)
					{
						smi.master.loopingSounds.UpdateSecondParameter(smi.master.doorClosingSound, Door.SOUND_PROGRESS_PARAMETER, smi.animController.GetPositionPercent());
					}
				}, UpdateRate.SIM_33ms, false)
				.Enter("SetActive", delegate(Door.Controller.Instance smi)
				{
					smi.master.SetActive(true);
				})
				.Exit("SetActive", delegate(Door.Controller.Instance smi)
				{
					smi.master.SetActive(false);
				})
				.PlayAnim("closing")
				.OnAnimQueueComplete(this.closed);
			this.open.PlayAnim("open").ParamTransition<bool>(this.isOpen, this.closeblocked, (Door.Controller.Instance smi, bool p) => !p).Enter("SetWorldStateOpen", delegate(Door.Controller.Instance smi)
			{
				smi.master.SetWorldState();
			});
			this.closed.PlayAnim("closed").ParamTransition<bool>(this.isOpen, this.opening, (Door.Controller.Instance smi, bool p) => p).ParamTransition<bool>(this.isLocked, this.locking, (Door.Controller.Instance smi, bool p) => p)
				.Enter("SetWorldStateClosed", delegate(Door.Controller.Instance smi)
				{
					smi.master.SetWorldState();
				});
			this.locking.PlayAnim("locked_pre").OnAnimQueueComplete(this.locked).Enter("SetWorldStateClosed", delegate(Door.Controller.Instance smi)
			{
				smi.master.SetWorldState();
			});
			this.locked.PlayAnim("locked").ParamTransition<bool>(this.isLocked, this.unlocking, (Door.Controller.Instance smi, bool p) => !p);
			this.unlocking.PlayAnim("locked_pst").OnAnimQueueComplete(this.closed);
			this.opening.ToggleTag(GameTags.Transition).ToggleLoopingSound("Opening loop", (Door.Controller.Instance smi) => smi.master.doorOpeningSound, (Door.Controller.Instance smi) => !string.IsNullOrEmpty(smi.master.doorOpeningSound)).Enter("SetParams", delegate(Door.Controller.Instance smi)
			{
				smi.master.UpdateAnimAndSoundParams(smi.master.on);
			})
				.Update(delegate(Door.Controller.Instance smi, float dt)
				{
					if (smi.master.doorOpeningSound != null)
					{
						smi.master.loopingSounds.UpdateSecondParameter(smi.master.doorOpeningSound, Door.SOUND_PROGRESS_PARAMETER, smi.animController.GetPositionPercent());
					}
				}, UpdateRate.SIM_33ms, false)
				.Enter("SetActive", delegate(Door.Controller.Instance smi)
				{
					smi.master.SetActive(true);
				})
				.Exit("SetActive", delegate(Door.Controller.Instance smi)
				{
					smi.master.SetActive(false);
				})
				.PlayAnim("opening")
				.OnAnimQueueComplete(this.open);
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
			this.Sealed.closed.PlayAnim("sealed", KAnim.PlayMode.Once);
			this.Sealed.awaiting_unlock.ToggleChore((Door.Controller.Instance smi) => this.CreateUnsealChore(smi, true), this.Sealed.chore_pst);
			this.Sealed.chore_pst.Enter(delegate(Door.Controller.Instance smi)
			{
				smi.master.hasBeenUnsealed = true;
				if (smi.master.GetComponent<Unsealable>().unsealed)
				{
					smi.GoTo(this.opening);
					FogOfWarMask.ClearMask(Grid.CellRight(Grid.PosToCell(smi.master.gameObject)));
					FogOfWarMask.ClearMask(Grid.CellLeft(Grid.PosToCell(smi.master.gameObject)));
				}
				else
				{
					smi.GoTo(this.Sealed.closed);
				}
			});
		}

		private Chore CreateUnsealChore(Door.Controller.Instance smi, bool approach_right)
		{
			return new WorkChore<Unsealable>(Db.Get().ChoreTypes.Toggle, smi.master, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
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
