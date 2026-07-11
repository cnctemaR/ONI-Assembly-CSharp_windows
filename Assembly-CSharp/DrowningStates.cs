using System;
using UnityEngine;

internal class DrowningStates : GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.drowning;
		this.root.ToggleStatusItem(Db.Get().CreatureStatusItems.Drowning, null).TagTransition(GameTags.Creatures.Drowning, null, true);
		this.drowning.PlayAnim("harvest", KAnim.PlayMode.Loop).ToggleScheduleCallback("IdleMove", (DrowningStates.Instance smi) => (float)global::UnityEngine.Random.Range(1, 3), delegate(DrowningStates.Instance smi)
		{
			smi.GoTo(this.escape);
		});
		this.escape.Enter(new StateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.State.Callback(this.MoveToSafeCell)).EventTransition(GameHashes.DestinationReached, this.drowning, null).EventTransition(GameHashes.NavigationFailed, this.drowning, null);
	}

	public void MoveToSafeCell(DrowningStates.Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		DrowningMonitor component2 = smi.GetComponent<DrowningMonitor>();
		DrowningStates.EscapeCellQuery escapeCellQuery = new DrowningStates.EscapeCellQuery(component2);
		component.RunQuery(escapeCellQuery);
		component.GoTo(escapeCellQuery.GetResultCell(), null);
	}

	public GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.State drowning;

	public GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.State escape;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.GameInstance
	{
		public Instance(Chore<DrowningStates.Instance> chore, DrowningStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.HasTag, GameTags.Creatures.Drowning);
		}
	}

	public class EscapeCellQuery : PathFinderQuery
	{
		public EscapeCellQuery(DrowningMonitor monitor)
		{
			this.monitor = monitor;
		}

		public override bool IsMatch(int cell, int parent_cell, int cost)
		{
			return this.monitor.IsCellSafe(cell);
		}

		private DrowningMonitor monitor;
	}
}
