using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	[TypeConverter(typeof(SizeFConverter))]
	[ComVisible(true)]
	[Serializable]
	public struct SizeF
	{
		public SizeF(PointF pt)
		{
			this.width = pt.X;
			this.height = pt.Y;
		}

		public SizeF(SizeF size)
		{
			this.width = size.Width;
			this.height = size.Height;
		}

		public SizeF(float width, float height)
		{
			this.width = width;
			this.height = height;
		}

		[Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				return (double)this.width == 0.0 && (double)this.height == 0.0;
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

		public override bool Equals(object obj)
		{
			return obj is SizeF && this == (SizeF)obj;
		}

		public override int GetHashCode()
		{
			return (int)this.width ^ (int)this.height;
		}

		public PointF ToPointF()
		{
			return new PointF(this.width, this.height);
		}

		public Size ToSize()
		{
			checked
			{
				int num = (int)this.width;
				int num2 = (int)this.height;
				return new Size(num, num2);
			}
		}

		public override string ToString()
		{
			return string.Format("{{Width={0}, Height={1}}}", this.width.ToString(CultureInfo.CurrentCulture), this.height.ToString(CultureInfo.CurrentCulture));
		}

		public static SizeF Add(SizeF sz1, SizeF sz2)
		{
			return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		public static SizeF Subtract(SizeF sz1, SizeF sz2)
		{
			return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public static SizeF operator +(SizeF sz1, SizeF sz2)
		{
			return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		public static bool operator ==(SizeF sz1, SizeF sz2)
		{
			return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
		}

		public static bool operator !=(SizeF sz1, SizeF sz2)
		{
			return sz1.Width != sz2.Width || sz1.Height != sz2.Height;
		}

		public static SizeF operator -(SizeF sz1, SizeF sz2)
		{
			return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public static explicit operator PointF(SizeF size)
		{
			return new PointF(size.Width, size.Height);
		}

		private float width;

		private float height;

		public static readonly SizeF Empty;
	}
}
