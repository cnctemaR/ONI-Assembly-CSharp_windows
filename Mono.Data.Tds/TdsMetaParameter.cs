using System;
using System.Collections.Generic;
using System.Text;
using Mono.Data.Tds.Protocol;

namespace Mono.Data.Tds
{
	public class TdsMetaParameter
	{
		public TdsMetaParameter(string name, object value)
			: this(name, string.Empty, value)
		{
		}

		public TdsMetaParameter(string name, FrameworkValueGetter valueGetter)
			: this(name, string.Empty, null)
		{
			this.frameworkValueGetter = valueGetter;
		}

		public TdsMetaParameter(string name, string typeName, object value)
		{
			this.ParameterName = name;
			this.Value = value;
			this.TypeName = typeName;
			this.IsNullable = false;
		}

		public TdsMetaParameter(string name, int size, bool isNullable, byte precision, byte scale, object value)
		{
			this.ParameterName = name;
			this.Size = size;
			this.IsNullable = isNullable;
			this.Precision = precision;
			this.Scale = scale;
			this.Value = value;
		}

		public TdsMetaParameter(string name, int size, bool isNullable, byte precision, byte scale, FrameworkValueGetter valueGetter)
		{
			this.ParameterName = name;
			this.Size = size;
			this.IsNullable = isNullable;
			this.Precision = precision;
			this.Scale = scale;
			this.frameworkValueGetter = valueGetter;
		}

		public TdsParameterDirection Direction
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

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
			set
			{
				this.typeName = value;
			}
		}

		public string ParameterName
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

		public bool IsNullable
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

		public object Value
		{
			get
			{
				if (this.frameworkValueGetter != null)
				{
					object obj = this.frameworkValueGetter(this.rawValue, ref this.isUpdated);
					if (this.isUpdated)
					{
						this.value = obj;
					}
				}
				if (this.isUpdated)
				{
					this.value = this.ResizeValue(this.value);
					this.isUpdated = false;
				}
				return this.value;
			}
			set
			{
				this.value = value;
				this.rawValue = value;
				this.isUpdated = true;
			}
		}

		public object RawValue
		{
			get
			{
				return this.rawValue;
			}
			set
			{
				this.Value = value;
			}
		}

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

		public byte Scale
		{
			get
			{
				if ((this.TypeName == "decimal" || this.TypeName == "numeric") && this.scale == 0 && !Convert.IsDBNull(this.Value))
				{
					int[] bits = decimal.GetBits(Convert.ToDecimal(this.Value));
					this.scale = (byte)((bits[3] >> 16) & 255);
				}
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		public int Size
		{
			get
			{
				return this.GetSize();
			}
			set
			{
				this.size = value;
				this.isUpdated = true;
				this.isSizeSet = true;
			}
		}

		public bool IsVariableSizeType
		{
			get
			{
				return this.isVariableSizeType;
			}
			set
			{
				this.isVariableSizeType = value;
			}
		}

		private object ResizeValue(object newValue)
		{
			if (newValue == DBNull.Value || newValue == null)
			{
				return newValue;
			}
			if (!this.isSizeSet || this.size <= 0)
			{
				return newValue;
			}
			string text = newValue as string;
			if (text != null)
			{
				if ((this.TypeName == "nvarchar" || this.TypeName == "nchar" || this.TypeName == "xml") && text.Length > this.size)
				{
					return text.Substring(0, this.size);
				}
			}
			else if (newValue.GetType() == typeof(byte[]))
			{
				byte[] array = (byte[])newValue;
				if (array.Length > this.size)
				{
					byte[] array2 = new byte[this.size];
					Array.Copy(array, array2, this.size);
					return array2;
				}
			}
			return newValue;
		}

		internal string Prepare()
		{
			string text = this.TypeName;
			if (text == "varbinary")
			{
				int actualSize = this.Size;
				if (actualSize <= 0)
				{
					actualSize = this.GetActualSize();
				}
				if (actualSize > 8000)
				{
					text = "image";
				}
			}
			string text2 = "@";
			if (this.ParameterName[0] == '@')
			{
				text2 = string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(string.Format("{0}{1} {2}", text2, this.ParameterName, text));
			string text3 = text;
			switch (text3)
			{
			case "decimal":
			case "numeric":
				stringBuilder.Append(string.Format("({0},{1})", (this.Precision != 0) ? this.Precision : 29, this.Scale));
				break;
			case "varchar":
			case "varbinary":
			{
				int num2 = this.Size;
				if (num2 <= 0)
				{
					num2 = this.GetActualSize();
					if (num2 <= 0)
					{
						num2 = 1;
					}
				}
				stringBuilder.Append((num2 <= 8000) ? string.Format("({0})", num2) : "(max)");
				break;
			}
			case "nvarchar":
			case "xml":
				stringBuilder.Append((this.Size <= 0) ? "(4000)" : ((this.Size <= 8000) ? string.Format("({0})", this.Size) : "(max)"));
				break;
			case "char":
			case "nchar":
			case "binary":
				if (this.isSizeSet && this.Size > 0)
				{
					stringBuilder.Append(string.Format("({0})", this.Size));
				}
				break;
			}
			return stringBuilder.ToString();
		}

		internal int GetActualSize()
		{
			if (this.Value == DBNull.Value || this.Value == null)
			{
				return 0;
			}
			string text = this.Value.GetType().ToString();
			if (text != null)
			{
				if (TdsMetaParameter.<>f__switch$map1 == null)
				{
					TdsMetaParameter.<>f__switch$map1 = new Dictionary<string, int>(2)
					{
						{ "System.String", 0 },
						{ "System.Byte[]", 1 }
					};
				}
				int num;
				if (TdsMetaParameter.<>f__switch$map1.TryGetValue(text, out num))
				{
					if (num == 0)
					{
						int num2 = ((string)this.value).Length;
						if (this.TypeName == "nvarchar" || this.TypeName == "nchar" || this.TypeName == "ntext" || this.TypeName == "xml")
						{
							num2 *= 2;
						}
						return num2;
					}
					if (num == 1)
					{
						return ((byte[])this.value).Length;
					}
				}
			}
			return this.GetSize();
		}

		private int GetSize()
		{
			string text = this.TypeName;
			switch (text)
			{
			case "decimal":
				return 17;
			case "uniqueidentifier":
				return 16;
			case "bigint":
			case "datetime":
			case "float":
			case "money":
				return 8;
			case "int":
			case "real":
			case "smalldatetime":
			case "smallmoney":
				return 4;
			case "smallint":
				return 2;
			case "tinyint":
			case "bit":
				return 1;
			case "nchar":
			case "ntext":
				return this.size * 2;
			}
			return this.size;
		}

		internal byte[] GetBytes()
		{
			byte[] array = new byte[0];
			if (this.Value == DBNull.Value || this.Value == null)
			{
				return array;
			}
			string text = this.TypeName;
			if (text != null)
			{
				if (TdsMetaParameter.<>f__switch$map3 == null)
				{
					TdsMetaParameter.<>f__switch$map3 = new Dictionary<string, int>(7)
					{
						{ "nvarchar", 0 },
						{ "nchar", 0 },
						{ "ntext", 0 },
						{ "xml", 0 },
						{ "varchar", 1 },
						{ "char", 1 },
						{ "text", 1 }
					};
				}
				int num;
				if (TdsMetaParameter.<>f__switch$map3.TryGetValue(text, out num))
				{
					if (num == 0)
					{
						return Encoding.Unicode.GetBytes((string)this.Value);
					}
					if (num == 1)
					{
						return Encoding.Default.GetBytes((string)this.Value);
					}
				}
			}
			return (byte[])this.Value;
		}

		internal TdsColumnType GetMetaType()
		{
			string text = this.TypeName;
			switch (text)
			{
			case "binary":
				return TdsColumnType.BigBinary;
			case "bit":
				if (this.IsNullable)
				{
					return TdsColumnType.BitN;
				}
				return TdsColumnType.Bit;
			case "bigint":
				if (this.IsNullable)
				{
					return TdsColumnType.IntN;
				}
				return TdsColumnType.BigInt;
			case "char":
				return TdsColumnType.Char;
			case "money":
				if (this.IsNullable)
				{
					return TdsColumnType.MoneyN;
				}
				return TdsColumnType.Money;
			case "smallmoney":
				if (this.IsNullable)
				{
					return TdsColumnType.MoneyN;
				}
				return TdsColumnType.Money4;
			case "decimal":
				return TdsColumnType.Decimal;
			case "datetime":
				if (this.IsNullable)
				{
					return TdsColumnType.DateTimeN;
				}
				return TdsColumnType.DateTime;
			case "smalldatetime":
				if (this.IsNullable)
				{
					return TdsColumnType.DateTimeN;
				}
				return TdsColumnType.DateTime4;
			case "float":
				if (this.IsNullable)
				{
					return TdsColumnType.FloatN;
				}
				return TdsColumnType.Float8;
			case "image":
				return TdsColumnType.Image;
			case "int":
				if (this.IsNullable)
				{
					return TdsColumnType.IntN;
				}
				return TdsColumnType.Int4;
			case "numeric":
				return TdsColumnType.Numeric;
			case "nchar":
				return TdsColumnType.NChar;
			case "ntext":
				return TdsColumnType.NText;
			case "xml":
			case "nvarchar":
				return TdsColumnType.BigNVarChar;
			case "real":
				if (this.IsNullable)
				{
					return TdsColumnType.FloatN;
				}
				return TdsColumnType.Real;
			case "smallint":
				if (this.IsNullable)
				{
					return TdsColumnType.IntN;
				}
				return TdsColumnType.Int2;
			case "text":
				return TdsColumnType.Text;
			case "tinyint":
				if (this.IsNullable)
				{
					return TdsColumnType.IntN;
				}
				return TdsColumnType.Int1;
			case "uniqueidentifier":
				return TdsColumnType.UniqueIdentifier;
			case "varbinary":
				return TdsColumnType.BigVarBinary;
			case "varchar":
				return TdsColumnType.BigVarChar;
			}
			throw new NotSupportedException("Unknown Type : " + this.TypeName);
		}

		public void Validate(int index)
		{
			if ((this.direction == TdsParameterDirection.InputOutput || this.direction == TdsParameterDirection.Output) && this.isVariableSizeType && (this.Value == DBNull.Value || this.Value == null) && this.Size == 0)
			{
				throw new InvalidOperationException(string.Format("{0}[{1}]: the Size property should not be of size 0", this.typeName, index));
			}
		}

		private TdsParameterDirection direction;

		private byte precision;

		private byte scale;

		private int size;

		private string typeName;

		private string name;

		private bool isSizeSet;

		private bool isNullable;

		private object value;

		private bool isVariableSizeType;

		private FrameworkValueGetter frameworkValueGetter;

		private object rawValue;

		private bool isUpdated;
	}
}
