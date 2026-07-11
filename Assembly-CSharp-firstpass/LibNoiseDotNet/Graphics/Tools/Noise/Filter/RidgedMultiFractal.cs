using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class RidgedMultiFractal : FilterModule, IModule3D, IModule, IModule2D
	{
		public RidgedMultiFractal()
		{
			this._gain = 2f;
			this._offset = 1f;
			this._spectralExponent = 0.9f;
			base.ComputeSpectralWeights();
		}

		public float GetValue(float x, float y, float z)
		{
			x *= this._frequency;
			y *= this._frequency;
			z *= this._frequency;
			float num = this._source3D.GetValue(x, y, z);
			if ((double)num < 0.0)
			{
				num = -num;
			}
			num = this._offset - num;
			num *= num;
			float num2 = num;
			float num3 = 1f;
			int num4 = 1;
			while ((double)num3 > 0.001 && (float)num4 < this._octaveCount)
			{
				x *= this._lacunarity;
				y *= this._lacunarity;
				z *= this._lacunarity;
				num3 = Libnoise.Clamp01(num * this._gain);
				num = this._source3D.GetValue(x, y, z);
				if (num < 0f)
				{
					num = -num;
				}
				num = this._offset - num;
				num *= num;
				num *= num3;
				num2 += num * this._spectralWeights[num4];
				num4++;
			}
			return num2;
		}

		public float GetValue(float x, float y)
		{
			x *= this._frequency;
			y *= this._frequency;
			float num = this._source2D.GetValue(x, y);
			if (num < 0f)
			{
				num = -num;
			}
			num = this._offset - num;
			num *= num;
			float num2 = num;
			float num3 = 1f;
			int num4 = 1;
			while ((double)num3 > 0.001 && (float)num4 < this._octaveCount)
			{
				x *= this._lacunarity;
				y *= this._lacunarity;
				num3 = Libnoise.Clamp01(num * this._gain);
				num = this._source2D.GetValue(x, y);
				if ((double)num < 0.0)
				{
					num = -num;
				}
				num = this._offset - num;
				num *= num;
				num *= num3;
				num2 += num * this._spectralWeights[num4];
				num4++;
			}
			return num2;
		}
	}
}
