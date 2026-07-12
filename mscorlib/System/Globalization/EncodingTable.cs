using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace System.Globalization
{
	internal static class EncodingTable
	{
		private static int GetNumEncodingItems()
		{
			return EncodingTable.encodingDataPtr.Length;
		}

		private static InternalEncodingDataItem ENC(string name, ushort cp)
		{
			return new InternalEncodingDataItem
			{
				webName = name,
				codePage = cp
			};
		}

		private static InternalCodePageDataItem MapCodePageDataItem(ushort cp, ushort fcp, string names, uint flags)
		{
			return new InternalCodePageDataItem
			{
				codePage = cp,
				uiFamilyCodePage = fcp,
				flags = flags,
				Names = names
			};
		}

		[SecuritySafeCritical]
		private static int internalGetCodePageFromName(string name)
		{
			int i = 0;
			int num = EncodingTable.lastEncodingItem;
			while (num - i > 3)
			{
				int num2 = (num - i) / 2 + i;
				int num3 = string.Compare(name, EncodingTable.encodingDataPtr[num2].webName, StringComparison.OrdinalIgnoreCase);
				if (num3 == 0)
				{
					return (int)EncodingTable.encodingDataPtr[num2].codePage;
				}
				if (num3 < 0)
				{
					num = num2;
				}
				else
				{
					i = num2;
				}
			}
			while (i <= num)
			{
				if (string.Compare(name, EncodingTable.encodingDataPtr[i].webName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return (int)EncodingTable.encodingDataPtr[i].codePage;
				}
				i++;
			}
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("'{0}' is not a supported encoding name. For information on defining a custom encoding, see the documentation for the Encoding.RegisterProvider method."), name), "name");
		}

		[SecuritySafeCritical]
		internal static EncodingInfo[] GetEncodings()
		{
			if (EncodingTable.lastCodePageItem == 0)
			{
				int num = 0;
				while (EncodingTable.codePageDataPtr[num].codePage != 0)
				{
					num++;
				}
				EncodingTable.lastCodePageItem = num;
			}
			EncodingInfo[] array = new EncodingInfo[EncodingTable.lastCodePageItem];
			for (int i = 0; i < EncodingTable.lastCodePageItem; i++)
			{
				array[i] = new EncodingInfo((int)EncodingTable.codePageDataPtr[i].codePage, CodePageDataItem.CreateString(EncodingTable.codePageDataPtr[i].Names, 0U), Environment.GetResourceString("Globalization.cp_" + EncodingTable.codePageDataPtr[i].codePage.ToString()));
			}
			return array;
		}

		internal static int GetCodePageFromName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			object syncRoot = ((ICollection)EncodingTable.hashByName).SyncRoot;
			int num2;
			lock (syncRoot)
			{
				int num;
				if (EncodingTable.hashByName.TryGetValue(name, out num))
				{
					num2 = num;
				}
				else
				{
					num = EncodingTable.internalGetCodePageFromName(name);
					EncodingTable.hashByName[name] = num;
					num2 = num;
				}
			}
			return num2;
		}

		[SecuritySafeCritical]
		internal static CodePageDataItem GetCodePageDataItem(int codepage)
		{
			object syncRoot = ((ICollection)EncodingTable.hashByCodePage).SyncRoot;
			lock (syncRoot)
			{
				CodePageDataItem codePageDataItem;
				if (EncodingTable.hashByCodePage.TryGetValue(codepage, out codePageDataItem))
				{
					return codePageDataItem;
				}
				int num = 0;
				int codePage;
				while ((codePage = (int)EncodingTable.codePageDataPtr[num].codePage) != 0)
				{
					if (codePage == codepage)
					{
						codePageDataItem = new CodePageDataItem(num);
						EncodingTable.hashByCodePage[codepage] = codePageDataItem;
						return codePageDataItem;
					}
					num++;
				}
			}
			return null;
		}

		internal static InternalEncodingDataItem[] encodingDataPtr = new InternalEncodingDataItem[]
		{
			EncodingTable.ENC("437", 437),
			EncodingTable.ENC("ANSI_X3.4-1968", 20127),
			EncodingTable.ENC("ANSI_X3.4-1986", 20127),
			EncodingTable.ENC("arabic", 28596),
			EncodingTable.ENC("ascii", 20127),
			EncodingTable.ENC("ASMO-708", 708),
			EncodingTable.ENC("Big5", 950),
			EncodingTable.ENC("Big5-HKSCS", 950),
			EncodingTable.ENC("CCSID00858", 858),
			EncodingTable.ENC("CCSID00924", 20924),
			EncodingTable.ENC("CCSID01140", 1140),
			EncodingTable.ENC("CCSID01141", 1141),
			EncodingTable.ENC("CCSID01142", 1142),
			EncodingTable.ENC("CCSID01143", 1143),
			EncodingTable.ENC("CCSID01144", 1144),
			EncodingTable.ENC("CCSID01145", 1145),
			EncodingTable.ENC("CCSID01146", 1146),
			EncodingTable.ENC("CCSID01147", 1147),
			EncodingTable.ENC("CCSID01148", 1148),
			EncodingTable.ENC("CCSID01149", 1149),
			EncodingTable.ENC("chinese", 936),
			EncodingTable.ENC("cn-big5", 950),
			EncodingTable.ENC("CN-GB", 936),
			EncodingTable.ENC("CP00858", 858),
			EncodingTable.ENC("CP00924", 20924),
			EncodingTable.ENC("CP01140", 1140),
			EncodingTable.ENC("CP01141", 1141),
			EncodingTable.ENC("CP01142", 1142),
			EncodingTable.ENC("CP01143", 1143),
			EncodingTable.ENC("CP01144", 1144),
			EncodingTable.ENC("CP01145", 1145),
			EncodingTable.ENC("CP01146", 1146),
			EncodingTable.ENC("CP01147", 1147),
			EncodingTable.ENC("CP01148", 1148),
			EncodingTable.ENC("CP01149", 1149),
			EncodingTable.ENC("cp037", 37),
			EncodingTable.ENC("cp1025", 21025),
			EncodingTable.ENC("CP1026", 1026),
			EncodingTable.ENC("cp1256", 1256),
			EncodingTable.ENC("CP273", 20273),
			EncodingTable.ENC("CP278", 20278),
			EncodingTable.ENC("CP280", 20280),
			EncodingTable.ENC("CP284", 20284),
			EncodingTable.ENC("CP285", 20285),
			EncodingTable.ENC("cp290", 20290),
			EncodingTable.ENC("cp297", 20297),
			EncodingTable.ENC("cp367", 20127),
			EncodingTable.ENC("cp420", 20420),
			EncodingTable.ENC("cp423", 20423),
			EncodingTable.ENC("cp424", 20424),
			EncodingTable.ENC("cp437", 437),
			EncodingTable.ENC("CP500", 500),
			EncodingTable.ENC("cp50227", 50227),
			EncodingTable.ENC("cp819", 28591),
			EncodingTable.ENC("cp850", 850),
			EncodingTable.ENC("cp852", 852),
			EncodingTable.ENC("cp855", 855),
			EncodingTable.ENC("cp857", 857),
			EncodingTable.ENC("cp858", 858),
			EncodingTable.ENC("cp860", 860),
			EncodingTable.ENC("cp861", 861),
			EncodingTable.ENC("cp862", 862),
			EncodingTable.ENC("cp863", 863),
			EncodingTable.ENC("cp864", 864),
			EncodingTable.ENC("cp865", 865),
			EncodingTable.ENC("cp866", 866),
			EncodingTable.ENC("cp869", 869),
			EncodingTable.ENC("CP870", 870),
			EncodingTable.ENC("CP871", 20871),
			EncodingTable.ENC("cp875", 875),
			EncodingTable.ENC("cp880", 20880),
			EncodingTable.ENC("CP905", 20905),
			EncodingTable.ENC("csASCII", 20127),
			EncodingTable.ENC("csbig5", 950),
			EncodingTable.ENC("csEUCKR", 51949),
			EncodingTable.ENC("csEUCPkdFmtJapanese", 51932),
			EncodingTable.ENC("csGB2312", 936),
			EncodingTable.ENC("csGB231280", 936),
			EncodingTable.ENC("csIBM037", 37),
			EncodingTable.ENC("csIBM1026", 1026),
			EncodingTable.ENC("csIBM273", 20273),
			EncodingTable.ENC("csIBM277", 20277),
			EncodingTable.ENC("csIBM278", 20278),
			EncodingTable.ENC("csIBM280", 20280),
			EncodingTable.ENC("csIBM284", 20284),
			EncodingTable.ENC("csIBM285", 20285),
			EncodingTable.ENC("csIBM290", 20290),
			EncodingTable.ENC("csIBM297", 20297),
			EncodingTable.ENC("csIBM420", 20420),
			EncodingTable.ENC("csIBM423", 20423),
			EncodingTable.ENC("csIBM424", 20424),
			EncodingTable.ENC("csIBM500", 500),
			EncodingTable.ENC("csIBM870", 870),
			EncodingTable.ENC("csIBM871", 20871),
			EncodingTable.ENC("csIBM880", 20880),
			EncodingTable.ENC("csIBM905", 20905),
			EncodingTable.ENC("csIBMThai", 20838),
			EncodingTable.ENC("csISO2022JP", 50221),
			EncodingTable.ENC("csISO2022KR", 50225),
			EncodingTable.ENC("csISO58GB231280", 936),
			EncodingTable.ENC("csISOLatin1", 28591),
			EncodingTable.ENC("csISOLatin2", 28592),
			EncodingTable.ENC("csISOLatin3", 28593),
			EncodingTable.ENC("csISOLatin4", 28594),
			EncodingTable.ENC("csISOLatin5", 28599),
			EncodingTable.ENC("csISOLatin9", 28605),
			EncodingTable.ENC("csISOLatinArabic", 28596),
			EncodingTable.ENC("csISOLatinCyrillic", 28595),
			EncodingTable.ENC("csISOLatinGreek", 28597),
			EncodingTable.ENC("csISOLatinHebrew", 28598),
			EncodingTable.ENC("csKOI8R", 20866),
			EncodingTable.ENC("csKSC56011987", 949),
			EncodingTable.ENC("csPC8CodePage437", 437),
			EncodingTable.ENC("csShiftJIS", 932),
			EncodingTable.ENC("csUnicode11UTF7", 65000),
			EncodingTable.ENC("csWindows31J", 932),
			EncodingTable.ENC("cyrillic", 28595),
			EncodingTable.ENC("DIN_66003", 20106),
			EncodingTable.ENC("DOS-720", 720),
			EncodingTable.ENC("DOS-862", 862),
			EncodingTable.ENC("DOS-874", 874),
			EncodingTable.ENC("ebcdic-cp-ar1", 20420),
			EncodingTable.ENC("ebcdic-cp-be", 500),
			EncodingTable.ENC("ebcdic-cp-ca", 37),
			EncodingTable.ENC("ebcdic-cp-ch", 500),
			EncodingTable.ENC("EBCDIC-CP-DK", 20277),
			EncodingTable.ENC("ebcdic-cp-es", 20284),
			EncodingTable.ENC("ebcdic-cp-fi", 20278),
			EncodingTable.ENC("ebcdic-cp-fr", 20297),
			EncodingTable.ENC("ebcdic-cp-gb", 20285),
			EncodingTable.ENC("ebcdic-cp-gr", 20423),
			EncodingTable.ENC("ebcdic-cp-he", 20424),
			EncodingTable.ENC("ebcdic-cp-is", 20871),
			EncodingTable.ENC("ebcdic-cp-it", 20280),
			EncodingTable.ENC("ebcdic-cp-nl", 37),
			EncodingTable.ENC("EBCDIC-CP-NO", 20277),
			EncodingTable.ENC("ebcdic-cp-roece", 870),
			EncodingTable.ENC("ebcdic-cp-se", 20278),
			EncodingTable.ENC("ebcdic-cp-tr", 20905),
			EncodingTable.ENC("ebcdic-cp-us", 37),
			EncodingTable.ENC("ebcdic-cp-wt", 37),
			EncodingTable.ENC("ebcdic-cp-yu", 870),
			EncodingTable.ENC("EBCDIC-Cyrillic", 20880),
			EncodingTable.ENC("ebcdic-de-273+euro", 1141),
			EncodingTable.ENC("ebcdic-dk-277+euro", 1142),
			EncodingTable.ENC("ebcdic-es-284+euro", 1145),
			EncodingTable.ENC("ebcdic-fi-278+euro", 1143),
			EncodingTable.ENC("ebcdic-fr-297+euro", 1147),
			EncodingTable.ENC("ebcdic-gb-285+euro", 1146),
			EncodingTable.ENC("ebcdic-international-500+euro", 1148),
			EncodingTable.ENC("ebcdic-is-871+euro", 1149),
			EncodingTable.ENC("ebcdic-it-280+euro", 1144),
			EncodingTable.ENC("EBCDIC-JP-kana", 20290),
			EncodingTable.ENC("ebcdic-Latin9--euro", 20924),
			EncodingTable.ENC("ebcdic-no-277+euro", 1142),
			EncodingTable.ENC("ebcdic-se-278+euro", 1143),
			EncodingTable.ENC("ebcdic-us-37+euro", 1140),
			EncodingTable.ENC("ECMA-114", 28596),
			EncodingTable.ENC("ECMA-118", 28597),
			EncodingTable.ENC("ELOT_928", 28597),
			EncodingTable.ENC("euc-cn", 51936),
			EncodingTable.ENC("euc-jp", 51932),
			EncodingTable.ENC("euc-kr", 51949),
			EncodingTable.ENC("Extended_UNIX_Code_Packed_Format_for_Japanese", 51932),
			EncodingTable.ENC("GB18030", 54936),
			EncodingTable.ENC("GB2312", 936),
			EncodingTable.ENC("GB2312-80", 936),
			EncodingTable.ENC("GB231280", 936),
			EncodingTable.ENC("GBK", 936),
			EncodingTable.ENC("GB_2312-80", 936),
			EncodingTable.ENC("German", 20106),
			EncodingTable.ENC("greek", 28597),
			EncodingTable.ENC("greek8", 28597),
			EncodingTable.ENC("hebrew", 28598),
			EncodingTable.ENC("hz-gb-2312", 52936),
			EncodingTable.ENC("IBM-Thai", 20838),
			EncodingTable.ENC("IBM00858", 858),
			EncodingTable.ENC("IBM00924", 20924),
			EncodingTable.ENC("IBM01047", 1047),
			EncodingTable.ENC("IBM01140", 1140),
			EncodingTable.ENC("IBM01141", 1141),
			EncodingTable.ENC("IBM01142", 1142),
			EncodingTable.ENC("IBM01143", 1143),
			EncodingTable.ENC("IBM01144", 1144),
			EncodingTable.ENC("IBM01145", 1145),
			EncodingTable.ENC("IBM01146", 1146),
			EncodingTable.ENC("IBM01147", 1147),
			EncodingTable.ENC("IBM01148", 1148),
			EncodingTable.ENC("IBM01149", 1149),
			EncodingTable.ENC("IBM037", 37),
			EncodingTable.ENC("IBM1026", 1026),
			EncodingTable.ENC("IBM273", 20273),
			EncodingTable.ENC("IBM277", 20277),
			EncodingTable.ENC("IBM278", 20278),
			EncodingTable.ENC("IBM280", 20280),
			EncodingTable.ENC("IBM284", 20284),
			EncodingTable.ENC("IBM285", 20285),
			EncodingTable.ENC("IBM290", 20290),
			EncodingTable.ENC("IBM297", 20297),
			EncodingTable.ENC("IBM367", 20127),
			EncodingTable.ENC("IBM420", 20420),
			EncodingTable.ENC("IBM423", 20423),
			EncodingTable.ENC("IBM424", 20424),
			EncodingTable.ENC("IBM437", 437),
			EncodingTable.ENC("IBM500", 500),
			EncodingTable.ENC("ibm737", 737),
			EncodingTable.ENC("ibm775", 775),
			EncodingTable.ENC("ibm819", 28591),
			EncodingTable.ENC("IBM850", 850),
			EncodingTable.ENC("IBM852", 852),
			EncodingTable.ENC("IBM855", 855),
			EncodingTable.ENC("IBM857", 857),
			EncodingTable.ENC("IBM860", 860),
			EncodingTable.ENC("IBM861", 861),
			EncodingTable.ENC("IBM862", 862),
			EncodingTable.ENC("IBM863", 863),
			EncodingTable.ENC("IBM864", 864),
			EncodingTable.ENC("IBM865", 865),
			EncodingTable.ENC("IBM866", 866),
			EncodingTable.ENC("IBM869", 869),
			EncodingTable.ENC("IBM870", 870),
			EncodingTable.ENC("IBM871", 20871),
			EncodingTable.ENC("IBM880", 20880),
			EncodingTable.ENC("IBM905", 20905),
			EncodingTable.ENC("irv", 20105),
			EncodingTable.ENC("ISO-10646-UCS-2", 1200),
			EncodingTable.ENC("iso-2022-jp", 50220),
			EncodingTable.ENC("iso-2022-jpeuc", 51932),
			EncodingTable.ENC("iso-2022-kr", 50225),
			EncodingTable.ENC("iso-2022-kr-7", 50225),
			EncodingTable.ENC("iso-2022-kr-7bit", 50225),
			EncodingTable.ENC("iso-2022-kr-8", 51949),
			EncodingTable.ENC("iso-2022-kr-8bit", 51949),
			EncodingTable.ENC("iso-8859-1", 28591),
			EncodingTable.ENC("iso-8859-11", 874),
			EncodingTable.ENC("iso-8859-13", 28603),
			EncodingTable.ENC("iso-8859-15", 28605),
			EncodingTable.ENC("iso-8859-2", 28592),
			EncodingTable.ENC("iso-8859-3", 28593),
			EncodingTable.ENC("iso-8859-4", 28594),
			EncodingTable.ENC("iso-8859-5", 28595),
			EncodingTable.ENC("iso-8859-6", 28596),
			EncodingTable.ENC("iso-8859-7", 28597),
			EncodingTable.ENC("iso-8859-8", 28598),
			EncodingTable.ENC("ISO-8859-8 Visual", 28598),
			EncodingTable.ENC("iso-8859-8-i", 38598),
			EncodingTable.ENC("iso-8859-9", 28599),
			EncodingTable.ENC("iso-ir-100", 28591),
			EncodingTable.ENC("iso-ir-101", 28592),
			EncodingTable.ENC("iso-ir-109", 28593),
			EncodingTable.ENC("iso-ir-110", 28594),
			EncodingTable.ENC("iso-ir-126", 28597),
			EncodingTable.ENC("iso-ir-127", 28596),
			EncodingTable.ENC("iso-ir-138", 28598),
			EncodingTable.ENC("iso-ir-144", 28595),
			EncodingTable.ENC("iso-ir-148", 28599),
			EncodingTable.ENC("iso-ir-149", 949),
			EncodingTable.ENC("iso-ir-58", 936),
			EncodingTable.ENC("iso-ir-6", 20127),
			EncodingTable.ENC("ISO646-US", 20127),
			EncodingTable.ENC("iso8859-1", 28591),
			EncodingTable.ENC("iso8859-2", 28592),
			EncodingTable.ENC("ISO_646.irv:1991", 20127),
			EncodingTable.ENC("iso_8859-1", 28591),
			EncodingTable.ENC("ISO_8859-15", 28605),
			EncodingTable.ENC("iso_8859-1:1987", 28591),
			EncodingTable.ENC("iso_8859-2", 28592),
			EncodingTable.ENC("iso_8859-2:1987", 28592),
			EncodingTable.ENC("ISO_8859-3", 28593),
			EncodingTable.ENC("ISO_8859-3:1988", 28593),
			EncodingTable.ENC("ISO_8859-4", 28594),
			EncodingTable.ENC("ISO_8859-4:1988", 28594),
			EncodingTable.ENC("ISO_8859-5", 28595),
			EncodingTable.ENC("ISO_8859-5:1988", 28595),
			EncodingTable.ENC("ISO_8859-6", 28596),
			EncodingTable.ENC("ISO_8859-6:1987", 28596),
			EncodingTable.ENC("ISO_8859-7", 28597),
			EncodingTable.ENC("ISO_8859-7:1987", 28597),
			EncodingTable.ENC("ISO_8859-8", 28598),
			EncodingTable.ENC("ISO_8859-8:1988", 28598),
			EncodingTable.ENC("ISO_8859-9", 28599),
			EncodingTable.ENC("ISO_8859-9:1989", 28599),
			EncodingTable.ENC("Johab", 1361),
			EncodingTable.ENC("koi", 20866),
			EncodingTable.ENC("koi8", 20866),
			EncodingTable.ENC("koi8-r", 20866),
			EncodingTable.ENC("koi8-ru", 21866),
			EncodingTable.ENC("koi8-u", 21866),
			EncodingTable.ENC("koi8r", 20866),
			EncodingTable.ENC("korean", 949),
			EncodingTable.ENC("ks-c-5601", 949),
			EncodingTable.ENC("ks-c5601", 949),
			EncodingTable.ENC("KSC5601", 949),
			EncodingTable.ENC("KSC_5601", 949),
			EncodingTable.ENC("ks_c_5601", 949),
			EncodingTable.ENC("ks_c_5601-1987", 949),
			EncodingTable.ENC("ks_c_5601-1989", 949),
			EncodingTable.ENC("ks_c_5601_1987", 949),
			EncodingTable.ENC("l1", 28591),
			EncodingTable.ENC("l2", 28592),
			EncodingTable.ENC("l3", 28593),
			EncodingTable.ENC("l4", 28594),
			EncodingTable.ENC("l5", 28599),
			EncodingTable.ENC("l9", 28605),
			EncodingTable.ENC("latin1", 28591),
			EncodingTable.ENC("latin2", 28592),
			EncodingTable.ENC("latin3", 28593),
			EncodingTable.ENC("latin4", 28594),
			EncodingTable.ENC("latin5", 28599),
			EncodingTable.ENC("latin9", 28605),
			EncodingTable.ENC("logical", 28598),
			EncodingTable.ENC("macintosh", 10000),
			EncodingTable.ENC("ms_Kanji", 932),
			EncodingTable.ENC("Norwegian", 20108),
			EncodingTable.ENC("NS_4551-1", 20108),
			EncodingTable.ENC("PC-Multilingual-850+euro", 858),
			EncodingTable.ENC("SEN_850200_B", 20107),
			EncodingTable.ENC("shift-jis", 932),
			EncodingTable.ENC("shift_jis", 932),
			EncodingTable.ENC("sjis", 932),
			EncodingTable.ENC("Swedish", 20107),
			EncodingTable.ENC("TIS-620", 874),
			EncodingTable.ENC("ucs-2", 1200),
			EncodingTable.ENC("unicode", 1200),
			EncodingTable.ENC("unicode-1-1-utf-7", 65000),
			EncodingTable.ENC("unicode-1-1-utf-8", 65001),
			EncodingTable.ENC("unicode-2-0-utf-7", 65000),
			EncodingTable.ENC("unicode-2-0-utf-8", 65001),
			EncodingTable.ENC("unicodeFFFE", 1201),
			EncodingTable.ENC("us", 20127),
			EncodingTable.ENC("us-ascii", 20127),
			EncodingTable.ENC("utf-16", 1200),
			EncodingTable.ENC("UTF-16BE", 1201),
			EncodingTable.ENC("UTF-16LE", 1200),
			EncodingTable.ENC("utf-32", 12000),
			EncodingTable.ENC("UTF-32BE", 12001),
			EncodingTable.ENC("UTF-32LE", 12000),
			EncodingTable.ENC("utf-7", 65000),
			EncodingTable.ENC("utf-8", 65001),
			EncodingTable.ENC("visual", 28598),
			EncodingTable.ENC("windows-1250", 1250),
			EncodingTable.ENC("windows-1251", 1251),
			EncodingTable.ENC("windows-1252", 1252),
			EncodingTable.ENC("windows-1253", 1253),
			EncodingTable.ENC("Windows-1254", 1254),
			EncodingTable.ENC("windows-1255", 1255),
			EncodingTable.ENC("windows-1256", 1256),
			EncodingTable.ENC("windows-1257", 1257),
			EncodingTable.ENC("windows-1258", 1258),
			EncodingTable.ENC("windows-874", 874),
			EncodingTable.ENC("x-ansi", 1252),
			EncodingTable.ENC("x-Chinese-CNS", 20000),
			EncodingTable.ENC("x-Chinese-Eten", 20002),
			EncodingTable.ENC("x-cp1250", 1250),
			EncodingTable.ENC("x-cp1251", 1251),
			EncodingTable.ENC("x-cp20001", 20001),
			EncodingTable.ENC("x-cp20003", 20003),
			EncodingTable.ENC("x-cp20004", 20004),
			EncodingTable.ENC("x-cp20005", 20005),
			EncodingTable.ENC("x-cp20261", 20261),
			EncodingTable.ENC("x-cp20269", 20269),
			EncodingTable.ENC("x-cp20936", 20936),
			EncodingTable.ENC("x-cp20949", 20949),
			EncodingTable.ENC("x-cp50227", 50227),
			EncodingTable.ENC("X-EBCDIC-KoreanExtended", 20833),
			EncodingTable.ENC("x-euc", 51932),
			EncodingTable.ENC("x-euc-cn", 51936),
			EncodingTable.ENC("x-euc-jp", 51932),
			EncodingTable.ENC("x-Europa", 29001),
			EncodingTable.ENC("x-IA5", 20105),
			EncodingTable.ENC("x-IA5-German", 20106),
			EncodingTable.ENC("x-IA5-Norwegian", 20108),
			EncodingTable.ENC("x-IA5-Swedish", 20107),
			EncodingTable.ENC("x-iscii-as", 57006),
			EncodingTable.ENC("x-iscii-be", 57003),
			EncodingTable.ENC("x-iscii-de", 57002),
			EncodingTable.ENC("x-iscii-gu", 57010),
			EncodingTable.ENC("x-iscii-ka", 57008),
			EncodingTable.ENC("x-iscii-ma", 57009),
			EncodingTable.ENC("x-iscii-or", 57007),
			EncodingTable.ENC("x-iscii-pa", 57011),
			EncodingTable.ENC("x-iscii-ta", 57004),
			EncodingTable.ENC("x-iscii-te", 57005),
			EncodingTable.ENC("x-mac-arabic", 10004),
			EncodingTable.ENC("x-mac-ce", 10029),
			EncodingTable.ENC("x-mac-chinesesimp", 10008),
			EncodingTable.ENC("x-mac-chinesetrad", 10002),
			EncodingTable.ENC("x-mac-croatian", 10082),
			EncodingTable.ENC("x-mac-cyrillic", 10007),
			EncodingTable.ENC("x-mac-greek", 10006),
			EncodingTable.ENC("x-mac-hebrew", 10005),
			EncodingTable.ENC("x-mac-icelandic", 10079),
			EncodingTable.ENC("x-mac-japanese", 10001),
			EncodingTable.ENC("x-mac-korean", 10003),
			EncodingTable.ENC("x-mac-romanian", 10010),
			EncodingTable.ENC("x-mac-thai", 10021),
			EncodingTable.ENC("x-mac-turkish", 10081),
			EncodingTable.ENC("x-mac-ukrainian", 10017),
			EncodingTable.ENC("x-ms-cp932", 932),
			EncodingTable.ENC("x-sjis", 932),
			EncodingTable.ENC("x-unicode-1-1-utf-7", 65000),
			EncodingTable.ENC("x-unicode-1-1-utf-8", 65001),
			EncodingTable.ENC("x-unicode-2-0-utf-7", 65000),
			EncodingTable.ENC("x-unicode-2-0-utf-8", 65001),
			EncodingTable.ENC("x-x-big5", 950)
		};

		internal static InternalCodePageDataItem[] codePageDataPtr = new InternalCodePageDataItem[]
		{
			EncodingTable.MapCodePageDataItem(37, 1252, "IBM037", 0U),
			EncodingTable.MapCodePageDataItem(437, 1252, "IBM437", 0U),
			EncodingTable.MapCodePageDataItem(500, 1252, "IBM500", 0U),
			EncodingTable.MapCodePageDataItem(708, 1256, "ASMO-708", 514U),
			EncodingTable.MapCodePageDataItem(737, 1253, "ibm737", 0U),
			EncodingTable.MapCodePageDataItem(775, 1257, "ibm775", 0U),
			EncodingTable.MapCodePageDataItem(850, 1252, "ibm850", 0U),
			EncodingTable.MapCodePageDataItem(852, 1250, "ibm852", 514U),
			EncodingTable.MapCodePageDataItem(855, 1252, "IBM855", 0U),
			EncodingTable.MapCodePageDataItem(857, 1254, "ibm857", 0U),
			EncodingTable.MapCodePageDataItem(858, 1252, "IBM00858", 0U),
			EncodingTable.MapCodePageDataItem(860, 1252, "IBM860", 0U),
			EncodingTable.MapCodePageDataItem(861, 1252, "ibm861", 0U),
			EncodingTable.MapCodePageDataItem(862, 1255, "DOS-862", 514U),
			EncodingTable.MapCodePageDataItem(863, 1252, "IBM863", 0U),
			EncodingTable.MapCodePageDataItem(864, 1256, "IBM864", 0U),
			EncodingTable.MapCodePageDataItem(865, 1252, "IBM865", 0U),
			EncodingTable.MapCodePageDataItem(866, 1251, "cp866", 514U),
			EncodingTable.MapCodePageDataItem(869, 1253, "ibm869", 0U),
			EncodingTable.MapCodePageDataItem(870, 1250, "IBM870", 0U),
			EncodingTable.MapCodePageDataItem(874, 874, "windows-874", 771U),
			EncodingTable.MapCodePageDataItem(875, 1253, "cp875", 0U),
			EncodingTable.MapCodePageDataItem(932, 932, "|shift_jis|iso-2022-jp|iso-2022-jp", 771U),
			EncodingTable.MapCodePageDataItem(936, 936, "gb2312", 771U),
			EncodingTable.MapCodePageDataItem(949, 949, "ks_c_5601-1987", 771U),
			EncodingTable.MapCodePageDataItem(950, 950, "big5", 771U),
			EncodingTable.MapCodePageDataItem(1026, 1254, "IBM1026", 0U),
			EncodingTable.MapCodePageDataItem(1047, 1252, "IBM01047", 0U),
			EncodingTable.MapCodePageDataItem(1140, 1252, "IBM01140", 0U),
			EncodingTable.MapCodePageDataItem(1141, 1252, "IBM01141", 0U),
			EncodingTable.MapCodePageDataItem(1142, 1252, "IBM01142", 0U),
			EncodingTable.MapCodePageDataItem(1143, 1252, "IBM01143", 0U),
			EncodingTable.MapCodePageDataItem(1144, 1252, "IBM01144", 0U),
			EncodingTable.MapCodePageDataItem(1145, 1252, "IBM01145", 0U),
			EncodingTable.MapCodePageDataItem(1146, 1252, "IBM01146", 0U),
			EncodingTable.MapCodePageDataItem(1147, 1252, "IBM01147", 0U),
			EncodingTable.MapCodePageDataItem(1148, 1252, "IBM01148", 0U),
			EncodingTable.MapCodePageDataItem(1149, 1252, "IBM01149", 0U),
			EncodingTable.MapCodePageDataItem(1200, 1200, "utf-16", 512U),
			EncodingTable.MapCodePageDataItem(1201, 1200, "utf-16BE", 0U),
			EncodingTable.MapCodePageDataItem(1250, 1250, "|windows-1250|windows-1250|iso-8859-2", 771U),
			EncodingTable.MapCodePageDataItem(1251, 1251, "|windows-1251|windows-1251|koi8-r", 771U),
			EncodingTable.MapCodePageDataItem(1252, 1252, "|Windows-1252|Windows-1252|iso-8859-1", 771U),
			EncodingTable.MapCodePageDataItem(1253, 1253, "|windows-1253|windows-1253|iso-8859-7", 771U),
			EncodingTable.MapCodePageDataItem(1254, 1254, "|windows-1254|windows-1254|iso-8859-9", 771U),
			EncodingTable.MapCodePageDataItem(1255, 1255, "windows-1255", 771U),
			EncodingTable.MapCodePageDataItem(1256, 1256, "windows-1256", 771U),
			EncodingTable.MapCodePageDataItem(1257, 1257, "windows-1257", 771U),
			EncodingTable.MapCodePageDataItem(1258, 1258, "windows-1258", 771U),
			EncodingTable.MapCodePageDataItem(10000, 1252, "macintosh", 0U),
			EncodingTable.MapCodePageDataItem(10079, 1252, "x-mac-icelandic", 0U),
			EncodingTable.MapCodePageDataItem(12000, 1200, "utf-32", 0U),
			EncodingTable.MapCodePageDataItem(12001, 1200, "utf-32BE", 0U),
			EncodingTable.MapCodePageDataItem(20127, 1252, "us-ascii", 257U),
			EncodingTable.MapCodePageDataItem(20273, 1252, "IBM273", 0U),
			EncodingTable.MapCodePageDataItem(20277, 1252, "IBM277", 0U),
			EncodingTable.MapCodePageDataItem(20278, 1252, "IBM278", 0U),
			EncodingTable.MapCodePageDataItem(20280, 1252, "IBM280", 0U),
			EncodingTable.MapCodePageDataItem(20284, 1252, "IBM284", 0U),
			EncodingTable.MapCodePageDataItem(20285, 1252, "IBM285", 0U),
			EncodingTable.MapCodePageDataItem(20290, 932, "IBM290", 0U),
			EncodingTable.MapCodePageDataItem(20297, 1252, "IBM297", 0U),
			EncodingTable.MapCodePageDataItem(20420, 1256, "IBM420", 0U),
			EncodingTable.MapCodePageDataItem(20424, 1255, "IBM424", 0U),
			EncodingTable.MapCodePageDataItem(20866, 1251, "koi8-r", 771U),
			EncodingTable.MapCodePageDataItem(20871, 1252, "IBM871", 0U),
			EncodingTable.MapCodePageDataItem(21025, 1251, "cp1025", 0U),
			EncodingTable.MapCodePageDataItem(21866, 1251, "koi8-u", 771U),
			EncodingTable.MapCodePageDataItem(28591, 1252, "iso-8859-1", 771U),
			EncodingTable.MapCodePageDataItem(28592, 1250, "iso-8859-2", 771U),
			EncodingTable.MapCodePageDataItem(28593, 1254, "iso-8859-3", 257U),
			EncodingTable.MapCodePageDataItem(28594, 1257, "iso-8859-4", 771U),
			EncodingTable.MapCodePageDataItem(28595, 1251, "iso-8859-5", 771U),
			EncodingTable.MapCodePageDataItem(28596, 1256, "iso-8859-6", 771U),
			EncodingTable.MapCodePageDataItem(28597, 1253, "iso-8859-7", 771U),
			EncodingTable.MapCodePageDataItem(28598, 1255, "iso-8859-8", 514U),
			EncodingTable.MapCodePageDataItem(28599, 1254, "iso-8859-9", 771U),
			EncodingTable.MapCodePageDataItem(28605, 1252, "iso-8859-15", 769U),
			EncodingTable.MapCodePageDataItem(38598, 1255, "iso-8859-8-i", 771U),
			EncodingTable.MapCodePageDataItem(50220, 932, "iso-2022-jp", 257U),
			EncodingTable.MapCodePageDataItem(50221, 932, "|csISO2022JP|iso-2022-jp|iso-2022-jp", 769U),
			EncodingTable.MapCodePageDataItem(50222, 932, "iso-2022-jp", 0U),
			EncodingTable.MapCodePageDataItem(51932, 932, "euc-jp", 771U),
			EncodingTable.MapCodePageDataItem(51949, 949, "euc-kr", 257U),
			EncodingTable.MapCodePageDataItem(54936, 936, "GB18030", 771U),
			EncodingTable.MapCodePageDataItem(57002, 57002, "x-iscii-de", 0U),
			EncodingTable.MapCodePageDataItem(57003, 57003, "x-iscii-be", 0U),
			EncodingTable.MapCodePageDataItem(57004, 57004, "x-iscii-ta", 0U),
			EncodingTable.MapCodePageDataItem(57005, 57005, "x-iscii-te", 0U),
			EncodingTable.MapCodePageDataItem(57006, 57006, "x-iscii-as", 0U),
			EncodingTable.MapCodePageDataItem(57007, 57007, "x-iscii-or", 0U),
			EncodingTable.MapCodePageDataItem(57008, 57008, "x-iscii-ka", 0U),
			EncodingTable.MapCodePageDataItem(57009, 57009, "x-iscii-ma", 0U),
			EncodingTable.MapCodePageDataItem(57010, 57010, "x-iscii-gu", 0U),
			EncodingTable.MapCodePageDataItem(57011, 57011, "x-iscii-pa", 0U),
			EncodingTable.MapCodePageDataItem(65000, 1200, "utf-7", 257U),
			EncodingTable.MapCodePageDataItem(65001, 1200, "utf-8", 771U),
			EncodingTable.MapCodePageDataItem(0, 0, null, 0U)
		};

		private const int MIMECONTF_MAILNEWS = 1;

		private const int MIMECONTF_BROWSER = 2;

		private const int MIMECONTF_MINIMAL = 4;

		private const int MIMECONTF_IMPORT = 8;

		private const int MIMECONTF_SAVABLE_MAILNEWS = 256;

		private const int MIMECONTF_SAVABLE_BROWSER = 512;

		private const int MIMECONTF_EXPORT = 1024;

		private const int MIMECONTF_PRIVCONVERTER = 65536;

		private const int MIMECONTF_VALID = 131072;

		private const int MIMECONTF_VALID_NLS = 262144;

		private const int MIMECONTF_MIME_IE4 = 268435456;

		private const int MIMECONTF_MIME_LATEST = 536870912;

		private const int MIMECONTF_MIME_REGISTRY = 1073741824;

		private static int lastEncodingItem = EncodingTable.GetNumEncodingItems() - 1;

		private static volatile int lastCodePageItem;

		private static Dictionary<string, int> hashByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<int, CodePageDataItem> hashByCodePage = new Dictionary<int, CodePageDataItem>();
	}
}
