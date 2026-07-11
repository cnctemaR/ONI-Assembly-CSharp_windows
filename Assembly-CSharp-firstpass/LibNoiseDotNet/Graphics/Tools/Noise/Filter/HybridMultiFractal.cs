using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class HybridMultiFractal : FilterModule, IModule3D, IModule, IModule2D
	{
		public HybridMultiFractal()
		{
			this._gain = 1f;
			this._offset = 0.7f;
			this._spectralExponent = 0.25f;
			base.ComputeSpectralWeights();
		}

		public float GetValue(float x, float y, float z)
		{
			x *= this._frequency;
			y *= this._frequency;
			z *= this._frequency;
			float num = this._source3D.GetValue(x, y, z) + this._offset;
			float num2 = this._gain * num;
			x *= this._lacunarity;
			y *= this._lacunarity;
			z *= this._lacunarity;
			int num3 = 1;
			while ((double)num2 > 0.001 && (float)num3 < this._octaveCount)
			{
				if ((double)num2 > 1.0)
				{
					num2 = 1f;
				}
				float num4 = (this._offset + this._source3D.GetValue(x, y, z)) * this._spectralWeights[num3];
				num4 *= num2;
				num += num4;
				num2 *= this._gain * num4;
				x *= this._lacunarity;
				y *= this._lacunarity;
				z *= this._lacunarity;
				num3++;
			}
			float num5 = this._octaveCount - (float)((int)this._octaveCount);
			if (num5 > 0f)
			{
				float num4 = this._source3D.GetValue(x, y, z);
				num4 *= this._spectralWeights[num3];
				num4 *= num5;
				num += num4;
			}
			return num;
		}

		public float GetValue(float x, float y)
		{
			x *= this._frequency;
			y *= this._frequency;
			float num = this._source2D.GetValue(x, y) + this._offset;
			float num2 = this._gain * num;
			x *= this._lacunarity;
			y *= this._lacunarity;
			int num3 = 1;
			while ((double)num2 > 0.001 && (float)num3 < this._octaveCount)
			{
				if ((double)num2 > 1.0)
				{
					num2 = 1f;
				}
				float num4 = (this._offset + this._source2D.GetValue(x, y)) * this._spectralWeights[num3];
				num4 *= num2;
				num += num4;
				num2 *= this._gain * num4;
				x *= this._lacunarity;
				y *= this._lacunarity;
				num3++;
			}
			float num5 = this._octaveCount - (float)((int)this._octaveCount);
			if (num5 > 0f)
			{
				float num4 = this._source2D.GetValue(x, y);
				num4 *= this._spectralWeights[num3];
				num4 *= num5;
				num += num4;
			}
			return num;
		}
	}
}
