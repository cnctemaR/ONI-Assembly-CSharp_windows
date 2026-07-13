using System;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	public static class noise
	{
		public static float2 cellular(float2 P)
		{
			float2 @float = noise.mod289(math.floor(P));
			float2 float2 = math.frac(P);
			float3 float3 = math.float3(-1f, 0f, 1f);
			float3 float4 = math.float3(-0.5f, 0.5f, 1.5f);
			float3 float5 = noise.permute(@float.x + float3);
			float3 float6 = noise.permute(float5.x + @float.y + float3);
			float3 float7 = math.frac(float6 * 0.14285715f) - 0.42857143f;
			float3 float8 = noise.mod7(math.floor(float6 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float9 = float2.x + 0.5f + 1f * float7;
			float3 float10 = float2.y - float4 + 1f * float8;
			float3 float11 = float9 * float9 + float10 * float10;
			float3 float12 = noise.permute(float5.y + @float.y + float3);
			float7 = math.frac(float12 * 0.14285715f) - 0.42857143f;
			float8 = noise.mod7(math.floor(float12 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float9 = float2.x - 0.5f + 1f * float7;
			float10 = float2.y - float4 + 1f * float8;
			float3 float13 = float9 * float9 + float10 * float10;
			float3 float14 = noise.permute(float5.z + @float.y + float3);
			float7 = math.frac(float14 * 0.14285715f) - 0.42857143f;
			float8 = noise.mod7(math.floor(float14 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float9 = float2.x - 1.5f + 1f * float7;
			float10 = float2.y - float4 + 1f * float8;
			float3 float15 = float9 * float9 + float10 * float10;
			float3 float16 = math.min(float11, float13);
			float13 = math.max(float11, float13);
			float13 = math.min(float13, float15);
			float11 = math.min(float16, float13);
			float13 = math.max(float16, float13);
			float11.xy = ((float11.x < float11.y) ? float11.xy : float11.yx);
			float11.xz = ((float11.x < float11.z) ? float11.xz : float11.zx);
			float11.yz = math.min(float11.yz, float13.yz);
			float11.y = math.min(float11.y, float11.z);
			float11.y = math.min(float11.y, float13.x);
			return math.sqrt(float11.xy);
		}

		public static float2 cellular2x2(float2 P)
		{
			float2 @float = noise.mod289(math.floor(P));
			float2 float2 = math.frac(P);
			float4 float3 = float2.x + math.float4(-0.5f, -1.5f, -0.5f, -1.5f);
			float4 float4 = float2.y + math.float4(-0.5f, -0.5f, -1.5f, -1.5f);
			float4 float5 = noise.permute(noise.permute(@float.x + math.float4(0f, 1f, 0f, 1f)) + @float.y + math.float4(0f, 0f, 1f, 1f));
			float4 float6 = noise.mod7(float5) * 0.14285715f + 0.071428575f;
			float4 float7 = noise.mod7(math.floor(float5 * 0.14285715f)) * 0.14285715f + 0.071428575f;
			float4 float8 = float3 + 0.8f * float6;
			float4 float9 = float4 + 0.8f * float7;
			float4 float10 = float8 * float8 + float9 * float9;
			float10.xy = ((float10.x < float10.y) ? float10.xy : float10.yx);
			float10.xz = ((float10.x < float10.z) ? float10.xz : float10.zx);
			float10.xw = ((float10.x < float10.w) ? float10.xw : float10.wx);
			float10.y = math.min(float10.y, float10.z);
			float10.y = math.min(float10.y, float10.w);
			return math.sqrt(float10.xy);
		}

		public static float2 cellular2x2x2(float3 P)
		{
			float3 @float = noise.mod289(math.floor(P));
			float3 float2 = math.frac(P);
			float4 float3 = float2.x + math.float4(0f, -1f, 0f, -1f);
			float4 float4 = float2.y + math.float4(0f, 0f, -1f, -1f);
			float4 float5 = noise.permute(noise.permute(@float.x + math.float4(0f, 1f, 0f, 1f)) + @float.y + math.float4(0f, 0f, 1f, 1f));
			float4 float6 = noise.permute(float5 + @float.z);
			float4 float7 = noise.permute(float5 + @float.z + math.float4(1f, 1f, 1f, 1f));
			float4 float8 = math.frac(float6 * 0.14285715f) - 0.42857143f;
			float4 float9 = noise.mod7(math.floor(float6 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float4 float10 = math.floor(float6 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float4 float11 = math.frac(float7 * 0.14285715f) - 0.42857143f;
			float4 float12 = noise.mod7(math.floor(float7 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float4 float13 = math.floor(float7 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float4 float14 = float3 + 0.8f * float8;
			float4 float15 = float4 + 0.8f * float9;
			float4 float16 = float2.z + 0.8f * float10;
			float4 float17 = float3 + 0.8f * float11;
			float4 float18 = float4 + 0.8f * float12;
			float4 float19 = float2.z - 1f + 0.8f * float13;
			float4 float20 = float14 * float14 + float15 * float15 + float16 * float16;
			float4 float21 = float17 * float17 + float18 * float18 + float19 * float19;
			float4 float22 = math.min(float20, float21);
			float21 = math.max(float20, float21);
			float22.xy = ((float22.x < float22.y) ? float22.xy : float22.yx);
			float22.xz = ((float22.x < float22.z) ? float22.xz : float22.zx);
			float22.xw = ((float22.x < float22.w) ? float22.xw : float22.wx);
			float22.yzw = math.min(float22.yzw, float21.yzw);
			float22.y = math.min(float22.y, float22.z);
			float22.y = math.min(float22.y, float22.w);
			float22.y = math.min(float22.y, float21.x);
			return math.sqrt(float22.xy);
		}

		public static float2 cellular(float3 P)
		{
			float3 @float = noise.mod289(math.floor(P));
			float3 float2 = math.frac(P) - 0.5f;
			float3 float3 = float2.x + math.float3(1f, 0f, -1f);
			float3 float4 = float2.y + math.float3(1f, 0f, -1f);
			float3 float5 = float2.z + math.float3(1f, 0f, -1f);
			float3 float6 = noise.permute(@float.x + math.float3(-1f, 0f, 1f));
			float3 float7 = noise.permute(float6 + @float.y - 1f);
			float3 float8 = noise.permute(float6 + @float.y);
			float3 float9 = noise.permute(float6 + @float.y + 1f);
			float3 float10 = noise.permute(float7 + @float.z - 1f);
			float3 float11 = noise.permute(float7 + @float.z);
			float3 float12 = noise.permute(float7 + @float.z + 1f);
			float3 float13 = noise.permute(float8 + @float.z - 1f);
			float3 float14 = noise.permute(float8 + @float.z);
			float3 float15 = noise.permute(float8 + @float.z + 1f);
			float3 float16 = noise.permute(float9 + @float.z - 1f);
			float3 float17 = noise.permute(float9 + @float.z);
			float3 float18 = noise.permute(float9 + @float.z + 1f);
			float3 float19 = math.frac(float10 * 0.14285715f) - 0.42857143f;
			float3 float20 = noise.mod7(math.floor(float10 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float21 = math.floor(float10 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float3 float22 = math.frac(float11 * 0.14285715f) - 0.42857143f;
			float3 float23 = noise.mod7(math.floor(float11 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float24 = math.floor(float11 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float3 float25 = math.frac(float12 * 0.14285715f) - 0.42857143f;
			float3 float26 = noise.mod7(math.floor(float12 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float27 = math.floor(float12 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float3 float28 = math.frac(float13 * 0.14285715f) - 0.42857143f;
			float3 float29 = noise.mod7(math.floor(float13 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float30 = math.floor(float13 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float3 float31 = math.frac(float14 * 0.14285715f) - 0.42857143f;
			float3 float32 = noise.mod7(math.floor(float14 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float33 = math.floor(float14 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float3 float34 = math.frac(float15 * 0.14285715f) - 0.42857143f;
			float3 float35 = noise.mod7(math.floor(float15 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float36 = math.floor(float15 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float3 float37 = math.frac(float16 * 0.14285715f) - 0.42857143f;
			float3 float38 = noise.mod7(math.floor(float16 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float39 = math.floor(float16 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float3 float40 = math.frac(float17 * 0.14285715f) - 0.42857143f;
			float3 float41 = noise.mod7(math.floor(float17 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float42 = math.floor(float17 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float3 float43 = math.frac(float18 * 0.14285715f) - 0.42857143f;
			float3 float44 = noise.mod7(math.floor(float18 * 0.14285715f)) * 0.14285715f - 0.42857143f;
			float3 float45 = math.floor(float18 * 0.020408163f) * 0.16666667f - 0.41666666f;
			float3 float46 = float3 + 1f * float19;
			float3 float47 = float4.x + 1f * float20;
			float3 float48 = float5.x + 1f * float21;
			float3 float49 = float3 + 1f * float22;
			float3 float50 = float4.x + 1f * float23;
			float3 float51 = float5.y + 1f * float24;
			float3 float52 = float3 + 1f * float25;
			float3 float53 = float4.x + 1f * float26;
			float3 float54 = float5.z + 1f * float27;
			float3 float55 = float3 + 1f * float28;
			float3 float56 = float4.y + 1f * float29;
			float3 float57 = float5.x + 1f * float30;
			float3 float58 = float3 + 1f * float31;
			float3 float59 = float4.y + 1f * float32;
			float3 float60 = float5.y + 1f * float33;
			float3 float61 = float3 + 1f * float34;
			float3 float62 = float4.y + 1f * float35;
			float3 float63 = float5.z + 1f * float36;
			float3 float64 = float3 + 1f * float37;
			float3 float65 = float4.z + 1f * float38;
			float3 float66 = float5.x + 1f * float39;
			float3 float67 = float3 + 1f * float40;
			float3 float68 = float4.z + 1f * float41;
			float3 float69 = float5.y + 1f * float42;
			float3 float70 = float3 + 1f * float43;
			float3 float71 = float4.z + 1f * float44;
			float3 float72 = float5.z + 1f * float45;
			float3 float73 = float46 * float46 + float47 * float47 + float48 * float48;
			float3 float74 = float49 * float49 + float50 * float50 + float51 * float51;
			float3 float75 = float52 * float52 + float53 * float53 + float54 * float54;
			float3 float76 = float55 * float55 + float56 * float56 + float57 * float57;
			float3 float77 = float58 * float58 + float59 * float59 + float60 * float60;
			float3 float78 = float61 * float61 + float62 * float62 + float63 * float63;
			float3 float79 = float64 * float64 + float65 * float65 + float66 * float66;
			float3 float80 = float67 * float67 + float68 * float68 + float69 * float69;
			float3 float81 = float70 * float70 + float71 * float71 + float72 * float72;
			float3 float82 = math.min(float73, float74);
			float74 = math.max(float73, float74);
			float73 = math.min(float82, float75);
			float75 = math.max(float82, float75);
			float74 = math.min(float74, float75);
			float3 float83 = math.min(float76, float77);
			float77 = math.max(float76, float77);
			float76 = math.min(float83, float78);
			float78 = math.max(float83, float78);
			float77 = math.min(float77, float78);
			float3 float84 = math.min(float79, float80);
			float80 = math.max(float79, float80);
			float79 = math.min(float84, float81);
			float81 = math.max(float84, float81);
			float80 = math.min(float80, float81);
			float3 float85 = math.min(float73, float76);
			float76 = math.max(float73, float76);
			float73 = math.min(float85, float79);
			float79 = math.max(float85, float79);
			float73.xy = ((float73.x < float73.y) ? float73.xy : float73.yx);
			float73.xz = ((float73.x < float73.z) ? float73.xz : float73.zx);
			float74 = math.min(float74, float76);
			float74 = math.min(float74, float77);
			float74 = math.min(float74, float79);
			float74 = math.min(float74, float80);
			float73.yz = math.min(float73.yz, float74.xy);
			float73.y = math.min(float73.y, float74.z);
			float73.y = math.min(float73.y, float73.z);
			return math.sqrt(float73.xy);
		}

		public static float cnoise(float2 P)
		{
			float4 @float = math.floor(P.xyxy) + math.float4(0f, 0f, 1f, 1f);
			float4 float2 = math.frac(P.xyxy) - math.float4(0f, 0f, 1f, 1f);
			@float = noise.mod289(@float);
			float4 xzxz = @float.xzxz;
			float4 yyww = @float.yyww;
			float4 xzxz2 = float2.xzxz;
			float4 yyww2 = float2.yyww;
			float4 float3 = math.frac(noise.permute(noise.permute(xzxz) + yyww) * 0.024390243f) * 2f - 1f;
			float4 float4 = math.abs(float3) - 0.5f;
			float4 float5 = math.floor(float3 + 0.5f);
			float4 float6 = float3 - float5;
			float2 float7 = math.float2(float6.x, float4.x);
			float2 float8 = math.float2(float6.y, float4.y);
			float2 float9 = math.float2(float6.z, float4.z);
			float2 float10 = math.float2(float6.w, float4.w);
			float4 float11 = noise.taylorInvSqrt(math.float4(math.dot(float7, float7), math.dot(float9, float9), math.dot(float8, float8), math.dot(float10, float10)));
			float7 *= float11.x;
			float9 *= float11.y;
			float8 *= float11.z;
			float10 *= float11.w;
			float num = math.dot(float7, math.float2(xzxz2.x, yyww2.x));
			float num2 = math.dot(float8, math.float2(xzxz2.y, yyww2.y));
			float num3 = math.dot(float9, math.float2(xzxz2.z, yyww2.z));
			float num4 = math.dot(float10, math.float2(xzxz2.w, yyww2.w));
			float2 float12 = noise.fade(float2.xy);
			float2 float13 = math.lerp(math.float2(num, num3), math.float2(num2, num4), float12.x);
			float num5 = math.lerp(float13.x, float13.y, float12.y);
			return 2.3f * num5;
		}

		public static float pnoise(float2 P, float2 rep)
		{
			float4 @float = math.floor(P.xyxy) + math.float4(0f, 0f, 1f, 1f);
			float4 float2 = math.frac(P.xyxy) - math.float4(0f, 0f, 1f, 1f);
			@float = math.fmod(@float, rep.xyxy);
			@float = noise.mod289(@float);
			float4 xzxz = @float.xzxz;
			float4 yyww = @float.yyww;
			float4 xzxz2 = float2.xzxz;
			float4 yyww2 = float2.yyww;
			float4 float3 = math.frac(noise.permute(noise.permute(xzxz) + yyww) * 0.024390243f) * 2f - 1f;
			float4 float4 = math.abs(float3) - 0.5f;
			float4 float5 = math.floor(float3 + 0.5f);
			float4 float6 = float3 - float5;
			float2 float7 = math.float2(float6.x, float4.x);
			float2 float8 = math.float2(float6.y, float4.y);
			float2 float9 = math.float2(float6.z, float4.z);
			float2 float10 = math.float2(float6.w, float4.w);
			float4 float11 = noise.taylorInvSqrt(math.float4(math.dot(float7, float7), math.dot(float9, float9), math.dot(float8, float8), math.dot(float10, float10)));
			float7 *= float11.x;
			float9 *= float11.y;
			float8 *= float11.z;
			float10 *= float11.w;
			float num = math.dot(float7, math.float2(xzxz2.x, yyww2.x));
			float num2 = math.dot(float8, math.float2(xzxz2.y, yyww2.y));
			float num3 = math.dot(float9, math.float2(xzxz2.z, yyww2.z));
			float num4 = math.dot(float10, math.float2(xzxz2.w, yyww2.w));
			float2 float12 = noise.fade(float2.xy);
			float2 float13 = math.lerp(math.float2(num, num3), math.float2(num2, num4), float12.x);
			float num5 = math.lerp(float13.x, float13.y, float12.y);
			return 2.3f * num5;
		}

		public static float cnoise(float3 P)
		{
			float3 @float = math.floor(P);
			float3 float2 = @float + math.float3(1f);
			@float = noise.mod289(@float);
			float2 = noise.mod289(float2);
			float3 float3 = math.frac(P);
			float3 float4 = float3 - math.float3(1f);
			float4 float5 = math.float4(@float.x, float2.x, @float.x, float2.x);
			float4 float6 = math.float4(@float.yy, float2.yy);
			float4 zzzz = @float.zzzz;
			float4 zzzz2 = float2.zzzz;
			float4 float7 = noise.permute(noise.permute(float5) + float6);
			float4 float8 = noise.permute(float7 + zzzz);
			float4 float9 = noise.permute(float7 + zzzz2);
			float4 float10 = float8 * 0.14285715f;
			float4 float11 = math.frac(math.floor(float10) * 0.14285715f) - 0.5f;
			float10 = math.frac(float10);
			float4 float12 = math.float4(0.5f) - math.abs(float10) - math.abs(float11);
			float4 float13 = math.step(float12, math.float4(0f));
			float10 -= float13 * (math.step(0f, float10) - 0.5f);
			float11 -= float13 * (math.step(0f, float11) - 0.5f);
			float4 float14 = float9 * 0.14285715f;
			float4 float15 = math.frac(math.floor(float14) * 0.14285715f) - 0.5f;
			float14 = math.frac(float14);
			float4 float16 = math.float4(0.5f) - math.abs(float14) - math.abs(float15);
			float4 float17 = math.step(float16, math.float4(0f));
			float14 -= float17 * (math.step(0f, float14) - 0.5f);
			float15 -= float17 * (math.step(0f, float15) - 0.5f);
			float3 float18 = math.float3(float10.x, float11.x, float12.x);
			float3 float19 = math.float3(float10.y, float11.y, float12.y);
			float3 float20 = math.float3(float10.z, float11.z, float12.z);
			float3 float21 = math.float3(float10.w, float11.w, float12.w);
			float3 float22 = math.float3(float14.x, float15.x, float16.x);
			float3 float23 = math.float3(float14.y, float15.y, float16.y);
			float3 float24 = math.float3(float14.z, float15.z, float16.z);
			float3 float25 = math.float3(float14.w, float15.w, float16.w);
			float4 float26 = noise.taylorInvSqrt(math.float4(math.dot(float18, float18), math.dot(float20, float20), math.dot(float19, float19), math.dot(float21, float21)));
			float18 *= float26.x;
			float20 *= float26.y;
			float19 *= float26.z;
			float21 *= float26.w;
			float4 float27 = noise.taylorInvSqrt(math.float4(math.dot(float22, float22), math.dot(float24, float24), math.dot(float23, float23), math.dot(float25, float25)));
			float22 *= float27.x;
			float24 *= float27.y;
			float23 *= float27.z;
			float25 *= float27.w;
			float num = math.dot(float18, float3);
			float num2 = math.dot(float19, math.float3(float4.x, float3.yz));
			float num3 = math.dot(float20, math.float3(float3.x, float4.y, float3.z));
			float num4 = math.dot(float21, math.float3(float4.xy, float3.z));
			float num5 = math.dot(float22, math.float3(float3.xy, float4.z));
			float num6 = math.dot(float23, math.float3(float4.x, float3.y, float4.z));
			float num7 = math.dot(float24, math.float3(float3.x, float4.yz));
			float num8 = math.dot(float25, float4);
			float3 float28 = noise.fade(float3);
			float4 float29 = math.lerp(math.float4(num, num2, num3, num4), math.float4(num5, num6, num7, num8), float28.z);
			float2 float30 = math.lerp(float29.xy, float29.zw, float28.y);
			float num9 = math.lerp(float30.x, float30.y, float28.x);
			return 2.2f * num9;
		}

		public static float pnoise(float3 P, float3 rep)
		{
			float3 @float = math.fmod(math.floor(P), rep);
			float3 float2 = math.fmod(@float + math.float3(1f), rep);
			@float = noise.mod289(@float);
			float2 = noise.mod289(float2);
			float3 float3 = math.frac(P);
			float3 float4 = float3 - math.float3(1f);
			float4 float5 = math.float4(@float.x, float2.x, @float.x, float2.x);
			float4 float6 = math.float4(@float.yy, float2.yy);
			float4 zzzz = @float.zzzz;
			float4 zzzz2 = float2.zzzz;
			float4 float7 = noise.permute(noise.permute(float5) + float6);
			float4 float8 = noise.permute(float7 + zzzz);
			float4 float9 = noise.permute(float7 + zzzz2);
			float4 float10 = float8 * 0.14285715f;
			float4 float11 = math.frac(math.floor(float10) * 0.14285715f) - 0.5f;
			float10 = math.frac(float10);
			float4 float12 = math.float4(0.5f) - math.abs(float10) - math.abs(float11);
			float4 float13 = math.step(float12, math.float4(0f));
			float10 -= float13 * (math.step(0f, float10) - 0.5f);
			float11 -= float13 * (math.step(0f, float11) - 0.5f);
			float4 float14 = float9 * 0.14285715f;
			float4 float15 = math.frac(math.floor(float14) * 0.14285715f) - 0.5f;
			float14 = math.frac(float14);
			float4 float16 = math.float4(0.5f) - math.abs(float14) - math.abs(float15);
			float4 float17 = math.step(float16, math.float4(0f));
			float14 -= float17 * (math.step(0f, float14) - 0.5f);
			float15 -= float17 * (math.step(0f, float15) - 0.5f);
			float3 float18 = math.float3(float10.x, float11.x, float12.x);
			float3 float19 = math.float3(float10.y, float11.y, float12.y);
			float3 float20 = math.float3(float10.z, float11.z, float12.z);
			float3 float21 = math.float3(float10.w, float11.w, float12.w);
			float3 float22 = math.float3(float14.x, float15.x, float16.x);
			float3 float23 = math.float3(float14.y, float15.y, float16.y);
			float3 float24 = math.float3(float14.z, float15.z, float16.z);
			float3 float25 = math.float3(float14.w, float15.w, float16.w);
			float4 float26 = noise.taylorInvSqrt(math.float4(math.dot(float18, float18), math.dot(float20, float20), math.dot(float19, float19), math.dot(float21, float21)));
			float18 *= float26.x;
			float20 *= float26.y;
			float19 *= float26.z;
			float21 *= float26.w;
			float4 float27 = noise.taylorInvSqrt(math.float4(math.dot(float22, float22), math.dot(float24, float24), math.dot(float23, float23), math.dot(float25, float25)));
			float22 *= float27.x;
			float24 *= float27.y;
			float23 *= float27.z;
			float25 *= float27.w;
			float num = math.dot(float18, float3);
			float num2 = math.dot(float19, math.float3(float4.x, float3.yz));
			float num3 = math.dot(float20, math.float3(float3.x, float4.y, float3.z));
			float num4 = math.dot(float21, math.float3(float4.xy, float3.z));
			float num5 = math.dot(float22, math.float3(float3.xy, float4.z));
			float num6 = math.dot(float23, math.float3(float4.x, float3.y, float4.z));
			float num7 = math.dot(float24, math.float3(float3.x, float4.yz));
			float num8 = math.dot(float25, float4);
			float3 float28 = noise.fade(float3);
			float4 float29 = math.lerp(math.float4(num, num2, num3, num4), math.float4(num5, num6, num7, num8), float28.z);
			float2 float30 = math.lerp(float29.xy, float29.zw, float28.y);
			float num9 = math.lerp(float30.x, float30.y, float28.x);
			return 2.2f * num9;
		}

		public static float cnoise(float4 P)
		{
			float4 @float = math.floor(P);
			float4 float2 = @float + 1f;
			@float = noise.mod289(@float);
			float2 = noise.mod289(float2);
			float4 float3 = math.frac(P);
			float4 float4 = float3 - 1f;
			float4 float5 = math.float4(@float.x, float2.x, @float.x, float2.x);
			float4 float6 = math.float4(@float.yy, float2.yy);
			float4 float7 = math.float4(@float.zzzz);
			float4 float8 = math.float4(float2.zzzz);
			float4 float9 = math.float4(@float.wwww);
			float4 float10 = math.float4(float2.wwww);
			float4 float11 = noise.permute(noise.permute(float5) + float6);
			float4 float12 = noise.permute(float11 + float7);
			float4 float13 = noise.permute(float11 + float8);
			float4 float14 = noise.permute(float12 + float9);
			float4 float15 = noise.permute(float12 + float10);
			float4 float16 = noise.permute(float13 + float9);
			float4 float17 = noise.permute(float13 + float10);
			float4 float18 = float14 * 0.14285715f;
			float4 float19 = math.floor(float18) * 0.14285715f;
			float4 float20 = math.floor(float19) * 0.16666667f;
			float18 = math.frac(float18) - 0.5f;
			float19 = math.frac(float19) - 0.5f;
			float20 = math.frac(float20) - 0.5f;
			float4 float21 = math.float4(0.75f) - math.abs(float18) - math.abs(float19) - math.abs(float20);
			float4 float22 = math.step(float21, math.float4(0f));
			float18 -= float22 * (math.step(0f, float18) - 0.5f);
			float19 -= float22 * (math.step(0f, float19) - 0.5f);
			float4 float23 = float15 * 0.14285715f;
			float4 float24 = math.floor(float23) * 0.14285715f;
			float4 float25 = math.floor(float24) * 0.16666667f;
			float23 = math.frac(float23) - 0.5f;
			float24 = math.frac(float24) - 0.5f;
			float25 = math.frac(float25) - 0.5f;
			float4 float26 = math.float4(0.75f) - math.abs(float23) - math.abs(float24) - math.abs(float25);
			float4 float27 = math.step(float26, math.float4(0f));
			float23 -= float27 * (math.step(0f, float23) - 0.5f);
			float24 -= float27 * (math.step(0f, float24) - 0.5f);
			float4 float28 = float16 * 0.14285715f;
			float4 float29 = math.floor(float28) * 0.14285715f;
			float4 float30 = math.floor(float29) * 0.16666667f;
			float28 = math.frac(float28) - 0.5f;
			float29 = math.frac(float29) - 0.5f;
			float30 = math.frac(float30) - 0.5f;
			float4 float31 = math.float4(0.75f) - math.abs(float28) - math.abs(float29) - math.abs(float30);
			float4 float32 = math.step(float31, math.float4(0f));
			float28 -= float32 * (math.step(0f, float28) - 0.5f);
			float29 -= float32 * (math.step(0f, float29) - 0.5f);
			float4 float33 = float17 * 0.14285715f;
			float4 float34 = math.floor(float33) * 0.14285715f;
			float4 float35 = math.floor(float34) * 0.16666667f;
			float33 = math.frac(float33) - 0.5f;
			float34 = math.frac(float34) - 0.5f;
			float35 = math.frac(float35) - 0.5f;
			float4 float36 = math.float4(0.75f) - math.abs(float33) - math.abs(float34) - math.abs(float35);
			float4 float37 = math.step(float36, math.float4(0f));
			float33 -= float37 * (math.step(0f, float33) - 0.5f);
			float34 -= float37 * (math.step(0f, float34) - 0.5f);
			float4 float38 = math.float4(float18.x, float19.x, float20.x, float21.x);
			float4 float39 = math.float4(float18.y, float19.y, float20.y, float21.y);
			float4 float40 = math.float4(float18.z, float19.z, float20.z, float21.z);
			float4 float41 = math.float4(float18.w, float19.w, float20.w, float21.w);
			float4 float42 = math.float4(float28.x, float29.x, float30.x, float31.x);
			float4 float43 = math.float4(float28.y, float29.y, float30.y, float31.y);
			float4 float44 = math.float4(float28.z, float29.z, float30.z, float31.z);
			float4 float45 = math.float4(float28.w, float29.w, float30.w, float31.w);
			float4 float46 = math.float4(float23.x, float24.x, float25.x, float26.x);
			float4 float47 = math.float4(float23.y, float24.y, float25.y, float26.y);
			float4 float48 = math.float4(float23.z, float24.z, float25.z, float26.z);
			float4 float49 = math.float4(float23.w, float24.w, float25.w, float26.w);
			float4 float50 = math.float4(float33.x, float34.x, float35.x, float36.x);
			float4 float51 = math.float4(float33.y, float34.y, float35.y, float36.y);
			float4 float52 = math.float4(float33.z, float34.z, float35.z, float36.z);
			float4 float53 = math.float4(float33.w, float34.w, float35.w, float36.w);
			float4 float54 = noise.taylorInvSqrt(math.float4(math.dot(float38, float38), math.dot(float40, float40), math.dot(float39, float39), math.dot(float41, float41)));
			float38 *= float54.x;
			float40 *= float54.y;
			float39 *= float54.z;
			float41 *= float54.w;
			float4 float55 = noise.taylorInvSqrt(math.float4(math.dot(float46, float46), math.dot(float48, float48), math.dot(float47, float47), math.dot(float49, float49)));
			float4 float56 = float46 * float55.x;
			float48 *= float55.y;
			float47 *= float55.z;
			float49 *= float55.w;
			float4 float57 = noise.taylorInvSqrt(math.float4(math.dot(float42, float42), math.dot(float44, float44), math.dot(float43, float43), math.dot(float45, float45)));
			float42 *= float57.x;
			float44 *= float57.y;
			float43 *= float57.z;
			float45 *= float57.w;
			float4 float58 = noise.taylorInvSqrt(math.float4(math.dot(float50, float50), math.dot(float52, float52), math.dot(float51, float51), math.dot(float53, float53)));
			float50 *= float58.x;
			float52 *= float58.y;
			float51 *= float58.z;
			float53 *= float58.w;
			float num = math.dot(float38, float3);
			float num2 = math.dot(float39, math.float4(float4.x, float3.yzw));
			float num3 = math.dot(float40, math.float4(float3.x, float4.y, float3.zw));
			float num4 = math.dot(float41, math.float4(float4.xy, float3.zw));
			float num5 = math.dot(float42, math.float4(float3.xy, float4.z, float3.w));
			float num6 = math.dot(float43, math.float4(float4.x, float3.y, float4.z, float3.w));
			float num7 = math.dot(float44, math.float4(float3.x, float4.yz, float3.w));
			float num8 = math.dot(float45, math.float4(float4.xyz, float3.w));
			float num9 = math.dot(float56, math.float4(float3.xyz, float4.w));
			float num10 = math.dot(float47, math.float4(float4.x, float3.yz, float4.w));
			float num11 = math.dot(float48, math.float4(float3.x, float4.y, float3.z, float4.w));
			float num12 = math.dot(float49, math.float4(float4.xy, float3.z, float4.w));
			float num13 = math.dot(float50, math.float4(float3.xy, float4.zw));
			float num14 = math.dot(float51, math.float4(float4.x, float3.y, float4.zw));
			float num15 = math.dot(float52, math.float4(float3.x, float4.yzw));
			float num16 = math.dot(float53, float4);
			float4 float59 = noise.fade(float3);
			float4 float60 = math.lerp(math.float4(num, num2, num3, num4), math.float4(num9, num10, num11, num12), float59.w);
			float4 float61 = math.lerp(math.float4(num5, num6, num7, num8), math.float4(num13, num14, num15, num16), float59.w);
			float4 float62 = math.lerp(float60, float61, float59.z);
			float2 float63 = math.lerp(float62.xy, float62.zw, float59.y);
			float num17 = math.lerp(float63.x, float63.y, float59.x);
			return 2.2f * num17;
		}

		public static float pnoise(float4 P, float4 rep)
		{
			float4 @float = math.fmod(math.floor(P), rep);
			float4 float2 = math.fmod(@float + 1f, rep);
			@float = noise.mod289(@float);
			float2 = noise.mod289(float2);
			float4 float3 = math.frac(P);
			float4 float4 = float3 - 1f;
			float4 float5 = math.float4(@float.x, float2.x, @float.x, float2.x);
			float4 float6 = math.float4(@float.yy, float2.yy);
			float4 float7 = math.float4(@float.zzzz);
			float4 float8 = math.float4(float2.zzzz);
			float4 float9 = math.float4(@float.wwww);
			float4 float10 = math.float4(float2.wwww);
			float4 float11 = noise.permute(noise.permute(float5) + float6);
			float4 float12 = noise.permute(float11 + float7);
			float4 float13 = noise.permute(float11 + float8);
			float4 float14 = noise.permute(float12 + float9);
			float4 float15 = noise.permute(float12 + float10);
			float4 float16 = noise.permute(float13 + float9);
			float4 float17 = noise.permute(float13 + float10);
			float4 float18 = float14 * 0.14285715f;
			float4 float19 = math.floor(float18) * 0.14285715f;
			float4 float20 = math.floor(float19) * 0.16666667f;
			float18 = math.frac(float18) - 0.5f;
			float19 = math.frac(float19) - 0.5f;
			float20 = math.frac(float20) - 0.5f;
			float4 float21 = math.float4(0.75f) - math.abs(float18) - math.abs(float19) - math.abs(float20);
			float4 float22 = math.step(float21, math.float4(0f));
			float18 -= float22 * (math.step(0f, float18) - 0.5f);
			float19 -= float22 * (math.step(0f, float19) - 0.5f);
			float4 float23 = float15 * 0.14285715f;
			float4 float24 = math.floor(float23) * 0.14285715f;
			float4 float25 = math.floor(float24) * 0.16666667f;
			float23 = math.frac(float23) - 0.5f;
			float24 = math.frac(float24) - 0.5f;
			float25 = math.frac(float25) - 0.5f;
			float4 float26 = math.float4(0.75f) - math.abs(float23) - math.abs(float24) - math.abs(float25);
			float4 float27 = math.step(float26, math.float4(0f));
			float23 -= float27 * (math.step(0f, float23) - 0.5f);
			float24 -= float27 * (math.step(0f, float24) - 0.5f);
			float4 float28 = float16 * 0.14285715f;
			float4 float29 = math.floor(float28) * 0.14285715f;
			float4 float30 = math.floor(float29) * 0.16666667f;
			float28 = math.frac(float28) - 0.5f;
			float29 = math.frac(float29) - 0.5f;
			float30 = math.frac(float30) - 0.5f;
			float4 float31 = math.float4(0.75f) - math.abs(float28) - math.abs(float29) - math.abs(float30);
			float4 float32 = math.step(float31, math.float4(0f));
			float28 -= float32 * (math.step(0f, float28) - 0.5f);
			float29 -= float32 * (math.step(0f, float29) - 0.5f);
			float4 float33 = float17 * 0.14285715f;
			float4 float34 = math.floor(float33) * 0.14285715f;
			float4 float35 = math.floor(float34) * 0.16666667f;
			float33 = math.frac(float33) - 0.5f;
			float34 = math.frac(float34) - 0.5f;
			float35 = math.frac(float35) - 0.5f;
			float4 float36 = math.float4(0.75f) - math.abs(float33) - math.abs(float34) - math.abs(float35);
			float4 float37 = math.step(float36, math.float4(0f));
			float33 -= float37 * (math.step(0f, float33) - 0.5f);
			float34 -= float37 * (math.step(0f, float34) - 0.5f);
			float4 float38 = math.float4(float18.x, float19.x, float20.x, float21.x);
			float4 float39 = math.float4(float18.y, float19.y, float20.y, float21.y);
			float4 float40 = math.float4(float18.z, float19.z, float20.z, float21.z);
			float4 float41 = math.float4(float18.w, float19.w, float20.w, float21.w);
			float4 float42 = math.float4(float28.x, float29.x, float30.x, float31.x);
			float4 float43 = math.float4(float28.y, float29.y, float30.y, float31.y);
			float4 float44 = math.float4(float28.z, float29.z, float30.z, float31.z);
			float4 float45 = math.float4(float28.w, float29.w, float30.w, float31.w);
			float4 float46 = math.float4(float23.x, float24.x, float25.x, float26.x);
			float4 float47 = math.float4(float23.y, float24.y, float25.y, float26.y);
			float4 float48 = math.float4(float23.z, float24.z, float25.z, float26.z);
			float4 float49 = math.float4(float23.w, float24.w, float25.w, float26.w);
			float4 float50 = math.float4(float33.x, float34.x, float35.x, float36.x);
			float4 float51 = math.float4(float33.y, float34.y, float35.y, float36.y);
			float4 float52 = math.float4(float33.z, float34.z, float35.z, float36.z);
			float4 float53 = math.float4(float33.w, float34.w, float35.w, float36.w);
			float4 float54 = noise.taylorInvSqrt(math.float4(math.dot(float38, float38), math.dot(float40, float40), math.dot(float39, float39), math.dot(float41, float41)));
			float38 *= float54.x;
			float40 *= float54.y;
			float39 *= float54.z;
			float41 *= float54.w;
			float4 float55 = noise.taylorInvSqrt(math.float4(math.dot(float46, float46), math.dot(float48, float48), math.dot(float47, float47), math.dot(float49, float49)));
			float4 float56 = float46 * float55.x;
			float48 *= float55.y;
			float47 *= float55.z;
			float49 *= float55.w;
			float4 float57 = noise.taylorInvSqrt(math.float4(math.dot(float42, float42), math.dot(float44, float44), math.dot(float43, float43), math.dot(float45, float45)));
			float42 *= float57.x;
			float44 *= float57.y;
			float43 *= float57.z;
			float45 *= float57.w;
			float4 float58 = noise.taylorInvSqrt(math.float4(math.dot(float50, float50), math.dot(float52, float52), math.dot(float51, float51), math.dot(float53, float53)));
			float50 *= float58.x;
			float52 *= float58.y;
			float51 *= float58.z;
			float53 *= float58.w;
			float num = math.dot(float38, float3);
			float num2 = math.dot(float39, math.float4(float4.x, float3.yzw));
			float num3 = math.dot(float40, math.float4(float3.x, float4.y, float3.zw));
			float num4 = math.dot(float41, math.float4(float4.xy, float3.zw));
			float num5 = math.dot(float42, math.float4(float3.xy, float4.z, float3.w));
			float num6 = math.dot(float43, math.float4(float4.x, float3.y, float4.z, float3.w));
			float num7 = math.dot(float44, math.float4(float3.x, float4.yz, float3.w));
			float num8 = math.dot(float45, math.float4(float4.xyz, float3.w));
			float num9 = math.dot(float56, math.float4(float3.xyz, float4.w));
			float num10 = math.dot(float47, math.float4(float4.x, float3.yz, float4.w));
			float num11 = math.dot(float48, math.float4(float3.x, float4.y, float3.z, float4.w));
			float num12 = math.dot(float49, math.float4(float4.xy, float3.z, float4.w));
			float num13 = math.dot(float50, math.float4(float3.xy, float4.zw));
			float num14 = math.dot(float51, math.float4(float4.x, float3.y, float4.zw));
			float num15 = math.dot(float52, math.float4(float3.x, float4.yzw));
			float num16 = math.dot(float53, float4);
			float4 float59 = noise.fade(float3);
			float4 float60 = math.lerp(math.float4(num, num2, num3, num4), math.float4(num9, num10, num11, num12), float59.w);
			float4 float61 = math.lerp(math.float4(num5, num6, num7, num8), math.float4(num13, num14, num15, num16), float59.w);
			float4 float62 = math.lerp(float60, float61, float59.z);
			float2 float63 = math.lerp(float62.xy, float62.zw, float59.y);
			float num17 = math.lerp(float63.x, float63.y, float59.x);
			return 2.2f * num17;
		}

		private static float mod289(float x)
		{
			return x - math.floor(x * 0.0034602077f) * 289f;
		}

		private static float2 mod289(float2 x)
		{
			return x - math.floor(x * 0.0034602077f) * 289f;
		}

		private static float3 mod289(float3 x)
		{
			return x - math.floor(x * 0.0034602077f) * 289f;
		}

		private static float4 mod289(float4 x)
		{
			return x - math.floor(x * 0.0034602077f) * 289f;
		}

		private static float3 mod7(float3 x)
		{
			return x - math.floor(x * 0.14285715f) * 7f;
		}

		private static float4 mod7(float4 x)
		{
			return x - math.floor(x * 0.14285715f) * 7f;
		}

		private static float permute(float x)
		{
			return noise.mod289((34f * x + 1f) * x);
		}

		private static float3 permute(float3 x)
		{
			return noise.mod289((34f * x + 1f) * x);
		}

		private static float4 permute(float4 x)
		{
			return noise.mod289((34f * x + 1f) * x);
		}

		private static float taylorInvSqrt(float r)
		{
			return 1.7928429f - 0.85373473f * r;
		}

		private static float4 taylorInvSqrt(float4 r)
		{
			return 1.7928429f - 0.85373473f * r;
		}

		private static float2 fade(float2 t)
		{
			return t * t * t * (t * (t * 6f - 15f) + 10f);
		}

		private static float3 fade(float3 t)
		{
			return t * t * t * (t * (t * 6f - 15f) + 10f);
		}

		private static float4 fade(float4 t)
		{
			return t * t * t * (t * (t * 6f - 15f) + 10f);
		}

		private static float4 grad4(float j, float4 ip)
		{
			float4 @float = math.float4(1f, 1f, 1f, -1f);
			float3 float2 = math.floor(math.frac(math.float3(j) * ip.xyz) * 7f) * ip.z - 1f;
			float num = 1.5f - math.dot(math.abs(float2), @float.xyz);
			float4 float3 = math.float4(float2, num);
			float4 float4 = math.float4(float3 < 0f);
			float3.xyz += (float4.xyz * 2f - 1f) * float4.www;
			return float3;
		}

		private static float2 rgrad2(float2 p, float rot)
		{
			float num = noise.permute(noise.permute(p.x) + p.y) * 0.024390243f + rot;
			num = math.frac(num) * 6.2831855f;
			return math.float2(math.cos(num), math.sin(num));
		}

		public static float snoise(float2 v)
		{
			float4 @float = math.float4(0.21132487f, 0.36602542f, -0.57735026f, 0.024390243f);
			float2 float2 = math.floor(v + math.dot(v, @float.yy));
			float2 float3 = v - float2 + math.dot(float2, @float.xx);
			float2 float4 = ((float3.x > float3.y) ? math.float2(1f, 0f) : math.float2(0f, 1f));
			float4 float5 = float3.xyxy + @float.xxzz;
			float5.xy -= float4;
			float2 = noise.mod289(float2);
			float3 float6 = noise.permute(noise.permute(float2.y + math.float3(0f, float4.y, 1f)) + float2.x + math.float3(0f, float4.x, 1f));
			float3 float7 = math.max(0.5f - math.float3(math.dot(float3, float3), math.dot(float5.xy, float5.xy), math.dot(float5.zw, float5.zw)), 0f);
			float7 *= float7;
			float7 *= float7;
			float3 float8 = 2f * math.frac(float6 * @float.www) - 1f;
			float3 float9 = math.abs(float8) - 0.5f;
			float3 float10 = math.floor(float8 + 0.5f);
			float3 float11 = float8 - float10;
			float7 *= 1.7928429f - 0.85373473f * (float11 * float11 + float9 * float9);
			float num = float11.x * float3.x + float9.x * float3.y;
			float2 float12 = float11.yz * float5.xz + float9.yz * float5.yw;
			float3 float13 = math.float3(num, float12);
			return 130f * math.dot(float7, float13);
		}

		public static float snoise(float3 v)
		{
			float2 @float = math.float2(0.16666667f, 0.33333334f);
			float4 float2 = math.float4(0f, 0.5f, 1f, 2f);
			float3 float3 = math.floor(v + math.dot(v, @float.yyy));
			float3 float4 = v - float3 + math.dot(float3, @float.xxx);
			float3 float5 = math.step(float4.yzx, float4.xyz);
			float3 float6 = 1f - float5;
			float3 float7 = math.min(float5.xyz, float6.zxy);
			float3 float8 = math.max(float5.xyz, float6.zxy);
			float3 float9 = float4 - float7 + @float.xxx;
			float3 float10 = float4 - float8 + @float.yyy;
			float3 float11 = float4 - float2.yyy;
			float3 = noise.mod289(float3);
			float4 float12 = noise.permute(noise.permute(noise.permute(float3.z + math.float4(0f, float7.z, float8.z, 1f)) + float3.y + math.float4(0f, float7.y, float8.y, 1f)) + float3.x + math.float4(0f, float7.x, float8.x, 1f));
			float3 float13 = 0.14285715f * float2.wyz - float2.xzx;
			float4 float14 = float12 - 49f * math.floor(float12 * float13.z * float13.z);
			float4 float15 = math.floor(float14 * float13.z);
			float4 float16 = math.floor(float14 - 7f * float15);
			float4 float17 = float15 * float13.x + float13.yyyy;
			float4 float18 = float16 * float13.x + float13.yyyy;
			float4 float19 = 1f - math.abs(float17) - math.abs(float18);
			float4 float20 = math.float4(float17.xy, float18.xy);
			float4 float21 = math.float4(float17.zw, float18.zw);
			float4 float22 = math.floor(float20) * 2f + 1f;
			float4 float23 = math.floor(float21) * 2f + 1f;
			float4 float24 = -math.step(float19, math.float4(0f));
			float4 float25 = float20.xzyw + float22.xzyw * float24.xxyy;
			float4 float26 = float21.xzyw + float23.xzyw * float24.zzww;
			float3 float27 = math.float3(float25.xy, float19.x);
			float3 float28 = math.float3(float25.zw, float19.y);
			float3 float29 = math.float3(float26.xy, float19.z);
			float3 float30 = math.float3(float26.zw, float19.w);
			float4 float31 = noise.taylorInvSqrt(math.float4(math.dot(float27, float27), math.dot(float28, float28), math.dot(float29, float29), math.dot(float30, float30)));
			float27 *= float31.x;
			float28 *= float31.y;
			float29 *= float31.z;
			float30 *= float31.w;
			float4 float32 = math.max(0.6f - math.float4(math.dot(float4, float4), math.dot(float9, float9), math.dot(float10, float10), math.dot(float11, float11)), 0f);
			float32 *= float32;
			return 42f * math.dot(float32 * float32, math.float4(math.dot(float27, float4), math.dot(float28, float9), math.dot(float29, float10), math.dot(float30, float11)));
		}

		public static float snoise(float3 v, out float3 gradient)
		{
			float2 @float = math.float2(0.16666667f, 0.33333334f);
			float4 float2 = math.float4(0f, 0.5f, 1f, 2f);
			float3 float3 = math.floor(v + math.dot(v, @float.yyy));
			float3 float4 = v - float3 + math.dot(float3, @float.xxx);
			float3 float5 = math.step(float4.yzx, float4.xyz);
			float3 float6 = 1f - float5;
			float3 float7 = math.min(float5.xyz, float6.zxy);
			float3 float8 = math.max(float5.xyz, float6.zxy);
			float3 float9 = float4 - float7 + @float.xxx;
			float3 float10 = float4 - float8 + @float.yyy;
			float3 float11 = float4 - float2.yyy;
			float3 = noise.mod289(float3);
			float4 float12 = noise.permute(noise.permute(noise.permute(float3.z + math.float4(0f, float7.z, float8.z, 1f)) + float3.y + math.float4(0f, float7.y, float8.y, 1f)) + float3.x + math.float4(0f, float7.x, float8.x, 1f));
			float3 float13 = 0.14285715f * float2.wyz - float2.xzx;
			float4 float14 = float12 - 49f * math.floor(float12 * float13.z * float13.z);
			float4 float15 = math.floor(float14 * float13.z);
			float4 float16 = math.floor(float14 - 7f * float15);
			float4 float17 = float15 * float13.x + float13.yyyy;
			float4 float18 = float16 * float13.x + float13.yyyy;
			float4 float19 = 1f - math.abs(float17) - math.abs(float18);
			float4 float20 = math.float4(float17.xy, float18.xy);
			float4 float21 = math.float4(float17.zw, float18.zw);
			float4 float22 = math.floor(float20) * 2f + 1f;
			float4 float23 = math.floor(float21) * 2f + 1f;
			float4 float24 = -math.step(float19, math.float4(0f));
			float4 float25 = float20.xzyw + float22.xzyw * float24.xxyy;
			float4 float26 = float21.xzyw + float23.xzyw * float24.zzww;
			float3 float27 = math.float3(float25.xy, float19.x);
			float3 float28 = math.float3(float25.zw, float19.y);
			float3 float29 = math.float3(float26.xy, float19.z);
			float3 float30 = math.float3(float26.zw, float19.w);
			float4 float31 = noise.taylorInvSqrt(math.float4(math.dot(float27, float27), math.dot(float28, float28), math.dot(float29, float29), math.dot(float30, float30)));
			float27 *= float31.x;
			float28 *= float31.y;
			float29 *= float31.z;
			float30 *= float31.w;
			float4 float32 = math.max(0.6f - math.float4(math.dot(float4, float4), math.dot(float9, float9), math.dot(float10, float10), math.dot(float11, float11)), 0f);
			float4 float33 = float32 * float32;
			float4 float34 = float33 * float33;
			float4 float35 = math.float4(math.dot(float27, float4), math.dot(float28, float9), math.dot(float29, float10), math.dot(float30, float11));
			float4 float36 = float33 * float32 * float35;
			gradient = -8f * (float36.x * float4 + float36.y * float9 + float36.z * float10 + float36.w * float11);
			gradient += float34.x * float27 + float34.y * float28 + float34.z * float29 + float34.w * float30;
			gradient *= 42f;
			return 42f * math.dot(float34, float35);
		}

		public static float snoise(float4 v)
		{
			float4 @float = math.float4(0.1381966f, 0.2763932f, 0.4145898f, -0.4472136f);
			float4 float2 = math.floor(v + math.dot(v, math.float4(0.309017f)));
			float4 float3 = v - float2 + math.dot(float2, @float.xxxx);
			float4 float4 = math.float4(0f);
			float3 float5 = math.step(float3.yzw, float3.xxx);
			float3 float6 = math.step(float3.zww, float3.yyz);
			float4.x = float5.x + float5.y + float5.z;
			float4.yzw = 1f - float5;
			float4.y += float6.x + float6.y;
			float4.zw += 1f - float6.xy;
			float4.z += float6.z;
			float4.w += 1f - float6.z;
			float4 float7 = math.clamp(float4, 0f, 1f);
			float4 float8 = math.clamp(float4 - 1f, 0f, 1f);
			float4 float9 = math.clamp(float4 - 2f, 0f, 1f);
			float4 float10 = float3 - float9 + @float.xxxx;
			float4 float11 = float3 - float8 + @float.yyyy;
			float4 float12 = float3 - float7 + @float.zzzz;
			float4 float13 = float3 + @float.wwww;
			float2 = noise.mod289(float2);
			float num = noise.permute(noise.permute(noise.permute(noise.permute(float2.w) + float2.z) + float2.y) + float2.x);
			float4 float14 = noise.permute(noise.permute(noise.permute(noise.permute(float2.w + math.float4(float9.w, float8.w, float7.w, 1f)) + float2.z + math.float4(float9.z, float8.z, float7.z, 1f)) + float2.y + math.float4(float9.y, float8.y, float7.y, 1f)) + float2.x + math.float4(float9.x, float8.x, float7.x, 1f));
			float4 float15 = math.float4(0.0034013605f, 0.020408163f, 0.14285715f, 0f);
			float4 float16 = noise.grad4(num, float15);
			float4 float17 = noise.grad4(float14.x, float15);
			float4 float18 = noise.grad4(float14.y, float15);
			float4 float19 = noise.grad4(float14.z, float15);
			float4 float20 = noise.grad4(float14.w, float15);
			float4 float21 = noise.taylorInvSqrt(math.float4(math.dot(float16, float16), math.dot(float17, float17), math.dot(float18, float18), math.dot(float19, float19)));
			float16 *= float21.x;
			float17 *= float21.y;
			float18 *= float21.z;
			float19 *= float21.w;
			float20 *= noise.taylorInvSqrt(math.dot(float20, float20));
			float3 float22 = math.max(0.6f - math.float3(math.dot(float3, float3), math.dot(float10, float10), math.dot(float11, float11)), 0f);
			float2 float23 = math.max(0.6f - math.float2(math.dot(float12, float12), math.dot(float13, float13)), 0f);
			float22 *= float22;
			float23 *= float23;
			return 49f * (math.dot(float22 * float22, math.float3(math.dot(float16, float3), math.dot(float17, float10), math.dot(float18, float11))) + math.dot(float23 * float23, math.float2(math.dot(float19, float12), math.dot(float20, float13))));
		}

		public static float3 psrdnoise(float2 pos, float2 per, float rot)
		{
			pos.y += 0.01f;
			float2 @float = math.float2(pos.x + pos.y * 0.5f, pos.y);
			float2 float2 = math.floor(@float);
			float2 float3 = math.frac(@float);
			float2 float4 = ((float3.x > float3.y) ? math.float2(1f, 0f) : math.float2(0f, 1f));
			float2 float5 = math.float2(float2.x - float2.y * 0.5f, float2.y);
			float2 float6 = math.float2(float5.x + float4.x - float4.y * 0.5f, float5.y + float4.y);
			float2 float7 = math.float2(float5.x + 0.5f, float5.y + 1f);
			float2 float8 = pos - float5;
			float2 float9 = pos - float6;
			float2 float10 = pos - float7;
			float3 float11 = math.fmod(math.float3(float5.x, float6.x, float7.x), per.x);
			float3 float12 = math.fmod(math.float3(float5.y, float6.y, float7.y), per.y);
			float3 float13 = float11 + 0.5f * float12;
			float3 float14 = float12;
			float2 float15 = noise.rgrad2(math.float2(float13.x, float14.x), rot);
			float2 float16 = noise.rgrad2(math.float2(float13.y, float14.y), rot);
			float2 float17 = noise.rgrad2(math.float2(float13.z, float14.z), rot);
			float3 float18 = math.float3(math.dot(float15, float8), math.dot(float16, float9), math.dot(float17, float10));
			float3 float19 = 0.8f - math.float3(math.dot(float8, float8), math.dot(float9, float9), math.dot(float10, float10));
			float3 float20 = -2f * math.float3(float8.x, float9.x, float10.x);
			float3 float21 = -2f * math.float3(float8.y, float9.y, float10.y);
			if (float19.x < 0f)
			{
				float20.x = 0f;
				float21.x = 0f;
				float19.x = 0f;
			}
			if (float19.y < 0f)
			{
				float20.y = 0f;
				float21.y = 0f;
				float19.y = 0f;
			}
			if (float19.z < 0f)
			{
				float20.z = 0f;
				float21.z = 0f;
				float19.z = 0f;
			}
			float3 float22 = float19 * float19;
			float3 float23 = float22 * float22;
			float3 float24 = float22 * float19;
			float num = math.dot(float23, float18);
			float2 float25 = math.float2(float20.x, float21.x) * 4f * float24.x;
			float2 float26 = float23.x * float15 + float25 * float18.x;
			float2 float27 = math.float2(float20.y, float21.y) * 4f * float24.y;
			float2 float28 = float23.y * float16 + float27 * float18.y;
			float2 float29 = math.float2(float20.z, float21.z) * 4f * float24.z;
			float2 float30 = float23.z * float17 + float29 * float18.z;
			return 11f * math.float3(num, float26 + float28 + float30);
		}

		public static float3 psrdnoise(float2 pos, float2 per)
		{
			return noise.psrdnoise(pos, per, 0f);
		}

		public static float psrnoise(float2 pos, float2 per, float rot)
		{
			pos.y += 0.001f;
			float2 @float = math.float2(pos.x + pos.y * 0.5f, pos.y);
			float2 float2 = math.floor(@float);
			float2 float3 = math.frac(@float);
			float2 float4 = ((float3.x > float3.y) ? math.float2(1f, 0f) : math.float2(0f, 1f));
			float2 float5 = math.float2(float2.x - float2.y * 0.5f, float2.y);
			float2 float6 = math.float2(float5.x + float4.x - float4.y * 0.5f, float5.y + float4.y);
			float2 float7 = math.float2(float5.x + 0.5f, float5.y + 1f);
			float2 float8 = pos - float5;
			float2 float9 = pos - float6;
			float2 float10 = pos - float7;
			float3 float11 = math.fmod(math.float3(float5.x, float6.x, float7.x), per.x);
			float3 float12 = math.fmod(math.float3(float5.y, float6.y, float7.y), per.y);
			float3 float13 = float11 + 0.5f * float12;
			float3 float14 = float12;
			float2 float15 = noise.rgrad2(math.float2(float13.x, float14.x), rot);
			float2 float16 = noise.rgrad2(math.float2(float13.y, float14.y), rot);
			float2 float17 = noise.rgrad2(math.float2(float13.z, float14.z), rot);
			float3 float18 = math.float3(math.dot(float15, float8), math.dot(float16, float9), math.dot(float17, float10));
			float3 float19 = math.max(0.8f - math.float3(math.dot(float8, float8), math.dot(float9, float9), math.dot(float10, float10)), 0f);
			float3 float20 = float19 * float19;
			float num = math.dot(float20 * float20, float18);
			return 11f * num;
		}

		public static float psrnoise(float2 pos, float2 per)
		{
			return noise.psrnoise(pos, per, 0f);
		}

		public static float3 srdnoise(float2 pos, float rot)
		{
			pos.y += 0.001f;
			float2 @float = math.float2(pos.x + pos.y * 0.5f, pos.y);
			float2 float2 = math.floor(@float);
			float2 float3 = math.frac(@float);
			float2 float4 = ((float3.x > float3.y) ? math.float2(1f, 0f) : math.float2(0f, 1f));
			float2 float5 = math.float2(float2.x - float2.y * 0.5f, float2.y);
			float2 float6 = math.float2(float5.x + float4.x - float4.y * 0.5f, float5.y + float4.y);
			float2 float7 = math.float2(float5.x + 0.5f, float5.y + 1f);
			float2 float8 = pos - float5;
			float2 float9 = pos - float6;
			float2 float10 = pos - float7;
			float3 float11 = math.float3(float5.x, float6.x, float7.x);
			float3 float12 = math.float3(float5.y, float6.y, float7.y);
			float3 float13 = float11 + 0.5f * float12;
			float3 float14 = float12;
			float3 float15 = noise.mod289(float13);
			float14 = noise.mod289(float14);
			float2 float16 = noise.rgrad2(math.float2(float15.x, float14.x), rot);
			float2 float17 = noise.rgrad2(math.float2(float15.y, float14.y), rot);
			float2 float18 = noise.rgrad2(math.float2(float15.z, float14.z), rot);
			float3 float19 = math.float3(math.dot(float16, float8), math.dot(float17, float9), math.dot(float18, float10));
			float3 float20 = 0.8f - math.float3(math.dot(float8, float8), math.dot(float9, float9), math.dot(float10, float10));
			float3 float21 = -2f * math.float3(float8.x, float9.x, float10.x);
			float3 float22 = -2f * math.float3(float8.y, float9.y, float10.y);
			if (float20.x < 0f)
			{
				float21.x = 0f;
				float22.x = 0f;
				float20.x = 0f;
			}
			if (float20.y < 0f)
			{
				float21.y = 0f;
				float22.y = 0f;
				float20.y = 0f;
			}
			if (float20.z < 0f)
			{
				float21.z = 0f;
				float22.z = 0f;
				float20.z = 0f;
			}
			float3 float23 = float20 * float20;
			float3 float24 = float23 * float23;
			float3 float25 = float23 * float20;
			float num = math.dot(float24, float19);
			float2 float26 = math.float2(float21.x, float22.x) * 4f * float25.x;
			float2 float27 = float24.x * float16 + float26 * float19.x;
			float2 float28 = math.float2(float21.y, float22.y) * 4f * float25.y;
			float2 float29 = float24.y * float17 + float28 * float19.y;
			float2 float30 = math.float2(float21.z, float22.z) * 4f * float25.z;
			float2 float31 = float24.z * float18 + float30 * float19.z;
			return 11f * math.float3(num, float27 + float29 + float31);
		}

		public static float3 srdnoise(float2 pos)
		{
			return noise.srdnoise(pos, 0f);
		}

		public static float srnoise(float2 pos, float rot)
		{
			pos.y += 0.001f;
			float2 @float = math.float2(pos.x + pos.y * 0.5f, pos.y);
			float2 float2 = math.floor(@float);
			float2 float3 = math.frac(@float);
			float2 float4 = ((float3.x > float3.y) ? math.float2(1f, 0f) : math.float2(0f, 1f));
			float2 float5 = math.float2(float2.x - float2.y * 0.5f, float2.y);
			float2 float6 = math.float2(float5.x + float4.x - float4.y * 0.5f, float5.y + float4.y);
			float2 float7 = math.float2(float5.x + 0.5f, float5.y + 1f);
			float2 float8 = pos - float5;
			float2 float9 = pos - float6;
			float2 float10 = pos - float7;
			float3 float11 = math.float3(float5.x, float6.x, float7.x);
			float3 float12 = math.float3(float5.y, float6.y, float7.y);
			float3 float13 = float11 + 0.5f * float12;
			float3 float14 = float12;
			float3 float15 = noise.mod289(float13);
			float14 = noise.mod289(float14);
			float2 float16 = noise.rgrad2(math.float2(float15.x, float14.x), rot);
			float2 float17 = noise.rgrad2(math.float2(float15.y, float14.y), rot);
			float2 float18 = noise.rgrad2(math.float2(float15.z, float14.z), rot);
			float3 float19 = math.float3(math.dot(float16, float8), math.dot(float17, float9), math.dot(float18, float10));
			float3 float20 = math.max(0.8f - math.float3(math.dot(float8, float8), math.dot(float9, float9), math.dot(float10, float10)), 0f);
			float3 float21 = float20 * float20;
			float num = math.dot(float21 * float21, float19);
			return 11f * num;
		}

		public static float srnoise(float2 pos)
		{
			return noise.srnoise(pos, 0f);
		}
	}
}
