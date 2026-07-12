using System;

namespace System.Drawing.Printing
{
	public class StandardPrintController : PrintController
	{
		public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
		{
			SysPrn.GlobalService.EndPage(e.GraphicsContext);
		}

		public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
		{
			IntPtr intPtr = SysPrn.GlobalService.CreateGraphicsContext(document.PrinterSettings, document.DefaultPageSettings);
			e.GraphicsContext = new GraphicsPrinter(null, intPtr);
			SysPrn.GlobalService.StartDoc(e.GraphicsContext, document.DocumentName, string.Empty);
		}

		public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
		{
			SysPrn.GlobalService.EndDoc(e.GraphicsContext);
		}

		public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
		{
			SysPrn.GlobalService.StartPage(e.GraphicsContext);
			return e.Graphics;
		}
	}
}
