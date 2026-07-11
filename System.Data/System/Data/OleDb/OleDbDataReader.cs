using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb
{
	public sealed class OleDbDataReader : DbDataReader, IDisposable
	{
		internal OleDbDataReader()
		{
		}

		public override int Depth
		{
			get
			{
				throw null;
			}
		}

		public override int FieldCount
		{
			get
			{
				throw null;
			}
		}

		[MonoTODO]
		public override bool HasRows
		{
			get
			{
				throw null;
			}
		}

		public override bool IsClosed
		{
			get
			{
				throw null;
			}
		}

		public override object this[int index]
		{
			get
			{
				throw null;
			}
		}

		public override object this[string name]
		{
			get
			{
				throw null;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				throw null;
			}
		}

		[MonoTODO]
		public override int VisibleFieldCount
		{
			get
			{
				throw null;
			}
		}

		public override void Close()
		{
		}

		public override bool GetBoolean(int ordinal)
		{
			throw null;
		}

		public override byte GetByte(int ordinal)
		{
			throw null;
		}

		[MonoTODO]
		public override long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override char GetChar(int ordinal)
		{
			throw null;
		}

		[MonoTODO]
		public override long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[MonoTODO]
		public new OleDbDataReader GetData(int ordinal)
		{
			throw null;
		}

		public override string GetDataTypeName(int index)
		{
			throw null;
		}

		public override DateTime GetDateTime(int ordinal)
		{
			throw null;
		}

		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			throw null;
		}

		[MonoTODO]
		public override decimal GetDecimal(int ordinal)
		{
			throw null;
		}

		public override double GetDouble(int ordinal)
		{
			throw null;
		}

		public override IEnumerator GetEnumerator()
		{
			throw null;
		}

		public override Type GetFieldType(int index)
		{
			throw null;
		}

		public override float GetFloat(int ordinal)
		{
			throw null;
		}

		[MonoTODO]
		public override Guid GetGuid(int ordinal)
		{
			throw null;
		}

		public override short GetInt16(int ordinal)
		{
			throw null;
		}

		public override int GetInt32(int ordinal)
		{
			throw null;
		}

		public override long GetInt64(int ordinal)
		{
			throw null;
		}

		public override string GetName(int index)
		{
			throw null;
		}

		public override int GetOrdinal(string name)
		{
			throw null;
		}

		public override DataTable GetSchemaTable()
		{
			throw null;
		}

		public override string GetString(int ordinal)
		{
			throw null;
		}

		[MonoTODO]
		public TimeSpan GetTimeSpan(int ordinal)
		{
			throw null;
		}

		public override object GetValue(int ordinal)
		{
			throw null;
		}

		[MonoTODO]
		public override int GetValues(object[] values)
		{
			throw null;
		}

		public override bool IsDBNull(int ordinal)
		{
			throw null;
		}

		public override bool NextResult()
		{
			throw null;
		}

		public override bool Read()
		{
			throw null;
		}

		void IDisposable.Dispose()
		{
		}
	}
}
