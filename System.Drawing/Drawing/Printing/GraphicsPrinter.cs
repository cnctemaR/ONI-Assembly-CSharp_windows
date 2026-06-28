using System;

namespace System.Drawing.Printing
{
	internal class GraphicsPrinter
	{
		internal GraphicsPrinter(Graphics gr, IntPtr dc)
		{
			this.graphics = gr;
			this.hDC = dc;
		}

		internal Graphics Graphics
		{
			get
			{
				return this.graphics;
			}
			set
			{
				this.graphics = value;
			}
		}

		internal IntPtr Hdc
		{
			get
			{
				return this.hDC;
			}
		}

		private Graphics graphics;

		private IntPtr hDC;
	}
}
