using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class HeterogeneousMultiFractal : FilterModule, IModule3D, IModule, IModule2D
	{
		public float GetValue(float x, float y, float z)
		{
			x *= this._frequency;
			y *= this._frequency;
			z *= this._frequency;
			float num = this._offset + this._source3D.GetValue(x, y, z);
			x *= this._lacunarity;
			y *= this._lacunarity;
			z *= this._lacunarity;
			int num2 = 1;
			while ((float)num2 < this._octaveCount)
			{
				float num3 = this._offset + this._source3D.GetValue(x, y, z);
				num3 *= this._spectralWeights[num2];
				num3 *= num;
				num += num3;
				x *= this._lacunarity;
				y *= this._lacunarity;
				z *= this._lacunarity;
				num2++;
			}
			float num4 = this._octaveCount - (float)((int)this._octaveCount);
			if (num4 > 0f)
			{
				float num3 = this._offset + this._source3D.GetValue(x, y, z);
				num3 *= this._spectralWeights[num2];
				num3 *= num;
				num3 *= num4;
				num += num3;
			}
			return num;
		}

		public float GetValue(float x, float y)
		{
			x *= this._frequency;
			y *= this._frequency;
			float num = this._offset + this._source2D.GetValue(x, y);
			x *= this._lacunarity;
			y *= this._lacunarity;
			int num2 = 1;
			while ((float)num2 < this._octaveCount)
			{
				float num3 = this._offset + this._source2D.GetValue(x, y);
				num3 *= this._spectralWeights[num2];
				num3 *= num;
				num += num3;
				x *= this._lacunarity;
				y *= this._lacunarity;
				num2++;
			}
			float num4 = this._octaveCount - (float)((int)this._octaveCount);
			if (num4 > 0f)
			{
				float num3 = this._offset + this._source2D.GetValue(x, y);
				num3 *= this._spectralWeights[num2];
				num3 *= num;
				num3 *= num4;
				num += num3;
			}
			return num;
		}
	}
}
