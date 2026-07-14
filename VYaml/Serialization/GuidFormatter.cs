using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class GuidFormatter : IYamlFormatter<Guid>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, Guid value, YamlSerializationContext context)
		{
			byte[] buffer = context.GetBuffer64();
			int num;
			if (Utf8Formatter.TryFormat(value, buffer, out num, default(StandardFormat)))
			{
				emitter.WriteScalar(buffer.AsSpan<byte>().Slice(0, num));
				return;
			}
			throw new YamlSerializerException(string.Format("Cannot serialize {0}", value));
		}

		[NullableContext(1)]
		public Guid Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			ReadOnlySpan<byte> readOnlySpan;
			Guid guid;
			int num;
			if (parser.TryGetScalarAsSpan(out readOnlySpan) && Utf8Parser.TryParse(readOnlySpan, out guid, out num, '\0') && num == readOnlySpan.Length)
			{
				parser.Read();
				return guid;
			}
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of Guid : {0} {1}", parser.CurrentEventType, parser.GetScalarAsString()));
		}

		[Nullable(1)]
		public static readonly GuidFormatter Instance = new GuidFormatter();
	}
}
