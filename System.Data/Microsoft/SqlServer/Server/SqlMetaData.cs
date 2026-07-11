using System;
using System.Data;
using System.Data.SqlTypes;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlMetaData
	{
		public SqlMetaData(string name, SqlDbType dbType)
		{
		}

		public SqlMetaData(string name, SqlDbType dbType, byte precision, byte scale)
		{
		}

		public SqlMetaData(string name, SqlDbType dbType, long maxLength)
		{
		}

		public SqlMetaData(string name, SqlDbType dbType, long maxLength, byte precision, byte scale, long locale, SqlCompareOptions compareOptions, Type userDefinedType)
		{
		}

		public SqlMetaData(string name, SqlDbType dbType, long maxLength, long locale, SqlCompareOptions compareOptions)
		{
		}

		public SqlMetaData(string name, SqlDbType dbType, string database, string owningSchema, string objectName)
		{
		}

		[MonoTODO]
		public SqlMetaData(string name, SqlDbType dbType, Type userDefinedType)
		{
		}

		public SqlCompareOptions CompareOptions
		{
			get
			{
				throw null;
			}
		}

		public DbType DbType
		{
			get
			{
				throw null;
			}
		}

		public long LocaleId
		{
			get
			{
				throw null;
			}
		}

		public static long Max
		{
			get
			{
				throw null;
			}
		}

		public long MaxLength
		{
			get
			{
				throw null;
			}
		}

		public string Name
		{
			get
			{
				throw null;
			}
		}

		public byte Precision
		{
			get
			{
				throw null;
			}
		}

		public byte Scale
		{
			get
			{
				throw null;
			}
		}

		public SqlDbType SqlDbType
		{
			get
			{
				throw null;
			}
		}

		[MonoTODO]
		public string TypeName
		{
			get
			{
				throw null;
			}
		}

		public string XmlSchemaCollectionDatabase
		{
			get
			{
				throw null;
			}
		}

		public string XmlSchemaCollectionName
		{
			get
			{
				throw null;
			}
		}

		public string XmlSchemaCollectionOwningSchema
		{
			get
			{
				throw null;
			}
		}

		public bool Adjust(bool value)
		{
			throw null;
		}

		public byte Adjust(byte value)
		{
			throw null;
		}

		public byte[] Adjust(byte[] value)
		{
			throw null;
		}

		public char Adjust(char value)
		{
			throw null;
		}

		public char[] Adjust(char[] value)
		{
			throw null;
		}

		public SqlBinary Adjust(SqlBinary value)
		{
			throw null;
		}

		public SqlBoolean Adjust(SqlBoolean value)
		{
			throw null;
		}

		public SqlByte Adjust(SqlByte value)
		{
			throw null;
		}

		public SqlBytes Adjust(SqlBytes value)
		{
			throw null;
		}

		public SqlChars Adjust(SqlChars value)
		{
			throw null;
		}

		public SqlDateTime Adjust(SqlDateTime value)
		{
			throw null;
		}

		public SqlDecimal Adjust(SqlDecimal value)
		{
			throw null;
		}

		public SqlDouble Adjust(SqlDouble value)
		{
			throw null;
		}

		public SqlGuid Adjust(SqlGuid value)
		{
			throw null;
		}

		public SqlInt16 Adjust(SqlInt16 value)
		{
			throw null;
		}

		public SqlInt32 Adjust(SqlInt32 value)
		{
			throw null;
		}

		public SqlInt64 Adjust(SqlInt64 value)
		{
			throw null;
		}

		public SqlMoney Adjust(SqlMoney value)
		{
			throw null;
		}

		public SqlSingle Adjust(SqlSingle value)
		{
			throw null;
		}

		public SqlString Adjust(SqlString value)
		{
			throw null;
		}

		public DateTime Adjust(DateTime value)
		{
			throw null;
		}

		public decimal Adjust(decimal value)
		{
			throw null;
		}

		public double Adjust(double value)
		{
			throw null;
		}

		public Guid Adjust(Guid value)
		{
			throw null;
		}

		public short Adjust(short value)
		{
			throw null;
		}

		public int Adjust(int value)
		{
			throw null;
		}

		public long Adjust(long value)
		{
			throw null;
		}

		public object Adjust(object value)
		{
			throw null;
		}

		public float Adjust(float value)
		{
			throw null;
		}

		public string Adjust(string value)
		{
			throw null;
		}

		public static SqlMetaData InferFromValue(object value, string name)
		{
			throw null;
		}
	}
}
