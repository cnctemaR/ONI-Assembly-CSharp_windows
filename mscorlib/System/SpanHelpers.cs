using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class SpanHelpers
	{
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

		public static int IndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : struct, IEquatable<T>
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

		public unsafe static int IndexOf<T>(ref T searchSpace, T value, int length) where T : struct, IEquatable<T>
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

		public unsafe static bool SequenceEqual<T>(ref T first, ref T second, int length) where T : struct, IEquatable<T>
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
				if (SpanHelpers.SequenceEqual(Unsafe.Add<byte>(ref searchSpace, num2 + 1), ref ptr, num))
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
				if (num2 != -1)
				{
					num = ((num == -1 || num > num2) ? num2 : num);
				}
			}
			return num;
		}

		public unsafe static int IndexOf(ref byte searchSpace, byte value, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)((long)((ulong)length));
			while ((void*)intPtr2 >= 8)
			{
				intPtr2 -= 8;
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr))
				{
					IL_014E:
					return (void*)intPtr;
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 1))
				{
					IL_0156:
					return (void*)(intPtr + 1);
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 2))
				{
					IL_0164:
					return (void*)(intPtr + 2);
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 3))
				{
					IL_0172:
					return (void*)(intPtr + 3);
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 4))
				{
					return (void*)(intPtr + 4);
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 5))
				{
					return (void*)(intPtr + 5);
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 6))
				{
					return (void*)(intPtr + 6);
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 7))
				{
					return (void*)(intPtr + 7);
				}
				intPtr += 8;
			}
			if ((void*)intPtr2 >= 4)
			{
				intPtr2 -= 4;
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr))
				{
					goto IL_014E;
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 1))
				{
					goto IL_0156;
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 2))
				{
					goto IL_0164;
				}
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr + 3))
				{
					goto IL_0172;
				}
				intPtr += 4;
			}
			while ((void*)intPtr2 != null)
			{
				intPtr2 -= 1;
				if (value == *Unsafe.Add<byte>(ref searchSpace, intPtr))
				{
					goto IL_014E;
				}
				intPtr += 1;
			}
			return -1;
		}

		public unsafe static int IndexOfAny(ref byte searchSpace, byte value0, byte value1, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)((long)((ulong)length));
			while ((void*)intPtr2 >= 8)
			{
				intPtr2 -= 8;
				uint num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01E6:
					return (void*)intPtr;
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01EE:
					return (void*)(intPtr + 1);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01FC:
					return (void*)(intPtr + 2);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_020A:
					return (void*)(intPtr + 3);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 4));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 4);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 5));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 5);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 6));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 6);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 7));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (void*)(intPtr + 7);
				}
				intPtr += 8;
			}
			if ((void*)intPtr2 >= 4)
			{
				intPtr2 -= 4;
				uint num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01E6;
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01EE;
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01FC;
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_020A;
				}
				intPtr += 4;
			}
			while ((void*)intPtr2 != null)
			{
				intPtr2 -= 1;
				uint num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01E6;
				}
				intPtr += 1;
			}
			return -1;
		}

		public unsafe static int IndexOfAny(ref byte searchSpace, byte value0, byte value1, byte value2, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)((long)((ulong)length));
			while ((void*)intPtr2 >= 8)
			{
				intPtr2 -= 8;
				uint num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_025B:
					return (void*)intPtr;
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0263:
					return (void*)(intPtr + 1);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0271:
					return (void*)(intPtr + 2);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_027F:
					return (void*)(intPtr + 3);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 4));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 4);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 5));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 5);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 6));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 6);
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 7));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (void*)(intPtr + 7);
				}
				intPtr += 8;
			}
			if ((void*)intPtr2 >= 4)
			{
				intPtr2 -= 4;
				uint num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_025B;
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0263;
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0271;
				}
				num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr + 3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_027F;
				}
				intPtr += 4;
			}
			while ((void*)intPtr2 != null)
			{
				intPtr2 -= 1;
				uint num = (uint)(*Unsafe.Add<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_025B;
				}
				intPtr += 1;
			}
			return -1;
		}

		public unsafe static bool SequenceEqual(ref byte first, ref byte second, int length)
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
