using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class DateTimeOffsetFormatter : IYamlFormatter<DateTimeOffset>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, DateTimeOffset value, YamlSerializationContext context)
		{
			byte[] buffer = context.GetBuffer64();
			int num;
			if (Utf8Formatter.TryFormat(value, buffer, out num, new StandardFormat('O', 255)))
			{
				emitter.WriteScalar(buffer.AsSpan<byte>().Slice(0, num));
				return;
			}
			throw new YamlSerializerException(string.Format("Cannot format {0}", value));
		}

		[NullableContext(1)]
		public DateTimeOffset Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			ReadOnlySpan<byte> readOnlySpan;
			DateTimeOffset dateTimeOffset;
			int num;
			if (parser.TryGetScalarAsSpan(out readOnlySpan) && Utf8Parser.TryParse(readOnlySpan, out dateTimeOffset, out num, '\0') && num == readOnlySpan.Length)
			{
				parser.Read();
				return dateTimeOffset;
			}
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of DateTimeOffset : {0} {1}", parser.CurrentEventType, parser.GetScalarAsString()));
		}

		[Nullable(1)]
		public static readonly DateTimeOffsetFormatter Instance = new DateTimeOffsetFormatter();
	}
}
