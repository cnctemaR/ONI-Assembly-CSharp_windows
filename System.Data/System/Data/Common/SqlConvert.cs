using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.Numerics;
using System.Xml;

namespace System.Data.Common
{
	internal static class SqlConvert
	{
		public static SqlByte ConvertToSqlByte(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlByte.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			if (storageType == StorageType.Byte)
			{
				return (byte)value;
			}
			if (storageType == StorageType.SqlByte)
			{
				return (SqlByte)value;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlByte));
		}

		public static SqlInt16 ConvertToSqlInt16(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlInt16.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			if (storageType <= StorageType.Int16)
			{
				if (storageType == StorageType.Byte)
				{
					return (short)((byte)value);
				}
				if (storageType == StorageType.Int16)
				{
					return (short)value;
				}
			}
			else
			{
				if (storageType == StorageType.SqlByte)
				{
					return (SqlByte)value;
				}
				if (storageType == StorageType.SqlInt16)
				{
					return (SqlInt16)value;
				}
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlInt16));
		}

		public static SqlInt32 ConvertToSqlInt32(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlInt32.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			if (storageType <= StorageType.SqlByte)
			{
				switch (storageType)
				{
				case StorageType.Byte:
					return (int)((byte)value);
				case StorageType.Int16:
					return (int)((short)value);
				case StorageType.UInt16:
					return (int)((ushort)value);
				case StorageType.Int32:
					return (int)value;
				default:
					if (storageType == StorageType.SqlByte)
					{
						return (SqlByte)value;
					}
					break;
				}
			}
			else
			{
				if (storageType == StorageType.SqlInt16)
				{
					return (SqlInt16)value;
				}
				if (storageType == StorageType.SqlInt32)
				{
					return (SqlInt32)value;
				}
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlInt32));
		}

		public static SqlInt64 ConvertToSqlInt64(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlInt32.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			switch (storageType)
			{
			case StorageType.Byte:
				return (long)((ulong)((byte)value));
			case StorageType.Int16:
				return (long)((short)value);
			case StorageType.UInt16:
				return (long)((ulong)((ushort)value));
			case StorageType.Int32:
				return (long)((int)value);
			case StorageType.UInt32:
				return (long)((ulong)((uint)value));
			case StorageType.Int64:
				return (long)value;
			default:
				if (storageType == StorageType.SqlByte)
				{
					return (SqlByte)value;
				}
				switch (storageType)
				{
				case StorageType.SqlInt16:
					return (SqlInt16)value;
				case StorageType.SqlInt32:
					return (SqlInt32)value;
				case StorageType.SqlInt64:
					return (SqlInt64)value;
				default:
					throw ExceptionBuilder.ConvertFailed(type, typeof(SqlInt64));
				}
				break;
			}
		}

		public static SqlDouble ConvertToSqlDouble(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlDouble.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			switch (storageType)
			{
			case StorageType.Byte:
				return (double)((byte)value);
			case StorageType.Int16:
				return (double)((short)value);
			case StorageType.UInt16:
				return (double)((ushort)value);
			case StorageType.Int32:
				return (double)((int)value);
			case StorageType.UInt32:
				return (uint)value;
			case StorageType.Int64:
				return (double)((long)value);
			case StorageType.UInt64:
				return (ulong)value;
			case StorageType.Single:
				return (double)((float)value);
			case StorageType.Double:
				return (double)value;
			default:
				switch (storageType)
				{
				case StorageType.SqlByte:
					return (SqlByte)value;
				case StorageType.SqlDecimal:
					return (SqlDecimal)value;
				case StorageType.SqlDouble:
					return (SqlDouble)value;
				case StorageType.SqlInt16:
					return (SqlInt16)value;
				case StorageType.SqlInt32:
					return (SqlInt32)value;
				case StorageType.SqlInt64:
					return (SqlInt64)value;
				case StorageType.SqlMoney:
					return (SqlMoney)value;
				case StorageType.SqlSingle:
					return (SqlSingle)value;
				}
				throw ExceptionBuilder.ConvertFailed(type, typeof(SqlDouble));
			}
		}

		public static SqlDecimal ConvertToSqlDecimal(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlDecimal.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			switch (storageType)
			{
			case StorageType.Byte:
				return (long)((ulong)((byte)value));
			case StorageType.Int16:
				return (long)((short)value);
			case StorageType.UInt16:
				return (long)((ulong)((ushort)value));
			case StorageType.Int32:
				return (long)((int)value);
			case StorageType.UInt32:
				return (long)((ulong)((uint)value));
			case StorageType.Int64:
				return (long)value;
			case StorageType.UInt64:
				return (ulong)value;
			case StorageType.Single:
			case StorageType.Double:
				break;
			case StorageType.Decimal:
				return (decimal)value;
			default:
				switch (storageType)
				{
				case StorageType.SqlByte:
					return (SqlByte)value;
				case StorageType.SqlDecimal:
					return (SqlDecimal)value;
				case StorageType.SqlInt16:
					return (SqlInt16)value;
				case StorageType.SqlInt32:
					return (SqlInt32)value;
				case StorageType.SqlInt64:
					return (SqlInt64)value;
				case StorageType.SqlMoney:
					return (SqlMoney)value;
				}
				break;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlDecimal));
		}

		public static SqlSingle ConvertToSqlSingle(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlSingle.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			switch (storageType)
			{
			case StorageType.Byte:
				return (float)((byte)value);
			case StorageType.Int16:
				return (float)((short)value);
			case StorageType.UInt16:
				return (float)((ushort)value);
			case StorageType.Int32:
				return (float)((int)value);
			case StorageType.UInt32:
				return (uint)value;
			case StorageType.Int64:
				return (float)((long)value);
			case StorageType.UInt64:
				return (ulong)value;
			case StorageType.Single:
				return (float)value;
			default:
				switch (storageType)
				{
				case StorageType.SqlByte:
					return (SqlByte)value;
				case StorageType.SqlDecimal:
					return (SqlDecimal)value;
				case StorageType.SqlInt16:
					return (SqlInt16)value;
				case StorageType.SqlInt32:
					return (SqlInt32)value;
				case StorageType.SqlInt64:
					return (SqlInt64)value;
				case StorageType.SqlMoney:
					return (SqlMoney)value;
				case StorageType.SqlSingle:
					return (SqlSingle)value;
				}
				throw ExceptionBuilder.ConvertFailed(type, typeof(SqlSingle));
			}
		}

		public static SqlMoney ConvertToSqlMoney(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlMoney.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			switch (storageType)
			{
			case StorageType.Byte:
				return (long)((ulong)((byte)value));
			case StorageType.Int16:
				return (long)((short)value);
			case StorageType.UInt16:
				return (long)((ulong)((ushort)value));
			case StorageType.Int32:
				return (long)((int)value);
			case StorageType.UInt32:
				return (long)((ulong)((uint)value));
			case StorageType.Int64:
				return (long)value;
			case StorageType.UInt64:
				return (ulong)value;
			case StorageType.Single:
			case StorageType.Double:
				break;
			case StorageType.Decimal:
				return (decimal)value;
			default:
				if (storageType == StorageType.SqlByte)
				{
					return (SqlByte)value;
				}
				switch (storageType)
				{
				case StorageType.SqlInt16:
					return (SqlInt16)value;
				case StorageType.SqlInt32:
					return (SqlInt32)value;
				case StorageType.SqlInt64:
					return (SqlInt64)value;
				case StorageType.SqlMoney:
					return (SqlMoney)value;
				}
				break;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlMoney));
		}

		public static SqlDateTime ConvertToSqlDateTime(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlDateTime.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			if (storageType == StorageType.DateTime)
			{
				return (DateTime)value;
			}
			if (storageType == StorageType.SqlDateTime)
			{
				return (SqlDateTime)value;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlDateTime));
		}

		public static SqlBoolean ConvertToSqlBoolean(object value)
		{
			if (value == DBNull.Value || value == null)
			{
				return SqlBoolean.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			if (storageType == StorageType.Boolean)
			{
				return (bool)value;
			}
			if (storageType == StorageType.SqlBoolean)
			{
				return (SqlBoolean)value;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlBoolean));
		}

		public static SqlGuid ConvertToSqlGuid(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlGuid.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			if (storageType == StorageType.Guid)
			{
				return (Guid)value;
			}
			if (storageType == StorageType.SqlGuid)
			{
				return (SqlGuid)value;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlGuid));
		}

		public static SqlBinary ConvertToSqlBinary(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlBinary.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			if (storageType == StorageType.ByteArray)
			{
				return (byte[])value;
			}
			if (storageType == StorageType.SqlBinary)
			{
				return (SqlBinary)value;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlBinary));
		}

		public static SqlString ConvertToSqlString(object value)
		{
			if (value == DBNull.Value || value == null)
			{
				return SqlString.Null;
			}
			Type type = value.GetType();
			StorageType storageType = DataStorage.GetStorageType(type);
			if (storageType == StorageType.String)
			{
				return (string)value;
			}
			if (storageType == StorageType.SqlString)
			{
				return (SqlString)value;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlString));
		}

		public static SqlChars ConvertToSqlChars(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlChars.Null;
			}
			Type type = value.GetType();
			if (DataStorage.GetStorageType(type) == StorageType.SqlChars)
			{
				return (SqlChars)value;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlChars));
		}

		public static SqlBytes ConvertToSqlBytes(object value)
		{
			if (value == DBNull.Value)
			{
				return SqlBytes.Null;
			}
			Type type = value.GetType();
			if (DataStorage.GetStorageType(type) == StorageType.SqlBytes)
			{
				return (SqlBytes)value;
			}
			throw ExceptionBuilder.ConvertFailed(type, typeof(SqlBytes));
		}

		public static DateTimeOffset ConvertStringToDateTimeOffset(string value, IFormatProvider formatProvider)
		{
			return DateTimeOffset.Parse(value, formatProvider);
		}

		public static object ChangeTypeForDefaultValue(object value, Type type, IFormatProvider formatProvider)
		{
			if (type == typeof(BigInteger))
			{
				if (DBNull.Value == value || value == null)
				{
					return DBNull.Value;
				}
				return BigIntegerStorage.ConvertToBigInteger(value, formatProvider);
			}
			else
			{
				if (value is BigInteger)
				{
					return BigIntegerStorage.ConvertFromBigInteger((BigInteger)value, type, formatProvider);
				}
				return SqlConvert.ChangeType2(value, DataStorage.GetStorageType(type), type, formatProvider);
			}
		}

		public static object ChangeType2(object value, StorageType stype, Type type, IFormatProvider formatProvider)
		{
			switch (stype)
			{
			case StorageType.SqlBinary:
				return SqlConvert.ConvertToSqlBinary(value);
			case StorageType.SqlBoolean:
				return SqlConvert.ConvertToSqlBoolean(value);
			case StorageType.SqlByte:
				return SqlConvert.ConvertToSqlByte(value);
			case StorageType.SqlBytes:
				return SqlConvert.ConvertToSqlBytes(value);
			case StorageType.SqlChars:
				return SqlConvert.ConvertToSqlChars(value);
			case StorageType.SqlDateTime:
				return SqlConvert.ConvertToSqlDateTime(value);
			case StorageType.SqlDecimal:
				return SqlConvert.ConvertToSqlDecimal(value);
			case StorageType.SqlDouble:
				return SqlConvert.ConvertToSqlDouble(value);
			case StorageType.SqlGuid:
				return SqlConvert.ConvertToSqlGuid(value);
			case StorageType.SqlInt16:
				return SqlConvert.ConvertToSqlInt16(value);
			case StorageType.SqlInt32:
				return SqlConvert.ConvertToSqlInt32(value);
			case StorageType.SqlInt64:
				return SqlConvert.ConvertToSqlInt64(value);
			case StorageType.SqlMoney:
				return SqlConvert.ConvertToSqlMoney(value);
			case StorageType.SqlSingle:
				return SqlConvert.ConvertToSqlSingle(value);
			case StorageType.SqlString:
				return SqlConvert.ConvertToSqlString(value);
			default:
			{
				if (DBNull.Value == value || value == null)
				{
					return DBNull.Value;
				}
				Type type2 = value.GetType();
				StorageType storageType = DataStorage.GetStorageType(type2);
				if (storageType - StorageType.SqlBinary <= 14)
				{
					throw ExceptionBuilder.ConvertFailed(type2, type);
				}
				if (StorageType.String == stype)
				{
					switch (storageType)
					{
					case StorageType.Boolean:
						return ((IConvertible)((bool)value)).ToString(formatProvider);
					case StorageType.Char:
						return ((IConvertible)((char)value)).ToString(formatProvider);
					case StorageType.SByte:
						return ((sbyte)value).ToString(formatProvider);
					case StorageType.Byte:
						return ((byte)value).ToString(formatProvider);
					case StorageType.Int16:
						return ((short)value).ToString(formatProvider);
					case StorageType.UInt16:
						return ((ushort)value).ToString(formatProvider);
					case StorageType.Int32:
						return ((int)value).ToString(formatProvider);
					case StorageType.UInt32:
						return ((uint)value).ToString(formatProvider);
					case StorageType.Int64:
						return ((long)value).ToString(formatProvider);
					case StorageType.UInt64:
						return ((ulong)value).ToString(formatProvider);
					case StorageType.Single:
						return ((float)value).ToString(formatProvider);
					case StorageType.Double:
						return ((double)value).ToString(formatProvider);
					case StorageType.Decimal:
						return ((decimal)value).ToString(formatProvider);
					case StorageType.DateTime:
						return ((DateTime)value).ToString(formatProvider);
					case StorageType.TimeSpan:
						return XmlConvert.ToString((TimeSpan)value);
					case StorageType.String:
						return (string)value;
					case StorageType.Guid:
						return XmlConvert.ToString((Guid)value);
					case StorageType.CharArray:
						return new string((char[])value);
					case StorageType.DateTimeOffset:
						return ((DateTimeOffset)value).ToString(formatProvider);
					case StorageType.BigInteger:
						goto IL_0512;
					}
					IConvertible convertible = value as IConvertible;
					if (convertible != null)
					{
						return convertible.ToString(formatProvider);
					}
					IFormattable formattable = value as IFormattable;
					if (formattable != null)
					{
						return formattable.ToString(null, formatProvider);
					}
					return value.ToString();
				}
				else if (StorageType.TimeSpan == stype)
				{
					if (storageType == StorageType.Int32)
					{
						return new TimeSpan((long)((int)value));
					}
					if (storageType == StorageType.Int64)
					{
						return new TimeSpan((long)value);
					}
					if (storageType == StorageType.String)
					{
						return XmlConvert.ToTimeSpan((string)value);
					}
					return (TimeSpan)value;
				}
				else
				{
					if (StorageType.DateTimeOffset == stype)
					{
						return (DateTimeOffset)value;
					}
					if (StorageType.String == storageType)
					{
						switch (stype)
						{
						case StorageType.Boolean:
							if ("1" == (string)value)
							{
								return true;
							}
							if ("0" == (string)value)
							{
								return false;
							}
							break;
						case StorageType.Char:
							return ((IConvertible)((string)value)).ToChar(formatProvider);
						case StorageType.SByte:
							return ((IConvertible)((string)value)).ToSByte(formatProvider);
						case StorageType.Byte:
							return ((IConvertible)((string)value)).ToByte(formatProvider);
						case StorageType.Int16:
							return ((IConvertible)((string)value)).ToInt16(formatProvider);
						case StorageType.UInt16:
							return ((IConvertible)((string)value)).ToUInt16(formatProvider);
						case StorageType.Int32:
							return ((IConvertible)((string)value)).ToInt32(formatProvider);
						case StorageType.UInt32:
							return ((IConvertible)((string)value)).ToUInt32(formatProvider);
						case StorageType.Int64:
							return ((IConvertible)((string)value)).ToInt64(formatProvider);
						case StorageType.UInt64:
							return ((IConvertible)((string)value)).ToUInt64(formatProvider);
						case StorageType.Single:
							return ((IConvertible)((string)value)).ToSingle(formatProvider);
						case StorageType.Double:
							return ((IConvertible)((string)value)).ToDouble(formatProvider);
						case StorageType.Decimal:
							return ((IConvertible)((string)value)).ToDecimal(formatProvider);
						case StorageType.DateTime:
							return ((IConvertible)((string)value)).ToDateTime(formatProvider);
						case StorageType.TimeSpan:
							return XmlConvert.ToTimeSpan((string)value);
						case StorageType.String:
							return (string)value;
						case StorageType.Guid:
							return XmlConvert.ToGuid((string)value);
						case StorageType.Uri:
							return new Uri((string)value);
						}
					}
				}
				IL_0512:
				return Convert.ChangeType(value, type, formatProvider);
			}
			}
		}

		public static object ChangeTypeForXML(object value, Type type)
		{
			StorageType storageType = DataStorage.GetStorageType(type);
			StorageType storageType2 = DataStorage.GetStorageType(value.GetType());
			switch (storageType)
			{
			case StorageType.Boolean:
				if ("1" == (string)value)
				{
					return true;
				}
				if ("0" == (string)value)
				{
					return false;
				}
				return XmlConvert.ToBoolean((string)value);
			case StorageType.Char:
				return XmlConvert.ToChar((string)value);
			case StorageType.SByte:
				return XmlConvert.ToSByte((string)value);
			case StorageType.Byte:
				return XmlConvert.ToByte((string)value);
			case StorageType.Int16:
				return XmlConvert.ToInt16((string)value);
			case StorageType.UInt16:
				return XmlConvert.ToUInt16((string)value);
			case StorageType.Int32:
				return XmlConvert.ToInt32((string)value);
			case StorageType.UInt32:
				return XmlConvert.ToUInt32((string)value);
			case StorageType.Int64:
				return XmlConvert.ToInt64((string)value);
			case StorageType.UInt64:
				return XmlConvert.ToUInt64((string)value);
			case StorageType.Single:
				return XmlConvert.ToSingle((string)value);
			case StorageType.Double:
				return XmlConvert.ToDouble((string)value);
			case StorageType.Decimal:
				return XmlConvert.ToDecimal((string)value);
			case StorageType.DateTime:
				return XmlConvert.ToDateTime((string)value, XmlDateTimeSerializationMode.RoundtripKind);
			case StorageType.TimeSpan:
				if (storageType2 == StorageType.Int32)
				{
					return new TimeSpan((long)((int)value));
				}
				if (storageType2 == StorageType.Int64)
				{
					return new TimeSpan((long)value);
				}
				if (storageType2 == StorageType.String)
				{
					return XmlConvert.ToTimeSpan((string)value);
				}
				return (TimeSpan)value;
			case StorageType.Guid:
				return XmlConvert.ToGuid((string)value);
			case StorageType.DateTimeOffset:
				return XmlConvert.ToDateTimeOffset((string)value);
			case StorageType.Uri:
				return new Uri((string)value);
			case StorageType.SqlBinary:
				return new SqlBinary(Convert.FromBase64String((string)value));
			case StorageType.SqlBoolean:
				return new SqlBoolean(XmlConvert.ToBoolean((string)value));
			case StorageType.SqlByte:
				return new SqlByte(XmlConvert.ToByte((string)value));
			case StorageType.SqlBytes:
				return new SqlBytes(Convert.FromBase64String((string)value));
			case StorageType.SqlChars:
				return new SqlChars(((string)value).ToCharArray());
			case StorageType.SqlDateTime:
				return new SqlDateTime(XmlConvert.ToDateTime((string)value, XmlDateTimeSerializationMode.RoundtripKind));
			case StorageType.SqlDecimal:
				return SqlDecimal.Parse((string)value);
			case StorageType.SqlDouble:
				return new SqlDouble(XmlConvert.ToDouble((string)value));
			case StorageType.SqlGuid:
				return new SqlGuid(XmlConvert.ToGuid((string)value));
			case StorageType.SqlInt16:
				return new SqlInt16(XmlConvert.ToInt16((string)value));
			case StorageType.SqlInt32:
				return new SqlInt32(XmlConvert.ToInt32((string)value));
			case StorageType.SqlInt64:
				return new SqlInt64(XmlConvert.ToInt64((string)value));
			case StorageType.SqlMoney:
				return new SqlMoney(XmlConvert.ToDecimal((string)value));
			case StorageType.SqlSingle:
				return new SqlSingle(XmlConvert.ToSingle((string)value));
			case StorageType.SqlString:
				return new SqlString((string)value);
			}
			if (DBNull.Value == value || value == null)
			{
				return DBNull.Value;
			}
			switch (storageType2)
			{
			case StorageType.Boolean:
				return XmlConvert.ToString((bool)value);
			case StorageType.Char:
				return XmlConvert.ToString((char)value);
			case StorageType.SByte:
				return XmlConvert.ToString((sbyte)value);
			case StorageType.Byte:
				return XmlConvert.ToString((byte)value);
			case StorageType.Int16:
				return XmlConvert.ToString((short)value);
			case StorageType.UInt16:
				return XmlConvert.ToString((ushort)value);
			case StorageType.Int32:
				return XmlConvert.ToString((int)value);
			case StorageType.UInt32:
				return XmlConvert.ToString((uint)value);
			case StorageType.Int64:
				return XmlConvert.ToString((long)value);
			case StorageType.UInt64:
				return XmlConvert.ToString((ulong)value);
			case StorageType.Single:
				return XmlConvert.ToString((float)value);
			case StorageType.Double:
				return XmlConvert.ToString((double)value);
			case StorageType.Decimal:
				return XmlConvert.ToString((decimal)value);
			case StorageType.DateTime:
				return XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.RoundtripKind);
			case StorageType.TimeSpan:
				return XmlConvert.ToString((TimeSpan)value);
			case StorageType.String:
				return (string)value;
			case StorageType.Guid:
				return XmlConvert.ToString((Guid)value);
			case StorageType.CharArray:
				return new string((char[])value);
			case StorageType.DateTimeOffset:
				return XmlConvert.ToString((DateTimeOffset)value);
			case StorageType.SqlBinary:
				return Convert.ToBase64String(((SqlBinary)value).Value);
			case StorageType.SqlBoolean:
				return XmlConvert.ToString(((SqlBoolean)value).Value);
			case StorageType.SqlByte:
				return XmlConvert.ToString(((SqlByte)value).Value);
			case StorageType.SqlBytes:
				return Convert.ToBase64String(((SqlBytes)value).Value);
			case StorageType.SqlChars:
				return new string(((SqlChars)value).Value);
			case StorageType.SqlDateTime:
				return XmlConvert.ToString(((SqlDateTime)value).Value, XmlDateTimeSerializationMode.RoundtripKind);
			case StorageType.SqlDecimal:
				return ((SqlDecimal)value).ToString();
			case StorageType.SqlDouble:
				return XmlConvert.ToString(((SqlDouble)value).Value);
			case StorageType.SqlGuid:
				return XmlConvert.ToString(((SqlGuid)value).Value);
			case StorageType.SqlInt16:
				return XmlConvert.ToString(((SqlInt16)value).Value);
			case StorageType.SqlInt32:
				return XmlConvert.ToString(((SqlInt32)value).Value);
			case StorageType.SqlInt64:
				return XmlConvert.ToString(((SqlInt64)value).Value);
			case StorageType.SqlMoney:
				return XmlConvert.ToString(((SqlMoney)value).Value);
			case StorageType.SqlSingle:
				return XmlConvert.ToString(((SqlSingle)value).Value);
			case StorageType.SqlString:
				return ((SqlString)value).Value;
			}
			IConvertible convertible = value as IConvertible;
			if (convertible != null)
			{
				return convertible.ToString(CultureInfo.InvariantCulture);
			}
			IFormattable formattable = value as IFormattable;
			if (formattable != null)
			{
				return formattable.ToString(null, CultureInfo.InvariantCulture);
			}
			return value.ToString();
		}
	}
}
