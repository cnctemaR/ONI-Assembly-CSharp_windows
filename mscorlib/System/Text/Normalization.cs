using System;
using System.Runtime.CompilerServices;
using Mono.Globalization.Unicode;

namespace System.Text
{
	internal class Normalization
	{
		private unsafe static uint PropValue(int cp)
		{
			return (uint)Normalization.props[NormalizationTableUtil.PropIdx(cp)];
		}

		private unsafe static int CharMapIdx(int cp)
		{
			return (int)Normalization.charMapIndex[NormalizationTableUtil.MapIdx(cp)];
		}

		private unsafe static byte GetCombiningClass(int c)
		{
			return Normalization.combiningClass[NormalizationTableUtil.Combining.ToIndex(c)];
		}

		private unsafe static int GetPrimaryCompositeFromMapIndex(int src)
		{
			return (int)Normalization.mapIdxToComposite[NormalizationTableUtil.Composite.ToIndex(src)];
		}

		private unsafe static int GetPrimaryCompositeHelperIndex(int cp)
		{
			return (int)Normalization.helperIndex[NormalizationTableUtil.Helper.ToIndex(cp)];
		}

		private static string Compose(string source, int checkType)
		{
			StringBuilder stringBuilder = null;
			Normalization.Decompose(source, ref stringBuilder, (checkType == 2) ? 3 : 1);
			if (stringBuilder == null)
			{
				stringBuilder = Normalization.Combine(source, 0, checkType);
			}
			else
			{
				Normalization.Combine(stringBuilder, 0, checkType);
			}
			if (stringBuilder == null)
			{
				return source;
			}
			return stringBuilder.ToString();
		}

		private static StringBuilder Combine(string source, int start, int checkType)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (Normalization.QuickCheck(source[i], checkType) != NormalizationCheck.Yes)
				{
					StringBuilder stringBuilder = new StringBuilder(source.Length + source.Length / 10);
					stringBuilder.Append(source);
					Normalization.Combine(stringBuilder, i, checkType);
					return stringBuilder;
				}
			}
			return null;
		}

		private static void Combine(StringBuilder sb, int i, int checkType)
		{
			Normalization.CombineHangul(sb, null, (i > 0) ? (i - 1) : i);
			while (i < sb.Length)
			{
				if (Normalization.QuickCheck(sb[i], checkType) == NormalizationCheck.Yes)
				{
					i++;
				}
				else
				{
					i = Normalization.TryComposeWithPreviousStarter(sb, null, i);
				}
			}
		}

		private static int CombineHangul(StringBuilder sb, string s, int current)
		{
			int num = ((sb != null) ? sb.Length : s.Length);
			int num2 = Normalization.Fetch(sb, s, current);
			int i = current + 1;
			while (i < num)
			{
				int num3 = Normalization.Fetch(sb, s, i);
				int num4 = num2 - 4352;
				if (0 > num4 || num4 >= 19)
				{
					goto IL_008A;
				}
				int num5 = num3 - 4449;
				if (0 > num5 || num5 >= 21)
				{
					goto IL_008A;
				}
				if (sb == null)
				{
					return -1;
				}
				num2 = 44032 + (num4 * 21 + num5) * 28;
				sb[i - 1] = (char)num2;
				sb.Remove(i, 1);
				i--;
				num--;
				IL_00E6:
				i++;
				continue;
				IL_008A:
				int num6 = num2 - 44032;
				if (0 <= num6 && num6 < 11172 && num6 % 28 == 0)
				{
					int num7 = num3 - 4519;
					if (0 < num7 && num7 < 28)
					{
						if (sb == null)
						{
							return -1;
						}
						num2 += num7;
						sb[i - 1] = (char)num2;
						sb.Remove(i, 1);
						i--;
						num--;
						goto IL_00E6;
					}
				}
				num2 = num3;
				goto IL_00E6;
			}
			return num;
		}

		private static int Fetch(StringBuilder sb, string s, int i)
		{
			if (sb == null)
			{
				return (int)s[i];
			}
			return (int)sb[i];
		}

		private static int TryComposeWithPreviousStarter(StringBuilder sb, string s, int current)
		{
			int num = current - 1;
			if (Normalization.GetCombiningClass(Normalization.Fetch(sb, s, current)) == 0)
			{
				if (num < 0 || Normalization.GetCombiningClass(Normalization.Fetch(sb, s, num)) != 0)
				{
					return current + 1;
				}
			}
			else
			{
				while (num >= 0 && Normalization.GetCombiningClass(Normalization.Fetch(sb, s, num)) != 0)
				{
					num--;
				}
				if (num < 0)
				{
					return current + 1;
				}
			}
			int num2 = Normalization.Fetch(sb, s, num);
			int primaryCompositeHelperIndex = Normalization.GetPrimaryCompositeHelperIndex(num2);
			if (primaryCompositeHelperIndex == 0)
			{
				return current + 1;
			}
			int num3 = ((sb != null) ? sb.Length : s.Length);
			int num4 = -1;
			for (int i = num + 1; i < num3; i++)
			{
				int num5 = Normalization.Fetch(sb, s, i);
				int num6 = (int)Normalization.GetCombiningClass(num5);
				if (num6 != num4)
				{
					int num7 = Normalization.TryCompose(primaryCompositeHelperIndex, num2, num5);
					if (num7 != 0)
					{
						if (sb == null)
						{
							return -1;
						}
						sb[num] = (char)num7;
						sb.Remove(i, 1);
						return current;
					}
					else
					{
						if (num6 == 0)
						{
							return i + 1;
						}
						num4 = num6;
					}
				}
			}
			return num3;
		}

		private unsafe static int TryCompose(int i, int starter, int candidate)
		{
			while (Normalization.mappedChars[i] == starter)
			{
				if (Normalization.mappedChars[i + 1] == candidate && Normalization.mappedChars[i + 2] == 0)
				{
					int primaryCompositeFromMapIndex = Normalization.GetPrimaryCompositeFromMapIndex(i);
					if ((Normalization.PropValue(primaryCompositeFromMapIndex) & 64U) == 0U)
					{
						return primaryCompositeFromMapIndex;
					}
				}
				while (Normalization.mappedChars[i] != 0)
				{
					i++;
				}
				i++;
			}
			return 0;
		}

		private static string Decompose(string source, int checkType)
		{
			StringBuilder stringBuilder = null;
			Normalization.Decompose(source, ref stringBuilder, checkType);
			if (stringBuilder == null)
			{
				return source;
			}
			return stringBuilder.ToString();
		}

		private static void Decompose(string source, ref StringBuilder sb, int checkType)
		{
			int[] array = null;
			int num = 0;
			for (int i = 0; i < source.Length; i++)
			{
				if (Normalization.QuickCheck(source[i], checkType) == NormalizationCheck.No)
				{
					Normalization.DecomposeChar(ref sb, ref array, source, i, checkType, ref num);
				}
			}
			if (sb != null)
			{
				sb.Append(source, num, source.Length - num);
			}
			Normalization.ReorderCanonical(source, ref sb, 1);
		}

		private static void ReorderCanonical(string src, ref StringBuilder sb, int start)
		{
			if (sb == null)
			{
				for (int i = 1; i < src.Length; i++)
				{
					int num = (int)Normalization.GetCombiningClass((int)src[i]);
					if (num != 0 && (int)Normalization.GetCombiningClass((int)src[i - 1]) > num)
					{
						sb = new StringBuilder(src.Length);
						sb.Append(src, 0, src.Length);
						Normalization.ReorderCanonical(src, ref sb, i);
						return;
					}
				}
				return;
			}
			int j = start;
			while (j < sb.Length)
			{
				int num2 = (int)Normalization.GetCombiningClass((int)sb[j]);
				if (num2 == 0 || (int)Normalization.GetCombiningClass((int)sb[j - 1]) <= num2)
				{
					j++;
				}
				else
				{
					char c = sb[j - 1];
					sb[j - 1] = sb[j];
					sb[j] = c;
					if (j > 1)
					{
						j--;
					}
				}
			}
		}

		private static void DecomposeChar(ref StringBuilder sb, ref int[] buf, string s, int i, int checkType, ref int start)
		{
			if (sb == null)
			{
				sb = new StringBuilder(s.Length + 100);
			}
			sb.Append(s, start, i - start);
			if (buf == null)
			{
				buf = new int[19];
			}
			int canonical = Normalization.GetCanonical((int)s[i], buf, 0, checkType);
			for (int j = 0; j < canonical; j++)
			{
				if (buf[j] < 65535)
				{
					sb.Append((char)buf[j]);
				}
				else
				{
					sb.Append((char)(buf[j] >> 10));
					sb.Append((char)((buf[j] & 4095) + 56320));
				}
			}
			start = i + 1;
		}

		public static NormalizationCheck QuickCheck(char c, int type)
		{
			switch (type)
			{
			case 1:
				if ('가' <= c && c <= '힣')
				{
					return NormalizationCheck.No;
				}
				if ((Normalization.PropValue((int)c) & 1U) == 0U)
				{
					return NormalizationCheck.Yes;
				}
				return NormalizationCheck.No;
			case 2:
			{
				uint num = Normalization.PropValue((int)c);
				if ((num & 16U) != 0U)
				{
					return NormalizationCheck.No;
				}
				if ((num & 32U) == 0U)
				{
					return NormalizationCheck.Yes;
				}
				return NormalizationCheck.Maybe;
			}
			case 3:
				if ('가' <= c && c <= '힣')
				{
					return NormalizationCheck.No;
				}
				if ((Normalization.PropValue((int)c) & 2U) == 0U)
				{
					return NormalizationCheck.Yes;
				}
				return NormalizationCheck.No;
			default:
			{
				uint num = Normalization.PropValue((int)c);
				if ((num & 4U) != 0U)
				{
					return NormalizationCheck.No;
				}
				if ((num & 8U) != 0U)
				{
					return NormalizationCheck.Maybe;
				}
				return NormalizationCheck.Yes;
			}
			}
		}

		private static int GetCanonicalHangul(int s, int[] buf, int bufIdx)
		{
			int num = s - 44032;
			if (num < 0 || num >= 11172)
			{
				return bufIdx;
			}
			int num2 = 4352 + num / 588;
			int num3 = 4449 + num % 588 / 28;
			int num4 = 4519 + num % 28;
			buf[bufIdx++] = num2;
			buf[bufIdx++] = num3;
			if (num4 != 4519)
			{
				buf[bufIdx++] = num4;
			}
			buf[bufIdx] = 0;
			return bufIdx;
		}

		private unsafe static int GetCanonical(int c, int[] buf, int bufIdx, int checkType)
		{
			int canonicalHangul = Normalization.GetCanonicalHangul(c, buf, bufIdx);
			if (canonicalHangul > bufIdx)
			{
				return canonicalHangul;
			}
			int num = Normalization.CharMapIdx(c);
			if (num == 0 || Normalization.mappedChars[num] == c)
			{
				buf[bufIdx++] = c;
			}
			else
			{
				while (Normalization.mappedChars[num] != 0)
				{
					int num2 = Normalization.mappedChars[num];
					if (num2 <= 65535 && Normalization.QuickCheck((char)num2, checkType) == NormalizationCheck.Yes)
					{
						buf[bufIdx++] = num2;
					}
					else
					{
						bufIdx = Normalization.GetCanonical(num2, buf, bufIdx, checkType);
					}
					num++;
				}
			}
			return bufIdx;
		}

		public static bool IsNormalized(string source, NormalizationForm normalizationForm)
		{
			switch (normalizationForm)
			{
			case NormalizationForm.FormD:
				return Normalization.IsNormalized(source, 1);
			default:
				return Normalization.IsNormalized(source, 0);
			case NormalizationForm.FormKC:
				return Normalization.IsNormalized(source, 2);
			case NormalizationForm.FormKD:
				return Normalization.IsNormalized(source, 3);
			}
		}

		public static bool IsNormalized(string source, int type)
		{
			int num = -1;
			int i = 0;
			while (i < source.Length)
			{
				int num2 = (int)Normalization.GetCombiningClass((int)source[i]);
				if (num2 != 0 && num2 < num)
				{
					return false;
				}
				num = num2;
				switch (Normalization.QuickCheck(source[i], type))
				{
				case NormalizationCheck.Yes:
					i++;
					break;
				case NormalizationCheck.No:
					return false;
				case NormalizationCheck.Maybe:
					if (type == 0 || type == 2)
					{
						return source == Normalization.Normalize(source, type);
					}
					i = Normalization.CombineHangul(null, source, (i > 0) ? (i - 1) : i);
					if (i < 0)
					{
						return false;
					}
					i = Normalization.TryComposeWithPreviousStarter(null, source, i);
					if (i < 0)
					{
						return false;
					}
					break;
				}
			}
			return true;
		}

		public static string Normalize(string source, NormalizationForm normalizationForm)
		{
			switch (normalizationForm)
			{
			case NormalizationForm.FormD:
				return Normalization.Normalize(source, 1);
			default:
				return Normalization.Normalize(source, 0);
			case NormalizationForm.FormKC:
				return Normalization.Normalize(source, 2);
			case NormalizationForm.FormKD:
				return Normalization.Normalize(source, 3);
			}
		}

		public static string Normalize(string source, int type)
		{
			switch (type)
			{
			case 1:
			case 3:
				return Normalization.Decompose(source, type);
			default:
				return Normalization.Compose(source, type);
			}
		}

		public static bool IsReady
		{
			get
			{
				return Normalization.isReady;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void load_normalization_resource(out IntPtr props, out IntPtr mappedChars, out IntPtr charMapIndex, out IntPtr helperIndex, out IntPtr mapIdxToComposite, out IntPtr combiningClass);

		unsafe static Normalization()
		{
			object obj = Normalization.forLock;
			lock (obj)
			{
				IntPtr intPtr;
				IntPtr intPtr2;
				IntPtr intPtr3;
				IntPtr intPtr4;
				IntPtr intPtr5;
				IntPtr intPtr6;
				Normalization.load_normalization_resource(out intPtr, out intPtr2, out intPtr3, out intPtr4, out intPtr5, out intPtr6);
				Normalization.props = (byte*)(void*)intPtr;
				Normalization.mappedChars = (int*)(void*)intPtr2;
				Normalization.charMapIndex = (short*)(void*)intPtr3;
				Normalization.helperIndex = (short*)(void*)intPtr4;
				Normalization.mapIdxToComposite = (ushort*)(void*)intPtr5;
				Normalization.combiningClass = (byte*)(void*)intPtr6;
			}
			Normalization.isReady = true;
		}

		public const int NoNfd = 1;

		public const int NoNfkd = 2;

		public const int NoNfc = 4;

		public const int MaybeNfc = 8;

		public const int NoNfkc = 16;

		public const int MaybeNfkc = 32;

		public const int FullCompositionExclusion = 64;

		public const int IsUnsafe = 128;

		private const int HangulSBase = 44032;

		private const int HangulLBase = 4352;

		private const int HangulVBase = 4449;

		private const int HangulTBase = 4519;

		private const int HangulLCount = 19;

		private const int HangulVCount = 21;

		private const int HangulTCount = 28;

		private const int HangulNCount = 588;

		private const int HangulSCount = 11172;

		private unsafe static byte* props;

		private unsafe static int* mappedChars;

		private unsafe static short* charMapIndex;

		private unsafe static short* helperIndex;

		private unsafe static ushort* mapIdxToComposite;

		private unsafe static byte* combiningClass;

		private static object forLock = new object();

		public static readonly bool isReady;
	}
}
