using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data
{
	internal sealed class DataColumnPropertyDescriptor : PropertyDescriptor
	{
		internal DataColumnPropertyDescriptor(DataColumn dataColumn)
			: base(dataColumn.ColumnName, null)
		{
			this.Column = dataColumn;
		}

		public override AttributeCollection Attributes
		{
			get
			{
				if (typeof(IList).IsAssignableFrom(this.PropertyType))
				{
					Attribute[] array = new Attribute[base.Attributes.Count + 1];
					base.Attributes.CopyTo(array, 0);
					array[array.Length - 1] = new ListBindableAttribute(false);
					return new AttributeCollection(array);
				}
				return base.Attributes;
			}
		}

		internal DataColumn Column { get; }

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
				return this.Column.ReadOnly;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.Column.DataType;
			}
		}

		public override bool Equals(object other)
		{
			return other is DataColumnPropertyDescriptor && ((DataColumnPropertyDescriptor)other).Column == this.Column;
		}

		public override int GetHashCode()
		{
			return this.Column.GetHashCode();
		}

		public override bool CanResetValue(object component)
		{
			DataRowView dataRowView = (DataRowView)component;
			if (!this.Column.IsSqlType)
			{
				return dataRowView.GetColumnValue(this.Column) != DBNull.Value;
			}
			return !DataStorage.IsObjectNull(dataRowView.GetColumnValue(this.Column));
		}

		public override object GetValue(object component)
		{
			return ((DataRowView)component).GetColumnValue(this.Column);
		}

		public override void ResetValue(object component)
		{
			((DataRowView)component).SetColumnValue(this.Column, DBNull.Value);
		}

		public override void SetValue(object component, object value)
		{
			((DataRowView)component).SetColumnValue(this.Column, value);
			this.OnValueChanged(component, EventArgs.Empty);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override bool IsBrowsable
		{
			get
			{
				return this.Column.ColumnMapping != MappingType.Hidden && base.IsBrowsable;
			}
		}
	}
}
