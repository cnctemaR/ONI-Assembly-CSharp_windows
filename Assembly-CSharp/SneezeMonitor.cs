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

	private static readonly HashedString[] SneezeAnims = new HashedString[] { "sneeze", "sneeze_pst" };

	public StateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.BoolParameter isSneezy = new StateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.BoolParameter(false);

	public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State taking_medicine;

	public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State sneezy;

	public const float SINGLE_SNEEZE_TIME = 70f;

	public const float SNEEZE_TIME_VARIANCE = 0.3f;

	public new class Instance : GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(master.gameObject);
			this.OnSneezyChange();
			AttributeInstance attributeInstance2 = attributeInstance;
			attributeInstance2.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance2.OnDirty, new global::System.Action(this.OnSneezyChange));
		}

		public override void StopSM(string reason)
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(base.master.gameObject);
			AttributeInstance attributeInstance2 = attributeInstance;
			attributeInstance2.OnDirty = (global::System.Action)Delegate.Remove(attributeInstance2.OnDirty, new global::System.Action(this.OnSneezyChange));
			base.StopSM(reason);
		}

		public float NextSneezeInterval()
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(base.master.gameObject);
			if (attributeInstance.GetTotalValue() <= 0f)
			{
				return 70f;
			}
			float num = 70f / attributeInstance.GetTotalValue();
			return global::UnityEngine.Random.Range(num * 0.7f, num * 1.3f);
		}

		private void OnSneezyChange()
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(base.master.gameObject);
			base.smi.sm.isSneezy.Set(attributeInstance.GetTotalValue() > 0f, base.smi);
		}

		public Reactable GetReactable()
		{
			float num = this.NextSneezeInterval();
			GameObject gameObject = base.master.gameObject;
			HashedString hashedString = "Sneeze";
			ChoreType cough = Db.Get().ChoreTypes.Cough;
			HashedString hashedString2 = "anim_sneeze_kanim";
			float num2 = num;
			return new SelfEmoteReactable(gameObject, hashedString, cough, hashedString2, 0f, num2, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
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
			AcousticDisturbance.Emit(go, 3);
		}

		private void ResetSneeze(GameObject go)
		{
			base.smi.GoTo(base.sm.idle);
		}

		private StatusItem statusItem;
	}
}
