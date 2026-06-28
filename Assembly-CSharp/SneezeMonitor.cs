using System;
using Klei.AI;
using UnityEngine;

public class SneezeMonitor : GameStateMachine<SneezeMonitor, SneezeMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.Sneezy.idle.ScheduleGoTo(global::UnityEngine.Random.Range(45f, 90f), this.Sneezy.sneeze_pre);
		this.Sneezy.sneeze_pre.ToggleScheduleCallback("Sneeze", (SneezeMonitor.Instance smi) => 2f, delegate(SneezeMonitor.Instance instanceObject)
		{
			AcousticDisturbance.Emit(instanceObject.master.gameObject, 3);
		}).ToggleChore((SneezeMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, "anim_sneeze_kanim", SneezeMonitor.SneezeAnims, null), this.Sneezy.sneeze_pst).ScheduleGoTo(5f, this.Sneezy.sneeze_pst);
		this.Sneezy.sneeze_pst.Enter(delegate(SneezeMonitor.Instance smi)
		{
			smi.GoTo(this.Sneezy.idle);
		});
	}

	private static readonly HashedString[] SneezeAnims = new HashedString[] { "sneeze" };

	public GameStateMachine<SneezeMonitor, SneezeMonitor.Instance, IStateMachineTarget, object>.State idle;

	public SneezeMonitor.SneezyStates Sneezy;

	public const float SNEEZE_INTERVAL_MIN = 45f;

	public const float SNEEZE_INTERVAL_MAX = 90f;

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

		private void OnSneezyChange()
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(base.master.gameObject);
			if (attributeInstance.GetTotalValue() > 0f)
			{
				if (base.smi.GetCurrentState() != base.smi.sm.Sneezy.idle)
				{
					base.smi.GoTo(base.smi.sm.Sneezy.idle);
				}
			}
			else if (base.smi.GetCurrentState() != base.smi.sm.idle)
			{
				base.smi.GoTo(base.smi.sm.idle);
			}
		}

		private StatusItem statusItem;
	}
}
