using System;

namespace System.Drawing.Printing
{
	internal abstract class GlobalPrintingServices
	{
		internal abstract PrinterSettings.StringCollection InstalledPrinters { get; }

		internal abstract IntPtr CreateGraphicsContext(PrinterSettings settings, PageSettings page_settings);

		internal abstract bool StartDoc(GraphicsPrinter gr, string doc_name, string output_file);

		internal abstract bool StartPage(GraphicsPrinter gr);

		internal abstract bool EndPage(GraphicsPrinter gr);

		internal abstract bool EndDoc(GraphicsPrinter gr);
	}
}
