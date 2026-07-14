using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class ValueTupleFormatter<[Nullable(2)] T1> : IYamlFormatter<ValueTuple<T1>>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[] { 0, 1 })] ValueTuple<T1> value, YamlSerializationContext context)
		{
			emitter.BeginSequence(SequenceStyle.Flow);
			context.Serialize<T1>(ref emitter, value.Item1);
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[] { 0, 1 })]
		public ValueTuple<T1> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				return default(ValueTuple<T1>);
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			T1 t = context.DeserializeWithAlias<T1>(ref parser);
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			return new ValueTuple<T1>(t);
		}
	}
}
