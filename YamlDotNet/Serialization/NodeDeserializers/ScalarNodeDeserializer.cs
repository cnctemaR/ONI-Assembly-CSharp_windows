using System;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ScalarNodeDeserializer : INodeDeserializer
	{
		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			Scalar scalar = reader.Allow<Scalar>();
			if (scalar == null)
			{
				value = null;
				return false;
			}
			if (expectedType.IsEnum())
			{
				value = Enum.Parse(expectedType, scalar.Value);
			}
			else
			{
				switch (expectedType.GetTypeCode())
				{
				case TypeCode.Boolean:
					value = bool.Parse(scalar.Value);
					return true;
				case TypeCode.Char:
					value = scalar.Value[0];
					return true;
				case TypeCode.SByte:
					value = sbyte.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.Byte:
					value = byte.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.Int16:
					value = short.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.UInt16:
					value = ushort.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.Int32:
					value = int.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.UInt32:
					value = uint.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.Int64:
					value = long.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.UInt64:
					value = ulong.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.Single:
					value = float.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.Double:
					value = double.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
					return true;
				case TypeCode.Decimal:
					value = decimal.Parse(scalar.Value, ScalarNodeDeserializer.numberFormat);
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

		private static readonly NumberFormatInfo numberFormat = new NumberFormatInfo
		{
			CurrencyDecimalSeparator = ".",
			CurrencyGroupSeparator = "_",
			CurrencyGroupSizes = new int[] { 3 },
			CurrencySymbol = string.Empty,
			CurrencyDecimalDigits = 99,
			NumberDecimalSeparator = ".",
			NumberGroupSeparator = "_",
			NumberGroupSizes = new int[] { 3 },
			NumberDecimalDigits = 99
		};
	}
}
