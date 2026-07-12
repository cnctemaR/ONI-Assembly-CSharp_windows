using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class ReactionMonitor : GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		base.serializable = StateMachine.SerializeType.Never;
		this.idle.Enter("ClearReactable", delegate(ReactionMonitor.Instance smi)
		{
			this.reactable.Set(null, smi, false);
		}).TagTransition(GameTags.Dead, this.dead, false);
		this.reacting.Enter("Reactable.Begin", delegate(ReactionMonitor.Instance smi)
		{
			this.reactable.Get(smi).Begin(smi.gameObject);
		}).Enter(delegate(ReactionMonitor.Instance smi)
		{
			smi.master.Trigger(-909573545, null);
		}).Enter("Reactable.AddChorePreventionTag", delegate(ReactionMonitor.Instance smi)
		{
			if (this.reactable.Get(smi).preventChoreInterruption)
			{
				smi.GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption, false);
			}
		})
			.Update("Reactable.Update", delegate(ReactionMonitor.Instance smi, float dt)
			{
				this.reactable.Get(smi).Update(dt);
			}, UpdateRate.SIM_200ms, false)
			.Exit(delegate(ReactionMonitor.Instance smi)
			{
				smi.master.Trigger(824899998, null);
			})
			.Exit("Reactable.End", delegate(ReactionMonitor.Instance smi)
			{
				this.reactable.Get(smi).End();
			})
			.Exit("Reactable.RemoveChorePreventionTag", delegate(ReactionMonitor.Instance smi)
			{
				if (this.reactable.Get(smi).preventChoreInterruption)
				{
					smi.GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
				}
			})
			.EventTransition(GameHashes.NavigationFailed, this.idle, null)
			.TagTransition(GameTags.Dying, this.dead, false)
			.TagTransition(GameTags.Dead, this.dead, false);
		this.dead.DoNothing();
	}

	private static bool ShouldReact(ReactionMonitor.Instance smi)
	{
		return smi.ImmediateReactable != null;
	}

	public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State idle;

	public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State reacting;

	public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State dead;

	public StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.ObjectParameter<Reactable> reactable;

	public class Def : StateMachine.BaseDef
	{
		public ObjectLayer ReactionLayer;
	}

	public new class Instance : GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.GameInstance
	{
		public Reactable ImmediateReactable { get; private set; }

		public Instance(IStateMachineTarget master, ReactionMonitor.Def def)
			: base(master, def)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
			this.lastReactTimes = new Dictionary<HashedString, float>();
			this.oneshotReactables = new List<Reactable>();
		}

		public bool CanReact(Emote e)
		{
			return this.animController != null && e.IsValidForController(this.animController);
		}

		public bool TryReact(Reactable reactable, float clockTime, Navigator.ActiveTransition transition = null)
		{
			if (reactable == null)
			{
				return false;
			}
			if (this.lastReactTimes.ContainsKey(reactable.id) && clockTime - this.lastReactTimes[reactable.id] < reactable.localCooldown)
			{
				return false;
			}
			if (!reactable.CanBegin(base.gameObject, transition))
			{
				return false;
			}
			this.lastReactTimes[reactable.id] = clockTime;
			base.sm.reactable.Set(reactable, base.smi, false);
			base.smi.GoTo(base.sm.reacting);
			return true;
		}

		public void PollForReactables(Navigator.ActiveTransition transition)
		{
			if (this.IsReacting())
			{
				return;
			}
			for (int i = this.oneshotReactables.Count - 1; i >= 0; i--)
			{
				Reactable reactable = this.oneshotReactables[i];
				if (reactable.IsExpired())
				{
					reactable.Cleanup();
					this.oneshotReactables.RemoveAt(i);
				}
			}
			Vector2I vector2I = Grid.CellToXY(Grid.PosToCell(base.smi.gameObject));
			ScenePartitionerLayer scenePartitionerLayer = GameScenePartitioner.Instance.objectLayers[(int)base.def.ReactionLayer];
			ListPool<ScenePartitionerEntry, ReactionMonitor>.PooledList pooledList = ListPool<ScenePartitionerEntry, ReactionMonitor>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(vector2I.x, vector2I.y, 1, 1, scenePartitionerLayer, pooledList);
			float time = GameClock.Instance.GetTime();
			for (int j = 0; j < pooledList.Count; j++)
			{
				Reactable reactable2 = pooledList[j].obj as Reactable;
				if (this.TryReact(reactable2, time, transition))
				{
					break;
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
					break;
				}
			}
			base.smi.GoTo(base.sm.idle);
		}

		public bool IsReacting()
		{
			return base.smi.IsInsideState(base.sm.reacting);
		}

		public SelfEmoteReactable AddSelfEmoteReactable(GameObject target, HashedString reactionId, Emote emote, bool isOneShot, ChoreType choreType, float globalCooldown = 0f, float localCooldown = 20f, float lifeSpan = float.NegativeInfinity, float maxInitialDelay = 0f, List<Reactable.ReactablePrecondition> emotePreconditions = null)
		{
			if (!this.CanReact(emote))
			{
				return null;
			}
			SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(target, reactionId, choreType, globalCooldown, localCooldown, lifeSpan, maxInitialDelay);
			selfEmoteReactable.SetEmote(emote);
			int num = 0;
			while (emotePreconditions != null && num < emotePreconditions.Count)
			{
				selfEmoteReactable.AddPrecondition(emotePreconditions[num]);
				num++;
			}
			if (isOneShot)
			{
				this.AddOneshotReactable(selfEmoteReactable);
			}
			return selfEmoteReactable;
		}

		public SelfEmoteReactable AddSelfEmoteReactable(GameObject target, string reactionId, string emoteAnim, bool isOneShot, ChoreType choreType, float globalCooldown = 0f, float localCooldown = 20f, float maxTriggerTime = float.NegativeInfinity, float maxInitialDelay = 0f, List<Reactable.ReactablePrecondition> emotePreconditions = null)
		{
			Emote emote = new Emote(null, reactionId, new EmoteStep[]
			{
				new EmoteStep
				{
					anim = "react"
				}
			}, emoteAnim);
			return this.AddSelfEmoteReactable(target, reactionId, emote, isOneShot, choreType, globalCooldown, localCooldown, maxTriggerTime, maxInitialDelay, emotePreconditions);
		}

		public void AddOneshotReactable(SelfEmoteReactable reactable)
		{
			if (reactable == null)
			{
				return;
			}
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
					this.oneshotReactables.RemoveAt(i);
					return;
				}
			}
		}

		public void CancelOneShotReactables(Emote reactionEmote)
		{
			for (int i = this.oneshotReactables.Count - 1; i >= 0; i--)
			{
				EmoteReactable emoteReactable = this.oneshotReactables[i] as EmoteReactable;
				if (emoteReactable != null && emoteReactable.emote == reactionEmote)
				{
					emoteReactable.Cleanup();
					this.oneshotReactables.RemoveAt(i);
				}
			}
		}

		private KBatchedAnimController animController;

		private Dictionary<HashedString, float> lastReactTimes;

		private List<Reactable> oneshotReactables;
	}
}
