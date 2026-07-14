using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class ByteMemoryFormatter : IYamlFormatter<Memory<byte>>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, Memory<byte> value, [Nullable(1)] YamlSerializationContext context)
		{
			emitter.WriteString(value.Span.ToBase64String(Base64FormattingOptions.None), ScalarStyle.Plain);
		}

		public Memory<byte> Deserialize(ref YamlParser parser, [Nullable(1)] YamlDeserializationContext context)
		{
			return Convert.FromBase64String(parser.ReadScalarAsString());
		}

		[Nullable(1)]
		public static readonly ByteMemoryFormatter Instance = new ByteMemoryFormatter();
	}
}
