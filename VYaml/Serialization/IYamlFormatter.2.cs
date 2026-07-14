using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	public interface IYamlFormatter<[Nullable(2)] T> : IYamlFormatter
	{
		void Serialize(ref Utf8YamlEmitter emitter, T value, YamlSerializationContext context);

		T Deserialize(ref YamlParser parser, YamlDeserializationContext context);
	}
}
