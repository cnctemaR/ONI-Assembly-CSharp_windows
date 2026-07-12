using System;
using System.ComponentModel;
using System.Numerics.Hashing;

namespace System.Drawing
{
	[TypeConverter(typeof(SizeConverter))]
	[Serializable]
	public struct Size : IEquatable<Size>
	{
		public Size(Point pt)
		{
			this.width = pt.X;
			this.height = pt.Y;
		}

		public Size(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public static implicit operator SizeF(Size p)
		{
			return new SizeF((float)p.Width, (float)p.Height);
		}

		public static Size operator +(Size sz1, Size sz2)
		{
			return Size.Add(sz1, sz2);
		}

		public static Size operator -(Size sz1, Size sz2)
		{
			return Size.Subtract(sz1, sz2);
		}

		public static Size operator *(int left, Size right)
		{
			return Size.Multiply(right, left);
		}

		public static Size operator *(Size left, int right)
		{
			return Size.Multiply(left, right);
		}

		public static Size operator /(Size left, int right)
		{
			return new Size(left.width / right, left.height / right);
		}

		public static SizeF operator *(float left, Size right)
		{
			return Size.Multiply(right, left);
		}

		public static SizeF operator *(Size left, float right)
		{
			return Size.Multiply(left, right);
		}

		public static SizeF operator /(Size left, float right)
		{
			return new SizeF((float)left.width / right, (float)left.height / right);
		}

		public static bool operator ==(Size sz1, Size sz2)
		{
			return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
		}

		public static bool operator !=(Size sz1, Size sz2)
		{
			return !(sz1 == sz2);
		}

		public static explicit operator Point(Size size)
		{
			return new Point(size.Width, size.Height);
		}

		[Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				return this.width == 0 && this.height == 0;
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

		public static Size Add(Size sz1, Size sz2)
		{
			return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		public static Size Ceiling(SizeF value)
		{
			return new Size((int)Math.Ceiling((double)value.Width), (int)Math.Ceiling((double)value.Height));
		}

		public static Size Subtract(Size sz1, Size sz2)
		{
			return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public static Size Truncate(SizeF value)
		{
			return new Size((int)value.Width, (int)value.Height);
		}

		public static Size Round(SizeF value)
		{
			return new Size((int)Math.Round((double)value.Width), (int)Math.Round((double)value.Height));
		}

		public override bool Equals(object obj)
		{
			return obj is Size && this.Equals((Size)obj);
		}

		public bool Equals(Size other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			return HashHelpers.Combine(this.Width, this.Height);
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

		private static Size Multiply(Size size, int multiplier)
		{
			return new Size(size.width * multiplier, size.height * multiplier);
		}

		private static SizeF Multiply(Size size, float multiplier)
		{
			return new SizeF((float)size.width * multiplier, (float)size.height * multiplier);
		}

		public static readonly Size Empty;

		private int width;

		private int height;
	}
}
