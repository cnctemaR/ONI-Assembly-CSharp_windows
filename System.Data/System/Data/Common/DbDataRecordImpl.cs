using System;

namespace System.Data.Common
{
	internal class DbDataRecordImpl : DbDataRecord
	{
		internal DbDataRecordImpl(SchemaInfo[] schema, object[] values)
		{
			this.schema = schema;
			this.values = values;
			this.fieldCount = values.Length;
		}

		public override int FieldCount
		{
			get
			{
				return this.fieldCount;
			}
		}

		public override object this[string name]
		{
			get
			{
				return this[this.GetOrdinal(name)];
			}
		}

		public override object this[int i]
		{
			get
			{
				return this.GetValue(i);
			}
		}

		public override bool GetBoolean(int i)
		{
			return (bool)this.GetValue(i);
		}

		public override byte GetByte(int i)
		{
			return (byte)this.GetValue(i);
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			object value = this.GetValue(i);
			if (!(value is byte[]))
			{
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			if (buffer == null)
			{
				return (long)((byte[])value).Length;
			}
			Array.Copy((byte[])value, (int)dataIndex, buffer, bufferIndex, length);
			return (long)((byte[])value).Length - dataIndex;
		}

		public override char GetChar(int i)
		{
			return (char)this.GetValue(i);
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			object value = this.GetValue(i);
			char[] array;
			if (value is char[])
			{
				array = (char[])value;
			}
			else
			{
				if (!(value is string))
				{
					throw new InvalidCastException("Type is " + value.GetType().ToString());
				}
				array = ((string)value).ToCharArray();
			}
			if (buffer == null)
			{
				return (long)array.Length;
			}
			Array.Copy(array, (int)dataIndex, buffer, bufferIndex, length);
			return (long)array.Length - dataIndex;
		}

		public override string GetDataTypeName(int i)
		{
			return this.schema[i].DataTypeName;
		}

		public override DateTime GetDateTime(int i)
		{
			return (DateTime)this.GetValue(i);
		}

		[MonoTODO]
		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override decimal GetDecimal(int i)
		{
			return (decimal)this.GetValue(i);
		}

		public override double GetDouble(int i)
		{
			return (double)this.GetValue(i);
		}

		public override Type GetFieldType(int i)
		{
			return this.schema[i].FieldType;
		}

		public override float GetFloat(int i)
		{
			return (float)this.GetValue(i);
		}

		public override Guid GetGuid(int i)
		{
			return (Guid)this.GetValue(i);
		}

		public override short GetInt16(int i)
		{
			return (short)this.GetValue(i);
		}

		public override int GetInt32(int i)
		{
			return (int)this.GetValue(i);
		}

		public override long GetInt64(int i)
		{
			return (long)this.GetValue(i);
		}

		public override string GetName(int i)
		{
			return this.schema[i].ColumnName;
		}

		public override int GetOrdinal(string name)
		{
			for (int i = 0; i < this.FieldCount; i++)
			{
				if (this.schema[i].ColumnName == name)
				{
					return i;
				}
			}
			return -1;
		}

		public override string GetString(int i)
		{
			return (string)this.GetValue(i);
		}

		public override object GetValue(int i)
		{
			if (i < 0 || i > this.fieldCount)
			{
				throw new IndexOutOfRangeException();
			}
			return this.values[i];
		}

		public override int GetValues(object[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = ((values.Length <= this.values.Length) ? values.Length : this.values.Length);
			for (int i = 0; i < num; i++)
			{
				values[i] = this.values[i];
			}
			return num;
		}

		public override bool IsDBNull(int i)
		{
			return this.GetValue(i) == DBNull.Value;
		}

		private readonly SchemaInfo[] schema;

		private readonly object[] values;

		private readonly int fieldCount;
	}
}
