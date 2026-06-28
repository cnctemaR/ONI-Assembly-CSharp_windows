using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public class DbEnumerator : IEnumerator
	{
		public DbEnumerator(IDataReader reader)
			: this(reader, false)
		{
		}

		public DbEnumerator(IDataReader reader, bool closeReader)
		{
			this.reader = reader;
			this.closeReader = closeReader;
			this.values = new object[reader.FieldCount];
			this.schema = DbEnumerator.LoadSchema(reader);
		}

		public object Current
		{
			get
			{
				this.reader.GetValues(this.values);
				return new DbDataRecordImpl(this.schema, this.values);
			}
		}

		private static SchemaInfo[] LoadSchema(IDataReader reader)
		{
			int fieldCount = reader.FieldCount;
			SchemaInfo[] array = new SchemaInfo[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				array[i] = new SchemaInfo
				{
					ColumnName = reader.GetName(i),
					ColumnOrdinal = i,
					DataTypeName = reader.GetDataTypeName(i),
					FieldType = reader.GetFieldType(i)
				};
			}
			return array;
		}

		public bool MoveNext()
		{
			if (this.reader.Read())
			{
				return true;
			}
			if (this.closeReader)
			{
				this.reader.Close();
			}
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		private readonly IDataReader reader;

		private readonly bool closeReader;

		private readonly SchemaInfo[] schema;

		private readonly object[] values;
	}
}
