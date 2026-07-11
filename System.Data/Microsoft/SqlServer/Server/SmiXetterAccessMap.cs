using System;

namespace Microsoft.SqlServer.Server
{
	internal class SmiXetterAccessMap
	{
		internal static bool IsSetterAccessValid(SmiMetaData metaData, SmiXetterTypeCode xetterType)
		{
			return SmiXetterAccessMap.s_isSetterAccessValid[(int)metaData.SqlDbType, (int)xetterType];
		}

		private const bool X = true;

		private const bool _ = false;

		private static bool[,] s_isSetterAccessValid = new bool[,]
		{
			{
				false, false, false, false, false, false, false, true, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, true, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				true, false, false, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, true, true, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, true, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				true, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, true,
				false, false, false, false, false, false, false
			},
			{
				false, false, true, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, true, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, true, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, true, true, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, true, true, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, true, true, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, true, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, false, true, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, true, false, false, false, false, false
			},
			{
				false, false, false, false, false, true, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, true, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, true, true, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, true, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, true, false, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, true, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, true, true, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, false, true, true
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, true, false, true, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, true, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, false, true, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, true, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, false, false, true, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, true, false, false, false, false, false
			},
			{
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, false, false, false, true
			}
		};
	}
}
