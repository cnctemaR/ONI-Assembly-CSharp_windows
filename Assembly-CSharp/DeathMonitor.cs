using System;
using STRINGS;

public class DeathMonitor : GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.alive;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.alive.ParamTransition<Death>(this.death, this.dying_duplicant, (DeathMonitor.Instance smi, Death p) => p != null && smi.IsDuplicant).ParamTransition<Death>(this.death, this.dying_creature, (DeathMonitor.Instance smi, Death p) => p != null && !smi.IsDuplicant);
		this.dying_duplicant.ToggleAnims("anim_emotes_default_kanim", 0f, "").ToggleTag(GameTags.Dying).ToggleChore((DeathMonitor.Instance smi) => new DieChore(smi.master, this.death.Get(smi)), this.die);
		this.dying_creature.ToggleBehaviour(GameTags.Creatures.Die, (DeathMonitor.Instance smi) => true, delegate(DeathMonitor.Instance smi)
		{
			smi.GoTo(this.dead_creature);
		});
		this.die.ToggleTag(GameTags.Dying).Enter("Die", delegate(DeathMonitor.Instance smi)
		{
			Death death = this.death.Get(smi);
			if (smi.IsDuplicant)
			{
				DeathMessage deathMessage = new DeathMessage(smi.gameObject, death);
				KFMOD.PlayOneShot(GlobalAssets.GetSound("Death_Notification_localized", false), smi.master.transform.GetPosition(), 1f);
				KFMOD.PlayUISound(GlobalAssets.GetSound("Death_Notification_ST", false));
				Messenger.Instance.QueueMessage(deathMessage);
			}
		}).TriggerOnExit(GameHashes.Died, null)
			.GoTo(this.dead);
		this.dead.ToggleAnims("anim_emotes_default_kanim", 0f, "").DefaultState(this.dead.ground).ToggleTag(GameTags.Dead)
			.Enter(delegate(DeathMonitor.Instance smi)
			{
				smi.ApplyDeath();
				Game.Instance.Trigger(282337316, smi.gameObject);
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
				smi.GetComponent<KAnimControllerBase>().Play(death2.loopAnim, KAnim.PlayMode.Loop, 1f, 0f);
			}
		}).EventTransition(GameHashes.OnStore, this.dead.carried, (DeathMonitor.Instance smi) => smi.IsDuplicant && smi.HasTag(GameTags.Stored));
		this.dead.carried.ToggleAnims("anim_dead_carried_kanim", 0f, "").PlayAnim("idle_default", KAnim.PlayMode.Loop).EventTransition(GameHashes.OnStore, this.dead.ground, (DeathMonitor.Instance smi) => !smi.HasTag(GameTags.Stored));
		this.dead_creature.ToggleTag(GameTags.Dead).PlayAnim("idle_dead", KAnim.PlayMode.Loop);
	}

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State alive;

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State dying_duplicant;

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State dying_creature;

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>.State die;

	public DeathMonitor.Dead dead;

	public DeathMonitor.Dead dead_creature;

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
			base.sm.death.Set(death, base.smi, false);
		}

		public void PickedUp(object data = null)
		{
			if (data is Storage || (data != null && (bool)data))
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
				Game.Instance.assignmentManager.RemoveFromAllGroups(base.GetComponent<MinionIdentity>().assignableProxy.Get());
				base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Dead, base.smi.sm.death.Get(base.smi));
				float num = 600f - GameClock.Instance.GetTimeSinceStartOfReport();
				ReportManager.Instance.ReportValue(ReportManager.ReportType.PersonalTime, num, string.Format(UI.ENDOFDAYREPORT.NOTES.PERSONAL_TIME, DUPLICANTS.CHORES.IS_DEAD_TASK), base.smi.master.gameObject.GetProperName());
				Pickupable component = base.GetComponent<Pickupable>();
				if (component != null)
				{
					component.RegisterListeners();
				}
			}
			base.GetComponent<KPrefabID>().AddTag(GameTags.Corpse, false);
		}

		private bool isDuplicant;
	}
}
