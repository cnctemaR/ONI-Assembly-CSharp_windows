using System;
using System.Collections.Generic;

namespace System.Data.SqlClient
{
	internal sealed class Result
	{
		internal Result(_SqlMetaDataSet metadata)
		{
			this._metadata = metadata;
			this._rowset = new List<Row>();
		}

		internal int Count
		{
			get
			{
				return this._rowset.Count;
			}
		}

		internal _SqlMetaDataSet MetaData
		{
			get
			{
				return this._metadata;
			}
		}

		internal Row this[int index]
		{
			get
			{
				return this._rowset[index];
			}
		}

		internal void AddRow(Row row)
		{
			this._rowset.Add(row);
		}

		private readonly _SqlMetaDataSet _metadata;

		private readonly List<Row> _rowset;
	}
}
