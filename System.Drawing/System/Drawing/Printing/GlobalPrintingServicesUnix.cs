using System;

namespace System.Drawing.Printing
{
	internal class GlobalPrintingServicesUnix : GlobalPrintingServices
	{
		internal override PrinterSettings.StringCollection InstalledPrinters
		{
			get
			{
				return PrintingServicesUnix.InstalledPrinters;
			}
		}

		internal override IntPtr CreateGraphicsContext(PrinterSettings settings, PageSettings default_page_settings)
		{
			return PrintingServicesUnix.CreateGraphicsContext(settings, default_page_settings);
		}

		internal override bool StartDoc(GraphicsPrinter gr, string doc_name, string output_file)
		{
			return PrintingServicesUnix.StartDoc(gr, doc_name, output_file);
		}

		internal override bool EndDoc(GraphicsPrinter gr)
		{
			return PrintingServicesUnix.EndDoc(gr);
		}

		internal override bool StartPage(GraphicsPrinter gr)
		{
			return PrintingServicesUnix.StartPage(gr);
		}

		internal override bool EndPage(GraphicsPrinter gr)
		{
			return PrintingServicesUnix.EndPage(gr);
		}
	}
}
