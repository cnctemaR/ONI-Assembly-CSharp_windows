using System;
using System.ComponentModel;

namespace System.Drawing.Printing
{
	[DefaultEvent("PrintPage")]
	[DefaultProperty("DocumentName")]
	[ToolboxItemFilter("System.Drawing.Printing", ToolboxItemFilterType.Allow)]
	public class PrintDocument : Component
	{
		public PrintDocument()
		{
			this.documentname = "document";
			this.printersettings = new PrinterSettings();
			this.defaultpagesettings = (PageSettings)this.printersettings.DefaultPageSettings.Clone();
			this.printcontroller = new StandardPrintController();
		}

		[SRDescription("The settings for the current page.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public PageSettings DefaultPageSettings
		{
			get
			{
				return this.defaultpagesettings;
			}
			set
			{
				this.defaultpagesettings = value;
			}
		}

		[DefaultValue("document")]
		[SRDescription("The name of the document.")]
		public string DocumentName
		{
			get
			{
				return this.documentname;
			}
			set
			{
				this.documentname = value;
			}
		}

		[Browsable(false)]
		[SRDescription("The print controller object.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PrintController PrintController
		{
			get
			{
				return this.printcontroller;
			}
			set
			{
				this.printcontroller = value;
			}
		}

		[SRDescription("The current settings for the active printer.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PrinterSettings PrinterSettings
		{
			get
			{
				return this.printersettings;
			}
			set
			{
				this.printersettings = ((value == null) ? new PrinterSettings() : value);
			}
		}

		[SRDescription("Determines if the origin is set at the specified margins.")]
		[DefaultValue(false)]
		public bool OriginAtMargins
		{
			get
			{
				return this.originAtMargins;
			}
			set
			{
				this.originAtMargins = value;
			}
		}

		public void Print()
		{
			PrintEventArgs e = new PrintEventArgs();
			this.OnBeginPrint(e);
			if (e.Cancel)
			{
				return;
			}
			this.PrintController.OnStartPrint(this, e);
			if (e.Cancel)
			{
				return;
			}
			Graphics graphics = null;
			if (e.GraphicsContext != null)
			{
				graphics = Graphics.FromHdc(e.GraphicsContext.Hdc);
				e.GraphicsContext.Graphics = graphics;
			}
			PrintPageEventArgs e3;
			do
			{
				QueryPageSettingsEventArgs e2 = new QueryPageSettingsEventArgs(this.DefaultPageSettings.Clone() as PageSettings);
				this.OnQueryPageSettings(e2);
				PageSettings pageSettings = e2.PageSettings;
				e3 = new PrintPageEventArgs(graphics, pageSettings.Bounds, new Rectangle(0, 0, pageSettings.PaperSize.Width, pageSettings.PaperSize.Height), pageSettings);
				e3.GraphicsContext = e.GraphicsContext;
				Graphics graphics2 = this.PrintController.OnStartPage(this, e3);
				e3.SetGraphics(graphics2);
				if (!e3.Cancel)
				{
					this.OnPrintPage(e3);
				}
				this.PrintController.OnEndPage(this, e3);
			}
			while (!e3.Cancel && e3.HasMorePages);
			this.OnEndPrint(e);
			this.PrintController.OnEndPrint(this, e);
		}

		public override string ToString()
		{
			return "[PrintDocument " + this.DocumentName + "]";
		}

		protected virtual void OnBeginPrint(PrintEventArgs e)
		{
			if (this.BeginPrint != null)
			{
				this.BeginPrint(this, e);
			}
		}

		protected virtual void OnEndPrint(PrintEventArgs e)
		{
			if (this.EndPrint != null)
			{
				this.EndPrint(this, e);
			}
		}

		protected virtual void OnPrintPage(PrintPageEventArgs e)
		{
			if (this.PrintPage != null)
			{
				this.PrintPage(this, e);
			}
		}

		protected virtual void OnQueryPageSettings(QueryPageSettingsEventArgs e)
		{
			if (this.QueryPageSettings != null)
			{
				this.QueryPageSettings(this, e);
			}
		}

		[SRDescription("Raised when printing begins")]
		public event PrintEventHandler BeginPrint;

		[SRDescription("Raised when printing ends")]
		public event PrintEventHandler EndPrint;

		[SRDescription("Raised when printing of a new page begins")]
		public event PrintPageEventHandler PrintPage;

		[SRDescription("Raised before printing of a new page begins")]
		public event QueryPageSettingsEventHandler QueryPageSettings;

		private PageSettings defaultpagesettings;

		private PrinterSettings printersettings;

		private PrintController printcontroller;

		private string documentname;

		private bool originAtMargins;
	}
}
