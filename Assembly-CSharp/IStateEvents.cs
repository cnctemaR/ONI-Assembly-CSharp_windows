using System;

public interface IStateEvents<StateMachine, StateMachineInstance>
{
	void Initialize(StateMachine sm);

	void OnEnter(StateMachineInstance smi);

	void OnUpdate(StateMachineInstance smi, float dt);

	void OnExit(StateMachineInstance smi);

	void CleanUp(StateMachineInstance smi);

	void OnAnimComplete(MegaBrainTank.StatesInstance smi, HashedString completedAnim);
}
