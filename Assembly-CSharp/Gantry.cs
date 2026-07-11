using System;
using STRINGS;

public class Gantry : Switch
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (Gantry.infoStatusItem == null)
		{
			Gantry.infoStatusItem = new StatusItem("GantryAutomationInfo", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Gantry.infoStatusItem.resolveStringCallback = new Func<string, object, string>(Gantry.ResolveInfoStatusItemString);
		}
		base.GetComponent<KAnimControllerBase>().PlaySpeedMultiplier = 0.5f;
		int num = Grid.PosToCell(this);
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		for (int i = 0; i < Gantry.TileOffsets.Length; i++)
		{
			CellOffset rotatedOffset = this.building.GetRotatedOffset(Gantry.TileOffsets[i]);
			int num2 = Grid.OffsetCell(num, rotatedOffset);
			SimMessages.ReplaceAndDisplaceElement(num2, component.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, component.Mass, component.Temperature, byte.MaxValue, 0, -1);
			Grid.Objects[num2, 1] = base.gameObject;
			Grid.Foundation[num2] = true;
			Grid.Objects[num2, 9] = base.gameObject;
			Grid.SetSolid(num2, true, CellEventLogger.Instance.SimCellOccupierForceSolid);
			Grid.RenderedByWorld[num2] = false;
			World.Instance.OnSolidChanged(num2);
			GameScenePartitioner.Instance.TriggerEvent(num2, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
		this.smi = new Gantry.Instance(this, base.IsSwitchedOn);
		this.smi.StartSM();
		base.GetComponent<KSelectable>().ToggleStatusItem(Gantry.infoStatusItem, true, this.smi);
	}

	protected override void OnCleanUp()
	{
		if (this.smi != null)
		{
			this.smi.StopSM("cleanup");
		}
		int num = Grid.PosToCell(this);
		foreach (CellOffset cellOffset in Gantry.TileOffsets)
		{
			CellOffset rotatedOffset = this.building.GetRotatedOffset(cellOffset);
			int num2 = Grid.OffsetCell(num, rotatedOffset);
			SimMessages.ReplaceAndDisplaceElement(num2, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0f, -1f, byte.MaxValue, 0, -1);
			Grid.Objects[num2, 1] = null;
			Grid.Objects[num2, 9] = null;
			Grid.Foundation[num2] = false;
			Grid.SetSolid(num2, false, CellEventLogger.Instance.SimCellOccupierDestroy);
			Grid.RenderedByWorld[num2] = true;
			World.Instance.OnSolidChanged(num2);
			GameScenePartitioner.Instance.TriggerEvent(num2, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
		foreach (CellOffset cellOffset2 in Gantry.RetractableOffsets)
		{
			CellOffset rotatedOffset2 = this.building.GetRotatedOffset(cellOffset2);
			int num3 = Grid.OffsetCell(num, rotatedOffset2);
			Grid.FakeFloor[num3] = false;
			Pathfinding.Instance.AddDirtyNavGridCell(num3);
		}
		base.OnCleanUp();
	}

	public void SetWalkable(bool active)
	{
		int num = Grid.PosToCell(this);
		foreach (CellOffset cellOffset in Gantry.RetractableOffsets)
		{
			CellOffset rotatedOffset = this.building.GetRotatedOffset(cellOffset);
			int num2 = Grid.OffsetCell(num, rotatedOffset);
			Grid.FakeFloor[num2] = active;
			Pathfinding.Instance.AddDirtyNavGridCell(num2);
		}
	}

	protected override void Toggle()
	{
		base.Toggle();
		this.smi.SetSwitchState(this.switchedOn);
	}

	protected override void OnRefreshUserMenu(object data)
	{
		if (!this.smi.IsAutomated())
		{
			base.OnRefreshUserMenu(data);
		}
	}

	protected override void UpdateSwitchStatus()
	{
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		Gantry.Instance instance = (Gantry.Instance)data;
		string text = ((!instance.IsAutomated()) ? BUILDING.STATUSITEMS.GANTRY.MANUAL_CONTROL : BUILDING.STATUSITEMS.GANTRY.AUTOMATION_CONTROL);
		string text2 = ((!instance.IsExtended()) ? BUILDING.STATUSITEMS.GANTRY.RETRACTED : BUILDING.STATUSITEMS.GANTRY.EXTENDED);
		return string.Format(text, text2);
	}

	public static readonly HashedString PORT_ID = "Gantry";

	[MyCmpReq]
	private Building building;

	public static CellOffset[] TileOffsets = new CellOffset[]
	{
		new CellOffset(-2, 1),
		new CellOffset(-1, 1)
	};

	public static CellOffset[] RetractableOffsets = new CellOffset[]
	{
		new CellOffset(0, 1),
		new CellOffset(1, 1),
		new CellOffset(2, 1),
		new CellOffset(3, 1)
	};

	private Gantry.Instance smi;

	private static StatusItem infoStatusItem;

	public class States : GameStateMachine<Gantry.States, Gantry.Instance, Gantry>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.extended;
			base.serializable = true;
			this.retracted_pre.Enter(delegate(Gantry.Instance smi)
			{
				smi.SetActive(true);
			}).Exit(delegate(Gantry.Instance smi)
			{
				smi.SetActive(false);
			}).PlayAnim("off_pre")
				.OnAnimQueueComplete(this.retracted);
			this.retracted.PlayAnim("off").ParamTransition<bool>(this.should_extend, this.extended_pre, GameStateMachine<Gantry.States, Gantry.Instance, Gantry, object>.IsTrue);
			this.extended_pre.Enter(delegate(Gantry.Instance smi)
			{
				smi.SetActive(true);
			}).Exit(delegate(Gantry.Instance smi)
			{
				smi.SetActive(false);
			}).PlayAnim("on_pre")
				.OnAnimQueueComplete(this.extended);
			this.extended.Enter(delegate(Gantry.Instance smi)
			{
				smi.master.SetWalkable(true);
			}).Exit(delegate(Gantry.Instance smi)
			{
				smi.master.SetWalkable(false);
			}).PlayAnim("on")
				.ParamTransition<bool>(this.should_extend, this.retracted_pre, GameStateMachine<Gantry.States, Gantry.Instance, Gantry, object>.IsFalse);
		}

		public GameStateMachine<Gantry.States, Gantry.Instance, Gantry, object>.State retracted_pre;

		public GameStateMachine<Gantry.States, Gantry.Instance, Gantry, object>.State retracted;

		public GameStateMachine<Gantry.States, Gantry.Instance, Gantry, object>.State extended_pre;

		public GameStateMachine<Gantry.States, Gantry.Instance, Gantry, object>.State extended;

		public StateMachine<Gantry.States, Gantry.Instance, Gantry, object>.BoolParameter should_extend;
	}

	public class Instance : GameStateMachine<Gantry.States, Gantry.Instance, Gantry, object>.GameInstance
	{
		public Instance(Gantry master, bool manual_start_state)
			: base(master)
		{
			this.manual_on = manual_start_state;
			this.operational = base.GetComponent<Operational>();
			this.logic = base.GetComponent<LogicPorts>();
			base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
			base.Subscribe(-801688580, new Action<object>(this.OnLogicValueChanged));
			base.smi.sm.should_extend.Set(true, base.smi);
		}

		public bool IsAutomated()
		{
			return this.logic.IsPortConnected(Gantry.PORT_ID);
		}

		public bool IsExtended()
		{
			return (!this.IsAutomated()) ? this.manual_on : this.logic_on;
		}

		public void SetSwitchState(bool on)
		{
			this.manual_on = on;
			this.UpdateShouldExtend();
		}

		public void SetActive(bool active)
		{
			this.operational.SetActive(this.operational.IsOperational && active, false);
		}

		private void OnOperationalChanged(object data)
		{
			this.UpdateShouldExtend();
		}

		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID != Gantry.PORT_ID)
			{
				return;
			}
			this.logic_on = logicValueChanged.newValue != 0;
			this.UpdateShouldExtend();
		}

		private void UpdateShouldExtend()
		{
			if (!this.operational.IsOperational)
			{
				return;
			}
			if (this.IsAutomated())
			{
				base.smi.sm.should_extend.Set(this.logic_on, base.smi);
			}
			else
			{
				base.smi.sm.should_extend.Set(this.manual_on, base.smi);
			}
		}

		private Operational operational;

		public LogicPorts logic;

		public bool logic_on = true;

		private bool manual_on;
	}
}
