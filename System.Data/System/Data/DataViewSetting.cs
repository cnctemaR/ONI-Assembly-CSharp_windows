using System;
using System.ComponentModel;

namespace System.Data
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DataViewSetting
	{
		internal DataViewSetting()
		{
		}

		public bool ApplyDefaultSort
		{
			get
			{
				return this._applyDefaultSort;
			}
			set
			{
				if (this._applyDefaultSort != value)
				{
					this._applyDefaultSort = value;
				}
			}
		}

		[Browsable(false)]
		public DataViewManager DataViewManager
		{
			get
			{
				return this._dataViewManager;
			}
		}

		internal void SetDataViewManager(DataViewManager dataViewManager)
		{
			if (this._dataViewManager != dataViewManager)
			{
				this._dataViewManager = dataViewManager;
			}
		}

		[Browsable(false)]
		public DataTable Table
		{
			get
			{
				return this._table;
			}
		}

		internal void SetDataTable(DataTable table)
		{
			if (this._table != table)
			{
				this._table = table;
			}
		}

		public string RowFilter
		{
			get
			{
				return this._rowFilter;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this._rowFilter != value)
				{
					this._rowFilter = value;
				}
			}
		}

		public DataViewRowState RowStateFilter
		{
			get
			{
				return this._rowStateFilter;
			}
			set
			{
				if (this._rowStateFilter != value)
				{
					this._rowStateFilter = value;
				}
			}
		}

		public string Sort
		{
			get
			{
				return this._sort;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this._sort != value)
				{
					this._sort = value;
				}
			}
		}

		private DataViewManager _dataViewManager;

		private DataTable _table;

		private string _sort = string.Empty;

		private string _rowFilter = string.Empty;

		private DataViewRowState _rowStateFilter = DataViewRowState.CurrentRows;

		private bool _applyDefaultSort;
	}
}
