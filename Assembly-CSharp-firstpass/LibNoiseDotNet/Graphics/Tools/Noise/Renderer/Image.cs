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
				max = (color = this._data[0]);
				min = color;
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

		int IMap2D<Color>.get_Width()
		{
			return base.Width;
		}

		int IMap2D<Color>.get_Height()
		{
			return base.Height;
		}

		Color IMap2D<Color>.get_BorderValue()
		{
			return base.BorderValue;
		}

		void IMap2D<Color>.set_BorderValue(Color value)
		{
			base.BorderValue = value;
		}

		Color IMap2D<Color>.GetValue(int x, int y)
		{
			return base.GetValue(x, y);
		}

		void IMap2D<Color>.SetValue(int x, int y, Color value)
		{
			base.SetValue(x, y, value);
		}

		void IMap2D<Color>.SetSize(int width, int height)
		{
			base.SetSize(width, height);
		}

		void IMap2D<Color>.Reset()
		{
			base.Reset();
		}

		void IMap2D<Color>.Clear(Color value)
		{
			base.Clear(value);
		}

		void IMap2D<Color>.Clear()
		{
			base.Clear();
		}

		public const int RASTER_MAX_WIDTH = 32767;

		public const int RASTER_MAX_HEIGHT = 32767;
	}
}
