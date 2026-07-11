using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class Billow : FilterModule, IModule3D, IModule, IModule2D
	{
		public float Scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this._scale = value;
			}
		}

		public float Bias
		{
			get
			{
				return this._bias;
			}
			set
			{
				this._bias = value;
			}
		}

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
				if (num3 < 0f)
				{
					num3 = -num3;
				}
				num += num3 * this._scale + this._bias;
				x *= this._lacunarity;
				y *= this._lacunarity;
				z *= this._lacunarity;
				num2++;
			}
			float num4 = this._octaveCount - (float)((int)this._octaveCount);
			if (num4 > 0f)
			{
				num += this._scale * num4 * this._source3D.GetValue(x, y, z) * this._spectralWeights[num2] + this._bias;
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
				if (num3 < 0f)
				{
					num3 = -num3;
				}
				num += num3 * this._scale + this._bias;
				x *= this._lacunarity;
				y *= this._lacunarity;
				num2++;
			}
			float num4 = this._octaveCount - (float)((int)this._octaveCount);
			if (num4 > 0f)
			{
				num += this._scale * num4 * this._source2D.GetValue(x, y) * this._spectralWeights[num2] + this._bias;
			}
			return num;
		}

		public const float DEFAULT_SCALE = 1f;

		public const float DEFAULT_BIAS = 0f;

		protected float _scale = 1f;

		protected float _bias;
	}
}
