using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	[ComVisible(true)]
	[TypeConverter(typeof(RectangleConverter))]
	[Serializable]
	public struct Rectangle
	{
		public Rectangle(Point location, Size size)
		{
			this.x = location.X;
			this.y = location.Y;
			this.width = size.Width;
			this.height = size.Height;
		}

		public Rectangle(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public static Rectangle Ceiling(RectangleF value)
		{
			checked
			{
				int num = (int)Math.Ceiling((double)value.X);
				int num2 = (int)Math.Ceiling((double)value.Y);
				int num3 = (int)Math.Ceiling((double)value.Width);
				int num4 = (int)Math.Ceiling((double)value.Height);
				return new Rectangle(num, num2, num3, num4);
			}
		}

		public static Rectangle FromLTRB(int left, int top, int right, int bottom)
		{
			return new Rectangle(left, top, right - left, bottom - top);
		}

		public static Rectangle Inflate(Rectangle rect, int x, int y)
		{
			Rectangle rectangle = new Rectangle(rect.Location, rect.Size);
			rectangle.Inflate(x, y);
			return rectangle;
		}

		public void Inflate(int width, int height)
		{
			this.Inflate(new Size(width, height));
		}

		public void Inflate(Size size)
		{
			this.x -= size.Width;
			this.y -= size.Height;
			this.Width += size.Width * 2;
			this.Height += size.Height * 2;
		}

		public static Rectangle Intersect(Rectangle a, Rectangle b)
		{
			if (!a.IntersectsWithInclusive(b))
			{
				return Rectangle.Empty;
			}
			return Rectangle.FromLTRB(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right), Math.Min(a.Bottom, b.Bottom));
		}

		public void Intersect(Rectangle rect)
		{
			this = Rectangle.Intersect(this, rect);
		}

		public static Rectangle Round(RectangleF value)
		{
			checked
			{
				int num = (int)Math.Round((double)value.X);
				int num2 = (int)Math.Round((double)value.Y);
				int num3 = (int)Math.Round((double)value.Width);
				int num4 = (int)Math.Round((double)value.Height);
				return new Rectangle(num, num2, num3, num4);
			}
		}

		public static Rectangle Truncate(RectangleF value)
		{
			checked
			{
				int num = (int)value.X;
				int num2 = (int)value.Y;
				int num3 = (int)value.Width;
				int num4 = (int)value.Height;
				return new Rectangle(num, num2, num3, num4);
			}
		}

		public static Rectangle Union(Rectangle a, Rectangle b)
		{
			return Rectangle.FromLTRB(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
		}

		[Browsable(false)]
		public int Bottom
		{
			get
			{
				return this.y + this.height;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		[Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				return this.x == 0 && this.y == 0 && this.width == 0 && this.height == 0;
			}
		}

		[Browsable(false)]
		public int Left
		{
			get
			{
				return this.X;
			}
		}

		[Browsable(false)]
		public Point Location
		{
			get
			{
				return new Point(this.x, this.y);
			}
			set
			{
				this.x = value.X;
				this.y = value.Y;
			}
		}

		[Browsable(false)]
		public int Right
		{
			get
			{
				return this.X + this.Width;
			}
		}

		[Browsable(false)]
		public Size Size
		{
			get
			{
				return new Size(this.Width, this.Height);
			}
			set
			{
				this.Width = value.Width;
				this.Height = value.Height;
			}
		}

		[Browsable(false)]
		public int Top
		{
			get
			{
				return this.y;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
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

		public bool Contains(int x, int y)
		{
			return x >= this.Left && x < this.Right && y >= this.Top && y < this.Bottom;
		}

		public bool Contains(Point pt)
		{
			return this.Contains(pt.X, pt.Y);
		}

		public bool Contains(Rectangle rect)
		{
			return rect == Rectangle.Intersect(this, rect);
		}

		public override bool Equals(object obj)
		{
			return obj is Rectangle && this == (Rectangle)obj;
		}

		public override int GetHashCode()
		{
			return (this.height + this.width) ^ (this.x + this.y);
		}

		public bool IntersectsWith(Rectangle rect)
		{
			return this.Left < rect.Right && this.Right > rect.Left && this.Top < rect.Bottom && this.Bottom > rect.Top;
		}

		private bool IntersectsWithInclusive(Rectangle r)
		{
			return this.Left <= r.Right && this.Right >= r.Left && this.Top <= r.Bottom && this.Bottom >= r.Top;
		}

		public void Offset(int x, int y)
		{
			this.x += x;
			this.y += y;
		}

		public void Offset(Point pos)
		{
			this.x += pos.X;
			this.y += pos.Y;
		}

		public override string ToString()
		{
			return string.Format("{{X={0},Y={1},Width={2},Height={3}}}", new object[] { this.x, this.y, this.width, this.height });
		}

		public static bool operator ==(Rectangle left, Rectangle right)
		{
			return left.Location == right.Location && left.Size == right.Size;
		}

		public static bool operator !=(Rectangle left, Rectangle right)
		{
			return left.Location != right.Location || left.Size != right.Size;
		}

		private int x;

		private int y;

		private int width;

		private int height;

		public static readonly Rectangle Empty;
	}
}
