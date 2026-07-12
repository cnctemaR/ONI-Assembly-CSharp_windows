using System;

public class RocketPassengerMonitor : GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.satisfied.ParamTransition<int>(this.targetCell, this.moving, (RocketPassengerMonitor.Instance smi, int p) => p != Grid.InvalidCell);
		this.moving.ParamTransition<int>(this.targetCell, this.satisfied, (RocketPassengerMonitor.Instance smi, int p) => p == Grid.InvalidCell).ToggleChore((RocketPassengerMonitor.Instance smi) => this.CreateChore(smi), this.satisfied).Exit(delegate(RocketPassengerMonitor.Instance smi)
		{
			this.targetCell.Set(Grid.InvalidCell, smi);
		});
		this.movingToModuleDeployPre.Enter(delegate(RocketPassengerMonitor.Instance smi)
		{
			this.targetCell.Set(smi.moduleDeployTaskTargetMoveCell, smi);
			smi.GoTo(this.movingToModuleDeploy);
		});
		this.movingToModuleDeploy.ParamTransition<int>(this.targetCell, this.satisfied, (RocketPassengerMonitor.Instance smi, int p) => p == Grid.InvalidCell).ToggleChore((RocketPassengerMonitor.Instance smi) => this.CreateChore(smi), this.moduleDeploy);
		this.moduleDeploy.Enter(delegate(RocketPassengerMonitor.Instance smi)
		{
			smi.moduleDeployCompleteCallback(null);
			this.targetCell.Set(Grid.InvalidCell, smi);
			smi.moduleDeployCompleteCallback = null;
			smi.GoTo(smi.sm.satisfied);
		});
	}

	public Chore CreateChore(RocketPassengerMonitor.Instance smi)
	{
		MoveChore moveChore = new MoveChore(smi.master, Db.Get().ChoreTypes.RocketEnterExit, (MoveChore.StatesInstance mover_smi) => this.targetCell.Get(smi), false);
		moveChore.AddPrecondition(ChorePreconditions.instance.CanMoveToCell, this.targetCell.Get(smi));
		return moveChore;
	}

	public StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.IntParameter targetCell = new StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.IntParameter(Grid.InvalidCell);

	public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State moving;

	public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State movingToModuleDeployPre;

	public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State movingToModuleDeploy;

	public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State moduleDeploy;

	public new class Instance : GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool ShouldMoveThroughRocketDoor()
		{
			int num = base.sm.targetCell.Get(this);
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if ((int)Grid.WorldIdx[num] == this.GetMyWorldId())
			{
				base.sm.targetCell.Set(Grid.InvalidCell, this);
				return false;
			}
			return true;
		}

		public void SetMoveTarget(int cell)
		{
			if ((int)Grid.WorldIdx[cell] == this.GetMyWorldId())
			{
				return;
			}
			base.sm.targetCell.Set(cell, this);
		}

		public void SetModuleDeployChore(int cell, Action<Chore> OnChoreCompleteCallback)
		{
			this.moduleDeployCompleteCallback = OnChoreCompleteCallback;
			this.moduleDeployTaskTargetMoveCell = cell;
			this.GoTo(base.sm.movingToModuleDeployPre);
			base.sm.targetCell.Set(cell, this);
		}

		public void CancelModuleDeployChore()
		{
			this.moduleDeployCompleteCallback = null;
			this.moduleDeployTaskTargetMoveCell = Grid.InvalidCell;
			base.sm.targetCell.Set(Grid.InvalidCell, base.smi);
		}

		public void ClearMoveTarget(int testCell)
		{
			int num = base.sm.targetCell.Get(this);
			if (Grid.IsValidCell(num) && Grid.WorldIdx[num] == Grid.WorldIdx[testCell])
			{
				base.sm.targetCell.Set(Grid.InvalidCell, this);
				if (base.IsInsideState(base.sm.moving))
				{
					this.GoTo(base.sm.satisfied);
				}
			}
		}

		public int lastWorldID;

		public Action<Chore> moduleDeployCompleteCallback;

		public int moduleDeployTaskTargetMoveCell;
	}
}
