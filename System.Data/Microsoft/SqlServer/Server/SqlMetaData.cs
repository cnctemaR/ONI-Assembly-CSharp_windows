using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using Unity;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlMetaData
	{
		public SqlMetaData(string name, SqlDbType dbType)
		{
			this.Construct(name, dbType, false, false, SortOrder.Unspecified, -1);
		}

		public SqlMetaData(string name, SqlDbType dbType, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.Construct(name, dbType, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
		}

		public SqlMetaData(string name, SqlDbType dbType, long maxLength)
		{
			this.Construct(name, dbType, maxLength, false, false, SortOrder.Unspecified, -1);
		}

		public SqlMetaData(string name, SqlDbType dbType, long maxLength, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.Construct(name, dbType, maxLength, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
		}

		public SqlMetaData(string name, SqlDbType dbType, byte precision, byte scale)
		{
			this.Construct(name, dbType, precision, scale, false, false, SortOrder.Unspecified, -1);
		}

		public SqlMetaData(string name, SqlDbType dbType, byte precision, byte scale, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.Construct(name, dbType, precision, scale, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
		}

		public SqlMetaData(string name, SqlDbType dbType, long maxLength, long locale, SqlCompareOptions compareOptions)
		{
			this.Construct(name, dbType, maxLength, locale, compareOptions, false, false, SortOrder.Unspecified, -1);
		}

		public SqlMetaData(string name, SqlDbType dbType, long maxLength, long locale, SqlCompareOptions compareOptions, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.Construct(name, dbType, maxLength, locale, compareOptions, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
		}

		public SqlMetaData(string name, SqlDbType dbType, string database, string owningSchema, string objectName, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.Construct(name, dbType, database, owningSchema, objectName, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
		}

		public SqlMetaData(string name, SqlDbType dbType, long maxLength, byte precision, byte scale, long locale, SqlCompareOptions compareOptions, Type userDefinedType)
			: this(name, dbType, maxLength, precision, scale, locale, compareOptions, userDefinedType, false, false, SortOrder.Unspecified, -1)
		{
		}

		public SqlMetaData(string name, SqlDbType dbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, Type userDefinedType, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			switch (dbType)
			{
			case SqlDbType.BigInt:
			case SqlDbType.Bit:
			case SqlDbType.DateTime:
			case SqlDbType.Float:
			case SqlDbType.Image:
			case SqlDbType.Int:
			case SqlDbType.Money:
			case SqlDbType.Real:
			case SqlDbType.UniqueIdentifier:
			case SqlDbType.SmallDateTime:
			case SqlDbType.SmallInt:
			case SqlDbType.SmallMoney:
			case SqlDbType.Timestamp:
			case SqlDbType.TinyInt:
			case SqlDbType.Xml:
			case SqlDbType.Date:
				this.Construct(name, dbType, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
				return;
			case SqlDbType.Binary:
			case SqlDbType.VarBinary:
				this.Construct(name, dbType, maxLength, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
				return;
			case SqlDbType.Char:
			case SqlDbType.NChar:
			case SqlDbType.NVarChar:
			case SqlDbType.VarChar:
				this.Construct(name, dbType, maxLength, localeId, compareOptions, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
				return;
			case SqlDbType.Decimal:
			case SqlDbType.Time:
			case SqlDbType.DateTime2:
			case SqlDbType.DateTimeOffset:
				this.Construct(name, dbType, precision, scale, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
				return;
			case SqlDbType.NText:
			case SqlDbType.Text:
				this.Construct(name, dbType, SqlMetaData.Max, localeId, compareOptions, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
				return;
			case SqlDbType.Variant:
				this.Construct(name, dbType, useServerDefault, isUniqueKey, columnSortOrder, sortOrdinal);
				return;
			case SqlDbType.Udt:
				throw ADP.DbTypeNotSupported(SqlDbType.Udt.ToString());
			}
			SQL.InvalidSqlDbTypeForConstructor(dbType);
		}

		public SqlMetaData(string name, SqlDbType dbType, string database, string owningSchema, string objectName)
		{
			this.Construct(name, dbType, database, owningSchema, objectName, false, false, SortOrder.Unspecified, -1);
		}

		internal SqlMetaData(string name, SqlDbType sqlDBType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, string xmlSchemaCollectionDatabase, string xmlSchemaCollectionOwningSchema, string xmlSchemaCollectionName, bool partialLength)
		{
			this.AssertNameIsValid(name);
			this._strName = name;
			this._sqlDbType = sqlDBType;
			this._lMaxLength = maxLength;
			this._bPrecision = precision;
			this._bScale = scale;
			this._lLocale = localeId;
			this._eCompareOptions = compareOptions;
			this._xmlSchemaCollectionDatabase = xmlSchemaCollectionDatabase;
			this._xmlSchemaCollectionOwningSchema = xmlSchemaCollectionOwningSchema;
			this._xmlSchemaCollectionName = xmlSchemaCollectionName;
			this._bPartialLength = partialLength;
			this.ThrowIfUdt(sqlDBType);
		}

		private SqlMetaData(string name, SqlDbType sqlDbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, bool partialLength)
		{
			this.AssertNameIsValid(name);
			this._strName = name;
			this._sqlDbType = sqlDbType;
			this._lMaxLength = maxLength;
			this._bPrecision = precision;
			this._bScale = scale;
			this._lLocale = localeId;
			this._eCompareOptions = compareOptions;
			this._bPartialLength = partialLength;
			this.ThrowIfUdt(sqlDbType);
		}

		public SqlCompareOptions CompareOptions
		{
			get
			{
				return this._eCompareOptions;
			}
		}

		public bool IsUniqueKey
		{
			get
			{
				return this._isUniqueKey;
			}
		}

		public long LocaleId
		{
			get
			{
				return this._lLocale;
			}
		}

		public static long Max
		{
			get
			{
				return -1L;
			}
		}

		public long MaxLength
		{
			get
			{
				return this._lMaxLength;
			}
		}

		public string Name
		{
			get
			{
				return this._strName;
			}
		}

		public byte Precision
		{
			get
			{
				return this._bPrecision;
			}
		}

		public byte Scale
		{
			get
			{
				return this._bScale;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				return this._columnSortOrder;
			}
		}

		public int SortOrdinal
		{
			get
			{
				return this._sortOrdinal;
			}
		}

		public SqlDbType SqlDbType
		{
			get
			{
				return this._sqlDbType;
			}
		}

		public string TypeName
		{
			get
			{
				return SqlMetaData.sxm_rgDefaults[(int)this.SqlDbType].Name;
			}
		}

		public bool UseServerDefault
		{
			get
			{
				return this._useServerDefault;
			}
		}

		public string XmlSchemaCollectionDatabase
		{
			get
			{
				return this._xmlSchemaCollectionDatabase;
			}
		}

		public string XmlSchemaCollectionName
		{
			get
			{
				return this._xmlSchemaCollectionName;
			}
		}

		public string XmlSchemaCollectionOwningSchema
		{
			get
			{
				return this._xmlSchemaCollectionOwningSchema;
			}
		}

		internal bool IsPartialLength
		{
			get
			{
				return this._bPartialLength;
			}
		}

		private void Construct(string name, SqlDbType dbType, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.AssertNameIsValid(name);
			this.ValidateSortOrder(columnSortOrder, sortOrdinal);
			if (dbType != SqlDbType.BigInt && SqlDbType.Bit != dbType && SqlDbType.DateTime != dbType && SqlDbType.Date != dbType && SqlDbType.DateTime2 != dbType && SqlDbType.DateTimeOffset != dbType && SqlDbType.Decimal != dbType && SqlDbType.Float != dbType && SqlDbType.Image != dbType && SqlDbType.Int != dbType && SqlDbType.Money != dbType && SqlDbType.NText != dbType && SqlDbType.Real != dbType && SqlDbType.SmallDateTime != dbType && SqlDbType.SmallInt != dbType && SqlDbType.SmallMoney != dbType && SqlDbType.Text != dbType && SqlDbType.Time != dbType && SqlDbType.Timestamp != dbType && SqlDbType.TinyInt != dbType && SqlDbType.UniqueIdentifier != dbType && SqlDbType.Variant != dbType && SqlDbType.Xml != dbType)
			{
				throw SQL.InvalidSqlDbTypeForConstructor(dbType);
			}
			this.ThrowIfUdt(dbType);
			this.SetDefaultsForType(dbType);
			if (SqlDbType.NText == dbType || SqlDbType.Text == dbType)
			{
				this._lLocale = (long)CultureInfo.CurrentCulture.LCID;
			}
			this._strName = name;
			this._useServerDefault = useServerDefault;
			this._isUniqueKey = isUniqueKey;
			this._columnSortOrder = columnSortOrder;
			this._sortOrdinal = sortOrdinal;
		}

		private void Construct(string name, SqlDbType dbType, long maxLength, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.AssertNameIsValid(name);
			this.ValidateSortOrder(columnSortOrder, sortOrdinal);
			long num = 0L;
			if (SqlDbType.Char == dbType)
			{
				if (maxLength > 8000L || maxLength < 0L)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
				num = (long)CultureInfo.CurrentCulture.LCID;
			}
			else if (SqlDbType.VarChar == dbType)
			{
				if ((maxLength > 8000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
				num = (long)CultureInfo.CurrentCulture.LCID;
			}
			else if (SqlDbType.NChar == dbType)
			{
				if (maxLength > 4000L || maxLength < 0L)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
				num = (long)CultureInfo.CurrentCulture.LCID;
			}
			else if (SqlDbType.NVarChar == dbType)
			{
				if ((maxLength > 4000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
				num = (long)CultureInfo.CurrentCulture.LCID;
			}
			else if (SqlDbType.NText == dbType || SqlDbType.Text == dbType)
			{
				if (SqlMetaData.Max != maxLength)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
				num = (long)CultureInfo.CurrentCulture.LCID;
			}
			else if (SqlDbType.Binary == dbType)
			{
				if (maxLength > 8000L || maxLength < 0L)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
			}
			else if (SqlDbType.VarBinary == dbType)
			{
				if ((maxLength > 8000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
			}
			else
			{
				if (SqlDbType.Image != dbType)
				{
					throw SQL.InvalidSqlDbTypeForConstructor(dbType);
				}
				if (SqlMetaData.Max != maxLength)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
			}
			this.SetDefaultsForType(dbType);
			this._strName = name;
			this._lMaxLength = maxLength;
			this._lLocale = num;
			this._useServerDefault = useServerDefault;
			this._isUniqueKey = isUniqueKey;
			this._columnSortOrder = columnSortOrder;
			this._sortOrdinal = sortOrdinal;
		}

		private void Construct(string name, SqlDbType dbType, long maxLength, long locale, SqlCompareOptions compareOptions, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.AssertNameIsValid(name);
			this.ValidateSortOrder(columnSortOrder, sortOrdinal);
			if (SqlDbType.Char == dbType)
			{
				if (maxLength > 8000L || maxLength < 0L)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
			}
			else if (SqlDbType.VarChar == dbType)
			{
				if ((maxLength > 8000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
			}
			else if (SqlDbType.NChar == dbType)
			{
				if (maxLength > 4000L || maxLength < 0L)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
			}
			else if (SqlDbType.NVarChar == dbType)
			{
				if ((maxLength > 4000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
			}
			else
			{
				if (SqlDbType.NText != dbType && SqlDbType.Text != dbType)
				{
					throw SQL.InvalidSqlDbTypeForConstructor(dbType);
				}
				if (SqlMetaData.Max != maxLength)
				{
					throw ADP.Argument(SR.GetString("Specified length '{0}' is out of range.", new object[] { maxLength.ToString(CultureInfo.InvariantCulture) }), "maxLength");
				}
			}
			if (SqlCompareOptions.BinarySort != compareOptions && (~(SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreNonSpace | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth) & compareOptions) != SqlCompareOptions.None)
			{
				throw ADP.InvalidEnumerationValue(typeof(SqlCompareOptions), (int)compareOptions);
			}
			this.SetDefaultsForType(dbType);
			this._strName = name;
			this._lMaxLength = maxLength;
			this._lLocale = locale;
			this._eCompareOptions = compareOptions;
			this._useServerDefault = useServerDefault;
			this._isUniqueKey = isUniqueKey;
			this._columnSortOrder = columnSortOrder;
			this._sortOrdinal = sortOrdinal;
		}

		private void Construct(string name, SqlDbType dbType, byte precision, byte scale, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.AssertNameIsValid(name);
			this.ValidateSortOrder(columnSortOrder, sortOrdinal);
			if (SqlDbType.Decimal == dbType)
			{
				if (precision > SqlDecimal.MaxPrecision || scale > precision)
				{
					throw SQL.PrecisionValueOutOfRange(precision);
				}
				if (scale > SqlDecimal.MaxScale)
				{
					throw SQL.ScaleValueOutOfRange(scale);
				}
			}
			else
			{
				if (SqlDbType.Time != dbType && SqlDbType.DateTime2 != dbType && SqlDbType.DateTimeOffset != dbType)
				{
					throw SQL.InvalidSqlDbTypeForConstructor(dbType);
				}
				if (scale > 7)
				{
					throw SQL.TimeScaleValueOutOfRange(scale);
				}
			}
			this.SetDefaultsForType(dbType);
			this._strName = name;
			this._bPrecision = precision;
			this._bScale = scale;
			if (SqlDbType.Decimal == dbType)
			{
				this._lMaxLength = (long)((ulong)SqlMetaData.s_maxLenFromPrecision[(int)(precision - 1)]);
			}
			else
			{
				this._lMaxLength -= (long)((ulong)SqlMetaData.s_maxVarTimeLenOffsetFromScale[(int)scale]);
			}
			this._useServerDefault = useServerDefault;
			this._isUniqueKey = isUniqueKey;
			this._columnSortOrder = columnSortOrder;
			this._sortOrdinal = sortOrdinal;
		}

		private void Construct(string name, SqlDbType dbType, string database, string owningSchema, string objectName, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			this.AssertNameIsValid(name);
			this.ValidateSortOrder(columnSortOrder, sortOrdinal);
			if (SqlDbType.Xml != dbType)
			{
				throw SQL.InvalidSqlDbTypeForConstructor(dbType);
			}
			if ((database != null || owningSchema != null) && objectName == null)
			{
				throw ADP.ArgumentNull("objectName");
			}
			this.SetDefaultsForType(SqlDbType.Xml);
			this._strName = name;
			this._xmlSchemaCollectionDatabase = database;
			this._xmlSchemaCollectionOwningSchema = owningSchema;
			this._xmlSchemaCollectionName = objectName;
			this._useServerDefault = useServerDefault;
			this._isUniqueKey = isUniqueKey;
			this._columnSortOrder = columnSortOrder;
			this._sortOrdinal = sortOrdinal;
		}

		private void AssertNameIsValid(string name)
		{
			if (name == null)
			{
				throw ADP.ArgumentNull("name");
			}
			if (128L < (long)name.Length)
			{
				throw SQL.NameTooLong("name");
			}
		}

		private void ValidateSortOrder(SortOrder columnSortOrder, int sortOrdinal)
		{
			if (SortOrder.Unspecified != columnSortOrder && columnSortOrder != SortOrder.Ascending && SortOrder.Descending != columnSortOrder)
			{
				throw SQL.InvalidSortOrder(columnSortOrder);
			}
			if (SortOrder.Unspecified == columnSortOrder != (-1 == sortOrdinal))
			{
				throw SQL.MustSpecifyBothSortOrderAndOrdinal(columnSortOrder, sortOrdinal);
			}
		}

		public short Adjust(short value)
		{
			if (SqlDbType.SmallInt != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public int Adjust(int value)
		{
			if (SqlDbType.Int != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public long Adjust(long value)
		{
			if (this.SqlDbType != SqlDbType.BigInt)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public float Adjust(float value)
		{
			if (SqlDbType.Real != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public double Adjust(double value)
		{
			if (SqlDbType.Float != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public string Adjust(string value)
		{
			if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
			{
				if (value != null && (long)value.Length < this.MaxLength)
				{
					value = value.PadRight((int)this.MaxLength);
				}
			}
			else if (SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (value == null)
			{
				return null;
			}
			if ((long)value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
			{
				value = value.Remove((int)this.MaxLength, (int)((long)value.Length - this.MaxLength));
			}
			return value;
		}

		public decimal Adjust(decimal value)
		{
			if (SqlDbType.Decimal != this.SqlDbType && SqlDbType.Money != this.SqlDbType && SqlDbType.SmallMoney != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (SqlDbType.Decimal != this.SqlDbType)
			{
				this.VerifyMoneyRange(new SqlMoney(value));
				return value;
			}
			return this.InternalAdjustSqlDecimal(new SqlDecimal(value)).Value;
		}

		public DateTime Adjust(DateTime value)
		{
			if (SqlDbType.DateTime == this.SqlDbType || SqlDbType.SmallDateTime == this.SqlDbType)
			{
				this.VerifyDateTimeRange(value);
			}
			else
			{
				if (SqlDbType.DateTime2 == this.SqlDbType)
				{
					return new DateTime(this.InternalAdjustTimeTicks(value.Ticks));
				}
				if (SqlDbType.Date == this.SqlDbType)
				{
					return value.Date;
				}
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public Guid Adjust(Guid value)
		{
			if (SqlDbType.UniqueIdentifier != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public SqlBoolean Adjust(SqlBoolean value)
		{
			if (SqlDbType.Bit != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public SqlByte Adjust(SqlByte value)
		{
			if (SqlDbType.TinyInt != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public SqlInt16 Adjust(SqlInt16 value)
		{
			if (SqlDbType.SmallInt != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public SqlInt32 Adjust(SqlInt32 value)
		{
			if (SqlDbType.Int != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public SqlInt64 Adjust(SqlInt64 value)
		{
			if (this.SqlDbType != SqlDbType.BigInt)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public SqlSingle Adjust(SqlSingle value)
		{
			if (SqlDbType.Real != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public SqlDouble Adjust(SqlDouble value)
		{
			if (SqlDbType.Float != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public SqlMoney Adjust(SqlMoney value)
		{
			if (SqlDbType.Money != this.SqlDbType && SqlDbType.SmallMoney != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (!value.IsNull)
			{
				this.VerifyMoneyRange(value);
			}
			return value;
		}

		public SqlDateTime Adjust(SqlDateTime value)
		{
			if (SqlDbType.DateTime != this.SqlDbType && SqlDbType.SmallDateTime != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (!value.IsNull)
			{
				this.VerifyDateTimeRange(value.Value);
			}
			return value;
		}

		public SqlDecimal Adjust(SqlDecimal value)
		{
			if (SqlDbType.Decimal != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return this.InternalAdjustSqlDecimal(value);
		}

		public SqlString Adjust(SqlString value)
		{
			if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
			{
				if (!value.IsNull && (long)value.Value.Length < this.MaxLength)
				{
					return new SqlString(value.Value.PadRight((int)this.MaxLength));
				}
			}
			else if (SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (value.IsNull)
			{
				return value;
			}
			if ((long)value.Value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
			{
				value = new SqlString(value.Value.Remove((int)this.MaxLength, (int)((long)value.Value.Length - this.MaxLength)));
			}
			return value;
		}

		public SqlBinary Adjust(SqlBinary value)
		{
			if (SqlDbType.Binary == this.SqlDbType || SqlDbType.Timestamp == this.SqlDbType)
			{
				if (!value.IsNull && (long)value.Length < this.MaxLength)
				{
					byte[] value2 = value.Value;
					byte[] array = new byte[this.MaxLength];
					Buffer.BlockCopy(value2, 0, array, 0, value2.Length);
					Array.Clear(array, value2.Length, array.Length - value2.Length);
					return new SqlBinary(array);
				}
			}
			else if (SqlDbType.VarBinary != this.SqlDbType && SqlDbType.Image != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (value.IsNull)
			{
				return value;
			}
			if ((long)value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
			{
				Array value3 = value.Value;
				byte[] array2 = new byte[this.MaxLength];
				Buffer.BlockCopy(value3, 0, array2, 0, (int)this.MaxLength);
				value = new SqlBinary(array2);
			}
			return value;
		}

		public SqlGuid Adjust(SqlGuid value)
		{
			if (SqlDbType.UniqueIdentifier != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public SqlChars Adjust(SqlChars value)
		{
			if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
			{
				if (value != null && !value.IsNull)
				{
					long length = value.Length;
					if (length < this.MaxLength)
					{
						if (value.MaxLength < this.MaxLength)
						{
							char[] array = new char[(int)this.MaxLength];
							Array.Copy(value.Buffer, 0, array, 0, (int)length);
							value = new SqlChars(array);
						}
						char[] buffer = value.Buffer;
						for (long num = length; num < this.MaxLength; num += 1L)
						{
							buffer[(int)(checked((IntPtr)num))] = ' ';
						}
						value.SetLength(this.MaxLength);
						return value;
					}
				}
			}
			else if (SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (value == null || value.IsNull)
			{
				return value;
			}
			if (value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
			{
				value.SetLength(this.MaxLength);
			}
			return value;
		}

		public SqlBytes Adjust(SqlBytes value)
		{
			if (SqlDbType.Binary == this.SqlDbType || SqlDbType.Timestamp == this.SqlDbType)
			{
				if (value != null && !value.IsNull)
				{
					int num = (int)value.Length;
					if ((long)num < this.MaxLength)
					{
						if (value.MaxLength < this.MaxLength)
						{
							byte[] array = new byte[this.MaxLength];
							Buffer.BlockCopy(value.Buffer, 0, array, 0, num);
							value = new SqlBytes(array);
						}
						byte[] buffer = value.Buffer;
						Array.Clear(buffer, num, buffer.Length - num);
						value.SetLength(this.MaxLength);
						return value;
					}
				}
			}
			else if (SqlDbType.VarBinary != this.SqlDbType && SqlDbType.Image != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (value == null || value.IsNull)
			{
				return value;
			}
			if (value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
			{
				value.SetLength(this.MaxLength);
			}
			return value;
		}

		public SqlXml Adjust(SqlXml value)
		{
			if (SqlDbType.Xml != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public TimeSpan Adjust(TimeSpan value)
		{
			if (SqlDbType.Time != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			this.VerifyTimeRange(value);
			return new TimeSpan(this.InternalAdjustTimeTicks(value.Ticks));
		}

		public DateTimeOffset Adjust(DateTimeOffset value)
		{
			if (SqlDbType.DateTimeOffset != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return new DateTimeOffset(this.InternalAdjustTimeTicks(value.Ticks), value.Offset);
		}

		public object Adjust(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (value is bool)
			{
				value = this.Adjust((bool)value);
			}
			else if (value is byte)
			{
				value = this.Adjust((byte)value);
			}
			else if (value is char)
			{
				value = this.Adjust((char)value);
			}
			else if (value is DateTime)
			{
				value = this.Adjust((DateTime)value);
			}
			else if (!(value is DBNull))
			{
				if (value is decimal)
				{
					value = this.Adjust((decimal)value);
				}
				else if (value is double)
				{
					value = this.Adjust((double)value);
				}
				else if (value is short)
				{
					value = this.Adjust((short)value);
				}
				else if (value is int)
				{
					value = this.Adjust((int)value);
				}
				else if (value is long)
				{
					value = this.Adjust((long)value);
				}
				else
				{
					if (value is sbyte)
					{
						throw ADP.InvalidDataType("SByte");
					}
					if (value is float)
					{
						value = this.Adjust((float)value);
					}
					else if (value is string)
					{
						value = this.Adjust((string)value);
					}
					else
					{
						if (value is ushort)
						{
							throw ADP.InvalidDataType("UInt16");
						}
						if (value is uint)
						{
							throw ADP.InvalidDataType("UInt32");
						}
						if (value is ulong)
						{
							throw ADP.InvalidDataType("UInt64");
						}
						if (value is byte[])
						{
							value = this.Adjust((byte[])value);
						}
						else if (value is char[])
						{
							value = this.Adjust((char[])value);
						}
						else if (value is Guid)
						{
							value = this.Adjust((Guid)value);
						}
						else if (value is SqlBinary)
						{
							value = this.Adjust((SqlBinary)value);
						}
						else if (value is SqlBoolean)
						{
							value = this.Adjust((SqlBoolean)value);
						}
						else if (value is SqlByte)
						{
							value = this.Adjust((SqlByte)value);
						}
						else if (value is SqlDateTime)
						{
							value = this.Adjust((SqlDateTime)value);
						}
						else if (value is SqlDouble)
						{
							value = this.Adjust((SqlDouble)value);
						}
						else if (value is SqlGuid)
						{
							value = this.Adjust((SqlGuid)value);
						}
						else if (value is SqlInt16)
						{
							value = this.Adjust((SqlInt16)value);
						}
						else if (value is SqlInt32)
						{
							value = this.Adjust((SqlInt32)value);
						}
						else if (value is SqlInt64)
						{
							value = this.Adjust((SqlInt64)value);
						}
						else if (value is SqlMoney)
						{
							value = this.Adjust((SqlMoney)value);
						}
						else if (value is SqlDecimal)
						{
							value = this.Adjust((SqlDecimal)value);
						}
						else if (value is SqlSingle)
						{
							value = this.Adjust((SqlSingle)value);
						}
						else if (value is SqlString)
						{
							value = this.Adjust((SqlString)value);
						}
						else if (value is SqlChars)
						{
							value = this.Adjust((SqlChars)value);
						}
						else if (value is SqlBytes)
						{
							value = this.Adjust((SqlBytes)value);
						}
						else if (value is SqlXml)
						{
							value = this.Adjust((SqlXml)value);
						}
						else if (value is TimeSpan)
						{
							value = this.Adjust((TimeSpan)value);
						}
						else
						{
							if (!(value is DateTimeOffset))
							{
								throw ADP.UnknownDataType(value.GetType());
							}
							value = this.Adjust((DateTimeOffset)value);
						}
					}
				}
			}
			return value;
		}

		public static SqlMetaData InferFromValue(object value, string name)
		{
			if (value == null)
			{
				throw ADP.ArgumentNull("value");
			}
			SqlMetaData sqlMetaData;
			if (value is bool)
			{
				sqlMetaData = new SqlMetaData(name, SqlDbType.Bit);
			}
			else if (value is byte)
			{
				sqlMetaData = new SqlMetaData(name, SqlDbType.TinyInt);
			}
			else if (value is char)
			{
				sqlMetaData = new SqlMetaData(name, SqlDbType.NVarChar, 1L);
			}
			else if (value is DateTime)
			{
				sqlMetaData = new SqlMetaData(name, SqlDbType.DateTime);
			}
			else
			{
				if (value is DBNull)
				{
					throw ADP.InvalidDataType("DBNull");
				}
				if (value is decimal)
				{
					SqlDecimal sqlDecimal = new SqlDecimal((decimal)value);
					sqlMetaData = new SqlMetaData(name, SqlDbType.Decimal, sqlDecimal.Precision, sqlDecimal.Scale);
				}
				else if (value is double)
				{
					sqlMetaData = new SqlMetaData(name, SqlDbType.Float);
				}
				else if (value is short)
				{
					sqlMetaData = new SqlMetaData(name, SqlDbType.SmallInt);
				}
				else if (value is int)
				{
					sqlMetaData = new SqlMetaData(name, SqlDbType.Int);
				}
				else if (value is long)
				{
					sqlMetaData = new SqlMetaData(name, SqlDbType.BigInt);
				}
				else
				{
					if (value is sbyte)
					{
						throw ADP.InvalidDataType("SByte");
					}
					if (value is float)
					{
						sqlMetaData = new SqlMetaData(name, SqlDbType.Real);
					}
					else if (value is string)
					{
						long num = (long)((string)value).Length;
						if (num < 1L)
						{
							num = 1L;
						}
						if (4000L < num)
						{
							num = SqlMetaData.Max;
						}
						sqlMetaData = new SqlMetaData(name, SqlDbType.NVarChar, num);
					}
					else
					{
						if (value is ushort)
						{
							throw ADP.InvalidDataType("UInt16");
						}
						if (value is uint)
						{
							throw ADP.InvalidDataType("UInt32");
						}
						if (value is ulong)
						{
							throw ADP.InvalidDataType("UInt64");
						}
						if (value is byte[])
						{
							long num2 = (long)((byte[])value).Length;
							if (num2 < 1L)
							{
								num2 = 1L;
							}
							if (8000L < num2)
							{
								num2 = SqlMetaData.Max;
							}
							sqlMetaData = new SqlMetaData(name, SqlDbType.VarBinary, num2);
						}
						else if (value is char[])
						{
							long num3 = (long)((char[])value).Length;
							if (num3 < 1L)
							{
								num3 = 1L;
							}
							if (4000L < num3)
							{
								num3 = SqlMetaData.Max;
							}
							sqlMetaData = new SqlMetaData(name, SqlDbType.NVarChar, num3);
						}
						else if (value is Guid)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.UniqueIdentifier);
						}
						else if (value != null)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.Variant);
						}
						else if (value is SqlBinary)
						{
							SqlBinary sqlBinary = (SqlBinary)value;
							long num4;
							if (!sqlBinary.IsNull)
							{
								num4 = (long)sqlBinary.Length;
								if (num4 < 1L)
								{
									num4 = 1L;
								}
								if (8000L < num4)
								{
									num4 = SqlMetaData.Max;
								}
							}
							else
							{
								num4 = SqlMetaData.sxm_rgDefaults[21].MaxLength;
							}
							sqlMetaData = new SqlMetaData(name, SqlDbType.VarBinary, num4);
						}
						else if (value is SqlBoolean)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.Bit);
						}
						else if (value is SqlByte)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.TinyInt);
						}
						else if (value is SqlDateTime)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.DateTime);
						}
						else if (value is SqlDouble)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.Float);
						}
						else if (value is SqlGuid)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.UniqueIdentifier);
						}
						else if (value is SqlInt16)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.SmallInt);
						}
						else if (value is SqlInt32)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.Int);
						}
						else if (value is SqlInt64)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.BigInt);
						}
						else if (value is SqlMoney)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.Money);
						}
						else if (value is SqlDecimal)
						{
							SqlDecimal sqlDecimal2 = (SqlDecimal)value;
							byte b;
							byte b2;
							if (!sqlDecimal2.IsNull)
							{
								b = sqlDecimal2.Precision;
								b2 = sqlDecimal2.Scale;
							}
							else
							{
								b = SqlMetaData.sxm_rgDefaults[5].Precision;
								b2 = SqlMetaData.sxm_rgDefaults[5].Scale;
							}
							sqlMetaData = new SqlMetaData(name, SqlDbType.Decimal, b, b2);
						}
						else if (value is SqlSingle)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.Real);
						}
						else if (value is SqlString)
						{
							SqlString sqlString = (SqlString)value;
							if (!sqlString.IsNull)
							{
								long num5 = (long)sqlString.Value.Length;
								if (num5 < 1L)
								{
									num5 = 1L;
								}
								if (num5 > 4000L)
								{
									num5 = SqlMetaData.Max;
								}
								sqlMetaData = new SqlMetaData(name, SqlDbType.NVarChar, num5, (long)sqlString.LCID, sqlString.SqlCompareOptions);
							}
							else
							{
								sqlMetaData = new SqlMetaData(name, SqlDbType.NVarChar, SqlMetaData.sxm_rgDefaults[12].MaxLength);
							}
						}
						else if (value is SqlChars)
						{
							SqlChars sqlChars = (SqlChars)value;
							long num6;
							if (!sqlChars.IsNull)
							{
								num6 = sqlChars.Length;
								if (num6 < 1L)
								{
									num6 = 1L;
								}
								if (num6 > 4000L)
								{
									num6 = SqlMetaData.Max;
								}
							}
							else
							{
								num6 = SqlMetaData.sxm_rgDefaults[12].MaxLength;
							}
							sqlMetaData = new SqlMetaData(name, SqlDbType.NVarChar, num6);
						}
						else if (value is SqlBytes)
						{
							SqlBytes sqlBytes = (SqlBytes)value;
							long num7;
							if (!sqlBytes.IsNull)
							{
								num7 = sqlBytes.Length;
								if (num7 < 1L)
								{
									num7 = 1L;
								}
								else if (8000L < num7)
								{
									num7 = SqlMetaData.Max;
								}
							}
							else
							{
								num7 = SqlMetaData.sxm_rgDefaults[21].MaxLength;
							}
							sqlMetaData = new SqlMetaData(name, SqlDbType.VarBinary, num7);
						}
						else if (value is SqlXml)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.Xml);
						}
						else if (value is TimeSpan)
						{
							sqlMetaData = new SqlMetaData(name, SqlDbType.Time, 0, SqlMetaData.InferScaleFromTimeTicks(((TimeSpan)value).Ticks));
						}
						else
						{
							if (!(value is DateTimeOffset))
							{
								throw ADP.UnknownDataType(value.GetType());
							}
							sqlMetaData = new SqlMetaData(name, SqlDbType.DateTimeOffset, 0, SqlMetaData.InferScaleFromTimeTicks(((DateTimeOffset)value).Ticks));
						}
					}
				}
			}
			return sqlMetaData;
		}

		public bool Adjust(bool value)
		{
			if (SqlDbType.Bit != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public byte Adjust(byte value)
		{
			if (SqlDbType.TinyInt != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public byte[] Adjust(byte[] value)
		{
			if (SqlDbType.Binary == this.SqlDbType || SqlDbType.Timestamp == this.SqlDbType)
			{
				if (value != null && (long)value.Length < this.MaxLength)
				{
					byte[] array = new byte[this.MaxLength];
					Buffer.BlockCopy(value, 0, array, 0, value.Length);
					Array.Clear(array, value.Length, array.Length - value.Length);
					return array;
				}
			}
			else if (SqlDbType.VarBinary != this.SqlDbType && SqlDbType.Image != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (value == null)
			{
				return null;
			}
			if ((long)value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
			{
				byte[] array2 = new byte[this.MaxLength];
				Buffer.BlockCopy(value, 0, array2, 0, (int)this.MaxLength);
				value = array2;
			}
			return value;
		}

		public char Adjust(char value)
		{
			if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
			{
				if (1L != this.MaxLength)
				{
					SqlMetaData.ThrowInvalidType();
				}
			}
			else if (1L > this.MaxLength || (SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType))
			{
				SqlMetaData.ThrowInvalidType();
			}
			return value;
		}

		public char[] Adjust(char[] value)
		{
			if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
			{
				if (value != null)
				{
					long num = (long)value.Length;
					if (num < this.MaxLength)
					{
						char[] array = new char[(int)this.MaxLength];
						Array.Copy(value, 0, array, 0, (int)num);
						for (long num2 = num; num2 < (long)array.Length; num2 += 1L)
						{
							array[(int)(checked((IntPtr)num2))] = ' ';
						}
						return array;
					}
				}
			}
			else if (SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (value == null)
			{
				return null;
			}
			if ((long)value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
			{
				char[] array2 = new char[this.MaxLength];
				Array.Copy(value, 0, array2, 0, (int)this.MaxLength);
				value = array2;
			}
			return value;
		}

		internal static SqlMetaData GetPartialLengthMetaData(SqlMetaData md)
		{
			if (md.IsPartialLength)
			{
				return md;
			}
			if (md.SqlDbType == SqlDbType.Xml)
			{
				SqlMetaData.ThrowInvalidType();
			}
			if (md.SqlDbType == SqlDbType.NVarChar || md.SqlDbType == SqlDbType.VarChar || md.SqlDbType == SqlDbType.VarBinary)
			{
				return new SqlMetaData(md.Name, md.SqlDbType, SqlMetaData.Max, 0, 0, md.LocaleId, md.CompareOptions, null, null, null, true);
			}
			return md;
		}

		private static void ThrowInvalidType()
		{
			throw ADP.InvalidMetaDataValue();
		}

		private void VerifyDateTimeRange(DateTime value)
		{
			if (SqlDbType.SmallDateTime == this.SqlDbType && (SqlMetaData.s_dtSmallMax < value || SqlMetaData.s_dtSmallMin > value))
			{
				SqlMetaData.ThrowInvalidType();
			}
		}

		private void VerifyMoneyRange(SqlMoney value)
		{
			if (SqlDbType.SmallMoney == this.SqlDbType && ((SqlMetaData.s_smSmallMax < value).Value || (SqlMetaData.s_smSmallMin > value).Value))
			{
				SqlMetaData.ThrowInvalidType();
			}
		}

		private SqlDecimal InternalAdjustSqlDecimal(SqlDecimal value)
		{
			if (!value.IsNull && (value.Precision != this.Precision || value.Scale != this.Scale))
			{
				if (value.Scale != this.Scale)
				{
					value = SqlDecimal.AdjustScale(value, (int)(this.Scale - value.Scale), false);
				}
				return SqlDecimal.ConvertToPrecScale(value, (int)this.Precision, (int)this.Scale);
			}
			return value;
		}

		private void VerifyTimeRange(TimeSpan value)
		{
			if (SqlDbType.Time == this.SqlDbType && (SqlMetaData.s_timeMin > value || value > SqlMetaData.s_timeMax))
			{
				SqlMetaData.ThrowInvalidType();
			}
		}

		private long InternalAdjustTimeTicks(long ticks)
		{
			return ticks / SqlMetaData.s_unitTicksFromScale[(int)this.Scale] * SqlMetaData.s_unitTicksFromScale[(int)this.Scale];
		}

		private static byte InferScaleFromTimeTicks(long ticks)
		{
			for (byte b = 0; b < 7; b += 1)
			{
				if (ticks / SqlMetaData.s_unitTicksFromScale[(int)b] * SqlMetaData.s_unitTicksFromScale[(int)b] == ticks)
				{
					return b;
				}
			}
			return 7;
		}

		private void SetDefaultsForType(SqlDbType dbType)
		{
			if (SqlDbType.BigInt <= dbType && SqlDbType.DateTimeOffset >= dbType)
			{
				SqlMetaData sqlMetaData = SqlMetaData.sxm_rgDefaults[(int)dbType];
				this._sqlDbType = dbType;
				this._lMaxLength = sqlMetaData.MaxLength;
				this._bPrecision = sqlMetaData.Precision;
				this._bScale = sqlMetaData.Scale;
				this._lLocale = sqlMetaData.LocaleId;
				this._eCompareOptions = sqlMetaData.CompareOptions;
			}
		}

		private void ThrowIfUdt(SqlDbType dbType)
		{
			if (dbType == SqlDbType.Udt)
			{
				throw ADP.DbTypeNotSupported(SqlDbType.Udt.ToString());
			}
		}

		public SqlMetaData(string name, SqlDbType dbType, Type userDefinedType)
			: this(name, dbType, -1L, 0, 0, 0L, SqlCompareOptions.None, userDefinedType)
		{
		}

		[MonoTODO]
		public DbType DbType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public SqlMetaData(string name, SqlDbType dbType, Type userDefinedType, string serverTypeName)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public SqlMetaData(string name, SqlDbType dbType, Type userDefinedType, string serverTypeName, bool useServerDefault, bool isUniqueKey, SortOrder columnSortOrder, int sortOrdinal)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public Type Type
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		private string _strName;

		private long _lMaxLength;

		private SqlDbType _sqlDbType;

		private byte _bPrecision;

		private byte _bScale;

		private long _lLocale;

		private SqlCompareOptions _eCompareOptions;

		private string _xmlSchemaCollectionDatabase;

		private string _xmlSchemaCollectionOwningSchema;

		private string _xmlSchemaCollectionName;

		private bool _bPartialLength;

		private bool _useServerDefault;

		private bool _isUniqueKey;

		private SortOrder _columnSortOrder;

		private int _sortOrdinal;

		private const long x_lMax = -1L;

		private const long x_lServerMaxUnicode = 4000L;

		private const long x_lServerMaxANSI = 8000L;

		private const long x_lServerMaxBinary = 8000L;

		private const bool x_defaultUseServerDefault = false;

		private const bool x_defaultIsUniqueKey = false;

		private const SortOrder x_defaultColumnSortOrder = SortOrder.Unspecified;

		private const int x_defaultSortOrdinal = -1;

		private const SqlCompareOptions x_eDefaultStringCompareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;

		private static byte[] s_maxLenFromPrecision = new byte[]
		{
			5, 5, 5, 5, 5, 5, 5, 5, 5, 9,
			9, 9, 9, 9, 9, 9, 9, 9, 9, 13,
			13, 13, 13, 13, 13, 13, 13, 13, 17, 17,
			17, 17, 17, 17, 17, 17, 17, 17
		};

		private const byte MaxTimeScale = 7;

		private static byte[] s_maxVarTimeLenOffsetFromScale = new byte[] { 2, 2, 2, 1, 1, 0, 0, 0 };

		private static readonly DateTime s_dtSmallMax = new DateTime(2079, 6, 6, 23, 59, 29, 998);

		private static readonly DateTime s_dtSmallMin = new DateTime(1899, 12, 31, 23, 59, 29, 999);

		private static readonly SqlMoney s_smSmallMax = new SqlMoney(214748.3647m);

		private static readonly SqlMoney s_smSmallMin = new SqlMoney(-214748.3648m);

		private static readonly TimeSpan s_timeMin = TimeSpan.Zero;

		private static readonly TimeSpan s_timeMax = new TimeSpan(863999999999L);

		private static readonly long[] s_unitTicksFromScale = new long[] { 10000000L, 1000000L, 100000L, 10000L, 1000L, 100L, 10L, 1L };

		internal static SqlMetaData[] sxm_rgDefaults = new SqlMetaData[]
		{
			new SqlMetaData("bigint", SqlDbType.BigInt, 8L, 19, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("binary", SqlDbType.Binary, 1L, 0, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("bit", SqlDbType.Bit, 1L, 1, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("char", SqlDbType.Char, 1L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("datetime", SqlDbType.DateTime, 8L, 23, 3, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("decimal", SqlDbType.Decimal, 9L, 18, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("float", SqlDbType.Float, 8L, 53, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("image", SqlDbType.Image, -1L, 0, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("int", SqlDbType.Int, 4L, 10, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("money", SqlDbType.Money, 8L, 19, 4, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("nchar", SqlDbType.NChar, 1L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("ntext", SqlDbType.NText, -1L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("nvarchar", SqlDbType.NVarChar, 4000L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("real", SqlDbType.Real, 4L, 24, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("uniqueidentifier", SqlDbType.UniqueIdentifier, 16L, 0, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("smalldatetime", SqlDbType.SmallDateTime, 4L, 16, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("smallint", SqlDbType.SmallInt, 2L, 5, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("smallmoney", SqlDbType.SmallMoney, 4L, 10, 4, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("text", SqlDbType.Text, -1L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("timestamp", SqlDbType.Timestamp, 8L, 0, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("tinyint", SqlDbType.TinyInt, 1L, 3, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("varbinary", SqlDbType.VarBinary, 8000L, 0, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("varchar", SqlDbType.VarChar, 8000L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("sql_variant", SqlDbType.Variant, 8016L, 0, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("nvarchar", SqlDbType.NVarChar, 1L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("xml", SqlDbType.Xml, -1L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, true),
			new SqlMetaData("nvarchar", SqlDbType.NVarChar, 1L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("nvarchar", SqlDbType.NVarChar, 4000L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("nvarchar", SqlDbType.NVarChar, 4000L, 0, 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
			new SqlMetaData("udt", SqlDbType.Structured, 0L, 0, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("table", SqlDbType.Structured, 0L, 0, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("date", SqlDbType.Date, 3L, 10, 0, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("time", SqlDbType.Time, 5L, 0, 7, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("datetime2", SqlDbType.DateTime2, 8L, 0, 7, 0L, SqlCompareOptions.None, false),
			new SqlMetaData("datetimeoffset", SqlDbType.DateTimeOffset, 10L, 0, 7, 0L, SqlCompareOptions.None, false)
		};
	}
}
