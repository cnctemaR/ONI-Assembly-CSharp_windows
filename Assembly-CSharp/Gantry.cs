using System;
using STRINGS;

public class Gantry : Switch
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (Gantry.infoStatusItem == null)
		{
			Gantry.infoStatusItem = new StatusItem("GantryAutomationInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			Gantry.infoStatusItem.resolveStringCallback = new Func<string, object, string>(Gantry.ResolveInfoStatusItemString);
		}
		base.GetComponent<KAnimControllerBase>().PlaySpeedMultiplier = 0.5f;
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
		base.OnCleanUp();
	}

	public void SetWalkable(bool active)
	{
		this.fakeFloorAdder.SetFloor(active);
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
		string text = (instance.IsAutomated() ? BUILDING.STATUSITEMS.GANTRY.AUTOMATION_CONTROL : BUILDING.STATUSITEMS.GANTRY.MANUAL_CONTROL);
		string text2 = (instance.IsExtended() ? BUILDING.STATUSITEMS.GANTRY.EXTENDED : BUILDING.STATUSITEMS.GANTRY.RETRACTED);
		return string.Format(text, text2);
	}

	public static readonly HashedString PORT_ID = "Gantry";

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private FakeFloorAdder fakeFloorAdder;

	private Gantry.Instance smi;

	private static StatusItem infoStatusItem;

	public class States : GameStateMachine<Gantry.States, Gantry.Instance, Gantry>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.extended;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
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
				.ParamTransition<bool>(this.should_extend, this.retracted_pre, GameStateMachine<Gantry.States, Gantry.Instance, Gantry, object>.IsFalse)
				.ToggleTag(GameTags.GantryExtended);
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
			base.smi.sm.should_extend.Set(true, base.smi, false);
		}

		public bool IsAutomated()
		{
			return this.logic.IsPortConnected(Gantry.PORT_ID);
		}

		public bool IsExtended()
		{
			if (!this.IsAutomated())
			{
				return this.manual_on;
			}
			return this.logic_on;
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

		private void OnOperationalChanged(object _)
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
			this.logic_on = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
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
				base.smi.sm.should_extend.Set(this.logic_on, base.smi, false);
				return;
			}
			base.smi.sm.should_extend.Set(this.manual_on, base.smi, false);
		}

		private Operational operational;

		public LogicPorts logic;

		public bool logic_on = true;

		private bool manual_on;
	}
}
