using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbDataReader : MarshalByRefObject, IEnumerable, IDataReader, IDataRecord, IDisposable
	{
		public abstract int Depth { get; }

		public abstract int FieldCount { get; }

		public abstract bool HasRows { get; }

		public abstract bool IsClosed { get; }

		public abstract object this[int ordinal] { get; }

		public abstract object this[string name] { get; }

		public abstract int RecordsAffected { get; }

		public virtual int VisibleFieldCount
		{
			get
			{
				throw null;
			}
		}

		public abstract void Close();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Dispose()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public abstract bool GetBoolean(int ordinal);

		public abstract byte GetByte(int ordinal);

		public abstract long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length);

		public abstract char GetChar(int ordinal);

		public abstract long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DbDataReader GetData(int ordinal)
		{
			throw null;
		}

		public abstract string GetDataTypeName(int ordinal);

		public abstract DateTime GetDateTime(int ordinal);

		protected virtual DbDataReader GetDbDataReader(int ordinal)
		{
			throw null;
		}

		public abstract decimal GetDecimal(int ordinal);

		public abstract double GetDouble(int ordinal);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract IEnumerator GetEnumerator();

		public abstract Type GetFieldType(int ordinal);

		public abstract float GetFloat(int ordinal);

		public abstract Guid GetGuid(int ordinal);

		public abstract short GetInt16(int ordinal);

		public abstract int GetInt32(int ordinal);

		public abstract long GetInt64(int ordinal);

		public abstract string GetName(int ordinal);

		public abstract int GetOrdinal(string name);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Type GetProviderSpecificFieldType(int ordinal)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual object GetProviderSpecificValue(int ordinal)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int GetProviderSpecificValues(object[] values)
		{
			throw null;
		}

		public abstract DataTable GetSchemaTable();

		public abstract string GetString(int ordinal);

		public abstract object GetValue(int ordinal);

		public abstract int GetValues(object[] values);

		public abstract bool IsDBNull(int ordinal);

		public abstract bool NextResult();

		public abstract bool Read();

		IDataReader IDataRecord.GetData(int ordinal)
		{
			throw null;
		}
	}
}
