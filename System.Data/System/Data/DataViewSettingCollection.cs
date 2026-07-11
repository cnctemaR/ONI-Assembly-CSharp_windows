using System;
using System.Collections;
using System.ComponentModel;
using Unity;

namespace System.Data
{
	public class DataViewSettingCollection : ICollection, IEnumerable
	{
		internal DataViewSettingCollection(DataViewManager dataViewManager)
		{
			this._list = new Hashtable();
			base..ctor();
			if (dataViewManager == null)
			{
				throw ExceptionBuilder.ArgumentNull("dataViewManager");
			}
			this._dataViewManager = dataViewManager;
		}

		public virtual DataViewSetting this[DataTable table]
		{
			get
			{
				if (table == null)
				{
					throw ExceptionBuilder.ArgumentNull("table");
				}
				DataViewSetting dataViewSetting = (DataViewSetting)this._list[table];
				if (dataViewSetting == null)
				{
					dataViewSetting = new DataViewSetting();
					this[table] = dataViewSetting;
				}
				return dataViewSetting;
			}
			set
			{
				if (table == null)
				{
					throw ExceptionBuilder.ArgumentNull("table");
				}
				value.SetDataViewManager(this._dataViewManager);
				value.SetDataTable(table);
				this._list[table] = value;
			}
		}

		private DataTable GetTable(string tableName)
		{
			DataTable dataTable = null;
			DataSet dataSet = this._dataViewManager.DataSet;
			if (dataSet != null)
			{
				dataTable = dataSet.Tables[tableName];
			}
			return dataTable;
		}

		private DataTable GetTable(int index)
		{
			DataTable dataTable = null;
			DataSet dataSet = this._dataViewManager.DataSet;
			if (dataSet != null)
			{
				dataTable = dataSet.Tables[index];
			}
			return dataTable;
		}

		public virtual DataViewSetting this[string tableName]
		{
			get
			{
				DataTable table = this.GetTable(tableName);
				if (table != null)
				{
					return this[table];
				}
				return null;
			}
		}

		public virtual DataViewSetting this[int index]
		{
			get
			{
				DataTable table = this.GetTable(index);
				if (table != null)
				{
					return this[table];
				}
				return null;
			}
			set
			{
				DataTable table = this.GetTable(index);
				if (table != null)
				{
					this[table] = value;
				}
			}
		}

		public void CopyTo(Array ar, int index)
		{
			foreach (object obj in this)
			{
				ar.SetValue(obj, index++);
			}
		}

		public void CopyTo(DataViewSetting[] ar, int index)
		{
			foreach (object obj in this)
			{
				ar.SetValue(obj, index++);
			}
		}

		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				DataSet dataSet = this._dataViewManager.DataSet;
				if (dataSet != null)
				{
					return dataSet.Tables.Count;
				}
				return 0;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return new DataViewSettingCollection.DataViewSettingsEnumerator(this._dataViewManager);
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal void Remove(DataTable table)
		{
			this._list.Remove(table);
		}

		internal DataViewSettingCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly DataViewManager _dataViewManager;

		private readonly Hashtable _list;

		private sealed class DataViewSettingsEnumerator : IEnumerator
		{
			public DataViewSettingsEnumerator(DataViewManager dvm)
			{
				if (dvm.DataSet != null)
				{
					this._dataViewSettings = dvm.DataViewSettings;
					this._tableEnumerator = dvm.DataSet.Tables.GetEnumerator();
					return;
				}
				this._dataViewSettings = null;
				this._tableEnumerator = Array.Empty<DataTable>().GetEnumerator();
			}

			public bool MoveNext()
			{
				return this._tableEnumerator.MoveNext();
			}

			public void Reset()
			{
				this._tableEnumerator.Reset();
			}

			public object Current
			{
				get
				{
					return this._dataViewSettings[(DataTable)this._tableEnumerator.Current];
				}
			}

			private DataViewSettingCollection _dataViewSettings;

			private IEnumerator _tableEnumerator;
		}
	}
}
