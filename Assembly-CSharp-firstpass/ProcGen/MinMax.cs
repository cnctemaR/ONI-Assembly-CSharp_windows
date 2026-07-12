using System;

namespace ProcGen
{
	[Serializable]
	public struct MinMax
	{
		public float min { readonly get; private set; }

		public float max { readonly get; private set; }

		public MinMax(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public float GetRandomValueWithinRange(SeededRandom rnd)
		{
			return rnd.RandomRange(this.min, this.max);
		}

		public float GetAverage()
		{
			return (this.min + this.max) / 2f;
		}

		public void Mod(MinMax mod)
		{
			this.min += mod.min;
			this.max += mod.max;
		}

		public override string ToString()
		{
			return string.Format("min:{0} max:{1}", this.min, this.max);
		}
	}
}
