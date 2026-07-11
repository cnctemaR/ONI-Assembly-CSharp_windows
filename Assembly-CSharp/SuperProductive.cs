using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SuperProductive : GameStateMachine<SuperProductive, SuperProductive.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.neutral;
		this.root.EventTransition(GameHashes.Died, null, null);
		this.neutral.TagTransition(GameTags.Overjoyed, this.overjoyed, false);
		this.overjoyed.TagTransition(GameTags.Overjoyed, this.neutral, true).ToggleStatusItem(Db.Get().DuplicantStatusItems.BeingProductive, null).Enter(delegate(SuperProductive.Instance smi)
		{
			if (PopFXManager.Instance != null)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, DUPLICANTS.TRAITS.SUPERPRODUCTIVE.NAME, smi.master.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
			}
			smi.fx = new SuperProductiveFX.Instance(smi.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f));
			smi.fx.StartSM();
		})
			.Exit(delegate(SuperProductive.Instance smi)
			{
				smi.fx.sm.destroyFX.Trigger(smi.fx);
			})
			.DefaultState(this.overjoyed.idle);
		this.overjoyed.idle.EventTransition(GameHashes.StartWork, this.overjoyed.working, null);
		this.overjoyed.working.ScheduleGoTo(0.33f, this.overjoyed.superProductive);
		this.overjoyed.superProductive.Enter(delegate(SuperProductive.Instance smi)
		{
			Worker component = smi.GetComponent<Worker>();
			if (component.state == Worker.State.Working)
			{
				float num = component.workable.WorkTimeRemaining;
				Diggable component2 = component.workable.GetComponent<Diggable>();
				if (component2 != null)
				{
					num = Diggable.GetApproximateDigTime(Grid.PosToCell(component.workable));
				}
				if (num > 1f && smi.ShouldSkipWork() && component.InstantlyFinish())
				{
					smi.ReactSuperProductive();
					smi.fx.sm.wasProductive.Trigger(smi.fx);
				}
			}
			smi.GoTo(this.overjoyed.idle);
		});
	}

	public GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State neutral;

	public SuperProductive.OverjoyedStates overjoyed;

	public class OverjoyedStates : GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State idle;

		public GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State working;

		public GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State superProductive;
	}

	public new class Instance : GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool ShouldSkipWork()
		{
			float num = global::UnityEngine.Random.Range(0f, 100f);
			return num <= TRAITS.JOY_REACTIONS.SUPER_PRODUCTIVE.INSTANT_SUCCESS_CHANCE;
		}

		public void ReactSuperProductive()
		{
			ReactionMonitor.Instance smi = base.gameObject.GetSMI<ReactionMonitor.Instance>();
			if (smi != null)
			{
				SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.gameObject, "SuperProductive", Db.Get().ChoreTypes.EmoteHighPriority, "anim_productive_kanim", 0f, 1f, 1f);
				selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
				{
					anim = "productive"
				});
				smi.AddOneshotReactable(selfEmoteReactable);
			}
		}

		public SuperProductiveFX.Instance fx;
	}
}
