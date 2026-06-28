using System;

namespace System.Drawing.Printing
{
	public abstract class PrintController
	{
		public virtual bool IsPreview
		{
			get
			{
				return false;
			}
		}

		public virtual void OnEndPage(PrintDocument document, PrintPageEventArgs e)
		{
		}

		public virtual void OnStartPrint(PrintDocument document, PrintEventArgs e)
		{
		}

		public virtual void OnEndPrint(PrintDocument document, PrintEventArgs e)
		{
		}

		public virtual Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
		{
			return null;
		}
	}
}
