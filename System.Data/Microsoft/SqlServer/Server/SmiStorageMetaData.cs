using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace Microsoft.SqlServer.Server
{
	internal class SmiStorageMetaData : SmiExtendedMetaData
	{
		internal SmiStorageMetaData(SqlDbType dbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, bool isMultiValued, IList<SmiExtendedMetaData> fieldMetaData, SmiMetaDataPropertyCollection extendedProperties, string name, string typeSpecificNamePart1, string typeSpecificNamePart2, string typeSpecificNamePart3, bool allowsDBNull, string serverName, string catalogName, string schemaName, string tableName, string columnName, SqlBoolean isKey, bool isIdentity, bool isColumnSet)
			: base(dbType, maxLength, precision, scale, localeId, compareOptions, isMultiValued, fieldMetaData, extendedProperties, name, typeSpecificNamePart1, typeSpecificNamePart2, typeSpecificNamePart3)
		{
			this._allowsDBNull = allowsDBNull;
			this._serverName = serverName;
			this._catalogName = catalogName;
			this._schemaName = schemaName;
			this._tableName = tableName;
			this._columnName = columnName;
			this._isKey = isKey;
			this._isIdentity = isIdentity;
			this._isColumnSet = isColumnSet;
		}

		internal SqlBoolean IsKey
		{
			get
			{
				return this._isKey;
			}
		}

		private bool _allowsDBNull;

		private string _serverName;

		private string _catalogName;

		private string _schemaName;

		private string _tableName;

		private string _columnName;

		private SqlBoolean _isKey;

		private bool _isIdentity;

		private bool _isColumnSet;
	}
}
