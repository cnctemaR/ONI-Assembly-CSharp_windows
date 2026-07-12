using System;

namespace System.Drawing.Imaging
{
	public sealed class ColorMap
	{
		public ColorMap()
		{
			this._oldColor = default(Color);
			this._newColor = default(Color);
		}

		public Color OldColor
		{
			get
			{
				return this._oldColor;
			}
			set
			{
				this._oldColor = value;
			}
		}

		public Color NewColor
		{
			get
			{
				return this._newColor;
			}
			set
			{
				this._newColor = value;
			}
		}

		private Color _oldColor;

		private Color _newColor;
	}
}
