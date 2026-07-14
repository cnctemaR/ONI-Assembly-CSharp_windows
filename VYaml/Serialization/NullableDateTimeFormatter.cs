using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableDateTimeFormatter : IYamlFormatter<DateTime?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, DateTime? value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			byte[] buffer = context.GetBuffer64();
			int num;
			if (Utf8Formatter.TryFormat(value.GetValueOrDefault(), buffer, out num, new StandardFormat('O', 255)))
			{
				emitter.WriteScalar(buffer.AsSpan<byte>().Slice(0, num));
				return;
			}
			throw new YamlSerializerException(string.Format("Cannot format {0}", value));
		}

		[NullableContext(1)]
		public DateTime? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			ReadOnlySpan<byte> readOnlySpan;
			DateTime dateTime;
			int num;
			if (parser.TryGetScalarAsSpan(out readOnlySpan) && Utf8Parser.TryParse(readOnlySpan, out dateTime, out num, '\0') && num == readOnlySpan.Length)
			{
				parser.Read();
				return new DateTime?(dateTime);
			}
			if (DateTime.TryParse(parser.GetScalarAsString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
			{
				parser.Read();
				return new DateTime?(dateTime);
			}
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of DateTime : {0} {1}", parser.CurrentEventType, parser.GetScalarAsString()));
		}

		[Nullable(1)]
		public static readonly NullableDateTimeFormatter Instance = new NullableDateTimeFormatter();
	}
}
