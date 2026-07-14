using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class KeyValuePairFormatter<[Nullable(2)] TKey, [Nullable(2)] TValue> : IYamlFormatter<KeyValuePair<TKey, TValue>>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[] { 0, 1, 1 })] KeyValuePair<TKey, TValue> value, YamlSerializationContext context)
		{
			emitter.BeginSequence(SequenceStyle.Block);
			context.Serialize<TKey>(ref emitter, value.Key);
			context.Serialize<TValue>(ref emitter, value.Value);
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[] { 0, 1, 1 })]
		public KeyValuePair<TKey, TValue> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				return default(KeyValuePair<TKey, TValue>);
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			TKey tkey = context.DeserializeWithAlias<TKey>(ref parser);
			TValue tvalue = context.DeserializeWithAlias<TValue>(ref parser);
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			return new KeyValuePair<TKey, TValue>(tkey, tvalue);
		}
	}
}
