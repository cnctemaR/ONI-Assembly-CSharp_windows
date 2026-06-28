using System;
using System.Collections.Generic;

public class ReactionMonitor : GameStateMachine<ReactionMonitor, ReactionMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		base.serializable = false;
		this.idle.Enter("ClearReactable", delegate(ReactionMonitor.Instance smi)
		{
			this.reactable.Set(null, smi);
		}).TagTransition(GameTags.Dead, this.dead, false);
		this.reacting.Enter("Reactable.Begin", delegate(ReactionMonitor.Instance smi)
		{
			this.reactable.Get(smi).Begin(smi.gameObject);
		}).Update("Reactable.Update", delegate(ReactionMonitor.Instance smi)
		{
			this.reactable.Get(smi).Update(smi.dt);
		}).Exit("Reactable.End", delegate(ReactionMonitor.Instance smi)
		{
			this.reactable.Get(smi).End();
		})
			.EventTransition(GameHashes.NavigationFailed, this.idle, null)
			.ToggleTag(GameTags.PreventChoreInterruption)
			.TagTransition(GameTags.Dying, this.dead, false)
			.TagTransition(GameTags.Dead, this.dead, false);
		this.dead.DoNothing();
	}

	public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, object>.State reacting;

	public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, object>.State dead;

	public StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, object>.ObjectParameter<Reactable> reactable;

	public new class Instance : GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void PollForReactables(Navigator.ActiveTransition transition)
		{
			if (this.IsReacting())
			{
				return;
			}
			if (this.justReacted)
			{
				this.justReacted = false;
			}
			else
			{
				this.lastReactable = null;
			}
			int num = Grid.PosToCell(base.smi.gameObject);
			List<ScenePartitionerEntry> list = GameScenePartitioner.Instance.ReserveList();
			GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(num).x, Grid.CellToXY(num).y, 1, 1, GameScenePartitioner.Instance.objectLayers[0], list);
			for (int i = 0; i < list.Count; i++)
			{
				Reactable reactable = list[i].obj as Reactable;
				if (reactable != null && reactable != this.lastReactable)
				{
					if (reactable.CanBegin(base.gameObject, transition))
					{
						this.justReacted = true;
						this.lastReactable = reactable;
						base.sm.reactable.Set(reactable, base.smi);
						base.smi.GoTo(base.sm.reacting);
						break;
					}
				}
			}
			GameScenePartitioner.Instance.ReleaseList(list);
		}

		public void StopReaction()
		{
			base.smi.GoTo(base.sm.idle);
		}

		public bool IsReacting()
		{
			return base.smi.IsInsideState(base.sm.reacting);
		}

		private bool justReacted;

		private Reactable lastReactable;
	}
}
