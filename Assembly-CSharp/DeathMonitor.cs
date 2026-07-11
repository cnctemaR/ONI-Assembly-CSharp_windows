using System;

public class DeathMonitor : GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.alive;
		base.serializable = true;
		this.alive.ParamTransition<Death>(this.death, this.dying_duplicant, (DeathMonitor.Instance smi, Death p) => p != null && smi.IsDuplicant).ParamTransition<Death>(this.death, this.dying_creature, (DeathMonitor.Instance smi, Death p) => p != null && !smi.IsDuplicant);
		this.dying_duplicant.ToggleTag(GameTags.Dying).ToggleChore((DeathMonitor.Instance smi) => new DieChore(smi.master, this.death.Get(smi)), this.die);
		this.dying_creature.ToggleBehaviour(GameTags.Creatures.Die, (DeathMonitor.Instance smi) => true, delegate(DeathMonitor.Instance smi)
		{
			smi.GoTo(this.dead);
		});
		this.die.ToggleTag(GameTags.Dying).Enter("Die", delegate(DeathMonitor.Instance smi)
		{
			Death death = this.death.Get(smi);
			if (smi.IsDuplicant)
			{
				DeathMessage deathMessage = new DeathMessage(smi.gameObject, death);
				KFMOD.PlayOneShot(GlobalAssets.GetSound("Death_Notification_localized", false), smi.master.transform.GetPosition());
				KFMOD.PlayOneShot(GlobalAssets.GetSound("Death_Notification_ST", false));
				Messenger.Instance.QueueMessage(deathMessage);
			}
		}).GoTo(this.dead);
		this.dead.defaultState = this.dead.ground.TriggerOnEnter(GameHashes.Died, null).ToggleTag(GameTags.Dead).ToggleAnims("anim_emotes_default_kanim", 0f)
			.Enter(delegate(DeathMonitor.Instance smi)
			{
				smi.ApplyDeath();
			});
		this.dead.ground.Enter(delegate(DeathMonitor.Instance smi)
		{
			Death death2 = this.death.Get(smi);
			if (death2 == null)
			{
				death2 = Db.Get().Deaths.Generic;
			}
			if (smi.IsDuplicant)
			{
				smi.GetComponent<KAnimControllerBase>().Play(death2.loopAnim, KAnim.PlayMode.Once, 1f, 0f);
			}
		}).Exit(delegate(DeathMonitor.Instance smi)
		{
			smi.Unsubscribe(856640610, new Action<object>(smi.PickedUp));
		});
		this.dead.carried.ToggleAnims("anim_dead_carried_kanim", 0f).Enter("ApplyDeath", delegate(DeathMonitor.Instance smi)
		{
			smi.Get<KBatchedAnimController>().Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
		}).Exit(delegate(DeathMonitor.Instance smi)
		{
			smi.Get<KBatchedAnimController>().ClearQueue();
		})
			.EventTransition(GameHashes.OnUnstored, this.dead.ground, null);
	}

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State alive;

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State dying_duplicant;

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State dying_creature;

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State die;

	public DeathMonitor.Dead dead;

	public StateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.ResourceParameter<Death> death;

	public class Def : StateMachine.BaseDef
	{
	}

	public class Dead : GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State
	{
		public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State ground;

		public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State carried;
	}

	public new class Instance : GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, DeathMonitor.Def def)
			: base(master, def)
		{
			this.isDuplicant = base.GetComponent<MinionIdentity>();
		}

		public bool IsDuplicant
		{
			get
			{
				return this.isDuplicant;
			}
		}

		public void Kill(Death death)
		{
			base.sm.death.Set(death, base.smi);
		}

		public void PickedUp(object data = null)
		{
			bool flag = data is Storage || (data != null && (bool)data);
			if (flag)
			{
				base.smi.GoTo(base.sm.dead.carried);
			}
		}

		public bool IsDead()
		{
			return base.smi.IsInsideState(base.smi.sm.dead);
		}

		public void ApplyDeath()
		{
			if (this.isDuplicant)
			{
				base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Dead, base.smi.sm.death.Get(base.smi));
				Pickupable component = base.GetComponent<Pickupable>();
				if (component != null)
				{
					component.RegisterListeners();
				}
			}
			base.GetComponent<KPrefabID>().AddTag(GameTags.Corpse);
		}

		private bool isDuplicant;
	}
}
