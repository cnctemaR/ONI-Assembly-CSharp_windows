using System;
using System.Data.Common;

namespace System.Data.ProviderBase
{
	internal abstract class DataReaderContainer
	{
		internal static DataReaderContainer Create(IDataReader dataReader, bool returnProviderSpecificTypes)
		{
			if (returnProviderSpecificTypes)
			{
				DbDataReader dbDataReader = dataReader as DbDataReader;
				if (dbDataReader != null)
				{
					return new DataReaderContainer.ProviderSpecificDataReader(dataReader, dbDataReader);
				}
			}
			return new DataReaderContainer.CommonLanguageSubsetDataReader(dataReader);
		}

		protected DataReaderContainer(IDataReader dataReader)
		{
			this._dataReader = dataReader;
		}

		internal int FieldCount
		{
			get
			{
				return this._fieldCount;
			}
		}

		internal abstract bool ReturnProviderSpecificTypes { get; }

		protected abstract int VisibleFieldCount { get; }

		internal abstract Type GetFieldType(int ordinal);

		internal abstract object GetValue(int ordinal);

		internal abstract int GetValues(object[] values);

		internal string GetName(int ordinal)
		{
			string name = this._dataReader.GetName(ordinal);
			if (name == null)
			{
				return "";
			}
			return name;
		}

		internal DataTable GetSchemaTable()
		{
			return this._dataReader.GetSchemaTable();
		}

		internal bool NextResult()
		{
			this._fieldCount = 0;
			if (this._dataReader.NextResult())
			{
				this._fieldCount = this.VisibleFieldCount;
				return true;
			}
			return false;
		}

		internal bool Read()
		{
			return this._dataReader.Read();
		}

		protected readonly IDataReader _dataReader;

		protected int _fieldCount;

		private sealed class ProviderSpecificDataReader : DataReaderContainer
		{
			internal ProviderSpecificDataReader(IDataReader dataReader, DbDataReader dbDataReader)
				: base(dataReader)
			{
				this._providerSpecificDataReader = dbDataReader;
				this._fieldCount = this.VisibleFieldCount;
			}

			internal override bool ReturnProviderSpecificTypes
			{
				get
				{
					return true;
				}
			}

			protected override int VisibleFieldCount
			{
				get
				{
					int visibleFieldCount = this._providerSpecificDataReader.VisibleFieldCount;
					if (0 > visibleFieldCount)
					{
						return 0;
					}
					return visibleFieldCount;
				}
			}

			internal override Type GetFieldType(int ordinal)
			{
				return this._providerSpecificDataReader.GetProviderSpecificFieldType(ordinal);
			}

			internal override object GetValue(int ordinal)
			{
				return this._providerSpecificDataReader.GetProviderSpecificValue(ordinal);
			}

			internal override int GetValues(object[] values)
			{
				return this._providerSpecificDataReader.GetProviderSpecificValues(values);
			}

			private DbDataReader _providerSpecificDataReader;
		}

		private sealed class CommonLanguageSubsetDataReader : DataReaderContainer
		{
			internal CommonLanguageSubsetDataReader(IDataReader dataReader)
				: base(dataReader)
			{
				this._fieldCount = this.VisibleFieldCount;
			}

			internal override bool ReturnProviderSpecificTypes
			{
				get
				{
					return false;
				}
			}

			protected override int VisibleFieldCount
			{
				get
				{
					int fieldCount = this._dataReader.FieldCount;
					if (0 > fieldCount)
					{
						return 0;
					}
					return fieldCount;
				}
			}

			internal override Type GetFieldType(int ordinal)
			{
				return this._dataReader.GetFieldType(ordinal);
			}

			internal override object GetValue(int ordinal)
			{
				return this._dataReader.GetValue(ordinal);
			}

			internal override int GetValues(object[] values)
			{
				return this._dataReader.GetValues(values);
			}
		}
	}
}
