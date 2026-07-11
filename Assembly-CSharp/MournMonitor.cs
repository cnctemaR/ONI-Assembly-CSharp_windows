using System;
using Klei.AI;

public class MournMonitor : GameStateMachine<MournMonitor, MournMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.EventHandler(GameHashes.EffectAdded, new GameStateMachine<MournMonitor, MournMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback(this.OnEffectAdded)).Enter(delegate(MournMonitor.Instance smi)
		{
			if (this.ShouldMourn(smi))
			{
				smi.GoTo(this.needsToMourn);
			}
		});
		this.needsToMourn.ToggleChore((MournMonitor.Instance smi) => new MournChore(smi.master), this.idle);
	}

	private bool ShouldMourn(MournMonitor.Instance smi)
	{
		Effect effect = Db.Get().effects.Get("Mourning");
		return smi.master.GetComponent<Effects>().HasEffect(effect);
	}

	private void OnEffectAdded(MournMonitor.Instance smi, object data)
	{
		if (this.ShouldMourn(smi))
		{
			smi.GoTo(this.needsToMourn);
		}
	}

	private GameStateMachine<MournMonitor, MournMonitor.Instance, IStateMachineTarget, object>.State idle;

	private GameStateMachine<MournMonitor, MournMonitor.Instance, IStateMachineTarget, object>.State needsToMourn;

	public new class Instance : GameStateMachine<MournMonitor, MournMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
