using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class Float64Formatter : IYamlFormatter<double>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, double value, YamlSerializationContext context)
		{
			emitter.WriteDouble(value);
		}

		[NullableContext(1)]
		public double Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			double scalarAsDouble = parser.GetScalarAsDouble();
			parser.Read();
			return scalarAsDouble;
		}

		[Nullable(1)]
		public static readonly Float64Formatter Instance = new Float64Formatter();
	}
}
