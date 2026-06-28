using System;

namespace System.Drawing.Imaging
{
	public sealed class ColorMap
	{
		public Color NewColor
		{
			get
			{
				return this.newColor;
			}
			set
			{
				this.newColor = value;
			}
		}

		public Color OldColor
		{
			get
			{
				return this.oldColor;
			}
			set
			{
				this.oldColor = value;
			}
		}

		private Color newColor;

		private Color oldColor;
	}
}
