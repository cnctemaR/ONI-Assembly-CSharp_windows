using System;
using UnityEngine;

public class TakeOffHatChore : Chore<TakeOffHatChore.StatesInstance>
{
	public TakeOffHatChore(IStateMachineTarget target, ChoreType chore_type)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, null, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new TakeOffHatChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.GameInstance
	{
		public StatesInstance(TakeOffHatChore master, GameObject duplicant)
			: base(master)
		{
			base.sm.duplicant.Set(duplicant, base.smi);
		}
	}

	public class States : GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.remove_hat_pre;
			base.Target(this.duplicant);
			this.remove_hat_pre.Enter(delegate(TakeOffHatChore.StatesInstance smi)
			{
				if (this.duplicant.Get(smi).GetComponent<MinionResume>().CurrentHat != null)
				{
					smi.GoTo(this.remove_hat);
				}
				else
				{
					smi.GoTo(this.complete);
				}
			});
			this.remove_hat.ToggleAnims("anim_hat_kanim", 0f).PlayAnim("hat_off").OnAnimQueueComplete(this.complete);
			this.complete.Enter(delegate(TakeOffHatChore.StatesInstance smi)
			{
				smi.master.GetComponent<MinionResume>().RemoveHat();
			}).ReturnSuccess();
		}

		public StateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.TargetParameter duplicant;

		public GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.State remove_hat_pre;

		public GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.State remove_hat;

		public GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.State complete;
	}
}
