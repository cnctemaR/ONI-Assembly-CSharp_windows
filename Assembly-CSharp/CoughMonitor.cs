using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

public class CoughMonitor : GameStateMachine<CoughMonitor, CoughMonitor.Instance, IStateMachineTarget, CoughMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.idle.EventHandler(GameHashes.PoorAirQuality, new GameStateMachine<CoughMonitor, CoughMonitor.Instance, IStateMachineTarget, CoughMonitor.Def>.GameEvent.Callback(this.OnBreatheDirtyAir)).ParamTransition<bool>(this.shouldCough, this.coughing, (CoughMonitor.Instance smi, bool bShouldCough) => bShouldCough);
		this.coughing.ToggleStatusItem(Db.Get().DuplicantStatusItems.Coughing, null).ToggleReactable((CoughMonitor.Instance smi) => smi.GetReactable()).ParamTransition<bool>(this.shouldCough, this.idle, (CoughMonitor.Instance smi, bool bShouldCough) => !bShouldCough);
	}

	private void OnBreatheDirtyAir(CoughMonitor.Instance smi, object data)
	{
		float timeInCycles = GameClock.Instance.GetTimeInCycles();
		if (timeInCycles > 0.1f && timeInCycles - smi.lastCoughTime <= 0.1f)
		{
			return;
		}
		float value = ((Boxed<float>)data).value;
		float num = ((smi.lastConsumeTime <= 0f) ? 0f : (timeInCycles - smi.lastConsumeTime));
		smi.lastConsumeTime = timeInCycles;
		smi.amountConsumed -= 0.05f * num;
		smi.amountConsumed = Mathf.Max(smi.amountConsumed, 0f);
		smi.amountConsumed += value;
		if (smi.amountConsumed >= 1f)
		{
			this.shouldCough.Set(true, smi, false);
			smi.lastConsumeTime = 0f;
			smi.amountConsumed = 0f;
		}
	}

	private const float amountToCough = 1f;

	private const float decayRate = 0.05f;

	private const float coughInterval = 0.1f;

	public GameStateMachine<CoughMonitor, CoughMonitor.Instance, IStateMachineTarget, CoughMonitor.Def>.State idle;

	public GameStateMachine<CoughMonitor, CoughMonitor.Instance, IStateMachineTarget, CoughMonitor.Def>.State coughing;

	public StateMachine<CoughMonitor, CoughMonitor.Instance, IStateMachineTarget, CoughMonitor.Def>.BoolParameter shouldCough = new StateMachine<CoughMonitor, CoughMonitor.Instance, IStateMachineTarget, CoughMonitor.Def>.BoolParameter(false);

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CoughMonitor, CoughMonitor.Instance, IStateMachineTarget, CoughMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CoughMonitor.Def def)
			: base(master, def)
		{
		}

		public Reactable GetReactable()
		{
			Emote cough_Small = Db.Get().Emotes.Minion.Cough_Small;
			SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.master.gameObject, "BadAirCough", Db.Get().ChoreTypes.Cough, 0f, 0f, float.PositiveInfinity, 0f);
			selfEmoteReactable.SetEmote(cough_Small);
			selfEmoteReactable.preventChoreInterruption = true;
			return selfEmoteReactable.RegisterEmoteStepCallbacks("react_small", null, new Action<GameObject>(this.FinishedCoughing));
		}

		private void FinishedCoughing(GameObject cougher)
		{
			cougher.GetComponent<Effects>().Add("ContaminatedLungs", true);
			base.sm.shouldCough.Set(false, base.smi, false);
			base.smi.lastCoughTime = GameClock.Instance.GetTimeInCycles();
		}

		[Serialize]
		public float lastCoughTime;

		[Serialize]
		public float lastConsumeTime;

		[Serialize]
		public float amountConsumed;
	}
}
