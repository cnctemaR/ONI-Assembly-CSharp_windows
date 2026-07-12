using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
	public struct HashCode
	{
		private unsafe static uint GenerateGlobalSeed()
		{
			uint num;
			Interop.GetRandomBytes((byte*)(&num), 4);
			return num;
		}

		public static int Combine<T1>(T1 value1)
		{
			uint num = (uint)((value1 != null) ? value1.GetHashCode() : 0);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.MixEmptyState() + 4U, num));
		}

		public static int Combine<T1, T2>(T1 value1, T2 value2)
		{
			uint num = (uint)((value1 != null) ? value1.GetHashCode() : 0);
			uint num2 = (uint)((value2 != null) ? value2.GetHashCode() : 0);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixEmptyState() + 8U, num), num2));
		}

		public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
		{
			uint num = (uint)((value1 != null) ? value1.GetHashCode() : 0);
			uint num2 = (uint)((value2 != null) ? value2.GetHashCode() : 0);
			uint num3 = (uint)((value3 != null) ? value3.GetHashCode() : 0);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixEmptyState() + 12U, num), num2), num3));
		}

		public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
		{
			uint num = (uint)((value1 != null) ? value1.GetHashCode() : 0);
			uint num2 = (uint)((value2 != null) ? value2.GetHashCode() : 0);
			uint num3 = (uint)((value3 != null) ? value3.GetHashCode() : 0);
			uint num4 = (uint)((value4 != null) ? value4.GetHashCode() : 0);
			uint num5;
			uint num6;
			uint num7;
			uint num8;
			HashCode.Initialize(out num5, out num6, out num7, out num8);
			num5 = HashCode.Round(num5, num);
			num6 = HashCode.Round(num6, num2);
			num7 = HashCode.Round(num7, num3);
			num8 = HashCode.Round(num8, num4);
			return (int)HashCode.MixFinal(HashCode.MixState(num5, num6, num7, num8) + 16U);
		}

		public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
		{
			uint num = (uint)((value1 != null) ? value1.GetHashCode() : 0);
			uint num2 = (uint)((value2 != null) ? value2.GetHashCode() : 0);
			uint num3 = (uint)((value3 != null) ? value3.GetHashCode() : 0);
			uint num4 = (uint)((value4 != null) ? value4.GetHashCode() : 0);
			uint num5 = (uint)((value5 != null) ? value5.GetHashCode() : 0);
			uint num6;
			uint num7;
			uint num8;
			uint num9;
			HashCode.Initialize(out num6, out num7, out num8, out num9);
			num6 = HashCode.Round(num6, num);
			num7 = HashCode.Round(num7, num2);
			num8 = HashCode.Round(num8, num3);
			num9 = HashCode.Round(num9, num4);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.MixState(num6, num7, num8, num9) + 20U, num5));
		}

		public static int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
		{
			uint num = (uint)((value1 != null) ? value1.GetHashCode() : 0);
			uint num2 = (uint)((value2 != null) ? value2.GetHashCode() : 0);
			uint num3 = (uint)((value3 != null) ? value3.GetHashCode() : 0);
			uint num4 = (uint)((value4 != null) ? value4.GetHashCode() : 0);
			uint num5 = (uint)((value5 != null) ? value5.GetHashCode() : 0);
			uint num6 = (uint)((value6 != null) ? value6.GetHashCode() : 0);
			uint num7;
			uint num8;
			uint num9;
			uint num10;
			HashCode.Initialize(out num7, out num8, out num9, out num10);
			num7 = HashCode.Round(num7, num);
			num8 = HashCode.Round(num8, num2);
			num9 = HashCode.Round(num9, num3);
			num10 = HashCode.Round(num10, num4);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixState(num7, num8, num9, num10) + 24U, num5), num6));
		}

		public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
		{
			uint num = (uint)((value1 != null) ? value1.GetHashCode() : 0);
			uint num2 = (uint)((value2 != null) ? value2.GetHashCode() : 0);
			uint num3 = (uint)((value3 != null) ? value3.GetHashCode() : 0);
			uint num4 = (uint)((value4 != null) ? value4.GetHashCode() : 0);
			uint num5 = (uint)((value5 != null) ? value5.GetHashCode() : 0);
			uint num6 = (uint)((value6 != null) ? value6.GetHashCode() : 0);
			uint num7 = (uint)((value7 != null) ? value7.GetHashCode() : 0);
			uint num8;
			uint num9;
			uint num10;
			uint num11;
			HashCode.Initialize(out num8, out num9, out num10, out num11);
			num8 = HashCode.Round(num8, num);
			num9 = HashCode.Round(num9, num2);
			num10 = HashCode.Round(num10, num3);
			num11 = HashCode.Round(num11, num4);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixState(num8, num9, num10, num11) + 28U, num5), num6), num7));
		}

		public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
		{
			uint num = (uint)((value1 != null) ? value1.GetHashCode() : 0);
			uint num2 = (uint)((value2 != null) ? value2.GetHashCode() : 0);
			uint num3 = (uint)((value3 != null) ? value3.GetHashCode() : 0);
			uint num4 = (uint)((value4 != null) ? value4.GetHashCode() : 0);
			uint num5 = (uint)((value5 != null) ? value5.GetHashCode() : 0);
			uint num6 = (uint)((value6 != null) ? value6.GetHashCode() : 0);
			uint num7 = (uint)((value7 != null) ? value7.GetHashCode() : 0);
			uint num8 = (uint)((value8 != null) ? value8.GetHashCode() : 0);
			uint num9;
			uint num10;
			uint num11;
			uint num12;
			HashCode.Initialize(out num9, out num10, out num11, out num12);
			num9 = HashCode.Round(num9, num);
			num10 = HashCode.Round(num10, num2);
			num11 = HashCode.Round(num11, num3);
			num12 = HashCode.Round(num12, num4);
			num9 = HashCode.Round(num9, num5);
			num10 = HashCode.Round(num10, num6);
			num11 = HashCode.Round(num11, num7);
			num12 = HashCode.Round(num12, num8);
			return (int)HashCode.MixFinal(HashCode.MixState(num9, num10, num11, num12) + 32U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint Rol(uint value, int count)
		{
			return (value << count) | (value >> 32 - count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
		{
			v1 = HashCode.s_seed + 2654435761U + 2246822519U;
			v2 = HashCode.s_seed + 2246822519U;
			v3 = HashCode.s_seed;
			v4 = HashCode.s_seed - 2654435761U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint Round(uint hash, uint input)
		{
			hash += input * 2246822519U;
			hash = HashCode.Rol(hash, 13);
			hash *= 2654435761U;
			return hash;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint QueueRound(uint hash, uint queuedValue)
		{
			hash += queuedValue * 3266489917U;
			return HashCode.Rol(hash, 17) * 668265263U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint MixState(uint v1, uint v2, uint v3, uint v4)
		{
			return HashCode.Rol(v1, 1) + HashCode.Rol(v2, 7) + HashCode.Rol(v3, 12) + HashCode.Rol(v4, 18);
		}

		private static uint MixEmptyState()
		{
			return HashCode.s_seed + 374761393U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint MixFinal(uint hash)
		{
			hash ^= hash >> 15;
			hash *= 2246822519U;
			hash ^= hash >> 13;
			hash *= 3266489917U;
			hash ^= hash >> 16;
			return hash;
		}

		public void Add<T>(T value)
		{
			this.Add((value != null) ? value.GetHashCode() : 0);
		}

		public void Add<T>(T value, IEqualityComparer<T> comparer)
		{
			this.Add((comparer != null) ? comparer.GetHashCode(value) : ((value != null) ? value.GetHashCode() : 0));
		}

		private void Add(int value)
		{
			uint length = this._length;
			this._length = length + 1U;
			uint num = length;
			uint num2 = num % 4U;
			if (num2 == 0U)
			{
				this._queue1 = (uint)value;
				return;
			}
			if (num2 == 1U)
			{
				this._queue2 = (uint)value;
				return;
			}
			if (num2 == 2U)
			{
				this._queue3 = (uint)value;
				return;
			}
			if (num == 3U)
			{
				HashCode.Initialize(out this._v1, out this._v2, out this._v3, out this._v4);
			}
			this._v1 = HashCode.Round(this._v1, this._queue1);
			this._v2 = HashCode.Round(this._v2, this._queue2);
			this._v3 = HashCode.Round(this._v3, this._queue3);
			this._v4 = HashCode.Round(this._v4, (uint)value);
		}

		public int ToHashCode()
		{
			uint length = this._length;
			uint num = length % 4U;
			uint num2 = ((length < 4U) ? HashCode.MixEmptyState() : HashCode.MixState(this._v1, this._v2, this._v3, this._v4));
			num2 += length * 4U;
			if (num > 0U)
			{
				num2 = HashCode.QueueRound(num2, this._queue1);
				if (num > 1U)
				{
					num2 = HashCode.QueueRound(num2, this._queue2);
					if (num > 2U)
					{
						num2 = HashCode.QueueRound(num2, this._queue3);
					}
				}
			}
			return (int)HashCode.MixFinal(num2);
		}

		[Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", true)]
		public override int GetHashCode()
		{
			throw new NotSupportedException("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.");
		}

		[Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
		public override bool Equals(object obj)
		{
			throw new NotSupportedException("HashCode is a mutable struct and should not be compared with other HashCodes.");
		}

		private static readonly uint s_seed = HashCode.GenerateGlobalSeed();

		private const uint Prime1 = 2654435761U;

		private const uint Prime2 = 2246822519U;

		private const uint Prime3 = 3266489917U;

		private const uint Prime4 = 668265263U;

		private const uint Prime5 = 374761393U;

		private uint _v1;

		private uint _v2;

		private uint _v3;

		private uint _v4;

		private uint _queue1;

		private uint _queue2;

		private uint _queue3;

		private uint _length;
	}
}
