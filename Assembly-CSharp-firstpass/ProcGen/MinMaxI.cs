using System;

namespace ProcGen
{
	[Serializable]
	public struct MinMaxI
	{
		public int min { readonly get; private set; }

		public int max { readonly get; private set; }

		public MinMaxI(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public int GetRandomValueWithinRange(SeededRandom rnd)
		{
			return rnd.RandomRange(this.min, this.max);
		}

		public int GetAverage()
		{
			return (this.min + this.max) / 2;
		}

		public void Mod(MinMaxI mod)
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
