using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.SqlServer.Server;

namespace System.Data.SqlClient
{
	internal sealed class MetaType
	{
		public MetaType(byte precision, byte scale, int fixedLength, bool isFixed, bool isLong, bool isPlp, byte tdsType, byte nullableTdsType, string typeName, Type classType, Type sqlType, SqlDbType sqldbType, DbType dbType, byte propBytes)
		{
			this.Precision = precision;
			this.Scale = scale;
			this.FixedLength = fixedLength;
			this.IsFixed = isFixed;
			this.IsLong = isLong;
			this.IsPlp = isPlp;
			this.TDSType = tdsType;
			this.NullableType = nullableTdsType;
			this.TypeName = typeName;
			this.SqlDbType = sqldbType;
			this.DbType = dbType;
			this.ClassType = classType;
			this.SqlType = sqlType;
			this.PropBytes = propBytes;
			this.IsAnsiType = MetaType._IsAnsiType(sqldbType);
			this.IsBinType = MetaType._IsBinType(sqldbType);
			this.IsCharType = MetaType._IsCharType(sqldbType);
			this.IsNCharType = MetaType._IsNCharType(sqldbType);
			this.IsSizeInCharacters = MetaType._IsSizeInCharacters(sqldbType);
			this.IsNewKatmaiType = MetaType._IsNewKatmaiType(sqldbType);
			this.IsVarTime = MetaType._IsVarTime(sqldbType);
			this.Is70Supported = MetaType._Is70Supported(this.SqlDbType);
			this.Is80Supported = MetaType._Is80Supported(this.SqlDbType);
			this.Is90Supported = MetaType._Is90Supported(this.SqlDbType);
			this.Is100Supported = MetaType._Is100Supported(this.SqlDbType);
		}

		public int TypeId
		{
			get
			{
				return 0;
			}
		}

		private static bool _IsAnsiType(SqlDbType type)
		{
			return type == SqlDbType.Char || type == SqlDbType.VarChar || type == SqlDbType.Text;
		}

		private static bool _IsSizeInCharacters(SqlDbType type)
		{
			return type == SqlDbType.NChar || type == SqlDbType.NVarChar || type == SqlDbType.Xml || type == SqlDbType.NText;
		}

		private static bool _IsCharType(SqlDbType type)
		{
			return type == SqlDbType.NChar || type == SqlDbType.NVarChar || type == SqlDbType.NText || type == SqlDbType.Char || type == SqlDbType.VarChar || type == SqlDbType.Text || type == SqlDbType.Xml;
		}

		private static bool _IsNCharType(SqlDbType type)
		{
			return type == SqlDbType.NChar || type == SqlDbType.NVarChar || type == SqlDbType.NText || type == SqlDbType.Xml;
		}

		private static bool _IsBinType(SqlDbType type)
		{
			return type == SqlDbType.Image || type == SqlDbType.Binary || type == SqlDbType.VarBinary || type == SqlDbType.Timestamp || type == SqlDbType.Udt || type == (SqlDbType)24;
		}

		private static bool _Is70Supported(SqlDbType type)
		{
			return type != SqlDbType.BigInt && type > SqlDbType.BigInt && type <= SqlDbType.VarChar;
		}

		private static bool _Is80Supported(SqlDbType type)
		{
			return type >= SqlDbType.BigInt && type <= SqlDbType.Variant;
		}

		private static bool _Is90Supported(SqlDbType type)
		{
			return MetaType._Is80Supported(type) || SqlDbType.Xml == type || SqlDbType.Udt == type;
		}

		private static bool _Is100Supported(SqlDbType type)
		{
			return MetaType._Is90Supported(type) || SqlDbType.Date == type || SqlDbType.Time == type || SqlDbType.DateTime2 == type || SqlDbType.DateTimeOffset == type;
		}

		private static bool _IsNewKatmaiType(SqlDbType type)
		{
			return SqlDbType.Structured == type;
		}

		internal static bool _IsVarTime(SqlDbType type)
		{
			return type == SqlDbType.Time || type == SqlDbType.DateTime2 || type == SqlDbType.DateTimeOffset;
		}

		internal static MetaType GetMetaTypeFromSqlDbType(SqlDbType target, bool isMultiValued)
		{
			switch (target)
			{
			case SqlDbType.BigInt:
				return MetaType.s_metaBigInt;
			case SqlDbType.Binary:
				return MetaType.s_metaBinary;
			case SqlDbType.Bit:
				return MetaType.s_metaBit;
			case SqlDbType.Char:
				return MetaType.s_metaChar;
			case SqlDbType.DateTime:
				return MetaType.s_metaDateTime;
			case SqlDbType.Decimal:
				return MetaType.MetaDecimal;
			case SqlDbType.Float:
				return MetaType.s_metaFloat;
			case SqlDbType.Image:
				return MetaType.MetaImage;
			case SqlDbType.Int:
				return MetaType.s_metaInt;
			case SqlDbType.Money:
				return MetaType.s_metaMoney;
			case SqlDbType.NChar:
				return MetaType.s_metaNChar;
			case SqlDbType.NText:
				return MetaType.MetaNText;
			case SqlDbType.NVarChar:
				return MetaType.MetaNVarChar;
			case SqlDbType.Real:
				return MetaType.s_metaReal;
			case SqlDbType.UniqueIdentifier:
				return MetaType.s_metaUniqueId;
			case SqlDbType.SmallDateTime:
				return MetaType.s_metaSmallDateTime;
			case SqlDbType.SmallInt:
				return MetaType.s_metaSmallInt;
			case SqlDbType.SmallMoney:
				return MetaType.s_metaSmallMoney;
			case SqlDbType.Text:
				return MetaType.MetaText;
			case SqlDbType.Timestamp:
				return MetaType.s_metaTimestamp;
			case SqlDbType.TinyInt:
				return MetaType.s_metaTinyInt;
			case SqlDbType.VarBinary:
				return MetaType.MetaVarBinary;
			case SqlDbType.VarChar:
				return MetaType.s_metaVarChar;
			case SqlDbType.Variant:
				return MetaType.s_metaVariant;
			case (SqlDbType)24:
				return MetaType.s_metaSmallVarBinary;
			case SqlDbType.Xml:
				return MetaType.MetaXml;
			case SqlDbType.Udt:
				return MetaType.MetaUdt;
			case SqlDbType.Structured:
				if (isMultiValued)
				{
					return MetaType.s_metaTable;
				}
				return MetaType.s_metaSUDT;
			case SqlDbType.Date:
				return MetaType.s_metaDate;
			case SqlDbType.Time:
				return MetaType.MetaTime;
			case SqlDbType.DateTime2:
				return MetaType.s_metaDateTime2;
			case SqlDbType.DateTimeOffset:
				return MetaType.MetaDateTimeOffset;
			}
			throw SQL.InvalidSqlDbType(target);
		}

		internal static MetaType GetMetaTypeFromDbType(DbType target)
		{
			switch (target)
			{
			case DbType.AnsiString:
				return MetaType.s_metaVarChar;
			case DbType.Binary:
				return MetaType.MetaVarBinary;
			case DbType.Byte:
				return MetaType.s_metaTinyInt;
			case DbType.Boolean:
				return MetaType.s_metaBit;
			case DbType.Currency:
				return MetaType.s_metaMoney;
			case DbType.Date:
			case DbType.DateTime:
				return MetaType.s_metaDateTime;
			case DbType.Decimal:
				return MetaType.MetaDecimal;
			case DbType.Double:
				return MetaType.s_metaFloat;
			case DbType.Guid:
				return MetaType.s_metaUniqueId;
			case DbType.Int16:
				return MetaType.s_metaSmallInt;
			case DbType.Int32:
				return MetaType.s_metaInt;
			case DbType.Int64:
				return MetaType.s_metaBigInt;
			case DbType.Object:
				return MetaType.s_metaVariant;
			case DbType.Single:
				return MetaType.s_metaReal;
			case DbType.String:
				return MetaType.MetaNVarChar;
			case DbType.Time:
				return MetaType.s_metaDateTime;
			case DbType.AnsiStringFixedLength:
				return MetaType.s_metaChar;
			case DbType.StringFixedLength:
				return MetaType.s_metaNChar;
			case DbType.Xml:
				return MetaType.MetaXml;
			case DbType.DateTime2:
				return MetaType.s_metaDateTime2;
			case DbType.DateTimeOffset:
				return MetaType.MetaDateTimeOffset;
			}
			throw ADP.DbTypeNotSupported(target, typeof(SqlDbType));
		}

		internal static MetaType GetMaxMetaTypeFromMetaType(MetaType mt)
		{
			SqlDbType sqlDbType = mt.SqlDbType;
			if (sqlDbType <= SqlDbType.NChar)
			{
				if (sqlDbType != SqlDbType.Binary)
				{
					if (sqlDbType == SqlDbType.Char)
					{
						goto IL_003E;
					}
					if (sqlDbType != SqlDbType.NChar)
					{
						return mt;
					}
					goto IL_0044;
				}
			}
			else if (sqlDbType <= SqlDbType.VarBinary)
			{
				if (sqlDbType == SqlDbType.NVarChar)
				{
					goto IL_0044;
				}
				if (sqlDbType != SqlDbType.VarBinary)
				{
					return mt;
				}
			}
			else
			{
				if (sqlDbType == SqlDbType.VarChar)
				{
					goto IL_003E;
				}
				if (sqlDbType != SqlDbType.Udt)
				{
					return mt;
				}
				return MetaType.s_metaMaxUdt;
			}
			return MetaType.MetaMaxVarBinary;
			IL_003E:
			return MetaType.MetaMaxVarChar;
			IL_0044:
			return MetaType.MetaMaxNVarChar;
		}

		internal static MetaType GetMetaTypeFromType(Type dataType)
		{
			return MetaType.GetMetaTypeFromValue(dataType, null, false, true);
		}

		internal static MetaType GetMetaTypeFromValue(object value, bool streamAllowed = true)
		{
			return MetaType.GetMetaTypeFromValue(value.GetType(), value, true, streamAllowed);
		}

		private static MetaType GetMetaTypeFromValue(Type dataType, object value, bool inferLen, bool streamAllowed)
		{
			switch (Type.GetTypeCode(dataType))
			{
			case TypeCode.Empty:
				throw ADP.InvalidDataType(TypeCode.Empty);
			case TypeCode.Object:
				if (dataType == typeof(byte[]))
				{
					if (!inferLen || ((byte[])value).Length <= 8000)
					{
						return MetaType.MetaVarBinary;
					}
					return MetaType.MetaImage;
				}
				else
				{
					if (dataType == typeof(Guid))
					{
						return MetaType.s_metaUniqueId;
					}
					if (dataType == typeof(object))
					{
						return MetaType.s_metaVariant;
					}
					if (dataType == typeof(SqlBinary))
					{
						return MetaType.MetaVarBinary;
					}
					if (dataType == typeof(SqlBoolean))
					{
						return MetaType.s_metaBit;
					}
					if (dataType == typeof(SqlByte))
					{
						return MetaType.s_metaTinyInt;
					}
					if (dataType == typeof(SqlBytes))
					{
						return MetaType.MetaVarBinary;
					}
					if (dataType == typeof(SqlChars))
					{
						return MetaType.MetaNVarChar;
					}
					if (dataType == typeof(SqlDateTime))
					{
						return MetaType.s_metaDateTime;
					}
					if (dataType == typeof(SqlDouble))
					{
						return MetaType.s_metaFloat;
					}
					if (dataType == typeof(SqlGuid))
					{
						return MetaType.s_metaUniqueId;
					}
					if (dataType == typeof(SqlInt16))
					{
						return MetaType.s_metaSmallInt;
					}
					if (dataType == typeof(SqlInt32))
					{
						return MetaType.s_metaInt;
					}
					if (dataType == typeof(SqlInt64))
					{
						return MetaType.s_metaBigInt;
					}
					if (dataType == typeof(SqlMoney))
					{
						return MetaType.s_metaMoney;
					}
					if (dataType == typeof(SqlDecimal))
					{
						return MetaType.MetaDecimal;
					}
					if (dataType == typeof(SqlSingle))
					{
						return MetaType.s_metaReal;
					}
					if (dataType == typeof(SqlXml))
					{
						return MetaType.MetaXml;
					}
					if (dataType == typeof(SqlString))
					{
						if (!inferLen || ((SqlString)value).IsNull)
						{
							return MetaType.MetaNVarChar;
						}
						return MetaType.PromoteStringType(((SqlString)value).Value);
					}
					else
					{
						if (dataType == typeof(IEnumerable<DbDataRecord>) || dataType == typeof(DataTable))
						{
							return MetaType.s_metaTable;
						}
						if (dataType == typeof(TimeSpan))
						{
							return MetaType.MetaTime;
						}
						if (dataType == typeof(DateTimeOffset))
						{
							return MetaType.MetaDateTimeOffset;
						}
						if (SqlUdtInfo.TryGetFromType(dataType) != null)
						{
							return MetaType.MetaUdt;
						}
						if (streamAllowed)
						{
							if (typeof(Stream).IsAssignableFrom(dataType))
							{
								return MetaType.MetaVarBinary;
							}
							if (typeof(TextReader).IsAssignableFrom(dataType))
							{
								return MetaType.MetaNVarChar;
							}
							if (typeof(XmlReader).IsAssignableFrom(dataType))
							{
								return MetaType.MetaXml;
							}
						}
						throw ADP.UnknownDataType(dataType);
					}
				}
				break;
			case TypeCode.DBNull:
				throw ADP.InvalidDataType(TypeCode.DBNull);
			case TypeCode.Boolean:
				return MetaType.s_metaBit;
			case TypeCode.Char:
				throw ADP.InvalidDataType(TypeCode.Char);
			case TypeCode.SByte:
				throw ADP.InvalidDataType(TypeCode.SByte);
			case TypeCode.Byte:
				return MetaType.s_metaTinyInt;
			case TypeCode.Int16:
				return MetaType.s_metaSmallInt;
			case TypeCode.UInt16:
				throw ADP.InvalidDataType(TypeCode.UInt16);
			case TypeCode.Int32:
				return MetaType.s_metaInt;
			case TypeCode.UInt32:
				throw ADP.InvalidDataType(TypeCode.UInt32);
			case TypeCode.Int64:
				return MetaType.s_metaBigInt;
			case TypeCode.UInt64:
				throw ADP.InvalidDataType(TypeCode.UInt64);
			case TypeCode.Single:
				return MetaType.s_metaReal;
			case TypeCode.Double:
				return MetaType.s_metaFloat;
			case TypeCode.Decimal:
				return MetaType.MetaDecimal;
			case TypeCode.DateTime:
				return MetaType.s_metaDateTime;
			case TypeCode.String:
				if (!inferLen)
				{
					return MetaType.MetaNVarChar;
				}
				return MetaType.PromoteStringType((string)value);
			}
			throw ADP.UnknownDataTypeCode(dataType, Type.GetTypeCode(dataType));
		}

		internal static object GetNullSqlValue(Type sqlType)
		{
			if (sqlType == typeof(SqlSingle))
			{
				return SqlSingle.Null;
			}
			if (sqlType == typeof(SqlString))
			{
				return SqlString.Null;
			}
			if (sqlType == typeof(SqlDouble))
			{
				return SqlDouble.Null;
			}
			if (sqlType == typeof(SqlBinary))
			{
				return SqlBinary.Null;
			}
			if (sqlType == typeof(SqlGuid))
			{
				return SqlGuid.Null;
			}
			if (sqlType == typeof(SqlBoolean))
			{
				return SqlBoolean.Null;
			}
			if (sqlType == typeof(SqlByte))
			{
				return SqlByte.Null;
			}
			if (sqlType == typeof(SqlInt16))
			{
				return SqlInt16.Null;
			}
			if (sqlType == typeof(SqlInt32))
			{
				return SqlInt32.Null;
			}
			if (sqlType == typeof(SqlInt64))
			{
				return SqlInt64.Null;
			}
			if (sqlType == typeof(SqlDecimal))
			{
				return SqlDecimal.Null;
			}
			if (sqlType == typeof(SqlDateTime))
			{
				return SqlDateTime.Null;
			}
			if (sqlType == typeof(SqlMoney))
			{
				return SqlMoney.Null;
			}
			if (sqlType == typeof(SqlXml))
			{
				return SqlXml.Null;
			}
			if (sqlType == typeof(object))
			{
				return DBNull.Value;
			}
			if (sqlType == typeof(IEnumerable<DbDataRecord>))
			{
				return DBNull.Value;
			}
			if (sqlType == typeof(DataTable))
			{
				return DBNull.Value;
			}
			if (sqlType == typeof(DateTime))
			{
				return DBNull.Value;
			}
			if (sqlType == typeof(TimeSpan))
			{
				return DBNull.Value;
			}
			sqlType == typeof(DateTimeOffset);
			return DBNull.Value;
		}

		internal static MetaType PromoteStringType(string s)
		{
			if (s.Length << 1 > 8000)
			{
				return MetaType.s_metaVarChar;
			}
			return MetaType.MetaNVarChar;
		}

		internal static object GetComValueFromSqlVariant(object sqlVal)
		{
			object obj = null;
			if (ADP.IsNull(sqlVal))
			{
				return obj;
			}
			if (sqlVal is SqlSingle)
			{
				obj = ((SqlSingle)sqlVal).Value;
			}
			else if (sqlVal is SqlString)
			{
				obj = ((SqlString)sqlVal).Value;
			}
			else if (sqlVal is SqlDouble)
			{
				obj = ((SqlDouble)sqlVal).Value;
			}
			else if (sqlVal is SqlBinary)
			{
				obj = ((SqlBinary)sqlVal).Value;
			}
			else if (sqlVal is SqlGuid)
			{
				obj = ((SqlGuid)sqlVal).Value;
			}
			else if (sqlVal is SqlBoolean)
			{
				obj = ((SqlBoolean)sqlVal).Value;
			}
			else if (sqlVal is SqlByte)
			{
				obj = ((SqlByte)sqlVal).Value;
			}
			else if (sqlVal is SqlInt16)
			{
				obj = ((SqlInt16)sqlVal).Value;
			}
			else if (sqlVal is SqlInt32)
			{
				obj = ((SqlInt32)sqlVal).Value;
			}
			else if (sqlVal is SqlInt64)
			{
				obj = ((SqlInt64)sqlVal).Value;
			}
			else if (sqlVal is SqlDecimal)
			{
				obj = ((SqlDecimal)sqlVal).Value;
			}
			else if (sqlVal is SqlDateTime)
			{
				obj = ((SqlDateTime)sqlVal).Value;
			}
			else if (sqlVal is SqlMoney)
			{
				obj = ((SqlMoney)sqlVal).Value;
			}
			else if (sqlVal is SqlXml)
			{
				obj = ((SqlXml)sqlVal).Value;
			}
			return obj;
		}

		[Conditional("DEBUG")]
		private static void AssertIsUserDefinedTypeInstance(object sqlValue, string failedAssertMessage)
		{
			SqlUserDefinedTypeAttribute[] array = (SqlUserDefinedTypeAttribute[])sqlValue.GetType().GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), true);
		}

		internal static object GetSqlValueFromComVariant(object comVal)
		{
			object obj = null;
			if (comVal != null && DBNull.Value != comVal)
			{
				if (comVal is float)
				{
					obj = new SqlSingle((float)comVal);
				}
				else if (comVal is string)
				{
					obj = new SqlString((string)comVal);
				}
				else if (comVal is double)
				{
					obj = new SqlDouble((double)comVal);
				}
				else if (comVal is byte[])
				{
					obj = new SqlBinary((byte[])comVal);
				}
				else if (comVal is char)
				{
					obj = new SqlString(((char)comVal).ToString());
				}
				else if (comVal is char[])
				{
					obj = new SqlChars((char[])comVal);
				}
				else if (comVal is Guid)
				{
					obj = new SqlGuid((Guid)comVal);
				}
				else if (comVal is bool)
				{
					obj = new SqlBoolean((bool)comVal);
				}
				else if (comVal is byte)
				{
					obj = new SqlByte((byte)comVal);
				}
				else if (comVal is short)
				{
					obj = new SqlInt16((short)comVal);
				}
				else if (comVal is int)
				{
					obj = new SqlInt32((int)comVal);
				}
				else if (comVal is long)
				{
					obj = new SqlInt64((long)comVal);
				}
				else if (comVal is decimal)
				{
					obj = new SqlDecimal((decimal)comVal);
				}
				else if (comVal is DateTime)
				{
					obj = new SqlDateTime((DateTime)comVal);
				}
				else if (comVal is XmlReader)
				{
					obj = new SqlXml((XmlReader)comVal);
				}
				else if (comVal is TimeSpan || comVal is DateTimeOffset)
				{
					obj = comVal;
				}
			}
			return obj;
		}

		internal static SqlDbType GetSqlDbTypeFromOleDbType(short dbType, string typeName)
		{
			return SqlDbType.Variant;
		}

		internal static MetaType GetSqlDataType(int tdsType, uint userType, int length)
		{
			if (tdsType <= 165)
			{
				if (tdsType <= 111)
				{
					switch (tdsType)
					{
					case 31:
					case 32:
					case 33:
					case 44:
					case 46:
					case 49:
					case 51:
					case 53:
					case 54:
					case 55:
					case 57:
						goto IL_0279;
					case 34:
						return MetaType.MetaImage;
					case 35:
						return MetaType.MetaText;
					case 36:
						return MetaType.s_metaUniqueId;
					case 37:
						return MetaType.s_metaSmallVarBinary;
					case 38:
						if (4 > length)
						{
							if (2 != length)
							{
								return MetaType.s_metaTinyInt;
							}
							return MetaType.s_metaSmallInt;
						}
						else
						{
							if (4 != length)
							{
								return MetaType.s_metaBigInt;
							}
							return MetaType.s_metaInt;
						}
						break;
					case 39:
						goto IL_01C6;
					case 40:
						return MetaType.s_metaDate;
					case 41:
						return MetaType.MetaTime;
					case 42:
						return MetaType.s_metaDateTime2;
					case 43:
						return MetaType.MetaDateTimeOffset;
					case 45:
						goto IL_01CC;
					case 47:
						goto IL_01E3;
					case 48:
						return MetaType.s_metaTinyInt;
					case 50:
						break;
					case 52:
						return MetaType.s_metaSmallInt;
					case 56:
						return MetaType.s_metaInt;
					case 58:
						return MetaType.s_metaSmallDateTime;
					case 59:
						return MetaType.s_metaReal;
					case 60:
						return MetaType.s_metaMoney;
					case 61:
						return MetaType.s_metaDateTime;
					case 62:
						return MetaType.s_metaFloat;
					default:
						switch (tdsType)
						{
						case 98:
							return MetaType.s_metaVariant;
						case 99:
							return MetaType.MetaNText;
						case 100:
						case 101:
						case 102:
						case 103:
						case 105:
						case 107:
							goto IL_0279;
						case 104:
							break;
						case 106:
						case 108:
							return MetaType.MetaDecimal;
						case 109:
							if (4 != length)
							{
								return MetaType.s_metaFloat;
							}
							return MetaType.s_metaReal;
						case 110:
							if (4 != length)
							{
								return MetaType.s_metaMoney;
							}
							return MetaType.s_metaSmallMoney;
						case 111:
							if (4 != length)
							{
								return MetaType.s_metaDateTime;
							}
							return MetaType.s_metaSmallDateTime;
						default:
							goto IL_0279;
						}
						break;
					}
					return MetaType.s_metaBit;
				}
				if (tdsType == 122)
				{
					return MetaType.s_metaSmallMoney;
				}
				if (tdsType == 127)
				{
					return MetaType.s_metaBigInt;
				}
				if (tdsType != 165)
				{
					goto IL_0279;
				}
				return MetaType.MetaVarBinary;
			}
			else if (tdsType <= 173)
			{
				if (tdsType != 167)
				{
					if (tdsType != 173)
					{
						goto IL_0279;
					}
					goto IL_01CC;
				}
			}
			else
			{
				if (tdsType == 175)
				{
					goto IL_01E3;
				}
				if (tdsType == 231)
				{
					return MetaType.MetaNVarChar;
				}
				switch (tdsType)
				{
				case 239:
					return MetaType.s_metaNChar;
				case 240:
					return MetaType.MetaUdt;
				case 241:
					return MetaType.MetaXml;
				case 242:
					goto IL_0279;
				case 243:
					return MetaType.s_metaTable;
				default:
					goto IL_0279;
				}
			}
			IL_01C6:
			return MetaType.s_metaVarChar;
			IL_01CC:
			if (80U != userType)
			{
				return MetaType.s_metaBinary;
			}
			return MetaType.s_metaTimestamp;
			IL_01E3:
			return MetaType.s_metaChar;
			IL_0279:
			throw SQL.InvalidSqlDbType((SqlDbType)tdsType);
		}

		internal static MetaType GetDefaultMetaType()
		{
			return MetaType.MetaNVarChar;
		}

		internal static string GetStringFromXml(XmlReader xmlreader)
		{
			return new SqlXml(xmlreader).Value;
		}

		public static TdsDateTime FromDateTime(DateTime dateTime, byte cb)
		{
			TdsDateTime tdsDateTime = default(TdsDateTime);
			SqlDateTime sqlDateTime;
			if (cb == 8)
			{
				sqlDateTime = new SqlDateTime(dateTime);
				tdsDateTime.time = sqlDateTime.TimeTicks;
			}
			else
			{
				sqlDateTime = new SqlDateTime(dateTime.AddSeconds(30.0));
				tdsDateTime.time = sqlDateTime.TimeTicks / SqlDateTime.SQLTicksPerMinute;
			}
			tdsDateTime.days = sqlDateTime.DayTicks;
			return tdsDateTime;
		}

		public static DateTime ToDateTime(int sqlDays, int sqlTime, int length)
		{
			if (length == 4)
			{
				return new SqlDateTime(sqlDays, sqlTime * SqlDateTime.SQLTicksPerMinute).Value;
			}
			return new SqlDateTime(sqlDays, sqlTime).Value;
		}

		internal static int GetTimeSizeFromScale(byte scale)
		{
			if (scale <= 2)
			{
				return 3;
			}
			if (scale <= 4)
			{
				return 4;
			}
			return 5;
		}

		internal readonly Type ClassType;

		internal readonly Type SqlType;

		internal readonly int FixedLength;

		internal readonly bool IsFixed;

		internal readonly bool IsLong;

		internal readonly bool IsPlp;

		internal readonly byte Precision;

		internal readonly byte Scale;

		internal readonly byte TDSType;

		internal readonly byte NullableType;

		internal readonly string TypeName;

		internal readonly SqlDbType SqlDbType;

		internal readonly DbType DbType;

		internal readonly byte PropBytes;

		internal readonly bool IsAnsiType;

		internal readonly bool IsBinType;

		internal readonly bool IsCharType;

		internal readonly bool IsNCharType;

		internal readonly bool IsSizeInCharacters;

		internal readonly bool IsNewKatmaiType;

		internal readonly bool IsVarTime;

		internal readonly bool Is70Supported;

		internal readonly bool Is80Supported;

		internal readonly bool Is90Supported;

		internal readonly bool Is100Supported;

		private static readonly MetaType s_metaBigInt = new MetaType(19, byte.MaxValue, 8, true, false, false, 127, 38, "bigint", typeof(long), typeof(SqlInt64), SqlDbType.BigInt, DbType.Int64, 0);

		private static readonly MetaType s_metaFloat = new MetaType(15, byte.MaxValue, 8, true, false, false, 62, 109, "float", typeof(double), typeof(SqlDouble), SqlDbType.Float, DbType.Double, 0);

		private static readonly MetaType s_metaReal = new MetaType(7, byte.MaxValue, 4, true, false, false, 59, 109, "real", typeof(float), typeof(SqlSingle), SqlDbType.Real, DbType.Single, 0);

		private static readonly MetaType s_metaBinary = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 173, 173, "binary", typeof(byte[]), typeof(SqlBinary), SqlDbType.Binary, DbType.Binary, 2);

		private static readonly MetaType s_metaTimestamp = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 173, 173, "timestamp", typeof(byte[]), typeof(SqlBinary), SqlDbType.Timestamp, DbType.Binary, 2);

		internal static readonly MetaType MetaVarBinary = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 165, 165, "varbinary", typeof(byte[]), typeof(SqlBinary), SqlDbType.VarBinary, DbType.Binary, 2);

		internal static readonly MetaType MetaMaxVarBinary = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, 165, 165, "varbinary", typeof(byte[]), typeof(SqlBinary), SqlDbType.VarBinary, DbType.Binary, 2);

		private static readonly MetaType s_metaSmallVarBinary = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 37, 173, ADP.StrEmpty, typeof(byte[]), typeof(SqlBinary), (SqlDbType)24, DbType.Binary, 2);

		internal static readonly MetaType MetaImage = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, false, 34, 34, "image", typeof(byte[]), typeof(SqlBinary), SqlDbType.Image, DbType.Binary, 0);

		private static readonly MetaType s_metaBit = new MetaType(byte.MaxValue, byte.MaxValue, 1, true, false, false, 50, 104, "bit", typeof(bool), typeof(SqlBoolean), SqlDbType.Bit, DbType.Boolean, 0);

		private static readonly MetaType s_metaTinyInt = new MetaType(3, byte.MaxValue, 1, true, false, false, 48, 38, "tinyint", typeof(byte), typeof(SqlByte), SqlDbType.TinyInt, DbType.Byte, 0);

		private static readonly MetaType s_metaSmallInt = new MetaType(5, byte.MaxValue, 2, true, false, false, 52, 38, "smallint", typeof(short), typeof(SqlInt16), SqlDbType.SmallInt, DbType.Int16, 0);

		private static readonly MetaType s_metaInt = new MetaType(10, byte.MaxValue, 4, true, false, false, 56, 38, "int", typeof(int), typeof(SqlInt32), SqlDbType.Int, DbType.Int32, 0);

		private static readonly MetaType s_metaChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 175, 175, "char", typeof(string), typeof(SqlString), SqlDbType.Char, DbType.AnsiStringFixedLength, 7);

		private static readonly MetaType s_metaVarChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 167, 167, "varchar", typeof(string), typeof(SqlString), SqlDbType.VarChar, DbType.AnsiString, 7);

		internal static readonly MetaType MetaMaxVarChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, 167, 167, "varchar", typeof(string), typeof(SqlString), SqlDbType.VarChar, DbType.AnsiString, 7);

		internal static readonly MetaType MetaText = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, false, 35, 35, "text", typeof(string), typeof(SqlString), SqlDbType.Text, DbType.AnsiString, 0);

		private static readonly MetaType s_metaNChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 239, 239, "nchar", typeof(string), typeof(SqlString), SqlDbType.NChar, DbType.StringFixedLength, 7);

		internal static readonly MetaType MetaNVarChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 231, 231, "nvarchar", typeof(string), typeof(SqlString), SqlDbType.NVarChar, DbType.String, 7);

		internal static readonly MetaType MetaMaxNVarChar = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, 231, 231, "nvarchar", typeof(string), typeof(SqlString), SqlDbType.NVarChar, DbType.String, 7);

		internal static readonly MetaType MetaNText = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, false, 99, 99, "ntext", typeof(string), typeof(SqlString), SqlDbType.NText, DbType.String, 7);

		internal static readonly MetaType MetaDecimal = new MetaType(38, 4, 17, true, false, false, 108, 108, "decimal", typeof(decimal), typeof(SqlDecimal), SqlDbType.Decimal, DbType.Decimal, 2);

		internal static readonly MetaType MetaXml = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, 241, 241, "xml", typeof(string), typeof(SqlXml), SqlDbType.Xml, DbType.Xml, 0);

		private static readonly MetaType s_metaDateTime = new MetaType(23, 3, 8, true, false, false, 61, 111, "datetime", typeof(DateTime), typeof(SqlDateTime), SqlDbType.DateTime, DbType.DateTime, 0);

		private static readonly MetaType s_metaSmallDateTime = new MetaType(16, 0, 4, true, false, false, 58, 111, "smalldatetime", typeof(DateTime), typeof(SqlDateTime), SqlDbType.SmallDateTime, DbType.DateTime, 0);

		private static readonly MetaType s_metaMoney = new MetaType(19, byte.MaxValue, 8, true, false, false, 60, 110, "money", typeof(decimal), typeof(SqlMoney), SqlDbType.Money, DbType.Currency, 0);

		private static readonly MetaType s_metaSmallMoney = new MetaType(10, byte.MaxValue, 4, true, false, false, 122, 110, "smallmoney", typeof(decimal), typeof(SqlMoney), SqlDbType.SmallMoney, DbType.Currency, 0);

		private static readonly MetaType s_metaUniqueId = new MetaType(byte.MaxValue, byte.MaxValue, 16, true, false, false, 36, 36, "uniqueidentifier", typeof(Guid), typeof(SqlGuid), SqlDbType.UniqueIdentifier, DbType.Guid, 0);

		private static readonly MetaType s_metaVariant = new MetaType(byte.MaxValue, byte.MaxValue, -1, true, false, false, 98, 98, "sql_variant", typeof(object), typeof(object), SqlDbType.Variant, DbType.Object, 0);

		internal static readonly MetaType MetaUdt = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, true, 240, 240, "udt", typeof(object), typeof(object), SqlDbType.Udt, DbType.Object, 0);

		private static readonly MetaType s_metaMaxUdt = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, true, true, 240, 240, "udt", typeof(object), typeof(object), SqlDbType.Udt, DbType.Object, 0);

		private static readonly MetaType s_metaTable = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 243, 243, "table", typeof(IEnumerable<DbDataRecord>), typeof(IEnumerable<DbDataRecord>), SqlDbType.Structured, DbType.Object, 0);

		private static readonly MetaType s_metaSUDT = new MetaType(byte.MaxValue, byte.MaxValue, -1, false, false, false, 31, 31, "", typeof(SqlDataRecord), typeof(SqlDataRecord), SqlDbType.Structured, DbType.Object, 0);

		private static readonly MetaType s_metaDate = new MetaType(byte.MaxValue, byte.MaxValue, 3, true, false, false, 40, 40, "date", typeof(DateTime), typeof(DateTime), SqlDbType.Date, DbType.Date, 0);

		internal static readonly MetaType MetaTime = new MetaType(byte.MaxValue, 7, -1, false, false, false, 41, 41, "time", typeof(TimeSpan), typeof(TimeSpan), SqlDbType.Time, DbType.Time, 1);

		private static readonly MetaType s_metaDateTime2 = new MetaType(byte.MaxValue, 7, -1, false, false, false, 42, 42, "datetime2", typeof(DateTime), typeof(DateTime), SqlDbType.DateTime2, DbType.DateTime2, 1);

		internal static readonly MetaType MetaDateTimeOffset = new MetaType(byte.MaxValue, 7, -1, false, false, false, 43, 43, "datetimeoffset", typeof(DateTimeOffset), typeof(DateTimeOffset), SqlDbType.DateTimeOffset, DbType.DateTimeOffset, 1);

		private static class MetaTypeName
		{
			public const string BIGINT = "bigint";

			public const string BINARY = "binary";

			public const string BIT = "bit";

			public const string CHAR = "char";

			public const string DATETIME = "datetime";

			public const string DECIMAL = "decimal";

			public const string FLOAT = "float";

			public const string IMAGE = "image";

			public const string INT = "int";

			public const string MONEY = "money";

			public const string NCHAR = "nchar";

			public const string NTEXT = "ntext";

			public const string NVARCHAR = "nvarchar";

			public const string REAL = "real";

			public const string ROWGUID = "uniqueidentifier";

			public const string SMALLDATETIME = "smalldatetime";

			public const string SMALLINT = "smallint";

			public const string SMALLMONEY = "smallmoney";

			public const string TEXT = "text";

			public const string TIMESTAMP = "timestamp";

			public const string TINYINT = "tinyint";

			public const string UDT = "udt";

			public const string VARBINARY = "varbinary";

			public const string VARCHAR = "varchar";

			public const string VARIANT = "sql_variant";

			public const string XML = "xml";

			public const string TABLE = "table";

			public const string DATE = "date";

			public const string TIME = "time";

			public const string DATETIME2 = "datetime2";

			public const string DATETIMEOFFSET = "datetimeoffset";
		}
	}
}
