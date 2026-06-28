using System;

namespace System.Drawing.Printing
{
	internal abstract class PrintingServices
	{
		internal abstract string DefaultPrinter { get; }

		internal abstract bool IsPrinterValid(string printer);

		internal abstract void LoadPrinterSettings(string printer, PrinterSettings settings);

		internal abstract void LoadPrinterResolutions(string printer, PrinterSettings settings);

		internal abstract void GetPrintDialogInfo(string printer, ref string port, ref string type, ref string status, ref string comment);

		internal void LoadDefaultResolutions(PrinterSettings.PrinterResolutionCollection col)
		{
			col.Add(new PrinterResolution(-4, -1, PrinterResolutionKind.High));
			col.Add(new PrinterResolution(-3, -1, PrinterResolutionKind.Medium));
			col.Add(new PrinterResolution(-2, -1, PrinterResolutionKind.Low));
			col.Add(new PrinterResolution(-1, -1, PrinterResolutionKind.Draft));
		}
	}
}
