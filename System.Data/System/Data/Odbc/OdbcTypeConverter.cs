using System;

namespace System.Data.Odbc
{
	internal class OdbcTypeConverter
	{
		public static OdbcTypeMap GetTypeMap(OdbcType odbcType)
		{
			return (OdbcTypeMap)OdbcTypeMap.Maps[odbcType];
		}

		public static OdbcTypeMap InferFromValue(object value)
		{
			if (!value.GetType().IsArray)
			{
				switch (Type.GetTypeCode(value.GetType()))
				{
				case TypeCode.Empty:
				case TypeCode.Object:
				case TypeCode.DBNull:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NVarChar];
				case TypeCode.Boolean:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Bit];
				case TypeCode.Char:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Char];
				case TypeCode.SByte:
					throw new ArgumentException("infering OdbcType from SByte is not supported");
				case TypeCode.Byte:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.TinyInt];
				case TypeCode.Int16:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.SmallInt];
				case TypeCode.UInt16:
				case TypeCode.Int32:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Int];
				case TypeCode.UInt32:
				case TypeCode.Int64:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.BigInt];
				case TypeCode.UInt64:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric];
				case TypeCode.Single:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Real];
				case TypeCode.Double:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Double];
				case TypeCode.Decimal:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric];
				case TypeCode.DateTime:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime];
				case TypeCode.String:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NVarChar];
				}
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
			}
			if (value.GetType().GetElementType() == typeof(byte))
			{
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Binary];
			}
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
		}

		public static OdbcTypeMap GetTypeMap(SQL_TYPE sqlType)
		{
			switch (sqlType + 11)
			{
			case (SQL_TYPE)0:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.UniqueIdentifier];
			case SQL_TYPE.CHAR:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NText];
			case SQL_TYPE.NUMERIC:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NVarChar];
			case SQL_TYPE.DECIMAL:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NChar];
			case SQL_TYPE.INTEGER:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Bit];
			case SQL_TYPE.SMALLINT:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.TinyInt];
			default:
				switch (sqlType)
				{
				case SQL_TYPE.TYPE_DATE:
				case SQL_TYPE.TYPE_TIMESTAMP:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime];
				case SQL_TYPE.TYPE_TIME:
					break;
				default:
					if (sqlType != SQL_TYPE.UNASSIGNED)
					{
						return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
					}
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
				case SQL_TYPE.INTERVAL_YEAR:
				case SQL_TYPE.INTERVAL_MONTH:
				case SQL_TYPE.INTERVAL_DAY:
				case SQL_TYPE.INTERVAL_HOUR:
				case SQL_TYPE.INTERVAL_MINUTE:
				case SQL_TYPE.INTERVAL_SECOND:
				case SQL_TYPE.INTERVAL_YEAR_TO_MONTH:
				case SQL_TYPE.INTERVAL_DAY_TO_HOUR:
				case SQL_TYPE.INTERVAL_DAY_TO_MINUTE:
				case SQL_TYPE.INTERVAL_DAY_TO_SECOND:
				case SQL_TYPE.INTERVAL_HOUR_TO_MINUTE:
				case SQL_TYPE.INTERVAL_HOUR_TO_SECOND:
				case SQL_TYPE.INTERVAL_MINUTE_TO_SECOND:
					return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime];
				}
				break;
			case SQL_TYPE.REAL:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Image];
			case SQL_TYPE.DOUBLE:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarBinary];
			case SQL_TYPE.DATE:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Binary];
			case SQL_TYPE.TIME:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Text];
			case SQL_TYPE.VARCHAR:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Char];
			case (SQL_TYPE)13:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric];
			case (SQL_TYPE)14:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Decimal];
			case (SQL_TYPE)15:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Int];
			case (SQL_TYPE)16:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.SmallInt];
			case (SQL_TYPE)18:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Real];
			case (SQL_TYPE)19:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Double];
			case (SQL_TYPE)20:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Date];
			case (SQL_TYPE)21:
				break;
			case (SQL_TYPE)22:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime];
			case (SQL_TYPE)23:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
			}
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Time];
		}

		public static OdbcTypeMap GetTypeMap(DbType dbType)
		{
			switch (dbType)
			{
			case DbType.AnsiString:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
			case DbType.Binary:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Binary];
			case DbType.Byte:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.TinyInt];
			case DbType.Boolean:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Bit];
			case DbType.Currency:
				throw new NotSupportedException("Infering OdbcType from DbType.Currency is not supported");
			case DbType.Date:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Date];
			case DbType.DateTime:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime];
			case DbType.Decimal:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric];
			case DbType.Double:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Double];
			case DbType.Guid:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.UniqueIdentifier];
			case DbType.Int16:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.SmallInt];
			case DbType.Int32:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Int];
			case DbType.Int64:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.BigInt];
			case DbType.Object:
				throw new NotSupportedException("Infering OdbcType from DbType.Object is not supported");
			case DbType.SByte:
				throw new NotSupportedException("Infering OdbcType from DbType.SByte is not supported");
			case DbType.Single:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Real];
			case DbType.String:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NVarChar];
			case DbType.Time:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Time];
			case DbType.UInt16:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Int];
			case DbType.UInt32:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.BigInt];
			case DbType.UInt64:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric];
			case DbType.VarNumeric:
				throw new NotSupportedException("Infering OdbcType from DbType.VarNumeric is not supported");
			case DbType.AnsiStringFixedLength:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Char];
			case DbType.StringFixedLength:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NChar];
			default:
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
			}
		}
	}
}
