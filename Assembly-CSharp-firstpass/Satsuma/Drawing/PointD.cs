using System;
using System.Globalization;

namespace Satsuma.Drawing
{
	public struct PointD : IEquatable<PointD>
	{
		public double X { readonly get; private set; }

		public double Y { readonly get; private set; }

		public PointD(double x, double y)
		{
			this = default(PointD);
			this.X = x;
			this.Y = y;
		}

		public bool Equals(PointD other)
		{
			return this.X == other.X && this.Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			return obj is PointD && this.Equals((PointD)obj);
		}

		public static bool operator ==(PointD a, PointD b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(PointD a, PointD b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() * 17 + this.Y.GetHashCode();
		}

		public string ToString(IFormatProvider provider)
		{
			return string.Format(provider, "({0} {1})", this.X, this.Y);
		}

		public override string ToString()
		{
			return this.ToString(CultureInfo.CurrentCulture);
		}

		public static PointD operator +(PointD a, PointD b)
		{
			return new PointD(a.X + b.X, a.Y + b.Y);
		}

		public static PointD Add(PointD a, PointD b)
		{
			return a + b;
		}

		public double Distance(PointD other)
		{
			return Math.Sqrt((this.X - other.X) * (this.X - other.X) + (this.Y - other.Y) * (this.Y - other.Y));
		}
	}
}
