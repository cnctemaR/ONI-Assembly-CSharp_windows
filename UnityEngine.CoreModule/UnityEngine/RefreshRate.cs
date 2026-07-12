using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType("Runtime/Graphics/RefreshRate.h")]
	public struct RefreshRate : IEquatable<RefreshRate>, IComparable<RefreshRate>
	{
		public double value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.numerator / this.denominator;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(RefreshRate other)
		{
			bool flag = this.denominator == 0U;
			bool flag2;
			if (flag)
			{
				flag2 = other.denominator == 0U;
			}
			else
			{
				bool flag3 = other.denominator == 0U;
				flag2 = !flag3 && (ulong)this.numerator * (ulong)other.denominator == (ulong)this.denominator * (ulong)other.numerator;
			}
			return flag2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(RefreshRate other)
		{
			bool flag = this.denominator == 0U;
			int num;
			if (flag)
			{
				num = ((other.denominator == 0U) ? 0 : 1);
			}
			else
			{
				bool flag2 = other.denominator == 0U;
				if (flag2)
				{
					num = -1;
				}
				else
				{
					num = ((ulong)this.numerator * (ulong)other.denominator).CompareTo((ulong)this.denominator * (ulong)other.numerator);
				}
			}
			return num;
		}

		public override string ToString()
		{
			return this.value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}

		[RequiredMember]
		public uint numerator;

		[RequiredMember]
		public uint denominator;
	}
}
