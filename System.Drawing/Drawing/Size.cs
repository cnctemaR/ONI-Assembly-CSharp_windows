using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	[ComVisible(true)]
	[TypeConverter(typeof(SizeConverter))]
	[Serializable]
	public struct Size
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

		public static Size Ceiling(SizeF value)
		{
			checked
			{
				int num = (int)Math.Ceiling((double)value.Width);
				int num2 = (int)Math.Ceiling((double)value.Height);
				return new Size(num, num2);
			}
		}

		public static Size Round(SizeF value)
		{
			checked
			{
				int num = (int)Math.Round((double)value.Width);
				int num2 = (int)Math.Round((double)value.Height);
				return new Size(num, num2);
			}
		}

		public static Size Truncate(SizeF value)
		{
			checked
			{
				int num = (int)value.Width;
				int num2 = (int)value.Height;
				return new Size(num, num2);
			}
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

		public override bool Equals(object obj)
		{
			return obj is Size && this == (Size)obj;
		}

		public override int GetHashCode()
		{
			return this.width ^ this.height;
		}

		public override string ToString()
		{
			return string.Format("{{Width={0}, Height={1}}}", this.width, this.height);
		}

		public static Size Add(Size sz1, Size sz2)
		{
			return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		public static Size Subtract(Size sz1, Size sz2)
		{
			return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public static Size operator +(Size sz1, Size sz2)
		{
			return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		public static bool operator ==(Size sz1, Size sz2)
		{
			return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
		}

		public static bool operator !=(Size sz1, Size sz2)
		{
			return sz1.Width != sz2.Width || sz1.Height != sz2.Height;
		}

		public static Size operator -(Size sz1, Size sz2)
		{
			return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public static explicit operator Point(Size size)
		{
			return new Point(size.Width, size.Height);
		}

		public static implicit operator SizeF(Size p)
		{
			return new SizeF((float)p.Width, (float)p.Height);
		}

		private int width;

		private int height;

		public static readonly Size Empty;
	}
}
