using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;

namespace System.Data.SqlClient
{
	public class SqlDataReader : DbDataReader, IDataReader, IDataRecord, IDisposable
	{
		internal SqlDataReader()
		{
		}

		protected SqlConnection Connection
		{
			get
			{
				throw null;
			}
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

		public override object this[int i]
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

		public override bool GetBoolean(int i)
		{
			throw null;
		}

		public override byte GetByte(int i)
		{
			throw null;
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override char GetChar(int i)
		{
			throw null;
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			throw null;
		}

		public override string GetDataTypeName(int i)
		{
			throw null;
		}

		public override DateTime GetDateTime(int i)
		{
			throw null;
		}

		[MonoTODO]
		public virtual DateTimeOffset GetDateTimeOffset(int i)
		{
			throw null;
		}

		public override decimal GetDecimal(int i)
		{
			throw null;
		}

		public override double GetDouble(int i)
		{
			throw null;
		}

		public override IEnumerator GetEnumerator()
		{
			throw null;
		}

		public override Type GetFieldType(int i)
		{
			throw null;
		}

		public override float GetFloat(int i)
		{
			throw null;
		}

		public override Guid GetGuid(int i)
		{
			throw null;
		}

		public override short GetInt16(int i)
		{
			throw null;
		}

		public override int GetInt32(int i)
		{
			throw null;
		}

		public override long GetInt64(int i)
		{
			throw null;
		}

		public override string GetName(int i)
		{
			throw null;
		}

		public override int GetOrdinal(string name)
		{
			throw null;
		}

		public override Type GetProviderSpecificFieldType(int i)
		{
			throw null;
		}

		public override object GetProviderSpecificValue(int i)
		{
			throw null;
		}

		public override int GetProviderSpecificValues(object[] values)
		{
			throw null;
		}

		public override DataTable GetSchemaTable()
		{
			throw null;
		}

		public virtual SqlBinary GetSqlBinary(int i)
		{
			throw null;
		}

		public virtual SqlBoolean GetSqlBoolean(int i)
		{
			throw null;
		}

		public virtual SqlByte GetSqlByte(int i)
		{
			throw null;
		}

		public virtual SqlBytes GetSqlBytes(int i)
		{
			throw null;
		}

		[MonoTODO]
		public virtual SqlChars GetSqlChars(int i)
		{
			throw null;
		}

		public virtual SqlDateTime GetSqlDateTime(int i)
		{
			throw null;
		}

		public virtual SqlDecimal GetSqlDecimal(int i)
		{
			throw null;
		}

		public virtual SqlDouble GetSqlDouble(int i)
		{
			throw null;
		}

		public virtual SqlGuid GetSqlGuid(int i)
		{
			throw null;
		}

		public virtual SqlInt16 GetSqlInt16(int i)
		{
			throw null;
		}

		public virtual SqlInt32 GetSqlInt32(int i)
		{
			throw null;
		}

		public virtual SqlInt64 GetSqlInt64(int i)
		{
			throw null;
		}

		public virtual SqlMoney GetSqlMoney(int i)
		{
			throw null;
		}

		public virtual SqlSingle GetSqlSingle(int i)
		{
			throw null;
		}

		public virtual SqlString GetSqlString(int i)
		{
			throw null;
		}

		public virtual object GetSqlValue(int i)
		{
			throw null;
		}

		public virtual int GetSqlValues(object[] values)
		{
			throw null;
		}

		public virtual SqlXml GetSqlXml(int i)
		{
			throw null;
		}

		public override string GetString(int i)
		{
			throw null;
		}

		[MonoTODO]
		public virtual TimeSpan GetTimeSpan(int i)
		{
			throw null;
		}

		public override object GetValue(int i)
		{
			throw null;
		}

		public override int GetValues(object[] values)
		{
			throw null;
		}

		protected bool IsCommandBehavior(CommandBehavior condition)
		{
			throw null;
		}

		public override bool IsDBNull(int i)
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
	}
}
