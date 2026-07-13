using System;

public class CommandConditions : KMonoBehaviour
{
	public ConditionDestinationReachable reachable;

	public CargoBayIsEmpty cargoEmpty;

	public ConditionHasMinimumMass destHasResources;

	public ConditionAllModulesComplete allModulesComplete;

	public ConditionHasEngine hasEngine;

	public ConditionHasNosecone hasNosecone;

	public ConditionOnLaunchPad onLaunchPad;

	public ConditionFlightPathIsClear flightPathIsClear;
}
