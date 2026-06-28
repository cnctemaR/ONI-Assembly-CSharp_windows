using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class BevinsValue : PrimitiveModule, IModule3D, IModule2D, IModule1D, IModule
	{
		public BevinsValue()
			: this(0, NoiseQuality.Standard)
		{
		}

		public BevinsValue(int seed, NoiseQuality quality)
		{
			this._seed = seed;
			this._quality = quality;
		}

		public float GetValue(float x, float y, float z)
		{
			return BevinsValue.ValueCoherentNoise3D(x, y, z, (long)this._seed, this._quality);
		}

		public static float ValueCoherentNoise3D(float x, float y, float z, long seed, NoiseQuality quality)
		{
			int num = (((double)x > 0.0) ? ((int)x) : ((int)x - 1));
			int num2 = num + 1;
			int num3 = (((double)y > 0.0) ? ((int)y) : ((int)y - 1));
			int num4 = num3 + 1;
			int num5 = (((double)z > 0.0) ? ((int)z) : ((int)z - 1));
			int num6 = num5 + 1;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			switch (quality)
			{
			case NoiseQuality.Fast:
				num7 = x - (float)num;
				num8 = y - (float)num3;
				num9 = z - (float)num5;
				break;
			case NoiseQuality.Standard:
				num7 = Libnoise.SCurve3(x - (float)num);
				num8 = Libnoise.SCurve3(y - (float)num3);
				num9 = Libnoise.SCurve3(z - (float)num5);
				break;
			case NoiseQuality.Best:
				num7 = Libnoise.SCurve5(x - (float)num);
				num8 = Libnoise.SCurve5(y - (float)num3);
				num9 = Libnoise.SCurve5(z - (float)num5);
				break;
			}
			float num10 = BevinsValue.ValueNoise3D(num, num3, num5, seed);
			float num11 = BevinsValue.ValueNoise3D(num2, num3, num5, seed);
			float num12 = Libnoise.Lerp(num10, num11, num7);
			num10 = BevinsValue.ValueNoise3D(num, num4, num5, seed);
			num11 = BevinsValue.ValueNoise3D(num2, num4, num5, seed);
			float num13 = Libnoise.Lerp(num10, num11, num7);
			float num14 = Libnoise.Lerp(num12, num13, num8);
			num10 = BevinsValue.ValueNoise3D(num, num3, num6, seed);
			num11 = BevinsValue.ValueNoise3D(num2, num3, num6, seed);
			num12 = Libnoise.Lerp(num10, num11, num7);
			num10 = BevinsValue.ValueNoise3D(num, num4, num6, seed);
			num11 = BevinsValue.ValueNoise3D(num2, num4, num6, seed);
			num13 = Libnoise.Lerp(num10, num11, num7);
			float num15 = Libnoise.Lerp(num12, num13, num8);
			return Libnoise.Lerp(num14, num15, num9);
		}

		public static float ValueNoise3D(int x, int y, int z, long seed)
		{
			return 1f - (float)BevinsValue.IntValueNoise3D(x, y, z, seed) / 1.0737418E+09f;
		}

		protected static int IntValueNoise3D(int x, int y, int z, long seed)
		{
			long num = ((long)(1619 * x + 31337 * y + 6971 * z) + 1013L * seed) & 2147483647L;
			num = (num >> 13) ^ num;
			return (int)(num * (num * num * 60493L + 19990303L) + 1376312589L) & int.MaxValue;
		}

		public float GetValue(float x, float y)
		{
			return this.ValueCoherentNoise2D(x, y, (long)this._seed, this._quality);
		}

		public float ValueCoherentNoise2D(float x, float y, long seed, NoiseQuality quality)
		{
			int num = (((double)x > 0.0) ? ((int)x) : ((int)x - 1));
			int num2 = num + 1;
			int num3 = (((double)y > 0.0) ? ((int)y) : ((int)y - 1));
			int num4 = num3 + 1;
			float num5 = 0f;
			float num6 = 0f;
			switch (quality)
			{
			case NoiseQuality.Fast:
				num5 = x - (float)num;
				num6 = y - (float)num3;
				break;
			case NoiseQuality.Standard:
				num5 = Libnoise.SCurve3(x - (float)num);
				num6 = Libnoise.SCurve3(y - (float)num3);
				break;
			case NoiseQuality.Best:
				num5 = Libnoise.SCurve5(x - (float)num);
				num6 = Libnoise.SCurve5(y - (float)num3);
				break;
			}
			float num7 = this.ValueNoise2D(num, num3, seed);
			float num8 = this.ValueNoise2D(num2, num3, seed);
			float num9 = Libnoise.Lerp(num7, num8, num5);
			num7 = this.ValueNoise2D(num, num4, seed);
			num8 = this.ValueNoise2D(num2, num4, seed);
			float num10 = Libnoise.Lerp(num7, num8, num5);
			return Libnoise.Lerp(num9, num10, num6);
		}

		public float ValueNoise2D(int x, int y, long seed)
		{
			return 1f - (float)this.IntValueNoise2D(x, y, seed) / 1.0737418E+09f;
		}

		public float ValueNoise2D(int x, int y)
		{
			return this.ValueNoise2D(x, y, (long)this._seed);
		}

		protected int IntValueNoise2D(int x, int y, long seed)
		{
			long num = ((long)(1619 * x + 31337 * y) + 1013L * seed) & 2147483647L;
			num = (num >> 13) ^ num;
			return (int)(num * (num * num * 60493L + 19990303L) + 1376312589L) & int.MaxValue;
		}

		public float GetValue(float x)
		{
			return BevinsValue.ValueCoherentNoise1D(x, (long)this._seed, this._quality);
		}

		public static float ValueCoherentNoise1D(float x, long seed, NoiseQuality quality)
		{
			int num = (((double)x > 0.0) ? ((int)x) : ((int)x - 1));
			int num2 = num + 1;
			float num3 = 0f;
			switch (quality)
			{
			case NoiseQuality.Fast:
				num3 = x - (float)num;
				break;
			case NoiseQuality.Standard:
				num3 = Libnoise.SCurve3(x - (float)num);
				break;
			case NoiseQuality.Best:
				num3 = Libnoise.SCurve5(x - (float)num);
				break;
			}
			float num4 = BevinsValue.ValueNoise1D(num, seed);
			float num5 = BevinsValue.ValueNoise1D(num2, seed);
			return Libnoise.Lerp(num4, num5, num3);
		}

		public static float ValueNoise1D(int x, long seed)
		{
			return 1f - (float)BevinsValue.IntValueNoise1D(x, seed) / 1.0737418E+09f;
		}

		public static float ValueNoise1D(int x)
		{
			return BevinsValue.ValueNoise1D(x, 0L);
		}

		protected static int IntValueNoise1D(int x, long seed)
		{
			long num = ((long)(1619 * x) + 1013L * seed) & 2147483647L;
			num = (num >> 13) ^ num;
			return (int)(num * (num * num * 60493L + 19990303L) + 1376312589L) & int.MaxValue;
		}

		public const int X_NOISE_GEN = 1619;

		public const int Y_NOISE_GEN = 31337;

		public const int Z_NOISE_GEN = 6971;

		public const int SEED_NOISE_GEN = 1013;

		public const int SHIFT_NOISE_GEN = 8;
	}
}
