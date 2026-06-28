using System;
using UnityEngine;

public class AggressiveChore : Chore<AggressiveChore.StatesInstance>
{
	public AggressiveChore(IStateMachineTarget target, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.StressActingOut, target, target.GetComponent<ChoreProvider>(), false, on_complete, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new AggressiveChore.StatesInstance(this, target.gameObject);
	}

	public override void Cleanup()
	{
		base.Cleanup();
	}

	public void PunchWallDamage()
	{
		if (Grid.Solid[this.smi.sm.wallCellToBreak])
		{
			WorldDamage.Instance.ApplyDamage(this.smi.sm.wallCellToBreak, 0.04f, this.smi.sm.wallCellToBreak, -1);
		}
	}

	public class StatesInstance : GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.GameInstance
	{
		public StatesInstance(AggressiveChore master, GameObject breaker)
			: base(master)
		{
			base.sm.breaker.Set(breaker, base.smi);
		}

		public void FindBreakable()
		{
			Navigator navigator = base.GetComponent<Navigator>();
			int num = int.MaxValue;
			Breakable breakable = null;
			if (global::UnityEngine.Random.Range(0, 100) >= 50)
			{
				foreach (Breakable breakable2 in Components.Breakables)
				{
					if (!(breakable2 == null))
					{
						if (!breakable2.isBroken())
						{
							int navigationCost = navigator.GetNavigationCost(breakable2);
							if (navigationCost != PathProber.InvalidCost)
							{
								if (navigationCost < num)
								{
									num = navigationCost;
									breakable = breakable2;
								}
							}
						}
					}
				}
			}
			if (breakable == null)
			{
				int num2 = GameUtil.FloodFillFind((int cell) => !Grid.Solid[cell] && navigator.CanReach(cell) && (Grid.Solid[Grid.CellLeft(cell)] || Grid.Solid[Grid.CellRight(cell)] || Grid.Solid[Grid.OffsetCell(cell, 1, 1)] || Grid.Solid[Grid.OffsetCell(cell, -1, 1)]), Grid.PosToCell(navigator.gameObject), 128);
				base.sm.moveToWallTarget.Set(num2, base.smi);
				this.GoTo(base.sm.move_notarget);
			}
			else
			{
				base.sm.breakable.Set(breakable, base.smi);
				this.GoTo(base.sm.move_target);
			}
		}
	}

	public class States : GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.findbreakable;
			base.Target(this.breaker);
			this.root.ToggleAnims("anim_loco_destructive_kanim", 0f);
			this.noTarget.Enter(delegate(AggressiveChore.StatesInstance smi)
			{
				smi.StopSM("complete/no more food");
			});
			this.findbreakable.Enter("FindBreakable", delegate(AggressiveChore.StatesInstance smi)
			{
				smi.FindBreakable();
			});
			this.move_notarget.MoveTo((AggressiveChore.StatesInstance smi) => smi.sm.moveToWallTarget.Get(smi), this.breaking_wall, this.noTarget, false);
			this.move_target.InitializeStates(this.breaker, this.breakable, this.breaking, this.findbreakable, null, null).ToggleStatusItem(Db.Get().DuplicantStatusItems.LashingOut, null);
			this.breaking_wall.DefaultState(this.breaking_wall.Pre).Enter(delegate(AggressiveChore.StatesInstance smi)
			{
				int num = Grid.PosToCell(smi.master.gameObject);
				if (Grid.Solid[Grid.OffsetCell(num, 1, 0)])
				{
					smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_low_kanim"), 0f);
					int num2 = Grid.OffsetCell(num, 1, 0);
					this.wallCellToBreak = num2;
				}
				else if (Grid.Solid[Grid.OffsetCell(num, -1, 0)])
				{
					smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_low_kanim"), 0f);
					int num3 = Grid.OffsetCell(num, -1, 0);
					this.wallCellToBreak = num3;
				}
				else if (Grid.Solid[Grid.OffsetCell(num, 1, 1)])
				{
					smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_high_kanim"), 0f);
					int num4 = Grid.OffsetCell(num, 1, 1);
					this.wallCellToBreak = num4;
				}
				else if (Grid.Solid[Grid.OffsetCell(num, -1, 1)])
				{
					smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_high_kanim"), 0f);
					int num5 = Grid.OffsetCell(num, -1, 1);
					this.wallCellToBreak = num5;
				}
				smi.master.GetComponent<Facing>().Face(Grid.CellToPos(this.wallCellToBreak));
			}).Exit(delegate(AggressiveChore.StatesInstance smi)
			{
				smi.sm.masterTarget.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_high_kanim"));
				smi.sm.masterTarget.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_low_kanim"));
			});
			this.breaking_wall.Pre.PlayAnim("working_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.breaking_wall.Loop);
			this.breaking_wall.Loop.ScheduleGoTo(26f, this.breaking_wall.Pst).ToggleSchedulePeriodic("PunchWallDamage", 0.7f, delegate(AggressiveChore.StatesInstance smi)
			{
				smi.master.PunchWallDamage();
			}).Enter(delegate(AggressiveChore.StatesInstance smi)
			{
				smi.Play("working_loop", KAnim.PlayMode.Loop);
			})
				.Update(delegate(AggressiveChore.StatesInstance smi)
				{
					if (!Grid.Solid[smi.sm.wallCellToBreak])
					{
						smi.GoTo(this.breaking_wall.Pst);
					}
				});
			this.breaking_wall.Pst.QueueAnim("working_pst", false, null).OnAnimQueueComplete(this.noTarget);
			this.breaking.ToggleWork<Breakable>(this.breakable, null, null);
		}

		public StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.TargetParameter breaker;

		public StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.TargetParameter breakable;

		public StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.IntParameter moveToWallTarget;

		public int wallCellToBreak;

		public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.ApproachSubState<Breakable> move_target;

		public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State move_notarget;

		public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State findbreakable;

		public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State noTarget;

		public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State breaking;

		public AggressiveChore.States.BreakingWall breaking_wall;

		public class BreakingWall : GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State
		{
			public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State Pre;

			public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State Loop;

			public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State Pst;
		}
	}
}
