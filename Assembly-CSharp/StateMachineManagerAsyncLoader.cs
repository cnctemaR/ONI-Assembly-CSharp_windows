using System;
using System.Collections.Generic;

internal class StateMachineManagerAsyncLoader : GlobalAsyncLoader<StateMachineManagerAsyncLoader>
{
	public override void Run()
	{
	}

	public List<StateMachine> stateMachines = new List<StateMachine>();
}
