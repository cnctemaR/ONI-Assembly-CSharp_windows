using System;
using UnityEngine;

public class FallMonitor : GameStateMachine<FallMonitor, FallMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.standing;
		this.root.EventTransition(GameHashes.OnStore, this.instorage, null).Update("CheckLanded", delegate(FallMonitor.Instance smi, float dt)
		{
			smi.UpdateFalling();
		}, UpdateRate.SIM_33ms, true);
		this.standing.ParamTransition<bool>(this.isEntombed, this.entombed, (FallMonitor.Instance smi, bool p) => p).ParamTransition<bool>(this.isFalling, this.falling_pre, (FallMonitor.Instance smi, bool p) => p);
		this.falling_pre.Enter("StopNavigator", delegate(FallMonitor.Instance smi)
		{
			smi.GetComponent<Navigator>().Stop(false);
		}).Enter("AttemptInitialRecovery", delegate(FallMonitor.Instance smi)
		{
			smi.AttemptInitialRecovery();
		}).GoTo(this.falling)
			.ToggleBrain("falling_pre");
		this.falling.ToggleBrain("falling").PlayAnim("fall_pre").QueueAnim("fall_loop", true, null)
			.ParamTransition<bool>(this.isEntombed, this.entombed, (FallMonitor.Instance smi, bool p) => p)
			.Transition(this.recoverladder, (FallMonitor.Instance smi) => smi.CanRecoverToLadder(), UpdateRate.SIM_33ms)
			.Transition(this.recoverpole, (FallMonitor.Instance smi) => smi.CanRecoverToPole(), UpdateRate.SIM_33ms)
			.ToggleGravity(this.landfloor);
		this.recoverinitialfall.ToggleBrain("recoverinitialfall").Enter("Recover", delegate(FallMonitor.Instance smi)
		{
			smi.Recover();
		}).EventTransition(GameHashes.DestinationReached, this.standing, null)
			.EventTransition(GameHashes.NavigationFailed, this.standing, null);
		this.landfloor.Enter("Land", delegate(FallMonitor.Instance smi)
		{
			smi.LandFloor();
		}).GoTo(this.standing);
		this.recoverladder.ToggleBrain("recoverladder").PlayAnim("floor_ladder_0_0").Enter("MountLadder", delegate(FallMonitor.Instance smi)
		{
			smi.MountLadder();
		})
			.OnAnimQueueComplete(this.standing);
		this.recoverpole.ToggleBrain("recoverpole").PlayAnim("floor_pole_0_0").Enter("MountPole", delegate(FallMonitor.Instance smi)
		{
			smi.MountPole();
		})
			.OnAnimQueueComplete(this.standing);
		this.instorage.EventTransition(GameHashes.OnStore, this.standing, null);
		this.entombed.DefaultState(this.entombed.recovering);
		this.entombed.recovering.Enter("TryEntombedEscape", delegate(FallMonitor.Instance smi)
		{
			smi.TryEntombedEscape();
		});
		this.entombed.stuck.Enter("StopNavigator", delegate(FallMonitor.Instance smi)
		{
			smi.GetComponent<Navigator>().Stop(false);
		}).ToggleChore((FallMonitor.Instance smi) => new EntombedChore(smi.master), this.standing).ParamTransition<bool>(this.isEntombed, this.standing, (FallMonitor.Instance smi, bool p) => !p);
	}

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State standing;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State falling_pre;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State falling;

	public FallMonitor.EntombedStates entombed;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State recoverladder;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State recoverpole;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State recoverinitialfall;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State landfloor;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State instorage;

	public StateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.BoolParameter isEntombed;

	public StateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.BoolParameter isFalling;

	public class EntombedStates : GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State recovering;

		public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State stuck;
	}

	public new class Instance : GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.navigator = base.GetComponent<Navigator>();
			Pathfinding.Instance.FlushNavGridsOnLoad();
		}

		public void Recover()
		{
			int num = Grid.PosToCell(this.navigator);
			foreach (NavGrid.Transition transition in this.navigator.NavGrid.transitions)
			{
				if (transition.isEscape)
				{
					if (this.navigator.CurrentNavType == transition.start)
					{
						int num2 = transition.IsValid(num, this.navigator.NavGrid.NavTable, Grid.BitFields, false);
						if (Grid.InvalidCell != num2)
						{
							this.navigator.BeginTransition(transition);
							break;
						}
					}
				}
			}
		}

		public void LandFloor()
		{
			this.navigator.SetCurrentNavType(NavType.Floor);
			base.GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(base.GetComponent<Transform>().GetPosition()), Grid.SceneLayer.Move));
		}

		public void AttemptInitialRecovery()
		{
			if (base.gameObject.HasTag(GameTags.Incapacitated))
			{
				return;
			}
			int num = Grid.PosToCell(this.navigator);
			foreach (NavGrid.Transition transition in this.navigator.NavGrid.transitions)
			{
				if (transition.isEscape)
				{
					if (this.navigator.CurrentNavType == transition.start)
					{
						int num2 = transition.IsValid(num, this.navigator.NavGrid.NavTable, Grid.BitFields, false);
						if (Grid.InvalidCell != num2)
						{
							base.smi.GoTo(base.smi.sm.recoverinitialfall);
							break;
						}
					}
				}
			}
		}

		public bool CanRecoverToLadder()
		{
			int num = Grid.PosToCell(base.master.transform.GetPosition());
			return this.navigator.NavGrid.NavTable.IsValid(num, NavType.Ladder) && !base.gameObject.HasTag(GameTags.Incapacitated);
		}

		public void MountLadder()
		{
			this.navigator.SetCurrentNavType(NavType.Ladder);
			base.GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(base.GetComponent<Transform>().GetPosition()), Grid.SceneLayer.Move));
		}

		public bool CanRecoverToPole()
		{
			int num = Grid.PosToCell(base.master.transform.GetPosition());
			return this.navigator.NavGrid.NavTable.IsValid(num, NavType.Pole) && !base.gameObject.HasTag(GameTags.Incapacitated);
		}

		public void MountPole()
		{
			this.navigator.SetCurrentNavType(NavType.Pole);
			base.GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(base.GetComponent<Transform>().GetPosition()), Grid.SceneLayer.Move));
		}

		public bool IsFalling()
		{
			if (this.navigator.IsMoving())
			{
				return false;
			}
			int num = Grid.PosToCell(base.master.transform.GetPosition());
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			int num2 = Grid.CellBelow(num);
			if (!Grid.IsValidCell(num2))
			{
				return false;
			}
			bool flag = this.navigator.NavGrid.NavTable.IsValid(num, this.navigator.CurrentNavType);
			return !flag;
		}

		public void UpdateFalling()
		{
			bool flag = false;
			bool flag2 = false;
			if (!this.navigator.IsMoving())
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				bool flag3 = this.navigator.NavGrid.NavTable.IsValid(num, this.navigator.CurrentNavType);
				flag3 = flag3 && (!base.gameObject.HasTag(GameTags.Incapacitated) || (this.navigator.CurrentNavType != NavType.Ladder && this.navigator.CurrentNavType != NavType.Pole));
				flag2 = !flag3 && (Grid.Solid[num] || Grid.Solid[Grid.CellAbove(num)]);
				flag = !flag3 && !flag2;
			}
			base.sm.isFalling.Set(flag, base.smi);
			base.sm.isEntombed.Set(flag2, base.smi);
		}

		public bool IsCellSafe(int cell)
		{
			return this.navigator.NavGrid.NavTable.IsValid(cell, this.navigator.CurrentNavType);
		}

		public void TryEntombedEscape()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			foreach (CellOffset cellOffset in this.entombedEscapeOffsets)
			{
				int num2 = Grid.OffsetCell(num, cellOffset);
				if (this.IsCellSafe(num2))
				{
					base.transform.SetPosition(Grid.CellToPosCBC(num2, Grid.SceneLayer.Move));
					base.transform.GetComponent<Navigator>().Stop(false);
					base.transform.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
					this.UpdateFalling();
					this.GoTo(base.sm.standing);
					return;
				}
			}
			foreach (CellOffset cellOffset2 in this.entombedEscapeOffsets)
			{
				int num3 = Grid.OffsetCell(num, cellOffset2);
				if (!Grid.Solid[num3] && !Grid.Solid[Grid.CellAbove(num3)])
				{
					base.transform.SetPosition(Grid.CellToPosCBC(num3, Grid.SceneLayer.Move));
					base.transform.GetComponent<Navigator>().Stop(false);
					base.transform.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
					this.UpdateFalling();
					this.GoTo(base.sm.standing);
					return;
				}
			}
			this.GoTo(base.sm.entombed.stuck);
		}

		private CellOffset[] entombedEscapeOffsets = new CellOffset[]
		{
			new CellOffset(0, 1),
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(1, 1),
			new CellOffset(-1, 1),
			new CellOffset(1, -1),
			new CellOffset(-1, -1)
		};

		private Navigator navigator;
	}
}
