using System;
using UnityEngine;

public class FallMonitor : GameStateMachine<FallMonitor, FallMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.standing;
		this.root.EventTransition(GameHashes.OnStore, this.instorage, null);
		this.standing.ParamTransition<bool>(this.isEntombed, this.entombed, (FallMonitor.Instance smi, bool p) => p).ParamTransition<bool>(this.isFalling, this.falling_pre, (FallMonitor.Instance smi, bool p) => p);
		this.falling_pre.Enter("StopNavigator", delegate(FallMonitor.Instance smi)
		{
			smi.GetComponent<Navigator>().Stop(false);
		}).Enter("AttemptInitialRecovery", delegate(FallMonitor.Instance smi)
		{
			smi.AttemptInitialRecovery();
		}).GoTo(this.falling)
			.ToggleBrain("falling_pre");
		this.falling.ToggleBrain("falling").PlayAnim("fall_pre", KAnim.PlayMode.Once, null).QueueAnim("fall_loop", true, null)
			.ParamTransition<bool>(this.isEntombed, this.entombed, (FallMonitor.Instance smi, bool p) => p)
			.Transition(this.recoverladder, (FallMonitor.Instance smi) => smi.CanRecoverToLadder())
			.ToggleGravity(this.landfloor);
		this.recoverinitialfall.ToggleBrain("recoverinitialfall").Enter("Recover", delegate(FallMonitor.Instance smi)
		{
			smi.Recover();
		}).EventTransition(GameHashes.DestinationReached, this.standing, null)
			.EventTransition(GameHashes.NavigationFailed, this.standing, null)
			.Enter("Recover", delegate(FallMonitor.Instance smi)
			{
				smi.UpdateEntombed();
			});
		this.landfloor.Enter("Land", delegate(FallMonitor.Instance smi)
		{
			smi.LandFloor();
		}).GoTo(this.standing);
		this.recoverladder.ToggleBrain("recoverladder").PlayAnim("floor_ladder_0_0", KAnim.PlayMode.Once, null).Enter("MountLadder", delegate(FallMonitor.Instance smi)
		{
			smi.MountLadder();
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
		}).ToggleChore((FallMonitor.Instance smi) => new EntombedChore(smi.master), this.standing, false).ParamTransition<bool>(this.isEntombed, this.standing, (FallMonitor.Instance smi, bool p) => !p);
	}

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State standing;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State falling_pre;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State falling;

	public FallMonitor.EntombedStates entombed;

	public GameStateMachine<FallMonitor, FallMonitor.Instance, IStateMachineTarget, object>.State recoverladder;

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
			base.GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(base.GetComponent<Transform>().position), Grid.SceneLayer.Move));
		}

		public void AttemptInitialRecovery()
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
							base.smi.GoTo(base.smi.sm.recoverinitialfall);
							break;
						}
					}
				}
			}
		}

		public bool CanRecoverToLadder()
		{
			int num = Grid.PosToCell(base.master.transform.position);
			return this.navigator.NavGrid.NavTable.IsValid(num, NavType.Ladder);
		}

		public void MountLadder()
		{
			this.navigator.SetCurrentNavType(NavType.Ladder);
			base.GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(base.GetComponent<Transform>().position), Grid.SceneLayer.Move));
		}

		public bool IsFalling()
		{
			if (this.navigator.IsMoving())
			{
				return false;
			}
			int num = Grid.PosToCell(base.master.transform.position);
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

		public void UpdateEntombed()
		{
			int num = Grid.PosToCell(base.transform.position);
			bool flag = !this.IsCellSafe(num) && !this.navigator.IsMoving();
			base.sm.isEntombed.Set(flag, base.smi);
		}

		public void FixedUpdate()
		{
			this.UpdateEntombed();
			base.sm.isFalling.Set(this.IsFalling(), base.smi);
		}

		public bool IsCellSafe(int cell)
		{
			int num = Grid.CellAbove(cell);
			return (!Grid.Solid[cell] || Grid.ForceField[cell] || Grid.HasDoor[cell]) && (!Grid.Solid[num] || Grid.ForceField[num] || Grid.HasDoor[num]);
		}

		public void TryEntombedEscape()
		{
			int num = Grid.PosToCell(base.transform.position);
			foreach (CellOffset cellOffset in this.entombedEscapeOffsets)
			{
				int num2 = Grid.OffsetCell(num, cellOffset);
				if (this.IsCellSafe(num2))
				{
					base.transform.SetPosition(Grid.CellToPosCBC(num2, Grid.SceneLayer.Move));
					base.transform.GetComponent<Navigator>().Stop(false);
					base.transform.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
					this.UpdateEntombed();
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
