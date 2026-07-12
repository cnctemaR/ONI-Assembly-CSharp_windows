using System;

namespace System.Drawing
{
	internal struct IconInfo
	{
		public bool IsIcon
		{
			get
			{
				return this.fIcon == 1;
			}
			set
			{
				this.fIcon = (value ? 1 : 0);
			}
		}

		private int fIcon;

		public int xHotspot;

		public int yHotspot;

		public IntPtr hbmMask;

		public IntPtr hbmColor;
	}
}
