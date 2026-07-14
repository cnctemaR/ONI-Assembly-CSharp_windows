using System;

public class BeIncapacitatedSuffocatingChore : Chore<BeIncapacitatedSuffocatingChore.StatesInstance>
{
	public BeIncapacitatedSuffocatingChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.BeIncapacitated, master, master.GetComponent<ChoreProvider>(), true, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new BeIncapacitatedSuffocatingChore.StatesInstance(this);
		base.smi.isDrowning = Grid.IsLiquid(Grid.PosToCell(base.smi));
	}

	public class StatesInstance : GameStateMachine<BeIncapacitatedSuffocatingChore.States, BeIncapacitatedSuffocatingChore.StatesInstance, BeIncapacitatedSuffocatingChore, object>.GameInstance
	{
		public StatesInstance(BeIncapacitatedSuffocatingChore master)
			: base(master)
		{
		}

		public bool isDrowning;
	}

	public class States : GameStateMachine<BeIncapacitatedSuffocatingChore.States, BeIncapacitatedSuffocatingChore.StatesInstance, BeIncapacitatedSuffocatingChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.incapacitated;
			this.root.ToggleAnims(new Func<BeIncapacitatedSuffocatingChore.StatesInstance, HashedString>(this.GetSuffocatingAnimSet)).ToggleStatusItem(Db.Get().DuplicantStatusItems.SuffocatingIncapacitated, (BeIncapacitatedSuffocatingChore.StatesInstance smi) => smi.master.gameObject.GetSMI<SuffocationMonitor.Instance>());
			this.incapacitated.EventHandler(GameHashes.Died, delegate(BeIncapacitatedSuffocatingChore.StatesInstance smi)
			{
				smi.SetStatus(StateMachine.Status.Failed);
				smi.StopSM("died");
			}).PlayAnim("incapacitate_pre").QueueAnim("incapacitate_loop", true, null)
				.ToggleChore((BeIncapacitatedSuffocatingChore.StatesInstance smi) => new ResuscitateSuffocatedChore(smi.master, this.masterTarget.Get(smi)), this.resuscitated, this.fail)
				.EventTransition(GameHashes.IncapacitationRecovery, this.resuscitated, null);
			this.fail.ReturnFailure();
			this.resuscitated.ReturnSuccess();
		}

		private HashedString GetSuffocatingAnimSet(BeIncapacitatedSuffocatingChore.StatesInstance smi)
		{
			if (smi.isDrowning)
			{
				return "anim_incapacitated_drowning_kanim";
			}
			return "anim_incapacitated_kanim";
		}

		public GameStateMachine<BeIncapacitatedSuffocatingChore.States, BeIncapacitatedSuffocatingChore.StatesInstance, BeIncapacitatedSuffocatingChore, object>.State incapacitated;

		public GameStateMachine<BeIncapacitatedSuffocatingChore.States, BeIncapacitatedSuffocatingChore.StatesInstance, BeIncapacitatedSuffocatingChore, object>.State resuscitated;

		public GameStateMachine<BeIncapacitatedSuffocatingChore.States, BeIncapacitatedSuffocatingChore.StatesInstance, BeIncapacitatedSuffocatingChore, object>.State fail;
	}
}
