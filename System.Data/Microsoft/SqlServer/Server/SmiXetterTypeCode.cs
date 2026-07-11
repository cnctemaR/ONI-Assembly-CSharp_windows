using System;

namespace Microsoft.SqlServer.Server
{
	internal enum SmiXetterTypeCode
	{
		XetBoolean,
		XetByte,
		XetBytes,
		XetChars,
		XetString,
		XetInt16,
		XetInt32,
		XetInt64,
		XetSingle,
		XetDouble,
		XetSqlDecimal,
		XetDateTime,
		XetGuid,
		GetVariantMetaData,
		GetXet,
		XetTime,
		XetTimeSpan = 15,
		XetDateTimeOffset
	}
}
