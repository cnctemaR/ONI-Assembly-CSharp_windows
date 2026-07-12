using System;

namespace System.Drawing.Drawing2D
{
	public sealed class Blend
	{
		public Blend()
		{
			this.Factors = new float[1];
			this.Positions = new float[1];
		}

		public Blend(int count)
		{
			this.Factors = new float[count];
			this.Positions = new float[count];
		}

		public float[] Factors { get; set; }

		public float[] Positions { get; set; }
	}
}
