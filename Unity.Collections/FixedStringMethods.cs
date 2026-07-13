using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	[BurstCompatible]
	[BurstCompatible]
	[BurstCompatible]
	public static class FixedStringMethods
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static FormatError Append<T>(this T fs, Unicode.Rune rune) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			int num = rune.LengthInUtf8Bytes();
			if (!fs.TryResize(length + num, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			return (ref fs).Write<T>(ref length, rune);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static FormatError Append<T>(this T fs, char ch) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return (ref fs).Append<T>((Unicode.Rune)ch);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError AppendRawByte<T>(this T fs, byte a) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			if (!fs.TryResize(length + 1, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			fs.GetUnsafePtr()[length] = a;
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this T fs, Unicode.Rune rune, int count) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			if (!fs.TryResize(length + rune.LengthInUtf8Bytes() * count, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			int capacity = fs.Capacity;
			byte* unsafePtr = fs.GetUnsafePtr();
			int num = length;
			for (int i = 0; i < count; i++)
			{
				if (Unicode.UcsToUtf8(unsafePtr, ref num, capacity, rune) != ConversionError.None)
				{
					return FormatError.Overflow;
				}
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this T fs, long input) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* ptr = stackalloc byte[(UIntPtr)20];
			int num = 20;
			if (input >= 0L)
			{
				do
				{
					byte b = (byte)(input % 10L);
					ptr[--num] = 48 + b;
					input /= 10L;
				}
				while (input != 0L);
			}
			else
			{
				do
				{
					byte b2 = (byte)(input % 10L);
					ptr[--num] = 48 - b2;
					input /= 10L;
				}
				while (input != 0L);
				ptr[--num] = 45;
			}
			return (ref fs).Append<T>(ptr + num, 20 - num);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static FormatError Append<T>(this T fs, int input) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return (ref fs).Append<T>((long)input);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this T fs, ulong input) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* ptr = stackalloc byte[(UIntPtr)20];
			int num = 20;
			do
			{
				byte b = (byte)(input % 10UL);
				ptr[--num] = 48 + b;
				input /= 10UL;
			}
			while (input != 0UL);
			return (ref fs).Append<T>(ptr + num, 20 - num);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static FormatError Append<T>(this T fs, uint input) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return (ref fs).Append<T>((ulong)input);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this T fs, float input, char decimalSeparator = '.') where T : struct, INativeList<byte>, IUTF8Bytes
		{
			FixedStringUtils.UintFloatUnion uintFloatUnion = new FixedStringUtils.UintFloatUnion
			{
				floatValue = input
			};
			uint num = uintFloatUnion.uintValue >> 31;
			uintFloatUnion.uintValue &= 2147483647U;
			if ((uintFloatUnion.uintValue & 2139095040U) == 2139095040U)
			{
				if (uintFloatUnion.uintValue != 2139095040U)
				{
					return (ref fs).Append<T>('N', 'a', 'N');
				}
				FormatError formatError;
				if (num != 0U && (formatError = (ref fs).Append<T>('-')) != FormatError.None)
				{
					return formatError;
				}
				return (ref fs).Append<T>('I', 'n', 'f', 'i', 'n', 'i', 't', 'y');
			}
			else
			{
				FormatError formatError;
				if (num != 0U && uintFloatUnion.uintValue != 0U && (formatError = (ref fs).Append<T>('-')) != FormatError.None)
				{
					return formatError;
				}
				ulong num2 = 0UL;
				int num3 = 0;
				FixedStringUtils.Base2ToBase10(ref num2, ref num3, uintFloatUnion.floatValue);
				char* ptr = stackalloc char[(UIntPtr)18];
				int i = 0;
				while (i < 9)
				{
					ulong num4 = num2 % 10UL;
					ptr[(IntPtr)(8 - i++) * 2] = (char)(48UL + num4);
					num2 /= 10UL;
					if (num2 <= 0UL)
					{
						char* ptr2 = ptr + 9 - i;
						int j = -num3 - i + 1;
						if (j > 0)
						{
							if (j > 4)
							{
								return (ref fs).AppendScientific<T>(ptr2, i, num3, decimalSeparator);
							}
							if ((formatError = (ref fs).Append<T>('0', decimalSeparator)) != FormatError.None)
							{
								return formatError;
							}
							for (j--; j > 0; j--)
							{
								if ((formatError = (ref fs).Append<T>('0')) != FormatError.None)
								{
									return formatError;
								}
							}
							for (int k = 0; k < i; k++)
							{
								if ((formatError = (ref fs).Append<T>(ptr2[k])) != FormatError.None)
								{
									return formatError;
								}
							}
							return FormatError.None;
						}
						else
						{
							int l = num3;
							if (l <= 0)
							{
								int num5 = i + num3;
								for (int m = 0; m < i; m++)
								{
									if (m == num5 && (formatError = (ref fs).Append<T>(decimalSeparator)) != FormatError.None)
									{
										return formatError;
									}
									if ((formatError = (ref fs).Append<T>(ptr2[m])) != FormatError.None)
									{
										return formatError;
									}
								}
								return FormatError.None;
							}
							if (l > 4)
							{
								return (ref fs).AppendScientific<T>(ptr2, i, num3, decimalSeparator);
							}
							for (int n = 0; n < i; n++)
							{
								if ((formatError = (ref fs).Append<T>(ptr2[n])) != FormatError.None)
								{
									return formatError;
								}
							}
							while (l > 0)
							{
								if ((formatError = (ref fs).Append<T>('0')) != FormatError.None)
								{
									return formatError;
								}
								l--;
							}
							return FormatError.None;
						}
					}
				}
				return FormatError.Overflow;
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static FormatError Append<T, T2>(this T fs, in T2 input) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(in input);
			return (ref fs).Append<T>(ptr.GetUnsafePtr(), ptr.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static CopyError CopyFrom<T, T2>(this T fs, in T2 input) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			fs.Length = 0;
			if ((ref fs).Append<T, T2>(in input) != FormatError.None)
			{
				return CopyError.Truncation;
			}
			return CopyError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this T fs, byte* utf8Bytes, int utf8BytesLength) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			if (!fs.TryResize(length + utf8BytesLength, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			UnsafeUtility.MemCpy((void*)(fs.GetUnsafePtr() + length), (void*)utf8Bytes, (long)utf8BytesLength);
			return FormatError.None;
		}

		[NotBurstCompatible]
		public unsafe static FormatError Append<T>(this T fs, string s) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = s.Length * 4;
			byte* ptr = stackalloc byte[(UIntPtr)num];
			int num2;
			fixed (string text = s)
			{
				char* ptr2 = text;
				if (ptr2 != null)
				{
					ptr2 += RuntimeHelpers.OffsetToStringData / 2;
				}
				if (UTF8ArrayUnsafeUtility.Copy(ptr, out num2, num, ptr2, s.Length) != CopyError.None)
				{
					return FormatError.Overflow;
				}
			}
			return (ref fs).Append<T>(ptr, num2);
		}

		[NotBurstCompatible]
		public static CopyError CopyFrom<T>(this T fs, string s) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			fs.Length = 0;
			if ((ref fs).Append<T>(s) != FormatError.None)
			{
				return CopyError.Truncation;
			}
			return CopyError.None;
		}

		[NotBurstCompatible]
		public unsafe static void CopyFromTruncated<T>(this T fs, string s) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			fixed (string text = s)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				int num;
				UTF8ArrayUnsafeUtility.Copy(fs.GetUnsafePtr(), out num, fs.Capacity, ptr, s.Length);
				fs.Length = num;
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0>(this T dest, in U format, in T0 arg0) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						if (unsafePtr[i + 1] - 48 == 0)
						{
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
						}
						else
						{
							(ref dest).AppendRawByte<T>(unsafePtr[i]);
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1>(this T dest, in U format, in T0 arg0, in T1 arg1) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						int num = (int)(unsafePtr[i + 1] - 48);
						if (num != 0)
						{
							if (num != 1)
							{
								(ref dest).AppendRawByte<T>(unsafePtr[i]);
							}
							else
							{
								(ref dest).Append<T, T1>(in arg1);
								i += 2;
							}
						}
						else
						{
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
							break;
						case 49:
							(ref dest).Append<T, T1>(in arg1);
							i += 2;
							break;
						case 50:
							(ref dest).Append<T, T2>(in arg2);
							i += 2;
							break;
						default:
							(ref dest).AppendRawByte<T>(unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
							break;
						case 49:
							(ref dest).Append<T, T1>(in arg1);
							i += 2;
							break;
						case 50:
							(ref dest).Append<T, T2>(in arg2);
							i += 2;
							break;
						case 51:
							(ref dest).Append<T, T3>(in arg3);
							i += 2;
							break;
						default:
							(ref dest).AppendRawByte<T>(unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
							break;
						case 49:
							(ref dest).Append<T, T1>(in arg1);
							i += 2;
							break;
						case 50:
							(ref dest).Append<T, T2>(in arg2);
							i += 2;
							break;
						case 51:
							(ref dest).Append<T, T3>(in arg3);
							i += 2;
							break;
						case 52:
							(ref dest).Append<T, T4>(in arg4);
							i += 2;
							break;
						default:
							(ref dest).AppendRawByte<T>(unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
							break;
						case 49:
							(ref dest).Append<T, T1>(in arg1);
							i += 2;
							break;
						case 50:
							(ref dest).Append<T, T2>(in arg2);
							i += 2;
							break;
						case 51:
							(ref dest).Append<T, T3>(in arg3);
							i += 2;
							break;
						case 52:
							(ref dest).Append<T, T4>(in arg4);
							i += 2;
							break;
						case 53:
							(ref dest).Append<T, T5>(in arg5);
							i += 2;
							break;
						default:
							(ref dest).AppendRawByte<T>(unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5, T6>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes where T6 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
							break;
						case 49:
							(ref dest).Append<T, T1>(in arg1);
							i += 2;
							break;
						case 50:
							(ref dest).Append<T, T2>(in arg2);
							i += 2;
							break;
						case 51:
							(ref dest).Append<T, T3>(in arg3);
							i += 2;
							break;
						case 52:
							(ref dest).Append<T, T4>(in arg4);
							i += 2;
							break;
						case 53:
							(ref dest).Append<T, T5>(in arg5);
							i += 2;
							break;
						case 54:
							(ref dest).Append<T, T6>(in arg6);
							i += 2;
							break;
						default:
							(ref dest).AppendRawByte<T>(unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5, T6, T7>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6, in T7 arg7) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes where T6 : struct, INativeList<byte>, IUTF8Bytes where T7 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
							break;
						case 49:
							(ref dest).Append<T, T1>(in arg1);
							i += 2;
							break;
						case 50:
							(ref dest).Append<T, T2>(in arg2);
							i += 2;
							break;
						case 51:
							(ref dest).Append<T, T3>(in arg3);
							i += 2;
							break;
						case 52:
							(ref dest).Append<T, T4>(in arg4);
							i += 2;
							break;
						case 53:
							(ref dest).Append<T, T5>(in arg5);
							i += 2;
							break;
						case 54:
							(ref dest).Append<T, T6>(in arg6);
							i += 2;
							break;
						case 55:
							(ref dest).Append<T, T7>(in arg7);
							i += 2;
							break;
						default:
							(ref dest).AppendRawByte<T>(unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6, in T7 arg7, in T8 arg8) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes where T6 : struct, INativeList<byte>, IUTF8Bytes where T7 : struct, INativeList<byte>, IUTF8Bytes where T8 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
							break;
						case 49:
							(ref dest).Append<T, T1>(in arg1);
							i += 2;
							break;
						case 50:
							(ref dest).Append<T, T2>(in arg2);
							i += 2;
							break;
						case 51:
							(ref dest).Append<T, T3>(in arg3);
							i += 2;
							break;
						case 52:
							(ref dest).Append<T, T4>(in arg4);
							i += 2;
							break;
						case 53:
							(ref dest).Append<T, T5>(in arg5);
							i += 2;
							break;
						case 54:
							(ref dest).Append<T, T6>(in arg6);
							i += 2;
							break;
						case 55:
							(ref dest).Append<T, T7>(in arg7);
							i += 2;
							break;
						case 56:
							(ref dest).Append<T, T8>(in arg8);
							i += 2;
							break;
						default:
							(ref dest).AppendRawByte<T>(unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6, in T7 arg7, in T8 arg8, in T9 arg9) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes where T6 : struct, INativeList<byte>, IUTF8Bytes where T7 : struct, INativeList<byte>, IUTF8Bytes where T8 : struct, INativeList<byte>, IUTF8Bytes where T9 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U ptr = ref UnsafeUtilityExtensions.AsRef<U>(in format);
			int length = ptr.Length;
			byte* unsafePtr = ptr.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							(ref dest).Append<T, T0>(in arg0);
							i += 2;
							break;
						case 49:
							(ref dest).Append<T, T1>(in arg1);
							i += 2;
							break;
						case 50:
							(ref dest).Append<T, T2>(in arg2);
							i += 2;
							break;
						case 51:
							(ref dest).Append<T, T3>(in arg3);
							i += 2;
							break;
						case 52:
							(ref dest).Append<T, T4>(in arg4);
							i += 2;
							break;
						case 53:
							(ref dest).Append<T, T5>(in arg5);
							i += 2;
							break;
						case 54:
							(ref dest).Append<T, T6>(in arg6);
							i += 2;
							break;
						case 55:
							(ref dest).Append<T, T7>(in arg7);
							i += 2;
							break;
						case 56:
							(ref dest).Append<T, T8>(in arg8);
							i += 2;
							break;
						case 57:
							(ref dest).Append<T, T9>(in arg9);
							i += 2;
							break;
						default:
							(ref dest).AppendRawByte<T>(unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					(ref dest).AppendRawByte<T>(unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static FormatError Append<T>(this T fs, char a, char b) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if ((FormatError.None | (ref fs).Append<T>((Unicode.Rune)a) | (ref fs).Append<T>((Unicode.Rune)b)) != FormatError.None)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static FormatError Append<T>(this T fs, char a, char b, char c) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if ((FormatError.None | (ref fs).Append<T>((Unicode.Rune)a) | (ref fs).Append<T>((Unicode.Rune)b) | (ref fs).Append<T>((Unicode.Rune)c)) != FormatError.None)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static FormatError Append<T>(this T fs, char a, char b, char c, char d, char e, char f, char g, char h) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if ((FormatError.None | (ref fs).Append<T>((Unicode.Rune)a) | (ref fs).Append<T>((Unicode.Rune)b) | (ref fs).Append<T>((Unicode.Rune)c) | (ref fs).Append<T>((Unicode.Rune)d) | (ref fs).Append<T>((Unicode.Rune)e) | (ref fs).Append<T>((Unicode.Rune)f) | (ref fs).Append<T>((Unicode.Rune)g) | (ref fs).Append<T>((Unicode.Rune)h)) != FormatError.None)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal unsafe static FormatError AppendScientific<T>(this T fs, char* source, int sourceLength, int decimalExponent, char decimalSeparator = '.') where T : struct, INativeList<byte>, IUTF8Bytes
		{
			FormatError formatError;
			if ((formatError = (ref fs).Append<T>(*source)) != FormatError.None)
			{
				return formatError;
			}
			if (sourceLength > 1)
			{
				if ((formatError = (ref fs).Append<T>(decimalSeparator)) != FormatError.None)
				{
					return formatError;
				}
				for (int i = 1; i < sourceLength; i++)
				{
					if ((formatError = (ref fs).Append<T>(source[i])) != FormatError.None)
					{
						return formatError;
					}
				}
			}
			if ((formatError = (ref fs).Append<T>('E')) != FormatError.None)
			{
				return formatError;
			}
			if (decimalExponent < 0)
			{
				if ((formatError = (ref fs).Append<T>('-')) != FormatError.None)
				{
					return formatError;
				}
				decimalExponent *= -1;
				decimalExponent -= sourceLength - 1;
			}
			else
			{
				if ((formatError = (ref fs).Append<T>('+')) != FormatError.None)
				{
					return formatError;
				}
				decimalExponent += sourceLength - 1;
			}
			char* ptr = stackalloc char[(UIntPtr)4];
			for (int j = 0; j < 2; j++)
			{
				int num = decimalExponent % 10;
				ptr[1 - j] = (char)(48 + num);
				decimalExponent /= 10;
			}
			for (int k = 0; k < 2; k++)
			{
				if ((formatError = (ref fs).Append<T>(ptr[k])) != FormatError.None)
				{
					return formatError;
				}
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static bool Found<T>(this T fs, ref int offset, char a, char b, char c) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			if (((ref fs).Read<T>(ref offset).value | 32) == (int)a && ((ref fs).Read<T>(ref offset).value | 32) == (int)b && ((ref fs).Read<T>(ref offset).value | 32) == (int)c)
			{
				return true;
			}
			offset = num;
			return false;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static bool Found<T>(this T fs, ref int offset, char a, char b, char c, char d, char e, char f, char g, char h) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			if (((ref fs).Read<T>(ref offset).value | 32) == (int)a && ((ref fs).Read<T>(ref offset).value | 32) == (int)b && ((ref fs).Read<T>(ref offset).value | 32) == (int)c && ((ref fs).Read<T>(ref offset).value | 32) == (int)d && ((ref fs).Read<T>(ref offset).value | 32) == (int)e && ((ref fs).Read<T>(ref offset).value | 32) == (int)f && ((ref fs).Read<T>(ref offset).value | 32) == (int)g && ((ref fs).Read<T>(ref offset).value | 32) == (int)h)
			{
				return true;
			}
			offset = num;
			return false;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int IndexOf<T>(this T fs, byte* bytes, int bytesLen) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int length = fs.Length;
			int i = 0;
			IL_003C:
			while (i <= length - bytesLen)
			{
				for (int j = 0; j < bytesLen; j++)
				{
					if (unsafePtr[i + j] != bytes[j])
					{
						i++;
						goto IL_003C;
					}
				}
				return i;
			}
			return -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int IndexOf<T>(this T fs, byte* bytes, int bytesLen, int startIndex, int distance = 2147483647) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int length = fs.Length;
			int num = Math.Min(distance - 1, length - bytesLen);
			int i = startIndex;
			IL_004F:
			while (i <= num)
			{
				for (int j = 0; j < bytesLen; j++)
				{
					if (unsafePtr[i + j] != bytes[j])
					{
						i++;
						goto IL_004F;
					}
				}
				return i;
			}
			return -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int IndexOf<T, T2>(this T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(in other);
			return (ref fs).IndexOf<T>(ptr.GetUnsafePtr(), ptr.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int IndexOf<T, T2>(this T fs, in T2 other, int startIndex, int distance = 2147483647) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(in other);
			return (ref fs).IndexOf<T>(ptr.GetUnsafePtr(), ptr.Length, startIndex, distance);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static bool Contains<T, T2>(this T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			return (ref fs).IndexOf<T, T2>(in other) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int LastIndexOf<T>(this T fs, byte* bytes, int bytesLen) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int i = fs.Length - bytesLen;
			IL_003C:
			while (i >= 0)
			{
				for (int j = 0; j < bytesLen; j++)
				{
					if (unsafePtr[i + j] != bytes[j])
					{
						i--;
						goto IL_003C;
					}
				}
				return i;
			}
			return -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int LastIndexOf<T>(this T fs, byte* bytes, int bytesLen, int startIndex, int distance = 2147483647) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			startIndex = Math.Min(fs.Length - bytesLen, startIndex);
			int num = Math.Max(0, startIndex - distance);
			int i = startIndex;
			IL_0050:
			while (i >= num)
			{
				for (int j = 0; j < bytesLen; j++)
				{
					if (unsafePtr[i + j] != bytes[j])
					{
						i--;
						goto IL_0050;
					}
				}
				return i;
			}
			return -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int LastIndexOf<T, T2>(this T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(in other);
			return (ref fs).LastIndexOf<T>(ptr.GetUnsafePtr(), ptr.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int LastIndexOf<T, T2>(this T fs, in T2 other, int startIndex, int distance = 2147483647) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(in other);
			return (ref fs).LastIndexOf<T>(ptr.GetUnsafePtr(), ptr.Length, startIndex, distance);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int CompareTo<T>(this T fs, byte* bytes, int bytesLen) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int length = fs.Length;
			int num = ((length < bytesLen) ? length : bytesLen);
			for (int i = 0; i < num; i++)
			{
				if (unsafePtr[i] < bytes[i])
				{
					return -1;
				}
				if (unsafePtr[i] > bytes[i])
				{
					return 1;
				}
			}
			if (length < bytesLen)
			{
				return -1;
			}
			if (length > bytesLen)
			{
				return 1;
			}
			return 0;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static int CompareTo<T, T2>(this T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(in other);
			return (ref fs).CompareTo<T>(ptr.GetUnsafePtr(), ptr.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static bool Equals<T>(this T fs, byte* bytes, int bytesLen) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			return fs.Length == bytesLen && (unsafePtr == bytes || (ref fs).CompareTo<T>(bytes, bytesLen) == 0);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static bool Equals<T, T2>(this T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 ptr = ref UnsafeUtilityExtensions.AsRef<T2>(in other);
			return (ref fs).Equals<T>(ptr.GetUnsafePtr(), ptr.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static Unicode.Rune Peek<T>(this T fs, int index) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if (index >= fs.Length)
			{
				return Unicode.BadRune;
			}
			Unicode.Rune rune;
			Unicode.Utf8ToUcs(out rune, fs.GetUnsafePtr(), ref index, fs.Capacity);
			return rune;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static Unicode.Rune Read<T>(this T fs, ref int index) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if (index >= fs.Length)
			{
				return Unicode.BadRune;
			}
			Unicode.Rune rune;
			Unicode.Utf8ToUcs(out rune, fs.GetUnsafePtr(), ref index, fs.Capacity);
			return rune;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static FormatError Write<T>(this T fs, ref int index, Unicode.Rune rune) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if (Unicode.UcsToUtf8(fs.GetUnsafePtr(), ref index, fs.Capacity, rune) != ConversionError.None)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[NotBurstCompatible]
		public unsafe static string ConvertToString<T>(this T fs) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			char* ptr;
			int num;
			checked
			{
				ptr = stackalloc char[unchecked((UIntPtr)(fs.Length * 2)) * 2];
				num = 0;
			}
			Unicode.Utf8ToUtf16(fs.GetUnsafePtr(), fs.Length, ptr, out num, fs.Length * 2);
			return new string(ptr, 0, num);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int ComputeHashCode<T>(this T fs) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return (int)CollectionHelper.Hash((void*)fs.GetUnsafePtr(), fs.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static int EffectiveSizeOf<T>(this T fs) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return 2 + fs.Length + 1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool ParseLongInternal<T>(ref T fs, ref int offset, out long value) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			int num2 = 1;
			if (offset < fs.Length)
			{
				if ((ref fs).Peek<T>(offset).value == 43)
				{
					(ref fs).Read<T>(ref offset);
				}
				else if ((ref fs).Peek<T>(offset).value == 45)
				{
					num2 = -1;
					(ref fs).Read<T>(ref offset);
				}
			}
			int num3 = offset;
			value = 0L;
			while (offset < fs.Length && Unicode.Rune.IsDigit((ref fs).Peek<T>(offset)))
			{
				value *= 10L;
				value += (long)((ref fs).Read<T>(ref offset).value - 48);
			}
			value = (long)num2 * value;
			if (offset == num3)
			{
				offset = num;
				return false;
			}
			return true;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static ParseError Parse<T>(this T fs, ref int offset, ref int output) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			long num;
			if (!FixedStringMethods.ParseLongInternal<T>(ref fs, ref offset, out num))
			{
				return ParseError.Syntax;
			}
			if (num > 2147483647L)
			{
				return ParseError.Overflow;
			}
			if (num < -2147483648L)
			{
				return ParseError.Overflow;
			}
			output = (int)num;
			return ParseError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static ParseError Parse<T>(this T fs, ref int offset, ref uint output) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			long num;
			if (!FixedStringMethods.ParseLongInternal<T>(ref fs, ref offset, out num))
			{
				return ParseError.Syntax;
			}
			if (num > (long)((ulong)(-1)))
			{
				return ParseError.Overflow;
			}
			if (num < 0L)
			{
				return ParseError.Overflow;
			}
			output = (uint)num;
			return ParseError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static ParseError Parse<T>(this T fs, ref int offset, ref float output, char decimalSeparator = '.') where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			int num2 = 1;
			if (offset < fs.Length)
			{
				if ((ref fs).Peek<T>(offset).value == 43)
				{
					(ref fs).Read<T>(ref offset);
				}
				else if ((ref fs).Peek<T>(offset).value == 45)
				{
					num2 = -1;
					(ref fs).Read<T>(ref offset);
				}
			}
			if ((ref fs).Found<T>(ref offset, 'n', 'a', 'n'))
			{
				output = new FixedStringUtils.UintFloatUnion
				{
					uintValue = 4290772992U
				}.floatValue;
				return ParseError.None;
			}
			if ((ref fs).Found<T>(ref offset, 'i', 'n', 'f', 'i', 'n', 'i', 't', 'y'))
			{
				output = ((num2 == 1) ? float.PositiveInfinity : float.NegativeInfinity);
				return ParseError.None;
			}
			ulong num3 = 0UL;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			while (offset < fs.Length && Unicode.Rune.IsDigit((ref fs).Peek<T>(offset)))
			{
				num6++;
				if (num4 < 9)
				{
					ulong num7 = num3 * 10UL + (ulong)((long)((ref fs).Peek<T>(offset).value - 48));
					if (num7 > num3)
					{
						num4++;
					}
					num3 = num7;
				}
				else
				{
					num5--;
				}
				(ref fs).Read<T>(ref offset);
			}
			if (offset < fs.Length && (ref fs).Peek<T>(offset).value == (int)decimalSeparator)
			{
				(ref fs).Read<T>(ref offset);
				while (offset < fs.Length && Unicode.Rune.IsDigit((ref fs).Peek<T>(offset)))
				{
					num6++;
					if (num4 < 9)
					{
						ulong num8 = num3 * 10UL + (ulong)((long)((ref fs).Peek<T>(offset).value - 48));
						if (num8 > num3)
						{
							num4++;
						}
						num3 = num8;
						num5++;
					}
					(ref fs).Read<T>(ref offset);
				}
			}
			if (num6 == 0)
			{
				offset = num;
				return ParseError.Syntax;
			}
			int num9 = 0;
			int num10 = 1;
			if (offset < fs.Length && ((ref fs).Peek<T>(offset).value | 32) == 101)
			{
				(ref fs).Read<T>(ref offset);
				if (offset < fs.Length)
				{
					if ((ref fs).Peek<T>(offset).value == 43)
					{
						(ref fs).Read<T>(ref offset);
					}
					else if ((ref fs).Peek<T>(offset).value == 45)
					{
						num10 = -1;
						(ref fs).Read<T>(ref offset);
					}
				}
				int num11 = offset;
				while (offset < fs.Length && Unicode.Rune.IsDigit((ref fs).Peek<T>(offset)))
				{
					num9 = num9 * 10 + ((ref fs).Peek<T>(offset).value - 48);
					(ref fs).Read<T>(ref offset);
				}
				if (offset == num11)
				{
					offset = num;
					return ParseError.Syntax;
				}
				if (num9 > 38)
				{
					if (num10 == 1)
					{
						return ParseError.Overflow;
					}
					return ParseError.Underflow;
				}
			}
			num9 = num9 * num10 - num5;
			ParseError parseError = FixedStringUtils.Base10ToBase2(ref output, num3, num9);
			if (parseError != ParseError.None)
			{
				return parseError;
			}
			output *= (float)num2;
			return ParseError.None;
		}
	}
}
