using System;

namespace System.Data.SqlClient
{
	internal sealed class _ColumnMapping
	{
		internal _ColumnMapping(int columnId, _SqlMetaData metadata)
		{
			this._sourceColumnOrdinal = columnId;
			this._metadata = metadata;
		}

		internal int _sourceColumnOrdinal;

		internal _SqlMetaData _metadata;
	}
}
