using System;
using System.Runtime.InteropServices;

namespace ProcGen
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct MinMax
	{
		public float min { get; private set; }

		public float max { get; private set; }

		public float GetRandomValueWithinRange(SeededRandom rnd)
		{
			return rnd.RandomRange(this.min, this.max);
		}
	}
}
