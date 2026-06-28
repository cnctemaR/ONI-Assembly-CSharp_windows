using System;

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType> : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget>.GameInstance
{
}
