using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbDataReader : DbDataReader
	{
		internal OleDbDataReader()
		{
		}

		public override int Depth
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override int FieldCount
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override bool HasRows
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override bool IsClosed
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override int RecordsAffected
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override int VisibleFieldCount
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override void Close()
		{
			throw ADP.OleDb();
		}

		public override bool GetBoolean(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override byte GetByte(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			throw ADP.OleDb();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override char GetChar(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			throw ADP.OleDb();
		}

		public new OleDbDataReader GetData(int ordinal)
		{
			throw ADP.OleDb();
		}

		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override string GetDataTypeName(int index)
		{
			throw ADP.OleDb();
		}

		public override DateTime GetDateTime(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override decimal GetDecimal(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override double GetDouble(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override IEnumerator GetEnumerator()
		{
			throw ADP.OleDb();
		}

		public override Type GetFieldType(int index)
		{
			throw ADP.OleDb();
		}

		public override float GetFloat(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override Guid GetGuid(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override short GetInt16(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override int GetInt32(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override long GetInt64(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override string GetName(int index)
		{
			throw ADP.OleDb();
		}

		public override int GetOrdinal(string name)
		{
			throw ADP.OleDb();
		}

		public override DataTable GetSchemaTable()
		{
			throw ADP.OleDb();
		}

		public override string GetString(int ordinal)
		{
			throw ADP.OleDb();
		}

		public TimeSpan GetTimeSpan(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override object GetValue(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override int GetValues(object[] values)
		{
			throw ADP.OleDb();
		}

		public override bool IsDBNull(int ordinal)
		{
			throw ADP.OleDb();
		}

		public override bool NextResult()
		{
			throw ADP.OleDb();
		}

		public override bool Read()
		{
			throw ADP.OleDb();
		}

		public override object this[int index]
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override object this[string name]
		{
			get
			{
				throw ADP.OleDb();
			}
		}
	}
}
