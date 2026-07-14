using System;

namespace ProcGenGame
{
	public interface SymbolicMapElement
	{
		void ConvertToMap(Chunk world, TerrainCell.ISimDataSetter setter, float temperatureMin, float temperatureRange, SeededRandom rnd);
	}
}
