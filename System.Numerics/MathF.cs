using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class MathF
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Abs(float x)
		{
			return Math.Abs(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Acos(float x)
		{
			return (float)Math.Acos((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Cos(float x)
		{
			return (float)Math.Cos((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float IEEERemainder(float x, float y)
		{
			return (float)Math.IEEERemainder((double)x, (double)y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Pow(float x, float y)
		{
			return (float)Math.Pow((double)x, (double)y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sin(float x)
		{
			return (float)Math.Sin((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sqrt(float x)
		{
			return (float)Math.Sqrt((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Tan(float x)
		{
			return (float)Math.Tan((double)x);
		}

		public const float PI = 3.1415927f;
	}
}
