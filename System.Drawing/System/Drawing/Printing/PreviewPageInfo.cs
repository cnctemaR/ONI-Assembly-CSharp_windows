using System;

namespace System.Drawing.Printing
{
	public sealed class PreviewPageInfo
	{
		public PreviewPageInfo(Image image, Size physicalSize)
		{
			this._image = image;
			this._physicalSize = physicalSize;
		}

		public Image Image
		{
			get
			{
				return this._image;
			}
		}

		public Size PhysicalSize
		{
			get
			{
				return this._physicalSize;
			}
		}

		private Image _image;

		private Size _physicalSize = Size.Empty;
	}
}
