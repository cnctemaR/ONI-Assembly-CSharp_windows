using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class SumFractal : FilterModule, IModule3D, IModule2D, IModule
	{
		public float GetValue(float x, float y, float z)
		{
			x *= this._frequency;
			y *= this._frequency;
			z *= this._frequency;
			float num = 0f;
			int num2 = 0;
			while ((float)num2 < this._octaveCount)
			{
				float num3 = this._source3D.GetValue(x, y, z) * this._spectralWeights[num2];
				num += num3;
				x *= this._lacunarity;
				y *= this._lacunarity;
				z *= this._lacunarity;
				num2++;
			}
			float num4 = this._octaveCount - (float)((int)this._octaveCount);
			if (num4 > 0f)
			{
				num += num4 * this._source3D.GetValue(x, y, z) * this._spectralWeights[num2];
			}
			return num;
		}

		public float GetValue(float x, float y)
		{
			x *= this._frequency;
			y *= this._frequency;
			float num = 0f;
			int num2 = 0;
			while ((float)num2 < this._octaveCount)
			{
				float num3 = this._source2D.GetValue(x, y) * this._spectralWeights[num2];
				num += num3;
				x *= this._lacunarity;
				y *= this._lacunarity;
				num2++;
			}
			float num4 = this._octaveCount - (float)((int)this._octaveCount);
			if (num4 > 0f)
			{
				num += num4 * this._source2D.GetValue(x, y) * this._spectralWeights[num2];
			}
			return num;
		}
	}
}
