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
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public DataViewManager DataViewManager
		{
			get
			{
				throw null;
			}
		}

		public string RowFilter
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public DataViewRowState RowStateFilter
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string Sort
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public DataTable Table
		{
			get
			{
				throw null;
			}
		}
	}
}
