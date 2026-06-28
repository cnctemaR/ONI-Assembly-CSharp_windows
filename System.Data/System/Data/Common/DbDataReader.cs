using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbDataReader : MarshalByRefObject, IDisposable, IDataReader, IDataRecord, IEnumerable
	{
		IDataReader IDataRecord.GetData(int i)
		{
			return ((IDataRecord)this).GetData(i);
		}

		public abstract int Depth { get; }

		public abstract int FieldCount { get; }

		public abstract bool HasRows { get; }

		public abstract bool IsClosed { get; }

		public abstract object this[int index] { get; }

		public abstract object this[string name] { get; }

		public abstract int RecordsAffected { get; }

		public virtual int VisibleFieldCount
		{
			get
			{
				return this.FieldCount;
			}
		}

		public abstract void Close();

		public abstract bool GetBoolean(int i);

		public abstract byte GetByte(int i);

		public abstract long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length);

		public abstract char GetChar(int i);

		public abstract long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DbDataReader GetData(int i)
		{
			return (DbDataReader)this[i];
		}

		public abstract string GetDataTypeName(int i);

		public abstract DateTime GetDateTime(int i);

		public abstract decimal GetDecimal(int i);

		public abstract double GetDouble(int i);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract IEnumerator GetEnumerator();

		public abstract Type GetFieldType(int i);

		public abstract float GetFloat(int i);

		public abstract Guid GetGuid(int i);

		public abstract short GetInt16(int i);

		public abstract int GetInt32(int i);

		public abstract long GetInt64(int i);

		public abstract string GetName(int i);

		public abstract int GetOrdinal(string name);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Type GetProviderSpecificFieldType(int i)
		{
			return this.GetFieldType(i);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual object GetProviderSpecificValue(int i)
		{
			return this.GetValue(i);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int GetProviderSpecificValues(object[] values)
		{
			return this.GetValues(values);
		}

		protected virtual DbDataReader GetDbDataReader(int ordinal)
		{
			return (DbDataReader)this[ordinal];
		}

		public abstract DataTable GetSchemaTable();

		public abstract string GetString(int i);

		public abstract object GetValue(int i);

		public abstract int GetValues(object[] values);

		public abstract bool IsDBNull(int i);

		public abstract bool NextResult();

		public abstract bool Read();

		internal static DataTable GetSchemaTableTemplate()
		{
			Type typeFromHandle = typeof(bool);
			Type typeFromHandle2 = typeof(string);
			Type typeFromHandle3 = typeof(int);
			Type typeFromHandle4 = typeof(Type);
			Type typeFromHandle5 = typeof(short);
			return new DataTable("SchemaTable")
			{
				Columns = 
				{
					{ "ColumnName", typeFromHandle2 },
					{ "ColumnOrdinal", typeFromHandle3 },
					{ "ColumnSize", typeFromHandle3 },
					{ "NumericPrecision", typeFromHandle5 },
					{ "NumericScale", typeFromHandle5 },
					{ "IsUnique", typeFromHandle },
					{ "IsKey", typeFromHandle },
					{ "BaseServerName", typeFromHandle2 },
					{ "BaseCatalogName", typeFromHandle2 },
					{ "BaseColumnName", typeFromHandle2 },
					{ "BaseSchemaName", typeFromHandle2 },
					{ "BaseTableName", typeFromHandle2 },
					{ "DataType", typeFromHandle4 },
					{ "AllowDBNull", typeFromHandle },
					{ "ProviderType", typeFromHandle3 },
					{ "IsAliased", typeFromHandle },
					{ "IsExpression", typeFromHandle },
					{ "IsIdentity", typeFromHandle },
					{ "IsAutoIncrement", typeFromHandle },
					{ "IsRowVersion", typeFromHandle },
					{ "IsHidden", typeFromHandle },
					{ "IsLong", typeFromHandle },
					{ "IsReadOnly", typeFromHandle }
				}
			};
		}
	}
}
