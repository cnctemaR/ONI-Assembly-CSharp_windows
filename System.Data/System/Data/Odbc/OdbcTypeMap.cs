using System;
using System.Collections;

namespace System.Data.Odbc
{
	internal struct OdbcTypeMap
	{
		public OdbcTypeMap(DbType dbType, OdbcType odbcType, SQL_C_TYPE nativeType, SQL_TYPE sqlType)
		{
			this.DbType = dbType;
			this.OdbcType = odbcType;
			this.SqlType = sqlType;
			this.NativeType = nativeType;
		}

		static OdbcTypeMap()
		{
			OdbcTypeMap.maps[OdbcType.BigInt] = new OdbcTypeMap(DbType.Int64, OdbcType.BigInt, SQL_C_TYPE.SBIGINT, SQL_TYPE.BIGINT);
			OdbcTypeMap.maps[OdbcType.Binary] = new OdbcTypeMap(DbType.Binary, OdbcType.Binary, SQL_C_TYPE.BINARY, SQL_TYPE.BINARY);
			OdbcTypeMap.maps[OdbcType.Bit] = new OdbcTypeMap(DbType.Boolean, OdbcType.Bit, SQL_C_TYPE.BIT, SQL_TYPE.BIT);
			OdbcTypeMap.maps[OdbcType.Char] = new OdbcTypeMap(DbType.String, OdbcType.Char, SQL_C_TYPE.CHAR, SQL_TYPE.CHAR);
			OdbcTypeMap.maps[OdbcType.Date] = new OdbcTypeMap(DbType.Date, OdbcType.Date, SQL_C_TYPE.DATE, SQL_TYPE.DATE);
			OdbcTypeMap.maps[OdbcType.DateTime] = new OdbcTypeMap(DbType.DateTime, OdbcType.DateTime, SQL_C_TYPE.TIMESTAMP, SQL_TYPE.TIMESTAMP);
			OdbcTypeMap.maps[OdbcType.Decimal] = new OdbcTypeMap(DbType.Decimal, OdbcType.Decimal, SQL_C_TYPE.NUMERIC, SQL_TYPE.NUMERIC);
			OdbcTypeMap.maps[OdbcType.Double] = new OdbcTypeMap(DbType.Double, OdbcType.Double, SQL_C_TYPE.DOUBLE, SQL_TYPE.DOUBLE);
			OdbcTypeMap.maps[OdbcType.Image] = new OdbcTypeMap(DbType.Binary, OdbcType.Image, SQL_C_TYPE.BINARY, SQL_TYPE.BINARY);
			OdbcTypeMap.maps[OdbcType.Int] = new OdbcTypeMap(DbType.Int32, OdbcType.Int, SQL_C_TYPE.LONG, SQL_TYPE.INTEGER);
			OdbcTypeMap.maps[OdbcType.NChar] = new OdbcTypeMap(DbType.String, OdbcType.NChar, SQL_C_TYPE.WCHAR, SQL_TYPE.WCHAR);
			OdbcTypeMap.maps[OdbcType.NText] = new OdbcTypeMap(DbType.String, OdbcType.NText, SQL_C_TYPE.WCHAR, SQL_TYPE.WLONGVARCHAR);
			OdbcTypeMap.maps[OdbcType.Numeric] = new OdbcTypeMap(DbType.Decimal, OdbcType.Numeric, SQL_C_TYPE.CHAR, SQL_TYPE.NUMERIC);
			OdbcTypeMap.maps[OdbcType.NVarChar] = new OdbcTypeMap(DbType.String, OdbcType.NVarChar, SQL_C_TYPE.WCHAR, SQL_TYPE.WVARCHAR);
			OdbcTypeMap.maps[OdbcType.Real] = new OdbcTypeMap(DbType.Single, OdbcType.Real, SQL_C_TYPE.FLOAT, SQL_TYPE.REAL);
			OdbcTypeMap.maps[OdbcType.SmallDateTime] = new OdbcTypeMap(DbType.DateTime, OdbcType.SmallDateTime, SQL_C_TYPE.TIMESTAMP, SQL_TYPE.TIMESTAMP);
			OdbcTypeMap.maps[OdbcType.SmallInt] = new OdbcTypeMap(DbType.Int16, OdbcType.SmallInt, SQL_C_TYPE.SHORT, SQL_TYPE.SMALLINT);
			OdbcTypeMap.maps[OdbcType.Text] = new OdbcTypeMap(DbType.String, OdbcType.Text, SQL_C_TYPE.CHAR, SQL_TYPE.LONGVARCHAR);
			OdbcTypeMap.maps[OdbcType.Time] = new OdbcTypeMap(DbType.DateTime, OdbcType.Time, SQL_C_TYPE.TIME, SQL_TYPE.TIME);
			OdbcTypeMap.maps[OdbcType.Timestamp] = new OdbcTypeMap(DbType.DateTime, OdbcType.Timestamp, SQL_C_TYPE.BINARY, SQL_TYPE.BINARY);
			OdbcTypeMap.maps[OdbcType.TinyInt] = new OdbcTypeMap(DbType.SByte, OdbcType.TinyInt, SQL_C_TYPE.UTINYINT, SQL_TYPE.TINYINT);
			OdbcTypeMap.maps[OdbcType.UniqueIdentifier] = new OdbcTypeMap(DbType.Guid, OdbcType.UniqueIdentifier, SQL_C_TYPE.GUID, SQL_TYPE.GUID);
			OdbcTypeMap.maps[OdbcType.VarBinary] = new OdbcTypeMap(DbType.Binary, OdbcType.VarBinary, SQL_C_TYPE.BINARY, SQL_TYPE.VARBINARY);
			OdbcTypeMap.maps[OdbcType.VarChar] = new OdbcTypeMap(DbType.String, OdbcType.VarChar, SQL_C_TYPE.CHAR, SQL_TYPE.VARCHAR);
		}

		public static Hashtable Maps
		{
			get
			{
				return OdbcTypeMap.maps;
			}
		}

		public DbType DbType;

		public OdbcType OdbcType;

		public SQL_C_TYPE NativeType;

		public SQL_TYPE SqlType;

		private static Hashtable maps = new Hashtable();
	}
}
