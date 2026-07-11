using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class ImprovedPerlin : PrimitiveModule, IModule3D, IModule, IModule2D, IModule1D
	{
		public override int Seed
		{
			get
			{
				return this._seed;
			}
			set
			{
				if (this._seed != value)
				{
					this._seed = value;
					this.Randomize(this._seed);
				}
			}
		}

		public ImprovedPerlin()
			: this(0, NoiseQuality.Standard)
		{
		}

		public ImprovedPerlin(int seed, NoiseQuality quality)
		{
			this._seed = seed;
			this._quality = quality;
			this.Randomize(this._seed);
		}

		protected void Randomize(int seed)
		{
			this._random = new int[512];
			if (seed != 0)
			{
				byte[] array = new byte[4];
				Libnoise.UnpackLittleUint32(seed, ref array);
				for (int i = 0; i < ImprovedPerlin._source.Length; i++)
				{
					this._random[i] = ImprovedPerlin._source[i] ^ (int)array[0];
					this._random[i] ^= (int)array[1];
					this._random[i] ^= (int)array[2];
					this._random[i] ^= (int)array[3];
					this._random[i + 256] = this._random[i];
				}
				return;
			}
			for (int j = 0; j < 256; j++)
			{
				this._random[j + 256] = (this._random[j] = ImprovedPerlin._source[j]);
			}
		}

		public float GetValue(float x, float y, float z)
		{
			int num = (((double)x > 0.0) ? ((int)x) : ((int)x - 1));
			int num2 = (((double)y > 0.0) ? ((int)y) : ((int)y - 1));
			int num3 = (((double)z > 0.0) ? ((int)z) : ((int)z - 1));
			int num4 = num & 255;
			int num5 = num2 & 255;
			int num6 = num3 & 255;
			x -= (float)num;
			y -= (float)num2;
			z -= (float)num3;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			switch (this._quality)
			{
			case NoiseQuality.Fast:
				num7 = x;
				num8 = y;
				num9 = z;
				break;
			case NoiseQuality.Standard:
				num7 = Libnoise.SCurve3(x);
				num8 = Libnoise.SCurve3(y);
				num9 = Libnoise.SCurve3(z);
				break;
			case NoiseQuality.Best:
				num7 = Libnoise.SCurve5(x);
				num8 = Libnoise.SCurve5(y);
				num9 = Libnoise.SCurve5(z);
				break;
			}
			int num10 = this._random[num4] + num5;
			int num11 = this._random[num10] + num6;
			int num12 = this._random[num10 + 1] + num6;
			int num13 = this._random[num4 + 1] + num5;
			int num14 = this._random[num13] + num6;
			int num15 = this._random[num13 + 1] + num6;
			return Libnoise.Lerp(Libnoise.Lerp(Libnoise.Lerp(this.Grad(this._random[num11], x, y, z), this.Grad(this._random[num14], x - 1f, y, z), num7), Libnoise.Lerp(this.Grad(this._random[num12], x, y - 1f, z), this.Grad(this._random[num15], x - 1f, y - 1f, z), num7), num8), Libnoise.Lerp(Libnoise.Lerp(this.Grad(this._random[num11 + 1], x, y, z - 1f), this.Grad(this._random[num14 + 1], x - 1f, y, z - 1f), num7), Libnoise.Lerp(this.Grad(this._random[num12 + 1], x, y - 1f, z - 1f), this.Grad(this._random[num15 + 1], x - 1f, y - 1f, z - 1f), num7), num8), num9);
		}

		protected float Grad(int hash, float x, float y, float z)
		{
			int num = hash & 15;
			float num2 = ((num < 8) ? x : y);
			float num3 = ((num < 4) ? y : ((num == 12 || num == 14) ? x : z));
			return (((num & 1) == 0) ? num2 : (-num2)) + (((num & 2) == 0) ? num3 : (-num3));
		}

		public float GetValue(float x, float y)
		{
			int num = (((double)x > 0.0) ? ((int)x) : ((int)x - 1));
			int num2 = (((double)y > 0.0) ? ((int)y) : ((int)y - 1));
			int num3 = num & 255;
			int num4 = num2 & 255;
			x -= (float)num;
			x -= (float)num2;
			float num5 = 0f;
			float num6 = 0f;
			switch (this._quality)
			{
			case NoiseQuality.Fast:
				num5 = x;
				num6 = y;
				break;
			case NoiseQuality.Standard:
				num5 = Libnoise.SCurve3(x);
				num6 = Libnoise.SCurve3(y);
				break;
			case NoiseQuality.Best:
				num5 = Libnoise.SCurve5(x);
				num6 = Libnoise.SCurve5(y);
				break;
			}
			int num7 = this._random[num3] + num4;
			int num8 = this._random[num3 + 1] + num4;
			return Libnoise.Lerp(Libnoise.Lerp(this.Grad(this._random[num7], x, y), this.Grad(this._random[num8], x - 1f, y), num5), Libnoise.Lerp(this.Grad(this._random[num7 + 1], x, y - 1f), this.Grad(this._random[num8 + 1], x - 1f, y - 1f), num5), num6);
		}

		protected float Grad(int hash, float x, float y)
		{
			int num = hash & 3;
			float num2 = (((num & 2) == 0) ? x : (-x));
			float num3 = (((num & 1) == 0) ? y : (-y));
			return num2 + num3;
		}

		public float GetValue(float x)
		{
			int num = (((double)x > 0.0) ? ((int)x) : ((int)x - 1));
			int num2 = num & 255;
			x -= (float)num;
			float num3 = 0f;
			switch (this._quality)
			{
			case NoiseQuality.Fast:
				num3 = x;
				break;
			case NoiseQuality.Standard:
				num3 = Libnoise.SCurve3(x);
				break;
			case NoiseQuality.Best:
				num3 = Libnoise.SCurve5(x);
				break;
			}
			return Libnoise.Lerp(this.Grad(this._random[num2], x), this.Grad(this._random[num2 + 1], x - 1f), num3);
		}

		protected float Grad(int hash, float x)
		{
			if ((hash & 1) != 0)
			{
				return -x;
			}
			return x;
		}

		protected const int RANDOM_SIZE = 256;

		protected static int[] _source = new int[]
		{
			151, 160, 137, 91, 90, 15, 131, 13, 201, 95,
			96, 53, 194, 233, 7, 225, 140, 36, 103, 30,
			69, 142, 8, 99, 37, 240, 21, 10, 23, 190,
			6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
			94, 252, 219, 203, 117, 35, 11, 32, 57, 177,
			33, 88, 237, 149, 56, 87, 174, 20, 125, 136,
			171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
			27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
			60, 211, 133, 230, 220, 105, 92, 41, 55, 46,
			245, 40, 244, 102, 143, 54, 65, 25, 63, 161,
			1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
			18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
			164, 100, 109, 198, 173, 186, 3, 64, 52, 217,
			226, 250, 124, 123, 5, 202, 38, 147, 118, 126,
			255, 82, 85, 212, 207, 206, 59, 227, 47, 16,
			58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
			119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
			101, 155, 167, 43, 172, 9, 129, 22, 39, 253,
			19, 98, 108, 110, 79, 113, 224, 232, 178, 185,
			112, 104, 218, 246, 97, 228, 251, 34, 242, 193,
			238, 210, 144, 12, 191, 179, 162, 241, 81, 51,
			145, 235, 249, 14, 239, 107, 49, 192, 214, 31,
			181, 199, 106, 157, 184, 84, 204, 176, 115, 121,
			50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
			222, 114, 67, 29, 24, 72, 243, 141, 128, 195,
			78, 66, 215, 61, 156, 180
		};

		protected int[] _random;
	}
}
