using System;

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType> : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.GameInstance where MasterType : IStateMachineTarget
{
}
