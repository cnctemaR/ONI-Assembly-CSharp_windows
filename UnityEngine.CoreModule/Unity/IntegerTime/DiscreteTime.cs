using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.IntegerTime
{
	[Serializable]
	public struct DiscreteTime : IEquatable<DiscreteTime>, IFormattable, IComparable<DiscreteTime>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DiscreteTime(DiscreteTime x)
		{
			this.Value = x.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DiscreteTime(float v)
		{
			this.Value = (long)Math.Round((double)v * 141120000.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DiscreteTime(double v)
		{
			this.Value = (long)Math.Round(v * 141120000.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DiscreteTime(long v)
		{
			this.Value = v * 141120000L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DiscreteTime(int v)
		{
			this.Value = (long)v * 141120000L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private DiscreteTime(long v, int _)
		{
			this.Value = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime FromTicks(long v)
		{
			return new DiscreteTime(v, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator DiscreteTime(float v)
		{
			return new DiscreteTime(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator DiscreteTime(double v)
		{
			return new DiscreteTime(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float(DiscreteTime d)
		{
			return (float)((double)d.Value / 141120000.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double(DiscreteTime d)
		{
			return (double)d.Value / 141120000.0;
		}

		public static implicit operator RationalTime(DiscreteTime t)
		{
			return new RationalTime(t.Value, RationalTime.TicksPerSecond.DiscreteTimeRate);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(DiscreteTime lhs, DiscreteTime rhs)
		{
			return lhs.Value == rhs.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(DiscreteTime lhs, DiscreteTime rhs)
		{
			return lhs.Value != rhs.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(DiscreteTime lhs, DiscreteTime rhs)
		{
			return lhs.Value < rhs.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(DiscreteTime lhs, DiscreteTime rhs)
		{
			return lhs.Value > rhs.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(DiscreteTime lhs, DiscreteTime rhs)
		{
			return lhs.Value <= rhs.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(DiscreteTime lhs, DiscreteTime rhs)
		{
			return lhs.Value >= rhs.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime operator +(DiscreteTime lhs, DiscreteTime rhs)
		{
			return DiscreteTime.FromTicks(lhs.Value + rhs.Value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime operator -(DiscreteTime lhs, DiscreteTime rhs)
		{
			return DiscreteTime.FromTicks(lhs.Value - rhs.Value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime operator *(DiscreteTime lhs, long rhs)
		{
			return DiscreteTime.FromTicks(lhs.Value * rhs);
		}

		public static DiscreteTime operator *(DiscreteTime lhs, double s)
		{
			double num2;
			double num = DiscreteTime.Modf(s, out num2);
			long num3 = lhs.Value * (long)num2;
			bool flag = Math.Abs(num) >= 7.0861678004535145E-09;
			if (flag)
			{
				int num4 = DiscreteTime.Lzcnt(Math.Abs(lhs.Value)) - 1;
				long num5 = 1L << (num4 & 31);
				bool flag2 = num4 >= DiscreteTime.TicksPerSecondBits;
				if (flag2)
				{
					num5 = 141120000L;
				}
				else
				{
					bool flag3 = num4 >= DiscreteTime.NonPow2TpsBits;
					if (flag3)
					{
						num5 = (long)(275625UL << ((num4 - DiscreteTime.NonPow2TpsBits) & 31));
					}
				}
				long num6 = (long)Math.Round((double)num5 / num);
				num3 += lhs.Value * num5 / num6;
			}
			return DiscreteTime.FromTicks(num3);
		}

		private static double Modf(double x, out double i)
		{
			i = Math.Truncate(x);
			return x - i;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static int Lzcnt(long x)
		{
			bool flag = x == 0L;
			int num;
			if (flag)
			{
				num = 64;
			}
			else
			{
				uint num2 = (uint)(x >> 32);
				uint num3 = ((num2 != 0U) ? num2 : ((uint)x));
				int num4 = ((num2 != 0U) ? 1054 : 1086);
				DiscreteTime.LongDoubleUnion longDoubleUnion;
				longDoubleUnion.doubleValue = 0.0;
				longDoubleUnion.longValue = (long)(4841369599423283200UL + (ulong)num3);
				longDoubleUnion.doubleValue -= 4503599627370496.0;
				num = num4 - (int)(longDoubleUnion.longValue >> 52);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime operator *(DiscreteTime lhs, float s)
		{
			return lhs * (double)s;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime operator /(DiscreteTime lhs, double s)
		{
			return lhs * (1.0 / s);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime operator /(DiscreteTime lhs, long s)
		{
			return DiscreteTime.FromTicks(lhs.Value / s);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime operator %(DiscreteTime lhs, DiscreteTime rhs)
		{
			return DiscreteTime.FromTicks(lhs.Value % rhs.Value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime operator -(DiscreteTime lhs)
		{
			return DiscreteTime.FromTicks(-lhs.Value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(DiscreteTime rhs)
		{
			return this.Value == rhs.Value;
		}

		public override readonly bool Equals(object o)
		{
			return this.Equals((DiscreteTime)o);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public override readonly string ToString()
		{
			return ((double)this).ToString();
		}

		public readonly string ToString(string format, IFormatProvider formatProvider)
		{
			return ((double)this).ToString(format, formatProvider);
		}

		public readonly int CompareTo(DiscreteTime other)
		{
			return this.Value.CompareTo(other.Value);
		}

		[SerializeField]
		public long Value;

		public static readonly DiscreteTime Zero = default(DiscreteTime);

		public static readonly DiscreteTime MinValue = new DiscreteTime(long.MinValue, 0);

		public static readonly DiscreteTime MaxValue = new DiscreteTime(long.MaxValue, 0);

		private const int Pow2Exp = 9;

		private const uint Pow2Tps = 512U;

		private const uint NonPow2Tps = 275625U;

		private static readonly int TicksPerSecondBits = (int)Mathf.Ceil(Mathf.Log(141120000f, 2f));

		private static readonly int NonPow2TpsBits = (int)Mathf.Ceil(Mathf.Log(275625f, 2f));

		public const uint TicksPerSecond = 141120000U;

		public const double Tick = 7.0861678004535145E-09;

		public const long MaxValueSeconds = 65358361939L;

		public const long MinValueSeconds = -65358361939L;

		public const uint Tick5Fps = 28224000U;

		public const uint Tick10Fps = 14112000U;

		public const uint Tick12Fps = 11760000U;

		public const uint Tick15Fps = 9408000U;

		public const uint Tick2397Fps = 5885880U;

		public const uint Tick24Fps = 5880000U;

		public const uint Tick25Fps = 5644800U;

		public const uint Tick2997Fps = 4708704U;

		public const uint Tick30Fps = 4704000U;

		public const uint Tick48Fps = 2940000U;

		public const uint Tick50Fps = 2822400U;

		public const uint Tick5995Fps = 2354352U;

		public const uint Tick60Fps = 2352000U;

		public const uint Tick90Fps = 1568000U;

		public const uint Tick11988Fps = 1177176U;

		public const uint Tick120Fps = 1176000U;

		public const uint Tick240Fps = 588000U;

		public const uint Tick1000Fps = 141120U;

		public const uint Tick8Khz = 17640U;

		public const uint Tick16Khz = 8820U;

		public const uint Tick22Khz = 6400U;

		public const uint Tick44Khz = 3200U;

		public const uint Tick48Khz = 2940U;

		public const uint Tick88Khz = 1600U;

		public const uint Tick96Khz = 1470U;

		public const uint Tick192Khz = 735U;

		[StructLayout(LayoutKind.Explicit)]
		private struct LongDoubleUnion
		{
			[FieldOffset(0)]
			public long longValue;

			[FieldOffset(0)]
			public double doubleValue;
		}
	}
}
