using System;
using System.ComponentModel;
using System.Data.ProviderBase;

namespace System.Data.Common
{
	internal sealed class DataRecordInternal : DbDataRecord, ICustomTypeDescriptor
	{
		internal DataRecordInternal(SchemaInfo[] schemaInfo, object[] values, PropertyDescriptorCollection descriptors, FieldNameLookup fieldNameLookup)
		{
			this._schemaInfo = schemaInfo;
			this._values = values;
			this._propertyDescriptors = descriptors;
			this._fieldNameLookup = fieldNameLookup;
		}

		public override int FieldCount
		{
			get
			{
				return this._schemaInfo.Length;
			}
		}

		public override int GetValues(object[] values)
		{
			if (values == null)
			{
				throw ADP.ArgumentNull("values");
			}
			int num = ((values.Length < this._schemaInfo.Length) ? values.Length : this._schemaInfo.Length);
			for (int i = 0; i < num; i++)
			{
				values[i] = this._values[i];
			}
			return num;
		}

		public override string GetName(int i)
		{
			return this._schemaInfo[i].name;
		}

		public override object GetValue(int i)
		{
			return this._values[i];
		}

		public override string GetDataTypeName(int i)
		{
			return this._schemaInfo[i].typeName;
		}

		public override Type GetFieldType(int i)
		{
			return this._schemaInfo[i].type;
		}

		public override int GetOrdinal(string name)
		{
			return this._fieldNameLookup.GetOrdinal(name);
		}

		public override object this[int i]
		{
			get
			{
				return this.GetValue(i);
			}
		}

		public override object this[string name]
		{
			get
			{
				return this.GetValue(this.GetOrdinal(name));
			}
		}

		public override bool GetBoolean(int i)
		{
			return (bool)this._values[i];
		}

		public override byte GetByte(int i)
		{
			return (byte)this._values[i];
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			int num = 0;
			byte[] array = (byte[])this._values[i];
			num = array.Length;
			if (dataIndex > 2147483647L)
			{
				throw ADP.InvalidSourceBufferIndex(num, dataIndex, "dataIndex");
			}
			int num2 = (int)dataIndex;
			if (buffer == null)
			{
				return (long)num;
			}
			try
			{
				if (num2 < num)
				{
					if (num2 + length > num)
					{
						num -= num2;
					}
					else
					{
						num = length;
					}
				}
				Array.Copy(array, num2, buffer, bufferIndex, num);
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				num = array.Length;
				if (length < 0)
				{
					throw ADP.InvalidDataLength((long)length);
				}
				if (bufferIndex < 0 || bufferIndex >= buffer.Length)
				{
					throw ADP.InvalidDestinationBufferIndex(length, bufferIndex, "bufferIndex");
				}
				if (dataIndex < 0L || dataIndex >= (long)num)
				{
					throw ADP.InvalidSourceBufferIndex(length, dataIndex, "dataIndex");
				}
				if (num + bufferIndex > buffer.Length)
				{
					throw ADP.InvalidBufferSizeOrIndex(num, bufferIndex);
				}
			}
			return (long)num;
		}

		public override char GetChar(int i)
		{
			return ((string)this._values[i])[0];
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			char[] array = ((string)this._values[i]).ToCharArray();
			int num = array.Length;
			if (dataIndex > 2147483647L)
			{
				throw ADP.InvalidSourceBufferIndex(num, dataIndex, "dataIndex");
			}
			int num2 = (int)dataIndex;
			if (buffer == null)
			{
				return (long)num;
			}
			try
			{
				if (num2 < num)
				{
					if (num2 + length > num)
					{
						num -= num2;
					}
					else
					{
						num = length;
					}
				}
				Array.Copy(array, num2, buffer, bufferIndex, num);
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				num = array.Length;
				if (length < 0)
				{
					throw ADP.InvalidDataLength((long)length);
				}
				if (bufferIndex < 0 || bufferIndex >= buffer.Length)
				{
					throw ADP.InvalidDestinationBufferIndex(buffer.Length, bufferIndex, "bufferIndex");
				}
				if (num2 < 0 || num2 >= num)
				{
					throw ADP.InvalidSourceBufferIndex(num, dataIndex, "dataIndex");
				}
				if (num + bufferIndex > buffer.Length)
				{
					throw ADP.InvalidBufferSizeOrIndex(num, bufferIndex);
				}
			}
			return (long)num;
		}

		public override Guid GetGuid(int i)
		{
			return (Guid)this._values[i];
		}

		public override short GetInt16(int i)
		{
			return (short)this._values[i];
		}

		public override int GetInt32(int i)
		{
			return (int)this._values[i];
		}

		public override long GetInt64(int i)
		{
			return (long)this._values[i];
		}

		public override float GetFloat(int i)
		{
			return (float)this._values[i];
		}

		public override double GetDouble(int i)
		{
			return (double)this._values[i];
		}

		public override string GetString(int i)
		{
			return (string)this._values[i];
		}

		public override decimal GetDecimal(int i)
		{
			return (decimal)this._values[i];
		}

		public override DateTime GetDateTime(int i)
		{
			return (DateTime)this._values[i];
		}

		public override bool IsDBNull(int i)
		{
			object obj = this._values[i];
			return obj == null || Convert.IsDBNull(obj);
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
			if (this._propertyDescriptors == null)
			{
				this._propertyDescriptors = new PropertyDescriptorCollection(null);
			}
			return this._propertyDescriptors;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		private SchemaInfo[] _schemaInfo;

		private object[] _values;

		private PropertyDescriptorCollection _propertyDescriptors;

		private FieldNameLookup _fieldNameLookup;
	}
}
