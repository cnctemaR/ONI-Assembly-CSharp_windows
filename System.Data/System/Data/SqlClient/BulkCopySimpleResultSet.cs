using System;
using System.Collections.Generic;

namespace System.Data.SqlClient
{
	internal sealed class BulkCopySimpleResultSet
	{
		internal BulkCopySimpleResultSet()
		{
			this._results = new List<Result>();
		}

		internal Result this[int idx]
		{
			get
			{
				return this._results[idx];
			}
		}

		internal void SetMetaData(_SqlMetaDataSet metadata)
		{
			this._resultSet = new Result(metadata);
			this._results.Add(this._resultSet);
			this._indexmap = new int[this._resultSet.MetaData.Length];
			for (int i = 0; i < this._indexmap.Length; i++)
			{
				this._indexmap[i] = i;
			}
		}

		internal int[] CreateIndexMap()
		{
			return this._indexmap;
		}

		internal object[] CreateRowBuffer()
		{
			Row row = new Row(this._resultSet.MetaData.Length);
			this._resultSet.AddRow(row);
			return row.DataFields;
		}

		private readonly List<Result> _results;

		private Result _resultSet;

		private int[] _indexmap;
	}
}
