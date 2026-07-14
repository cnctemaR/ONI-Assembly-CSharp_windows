using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class DecimalFormatter : IYamlFormatter<decimal>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, decimal value, YamlSerializationContext context)
		{
			byte[] buffer = context.GetBuffer64();
			int num;
			if (Utf8Formatter.TryFormat(value, buffer, out num, default(StandardFormat)))
			{
				emitter.WriteScalar(buffer.AsSpan<byte>().Slice(0, num));
				return;
			}
			throw new YamlSerializerException(string.Format("Cannot serialize a value: {0}", value));
		}

		[NullableContext(1)]
		public decimal Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			ReadOnlySpan<byte> readOnlySpan;
			decimal num;
			int num2;
			if (parser.TryGetScalarAsSpan(out readOnlySpan) && Utf8Parser.TryParse(readOnlySpan, out num, out num2, '\0') && num2 == readOnlySpan.Length)
			{
				parser.Read();
				return num;
			}
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of decimal : {0} {1}", parser.CurrentEventType, parser.GetScalarAsString()));
		}

		[Nullable(1)]
		public static readonly DecimalFormatter Instance = new DecimalFormatter();
	}
}
