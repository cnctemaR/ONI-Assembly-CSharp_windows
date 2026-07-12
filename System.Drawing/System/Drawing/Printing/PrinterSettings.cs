using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace System.Drawing.Printing
{
	[Serializable]
	public class PrinterSettings : ICloneable
	{
		public PrinterSettings()
			: this(SysPrn.CreatePrintingService())
		{
		}

		internal PrinterSettings(PrintingServices printing_services)
		{
			this.printing_services = printing_services;
			this.printer_name = printing_services.DefaultPrinter;
			this.ResetToDefaults();
			printing_services.LoadPrinterSettings(this.printer_name, this);
		}

		private void ResetToDefaults()
		{
			this.printer_resolutions = null;
			this.paper_sizes = null;
			this.paper_sources = null;
			this.default_pagesettings = null;
			this.maximum_page = 9999;
			this.copies = 1;
			this.collate = true;
		}

		public bool CanDuplex
		{
			get
			{
				return this.can_duplex;
			}
		}

		public bool Collate
		{
			get
			{
				return this.collate;
			}
			set
			{
				this.collate = value;
			}
		}

		public short Copies
		{
			get
			{
				return this.copies;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the Copies property is less than zero.");
				}
				this.copies = value;
			}
		}

		public PageSettings DefaultPageSettings
		{
			get
			{
				if (this.default_pagesettings == null)
				{
					this.default_pagesettings = new PageSettings(this, this.SupportsColor, false, new PaperSize("A4", 827, 1169), new PaperSource(PaperSourceKind.FormSource, "Tray"), new PrinterResolution(PrinterResolutionKind.Medium, 200, 200));
				}
				return this.default_pagesettings;
			}
		}

		public Duplex Duplex
		{
			get
			{
				return this.duplex;
			}
			set
			{
				this.duplex = value;
			}
		}

		public int FromPage
		{
			get
			{
				return this.from_page;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the FromPage property is less than zero");
				}
				this.from_page = value;
			}
		}

		public static PrinterSettings.StringCollection InstalledPrinters
		{
			get
			{
				return SysPrn.GlobalService.InstalledPrinters;
			}
		}

		public bool IsDefaultPrinter
		{
			get
			{
				return this.printer_name == this.printing_services.DefaultPrinter;
			}
		}

		public bool IsPlotter
		{
			get
			{
				return this.is_plotter;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.printing_services.IsPrinterValid(this.printer_name);
			}
		}

		public int LandscapeAngle
		{
			get
			{
				return this.landscape_angle;
			}
		}

		public int MaximumCopies
		{
			get
			{
				return this.maximum_copies;
			}
		}

		public int MaximumPage
		{
			get
			{
				return this.maximum_page;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the MaximumPage property is less than zero");
				}
				this.maximum_page = value;
			}
		}

		public int MinimumPage
		{
			get
			{
				return this.minimum_page;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the MaximumPage property is less than zero");
				}
				this.minimum_page = value;
			}
		}

		public PrinterSettings.PaperSizeCollection PaperSizes
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidPrinterException(this);
				}
				return this.paper_sizes;
			}
		}

		public PrinterSettings.PaperSourceCollection PaperSources
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidPrinterException(this);
				}
				return this.paper_sources;
			}
		}

		public string PrintFileName
		{
			get
			{
				return this.print_filename;
			}
			set
			{
				this.print_filename = value;
			}
		}

		public string PrinterName
		{
			get
			{
				return this.printer_name;
			}
			set
			{
				if (this.printer_name == value)
				{
					return;
				}
				this.printer_name = value;
				this.printing_services.LoadPrinterSettings(this.printer_name, this);
			}
		}

		public PrinterSettings.PrinterResolutionCollection PrinterResolutions
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidPrinterException(this);
				}
				if (this.printer_resolutions == null)
				{
					this.printer_resolutions = new PrinterSettings.PrinterResolutionCollection(new PrinterResolution[0]);
					this.printing_services.LoadPrinterResolutions(this.printer_name, this);
				}
				return this.printer_resolutions;
			}
		}

		public PrintRange PrintRange
		{
			get
			{
				return this.print_range;
			}
			set
			{
				if (value != PrintRange.AllPages && value != PrintRange.Selection && value != PrintRange.SomePages)
				{
					throw new InvalidEnumArgumentException("The value of the PrintRange property is not one of the PrintRange values");
				}
				this.print_range = value;
			}
		}

		public bool PrintToFile
		{
			get
			{
				return this.print_tofile;
			}
			set
			{
				this.print_tofile = value;
			}
		}

		public bool SupportsColor
		{
			get
			{
				return this.supports_color;
			}
		}

		public int ToPage
		{
			get
			{
				return this.to_page;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the ToPage property is less than zero");
				}
				this.to_page = value;
			}
		}

		internal NameValueCollection PrinterCapabilities
		{
			get
			{
				if (this.printer_capabilities == null)
				{
					this.printer_capabilities = new NameValueCollection();
				}
				return this.printer_capabilities;
			}
		}

		public object Clone()
		{
			return new PrinterSettings(this.printing_services);
		}

		[MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
		public Graphics CreateMeasurementGraphics()
		{
			throw new NotImplementedException();
		}

		[MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
		public Graphics CreateMeasurementGraphics(bool honorOriginAtMargins)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
		public Graphics CreateMeasurementGraphics(PageSettings pageSettings)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
		public Graphics CreateMeasurementGraphics(PageSettings pageSettings, bool honorOriginAtMargins)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("PrinterSettings.GetHdevmode")]
		public IntPtr GetHdevmode()
		{
			throw new NotImplementedException();
		}

		[MonoTODO("PrinterSettings.GetHdevmode")]
		public IntPtr GetHdevmode(PageSettings pageSettings)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("PrinterSettings.GetHdevname")]
		public IntPtr GetHdevnames()
		{
			throw new NotImplementedException();
		}

		[MonoTODO("IsDirectPrintingSupported")]
		public bool IsDirectPrintingSupported(Image image)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("IsDirectPrintingSupported")]
		public bool IsDirectPrintingSupported(ImageFormat imageFormat)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("PrinterSettings.SetHdevmode")]
		public void SetHdevmode(IntPtr hdevmode)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("PrinterSettings.SetHdevnames")]
		public void SetHdevnames(IntPtr hdevnames)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Printer [PrinterSettings ",
				this.printer_name,
				" Copies=",
				this.copies.ToString(),
				" Collate=",
				this.collate.ToString(),
				" Duplex=",
				this.can_duplex.ToString(),
				" FromPage=",
				this.from_page.ToString(),
				" LandscapeAngle=",
				this.landscape_angle.ToString(),
				" MaximumCopies=",
				this.maximum_copies.ToString(),
				" OutputPort= ToPage=",
				this.to_page.ToString(),
				"]"
			});
		}

		private string printer_name;

		private string print_filename;

		private short copies;

		private int maximum_page;

		private int minimum_page;

		private int from_page;

		private int to_page;

		private bool collate;

		private PrintRange print_range;

		internal int maximum_copies;

		internal bool can_duplex;

		internal bool supports_color;

		internal int landscape_angle;

		private bool print_tofile;

		internal PrinterSettings.PrinterResolutionCollection printer_resolutions;

		internal PrinterSettings.PaperSizeCollection paper_sizes;

		internal PrinterSettings.PaperSourceCollection paper_sources;

		private PageSettings default_pagesettings;

		private Duplex duplex;

		internal bool is_plotter;

		private PrintingServices printing_services;

		internal NameValueCollection printer_capabilities;

		public class PaperSourceCollection : ICollection, IEnumerable
		{
			public PaperSourceCollection(PaperSource[] array)
			{
				foreach (PaperSource paperSource in array)
				{
					this._PaperSources.Add(paperSource);
				}
			}

			public int Count
			{
				get
				{
					return this._PaperSources.Count;
				}
			}

			int ICollection.Count
			{
				get
				{
					return this._PaperSources.Count;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			[EditorBrowsable(EditorBrowsableState.Never)]
			public int Add(PaperSource paperSource)
			{
				return this._PaperSources.Add(paperSource);
			}

			public void CopyTo(PaperSource[] paperSources, int index)
			{
				throw new NotImplementedException();
			}

			public virtual PaperSource this[int index]
			{
				get
				{
					return this._PaperSources[index] as PaperSource;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._PaperSources.GetEnumerator();
			}

			public IEnumerator GetEnumerator()
			{
				return this._PaperSources.GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				this._PaperSources.CopyTo(array, index);
			}

			internal void Clear()
			{
				this._PaperSources.Clear();
			}

			private ArrayList _PaperSources = new ArrayList();
		}

		public class PaperSizeCollection : ICollection, IEnumerable
		{
			public PaperSizeCollection(PaperSize[] array)
			{
				foreach (PaperSize paperSize in array)
				{
					this._PaperSizes.Add(paperSize);
				}
			}

			public int Count
			{
				get
				{
					return this._PaperSizes.Count;
				}
			}

			int ICollection.Count
			{
				get
				{
					return this._PaperSizes.Count;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			[EditorBrowsable(EditorBrowsableState.Never)]
			public int Add(PaperSize paperSize)
			{
				return this._PaperSizes.Add(paperSize);
			}

			public void CopyTo(PaperSize[] paperSizes, int index)
			{
				throw new NotImplementedException();
			}

			public virtual PaperSize this[int index]
			{
				get
				{
					return this._PaperSizes[index] as PaperSize;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._PaperSizes.GetEnumerator();
			}

			public IEnumerator GetEnumerator()
			{
				return this._PaperSizes.GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				this._PaperSizes.CopyTo(array, index);
			}

			internal void Clear()
			{
				this._PaperSizes.Clear();
			}

			private ArrayList _PaperSizes = new ArrayList();
		}

		public class PrinterResolutionCollection : ICollection, IEnumerable
		{
			public PrinterResolutionCollection(PrinterResolution[] array)
			{
				foreach (PrinterResolution printerResolution in array)
				{
					this._PrinterResolutions.Add(printerResolution);
				}
			}

			public int Count
			{
				get
				{
					return this._PrinterResolutions.Count;
				}
			}

			int ICollection.Count
			{
				get
				{
					return this._PrinterResolutions.Count;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			[EditorBrowsable(EditorBrowsableState.Never)]
			public int Add(PrinterResolution printerResolution)
			{
				return this._PrinterResolutions.Add(printerResolution);
			}

			public void CopyTo(PrinterResolution[] printerResolutions, int index)
			{
				throw new NotImplementedException();
			}

			public virtual PrinterResolution this[int index]
			{
				get
				{
					return this._PrinterResolutions[index] as PrinterResolution;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._PrinterResolutions.GetEnumerator();
			}

			public IEnumerator GetEnumerator()
			{
				return this._PrinterResolutions.GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				this._PrinterResolutions.CopyTo(array, index);
			}

			internal void Clear()
			{
				this._PrinterResolutions.Clear();
			}

			private ArrayList _PrinterResolutions = new ArrayList();
		}

		public class StringCollection : ICollection, IEnumerable
		{
			public StringCollection(string[] array)
			{
				foreach (string text in array)
				{
					this._Strings.Add(text);
				}
			}

			public int Count
			{
				get
				{
					return this._Strings.Count;
				}
			}

			int ICollection.Count
			{
				get
				{
					return this._Strings.Count;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			public virtual string this[int index]
			{
				get
				{
					return this._Strings[index] as string;
				}
			}

			[EditorBrowsable(EditorBrowsableState.Never)]
			public int Add(string value)
			{
				return this._Strings.Add(value);
			}

			public void CopyTo(string[] strings, int index)
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._Strings.GetEnumerator();
			}

			public IEnumerator GetEnumerator()
			{
				return this._Strings.GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				this._Strings.CopyTo(array, index);
			}

			private ArrayList _Strings = new ArrayList();
		}
	}
}
