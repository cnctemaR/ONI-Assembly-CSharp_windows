using System;
using LibNoiseDotNet.Graphics.Tools.Noise.Utils;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap16 : DataMap<ushort>, IMap2D<ushort>
	{
		public Heightmap16()
		{
			this._borderValue = 0;
			base.AllocateBuffer();
		}

		public Heightmap16(int width, int height)
		{
			this._borderValue = 0;
			base.AllocateBuffer(width, height);
		}

		public Heightmap16(Heightmap16 copy)
		{
			this._borderValue = 0;
			base.CopyFrom(copy);
		}

		public void MinMax(out ushort min, out ushort max)
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
			return 16;
		}

		protected override ushort MaxvalofT()
		{
			return ushort.MaxValue;
		}

		protected override ushort MinvalofT()
		{
			return 0;
		}
	}
}
