using System;
using System.ComponentModel;

namespace System.Data
{
	internal class DataViewManagerListItemTypeDescriptor : ICustomTypeDescriptor
	{
		internal DataViewManagerListItemTypeDescriptor(DataViewManager dvm)
		{
			this.dvm = dvm;
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return new AttributeCollection(null);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return null;
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return new EventDescriptorCollection(null);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return new EventDescriptorCollection(null);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return this.GetProperties();
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		internal DataViewManager DataViewManager
		{
			get
			{
				return this.dvm;
			}
		}

		public PropertyDescriptorCollection GetProperties()
		{
			DataSet dataSet = this.dvm.DataSet;
			if (dataSet == null)
			{
				return null;
			}
			DataTableCollection tables = dataSet.Tables;
			int num = 0;
			PropertyDescriptor[] array = new PropertyDescriptor[tables.Count];
			foreach (object obj in tables)
			{
				DataTable dataTable = (DataTable)obj;
				array[num++] = new DataTablePropertyDescriptor(dataTable);
			}
			return new PropertyDescriptorCollection(array);
		}

		private DataViewManager dvm;
	}
}
