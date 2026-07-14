using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class TupleFormatter<[Nullable(2)] T1> : IYamlFormatter<Tuple<T1>>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[] { 2, 1 })] Tuple<T1> value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			emitter.BeginSequence(SequenceStyle.Flow);
			context.Serialize<T1>(ref emitter, value.Item1);
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1 })]
		public Tuple<T1> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				return null;
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			T1 t = context.DeserializeWithAlias<T1>(ref parser);
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			return new Tuple<T1>(t);
		}
	}
}
