using System;

public interface IEmptyableCargo
{
	bool CanEmptyCargo();

	void EmptyCargo();

	IStateMachineTarget master { get; }

	bool CanAutoDeploy { get; }

	bool AutoDeploy { get; set; }

	bool ChooseDuplicant { get; }

	MinionIdentity ChosenDuplicant { get; set; }
}
