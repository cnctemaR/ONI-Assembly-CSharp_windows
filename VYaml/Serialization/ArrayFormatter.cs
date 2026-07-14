using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class ArrayFormatter<[Nullable(2)] T> : IYamlFormatter<T[]>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[] { 2, 1 })] T[] value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			IYamlFormatter<T> formatterWithVerify = context.Resolver.GetFormatterWithVerify<T>();
			emitter.BeginSequence(SequenceStyle.Block);
			foreach (T t in value)
			{
				formatterWithVerify.Serialize(ref emitter, t, context);
			}
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1 })]
		public T[] Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			List<T> list = new List<T>();
			IYamlFormatter<T> formatterWithVerify = context.Resolver.GetFormatterWithVerify<T>();
			while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
			{
				T t = context.DeserializeWithAlias<T>(formatterWithVerify, ref parser);
				list.Add(t);
			}
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			return list.ToArray();
		}
	}
}
