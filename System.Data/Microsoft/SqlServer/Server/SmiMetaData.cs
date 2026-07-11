using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;

namespace Microsoft.SqlServer.Server
{
	internal class SmiMetaData
	{
		internal static SmiMetaData DefaultNVarChar
		{
			get
			{
				return new SmiMetaData(SmiMetaData.DefaultNVarChar_NoCollation.SqlDbType, SmiMetaData.DefaultNVarChar_NoCollation.MaxLength, SmiMetaData.DefaultNVarChar_NoCollation.Precision, SmiMetaData.DefaultNVarChar_NoCollation.Scale, (long)CultureInfo.CurrentCulture.LCID, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth);
			}
		}

		internal SmiMetaData(SqlDbType dbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions)
			: this(dbType, maxLength, precision, scale, localeId, compareOptions, false, null, null)
		{
		}

		internal SmiMetaData(SqlDbType dbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, bool isMultiValued, IList<SmiExtendedMetaData> fieldTypes, SmiMetaDataPropertyCollection extendedProperties)
		{
			this.SetDefaultsForType(dbType);
			switch (dbType)
			{
			case SqlDbType.Binary:
			case SqlDbType.VarBinary:
				this._maxLength = maxLength;
				break;
			case SqlDbType.Char:
			case SqlDbType.NChar:
			case SqlDbType.NVarChar:
			case SqlDbType.VarChar:
				this._maxLength = maxLength;
				this._localeId = localeId;
				this._compareOptions = compareOptions;
				break;
			case SqlDbType.Decimal:
				this._precision = precision;
				this._scale = scale;
				this._maxLength = (long)((ulong)SmiMetaData.s_maxLenFromPrecision[(int)(precision - 1)]);
				break;
			case SqlDbType.NText:
			case SqlDbType.Text:
				this._localeId = localeId;
				this._compareOptions = compareOptions;
				break;
			case SqlDbType.Udt:
				throw ADP.DbTypeNotSupported(SqlDbType.Udt.ToString());
			case SqlDbType.Structured:
				if (fieldTypes != null)
				{
					this._fieldMetaData = new ReadOnlyCollection<SmiExtendedMetaData>(fieldTypes);
				}
				this._isMultiValued = isMultiValued;
				this._maxLength = (long)this._fieldMetaData.Count;
				break;
			case SqlDbType.Time:
				this._scale = scale;
				this._maxLength = (long)(5 - SmiMetaData.s_maxVarTimeLenOffsetFromScale[(int)scale]);
				break;
			case SqlDbType.DateTime2:
				this._scale = scale;
				this._maxLength = (long)(8 - SmiMetaData.s_maxVarTimeLenOffsetFromScale[(int)scale]);
				break;
			case SqlDbType.DateTimeOffset:
				this._scale = scale;
				this._maxLength = (long)(10 - SmiMetaData.s_maxVarTimeLenOffsetFromScale[(int)scale]);
				break;
			}
			if (extendedProperties != null)
			{
				extendedProperties.SetReadOnly();
				this._extendedProperties = extendedProperties;
			}
		}

		internal SqlCompareOptions CompareOptions
		{
			get
			{
				return this._compareOptions;
			}
		}

		internal long LocaleId
		{
			get
			{
				return this._localeId;
			}
		}

		internal long MaxLength
		{
			get
			{
				return this._maxLength;
			}
		}

		internal byte Precision
		{
			get
			{
				return this._precision;
			}
		}

		internal byte Scale
		{
			get
			{
				return this._scale;
			}
		}

		internal SqlDbType SqlDbType
		{
			get
			{
				return this._databaseType;
			}
		}

		internal bool IsMultiValued
		{
			get
			{
				return this._isMultiValued;
			}
		}

		internal IList<SmiExtendedMetaData> FieldMetaData
		{
			get
			{
				return this._fieldMetaData;
			}
		}

		internal SmiMetaDataPropertyCollection ExtendedProperties
		{
			get
			{
				return this._extendedProperties;
			}
		}

		internal static bool IsSupportedDbType(SqlDbType dbType)
		{
			return (SqlDbType.BigInt <= dbType && SqlDbType.Xml >= dbType) || (SqlDbType.Udt <= dbType && SqlDbType.DateTimeOffset >= dbType);
		}

		internal static SmiMetaData GetDefaultForType(SqlDbType dbType)
		{
			return SmiMetaData.s_defaultValues[(int)dbType];
		}

		private SmiMetaData(SqlDbType sqlDbType, long maxLength, byte precision, byte scale, SqlCompareOptions compareOptions)
		{
			this._databaseType = sqlDbType;
			this._maxLength = maxLength;
			this._precision = precision;
			this._scale = scale;
			this._compareOptions = compareOptions;
			this._localeId = 0L;
			this._isMultiValued = false;
			this._fieldMetaData = SmiMetaData.s_emptyFieldList;
			this._extendedProperties = SmiMetaDataPropertyCollection.EmptyInstance;
		}

		private void SetDefaultsForType(SqlDbType dbType)
		{
			SmiMetaData defaultForType = SmiMetaData.GetDefaultForType(dbType);
			this._databaseType = dbType;
			this._maxLength = defaultForType.MaxLength;
			this._precision = defaultForType.Precision;
			this._scale = defaultForType.Scale;
			this._localeId = defaultForType.LocaleId;
			this._compareOptions = defaultForType.CompareOptions;
			this._isMultiValued = defaultForType._isMultiValued;
			this._fieldMetaData = defaultForType._fieldMetaData;
			this._extendedProperties = defaultForType._extendedProperties;
		}

		private SqlDbType _databaseType;

		private long _maxLength;

		private byte _precision;

		private byte _scale;

		private long _localeId;

		private SqlCompareOptions _compareOptions;

		private bool _isMultiValued;

		private IList<SmiExtendedMetaData> _fieldMetaData;

		private SmiMetaDataPropertyCollection _extendedProperties;

		internal const long UnlimitedMaxLengthIndicator = -1L;

		internal const long MaxUnicodeCharacters = 4000L;

		internal const long MaxANSICharacters = 8000L;

		internal const long MaxBinaryLength = 8000L;

		internal const int MinPrecision = 1;

		internal const int MinScale = 0;

		internal const int MaxTimeScale = 7;

		internal static readonly DateTime MaxSmallDateTime = new DateTime(2079, 6, 6, 23, 59, 29, 998);

		internal static readonly DateTime MinSmallDateTime = new DateTime(1899, 12, 31, 23, 59, 29, 999);

		internal static readonly SqlMoney MaxSmallMoney = new SqlMoney(214748.3647m);

		internal static readonly SqlMoney MinSmallMoney = new SqlMoney(-214748.3648m);

		internal const SqlCompareOptions DefaultStringCompareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;

		internal const long MaxNameLength = 128L;

		private static readonly IList<SmiExtendedMetaData> s_emptyFieldList = new SmiExtendedMetaData[0];

		private static byte[] s_maxLenFromPrecision = new byte[]
		{
			5, 5, 5, 5, 5, 5, 5, 5, 5, 9,
			9, 9, 9, 9, 9, 9, 9, 9, 9, 13,
			13, 13, 13, 13, 13, 13, 13, 13, 17, 17,
			17, 17, 17, 17, 17, 17, 17, 17
		};

		private static byte[] s_maxVarTimeLenOffsetFromScale = new byte[] { 2, 2, 2, 1, 1, 0, 0, 0 };

		internal static readonly SmiMetaData DefaultBigInt = new SmiMetaData(SqlDbType.BigInt, 8L, 19, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultBinary = new SmiMetaData(SqlDbType.Binary, 1L, 0, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultBit = new SmiMetaData(SqlDbType.Bit, 1L, 1, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultChar_NoCollation = new SmiMetaData(SqlDbType.Char, 1L, 0, 0, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth);

		internal static readonly SmiMetaData DefaultDateTime = new SmiMetaData(SqlDbType.DateTime, 8L, 23, 3, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultDecimal = new SmiMetaData(SqlDbType.Decimal, 9L, 18, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultFloat = new SmiMetaData(SqlDbType.Float, 8L, 53, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultImage = new SmiMetaData(SqlDbType.Image, -1L, 0, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultInt = new SmiMetaData(SqlDbType.Int, 4L, 10, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultMoney = new SmiMetaData(SqlDbType.Money, 8L, 19, 4, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultNChar_NoCollation = new SmiMetaData(SqlDbType.NChar, 1L, 0, 0, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth);

		internal static readonly SmiMetaData DefaultNText_NoCollation = new SmiMetaData(SqlDbType.NText, -1L, 0, 0, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth);

		internal static readonly SmiMetaData DefaultNVarChar_NoCollation = new SmiMetaData(SqlDbType.NVarChar, 4000L, 0, 0, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth);

		internal static readonly SmiMetaData DefaultReal = new SmiMetaData(SqlDbType.Real, 4L, 24, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultUniqueIdentifier = new SmiMetaData(SqlDbType.UniqueIdentifier, 16L, 0, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultSmallDateTime = new SmiMetaData(SqlDbType.SmallDateTime, 4L, 16, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultSmallInt = new SmiMetaData(SqlDbType.SmallInt, 2L, 5, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultSmallMoney = new SmiMetaData(SqlDbType.SmallMoney, 4L, 10, 4, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultText_NoCollation = new SmiMetaData(SqlDbType.Text, -1L, 0, 0, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth);

		internal static readonly SmiMetaData DefaultTimestamp = new SmiMetaData(SqlDbType.Timestamp, 8L, 0, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultTinyInt = new SmiMetaData(SqlDbType.TinyInt, 1L, 3, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultVarBinary = new SmiMetaData(SqlDbType.VarBinary, 8000L, 0, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultVarChar_NoCollation = new SmiMetaData(SqlDbType.VarChar, 8000L, 0, 0, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth);

		internal static readonly SmiMetaData DefaultVariant = new SmiMetaData(SqlDbType.Variant, 8016L, 0, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultXml = new SmiMetaData(SqlDbType.Xml, -1L, 0, 0, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth);

		internal static readonly SmiMetaData DefaultStructured = new SmiMetaData(SqlDbType.Structured, 0L, 0, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultDate = new SmiMetaData(SqlDbType.Date, 3L, 10, 0, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultTime = new SmiMetaData(SqlDbType.Time, 5L, 0, 7, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultDateTime2 = new SmiMetaData(SqlDbType.DateTime2, 8L, 0, 7, SqlCompareOptions.None);

		internal static readonly SmiMetaData DefaultDateTimeOffset = new SmiMetaData(SqlDbType.DateTimeOffset, 10L, 0, 7, SqlCompareOptions.None);

		private static SmiMetaData[] s_defaultValues = new SmiMetaData[]
		{
			SmiMetaData.DefaultBigInt,
			SmiMetaData.DefaultBinary,
			SmiMetaData.DefaultBit,
			SmiMetaData.DefaultChar_NoCollation,
			SmiMetaData.DefaultDateTime,
			SmiMetaData.DefaultDecimal,
			SmiMetaData.DefaultFloat,
			SmiMetaData.DefaultImage,
			SmiMetaData.DefaultInt,
			SmiMetaData.DefaultMoney,
			SmiMetaData.DefaultNChar_NoCollation,
			SmiMetaData.DefaultNText_NoCollation,
			SmiMetaData.DefaultNVarChar_NoCollation,
			SmiMetaData.DefaultReal,
			SmiMetaData.DefaultUniqueIdentifier,
			SmiMetaData.DefaultSmallDateTime,
			SmiMetaData.DefaultSmallInt,
			SmiMetaData.DefaultSmallMoney,
			SmiMetaData.DefaultText_NoCollation,
			SmiMetaData.DefaultTimestamp,
			SmiMetaData.DefaultTinyInt,
			SmiMetaData.DefaultVarBinary,
			SmiMetaData.DefaultVarChar_NoCollation,
			SmiMetaData.DefaultVariant,
			SmiMetaData.DefaultNVarChar_NoCollation,
			SmiMetaData.DefaultXml,
			SmiMetaData.DefaultNVarChar_NoCollation,
			SmiMetaData.DefaultNVarChar_NoCollation,
			SmiMetaData.DefaultNVarChar_NoCollation,
			null,
			SmiMetaData.DefaultStructured,
			SmiMetaData.DefaultDate,
			SmiMetaData.DefaultTime,
			SmiMetaData.DefaultDateTime2,
			SmiMetaData.DefaultDateTimeOffset
		};
	}
}
