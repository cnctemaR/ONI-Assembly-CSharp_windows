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
			col.Add(new PrinterResolution(PrinterResolutionKind.High, -4, -1));
			col.Add(new PrinterResolution(PrinterResolutionKind.Medium, -3, -1));
			col.Add(new PrinterResolution(PrinterResolutionKind.Low, -2, -1));
			col.Add(new PrinterResolution(PrinterResolutionKind.Draft, -1, -1));
		}
	}
}
