using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class ComplexFormatter : IYamlFormatter<Complex>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, Complex value, YamlSerializationContext context)
		{
			emitter.WriteString(string.Format("{0}+{1}i", value.Real, value.Imaginary), ScalarStyle.Any);
		}

		[NullableContext(1)]
		public Complex Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				return default(Complex);
			}
			string text = parser.ReadScalarAsString();
			int num = text.IndexOf('+');
			double num2 = double.Parse(text.AsSpan(0, num), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, null);
			double num3 = double.Parse(text.AsSpan(num + 1, text.Length - num - 2), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, null);
			return new Complex(num2, num3);
		}

		[Nullable(1)]
		public static readonly ComplexFormatter Instance = new ComplexFormatter();
	}
}
