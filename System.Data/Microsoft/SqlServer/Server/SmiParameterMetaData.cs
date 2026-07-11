using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace Microsoft.SqlServer.Server
{
	internal sealed class SmiParameterMetaData : SmiExtendedMetaData
	{
		internal SmiParameterMetaData(SqlDbType dbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, bool isMultiValued, IList<SmiExtendedMetaData> fieldMetaData, SmiMetaDataPropertyCollection extendedProperties, string name, string typeSpecificNamePart1, string typeSpecificNamePart2, string typeSpecificNamePart3, ParameterDirection direction)
			: base(dbType, maxLength, precision, scale, localeId, compareOptions, isMultiValued, fieldMetaData, extendedProperties, name, typeSpecificNamePart1, typeSpecificNamePart2, typeSpecificNamePart3)
		{
			this._direction = direction;
		}

		internal ParameterDirection Direction
		{
			get
			{
				return this._direction;
			}
		}

		private ParameterDirection _direction;
	}
}
