using System;

public class DeathMonitor : GameStateMachine<DeathMonitor, DeathMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = true;
		this.satisfied.ParamTransition<Death>(this.death, this.dying, (DeathMonitor.Instance smi, Death p) => p != null);
		this.dying.ToggleChore((DeathMonitor.Instance smi) => new DieChore(smi.master, this.death.Get(smi)), this.die, false);
		this.die.Enter("Die", delegate(DeathMonitor.Instance smi)
		{
			Death death = this.death.Get(smi);
			smi.GetComponent<Health>().KillImmediate(death);
			DeathMessage deathMessage = new DeathMessage(smi.gameObject, death);
			KFMOD.PlayOneShot(GlobalAssets.GetSound("Death_Notification_localized", false), smi.master.transform.position);
			KFMOD.PlayOneShot(GlobalAssets.GetSound("Death_Notification_ST", false));
			Messenger.Instance.QueueMessage(deathMessage);
		}).GoTo(this.dead);
		this.dead.Enter("ApplyDeath", delegate(DeathMonitor.Instance smi)
		{
			Death death2 = this.death.Get(smi);
			if (death2 == null)
			{
				death2 = Db.Get().Deaths.Generic;
			}
			smi.GetComponent<KAnimControllerBase>().Play(death2.loopAnim, KAnim.PlayMode.Once, 1f, 0f);
		});
	}

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, object>.State dying;

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, object>.State die;

	public GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, object>.State dead;

	public StateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, object>.ResourceParameter<Death> death;

	public new class Instance : GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void Kill(Death death)
		{
			base.sm.death.Set(death, base.smi);
		}
	}
}
