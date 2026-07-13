using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.IntegerTime
{
	[NativeHeader("Runtime/Input/RationalTime.h")]
	[Serializable]
	public struct RationalTime
	{
		public RationalTime(long count, RationalTime.TicksPerSecond ticks)
		{
			this.m_Count = count;
			this.m_TicksPerSecond = ticks;
		}

		public long Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public RationalTime.TicksPerSecond Ticks
		{
			get
			{
				return this.m_TicksPerSecond;
			}
		}

		[FreeFunction("IntegerTime::RationalTime::FromDouble", IsFreeFunction = true, ThrowsException = true)]
		public static RationalTime FromDouble(double t, RationalTime.TicksPerSecond ticksPerSecond)
		{
			RationalTime rationalTime;
			RationalTime.FromDouble_Injected(t, ref ticksPerSecond, out rationalTime);
			return rationalTime;
		}

		public static explicit operator DiscreteTime(RationalTime t)
		{
			return DiscreteTime.FromTicks(t.Convert(RationalTime.TicksPerSecond.DiscreteTimeRate).Count);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FromDouble_Injected(double t, [In] ref RationalTime.TicksPerSecond ticksPerSecond, out RationalTime ret);

		[SerializeField]
		private long m_Count;

		[SerializeField]
		private RationalTime.TicksPerSecond m_TicksPerSecond;

		[Serializable]
		public struct TicksPerSecond : IEquatable<RationalTime.TicksPerSecond>
		{
			public TicksPerSecond(uint num, uint den = 1U)
			{
				this.m_Numerator = num;
				this.m_Denominator = den;
				RationalTime.TicksPerSecond.Simplify(ref this.m_Numerator, ref this.m_Denominator);
			}

			public readonly uint Numerator
			{
				get
				{
					return this.m_Numerator;
				}
			}

			public readonly uint Denominator
			{
				get
				{
					return this.m_Denominator;
				}
			}

			public readonly bool Valid
			{
				get
				{
					return RationalTime.TicksPerSecond.IsValid(this);
				}
			}

			public readonly bool Equals(RationalTime.TicksPerSecond rhs)
			{
				return this.m_Numerator == rhs.m_Numerator && this.m_Denominator == rhs.m_Denominator;
			}

			public override readonly bool Equals(object rhs)
			{
				bool flag;
				if (rhs is RationalTime.TicksPerSecond)
				{
					RationalTime.TicksPerSecond ticksPerSecond = (RationalTime.TicksPerSecond)rhs;
					flag = this.Equals(ticksPerSecond);
				}
				else
				{
					flag = false;
				}
				return flag;
			}

			public override readonly int GetHashCode()
			{
				return HashCode.Combine<uint, uint>(this.m_Numerator, this.m_Denominator);
			}

			private static void Simplify(ref uint num, ref uint den)
			{
				bool flag = den > 1U && num > 0U;
				if (flag)
				{
					uint num2 = RationalTime.TicksPerSecond.Gcd(num, den);
					num /= num2;
					den /= num2;
				}
			}

			private static uint Gcd(uint a, uint b)
			{
				for (;;)
				{
					bool flag = a == 0U;
					if (flag)
					{
						break;
					}
					b %= a;
					bool flag2 = b == 0U;
					if (flag2)
					{
						goto Block_2;
					}
					a %= b;
				}
				return b;
				Block_2:
				return a;
			}

			[FreeFunction("IntegerTime::TicksPerSecond::IsValid", IsFreeFunction = true, ThrowsException = false)]
			private static bool IsValid(RationalTime.TicksPerSecond tps)
			{
				return RationalTime.TicksPerSecond.IsValid_Injected(ref tps);
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool IsValid_Injected([In] ref RationalTime.TicksPerSecond tps);

			private const uint k_DefaultTicksPerSecond = 141120000U;

			[SerializeField]
			private uint m_Numerator;

			[SerializeField]
			private uint m_Denominator;

			public static readonly RationalTime.TicksPerSecond DefaultTicksPerSecond = new RationalTime.TicksPerSecond(141120000U, 1U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond24 = new RationalTime.TicksPerSecond(24U, 1U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond25 = new RationalTime.TicksPerSecond(25U, 1U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond30 = new RationalTime.TicksPerSecond(30U, 1U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond50 = new RationalTime.TicksPerSecond(50U, 1U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond60 = new RationalTime.TicksPerSecond(60U, 1U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond120 = new RationalTime.TicksPerSecond(120U, 1U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond2397 = new RationalTime.TicksPerSecond(24000U, 1001U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond2425 = new RationalTime.TicksPerSecond(25000U, 1001U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond2997 = new RationalTime.TicksPerSecond(30000U, 1001U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond5994 = new RationalTime.TicksPerSecond(60000U, 1001U);

			public static readonly RationalTime.TicksPerSecond TicksPerSecond11988 = new RationalTime.TicksPerSecond(120000U, 1001U);

			internal static readonly RationalTime.TicksPerSecond DiscreteTimeRate = new RationalTime.TicksPerSecond(141120000U, 1U);
		}
	}
}
