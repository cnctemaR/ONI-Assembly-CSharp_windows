using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine
{
	internal static class SpookyHash
	{
		private static bool AttemptDetectAllowUnalignedRead()
		{
			string processorType = SystemInfo.processorType;
			return processorType == "x86" || processorType == "AMD64";
		}

		public unsafe static void Hash(void* message, ulong length, ulong* hash1, ulong* hash2)
		{
			bool flag = length < 192UL;
			if (flag)
			{
				SpookyHash.Short(message, length, hash1, hash2);
			}
			else
			{
				ulong* ptr = stackalloc ulong[(UIntPtr)96];
				ulong num4;
				ulong num3;
				ulong num2;
				ulong num = (num2 = (num3 = (num4 = *hash1)));
				ulong num8;
				ulong num7;
				ulong num6;
				ulong num5 = (num6 = (num7 = (num8 = *hash2)));
				ulong num12;
				ulong num11;
				ulong num10;
				ulong num9 = (num10 = (num11 = (num12 = 16045690984833335023UL)));
				SpookyHash.U u = new SpookyHash.U((ushort*)message);
				ulong* ptr2 = u.p64 + length / 96UL * 12UL * 8UL / 8UL;
				bool flag2 = SpookyHash.AllowUnalignedRead || (u.i & 7UL) == 0UL;
				if (flag2)
				{
					while (u.p64 < ptr2)
					{
						SpookyHash.Mix(u.p64, ref num2, ref num6, ref num10, ref num, ref num5, ref num9, ref num3, ref num7, ref num11, ref num4, ref num8, ref num12);
						u.p64 += (IntPtr)12 * 8;
					}
				}
				else
				{
					while (u.p64 < ptr2)
					{
						UnsafeUtility.MemCpy((void*)ptr, (void*)u.p64, 96L);
						SpookyHash.Mix(ptr, ref num2, ref num6, ref num10, ref num, ref num5, ref num9, ref num3, ref num7, ref num11, ref num4, ref num8, ref num12);
						u.p64 += (IntPtr)12 * 8;
					}
				}
				ulong num13 = length - (ulong)((long)((byte*)ptr2 - (byte*)message));
				UnsafeUtility.MemCpy((void*)ptr, (void*)ptr2, (long)num13);
				SpookyHash.memset((void*)(ptr + num13 / 8UL), 0, 96UL - num13);
				((byte*)ptr)[95] = (byte)num13;
				SpookyHash.End(ptr, ref num2, ref num6, ref num10, ref num, ref num5, ref num9, ref num3, ref num7, ref num11, ref num4, ref num8, ref num12);
				*hash1 = num2;
				*hash2 = num6;
			}
		}

		private unsafe static void End(ulong* data, ref ulong h0, ref ulong h1, ref ulong h2, ref ulong h3, ref ulong h4, ref ulong h5, ref ulong h6, ref ulong h7, ref ulong h8, ref ulong h9, ref ulong h10, ref ulong h11)
		{
			h0 += *data;
			h1 += data[1];
			h2 += data[2];
			h3 += data[3];
			h4 += data[4];
			h5 += data[5];
			h6 += data[6];
			h7 += data[7];
			h8 += data[8];
			h9 += data[9];
			h10 += data[10];
			h11 += data[11];
			SpookyHash.EndPartial(ref h0, ref h1, ref h2, ref h3, ref h4, ref h5, ref h6, ref h7, ref h8, ref h9, ref h10, ref h11);
			SpookyHash.EndPartial(ref h0, ref h1, ref h2, ref h3, ref h4, ref h5, ref h6, ref h7, ref h8, ref h9, ref h10, ref h11);
			SpookyHash.EndPartial(ref h0, ref h1, ref h2, ref h3, ref h4, ref h5, ref h6, ref h7, ref h8, ref h9, ref h10, ref h11);
		}

		private static void EndPartial(ref ulong h0, ref ulong h1, ref ulong h2, ref ulong h3, ref ulong h4, ref ulong h5, ref ulong h6, ref ulong h7, ref ulong h8, ref ulong h9, ref ulong h10, ref ulong h11)
		{
			h11 += h1;
			h2 ^= h11;
			SpookyHash.Rot64(ref h1, 44);
			h0 += h2;
			h3 ^= h0;
			SpookyHash.Rot64(ref h2, 15);
			h1 += h3;
			h4 ^= h1;
			SpookyHash.Rot64(ref h3, 34);
			h2 += h4;
			h5 ^= h2;
			SpookyHash.Rot64(ref h4, 21);
			h3 += h5;
			h6 ^= h3;
			SpookyHash.Rot64(ref h5, 38);
			h4 += h6;
			h7 ^= h4;
			SpookyHash.Rot64(ref h6, 33);
			h5 += h7;
			h8 ^= h5;
			SpookyHash.Rot64(ref h7, 10);
			h6 += h8;
			h9 ^= h6;
			SpookyHash.Rot64(ref h8, 13);
			h7 += h9;
			h10 ^= h7;
			SpookyHash.Rot64(ref h9, 38);
			h8 += h10;
			h11 ^= h8;
			SpookyHash.Rot64(ref h10, 53);
			h9 += h11;
			h0 ^= h9;
			SpookyHash.Rot64(ref h11, 42);
			h10 += h0;
			h1 ^= h10;
			SpookyHash.Rot64(ref h0, 54);
		}

		private static void Rot64(ref ulong x, int k)
		{
			x = (x << k) | (x >> 64 - k);
		}

		private unsafe static void Short(void* message, ulong length, ulong* hash1, ulong* hash2)
		{
			ulong* ptr = stackalloc ulong[(UIntPtr)192];
			SpookyHash.U u = new SpookyHash.U((ushort*)message);
			bool flag = !SpookyHash.AllowUnalignedRead && (u.i & 7UL) > 0UL;
			if (flag)
			{
				UnsafeUtility.MemCpy((void*)ptr, message, (long)length);
				u.p64 = ptr;
			}
			ulong num = length % 32UL;
			ulong num2 = *hash1;
			ulong num3 = *hash2;
			ulong num4 = 16045690984833335023UL;
			ulong num5 = 16045690984833335023UL;
			bool flag2 = length > 15UL;
			if (flag2)
			{
				ulong* ptr2 = u.p64 + length / 32UL * 4UL * 8UL / 8UL;
				while (u.p64 < ptr2)
				{
					num4 += *u.p64;
					num5 += u.p64[1];
					SpookyHash.ShortMix(ref num2, ref num3, ref num4, ref num5);
					num2 += u.p64[2];
					num3 += u.p64[3];
					u.p64 += (IntPtr)4 * 8;
				}
				bool flag3 = num >= 16UL;
				if (flag3)
				{
					num4 += *u.p64;
					num5 += u.p64[1];
					SpookyHash.ShortMix(ref num2, ref num3, ref num4, ref num5);
					u.p64 += (IntPtr)2 * 8;
					num -= 16UL;
				}
			}
			num5 += length << 56;
			ulong num6 = num;
			ulong num7 = num6;
			if (num7 <= 15UL)
			{
				switch ((uint)num7)
				{
				case 0U:
					num4 += 16045690984833335023UL;
					num5 += 16045690984833335023UL;
					goto IL_0313;
				case 1U:
					goto IL_02E6;
				case 2U:
					goto IL_02D3;
				case 3U:
					num4 += (ulong)u.p8[2] << 16;
					goto IL_02D3;
				case 4U:
					goto IL_02AD;
				case 5U:
					goto IL_0296;
				case 6U:
					num4 += (ulong)u.p8[5] << 40;
					goto IL_0296;
				case 7U:
					num4 += (ulong)u.p8[6] << 48;
					goto IL_0243;
				case 8U:
					goto IL_0257;
				case 9U:
					goto IL_0243;
				case 10U:
					goto IL_022C;
				case 11U:
					num5 += (ulong)u.p8[10] << 16;
					goto IL_022C;
				case 12U:
					goto IL_01F1;
				case 13U:
					goto IL_01D9;
				case 14U:
					break;
				case 15U:
					num5 += (ulong)u.p8[14] << 48;
					break;
				default:
					goto IL_0313;
				}
				num5 += (ulong)u.p8[13] << 40;
				IL_01D9:
				num5 += (ulong)u.p8[12] << 32;
				IL_01F1:
				num5 += (ulong)u.p32[2];
				num4 += *u.p64;
				goto IL_0313;
				IL_022C:
				num5 += (ulong)u.p8[9] << 8;
				IL_0243:
				num5 += (ulong)u.p8[8];
				IL_0257:
				num4 += *u.p64;
				goto IL_0313;
				IL_0296:
				num4 += (ulong)u.p8[4] << 32;
				IL_02AD:
				num4 += (ulong)(*u.p32);
				goto IL_0313;
				IL_02D3:
				num4 += (ulong)u.p8[1] << 8;
				IL_02E6:
				num4 += (ulong)(*u.p8);
			}
			IL_0313:
			SpookyHash.ShortEnd(ref num2, ref num3, ref num4, ref num5);
			*hash1 = num2;
			*hash2 = num3;
		}

		private static void ShortMix(ref ulong h0, ref ulong h1, ref ulong h2, ref ulong h3)
		{
			SpookyHash.Rot64(ref h2, 50);
			h2 += h3;
			h0 ^= h2;
			SpookyHash.Rot64(ref h3, 52);
			h3 += h0;
			h1 ^= h3;
			SpookyHash.Rot64(ref h0, 30);
			h0 += h1;
			h2 ^= h0;
			SpookyHash.Rot64(ref h1, 41);
			h1 += h2;
			h3 ^= h1;
			SpookyHash.Rot64(ref h2, 54);
			h2 += h3;
			h0 ^= h2;
			SpookyHash.Rot64(ref h3, 48);
			h3 += h0;
			h1 ^= h3;
			SpookyHash.Rot64(ref h0, 38);
			h0 += h1;
			h2 ^= h0;
			SpookyHash.Rot64(ref h1, 37);
			h1 += h2;
			h3 ^= h1;
			SpookyHash.Rot64(ref h2, 62);
			h2 += h3;
			h0 ^= h2;
			SpookyHash.Rot64(ref h3, 34);
			h3 += h0;
			h1 ^= h3;
			SpookyHash.Rot64(ref h0, 5);
			h0 += h1;
			h2 ^= h0;
			SpookyHash.Rot64(ref h1, 36);
			h1 += h2;
			h3 ^= h1;
		}

		private static void ShortEnd(ref ulong h0, ref ulong h1, ref ulong h2, ref ulong h3)
		{
			h3 ^= h2;
			SpookyHash.Rot64(ref h2, 15);
			h3 += h2;
			h0 ^= h3;
			SpookyHash.Rot64(ref h3, 52);
			h0 += h3;
			h1 ^= h0;
			SpookyHash.Rot64(ref h0, 26);
			h1 += h0;
			h2 ^= h1;
			SpookyHash.Rot64(ref h1, 51);
			h2 += h1;
			h3 ^= h2;
			SpookyHash.Rot64(ref h2, 28);
			h3 += h2;
			h0 ^= h3;
			SpookyHash.Rot64(ref h3, 9);
			h0 += h3;
			h1 ^= h0;
			SpookyHash.Rot64(ref h0, 47);
			h1 += h0;
			h2 ^= h1;
			SpookyHash.Rot64(ref h1, 54);
			h2 += h1;
			h3 ^= h2;
			SpookyHash.Rot64(ref h2, 32);
			h3 += h2;
			h0 ^= h3;
			SpookyHash.Rot64(ref h3, 25);
			h0 += h3;
			h1 ^= h0;
			SpookyHash.Rot64(ref h0, 63);
			h1 += h0;
		}

		private unsafe static void Mix(ulong* data, ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3, ref ulong s4, ref ulong s5, ref ulong s6, ref ulong s7, ref ulong s8, ref ulong s9, ref ulong s10, ref ulong s11)
		{
			s0 += *data;
			s2 ^= s10;
			s11 ^= s0;
			SpookyHash.Rot64(ref s0, 11);
			s11 += s1;
			s1 += data[1];
			s3 ^= s11;
			s0 ^= s1;
			SpookyHash.Rot64(ref s1, 32);
			s0 += s2;
			s2 += data[2];
			s4 ^= s0;
			s1 ^= s2;
			SpookyHash.Rot64(ref s2, 43);
			s1 += s3;
			s3 += data[3];
			s5 ^= s1;
			s2 ^= s3;
			SpookyHash.Rot64(ref s3, 31);
			s2 += s4;
			s4 += data[4];
			s6 ^= s2;
			s3 ^= s4;
			SpookyHash.Rot64(ref s4, 17);
			s3 += s5;
			s5 += data[5];
			s7 ^= s3;
			s4 ^= s5;
			SpookyHash.Rot64(ref s5, 28);
			s4 += s6;
			s6 += data[6];
			s8 ^= s4;
			s5 ^= s6;
			SpookyHash.Rot64(ref s6, 39);
			s5 += s7;
			s7 += data[7];
			s9 ^= s5;
			s6 ^= s7;
			SpookyHash.Rot64(ref s7, 57);
			s6 += s8;
			s8 += data[8];
			s10 ^= s6;
			s7 ^= s8;
			SpookyHash.Rot64(ref s8, 55);
			s7 += s9;
			s9 += data[9];
			s11 ^= s7;
			s8 ^= s9;
			SpookyHash.Rot64(ref s9, 54);
			s8 += s10;
			s10 += data[10];
			s0 ^= s8;
			s9 ^= s10;
			SpookyHash.Rot64(ref s10, 22);
			s9 += s11;
			s11 += data[11];
			s1 ^= s9;
			s10 ^= s11;
			SpookyHash.Rot64(ref s11, 46);
			s10 += s0;
		}

		private unsafe static void memset(void* dst, int value, ulong numberOfBytes)
		{
			ulong num = (ulong)(value | value);
			ulong* ptr = (ulong*)dst;
			ulong num2 = numberOfBytes >> 3;
			for (ulong num3 = 0UL; num3 < num2; num3 += 1UL)
			{
				ptr[num3 * 8UL / 8UL] = num;
			}
			dst = (void*)ptr;
			numberOfBytes -= num2;
			byte* ptr2 = stackalloc byte[(UIntPtr)4];
			*ptr2 = (byte)(value & 15);
			ptr2[1] = (byte)(((uint)value >> 4) & 15U);
			ptr2[2] = (byte)(((uint)value >> 8) & 15U);
			ptr2[3] = (byte)(((uint)value >> 12) & 15U);
			byte* ptr3 = (byte*)dst;
			ulong num4 = numberOfBytes;
			for (ulong num5 = 0UL; num5 < num4; num5 += 1UL)
			{
				ptr3[num5] = ptr2[num5 % 4UL];
			}
		}

		private static readonly bool AllowUnalignedRead = SpookyHash.AttemptDetectAllowUnalignedRead();

		private const int k_NumVars = 12;

		private const int k_BlockSize = 96;

		private const int k_BufferSize = 192;

		private const ulong k_DeadBeefConst = 16045690984833335023UL;

		[StructLayout(LayoutKind.Explicit)]
		private struct U
		{
			public unsafe U(ushort* p8)
			{
				this.p32 = null;
				this.p64 = null;
				this.i = 0UL;
				this.p8 = p8;
			}

			[FieldOffset(0)]
			public unsafe ushort* p8;

			[FieldOffset(0)]
			public unsafe uint* p32;

			[FieldOffset(0)]
			public unsafe ulong* p64;

			[FieldOffset(0)]
			public ulong i;
		}
	}
}
