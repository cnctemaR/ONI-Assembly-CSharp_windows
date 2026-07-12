using System;
using System.ComponentModel;

namespace System.Drawing.Printing
{
	public class PrintEventArgs : CancelEventArgs
	{
		public PrintEventArgs()
		{
		}

		internal PrintEventArgs(PrintAction action)
		{
			this.action = action;
		}

		public PrintAction PrintAction
		{
			get
			{
				return this.action;
			}
		}

		internal GraphicsPrinter GraphicsContext
		{
			get
			{
				return this.graphics_context;
			}
			set
			{
				this.graphics_context = value;
			}
		}

		private GraphicsPrinter graphics_context;

		private PrintAction action;
	}
}
