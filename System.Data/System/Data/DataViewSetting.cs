using System;
using System.ComponentModel;

namespace System.Data
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DataViewSetting
	{
		internal DataViewSetting(DataViewManager manager, DataTable table)
		{
			this.dataViewManager = manager;
			this.dataTable = table;
		}

		public bool ApplyDefaultSort
		{
			get
			{
				return this.applyDefaultSort;
			}
			set
			{
				this.applyDefaultSort = value;
			}
		}

		[Browsable(false)]
		public DataViewManager DataViewManager
		{
			get
			{
				return this.dataViewManager;
			}
		}

		public string RowFilter
		{
			get
			{
				return this.rowFilter;
			}
			set
			{
				this.rowFilter = value;
			}
		}

		public DataViewRowState RowStateFilter
		{
			get
			{
				return this.rowStateFilter;
			}
			set
			{
				this.rowStateFilter = value;
			}
		}

		public string Sort
		{
			get
			{
				return this.sort;
			}
			set
			{
				this.sort = value;
			}
		}

		[Browsable(false)]
		public DataTable Table
		{
			get
			{
				return this.dataTable;
			}
		}

		private bool applyDefaultSort;

		private DataViewManager dataViewManager;

		private string rowFilter = string.Empty;

		private DataViewRowState rowStateFilter = DataViewRowState.CurrentRows;

		private string sort = string.Empty;

		private DataTable dataTable;
	}
}
