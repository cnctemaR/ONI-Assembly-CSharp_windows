using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace System
{
	[NullableContext(1)]
	[Nullable(0)]
	internal struct HashCode
	{
		private unsafe static uint GenerateGlobalSeed()
		{
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)4], 4);
			RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			try
			{
				byte[] array = new byte[span.Length];
				randomNumberGenerator.GetBytes(array);
				array.AsSpan<byte>().CopyTo(span);
			}
			finally
			{
				RandomNumberGenerator randomNumberGenerator2 = randomNumberGenerator;
				if (randomNumberGenerator2 != null)
				{
					((IDisposable)randomNumberGenerator2).Dispose();
				}
			}
			return Unsafe.ReadUnaligned<uint>(span[0]);
		}

		public static int Combine<[Nullable(2)] T1>(T1 value1)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0031;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0031:
			uint num2 = num;
			return (int)global::System.HashCode.MixFinal(global::System.HashCode.QueueRound(global::System.HashCode.MixEmptyState() + 4U, num2));
		}

		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2>(T1 value1, T2 value2)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0031;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0031:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0063;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0063:
			uint num4 = num3;
			return (int)global::System.HashCode.MixFinal(global::System.HashCode.QueueRound(global::System.HashCode.QueueRound(global::System.HashCode.MixEmptyState() + 8U, num2), num4));
		}

		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>(T1 value1, T2 value2, T3 value3)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0031;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0031:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0066;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0066:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009B;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009B:
			uint num6 = num5;
			return (int)global::System.HashCode.MixFinal(global::System.HashCode.QueueRound(global::System.HashCode.QueueRound(global::System.HashCode.QueueRound(global::System.HashCode.MixEmptyState() + 12U, num2), num4), num6));
		}

		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4>(T1 value1, T2 value2, T3 value3, T4 value4)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			uint num9;
			uint num10;
			uint num11;
			uint num12;
			global::System.HashCode.Initialize(out num9, out num10, out num11, out num12);
			num9 = global::System.HashCode.Round(num9, num2);
			num10 = global::System.HashCode.Round(num10, num4);
			num11 = global::System.HashCode.Round(num11, num6);
			num12 = global::System.HashCode.Round(num12, num8);
			return (int)global::System.HashCode.MixFinal(global::System.HashCode.MixState(num9, num10, num11, num12) + 16U);
		}

		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			ref T5 ptr5 = ref value5;
			T5 t5 = default(T5);
			uint num9;
			if (t5 == null)
			{
				t5 = value5;
				ptr5 = ref t5;
				if (t5 == null)
				{
					num9 = 0U;
					goto IL_0108;
				}
			}
			num9 = (uint)ptr5.GetHashCode();
			IL_0108:
			uint num10 = num9;
			uint num11;
			uint num12;
			uint num13;
			uint num14;
			global::System.HashCode.Initialize(out num11, out num12, out num13, out num14);
			num11 = global::System.HashCode.Round(num11, num2);
			num12 = global::System.HashCode.Round(num12, num4);
			num13 = global::System.HashCode.Round(num13, num6);
			num14 = global::System.HashCode.Round(num14, num8);
			return (int)global::System.HashCode.MixFinal(global::System.HashCode.QueueRound(global::System.HashCode.MixState(num11, num12, num13, num14) + 20U, num10));
		}

		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			ref T5 ptr5 = ref value5;
			T5 t5 = default(T5);
			uint num9;
			if (t5 == null)
			{
				t5 = value5;
				ptr5 = ref t5;
				if (t5 == null)
				{
					num9 = 0U;
					goto IL_0108;
				}
			}
			num9 = (uint)ptr5.GetHashCode();
			IL_0108:
			uint num10 = num9;
			ref T6 ptr6 = ref value6;
			T6 t6 = default(T6);
			uint num11;
			if (t6 == null)
			{
				t6 = value6;
				ptr6 = ref t6;
				if (t6 == null)
				{
					num11 = 0U;
					goto IL_013E;
				}
			}
			num11 = (uint)ptr6.GetHashCode();
			IL_013E:
			uint num12 = num11;
			uint num13;
			uint num14;
			uint num15;
			uint num16;
			global::System.HashCode.Initialize(out num13, out num14, out num15, out num16);
			num13 = global::System.HashCode.Round(num13, num2);
			num14 = global::System.HashCode.Round(num14, num4);
			num15 = global::System.HashCode.Round(num15, num6);
			num16 = global::System.HashCode.Round(num16, num8);
			return (int)global::System.HashCode.MixFinal(global::System.HashCode.QueueRound(global::System.HashCode.QueueRound(global::System.HashCode.MixState(num13, num14, num15, num16) + 24U, num10), num12));
		}

		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			ref T5 ptr5 = ref value5;
			T5 t5 = default(T5);
			uint num9;
			if (t5 == null)
			{
				t5 = value5;
				ptr5 = ref t5;
				if (t5 == null)
				{
					num9 = 0U;
					goto IL_0108;
				}
			}
			num9 = (uint)ptr5.GetHashCode();
			IL_0108:
			uint num10 = num9;
			ref T6 ptr6 = ref value6;
			T6 t6 = default(T6);
			uint num11;
			if (t6 == null)
			{
				t6 = value6;
				ptr6 = ref t6;
				if (t6 == null)
				{
					num11 = 0U;
					goto IL_013E;
				}
			}
			num11 = (uint)ptr6.GetHashCode();
			IL_013E:
			uint num12 = num11;
			ref T7 ptr7 = ref value7;
			T7 t7 = default(T7);
			uint num13;
			if (t7 == null)
			{
				t7 = value7;
				ptr7 = ref t7;
				if (t7 == null)
				{
					num13 = 0U;
					goto IL_0174;
				}
			}
			num13 = (uint)ptr7.GetHashCode();
			IL_0174:
			uint num14 = num13;
			uint num15;
			uint num16;
			uint num17;
			uint num18;
			global::System.HashCode.Initialize(out num15, out num16, out num17, out num18);
			num15 = global::System.HashCode.Round(num15, num2);
			num16 = global::System.HashCode.Round(num16, num4);
			num17 = global::System.HashCode.Round(num17, num6);
			num18 = global::System.HashCode.Round(num18, num8);
			return (int)global::System.HashCode.MixFinal(global::System.HashCode.QueueRound(global::System.HashCode.QueueRound(global::System.HashCode.QueueRound(global::System.HashCode.MixState(num15, num16, num17, num18) + 28U, num10), num12), num14));
		}

		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			ref T5 ptr5 = ref value5;
			T5 t5 = default(T5);
			uint num9;
			if (t5 == null)
			{
				t5 = value5;
				ptr5 = ref t5;
				if (t5 == null)
				{
					num9 = 0U;
					goto IL_0108;
				}
			}
			num9 = (uint)ptr5.GetHashCode();
			IL_0108:
			uint num10 = num9;
			ref T6 ptr6 = ref value6;
			T6 t6 = default(T6);
			uint num11;
			if (t6 == null)
			{
				t6 = value6;
				ptr6 = ref t6;
				if (t6 == null)
				{
					num11 = 0U;
					goto IL_013E;
				}
			}
			num11 = (uint)ptr6.GetHashCode();
			IL_013E:
			uint num12 = num11;
			ref T7 ptr7 = ref value7;
			T7 t7 = default(T7);
			uint num13;
			if (t7 == null)
			{
				t7 = value7;
				ptr7 = ref t7;
				if (t7 == null)
				{
					num13 = 0U;
					goto IL_0174;
				}
			}
			num13 = (uint)ptr7.GetHashCode();
			IL_0174:
			uint num14 = num13;
			ref T8 ptr8 = ref value8;
			T8 t8 = default(T8);
			uint num15;
			if (t8 == null)
			{
				t8 = value8;
				ptr8 = ref t8;
				if (t8 == null)
				{
					num15 = 0U;
					goto IL_01AA;
				}
			}
			num15 = (uint)ptr8.GetHashCode();
			IL_01AA:
			uint num16 = num15;
			uint num17;
			uint num18;
			uint num19;
			uint num20;
			global::System.HashCode.Initialize(out num17, out num18, out num19, out num20);
			num17 = global::System.HashCode.Round(num17, num2);
			num18 = global::System.HashCode.Round(num18, num4);
			num19 = global::System.HashCode.Round(num19, num6);
			num20 = global::System.HashCode.Round(num20, num8);
			num17 = global::System.HashCode.Round(num17, num10);
			num18 = global::System.HashCode.Round(num18, num12);
			num19 = global::System.HashCode.Round(num19, num14);
			num20 = global::System.HashCode.Round(num20, num16);
			return (int)global::System.HashCode.MixFinal(global::System.HashCode.MixState(num17, num18, num19, num20) + 32U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
		{
			v1 = global::System.HashCode.s_seed + 2654435761U + 2246822519U;
			v2 = global::System.HashCode.s_seed + 2246822519U;
			v3 = global::System.HashCode.s_seed;
			v4 = global::System.HashCode.s_seed - 2654435761U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint Round(uint hash, uint input)
		{
			return BitOperations.RotateLeft(hash + input * 2246822519U, 13) * 2654435761U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint QueueRound(uint hash, uint queuedValue)
		{
			return BitOperations.RotateLeft(hash + queuedValue * 3266489917U, 17) * 668265263U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint MixState(uint v1, uint v2, uint v3, uint v4)
		{
			return BitOperations.RotateLeft(v1, 1) + BitOperations.RotateLeft(v2, 7) + BitOperations.RotateLeft(v3, 12) + BitOperations.RotateLeft(v4, 18);
		}

		private static uint MixEmptyState()
		{
			return global::System.HashCode.s_seed + 374761393U;
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

		public void Add<[Nullable(2)] T>(T value)
		{
			ref T ptr = ref value;
			T t = default(T);
			int num;
			if (t == null)
			{
				t = value;
				ptr = ref t;
				if (t == null)
				{
					num = 0;
					goto IL_0032;
				}
			}
			num = ptr.GetHashCode();
			IL_0032:
			this.Add(num);
		}

		public void Add<[Nullable(2)] T>(T value, [Nullable(new byte[] { 2, 1 })] IEqualityComparer<T> comparer)
		{
			this.Add((value == null) ? 0 : ((comparer != null) ? comparer.GetHashCode(value) : value.GetHashCode()));
		}

		[NullableContext(0)]
		public void AddBytes(global::System.ReadOnlySpan<byte> value)
		{
			ref byte ptr = ref global::System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(value);
			ref byte ptr2 = ref Unsafe.Add<byte>(ref ptr, value.Length);
			while (Unsafe.ByteOffset<byte>(ref ptr, ref ptr2) >= (IntPtr)4)
			{
				this.Add(Unsafe.ReadUnaligned<int>(ref ptr));
				ptr = Unsafe.Add<byte>(ref ptr, 4);
			}
			while (Unsafe.IsAddressLessThan<byte>(ref ptr, ref ptr2))
			{
				this.Add((int)ptr);
				ptr = Unsafe.Add<byte>(ref ptr, 1);
			}
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
				global::System.HashCode.Initialize(out this._v1, out this._v2, out this._v3, out this._v4);
			}
			this._v1 = global::System.HashCode.Round(this._v1, this._queue1);
			this._v2 = global::System.HashCode.Round(this._v2, this._queue2);
			this._v3 = global::System.HashCode.Round(this._v3, this._queue3);
			this._v4 = global::System.HashCode.Round(this._v4, (uint)value);
		}

		public int ToHashCode()
		{
			uint length = this._length;
			uint num = length % 4U;
			uint num2 = ((length < 4U) ? global::System.HashCode.MixEmptyState() : global::System.HashCode.MixState(this._v1, this._v2, this._v3, this._v4));
			num2 += length * 4U;
			if (num > 0U)
			{
				num2 = global::System.HashCode.QueueRound(num2, this._queue1);
				if (num > 1U)
				{
					num2 = global::System.HashCode.QueueRound(num2, this._queue2);
					if (num > 2U)
					{
						num2 = global::System.HashCode.QueueRound(num2, this._queue3);
					}
				}
			}
			return (int)global::System.HashCode.MixFinal(num2);
		}

		[Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			throw new NotSupportedException("GetHashCode on HashCode is not supported");
		}

		[NullableContext(2)]
		[Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			throw new NotSupportedException("Equals on HashCode is not supported");
		}

		private static readonly uint s_seed = global::System.HashCode.GenerateGlobalSeed();

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
