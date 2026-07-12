using System;

namespace System.Drawing.Drawing2D
{
	public sealed class ColorBlend
	{
		public ColorBlend()
		{
			this.Colors = new Color[1];
			this.Positions = new float[1];
		}

		public ColorBlend(int count)
		{
			this.Colors = new Color[count];
			this.Positions = new float[count];
		}

		public Color[] Colors { get; set; }

		public float[] Positions { get; set; }
	}
}
