using System;

namespace ClipperLib
{
	internal struct IntPoint
	{
		public IntPoint(long X, long Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public IntPoint(double x, double y)
		{
			this.X = (long)x;
			this.Y = (long)y;
		}

		public IntPoint(IntPoint pt)
		{
			this.X = pt.X;
			this.Y = pt.Y;
		}

		public static bool operator ==(IntPoint a, IntPoint b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(IntPoint a, IntPoint b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = obj is IntPoint;
				if (flag3)
				{
					IntPoint intPoint = (IntPoint)obj;
					flag2 = this.X == intPoint.X && this.Y == intPoint.Y;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public long X;

		public long Y;
	}
}
