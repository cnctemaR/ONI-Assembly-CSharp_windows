using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data
{
	internal class DataColumnPropertyDescriptor : PropertyDescriptor
	{
		public DataColumnPropertyDescriptor(string name, int columnIndex, Attribute[] attrs)
			: base(name, attrs)
		{
			this.columnIndex = columnIndex;
		}

		public DataColumnPropertyDescriptor(DataColumn dc)
			: base(dc.ColumnName, null)
		{
			this.columnIndex = dc.Ordinal;
			this.componentType = typeof(DataRowView);
			this.propertyType = dc.DataType;
			this.readOnly = dc.ReadOnly;
		}

		public void SetReadOnly(bool value)
		{
			this.readOnly = value;
		}

		public void SetComponentType(Type type)
		{
			this.componentType = type;
		}

		public void SetPropertyType(Type type)
		{
			this.propertyType = type;
		}

		public void SetBrowsable(bool browsable)
		{
			this.browsable = browsable;
		}

		public override object GetValue(object component)
		{
			if (this.componentType == typeof(DataRowView) && component is DataRowView)
			{
				DataRowView dataRowView = (DataRowView)component;
				return dataRowView[base.Name];
			}
			if (this.componentType == typeof(DbDataRecord) && component is DbDataRecord)
			{
				DbDataRecord dbDataRecord = (DbDataRecord)component;
				return dbDataRecord[this.columnIndex];
			}
			throw new InvalidOperationException();
		}

		public override void SetValue(object component, object value)
		{
			DataRowView dataRowView = (DataRowView)component;
			dataRowView[base.Name] = value;
		}

		[MonoTODO]
		public override void ResetValue(object component)
		{
		}

		[MonoTODO]
		public override bool CanResetValue(object component)
		{
			return false;
		}

		[MonoTODO]
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get
			{
				return this.componentType;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public override bool IsBrowsable
		{
			get
			{
				return this.browsable && base.IsBrowsable;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.propertyType;
			}
		}

		private bool readOnly = true;

		private Type componentType;

		private Type propertyType;

		private bool browsable = true;

		private int columnIndex;
	}
}
