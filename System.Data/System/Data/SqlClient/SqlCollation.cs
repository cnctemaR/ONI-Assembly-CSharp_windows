using System;
using System.Data.SqlTypes;

namespace System.Data.SqlClient
{
	internal sealed class SqlCollation
	{
		private static int FirstSupportedCollationVersion(int lcid)
		{
			if (lcid <= 1157)
			{
				if (lcid <= 1093)
				{
					if (lcid <= 1047)
					{
						if (lcid == 1044)
						{
							return 2;
						}
						if (lcid == 1047)
						{
							return 2;
						}
					}
					else
					{
						if (lcid == 1056)
						{
							return 2;
						}
						switch (lcid)
						{
						case 1065:
							return 2;
						case 1066:
						case 1067:
						case 1069:
							break;
						case 1068:
							return 2;
						case 1070:
							return 2;
						case 1071:
							return 1;
						default:
							switch (lcid)
							{
							case 1081:
								return 1;
							case 1082:
								return 2;
							case 1083:
								return 2;
							case 1087:
								return 1;
							case 1090:
								return 2;
							case 1091:
								return 1;
							case 1092:
								return 1;
							case 1093:
								return 2;
							}
							break;
						}
					}
				}
				else if (lcid <= 1114)
				{
					switch (lcid)
					{
					case 1101:
						return 2;
					case 1102:
					case 1103:
					case 1104:
						break;
					case 1105:
						return 2;
					case 1106:
						return 2;
					case 1107:
						return 2;
					case 1108:
						return 2;
					default:
						if (lcid == 1114)
						{
							return 1;
						}
						break;
					}
				}
				else
				{
					switch (lcid)
					{
					case 1121:
						return 2;
					case 1122:
						return 2;
					case 1123:
						return 2;
					case 1124:
						break;
					case 1125:
						return 1;
					default:
						if (lcid == 1133)
						{
							return 2;
						}
						switch (lcid)
						{
						case 1146:
							return 2;
						case 1148:
							return 2;
						case 1150:
							return 2;
						case 1152:
							return 2;
						case 1153:
							return 2;
						case 1155:
							return 2;
						case 1157:
							return 2;
						}
						break;
					}
				}
			}
			else if (lcid <= 2143)
			{
				if (lcid <= 2074)
				{
					if (lcid == 1164)
					{
						return 2;
					}
					if (lcid == 2074)
					{
						return 2;
					}
				}
				else
				{
					if (lcid == 2092)
					{
						return 2;
					}
					if (lcid == 2107)
					{
						return 2;
					}
					if (lcid == 2143)
					{
						return 2;
					}
				}
			}
			else if (lcid <= 3098)
			{
				if (lcid == 3076)
				{
					return 1;
				}
				if (lcid == 3098)
				{
					return 2;
				}
			}
			else
			{
				if (lcid == 5124)
				{
					return 2;
				}
				if (lcid == 5146)
				{
					return 2;
				}
				if (lcid == 8218)
				{
					return 2;
				}
			}
			return 0;
		}

		internal int LCID
		{
			get
			{
				return (int)(this.info & 1048575U);
			}
			set
			{
				int num = value & 1048575;
				int num2 = SqlCollation.FirstSupportedCollationVersion(num) << 28;
				this.info = (this.info & 32505856U) | (uint)num | (uint)num2;
			}
		}

		internal SqlCompareOptions SqlCompareOptions
		{
			get
			{
				SqlCompareOptions sqlCompareOptions = SqlCompareOptions.None;
				if ((this.info & 1048576U) != 0U)
				{
					sqlCompareOptions |= SqlCompareOptions.IgnoreCase;
				}
				if ((this.info & 2097152U) != 0U)
				{
					sqlCompareOptions |= SqlCompareOptions.IgnoreNonSpace;
				}
				if ((this.info & 4194304U) != 0U)
				{
					sqlCompareOptions |= SqlCompareOptions.IgnoreWidth;
				}
				if ((this.info & 8388608U) != 0U)
				{
					sqlCompareOptions |= SqlCompareOptions.IgnoreKanaType;
				}
				if ((this.info & 16777216U) != 0U)
				{
					sqlCompareOptions |= SqlCompareOptions.BinarySort;
				}
				return sqlCompareOptions;
			}
			set
			{
				uint num = 0U;
				if ((value & SqlCompareOptions.IgnoreCase) != SqlCompareOptions.None)
				{
					num |= 1048576U;
				}
				if ((value & SqlCompareOptions.IgnoreNonSpace) != SqlCompareOptions.None)
				{
					num |= 2097152U;
				}
				if ((value & SqlCompareOptions.IgnoreWidth) != SqlCompareOptions.None)
				{
					num |= 4194304U;
				}
				if ((value & SqlCompareOptions.IgnoreKanaType) != SqlCompareOptions.None)
				{
					num |= 8388608U;
				}
				if ((value & SqlCompareOptions.BinarySort) != SqlCompareOptions.None)
				{
					num |= 16777216U;
				}
				this.info = (this.info & 1048575U) | num;
			}
		}

		internal static bool AreSame(SqlCollation a, SqlCollation b)
		{
			if (a == null || b == null)
			{
				return a == b;
			}
			return a.info == b.info && a.sortId == b.sortId;
		}

		private const uint IgnoreCase = 1048576U;

		private const uint IgnoreNonSpace = 2097152U;

		private const uint IgnoreWidth = 4194304U;

		private const uint IgnoreKanaType = 8388608U;

		private const uint BinarySort = 16777216U;

		internal const uint MaskLcid = 1048575U;

		private const int LcidVersionBitOffset = 28;

		private const uint MaskLcidVersion = 4026531840U;

		private const uint MaskCompareOpt = 32505856U;

		internal uint info;

		internal byte sortId;
	}
}
