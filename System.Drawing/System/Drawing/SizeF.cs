using System;
using System.ComponentModel;
using System.Numerics.Hashing;

namespace System.Drawing
{
	[TypeConverter(typeof(SizeFConverter))]
	[Serializable]
	public struct SizeF : IEquatable<SizeF>
	{
		public SizeF(SizeF size)
		{
			this.width = size.width;
			this.height = size.height;
		}

		public SizeF(PointF pt)
		{
			this.width = pt.X;
			this.height = pt.Y;
		}

		public SizeF(float width, float height)
		{
			this.width = width;
			this.height = height;
		}

		public static SizeF operator +(SizeF sz1, SizeF sz2)
		{
			return SizeF.Add(sz1, sz2);
		}

		public static SizeF operator -(SizeF sz1, SizeF sz2)
		{
			return SizeF.Subtract(sz1, sz2);
		}

		public static SizeF operator *(float left, SizeF right)
		{
			return SizeF.Multiply(right, left);
		}

		public static SizeF operator *(SizeF left, float right)
		{
			return SizeF.Multiply(left, right);
		}

		public static SizeF operator /(SizeF left, float right)
		{
			return new SizeF(left.width / right, left.height / right);
		}

		public static bool operator ==(SizeF sz1, SizeF sz2)
		{
			return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
		}

		public static bool operator !=(SizeF sz1, SizeF sz2)
		{
			return !(sz1 == sz2);
		}

		public static explicit operator PointF(SizeF size)
		{
			return new PointF(size.Width, size.Height);
		}

		[Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				return this.width == 0f && this.height == 0f;
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

		public static SizeF Add(SizeF sz1, SizeF sz2)
		{
			return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		public static SizeF Subtract(SizeF sz1, SizeF sz2)
		{
			return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public override bool Equals(object obj)
		{
			return obj is SizeF && this.Equals((SizeF)obj);
		}

		public bool Equals(SizeF other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			return HashHelpers.Combine(this.Width.GetHashCode(), this.Height.GetHashCode());
		}

		public PointF ToPointF()
		{
			return (PointF)this;
		}

		public Size ToSize()
		{
			return Size.Truncate(this);
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"{Width=",
				this.width.ToString(),
				", Height=",
				this.height.ToString(),
				"}"
			});
		}

		private static SizeF Multiply(SizeF size, float multiplier)
		{
			return new SizeF(size.width * multiplier, size.height * multiplier);
		}

		public static readonly SizeF Empty;

		private float width;

		private float height;
	}
}
