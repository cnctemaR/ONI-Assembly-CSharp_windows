using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(2)]
	[Nullable(0)]
	public class ValueTupleFormatter<T1, T2, T3> : IYamlFormatter<ValueTuple<T1, T2, T3>>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[] { 0, 1, 1, 1 })] ValueTuple<T1, T2, T3> value, YamlSerializationContext context)
		{
			emitter.BeginSequence(SequenceStyle.Flow);
			context.Serialize<T1>(ref emitter, value.Item1);
			context.Serialize<T2>(ref emitter, value.Item2);
			context.Serialize<T3>(ref emitter, value.Item3);
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[] { 0, 1, 1, 1 })]
		public ValueTuple<T1, T2, T3> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				return default(ValueTuple<T1, T2, T3>);
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			T1 t = context.DeserializeWithAlias<T1>(ref parser);
			T2 t2 = context.DeserializeWithAlias<T2>(ref parser);
			T3 t3 = context.DeserializeWithAlias<T3>(ref parser);
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			return new ValueTuple<T1, T2, T3>(t, t2, t3);
		}
	}
}
