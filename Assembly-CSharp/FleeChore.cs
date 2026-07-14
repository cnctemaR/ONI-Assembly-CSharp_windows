using System;
using UnityEngine;

public class FleeChore : Chore<FleeChore.StatesInstance>
{
	public FleeChore(IStateMachineTarget target, GameObject enemy)
		: base(Db.Get().ChoreTypes.Flee, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new FleeChore.StatesInstance(this);
		base.smi.sm.self.Set(this.gameObject, base.smi, false);
		this.nav = this.gameObject.GetComponent<Navigator>();
		base.smi.sm.fleeFromTarget.Set(enemy, base.smi, false);
	}

	private bool isInFavoredDirection(int cell, int fleeFromCell)
	{
		bool flag = Grid.CellToPos(fleeFromCell).x < this.gameObject.transform.GetPosition().x;
		bool flag2 = Grid.CellToPos(fleeFromCell).x < Grid.CellToPos(cell).x;
		return flag == flag2;
	}

	private bool CanFleeTo(int cell)
	{
		return this.nav.CanReach(cell) || this.nav.CanReach(Grid.OffsetCell(cell, -1, -1)) || this.nav.CanReach(Grid.OffsetCell(cell, 1, -1)) || this.nav.CanReach(Grid.OffsetCell(cell, -1, 1)) || this.nav.CanReach(Grid.OffsetCell(cell, 1, 1));
	}

	public GameObject CreateLocator(Vector3 pos)
	{
		return ChoreHelpers.CreateLocator("GoToLocator", pos);
	}

	protected override void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		if (base.smi.sm.fleeToTarget.Get(base.smi) != null)
		{
			ChoreHelpers.DestroyLocator(base.smi.sm.fleeToTarget.Get(base.smi));
		}
		base.OnStateMachineStop(reason, status);
	}

	private Navigator nav;

	public class StatesInstance : GameStateMachine<FleeChore.States, FleeChore.StatesInstance, FleeChore, object>.GameInstance
	{
		public StatesInstance(FleeChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<FleeChore.States, FleeChore.StatesInstance, FleeChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.planFleeRoute;
			this.root.ToggleStatusItem(Db.Get().DuplicantStatusItems.Fleeing, null).ToggleNotification((FleeChore.StatesInstance smi) => new Notification(Db.Get().DuplicantStatusItems.Fleeing.notificationText, Db.Get().DuplicantStatusItems.Fleeing.notificationType, null, null, true, 0f, null, null, smi.master.gameObject.transform, true, false, false));
			this.planFleeRoute.Enter(delegate(FleeChore.StatesInstance smi)
			{
				FleeChore.States.<>c__DisplayClass7_0 CS$<>8__locals1 = new FleeChore.States.<>c__DisplayClass7_0();
				CS$<>8__locals1.smi = smi;
				CS$<>8__locals1.fleeFromCell = Grid.PosToCell(this.fleeFromTarget.Get(CS$<>8__locals1.smi));
				int num = Grid.PosToCell(CS$<>8__locals1.smi.master.gameObject);
				int num2 = FloodFill.FindBest(new Func<int, float>(CS$<>8__locals1.<InitializeStates>g__RateCell|3), new Func<int, FloodFill.BoundaryCheckResult>(CS$<>8__locals1.<InitializeStates>g__BoundaryCondition|4), num, 300);
				if (num2 == -1)
				{
					CS$<>8__locals1.smi.GoTo(this.cower);
					return;
				}
				CS$<>8__locals1.smi.sm.fleeToTarget.Set(CS$<>8__locals1.smi.master.CreateLocator(Grid.CellToPos(num2)), CS$<>8__locals1.smi, false);
				CS$<>8__locals1.smi.sm.fleeToTarget.Get(CS$<>8__locals1.smi).name = "FleeLocator";
				if (num2 == CS$<>8__locals1.fleeFromCell)
				{
					CS$<>8__locals1.smi.GoTo(this.cower);
					return;
				}
				CS$<>8__locals1.smi.GoTo(this.flee);
			});
			this.flee.InitializeStates(this.self, this.fleeToTarget, this.cower, this.cower, null, NavigationTactics.ReduceTravelDistance).ToggleAnims("anim_loco_run_insane_kanim", 2f);
			this.cower.ToggleAnims("anim_cringe_kanim", 4f).PlayAnim("cringe_pre").QueueAnim("cringe_loop", false, null)
				.QueueAnim("cringe_pst", false, null)
				.OnAnimQueueComplete(this.end);
			this.end.Enter(delegate(FleeChore.StatesInstance smi)
			{
				smi.StopSM("stopped");
			});
		}

		public StateMachine<FleeChore.States, FleeChore.StatesInstance, FleeChore, object>.TargetParameter fleeFromTarget;

		public StateMachine<FleeChore.States, FleeChore.StatesInstance, FleeChore, object>.TargetParameter fleeToTarget;

		public StateMachine<FleeChore.States, FleeChore.StatesInstance, FleeChore, object>.TargetParameter self;

		public GameStateMachine<FleeChore.States, FleeChore.StatesInstance, FleeChore, object>.State planFleeRoute;

		public GameStateMachine<FleeChore.States, FleeChore.StatesInstance, FleeChore, object>.ApproachSubState<IApproachable> flee;

		public GameStateMachine<FleeChore.States, FleeChore.StatesInstance, FleeChore, object>.State cower;

		public GameStateMachine<FleeChore.States, FleeChore.StatesInstance, FleeChore, object>.State end;
	}
}
