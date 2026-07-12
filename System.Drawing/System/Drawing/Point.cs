using System;
using System.ComponentModel;
using System.Numerics.Hashing;

namespace System.Drawing
{
	[TypeConverter(typeof(PointConverter))]
	[Serializable]
	public struct Point : IEquatable<Point>
	{
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Point(Size sz)
		{
			this.x = sz.Width;
			this.y = sz.Height;
		}

		public Point(int dw)
		{
			this.x = (int)Point.LowInt16(dw);
			this.y = (int)Point.HighInt16(dw);
		}

		[Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				return this.x == 0 && this.y == 0;
			}
		}

		public int X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		public int Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public static implicit operator PointF(Point p)
		{
			return new PointF((float)p.X, (float)p.Y);
		}

		public static explicit operator Size(Point p)
		{
			return new Size(p.X, p.Y);
		}

		public static Point operator +(Point pt, Size sz)
		{
			return Point.Add(pt, sz);
		}

		public static Point operator -(Point pt, Size sz)
		{
			return Point.Subtract(pt, sz);
		}

		public static bool operator ==(Point left, Point right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator !=(Point left, Point right)
		{
			return !(left == right);
		}

		public static Point Add(Point pt, Size sz)
		{
			return new Point(pt.X + sz.Width, pt.Y + sz.Height);
		}

		public static Point Subtract(Point pt, Size sz)
		{
			return new Point(pt.X - sz.Width, pt.Y - sz.Height);
		}

		public static Point Ceiling(PointF value)
		{
			return new Point((int)Math.Ceiling((double)value.X), (int)Math.Ceiling((double)value.Y));
		}

		public static Point Truncate(PointF value)
		{
			return new Point((int)value.X, (int)value.Y);
		}

		public static Point Round(PointF value)
		{
			return new Point((int)Math.Round((double)value.X), (int)Math.Round((double)value.Y));
		}

		public override bool Equals(object obj)
		{
			return obj is Point && this.Equals((Point)obj);
		}

		public bool Equals(Point other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			return HashHelpers.Combine(this.X, this.Y);
		}

		public void Offset(int dx, int dy)
		{
			this.X += dx;
			this.Y += dy;
		}

		public void Offset(Point p)
		{
			this.Offset(p.X, p.Y);
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"{X=",
				this.X.ToString(),
				",Y=",
				this.Y.ToString(),
				"}"
			});
		}

		private static short HighInt16(int n)
		{
			return (short)((n >> 16) & 65535);
		}

		private static short LowInt16(int n)
		{
			return (short)(n & 65535);
		}

		public static readonly Point Empty;

		private int x;

		private int y;
	}
}
