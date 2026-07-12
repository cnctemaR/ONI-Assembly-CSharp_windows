using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class IPv6AddressHelper
	{
		[return: TupleElementNames(new string[] { "longestSequenceStart", "longestSequenceLength" })]
		internal unsafe static ValueTuple<int, int> FindCompressionRange(ReadOnlySpan<ushort> numbers)
		{
			int num = 0;
			int num2 = -1;
			int num3 = 0;
			for (int i = 0; i < numbers.Length; i++)
			{
				if (*numbers[i] == 0)
				{
					num3++;
					if (num3 > num)
					{
						num = num3;
						num2 = i - num3 + 1;
					}
				}
				else
				{
					num3 = 0;
				}
			}
			if (num <= 1)
			{
				return new ValueTuple<int, int>(-1, -1);
			}
			return new ValueTuple<int, int>(num2, num2 + num);
		}

		internal unsafe static bool ShouldHaveIpv4Embedded(ReadOnlySpan<ushort> numbers)
		{
			if (*numbers[0] == 0 && *numbers[1] == 0 && *numbers[2] == 0 && *numbers[3] == 0 && *numbers[6] != 0)
			{
				if (*numbers[4] == 0 && (*numbers[5] == 0 || *numbers[5] == 65535))
				{
					return true;
				}
				if (*numbers[4] == 65535 && *numbers[5] == 0)
				{
					return true;
				}
			}
			return *numbers[4] == 0 && *numbers[5] == 24318;
		}

		internal unsafe static bool IsValidStrict(char* name, int start, ref int end)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			int num3 = 1;
			bool flag4 = false;
			if (start < end && name[start] == '[')
			{
				start++;
				flag4 = true;
			}
			for (int i = start; i < end; i++)
			{
				if (Uri.IsHexDigit(name[i]))
				{
					num2++;
					flag3 = false;
				}
				else
				{
					if (num2 > 4)
					{
						return false;
					}
					if (num2 != 0)
					{
						num++;
						num3 = i - num2;
						num2 = 0;
					}
					char c = name[i];
					if (c <= '.')
					{
						if (c == '%')
						{
							while (i + 1 < end)
							{
								i++;
								if (name[i] == ']')
								{
									goto IL_00D0;
								}
								if (name[i] == '/')
								{
									return false;
								}
							}
							goto IL_01C7;
						}
						if (c != '.')
						{
							return false;
						}
						if (flag2)
						{
							return false;
						}
						i = end;
						if (!IPv4AddressHelper.IsValid(name, num3, ref i, true, false, false))
						{
							return false;
						}
						num++;
						num3 = i - num2;
						flag2 = true;
						i--;
						goto IL_01C7;
					}
					else
					{
						if (c == '/')
						{
							return false;
						}
						if (c != ':')
						{
							if (c != ']')
							{
								return false;
							}
						}
						else
						{
							if (i <= 0 || name[i - 1] != ':')
							{
								flag3 = true;
								goto IL_01C7;
							}
							if (flag)
							{
								return false;
							}
							flag = true;
							flag3 = false;
							goto IL_01C7;
						}
					}
					IL_00D0:
					if (!flag4)
					{
						return false;
					}
					flag4 = false;
					if (i + 1 < end && name[i + 1] != ':')
					{
						return false;
					}
					if (i + 3 < end && name[i + 2] == '0' && name[i + 3] == 'x')
					{
						for (i += 4; i < end; i++)
						{
							if (!Uri.IsHexDigit(name[i]))
							{
								return false;
							}
						}
						goto IL_01C9;
					}
					for (i += 2; i < end; i++)
					{
						if (name[i] < '0' || name[i] > '9')
						{
							return false;
						}
					}
					goto IL_01C9;
					IL_01C7:
					num2 = 0;
				}
				IL_01C9:;
			}
			if (num2 != 0)
			{
				if (num2 > 4)
				{
					return false;
				}
				num++;
			}
			return !flag3 && (flag ? (num < 8) : (num == 8)) && !flag4;
		}

		internal unsafe static void Parse(ReadOnlySpan<char> address, ushort* numbers, int start, ref string scopeId)
		{
			int num = 0;
			int num2 = 0;
			int num3 = -1;
			bool flag = true;
			int num4 = 0;
			if (*address[start] == 91)
			{
				start++;
			}
			int i = start;
			while (i < address.Length && *address[i] != 93)
			{
				char c = (char)(*address[i]);
				if (c != '%')
				{
					if (c != '/')
					{
						if (c != ':')
						{
							num = num * 16 + Uri.FromHex((char)(*address[i++]));
						}
						else
						{
							numbers[num2++] = (ushort)num;
							num = 0;
							i++;
							if (*address[i] == 58)
							{
								num3 = num2;
								i++;
							}
							else if (num3 < 0 && num2 < 6)
							{
								continue;
							}
							int num5 = i;
							while (num5 < address.Length && *address[num5] != 93 && *address[num5] != 58 && *address[num5] != 37 && *address[num5] != 47)
							{
								if (num5 >= i + 4)
								{
									break;
								}
								if (*address[num5] == 46)
								{
									while (num5 < address.Length && *address[num5] != 93 && *address[num5] != 47 && *address[num5] != 37)
									{
										num5++;
									}
									num = IPv4AddressHelper.ParseHostNumber(address, i, num5);
									numbers[num2++] = (ushort)(num >> 16);
									numbers[num2++] = (ushort)num;
									i = num5;
									num = 0;
									flag = false;
									break;
								}
								num5++;
							}
						}
					}
					else
					{
						if (flag)
						{
							numbers[num2++] = (ushort)num;
							flag = false;
						}
						i++;
						while (*address[i] != 93)
						{
							num4 = num4 * 10 + (int)(*address[i] - 48);
							i++;
						}
					}
				}
				else
				{
					if (flag)
					{
						numbers[num2++] = (ushort)num;
						flag = false;
					}
					start = i;
					i++;
					while (i < address.Length && *address[i] != 93 && *address[i] != 47)
					{
						i++;
					}
					scopeId = new string(address.Slice(start, i - start));
					while (i < address.Length)
					{
						if (*address[i] == 93)
						{
							break;
						}
						i++;
					}
				}
			}
			if (flag)
			{
				numbers[num2++] = (ushort)num;
			}
			if (num3 > 0)
			{
				int num6 = 7;
				int num7 = num2 - 1;
				for (int j = num2 - num3; j > 0; j--)
				{
					numbers[num6--] = numbers[num7];
					numbers[num7--] = 0;
				}
			}
		}

		internal unsafe static string ParseCanonicalName(string str, int start, ref bool isLoopback, ref string scopeId)
		{
			ushort* ptr = stackalloc ushort[(UIntPtr)16];
			*(long*)ptr = 0L;
			*(long*)(ptr + 4) = 0L;
			Span<ushort> span = new Span<ushort>((void*)ptr, 8);
			IPv6AddressHelper.Parse(str, ptr, start, ref scopeId);
			isLoopback = IPv6AddressHelper.IsLoopback(span);
			ValueTuple<int, int> valueTuple = IPv6AddressHelper.FindCompressionRange(span);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			bool flag = IPv6AddressHelper.ShouldHaveIpv4Embedded(span);
			Span<char> span2 = new Span<char>(stackalloc byte[(UIntPtr)96], 48);
			*span2[0] = '[';
			int num = 1;
			for (int i = 0; i < 8; i++)
			{
				if (flag && i == 6)
				{
					*span2[num++] = ':';
					int num2;
					(*span[i] >> 8).TryFormat(span2.Slice(num), out num2, default(ReadOnlySpan<char>), null);
					num += num2;
					*span2[num++] = '.';
					((int)(*span[i] & 255)).TryFormat(span2.Slice(num), out num2, default(ReadOnlySpan<char>), null);
					num += num2;
					*span2[num++] = '.';
					(*span[i + 1] >> 8).TryFormat(span2.Slice(num), out num2, default(ReadOnlySpan<char>), null);
					num += num2;
					*span2[num++] = '.';
					((int)(*span[i + 1] & 255)).TryFormat(span2.Slice(num), out num2, default(ReadOnlySpan<char>), null);
					num += num2;
					break;
				}
				if (item == i)
				{
					*span2[num++] = ':';
				}
				if (item <= i && item2 == 8)
				{
					*span2[num++] = ':';
					break;
				}
				if (item > i || i >= item2)
				{
					if (i != 0)
					{
						*span2[num++] = ':';
					}
					int num2;
					span[i].TryFormat(span2.Slice(num), out num2, "x", null);
					num += num2;
				}
			}
			*span2[num++] = ']';
			return new string(span2.Slice(0, num));
		}

		private unsafe static bool IsLoopback(ReadOnlySpan<ushort> numbers)
		{
			return *numbers[0] == 0 && *numbers[1] == 0 && *numbers[2] == 0 && *numbers[3] == 0 && *numbers[4] == 0 && ((*numbers[5] == 0 && *numbers[6] == 0 && *numbers[7] == 1) || (*numbers[6] == 32512 && *numbers[7] == 1 && (*numbers[5] == 0 || *numbers[5] == ushort.MaxValue)));
		}

		private unsafe static bool ShouldHaveIpv4Embedded(ushort* numbers)
		{
			if (*numbers == 0 && numbers[1] == 0 && numbers[2] == 0 && numbers[3] == 0 && numbers[6] != 0)
			{
				if (numbers[4] == 0 && (numbers[5] == 0 || numbers[5] == 65535))
				{
					return true;
				}
				if (numbers[4] == 65535 && numbers[5] == 0)
				{
					return true;
				}
			}
			return numbers[4] == 0 && numbers[5] == 24318;
		}

		private unsafe static bool InternalIsValid(char* name, int start, ref int end, bool validateStrictAddress)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			int num3 = 1;
			int i;
			for (i = start; i < end; i++)
			{
				if (flag3 ? (name[i] >= '0' && name[i] <= '9') : Uri.IsHexDigit(name[i]))
				{
					num2++;
					flag4 = false;
				}
				else
				{
					if (num2 > 4)
					{
						return false;
					}
					if (num2 != 0)
					{
						num++;
						num3 = i - num2;
					}
					char c = name[i];
					if (c <= '.')
					{
						if (c == '%')
						{
							while (++i != end)
							{
								if (name[i] == ']')
								{
									goto IL_00CD;
								}
								if (name[i] == '/')
								{
									goto IL_00FB;
								}
							}
							return false;
						}
						if (c != '.')
						{
							return false;
						}
						if (flag2)
						{
							return false;
						}
						i = end;
						if (!IPv4AddressHelper.IsValid(name, num3, ref i, true, false, false))
						{
							return false;
						}
						num++;
						flag2 = true;
						i--;
						goto IL_013D;
					}
					else
					{
						if (c == '/')
						{
							goto IL_00FB;
						}
						if (c != ':')
						{
							if (c != ']')
							{
								return false;
							}
						}
						else
						{
							if (i <= 0 || name[i - 1] != ':')
							{
								flag4 = true;
								goto IL_013D;
							}
							if (flag)
							{
								return false;
							}
							flag = true;
							flag4 = false;
							goto IL_013D;
						}
					}
					IL_00CD:
					start = i;
					i = end;
					goto IL_013F;
					IL_013D:
					num2 = 0;
					goto IL_013F;
					IL_00FB:
					if (validateStrictAddress)
					{
						return false;
					}
					if (num == 0 || flag3)
					{
						return false;
					}
					flag3 = true;
					flag4 = true;
					goto IL_013D;
				}
				IL_013F:;
			}
			if (flag3 && (num2 < 1 || num2 > 2))
			{
				return false;
			}
			int num4 = 8 + (flag3 ? 1 : 0);
			if (flag4 || num2 > 4 || !(flag ? (num < num4) : (num == num4)))
			{
				return false;
			}
			if (i == end + 1)
			{
				end = start + 1;
				return true;
			}
			return false;
		}

		internal unsafe static bool IsValid(char* name, int start, ref int end)
		{
			return IPv6AddressHelper.InternalIsValid(name, start, ref end, false);
		}

		private const int NumberOfLabels = 8;
	}
}
