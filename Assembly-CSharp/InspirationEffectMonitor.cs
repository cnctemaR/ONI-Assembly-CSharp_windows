using System;
using UnityEngine;

public class InspirationEffectMonitor : GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.idle.EventHandler(GameHashes.CatchyTune, new GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.GameEvent.Callback(this.OnCatchyTune)).ParamTransition<bool>(this.shouldCatchyTune, this.catchyTune, (InspirationEffectMonitor.Instance smi, bool shouldCatchyTune) => shouldCatchyTune);
		this.catchyTune.Exit(delegate(InspirationEffectMonitor.Instance smi)
		{
			this.shouldCatchyTune.Set(false, smi, false);
		}).ToggleEffect("HeardJoySinger").ToggleThought(Db.Get().Thoughts.CatchyTune, null)
			.EventHandler(GameHashes.StartWork, new GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.GameEvent.Callback(this.TryThinkCatchyTune))
			.Enter(delegate(InspirationEffectMonitor.Instance smi)
			{
				this.SingCatchyTune(smi);
			})
			.Update(delegate(InspirationEffectMonitor.Instance smi, float dt)
			{
				this.TryThinkCatchyTune(smi, null);
				this.inspirationTimeRemaining.Delta(-dt, smi);
			}, UpdateRate.SIM_4000ms, false)
			.ParamTransition<float>(this.inspirationTimeRemaining, this.idle, (InspirationEffectMonitor.Instance smi, float p) => p <= 0f);
	}

	private void OnCatchyTune(InspirationEffectMonitor.Instance smi, object data)
	{
		this.inspirationTimeRemaining.Set(600f, smi, false);
		this.shouldCatchyTune.Set(true, smi, false);
	}

	private void TryThinkCatchyTune(InspirationEffectMonitor.Instance smi, object data)
	{
		if (global::UnityEngine.Random.Range(1, 101) > 66)
		{
			this.SingCatchyTune(smi);
		}
	}

	private void SingCatchyTune(InspirationEffectMonitor.Instance smi)
	{
		smi.master.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.CatchyTune);
		if (!smi.GetSpeechMonitor().IsPlayingSpeech() && SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
		{
			smi.GetSpeechMonitor().PlaySpeech(Db.Get().Thoughts.CatchyTune.speechPrefix, Db.Get().Thoughts.CatchyTune.sound);
		}
	}

	public StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.BoolParameter shouldCatchyTune;

	public StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.FloatParameter inspirationTimeRemaining;

	public GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.State idle;

	public GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.State catchyTune;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, InspirationEffectMonitor.Def def)
			: base(master, def)
		{
		}

		public SpeechMonitor.Instance GetSpeechMonitor()
		{
			if (this.speechMonitor == null)
			{
				this.speechMonitor = base.master.gameObject.GetSMI<SpeechMonitor.Instance>();
			}
			return this.speechMonitor;
		}

		public SpeechMonitor.Instance speechMonitor;
	}
}
