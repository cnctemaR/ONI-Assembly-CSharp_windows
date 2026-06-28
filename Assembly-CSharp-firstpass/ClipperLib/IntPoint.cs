using System;

namespace ClipperLib
{
	public struct IntPoint
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
			bool flag;
			if (obj == null)
			{
				flag = false;
			}
			else if (obj is IntPoint)
			{
				IntPoint intPoint = (IntPoint)obj;
				flag = this.X == intPoint.X && this.Y == intPoint.Y;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public long X;

		public long Y;
	}
}
