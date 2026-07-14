using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class NullableStringFormatter : IYamlFormatter<string>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] string value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			emitter.WriteString(value, ScalarStyle.Any);
		}

		[return: Nullable(2)]
		public string Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			return parser.ReadScalarAsString();
		}

		public static readonly NullableStringFormatter Instance = new NullableStringFormatter();
	}
}
