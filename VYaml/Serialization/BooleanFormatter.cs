using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class BooleanFormatter : IYamlFormatter<bool>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, bool value, YamlSerializationContext context)
		{
			emitter.WriteBool(value);
		}

		[NullableContext(1)]
		public bool Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			bool scalarAsBool = parser.GetScalarAsBool();
			parser.Read();
			return scalarAsBool;
		}

		[Nullable(1)]
		public static readonly BooleanFormatter Instance = new BooleanFormatter();
	}
}
