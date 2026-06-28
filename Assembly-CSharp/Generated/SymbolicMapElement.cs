using System;
using Klei;

namespace Generated
{
	public interface SymbolicMapElement
	{
		void ConvertToMap(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange);
	}
}
