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
		}).Update("Reactable.Update", delegate(ReactionMonitor.Instance smi, float dt)
		{
			this.reactable.Get(smi).Update(dt);
		}, UpdateRate.SIM_200ms, false).Exit("Reactable.End", delegate(ReactionMonitor.Instance smi)
		{
			this.reactable.Get(smi).End();
		})
			.EventTransition(GameHashes.NavigationFailed, this.idle, null)
			.Enter("Reactable.AddChorePreventionTag", delegate(ReactionMonitor.Instance smi)
			{
				if (this.reactable.Get(smi).preventChoreInterruption)
				{
					smi.GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption, false);
				}
			})
			.Exit("Reactable.RemoveChorePreventionTag", delegate(ReactionMonitor.Instance smi)
			{
				if (this.reactable.Get(smi).preventChoreInterruption)
				{
					smi.GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
				}
			})
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
			this.lastReactTimes = new Dictionary<HashedString, float>();
			this.oneshotReactables = new List<Reactable>();
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
			for (int i = this.oneshotReactables.Count - 1; i >= 0; i--)
			{
				Reactable reactable = this.oneshotReactables[i];
				if (reactable.IsExpired())
				{
					reactable.Cleanup();
				}
			}
			int num = Grid.PosToCell(base.smi.gameObject);
			ListPool<ScenePartitionerEntry, ReactionMonitor>.PooledList pooledList = ListPool<ScenePartitionerEntry, ReactionMonitor>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(num).x, Grid.CellToXY(num).y, 1, 1, GameScenePartitioner.Instance.objectLayers[0], pooledList);
			for (int j = 0; j < pooledList.Count; j++)
			{
				Reactable reactable2 = pooledList[j].obj as Reactable;
				if (reactable2 != null && reactable2 != this.lastReactable)
				{
					if (!this.lastReactTimes.ContainsKey(reactable2.id) || GameClock.Instance.GetTime() - this.lastReactTimes[reactable2.id] >= reactable2.minReactorTime)
					{
						if (reactable2.CanBegin(base.gameObject, transition))
						{
							this.justReacted = true;
							this.lastReactable = reactable2;
							this.lastReactTimes[reactable2.id] = GameClock.Instance.GetTime();
							base.sm.reactable.Set(reactable2, base.smi);
							base.smi.GoTo(base.sm.reacting);
							break;
						}
					}
				}
			}
			pooledList.Recycle();
		}

		public void StopReaction()
		{
			for (int i = this.oneshotReactables.Count - 1; i >= 0; i--)
			{
				if (base.sm.reactable.Get(base.smi) == this.oneshotReactables[i])
				{
					this.oneshotReactables[i].Cleanup();
					this.oneshotReactables.RemoveAt(i);
				}
			}
			base.smi.GoTo(base.sm.idle);
		}

		public bool IsReacting()
		{
			return base.smi.IsInsideState(base.sm.reacting);
		}

		public void AddOneshotReactable(SelfEmoteReactable reactable)
		{
			this.oneshotReactables.Add(reactable);
		}

		public void CancelOneShotReactable(SelfEmoteReactable cancel_target)
		{
			for (int i = this.oneshotReactables.Count - 1; i >= 0; i--)
			{
				Reactable reactable = this.oneshotReactables[i];
				if (cancel_target == reactable)
				{
					reactable.Cleanup();
					break;
				}
			}
		}

		private bool justReacted;

		private Reactable lastReactable;

		private Dictionary<HashedString, float> lastReactTimes;

		private List<Reactable> oneshotReactables;
	}
}
