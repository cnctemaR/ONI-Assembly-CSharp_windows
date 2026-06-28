using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class VomitChore : Chore<VomitChore.StatesInstance>
{
	public VomitChore(ChoreType chore_type, IStateMachineTarget target, StatusItem status_item, Notification notification, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.Vomit, target, target.GetComponent<ChoreProvider>(), false, on_complete, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new VomitChore.StatesInstance(this, target.gameObject, status_item, notification);
	}

	public class StatesInstance : GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.GameInstance
	{
		public StatesInstance(VomitChore master, GameObject vomiter, StatusItem status_item, Notification notification)
			: base(master)
		{
			base.sm.vomiter.Set(vomiter, base.smi);
			this.bodyTemperature = Db.Get().Amounts.Temperature.Lookup(vomiter);
			this.statusItem = status_item;
			this.notification = notification;
			this.vomitCellQuery = new SafetyQuery(Game.Instance.safetyConditions.VomitCellChecker, base.GetComponent<KMonoBehaviour>(), 10);
		}

		public void SpawnDirtyWater(float dt)
		{
			if (dt > 0f)
			{
				int frontCell = base.sm.vomiter.Get(base.smi).GetComponent<Facing>().GetFrontCell();
				SimMessages.AddRemoveSubstance(frontCell, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, 1f * STRESS.VOMIT_RATE * dt, this.bodyTemperature.value, -1);
			}
		}

		public int GetVomitCell()
		{
			this.vomitCellQuery.Reset();
			Navigator component = base.GetComponent<Navigator>();
			component.RunQuery(this.vomitCellQuery);
			int num = this.vomitCellQuery.GetResultCell();
			if (Grid.InvalidCell == num)
			{
				num = Grid.PosToCell(component);
			}
			return num;
		}

		public StatusItem statusItem;

		private AmountInstance bodyTemperature;

		public Notification notification;

		private SafetyQuery vomitCellQuery;
	}

	public class States : GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.moveto;
			base.Target(this.vomiter);
			this.moveto.TriggerOnEnter(GameHashes.BeginWalk, null).TriggerOnExit(GameHashes.EndWalk).ToggleAnims("anim_loco_vomiter_kanim", 0f)
				.MoveTo((VomitChore.StatesInstance smi) => smi.GetVomitCell(), this.buildup, this.buildup, false);
			this.buildup.ScheduleGoTo(2.5f, this.release).ToggleAnims("anim_vomit_kanim", 0f).PlayAnim("vomit", KAnim.PlayMode.Once, null)
				.OnAnimQueueComplete(null);
			this.release.ToggleEffect("Vomiting").ToggleStatusItem((VomitChore.StatesInstance smi) => smi.statusItem, null).DoNotification((VomitChore.StatesInstance smi) => smi.notification)
				.DoTutorial(Tutorial.TutorialMessages.TM_Mopping)
				.Update("SpawnDirtyWater", delegate(VomitChore.StatesInstance smi)
				{
					smi.SpawnDirtyWater(smi.deltatime);
				})
				.OnAnimQueueComplete(this.recover);
			this.recover.PlayAnim("breathe_pre", KAnim.PlayMode.Once, null).QueueAnim("breathe_loop", true, null).ScheduleGoTo(8f, this.recover_pst);
			this.recover_pst.QueueAnim("breathe_pst", false, null).OnAnimQueueComplete(this.complete);
			this.complete.Enter(delegate(VomitChore.StatesInstance smi)
			{
				smi.StopSM("complete");
			});
		}

		public StateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.TargetParameter vomiter;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State buildup;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State moveto;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State release;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State recover;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State recover_pst;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State complete;
	}
}
