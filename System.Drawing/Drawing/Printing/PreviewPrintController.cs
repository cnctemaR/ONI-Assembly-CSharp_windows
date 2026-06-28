using System;
using System.Collections;

namespace System.Drawing.Printing
{
	public class PreviewPrintController : PrintController
	{
		public PreviewPrintController()
		{
			this.pageInfoList = new ArrayList();
		}

		public override bool IsPreview
		{
			get
			{
				return true;
			}
		}

		[MonoTODO]
		public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
		{
		}

		[MonoTODO]
		public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
		{
			if (!document.PrinterSettings.IsValid)
			{
				throw new InvalidPrinterException(document.PrinterSettings);
			}
			foreach (object obj in this.pageInfoList)
			{
				PreviewPageInfo previewPageInfo = (PreviewPageInfo)obj;
				previewPageInfo.Image.Dispose();
			}
			this.pageInfoList.Clear();
		}

		[MonoTODO]
		public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
		{
		}

		[MonoTODO]
		public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
		{
			Image image = new Bitmap(e.PageSettings.PaperSize.Width, e.PageSettings.PaperSize.Height);
			PreviewPageInfo previewPageInfo = new PreviewPageInfo(image, new Size(e.PageSettings.PaperSize.Width, e.PageSettings.PaperSize.Height));
			this.pageInfoList.Add(previewPageInfo);
			Graphics graphics = Graphics.FromImage(previewPageInfo.Image);
			graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(0, 0), new Size(image.Width, image.Height)));
			return graphics;
		}

		public virtual bool UseAntiAlias
		{
			get
			{
				return this.useantialias;
			}
			set
			{
				this.useantialias = value;
			}
		}

		public PreviewPageInfo[] GetPreviewPageInfo()
		{
			PreviewPageInfo[] array = new PreviewPageInfo[this.pageInfoList.Count];
			this.pageInfoList.CopyTo(array);
			return array;
		}

		private bool useantialias;

		private ArrayList pageInfoList;
	}
}
