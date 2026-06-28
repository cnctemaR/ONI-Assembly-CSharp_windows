using System;

public class PerlinNoise
{
	public PerlinNoise(int seed)
	{
		this._random = new Random(seed);
		this.InitGradients();
	}

	public double Noise(double x, double y, double z)
	{
		int num = (int)Math.Floor(x);
		double num2 = x - (double)num;
		double num3 = num2 - 1.0;
		double num4 = this.Smooth(num2);
		int num5 = (int)Math.Floor(y);
		double num6 = y - (double)num5;
		double num7 = num6 - 1.0;
		double num8 = this.Smooth(num6);
		int num9 = (int)Math.Floor(z);
		double num10 = z - (double)num9;
		double num11 = num10 - 1.0;
		double num12 = this.Smooth(num10);
		double num13 = this.Lattice(num, num5, num9, num2, num6, num10);
		double num14 = this.Lattice(num + 1, num5, num9, num3, num6, num10);
		double num15 = this.Lerp(num4, num13, num14);
		num13 = this.Lattice(num, num5 + 1, num9, num2, num7, num10);
		num14 = this.Lattice(num + 1, num5 + 1, num9, num3, num7, num10);
		double num16 = this.Lerp(num4, num13, num14);
		double num17 = this.Lerp(num8, num15, num16);
		num13 = this.Lattice(num, num5, num9 + 1, num2, num6, num11);
		num14 = this.Lattice(num + 1, num5, num9 + 1, num3, num6, num11);
		num15 = this.Lerp(num4, num13, num14);
		num13 = this.Lattice(num, num5 + 1, num9 + 1, num2, num7, num11);
		num14 = this.Lattice(num + 1, num5 + 1, num9 + 1, num3, num7, num11);
		num16 = this.Lerp(num4, num13, num14);
		double num18 = this.Lerp(num8, num15, num16);
		return this.Lerp(num12, num17, num18);
	}

	private void InitGradients()
	{
		for (int i = 0; i < 256; i++)
		{
			double num = 1.0 - 2.0 * this._random.NextDouble();
			double num2 = Math.Sqrt(1.0 - num * num);
			double num3 = 6.283185307179586 * this._random.NextDouble();
			this._gradients[i * 3] = num2 * Math.Cos(num3);
			this._gradients[i * 3 + 1] = num2 * Math.Sin(num3);
			this._gradients[i * 3 + 2] = num;
		}
	}

	private int Permutate(int x)
	{
		return (int)this._perm[x & 255];
	}

	private int Index(int ix, int iy, int iz)
	{
		return this.Permutate(ix + this.Permutate(iy + this.Permutate(iz)));
	}

	private double Lattice(int ix, int iy, int iz, double fx, double fy, double fz)
	{
		int num = this.Index(ix, iy, iz);
		int num2 = num * 3;
		return this._gradients[num2] * fx + this._gradients[num2 + 1] * fy + this._gradients[num2 + 2] * fz;
	}

	private double Lerp(double t, double value0, double value1)
	{
		return value0 + t * (value1 - value0);
	}

	private double Smooth(double x)
	{
		return x * x * (3.0 - 2.0 * x);
	}

	private const int GradientSizeTable = 256;

	private readonly Random _random;

	private readonly double[] _gradients = new double[768];

	private readonly byte[] _perm = new byte[]
	{
		225, 155, 210, 108, 175, 199, 221, 144, 203, 116,
		70, 213, 69, 158, 33, 252, 5, 82, 173, 133,
		222, 139, 174, 27, 9, 71, 90, 246, 75, 130,
		91, 191, 169, 138, 2, 151, 194, 235, 81, 7,
		25, 113, 228, 159, 205, 253, 134, 142, 248, 65,
		224, 217, 22, 121, 229, 63, 89, 103, 96, 104,
		156, 17, 201, 129, 36, 8, 165, 110, 237, 117,
		231, 56, 132, 211, 152, 20, 181, 111, 239, 218,
		170, 163, 51, 172, 157, 47, 80, 212, 176, 250,
		87, 49, 99, 242, 136, 189, 162, 115, 44, 43,
		124, 94, 150, 16, 141, 247, 32, 10, 198, 223,
		byte.MaxValue, 72, 53, 131, 84, 57, 220, 197, 58, 50,
		208, 11, 241, 28, 3, 192, 62, 202, 18, 215,
		153, 24, 76, 41, 15, 179, 39, 46, 55, 6,
		128, 167, 23, 188, 106, 34, 187, 140, 164, 73,
		112, 182, 244, 195, 227, 13, 35, 77, 196, 185,
		26, 200, 226, 119, 31, 123, 168, 125, 249, 68,
		183, 230, 177, 135, 160, 180, 12, 1, 243, 148,
		102, 166, 38, 238, 251, 37, 240, 126, 64, 74,
		161, 40, 184, 149, 171, 178, 101, 66, 29, 59,
		146, 61, 254, 107, 42, 86, 154, 4, 236, 232,
		120, 21, 233, 209, 45, 98, 193, 114, 78, 19,
		206, 14, 118, 127, 48, 79, 147, 85, 30, 207,
		219, 54, 88, 234, 190, 122, 95, 67, 143, 109,
		137, 214, 145, 93, 92, 100, 245, 0, 216, 186,
		60, 83, 105, 97, 204, 52
	};
}
