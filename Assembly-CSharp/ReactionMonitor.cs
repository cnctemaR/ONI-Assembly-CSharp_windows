using System;
using System.Collections.Generic;

public class ReactionMonitor : GameStateMachine<ReactionMonitor, ReactionMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		base.serializable = false;
		this.idle.ToggleSchedulePeriodic("PollForReactables", 1f, delegate(ReactionMonitor.Instance smi)
		{
			smi.PollForReactables();
		}).ToggleSchedulePeriodic("ClearRecentReactablesList", 20f, delegate(ReactionMonitor.Instance smi)
		{
			smi.recently_reacted_to.Clear();
		}).Enter(delegate(ReactionMonitor.Instance smi)
		{
			this.reactable.Set(null, smi);
		})
			.Update(delegate(ReactionMonitor.Instance smi)
			{
				smi.PollForReactables();
			});
		this.reacting.ToggleChore((ReactionMonitor.Instance smi) => smi.CreateEmoteChore(this.reactable.Get(smi)), this.idle, false);
	}

	public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, object>.State reacting;

	public StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, object>.ObjectParameter<Reactable> reactable;

	public new class Instance : GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void PollForReactables()
		{
			int num = Grid.PosToCell(base.smi.gameObject);
			List<ScenePartitionerEntry> list = GameScenePartitioner.Instance.ReserveList();
			GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(num).x, Grid.CellToXY(num).y, 3, 2, GameScenePartitioner.Instance.objectLayerMasks[0].mask, list);
			for (int i = 0; i < list.Count; i++)
			{
				Reactable reactable = list[i].obj as Reactable;
				if (reactable != null)
				{
					if (!(reactable.sourceGameObject == base.gameObject))
					{
						if (!this.recently_reacted_to.Contains(reactable))
						{
							this.recently_reacted_to.Add(reactable);
							base.sm.reactable.Set(reactable, base.smi);
							base.smi.GoTo(base.sm.reacting);
						}
					}
				}
			}
			GameScenePartitioner.Instance.ReleaseList(list);
		}

		public Chore CreateEmoteChore(Reactable reactable)
		{
			return new ReactEmoteChore(base.smi.master, Db.Get().ChoreTypes.EmoteHighPriority, reactable, "anim_putoff_kanim", new HashedString[] { "putoff_pre", "putoff_loop", "putoff_pst" }, KAnim.PlayMode.Once, null);
		}

		public List<Reactable> recently_reacted_to = new List<Reactable>();
	}
}
