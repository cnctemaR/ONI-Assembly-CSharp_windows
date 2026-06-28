using System;
using System.ComponentModel;

namespace System.Drawing
{
	[Serializable]
	public struct RectangleF
	{
		public RectangleF(PointF location, SizeF size)
		{
			this.x = location.X;
			this.y = location.Y;
			this.width = size.Width;
			this.height = size.Height;
		}

		public RectangleF(float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public static RectangleF FromLTRB(float left, float top, float right, float bottom)
		{
			return new RectangleF(left, top, right - left, bottom - top);
		}

		public static RectangleF Inflate(RectangleF rect, float x, float y)
		{
			RectangleF rectangleF = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
			rectangleF.Inflate(x, y);
			return rectangleF;
		}

		public void Inflate(float x, float y)
		{
			this.Inflate(new SizeF(x, y));
		}

		public void Inflate(SizeF size)
		{
			this.x -= size.Width;
			this.y -= size.Height;
			this.width += size.Width * 2f;
			this.height += size.Height * 2f;
		}

		public static RectangleF Intersect(RectangleF a, RectangleF b)
		{
			if (!a.IntersectsWithInclusive(b))
			{
				return RectangleF.Empty;
			}
			return RectangleF.FromLTRB(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right), Math.Min(a.Bottom, b.Bottom));
		}

		public void Intersect(RectangleF rect)
		{
			this = RectangleF.Intersect(this, rect);
		}

		public static RectangleF Union(RectangleF a, RectangleF b)
		{
			return RectangleF.FromLTRB(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
		}

		[Browsable(false)]
		public float Bottom
		{
			get
			{
				return this.Y + this.Height;
			}
		}

		public float Height
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
				return this.width <= 0f || this.height <= 0f;
			}
		}

		[Browsable(false)]
		public float Left
		{
			get
			{
				return this.X;
			}
		}

		[Browsable(false)]
		public PointF Location
		{
			get
			{
				return new PointF(this.x, this.y);
			}
			set
			{
				this.x = value.X;
				this.y = value.Y;
			}
		}

		[Browsable(false)]
		public float Right
		{
			get
			{
				return this.X + this.Width;
			}
		}

		[Browsable(false)]
		public SizeF Size
		{
			get
			{
				return new SizeF(this.width, this.height);
			}
			set
			{
				this.width = value.Width;
				this.height = value.Height;
			}
		}

		[Browsable(false)]
		public float Top
		{
			get
			{
				return this.Y;
			}
		}

		public float Width
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

		public float X
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

		public float Y
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

		public bool Contains(float x, float y)
		{
			return x >= this.Left && x < this.Right && y >= this.Top && y < this.Bottom;
		}

		public bool Contains(PointF pt)
		{
			return this.Contains(pt.X, pt.Y);
		}

		public bool Contains(RectangleF rect)
		{
			return rect == RectangleF.Intersect(this, rect);
		}

		public override bool Equals(object obj)
		{
			return obj is RectangleF && this == (RectangleF)obj;
		}

		public override int GetHashCode()
		{
			return (int)(this.x + this.y + this.width + this.height);
		}

		public bool IntersectsWith(RectangleF rect)
		{
			return this.Left < rect.Right && this.Right > rect.Left && this.Top < rect.Bottom && this.Bottom > rect.Top;
		}

		private bool IntersectsWithInclusive(RectangleF r)
		{
			return this.Left <= r.Right && this.Right >= r.Left && this.Top <= r.Bottom && this.Bottom >= r.Top;
		}

		public void Offset(float x, float y)
		{
			this.X += x;
			this.Y += y;
		}

		public void Offset(PointF pos)
		{
			this.Offset(pos.X, pos.Y);
		}

		public override string ToString()
		{
			return string.Format("{{X={0},Y={1},Width={2},Height={3}}}", new object[] { this.x, this.y, this.width, this.height });
		}

		public static bool operator ==(RectangleF left, RectangleF right)
		{
			return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
		}

		public static bool operator !=(RectangleF left, RectangleF right)
		{
			return left.X != right.X || left.Y != right.Y || left.Width != right.Width || left.Height != right.Height;
		}

		public static implicit operator RectangleF(Rectangle r)
		{
			return new RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
		}

		private float x;

		private float y;

		private float width;

		private float height;

		public static readonly RectangleF Empty;
	}
}
