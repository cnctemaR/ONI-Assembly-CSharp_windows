using System;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
	internal sealed class _SqlMetaDataSet
	{
		internal _SqlMetaDataSet(int count)
		{
			this._metaDataArray = new _SqlMetaData[count];
			for (int i = 0; i < this._metaDataArray.Length; i++)
			{
				this._metaDataArray[i] = new _SqlMetaData(i);
			}
		}

		private _SqlMetaDataSet(_SqlMetaDataSet original)
		{
			this.id = original.id;
			this.indexMap = original.indexMap;
			this.visibleColumns = original.visibleColumns;
			this.dbColumnSchema = original.dbColumnSchema;
			if (original._metaDataArray == null)
			{
				this._metaDataArray = null;
				return;
			}
			this._metaDataArray = new _SqlMetaData[original._metaDataArray.Length];
			for (int i = 0; i < this._metaDataArray.Length; i++)
			{
				this._metaDataArray[i] = (_SqlMetaData)original._metaDataArray[i].Clone();
			}
		}

		internal int Length
		{
			get
			{
				return this._metaDataArray.Length;
			}
		}

		internal _SqlMetaData this[int index]
		{
			get
			{
				return this._metaDataArray[index];
			}
			set
			{
				this._metaDataArray[index] = value;
			}
		}

		public object Clone()
		{
			return new _SqlMetaDataSet(this);
		}

		internal ushort id;

		internal int[] indexMap;

		internal int visibleColumns;

		internal DataTable schemaTable;

		private readonly _SqlMetaData[] _metaDataArray;

		internal ReadOnlyCollection<DbColumn> dbColumnSchema;
	}
}
