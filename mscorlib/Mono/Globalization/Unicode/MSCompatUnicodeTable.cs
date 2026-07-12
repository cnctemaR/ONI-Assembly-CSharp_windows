using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Mono.Globalization.Unicode
{
	internal class MSCompatUnicodeTable
	{
		public static TailoringInfo GetTailoringInfo(int lcid)
		{
			for (int i = 0; i < MSCompatUnicodeTable.tailoringInfos.Length; i++)
			{
				if (MSCompatUnicodeTable.tailoringInfos[i].LCID == lcid)
				{
					return MSCompatUnicodeTable.tailoringInfos[i];
				}
			}
			return null;
		}

		public unsafe static void BuildTailoringTables(CultureInfo culture, TailoringInfo t, ref Contraction[] contractions, ref Level2Map[] diacriticals)
		{
			List<Contraction> list = new List<Contraction>();
			List<Level2Map> list2 = new List<Level2Map>();
			int num = 0;
			char[] array;
			char* ptr;
			if ((array = MSCompatUnicodeTable.tailoringArr) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			int i = t.TailoringIndex;
			int num2 = i + t.TailoringCount;
			while (i < num2)
			{
				int num3 = i + 1;
				switch (ptr[i])
				{
				case '\u0001':
				{
					i++;
					while (ptr[num3] != '\0')
					{
						num3++;
					}
					char[] array2 = new char[num3 - i];
					Marshal.Copy((IntPtr)((void*)(ptr + i)), array2, 0, num3 - i);
					byte[] array3 = new byte[4];
					for (int j = 0; j < 4; j++)
					{
						array3[j] = (byte)ptr[num3 + 1 + j];
					}
					list.Add(new Contraction(num, array2, null, array3));
					i = num3 + 6;
					num++;
					break;
				}
				case '\u0002':
					list2.Add(new Level2Map((byte)ptr[i + 1], (byte)ptr[i + 2]));
					i += 3;
					break;
				case '\u0003':
				{
					i++;
					while (ptr[num3] != '\0')
					{
						num3++;
					}
					char[] array2 = new char[num3 - i];
					Marshal.Copy((IntPtr)((void*)(ptr + i)), array2, 0, num3 - i);
					num3++;
					int num4 = num3;
					while (ptr[num4] != '\0')
					{
						num4++;
					}
					string text = new string(ptr, num3, num4 - num3);
					list.Add(new Contraction(num, array2, text, null));
					i = num4 + 1;
					num++;
					break;
				}
				default:
					throw new NotImplementedException(string.Format("Mono INTERNAL ERROR (Should not happen): Collation tailoring table is broken for culture {0} ({1}) at 0x{2:X}", culture.LCID, culture.Name, i));
				}
			}
			array = null;
			list.Sort(ContractionComparer.Instance);
			list2.Sort((Level2Map a, Level2Map b) => (int)(a.Source - b.Source));
			contractions = list.ToArray();
			diacriticals = list2.ToArray();
		}

		private unsafe static void SetCJKReferences(string name, ref CodePointIndexer cjkIndexer, ref byte* catTable, ref byte* lv1Table, ref CodePointIndexer lv2Indexer, ref byte* lv2Table)
		{
			if (name == "zh-CHS")
			{
				catTable = MSCompatUnicodeTable.cjkCHScategory;
				lv1Table = MSCompatUnicodeTable.cjkCHSlv1;
				cjkIndexer = MSCompatUnicodeTableUtil.CjkCHS;
				return;
			}
			if (name == "zh-CHT")
			{
				catTable = MSCompatUnicodeTable.cjkCHTcategory;
				lv1Table = MSCompatUnicodeTable.cjkCHTlv1;
				cjkIndexer = MSCompatUnicodeTableUtil.Cjk;
				return;
			}
			if (name == "ja")
			{
				catTable = MSCompatUnicodeTable.cjkJAcategory;
				lv1Table = MSCompatUnicodeTable.cjkJAlv1;
				cjkIndexer = MSCompatUnicodeTableUtil.Cjk;
				return;
			}
			if (!(name == "ko"))
			{
				return;
			}
			catTable = MSCompatUnicodeTable.cjkKOcategory;
			lv1Table = MSCompatUnicodeTable.cjkKOlv1;
			lv2Table = MSCompatUnicodeTable.cjkKOlv2;
			cjkIndexer = MSCompatUnicodeTableUtil.Cjk;
			lv2Indexer = MSCompatUnicodeTableUtil.Cjk;
		}

		public unsafe static byte Category(int cp)
		{
			return MSCompatUnicodeTable.categories[MSCompatUnicodeTableUtil.Category.ToIndex(cp)];
		}

		public unsafe static byte Level1(int cp)
		{
			return MSCompatUnicodeTable.level1[MSCompatUnicodeTableUtil.Level1.ToIndex(cp)];
		}

		public unsafe static byte Level2(int cp)
		{
			return MSCompatUnicodeTable.level2[MSCompatUnicodeTableUtil.Level2.ToIndex(cp)];
		}

		public unsafe static byte Level3(int cp)
		{
			return MSCompatUnicodeTable.level3[MSCompatUnicodeTableUtil.Level3.ToIndex(cp)];
		}

		public static bool IsSortable(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (!MSCompatUnicodeTable.IsSortable((int)s[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsSortable(int cp)
		{
			return !MSCompatUnicodeTable.IsIgnorable(cp) || (cp == 0 || cp == 1600 || cp == 65279) || (6155 <= cp && cp <= 6158) || (8204 <= cp && cp <= 8207) || (8234 <= cp && cp <= 8238) || (8298 <= cp && cp <= 8303) || (8204 <= cp && cp <= 8207) || (65529 <= cp && cp <= 65533);
		}

		public static bool IsIgnorable(int cp)
		{
			return MSCompatUnicodeTable.IsIgnorable(cp, 1);
		}

		public unsafe static bool IsIgnorable(int cp, byte flag)
		{
			if (cp == 0)
			{
				return true;
			}
			if ((flag & 1) != 0)
			{
				if (char.GetUnicodeCategory((char)cp) == UnicodeCategory.OtherNotAssigned)
				{
					return true;
				}
				if (55424 <= cp && cp < 56192)
				{
					return true;
				}
			}
			int num = MSCompatUnicodeTableUtil.Ignorable.ToIndex(cp);
			return num >= 0 && (MSCompatUnicodeTable.ignorableFlags[num] & flag) > 0;
		}

		public static bool IsIgnorableSymbol(int cp)
		{
			return MSCompatUnicodeTable.IsIgnorable(cp, 2);
		}

		public static bool IsIgnorableNonSpacing(int cp)
		{
			return MSCompatUnicodeTable.IsIgnorable(cp, 4);
		}

		public static int ToKanaTypeInsensitive(int i)
		{
			if (12353 > i || i > 12436)
			{
				return i;
			}
			return i + 96;
		}

		public static int ToWidthCompat(int i)
		{
			if (i < 8592)
			{
				return i;
			}
			if (i > 65280)
			{
				if (i <= 65374)
				{
					return i - 65280 + 32;
				}
				switch (i)
				{
				case 65504:
					return 162;
				case 65505:
					return 163;
				case 65506:
					return 172;
				case 65507:
					return 175;
				case 65508:
					return 166;
				case 65509:
					return 165;
				case 65510:
					return 8361;
				}
			}
			if (i > 13054)
			{
				return i;
			}
			if (i <= 8595)
			{
				return 56921 + i;
			}
			if (i < 9474)
			{
				return i;
			}
			if (i <= 9675)
			{
				if (i == 9474)
				{
					return 65512;
				}
				if (i == 9632)
				{
					return 65517;
				}
				if (i != 9675)
				{
					return i;
				}
				return 65518;
			}
			else
			{
				if (i < 12288)
				{
					return i;
				}
				if (i < 12593)
				{
					if (i <= 12300)
					{
						switch (i)
						{
						case 12288:
							return 32;
						case 12289:
							return 65380;
						case 12290:
							return 65377;
						default:
							if (i == 12300)
							{
								return 65378;
							}
							break;
						}
					}
					else
					{
						if (i == 12301)
						{
							return 65379;
						}
						if (i == 12539)
						{
							return 65381;
						}
					}
					return i;
				}
				if (i < 12644)
				{
					return i - 12592 + 65440;
				}
				if (i == 12644)
				{
					return 65440;
				}
				return i;
			}
		}

		public static bool HasSpecialWeight(char c)
		{
			if (c < 'ぁ')
			{
				return false;
			}
			if ('ｦ' <= c && c < 'ﾞ')
			{
				return true;
			}
			if ('㌀' <= c)
			{
				return false;
			}
			if (c < 'ゝ')
			{
				return c < '\u3099';
			}
			if (c < '\u3100')
			{
				return c != '・';
			}
			return c >= '㋐' && c < '㋿';
		}

		public static byte GetJapaneseDashType(char c)
		{
			if (c <= 'ゞ')
			{
				if (c != 'ゝ' && c != 'ゞ')
				{
					return 3;
				}
			}
			else
			{
				switch (c)
				{
				case 'ー':
					return 5;
				case 'ヽ':
				case 'ヾ':
					break;
				default:
					if (c != 'ｰ')
					{
						return 3;
					}
					break;
				}
			}
			return 4;
		}

		public static bool IsHalfWidthKana(char c)
		{
			return 'ｦ' <= c && c <= 'ﾝ';
		}

		public static bool IsHiragana(char c)
		{
			return 'ぁ' <= c && c <= 'ゔ';
		}

		public static bool IsJapaneseSmallLetter(char c)
		{
			if ('ｧ' <= c && c <= 'ｯ')
			{
				return true;
			}
			if ('\u3040' < c && c < 'ヺ')
			{
				if (c <= 'ォ')
				{
					if (c <= 'っ')
					{
						switch (c)
						{
						case 'ぁ':
						case 'ぃ':
						case 'ぅ':
						case 'ぇ':
						case 'ぉ':
							break;
						case 'あ':
						case 'い':
						case 'う':
						case 'え':
							return false;
						default:
							if (c != 'っ')
							{
								return false;
							}
							break;
						}
					}
					else
					{
						switch (c)
						{
						case 'ゃ':
						case 'ゅ':
						case 'ょ':
							break;
						case 'や':
						case 'ゆ':
							return false;
						default:
							if (c != 'ゎ')
							{
								switch (c)
								{
								case 'ァ':
								case 'ィ':
								case 'ゥ':
								case 'ェ':
								case 'ォ':
									break;
								case 'ア':
								case 'イ':
								case 'ウ':
								case 'エ':
									return false;
								default:
									return false;
								}
							}
							break;
						}
					}
				}
				else if (c <= 'ョ')
				{
					if (c != 'ッ')
					{
						switch (c)
						{
						case 'ャ':
						case 'ュ':
						case 'ョ':
							break;
						case 'ヤ':
						case 'ユ':
							return false;
						default:
							return false;
						}
					}
				}
				else if (c != 'ヮ' && c != 'ヵ' && c != 'ヶ')
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static bool IsReady
		{
			get
			{
				return MSCompatUnicodeTable.isReady;
			}
		}

		private static IntPtr GetResource(string name)
		{
			int num;
			Module module;
			return ((RuntimeAssembly)Assembly.GetExecutingAssembly()).GetManifestResourceInternal(name, out num, out module);
		}

		private unsafe static uint UInt32FromBytePtr(byte* raw, uint idx)
		{
			return (uint)((int)raw[idx] + ((int)raw[idx + 1U] << 8) + ((int)raw[idx + 2U] << 16) + ((int)raw[idx + 3U] << 24));
		}

		unsafe static MSCompatUnicodeTable()
		{
			IntPtr intPtr = MSCompatUnicodeTable.GetResource("collation.core.bin");
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			byte* ptr = (byte*)(void*)intPtr;
			intPtr = MSCompatUnicodeTable.GetResource("collation.tailoring.bin");
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			byte* ptr2 = (byte*)(void*)intPtr;
			if (ptr == null || ptr2 == null)
			{
				return;
			}
			if (*ptr != 3 || *ptr2 != 3)
			{
				return;
			}
			uint num = 1U;
			uint num2 = MSCompatUnicodeTable.UInt32FromBytePtr(ptr, num);
			num += 4U;
			MSCompatUnicodeTable.ignorableFlags = ptr + num;
			num += num2;
			num2 = MSCompatUnicodeTable.UInt32FromBytePtr(ptr, num);
			num += 4U;
			MSCompatUnicodeTable.categories = ptr + num;
			num += num2;
			num2 = MSCompatUnicodeTable.UInt32FromBytePtr(ptr, num);
			num += 4U;
			MSCompatUnicodeTable.level1 = ptr + num;
			num += num2;
			num2 = MSCompatUnicodeTable.UInt32FromBytePtr(ptr, num);
			num += 4U;
			MSCompatUnicodeTable.level2 = ptr + num;
			num += num2;
			num2 = MSCompatUnicodeTable.UInt32FromBytePtr(ptr, num);
			num += 4U;
			MSCompatUnicodeTable.level3 = ptr + num;
			num += num2;
			num = 1U;
			uint num3 = MSCompatUnicodeTable.UInt32FromBytePtr(ptr2, num);
			num += 4U;
			MSCompatUnicodeTable.tailoringInfos = new TailoringInfo[num3];
			int num4 = 0;
			while ((long)num4 < (long)((ulong)num3))
			{
				int num5 = (int)MSCompatUnicodeTable.UInt32FromBytePtr(ptr2, num);
				num += 4U;
				int num6 = (int)MSCompatUnicodeTable.UInt32FromBytePtr(ptr2, num);
				num += 4U;
				int num7 = (int)MSCompatUnicodeTable.UInt32FromBytePtr(ptr2, num);
				num += 4U;
				TailoringInfo tailoringInfo = new TailoringInfo(num5, num6, num7, ptr2[(UIntPtr)(num++)] > 0);
				MSCompatUnicodeTable.tailoringInfos[num4] = tailoringInfo;
				num4++;
			}
			num += 2U;
			num3 = MSCompatUnicodeTable.UInt32FromBytePtr(ptr2, num);
			num += 4U;
			MSCompatUnicodeTable.tailoringArr = new char[num3];
			int num8 = 0;
			while ((long)num8 < (long)((ulong)num3))
			{
				MSCompatUnicodeTable.tailoringArr[num8] = (char)((int)ptr2[num] + ((int)ptr2[num + 1U] << 8));
				num8++;
				num += 2U;
			}
			MSCompatUnicodeTable.isReady = true;
		}

		public unsafe static void FillCJK(string culture, ref CodePointIndexer cjkIndexer, ref byte* catTable, ref byte* lv1Table, ref CodePointIndexer lv2Indexer, ref byte* lv2Table)
		{
			object obj = MSCompatUnicodeTable.forLock;
			lock (obj)
			{
				MSCompatUnicodeTable.FillCJKCore(culture, ref cjkIndexer, ref catTable, ref lv1Table, ref lv2Indexer, ref lv2Table);
				MSCompatUnicodeTable.SetCJKReferences(culture, ref cjkIndexer, ref catTable, ref lv1Table, ref lv2Indexer, ref lv2Table);
			}
		}

		private unsafe static void FillCJKCore(string culture, ref CodePointIndexer cjkIndexer, ref byte* catTable, ref byte* lv1Table, ref CodePointIndexer cjkLv2Indexer, ref byte* lv2Table)
		{
			if (!MSCompatUnicodeTable.IsReady)
			{
				return;
			}
			string text = null;
			if (!(culture == "zh-CHS"))
			{
				if (!(culture == "zh-CHT"))
				{
					if (!(culture == "ja"))
					{
						if (culture == "ko")
						{
							text = "cjkKO";
							catTable = MSCompatUnicodeTable.cjkKOcategory;
							lv1Table = MSCompatUnicodeTable.cjkKOlv1;
						}
					}
					else
					{
						text = "cjkJA";
						catTable = MSCompatUnicodeTable.cjkJAcategory;
						lv1Table = MSCompatUnicodeTable.cjkJAlv1;
					}
				}
				else
				{
					text = "cjkCHT";
					catTable = MSCompatUnicodeTable.cjkCHTcategory;
					lv1Table = MSCompatUnicodeTable.cjkCHTlv1;
				}
			}
			else
			{
				text = "cjkCHS";
				catTable = MSCompatUnicodeTable.cjkCHScategory;
				lv1Table = MSCompatUnicodeTable.cjkCHSlv1;
			}
			if (text == null || lv1Table != (IntPtr)((UIntPtr)0))
			{
				return;
			}
			uint num = 0U;
			IntPtr intPtr = MSCompatUnicodeTable.GetResource(string.Format("collation.{0}.bin", text));
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			byte* ptr = (byte*)(void*)intPtr;
			num += 1U;
			uint num2 = MSCompatUnicodeTable.UInt32FromBytePtr(ptr, num);
			num += 4U;
			catTable = ptr + num;
			lv1Table = ptr + num + num2;
			if (!(culture == "zh-CHS"))
			{
				if (!(culture == "zh-CHT"))
				{
					if (!(culture == "ja"))
					{
						if (culture == "ko")
						{
							MSCompatUnicodeTable.cjkKOcategory = catTable;
							MSCompatUnicodeTable.cjkKOlv1 = lv1Table;
						}
					}
					else
					{
						MSCompatUnicodeTable.cjkJAcategory = catTable;
						MSCompatUnicodeTable.cjkJAlv1 = lv1Table;
					}
				}
				else
				{
					MSCompatUnicodeTable.cjkCHTcategory = catTable;
					MSCompatUnicodeTable.cjkCHTlv1 = lv1Table;
				}
			}
			else
			{
				MSCompatUnicodeTable.cjkCHScategory = catTable;
				MSCompatUnicodeTable.cjkCHSlv1 = lv1Table;
			}
			if (text != "cjkKO")
			{
				return;
			}
			intPtr = MSCompatUnicodeTable.GetResource("collation.cjkKOlv2.bin");
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			ptr = (byte*)(void*)intPtr;
			num = 5U;
			MSCompatUnicodeTable.cjkKOlv2 = ptr + num;
			lv2Table = MSCompatUnicodeTable.cjkKOlv2;
		}

		public static int MaxExpansionLength = 3;

		private unsafe static readonly byte* ignorableFlags;

		private unsafe static readonly byte* categories;

		private unsafe static readonly byte* level1;

		private unsafe static readonly byte* level2;

		private unsafe static readonly byte* level3;

		private unsafe static byte* cjkCHScategory;

		private unsafe static byte* cjkCHTcategory;

		private unsafe static byte* cjkJAcategory;

		private unsafe static byte* cjkKOcategory;

		private unsafe static byte* cjkCHSlv1;

		private unsafe static byte* cjkCHTlv1;

		private unsafe static byte* cjkJAlv1;

		private unsafe static byte* cjkKOlv1;

		private unsafe static byte* cjkKOlv2;

		private const int ResourceVersionSize = 1;

		private static readonly char[] tailoringArr;

		private static readonly TailoringInfo[] tailoringInfos;

		private static object forLock = new object();

		public static readonly bool isReady;
	}
}
