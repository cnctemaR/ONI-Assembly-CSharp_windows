using System;
using System.Collections.Generic;

namespace System.Data.SqlClient
{
	internal sealed class _SqlMetaDataSetCollection
	{
		internal _SqlMetaDataSetCollection()
		{
			this._altMetaDataSetArray = new List<_SqlMetaDataSet>();
		}

		internal void SetAltMetaData(_SqlMetaDataSet altMetaDataSet)
		{
			int id = (int)altMetaDataSet.id;
			for (int i = 0; i < this._altMetaDataSetArray.Count; i++)
			{
				if ((int)this._altMetaDataSetArray[i].id == id)
				{
					this._altMetaDataSetArray[i] = altMetaDataSet;
					return;
				}
			}
			this._altMetaDataSetArray.Add(altMetaDataSet);
		}

		internal _SqlMetaDataSet GetAltMetaData(int id)
		{
			foreach (_SqlMetaDataSet sqlMetaDataSet in this._altMetaDataSetArray)
			{
				if ((int)sqlMetaDataSet.id == id)
				{
					return sqlMetaDataSet;
				}
			}
			return null;
		}

		public object Clone()
		{
			_SqlMetaDataSetCollection sqlMetaDataSetCollection = new _SqlMetaDataSetCollection();
			sqlMetaDataSetCollection.metaDataSet = ((this.metaDataSet == null) ? null : ((_SqlMetaDataSet)this.metaDataSet.Clone()));
			foreach (_SqlMetaDataSet sqlMetaDataSet in this._altMetaDataSetArray)
			{
				sqlMetaDataSetCollection._altMetaDataSetArray.Add((_SqlMetaDataSet)sqlMetaDataSet.Clone());
			}
			return sqlMetaDataSetCollection;
		}

		private readonly List<_SqlMetaDataSet> _altMetaDataSetArray;

		internal _SqlMetaDataSet metaDataSet;
	}
}
