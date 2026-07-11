using System;
using LibNoiseDotNet.Graphics.Tools.Noise.Utils;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap8 : DataMap<byte>, IMap2D<byte>
	{
		public Heightmap8()
		{
			this._borderValue = 0;
			base.AllocateBuffer();
		}

		public Heightmap8(int width, int height)
		{
			this._borderValue = 0;
			base.AllocateBuffer(width, height);
		}

		public Heightmap8(Heightmap8 copy)
		{
			this._borderValue = 0;
			base.CopyFrom(copy);
		}

		public void MinMax(out byte min, out byte max)
		{
			min = (max = 0);
			if (this._data != null && this._data.Length > 0)
			{
				min = (max = this._data[0]);
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
			return 8;
		}

		protected override byte MaxvalofT()
		{
			return byte.MaxValue;
		}

		protected override byte MinvalofT()
		{
			return 0;
		}

		int IMap2D<byte>.get_Width()
		{
			return base.Width;
		}

		int IMap2D<byte>.get_Height()
		{
			return base.Height;
		}

		byte IMap2D<byte>.get_BorderValue()
		{
			return base.BorderValue;
		}

		void IMap2D<byte>.set_BorderValue(byte value)
		{
			base.BorderValue = value;
		}

		byte IMap2D<byte>.GetValue(int x, int y)
		{
			return base.GetValue(x, y);
		}

		void IMap2D<byte>.SetValue(int x, int y, byte value)
		{
			base.SetValue(x, y, value);
		}

		void IMap2D<byte>.SetSize(int width, int height)
		{
			base.SetSize(width, height);
		}

		void IMap2D<byte>.Reset()
		{
			base.Reset();
		}

		void IMap2D<byte>.Clear(byte value)
		{
			base.Clear(value);
		}

		void IMap2D<byte>.Clear()
		{
			base.Clear();
		}
	}
}
