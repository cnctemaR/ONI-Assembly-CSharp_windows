using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public sealed class TwoDimensionalArrayFormatter<[Nullable(2)] T> : IYamlFormatter<T[,]>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[] { 2, 1 })] T[,] value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			IYamlFormatter<T> formatterWithVerify = context.Resolver.GetFormatterWithVerify<T>();
			emitter.BeginSequence(SequenceStyle.Block);
			for (int i = 0; i < value.GetLength(0); i++)
			{
				emitter.BeginSequence(SequenceStyle.Block);
				for (int j = 0; j < value.GetLength(1); j++)
				{
					formatterWithVerify.Serialize(ref emitter, value[i, j], context);
				}
				emitter.EndSequence();
			}
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1 })]
		public T[,] Deserialize(ref YamlParser parser, YamlDeserializationContext options)
		{
			if (parser.IsNullScalar())
			{
				return null;
			}
			IYamlFormatter<T> formatterWithVerify = options.Resolver.GetFormatterWithVerify<T>();
			List<List<T>> list = new List<List<T>>();
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			while (parser.CurrentEventType != ParseEventType.SequenceEnd)
			{
				List<T> list2 = new List<T>();
				parser.ReadWithVerify(ParseEventType.SequenceStart);
				while (parser.CurrentEventType != ParseEventType.SequenceEnd)
				{
					list2.Add(options.DeserializeWithAlias<T>(formatterWithVerify, ref parser));
				}
				parser.ReadWithVerify(ParseEventType.SequenceEnd);
				list.Add(list2);
			}
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			T[,] array = new T[list.Count, (list.Count > 0) ? list[0].Count : 0];
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list[i].Count; j++)
				{
					array[i, j] = list[i][j];
				}
			}
			return array;
		}
	}
}
