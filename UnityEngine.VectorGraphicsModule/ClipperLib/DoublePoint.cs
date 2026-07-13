using System;

namespace ClipperLib
{
	internal struct DoublePoint
	{
		public DoublePoint(double x = 0.0, double y = 0.0)
		{
			this.X = x;
			this.Y = y;
		}

		public DoublePoint(DoublePoint dp)
		{
			this.X = dp.X;
			this.Y = dp.Y;
		}

		public DoublePoint(IntPoint ip)
		{
			this.X = (double)ip.X;
			this.Y = (double)ip.Y;
		}

		public double X;

		public double Y;
	}
}
