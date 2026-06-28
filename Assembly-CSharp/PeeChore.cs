using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class PeeChore : Chore<PeeChore.StatesInstance>
{
	public PeeChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Pee, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true)
	{
		this.smi = new PeeChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore>.GameInstance
	{
		public StatesInstance(PeeChore master, GameObject worker)
		{
			Func<List<Notification>, object, string> func = (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false);
			this.stressfullyEmptyingBladder = new Notification(DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_NAME, NotificationType.Bad, null, func, null, true, 0f, null, null, null);
			base..ctor(master);
			this.bladder = Db.Get().Amounts.Bladder.Lookup(worker);
			this.bodyTemperature = Db.Get().Amounts.Temperature.Lookup(worker);
			base.sm.worker.Set(worker, base.smi);
		}

		public bool IsDonePeeing()
		{
			return this.bladder.value <= 0f;
		}

		public void SpawnDirtyWater()
		{
			int num = Grid.PosToCell(base.sm.worker.Get<KMonoBehaviour>(base.smi));
			SimMessages.AddRemoveSubstance(num, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, 0.05f, this.bodyTemperature.value, -1);
		}

		public Notification stressfullyEmptyingBladder;

		public AmountInstance bladder;

		private AmountInstance bodyTemperature;
	}

	public class States : GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.running;
			base.Target(this.worker);
			this.running.ToggleAnims("anim_expel", 0f).ToggleStatusItem(Db.Get().DuplicantStatusItems.StressfullyEmptyingBladder, null).DoNotification((PeeChore.StatesInstance smi) => smi.stressfullyEmptyingBladder)
				.DoReport(ReportManager.ReportType.ToiletIncident, (PeeChore.StatesInstance smi) => 1f)
				.DoTutorial(Tutorial.TutorialMessages.TM_Mopping)
				.Transition(null, (PeeChore.StatesInstance smi) => smi.IsDonePeeing())
				.Update("SpawnDirtyWater", delegate(PeeChore.StatesInstance smi)
				{
					smi.SpawnDirtyWater();
				})
				.PlayAnim("working_loop", KAnim.PlayMode.Loop, null);
		}

		public StateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore>.TargetParameter worker;

		public GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore>.State running;
	}
}
