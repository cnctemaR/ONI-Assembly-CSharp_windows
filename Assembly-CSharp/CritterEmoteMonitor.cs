using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class CritterEmoteMonitor : GameStateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.satisfied.ScheduleGoTo((CritterEmoteMonitor.Instance smi) => global::UnityEngine.Random.Range(75f, 150f), this.ready);
		this.ready.Enter(new StateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance, IStateMachineTarget, object>.State.Callback(CritterEmoteMonitor.CreateChore)).ToggleUrge(Db.Get().Urges.Emote).EventHandler(GameHashes.BeginChore, delegate(CritterEmoteMonitor.Instance smi, object o)
		{
			smi.OnStartChore(o);
		});
	}

	public static void CreateChore(CritterEmoteMonitor.Instance smi)
	{
		new EmoteChore(smi.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.Emote, smi.emotes.GetRandom<Emote>(), 1, null);
	}

	public GameStateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance, IStateMachineTarget, object>.State ready;

	public new class Instance : GameStateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, List<Emote> emotes)
			: base(master)
		{
			this.emotes = emotes;
		}

		public void OnStartChore(object o)
		{
			if (((Chore)o).SatisfiesUrge(Db.Get().Urges.Emote))
			{
				this.GoTo(base.sm.satisfied);
			}
		}

		public List<Emote> emotes;
	}
}
