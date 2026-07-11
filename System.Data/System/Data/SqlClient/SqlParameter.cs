using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using Microsoft.SqlServer.Server;
using Unity;

namespace System.Data.SqlClient
{
	public sealed class SqlParameter : DbParameter, IDbDataParameter, IDataParameter, ICloneable
	{
		public SqlParameter()
		{
		}

		public SqlParameter(string parameterName, SqlDbType dbType)
			: this()
		{
			this.ParameterName = parameterName;
			this.SqlDbType = dbType;
		}

		public SqlParameter(string parameterName, object value)
			: this()
		{
			this.ParameterName = parameterName;
			this.Value = value;
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size)
			: this()
		{
			this.ParameterName = parameterName;
			this.SqlDbType = dbType;
			this.Size = size;
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size, string sourceColumn)
			: this()
		{
			this.ParameterName = parameterName;
			this.SqlDbType = dbType;
			this.Size = size;
			this.SourceColumn = sourceColumn;
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
			: this(parameterName, dbType, size, sourceColumn)
		{
			this.Direction = direction;
			this.IsNullable = isNullable;
			this.Precision = precision;
			this.Scale = scale;
			this.SourceVersion = sourceVersion;
			this.Value = value;
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value, string xmlSchemaCollectionDatabase, string xmlSchemaCollectionOwningSchema, string xmlSchemaCollectionName)
			: this()
		{
			this.ParameterName = parameterName;
			this.SqlDbType = dbType;
			this.Size = size;
			this.Direction = direction;
			this.Precision = precision;
			this.Scale = scale;
			this.SourceColumn = sourceColumn;
			this.SourceVersion = sourceVersion;
			this.SourceColumnNullMapping = sourceColumnNullMapping;
			this.Value = value;
			this.XmlSchemaCollectionDatabase = xmlSchemaCollectionDatabase;
			this.XmlSchemaCollectionOwningSchema = xmlSchemaCollectionOwningSchema;
			this.XmlSchemaCollectionName = xmlSchemaCollectionName;
		}

		private SqlParameter(SqlParameter source)
			: this()
		{
			ADP.CheckArgumentNull(source, "source");
			source.CloneHelper(this);
			ICloneable cloneable = this._value as ICloneable;
			if (cloneable != null)
			{
				this._value = cloneable.Clone();
			}
		}

		internal SqlCollation Collation
		{
			get
			{
				return this._collation;
			}
			set
			{
				this._collation = value;
			}
		}

		public SqlCompareOptions CompareInfo
		{
			get
			{
				SqlCollation collation = this._collation;
				if (collation != null)
				{
					return collation.SqlCompareOptions;
				}
				return SqlCompareOptions.None;
			}
			set
			{
				SqlCollation sqlCollation = this._collation;
				if (sqlCollation == null)
				{
					sqlCollation = (this._collation = new SqlCollation());
				}
				if ((value & (SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreNonSpace | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth | SqlCompareOptions.BinarySort | SqlCompareOptions.BinarySort2)) != value)
				{
					throw ADP.ArgumentOutOfRange("CompareInfo");
				}
				sqlCollation.SqlCompareOptions = value;
			}
		}

		public string XmlSchemaCollectionDatabase
		{
			get
			{
				string xmlSchemaCollectionDatabase = this._xmlSchemaCollectionDatabase;
				if (xmlSchemaCollectionDatabase == null)
				{
					return ADP.StrEmpty;
				}
				return xmlSchemaCollectionDatabase;
			}
			set
			{
				this._xmlSchemaCollectionDatabase = value;
			}
		}

		public string XmlSchemaCollectionOwningSchema
		{
			get
			{
				string xmlSchemaCollectionOwningSchema = this._xmlSchemaCollectionOwningSchema;
				if (xmlSchemaCollectionOwningSchema == null)
				{
					return ADP.StrEmpty;
				}
				return xmlSchemaCollectionOwningSchema;
			}
			set
			{
				this._xmlSchemaCollectionOwningSchema = value;
			}
		}

		public string XmlSchemaCollectionName
		{
			get
			{
				string xmlSchemaCollectionName = this._xmlSchemaCollectionName;
				if (xmlSchemaCollectionName == null)
				{
					return ADP.StrEmpty;
				}
				return xmlSchemaCollectionName;
			}
			set
			{
				this._xmlSchemaCollectionName = value;
			}
		}

		public override DbType DbType
		{
			get
			{
				return this.GetMetaTypeOnly().DbType;
			}
			set
			{
				MetaType metaType = this._metaType;
				if (metaType == null || metaType.DbType != value || value == DbType.Date || value == DbType.Time)
				{
					this.PropertyTypeChanging();
					this._metaType = MetaType.GetMetaTypeFromDbType(value);
				}
			}
		}

		public override void ResetDbType()
		{
			this.ResetSqlDbType();
		}

		internal MetaType InternalMetaType
		{
			get
			{
				return this._internalMetaType;
			}
			set
			{
				this._internalMetaType = value;
			}
		}

		public int LocaleId
		{
			get
			{
				SqlCollation collation = this._collation;
				if (collation != null)
				{
					return collation.LCID;
				}
				return 0;
			}
			set
			{
				SqlCollation sqlCollation = this._collation;
				if (sqlCollation == null)
				{
					sqlCollation = (this._collation = new SqlCollation());
				}
				if ((long)value != (1048575L & (long)value))
				{
					throw ADP.ArgumentOutOfRange("LocaleId");
				}
				sqlCollation.LCID = value;
			}
		}

		internal bool SizeInferred
		{
			get
			{
				return this._size == 0;
			}
		}

		internal SmiParameterMetaData MetaDataForSmi(out ParameterPeekAheadValue peekAhead)
		{
			peekAhead = null;
			MetaType metaType = this.ValidateTypeLengths();
			long num = (long)this.GetActualSize();
			long num2 = (long)this.Size;
			if (!metaType.IsLong)
			{
				if (SqlDbType.NChar == metaType.SqlDbType || SqlDbType.NVarChar == metaType.SqlDbType)
				{
					num /= 2L;
				}
				if (num > num2)
				{
					num2 = num;
				}
			}
			if (num2 == 0L)
			{
				if (SqlDbType.Binary == metaType.SqlDbType || SqlDbType.VarBinary == metaType.SqlDbType)
				{
					num2 = 8000L;
				}
				else if (SqlDbType.Char == metaType.SqlDbType || SqlDbType.VarChar == metaType.SqlDbType)
				{
					num2 = 8000L;
				}
				else if (SqlDbType.NChar == metaType.SqlDbType || SqlDbType.NVarChar == metaType.SqlDbType)
				{
					num2 = 4000L;
				}
			}
			else if ((num2 > 8000L && (SqlDbType.Binary == metaType.SqlDbType || SqlDbType.VarBinary == metaType.SqlDbType)) || (num2 > 8000L && (SqlDbType.Char == metaType.SqlDbType || SqlDbType.VarChar == metaType.SqlDbType)) || (num2 > 4000L && (SqlDbType.NChar == metaType.SqlDbType || SqlDbType.NVarChar == metaType.SqlDbType)))
			{
				num2 = -1L;
			}
			int num3 = this.LocaleId;
			if (num3 == 0 && metaType.IsCharType)
			{
				object coercedValue = this.GetCoercedValue();
				if (coercedValue is SqlString && !((SqlString)coercedValue).IsNull)
				{
					num3 = ((SqlString)coercedValue).LCID;
				}
				else
				{
					num3 = CultureInfo.CurrentCulture.LCID;
				}
			}
			SqlCompareOptions sqlCompareOptions = this.CompareInfo;
			if (sqlCompareOptions == SqlCompareOptions.None && metaType.IsCharType)
			{
				object coercedValue2 = this.GetCoercedValue();
				if (coercedValue2 is SqlString && !((SqlString)coercedValue2).IsNull)
				{
					sqlCompareOptions = ((SqlString)coercedValue2).SqlCompareOptions;
				}
				else
				{
					sqlCompareOptions = SmiMetaData.GetDefaultForType(metaType.SqlDbType).CompareOptions;
				}
			}
			string text = null;
			string text2 = null;
			string text3 = null;
			if (SqlDbType.Xml == metaType.SqlDbType)
			{
				text = this.XmlSchemaCollectionDatabase;
				text2 = this.XmlSchemaCollectionOwningSchema;
				text3 = this.XmlSchemaCollectionName;
			}
			else if (SqlDbType.Udt == metaType.SqlDbType || (SqlDbType.Structured == metaType.SqlDbType && !string.IsNullOrEmpty(this.TypeName)))
			{
				if (SqlDbType.Udt == metaType.SqlDbType)
				{
					throw ADP.DbTypeNotSupported(SqlDbType.Udt.ToString());
				}
				string[] array = SqlParameter.ParseTypeName(this.TypeName);
				if (1 == array.Length)
				{
					text3 = array[0];
				}
				else if (2 == array.Length)
				{
					text2 = array[0];
					text3 = array[1];
				}
				else
				{
					if (3 != array.Length)
					{
						throw ADP.ArgumentOutOfRange("names");
					}
					text = array[0];
					text2 = array[1];
					text3 = array[2];
				}
				if ((!string.IsNullOrEmpty(text) && 255 < text.Length) || (!string.IsNullOrEmpty(text2) && 255 < text2.Length) || (!string.IsNullOrEmpty(text3) && 255 < text3.Length))
				{
					throw ADP.ArgumentOutOfRange("names");
				}
			}
			byte b = this.GetActualPrecision();
			byte actualScale = this.GetActualScale();
			if (SqlDbType.Decimal == metaType.SqlDbType && b == 0)
			{
				b = 29;
			}
			List<SmiExtendedMetaData> list = null;
			SmiMetaDataPropertyCollection smiMetaDataPropertyCollection = null;
			if (SqlDbType.Structured == metaType.SqlDbType)
			{
				this.GetActualFieldsAndProperties(out list, out smiMetaDataPropertyCollection, out peekAhead);
			}
			return new SmiParameterMetaData(metaType.SqlDbType, num2, b, actualScale, (long)num3, sqlCompareOptions, SqlDbType.Structured == metaType.SqlDbType, list, smiMetaDataPropertyCollection, this.ParameterNameFixed, text, text2, text3, this.Direction);
		}

		internal bool ParameterIsSqlType
		{
			get
			{
				return this._isSqlParameterSqlType;
			}
			set
			{
				this._isSqlParameterSqlType = value;
			}
		}

		public override string ParameterName
		{
			get
			{
				string parameterName = this._parameterName;
				if (parameterName == null)
				{
					return ADP.StrEmpty;
				}
				return parameterName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && value.Length >= 128 && ('@' != value[0] || value.Length > 128))
				{
					throw SQL.InvalidParameterNameLength(value);
				}
				if (this._parameterName != value)
				{
					this.PropertyChanging();
					this._parameterName = value;
					return;
				}
			}
		}

		internal string ParameterNameFixed
		{
			get
			{
				string text = this.ParameterName;
				if (0 < text.Length && '@' != text[0])
				{
					text = "@" + text;
				}
				return text;
			}
		}

		public override byte Precision
		{
			get
			{
				return this.PrecisionInternal;
			}
			set
			{
				this.PrecisionInternal = value;
			}
		}

		internal byte PrecisionInternal
		{
			get
			{
				byte b = this._precision;
				SqlDbType metaSqlDbTypeOnly = this.GetMetaSqlDbTypeOnly();
				if (b == 0 && SqlDbType.Decimal == metaSqlDbTypeOnly)
				{
					b = this.ValuePrecision(this.SqlValue);
				}
				return b;
			}
			set
			{
				if (this.SqlDbType == SqlDbType.Decimal && value > 38)
				{
					throw SQL.PrecisionValueOutOfRange(value);
				}
				if (this._precision != value)
				{
					this.PropertyChanging();
					this._precision = value;
				}
			}
		}

		private bool ShouldSerializePrecision()
		{
			return this._precision > 0;
		}

		public override byte Scale
		{
			get
			{
				return this.ScaleInternal;
			}
			set
			{
				this.ScaleInternal = value;
			}
		}

		internal byte ScaleInternal
		{
			get
			{
				byte b = this._scale;
				SqlDbType metaSqlDbTypeOnly = this.GetMetaSqlDbTypeOnly();
				if (b == 0 && SqlDbType.Decimal == metaSqlDbTypeOnly)
				{
					b = this.ValueScale(this.SqlValue);
				}
				return b;
			}
			set
			{
				if (this._scale != value || !this._hasScale)
				{
					this.PropertyChanging();
					this._scale = value;
					this._hasScale = true;
					this._actualSize = -1;
				}
			}
		}

		private bool ShouldSerializeScale()
		{
			return this._scale > 0;
		}

		public SqlDbType SqlDbType
		{
			get
			{
				return this.GetMetaTypeOnly().SqlDbType;
			}
			set
			{
				MetaType metaType = this._metaType;
				if ((SqlDbType)24 == value)
				{
					throw SQL.InvalidSqlDbType(value);
				}
				if (metaType == null || metaType.SqlDbType != value)
				{
					this.PropertyTypeChanging();
					this._metaType = MetaType.GetMetaTypeFromSqlDbType(value, value == SqlDbType.Structured);
				}
			}
		}

		private bool ShouldSerializeSqlDbType()
		{
			return this._metaType != null;
		}

		public void ResetSqlDbType()
		{
			if (this._metaType != null)
			{
				this.PropertyTypeChanging();
				this._metaType = null;
			}
		}

		public object SqlValue
		{
			get
			{
				if (this._value != null)
				{
					if (this._value == DBNull.Value)
					{
						return MetaType.GetNullSqlValue(this.GetMetaTypeOnly().SqlType);
					}
					if (this._value is INullable)
					{
						return this._value;
					}
					if (this._value is DateTime)
					{
						SqlDbType sqlDbType = this.GetMetaTypeOnly().SqlDbType;
						if (sqlDbType == SqlDbType.Date || sqlDbType == SqlDbType.DateTime2)
						{
							return this._value;
						}
					}
					return MetaType.GetSqlValueFromComVariant(this._value);
				}
				else
				{
					if (this._sqlBufferReturnValue != null)
					{
						return this._sqlBufferReturnValue.SqlValue;
					}
					return null;
				}
			}
			set
			{
				this.Value = value;
			}
		}

		public string TypeName
		{
			get
			{
				string typeName = this._typeName;
				if (typeName == null)
				{
					return ADP.StrEmpty;
				}
				return typeName;
			}
			set
			{
				this._typeName = value;
			}
		}

		public override object Value
		{
			get
			{
				if (this._value != null)
				{
					return this._value;
				}
				if (this._sqlBufferReturnValue == null)
				{
					return null;
				}
				if (this.ParameterIsSqlType)
				{
					return this._sqlBufferReturnValue.SqlValue;
				}
				return this._sqlBufferReturnValue.Value;
			}
			set
			{
				this._value = value;
				this._sqlBufferReturnValue = null;
				this._coercedValue = null;
				this._valueAsINullable = this._value as INullable;
				this._isSqlParameterSqlType = this._valueAsINullable != null;
				this._isNull = this._value == null || this._value == DBNull.Value || (this._isSqlParameterSqlType && this._valueAsINullable.IsNull);
				this._actualSize = -1;
			}
		}

		internal INullable ValueAsINullable
		{
			get
			{
				return this._valueAsINullable;
			}
		}

		internal bool IsNull
		{
			get
			{
				if (this._internalMetaType.SqlDbType == SqlDbType.Udt)
				{
					this._isNull = this._value == null || this._value == DBNull.Value || (this._isSqlParameterSqlType && this._valueAsINullable.IsNull);
				}
				return this._isNull;
			}
		}

		internal int GetActualSize()
		{
			MetaType metaType = this.InternalMetaType;
			SqlDbType sqlDbType = metaType.SqlDbType;
			if (this._actualSize == -1 || sqlDbType == SqlDbType.Udt)
			{
				this._actualSize = 0;
				object coercedValue = this.GetCoercedValue();
				bool flag = false;
				if (this.IsNull && !metaType.IsVarTime)
				{
					return 0;
				}
				if (sqlDbType == SqlDbType.Variant)
				{
					metaType = MetaType.GetMetaTypeFromValue(coercedValue, true, false);
					sqlDbType = MetaType.GetSqlDataType((int)metaType.TDSType, 0U, 0).SqlDbType;
					flag = true;
				}
				if (metaType.IsFixed)
				{
					this._actualSize = metaType.FixedLength;
				}
				else
				{
					int num = 0;
					if (sqlDbType <= SqlDbType.Char)
					{
						if (sqlDbType == SqlDbType.Binary)
						{
							goto IL_01E8;
						}
						if (sqlDbType != SqlDbType.Char)
						{
							goto IL_02BE;
						}
					}
					else
					{
						if (sqlDbType != SqlDbType.Image)
						{
							if (sqlDbType - SqlDbType.NChar > 2)
							{
								switch (sqlDbType)
								{
								case SqlDbType.Text:
								case SqlDbType.VarChar:
									goto IL_0175;
								case SqlDbType.Timestamp:
								case SqlDbType.VarBinary:
									goto IL_01E8;
								case SqlDbType.TinyInt:
								case SqlDbType.Variant:
								case (SqlDbType)24:
								case (SqlDbType)26:
								case (SqlDbType)27:
								case (SqlDbType)28:
								case SqlDbType.Date:
									goto IL_02BE;
								case SqlDbType.Xml:
									break;
								case SqlDbType.Udt:
									throw ADP.DbTypeNotSupported(SqlDbType.Udt.ToString());
								case SqlDbType.Structured:
									num = -1;
									goto IL_02BE;
								case SqlDbType.Time:
									this._actualSize = (flag ? 5 : MetaType.GetTimeSizeFromScale(this.GetActualScale()));
									goto IL_02BE;
								case SqlDbType.DateTime2:
									this._actualSize = 3 + (flag ? 5 : MetaType.GetTimeSizeFromScale(this.GetActualScale()));
									goto IL_02BE;
								case SqlDbType.DateTimeOffset:
									this._actualSize = 5 + (flag ? 5 : MetaType.GetTimeSizeFromScale(this.GetActualScale()));
									goto IL_02BE;
								default:
									goto IL_02BE;
								}
							}
							num = ((!this._isNull && !this._coercedValueIsDataFeed) ? SqlParameter.StringSize(coercedValue, this._coercedValueIsSqlType) : 0);
							this._actualSize = (this.ShouldSerializeSize() ? this.Size : 0);
							this._actualSize = ((this.ShouldSerializeSize() && this._actualSize <= num) ? this._actualSize : num);
							if (this._actualSize == -1)
							{
								this._actualSize = num;
							}
							this._actualSize <<= 1;
							goto IL_02BE;
						}
						goto IL_01E8;
					}
					IL_0175:
					num = ((!this._isNull && !this._coercedValueIsDataFeed) ? SqlParameter.StringSize(coercedValue, this._coercedValueIsSqlType) : 0);
					this._actualSize = (this.ShouldSerializeSize() ? this.Size : 0);
					this._actualSize = ((this.ShouldSerializeSize() && this._actualSize <= num) ? this._actualSize : num);
					if (this._actualSize == -1)
					{
						this._actualSize = num;
						goto IL_02BE;
					}
					goto IL_02BE;
					IL_01E8:
					num = ((!this._isNull && !this._coercedValueIsDataFeed) ? SqlParameter.BinarySize(coercedValue, this._coercedValueIsSqlType) : 0);
					this._actualSize = (this.ShouldSerializeSize() ? this.Size : 0);
					this._actualSize = ((this.ShouldSerializeSize() && this._actualSize <= num) ? this._actualSize : num);
					if (this._actualSize == -1)
					{
						this._actualSize = num;
					}
					IL_02BE:
					if (flag && num > 8000)
					{
						throw SQL.ParameterInvalidVariant(this.ParameterName);
					}
				}
			}
			return this._actualSize;
		}

		internal static object CoerceValue(object value, MetaType destinationType, out bool coercedToDataFeed, out bool typeChanged, bool allowStreaming = true)
		{
			coercedToDataFeed = false;
			typeChanged = false;
			Type type = value.GetType();
			if (typeof(object) != destinationType.ClassType && type != destinationType.ClassType && (type != destinationType.SqlType || SqlDbType.Xml == destinationType.SqlDbType))
			{
				try
				{
					typeChanged = true;
					if (typeof(string) == destinationType.ClassType)
					{
						if (typeof(SqlXml) == type)
						{
							value = MetaType.GetStringFromXml(((SqlXml)value).CreateReader());
						}
						else if (typeof(SqlString) == type)
						{
							typeChanged = false;
						}
						else if (typeof(XmlReader).IsAssignableFrom(type))
						{
							if (allowStreaming)
							{
								coercedToDataFeed = true;
								value = new XmlDataFeed((XmlReader)value);
							}
							else
							{
								value = MetaType.GetStringFromXml((XmlReader)value);
							}
						}
						else if (typeof(char[]) == type)
						{
							value = new string((char[])value);
						}
						else if (typeof(SqlChars) == type)
						{
							value = new string(((SqlChars)value).Value);
						}
						else if (value is TextReader && allowStreaming)
						{
							coercedToDataFeed = true;
							value = new TextDataFeed((TextReader)value);
						}
						else
						{
							value = Convert.ChangeType(value, destinationType.ClassType, null);
						}
					}
					else if (DbType.Currency == destinationType.DbType && typeof(string) == type)
					{
						value = decimal.Parse((string)value, NumberStyles.Currency, null);
					}
					else if (typeof(SqlBytes) == type && typeof(byte[]) == destinationType.ClassType)
					{
						typeChanged = false;
					}
					else if (typeof(string) == type && SqlDbType.Time == destinationType.SqlDbType)
					{
						value = TimeSpan.Parse((string)value);
					}
					else if (typeof(string) == type && SqlDbType.DateTimeOffset == destinationType.SqlDbType)
					{
						value = DateTimeOffset.Parse((string)value, null);
					}
					else if (typeof(DateTime) == type && SqlDbType.DateTimeOffset == destinationType.SqlDbType)
					{
						value = new DateTimeOffset((DateTime)value);
					}
					else if (243 == destinationType.TDSType && (value is DataTable || value is DbDataReader || value is IEnumerable<SqlDataRecord>))
					{
						typeChanged = false;
					}
					else if (destinationType.ClassType == typeof(byte[]) && value is Stream && allowStreaming)
					{
						coercedToDataFeed = true;
						value = new StreamDataFeed((Stream)value);
					}
					else
					{
						value = Convert.ChangeType(value, destinationType.ClassType, null);
					}
				}
				catch (Exception ex)
				{
					if (!ADP.IsCatchableExceptionType(ex))
					{
						throw;
					}
					throw ADP.ParameterConversionFailed(value, destinationType.ClassType, ex);
				}
			}
			return value;
		}

		internal void FixStreamDataForNonPLP()
		{
			object coercedValue = this.GetCoercedValue();
			if (!this._coercedValueIsDataFeed)
			{
				return;
			}
			this._coercedValueIsDataFeed = false;
			if (coercedValue is TextDataFeed)
			{
				if (this.Size > 0)
				{
					char[] array = new char[this.Size];
					int num = ((TextDataFeed)coercedValue)._source.ReadBlock(array, 0, this.Size);
					this.CoercedValue = new string(array, 0, num);
					return;
				}
				this.CoercedValue = ((TextDataFeed)coercedValue)._source.ReadToEnd();
				return;
			}
			else if (coercedValue is StreamDataFeed)
			{
				if (this.Size > 0)
				{
					byte[] array2 = new byte[this.Size];
					int i = 0;
					Stream source = ((StreamDataFeed)coercedValue)._source;
					while (i < this.Size)
					{
						int num2 = source.Read(array2, i, this.Size - i);
						if (num2 == 0)
						{
							break;
						}
						i += num2;
					}
					if (i < this.Size)
					{
						Array.Resize<byte>(ref array2, i);
					}
					this.CoercedValue = array2;
					return;
				}
				MemoryStream memoryStream = new MemoryStream();
				((StreamDataFeed)coercedValue)._source.CopyTo(memoryStream);
				this.CoercedValue = memoryStream.ToArray();
				return;
			}
			else
			{
				if (coercedValue is XmlDataFeed)
				{
					this.CoercedValue = MetaType.GetStringFromXml(((XmlDataFeed)coercedValue)._source);
					return;
				}
				return;
			}
		}

		internal byte GetActualPrecision()
		{
			if (!this.ShouldSerializePrecision())
			{
				return this.ValuePrecision(this.CoercedValue);
			}
			return this.PrecisionInternal;
		}

		internal byte GetActualScale()
		{
			if (this.ShouldSerializeScale())
			{
				return this.ScaleInternal;
			}
			if (this.GetMetaTypeOnly().IsVarTime)
			{
				return 7;
			}
			return this.ValueScale(this.CoercedValue);
		}

		internal int GetParameterSize()
		{
			if (!this.ShouldSerializeSize())
			{
				return this.ValueSize(this.CoercedValue);
			}
			return this.Size;
		}

		private void GetActualFieldsAndProperties(out List<SmiExtendedMetaData> fields, out SmiMetaDataPropertyCollection props, out ParameterPeekAheadValue peekAhead)
		{
			fields = null;
			props = null;
			peekAhead = null;
			object coercedValue = this.GetCoercedValue();
			DataTable dataTable;
			if ((dataTable = coercedValue as DataTable) != null)
			{
				if (dataTable.Columns.Count <= 0)
				{
					throw SQL.NotEnoughColumnsInStructuredType();
				}
				fields = new List<SmiExtendedMetaData>(dataTable.Columns.Count);
				bool[] array = new bool[dataTable.Columns.Count];
				bool flag = false;
				if (dataTable.PrimaryKey != null && dataTable.PrimaryKey.Length != 0)
				{
					foreach (DataColumn dataColumn in dataTable.PrimaryKey)
					{
						array[dataColumn.Ordinal] = true;
						flag = true;
					}
				}
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					fields.Add(MetaDataUtilsSmi.SmiMetaDataFromDataColumn(dataTable.Columns[j], dataTable));
					if (!flag && dataTable.Columns[j].Unique)
					{
						array[j] = true;
						flag = true;
					}
				}
				if (flag)
				{
					props = new SmiMetaDataPropertyCollection();
					props[SmiPropertySelector.UniqueKey] = new SmiUniqueKeyProperty(new List<bool>(array));
					return;
				}
			}
			else if (coercedValue is SqlDataReader)
			{
				fields = new List<SmiExtendedMetaData>(((SqlDataReader)coercedValue).GetInternalSmiMetaData());
				if (fields.Count <= 0)
				{
					throw SQL.NotEnoughColumnsInStructuredType();
				}
				bool[] array2 = new bool[fields.Count];
				bool flag2 = false;
				for (int k = 0; k < fields.Count; k++)
				{
					SmiQueryMetaData smiQueryMetaData = fields[k] as SmiQueryMetaData;
					if (smiQueryMetaData != null && !smiQueryMetaData.IsKey.IsNull && smiQueryMetaData.IsKey.Value)
					{
						array2[k] = true;
						flag2 = true;
					}
				}
				if (flag2)
				{
					props = new SmiMetaDataPropertyCollection();
					props[SmiPropertySelector.UniqueKey] = new SmiUniqueKeyProperty(new List<bool>(array2));
					return;
				}
			}
			else
			{
				if (coercedValue is IEnumerable<SqlDataRecord>)
				{
					IEnumerator<SqlDataRecord> enumerator = ((IEnumerable<SqlDataRecord>)coercedValue).GetEnumerator();
					try
					{
						if (!enumerator.MoveNext())
						{
							throw SQL.IEnumerableOfSqlDataRecordHasNoRows();
						}
						SqlDataRecord sqlDataRecord = enumerator.Current;
						int fieldCount = sqlDataRecord.FieldCount;
						if (0 < fieldCount)
						{
							bool[] array3 = new bool[fieldCount];
							bool[] array4 = new bool[fieldCount];
							bool[] array5 = new bool[fieldCount];
							int num = -1;
							bool flag3 = false;
							bool flag4 = false;
							int num2 = 0;
							SmiOrderProperty.SmiColumnOrder[] array6 = new SmiOrderProperty.SmiColumnOrder[fieldCount];
							fields = new List<SmiExtendedMetaData>(fieldCount);
							for (int l = 0; l < fieldCount; l++)
							{
								SqlMetaData sqlMetaData = sqlDataRecord.GetSqlMetaData(l);
								fields.Add(MetaDataUtilsSmi.SqlMetaDataToSmiExtendedMetaData(sqlMetaData));
								if (sqlMetaData.IsUniqueKey)
								{
									array3[l] = true;
									flag3 = true;
								}
								if (sqlMetaData.UseServerDefault)
								{
									array4[l] = true;
									flag4 = true;
								}
								array6[l].Order = sqlMetaData.SortOrder;
								if (SortOrder.Unspecified != sqlMetaData.SortOrder)
								{
									if (fieldCount <= sqlMetaData.SortOrdinal)
									{
										throw SQL.SortOrdinalGreaterThanFieldCount(l, sqlMetaData.SortOrdinal);
									}
									if (array5[sqlMetaData.SortOrdinal])
									{
										throw SQL.DuplicateSortOrdinal(sqlMetaData.SortOrdinal);
									}
									array6[l].SortOrdinal = sqlMetaData.SortOrdinal;
									array5[sqlMetaData.SortOrdinal] = true;
									if (sqlMetaData.SortOrdinal > num)
									{
										num = sqlMetaData.SortOrdinal;
									}
									num2++;
								}
							}
							if (flag3)
							{
								props = new SmiMetaDataPropertyCollection();
								props[SmiPropertySelector.UniqueKey] = new SmiUniqueKeyProperty(new List<bool>(array3));
							}
							if (flag4)
							{
								if (props == null)
								{
									props = new SmiMetaDataPropertyCollection();
								}
								props[SmiPropertySelector.DefaultFields] = new SmiDefaultFieldsProperty(new List<bool>(array4));
							}
							if (0 < num2)
							{
								if (num >= num2)
								{
									int num3 = 0;
									while (num3 < num2 && array5[num3])
									{
										num3++;
									}
									throw SQL.MissingSortOrdinal(num3);
								}
								if (props == null)
								{
									props = new SmiMetaDataPropertyCollection();
								}
								props[SmiPropertySelector.SortOrder] = new SmiOrderProperty(new List<SmiOrderProperty.SmiColumnOrder>(array6));
							}
							peekAhead = new ParameterPeekAheadValue();
							peekAhead.Enumerator = enumerator;
							peekAhead.FirstRecord = sqlDataRecord;
							enumerator = null;
							return;
						}
						throw SQL.NotEnoughColumnsInStructuredType();
					}
					finally
					{
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
				}
				if (coercedValue is DbDataReader)
				{
					DataTable schemaTable = ((DbDataReader)coercedValue).GetSchemaTable();
					if (schemaTable.Rows.Count <= 0)
					{
						throw SQL.NotEnoughColumnsInStructuredType();
					}
					int count = schemaTable.Rows.Count;
					fields = new List<SmiExtendedMetaData>(count);
					bool[] array7 = new bool[count];
					bool flag5 = false;
					int ordinal = schemaTable.Columns[SchemaTableColumn.IsKey].Ordinal;
					int ordinal2 = schemaTable.Columns[SchemaTableColumn.ColumnOrdinal].Ordinal;
					for (int m = 0; m < count; m++)
					{
						DataRow dataRow = schemaTable.Rows[m];
						SmiExtendedMetaData smiExtendedMetaData = MetaDataUtilsSmi.SmiMetaDataFromSchemaTableRow(dataRow);
						int n = m;
						if (!dataRow.IsNull(ordinal2))
						{
							n = (int)dataRow[ordinal2];
						}
						if (n >= count || n < 0)
						{
							throw SQL.InvalidSchemaTableOrdinals();
						}
						while (n > fields.Count)
						{
							fields.Add(null);
						}
						if (fields.Count == n)
						{
							fields.Add(smiExtendedMetaData);
						}
						else
						{
							if (fields[n] != null)
							{
								throw SQL.InvalidSchemaTableOrdinals();
							}
							fields[n] = smiExtendedMetaData;
						}
						if (!dataRow.IsNull(ordinal) && (bool)dataRow[ordinal])
						{
							array7[n] = true;
							flag5 = true;
						}
					}
					if (flag5)
					{
						props = new SmiMetaDataPropertyCollection();
						props[SmiPropertySelector.UniqueKey] = new SmiUniqueKeyProperty(new List<bool>(array7));
					}
				}
			}
		}

		internal object GetCoercedValue()
		{
			if (this._coercedValue == null || this._internalMetaType.SqlDbType == SqlDbType.Udt)
			{
				bool flag = this.Value is DataFeed;
				if (this.IsNull || flag)
				{
					this._coercedValue = this.Value;
					this._coercedValueIsSqlType = this._coercedValue != null && this._isSqlParameterSqlType;
					this._coercedValueIsDataFeed = flag;
					this._actualSize = (this.IsNull ? 0 : (-1));
				}
				else
				{
					bool flag2;
					this._coercedValue = SqlParameter.CoerceValue(this.Value, this._internalMetaType, out this._coercedValueIsDataFeed, out flag2, true);
					this._coercedValueIsSqlType = this._isSqlParameterSqlType && !flag2;
					this._actualSize = -1;
				}
			}
			return this._coercedValue;
		}

		internal bool CoercedValueIsSqlType
		{
			get
			{
				if (this._coercedValue == null)
				{
					this.GetCoercedValue();
				}
				return this._coercedValueIsSqlType;
			}
		}

		internal bool CoercedValueIsDataFeed
		{
			get
			{
				if (this._coercedValue == null)
				{
					this.GetCoercedValue();
				}
				return this._coercedValueIsDataFeed;
			}
		}

		[Conditional("DEBUG")]
		internal void AssertCachedPropertiesAreValid()
		{
		}

		[Conditional("DEBUG")]
		internal void AssertPropertiesAreValid(object value, bool? isSqlType = null, bool? isDataFeed = null, bool? isNull = null)
		{
		}

		private SqlDbType GetMetaSqlDbTypeOnly()
		{
			MetaType metaType = this._metaType;
			if (metaType == null)
			{
				metaType = MetaType.GetDefaultMetaType();
			}
			return metaType.SqlDbType;
		}

		private MetaType GetMetaTypeOnly()
		{
			if (this._metaType != null)
			{
				return this._metaType;
			}
			if (this._value != null && DBNull.Value != this._value)
			{
				if (this._value is char)
				{
					this._value = this._value.ToString();
				}
				else if (this.Value is char[])
				{
					this._value = new string((char[])this._value);
				}
				return MetaType.GetMetaTypeFromValue(this._value, false, true);
			}
			if (this._sqlBufferReturnValue != null)
			{
				Type typeFromStorageType = this._sqlBufferReturnValue.GetTypeFromStorageType(this._isSqlParameterSqlType);
				if (null != typeFromStorageType)
				{
					return MetaType.GetMetaTypeFromType(typeFromStorageType, true);
				}
			}
			return MetaType.GetDefaultMetaType();
		}

		internal void Prepare(SqlCommand cmd)
		{
			if (this._metaType == null)
			{
				throw ADP.PrepareParameterType(cmd);
			}
			if (!this.ShouldSerializeSize() && !this._metaType.IsFixed)
			{
				throw ADP.PrepareParameterSize(cmd);
			}
			if (!this.ShouldSerializePrecision() && !this.ShouldSerializeScale() && this._metaType.SqlDbType == SqlDbType.Decimal)
			{
				throw ADP.PrepareParameterScale(cmd, this.SqlDbType.ToString());
			}
		}

		private void PropertyChanging()
		{
			this._internalMetaType = null;
		}

		private void PropertyTypeChanging()
		{
			this.PropertyChanging();
			this.CoercedValue = null;
		}

		internal void SetSqlBuffer(SqlBuffer buff)
		{
			this._sqlBufferReturnValue = buff;
			this._value = null;
			this._coercedValue = null;
			this._isNull = this._sqlBufferReturnValue.IsNull;
			this._coercedValueIsDataFeed = false;
			this._coercedValueIsSqlType = false;
			this._actualSize = -1;
		}

		internal void Validate(int index, bool isCommandProc)
		{
			MetaType metaTypeOnly = this.GetMetaTypeOnly();
			this._internalMetaType = metaTypeOnly;
			if (ADP.IsDirection(this, ParameterDirection.Output) && !ADP.IsDirection(this, ParameterDirection.ReturnValue) && !metaTypeOnly.IsFixed && !this.ShouldSerializeSize() && (this._value == null || this._value == DBNull.Value) && this.SqlDbType != SqlDbType.Timestamp && this.SqlDbType != SqlDbType.Udt && this.SqlDbType != SqlDbType.Xml && !metaTypeOnly.IsVarTime)
			{
				throw ADP.UninitializedParameterSize(index, metaTypeOnly.ClassType);
			}
			if (metaTypeOnly.SqlDbType != SqlDbType.Udt && this.Direction != ParameterDirection.Output)
			{
				this.GetCoercedValue();
			}
			if (metaTypeOnly.SqlDbType == SqlDbType.Structured)
			{
				if (!isCommandProc && string.IsNullOrEmpty(this.TypeName))
				{
					throw SQL.MustSetTypeNameForParam(metaTypeOnly.TypeName, this.ParameterName);
				}
				if (ParameterDirection.Input != this.Direction)
				{
					throw SQL.UnsupportedTVPOutputParameter(this.Direction, this.ParameterName);
				}
				if (DBNull.Value == this.GetCoercedValue())
				{
					throw SQL.DBNullNotSupportedForTVPValues(this.ParameterName);
				}
			}
			else if (!string.IsNullOrEmpty(this.TypeName))
			{
				throw SQL.UnexpectedTypeNameForNonStructParams(this.ParameterName);
			}
		}

		internal MetaType ValidateTypeLengths()
		{
			MetaType metaType = this.InternalMetaType;
			if (SqlDbType.Udt != metaType.SqlDbType && !metaType.IsFixed && !metaType.IsLong)
			{
				long num = (long)this.GetActualSize();
				long num2 = (long)this.Size;
				long num3;
				if (metaType.IsNCharType)
				{
					num3 = ((num2 * 2L > num) ? (num2 * 2L) : num);
				}
				else
				{
					num3 = ((num2 > num) ? num2 : num);
				}
				if (num3 > 8000L || this._coercedValueIsDataFeed || num2 == -1L || num == -1L)
				{
					metaType = MetaType.GetMaxMetaTypeFromMetaType(metaType);
					this._metaType = metaType;
					this.InternalMetaType = metaType;
					if (!metaType.IsPlp)
					{
						if (metaType.SqlDbType == SqlDbType.Xml)
						{
							throw ADP.InvalidMetaDataValue();
						}
						if (metaType.SqlDbType == SqlDbType.NVarChar || metaType.SqlDbType == SqlDbType.VarChar || metaType.SqlDbType == SqlDbType.VarBinary)
						{
							this.Size = -1;
						}
					}
				}
			}
			return metaType;
		}

		private byte ValuePrecision(object value)
		{
			if (!(value is SqlDecimal))
			{
				return this.ValuePrecisionCore(value);
			}
			if (((SqlDecimal)value).IsNull)
			{
				return 0;
			}
			return ((SqlDecimal)value).Precision;
		}

		private byte ValueScale(object value)
		{
			if (!(value is SqlDecimal))
			{
				return this.ValueScaleCore(value);
			}
			if (((SqlDecimal)value).IsNull)
			{
				return 0;
			}
			return ((SqlDecimal)value).Scale;
		}

		private static int StringSize(object value, bool isSqlType)
		{
			if (isSqlType)
			{
				if (value is SqlString)
				{
					return ((SqlString)value).Value.Length;
				}
				if (value is SqlChars)
				{
					return ((SqlChars)value).Value.Length;
				}
			}
			else
			{
				string text = value as string;
				if (text != null)
				{
					return text.Length;
				}
				char[] array = value as char[];
				if (array != null)
				{
					return array.Length;
				}
				if (value is char)
				{
					return 1;
				}
			}
			return 0;
		}

		private static int BinarySize(object value, bool isSqlType)
		{
			if (isSqlType)
			{
				if (value is SqlBinary)
				{
					return ((SqlBinary)value).Length;
				}
				if (value is SqlBytes)
				{
					return ((SqlBytes)value).Value.Length;
				}
			}
			else
			{
				byte[] array = value as byte[];
				if (array != null)
				{
					return array.Length;
				}
				if (value is byte)
				{
					return 1;
				}
			}
			return 0;
		}

		private int ValueSize(object value)
		{
			if (value is SqlString)
			{
				if (((SqlString)value).IsNull)
				{
					return 0;
				}
				return ((SqlString)value).Value.Length;
			}
			else if (value is SqlChars)
			{
				if (((SqlChars)value).IsNull)
				{
					return 0;
				}
				return ((SqlChars)value).Value.Length;
			}
			else if (value is SqlBinary)
			{
				if (((SqlBinary)value).IsNull)
				{
					return 0;
				}
				return ((SqlBinary)value).Length;
			}
			else if (value is SqlBytes)
			{
				if (((SqlBytes)value).IsNull)
				{
					return 0;
				}
				return (int)((SqlBytes)value).Length;
			}
			else
			{
				if (value is DataFeed)
				{
					return 0;
				}
				return this.ValueSizeCore(value);
			}
		}

		internal static string[] ParseTypeName(string typeName)
		{
			string[] array;
			try
			{
				string text = "SqlParameter.TypeName is an invalid multipart name";
				array = MultipartIdentifier.ParseMultipartIdentifier(typeName, "[\"", "]\"", '.', 3, true, text, true);
			}
			catch (ArgumentException)
			{
				throw SQL.InvalidParameterTypeNameFormat();
			}
			return array;
		}

		object ICloneable.Clone()
		{
			return new SqlParameter(this);
		}

		private void CloneHelper(SqlParameter destination)
		{
			this.CloneHelperCore(destination);
			destination._metaType = this._metaType;
			destination._collation = this._collation;
			destination._xmlSchemaCollectionDatabase = this._xmlSchemaCollectionDatabase;
			destination._xmlSchemaCollectionOwningSchema = this._xmlSchemaCollectionOwningSchema;
			destination._xmlSchemaCollectionName = this._xmlSchemaCollectionName;
			destination._typeName = this._typeName;
			destination._parameterName = this._parameterName;
			destination._precision = this._precision;
			destination._scale = this._scale;
			destination._sqlBufferReturnValue = this._sqlBufferReturnValue;
			destination._isSqlParameterSqlType = this._isSqlParameterSqlType;
			destination._internalMetaType = this._internalMetaType;
			destination.CoercedValue = this.CoercedValue;
			destination._valueAsINullable = this._valueAsINullable;
			destination._isNull = this._isNull;
			destination._coercedValueIsDataFeed = this._coercedValueIsDataFeed;
			destination._coercedValueIsSqlType = this._coercedValueIsSqlType;
			destination._actualSize = this._actualSize;
		}

		private void CloneHelperCore(SqlParameter destination)
		{
			destination._value = this._value;
			destination._direction = this._direction;
			destination._size = this._size;
			destination._offset = this._offset;
			destination._sourceColumn = this._sourceColumn;
			destination._sourceVersion = this._sourceVersion;
			destination._sourceColumnNullMapping = this._sourceColumnNullMapping;
			destination._isNullable = this._isNullable;
		}

		public override DataRowVersion SourceVersion
		{
			get
			{
				DataRowVersion sourceVersion = this._sourceVersion;
				if (sourceVersion == (DataRowVersion)0)
				{
					return DataRowVersion.Current;
				}
				return sourceVersion;
			}
			set
			{
				if (value <= DataRowVersion.Current)
				{
					if (value != DataRowVersion.Original && value != DataRowVersion.Current)
					{
						goto IL_0032;
					}
				}
				else if (value != DataRowVersion.Proposed && value != DataRowVersion.Default)
				{
					goto IL_0032;
				}
				this._sourceVersion = value;
				return;
				IL_0032:
				throw ADP.InvalidDataRowVersion(value);
			}
		}

		private object CoercedValue
		{
			get
			{
				return this._coercedValue;
			}
			set
			{
				this._coercedValue = value;
			}
		}

		public override ParameterDirection Direction
		{
			get
			{
				ParameterDirection direction = this._direction;
				if (direction == (ParameterDirection)0)
				{
					return ParameterDirection.Input;
				}
				return direction;
			}
			set
			{
				if (this._direction == value)
				{
					return;
				}
				if (value - ParameterDirection.Input <= 2 || value == ParameterDirection.ReturnValue)
				{
					this.PropertyChanging();
					this._direction = value;
					return;
				}
				throw ADP.InvalidParameterDirection(value);
			}
		}

		public override bool IsNullable
		{
			get
			{
				return this._isNullable;
			}
			set
			{
				this._isNullable = value;
			}
		}

		public int Offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				if (value < 0)
				{
					throw ADP.InvalidOffsetValue(value);
				}
				this._offset = value;
			}
		}

		public override int Size
		{
			get
			{
				int num = this._size;
				if (num == 0)
				{
					num = this.ValueSize(this.Value);
				}
				return num;
			}
			set
			{
				if (this._size != value)
				{
					if (value < -1)
					{
						throw ADP.InvalidSizeValue(value);
					}
					this.PropertyChanging();
					this._size = value;
				}
			}
		}

		private bool ShouldSerializeSize()
		{
			return this._size != 0;
		}

		public override string SourceColumn
		{
			get
			{
				string sourceColumn = this._sourceColumn;
				if (sourceColumn == null)
				{
					return ADP.StrEmpty;
				}
				return sourceColumn;
			}
			set
			{
				this._sourceColumn = value;
			}
		}

		public override bool SourceColumnNullMapping
		{
			get
			{
				return this._sourceColumnNullMapping;
			}
			set
			{
				this._sourceColumnNullMapping = value;
			}
		}

		internal object CompareExchangeParent(object value, object comparand)
		{
			object parent = this._parent;
			if (comparand == parent)
			{
				this._parent = value;
			}
			return parent;
		}

		internal void ResetParent()
		{
			this._parent = null;
		}

		public override string ToString()
		{
			return this.ParameterName;
		}

		private byte ValuePrecisionCore(object value)
		{
			if (value is decimal)
			{
				return ((decimal)value).Precision;
			}
			return 0;
		}

		private byte ValueScaleCore(object value)
		{
			if (value is decimal)
			{
				return (byte)((decimal.GetBits((decimal)value)[3] & 16711680) >> 16);
			}
			return 0;
		}

		private int ValueSizeCore(object value)
		{
			if (!ADP.IsNull(value))
			{
				string text = value as string;
				if (text != null)
				{
					return text.Length;
				}
				byte[] array = value as byte[];
				if (array != null)
				{
					return array.Length;
				}
				char[] array2 = value as char[];
				if (array2 != null)
				{
					return array2.Length;
				}
				if (value is byte || value is char)
				{
					return 1;
				}
			}
			return 0;
		}

		internal void CopyTo(SqlParameter destination)
		{
			ADP.CheckArgumentNull(destination, "destination");
			destination._value = this._value;
			destination._direction = this._direction;
			destination._size = this._size;
			destination._offset = this._offset;
			destination._sourceColumn = this._sourceColumn;
			destination._sourceVersion = this._sourceVersion;
			destination._sourceColumnNullMapping = this._sourceColumnNullMapping;
			destination._isNullable = this._isNullable;
		}

		[MonoTODO]
		public string UdtTypeName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool ForceColumnEncryption
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			[CompilerGenerated]
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		private MetaType _metaType;

		private SqlCollation _collation;

		private string _xmlSchemaCollectionDatabase;

		private string _xmlSchemaCollectionOwningSchema;

		private string _xmlSchemaCollectionName;

		private string _typeName;

		private string _parameterName;

		private byte _precision;

		private byte _scale;

		private bool _hasScale;

		private MetaType _internalMetaType;

		private SqlBuffer _sqlBufferReturnValue;

		private INullable _valueAsINullable;

		private bool _isSqlParameterSqlType;

		private bool _isNull = true;

		private bool _coercedValueIsSqlType;

		private bool _coercedValueIsDataFeed;

		private int _actualSize = -1;

		private DataRowVersion _sourceVersion;

		private object _value;

		private object _parent;

		private ParameterDirection _direction;

		private int _size;

		private int _offset;

		private string _sourceColumn;

		private bool _sourceColumnNullMapping;

		private bool _isNullable;

		private object _coercedValue;
	}
}
