using System;
using System.Collections;
using System.ComponentModel;
using System.Data.ProviderBase;

namespace System.Data.Common
{
	public class DbEnumerator : IEnumerator
	{
		public DbEnumerator(IDataReader reader)
		{
			if (reader == null)
			{
				throw ADP.ArgumentNull("reader");
			}
			this._reader = reader;
		}

		public DbEnumerator(IDataReader reader, bool closeReader)
		{
			if (reader == null)
			{
				throw ADP.ArgumentNull("reader");
			}
			this._reader = reader;
			this._closeReader = closeReader;
		}

		public DbEnumerator(DbDataReader reader)
			: this(reader)
		{
		}

		public DbEnumerator(DbDataReader reader, bool closeReader)
			: this(reader, closeReader)
		{
		}

		public object Current
		{
			get
			{
				return this._current;
			}
		}

		public bool MoveNext()
		{
			if (this._schemaInfo == null)
			{
				this.BuildSchemaInfo();
			}
			this._current = null;
			if (this._reader.Read())
			{
				object[] array = new object[this._schemaInfo.Length];
				this._reader.GetValues(array);
				this._current = new DataRecordInternal(this._schemaInfo, array, this._descriptors, this._fieldNameLookup);
				return true;
			}
			if (this._closeReader)
			{
				this._reader.Close();
			}
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset()
		{
			throw ADP.NotSupported();
		}

		private void BuildSchemaInfo()
		{
			int fieldCount = this._reader.FieldCount;
			string[] array = new string[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				array[i] = this._reader.GetName(i);
			}
			ADP.BuildSchemaTableInfoTableNames(array);
			SchemaInfo[] array2 = new SchemaInfo[fieldCount];
			PropertyDescriptor[] array3 = new PropertyDescriptor[this._reader.FieldCount];
			for (int j = 0; j < array2.Length; j++)
			{
				SchemaInfo schemaInfo = default(SchemaInfo);
				schemaInfo.name = this._reader.GetName(j);
				schemaInfo.type = this._reader.GetFieldType(j);
				schemaInfo.typeName = this._reader.GetDataTypeName(j);
				array3[j] = new DbEnumerator.DbColumnDescriptor(j, array[j], schemaInfo.type);
				array2[j] = schemaInfo;
			}
			this._schemaInfo = array2;
			this._fieldNameLookup = new FieldNameLookup(this._reader, -1);
			this._descriptors = new PropertyDescriptorCollection(array3);
		}

		internal IDataReader _reader;

		internal DbDataRecord _current;

		internal SchemaInfo[] _schemaInfo;

		internal PropertyDescriptorCollection _descriptors;

		private FieldNameLookup _fieldNameLookup;

		private bool _closeReader;

		private sealed class DbColumnDescriptor : PropertyDescriptor
		{
			internal DbColumnDescriptor(int ordinal, string name, Type type)
				: base(name, null)
			{
				this._ordinal = ordinal;
				this._type = type;
			}

			public override Type ComponentType
			{
				get
				{
					return typeof(IDataRecord);
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return this._type;
				}
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override object GetValue(object component)
			{
				return ((IDataRecord)component)[this._ordinal];
			}

			public override void ResetValue(object component)
			{
				throw ADP.NotSupported();
			}

			public override void SetValue(object component, object value)
			{
				throw ADP.NotSupported();
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}

			private int _ordinal;

			private Type _type;
		}
	}
}
