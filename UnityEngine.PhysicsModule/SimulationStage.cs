using System;

namespace UnityEngine
{
	public enum SimulationStage : ushort
	{
		None,
		PrepareSimulation,
		RunSimulation,
		PublishSimulationResults = 4,
		All = 7
	}
}
