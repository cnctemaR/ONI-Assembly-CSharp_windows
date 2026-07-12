using System;

public class BrainTankState : GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State, IStateEvents<MegaBrainTank.States, MegaBrainTank.StatesInstance>
{
	public virtual void Initialize(MegaBrainTank.States sm)
	{
	}

	public virtual void OnEnter(MegaBrainTank.StatesInstance smi)
	{
	}

	public virtual void OnUpdate(MegaBrainTank.StatesInstance smi, float dt)
	{
	}

	public virtual void OnExit(MegaBrainTank.StatesInstance smi)
	{
	}

	public virtual void CleanUp(MegaBrainTank.StatesInstance smi)
	{
	}

	public virtual void OnAnimComplete(MegaBrainTank.StatesInstance smi, HashedString completedAnim)
	{
	}
}
