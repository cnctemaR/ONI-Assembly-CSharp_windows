using System;
using LibNoiseDotNet.Graphics.Tools.Noise.Utils;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class NoiseMap : DataMap<float>, IMap2D<float>
	{
		public NoiseMap()
		{
			this._hasMaxDimension = false;
			this._borderValue = 0f;
			base.AllocateBuffer();
		}

		public NoiseMap(int width, int height)
		{
			this._hasMaxDimension = false;
			this._borderValue = 0f;
			base.AllocateBuffer(width, height);
		}

		public NoiseMap(NoiseMap copy)
		{
			this._hasMaxDimension = false;
			this._borderValue = 0f;
			base.CopyFrom(copy);
		}

		public void MinMax(out float min, out float max)
		{
			min = (max = 0f);
			if (this._data != null && this._data.Length != 0)
			{
				min = (max = this._data[0]);
				for (int i = 1; i < this._data.Length; i++)
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
			return 32;
		}

		protected override float MaxvalofT()
		{
			return float.MaxValue;
		}

		protected override float MinvalofT()
		{
			return float.MinValue;
		}
	}
}
