using System;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbDataRecord : IDataRecord, ICustomTypeDescriptor
	{
		[MonoTODO]
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return new AttributeCollection(null);
		}

		[MonoTODO]
		string ICustomTypeDescriptor.GetClassName()
		{
			return string.Empty;
		}

		[MonoTODO]
		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		[MonoTODO]
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return null;
		}

		[MonoTODO]
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		[MonoTODO]
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		[MonoTODO]
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		[MonoTODO]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return new EventDescriptorCollection(null);
		}

		[MonoTODO]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return new EventDescriptorCollection(null);
		}

		[MonoTODO]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			DataColumnPropertyDescriptor[] array = new DataColumnPropertyDescriptor[this.FieldCount];
			for (int i = 0; i < this.FieldCount; i++)
			{
				DataColumnPropertyDescriptor dataColumnPropertyDescriptor = new DataColumnPropertyDescriptor(this.GetName(i), i, null);
				dataColumnPropertyDescriptor.SetComponentType(typeof(DbDataRecord));
				dataColumnPropertyDescriptor.SetPropertyType(this.GetFieldType(i));
				array[i] = dataColumnPropertyDescriptor;
			}
			return new PropertyDescriptorCollection(array);
		}

		[MonoTODO]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return ((ICustomTypeDescriptor)this).GetProperties();
		}

		[MonoTODO]
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public abstract int FieldCount { get; }

		public abstract object this[string name] { get; }

		public abstract object this[int i] { get; }

		public abstract bool GetBoolean(int i);

		public abstract byte GetByte(int i);

		public abstract long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length);

		public abstract char GetChar(int i);

		public abstract long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length);

		public abstract string GetDataTypeName(int i);

		protected abstract DbDataReader GetDbDataReader(int i);

		public abstract DateTime GetDateTime(int i);

		public abstract decimal GetDecimal(int i);

		public abstract double GetDouble(int i);

		public abstract Type GetFieldType(int i);

		public abstract float GetFloat(int i);

		public abstract Guid GetGuid(int i);

		public abstract short GetInt16(int i);

		public abstract int GetInt32(int i);

		public abstract long GetInt64(int i);

		public abstract string GetName(int i);

		public abstract int GetOrdinal(string name);

		public abstract string GetString(int i);

		public abstract object GetValue(int i);

		public abstract int GetValues(object[] values);

		public abstract bool IsDBNull(int i);

		public IDataReader GetData(int i)
		{
			return (IDataReader)this.GetValue(i);
		}
	}
}
