using System;

namespace Klei
{
	public class WorldGenStats
	{
		public int TotalNodeCount;

		public int MissedCellCount;

		public long GenerateTime;

		public long RenderDataTime;

		public long GenerateDataTime;

		public long GenerateNoiseTime;

		public long GenerateLayoutTime;

		public long ConvertVoroToMapTime;

		public float MinDataValue = float.MaxValue;

		public float MaxDataValue = float.MinValue;

		public float MinHeatValue = float.MaxValue;

		public float MaxHeatValue = float.MinValue;
	}
}
