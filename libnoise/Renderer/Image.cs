using System;
using LibNoiseDotNet.Graphics.Tools.Noise.Utils;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Image : DataMap<Color>, IMap2D<Color>
	{
		public Image()
		{
			this._hasMaxDimension = true;
			this._maxHeight = 32767;
			this._maxWidth = 32767;
			this._borderValue = Color.TRANSPARENT;
			base.AllocateBuffer();
		}

		public Image(int width, int height)
		{
			this._hasMaxDimension = true;
			this._maxHeight = 32767;
			this._maxWidth = 32767;
			this._borderValue = Color.WHITE;
			base.AllocateBuffer(width, height);
		}

		public Image(Image copy)
		{
			this._hasMaxDimension = true;
			this._maxHeight = 32767;
			this._maxWidth = 32767;
			this._borderValue = Color.WHITE;
			base.CopyFrom(copy);
		}

		public void MinMax(out Color min, out Color max)
		{
			Color color;
			max = (color = this.MinvalofT());
			min = color;
			if (this._data != null && this._data.Length > 0)
			{
				Color color2;
				max = (color2 = this._data[0]);
				min = color2;
				for (int i = 0; i < this._data.Length; i++)
				{
					if (min > this._data[i])
					{
						min = this._data[i];
					}
					else if (max < this._data[i])
					{
						max = this._data[i];
					}
				}
			}
		}

		protected override int SizeofT()
		{
			return 64;
		}

		protected override Color MaxvalofT()
		{
			return Color.WHITE;
		}

		protected override Color MinvalofT()
		{
			return Color.BLACK;
		}

		public const int RASTER_MAX_WIDTH = 32767;

		public const int RASTER_MAX_HEIGHT = 32767;
	}
}
