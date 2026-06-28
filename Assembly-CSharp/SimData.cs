using System;

public class SimData
{
	public unsafe Sim.EmittedMassInfo* emittedMassEntries;

	public unsafe Sim.ElementChunkInfo* elementChunks;

	public unsafe Sim.BuildingTemperatureInfo* buildingTemperatures;
}
