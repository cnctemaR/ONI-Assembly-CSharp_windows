using System;
using Klei.AI;
using UnityEngine;

public class SneezeMonitor : GameStateMachine<SneezeMonitor, SneezeMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.ParamTransition<bool>(this.isSneezy, this.Sneezy.idle, (SneezeMonitor.Instance smi, bool p) => p);
		this.taking_medicine.TagTransition(GameTags.TakingMedicine, this.Sneezy.idle, true);
		this.Sneezy.idle.ScheduleGoTo((SneezeMonitor.Instance smi) => smi.NextSneezeTime(), this.Sneezy.sneeze_pre).ParamTransition<bool>(this.isSneezy, this.idle, (SneezeMonitor.Instance smi, bool p) => !p).TagTransition(GameTags.TakingMedicine, this.taking_medicine, false);
		this.Sneezy.sneeze_pre.ToggleScheduleCallback("Sneeze", (SneezeMonitor.Instance smi) => 2f, delegate(SneezeMonitor.Instance instanceObject)
		{
			AcousticDisturbance.Emit(instanceObject.master.gameObject, 3);
		}).ToggleChore((SneezeMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, "anim_sneeze_kanim", SneezeMonitor.SneezeAnims, null), this.Sneezy.sneeze_pst).ScheduleGoTo(5f, this.Sneezy.sneeze_pst);
		this.Sneezy.sneeze_pst.Enter(delegate(SneezeMonitor.Instance smi)
		{
			smi.GoTo(this.Sneezy.idle);
		});
	}

	private static readonly HashedString[] SneezeAnims = new HashedString[] { "sneeze", "sneeze_pst" };

	public StateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.BoolParameter isSneezy = new StateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.BoolParameter(false);

	public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State taking_medicine;

	public SneezeMonitor.SneezyStates Sneezy;

	public const float SINGLE_SNEEZE_TIME = 70f;

	public const float SNEEZE_TIME_VARIANCE = 0.3f;

	public class SneezyStates : GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State idle;

		public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State sneeze_pre;

		public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State sneeze_pst;
	}

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

		public float NextSneezeTime()
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

		private StatusItem statusItem;
	}
}
