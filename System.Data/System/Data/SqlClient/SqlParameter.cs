using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.Xml;
using Mono.Data.Tds;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	[TypeConverter("System.Data.SqlClient.SqlParameter+SqlParameterConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class SqlParameter : DbParameter, IDataParameter, IDbDataParameter, ICloneable
	{
		public SqlParameter()
			: this(string.Empty, SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, null)
		{
			this.isTypeSet = false;
		}

		public SqlParameter(string parameterName, object value)
		{
			this.direction = ParameterDirection.Input;
			this.xmlSchemaCollectionDatabase = string.Empty;
			this.xmlSchemaCollectionOwningSchema = string.Empty;
			this.xmlSchemaCollectionName = string.Empty;
			base..ctor();
			if (parameterName == null)
			{
				parameterName = string.Empty;
			}
			this.metaParameter = new TdsMetaParameter(parameterName, new FrameworkValueGetter(this.GetFrameworkValue));
			this.metaParameter.RawValue = value;
			this.InferSqlType(value);
			this.sourceVersion = DataRowVersion.Current;
		}

		public SqlParameter(string parameterName, SqlDbType dbType)
			: this(parameterName, dbType, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, null)
		{
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size)
			: this(parameterName, dbType, size, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, null)
		{
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size, string sourceColumn)
			: this(parameterName, dbType, size, ParameterDirection.Input, false, 0, 0, sourceColumn, DataRowVersion.Current, null)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SqlParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
		{
			this.direction = ParameterDirection.Input;
			this.xmlSchemaCollectionDatabase = string.Empty;
			this.xmlSchemaCollectionOwningSchema = string.Empty;
			this.xmlSchemaCollectionName = string.Empty;
			base..ctor();
			if (parameterName == null)
			{
				parameterName = string.Empty;
			}
			this.metaParameter = new TdsMetaParameter(parameterName, size, isNullable, precision, scale, new FrameworkValueGetter(this.GetFrameworkValue));
			this.metaParameter.RawValue = value;
			if (dbType != SqlDbType.Variant)
			{
				this.SqlDbType = dbType;
			}
			this.Direction = direction;
			this.SourceColumn = sourceColumn;
			this.SourceVersion = sourceVersion;
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value, string xmlSchemaCollectionDatabase, string xmlSchemaCollectionOwningSchema, string xmlSchemaCollectionName)
			: this(parameterName, dbType, size, direction, false, precision, scale, sourceColumn, sourceVersion, value)
		{
			this.XmlSchemaCollectionDatabase = xmlSchemaCollectionDatabase;
			this.XmlSchemaCollectionOwningSchema = xmlSchemaCollectionOwningSchema;
			this.XmlSchemaCollectionName = xmlSchemaCollectionName;
			this.SourceColumnNullMapping = sourceColumnNullMapping;
		}

		internal SqlParameter(object[] dbValues)
			: this(dbValues[3].ToString(), null)
		{
			this.ParameterName = (string)dbValues[3];
			switch ((short)dbValues[5])
			{
			case 1:
				this.Direction = ParameterDirection.Input;
				break;
			case 2:
				this.Direction = ParameterDirection.InputOutput;
				break;
			case 3:
				this.Direction = ParameterDirection.Output;
				break;
			case 4:
				this.Direction = ParameterDirection.ReturnValue;
				break;
			default:
				this.Direction = ParameterDirection.Input;
				break;
			}
			this.SqlDbType = this.FrameworkDbTypeFromName((string)dbValues[16]);
			if (this.MetaParameter.IsVariableSizeType && dbValues[10] != DBNull.Value)
			{
				this.Size = (int)dbValues[10];
			}
			if (this.SqlDbType == SqlDbType.Decimal)
			{
				if (dbValues[12] != null && dbValues[12] != DBNull.Value)
				{
					this.Precision = (byte)((short)dbValues[12]);
				}
				if (dbValues[13] != null && dbValues[13] != DBNull.Value)
				{
					this.Scale = (byte)((short)dbValues[13]);
				}
			}
		}

		static SqlParameter()
		{
			if (DbParameter.DbTypeMapping == null)
			{
				DbParameter.DbTypeMapping = new Hashtable();
			}
			DbParameter.DbTypeMapping.Add(SqlDbType.BigInt, typeof(long));
			DbParameter.DbTypeMapping.Add(SqlDbType.Bit, typeof(bool));
			DbParameter.DbTypeMapping.Add(SqlDbType.Char, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.NChar, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.Text, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.NText, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.VarChar, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.NVarChar, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.SmallDateTime, typeof(DateTime));
			DbParameter.DbTypeMapping.Add(SqlDbType.DateTime, typeof(DateTime));
			DbParameter.DbTypeMapping.Add(SqlDbType.Decimal, typeof(decimal));
			DbParameter.DbTypeMapping.Add(SqlDbType.Float, typeof(double));
			DbParameter.DbTypeMapping.Add(SqlDbType.Binary, typeof(byte[]));
			DbParameter.DbTypeMapping.Add(SqlDbType.Image, typeof(byte[]));
			DbParameter.DbTypeMapping.Add(SqlDbType.Money, typeof(decimal));
			DbParameter.DbTypeMapping.Add(SqlDbType.SmallMoney, typeof(decimal));
			DbParameter.DbTypeMapping.Add(SqlDbType.VarBinary, typeof(byte[]));
			DbParameter.DbTypeMapping.Add(SqlDbType.TinyInt, typeof(byte));
			DbParameter.DbTypeMapping.Add(SqlDbType.Int, typeof(int));
			DbParameter.DbTypeMapping.Add(SqlDbType.Real, typeof(float));
			DbParameter.DbTypeMapping.Add(SqlDbType.SmallInt, typeof(short));
			DbParameter.DbTypeMapping.Add(SqlDbType.UniqueIdentifier, typeof(Guid));
			DbParameter.DbTypeMapping.Add(SqlDbType.Variant, typeof(object));
			DbParameter.DbTypeMapping.Add(SqlDbType.Xml, typeof(string));
			SqlParameter.type_mapping = new Hashtable();
			SqlParameter.type_mapping.Add(typeof(long), SqlDbType.BigInt);
			SqlParameter.type_mapping.Add(typeof(SqlInt64), SqlDbType.BigInt);
			SqlParameter.type_mapping.Add(typeof(bool), SqlDbType.Bit);
			SqlParameter.type_mapping.Add(typeof(SqlBoolean), SqlDbType.Bit);
			SqlParameter.type_mapping.Add(typeof(char), SqlDbType.NVarChar);
			SqlParameter.type_mapping.Add(typeof(char[]), SqlDbType.NVarChar);
			SqlParameter.type_mapping.Add(typeof(SqlChars), SqlDbType.NVarChar);
			SqlParameter.type_mapping.Add(typeof(string), SqlDbType.NVarChar);
			SqlParameter.type_mapping.Add(typeof(SqlString), SqlDbType.NVarChar);
			SqlParameter.type_mapping.Add(typeof(DateTime), SqlDbType.DateTime);
			SqlParameter.type_mapping.Add(typeof(SqlDateTime), SqlDbType.DateTime);
			SqlParameter.type_mapping.Add(typeof(decimal), SqlDbType.Decimal);
			SqlParameter.type_mapping.Add(typeof(SqlDecimal), SqlDbType.Decimal);
			SqlParameter.type_mapping.Add(typeof(double), SqlDbType.Float);
			SqlParameter.type_mapping.Add(typeof(SqlDouble), SqlDbType.Float);
			SqlParameter.type_mapping.Add(typeof(byte[]), SqlDbType.VarBinary);
			SqlParameter.type_mapping.Add(typeof(SqlBinary), SqlDbType.VarBinary);
			SqlParameter.type_mapping.Add(typeof(SqlBytes), SqlDbType.VarBinary);
			SqlParameter.type_mapping.Add(typeof(byte), SqlDbType.TinyInt);
			SqlParameter.type_mapping.Add(typeof(SqlByte), SqlDbType.TinyInt);
			SqlParameter.type_mapping.Add(typeof(int), SqlDbType.Int);
			SqlParameter.type_mapping.Add(typeof(SqlInt32), SqlDbType.Int);
			SqlParameter.type_mapping.Add(typeof(float), SqlDbType.Real);
			SqlParameter.type_mapping.Add(typeof(SqlSingle), SqlDbType.Real);
			SqlParameter.type_mapping.Add(typeof(short), SqlDbType.SmallInt);
			SqlParameter.type_mapping.Add(typeof(SqlInt16), SqlDbType.SmallInt);
			SqlParameter.type_mapping.Add(typeof(Guid), SqlDbType.UniqueIdentifier);
			SqlParameter.type_mapping.Add(typeof(SqlGuid), SqlDbType.UniqueIdentifier);
			SqlParameter.type_mapping.Add(typeof(SqlMoney), SqlDbType.Money);
			SqlParameter.type_mapping.Add(typeof(XmlReader), SqlDbType.Xml);
			SqlParameter.type_mapping.Add(typeof(SqlXml), SqlDbType.Xml);
			SqlParameter.type_mapping.Add(typeof(object), SqlDbType.Variant);
		}

		object ICloneable.Clone()
		{
			return new SqlParameter(this.ParameterName, this.SqlDbType, this.Size, this.Direction, this.IsNullable, this.Precision, this.Scale, this.SourceColumn, this.SourceVersion, this.Value);
		}

		internal SqlParameterCollection Container
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

		internal void CheckIfInitialized()
		{
			if (!this.isTypeSet)
			{
				throw new Exception("all parameters to have an explicity set type");
			}
			if (this.MetaParameter.IsVariableSizeType)
			{
				if (this.SqlDbType == SqlDbType.Decimal && this.Precision == 0)
				{
					throw new Exception("Parameter of type 'Decimal' have an explicitly set Precision and Scale");
				}
				if (this.Size == 0)
				{
					throw new Exception("all variable length parameters to have an explicitly set non-zero Size");
				}
			}
		}

		public override DbType DbType
		{
			get
			{
				return this.dbType;
			}
			set
			{
				this.SetDbType(value);
				this.typeChanged = true;
				this.isTypeSet = true;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public override ParameterDirection Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
				switch (this.direction)
				{
				case ParameterDirection.Output:
					this.MetaParameter.Direction = TdsParameterDirection.Output;
					break;
				case ParameterDirection.InputOutput:
					this.MetaParameter.Direction = TdsParameterDirection.InputOutput;
					break;
				case ParameterDirection.ReturnValue:
					this.MetaParameter.Direction = TdsParameterDirection.ReturnValue;
					break;
				}
			}
		}

		internal TdsMetaParameter MetaParameter
		{
			get
			{
				return this.metaParameter;
			}
		}

		public override bool IsNullable
		{
			get
			{
				return this.metaParameter.IsNullable;
			}
			set
			{
				this.metaParameter.IsNullable = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		public override string ParameterName
		{
			get
			{
				return this.metaParameter.ParameterName;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				this.metaParameter.ParameterName = value;
			}
		}

		[DefaultValue(0)]
		public byte Precision
		{
			get
			{
				return this.metaParameter.Precision;
			}
			set
			{
				this.metaParameter.Precision = value;
			}
		}

		[DefaultValue(0)]
		public byte Scale
		{
			get
			{
				return this.metaParameter.Scale;
			}
			set
			{
				this.metaParameter.Scale = value;
			}
		}

		public override int Size
		{
			get
			{
				return this.metaParameter.Size;
			}
			set
			{
				this.metaParameter.Size = value;
			}
		}

		public override string SourceColumn
		{
			get
			{
				if (this.sourceColumn == null)
				{
					return string.Empty;
				}
				return this.sourceColumn;
			}
			set
			{
				this.sourceColumn = value;
			}
		}

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

		[DbProviderSpecificTypeProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public SqlDbType SqlDbType
		{
			get
			{
				return this.sqlDbType;
			}
			set
			{
				this.SetSqlDbType(value);
				this.typeChanged = true;
				this.isTypeSet = true;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(StringConverter))]
		public override object Value
		{
			get
			{
				if (this.sqlType != null)
				{
					return this.GetSqlValue(this.metaParameter.RawValue);
				}
				return this.metaParameter.RawValue;
			}
			set
			{
				if (!this.isTypeSet)
				{
					this.InferSqlType(value);
				}
				if (value is INullable)
				{
					this.sqlType = value.GetType();
					value = this.SqlTypeToFrameworkType(value);
				}
				this.metaParameter.RawValue = value;
			}
		}

		[Browsable(false)]
		public SqlCompareOptions CompareInfo
		{
			get
			{
				return this.compareInfo;
			}
			set
			{
				this.compareInfo = value;
			}
		}

		[Browsable(false)]
		public int LocaleId
		{
			get
			{
				return this.localeId;
			}
			set
			{
				this.localeId = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public object SqlValue
		{
			get
			{
				return this.GetSqlValue(this.metaParameter.RawValue);
			}
			set
			{
				this.Value = value;
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

		public string XmlSchemaCollectionDatabase
		{
			get
			{
				return this.xmlSchemaCollectionDatabase;
			}
			set
			{
				this.xmlSchemaCollectionDatabase = ((value != null) ? value : string.Empty);
			}
		}

		public string XmlSchemaCollectionName
		{
			get
			{
				return this.xmlSchemaCollectionName;
			}
			set
			{
				this.xmlSchemaCollectionName = ((value != null) ? value : string.Empty);
			}
		}

		public string XmlSchemaCollectionOwningSchema
		{
			get
			{
				return this.xmlSchemaCollectionOwningSchema;
			}
			set
			{
				this.xmlSchemaCollectionOwningSchema = ((value != null) ? value : string.Empty);
			}
		}

		private void InferSqlType(object value)
		{
			if (value == null || value == DBNull.Value)
			{
				this.SetSqlDbType(SqlDbType.NVarChar);
				return;
			}
			Type type = value.GetType();
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			object obj = SqlParameter.type_mapping[type];
			if (obj == null)
			{
				throw new ArgumentException(string.Format("The parameter data type of {0} is invalid.", type.FullName));
			}
			this.SetSqlDbType((SqlDbType)((int)obj));
		}

		internal override Type SystemType
		{
			get
			{
				return (Type)DbParameter.DbTypeMapping[this.sqlDbType];
			}
		}

		internal override object FrameworkDbType
		{
			get
			{
				return this.sqlDbType;
			}
			set
			{
				try
				{
					object obj = this.DbTypeFromName((string)value);
					this.SetDbType((DbType)((int)obj));
				}
				catch (ArgumentException)
				{
					object obj = this.FrameworkDbTypeFromName((string)value);
					this.SetSqlDbType((SqlDbType)((int)obj));
				}
			}
		}

		private DbType DbTypeFromName(string name)
		{
			string text = name.ToLower();
			switch (text)
			{
			case "ansistring":
				return DbType.AnsiString;
			case "ansistringfixedlength":
				return DbType.AnsiStringFixedLength;
			case "binary":
				return DbType.Binary;
			case "boolean":
				return DbType.Boolean;
			case "byte":
				return DbType.Byte;
			case "currency":
				return DbType.Currency;
			case "date":
				return DbType.Date;
			case "datetime":
				return DbType.DateTime;
			case "decimal":
				return DbType.Decimal;
			case "double":
				return DbType.Double;
			case "guid":
				return DbType.Guid;
			case "int16":
				return DbType.Int16;
			case "int32":
				return DbType.Int32;
			case "int64":
				return DbType.Int64;
			case "object":
				return DbType.Object;
			case "single":
				return DbType.Single;
			case "string":
				return DbType.String;
			case "stringfixedlength":
				return DbType.StringFixedLength;
			case "time":
				return DbType.Time;
			case "xml":
				return DbType.Xml;
			}
			string text2 = string.Format("No mapping exists from {0} to a known DbType.", name);
			throw new ArgumentException(text2);
		}

		private void SetDbType(DbType type)
		{
			switch (type)
			{
			case DbType.AnsiString:
				this.MetaParameter.TypeName = "varchar";
				this.sqlDbType = SqlDbType.VarChar;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_02F4;
			case DbType.Binary:
				this.MetaParameter.TypeName = "varbinary";
				this.sqlDbType = SqlDbType.VarBinary;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_02F4;
			case DbType.Byte:
				this.MetaParameter.TypeName = "tinyint";
				this.sqlDbType = SqlDbType.TinyInt;
				goto IL_02F4;
			case DbType.Boolean:
				this.MetaParameter.TypeName = "bit";
				this.sqlDbType = SqlDbType.Bit;
				goto IL_02F4;
			case DbType.Currency:
				this.sqlDbType = SqlDbType.Money;
				this.MetaParameter.TypeName = "money";
				goto IL_02F4;
			case DbType.Date:
			case DbType.DateTime:
				this.MetaParameter.TypeName = "datetime";
				this.sqlDbType = SqlDbType.DateTime;
				goto IL_02F4;
			case DbType.Decimal:
				this.MetaParameter.TypeName = "decimal";
				this.sqlDbType = SqlDbType.Decimal;
				goto IL_02F4;
			case DbType.Double:
				this.MetaParameter.TypeName = "float";
				this.sqlDbType = SqlDbType.Float;
				goto IL_02F4;
			case DbType.Guid:
				this.MetaParameter.TypeName = "uniqueidentifier";
				this.sqlDbType = SqlDbType.UniqueIdentifier;
				goto IL_02F4;
			case DbType.Int16:
				this.MetaParameter.TypeName = "smallint";
				this.sqlDbType = SqlDbType.SmallInt;
				goto IL_02F4;
			case DbType.Int32:
				this.MetaParameter.TypeName = "int";
				this.sqlDbType = SqlDbType.Int;
				goto IL_02F4;
			case DbType.Int64:
				this.MetaParameter.TypeName = "bigint";
				this.sqlDbType = SqlDbType.BigInt;
				goto IL_02F4;
			case DbType.Object:
				this.MetaParameter.TypeName = "sql_variant";
				this.sqlDbType = SqlDbType.Variant;
				goto IL_02F4;
			case DbType.Single:
				this.MetaParameter.TypeName = "real";
				this.sqlDbType = SqlDbType.Real;
				goto IL_02F4;
			case DbType.String:
				this.MetaParameter.TypeName = "nvarchar";
				this.sqlDbType = SqlDbType.NVarChar;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_02F4;
			case DbType.Time:
				this.MetaParameter.TypeName = "datetime";
				this.sqlDbType = SqlDbType.DateTime;
				goto IL_02F4;
			case DbType.AnsiStringFixedLength:
				this.MetaParameter.TypeName = "char";
				this.sqlDbType = SqlDbType.Char;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_02F4;
			case DbType.StringFixedLength:
				this.MetaParameter.TypeName = "nchar";
				this.sqlDbType = SqlDbType.NChar;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_02F4;
			case DbType.Xml:
				this.MetaParameter.TypeName = "xml";
				this.sqlDbType = SqlDbType.Xml;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_02F4;
			}
			string text = string.Format("No mapping exists from DbType {0} to a known SqlDbType.", type);
			throw new ArgumentException(text);
			IL_02F4:
			this.dbType = type;
		}

		private SqlDbType FrameworkDbTypeFromName(string dbTypeName)
		{
			string text = dbTypeName.ToLower();
			switch (text)
			{
			case "bigint":
				return SqlDbType.BigInt;
			case "binary":
				return SqlDbType.Binary;
			case "bit":
				return SqlDbType.Bit;
			case "char":
				return SqlDbType.Char;
			case "datetime":
				return SqlDbType.DateTime;
			case "decimal":
				return SqlDbType.Decimal;
			case "float":
				return SqlDbType.Float;
			case "image":
				return SqlDbType.Image;
			case "int":
				return SqlDbType.Int;
			case "money":
				return SqlDbType.Money;
			case "nchar":
				return SqlDbType.NChar;
			case "ntext":
				return SqlDbType.NText;
			case "nvarchar":
				return SqlDbType.NVarChar;
			case "real":
				return SqlDbType.Real;
			case "smalldatetime":
				return SqlDbType.SmallDateTime;
			case "smallint":
				return SqlDbType.SmallInt;
			case "smallmoney":
				return SqlDbType.SmallMoney;
			case "text":
				return SqlDbType.Text;
			case "timestamp":
				return SqlDbType.Timestamp;
			case "tinyint":
				return SqlDbType.TinyInt;
			case "uniqueidentifier":
				return SqlDbType.UniqueIdentifier;
			case "varbinary":
				return SqlDbType.VarBinary;
			case "varchar":
				return SqlDbType.VarChar;
			case "sql_variant":
				return SqlDbType.Variant;
			case "xml":
				return SqlDbType.Xml;
			}
			return SqlDbType.Variant;
		}

		internal void SetSqlDbType(SqlDbType type)
		{
			switch (type)
			{
			case SqlDbType.BigInt:
				this.MetaParameter.TypeName = "bigint";
				this.dbType = DbType.Int64;
				goto IL_03D1;
			case SqlDbType.Binary:
				this.MetaParameter.TypeName = "binary";
				this.dbType = DbType.Binary;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			case SqlDbType.Bit:
				this.MetaParameter.TypeName = "bit";
				this.dbType = DbType.Boolean;
				goto IL_03D1;
			case SqlDbType.Char:
				this.MetaParameter.TypeName = "char";
				this.dbType = DbType.AnsiStringFixedLength;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			case SqlDbType.DateTime:
				this.MetaParameter.TypeName = "datetime";
				this.dbType = DbType.DateTime;
				goto IL_03D1;
			case SqlDbType.Decimal:
				this.MetaParameter.TypeName = "decimal";
				this.dbType = DbType.Decimal;
				goto IL_03D1;
			case SqlDbType.Float:
				this.MetaParameter.TypeName = "float";
				this.dbType = DbType.Double;
				goto IL_03D1;
			case SqlDbType.Image:
				this.MetaParameter.TypeName = "image";
				this.dbType = DbType.Binary;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			case SqlDbType.Int:
				this.MetaParameter.TypeName = "int";
				this.dbType = DbType.Int32;
				goto IL_03D1;
			case SqlDbType.Money:
				this.MetaParameter.TypeName = "money";
				this.dbType = DbType.Currency;
				goto IL_03D1;
			case SqlDbType.NChar:
				this.MetaParameter.TypeName = "nchar";
				this.dbType = DbType.StringFixedLength;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			case SqlDbType.NText:
				this.MetaParameter.TypeName = "ntext";
				this.dbType = DbType.String;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			case SqlDbType.NVarChar:
				this.MetaParameter.TypeName = "nvarchar";
				this.dbType = DbType.String;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			case SqlDbType.Real:
				this.MetaParameter.TypeName = "real";
				this.dbType = DbType.Single;
				goto IL_03D1;
			case SqlDbType.UniqueIdentifier:
				this.MetaParameter.TypeName = "uniqueidentifier";
				this.dbType = DbType.Guid;
				goto IL_03D1;
			case SqlDbType.SmallDateTime:
				this.MetaParameter.TypeName = "smalldatetime";
				this.dbType = DbType.DateTime;
				goto IL_03D1;
			case SqlDbType.SmallInt:
				this.MetaParameter.TypeName = "smallint";
				this.dbType = DbType.Int16;
				goto IL_03D1;
			case SqlDbType.SmallMoney:
				this.MetaParameter.TypeName = "smallmoney";
				this.dbType = DbType.Currency;
				goto IL_03D1;
			case SqlDbType.Text:
				this.MetaParameter.TypeName = "text";
				this.dbType = DbType.AnsiString;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			case SqlDbType.Timestamp:
				this.MetaParameter.TypeName = "timestamp";
				this.dbType = DbType.Binary;
				goto IL_03D1;
			case SqlDbType.TinyInt:
				this.MetaParameter.TypeName = "tinyint";
				this.dbType = DbType.Byte;
				goto IL_03D1;
			case SqlDbType.VarBinary:
				this.MetaParameter.TypeName = "varbinary";
				this.dbType = DbType.Binary;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			case SqlDbType.VarChar:
				this.MetaParameter.TypeName = "varchar";
				this.dbType = DbType.AnsiString;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			case SqlDbType.Variant:
				this.MetaParameter.TypeName = "sql_variant";
				this.dbType = DbType.Object;
				goto IL_03D1;
			case SqlDbType.Xml:
				this.MetaParameter.TypeName = "xml";
				this.dbType = DbType.Xml;
				this.MetaParameter.IsVariableSizeType = true;
				goto IL_03D1;
			}
			string text = string.Format("No mapping exists from SqlDbType {0} to a known DbType.", type);
			throw new ArgumentOutOfRangeException("SqlDbType", text);
			IL_03D1:
			this.sqlDbType = type;
		}

		public override string ToString()
		{
			return this.ParameterName;
		}

		private object GetFrameworkValue(object rawValue, ref bool updated)
		{
			updated = this.typeChanged || updated;
			object obj;
			if (updated)
			{
				obj = this.SqlTypeToFrameworkType(rawValue);
				this.typeChanged = false;
			}
			else
			{
				obj = null;
			}
			return obj;
		}

		private object GetSqlValue(object value)
		{
			if (value == null)
			{
				return value;
			}
			switch (this.sqlDbType)
			{
			case SqlDbType.BigInt:
				if (value == DBNull.Value)
				{
					return SqlInt64.Null;
				}
				return (long)value;
			case SqlDbType.Binary:
			case SqlDbType.Image:
			case SqlDbType.Timestamp:
			case SqlDbType.VarBinary:
				if (value == DBNull.Value)
				{
					return SqlBinary.Null;
				}
				return (byte[])value;
			case SqlDbType.Bit:
				if (value == DBNull.Value)
				{
					return SqlBoolean.Null;
				}
				return (bool)value;
			case SqlDbType.Char:
			case SqlDbType.NChar:
			case SqlDbType.NText:
			case SqlDbType.NVarChar:
			case SqlDbType.Text:
			case SqlDbType.VarChar:
			{
				if (value == DBNull.Value)
				{
					return SqlString.Null;
				}
				Type type = value.GetType();
				string text;
				if (type == typeof(char))
				{
					text = value.ToString();
				}
				else if (type == typeof(char[]))
				{
					text = new string((char[])value);
				}
				else
				{
					text = (string)value;
				}
				return text;
			}
			case SqlDbType.DateTime:
			case SqlDbType.SmallDateTime:
				if (value == DBNull.Value)
				{
					return SqlDateTime.Null;
				}
				return (DateTime)value;
			case SqlDbType.Decimal:
				if (value == DBNull.Value)
				{
					return SqlDecimal.Null;
				}
				if (value is TdsBigDecimal)
				{
					return SqlDecimal.FromTdsBigDecimal((TdsBigDecimal)value);
				}
				return (decimal)value;
			case SqlDbType.Float:
				if (value == DBNull.Value)
				{
					return SqlDouble.Null;
				}
				return (double)value;
			case SqlDbType.Int:
				if (value == DBNull.Value)
				{
					return SqlInt32.Null;
				}
				return (int)value;
			case SqlDbType.Money:
			case SqlDbType.SmallMoney:
				if (value == DBNull.Value)
				{
					return SqlMoney.Null;
				}
				return (decimal)value;
			case SqlDbType.Real:
				if (value == DBNull.Value)
				{
					return SqlSingle.Null;
				}
				return (float)value;
			case SqlDbType.UniqueIdentifier:
				if (value == DBNull.Value)
				{
					return SqlGuid.Null;
				}
				return (Guid)value;
			case SqlDbType.SmallInt:
				if (value == DBNull.Value)
				{
					return SqlInt16.Null;
				}
				return (short)value;
			case SqlDbType.TinyInt:
				if (value == DBNull.Value)
				{
					return SqlByte.Null;
				}
				return (byte)value;
			case SqlDbType.Xml:
				if (value == DBNull.Value)
				{
					return SqlXml.Null;
				}
				return (SqlXml)value;
			}
			throw new NotImplementedException("Type '" + this.sqlDbType + "' not implemented.");
		}

		private object SqlTypeToFrameworkType(object value)
		{
			INullable nullable = value as INullable;
			if (nullable == null)
			{
				return this.ConvertToFrameworkType(value);
			}
			if (nullable.IsNull)
			{
				return DBNull.Value;
			}
			Type type = value.GetType();
			if (typeof(SqlString) == type)
			{
				return ((SqlString)value).Value;
			}
			if (typeof(SqlInt16) == type)
			{
				return ((SqlInt16)value).Value;
			}
			if (typeof(SqlInt32) == type)
			{
				return ((SqlInt32)value).Value;
			}
			if (typeof(SqlDateTime) == type)
			{
				return ((SqlDateTime)value).Value;
			}
			if (typeof(SqlInt64) == type)
			{
				return ((SqlInt64)value).Value;
			}
			if (typeof(SqlBinary) == type)
			{
				return ((SqlBinary)value).Value;
			}
			if (typeof(SqlBytes) == type)
			{
				return ((SqlBytes)value).Value;
			}
			if (typeof(SqlChars) == type)
			{
				return ((SqlChars)value).Value;
			}
			if (typeof(SqlBoolean) == type)
			{
				return ((SqlBoolean)value).Value;
			}
			if (typeof(SqlByte) == type)
			{
				return ((SqlByte)value).Value;
			}
			if (typeof(SqlDecimal) == type)
			{
				return ((SqlDecimal)value).Value;
			}
			if (typeof(SqlDouble) == type)
			{
				return ((SqlDouble)value).Value;
			}
			if (typeof(SqlGuid) == type)
			{
				return ((SqlGuid)value).Value;
			}
			if (typeof(SqlMoney) == type)
			{
				return ((SqlMoney)value).Value;
			}
			if (typeof(SqlSingle) == type)
			{
				return ((SqlSingle)value).Value;
			}
			return value;
		}

		internal object ConvertToFrameworkType(object value)
		{
			if (value == null || value == DBNull.Value)
			{
				return value;
			}
			if (this.sqlDbType == SqlDbType.Variant)
			{
				return this.metaParameter.Value;
			}
			Type systemType = this.SystemType;
			if (systemType == null)
			{
				throw new NotImplementedException("Type Not Supported : " + this.sqlDbType.ToString());
			}
			Type type = value.GetType();
			if (type == systemType)
			{
				return value;
			}
			object obj = null;
			try
			{
				obj = this.ConvertToFrameworkType(value, systemType);
			}
			catch (FormatException ex)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Parameter value could not be converted from {0} to {1}.", new object[] { type.Name, systemType.Name }), ex);
			}
			return obj;
		}

		private object ConvertToFrameworkType(object value, Type frameworkType)
		{
			object obj = Convert.ChangeType(value, frameworkType);
			SqlDbType sqlDbType = this.sqlDbType;
			if (sqlDbType == SqlDbType.Money || sqlDbType == SqlDbType.SmallMoney)
			{
				obj = decimal.Round((decimal)obj, 4);
			}
			return obj;
		}

		public override void ResetDbType()
		{
			this.InferSqlType(this.Value);
		}

		public void ResetSqlDbType()
		{
			this.InferSqlType(this.Value);
		}

		private TdsMetaParameter metaParameter;

		private SqlParameterCollection container;

		private DbType dbType;

		private ParameterDirection direction;

		private bool isTypeSet;

		private int offset;

		private SqlDbType sqlDbType;

		private string sourceColumn;

		private DataRowVersion sourceVersion;

		private SqlCompareOptions compareInfo;

		private int localeId;

		private Type sqlType;

		private bool typeChanged;

		private bool sourceColumnNullMapping;

		private string xmlSchemaCollectionDatabase;

		private string xmlSchemaCollectionOwningSchema;

		private string xmlSchemaCollectionName;

		private static Hashtable type_mapping;
	}
}
