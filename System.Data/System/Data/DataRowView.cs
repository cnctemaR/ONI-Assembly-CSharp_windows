using System;
using System.ComponentModel;

namespace System.Data
{
	public class DataRowView : ICustomTypeDescriptor, IDataErrorInfo, IEditableObject, INotifyPropertyChanged
	{
		internal DataRowView()
		{
		}

		public DataView DataView
		{
			get
			{
				throw null;
			}
		}

		public bool IsEdit
		{
			get
			{
				throw null;
			}
		}

		public bool IsNew
		{
			get
			{
				throw null;
			}
		}

		public object this[int ndx]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public object this[string property]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public DataRow Row
		{
			get
			{
				throw null;
			}
		}

		public DataRowVersion RowVersion
		{
			get
			{
				throw null;
			}
		}

		string IDataErrorInfo.Error
		{
			[MonoTODO("Not implemented, always returns String.Empty")]
			get
			{
				throw null;
			}
		}

		string IDataErrorInfo.this[string colName]
		{
			[MonoTODO("Not implemented, always returns String.Empty")]
			get
			{
				throw null;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public void BeginEdit()
		{
		}

		public void CancelEdit()
		{
		}

		public DataView CreateChildView(DataRelation relation)
		{
			throw null;
		}

		public DataView CreateChildView(string relationName)
		{
			throw null;
		}

		public void Delete()
		{
		}

		public void EndEdit()
		{
		}

		public override bool Equals(object other)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			throw null;
		}

		[MonoTODO("Not implemented.   Always returns String.Empty")]
		string ICustomTypeDescriptor.GetClassName()
		{
			throw null;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		string ICustomTypeDescriptor.GetComponentName()
		{
			throw null;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			throw null;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			throw null;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			throw null;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			throw null;
		}

		[MonoTODO("Not implemented.   Always returns an empty collection")]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			throw null;
		}

		[MonoTODO("Not implemented.   Always returns an empty collection")]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			throw null;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			throw null;
		}

		[MonoTODO("It currently reports more descriptors than necessary")]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			throw null;
		}

		[MonoTODO]
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			throw null;
		}
	}
}
