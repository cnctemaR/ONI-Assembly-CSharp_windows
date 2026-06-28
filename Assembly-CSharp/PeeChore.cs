using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class PeeChore : Chore<PeeChore.StatesInstance>
{
	public PeeChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Pee, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0, null)
	{
		this.smi = new PeeChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.GameInstance
	{
		public StatesInstance(PeeChore master, GameObject worker)
			: base(master)
		{
			this.bladder = Db.Get().Amounts.Bladder.Lookup(worker);
			this.bodyTemperature = Db.Get().Amounts.Temperature.Lookup(worker);
			base.sm.worker.Set(worker, base.smi);
		}

		public bool IsDonePeeing()
		{
			return this.bladder.value <= 0f;
		}

		public void SpawnDirtyWater(float dt)
		{
			int num = Grid.PosToCell(base.sm.worker.Get<KMonoBehaviour>(base.smi));
			byte index = Db.Get().Diseases.GetIndex("FoodPoisoning");
			float num2 = dt * -this.bladder.GetDelta() / this.bladder.GetMax();
			if (num2 > 0f)
			{
				SimMessages.AddRemoveSubstance(num, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, 2f * num2, this.bodyTemperature.value, index, Mathf.CeilToInt(100000f * num2), -1);
			}
		}

		public Notification stressfullyEmptyingBladder = new Notification(DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null);

		public AmountInstance bladder;

		private AmountInstance bodyTemperature;
	}

	public class States : GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.running;
			base.Target(this.worker);
			this.running.ToggleAnims("anim_expel_kanim", 0f).ToggleEffect("StressfulyEmptyingBladder").DoNotification((PeeChore.StatesInstance smi) => smi.stressfullyEmptyingBladder)
				.DoReport(ReportManager.ReportType.ToiletIncident, (PeeChore.StatesInstance smi) => 1f, (PeeChore.StatesInstance smi) => this.masterTarget.Get(smi).GetProperName())
				.DoTutorial(Tutorial.TutorialMessages.TM_Mopping)
				.Transition(null, (PeeChore.StatesInstance smi) => smi.IsDonePeeing(), UpdateRate.SIM_200ms)
				.Update("SpawnDirtyWater", delegate(PeeChore.StatesInstance smi, float dt)
				{
					smi.SpawnDirtyWater(dt);
				}, UpdateRate.SIM_200ms, false)
				.PlayAnim("working_loop", KAnim.PlayMode.Loop);
		}

		public StateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.TargetParameter worker;

		public GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.State running;
	}
}
