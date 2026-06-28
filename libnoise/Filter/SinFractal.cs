using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class SinFractal : FilterModule, IModule3D, IModule2D, IModule
	{
		public float GetValue(float x, float y, float z)
		{
			float num = x;
			x *= this._frequency;
			y *= this._frequency;
			z *= this._frequency;
			float num2 = 0f;
			int num3 = 0;
			while ((float)num3 < this._octaveCount)
			{
				float num4 = this._source3D.GetValue(x, y, z) * this._spectralWeights[num3];
				if ((double)num4 < 0.0)
				{
					num4 = -num4;
				}
				num2 += num4;
				x *= this._lacunarity;
				y *= this._lacunarity;
				z *= this._lacunarity;
				num3++;
			}
			float num5 = this._octaveCount - (float)((int)this._octaveCount);
			if (num5 > 0f)
			{
				num2 += num5 * this._source3D.GetValue(x, y, z) * this._spectralWeights[num3];
			}
			return (float)Math.Sin((double)(num + num2));
		}

		public float GetValue(float x, float y)
		{
			float num = x;
			x *= this._frequency;
			y *= this._frequency;
			float num2 = 0f;
			int num3 = 0;
			while ((float)num3 < this._octaveCount)
			{
				float num4 = this._source2D.GetValue(x, y) * this._spectralWeights[num3];
				if ((double)num4 < 0.0)
				{
					num4 = -num4;
				}
				num2 += num4;
				x *= this._lacunarity;
				y *= this._lacunarity;
				num3++;
			}
			float num5 = this._octaveCount - (float)((int)this._octaveCount);
			if (num5 > 0f)
			{
				num2 += num5 * this._source2D.GetValue(x, y) * this._spectralWeights[num3];
			}
			return (float)Math.Sin((double)(num + num2));
		}
	}
}
