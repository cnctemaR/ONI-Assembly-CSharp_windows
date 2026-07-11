using System;
using System.ComponentModel;

namespace System.Data
{
	internal sealed class DataTablePropertyDescriptor : PropertyDescriptor
	{
		public DataTable Table { get; }

		internal DataTablePropertyDescriptor(DataTable dataTable)
			: base(dataTable.TableName, null)
		{
			this.Table = dataTable;
		}

		public override Type ComponentType
		{
			get
			{
				return typeof(DataRowView);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return typeof(IBindingList);
			}
		}

		public override bool Equals(object other)
		{
			return other is DataTablePropertyDescriptor && ((DataTablePropertyDescriptor)other).Table == this.Table;
		}

		public override int GetHashCode()
		{
			return this.Table.GetHashCode();
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return ((DataViewManagerListItemTypeDescriptor)component).GetDataView(this.Table);
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
