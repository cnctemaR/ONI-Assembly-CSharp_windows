using System;

namespace System.Drawing.Printing
{
	internal class SysPrn
	{
		internal static PrintingServices CreatePrintingService()
		{
			if (SysPrn.is_unix)
			{
				return new PrintingServicesUnix();
			}
			return new PrintingServicesWin32();
		}

		internal static GlobalPrintingServices GlobalService
		{
			get
			{
				if (SysPrn.global_printing_services == null)
				{
					if (SysPrn.is_unix)
					{
						SysPrn.global_printing_services = new GlobalPrintingServicesUnix();
					}
					else
					{
						SysPrn.global_printing_services = new GlobalPrintingServicesWin32();
					}
				}
				return SysPrn.global_printing_services;
			}
		}

		internal static void GetPrintDialogInfo(string printer, ref string port, ref string type, ref string status, ref string comment)
		{
			SysPrn.CreatePrintingService().GetPrintDialogInfo(printer, ref port, ref type, ref status, ref comment);
		}

		private static GlobalPrintingServices global_printing_services;

		private static bool is_unix = GDIPlus.RunningOnUnix();

		internal class Printer
		{
			public Printer(string port, string type, string status, string comment)
			{
				this.Port = port;
				this.Type = type;
				this.Status = status;
				this.Comment = comment;
			}

			public readonly string Comment;

			public readonly string Port;

			public readonly string Type;

			public readonly string Status;

			public PrinterSettings Settings;
		}
	}
}
