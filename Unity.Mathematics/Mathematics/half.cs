using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct half : IEquatable<half>, IFormattable
	{
		public static float MaxValue
		{
			get
			{
				return 65504f;
			}
		}

		public static float MinValue
		{
			get
			{
				return -65504f;
			}
		}

		public static half MaxValueAsHalf
		{
			get
			{
				return new half(half.MaxValue);
			}
		}

		public static half MinValueAsHalf
		{
			get
			{
				return new half(half.MinValue);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half(half x)
		{
			this.value = x.value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half(float v)
		{
			this.value = (ushort)math.f32tof16(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half(double v)
		{
			this.value = (ushort)math.f32tof16((float)v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator half(float v)
		{
			return new half(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator half(double v)
		{
			return new half(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float(half d)
		{
			return math.f16tof32((uint)d.value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double(half d)
		{
			return (double)math.f16tof32((uint)d.value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(half lhs, half rhs)
		{
			return lhs.value == rhs.value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(half lhs, half rhs)
		{
			return lhs.value != rhs.value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(half rhs)
		{
			return this.value == rhs.value;
		}

		public override bool Equals(object o)
		{
			if (o is half)
			{
				half half = (half)o;
				return this.Equals(half);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return (int)this.value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return math.f16tof32((uint)this.value).ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return math.f16tof32((uint)this.value).ToString(format, formatProvider);
		}

		public ushort value;

		public static readonly half zero;
	}
}
