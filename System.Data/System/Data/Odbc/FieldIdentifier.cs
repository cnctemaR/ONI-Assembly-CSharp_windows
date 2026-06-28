using System;

namespace System.Data.Odbc
{
	internal enum FieldIdentifier : short
	{
		AutoUniqueValue = 11,
		BaseColumnName = 22,
		BaseTableName,
		CaseSensitive = 12,
		CatelogName = 17,
		ConsiseType = 2,
		Count = 1001,
		DisplaySize = 6,
		FixedPrecScale = 9,
		Label = 18,
		Length = 1003,
		LiteralPrefix = 27,
		LiteralSuffix,
		LocalTypeName,
		Name = 1011,
		Nullable = 1008,
		NumPrecRadix = 32,
		OctetLength = 1013,
		Precision = 1005,
		Scale,
		SchemaName = 16,
		Searchable = 13,
		TableName = 15,
		Type = 1002,
		TypeName = 14,
		Unnamed = 1012,
		Unsigned = 8,
		Updatable = 10
	}
}
