using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class SimplexPerlin : ImprovedPerlin, IModule4D, IModule3D, IModule2D, IModule
	{
		public SimplexPerlin()
			: base(0, NoiseQuality.Standard)
		{
		}

		public SimplexPerlin(int seed, NoiseQuality quality)
			: base(seed, quality)
		{
		}

		public float GetValue(float x, float y, float z, float w)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = (x + y + z + w) * SimplexPerlin.F4;
			int num7 = Libnoise.FastFloor(x + num6);
			int num8 = Libnoise.FastFloor(y + num6);
			int num9 = Libnoise.FastFloor(z + num6);
			int num10 = Libnoise.FastFloor(w + num6);
			float num11 = (float)(num7 + num8 + num9 + num10) * SimplexPerlin.G4;
			float num12 = x - ((float)num7 - num11);
			float num13 = y - ((float)num8 - num11);
			float num14 = z - ((float)num9 - num11);
			float num15 = w - ((float)num10 - num11);
			int num16 = 0;
			if (num12 > num13)
			{
				num16 = 32;
			}
			if (num12 > num14)
			{
				num16 |= 16;
			}
			if (num13 > num14)
			{
				num16 |= 8;
			}
			if (num12 > num15)
			{
				num16 |= 4;
			}
			if (num13 > num15)
			{
				num16 |= 2;
			}
			if (num14 > num15)
			{
				num16 |= 1;
			}
			int[] array = SimplexPerlin._simplex[num16];
			int num17 = ((array[0] >= 3) ? 1 : 0);
			int num18 = ((array[1] >= 3) ? 1 : 0);
			int num19 = ((array[2] >= 3) ? 1 : 0);
			int num20 = ((array[3] >= 3) ? 1 : 0);
			int num21 = ((array[0] >= 2) ? 1 : 0);
			int num22 = ((array[1] >= 2) ? 1 : 0);
			int num23 = ((array[2] >= 2) ? 1 : 0);
			int num24 = ((array[3] >= 2) ? 1 : 0);
			int num25 = ((array[0] >= 1) ? 1 : 0);
			int num26 = ((array[1] >= 1) ? 1 : 0);
			int num27 = ((array[2] >= 1) ? 1 : 0);
			int num28 = ((array[3] >= 1) ? 1 : 0);
			float num29 = num12 - (float)num17 + SimplexPerlin.G4;
			float num30 = num13 - (float)num18 + SimplexPerlin.G4;
			float num31 = num14 - (float)num19 + SimplexPerlin.G4;
			float num32 = num15 - (float)num20 + SimplexPerlin.G4;
			float num33 = num12 - (float)num21 + SimplexPerlin.G42;
			float num34 = num13 - (float)num22 + SimplexPerlin.G42;
			float num35 = num14 - (float)num23 + SimplexPerlin.G42;
			float num36 = num15 - (float)num24 + SimplexPerlin.G42;
			float num37 = num12 - (float)num25 + SimplexPerlin.G43;
			float num38 = num13 - (float)num26 + SimplexPerlin.G43;
			float num39 = num14 - (float)num27 + SimplexPerlin.G43;
			float num40 = num15 - (float)num28 + SimplexPerlin.G43;
			float num41 = num12 + SimplexPerlin.G44;
			float num42 = num13 + SimplexPerlin.G44;
			float num43 = num14 + SimplexPerlin.G44;
			float num44 = num15 + SimplexPerlin.G44;
			int num45 = num7 & 255;
			int num46 = num8 & 255;
			int num47 = num9 & 255;
			int num48 = num10 & 255;
			float num49 = 0.6f - num12 * num12 - num13 * num13 - num14 * num14 - num15 * num15;
			if (num49 > 0f)
			{
				num49 *= num49;
				int num50 = this._random[num45 + this._random[num46 + this._random[num47 + this._random[num48]]]] % 32;
				num = num49 * num49 * this.Dot(SimplexPerlin._grad4[num50], num12, num13, num14, num15);
			}
			float num51 = 0.6f - num29 * num29 - num30 * num30 - num31 * num31 - num32 * num32;
			if (num51 > 0f)
			{
				num51 *= num51;
				int num52 = this._random[num45 + num17 + this._random[num46 + num18 + this._random[num47 + num19 + this._random[num48 + num20]]]] % 32;
				num2 = num51 * num51 * this.Dot(SimplexPerlin._grad4[num52], num29, num30, num31, num32);
			}
			float num53 = 0.6f - num33 * num33 - num34 * num34 - num35 * num35 - num36 * num36;
			if (num53 > 0f)
			{
				num53 *= num53;
				int num54 = this._random[num45 + num21 + this._random[num46 + num22 + this._random[num47 + num23 + this._random[num48 + num24]]]] % 32;
				num3 = num53 * num53 * this.Dot(SimplexPerlin._grad4[num54], num33, num34, num35, num36);
			}
			float num55 = 0.6f - num37 * num37 - num38 * num38 - num39 * num39 - num40 * num40;
			if (num55 > 0f)
			{
				num55 *= num55;
				int num56 = this._random[num45 + num25 + this._random[num46 + num26 + this._random[num47 + num27 + this._random[num48 + num28]]]] % 32;
				num4 = num55 * num55 * this.Dot(SimplexPerlin._grad4[num56], num37, num38, num39, num40);
			}
			float num57 = 0.6f - num41 * num41 - num42 * num42 - num43 * num43 - num44 * num44;
			if (num57 > 0f)
			{
				num57 *= num57;
				int num58 = this._random[num45 + 1 + this._random[num46 + 1 + this._random[num47 + 1 + this._random[num48 + 1]]]] % 32;
				num5 = num57 * num57 * this.Dot(SimplexPerlin._grad4[num58], num41, num42, num43, num44);
			}
			return 27f * (num + num2 + num3 + num4 + num5);
		}

		protected float Dot(int[] g, float x, float y, float z, float t)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z + (float)g[3] * t;
		}

		public new float GetValue(float x, float y, float z)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = (x + y + z) * SimplexPerlin.F3;
			int num6 = Libnoise.FastFloor(x + num5);
			int num7 = Libnoise.FastFloor(y + num5);
			int num8 = Libnoise.FastFloor(z + num5);
			float num9 = (float)(num6 + num7 + num8) * SimplexPerlin.G3;
			float num10 = x - ((float)num6 - num9);
			float num11 = y - ((float)num7 - num9);
			float num12 = z - ((float)num8 - num9);
			int num13;
			int num14;
			int num15;
			int num16;
			int num17;
			int num18;
			if (num10 >= num11)
			{
				if (num11 >= num12)
				{
					num13 = 1;
					num14 = 0;
					num15 = 0;
					num16 = 1;
					num17 = 1;
					num18 = 0;
				}
				else if (num10 >= num12)
				{
					num13 = 1;
					num14 = 0;
					num15 = 0;
					num16 = 1;
					num17 = 0;
					num18 = 1;
				}
				else
				{
					num13 = 0;
					num14 = 0;
					num15 = 1;
					num16 = 1;
					num17 = 0;
					num18 = 1;
				}
			}
			else if (num11 < num12)
			{
				num13 = 0;
				num14 = 0;
				num15 = 1;
				num16 = 0;
				num17 = 1;
				num18 = 1;
			}
			else if (num10 < num12)
			{
				num13 = 0;
				num14 = 1;
				num15 = 0;
				num16 = 0;
				num17 = 1;
				num18 = 1;
			}
			else
			{
				num13 = 0;
				num14 = 1;
				num15 = 0;
				num16 = 1;
				num17 = 1;
				num18 = 0;
			}
			float num19 = num10 - (float)num13 + SimplexPerlin.G3;
			float num20 = num11 - (float)num14 + SimplexPerlin.G3;
			float num21 = num12 - (float)num15 + SimplexPerlin.G3;
			float num22 = num10 - (float)num16 + SimplexPerlin.F3;
			float num23 = num11 - (float)num17 + SimplexPerlin.F3;
			float num24 = num12 - (float)num18 + SimplexPerlin.F3;
			float num25 = num10 - 0.5f;
			float num26 = num11 - 0.5f;
			float num27 = num12 - 0.5f;
			int num28 = num6 & 255;
			int num29 = num7 & 255;
			int num30 = num8 & 255;
			float num31 = 0.6f - num10 * num10 - num11 * num11 - num12 * num12;
			if (num31 > 0f)
			{
				num31 *= num31;
				int num32 = this._random[num28 + this._random[num29 + this._random[num30]]] % 12;
				num = num31 * num31 * this.Dot(SimplexPerlin._grad3[num32], num10, num11, num12);
			}
			float num33 = 0.6f - num19 * num19 - num20 * num20 - num21 * num21;
			if (num33 > 0f)
			{
				num33 *= num33;
				int num34 = this._random[num28 + num13 + this._random[num29 + num14 + this._random[num30 + num15]]] % 12;
				num2 = num33 * num33 * this.Dot(SimplexPerlin._grad3[num34], num19, num20, num21);
			}
			float num35 = 0.6f - num22 * num22 - num23 * num23 - num24 * num24;
			if (num35 > 0f)
			{
				num35 *= num35;
				int num36 = this._random[num28 + num16 + this._random[num29 + num17 + this._random[num30 + num18]]] % 12;
				num3 = num35 * num35 * this.Dot(SimplexPerlin._grad3[num36], num22, num23, num24);
			}
			float num37 = 0.6f - num25 * num25 - num26 * num26 - num27 * num27;
			if (num37 > 0f)
			{
				num37 *= num37;
				int num38 = this._random[num28 + 1 + this._random[num29 + 1 + this._random[num30 + 1]]] % 12;
				num4 = num37 * num37 * this.Dot(SimplexPerlin._grad3[num38], num25, num26, num27);
			}
			return 32f * (num + num2 + num3 + num4);
		}

		protected float Dot(int[] g, float x, float y, float z)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z;
		}

		public new float GetValue(float x, float y)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = (x + y) * SimplexPerlin.F2;
			int num5 = Libnoise.FastFloor(x + num4);
			int num6 = Libnoise.FastFloor(y + num4);
			float num7 = (float)(num5 + num6) * SimplexPerlin.G2;
			float num8 = x - ((float)num5 - num7);
			float num9 = y - ((float)num6 - num7);
			int num10;
			int num11;
			if (num8 > num9)
			{
				num10 = 1;
				num11 = 0;
			}
			else
			{
				num10 = 0;
				num11 = 1;
			}
			float num12 = num8 - (float)num10 + SimplexPerlin.G2;
			float num13 = num9 - (float)num11 + SimplexPerlin.G2;
			float num14 = num8 + SimplexPerlin.G22;
			float num15 = num9 + SimplexPerlin.G22;
			int num16 = num5 & 255;
			int num17 = num6 & 255;
			float num18 = 0.5f - num8 * num8 - num9 * num9;
			if (num18 > 0f)
			{
				num18 *= num18;
				int num19 = this._random[num16 + this._random[num17]] % 12;
				num = num18 * num18 * this.Dot(SimplexPerlin._grad3[num19], num8, num9);
			}
			float num20 = 0.5f - num12 * num12 - num13 * num13;
			if (num20 > 0f)
			{
				num20 *= num20;
				int num21 = this._random[num16 + num10 + this._random[num17 + num11]] % 12;
				num2 = num20 * num20 * this.Dot(SimplexPerlin._grad3[num21], num12, num13);
			}
			float num22 = 0.5f - num14 * num14 - num15 * num15;
			if (num22 > 0f)
			{
				num22 *= num22;
				int num23 = this._random[num16 + 1 + this._random[num17 + 1]] % 12;
				num3 = num22 * num22 * this.Dot(SimplexPerlin._grad3[num23], num14, num15);
			}
			return 70f * (num + num2 + num3);
		}

		protected float Dot(int[] g, float x, float y)
		{
			return (float)g[0] * x + (float)g[1] * y;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static SimplexPerlin()
		{
			int[][] array = new int[12][];
			int[][] array2 = array;
			int num = 0;
			int[] array3 = new int[3];
			array3[0] = 1;
			array3[1] = 1;
			array2[num] = array3;
			int[][] array4 = array;
			int num2 = 1;
			int[] array5 = new int[3];
			array5[0] = -1;
			array5[1] = 1;
			array4[num2] = array5;
			int[][] array6 = array;
			int num3 = 2;
			int[] array7 = new int[3];
			array7[0] = 1;
			array7[1] = -1;
			array6[num3] = array7;
			int[][] array8 = array;
			int num4 = 3;
			int[] array9 = new int[3];
			array9[0] = -1;
			array9[1] = -1;
			array8[num4] = array9;
			array[4] = new int[] { 1, 0, 1 };
			array[5] = new int[] { -1, 0, 1 };
			array[6] = new int[] { 1, 0, -1 };
			array[7] = new int[] { -1, 0, -1 };
			array[8] = new int[] { 0, 1, 1 };
			array[9] = new int[] { 0, -1, 1 };
			array[10] = new int[] { 0, 1, -1 };
			array[11] = new int[] { 0, -1, -1 };
			SimplexPerlin._grad3 = array;
			SimplexPerlin._grad4 = new int[][]
			{
				new int[] { 0, 1, 1, 1 },
				new int[] { 0, 1, 1, -1 },
				new int[] { 0, 1, -1, 1 },
				new int[] { 0, 1, -1, -1 },
				new int[] { 0, -1, 1, 1 },
				new int[] { 0, -1, 1, -1 },
				new int[] { 0, -1, -1, 1 },
				new int[] { 0, -1, -1, -1 },
				new int[] { 1, 0, 1, 1 },
				new int[] { 1, 0, 1, -1 },
				new int[] { 1, 0, -1, 1 },
				new int[] { 1, 0, -1, -1 },
				new int[] { -1, 0, 1, 1 },
				new int[] { -1, 0, 1, -1 },
				new int[] { -1, 0, -1, 1 },
				new int[] { -1, 0, -1, -1 },
				new int[] { 1, 1, 0, 1 },
				new int[] { 1, 1, 0, -1 },
				new int[] { 1, -1, 0, 1 },
				new int[] { 1, -1, 0, -1 },
				new int[] { -1, 1, 0, 1 },
				new int[] { -1, 1, 0, -1 },
				new int[] { -1, -1, 0, 1 },
				new int[] { -1, -1, 0, -1 },
				new int[] { 1, 1, 1, 0 },
				new int[] { 1, 1, -1, 0 },
				new int[] { 1, -1, 1, 0 },
				new int[] { 1, -1, -1, 0 },
				new int[] { -1, 1, 1, 0 },
				new int[] { -1, 1, -1, 0 },
				new int[] { -1, -1, 1, 0 },
				new int[] { -1, -1, -1, 0 }
			};
			int[][] array10 = new int[64][];
			array10[0] = new int[] { 0, 1, 2, 3 };
			array10[1] = new int[] { 0, 1, 3, 2 };
			int[][] array11 = array10;
			int num5 = 2;
			int[] array12 = new int[4];
			array11[num5] = array12;
			array10[3] = new int[] { 0, 2, 3, 1 };
			int[][] array13 = array10;
			int num6 = 4;
			int[] array14 = new int[4];
			array13[num6] = array14;
			int[][] array15 = array10;
			int num7 = 5;
			int[] array16 = new int[4];
			array15[num7] = array16;
			int[][] array17 = array10;
			int num8 = 6;
			int[] array18 = new int[4];
			array17[num8] = array18;
			array10[7] = new int[] { 1, 2, 3, 0 };
			array10[8] = new int[] { 0, 2, 1, 3 };
			int[][] array19 = array10;
			int num9 = 9;
			int[] array20 = new int[4];
			array19[num9] = array20;
			array10[10] = new int[] { 0, 3, 1, 2 };
			array10[11] = new int[] { 0, 3, 2, 1 };
			int[][] array21 = array10;
			int num10 = 12;
			int[] array22 = new int[4];
			array21[num10] = array22;
			int[][] array23 = array10;
			int num11 = 13;
			int[] array24 = new int[4];
			array23[num11] = array24;
			int[][] array25 = array10;
			int num12 = 14;
			int[] array26 = new int[4];
			array25[num12] = array26;
			array10[15] = new int[] { 1, 3, 2, 0 };
			int[][] array27 = array10;
			int num13 = 16;
			int[] array28 = new int[4];
			array27[num13] = array28;
			int[][] array29 = array10;
			int num14 = 17;
			int[] array30 = new int[4];
			array29[num14] = array30;
			int[][] array31 = array10;
			int num15 = 18;
			int[] array32 = new int[4];
			array31[num15] = array32;
			int[][] array33 = array10;
			int num16 = 19;
			int[] array34 = new int[4];
			array33[num16] = array34;
			int[][] array35 = array10;
			int num17 = 20;
			int[] array36 = new int[4];
			array35[num17] = array36;
			int[][] array37 = array10;
			int num18 = 21;
			int[] array38 = new int[4];
			array37[num18] = array38;
			int[][] array39 = array10;
			int num19 = 22;
			int[] array40 = new int[4];
			array39[num19] = array40;
			int[][] array41 = array10;
			int num20 = 23;
			int[] array42 = new int[4];
			array41[num20] = array42;
			array10[24] = new int[] { 1, 2, 0, 3 };
			int[][] array43 = array10;
			int num21 = 25;
			int[] array44 = new int[4];
			array43[num21] = array44;
			array10[26] = new int[] { 1, 3, 0, 2 };
			int[][] array45 = array10;
			int num22 = 27;
			int[] array46 = new int[4];
			array45[num22] = array46;
			int[][] array47 = array10;
			int num23 = 28;
			int[] array48 = new int[4];
			array47[num23] = array48;
			int[][] array49 = array10;
			int num24 = 29;
			int[] array50 = new int[4];
			array49[num24] = array50;
			array10[30] = new int[] { 2, 3, 0, 1 };
			array10[31] = new int[] { 2, 3, 1, 0 };
			array10[32] = new int[] { 1, 0, 2, 3 };
			array10[33] = new int[] { 1, 0, 3, 2 };
			int[][] array51 = array10;
			int num25 = 34;
			int[] array52 = new int[4];
			array51[num25] = array52;
			int[][] array53 = array10;
			int num26 = 35;
			int[] array54 = new int[4];
			array53[num26] = array54;
			int[][] array55 = array10;
			int num27 = 36;
			int[] array56 = new int[4];
			array55[num27] = array56;
			array10[37] = new int[] { 2, 0, 3, 1 };
			int[][] array57 = array10;
			int num28 = 38;
			int[] array58 = new int[4];
			array57[num28] = array58;
			array10[39] = new int[] { 2, 1, 3, 0 };
			int[][] array59 = array10;
			int num29 = 40;
			int[] array60 = new int[4];
			array59[num29] = array60;
			int[][] array61 = array10;
			int num30 = 41;
			int[] array62 = new int[4];
			array61[num30] = array62;
			int[][] array63 = array10;
			int num31 = 42;
			int[] array64 = new int[4];
			array63[num31] = array64;
			int[][] array65 = array10;
			int num32 = 43;
			int[] array66 = new int[4];
			array65[num32] = array66;
			int[][] array67 = array10;
			int num33 = 44;
			int[] array68 = new int[4];
			array67[num33] = array68;
			int[][] array69 = array10;
			int num34 = 45;
			int[] array70 = new int[4];
			array69[num34] = array70;
			int[][] array71 = array10;
			int num35 = 46;
			int[] array72 = new int[4];
			array71[num35] = array72;
			int[][] array73 = array10;
			int num36 = 47;
			int[] array74 = new int[4];
			array73[num36] = array74;
			array10[48] = new int[] { 2, 0, 1, 3 };
			int[][] array75 = array10;
			int num37 = 49;
			int[] array76 = new int[4];
			array75[num37] = array76;
			int[][] array77 = array10;
			int num38 = 50;
			int[] array78 = new int[4];
			array77[num38] = array78;
			int[][] array79 = array10;
			int num39 = 51;
			int[] array80 = new int[4];
			array79[num39] = array80;
			array10[52] = new int[] { 3, 0, 1, 2 };
			array10[53] = new int[] { 3, 0, 2, 1 };
			int[][] array81 = array10;
			int num40 = 54;
			int[] array82 = new int[4];
			array81[num40] = array82;
			array10[55] = new int[] { 3, 1, 2, 0 };
			array10[56] = new int[] { 2, 1, 0, 3 };
			int[][] array83 = array10;
			int num41 = 57;
			int[] array84 = new int[4];
			array83[num41] = array84;
			int[][] array85 = array10;
			int num42 = 58;
			int[] array86 = new int[4];
			array85[num42] = array86;
			int[][] array87 = array10;
			int num43 = 59;
			int[] array88 = new int[4];
			array87[num43] = array88;
			array10[60] = new int[] { 3, 1, 0, 2 };
			int[][] array89 = array10;
			int num44 = 61;
			int[] array90 = new int[4];
			array89[num44] = array90;
			array10[62] = new int[] { 3, 2, 0, 1 };
			array10[63] = new int[] { 3, 2, 1, 0 };
			SimplexPerlin._simplex = array10;
		}

		protected static float F2 = 0.3660254f;

		protected static float G2 = 0.21132487f;

		protected static float G22 = SimplexPerlin.G2 * 2f - 1f;

		protected static float F3 = 0.33333334f;

		protected static float G3 = 0.16666667f;

		protected static float F4 = 0.309017f;

		protected static float G4 = 0.1381966f;

		protected static float G42 = SimplexPerlin.G4 * 2f;

		protected static float G43 = SimplexPerlin.G4 * 3f;

		protected static float G44 = SimplexPerlin.G4 * 4f - 1f;

		protected static int[][] _grad3;

		protected static int[][] _grad4;

		protected static int[][] _simplex;
	}
}
