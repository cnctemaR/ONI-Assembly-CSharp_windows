using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ScalarNodeDeserializer : INodeDeserializer
	{
		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			Scalar scalar = parser.Allow<Scalar>();
			if (scalar == null)
			{
				value = null;
				return false;
			}
			if (expectedType.IsEnum())
			{
				value = Enum.Parse(expectedType, scalar.Value, true);
			}
			else
			{
				TypeCode typeCode = expectedType.GetTypeCode();
				switch (typeCode)
				{
				case TypeCode.Boolean:
					value = this.DeserializeBooleanHelper(scalar.Value);
					return true;
				case TypeCode.Char:
					value = scalar.Value[0];
					return true;
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					value = this.DeserializeIntegerHelper(typeCode, scalar.Value);
					return true;
				case TypeCode.Single:
					value = float.Parse(scalar.Value, YamlFormatter.NumberFormat);
					return true;
				case TypeCode.Double:
					value = double.Parse(scalar.Value, YamlFormatter.NumberFormat);
					return true;
				case TypeCode.Decimal:
					value = decimal.Parse(scalar.Value, YamlFormatter.NumberFormat);
					return true;
				case TypeCode.DateTime:
					value = DateTime.Parse(scalar.Value, CultureInfo.InvariantCulture);
					return true;
				case TypeCode.String:
					value = scalar.Value;
					return true;
				}
				if (expectedType == typeof(object))
				{
					value = scalar.Value;
				}
				else
				{
					value = TypeConverter.ChangeType(scalar.Value, expectedType);
				}
			}
			return true;
		}

		private object DeserializeBooleanHelper(string value)
		{
			bool flag;
			if (Regex.IsMatch(value, "^(true|y|yes|on)$", RegexOptions.IgnoreCase))
			{
				flag = true;
			}
			else
			{
				if (!Regex.IsMatch(value, "^(false|n|no|off)$", RegexOptions.IgnoreCase))
				{
					throw new FormatException(string.Format("The value \"{0}\" is not a valid YAML Boolean", value));
				}
				flag = false;
			}
			return flag;
		}

		private object DeserializeIntegerHelper(TypeCode typeCode, string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			bool flag = false;
			ulong num = 0UL;
			if (value[0] == '-')
			{
				i++;
				flag = true;
			}
			else if (value[0] == '+')
			{
				i++;
			}
			if (value[i] == '0')
			{
				int num2;
				if (i == value.Length - 1)
				{
					num2 = 10;
					num = 0UL;
				}
				else
				{
					i++;
					if (value[i] == 'b')
					{
						num2 = 2;
						i++;
					}
					else if (value[i] == 'x')
					{
						num2 = 16;
						i++;
					}
					else
					{
						num2 = 8;
					}
				}
				while (i < value.Length)
				{
					if (value[i] != '_')
					{
						stringBuilder.Append(value[i]);
					}
					i++;
				}
				if (num2 <= 8)
				{
					if (num2 == 2 || num2 == 8)
					{
						num = Convert.ToUInt64(stringBuilder.ToString(), num2);
					}
				}
				else if (num2 != 10)
				{
					if (num2 == 16)
					{
						num = ulong.Parse(stringBuilder.ToString(), NumberStyles.HexNumber, YamlFormatter.NumberFormat);
					}
				}
			}
			else
			{
				string[] array = value.Substring(i).Split(':', StringSplitOptions.None);
				num = 0UL;
				for (int j = 0; j < array.Length; j++)
				{
					num *= 60UL;
					num += ulong.Parse(array[j].Replace("_", ""));
				}
			}
			if (flag)
			{
				return ScalarNodeDeserializer.CastInteger(checked(0L - (long)num), typeCode);
			}
			return ScalarNodeDeserializer.CastInteger(num, typeCode);
		}

		private static object CastInteger(long number, TypeCode typeCode)
		{
			checked
			{
				switch (typeCode)
				{
				case TypeCode.SByte:
					return (sbyte)number;
				case TypeCode.Byte:
					return (byte)number;
				case TypeCode.Int16:
					return (short)number;
				case TypeCode.UInt16:
					return (ushort)number;
				case TypeCode.Int32:
					return (int)number;
				case TypeCode.UInt32:
					return (uint)number;
				case TypeCode.Int64:
					return number;
				case TypeCode.UInt64:
					return (ulong)number;
				default:
					return number;
				}
			}
		}

		private static object CastInteger(ulong number, TypeCode typeCode)
		{
			checked
			{
				switch (typeCode)
				{
				case TypeCode.SByte:
					return (sbyte)number;
				case TypeCode.Byte:
					return (byte)number;
				case TypeCode.Int16:
					return (short)number;
				case TypeCode.UInt16:
					return (ushort)number;
				case TypeCode.Int32:
					return (int)number;
				case TypeCode.UInt32:
					return (uint)number;
				case TypeCode.Int64:
					return (long)number;
				case TypeCode.UInt64:
					return number;
				default:
					return number;
				}
			}
		}

		private const string BooleanTruePattern = "^(true|y|yes|on)$";

		private const string BooleanFalsePattern = "^(false|n|no|off)$";
	}
}
