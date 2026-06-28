using System;

public class SimData
{
	public unsafe Sim.MassChangeInfo* removedMassEntries;

	public unsafe Sim.MassChangeInfo* emittedMassEntries;

	public unsafe Sim.ElementChunkInfo* elementChunks;
}
