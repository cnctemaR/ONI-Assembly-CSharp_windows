using System;
using Klei;
using Klei.AI;
using TUNING;
using UnityEngine;

public class VomitChore : Chore<VomitChore.StatesInstance>
{
	public VomitChore(ChoreType chore_type, IStateMachineTarget target, StatusItem status_item, Notification notification, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.Vomit, target, target.GetComponent<ChoreProvider>(), true, on_complete, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0, null)
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

		private static bool CanEmitLiquid(int cell)
		{
			bool flag = true;
			if (Grid.Solid[cell] || (Grid.Cell[cell].properties & 2) != 0)
			{
				flag = false;
			}
			return flag;
		}

		public void SpawnDirtyWater(float dt)
		{
			if (dt > 0f)
			{
				float totalTime = this.animController.CurrentAnim.totalTime;
				float num = dt / totalTime;
				Diseases diseases = base.master.GetComponent<MinionModifiers>().diseases;
				SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
				for (int i = 0; i < diseases.Count; i++)
				{
					DiseaseInstance diseaseInstance = diseases[i];
					if (diseaseInstance.modifier.diseaseType == Disease.DiseaseType.Pathogen)
					{
						invalid.idx = Db.Get().Diseases.GetIndex(diseaseInstance.modifier.id);
						invalid.count = Mathf.RoundToInt(100000f * num);
						break;
					}
				}
				Facing component = base.sm.vomiter.Get(base.smi).GetComponent<Facing>();
				int num2 = Grid.PosToCell(component.transform.GetPosition());
				int frontCell = component.GetFrontCell();
				int num3 = frontCell;
				if (!VomitChore.StatesInstance.CanEmitLiquid(num3))
				{
					num3 = num2;
				}
				SimMessages.AddRemoveSubstance(num3, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, STRESS.VOMIT_AMOUNT * num, this.bodyTemperature.value, invalid.idx, invalid.count, -1);
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
				.MoveTo((VomitChore.StatesInstance smi) => smi.GetVomitCell(), this.vomit, this.vomit, false);
			this.vomit.DefaultState(this.vomit.buildup).ToggleAnims("anim_vomit_kanim", 0f).ToggleStatusItem((VomitChore.StatesInstance smi) => smi.statusItem, null)
				.DoNotification((VomitChore.StatesInstance smi) => smi.notification)
				.DoTutorial(Tutorial.TutorialMessages.TM_Mopping);
			this.vomit.buildup.PlayAnim("vomit_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.vomit.release);
			this.vomit.release.ToggleEffect("Vomiting").PlayAnim("vomit_loop", KAnim.PlayMode.Once).Update("SpawnDirtyWater", delegate(VomitChore.StatesInstance smi, float dt)
			{
				smi.SpawnDirtyWater(dt);
			}, UpdateRate.SIM_200ms, false)
				.OnAnimQueueComplete(this.vomit.release_pst);
			this.vomit.release_pst.PlayAnim("vomit_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.recover);
			this.recover.PlayAnim("breathe_pre").QueueAnim("breathe_loop", true, null).ScheduleGoTo(8f, this.recover_pst);
			this.recover_pst.QueueAnim("breathe_pst", false, null).OnAnimQueueComplete(this.complete);
			this.complete.ReturnSuccess();
		}

		public StateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.TargetParameter vomiter;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State moveto;

		public VomitChore.States.VomitState vomit;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State recover;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State recover_pst;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State complete;

		public class VomitState : GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State
		{
			public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State buildup;

			public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State release;

			public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore, object>.State release_pst;
		}
	}
}
