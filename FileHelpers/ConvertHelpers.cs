using System;
using System.Globalization;
using System.Text;

namespace FileHelpers
{
	internal static class ConvertHelpers
	{
		private static CultureInfo CreateCulture(string decimalSep)
		{
			CultureInfo cultureInfo = new CultureInfo(CultureInfo.CurrentCulture.LCID);
			if (decimalSep == ".")
			{
				cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
				cultureInfo.NumberFormat.NumberGroupSeparator = ",";
			}
			else
			{
				if (!(decimalSep == ","))
				{
					throw new BadUsageException("You can only use '.' or ',' as decimal or group separators");
				}
				cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
				cultureInfo.NumberFormat.NumberGroupSeparator = ".";
			}
			return cultureInfo;
		}

		internal static ConverterBase GetDefaultConverter(string fieldName, Type fieldType)
		{
			if (fieldType.IsArray)
			{
				if (fieldType.GetArrayRank() != 1)
				{
					throw new BadUsageException("The array field: '" + fieldName + "' has more than one dimension and is not supported by the library.");
				}
				fieldType = fieldType.GetElementType();
				if (fieldType.IsArray)
				{
					throw new BadUsageException("The array field: '" + fieldName + "' is a jagged array and is not supported by the library.");
				}
			}
			if (fieldType.IsValueType && fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				fieldType = fieldType.GetGenericArguments()[0];
			}
			if (fieldType == typeof(string))
			{
				return null;
			}
			if (fieldType == typeof(short))
			{
				return new ConvertHelpers.Int16Converter();
			}
			if (fieldType == typeof(int))
			{
				return new ConvertHelpers.Int32Converter();
			}
			if (fieldType == typeof(long))
			{
				return new ConvertHelpers.Int64Converter();
			}
			if (fieldType == typeof(sbyte))
			{
				return new ConvertHelpers.SByteConverter();
			}
			if (fieldType == typeof(ushort))
			{
				return new ConvertHelpers.UInt16Converter();
			}
			if (fieldType == typeof(uint))
			{
				return new ConvertHelpers.UInt32Converter();
			}
			if (fieldType == typeof(ulong))
			{
				return new ConvertHelpers.UInt64Converter();
			}
			if (fieldType == typeof(byte))
			{
				return new ConvertHelpers.ByteConverter();
			}
			if (fieldType == typeof(decimal))
			{
				return new ConvertHelpers.DecimalConverter();
			}
			if (fieldType == typeof(double))
			{
				return new ConvertHelpers.DoubleConverter();
			}
			if (fieldType == typeof(float))
			{
				return new ConvertHelpers.SingleConverter();
			}
			if (fieldType == typeof(DateTime))
			{
				return new ConvertHelpers.DateTimeConverter();
			}
			if (fieldType == typeof(bool))
			{
				return new ConvertHelpers.BooleanConverter();
			}
			if (fieldType == typeof(char))
			{
				return new ConvertHelpers.CharConverter();
			}
			if (fieldType == typeof(Guid))
			{
				return new ConvertHelpers.GuidConverter();
			}
			if (fieldType.IsEnum)
			{
				return new EnumConverter(fieldType);
			}
			throw new BadUsageException(string.Concat(new string[] { "The field: '", fieldName, "' has the type: ", fieldType.Name, " that is not a system type, so this field need a CustomConverter ( Please Check the docs for more Info)." }));
		}

		private const string DefaultDecimalSep = ".";

		internal abstract class CultureConverter : ConverterBase
		{
			protected CultureConverter(Type T, string decimalSep)
			{
				this.mCulture = ConvertHelpers.CreateCulture(decimalSep);
				this.mType = T;
			}

			public sealed override string FieldToString(object from)
			{
				if (from == null)
				{
					return string.Empty;
				}
				return ((IConvertible)from).ToString(this.mCulture);
			}

			public sealed override object StringToField(string from)
			{
				return this.ParseString(from);
			}

			protected abstract object ParseString(string from);

			protected CultureInfo mCulture;

			protected Type mType;
		}

		internal sealed class ByteConverter : ConvertHelpers.CultureConverter
		{
			public ByteConverter()
				: this(".")
			{
			}

			public ByteConverter(string decimalSep)
				: base(typeof(byte), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				byte b;
				if (!byte.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.Number, this.mCulture, out b))
				{
					throw new ConvertException(from, this.mType);
				}
				return b;
			}
		}

		internal sealed class UInt16Converter : ConvertHelpers.CultureConverter
		{
			public UInt16Converter()
				: this(".")
			{
			}

			public UInt16Converter(string decimalSep)
				: base(typeof(ushort), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				ushort num;
				if (!ushort.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
				{
					throw new ConvertException(from, this.mType);
				}
				return num;
			}
		}

		internal sealed class UInt32Converter : ConvertHelpers.CultureConverter
		{
			public UInt32Converter()
				: this(".")
			{
			}

			public UInt32Converter(string decimalSep)
				: base(typeof(uint), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				uint num;
				if (!uint.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
				{
					throw new ConvertException(from, this.mType);
				}
				return num;
			}
		}

		internal sealed class UInt64Converter : ConvertHelpers.CultureConverter
		{
			public UInt64Converter()
				: this(".")
			{
			}

			public UInt64Converter(string decimalSep)
				: base(typeof(ulong), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				ulong num;
				if (!ulong.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
				{
					throw new ConvertException(from, this.mType);
				}
				return num;
			}
		}

		internal sealed class SByteConverter : ConvertHelpers.CultureConverter
		{
			public SByteConverter()
				: this(".")
			{
			}

			public SByteConverter(string decimalSep)
				: base(typeof(sbyte), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				sbyte b;
				if (!sbyte.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.Number, this.mCulture, out b))
				{
					throw new ConvertException(from, this.mType);
				}
				return b;
			}
		}

		internal sealed class Int16Converter : ConvertHelpers.CultureConverter
		{
			public Int16Converter()
				: this(".")
			{
			}

			public Int16Converter(string decimalSep)
				: base(typeof(short), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				short num;
				if (!short.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
				{
					throw new ConvertException(from, this.mType);
				}
				return num;
			}
		}

		internal sealed class Int32Converter : ConvertHelpers.CultureConverter
		{
			public Int32Converter()
				: this(".")
			{
			}

			public Int32Converter(string decimalSep)
				: base(typeof(int), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				int num;
				if (!int.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
				{
					throw new ConvertException(from, this.mType);
				}
				return num;
			}
		}

		internal sealed class Int64Converter : ConvertHelpers.CultureConverter
		{
			public Int64Converter()
				: this(".")
			{
			}

			public Int64Converter(string decimalSep)
				: base(typeof(long), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				long num;
				if (!long.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
				{
					throw new ConvertException(from, this.mType);
				}
				return num;
			}
		}

		internal sealed class DecimalConverter : ConvertHelpers.CultureConverter
		{
			public DecimalConverter()
				: this(".")
			{
			}

			public DecimalConverter(string decimalSep)
				: base(typeof(decimal), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				decimal num;
				if (!decimal.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
				{
					throw new ConvertException(from, this.mType);
				}
				return num;
			}
		}

		internal sealed class SingleConverter : ConvertHelpers.CultureConverter
		{
			public SingleConverter()
				: this(".")
			{
			}

			public SingleConverter(string decimalSep)
				: base(typeof(float), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				float num;
				if (!float.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
				{
					throw new ConvertException(from, this.mType);
				}
				return num;
			}
		}

		internal sealed class DoubleConverter : ConvertHelpers.CultureConverter
		{
			public DoubleConverter()
				: this(".")
			{
			}

			public DoubleConverter(string decimalSep)
				: base(typeof(double), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				double num;
				if (!double.TryParse(StringHelper.RemoveBlanks(from), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
				{
					throw new ConvertException(from, this.mType);
				}
				return num;
			}
		}

		internal sealed class PercentDoubleConverter : ConvertHelpers.CultureConverter
		{
			public PercentDoubleConverter()
				: this(".")
			{
			}

			public PercentDoubleConverter(string decimalSep)
				: base(typeof(double), decimalSep)
			{
			}

			protected override object ParseString(string from)
			{
				string text = StringHelper.RemoveBlanks(from);
				if (text.EndsWith("%"))
				{
					double num;
					if (!double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
					{
						throw new ConvertException(from, this.mType);
					}
					return num / 100.0;
				}
				else
				{
					double num;
					if (!double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.mCulture, out num))
					{
						throw new ConvertException(from, this.mType);
					}
					return num;
				}
			}
		}

		internal sealed class DateTimeConverter : ConverterBase
		{
			public DateTimeConverter()
				: this(ConverterBase.DefaultDateTimeFormat)
			{
			}

			public DateTimeConverter(string format)
			{
				if (string.IsNullOrEmpty(format))
				{
					throw new BadUsageException("The format of the DateTime Converter cannot be null or empty.");
				}
				try
				{
					DateTime.Now.ToString(format);
				}
				catch
				{
					throw new BadUsageException("The format: '" + format + " is invalid for the DateTime Converter.");
				}
				this.mFormat = format;
			}

			public override object StringToField(string from)
			{
				if (from == null)
				{
					from = string.Empty;
				}
				DateTime dateTime;
				if (!DateTime.TryParseExact(from.Trim(), this.mFormat, null, DateTimeStyles.None, out dateTime))
				{
					string text;
					if (from.Length > this.mFormat.Length)
					{
						text = " There are more chars in the Input String than in the Format string: '" + this.mFormat + "'";
					}
					else if (from.Length < this.mFormat.Length)
					{
						text = " There are less chars in the Input String than in the Format string: '" + this.mFormat + "'";
					}
					else
					{
						text = " Using the format: '" + this.mFormat + "'";
					}
					throw new ConvertException(from, typeof(DateTime), text);
				}
				return dateTime;
			}

			public override string FieldToString(object from)
			{
				if (from == null)
				{
					return string.Empty;
				}
				return Convert.ToDateTime(from).ToString(this.mFormat);
			}

			private readonly string mFormat;
		}

		internal sealed class DateTimeMultiFormatConverter : ConverterBase
		{
			public DateTimeMultiFormatConverter(string format1, string format2)
				: this(new string[] { format1, format2 })
			{
			}

			public DateTimeMultiFormatConverter(string format1, string format2, string format3)
				: this(new string[] { format1, format2, format3 })
			{
			}

			private DateTimeMultiFormatConverter(string[] formats)
			{
				for (int i = 0; i < formats.Length; i++)
				{
					if (formats[i] == null || formats[i] == string.Empty)
					{
						throw new BadUsageException("The format of the DateTime Converter can be null or empty.");
					}
					try
					{
						DateTime.Now.ToString(formats[i]);
					}
					catch
					{
						throw new BadUsageException("The format: '" + formats[i] + " is invalid for the DateTime Converter.");
					}
				}
				this.mFormats = formats;
			}

			public override object StringToField(string from)
			{
				if (from == null)
				{
					from = string.Empty;
				}
				DateTime dateTime;
				if (!DateTime.TryParseExact(from.Trim(), this.mFormats, null, DateTimeStyles.None, out dateTime))
				{
					string text = " does not match any of the given formats: " + this.CreateFormats();
					throw new ConvertException(from, typeof(DateTime), text);
				}
				return dateTime;
			}

			private string CreateFormats()
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.mFormats.Length; i++)
				{
					if (i == 0)
					{
						stringBuilder.Append("'" + this.mFormats[i] + "'");
					}
					else
					{
						stringBuilder.Append(", '" + this.mFormats[i] + "'");
					}
				}
				return stringBuilder.ToString();
			}

			public override string FieldToString(object from)
			{
				if (from == null)
				{
					return string.Empty;
				}
				return Convert.ToDateTime(from).ToString(this.mFormats[0]);
			}

			private readonly string[] mFormats;
		}

		internal sealed class BooleanConverter : ConverterBase
		{
			public BooleanConverter()
			{
			}

			public BooleanConverter(string trueStr, string falseStr)
			{
				this.mTrueString = trueStr;
				this.mFalseString = falseStr;
				this.mTrueStringLower = trueStr.ToLower();
				this.mFalseStringLower = falseStr.ToLower();
			}

			public override object StringToField(string from)
			{
				string text = from.ToLower();
				if (this.mTrueString == null)
				{
					text = text.Trim();
					string text2;
					switch (text2 = text)
					{
					case "true":
					case "1":
					case "y":
					case "t":
						return true;
					case "false":
					case "0":
					case "n":
					case "f":
					case "":
						return false;
					}
					throw new ConvertException(from, typeof(bool), "The string: " + from + " can't be recognized as boolean using default true/false values.");
				}
				object obj;
				if (text == this.mTrueStringLower)
				{
					obj = true;
				}
				else if (text == this.mFalseStringLower)
				{
					obj = false;
				}
				else
				{
					text = text.Trim();
					if (text == this.mTrueStringLower)
					{
						obj = true;
					}
					else
					{
						if (!(text == this.mFalseStringLower))
						{
							throw new ConvertException(from, typeof(bool), string.Concat(new string[] { "The string: ", from, " can't be recognized as boolean using the true/false values: ", this.mTrueString, "/", this.mFalseString }));
						}
						obj = false;
					}
				}
				return obj;
			}

			public override string FieldToString(object from)
			{
				bool flag = Convert.ToBoolean(from);
				if (flag)
				{
					if (this.mTrueString == null)
					{
						return "True";
					}
					return this.mTrueString;
				}
				else
				{
					if (this.mFalseString == null)
					{
						return "False";
					}
					return this.mFalseString;
				}
			}

			private readonly string mTrueString;

			private readonly string mFalseString;

			private readonly string mTrueStringLower;

			private readonly string mFalseStringLower;
		}

		internal sealed class CharConverter : ConverterBase
		{
			public CharConverter()
				: this("")
			{
			}

			public CharConverter(string format)
			{
				string text;
				if ((text = format.Trim()) != null)
				{
					if (text == "x" || text == "lower")
					{
						this.mFormat = ConvertHelpers.CharConverter.CharFormat.Lower;
						return;
					}
					if (text == "X" || text == "upper")
					{
						this.mFormat = ConvertHelpers.CharConverter.CharFormat.Upper;
						return;
					}
					if (text == "")
					{
						this.mFormat = ConvertHelpers.CharConverter.CharFormat.NoChange;
						return;
					}
				}
				throw new BadUsageException("The format of the Char Converter must be \"\", \"x\" or \"lower\" for lower case, \"X\" or \"upper\" for upper case");
			}

			public override object StringToField(string from)
			{
				if (string.IsNullOrEmpty(from))
				{
					return '\0';
				}
				object obj;
				try
				{
					switch (this.mFormat)
					{
					case ConvertHelpers.CharConverter.CharFormat.NoChange:
						obj = from[0];
						break;
					case ConvertHelpers.CharConverter.CharFormat.Lower:
						obj = char.ToLower(from[0]);
						break;
					case ConvertHelpers.CharConverter.CharFormat.Upper:
						obj = char.ToUpper(from[0]);
						break;
					default:
						throw new ConvertException(from, typeof(char), "Unknown char convert flag " + this.mFormat.ToString());
					}
				}
				catch
				{
					throw new ConvertException(from, typeof(char), "Upper or lower case of input string failed");
				}
				return obj;
			}

			public override string FieldToString(object from)
			{
				switch (this.mFormat)
				{
				case ConvertHelpers.CharConverter.CharFormat.NoChange:
					return Convert.ToChar(from).ToString();
				case ConvertHelpers.CharConverter.CharFormat.Lower:
					return char.ToLower(Convert.ToChar(from)).ToString();
				case ConvertHelpers.CharConverter.CharFormat.Upper:
					return char.ToUpper(Convert.ToChar(from)).ToString();
				default:
					throw new ConvertException("", typeof(char), "Unknown char convert flag " + this.mFormat.ToString());
				}
			}

			private readonly ConvertHelpers.CharConverter.CharFormat mFormat;

			private enum CharFormat
			{
				NoChange,
				Lower,
				Upper
			}
		}

		internal sealed class GuidConverter : ConverterBase
		{
			public GuidConverter()
				: this("D")
			{
			}

			public GuidConverter(string format)
			{
				if (string.IsNullOrEmpty(format))
				{
					format = "D";
				}
				format = format.Trim().ToUpper();
				if (!(format == "N") && !(format == "D") && !(format == "B") && !(format == "P"))
				{
					throw new BadUsageException("The format of the Guid Converter must be N, D, B or P.");
				}
				this.mFormat = format;
			}

			public override object StringToField(string from)
			{
				if (string.IsNullOrEmpty(from))
				{
					return Guid.Empty;
				}
				object obj;
				try
				{
					obj = new Guid(from);
				}
				catch
				{
					throw new ConvertException(from, typeof(Guid));
				}
				return obj;
			}

			public override string FieldToString(object from)
			{
				if (from == null)
				{
					return string.Empty;
				}
				return ((Guid)from).ToString(this.mFormat);
			}

			private readonly string mFormat;
		}
	}
}
