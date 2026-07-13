using System;
using UnityEngine;

public class CritterRoarMonitor : GameStateMachine<CritterRoarMonitor, CritterRoarMonitor.Instance, IStateMachineTarget, CritterRoarMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState defaultState)
	{
		defaultState = this.wait;
		this.wait.ScheduleGoTo((CritterRoarMonitor.Instance smi) => smi.NextWaitDuration(), this.roar);
		this.roar.ToggleBehaviour(CritterRoarMonitor.TAG, CritterRoarMonitor.ALWAYS_TRUE, delegate(CritterRoarMonitor.Instance smi)
		{
			smi.GoTo(this.cooldown);
		});
		this.cooldown.ScheduleGoTo((CritterRoarMonitor.Instance smi) => smi.Def.Cooldown, this.wait);
	}

	public static Tag TAG = GameTags.Creatures.Behaviours.CritterRoarBehaviour;

	private readonly GameStateMachine<CritterRoarMonitor, CritterRoarMonitor.Instance, IStateMachineTarget, CritterRoarMonitor.Def>.State wait;

	private readonly GameStateMachine<CritterRoarMonitor, CritterRoarMonitor.Instance, IStateMachineTarget, CritterRoarMonitor.Def>.State roar;

	private readonly GameStateMachine<CritterRoarMonitor, CritterRoarMonitor.Instance, IStateMachineTarget, CritterRoarMonitor.Def>.State cooldown;

	private static readonly StateMachine<CritterRoarMonitor, CritterRoarMonitor.Instance, IStateMachineTarget, CritterRoarMonitor.Def>.Transition.ConditionCallback ALWAYS_TRUE = (CritterRoarMonitor.Instance smi) => true;

	public class Def : StateMachine.BaseDef
	{
		public float SecondsPerRoarMax { get; private set; }

		public float Cooldown { get; private set; }

		public void Initialize(int roarsPerCycle, float cooldown)
		{
			this.SecondsPerRoarMax = 600f / (float)roarsPerCycle;
			this.Cooldown = cooldown;
		}
	}

	public new class Instance : GameStateMachine<CritterRoarMonitor, CritterRoarMonitor.Instance, IStateMachineTarget, CritterRoarMonitor.Def>.GameInstance
	{
		public CritterRoarMonitor.Def Def { get; private set; }

		public Instance(IStateMachineTarget master, CritterRoarMonitor.Def def)
			: base(master, def)
		{
			this.Def = def;
			this.wait = this.Def.SecondsPerRoarMax;
			DebugUtil.DevAssert(this.Def.SecondsPerRoarMax >= this.Def.Cooldown, "Cooldown is so long so as to prevent us from achieving desired roars per cycle.", null);
			this.maxWait = this.Def.SecondsPerRoarMax - this.Def.Cooldown;
		}

		public float NextWaitDuration()
		{
			float num = this.Def.SecondsPerRoarMax - this.wait;
			this.wait = global::UnityEngine.Random.Range(num, num + this.maxWait);
			return this.wait;
		}

		private readonly float maxWait;

		private float wait;
	}
}
