using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class Float32Formatter : IYamlFormatter<float>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, float value, YamlSerializationContext context)
		{
			emitter.WriteFloat(value);
		}

		[NullableContext(1)]
		public float Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			float scalarAsFloat = parser.GetScalarAsFloat();
			parser.Read();
			return scalarAsFloat;
		}

		[Nullable(1)]
		public static readonly Float32Formatter Instance = new Float32Formatter();
	}
}
