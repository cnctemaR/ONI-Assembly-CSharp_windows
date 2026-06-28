using System;

namespace System.Drawing.Printing
{
	public sealed class PreviewPageInfo
	{
		public PreviewPageInfo(Image image, Size physicalSize)
		{
			this.image = image;
			this.physicalSize = physicalSize;
		}

		public Image Image
		{
			get
			{
				return this.image;
			}
		}

		public Size PhysicalSize
		{
			get
			{
				return this.physicalSize;
			}
		}

		private Image image;

		private Size physicalSize;
	}
}
