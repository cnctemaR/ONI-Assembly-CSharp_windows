using System;
using Klei.AI;

public class RobotBatteryMonitor : GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.highBattery;
		this.lowBatteryStates.ToggleBehaviour(GameTags.Robots.Behaviours.RechargeBehaviour, (RobotBatteryMonitor.Instance data) => true, null).Enter(delegate(RobotBatteryMonitor.Instance smi)
		{
		});
		this.lowBatteryStates.lowBattery.ToggleStatusItem(Db.Get().RobotStatusItems.LowBattery, (RobotBatteryMonitor.Instance smi) => smi.gameObject).Transition(this.lowBatteryStates.mediumBattery, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeDecent), UpdateRate.SIM_200ms).Exit(delegate(RobotBatteryMonitor.Instance smi)
		{
		});
		this.lowBatteryStates.mediumBattery.Transition(this.lowBatteryStates.lowBattery, GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Not(new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeDecent)), UpdateRate.SIM_200ms).Transition(this.highBattery, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeComplete), UpdateRate.SIM_200ms);
		this.scheduledBatteryCharge.ToggleBehaviour(GameTags.Robots.Behaviours.RechargeBehaviour, (RobotBatteryMonitor.Instance data) => true, null).Transition(this.highBattery, GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Not(new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.IsScheduledRecharge)), UpdateRate.SIM_200ms);
		this.highBattery.Transition(this.lowBatteryStates.lowBattery, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.NeedsRecharge), UpdateRate.SIM_200ms).Transition(this.scheduledBatteryCharge, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.IsScheduledRecharge), UpdateRate.SIM_200ms);
	}

	public static bool NeedsRecharge(RobotBatteryMonitor.Instance smi)
	{
		return smi.master.gameObject.GetAmounts().GetValue(Db.Get().Amounts.InternalBattery.Id) <= 0f;
	}

	public static bool IsScheduledRecharge(RobotBatteryMonitor.Instance smi)
	{
		return GameClock.Instance.IsNighttime();
	}

	public static bool ChargeDecent(RobotBatteryMonitor.Instance smi)
	{
		return smi.master.gameObject.GetAmounts().GetValue(Db.Get().Amounts.InternalBattery.Id) >= smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.InternalBattery.Id).GetMax() * 0.5f;
	}

	public static bool ChargeComplete(RobotBatteryMonitor.Instance smi)
	{
		return smi.master.gameObject.GetAmounts().GetValue(Db.Get().Amounts.InternalBattery.Id) >= smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.InternalBattery.Id).GetMax();
	}

	public StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.ObjectParameter<Storage> internalStorage = new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.ObjectParameter<Storage>();

	public RobotBatteryMonitor.LowBatteryStates lowBatteryStates;

	public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State scheduledBatteryCharge;

	public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State highBattery;

	public class Def : StateMachine.BaseDef
	{
	}

	public class LowBatteryStates : GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State
	{
		public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State lowBattery;

		public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State mediumBattery;
	}

	public new class Instance : GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, RobotBatteryMonitor.Def def)
			: base(master, def)
		{
			AmountInstance amountInstance = Db.Get().Amounts.InternalBattery.Lookup(base.gameObject);
			amountInstance.value = amountInstance.GetMax();
		}
	}
}
