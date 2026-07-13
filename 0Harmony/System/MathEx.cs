using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class MathEx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte Clamp(byte value, byte min, byte max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<byte>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Clamp(decimal value, decimal min, decimal max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<decimal>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Clamp(double value, double min, double max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<double>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short Clamp(short value, short min, short max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<short>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Clamp(int value, int min, int max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<int>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Clamp(long value, long min, long max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<long>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: NativeInteger]
		public static IntPtr Clamp([NativeInteger] IntPtr value, [NativeInteger] IntPtr min, [NativeInteger] IntPtr max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<IntPtr>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<sbyte>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Clamp(float value, float min, float max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<float>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort Clamp(ushort value, ushort min, ushort max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<ushort>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Clamp(uint value, uint min, uint max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<uint>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Clamp(ulong value, ulong min, ulong max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<ulong>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: NativeInteger]
		public static UIntPtr Clamp([NativeInteger] UIntPtr value, [NativeInteger] UIntPtr min, [NativeInteger] UIntPtr max)
		{
			if (min > max)
			{
				MathEx.ThrowMinMaxException<UIntPtr>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[NullableContext(1)]
		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		private static void ThrowMinMaxException<[Nullable(2)] T>(T min, T max)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Minimum ");
			defaultInterpolatedStringHandler.AppendFormatted<T>(min);
			defaultInterpolatedStringHandler.AppendLiteral(" is less than maximum ");
			defaultInterpolatedStringHandler.AppendFormatted<T>(max);
			throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
