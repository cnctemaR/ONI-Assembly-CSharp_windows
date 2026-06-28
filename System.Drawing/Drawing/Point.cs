using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	[ComVisible(true)]
	[TypeConverter(typeof(PointConverter))]
	[Serializable]
	public struct Point
	{
		public Point(int dw)
		{
			this.y = dw >> 16;
			this.x = dw & 65535;
		}

		public Point(Size sz)
		{
			this.x = sz.Width;
			this.y = sz.Height;
		}

		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static Point Ceiling(PointF value)
		{
			checked
			{
				int num = (int)Math.Ceiling((double)value.X);
				int num2 = (int)Math.Ceiling((double)value.Y);
				return new Point(num, num2);
			}
		}

		public static Point Round(PointF value)
		{
			checked
			{
				int num = (int)Math.Round((double)value.X);
				int num2 = (int)Math.Round((double)value.Y);
				return new Point(num, num2);
			}
		}

		public static Point Truncate(PointF value)
		{
			checked
			{
				int num = (int)value.X;
				int num2 = (int)value.Y;
				return new Point(num, num2);
			}
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

		public override bool Equals(object obj)
		{
			return obj is Point && this == (Point)obj;
		}

		public override int GetHashCode()
		{
			return this.x ^ this.y;
		}

		public void Offset(int dx, int dy)
		{
			this.x += dx;
			this.y += dy;
		}

		public override string ToString()
		{
			return string.Format("{{X={0},Y={1}}}", this.x.ToString(CultureInfo.InvariantCulture), this.y.ToString(CultureInfo.InvariantCulture));
		}

		public static Point Add(Point pt, Size sz)
		{
			return new Point(pt.X + sz.Width, pt.Y + sz.Height);
		}

		public void Offset(Point p)
		{
			this.Offset(p.X, p.Y);
		}

		public static Point Subtract(Point pt, Size sz)
		{
			return new Point(pt.X - sz.Width, pt.Y - sz.Height);
		}

		public static Point operator +(Point pt, Size sz)
		{
			return new Point(pt.X + sz.Width, pt.Y + sz.Height);
		}

		public static bool operator ==(Point left, Point right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator !=(Point left, Point right)
		{
			return left.X != right.X || left.Y != right.Y;
		}

		public static Point operator -(Point pt, Size sz)
		{
			return new Point(pt.X - sz.Width, pt.Y - sz.Height);
		}

		public static explicit operator Size(Point p)
		{
			return new Size(p.X, p.Y);
		}

		public static implicit operator PointF(Point p)
		{
			return new PointF((float)p.X, (float)p.Y);
		}

		private int x;

		private int y;

		public static readonly Point Empty;
	}
}
