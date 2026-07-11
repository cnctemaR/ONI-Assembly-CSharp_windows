using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace Microsoft.SqlServer.Server
{
	internal class SmiQueryMetaData : SmiStorageMetaData
	{
		internal SmiQueryMetaData(SqlDbType dbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, bool isMultiValued, IList<SmiExtendedMetaData> fieldMetaData, SmiMetaDataPropertyCollection extendedProperties, string name, string typeSpecificNamePart1, string typeSpecificNamePart2, string typeSpecificNamePart3, bool allowsDBNull, string serverName, string catalogName, string schemaName, string tableName, string columnName, SqlBoolean isKey, bool isIdentity, bool isReadOnly, SqlBoolean isExpression, SqlBoolean isAliased, SqlBoolean isHidden)
			: this(dbType, maxLength, precision, scale, localeId, compareOptions, isMultiValued, fieldMetaData, extendedProperties, name, typeSpecificNamePart1, typeSpecificNamePart2, typeSpecificNamePart3, allowsDBNull, serverName, catalogName, schemaName, tableName, columnName, isKey, isIdentity, false, isReadOnly, isExpression, isAliased, isHidden)
		{
		}

		internal SmiQueryMetaData(SqlDbType dbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, bool isMultiValued, IList<SmiExtendedMetaData> fieldMetaData, SmiMetaDataPropertyCollection extendedProperties, string name, string typeSpecificNamePart1, string typeSpecificNamePart2, string typeSpecificNamePart3, bool allowsDBNull, string serverName, string catalogName, string schemaName, string tableName, string columnName, SqlBoolean isKey, bool isIdentity, bool isColumnSet, bool isReadOnly, SqlBoolean isExpression, SqlBoolean isAliased, SqlBoolean isHidden)
			: base(dbType, maxLength, precision, scale, localeId, compareOptions, isMultiValued, fieldMetaData, extendedProperties, name, typeSpecificNamePart1, typeSpecificNamePart2, typeSpecificNamePart3, allowsDBNull, serverName, catalogName, schemaName, tableName, columnName, isKey, isIdentity, isColumnSet)
		{
			this._isReadOnly = isReadOnly;
			this._isExpression = isExpression;
			this._isAliased = isAliased;
			this._isHidden = isHidden;
		}

		private bool _isReadOnly;

		private SqlBoolean _isExpression;

		private SqlBoolean _isAliased;

		private SqlBoolean _isHidden;
	}
}
