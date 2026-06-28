using System;

namespace System.Drawing.Printing
{
	[Serializable]
	public class PrinterResolution
	{
		public PrinterResolution()
		{
		}

		internal PrinterResolution(int x, int y, PrinterResolutionKind kind)
		{
			this.x = x;
			this.y = y;
			this.kind = kind;
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

		public PrinterResolutionKind Kind
		{
			get
			{
				return this.kind;
			}
			set
			{
				this.kind = value;
			}
		}

		public override string ToString()
		{
			if (this.kind != PrinterResolutionKind.Custom)
			{
				return "[PrinterResolution " + this.kind.ToString() + "]";
			}
			return string.Concat(new object[] { "[PrinterResolution X=", this.x, " Y=", this.y, "]" });
		}

		private PrinterResolutionKind kind;

		private int x;

		private int y;
	}
}
