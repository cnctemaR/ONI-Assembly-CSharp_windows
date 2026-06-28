using System;
using System.Collections;
using System.Text;

namespace Mono.Data.Tds
{
	internal static class TdsCharset
	{
		static TdsCharset()
		{
			TdsCharset.lcidCodes[1078] = 1252;
			TdsCharset.lcidCodes[1052] = 1250;
			TdsCharset.lcidCodes[1025] = 1256;
			TdsCharset.lcidCodes[2049] = 1256;
			TdsCharset.lcidCodes[3073] = 1256;
			TdsCharset.lcidCodes[4097] = 1256;
			TdsCharset.lcidCodes[5121] = 1256;
			TdsCharset.lcidCodes[6145] = 1256;
			TdsCharset.lcidCodes[7169] = 1256;
			TdsCharset.lcidCodes[8193] = 1256;
			TdsCharset.lcidCodes[9217] = 1256;
			TdsCharset.lcidCodes[10241] = 1256;
			TdsCharset.lcidCodes[11265] = 1256;
			TdsCharset.lcidCodes[12289] = 1256;
			TdsCharset.lcidCodes[13313] = 1256;
			TdsCharset.lcidCodes[14337] = 1256;
			TdsCharset.lcidCodes[15361] = 1256;
			TdsCharset.lcidCodes[16385] = 1256;
			TdsCharset.lcidCodes[1069] = 1252;
			TdsCharset.lcidCodes[1059] = 1251;
			TdsCharset.lcidCodes[1026] = 1251;
			TdsCharset.lcidCodes[1027] = 1252;
			TdsCharset.lcidCodes[197636] = 950;
			TdsCharset.lcidCodes[1028] = 950;
			TdsCharset.lcidCodes[2052] = 936;
			TdsCharset.lcidCodes[133124] = 936;
			TdsCharset.lcidCodes[4100] = 936;
			TdsCharset.lcidCodes[1050] = 1250;
			TdsCharset.lcidCodes[1029] = 1250;
			TdsCharset.lcidCodes[1030] = 1252;
			TdsCharset.lcidCodes[1043] = 1252;
			TdsCharset.lcidCodes[2067] = 1252;
			TdsCharset.lcidCodes[1033] = 1252;
			TdsCharset.lcidCodes[2057] = 1252;
			TdsCharset.lcidCodes[4105] = 1252;
			TdsCharset.lcidCodes[5129] = 1252;
			TdsCharset.lcidCodes[3081] = 1252;
			TdsCharset.lcidCodes[6153] = 1252;
			TdsCharset.lcidCodes[7177] = 1252;
			TdsCharset.lcidCodes[9225] = 1252;
			TdsCharset.lcidCodes[8201] = 1252;
			TdsCharset.lcidCodes[1061] = 1257;
			TdsCharset.lcidCodes[1080] = 1252;
			TdsCharset.lcidCodes[1065] = 1256;
			TdsCharset.lcidCodes[1035] = 1252;
			TdsCharset.lcidCodes[1036] = 1252;
			TdsCharset.lcidCodes[2060] = 1252;
			TdsCharset.lcidCodes[4108] = 1252;
			TdsCharset.lcidCodes[3084] = 1252;
			TdsCharset.lcidCodes[5132] = 1252;
			TdsCharset.lcidCodes[66615] = 1252;
			TdsCharset.lcidCodes[66567] = 1252;
			TdsCharset.lcidCodes[1031] = 1252;
			TdsCharset.lcidCodes[2055] = 1252;
			TdsCharset.lcidCodes[3079] = 1252;
			TdsCharset.lcidCodes[4103] = 1252;
			TdsCharset.lcidCodes[5127] = 1252;
			TdsCharset.lcidCodes[1032] = 1253;
			TdsCharset.lcidCodes[1037] = 1255;
			TdsCharset.lcidCodes[1081] = 65001;
			TdsCharset.lcidCodes[1038] = 1250;
			TdsCharset.lcidCodes[4174] = 1250;
			TdsCharset.lcidCodes[1039] = 1252;
			TdsCharset.lcidCodes[1057] = 1252;
			TdsCharset.lcidCodes[1040] = 1252;
			TdsCharset.lcidCodes[2064] = 1252;
			TdsCharset.lcidCodes[1041] = 932;
			TdsCharset.lcidCodes[66577] = 932;
			TdsCharset.lcidCodes[1042] = 949;
			TdsCharset.lcidCodes[1042] = 949;
			TdsCharset.lcidCodes[1062] = 1257;
			TdsCharset.lcidCodes[1063] = 1257;
			TdsCharset.lcidCodes[2087] = 1257;
			TdsCharset.lcidCodes[1052] = 1251;
			TdsCharset.lcidCodes[1044] = 1252;
			TdsCharset.lcidCodes[2068] = 1252;
			TdsCharset.lcidCodes[1045] = 1250;
			TdsCharset.lcidCodes[2070] = 1252;
			TdsCharset.lcidCodes[1046] = 1252;
			TdsCharset.lcidCodes[1048] = 1250;
			TdsCharset.lcidCodes[1049] = 1251;
			TdsCharset.lcidCodes[2074] = 1251;
			TdsCharset.lcidCodes[3098] = 1251;
			TdsCharset.lcidCodes[1051] = 1250;
			TdsCharset.lcidCodes[1060] = 1250;
			TdsCharset.lcidCodes[2058] = 1252;
			TdsCharset.lcidCodes[1034] = 1252;
			TdsCharset.lcidCodes[3082] = 1252;
			TdsCharset.lcidCodes[4106] = 1252;
			TdsCharset.lcidCodes[5130] = 1252;
			TdsCharset.lcidCodes[6154] = 1252;
			TdsCharset.lcidCodes[7178] = 1252;
			TdsCharset.lcidCodes[8202] = 1252;
			TdsCharset.lcidCodes[9226] = 1252;
			TdsCharset.lcidCodes[10250] = 1252;
			TdsCharset.lcidCodes[11274] = 1252;
			TdsCharset.lcidCodes[12298] = 1252;
			TdsCharset.lcidCodes[13322] = 1252;
			TdsCharset.lcidCodes[14346] = 1252;
			TdsCharset.lcidCodes[15370] = 1252;
			TdsCharset.lcidCodes[16394] = 1252;
			TdsCharset.lcidCodes[1053] = 1252;
			TdsCharset.lcidCodes[1054] = 874;
			TdsCharset.lcidCodes[1055] = 1254;
			TdsCharset.lcidCodes[1058] = 1251;
			TdsCharset.lcidCodes[1056] = 1256;
			TdsCharset.lcidCodes[1066] = 1258;
			TdsCharset.sortCodes[30] = 437;
			TdsCharset.sortCodes[31] = 437;
			TdsCharset.sortCodes[32] = 437;
			TdsCharset.sortCodes[33] = 437;
			TdsCharset.sortCodes[34] = 437;
			TdsCharset.sortCodes[40] = 850;
			TdsCharset.sortCodes[41] = 850;
			TdsCharset.sortCodes[42] = 850;
			TdsCharset.sortCodes[43] = 850;
			TdsCharset.sortCodes[44] = 850;
			TdsCharset.sortCodes[49] = 850;
			TdsCharset.sortCodes[50] = 1252;
			TdsCharset.sortCodes[51] = 1252;
			TdsCharset.sortCodes[52] = 1252;
			TdsCharset.sortCodes[53] = 1252;
			TdsCharset.sortCodes[54] = 1252;
			TdsCharset.sortCodes[55] = 850;
			TdsCharset.sortCodes[56] = 850;
			TdsCharset.sortCodes[57] = 850;
			TdsCharset.sortCodes[58] = 850;
			TdsCharset.sortCodes[59] = 850;
			TdsCharset.sortCodes[60] = 850;
			TdsCharset.sortCodes[61] = 850;
			TdsCharset.sortCodes[71] = 1252;
			TdsCharset.sortCodes[72] = 1252;
			TdsCharset.sortCodes[73] = 1252;
			TdsCharset.sortCodes[74] = 1252;
			TdsCharset.sortCodes[75] = 1252;
			TdsCharset.sortCodes[80] = 1250;
			TdsCharset.sortCodes[81] = 1250;
			TdsCharset.sortCodes[82] = 1250;
			TdsCharset.sortCodes[83] = 1250;
			TdsCharset.sortCodes[84] = 1250;
			TdsCharset.sortCodes[85] = 1250;
			TdsCharset.sortCodes[86] = 1250;
			TdsCharset.sortCodes[87] = 1250;
			TdsCharset.sortCodes[88] = 1250;
			TdsCharset.sortCodes[89] = 1250;
			TdsCharset.sortCodes[90] = 1250;
			TdsCharset.sortCodes[91] = 1250;
			TdsCharset.sortCodes[92] = 1250;
			TdsCharset.sortCodes[93] = 1250;
			TdsCharset.sortCodes[94] = 1250;
			TdsCharset.sortCodes[95] = 1250;
			TdsCharset.sortCodes[96] = 1250;
			TdsCharset.sortCodes[97] = 1250;
			TdsCharset.sortCodes[98] = 1250;
			TdsCharset.sortCodes[104] = 1251;
			TdsCharset.sortCodes[105] = 1251;
			TdsCharset.sortCodes[106] = 1251;
			TdsCharset.sortCodes[107] = 1251;
			TdsCharset.sortCodes[108] = 1251;
			TdsCharset.sortCodes[112] = 1253;
			TdsCharset.sortCodes[113] = 1253;
			TdsCharset.sortCodes[114] = 1253;
			TdsCharset.sortCodes[120] = 1253;
			TdsCharset.sortCodes[121] = 1253;
			TdsCharset.sortCodes[124] = 1253;
			TdsCharset.sortCodes[128] = 1254;
			TdsCharset.sortCodes[129] = 1254;
			TdsCharset.sortCodes[130] = 1254;
			TdsCharset.sortCodes[136] = 1255;
			TdsCharset.sortCodes[137] = 1255;
			TdsCharset.sortCodes[138] = 1255;
			TdsCharset.sortCodes[144] = 1256;
			TdsCharset.sortCodes[145] = 1256;
			TdsCharset.sortCodes[146] = 1256;
			TdsCharset.sortCodes[152] = 1257;
			TdsCharset.sortCodes[153] = 1257;
			TdsCharset.sortCodes[154] = 1257;
			TdsCharset.sortCodes[155] = 1257;
			TdsCharset.sortCodes[156] = 1257;
			TdsCharset.sortCodes[157] = 1257;
			TdsCharset.sortCodes[158] = 1257;
			TdsCharset.sortCodes[159] = 1257;
			TdsCharset.sortCodes[160] = 1257;
			TdsCharset.sortCodes[183] = 1252;
			TdsCharset.sortCodes[184] = 1252;
			TdsCharset.sortCodes[185] = 1252;
			TdsCharset.sortCodes[186] = 1252;
			TdsCharset.sortCodes[192] = 932;
			TdsCharset.sortCodes[193] = 932;
			TdsCharset.sortCodes[194] = 949;
			TdsCharset.sortCodes[195] = 949;
			TdsCharset.sortCodes[196] = 950;
			TdsCharset.sortCodes[197] = 950;
			TdsCharset.sortCodes[198] = 936;
			TdsCharset.sortCodes[199] = 936;
			TdsCharset.sortCodes[200] = 932;
			TdsCharset.sortCodes[201] = 949;
			TdsCharset.sortCodes[202] = 950;
			TdsCharset.sortCodes[203] = 936;
			TdsCharset.sortCodes[204] = 874;
			TdsCharset.sortCodes[205] = 874;
			TdsCharset.sortCodes[206] = 874;
		}

		public static Encoding GetEncoding(byte[] collation)
		{
			if (TdsCollation.SortId(collation) != 0)
			{
				return TdsCharset.GetEncodingFromSortOrder(collation);
			}
			return TdsCharset.GetEncodingFromLCID(collation);
		}

		public static Encoding GetEncodingFromLCID(byte[] collation)
		{
			int num = TdsCollation.LCID(collation);
			return TdsCharset.GetEncodingFromLCID(num);
		}

		public static Encoding GetEncodingFromLCID(int lcid)
		{
			if (TdsCharset.lcidCodes[lcid] != null)
			{
				return Encoding.GetEncoding((int)TdsCharset.lcidCodes[lcid]);
			}
			return null;
		}

		public static Encoding GetEncodingFromSortOrder(byte[] collation)
		{
			int num = TdsCollation.SortId(collation);
			return TdsCharset.GetEncodingFromSortOrder(num);
		}

		public static Encoding GetEncodingFromSortOrder(int sortId)
		{
			if (TdsCharset.sortCodes[sortId] != null)
			{
				return Encoding.GetEncoding((int)TdsCharset.sortCodes[sortId]);
			}
			return null;
		}

		private static Hashtable lcidCodes = new Hashtable();

		private static Hashtable sortCodes = new Hashtable();
	}
}
