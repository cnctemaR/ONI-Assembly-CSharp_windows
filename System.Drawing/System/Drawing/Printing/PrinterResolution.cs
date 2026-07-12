using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Drawing.Printing
{
	[Serializable]
	public class PrinterResolution
	{
		public PrinterResolution()
		{
			this._kind = PrinterResolutionKind.Custom;
		}

		internal PrinterResolution(PrinterResolutionKind kind, int x, int y)
		{
			this._kind = kind;
			this._x = x;
			this._y = y;
		}

		public PrinterResolutionKind Kind
		{
			get
			{
				return this._kind;
			}
			set
			{
				if (value < PrinterResolutionKind.High || value > PrinterResolutionKind.Custom)
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(PrinterResolutionKind));
				}
				this._kind = value;
			}
		}

		public int X
		{
			get
			{
				return this._x;
			}
			set
			{
				this._x = value;
			}
		}

		public int Y
		{
			get
			{
				return this._y;
			}
			set
			{
				this._y = value;
			}
		}

		public override string ToString()
		{
			if (this._kind != PrinterResolutionKind.Custom)
			{
				return "[PrinterResolution " + this.Kind.ToString() + "]";
			}
			return string.Concat(new string[]
			{
				"[PrinterResolution X=",
				this.X.ToString(CultureInfo.InvariantCulture),
				" Y=",
				this.Y.ToString(CultureInfo.InvariantCulture),
				"]"
			});
		}

		private int _x;

		private int _y;

		private PrinterResolutionKind _kind;
	}
}
