using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb
{
	[TypeConverter("System.Data.OleDb.OleDbParameter+OleDbParameterConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class OleDbParameter : DbParameter, IDataParameter, IDbDataParameter, ICloneable
	{
		public OleDbParameter()
		{
			this.name = string.Empty;
			this.isNullable = true;
			this.sourceColumn = string.Empty;
			this.gdaParameter = IntPtr.Zero;
		}

		public OleDbParameter(string name, object value)
			: this()
		{
			this.name = name;
			this.value = value;
			this.OleDbType = this.GetOleDbType(value);
		}

		public OleDbParameter(string name, OleDbType dataType)
			: this()
		{
			this.name = name;
			this.OleDbType = dataType;
		}

		public OleDbParameter(string name, OleDbType dataType, int size)
			: this(name, dataType)
		{
			this.size = size;
		}

		public OleDbParameter(string name, OleDbType dataType, int size, string srcColumn)
			: this(name, dataType, size)
		{
			this.sourceColumn = srcColumn;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public OleDbParameter(string parameterName, OleDbType dbType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
			: this(parameterName, dbType, size, srcColumn)
		{
			this.direction = direction;
			this.isNullable = isNullable;
			this.precision = precision;
			this.scale = scale;
			this.sourceVersion = srcVersion;
			this.value = value;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public OleDbParameter(string parameterName, OleDbType dbType, int size, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
			: this(parameterName, dbType, size, sourceColumn)
		{
			this.direction = direction;
			this.precision = precision;
			this.scale = scale;
			this.sourceVersion = sourceVersion;
			this.sourceColumnNullMapping = sourceColumnNullMapping;
			this.value = value;
		}

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		[DataCategory("DataCategory_Data")]
		public override DbType DbType
		{
			get
			{
				return this.dbType;
			}
			set
			{
				this.dbType = value;
				this.oleDbType = this.DbTypeToOleDbType(value);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DataCategory("DataCategory_Data")]
		public override ParameterDirection Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}

		public override bool IsNullable
		{
			get
			{
				return this.isNullable;
			}
			set
			{
				this.isNullable = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DbProviderSpecificTypeProperty(true)]
		[DataCategory("DataCategory_Data")]
		public OleDbType OleDbType
		{
			get
			{
				return this.oleDbType;
			}
			set
			{
				this.oleDbType = value;
				this.dbType = this.OleDbTypeToDbType(value);
			}
		}

		public override string ParameterName
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[DataCategory("DataCategory_Data")]
		[DefaultValue(0)]
		public byte Precision
		{
			get
			{
				return this.precision;
			}
			set
			{
				this.precision = value;
			}
		}

		[DefaultValue(0)]
		[DataCategory("DataCategory_Data")]
		public byte Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		[DataCategory("DataCategory_Data")]
		public override int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		[DataCategory("DataCategory_Data")]
		public override string SourceColumn
		{
			get
			{
				return this.sourceColumn;
			}
			set
			{
				this.sourceColumn = value;
			}
		}

		public override bool SourceColumnNullMapping
		{
			get
			{
				return this.sourceColumnNullMapping;
			}
			set
			{
				this.sourceColumnNullMapping = value;
			}
		}

		[DataCategory("DataCategory_Data")]
		public override DataRowVersion SourceVersion
		{
			get
			{
				return this.sourceVersion;
			}
			set
			{
				this.sourceVersion = value;
			}
		}

		[DataCategory("DataCategory_Data")]
		[TypeConverter(typeof(StringConverter))]
		[RefreshProperties(RefreshProperties.All)]
		public override object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		internal OleDbParameterCollection Container
		{
			get
			{
				return this.container;
			}
			set
			{
				this.container = value;
			}
		}

		internal IntPtr GdaParameter
		{
			get
			{
				return this.gdaParameter;
			}
		}

		public override void ResetDbType()
		{
			this.ResetOleDbType();
		}

		public void ResetOleDbType()
		{
			this.oleDbType = this.GetOleDbType(this.Value);
			this.dbType = this.OleDbTypeToDbType(this.oleDbType);
		}

		public override string ToString()
		{
			return this.ParameterName;
		}

		private OleDbType DbTypeToOleDbType(DbType dbType)
		{
			switch (dbType)
			{
			case DbType.AnsiString:
				return OleDbType.VarChar;
			case DbType.Binary:
				return OleDbType.Binary;
			case DbType.Byte:
				return OleDbType.UnsignedTinyInt;
			case DbType.Boolean:
				return OleDbType.Boolean;
			case DbType.Currency:
				return OleDbType.Currency;
			case DbType.Date:
				return OleDbType.Date;
			case DbType.DateTime:
				throw new NotImplementedException();
			case DbType.Decimal:
				return OleDbType.Decimal;
			case DbType.Double:
				return OleDbType.Double;
			case DbType.Guid:
				return OleDbType.Guid;
			case DbType.Int16:
				return OleDbType.SmallInt;
			case DbType.Int32:
				return OleDbType.Integer;
			case DbType.Int64:
				return OleDbType.BigInt;
			case DbType.Object:
				return OleDbType.Variant;
			case DbType.SByte:
				return OleDbType.TinyInt;
			case DbType.Single:
				return OleDbType.Single;
			case DbType.String:
				return OleDbType.WChar;
			case DbType.Time:
				throw new NotImplementedException();
			case DbType.UInt16:
				return OleDbType.UnsignedSmallInt;
			case DbType.UInt32:
				return OleDbType.UnsignedInt;
			case DbType.UInt64:
				return OleDbType.UnsignedBigInt;
			case DbType.VarNumeric:
				return OleDbType.VarNumeric;
			case DbType.AnsiStringFixedLength:
				return OleDbType.Char;
			case DbType.StringFixedLength:
				return OleDbType.VarWChar;
			default:
				return OleDbType.Variant;
			}
		}

		private DbType OleDbTypeToDbType(OleDbType oleDbType)
		{
			switch (oleDbType)
			{
			case OleDbType.Empty:
				throw new NotImplementedException();
			default:
				switch (oleDbType)
				{
				case OleDbType.Binary:
					return DbType.Binary;
				case OleDbType.Char:
					return DbType.AnsiStringFixedLength;
				case OleDbType.WChar:
					return DbType.String;
				case OleDbType.Numeric:
					return DbType.Decimal;
				default:
					switch (oleDbType)
					{
					case OleDbType.VarChar:
						return DbType.AnsiString;
					case OleDbType.LongVarChar:
						return DbType.AnsiString;
					case OleDbType.VarWChar:
						return DbType.StringFixedLength;
					case OleDbType.LongVarWChar:
						return DbType.String;
					case OleDbType.VarBinary:
						return DbType.Binary;
					case OleDbType.LongVarBinary:
						return DbType.Binary;
					default:
						if (oleDbType == OleDbType.Filetime)
						{
							return DbType.DateTime;
						}
						if (oleDbType != OleDbType.Guid)
						{
							return DbType.Object;
						}
						return DbType.Guid;
					}
					break;
				case OleDbType.DBDate:
					return DbType.DateTime;
				case OleDbType.DBTime:
					throw new NotImplementedException();
				case OleDbType.DBTimeStamp:
					return DbType.DateTime;
				case OleDbType.PropVariant:
					return DbType.Object;
				case OleDbType.VarNumeric:
					return DbType.VarNumeric;
				}
				break;
			case OleDbType.SmallInt:
				return DbType.Int16;
			case OleDbType.Integer:
				return DbType.Int32;
			case OleDbType.Single:
				return DbType.Single;
			case OleDbType.Double:
				return DbType.Double;
			case OleDbType.Currency:
				return DbType.Currency;
			case OleDbType.Date:
				return DbType.DateTime;
			case OleDbType.BSTR:
				return DbType.AnsiString;
			case OleDbType.IDispatch:
				return DbType.Object;
			case OleDbType.Error:
				throw new NotImplementedException();
			case OleDbType.Boolean:
				return DbType.Boolean;
			case OleDbType.Variant:
				return DbType.Object;
			case OleDbType.IUnknown:
				return DbType.Object;
			case OleDbType.Decimal:
				return DbType.Decimal;
			case OleDbType.TinyInt:
				return DbType.SByte;
			case OleDbType.UnsignedTinyInt:
				return DbType.Byte;
			case OleDbType.UnsignedSmallInt:
				return DbType.UInt16;
			case OleDbType.UnsignedInt:
				return DbType.UInt32;
			case OleDbType.BigInt:
				return DbType.Int64;
			case OleDbType.UnsignedBigInt:
				return DbType.UInt64;
			}
		}

		private OleDbType GetOleDbType(object value)
		{
			if (value is Guid)
			{
				return OleDbType.Guid;
			}
			if (value is TimeSpan)
			{
				return OleDbType.DBTime;
			}
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.Empty:
				return OleDbType.Empty;
			case TypeCode.Object:
				return OleDbType.Variant;
			case TypeCode.DBNull:
				return OleDbType.Empty;
			case TypeCode.Boolean:
				return OleDbType.Boolean;
			case TypeCode.Char:
				return OleDbType.Char;
			case TypeCode.SByte:
				return OleDbType.TinyInt;
			case TypeCode.Byte:
				if (value.GetType().IsArray)
				{
					return OleDbType.Binary;
				}
				return OleDbType.UnsignedTinyInt;
			case TypeCode.Int16:
				return OleDbType.SmallInt;
			case TypeCode.UInt16:
				return OleDbType.UnsignedSmallInt;
			case TypeCode.Int32:
				return OleDbType.Integer;
			case TypeCode.UInt32:
				return OleDbType.UnsignedInt;
			case TypeCode.Int64:
				return OleDbType.BigInt;
			case TypeCode.UInt64:
				return OleDbType.UnsignedBigInt;
			case TypeCode.Single:
				return OleDbType.Single;
			case TypeCode.Double:
				return OleDbType.Double;
			case TypeCode.Decimal:
				return OleDbType.Decimal;
			case TypeCode.DateTime:
				return OleDbType.Date;
			case TypeCode.String:
				return OleDbType.VarChar;
			}
			return OleDbType.IUnknown;
		}

		private string name;

		private object value;

		private int size;

		private bool isNullable;

		private byte precision;

		private byte scale;

		private DataRowVersion sourceVersion;

		private string sourceColumn;

		private bool sourceColumnNullMapping;

		private ParameterDirection direction;

		private OleDbType oleDbType;

		private DbType dbType;

		private OleDbParameterCollection container;

		private IntPtr gdaParameter;
	}
}
