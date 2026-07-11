using System;
using System.ComponentModel;

namespace System.Data
{
	internal sealed class DataViewManagerListItemTypeDescriptor : ICustomTypeDescriptor
	{
		internal DataViewManagerListItemTypeDescriptor(DataViewManager dataViewManager)
		{
			this._dataViewManager = dataViewManager;
		}

		internal void Reset()
		{
			this._propsCollection = null;
		}

		internal DataView GetDataView(DataTable table)
		{
			DataView dataView = new DataView(table);
			dataView.SetDataViewManager(this._dataViewManager);
			return dataView;
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

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			if (this._propsCollection == null)
			{
				PropertyDescriptor[] array = null;
				DataSet dataSet = this._dataViewManager.DataSet;
				if (dataSet != null)
				{
					int count = dataSet.Tables.Count;
					array = new PropertyDescriptor[count];
					for (int i = 0; i < count; i++)
					{
						array[i] = new DataTablePropertyDescriptor(dataSet.Tables[i]);
					}
				}
				this._propsCollection = new PropertyDescriptorCollection(array);
			}
			return this._propsCollection;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		private DataViewManager _dataViewManager;

		private PropertyDescriptorCollection _propsCollection;
	}
}
