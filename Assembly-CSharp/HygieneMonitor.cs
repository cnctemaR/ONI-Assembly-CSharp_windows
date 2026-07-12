using System;
using Klei.AI;

public class HygieneMonitor : GameStateMachine<HygieneMonitor, HygieneMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.needsshower;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.clean.EventTransition(GameHashes.EffectRemoved, this.needsshower, (HygieneMonitor.Instance smi) => smi.NeedsShower());
		this.needsshower.EventTransition(GameHashes.EffectAdded, this.clean, (HygieneMonitor.Instance smi) => !smi.NeedsShower()).ToggleUrge(Db.Get().Urges.Shower).Enter(delegate(HygieneMonitor.Instance smi)
		{
			smi.SetDirtiness(1f);
		});
	}

	public StateMachine<HygieneMonitor, HygieneMonitor.Instance, IStateMachineTarget, object>.FloatParameter dirtiness;

	public GameStateMachine<HygieneMonitor, HygieneMonitor.Instance, IStateMachineTarget, object>.State clean;

	public GameStateMachine<HygieneMonitor, HygieneMonitor.Instance, IStateMachineTarget, object>.State needsshower;

	public new class Instance : GameStateMachine<HygieneMonitor, HygieneMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.effects = master.GetComponent<Effects>();
		}

		public float GetDirtiness()
		{
			return base.sm.dirtiness.Get(this);
		}

		public void SetDirtiness(float dirtiness)
		{
			base.sm.dirtiness.Set(dirtiness, this, false);
		}

		public bool NeedsShower()
		{
			return !this.effects.HasEffect(Shower.SHOWER_EFFECT);
		}

		private Effects effects;
	}
}
