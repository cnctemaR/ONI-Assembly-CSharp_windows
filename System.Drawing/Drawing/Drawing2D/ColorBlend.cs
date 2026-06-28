using System;

namespace System.Drawing.Drawing2D
{
	public sealed class ColorBlend
	{
		public ColorBlend()
		{
			this.positions = new float[1];
			this.colors = new Color[1];
		}

		public ColorBlend(int count)
		{
			this.positions = new float[count];
			this.colors = new Color[count];
		}

		public Color[] Colors
		{
			get
			{
				return this.colors;
			}
			set
			{
				this.colors = value;
			}
		}

		public float[] Positions
		{
			get
			{
				return this.positions;
			}
			set
			{
				this.positions = value;
			}
		}

		private float[] positions;

		private Color[] colors;
	}
}
