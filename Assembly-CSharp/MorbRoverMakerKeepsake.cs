using System;
using UnityEngine;

public class MorbRoverMakerKeepsake : GameStateMachine<MorbRoverMakerKeepsake, MorbRoverMakerKeepsake.Instance, IStateMachineTarget, MorbRoverMakerKeepsake.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.silent;
		this.silent.PlayAnim("silent").Enter(new StateMachine<MorbRoverMakerKeepsake, MorbRoverMakerKeepsake.Instance, IStateMachineTarget, MorbRoverMakerKeepsake.Def>.State.Callback(MorbRoverMakerKeepsake.CalculateNextActivationTime)).Update(new Action<MorbRoverMakerKeepsake.Instance, float>(MorbRoverMakerKeepsake.TimerUpdate), UpdateRate.SIM_200ms, false);
		this.talking.PlayAnim("idle").OnAnimQueueComplete(this.silent);
	}

	public static void CalculateNextActivationTime(MorbRoverMakerKeepsake.Instance smi)
	{
		smi.CalculateNextActivationTime();
	}

	public static void TimerUpdate(MorbRoverMakerKeepsake.Instance smi, float dt)
	{
		if (GameClock.Instance.GetTime() > smi.NextActivationTime)
		{
			smi.GoTo(smi.sm.talking);
		}
	}

	public const string SILENT_ANIMATION_NAME = "silent";

	public const string TALKING_ANIMATION_NAME = "idle";

	public GameStateMachine<MorbRoverMakerKeepsake, MorbRoverMakerKeepsake.Instance, IStateMachineTarget, MorbRoverMakerKeepsake.Def>.State silent;

	public GameStateMachine<MorbRoverMakerKeepsake, MorbRoverMakerKeepsake.Instance, IStateMachineTarget, MorbRoverMakerKeepsake.Def>.State talking;

	public class Def : StateMachine.BaseDef
	{
		public Vector2 OperationalRandomnessRange = new Vector2(120f, 600f);
	}

	public new class Instance : GameStateMachine<MorbRoverMakerKeepsake, MorbRoverMakerKeepsake.Instance, IStateMachineTarget, MorbRoverMakerKeepsake.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, MorbRoverMakerKeepsake.Def def)
			: base(master, def)
		{
		}

		public void CalculateNextActivationTime()
		{
			float time = GameClock.Instance.GetTime();
			float num = time + base.def.OperationalRandomnessRange.x;
			float num2 = time + base.def.OperationalRandomnessRange.y;
			this.NextActivationTime = global::UnityEngine.Random.Range(num, num2);
		}

		public float NextActivationTime = -1f;
	}
}
