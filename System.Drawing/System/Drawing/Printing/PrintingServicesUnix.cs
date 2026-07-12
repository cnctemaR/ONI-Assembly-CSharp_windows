using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Drawing.Printing
{
	internal class PrintingServicesUnix : PrintingServices
	{
		internal PrintingServicesUnix()
		{
		}

		static PrintingServicesUnix()
		{
			PrintingServicesUnix.CheckCupsInstalled();
		}

		internal static PrinterSettings.StringCollection InstalledPrinters
		{
			get
			{
				PrintingServicesUnix.LoadPrinters();
				PrinterSettings.StringCollection stringCollection = new PrinterSettings.StringCollection(new string[0]);
				foreach (object obj in PrintingServicesUnix.installed_printers.Keys)
				{
					stringCollection.Add(obj.ToString());
				}
				return stringCollection;
			}
		}

		internal override string DefaultPrinter
		{
			get
			{
				if (PrintingServicesUnix.installed_printers.Count == 0)
				{
					PrintingServicesUnix.LoadPrinters();
				}
				return PrintingServicesUnix.default_printer;
			}
		}

		private static void CheckCupsInstalled()
		{
			try
			{
				PrintingServicesUnix.cupsGetDefault();
			}
			catch (DllNotFoundException)
			{
				Console.WriteLine("libcups not found. To have printing support, you need cups installed");
				PrintingServicesUnix.cups_installed = false;
				return;
			}
			PrintingServicesUnix.cups_installed = true;
		}

		private IntPtr OpenPrinter(string printer)
		{
			try
			{
				return PrintingServicesUnix.ppdOpenFile(Marshal.PtrToStringAnsi(PrintingServicesUnix.cupsGetPPD(printer)));
			}
			catch (Exception)
			{
				Console.WriteLine("There was an error opening the printer {0}. Please check your cups installation.");
			}
			return IntPtr.Zero;
		}

		private void ClosePrinter(ref IntPtr handle)
		{
			try
			{
				if (handle != IntPtr.Zero)
				{
					PrintingServicesUnix.ppdClose(handle);
				}
			}
			finally
			{
				handle = IntPtr.Zero;
			}
		}

		private static int OpenDests(ref IntPtr ptr)
		{
			try
			{
				return PrintingServicesUnix.cupsGetDests(ref ptr);
			}
			catch
			{
				ptr = IntPtr.Zero;
			}
			return 0;
		}

		private static void CloseDests(ref IntPtr ptr, int count)
		{
			try
			{
				if (ptr != IntPtr.Zero)
				{
					PrintingServicesUnix.cupsFreeDests(count, ptr);
				}
			}
			finally
			{
				ptr = IntPtr.Zero;
			}
		}

		internal override bool IsPrinterValid(string printer)
		{
			return PrintingServicesUnix.cups_installed && !((printer == null) | (printer == string.Empty)) && PrintingServicesUnix.installed_printers.Contains(printer);
		}

		internal override void LoadPrinterSettings(string printer, PrinterSettings settings)
		{
			if (!PrintingServicesUnix.cups_installed || printer == null || printer == string.Empty)
			{
				return;
			}
			if (PrintingServicesUnix.installed_printers.Count == 0)
			{
				PrintingServicesUnix.LoadPrinters();
			}
			if (((SysPrn.Printer)PrintingServicesUnix.installed_printers[printer]).Settings != null)
			{
				SysPrn.Printer printer2 = (SysPrn.Printer)PrintingServicesUnix.installed_printers[printer];
				settings.can_duplex = printer2.Settings.can_duplex;
				settings.is_plotter = printer2.Settings.is_plotter;
				settings.landscape_angle = printer2.Settings.landscape_angle;
				settings.maximum_copies = printer2.Settings.maximum_copies;
				settings.paper_sizes = printer2.Settings.paper_sizes;
				settings.paper_sources = printer2.Settings.paper_sources;
				settings.printer_capabilities = printer2.Settings.printer_capabilities;
				settings.printer_resolutions = printer2.Settings.printer_resolutions;
				settings.supports_color = printer2.Settings.supports_color;
				return;
			}
			settings.PrinterCapabilities.Clear();
			IntPtr zero = IntPtr.Zero;
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			string text = string.Empty;
			int num = 0;
			try
			{
				num = PrintingServicesUnix.OpenDests(ref zero);
				if (num != 0)
				{
					int num2 = Marshal.SizeOf(typeof(PrintingServicesUnix.CUPS_DESTS));
					intPtr = zero;
					for (int i = 0; i < num; i++)
					{
						if (Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(intPtr)).Equals(printer))
						{
							text = printer;
							break;
						}
						intPtr = (IntPtr)((long)intPtr + (long)num2);
					}
					if (text.Equals(printer))
					{
						intPtr2 = this.OpenPrinter(printer);
						if (!(intPtr2 == IntPtr.Zero))
						{
							PrintingServicesUnix.CUPS_DESTS cups_DESTS = (PrintingServicesUnix.CUPS_DESTS)Marshal.PtrToStructure(intPtr, typeof(PrintingServicesUnix.CUPS_DESTS));
							NameValueCollection nameValueCollection = new NameValueCollection();
							NameValueCollection nameValueCollection2 = new NameValueCollection();
							NameValueCollection nameValueCollection3 = new NameValueCollection();
							string text2;
							string text3;
							PrintingServicesUnix.LoadPrinterOptions(cups_DESTS.options, cups_DESTS.num_options, intPtr2, nameValueCollection, nameValueCollection2, out text2, nameValueCollection3, out text3);
							if (settings.paper_sizes == null)
							{
								settings.paper_sizes = new PrinterSettings.PaperSizeCollection(new PaperSize[0]);
							}
							else
							{
								settings.paper_sizes.Clear();
							}
							if (settings.paper_sources == null)
							{
								settings.paper_sources = new PrinterSettings.PaperSourceCollection(new PaperSource[0]);
							}
							else
							{
								settings.paper_sources.Clear();
							}
							settings.DefaultPageSettings.PaperSource = this.LoadPrinterPaperSources(settings, text3, nameValueCollection3);
							settings.DefaultPageSettings.PaperSize = this.LoadPrinterPaperSizes(intPtr2, settings, text2, nameValueCollection2);
							this.LoadPrinterResolutionsAndDefault(printer, settings, intPtr2);
							PrintingServicesUnix.PPD_FILE ppd_FILE = (PrintingServicesUnix.PPD_FILE)Marshal.PtrToStructure(intPtr2, typeof(PrintingServicesUnix.PPD_FILE));
							settings.landscape_angle = ppd_FILE.landscape;
							settings.supports_color = ppd_FILE.color_device != 0;
							settings.can_duplex = nameValueCollection["Duplex"] != null;
							this.ClosePrinter(ref intPtr2);
							((SysPrn.Printer)PrintingServicesUnix.installed_printers[printer]).Settings = settings;
						}
					}
				}
			}
			finally
			{
				PrintingServicesUnix.CloseDests(ref zero, num);
			}
		}

		private static void LoadPrinterOptions(IntPtr options, int numOptions, IntPtr ppd, NameValueCollection list, NameValueCollection paper_names, out string defsize, NameValueCollection paper_sources, out string defsource)
		{
			int num = Marshal.SizeOf(typeof(PrintingServicesUnix.CUPS_OPTIONS));
			PrintingServicesUnix.LoadOptionList(ppd, "PageSize", paper_names, out defsize);
			PrintingServicesUnix.LoadOptionList(ppd, "InputSlot", paper_sources, out defsource);
			for (int i = 0; i < numOptions; i++)
			{
				PrintingServicesUnix.CUPS_OPTIONS cups_OPTIONS = (PrintingServicesUnix.CUPS_OPTIONS)Marshal.PtrToStructure(options, typeof(PrintingServicesUnix.CUPS_OPTIONS));
				string text = Marshal.PtrToStringAnsi(cups_OPTIONS.name);
				string text2 = Marshal.PtrToStringAnsi(cups_OPTIONS.val);
				if (text == "PageSize")
				{
					defsize = text2;
				}
				else if (text == "InputSlot")
				{
					defsource = text2;
				}
				list.Add(text, text2);
				options = (IntPtr)((long)options + (long)num);
			}
		}

		private static NameValueCollection LoadPrinterOptions(IntPtr options, int numOptions)
		{
			int num = Marshal.SizeOf(typeof(PrintingServicesUnix.CUPS_OPTIONS));
			NameValueCollection nameValueCollection = new NameValueCollection();
			for (int i = 0; i < numOptions; i++)
			{
				PrintingServicesUnix.CUPS_OPTIONS cups_OPTIONS = (PrintingServicesUnix.CUPS_OPTIONS)Marshal.PtrToStructure(options, typeof(PrintingServicesUnix.CUPS_OPTIONS));
				string text = Marshal.PtrToStringAnsi(cups_OPTIONS.name);
				string text2 = Marshal.PtrToStringAnsi(cups_OPTIONS.val);
				nameValueCollection.Add(text, text2);
				options = (IntPtr)((long)options + (long)num);
			}
			return nameValueCollection;
		}

		private static void LoadOptionList(IntPtr ppd, string option_name, NameValueCollection list, out string defoption)
		{
			IntPtr intPtr = IntPtr.Zero;
			int num = Marshal.SizeOf(typeof(PrintingServicesUnix.PPD_CHOICE));
			defoption = null;
			intPtr = PrintingServicesUnix.ppdFindOption(ppd, option_name);
			if (intPtr != IntPtr.Zero)
			{
				PrintingServicesUnix.PPD_OPTION ppd_OPTION = (PrintingServicesUnix.PPD_OPTION)Marshal.PtrToStructure(intPtr, typeof(PrintingServicesUnix.PPD_OPTION));
				defoption = ppd_OPTION.defchoice;
				intPtr = ppd_OPTION.choices;
				for (int i = 0; i < ppd_OPTION.num_choices; i++)
				{
					PrintingServicesUnix.PPD_CHOICE ppd_CHOICE = (PrintingServicesUnix.PPD_CHOICE)Marshal.PtrToStructure(intPtr, typeof(PrintingServicesUnix.PPD_CHOICE));
					list.Add(ppd_CHOICE.choice, ppd_CHOICE.text);
					intPtr = (IntPtr)((long)intPtr + (long)num);
				}
			}
		}

		internal override void LoadPrinterResolutions(string printer, PrinterSettings settings)
		{
			IntPtr intPtr = this.OpenPrinter(printer);
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			this.LoadPrinterResolutionsAndDefault(printer, settings, intPtr);
			this.ClosePrinter(ref intPtr);
		}

		private PrinterResolution ParseResolution(string resolution)
		{
			if (string.IsNullOrEmpty(resolution))
			{
				return null;
			}
			int num = resolution.IndexOf("dpi");
			if (num == -1)
			{
				return null;
			}
			resolution = resolution.Substring(0, num);
			int num2;
			int num3;
			try
			{
				if (resolution.Contains("x"))
				{
					string[] array = resolution.Split(new char[] { 'x' });
					num2 = Convert.ToInt32(array[0]);
					num3 = Convert.ToInt32(array[1]);
				}
				else
				{
					num2 = Convert.ToInt32(resolution);
					num3 = num2;
				}
			}
			catch (Exception)
			{
				return null;
			}
			return new PrinterResolution(PrinterResolutionKind.Custom, num2, num3);
		}

		private PaperSize LoadPrinterPaperSizes(IntPtr ppd_handle, PrinterSettings settings, string def_size, NameValueCollection paper_names)
		{
			PaperSize paperSize = new PaperSize(this.GetPaperKind(827, 1169), "A4", 827, 1169);
			PrintingServicesUnix.PPD_FILE ppd_FILE = (PrintingServicesUnix.PPD_FILE)Marshal.PtrToStructure(ppd_handle, typeof(PrintingServicesUnix.PPD_FILE));
			IntPtr intPtr = ppd_FILE.sizes;
			for (int i = 0; i < ppd_FILE.num_sizes; i++)
			{
				PrintingServicesUnix.PPD_SIZE ppd_SIZE = (PrintingServicesUnix.PPD_SIZE)Marshal.PtrToStructure(intPtr, typeof(PrintingServicesUnix.PPD_SIZE));
				string text = paper_names[ppd_SIZE.name];
				float num = ppd_SIZE.width * 100f / 72f;
				float num2 = ppd_SIZE.length * 100f / 72f;
				PaperKind paperKind = this.GetPaperKind((int)num, (int)num2);
				PaperSize paperSize2 = new PaperSize(paperKind, text, (int)num, (int)num2);
				paperSize2.RawKind = (int)paperKind;
				if (def_size == paperSize2.Kind.ToString())
				{
					paperSize = paperSize2;
				}
				settings.paper_sizes.Add(paperSize2);
				intPtr = (IntPtr)((long)intPtr + (long)Marshal.SizeOf<PrintingServicesUnix.PPD_SIZE>(ppd_SIZE));
			}
			return paperSize;
		}

		private PaperSource LoadPrinterPaperSources(PrinterSettings settings, string def_source, NameValueCollection paper_sources)
		{
			PaperSource paperSource = null;
			foreach (object obj in paper_sources)
			{
				string text = (string)obj;
				PaperSourceKind paperSourceKind;
				if (!(text == "Auto"))
				{
					if (!(text == "Standard"))
					{
						if (!(text == "Tray"))
						{
							if (!(text == "Envelope"))
							{
								if (!(text == "Manual"))
								{
									paperSourceKind = PaperSourceKind.Custom;
								}
								else
								{
									paperSourceKind = PaperSourceKind.Manual;
								}
							}
							else
							{
								paperSourceKind = PaperSourceKind.Envelope;
							}
						}
						else
						{
							paperSourceKind = PaperSourceKind.AutomaticFeed;
						}
					}
					else
					{
						paperSourceKind = PaperSourceKind.AutomaticFeed;
					}
				}
				else
				{
					paperSourceKind = PaperSourceKind.AutomaticFeed;
				}
				settings.paper_sources.Add(new PaperSource(paperSourceKind, paper_sources[text]));
				if (def_source == text)
				{
					paperSource = settings.paper_sources[settings.paper_sources.Count - 1];
				}
			}
			if (paperSource == null && settings.paper_sources.Count > 0)
			{
				return settings.paper_sources[0];
			}
			return paperSource;
		}

		private void LoadPrinterResolutionsAndDefault(string printer, PrinterSettings settings, IntPtr ppd_handle)
		{
			if (settings.printer_resolutions == null)
			{
				settings.printer_resolutions = new PrinterSettings.PrinterResolutionCollection(new PrinterResolution[0]);
			}
			else
			{
				settings.printer_resolutions.Clear();
			}
			NameValueCollection nameValueCollection = new NameValueCollection();
			string text;
			PrintingServicesUnix.LoadOptionList(ppd_handle, "Resolution", nameValueCollection, out text);
			foreach (object obj in nameValueCollection.Keys)
			{
				PrinterResolution printerResolution = this.ParseResolution(obj.ToString());
				settings.PrinterResolutions.Add(printerResolution);
			}
			PrinterResolution printerResolution2 = this.ParseResolution(text);
			if (printerResolution2 == null)
			{
				printerResolution2 = this.ParseResolution("300dpi");
			}
			if (nameValueCollection.Count == 0)
			{
				settings.PrinterResolutions.Add(printerResolution2);
			}
			settings.DefaultPageSettings.PrinterResolution = printerResolution2;
		}

		private static void LoadPrinters()
		{
			PrintingServicesUnix.installed_printers.Clear();
			if (!PrintingServicesUnix.cups_installed)
			{
				return;
			}
			IntPtr zero = IntPtr.Zero;
			int num = 0;
			int num2 = Marshal.SizeOf(typeof(PrintingServicesUnix.CUPS_DESTS));
			string text3;
			string text2;
			string text = (text2 = (text3 = string.Empty));
			int num3 = 0;
			try
			{
				num = PrintingServicesUnix.OpenDests(ref zero);
				IntPtr intPtr = zero;
				for (int i = 0; i < num; i++)
				{
					PrintingServicesUnix.CUPS_DESTS cups_DESTS = (PrintingServicesUnix.CUPS_DESTS)Marshal.PtrToStructure(intPtr, typeof(PrintingServicesUnix.CUPS_DESTS));
					string text4 = Marshal.PtrToStringAnsi(cups_DESTS.name);
					if (cups_DESTS.is_default == 1)
					{
						PrintingServicesUnix.default_printer = text4;
					}
					if (text2.Equals(string.Empty))
					{
						text2 = text4;
					}
					NameValueCollection nameValueCollection = PrintingServicesUnix.LoadPrinterOptions(cups_DESTS.options, cups_DESTS.num_options);
					if (nameValueCollection["printer-state"] != null)
					{
						num3 = int.Parse(nameValueCollection["printer-state"]);
					}
					if (nameValueCollection["printer-comment"] != null)
					{
						text3 = nameValueCollection["printer-state"];
					}
					string text5;
					if (num3 != 4)
					{
						if (num3 != 5)
						{
							text5 = "Ready";
						}
						else
						{
							text5 = "Stopped";
						}
					}
					else
					{
						text5 = "Printing";
					}
					PrintingServicesUnix.installed_printers.Add(text4, new SysPrn.Printer(string.Empty, text, text5, text3));
					intPtr = (IntPtr)((long)intPtr + (long)num2);
				}
			}
			finally
			{
				PrintingServicesUnix.CloseDests(ref zero, num);
			}
			if (PrintingServicesUnix.default_printer.Equals(string.Empty))
			{
				PrintingServicesUnix.default_printer = text2;
			}
		}

		internal override void GetPrintDialogInfo(string printer, ref string port, ref string type, ref string status, ref string comment)
		{
			int num = 0;
			int num2 = -1;
			bool flag = false;
			IntPtr zero = IntPtr.Zero;
			int num3 = Marshal.SizeOf(typeof(PrintingServicesUnix.CUPS_DESTS));
			if (!PrintingServicesUnix.cups_installed)
			{
				return;
			}
			try
			{
				num = PrintingServicesUnix.OpenDests(ref zero);
				if (num != 0)
				{
					IntPtr intPtr = zero;
					for (int i = 0; i < num; i++)
					{
						if (Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(intPtr)).Equals(printer))
						{
							flag = true;
							break;
						}
						intPtr = (IntPtr)((long)intPtr + (long)num3);
					}
					if (flag)
					{
						PrintingServicesUnix.CUPS_DESTS cups_DESTS = (PrintingServicesUnix.CUPS_DESTS)Marshal.PtrToStructure(intPtr, typeof(PrintingServicesUnix.CUPS_DESTS));
						NameValueCollection nameValueCollection = PrintingServicesUnix.LoadPrinterOptions(cups_DESTS.options, cups_DESTS.num_options);
						if (nameValueCollection["printer-state"] != null)
						{
							num2 = int.Parse(nameValueCollection["printer-state"]);
						}
						if (nameValueCollection["printer-comment"] != null)
						{
							comment = nameValueCollection["printer-state"];
						}
						if (num2 != 4)
						{
							if (num2 != 5)
							{
								status = "Ready";
							}
							else
							{
								status = "Stopped";
							}
						}
						else
						{
							status = "Printing";
						}
					}
				}
			}
			finally
			{
				PrintingServicesUnix.CloseDests(ref zero, num);
			}
		}

		private PaperKind GetPaperKind(int width, int height)
		{
			if (width == 827 && height == 1169)
			{
				return PaperKind.A4;
			}
			if (width == 583 && height == 827)
			{
				return PaperKind.A5;
			}
			if (width == 717 && height == 1012)
			{
				return PaperKind.B5;
			}
			if (width == 693 && height == 984)
			{
				return PaperKind.B5Envelope;
			}
			if (width == 638 && height == 902)
			{
				return PaperKind.C5Envelope;
			}
			if (width == 449 && height == 638)
			{
				return PaperKind.C6Envelope;
			}
			if (width == 1700 && height == 2200)
			{
				return PaperKind.CSheet;
			}
			if (width == 433 && height == 866)
			{
				return PaperKind.DLEnvelope;
			}
			if (width == 2200 && height == 3400)
			{
				return PaperKind.DSheet;
			}
			if (width == 3400 && height == 4400)
			{
				return PaperKind.ESheet;
			}
			if (width == 725 && height == 1050)
			{
				return PaperKind.Executive;
			}
			if (width == 850 && height == 1300)
			{
				return PaperKind.Folio;
			}
			if (width == 850 && height == 1200)
			{
				return PaperKind.GermanStandardFanfold;
			}
			if (width == 1700 && height == 1100)
			{
				return PaperKind.Ledger;
			}
			if (width == 850 && height == 1400)
			{
				return PaperKind.Legal;
			}
			if (width == 927 && height == 1500)
			{
				return PaperKind.LegalExtra;
			}
			if (width == 850 && height == 1100)
			{
				return PaperKind.Letter;
			}
			if (width == 927 && height == 1200)
			{
				return PaperKind.LetterExtra;
			}
			if (width == 850 && height == 1269)
			{
				return PaperKind.LetterPlus;
			}
			if (width == 387 && height == 750)
			{
				return PaperKind.MonarchEnvelope;
			}
			if (width == 387 && height == 887)
			{
				return PaperKind.Number9Envelope;
			}
			if (width == 413 && height == 950)
			{
				return PaperKind.Number10Envelope;
			}
			if (width == 450 && height == 1037)
			{
				return PaperKind.Number11Envelope;
			}
			if (width == 475 && height == 1100)
			{
				return PaperKind.Number12Envelope;
			}
			if (width == 500 && height == 1150)
			{
				return PaperKind.Number14Envelope;
			}
			if (width == 363 && height == 650)
			{
				return PaperKind.PersonalEnvelope;
			}
			if (width == 1000 && height == 1100)
			{
				return PaperKind.Standard10x11;
			}
			if (width == 1000 && height == 1400)
			{
				return PaperKind.Standard10x14;
			}
			if (width == 1100 && height == 1700)
			{
				return PaperKind.Standard11x17;
			}
			if (width == 1200 && height == 1100)
			{
				return PaperKind.Standard12x11;
			}
			if (width == 1500 && height == 1100)
			{
				return PaperKind.Standard15x11;
			}
			if (width == 900 && height == 1100)
			{
				return PaperKind.Standard9x11;
			}
			if (width == 550 && height == 850)
			{
				return PaperKind.Statement;
			}
			if (width == 1100 && height == 1700)
			{
				return PaperKind.Tabloid;
			}
			if (width == 1487 && height == 1100)
			{
				return PaperKind.USStandardFanfold;
			}
			return PaperKind.Custom;
		}

		internal static int GetCupsOptions(PrinterSettings printer_settings, PageSettings page_settings, out IntPtr options)
		{
			options = IntPtr.Zero;
			PaperSize paperSize = page_settings.PaperSize;
			int num = paperSize.Width * 72 / 100;
			int num2 = paperSize.Height * 72 / 100;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat(new string[]
			{
				"copies=",
				printer_settings.Copies.ToString(),
				" Collate=",
				printer_settings.Collate.ToString(),
				" ColorModel=",
				page_settings.Color ? "Color" : "Black",
				" PageSize=",
				string.Format("Custom.{0}x{1}", num, num2),
				" landscape=",
				page_settings.Landscape.ToString()
			}));
			if (printer_settings.CanDuplex)
			{
				if (printer_settings.Duplex == Duplex.Simplex)
				{
					stringBuilder.Append(" Duplex=None");
				}
				else
				{
					stringBuilder.Append(" Duplex=DuplexNoTumble");
				}
			}
			return PrintingServicesUnix.cupsParseOptions(stringBuilder.ToString(), 0, ref options);
		}

		internal static bool StartDoc(GraphicsPrinter gr, string doc_name, string output_file)
		{
			((PrintingServicesUnix.DOCINFO)PrintingServicesUnix.doc_info[gr.Hdc]).title = doc_name;
			return true;
		}

		internal static bool EndDoc(GraphicsPrinter gr)
		{
			PrintingServicesUnix.DOCINFO docinfo = (PrintingServicesUnix.DOCINFO)PrintingServicesUnix.doc_info[gr.Hdc];
			gr.Graphics.Dispose();
			IntPtr intPtr;
			int cupsOptions = PrintingServicesUnix.GetCupsOptions(docinfo.settings, docinfo.default_page_settings, out intPtr);
			PrintingServicesUnix.cupsPrintFile(docinfo.settings.PrinterName, docinfo.filename, docinfo.title, cupsOptions, intPtr);
			PrintingServicesUnix.cupsFreeOptions(cupsOptions, intPtr);
			PrintingServicesUnix.doc_info.Remove(gr.Hdc);
			if (PrintingServicesUnix.tmpfile != null)
			{
				try
				{
					File.Delete(PrintingServicesUnix.tmpfile);
				}
				catch
				{
				}
			}
			return true;
		}

		internal static bool StartPage(GraphicsPrinter gr)
		{
			return true;
		}

		internal static bool EndPage(GraphicsPrinter gr)
		{
			PrintingServicesUnix.GdipGetPostScriptSavePage(gr.Hdc);
			return true;
		}

		internal static IntPtr CreateGraphicsContext(PrinterSettings settings, PageSettings default_page_settings)
		{
			IntPtr zero = IntPtr.Zero;
			string text;
			if (!settings.PrintToFile)
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				int capacity = stringBuilder.Capacity;
				PrintingServicesUnix.cupsTempFd(stringBuilder, capacity);
				text = stringBuilder.ToString();
				PrintingServicesUnix.tmpfile = text;
			}
			else
			{
				text = settings.PrintFileName;
			}
			PaperSize paperSize = default_page_settings.PaperSize;
			int num;
			int num2;
			if (default_page_settings.Landscape)
			{
				num = paperSize.Height;
				num2 = paperSize.Width;
			}
			else
			{
				num = paperSize.Width;
				num2 = paperSize.Height;
			}
			PrintingServicesUnix.GdipGetPostScriptGraphicsContext(text, num * 72 / 100, num2 * 72 / 100, (double)default_page_settings.PrinterResolution.X, (double)default_page_settings.PrinterResolution.Y, ref zero);
			PrintingServicesUnix.DOCINFO docinfo = default(PrintingServicesUnix.DOCINFO);
			docinfo.filename = text;
			docinfo.settings = settings;
			docinfo.default_page_settings = default_page_settings;
			PrintingServicesUnix.doc_info.Add(zero, docinfo);
			return zero;
		}

		[DllImport("libcups", CharSet = CharSet.Ansi)]
		private static extern int cupsGetDests(ref IntPtr dests);

		[DllImport("libcups")]
		private static extern void cupsFreeDests(int num_dests, IntPtr dests);

		[DllImport("libcups", CharSet = CharSet.Ansi)]
		private static extern IntPtr cupsTempFd(StringBuilder sb, int len);

		[DllImport("libcups", CharSet = CharSet.Ansi)]
		private static extern IntPtr cupsGetDefault();

		[DllImport("libcups", CharSet = CharSet.Ansi)]
		private static extern int cupsPrintFile(string printer, string filename, string title, int num_options, IntPtr options);

		[DllImport("libcups", CharSet = CharSet.Ansi)]
		private static extern IntPtr cupsGetPPD(string printer);

		[DllImport("libcups", CharSet = CharSet.Ansi)]
		private static extern IntPtr ppdOpenFile(string filename);

		[DllImport("libcups", CharSet = CharSet.Ansi)]
		private static extern IntPtr ppdFindOption(IntPtr ppd_file, string keyword);

		[DllImport("libcups")]
		private static extern void ppdClose(IntPtr ppd);

		[DllImport("libcups", CharSet = CharSet.Ansi)]
		private static extern int cupsParseOptions(string arg, int number_of_options, ref IntPtr options);

		[DllImport("libcups")]
		private static extern void cupsFreeOptions(int number_options, IntPtr options);

		[DllImport("gdiplus.dll", CharSet = CharSet.Ansi)]
		private static extern int GdipGetPostScriptGraphicsContext(string filename, int with, int height, double dpix, double dpiy, ref IntPtr graphics);

		[DllImport("gdiplus.dll")]
		private static extern int GdipGetPostScriptSavePage(IntPtr graphics);

		private static Hashtable doc_info = new Hashtable();

		private static bool cups_installed;

		private static Hashtable installed_printers = new Hashtable();

		private static string default_printer = string.Empty;

		private static string tmpfile;

		public struct DOCINFO
		{
			public PrinterSettings settings;

			public PageSettings default_page_settings;

			public string title;

			public string filename;
		}

		public struct PPD_SIZE
		{
			public int marked;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 42)]
			public string name;

			public float width;

			public float length;

			public float left;

			public float bottom;

			public float right;

			public float top;
		}

		public struct PPD_GROUP
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
			public string text;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 42)]
			public string name;

			public int num_options;

			public IntPtr options;

			public int num_subgroups;

			public IntPtr subgrups;
		}

		public struct PPD_OPTION
		{
			public byte conflicted;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
			public string keyword;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
			public string defchoice;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
			public string text;

			public int ui;

			public int section;

			public float order;

			public int num_choices;

			public IntPtr choices;
		}

		public struct PPD_CHOICE
		{
			public byte marked;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
			public string choice;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
			public string text;

			public IntPtr code;

			public IntPtr option;
		}

		public struct PPD_FILE
		{
			public int language_level;

			public int color_device;

			public int variable_sizes;

			public int accurate_screens;

			public int contone_only;

			public int landscape;

			public int model_number;

			public int manual_copies;

			public int throughput;

			public int colorspace;

			public IntPtr patches;

			public int num_emulations;

			public IntPtr emulations;

			public IntPtr jcl_begin;

			public IntPtr jcl_ps;

			public IntPtr jcl_end;

			public IntPtr lang_encoding;

			public IntPtr lang_version;

			public IntPtr modelname;

			public IntPtr ttrasterizer;

			public IntPtr manufacturer;

			public IntPtr product;

			public IntPtr nickname;

			public IntPtr shortnickname;

			public int num_groups;

			public IntPtr groups;

			public int num_sizes;

			public IntPtr sizes;
		}

		public struct CUPS_OPTIONS
		{
			public IntPtr name;

			public IntPtr val;
		}

		public struct CUPS_DESTS
		{
			public IntPtr name;

			public IntPtr instance;

			public int is_default;

			public int num_options;

			public IntPtr options;
		}
	}
}
