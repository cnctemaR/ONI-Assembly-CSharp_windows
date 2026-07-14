using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class ByteReadOnlySequenceFormatter : IYamlFormatter<ReadOnlySequence<byte>>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, ReadOnlySequence<byte> value, [Nullable(1)] YamlSerializationContext context)
		{
			StringBuilder stringBuilder = new StringBuilder((int)value.Length);
			foreach (ReadOnlyMemory<byte> readOnlyMemory in value)
			{
				stringBuilder.Append(readOnlyMemory.Span.ToBase64String(Base64FormattingOptions.None));
			}
			emitter.WriteString(stringBuilder.ToString(), ScalarStyle.Plain);
		}

		public ReadOnlySequence<byte> Deserialize(ref YamlParser parser, [Nullable(1)] YamlDeserializationContext context)
		{
			return new ReadOnlySequence<byte>(Convert.FromBase64String(parser.ReadScalarAsString()));
		}

		[Nullable(1)]
		public static readonly ByteReadOnlySequenceFormatter Instance = new ByteReadOnlySequenceFormatter();
	}
}
