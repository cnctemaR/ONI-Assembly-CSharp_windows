using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Drawing.Printing
{
	[TypeConverter(typeof(MarginsConverter))]
	[Serializable]
	public class Margins : ICloneable
	{
		[OnDeserialized]
		private void OnDeserializedMethod(StreamingContext context)
		{
			if (this._doubleLeft == 0.0 && this._left != 0)
			{
				this._doubleLeft = (double)this._left;
			}
			if (this._doubleRight == 0.0 && this._right != 0)
			{
				this._doubleRight = (double)this._right;
			}
			if (this._doubleTop == 0.0 && this._top != 0)
			{
				this._doubleTop = (double)this._top;
			}
			if (this._doubleBottom == 0.0 && this._bottom != 0)
			{
				this._doubleBottom = (double)this._bottom;
			}
		}

		public Margins()
			: this(100, 100, 100, 100)
		{
		}

		public Margins(int left, int right, int top, int bottom)
		{
			this.CheckMargin(left, "left");
			this.CheckMargin(right, "right");
			this.CheckMargin(top, "top");
			this.CheckMargin(bottom, "bottom");
			this._left = left;
			this._right = right;
			this._top = top;
			this._bottom = bottom;
			this._doubleLeft = (double)left;
			this._doubleRight = (double)right;
			this._doubleTop = (double)top;
			this._doubleBottom = (double)bottom;
		}

		public int Left
		{
			get
			{
				return this._left;
			}
			set
			{
				this.CheckMargin(value, "Left");
				this._left = value;
				this._doubleLeft = (double)value;
			}
		}

		public int Right
		{
			get
			{
				return this._right;
			}
			set
			{
				this.CheckMargin(value, "Right");
				this._right = value;
				this._doubleRight = (double)value;
			}
		}

		public int Top
		{
			get
			{
				return this._top;
			}
			set
			{
				this.CheckMargin(value, "Top");
				this._top = value;
				this._doubleTop = (double)value;
			}
		}

		public int Bottom
		{
			get
			{
				return this._bottom;
			}
			set
			{
				this.CheckMargin(value, "Bottom");
				this._bottom = value;
				this._doubleBottom = (double)value;
			}
		}

		internal double DoubleLeft
		{
			get
			{
				return this._doubleLeft;
			}
			set
			{
				this.Left = (int)Math.Round(value);
				this._doubleLeft = value;
			}
		}

		internal double DoubleRight
		{
			get
			{
				return this._doubleRight;
			}
			set
			{
				this.Right = (int)Math.Round(value);
				this._doubleRight = value;
			}
		}

		internal double DoubleTop
		{
			get
			{
				return this._doubleTop;
			}
			set
			{
				this.Top = (int)Math.Round(value);
				this._doubleTop = value;
			}
		}

		internal double DoubleBottom
		{
			get
			{
				return this._doubleBottom;
			}
			set
			{
				this.Bottom = (int)Math.Round(value);
				this._doubleBottom = value;
			}
		}

		private void CheckMargin(int margin, string name)
		{
			if (margin < 0)
			{
				throw new ArgumentException(SR.Format("Value of '{1}' is not valid for '{0}'. '{0}' must be greater than or equal to {2}.", new object[] { name, margin, "0" }));
			}
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			Margins margins = obj as Margins;
			return margins == this || (!(margins == null) && (margins.Left == this.Left && margins.Right == this.Right && margins.Top == this.Top) && margins.Bottom == this.Bottom);
		}

		public override int GetHashCode()
		{
			int left = this.Left;
			uint right = (uint)this.Right;
			uint top = (uint)this.Top;
			uint bottom = (uint)this.Bottom;
			return left ^ (int)((right << 13) | (right >> 19)) ^ (int)((top << 26) | (top >> 6)) ^ (int)((bottom << 7) | (bottom >> 25));
		}

		public static bool operator ==(Margins m1, Margins m2)
		{
			return m1 == null == (m2 == null) && (m1 == null || (m1.Left == m2.Left && m1.Top == m2.Top && m1.Right == m2.Right && m1.Bottom == m2.Bottom));
		}

		public static bool operator !=(Margins m1, Margins m2)
		{
			return !(m1 == m2);
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[Margins Left=",
				this.Left.ToString(CultureInfo.InvariantCulture),
				" Right=",
				this.Right.ToString(CultureInfo.InvariantCulture),
				" Top=",
				this.Top.ToString(CultureInfo.InvariantCulture),
				" Bottom=",
				this.Bottom.ToString(CultureInfo.InvariantCulture),
				"]"
			});
		}

		private int _left;

		private int _right;

		private int _bottom;

		private int _top;

		[OptionalField]
		private double _doubleLeft;

		[OptionalField]
		private double _doubleRight;

		[OptionalField]
		private double _doubleTop;

		[OptionalField]
		private double _doubleBottom;
	}
}
