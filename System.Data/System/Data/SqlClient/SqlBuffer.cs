using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Data.SqlClient
{
	internal sealed class SqlBuffer
	{
		internal SqlBuffer()
		{
		}

		private SqlBuffer(SqlBuffer value)
		{
			this._isNull = value._isNull;
			this._type = value._type;
			this._value = value._value;
			this._object = value._object;
		}

		internal bool IsEmpty
		{
			get
			{
				return this._type == SqlBuffer.StorageType.Empty;
			}
		}

		internal bool IsNull
		{
			get
			{
				return this._isNull;
			}
		}

		internal SqlBuffer.StorageType VariantInternalStorageType
		{
			get
			{
				return this._type;
			}
		}

		internal bool Boolean
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Boolean == this._type)
				{
					return this._value._boolean;
				}
				return (bool)this.Value;
			}
			set
			{
				this._value._boolean = value;
				this._type = SqlBuffer.StorageType.Boolean;
				this._isNull = false;
			}
		}

		internal byte Byte
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Byte == this._type)
				{
					return this._value._byte;
				}
				return (byte)this.Value;
			}
			set
			{
				this._value._byte = value;
				this._type = SqlBuffer.StorageType.Byte;
				this._isNull = false;
			}
		}

		internal byte[] ByteArray
		{
			get
			{
				this.ThrowIfNull();
				return this.SqlBinary.Value;
			}
		}

		internal DateTime DateTime
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Date == this._type)
				{
					return DateTime.MinValue.AddDays((double)this._value._int32);
				}
				if (SqlBuffer.StorageType.DateTime2 == this._type)
				{
					return new DateTime(SqlBuffer.GetTicksFromDateTime2Info(this._value._dateTime2Info));
				}
				if (SqlBuffer.StorageType.DateTime == this._type)
				{
					return SqlTypeWorkarounds.SqlDateTimeToDateTime(this._value._dateTimeInfo.daypart, this._value._dateTimeInfo.timepart);
				}
				return (DateTime)this.Value;
			}
		}

		internal decimal Decimal
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Decimal == this._type)
				{
					if (this._value._numericInfo.data4 != 0 || this._value._numericInfo.scale > 28)
					{
						throw new OverflowException(SQLResource.ConversionOverflowMessage);
					}
					return new decimal(this._value._numericInfo.data1, this._value._numericInfo.data2, this._value._numericInfo.data3, !this._value._numericInfo.positive, this._value._numericInfo.scale);
				}
				else
				{
					if (SqlBuffer.StorageType.Money == this._type)
					{
						long num = this._value._int64;
						bool flag = false;
						if (num < 0L)
						{
							flag = true;
							num = -num;
						}
						return new decimal((int)(num & (long)((ulong)(-1))), (int)(num >> 32), 0, flag, 4);
					}
					return (decimal)this.Value;
				}
			}
		}

		internal double Double
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Double == this._type)
				{
					return this._value._double;
				}
				return (double)this.Value;
			}
			set
			{
				this._value._double = value;
				this._type = SqlBuffer.StorageType.Double;
				this._isNull = false;
			}
		}

		internal Guid Guid
		{
			get
			{
				this.ThrowIfNull();
				return this.SqlGuid.Value;
			}
		}

		internal short Int16
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Int16 == this._type)
				{
					return this._value._int16;
				}
				return (short)this.Value;
			}
			set
			{
				this._value._int16 = value;
				this._type = SqlBuffer.StorageType.Int16;
				this._isNull = false;
			}
		}

		internal int Int32
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Int32 == this._type)
				{
					return this._value._int32;
				}
				return (int)this.Value;
			}
			set
			{
				this._value._int32 = value;
				this._type = SqlBuffer.StorageType.Int32;
				this._isNull = false;
			}
		}

		internal long Int64
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Int64 == this._type)
				{
					return this._value._int64;
				}
				return (long)this.Value;
			}
			set
			{
				this._value._int64 = value;
				this._type = SqlBuffer.StorageType.Int64;
				this._isNull = false;
			}
		}

		internal float Single
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Single == this._type)
				{
					return this._value._single;
				}
				return (float)this.Value;
			}
			set
			{
				this._value._single = value;
				this._type = SqlBuffer.StorageType.Single;
				this._isNull = false;
			}
		}

		internal string String
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.String == this._type)
				{
					return (string)this._object;
				}
				if (SqlBuffer.StorageType.SqlCachedBuffer == this._type)
				{
					return ((SqlCachedBuffer)this._object).ToString();
				}
				return (string)this.Value;
			}
		}

		internal string KatmaiDateTimeString
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Date == this._type)
				{
					return this.DateTime.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
				}
				if (SqlBuffer.StorageType.Time == this._type)
				{
					byte scale = this._value._timeInfo.scale;
					return new DateTime(this._value._timeInfo.ticks).ToString(SqlBuffer.s_katmaiTimeFormatByScale[(int)scale], DateTimeFormatInfo.InvariantInfo);
				}
				if (SqlBuffer.StorageType.DateTime2 == this._type)
				{
					byte scale2 = this._value._dateTime2Info.timeInfo.scale;
					return this.DateTime.ToString(SqlBuffer.s_katmaiDateTime2FormatByScale[(int)scale2], DateTimeFormatInfo.InvariantInfo);
				}
				if (SqlBuffer.StorageType.DateTimeOffset == this._type)
				{
					DateTimeOffset dateTimeOffset = this.DateTimeOffset;
					byte scale3 = this._value._dateTimeOffsetInfo.dateTime2Info.timeInfo.scale;
					return dateTimeOffset.ToString(SqlBuffer.s_katmaiDateTimeOffsetFormatByScale[(int)scale3], DateTimeFormatInfo.InvariantInfo);
				}
				return (string)this.Value;
			}
		}

		internal SqlString KatmaiDateTimeSqlString
		{
			get
			{
				if (SqlBuffer.StorageType.Date != this._type && SqlBuffer.StorageType.Time != this._type && SqlBuffer.StorageType.DateTime2 != this._type && SqlBuffer.StorageType.DateTimeOffset != this._type)
				{
					return (SqlString)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlString.Null;
				}
				return new SqlString(this.KatmaiDateTimeString);
			}
		}

		internal TimeSpan Time
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.Time == this._type)
				{
					return new TimeSpan(this._value._timeInfo.ticks);
				}
				return (TimeSpan)this.Value;
			}
		}

		internal DateTimeOffset DateTimeOffset
		{
			get
			{
				this.ThrowIfNull();
				if (SqlBuffer.StorageType.DateTimeOffset == this._type)
				{
					TimeSpan timeSpan = new TimeSpan(0, (int)this._value._dateTimeOffsetInfo.offset, 0);
					return new DateTimeOffset(SqlBuffer.GetTicksFromDateTime2Info(this._value._dateTimeOffsetInfo.dateTime2Info) + timeSpan.Ticks, timeSpan);
				}
				return (DateTimeOffset)this.Value;
			}
		}

		private static long GetTicksFromDateTime2Info(SqlBuffer.DateTime2Info dateTime2Info)
		{
			return (long)dateTime2Info.date * 864000000000L + dateTime2Info.timeInfo.ticks;
		}

		internal SqlBinary SqlBinary
		{
			get
			{
				if (SqlBuffer.StorageType.SqlBinary == this._type)
				{
					return (SqlBinary)this._object;
				}
				return (SqlBinary)this.SqlValue;
			}
			set
			{
				this._object = value;
				this._type = SqlBuffer.StorageType.SqlBinary;
				this._isNull = value.IsNull;
			}
		}

		internal SqlBoolean SqlBoolean
		{
			get
			{
				if (SqlBuffer.StorageType.Boolean != this._type)
				{
					return (SqlBoolean)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlBoolean.Null;
				}
				return new SqlBoolean(this._value._boolean);
			}
		}

		internal SqlByte SqlByte
		{
			get
			{
				if (SqlBuffer.StorageType.Byte != this._type)
				{
					return (SqlByte)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlByte.Null;
				}
				return new SqlByte(this._value._byte);
			}
		}

		internal SqlCachedBuffer SqlCachedBuffer
		{
			get
			{
				if (SqlBuffer.StorageType.SqlCachedBuffer != this._type)
				{
					return (SqlCachedBuffer)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlCachedBuffer.Null;
				}
				return (SqlCachedBuffer)this._object;
			}
			set
			{
				this._object = value;
				this._type = SqlBuffer.StorageType.SqlCachedBuffer;
				this._isNull = value.IsNull;
			}
		}

		internal SqlXml SqlXml
		{
			get
			{
				if (SqlBuffer.StorageType.SqlXml != this._type)
				{
					return (SqlXml)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlXml.Null;
				}
				return (SqlXml)this._object;
			}
			set
			{
				this._object = value;
				this._type = SqlBuffer.StorageType.SqlXml;
				this._isNull = value.IsNull;
			}
		}

		internal SqlDateTime SqlDateTime
		{
			get
			{
				if (SqlBuffer.StorageType.DateTime != this._type)
				{
					return (SqlDateTime)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlDateTime.Null;
				}
				return new SqlDateTime(this._value._dateTimeInfo.daypart, this._value._dateTimeInfo.timepart);
			}
		}

		internal SqlDecimal SqlDecimal
		{
			get
			{
				if (SqlBuffer.StorageType.Decimal != this._type)
				{
					return (SqlDecimal)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlDecimal.Null;
				}
				return new SqlDecimal(this._value._numericInfo.precision, this._value._numericInfo.scale, this._value._numericInfo.positive, this._value._numericInfo.data1, this._value._numericInfo.data2, this._value._numericInfo.data3, this._value._numericInfo.data4);
			}
		}

		internal SqlDouble SqlDouble
		{
			get
			{
				if (SqlBuffer.StorageType.Double != this._type)
				{
					return (SqlDouble)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlDouble.Null;
				}
				return new SqlDouble(this._value._double);
			}
		}

		internal SqlGuid SqlGuid
		{
			get
			{
				if (SqlBuffer.StorageType.SqlGuid == this._type)
				{
					return (SqlGuid)this._object;
				}
				return (SqlGuid)this.SqlValue;
			}
			set
			{
				this._object = value;
				this._type = SqlBuffer.StorageType.SqlGuid;
				this._isNull = value.IsNull;
			}
		}

		internal SqlInt16 SqlInt16
		{
			get
			{
				if (SqlBuffer.StorageType.Int16 != this._type)
				{
					return (SqlInt16)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlInt16.Null;
				}
				return new SqlInt16(this._value._int16);
			}
		}

		internal SqlInt32 SqlInt32
		{
			get
			{
				if (SqlBuffer.StorageType.Int32 != this._type)
				{
					return (SqlInt32)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlInt32.Null;
				}
				return new SqlInt32(this._value._int32);
			}
		}

		internal SqlInt64 SqlInt64
		{
			get
			{
				if (SqlBuffer.StorageType.Int64 != this._type)
				{
					return (SqlInt64)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlInt64.Null;
				}
				return new SqlInt64(this._value._int64);
			}
		}

		internal SqlMoney SqlMoney
		{
			get
			{
				if (SqlBuffer.StorageType.Money != this._type)
				{
					return (SqlMoney)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlMoney.Null;
				}
				return SqlTypeWorkarounds.SqlMoneyCtor(this._value._int64, 1);
			}
		}

		internal SqlSingle SqlSingle
		{
			get
			{
				if (SqlBuffer.StorageType.Single != this._type)
				{
					return (SqlSingle)this.SqlValue;
				}
				if (this.IsNull)
				{
					return SqlSingle.Null;
				}
				return new SqlSingle(this._value._single);
			}
		}

		internal SqlString SqlString
		{
			get
			{
				if (SqlBuffer.StorageType.String == this._type)
				{
					if (this.IsNull)
					{
						return SqlString.Null;
					}
					return new SqlString((string)this._object);
				}
				else
				{
					if (SqlBuffer.StorageType.SqlCachedBuffer != this._type)
					{
						return (SqlString)this.SqlValue;
					}
					SqlCachedBuffer sqlCachedBuffer = (SqlCachedBuffer)this._object;
					if (sqlCachedBuffer.IsNull)
					{
						return SqlString.Null;
					}
					return sqlCachedBuffer.ToSqlString();
				}
			}
		}

		internal object SqlValue
		{
			get
			{
				switch (this._type)
				{
				case SqlBuffer.StorageType.Empty:
					return DBNull.Value;
				case SqlBuffer.StorageType.Boolean:
					return this.SqlBoolean;
				case SqlBuffer.StorageType.Byte:
					return this.SqlByte;
				case SqlBuffer.StorageType.DateTime:
					return this.SqlDateTime;
				case SqlBuffer.StorageType.Decimal:
					return this.SqlDecimal;
				case SqlBuffer.StorageType.Double:
					return this.SqlDouble;
				case SqlBuffer.StorageType.Int16:
					return this.SqlInt16;
				case SqlBuffer.StorageType.Int32:
					return this.SqlInt32;
				case SqlBuffer.StorageType.Int64:
					return this.SqlInt64;
				case SqlBuffer.StorageType.Money:
					return this.SqlMoney;
				case SqlBuffer.StorageType.Single:
					return this.SqlSingle;
				case SqlBuffer.StorageType.String:
					return this.SqlString;
				case SqlBuffer.StorageType.SqlBinary:
				case SqlBuffer.StorageType.SqlGuid:
					return this._object;
				case SqlBuffer.StorageType.SqlCachedBuffer:
				{
					SqlCachedBuffer sqlCachedBuffer = (SqlCachedBuffer)this._object;
					if (sqlCachedBuffer.IsNull)
					{
						return SqlXml.Null;
					}
					return sqlCachedBuffer.ToSqlXml();
				}
				case SqlBuffer.StorageType.SqlXml:
					if (this._isNull)
					{
						return SqlXml.Null;
					}
					return (SqlXml)this._object;
				case SqlBuffer.StorageType.Date:
				case SqlBuffer.StorageType.DateTime2:
					if (this._isNull)
					{
						return DBNull.Value;
					}
					return this.DateTime;
				case SqlBuffer.StorageType.DateTimeOffset:
					if (this._isNull)
					{
						return DBNull.Value;
					}
					return this.DateTimeOffset;
				case SqlBuffer.StorageType.Time:
					if (this._isNull)
					{
						return DBNull.Value;
					}
					return this.Time;
				default:
					return null;
				}
			}
		}

		internal object Value
		{
			get
			{
				if (this.IsNull)
				{
					return DBNull.Value;
				}
				switch (this._type)
				{
				case SqlBuffer.StorageType.Empty:
					return DBNull.Value;
				case SqlBuffer.StorageType.Boolean:
					return this.Boolean;
				case SqlBuffer.StorageType.Byte:
					return this.Byte;
				case SqlBuffer.StorageType.DateTime:
					return this.DateTime;
				case SqlBuffer.StorageType.Decimal:
					return this.Decimal;
				case SqlBuffer.StorageType.Double:
					return this.Double;
				case SqlBuffer.StorageType.Int16:
					return this.Int16;
				case SqlBuffer.StorageType.Int32:
					return this.Int32;
				case SqlBuffer.StorageType.Int64:
					return this.Int64;
				case SqlBuffer.StorageType.Money:
					return this.Decimal;
				case SqlBuffer.StorageType.Single:
					return this.Single;
				case SqlBuffer.StorageType.String:
					return this.String;
				case SqlBuffer.StorageType.SqlBinary:
					return this.ByteArray;
				case SqlBuffer.StorageType.SqlCachedBuffer:
					return ((SqlCachedBuffer)this._object).ToString();
				case SqlBuffer.StorageType.SqlGuid:
					return this.Guid;
				case SqlBuffer.StorageType.SqlXml:
					return ((SqlXml)this._object).Value;
				case SqlBuffer.StorageType.Date:
					return this.DateTime;
				case SqlBuffer.StorageType.DateTime2:
					return this.DateTime;
				case SqlBuffer.StorageType.DateTimeOffset:
					return this.DateTimeOffset;
				case SqlBuffer.StorageType.Time:
					return this.Time;
				default:
					return null;
				}
			}
		}

		internal Type GetTypeFromStorageType(bool isSqlType)
		{
			if (isSqlType)
			{
				switch (this._type)
				{
				case SqlBuffer.StorageType.Empty:
					return null;
				case SqlBuffer.StorageType.Boolean:
					return typeof(SqlBoolean);
				case SqlBuffer.StorageType.Byte:
					return typeof(SqlByte);
				case SqlBuffer.StorageType.DateTime:
					return typeof(SqlDateTime);
				case SqlBuffer.StorageType.Decimal:
					return typeof(SqlDecimal);
				case SqlBuffer.StorageType.Double:
					return typeof(SqlDouble);
				case SqlBuffer.StorageType.Int16:
					return typeof(SqlInt16);
				case SqlBuffer.StorageType.Int32:
					return typeof(SqlInt32);
				case SqlBuffer.StorageType.Int64:
					return typeof(SqlInt64);
				case SqlBuffer.StorageType.Money:
					return typeof(SqlMoney);
				case SqlBuffer.StorageType.Single:
					return typeof(SqlSingle);
				case SqlBuffer.StorageType.String:
					return typeof(SqlString);
				case SqlBuffer.StorageType.SqlBinary:
					return typeof(object);
				case SqlBuffer.StorageType.SqlCachedBuffer:
					return typeof(SqlString);
				case SqlBuffer.StorageType.SqlGuid:
					return typeof(object);
				case SqlBuffer.StorageType.SqlXml:
					return typeof(SqlXml);
				}
			}
			else
			{
				switch (this._type)
				{
				case SqlBuffer.StorageType.Empty:
					return null;
				case SqlBuffer.StorageType.Boolean:
					return typeof(bool);
				case SqlBuffer.StorageType.Byte:
					return typeof(byte);
				case SqlBuffer.StorageType.DateTime:
					return typeof(DateTime);
				case SqlBuffer.StorageType.Decimal:
					return typeof(decimal);
				case SqlBuffer.StorageType.Double:
					return typeof(double);
				case SqlBuffer.StorageType.Int16:
					return typeof(short);
				case SqlBuffer.StorageType.Int32:
					return typeof(int);
				case SqlBuffer.StorageType.Int64:
					return typeof(long);
				case SqlBuffer.StorageType.Money:
					return typeof(decimal);
				case SqlBuffer.StorageType.Single:
					return typeof(float);
				case SqlBuffer.StorageType.String:
					return typeof(string);
				case SqlBuffer.StorageType.SqlBinary:
					return typeof(byte[]);
				case SqlBuffer.StorageType.SqlCachedBuffer:
					return typeof(string);
				case SqlBuffer.StorageType.SqlGuid:
					return typeof(Guid);
				case SqlBuffer.StorageType.SqlXml:
					return typeof(string);
				}
			}
			return null;
		}

		internal static SqlBuffer[] CreateBufferArray(int length)
		{
			SqlBuffer[] array = new SqlBuffer[length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SqlBuffer();
			}
			return array;
		}

		internal static SqlBuffer[] CloneBufferArray(SqlBuffer[] values)
		{
			SqlBuffer[] array = new SqlBuffer[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = new SqlBuffer(values[i]);
			}
			return array;
		}

		internal static void Clear(SqlBuffer[] values)
		{
			if (values != null)
			{
				for (int i = 0; i < values.Length; i++)
				{
					values[i].Clear();
				}
			}
		}

		internal void Clear()
		{
			this._isNull = false;
			this._type = SqlBuffer.StorageType.Empty;
			this._object = null;
		}

		internal void SetToDateTime(int daypart, int timepart)
		{
			this._value._dateTimeInfo.daypart = daypart;
			this._value._dateTimeInfo.timepart = timepart;
			this._type = SqlBuffer.StorageType.DateTime;
			this._isNull = false;
		}

		internal void SetToDecimal(byte precision, byte scale, bool positive, int[] bits)
		{
			this._value._numericInfo.precision = precision;
			this._value._numericInfo.scale = scale;
			this._value._numericInfo.positive = positive;
			this._value._numericInfo.data1 = bits[0];
			this._value._numericInfo.data2 = bits[1];
			this._value._numericInfo.data3 = bits[2];
			this._value._numericInfo.data4 = bits[3];
			this._type = SqlBuffer.StorageType.Decimal;
			this._isNull = false;
		}

		internal void SetToMoney(long value)
		{
			this._value._int64 = value;
			this._type = SqlBuffer.StorageType.Money;
			this._isNull = false;
		}

		internal void SetToNullOfType(SqlBuffer.StorageType storageType)
		{
			this._type = storageType;
			this._isNull = true;
			this._object = null;
		}

		internal void SetToString(string value)
		{
			this._object = value;
			this._type = SqlBuffer.StorageType.String;
			this._isNull = false;
		}

		internal void SetToDate(byte[] bytes)
		{
			this._type = SqlBuffer.StorageType.Date;
			this._value._int32 = SqlBuffer.GetDateFromByteArray(bytes, 0);
			this._isNull = false;
		}

		internal void SetToDate(DateTime date)
		{
			this._type = SqlBuffer.StorageType.Date;
			this._value._int32 = date.Subtract(DateTime.MinValue).Days;
			this._isNull = false;
		}

		internal void SetToTime(byte[] bytes, int length, byte scale)
		{
			this._type = SqlBuffer.StorageType.Time;
			SqlBuffer.FillInTimeInfo(ref this._value._timeInfo, bytes, length, scale);
			this._isNull = false;
		}

		internal void SetToTime(TimeSpan timeSpan, byte scale)
		{
			this._type = SqlBuffer.StorageType.Time;
			this._value._timeInfo.ticks = timeSpan.Ticks;
			this._value._timeInfo.scale = scale;
			this._isNull = false;
		}

		internal void SetToDateTime2(byte[] bytes, int length, byte scale)
		{
			this._type = SqlBuffer.StorageType.DateTime2;
			SqlBuffer.FillInTimeInfo(ref this._value._dateTime2Info.timeInfo, bytes, length - 3, scale);
			this._value._dateTime2Info.date = SqlBuffer.GetDateFromByteArray(bytes, length - 3);
			this._isNull = false;
		}

		internal void SetToDateTime2(DateTime dateTime, byte scale)
		{
			this._type = SqlBuffer.StorageType.DateTime2;
			this._value._dateTime2Info.timeInfo.ticks = dateTime.TimeOfDay.Ticks;
			this._value._dateTime2Info.timeInfo.scale = scale;
			this._value._dateTime2Info.date = dateTime.Subtract(DateTime.MinValue).Days;
			this._isNull = false;
		}

		internal void SetToDateTimeOffset(byte[] bytes, int length, byte scale)
		{
			this._type = SqlBuffer.StorageType.DateTimeOffset;
			SqlBuffer.FillInTimeInfo(ref this._value._dateTimeOffsetInfo.dateTime2Info.timeInfo, bytes, length - 5, scale);
			this._value._dateTimeOffsetInfo.dateTime2Info.date = SqlBuffer.GetDateFromByteArray(bytes, length - 5);
			this._value._dateTimeOffsetInfo.offset = (short)((int)bytes[length - 2] + ((int)bytes[length - 1] << 8));
			this._isNull = false;
		}

		internal void SetToDateTimeOffset(DateTimeOffset dateTimeOffset, byte scale)
		{
			this._type = SqlBuffer.StorageType.DateTimeOffset;
			DateTime utcDateTime = dateTimeOffset.UtcDateTime;
			this._value._dateTimeOffsetInfo.dateTime2Info.timeInfo.ticks = utcDateTime.TimeOfDay.Ticks;
			this._value._dateTimeOffsetInfo.dateTime2Info.timeInfo.scale = scale;
			this._value._dateTimeOffsetInfo.dateTime2Info.date = utcDateTime.Subtract(DateTime.MinValue).Days;
			this._value._dateTimeOffsetInfo.offset = (short)dateTimeOffset.Offset.TotalMinutes;
			this._isNull = false;
		}

		private static void FillInTimeInfo(ref SqlBuffer.TimeInfo timeInfo, byte[] timeBytes, int length, byte scale)
		{
			long num = (long)((ulong)timeBytes[0] + ((ulong)timeBytes[1] << 8) + ((ulong)timeBytes[2] << 16));
			if (length > 3)
			{
				num += (long)((long)((ulong)timeBytes[3]) << 24);
			}
			if (length > 4)
			{
				num += (long)((long)((ulong)timeBytes[4]) << 32);
			}
			timeInfo.ticks = num * TdsEnums.TICKS_FROM_SCALE[(int)scale];
			timeInfo.scale = scale;
		}

		private static int GetDateFromByteArray(byte[] buf, int offset)
		{
			return (int)buf[offset] + ((int)buf[offset + 1] << 8) + ((int)buf[offset + 2] << 16);
		}

		private void ThrowIfNull()
		{
			if (this.IsNull)
			{
				throw new SqlNullValueException();
			}
		}

		private bool _isNull;

		private SqlBuffer.StorageType _type;

		private SqlBuffer.Storage _value;

		private object _object;

		private static string[] s_katmaiDateTimeOffsetFormatByScale = new string[] { "yyyy-MM-dd HH:mm:ss zzz", "yyyy-MM-dd HH:mm:ss.f zzz", "yyyy-MM-dd HH:mm:ss.ff zzz", "yyyy-MM-dd HH:mm:ss.fff zzz", "yyyy-MM-dd HH:mm:ss.ffff zzz", "yyyy-MM-dd HH:mm:ss.fffff zzz", "yyyy-MM-dd HH:mm:ss.ffffff zzz", "yyyy-MM-dd HH:mm:ss.fffffff zzz" };

		private static string[] s_katmaiDateTime2FormatByScale = new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss.f", "yyyy-MM-dd HH:mm:ss.ff", "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss.ffff", "yyyy-MM-dd HH:mm:ss.fffff", "yyyy-MM-dd HH:mm:ss.ffffff", "yyyy-MM-dd HH:mm:ss.fffffff" };

		private static string[] s_katmaiTimeFormatByScale = new string[] { "HH:mm:ss", "HH:mm:ss.f", "HH:mm:ss.ff", "HH:mm:ss.fff", "HH:mm:ss.ffff", "HH:mm:ss.fffff", "HH:mm:ss.ffffff", "HH:mm:ss.fffffff" };

		internal enum StorageType
		{
			Empty,
			Boolean,
			Byte,
			DateTime,
			Decimal,
			Double,
			Int16,
			Int32,
			Int64,
			Money,
			Single,
			String,
			SqlBinary,
			SqlCachedBuffer,
			SqlGuid,
			SqlXml,
			Date,
			DateTime2,
			DateTimeOffset,
			Time
		}

		internal struct DateTimeInfo
		{
			internal int daypart;

			internal int timepart;
		}

		internal struct NumericInfo
		{
			internal int data1;

			internal int data2;

			internal int data3;

			internal int data4;

			internal byte precision;

			internal byte scale;

			internal bool positive;
		}

		internal struct TimeInfo
		{
			internal long ticks;

			internal byte scale;
		}

		internal struct DateTime2Info
		{
			internal int date;

			internal SqlBuffer.TimeInfo timeInfo;
		}

		internal struct DateTimeOffsetInfo
		{
			internal SqlBuffer.DateTime2Info dateTime2Info;

			internal short offset;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct Storage
		{
			[FieldOffset(0)]
			internal bool _boolean;

			[FieldOffset(0)]
			internal byte _byte;

			[FieldOffset(0)]
			internal SqlBuffer.DateTimeInfo _dateTimeInfo;

			[FieldOffset(0)]
			internal double _double;

			[FieldOffset(0)]
			internal SqlBuffer.NumericInfo _numericInfo;

			[FieldOffset(0)]
			internal short _int16;

			[FieldOffset(0)]
			internal int _int32;

			[FieldOffset(0)]
			internal long _int64;

			[FieldOffset(0)]
			internal float _single;

			[FieldOffset(0)]
			internal SqlBuffer.TimeInfo _timeInfo;

			[FieldOffset(0)]
			internal SqlBuffer.DateTime2Info _dateTime2Info;

			[FieldOffset(0)]
			internal SqlBuffer.DateTimeOffsetInfo _dateTimeOffsetInfo;
		}
	}
}
