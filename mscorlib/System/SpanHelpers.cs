using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	internal static class SpanHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TComparable>(this ReadOnlySpan<T> span, TComparable comparable) where TComparable : IComparable<T>
		{
			if (comparable == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparable);
			}
			return SpanHelpers.BinarySearch<T, TComparable>(MemoryMarshal.GetReference<T>(span), span.Length, comparable);
		}

		public unsafe static int BinarySearch<T, TComparable>(ref T spanStart, int length, TComparable comparable) where TComparable : IComparable<T>
		{
			int i = 0;
			int num = length - 1;
			while (i <= num)
			{
				int num2 = (int)((uint)(num + i) >> 1);
				int num3 = comparable.CompareTo(*Unsafe.Add<T>(ref spanStart, num2));
				if (num3 == 0)
				{
					return num2;
				}
				if (num3 > 0)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return ~i;
		}

		public static int IndexOf(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			byte b = value;
			ref byte ptr = ref Unsafe.Add<byte>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				int num4 = SpanHelpers.IndexOf(Unsafe.Add<byte>(ref searchSpace, num2), b, num3);
				if (num4 == -1)
				{
					return -1;
				}
				num2 += num4;
				if (SpanHelpers.SequenceEqual<byte>(Unsafe.Add<byte>(ref searchSpace, num2 + 1), ref ptr, num))
				{
					break;
				}
				num2++;
			}
			return num2;
		}

		public unsafe static int IndexOfAny(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.IndexOf(ref searchSpace, *Unsafe.Add<byte>(ref value, i), searchSpaceLength);
				if (num2 < num)
				{
					num = num2;
					searchSpaceLength = num2;
					if (num == 0)
					{
						break;
					}
				}
			}
			return num;
		}

		public unsafe static int LastIndexOfAny(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.LastIndexOf(ref searchSpace, *Unsafe.Add<byte>(ref value, i), searchSpaceLength);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		public unsafe static int IndexOf(ref byte searchSpace, byte value, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)length;
			while ((void*)intPtr2 >= 8)
			{
				intPtr2 -= 8;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					IL_014D:
					return (void*)intPtr;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1))
				{
					IL_0155:
					return (void*)(intPtr + 1);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2))
				{
					IL_0163:
					return (void*)(intPtr + 2);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3))
				{
					IL_0171:
					return (void*)(intPtr + 3);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4))
				{
					return (void*)(intPtr + 4);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5))
				{
					return (void*)(intPtr + 5);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6))
				{
					return (void*)(intPtr + 6);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7))
				{
					return (void*)(intPtr + 7);
				}
				intPtr += 8;
			}
			if ((void*)intPtr2 >= 4)
			{
				intPtr2 -= 4;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					goto IL_014D;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1))
				{
					goto IL_0155;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2))
				{
					goto IL_0163;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3))
				{
					goto IL_0171;
				}
				intPtr += 4;
			}
			while ((void*)intPtr2 != null)
			{
				intPtr2 -= 1;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					goto IL_014D;
				}
				intPtr += 1;
			}
			return -1;
		}

		public static int LastIndexOf(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			byte b = value;
			ref byte ptr = ref Unsafe.Add<byte>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			int num4;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				num4 = SpanHelpers.LastIndexOf(ref searchSpace, b, num3);
				if (num4 == -1)
				{
					return -1;
				}
				if (SpanHelpers.SequenceEqual<byte>(Unsafe.Add<byte>(ref searchSpace, num4 + 1), ref ptr, num))
				{
					break;
				}
				num2 += num3 - num4;
			}
			return num4;
		}

		public unsafe static int LastIndexOf(ref byte searchSpace, byte value, int length)
		{
			IntPtr intPtr = (IntPtr)length;
			IntPtr intPtr2 = (IntPtr)length;
			while ((void*)intPtr2 >= 8)
			{
				intPtr2 -= 8;
				intPtr -= 8;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7))
				{
					return (void*)(intPtr + 7);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6))
				{
					return (void*)(intPtr + 6);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5))
				{
					return (void*)(intPtr + 5);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4))
				{
					return (void*)(intPtr + 4);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3))
				{
					IL_0171:
					return (void*)(intPtr + 3);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2))
				{
					IL_0163:
					return (void*)(intPtr + 2);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1))
				{
					IL_0155:
					return (void*)(intPtr + 1);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					IL_014D:
					return (void*)intPtr;
				}
			}
			if ((void*)intPtr2 >= 4)
			{
				intPtr2 -= 4;
				intPtr -= 4;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3))
				{
					goto IL_0171;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2))
				{
					goto IL_0163;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1))
				{
					goto IL_0155;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					goto IL_014D;
				}
			}
			while ((void*)intPtr2 != null)
			{
				intPtr2 -= 1;
				intPtr -= 1;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					goto IL_014D;
				}
			}
			return -1;
		}

		public unsafe static int IndexOfAny(ref byte searchSpace, byte value0, byte value1, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)length;
			while ((void*)intPtr2 >= 8)
			{
				intPtr2 -= 8;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01E5:
					return (void*)intPtr;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01ED:
					return (void*)(intPtr + 1);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01FB:
					return (void*)(intPtr + 2);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_0209:
					return (void*)(intPtr + 3);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 4);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 5);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 6);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 7);
				}
				intPtr += 8;
			}
			if ((void*)intPtr2 >= 4)
			{
				intPtr2 -= 4;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01E5;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01ED;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01FB;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_0209;
				}
				intPtr += 4;
			}
			while ((void*)intPtr2 != null)
			{
				intPtr2 -= 1;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01E5;
				}
				intPtr += 1;
			}
			return -1;
		}

		public unsafe static int IndexOfAny(ref byte searchSpace, byte value0, byte value1, byte value2, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)length;
			while ((void*)intPtr2 >= 8)
			{
				intPtr2 -= 8;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_025A:
					return (void*)intPtr;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0262:
					return (void*)(intPtr + 1);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0270:
					return (void*)(intPtr + 2);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_027E:
					return (void*)(intPtr + 3);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 4);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 5);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 6);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 7);
				}
				intPtr += 8;
			}
			if ((void*)intPtr2 >= 4)
			{
				intPtr2 -= 4;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_025A;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0262;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0270;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_027E;
				}
				intPtr += 4;
			}
			while ((void*)intPtr2 != null)
			{
				intPtr2 -= 1;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_025A;
				}
				intPtr += 1;
			}
			return -1;
		}

		public unsafe static int LastIndexOfAny(ref byte searchSpace, byte value0, byte value1, int length)
		{
			IntPtr intPtr = (IntPtr)length;
			IntPtr intPtr2 = (IntPtr)length;
			while ((void*)intPtr2 >= 8)
			{
				intPtr2 -= 8;
				intPtr -= 8;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 7);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 6);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 5);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 4);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_0209:
					return (void*)(intPtr + 3);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01FB:
					return (void*)(intPtr + 2);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01ED:
					return (void*)(intPtr + 1);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01E5:
					return (void*)intPtr;
				}
			}
			if ((void*)intPtr2 >= 4)
			{
				intPtr2 -= 4;
				intPtr -= 4;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_0209;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01FB;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01ED;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num)
				{
					goto IL_01E5;
				}
				if ((uint)value1 == num)
				{
					goto IL_01E5;
				}
			}
			while ((void*)intPtr2 != null)
			{
				intPtr2 -= 1;
				intPtr -= 1;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01E5;
				}
			}
			return -1;
		}

		public unsafe static int LastIndexOfAny(ref byte searchSpace, byte value0, byte value1, byte value2, int length)
		{
			IntPtr intPtr = (IntPtr)length;
			IntPtr intPtr2 = (IntPtr)length;
			while ((void*)intPtr2 >= 8)
			{
				intPtr2 -= 8;
				intPtr -= 8;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 7);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 6);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 5);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 4);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_027C:
					return (void*)(intPtr + 3);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_026E:
					return (void*)(intPtr + 2);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0260:
					return (void*)(intPtr + 1);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0258:
					return (void*)intPtr;
				}
			}
			if ((void*)intPtr2 >= 4)
			{
				intPtr2 -= 4;
				intPtr -= 4;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_027C;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_026E;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0260;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_0258;
				}
				if ((uint)value2 == num)
				{
					goto IL_0258;
				}
			}
			while ((void*)intPtr2 != null)
			{
				intPtr2 -= 1;
				intPtr -= 1;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0258;
				}
			}
			return -1;
		}

		public unsafe static bool SequenceEqual(ref byte first, ref byte second, ulong length)
		{
			if (!Unsafe.AreSame<byte>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)0;
				IntPtr intPtr2 = (IntPtr)length;
				if ((void*)intPtr2 >= sizeof(UIntPtr))
				{
					intPtr2 -= sizeof(UIntPtr);
					while ((void*)intPtr2 != (void*)intPtr)
					{
						if (Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr)) != Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr)))
						{
							return false;
						}
						intPtr += sizeof(UIntPtr);
					}
					return Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr2)) == Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr2));
				}
				while ((void*)intPtr2 != (void*)intPtr)
				{
					if (*Unsafe.AddByteOffset<byte>(ref first, intPtr) != *Unsafe.AddByteOffset<byte>(ref second, intPtr))
					{
						return false;
					}
					intPtr += 1;
				}
				return true;
			}
			return true;
		}

		public unsafe static int SequenceCompareTo(ref byte first, int firstLength, ref byte second, int secondLength)
		{
			if (!Unsafe.AreSame<byte>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)((firstLength < secondLength) ? firstLength : secondLength);
				IntPtr intPtr2 = (IntPtr)0;
				IntPtr intPtr3 = (IntPtr)((void*)intPtr);
				if ((void*)intPtr3 != sizeof(UIntPtr))
				{
					intPtr3 -= sizeof(UIntPtr);
					while ((void*)intPtr3 != (void*)intPtr2)
					{
						if (Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr2)) != Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr2)))
						{
							break;
						}
						intPtr2 += sizeof(UIntPtr);
					}
				}
				while ((void*)intPtr != (void*)intPtr2)
				{
					int num = Unsafe.AddByteOffset<byte>(ref first, intPtr2).CompareTo(*Unsafe.AddByteOffset<byte>(ref second, intPtr2));
					if (num != 0)
					{
						return num;
					}
					intPtr2 += 1;
				}
			}
			return firstLength - secondLength;
		}

		public unsafe static int SequenceCompareTo(ref char first, int firstLength, ref char second, int secondLength)
		{
			int num = firstLength - secondLength;
			if (!Unsafe.AreSame<char>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)((firstLength < secondLength) ? firstLength : secondLength);
				IntPtr intPtr2 = (IntPtr)0;
				if ((void*)intPtr >= sizeof(UIntPtr) / 2)
				{
					if (Vector.IsHardwareAccelerated && (void*)intPtr >= Vector<ushort>.Count)
					{
						IntPtr intPtr3 = intPtr - Vector<ushort>.Count;
						while (!(Unsafe.ReadUnaligned<Vector<ushort>>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref first, intPtr2))) != Unsafe.ReadUnaligned<Vector<ushort>>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref second, intPtr2)))))
						{
							intPtr2 += Vector<ushort>.Count;
							if ((void*)intPtr3 < (void*)intPtr2)
							{
								break;
							}
						}
					}
					while ((void*)intPtr >= (void*)(intPtr2 + sizeof(UIntPtr) / 2) && !(Unsafe.ReadUnaligned<UIntPtr>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref first, intPtr2))) != Unsafe.ReadUnaligned<UIntPtr>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref second, intPtr2)))))
					{
						intPtr2 += sizeof(UIntPtr) / 2;
					}
				}
				if (sizeof(UIntPtr) > 4 && (void*)intPtr >= (void*)(intPtr2 + 2) && Unsafe.ReadUnaligned<int>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref first, intPtr2))) == Unsafe.ReadUnaligned<int>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref second, intPtr2))))
				{
					intPtr2 += 2;
				}
				while ((void*)intPtr2 < (void*)intPtr)
				{
					int num2 = Unsafe.Add<char>(ref first, intPtr2).CompareTo(*Unsafe.Add<char>(ref second, intPtr2));
					if (num2 != 0)
					{
						return num2;
					}
					intPtr2 += 1;
				}
			}
			return num;
		}

		public unsafe static int IndexOf(ref char searchSpace, char value, int length)
		{
			fixed (char* ptr = &searchSpace)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2;
				char* ptr4 = ptr3 + length;
				if (Vector.IsHardwareAccelerated && length >= Vector<ushort>.Count * 2)
				{
					int num = (ptr3 & (Unsafe.SizeOf<Vector<ushort>>() - 1)) / 2;
					length = (Vector<ushort>.Count - num) & (Vector<ushort>.Count - 1);
				}
				Vector<ushort> vector2;
				for (;;)
				{
					if (length < 4)
					{
						while (length > 0)
						{
							length--;
							if (*ptr3 == value)
							{
								goto IL_0127;
							}
							ptr3++;
						}
						if (!Vector.IsHardwareAccelerated || ptr3 >= ptr4)
						{
							return -1;
						}
						length = (int)((long)(ptr4 - ptr3) & (long)(~(long)(Vector<ushort>.Count - 1)));
						Vector<ushort> vector = new Vector<ushort>((ushort)value);
						while (length > 0)
						{
							vector2 = Vector.Equals<ushort>(vector, Unsafe.Read<Vector<ushort>>((void*)ptr3));
							if (!Vector<ushort>.Zero.Equals(vector2))
							{
								goto IL_00F3;
							}
							ptr3 += Vector<ushort>.Count;
							length -= Vector<ushort>.Count;
						}
						if (ptr3 >= ptr4)
						{
							return -1;
						}
						length = (int)((long)(ptr4 - ptr3));
					}
					else
					{
						length -= 4;
						if (*ptr3 == value)
						{
							goto IL_0127;
						}
						if (ptr3[1] == value)
						{
							goto IL_0123;
						}
						if (ptr3[2] == value)
						{
							goto IL_011F;
						}
						if (ptr3[3] == value)
						{
							goto IL_011B;
						}
						ptr3 += 4;
					}
				}
				IL_00F3:
				return (int)((long)(ptr3 - ptr2)) + SpanHelpers.LocateFirstFoundChar(vector2);
				IL_011B:
				ptr3++;
				IL_011F:
				ptr3++;
				IL_0123:
				ptr3++;
				IL_0127:
				return (int)((long)(ptr3 - ptr2));
			}
		}

		public unsafe static int LastIndexOf(ref char searchSpace, char value, int length)
		{
			fixed (char* ptr = &searchSpace)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + length;
				char* ptr4 = ptr2;
				if (Vector.IsHardwareAccelerated && length >= Vector<ushort>.Count * 2)
				{
					length = (ptr3 & (Unsafe.SizeOf<Vector<ushort>>() - 1)) / 2;
				}
				char* ptr5;
				Vector<ushort> vector2;
				for (;;)
				{
					if (length < 4)
					{
						while (length > 0)
						{
							length--;
							ptr3--;
							if (*ptr3 == value)
							{
								goto IL_011A;
							}
						}
						if (!Vector.IsHardwareAccelerated || ptr3 == ptr4)
						{
							return -1;
						}
						length = (int)((long)(ptr3 - ptr4) & (long)(~(long)(Vector<ushort>.Count - 1)));
						Vector<ushort> vector = new Vector<ushort>((ushort)value);
						while (length > 0)
						{
							ptr5 = ptr3 - Vector<ushort>.Count;
							vector2 = Vector.Equals<ushort>(vector, Unsafe.Read<Vector<ushort>>((void*)ptr5));
							if (!Vector<ushort>.Zero.Equals(vector2))
							{
								goto IL_00F1;
							}
							ptr3 -= Vector<ushort>.Count;
							length -= Vector<ushort>.Count;
						}
						if (ptr3 == ptr4)
						{
							return -1;
						}
						length = (int)((long)(ptr3 - ptr4));
					}
					else
					{
						length -= 4;
						ptr3 -= 4;
						if (ptr3[3] == value)
						{
							goto IL_0136;
						}
						if (ptr3[2] == value)
						{
							goto IL_012C;
						}
						if (ptr3[1] == value)
						{
							goto IL_0122;
						}
						if (*ptr3 == value)
						{
							goto IL_011A;
						}
					}
				}
				IL_00F1:
				return (int)((long)(ptr5 - ptr4)) + SpanHelpers.LocateLastFoundChar(vector2);
				IL_011A:
				return (int)((long)(ptr3 - ptr4));
				IL_0122:
				return (int)((long)(ptr3 - ptr4)) + 1;
				IL_012C:
				return (int)((long)(ptr3 - ptr4)) + 2;
				IL_0136:
				return (int)((long)(ptr3 - ptr4)) + 3;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateFirstFoundChar(Vector<ushort> match)
		{
			Vector<ulong> vector = Vector.AsVectorUInt64<ushort>(match);
			ulong num = 0UL;
			int i;
			for (i = 0; i < Vector<ulong>.Count; i++)
			{
				num = vector[i];
				if (num != 0UL)
				{
					break;
				}
			}
			return i * 4 + SpanHelpers.LocateFirstFoundChar(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateFirstFoundChar(ulong match)
		{
			return (int)((match ^ (match - 1UL)) * 4295098372UL >> 49);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateLastFoundChar(Vector<ushort> match)
		{
			Vector<ulong> vector = Vector.AsVectorUInt64<ushort>(match);
			ulong num = 0UL;
			int i;
			for (i = Vector<ulong>.Count - 1; i >= 0; i--)
			{
				num = vector[i];
				if (num != 0UL)
				{
					break;
				}
			}
			return i * 4 + SpanHelpers.LocateLastFoundChar(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateLastFoundChar(ulong match)
		{
			int num = 3;
			while (match > 0UL)
			{
				match <<= 16;
				num--;
			}
			return num;
		}

		public static int IndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			T t = value;
			ref T ptr = ref Unsafe.Add<T>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				int num4 = SpanHelpers.IndexOf<T>(Unsafe.Add<T>(ref searchSpace, num2), t, num3);
				if (num4 == -1)
				{
					return -1;
				}
				num2 += num4;
				if (SpanHelpers.SequenceEqual<T>(Unsafe.Add<T>(ref searchSpace, num2 + 1), ref ptr, num))
				{
					break;
				}
				num2++;
			}
			return num2;
		}

		public unsafe static int IndexOf<T>(ref T searchSpace, T value, int length) where T : IEquatable<T>
		{
			IntPtr intPtr = (IntPtr)0;
			while (length >= 8)
			{
				length -= 8;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr)))
				{
					IL_0202:
					return (void*)intPtr;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 1)))
				{
					IL_020A:
					return (void*)(intPtr + 1);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 2)))
				{
					IL_0218:
					return (void*)(intPtr + 2);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 3)))
				{
					IL_0226:
					return (void*)(intPtr + 3);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 4)))
				{
					return (void*)(intPtr + 4);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 5)))
				{
					return (void*)(intPtr + 5);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 6)))
				{
					return (void*)(intPtr + 6);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 7)))
				{
					return (void*)(intPtr + 7);
				}
				intPtr += 8;
			}
			if (length >= 4)
			{
				length -= 4;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr)))
				{
					goto IL_0202;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 1)))
				{
					goto IL_020A;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 2)))
				{
					goto IL_0218;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 3)))
				{
					goto IL_0226;
				}
				intPtr += 4;
			}
			while (length > 0)
			{
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr)))
				{
					goto IL_0202;
				}
				intPtr += 1;
				length--;
			}
			return -1;
		}

		public unsafe static int IndexOfAny<T>(ref T searchSpace, T value0, T value1, int length) where T : IEquatable<T>
		{
			int i = 0;
			while (length - i >= 8)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02CB:
					return i + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02CF:
					return i + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02D3:
					return i + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 4);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 5);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 6);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 7);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 7;
				}
				i += 8;
			}
			if (length - i >= 4)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02CB;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02CF;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02D3;
				}
				i += 4;
			}
			while (i < length)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		public unsafe static int IndexOfAny<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : IEquatable<T>
		{
			int i = 0;
			while (length - i >= 8)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03C2:
					return i + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03C6:
					return i + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03CA:
					return i + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 4);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 5);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 6);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 7);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 7;
				}
				i += 8;
			}
			if (length - i >= 4)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03C2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03C6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03CA;
				}
				i += 4;
			}
			while (i < length)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		public unsafe static int IndexOfAny<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.IndexOf<T>(ref searchSpace, *Unsafe.Add<T>(ref value, i), searchSpaceLength);
				if (num2 < num)
				{
					num = num2;
					searchSpaceLength = num2;
					if (num == 0)
					{
						break;
					}
				}
			}
			return num;
		}

		public static int LastIndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			T t = value;
			ref T ptr = ref Unsafe.Add<T>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			int num4;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				num4 = SpanHelpers.LastIndexOf<T>(ref searchSpace, t, num3);
				if (num4 == -1)
				{
					return -1;
				}
				if (SpanHelpers.SequenceEqual<T>(Unsafe.Add<T>(ref searchSpace, num4 + 1), ref ptr, num))
				{
					break;
				}
				num2 += num3 - num4;
			}
			return num4;
		}

		public unsafe static int LastIndexOf<T>(ref T searchSpace, T value, int length) where T : IEquatable<T>
		{
			while (length >= 8)
			{
				length -= 8;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 7)))
				{
					return length + 7;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 6)))
				{
					return length + 6;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 5)))
				{
					return length + 5;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 4)))
				{
					return length + 4;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 3)))
				{
					IL_01C2:
					return length + 3;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 2)))
				{
					IL_01BE:
					return length + 2;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 1)))
				{
					IL_01BA:
					return length + 1;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length)))
				{
					return length;
				}
			}
			if (length >= 4)
			{
				length -= 4;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 3)))
				{
					goto IL_01C2;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 2)))
				{
					goto IL_01BE;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 1)))
				{
					goto IL_01BA;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length)))
				{
					return length;
				}
			}
			while (length > 0)
			{
				length--;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length)))
				{
					return length;
				}
			}
			return -1;
		}

		public unsafe static int LastIndexOfAny<T>(ref T searchSpace, T value0, T value1, int length) where T : IEquatable<T>
		{
			while (length >= 8)
			{
				length -= 8;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 7);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 7;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 6);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 5);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 4);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02CD:
					return length + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02C9:
					return length + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02C5:
					return length + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length;
				}
			}
			if (length >= 4)
			{
				length -= 4;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02CD;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02C9;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02C5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t))
				{
					return length;
				}
				if (value1.Equals(t))
				{
					return length;
				}
			}
			while (length > 0)
			{
				length--;
				T t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length;
				}
			}
			return -1;
		}

		public unsafe static int LastIndexOfAny<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : IEquatable<T>
		{
			while (length >= 8)
			{
				length -= 8;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 7);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 7;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 6);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 5);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 4);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03DA:
					return length + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03D5:
					return length + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03D0:
					return length + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length;
				}
			}
			if (length >= 4)
			{
				length -= 4;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03DA;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03D5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03D0;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length;
				}
				if (value2.Equals(t))
				{
					return length;
				}
			}
			while (length > 0)
			{
				length--;
				T t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length;
				}
			}
			return -1;
		}

		public unsafe static int LastIndexOfAny<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.LastIndexOf<T>(ref searchSpace, *Unsafe.Add<T>(ref value, i), searchSpaceLength);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		public unsafe static bool SequenceEqual<T>(ref T first, ref T second, int length) where T : IEquatable<T>
		{
			if (!Unsafe.AreSame<T>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)0;
				while (length >= 8)
				{
					length -= 8;
					if (!Unsafe.Add<T>(ref first, intPtr).Equals(*Unsafe.Add<T>(ref second, intPtr)) || !Unsafe.Add<T>(ref first, intPtr + 1).Equals(*Unsafe.Add<T>(ref second, intPtr + 1)) || !Unsafe.Add<T>(ref first, intPtr + 2).Equals(*Unsafe.Add<T>(ref second, intPtr + 2)) || !Unsafe.Add<T>(ref first, intPtr + 3).Equals(*Unsafe.Add<T>(ref second, intPtr + 3)) || !Unsafe.Add<T>(ref first, intPtr + 4).Equals(*Unsafe.Add<T>(ref second, intPtr + 4)) || !Unsafe.Add<T>(ref first, intPtr + 5).Equals(*Unsafe.Add<T>(ref second, intPtr + 5)) || !Unsafe.Add<T>(ref first, intPtr + 6).Equals(*Unsafe.Add<T>(ref second, intPtr + 6)) || !Unsafe.Add<T>(ref first, intPtr + 7).Equals(*Unsafe.Add<T>(ref second, intPtr + 7)))
					{
						return false;
					}
					intPtr += 8;
				}
				if (length >= 4)
				{
					length -= 4;
					if (!Unsafe.Add<T>(ref first, intPtr).Equals(*Unsafe.Add<T>(ref second, intPtr)) || !Unsafe.Add<T>(ref first, intPtr + 1).Equals(*Unsafe.Add<T>(ref second, intPtr + 1)) || !Unsafe.Add<T>(ref first, intPtr + 2).Equals(*Unsafe.Add<T>(ref second, intPtr + 2)) || !Unsafe.Add<T>(ref first, intPtr + 3).Equals(*Unsafe.Add<T>(ref second, intPtr + 3)))
					{
						return false;
					}
					intPtr += 4;
				}
				while (length > 0)
				{
					if (!Unsafe.Add<T>(ref first, intPtr).Equals(*Unsafe.Add<T>(ref second, intPtr)))
					{
						return false;
					}
					intPtr += 1;
					length--;
				}
			}
			return true;
		}

		public unsafe static int SequenceCompareTo<T>(ref T first, int firstLength, ref T second, int secondLength) where T : IComparable<T>
		{
			int num = firstLength;
			if (num > secondLength)
			{
				num = secondLength;
			}
			for (int i = 0; i < num; i++)
			{
				int num2 = Unsafe.Add<T>(ref first, i).CompareTo(*Unsafe.Add<T>(ref second, i));
				if (num2 != 0)
				{
					return num2;
				}
			}
			return firstLength.CompareTo(secondLength);
		}

		public static int IndexOfCultureHelper(ReadOnlySpan<char> span, ReadOnlySpan<char> value, CompareInfo compareInfo)
		{
			if (GlobalizationMode.Invariant)
			{
				return CompareInfo.InvariantIndexOf(span, value, false);
			}
			return compareInfo.IndexOf(span, value, CompareOptions.None);
		}

		public static int IndexOfCultureIgnoreCaseHelper(ReadOnlySpan<char> span, ReadOnlySpan<char> value, CompareInfo compareInfo)
		{
			if (GlobalizationMode.Invariant)
			{
				return CompareInfo.InvariantIndexOf(span, value, true);
			}
			return compareInfo.IndexOf(span, value, CompareOptions.IgnoreCase);
		}

		public static int IndexOfOrdinalHelper(ReadOnlySpan<char> span, ReadOnlySpan<char> value, bool ignoreCase)
		{
			if (GlobalizationMode.Invariant)
			{
				return CompareInfo.InvariantIndexOf(span, value, ignoreCase);
			}
			return CompareInfo.Invariant.IndexOfOrdinal(span, value, ignoreCase);
		}

		public static bool StartsWithCultureHelper(ReadOnlySpan<char> span, ReadOnlySpan<char> value, CompareInfo compareInfo)
		{
			if (GlobalizationMode.Invariant)
			{
				return span.StartsWith<char>(value);
			}
			return span.Length != 0 && compareInfo.IsPrefix(span, value, CompareOptions.None);
		}

		public static bool StartsWithCultureIgnoreCaseHelper(ReadOnlySpan<char> span, ReadOnlySpan<char> value, CompareInfo compareInfo)
		{
			if (GlobalizationMode.Invariant)
			{
				return SpanHelpers.StartsWithOrdinalIgnoreCaseHelper(span, value);
			}
			return span.Length != 0 && compareInfo.IsPrefix(span, value, CompareOptions.IgnoreCase);
		}

		public static bool StartsWithOrdinalIgnoreCaseHelper(ReadOnlySpan<char> span, ReadOnlySpan<char> value)
		{
			return span.Length >= value.Length && CompareInfo.CompareOrdinalIgnoreCase(span.Slice(0, value.Length), value) == 0;
		}

		public static bool EndsWithCultureHelper(ReadOnlySpan<char> span, ReadOnlySpan<char> value, CompareInfo compareInfo)
		{
			if (GlobalizationMode.Invariant)
			{
				return span.EndsWith<char>(value);
			}
			return span.Length != 0 && compareInfo.IsSuffix(span, value, CompareOptions.None);
		}

		public static bool EndsWithCultureIgnoreCaseHelper(ReadOnlySpan<char> span, ReadOnlySpan<char> value, CompareInfo compareInfo)
		{
			if (GlobalizationMode.Invariant)
			{
				return SpanHelpers.EndsWithOrdinalIgnoreCaseHelper(span, value);
			}
			return span.Length != 0 && compareInfo.IsSuffix(span, value, CompareOptions.IgnoreCase);
		}

		public static bool EndsWithOrdinalIgnoreCaseHelper(ReadOnlySpan<char> span, ReadOnlySpan<char> value)
		{
			return span.Length >= value.Length && CompareInfo.CompareOrdinalIgnoreCase(span.Slice(span.Length - value.Length), value) == 0;
		}

		public unsafe static void ClearWithoutReferences(ref byte b, ulong byteLength)
		{
			if (byteLength == 0UL)
			{
				return;
			}
			ulong num = byteLength - 1UL;
			if (num <= 21UL)
			{
				switch ((uint)num)
				{
				case 0U:
					b = 0;
					return;
				case 1U:
					*Unsafe.As<byte, short>(ref b) = 0;
					return;
				case 2U:
					*Unsafe.As<byte, short>(ref b) = 0;
					*Unsafe.Add<byte>(ref b, 2) = 0;
					return;
				case 3U:
					*Unsafe.As<byte, int>(ref b) = 0;
					return;
				case 4U:
					*Unsafe.As<byte, int>(ref b) = 0;
					*Unsafe.Add<byte>(ref b, 4) = 0;
					return;
				case 5U:
					*Unsafe.As<byte, int>(ref b) = 0;
					*Unsafe.As<byte, short>(Unsafe.Add<byte>(ref b, 4)) = 0;
					return;
				case 6U:
					*Unsafe.As<byte, int>(ref b) = 0;
					*Unsafe.As<byte, short>(Unsafe.Add<byte>(ref b, 4)) = 0;
					*Unsafe.Add<byte>(ref b, 6) = 0;
					return;
				case 7U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					return;
				case 8U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.Add<byte>(ref b, 8) = 0;
					return;
				case 9U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, short>(Unsafe.Add<byte>(ref b, 8)) = 0;
					return;
				case 10U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, short>(Unsafe.Add<byte>(ref b, 8)) = 0;
					*Unsafe.Add<byte>(ref b, 10) = 0;
					return;
				case 11U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, 8)) = 0;
					return;
				case 12U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, 8)) = 0;
					*Unsafe.Add<byte>(ref b, 12) = 0;
					return;
				case 13U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, 8)) = 0;
					*Unsafe.As<byte, short>(Unsafe.Add<byte>(ref b, 12)) = 0;
					return;
				case 14U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, 8)) = 0;
					*Unsafe.As<byte, short>(Unsafe.Add<byte>(ref b, 12)) = 0;
					*Unsafe.Add<byte>(ref b, 14) = 0;
					return;
				case 15U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, 8)) = 0L;
					return;
				case 16U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, 8)) = 0L;
					*Unsafe.Add<byte>(ref b, 16) = 0;
					return;
				case 17U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, 8)) = 0L;
					*Unsafe.As<byte, short>(Unsafe.Add<byte>(ref b, 16)) = 0;
					return;
				case 18U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, 8)) = 0L;
					*Unsafe.As<byte, short>(Unsafe.Add<byte>(ref b, 16)) = 0;
					*Unsafe.Add<byte>(ref b, 18) = 0;
					return;
				case 19U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, 8)) = 0L;
					*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, 16)) = 0;
					return;
				case 20U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, 8)) = 0L;
					*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, 16)) = 0;
					*Unsafe.Add<byte>(ref b, 20) = 0;
					return;
				case 21U:
					*Unsafe.As<byte, long>(ref b) = 0L;
					*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, 8)) = 0L;
					*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, 16)) = 0;
					*Unsafe.As<byte, short>(Unsafe.Add<byte>(ref b, 20)) = 0;
					return;
				}
			}
			if (byteLength < 512UL)
			{
				ulong num2 = 0UL;
				if ((*Unsafe.As<byte, int>(ref b) & 3) != 0)
				{
					if ((*Unsafe.As<byte, int>(ref b) & 1) != 0)
					{
						*Unsafe.AddByteOffset<byte>(ref b, num2) = 0;
						num2 += 1UL;
						if ((*Unsafe.As<byte, int>(ref b) & 2) != 0)
						{
							goto IL_0349;
						}
					}
					*Unsafe.As<byte, short>(Unsafe.AddByteOffset<byte>(ref b, num2)) = 0;
					num2 += 2UL;
				}
				IL_0349:
				if (((*Unsafe.As<byte, int>(ref b) - 1) & 4) == 0)
				{
					*Unsafe.As<byte, int>(Unsafe.AddByteOffset<byte>(ref b, num2)) = 0;
					num2 += 4UL;
				}
				ulong num3 = byteLength - 16UL;
				byteLength -= num2;
				ulong num4;
				do
				{
					num4 = num2 + 16UL;
					*Unsafe.As<byte, long>(Unsafe.AddByteOffset<byte>(ref b, num2)) = 0L;
					*Unsafe.As<byte, long>(Unsafe.AddByteOffset<byte>(ref b, num2 + 8UL)) = 0L;
					num2 = num4;
				}
				while (num4 <= num3);
				if ((byteLength & 8UL) != 0UL)
				{
					*Unsafe.As<byte, long>(Unsafe.AddByteOffset<byte>(ref b, num2)) = 0L;
					num2 += 8UL;
				}
				if ((byteLength & 4UL) != 0UL)
				{
					*Unsafe.As<byte, int>(Unsafe.AddByteOffset<byte>(ref b, num2)) = 0;
					num2 += 4UL;
				}
				if ((byteLength & 2UL) != 0UL)
				{
					*Unsafe.As<byte, short>(Unsafe.AddByteOffset<byte>(ref b, num2)) = 0;
					num2 += 2UL;
				}
				if ((byteLength & 1UL) != 0UL)
				{
					*Unsafe.AddByteOffset<byte>(ref b, num2) = 0;
				}
				return;
			}
			RuntimeImports.RhZeroMemory(ref b, byteLength);
		}

		public unsafe static void ClearWithReferences(ref IntPtr ip, ulong pointerSizeLength)
		{
			while (pointerSizeLength >= 8UL)
			{
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -1) = 0;
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -2) = 0;
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -3) = 0;
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -4) = 0;
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -5) = 0;
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -6) = 0;
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -7) = 0;
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -8) = 0;
				pointerSizeLength -= 8UL;
			}
			if (pointerSizeLength < 4UL)
			{
				if (pointerSizeLength < 2UL)
				{
					if (pointerSizeLength <= 0UL)
					{
						return;
					}
					goto IL_015B;
				}
			}
			else
			{
				*Unsafe.Add<IntPtr>(ref ip, 2) = 0;
				*Unsafe.Add<IntPtr>(ref ip, 3) = 0;
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -3) = 0;
				*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -2) = 0;
			}
			*Unsafe.Add<IntPtr>(ref ip, 1) = 0;
			*Unsafe.Add<IntPtr>(Unsafe.Add<IntPtr>(ref ip, (IntPtr)((long)pointerSizeLength)), -1) = 0;
			IL_015B:
			ip = 0;
		}

		public unsafe static void CopyTo<T>(ref T dst, int dstLength, ref T src, int srcLength)
		{
			IntPtr intPtr = Unsafe.ByteOffset<T>(ref src, Unsafe.Add<T>(ref src, srcLength));
			IntPtr intPtr2 = Unsafe.ByteOffset<T>(ref dst, Unsafe.Add<T>(ref dst, dstLength));
			IntPtr intPtr3 = Unsafe.ByteOffset<T>(ref src, ref dst);
			if (!((sizeof(IntPtr) == 4) ? ((int)intPtr3 < (int)intPtr || (int)intPtr3 > -(int)intPtr2) : ((long)intPtr3 < (long)intPtr || (long)intPtr3 > -(long)intPtr2)) && !SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ref byte ptr = ref Unsafe.As<T, byte>(ref dst);
				ref byte ptr2 = ref Unsafe.As<T, byte>(ref src);
				ulong num = (ulong)(long)intPtr;
				uint num3;
				for (ulong num2 = 0UL; num2 < num; num2 += (ulong)num3)
				{
					num3 = ((num - num2 > (ulong)(-1)) ? uint.MaxValue : ((uint)(num - num2)));
					Unsafe.CopyBlock(Unsafe.Add<byte>(ref ptr, (IntPtr)((long)num2)), Unsafe.Add<byte>(ref ptr2, (IntPtr)((long)num2)), num3);
				}
				return;
			}
			bool flag = ((sizeof(IntPtr) == 4) ? ((int)intPtr3 > -(int)intPtr2) : ((long)intPtr3 > -(long)intPtr2));
			int num4 = (flag ? 1 : (-1));
			int num5 = (flag ? 0 : (srcLength - 1));
			int i;
			for (i = 0; i < (srcLength & -8); i += 8)
			{
				*Unsafe.Add<T>(ref dst, num5) = *Unsafe.Add<T>(ref src, num5);
				*Unsafe.Add<T>(ref dst, num5 + num4) = *Unsafe.Add<T>(ref src, num5 + num4);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 2) = *Unsafe.Add<T>(ref src, num5 + num4 * 2);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 3) = *Unsafe.Add<T>(ref src, num5 + num4 * 3);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 4) = *Unsafe.Add<T>(ref src, num5 + num4 * 4);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 5) = *Unsafe.Add<T>(ref src, num5 + num4 * 5);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 6) = *Unsafe.Add<T>(ref src, num5 + num4 * 6);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 7) = *Unsafe.Add<T>(ref src, num5 + num4 * 7);
				num5 += num4 * 8;
			}
			if (i < (srcLength & -4))
			{
				*Unsafe.Add<T>(ref dst, num5) = *Unsafe.Add<T>(ref src, num5);
				*Unsafe.Add<T>(ref dst, num5 + num4) = *Unsafe.Add<T>(ref src, num5 + num4);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 2) = *Unsafe.Add<T>(ref src, num5 + num4 * 2);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 3) = *Unsafe.Add<T>(ref src, num5 + num4 * 3);
				num5 += num4 * 4;
				i += 4;
			}
			while (i < srcLength)
			{
				*Unsafe.Add<T>(ref dst, num5) = *Unsafe.Add<T>(ref src, num5);
				num5 += num4;
				i++;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static IntPtr Add<T>(this IntPtr start, int index)
		{
			if (sizeof(IntPtr) == 4)
			{
				uint num = (uint)(index * Unsafe.SizeOf<T>());
				return (IntPtr)((void*)((byte*)(void*)start + num));
			}
			ulong num2 = (ulong)((long)index * (long)Unsafe.SizeOf<T>());
			return (IntPtr)((void*)((byte*)(void*)start + num2));
		}

		public static bool IsReferenceOrContainsReferences<T>()
		{
			return SpanHelpers.PerTypeValues<T>.IsReferenceOrContainsReferences;
		}

		private static bool IsReferenceOrContainsReferencesCore(Type type)
		{
			if (type.GetTypeInfo().IsPrimitive)
			{
				return false;
			}
			if (!type.GetTypeInfo().IsValueType)
			{
				return true;
			}
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			if (type.GetTypeInfo().IsEnum)
			{
				return false;
			}
			foreach (FieldInfo fieldInfo in type.GetTypeInfo().DeclaredFields)
			{
				if (!fieldInfo.IsStatic && SpanHelpers.IsReferenceOrContainsReferencesCore(fieldInfo.FieldType))
				{
					return true;
				}
			}
			return false;
		}

		public unsafe static void ClearLessThanPointerSized(byte* ptr, UIntPtr byteLength)
		{
			if (sizeof(UIntPtr) == 4)
			{
				Unsafe.InitBlockUnaligned((void*)ptr, 0, (uint)byteLength);
				return;
			}
			ulong num = (ulong)byteLength;
			uint num2 = (uint)(num & (ulong)(-1));
			Unsafe.InitBlockUnaligned((void*)ptr, 0, num2);
			num -= (ulong)num2;
			ptr += num2;
			while (num > 0UL)
			{
				num2 = ((num >= (ulong)(-1)) ? uint.MaxValue : ((uint)num));
				Unsafe.InitBlockUnaligned((void*)ptr, 0, num2);
				ptr += num2;
				num -= (ulong)num2;
			}
		}

		public static void ClearLessThanPointerSized(ref byte b, UIntPtr byteLength)
		{
			if (sizeof(UIntPtr) == 4)
			{
				Unsafe.InitBlockUnaligned(ref b, 0, (uint)byteLength);
				return;
			}
			ulong num = (ulong)byteLength;
			uint num2 = (uint)(num & (ulong)(-1));
			Unsafe.InitBlockUnaligned(ref b, 0, num2);
			num -= (ulong)num2;
			long num3 = (long)((ulong)num2);
			while (num > 0UL)
			{
				num2 = ((num >= (ulong)(-1)) ? uint.MaxValue : ((uint)num));
				Unsafe.InitBlockUnaligned(Unsafe.Add<byte>(ref b, (IntPtr)num3), 0, num2);
				num3 += (long)((ulong)num2);
				num -= (ulong)num2;
			}
		}

		public unsafe static void ClearPointerSizedWithoutReferences(ref byte b, UIntPtr byteLength)
		{
			IntPtr intPtr = IntPtr.Zero;
			while (intPtr.LessThanEqual(byteLength - sizeof(SpanHelpers.Reg64)))
			{
				*Unsafe.As<byte, SpanHelpers.Reg64>(Unsafe.Add<byte>(ref b, intPtr)) = default(SpanHelpers.Reg64);
				intPtr += sizeof(SpanHelpers.Reg64);
			}
			if (intPtr.LessThanEqual(byteLength - sizeof(SpanHelpers.Reg32)))
			{
				*Unsafe.As<byte, SpanHelpers.Reg32>(Unsafe.Add<byte>(ref b, intPtr)) = default(SpanHelpers.Reg32);
				intPtr += sizeof(SpanHelpers.Reg32);
			}
			if (intPtr.LessThanEqual(byteLength - sizeof(SpanHelpers.Reg16)))
			{
				*Unsafe.As<byte, SpanHelpers.Reg16>(Unsafe.Add<byte>(ref b, intPtr)) = default(SpanHelpers.Reg16);
				intPtr += sizeof(SpanHelpers.Reg16);
			}
			if (intPtr.LessThanEqual(byteLength - 8))
			{
				*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, intPtr)) = 0L;
				intPtr += 8;
			}
			if (sizeof(IntPtr) == 4 && intPtr.LessThanEqual(byteLength - 4))
			{
				*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, intPtr)) = 0;
				intPtr += 4;
			}
		}

		public unsafe static void ClearPointerSizedWithReferences(ref IntPtr ip, UIntPtr pointerSizeLength)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			while ((intPtr2 = intPtr + 8).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 0) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 1) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 2) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 3) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 4) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 5) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 6) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 7) = 0;
				intPtr = intPtr2;
			}
			if ((intPtr2 = intPtr + 4).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 0) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 1) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 2) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 3) = 0;
				intPtr = intPtr2;
			}
			if ((intPtr2 = intPtr + 2).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 0) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 1) = 0;
				intPtr = intPtr2;
			}
			if ((intPtr + 1).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr) = 0;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool LessThanEqual(this IntPtr index, UIntPtr length)
		{
			if (sizeof(UIntPtr) != 4)
			{
				return (long)index <= (long)(ulong)length;
			}
			return (int)index <= (int)(uint)length;
		}

		private const ulong XorPowerOfTwoToHighChar = 4295098372UL;

		internal struct ComparerComparable<T, TComparer> : IComparable<T> where TComparer : IComparer<T>
		{
			public ComparerComparable(T value, TComparer comparer)
			{
				this._value = value;
				this._comparer = comparer;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CompareTo(T other)
			{
				TComparer comparer = this._comparer;
				return comparer.Compare(this._value, other);
			}

			private readonly T _value;

			private readonly TComparer _comparer;
		}

		private struct Reg64
		{
		}

		private struct Reg32
		{
		}

		private struct Reg16
		{
		}

		public static class PerTypeValues<T>
		{
			private static IntPtr MeasureArrayAdjustment()
			{
				T[] array = new T[1];
				return Unsafe.ByteOffset<T>(ref Unsafe.As<Pinnable<T>>(array).Data, ref array[0]);
			}

			public static readonly bool IsReferenceOrContainsReferences = SpanHelpers.IsReferenceOrContainsReferencesCore(typeof(T));

			public static readonly T[] EmptyArray = new T[0];

			public static readonly IntPtr ArrayAdjustment = SpanHelpers.PerTypeValues<T>.MeasureArrayAdjustment();
		}
	}
}
