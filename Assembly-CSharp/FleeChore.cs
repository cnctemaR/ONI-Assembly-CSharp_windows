using System;
using System.Collections.Generic;
using UnityEngine;

public class FleeChore : Chore<FleeChore.StatesInstance>
{
	public FleeChore(IStateMachineTarget target, GameObject enemy)
		: base(Db.Get().ChoreTypes.Flee, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.emergency, 0, false, true, 0, null)
	{
		this.smi = new FleeChore.StatesInstance(this);
		this.smi.sm.self.Set(this.gameObject, this.smi);
		this.nav = this.gameObject.GetComponent<Navigator>();
		this.smi.sm.fleeFromTarget.Set(enemy, this.smi);
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
		if (this.smi.sm.fleeToTarget.Get(this.smi) != null)
		{
			ChoreHelpers.DestroyLocator(this.smi.sm.fleeToTarget.Get(this.smi));
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
			this.root.ToggleStatusItem(Db.Get().DuplicantStatusItems.Fleeing, null);
			this.planFleeRoute.Enter(delegate(FleeChore.StatesInstance smi)
			{
				int num = Grid.PosToCell(this.fleeFromTarget.Get(smi));
				HashSet<int> hashSet = GameUtil.FloodCollectCells(Grid.PosToCell(smi.master.gameObject), new Func<int, bool>(smi.master.CanFleeTo), 300, null);
				int num2 = -1;
				int num3 = -1;
				foreach (int num4 in hashSet)
				{
					if (smi.master.nav.CanReach(num4))
					{
						int num5 = -1;
						num5 += Grid.GetCellDistance(num4, num);
						if (smi.master.isInFavoredDirection(num4, num))
						{
							num5 += 8;
						}
						if (num5 > num3)
						{
							num3 = num5;
							num2 = num4;
						}
					}
				}
				int num6 = num2;
				if (num6 != -1)
				{
					smi.sm.fleeToTarget.Set(smi.master.CreateLocator(Grid.CellToPos(num6)), smi);
					smi.sm.fleeToTarget.Get(smi).name = "FleeLocator";
					if (num6 == num)
					{
						smi.GoTo(this.cower);
					}
					else
					{
						smi.GoTo(this.flee);
					}
				}
				else
				{
					smi.GoTo(this.cower);
				}
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
