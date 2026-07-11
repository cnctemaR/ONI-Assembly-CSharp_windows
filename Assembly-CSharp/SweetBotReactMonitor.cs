using System;
using UnityEngine;

public class SweetBotReactMonitor : GameStateMachine<SweetBotReactMonitor, SweetBotReactMonitor.Instance, IStateMachineTarget, SweetBotReactMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.EventHandler(GameHashes.OccupantChanged, delegate(SweetBotReactMonitor.Instance smi)
		{
			if (smi.master.gameObject.GetComponent<OrnamentReceptacle>().Occupant != null)
			{
				smi.GoTo(this.reactNewOrnament);
			}
		}).Update(delegate(SweetBotReactMonitor.Instance smi, float dt)
		{
			SweepStates.Instance smi2 = smi.master.gameObject.GetSMI<SweepStates.Instance>();
			int checkCell = Grid.InvalidCell;
			if (smi2 != null)
			{
				if (smi2.sm.headingRight.Get(smi2))
				{
					checkCell = Grid.CellRight(Grid.PosToCell(smi.master.gameObject));
				}
				else
				{
					checkCell = Grid.CellLeft(Grid.PosToCell(smi.master.gameObject));
				}
				Brain brain = Components.Brains.Items.Find((Brain match) => Grid.PosToCell(match) == checkCell);
				if (brain != null && brain.GetComponent<KPrefabID>().PrefabID() == "SweepBot")
				{
					if (Vector3.Distance(smi.master.gameObject.transform.position, brain.gameObject.transform.position) < Grid.CellSizeInMeters)
					{
						smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("bump");
						smi2.sm.headingRight.Set(!smi2.sm.headingRight.Get(smi2), smi2);
						return;
					}
				}
				else if (brain != null && smi.timeinstate > 10f && Grid.IsValidCell(checkCell))
				{
					if (Grid.Objects[checkCell, 0] != null && !Grid.Objects[checkCell, 0].HasTag(GameTags.Dead))
					{
						smi.GoTo(this.reactFriendlyThing);
						return;
					}
					if (smi2.sm.bored.Get(smi2) && Grid.Objects[checkCell, 3] != null)
					{
						smi.GoTo(this.reactFriendlyThing);
						return;
					}
					smi.GoTo(this.reactScaryThing);
				}
			}
		}, UpdateRate.SIM_33ms, false);
		this.reactScaryThing.Enter(delegate(SweetBotReactMonitor.Instance smi)
		{
			smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("react_neg");
		}).OnAnimQueueComplete(this.idle);
		this.reactFriendlyThing.Enter(delegate(SweetBotReactMonitor.Instance smi)
		{
			smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("react_pos");
		}).OnAnimQueueComplete(this.idle);
		this.reactNewOrnament.Enter(delegate(SweetBotReactMonitor.Instance smi)
		{
			smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("react_ornament");
		}).OnAnimQueueComplete(this.idle);
	}

	private GameStateMachine<SweetBotReactMonitor, SweetBotReactMonitor.Instance, IStateMachineTarget, SweetBotReactMonitor.Def>.State idle;

	private GameStateMachine<SweetBotReactMonitor, SweetBotReactMonitor.Instance, IStateMachineTarget, SweetBotReactMonitor.Def>.State reactScaryThing;

	private GameStateMachine<SweetBotReactMonitor, SweetBotReactMonitor.Instance, IStateMachineTarget, SweetBotReactMonitor.Def>.State reactFriendlyThing;

	private GameStateMachine<SweetBotReactMonitor, SweetBotReactMonitor.Instance, IStateMachineTarget, SweetBotReactMonitor.Def>.State reactNewOrnament;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<SweetBotReactMonitor, SweetBotReactMonitor.Instance, IStateMachineTarget, SweetBotReactMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, SweetBotReactMonitor.Def def)
			: base(master, def)
		{
		}
	}
}
