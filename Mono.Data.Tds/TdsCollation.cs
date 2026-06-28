using System;

namespace Mono.Data.Tds
{
	internal static class TdsCollation
	{
		public static int LCID(byte[] collation)
		{
			if (collation == null)
			{
				return -1;
			}
			return (int)collation[0] | ((int)collation[1] << 8) | ((int)(collation[2] & 15) << 16);
		}

		public static int CollationFlags(byte[] collation)
		{
			if (collation == null)
			{
				return -1;
			}
			return (int)(collation[2] & 240) | ((int)(collation[3] & 15) << 4);
		}

		public static int Version(byte[] collation)
		{
			if (collation == null)
			{
				return -1;
			}
			return (int)(collation[3] & 240);
		}

		public static int SortId(byte[] collation)
		{
			if (collation == null)
			{
				return -1;
			}
			return (int)collation[4];
		}
	}
}
