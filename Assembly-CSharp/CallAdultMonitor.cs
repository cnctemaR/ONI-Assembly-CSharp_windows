using System;
using UnityEngine;

public class CallAdultMonitor : GameStateMachine<CallAdultMonitor, CallAdultMonitor.Instance, IStateMachineTarget, CallAdultMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleBehaviour(GameTags.Creatures.Behaviours.CallAdultBehaviour, new StateMachine<CallAdultMonitor, CallAdultMonitor.Instance, IStateMachineTarget, CallAdultMonitor.Def>.Transition.ConditionCallback(CallAdultMonitor.ShouldCallAdult), delegate(CallAdultMonitor.Instance smi)
		{
			smi.RefreshCallTime();
		});
	}

	public static bool ShouldCallAdult(CallAdultMonitor.Instance smi)
	{
		return Time.time >= smi.nextCallTime;
	}

	public class Def : StateMachine.BaseDef
	{
		public float callMinInterval = 120f;

		public float callMaxInterval = 240f;
	}

	public new class Instance : GameStateMachine<CallAdultMonitor, CallAdultMonitor.Instance, IStateMachineTarget, CallAdultMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CallAdultMonitor.Def def)
			: base(master, def)
		{
			this.RefreshCallTime();
		}

		public void RefreshCallTime()
		{
			this.nextCallTime = Time.time + global::UnityEngine.Random.value * (base.def.callMaxInterval - base.def.callMinInterval) + base.def.callMinInterval;
		}

		public float nextCallTime;
	}
}
