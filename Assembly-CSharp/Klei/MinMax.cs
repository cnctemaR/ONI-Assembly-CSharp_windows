using System;

namespace Klei
{
	public struct MinMax
	{
		public float min { get; private set; }

		public float max { get; private set; }

		public float GetValue()
		{
			return WorldGen.RandomRange(this.min, this.max);
		}
	}
}
