using System;
using Klei.AI;
using UnityEngine;

public class VomitChore : Chore<VomitChore.StatesInstance>
{
	public VomitChore(ChoreType chore_type, IStateMachineTarget target, StatusItem status_item, Notification notification, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.Vomit, target, target.GetComponent<ChoreProvider>(), false, on_complete, null, null, int.MaxValue, false, true)
	{
		this.smi = new VomitChore.StatesInstance(this, target.gameObject, status_item, notification);
	}

	public class StatesInstance : GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore>.GameInstance
	{
		public StatesInstance(VomitChore master, GameObject vomiter, StatusItem status_item, Notification notification)
			: base(master)
		{
			base.sm.vomiter.Set(vomiter, base.smi);
			this.bodyTemperature = Db.Get().Amounts.Temperature.Lookup(vomiter);
			this.statusItem = status_item;
			this.notification = notification;
		}

		public void SpawnDirtyWater()
		{
			int frontCell = base.sm.vomiter.Get(base.smi).GetComponent<Facing>().GetFrontCell();
			SimMessages.AddRemoveSubstance(frontCell, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, 1f, this.bodyTemperature.value, -1);
		}

		public StatusItem statusItem;

		private AmountInstance bodyTemperature;

		public Notification notification;
	}

	public class States : GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.buildup;
			base.Target(this.vomiter);
			this.root.ToggleAnims("anim_vomit", 0f).PlayAnim("vomit", KAnim.PlayMode.Once, null).OnAnimQueueComplete(null);
			this.buildup.ScheduleGoTo(2.5f, this.release);
			this.release.ToggleStatusItem((VomitChore.StatesInstance smi) => smi.statusItem, null).DoNotification((VomitChore.StatesInstance smi) => smi.notification).DoTutorial(Tutorial.TutorialMessages.TM_Mopping)
				.Update("SpawnDirtyWater", delegate(VomitChore.StatesInstance smi)
				{
					smi.SpawnDirtyWater();
				});
		}

		public StateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore>.TargetParameter vomiter;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore>.State buildup;

		public GameStateMachine<VomitChore.States, VomitChore.StatesInstance, VomitChore>.State release;
	}
}
