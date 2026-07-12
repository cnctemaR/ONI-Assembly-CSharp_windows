using System;
using Klei.AI;
using UnityEngine;

public class SneezeMonitor : GameStateMachine<SneezeMonitor, SneezeMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.ParamTransition<bool>(this.isSneezy, this.sneezy, (SneezeMonitor.Instance smi, bool p) => p);
		this.sneezy.ParamTransition<bool>(this.isSneezy, this.idle, (SneezeMonitor.Instance smi, bool p) => !p).ToggleReactable((SneezeMonitor.Instance smi) => smi.GetReactable());
	}

	public StateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.BoolParameter isSneezy = new StateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.BoolParameter(false);

	public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State taking_medicine;

	public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State sneezy;

	public const float SINGLE_SNEEZE_TIME_MINOR = 140f;

	public const float SINGLE_SNEEZE_TIME_MAJOR = 70f;

	public const float SNEEZE_TIME_VARIANCE = 0.3f;

	public const float SHORT_SNEEZE_THRESHOLD = 5f;

	public new class Instance : GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.sneezyness = Db.Get().Attributes.Sneezyness.Lookup(master.gameObject);
			this.OnSneezyChange();
			AttributeInstance attributeInstance = this.sneezyness;
			attributeInstance.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance.OnDirty, new global::System.Action(this.OnSneezyChange));
		}

		public override void StopSM(string reason)
		{
			AttributeInstance attributeInstance = this.sneezyness;
			attributeInstance.OnDirty = (global::System.Action)Delegate.Remove(attributeInstance.OnDirty, new global::System.Action(this.OnSneezyChange));
			base.StopSM(reason);
		}

		public float NextSneezeInterval()
		{
			if (this.sneezyness.GetTotalValue() <= 0f)
			{
				return 70f;
			}
			float num = (this.IsMinorSneeze() ? 140f : 70f) / this.sneezyness.GetTotalValue();
			return global::UnityEngine.Random.Range(num * 0.7f, num * 1.3f);
		}

		public bool IsMinorSneeze()
		{
			return this.sneezyness.GetTotalValue() <= 5f;
		}

		private void OnSneezyChange()
		{
			base.smi.sm.isSneezy.Set(this.sneezyness.GetTotalValue() > 0f, base.smi);
		}

		public Reactable GetReactable()
		{
			if (this.IsMinorSneeze())
			{
				float num = this.NextSneezeInterval();
				return new SelfEmoteReactable(base.master.gameObject, "Sneeze", Db.Get().ChoreTypes.Cough, "anim_sneeze_kanim", 0f, num, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "sneeze_short",
					startcb = new Action<GameObject>(this.TriggerDisurbance)
				}).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "sneeze_short_pst",
					finishcb = new Action<GameObject>(this.ResetSneeze)
				});
			}
			float num2 = this.NextSneezeInterval();
			return new SelfEmoteReactable(base.master.gameObject, "Sneeze", Db.Get().ChoreTypes.Cough, "anim_sneeze_kanim", 0f, num2, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "sneeze",
				startcb = new Action<GameObject>(this.TriggerDisurbance)
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "sneeze_pst",
				finishcb = new Action<GameObject>(this.ResetSneeze)
			});
		}

		private void TriggerDisurbance(GameObject go)
		{
			if (this.IsMinorSneeze())
			{
				AcousticDisturbance.Emit(go, 2);
				return;
			}
			AcousticDisturbance.Emit(go, 3);
		}

		private void ResetSneeze(GameObject go)
		{
			base.smi.GoTo(base.sm.idle);
		}

		private AttributeInstance sneezyness;

		private StatusItem statusItem;
	}
}
