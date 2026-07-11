using System;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbDataRecord : ICustomTypeDescriptor, IDataRecord
	{
		public abstract int FieldCount { get; }

		public abstract object this[int i] { get; }

		public abstract object this[string name] { get; }

		public abstract bool GetBoolean(int i);

		public abstract byte GetByte(int i);

		public abstract long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length);

		public abstract char GetChar(int i);

		public abstract long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length);

		public IDataReader GetData(int i)
		{
			throw null;
		}

		public abstract string GetDataTypeName(int i);

		public abstract DateTime GetDateTime(int i);

		protected virtual DbDataReader GetDbDataReader(int i)
		{
			throw null;
		}

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

		[MonoTODO]
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			throw null;
		}

		[MonoTODO]
		string ICustomTypeDescriptor.GetClassName()
		{
			throw null;
		}

		[MonoTODO]
		string ICustomTypeDescriptor.GetComponentName()
		{
			throw null;
		}

		[MonoTODO]
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			throw null;
		}

		[MonoTODO]
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			throw null;
		}

		[MonoTODO]
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			throw null;
		}

		[MonoTODO]
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			throw null;
		}

		[MonoTODO]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			throw null;
		}

		[MonoTODO]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			throw null;
		}

		[MonoTODO]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			throw null;
		}

		[MonoTODO]
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
