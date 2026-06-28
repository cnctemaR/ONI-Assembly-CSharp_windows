using System;
using System.Data;
using System.Data.SqlTypes;
using System.Threading;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlMetaData
	{
		public SqlMetaData(string name, SqlDbType sqlDbType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			switch (sqlDbType)
			{
			case SqlDbType.BigInt:
				this.maxLength = 8L;
				this.precision = 19;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Int64;
				this.type = typeof(long);
				goto IL_045E;
			case SqlDbType.Bit:
				this.maxLength = 1L;
				this.precision = 1;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Boolean;
				this.type = typeof(bool);
				goto IL_045E;
			case SqlDbType.DateTime:
				this.maxLength = 8L;
				this.precision = 23;
				this.scale = 3;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.DateTime;
				this.type = typeof(DateTime);
				goto IL_045E;
			case SqlDbType.Decimal:
				this.maxLength = 9L;
				this.precision = 18;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Decimal;
				this.type = typeof(decimal);
				goto IL_045E;
			case SqlDbType.Float:
				this.maxLength = 8L;
				this.precision = 53;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Double;
				this.type = typeof(float);
				goto IL_045E;
			case SqlDbType.Int:
				this.maxLength = 4L;
				this.precision = 10;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Int32;
				this.type = typeof(int);
				goto IL_045E;
			case SqlDbType.Money:
				this.maxLength = 8L;
				this.precision = 19;
				this.scale = 4;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Currency;
				this.type = typeof(double);
				goto IL_045E;
			case SqlDbType.UniqueIdentifier:
				this.maxLength = 16L;
				this.precision = 0;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Guid;
				this.type = typeof(Guid);
				goto IL_045E;
			case SqlDbType.SmallDateTime:
				this.maxLength = 4L;
				this.precision = 16;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.DateTime;
				this.type = typeof(DateTime);
				goto IL_045E;
			case SqlDbType.SmallInt:
				this.maxLength = 2L;
				this.precision = 5;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Int16;
				this.type = typeof(short);
				goto IL_045E;
			case SqlDbType.SmallMoney:
				this.maxLength = 4L;
				this.precision = 10;
				this.scale = 4;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Currency;
				this.type = typeof(double);
				goto IL_045E;
			case SqlDbType.Timestamp:
				this.maxLength = 8L;
				this.precision = 0;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.DateTime;
				this.type = typeof(DateTime);
				goto IL_045E;
			case SqlDbType.TinyInt:
				this.maxLength = 1L;
				this.precision = 3;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Int16;
				this.type = typeof(short);
				goto IL_045E;
			case SqlDbType.Xml:
				this.maxLength = -1L;
				this.precision = 0;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				this.dbType = DbType.Xml;
				this.type = typeof(string);
				goto IL_045E;
			}
			throw new ArgumentException("SqlDbType not supported");
			IL_045E:
			this.name = name;
			this.sqlDbType = sqlDbType;
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, long maxLength)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			switch (sqlDbType)
			{
			case SqlDbType.Binary:
				this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				this.dbType = DbType.Binary;
				this.type = typeof(byte[]);
				break;
			default:
				switch (sqlDbType)
				{
				case SqlDbType.Text:
					maxLength = -1L;
					this.precision = 0;
					this.scale = 0;
					this.localeId = (long)Thread.CurrentThread.CurrentCulture.LCID;
					this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
					this.dbType = DbType.String;
					this.type = typeof(char[]);
					goto IL_02BD;
				case SqlDbType.VarBinary:
					maxLength = -1L;
					this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
					this.dbType = DbType.Binary;
					this.type = typeof(byte[]);
					goto IL_02BD;
				case SqlDbType.VarChar:
					maxLength = -1L;
					this.localeId = (long)Thread.CurrentThread.CurrentCulture.LCID;
					this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
					this.dbType = DbType.String;
					this.type = typeof(char[]);
					goto IL_02BD;
				}
				throw new ArgumentException("SqlDbType not supported");
			case SqlDbType.Char:
				this.localeId = (long)Thread.CurrentThread.CurrentCulture.LCID;
				this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				this.dbType = DbType.AnsiStringFixedLength;
				this.type = typeof(string);
				break;
			case SqlDbType.Image:
				maxLength = -1L;
				this.precision = 0;
				this.scale = 0;
				this.localeId = 0L;
				this.compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Binary;
				this.type = typeof(byte[]);
				break;
			case SqlDbType.NChar:
				this.localeId = (long)Thread.CurrentThread.CurrentCulture.LCID;
				this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				this.dbType = DbType.String;
				this.type = typeof(string);
				break;
			case SqlDbType.NText:
				maxLength = -1L;
				this.precision = 0;
				this.scale = 0;
				this.localeId = (long)Thread.CurrentThread.CurrentCulture.LCID;
				this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				this.dbType = DbType.String;
				this.type = typeof(string);
				break;
			case SqlDbType.NVarChar:
				maxLength = -1L;
				this.localeId = (long)Thread.CurrentThread.CurrentCulture.LCID;
				this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				this.dbType = DbType.String;
				this.type = typeof(string);
				break;
			}
			IL_02BD:
			this.maxLength = maxLength;
			this.name = name;
			this.sqlDbType = sqlDbType;
		}

		[MonoTODO]
		public SqlMetaData(string name, SqlDbType sqlDbType, Type userDefinedType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			if (sqlDbType != SqlDbType.Udt)
			{
				throw new ArgumentException("SqlDbType not supported");
			}
			this.maxLength = -1L;
			this.precision = 0;
			this.scale = 0;
			this.localeId = 0L;
			this.compareOptions = SqlCompareOptions.None;
			this.dbType = DbType.Guid;
			this.type = typeof(Guid);
			this.name = name;
			throw new NotImplementedException();
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, byte precision, byte scale)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			if (sqlDbType != SqlDbType.Decimal)
			{
				throw new ArgumentException("SqlDbType not supported");
			}
			this.maxLength = 9L;
			this.precision = precision;
			this.scale = scale;
			this.localeId = 0L;
			this.compareOptions = SqlCompareOptions.None;
			this.dbType = DbType.Decimal;
			this.type = typeof(decimal);
			this.name = name;
			this.sqlDbType = sqlDbType;
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, long maxLength, long locale, SqlCompareOptions compareOptions)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			switch (sqlDbType)
			{
			case SqlDbType.NChar:
				this.dbType = DbType.StringFixedLength;
				this.type = typeof(char[]);
				break;
			case SqlDbType.NText:
			case SqlDbType.NVarChar:
				this.dbType = DbType.String;
				this.type = typeof(string);
				break;
			default:
				if (sqlDbType != SqlDbType.Char)
				{
					if (sqlDbType != SqlDbType.Text && sqlDbType != SqlDbType.VarChar)
					{
						throw new ArgumentException("SqlDbType not supported");
					}
					this.dbType = DbType.AnsiString;
					this.type = typeof(char[]);
				}
				else
				{
					this.dbType = DbType.AnsiStringFixedLength;
					this.type = typeof(char[]);
				}
				break;
			}
			this.compareOptions = compareOptions;
			this.localeId = locale;
			this.maxLength = maxLength;
			this.name = name;
			this.sqlDbType = sqlDbType;
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, string database, string owningSchema, string objectName)
		{
			if ((name == null || objectName == null) && database != null && owningSchema != null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			if (sqlDbType != SqlDbType.Xml)
			{
				throw new ArgumentException("SqlDbType not supported");
			}
			this.maxLength = -1L;
			this.precision = 0;
			this.scale = 0;
			this.localeId = 0L;
			this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
			this.dbType = DbType.String;
			this.type = typeof(string);
			this.name = name;
			this.sqlDbType = sqlDbType;
			this.databaseName = database;
			this.owningSchema = owningSchema;
			this.objectName = objectName;
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, Type userDefinedType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			this.compareOptions = compareOptions;
			this.localeId = localeId;
			this.maxLength = maxLength;
			this.precision = precision;
			this.scale = scale;
			switch (sqlDbType)
			{
			case SqlDbType.BigInt:
				maxLength = 8L;
				precision = 19;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Int64;
				this.type = typeof(long);
				goto IL_04B2;
			case SqlDbType.Bit:
				maxLength = 1L;
				precision = 1;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Boolean;
				this.type = typeof(bool);
				goto IL_04B2;
			case SqlDbType.DateTime:
				maxLength = 8L;
				precision = 23;
				scale = 3;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.DateTime;
				this.type = typeof(DateTime);
				goto IL_04B2;
			case SqlDbType.Decimal:
				maxLength = 9L;
				precision = 18;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Decimal;
				this.type = typeof(decimal);
				goto IL_04B2;
			case SqlDbType.Float:
				maxLength = 8L;
				precision = 53;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Decimal;
				this.type = typeof(float);
				goto IL_04B2;
			case SqlDbType.Image:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Binary;
				this.type = typeof(byte[]);
				goto IL_04B2;
			case SqlDbType.Int:
				maxLength = 4L;
				precision = 10;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Int32;
				this.type = typeof(int);
				goto IL_04B2;
			case SqlDbType.Money:
				maxLength = 8L;
				precision = 19;
				scale = 4;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Currency;
				this.type = typeof(decimal);
				goto IL_04B2;
			case SqlDbType.NText:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = (long)Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				this.dbType = DbType.String;
				this.type = typeof(string);
				goto IL_04B2;
			case SqlDbType.Real:
				maxLength = 4L;
				precision = 24;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Single;
				this.type = typeof(float);
				goto IL_04B2;
			case SqlDbType.UniqueIdentifier:
				maxLength = 16L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Guid;
				this.type = typeof(Guid);
				goto IL_04B2;
			case SqlDbType.SmallDateTime:
				maxLength = 4L;
				precision = 16;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.DateTime;
				this.type = typeof(DateTime);
				goto IL_04B2;
			case SqlDbType.SmallInt:
				maxLength = 2L;
				precision = 5;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Int16;
				this.type = typeof(short);
				goto IL_04B2;
			case SqlDbType.SmallMoney:
				maxLength = 4L;
				precision = 10;
				scale = 4;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Currency;
				this.type = typeof(decimal);
				goto IL_04B2;
			case SqlDbType.Text:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = (long)Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				this.dbType = DbType.AnsiString;
				this.type = typeof(char[]);
				goto IL_04B2;
			case SqlDbType.Timestamp:
				maxLength = 8L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Byte;
				this.type = typeof(byte[]);
				goto IL_04B2;
			case SqlDbType.TinyInt:
				maxLength = 1L;
				precision = 3;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Int16;
				this.type = typeof(short);
				goto IL_04B2;
			case SqlDbType.Variant:
				maxLength = 8016L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Object;
				this.type = typeof(object);
				goto IL_04B2;
			case SqlDbType.Xml:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				this.dbType = DbType.Xml;
				this.type = typeof(string);
				goto IL_04B2;
			case SqlDbType.Udt:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				this.dbType = DbType.Object;
				this.type = typeof(object);
				goto IL_04B2;
			}
			throw new ArgumentException("SqlDbType not supported");
			IL_04B2:
			this.name = name;
			this.sqlDbType = sqlDbType;
		}

		public SqlCompareOptions CompareOptions
		{
			get
			{
				return this.compareOptions;
			}
		}

		public DbType DbType
		{
			get
			{
				return this.dbType;
			}
		}

		public long LocaleId
		{
			get
			{
				return this.localeId;
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
				return this.maxLength;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public byte Precision
		{
			get
			{
				return this.precision;
			}
		}

		public byte Scale
		{
			get
			{
				return this.scale;
			}
		}

		public SqlDbType SqlDbType
		{
			get
			{
				return this.sqlDbType;
			}
		}

		public string XmlSchemaCollectionDatabase
		{
			get
			{
				return this.databaseName;
			}
		}

		public string XmlSchemaCollectionName
		{
			get
			{
				return this.objectName;
			}
		}

		public string XmlSchemaCollectionOwningSchema
		{
			get
			{
				return this.owningSchema;
			}
		}

		[MonoTODO]
		public string TypeName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool Adjust(bool value)
		{
			if (this.type != typeof(bool))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public byte Adjust(byte value)
		{
			if (this.type != typeof(byte))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public byte[] Adjust(byte[] value)
		{
			if (this.type != typeof(byte[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public char Adjust(char value)
		{
			if (this.type != typeof(char))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public char[] Adjust(char[] value)
		{
			if (this.type != typeof(char[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public DateTime Adjust(DateTime value)
		{
			if (this.type != typeof(DateTime))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public decimal Adjust(decimal value)
		{
			if (this.type != typeof(decimal))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public double Adjust(double value)
		{
			if (this.type != typeof(double))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public Guid Adjust(Guid value)
		{
			if (this.type != typeof(Guid))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public short Adjust(short value)
		{
			if (this.type != typeof(short))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public int Adjust(int value)
		{
			if (this.type != typeof(int))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public long Adjust(long value)
		{
			if (this.type != typeof(long))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public object Adjust(object value)
		{
			if (this.type != typeof(object))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public float Adjust(float value)
		{
			if (this.type != typeof(float))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlBinary Adjust(SqlBinary value)
		{
			if (this.type != typeof(byte[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlBoolean Adjust(SqlBoolean value)
		{
			if (this.type != typeof(bool))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlByte Adjust(SqlByte value)
		{
			if (this.type != typeof(byte))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlBytes Adjust(SqlBytes value)
		{
			if (this.type != typeof(byte[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlChars Adjust(SqlChars value)
		{
			if (this.type != typeof(char[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlDateTime Adjust(SqlDateTime value)
		{
			if (this.type != typeof(DateTime))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlDecimal Adjust(SqlDecimal value)
		{
			if (this.type != typeof(decimal))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlDouble Adjust(SqlDouble value)
		{
			if (this.type != typeof(double))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlGuid Adjust(SqlGuid value)
		{
			if (this.type != typeof(Guid))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlInt16 Adjust(SqlInt16 value)
		{
			if (this.type != typeof(short))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlInt32 Adjust(SqlInt32 value)
		{
			if (this.type != typeof(int))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlInt64 Adjust(SqlInt64 value)
		{
			if (this.type != typeof(long))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlMoney Adjust(SqlMoney value)
		{
			if (this.type != typeof(decimal))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlSingle Adjust(SqlSingle value)
		{
			if (this.type != typeof(float))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlString Adjust(SqlString value)
		{
			if (this.type != typeof(string))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public string Adjust(string value)
		{
			if (this.type != typeof(string))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public static SqlMetaData InferFromValue(object value, string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			if (value == null)
			{
				throw new ArgumentException("value can not be null");
			}
			string text = value.GetType().ToString();
			switch (text)
			{
			case "System.Boolean":
				return new SqlMetaData(name, SqlDbType.Bit);
			case "System.Byte":
				return new SqlMetaData(name, SqlDbType.Binary);
			case "System.Byte[]":
				return new SqlMetaData(name, SqlDbType.VarBinary);
			case "System.Char":
				return new SqlMetaData(name, SqlDbType.Char);
			case "System.Char[]":
				return new SqlMetaData(name, SqlDbType.VarChar);
			case "System.DateTime":
				return new SqlMetaData(name, SqlDbType.DateTime);
			case "System.Decimal":
				return new SqlMetaData(name, SqlDbType.Decimal);
			case "System.Double":
				return new SqlMetaData(name, SqlDbType.Float);
			case "System.Guid":
				return new SqlMetaData(name, SqlDbType.UniqueIdentifier);
			case "System.Int16":
				return new SqlMetaData(name, SqlDbType.SmallInt);
			case "System.Int32":
				return new SqlMetaData(name, SqlDbType.Int);
			case "System.Int64":
				return new SqlMetaData(name, SqlDbType.BigInt);
			case "System.Single":
				return new SqlMetaData(name, SqlDbType.Real);
			case "System.String":
				return new SqlMetaData(name, SqlDbType.NVarChar);
			}
			return new SqlMetaData(name, SqlDbType.Variant);
		}

		private SqlCompareOptions compareOptions;

		private string databaseName;

		private long localeId;

		private long maxLength;

		private string name;

		private byte precision = 10;

		private byte scale;

		private string owningSchema;

		private string objectName;

		private SqlDbType sqlDbType = SqlDbType.NVarChar;

		private DbType dbType = DbType.String;

		private Type type = typeof(string);
	}
}
