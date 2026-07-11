using System;
using UnityEngine;

public class EmoteMonitor : GameStateMachine<EmoteMonitor, EmoteMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = true;
		this.satisfied.ScheduleGoTo((float)global::UnityEngine.Random.Range(30, 90), this.ready);
		this.ready.ToggleUrge(Db.Get().Urges.Emote).EventHandler(GameHashes.BeginChore, delegate(EmoteMonitor.Instance smi, object o)
		{
			smi.OnStartChore(o);
		});
	}

	public GameStateMachine<EmoteMonitor, EmoteMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<EmoteMonitor, EmoteMonitor.Instance, IStateMachineTarget, object>.State ready;

	public new class Instance : GameStateMachine<EmoteMonitor, EmoteMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void OnStartChore(object o)
		{
			if (((Chore)o).SatisfiesUrge(Db.Get().Urges.Emote))
			{
				this.GoTo(base.sm.satisfied);
			}
		}
	}
}
