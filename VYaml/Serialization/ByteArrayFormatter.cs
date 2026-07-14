using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public sealed class ByteArrayFormatter : IYamlFormatter<byte[]>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] byte[] value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			emitter.WriteString(value.AsSpan<byte>().ToBase64String(Base64FormattingOptions.None), ScalarStyle.Plain);
		}

		[NullableContext(1)]
		[return: Nullable(2)]
		public byte[] Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			return Convert.FromBase64String(parser.ReadScalarAsString());
		}

		[Nullable(new byte[] { 1, 2 })]
		public static readonly IYamlFormatter<byte[]> Instance = new ByteArrayFormatter();
	}
}
