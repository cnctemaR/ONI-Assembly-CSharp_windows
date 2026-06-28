using System;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlString : IXmlSerializable, IComparable, INullable
	{
		public SqlString(string data)
		{
			this.value = data;
			this.lcid = CultureInfo.CurrentCulture.LCID;
			if (this.value != null)
			{
				this.notNull = true;
			}
			else
			{
				this.notNull = false;
			}
			this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
		}

		public SqlString(string data, int lcid)
		{
			this.value = data;
			this.lcid = lcid;
			if (this.value != null)
			{
				this.notNull = true;
			}
			else
			{
				this.notNull = false;
			}
			this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
		}

		public SqlString(int lcid, SqlCompareOptions compareOptions, byte[] data)
		{
			this = new SqlString(lcid, compareOptions, data, true);
		}

		public SqlString(string data, int lcid, SqlCompareOptions compareOptions)
		{
			this.value = data;
			this.lcid = lcid;
			this.compareOptions = compareOptions;
			if (this.value != null)
			{
				this.notNull = true;
			}
			else
			{
				this.notNull = false;
			}
		}

		public SqlString(int lcid, SqlCompareOptions compareOptions, byte[] data, bool fUnicode)
		{
			Encoding encoding = ((!fUnicode) ? Encoding.ASCII : Encoding.Unicode);
			this.value = encoding.GetString(data);
			this.lcid = lcid;
			this.compareOptions = compareOptions;
			if (this.value != null)
			{
				this.notNull = true;
			}
			else
			{
				this.notNull = false;
			}
		}

		public SqlString(int lcid, SqlCompareOptions compareOptions, byte[] data, int index, int count)
		{
			this = new SqlString(lcid, compareOptions, data, index, count, true);
		}

		public SqlString(int lcid, SqlCompareOptions compareOptions, byte[] data, int index, int count, bool fUnicode)
		{
			Encoding encoding = ((!fUnicode) ? Encoding.ASCII : Encoding.Unicode);
			this.value = encoding.GetString(data, index, count);
			this.lcid = lcid;
			this.compareOptions = compareOptions;
			if (this.value != null)
			{
				this.notNull = true;
			}
			else
			{
				this.notNull = false;
			}
		}

		static SqlString()
		{
			SqlString.DecimalFormat.NumberDecimalDigits = 13;
			SqlString.DecimalFormat.NumberGroupSeparator = string.Empty;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				return;
			}
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return;
			default:
				reader.MoveToContent();
				if (reader.EOF)
				{
					return;
				}
				reader.Read();
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					return;
				}
				if (reader.Value.Length > 0)
				{
					if (string.Compare("Null", reader.Value) == 0)
					{
						this.notNull = false;
						return;
					}
					this.value = reader.Value;
					this.notNull = true;
					this.compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				}
				return;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(this.ToString());
		}

		public CompareInfo CompareInfo
		{
			get
			{
				return new CultureInfo(this.lcid).CompareInfo;
			}
		}

		public CultureInfo CultureInfo
		{
			get
			{
				return new CultureInfo(this.lcid);
			}
		}

		public bool IsNull
		{
			get
			{
				return !this.notNull;
			}
		}

		public int LCID
		{
			get
			{
				return this.lcid;
			}
		}

		public SqlCompareOptions SqlCompareOptions
		{
			get
			{
				return this.compareOptions;
			}
		}

		public string Value
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException(Locale.GetText("The property contains Null."));
				}
				return this.value;
			}
		}

		private CompareOptions CompareOptions
		{
			get
			{
				return (CompareOptions)(((this.compareOptions & SqlCompareOptions.BinarySort) == SqlCompareOptions.None) ? (this.compareOptions & (SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreNonSpace | SqlCompareOptions.IgnoreWidth)) : ((SqlCompareOptions)1073741824));
			}
		}

		public SqlString Clone()
		{
			return new SqlString(this.value, this.lcid, this.compareOptions);
		}

		public static CompareOptions CompareOptionsFromSqlCompareOptions(SqlCompareOptions compareOptions)
		{
			CompareOptions compareOptions2 = CompareOptions.None;
			if ((compareOptions & SqlCompareOptions.IgnoreCase) != SqlCompareOptions.None)
			{
				compareOptions2 |= CompareOptions.IgnoreCase;
			}
			if ((compareOptions & SqlCompareOptions.IgnoreKanaType) != SqlCompareOptions.None)
			{
				compareOptions2 |= CompareOptions.IgnoreKanaType;
			}
			if ((compareOptions & SqlCompareOptions.IgnoreNonSpace) != SqlCompareOptions.None)
			{
				compareOptions2 |= CompareOptions.IgnoreNonSpace;
			}
			if ((compareOptions & SqlCompareOptions.IgnoreWidth) != SqlCompareOptions.None)
			{
				compareOptions2 |= CompareOptions.IgnoreWidth;
			}
			if ((compareOptions & SqlCompareOptions.BinarySort) != SqlCompareOptions.None)
			{
				throw new ArgumentOutOfRangeException();
			}
			return compareOptions2;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlString))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Data.SqlTypes.SqlString"));
			}
			return this.CompareSqlString((SqlString)value);
		}

		private int CompareSqlString(SqlString value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			if (value.CompareOptions != this.CompareOptions)
			{
				throw new SqlTypeException(Locale.GetText("Two strings to be compared have different collation"));
			}
			return this.CultureInfo.CompareInfo.Compare(this.value, value.Value, this.CompareOptions);
		}

		public static SqlString Concat(SqlString x, SqlString y)
		{
			return x + y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlString))
			{
				return false;
			}
			if (this.IsNull)
			{
				return ((SqlString)value).IsNull;
			}
			return !((SqlString)value).IsNull && (bool)(this == (SqlString)value);
		}

		public static SqlBoolean Equals(SqlString x, SqlString y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			int num = 10;
			for (int i = 0; i < this.value.Length; i++)
			{
				num = 91 * num + (int)(this.value[i] ^ this.value[i]);
			}
			num = 91 * num + this.lcid.GetHashCode();
			return (int)(91 * num + this.compareOptions);
		}

		public byte[] GetNonUnicodeBytes()
		{
			return Encoding.ASCII.GetBytes(this.value);
		}

		public byte[] GetUnicodeBytes()
		{
			return Encoding.Unicode.GetBytes(this.value);
		}

		public static SqlBoolean GreaterThan(SqlString x, SqlString y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlString x, SqlString y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlString x, SqlString y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlString x, SqlString y)
		{
			return x <= y;
		}

		public static SqlBoolean NotEquals(SqlString x, SqlString y)
		{
			return x != y;
		}

		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		public SqlByte ToSqlByte()
		{
			return (SqlByte)this;
		}

		public SqlDateTime ToSqlDateTime()
		{
			return (SqlDateTime)this;
		}

		public SqlDecimal ToSqlDecimal()
		{
			return (SqlDecimal)this;
		}

		public SqlDouble ToSqlDouble()
		{
			return (SqlDouble)this;
		}

		public SqlGuid ToSqlGuid()
		{
			return (SqlGuid)this;
		}

		public SqlInt16 ToSqlInt16()
		{
			return (SqlInt16)this;
		}

		public SqlInt32 ToSqlInt32()
		{
			return (SqlInt32)this;
		}

		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
		}

		public SqlMoney ToSqlMoney()
		{
			return (SqlMoney)this;
		}

		public SqlSingle ToSqlSingle()
		{
			return (SqlSingle)this;
		}

		public override string ToString()
		{
			if (!this.notNull)
			{
				return "Null";
			}
			return (string)this;
		}

		public static SqlString Add(SqlString x, SqlString y)
		{
			return x + y;
		}

		public int CompareTo(SqlString value)
		{
			return this.CompareSqlString(value);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			if (schemaSet != null && schemaSet.Count == 0)
			{
				XmlSchema xmlSchema = new XmlSchema();
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = "string";
				xmlSchema.Items.Add(xmlSchemaComplexType);
				schemaSet.Add(xmlSchema);
			}
			return new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlString operator +(SqlString x, SqlString y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value + y.Value);
		}

		public static SqlBoolean operator ==(SqlString x, SqlString y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlBoolean operator >(SqlString x, SqlString y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.CompareTo(y) > 0);
		}

		public static SqlBoolean operator >=(SqlString x, SqlString y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.CompareTo(y) >= 0);
		}

		public static SqlBoolean operator !=(SqlString x, SqlString y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value != y.Value);
		}

		public static SqlBoolean operator <(SqlString x, SqlString y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.CompareTo(y) < 0);
		}

		public static SqlBoolean operator <=(SqlString x, SqlString y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.CompareTo(y) <= 0);
		}

		public static explicit operator SqlString(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator SqlString(SqlByte x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator SqlString(SqlDateTime x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator SqlString(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator SqlString(SqlDouble x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator SqlString(SqlGuid x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator SqlString(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator SqlString(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator SqlString(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator SqlString(SqlMoney x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.ToString());
		}

		public static explicit operator SqlString(SqlSingle x)
		{
			if (x.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(x.Value.ToString());
		}

		public static explicit operator string(SqlString x)
		{
			return x.Value;
		}

		public static implicit operator SqlString(string x)
		{
			return new SqlString(x);
		}

		private string value;

		private bool notNull;

		private int lcid;

		private SqlCompareOptions compareOptions;

		public static readonly int BinarySort = 32768;

		public static readonly int IgnoreCase = 1;

		public static readonly int IgnoreKanaType = 8;

		public static readonly int IgnoreNonSpace = 2;

		public static readonly int IgnoreWidth = 16;

		public static readonly SqlString Null;

		internal static NumberFormatInfo DecimalFormat = (NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();
	}
}
