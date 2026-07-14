using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableFloat64Formatter : IYamlFormatter<double?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, double? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteDouble(value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public double? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			double scalarAsDouble = parser.GetScalarAsDouble();
			parser.Read();
			return new double?(scalarAsDouble);
		}

		[Nullable(1)]
		public static readonly NullableFloat64Formatter Instance = new NullableFloat64Formatter();
	}
}
