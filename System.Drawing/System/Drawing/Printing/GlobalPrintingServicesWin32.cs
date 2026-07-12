using System;

namespace System.Drawing.Printing
{
	internal class GlobalPrintingServicesWin32 : GlobalPrintingServices
	{
		internal override PrinterSettings.StringCollection InstalledPrinters
		{
			get
			{
				return PrintingServicesWin32.InstalledPrinters;
			}
		}

		internal override IntPtr CreateGraphicsContext(PrinterSettings settings, PageSettings default_page_settings)
		{
			return PrintingServicesWin32.CreateGraphicsContext(settings, default_page_settings);
		}

		internal override bool StartDoc(GraphicsPrinter gr, string doc_name, string output_file)
		{
			return PrintingServicesWin32.StartDoc(gr, doc_name, output_file);
		}

		internal override bool EndDoc(GraphicsPrinter gr)
		{
			return PrintingServicesWin32.EndDoc(gr);
		}

		internal override bool StartPage(GraphicsPrinter gr)
		{
			return PrintingServicesWin32.StartPage(gr);
		}

		internal override bool EndPage(GraphicsPrinter gr)
		{
			return PrintingServicesWin32.EndPage(gr);
		}
	}
}
