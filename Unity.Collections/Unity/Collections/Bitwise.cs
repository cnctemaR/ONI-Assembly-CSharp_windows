using System;
using Unity.Mathematics;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	internal struct Bitwise
	{
		internal static int AlignDown(int value, int alignPow2)
		{
			return value & ~(alignPow2 - 1);
		}

		internal static int AlignUp(int value, int alignPow2)
		{
			return Bitwise.AlignDown(value + alignPow2 - 1, alignPow2);
		}

		internal static int FromBool(bool value)
		{
			if (!value)
			{
				return 0;
			}
			return 1;
		}

		internal static uint ExtractBits(uint input, int pos, uint mask)
		{
			return (input >> pos) & mask;
		}

		internal static uint ReplaceBits(uint input, int pos, uint mask, uint value)
		{
			uint num = (value & mask) << pos;
			uint num2 = input & ~(mask << pos);
			return num | num2;
		}

		internal static uint SetBits(uint input, int pos, uint mask, bool value)
		{
			return Bitwise.ReplaceBits(input, pos, mask, (uint)(-(uint)Bitwise.FromBool(value)));
		}

		internal static ulong ExtractBits(ulong input, int pos, ulong mask)
		{
			return (input >> pos) & mask;
		}

		internal static ulong ReplaceBits(ulong input, int pos, ulong mask, ulong value)
		{
			ulong num = (value & mask) << pos;
			ulong num2 = input & ~(mask << pos);
			return num | num2;
		}

		internal static ulong SetBits(ulong input, int pos, ulong mask, bool value)
		{
			return Bitwise.ReplaceBits(input, pos, mask, (ulong)(-(ulong)((long)Bitwise.FromBool(value))));
		}

		internal static int lzcnt(byte value)
		{
			return math.lzcnt((uint)value) - 24;
		}

		internal static int tzcnt(byte value)
		{
			return math.min(8, math.tzcnt((uint)value));
		}

		internal static int lzcnt(ushort value)
		{
			return math.lzcnt((uint)value) - 16;
		}

		internal static int tzcnt(ushort value)
		{
			return math.min(16, math.tzcnt((uint)value));
		}

		private unsafe static int FindUlong(ulong* ptr, int beginBit, int endBit, int numBits)
		{
			int num = numBits + 63 >> 6;
			int num2 = 64;
			int i = beginBit / num2;
			int num3 = Bitwise.AlignUp(endBit, num2) / num2;
			while (i < num3)
			{
				if (ptr[i] == 0UL)
				{
					int num4 = i * num2;
					int num5 = math.min(num4 + num2, endBit) - num4;
					if (num4 != beginBit)
					{
						ulong num6 = ptr[num4 / num2 - 1];
						int num7 = math.max(num4 - math.lzcnt(num6), beginBit);
						num5 += num4 - num7;
						num4 = num7;
					}
					for (i++; i < num3; i++)
					{
						if (num5 >= numBits)
						{
							return num4;
						}
						ulong num8 = ptr[i];
						int num9 = i * num2;
						num5 += math.min(num9 + math.tzcnt(num8), endBit) - num9;
						if (num8 != 0UL)
						{
							break;
						}
					}
					if (num5 >= numBits)
					{
						return num4;
					}
				}
				i++;
			}
			return endBit;
		}

		private unsafe static int FindUint(ulong* ptr, int beginBit, int endBit, int numBits)
		{
			int num = numBits + 31 >> 5;
			int num2 = 32;
			int i = beginBit / num2;
			int num3 = Bitwise.AlignUp(endBit, num2) / num2;
			while (i < num3)
			{
				if (*(uint*)(ptr + (IntPtr)i * 4 / 8) == 0U)
				{
					int num4 = i * num2;
					int num5 = math.min(num4 + num2, endBit) - num4;
					if (num4 != beginBit)
					{
						uint num6 = *(uint*)(ptr + (IntPtr)(num4 / num2 - 1) * 4 / 8);
						int num7 = math.max(num4 - math.lzcnt(num6), beginBit);
						num5 += num4 - num7;
						num4 = num7;
					}
					for (i++; i < num3; i++)
					{
						if (num5 >= numBits)
						{
							return num4;
						}
						uint num8 = *(uint*)(ptr + (IntPtr)i * 4 / 8);
						int num9 = i * num2;
						num5 += math.min(num9 + math.tzcnt(num8), endBit) - num9;
						if (num8 != 0U)
						{
							break;
						}
					}
					if (num5 >= numBits)
					{
						return num4;
					}
				}
				i++;
			}
			return endBit;
		}

		private unsafe static int FindUshort(ulong* ptr, int beginBit, int endBit, int numBits)
		{
			int num = numBits + 15 >> 4;
			int num2 = 16;
			int i = beginBit / num2;
			int num3 = Bitwise.AlignUp(endBit, num2) / num2;
			while (i < num3)
			{
				if (*(ushort*)(ptr + (IntPtr)i * 2 / 8) == 0)
				{
					int num4 = i * num2;
					int num5 = math.min(num4 + num2, endBit) - num4;
					if (num4 != beginBit)
					{
						ushort num6 = *(ushort*)(ptr + (IntPtr)(num4 / num2 - 1) * 2 / 8);
						int num7 = math.max(num4 - Bitwise.lzcnt(num6), beginBit);
						num5 += num4 - num7;
						num4 = num7;
					}
					for (i++; i < num3; i++)
					{
						if (num5 >= numBits)
						{
							return num4;
						}
						ushort num8 = *(ushort*)(ptr + (IntPtr)i * 2 / 8);
						int num9 = i * num2;
						num5 += math.min(num9 + Bitwise.tzcnt(num8), endBit) - num9;
						if (num8 != 0)
						{
							break;
						}
					}
					if (num5 >= numBits)
					{
						return num4;
					}
				}
				i++;
			}
			return endBit;
		}

		private unsafe static int FindByte(ulong* ptr, int beginBit, int endBit, int numBits)
		{
			int num = numBits + 7 >> 3;
			int num2 = 8;
			int i = beginBit / num2;
			int num3 = Bitwise.AlignUp(endBit, num2) / num2;
			while (i < num3)
			{
				if (*(byte*)(ptr + i / 8) == 0)
				{
					int num4 = i * num2;
					int num5 = math.min(num4 + num2, endBit) - num4;
					if (num4 != beginBit)
					{
						byte b = *(byte*)(ptr + (num4 / num2 - 1) / 8);
						int num6 = math.max(num4 - Bitwise.lzcnt(b), beginBit);
						num5 += num4 - num6;
						num4 = num6;
					}
					for (i++; i < num3; i++)
					{
						if (num5 >= numBits)
						{
							return num4;
						}
						byte b2 = *(byte*)(ptr + i / 8);
						int num7 = i * num2;
						num5 += math.min(num7 + Bitwise.tzcnt(b2), endBit) - num7;
						if (b2 != 0)
						{
							break;
						}
					}
					if (num5 >= numBits)
					{
						return num4;
					}
				}
				i++;
			}
			return endBit;
		}

		private unsafe static int FindUpto14bits(ulong* ptr, int beginBit, int endBit, int numBits)
		{
			byte b = (byte)(beginBit & 7);
			byte b2 = (byte)(~(byte)(255 << (int)b));
			int num = 0;
			int num2 = beginBit / 8;
			int num3 = Bitwise.AlignUp(endBit, 8) / 8;
			for (int i = num2; i < num3; i++)
			{
				byte b3 = *(byte*)(ptr + i / 8);
				b3 |= ((i == num2) ? b2 : 0);
				if (b3 != 255)
				{
					int num4 = i * 8;
					int num5 = math.min(num4 + Bitwise.tzcnt(b3), endBit) - num4;
					if (num + num5 >= numBits)
					{
						return num4 - num;
					}
					num = Bitwise.lzcnt(b3);
					int num6 = num4 + 8;
					int num7 = math.max(num6 - num, beginBit);
					num = math.min(num6, endBit) - num7;
					if (num >= numBits)
					{
						return num7;
					}
				}
			}
			return endBit;
		}

		private unsafe static int FindUpto6bits(ulong* ptr, int beginBit, int endBit, int numBits)
		{
			byte b = (byte)(~(byte)(255 << (beginBit & 7)));
			byte b2 = (byte)(~(byte)(255 >> ((8 - (endBit & 7)) & 7)));
			int num = 1 << numBits - 1;
			int num2 = beginBit / 8;
			int num3 = Bitwise.AlignUp(endBit, 8) / 8;
			for (int i = num2; i < num3; i++)
			{
				byte b3 = *(byte*)(ptr + i / 8);
				b3 |= ((i == num2) ? b : 0);
				b3 |= ((i == num3 - 1) ? b2 : 0);
				if (b3 != 255)
				{
					int j = i * 8;
					int num4 = j + 7;
					while (j < num4)
					{
						int num5 = Bitwise.tzcnt(b3 ^ byte.MaxValue);
						b3 = (byte)(b3 >> num5);
						j += num5;
						if (((int)b3 & num) == 0)
						{
							return j;
						}
						b3 = (byte)(b3 >> 1);
						j++;
					}
				}
			}
			return endBit;
		}

		internal unsafe static int FindWithBeginEnd(ulong* ptr, int beginBit, int endBit, int numBits)
		{
			int num;
			if (numBits >= 127)
			{
				num = Bitwise.FindUlong(ptr, beginBit, endBit, numBits);
				if (num != endBit)
				{
					return num;
				}
			}
			if (numBits >= 63)
			{
				num = Bitwise.FindUint(ptr, beginBit, endBit, numBits);
				if (num != endBit)
				{
					return num;
				}
			}
			if (numBits >= 128)
			{
				return int.MaxValue;
			}
			if (numBits >= 31)
			{
				num = Bitwise.FindUshort(ptr, beginBit, endBit, numBits);
				if (num != endBit)
				{
					return num;
				}
			}
			if (numBits >= 64)
			{
				return int.MaxValue;
			}
			num = Bitwise.FindByte(ptr, beginBit, endBit, numBits);
			if (num != endBit)
			{
				return num;
			}
			if (numBits < 15)
			{
				num = Bitwise.FindUpto14bits(ptr, beginBit, endBit, numBits);
				if (num != endBit)
				{
					return num;
				}
				if (numBits < 7)
				{
					num = Bitwise.FindUpto6bits(ptr, beginBit, endBit, numBits);
					if (num != endBit)
					{
						return num;
					}
				}
			}
			return int.MaxValue;
		}

		internal unsafe static int Find(ulong* ptr, int pos, int count, int numBits)
		{
			return Bitwise.FindWithBeginEnd(ptr, pos, pos + count, numBits);
		}

		internal unsafe static bool TestNone(ulong* ptr, int length, int pos, int numBits = 1)
		{
			int num = math.min(pos + numBits, length);
			int num2 = pos >> 6;
			int num3 = pos & 63;
			int num4 = num - 1 >> 6;
			int num5 = num & 63;
			ulong num6 = ulong.MaxValue << num3;
			ulong num7 = ulong.MaxValue >> 64 - num5;
			if (num2 == num4)
			{
				ulong num8 = num6 & num7;
				return (ptr[num2] & num8) == 0UL;
			}
			if ((ptr[num2] & num6) != 0UL)
			{
				return false;
			}
			for (int i = num2 + 1; i < num4; i++)
			{
				if (ptr[i] != 0UL)
				{
					return false;
				}
			}
			return (ptr[num4] & num7) == 0UL;
		}

		internal unsafe static bool TestAny(ulong* ptr, int length, int pos, int numBits = 1)
		{
			int num = math.min(pos + numBits, length);
			int num2 = pos >> 6;
			int num3 = pos & 63;
			int num4 = num - 1 >> 6;
			int num5 = num & 63;
			ulong num6 = ulong.MaxValue << num3;
			ulong num7 = ulong.MaxValue >> 64 - num5;
			if (num2 == num4)
			{
				ulong num8 = num6 & num7;
				return (ptr[num2] & num8) > 0UL;
			}
			if ((ptr[num2] & num6) != 0UL)
			{
				return true;
			}
			for (int i = num2 + 1; i < num4; i++)
			{
				if (ptr[i] != 0UL)
				{
					return true;
				}
			}
			return (ptr[num4] & num7) > 0UL;
		}

		internal unsafe static bool TestAll(ulong* ptr, int length, int pos, int numBits = 1)
		{
			int num = math.min(pos + numBits, length);
			int num2 = pos >> 6;
			int num3 = pos & 63;
			int num4 = num - 1 >> 6;
			int num5 = num & 63;
			ulong num6 = ulong.MaxValue << num3;
			ulong num7 = ulong.MaxValue >> 64 - num5;
			if (num2 == num4)
			{
				ulong num8 = num6 & num7;
				return num8 == (ptr[num2] & num8);
			}
			if (num6 != (ptr[num2] & num6))
			{
				return false;
			}
			for (int i = num2 + 1; i < num4; i++)
			{
				if (18446744073709551615UL != ptr[i])
				{
					return false;
				}
			}
			return num7 == (ptr[num4] & num7);
		}

		internal unsafe static int CountBits(ulong* ptr, int length, int pos, int numBits = 1)
		{
			int num = math.min(pos + numBits, length);
			int num2 = pos >> 6;
			int num3 = pos & 63;
			int num4 = num - 1 >> 6;
			int num5 = num & 63;
			ulong num6 = ulong.MaxValue << num3;
			ulong num7 = ulong.MaxValue >> 64 - num5;
			if (num2 == num4)
			{
				ulong num8 = num6 & num7;
				return math.countbits(ptr[num2] & num8);
			}
			int num9 = math.countbits(ptr[num2] & num6);
			for (int i = num2 + 1; i < num4; i++)
			{
				num9 += math.countbits(ptr[i]);
			}
			return num9 + math.countbits(ptr[num4] & num7);
		}

		internal unsafe static bool IsSet(ulong* ptr, int pos)
		{
			int num = pos >> 6;
			int num2 = pos & 63;
			ulong num3 = 1UL << num2;
			return (ptr[num] & num3) > 0UL;
		}

		internal unsafe static ulong GetBits(ulong* ptr, int length, int pos, int numBits = 1)
		{
			int num = pos >> 6;
			int num2 = pos & 63;
			if (num2 + numBits <= 64)
			{
				ulong num3 = ulong.MaxValue >> 64 - numBits;
				return Bitwise.ExtractBits(ptr[num], num2, num3);
			}
			int num4 = math.min(pos + numBits, length);
			int num5 = num4 - 1 >> 6;
			int num6 = num4 & 63;
			ulong num7 = ulong.MaxValue >> num2;
			ulong num8 = Bitwise.ExtractBits(ptr[num], num2, num7);
			ulong num9 = ulong.MaxValue >> 64 - num6;
			return (Bitwise.ExtractBits(ptr[num5], 0, num9) << 64 - num2) | num8;
		}
	}
}
