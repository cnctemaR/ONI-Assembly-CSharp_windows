using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public sealed class ThreeDimensionalArrayFormatter<[Nullable(2)] T> : IYamlFormatter<T[,,]>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[] { 2, 1 })] T[,,] value, YamlSerializationContext context)
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
					emitter.BeginSequence(SequenceStyle.Block);
					for (int k = 0; k < value.GetLength(2); k++)
					{
						formatterWithVerify.Serialize(ref emitter, value[i, j, k], context);
					}
					emitter.EndSequence();
				}
				emitter.EndSequence();
			}
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1 })]
		public T[,,] Deserialize(ref YamlParser parser, YamlDeserializationContext options)
		{
			if (parser.IsNullScalar())
			{
				return null;
			}
			IYamlFormatter<T> formatterWithVerify = options.Resolver.GetFormatterWithVerify<T>();
			List<List<List<T>>> list = new List<List<List<T>>>();
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			while (parser.CurrentEventType != ParseEventType.SequenceEnd)
			{
				List<List<T>> list2 = new List<List<T>>();
				parser.ReadWithVerify(ParseEventType.SequenceStart);
				while (parser.CurrentEventType != ParseEventType.SequenceEnd)
				{
					List<T> list3 = new List<T>();
					parser.ReadWithVerify(ParseEventType.SequenceStart);
					while (parser.CurrentEventType != ParseEventType.SequenceEnd)
					{
						list3.Add(options.DeserializeWithAlias<T>(formatterWithVerify, ref parser));
					}
					parser.ReadWithVerify(ParseEventType.SequenceEnd);
					list2.Add(list3);
				}
				parser.ReadWithVerify(ParseEventType.SequenceEnd);
				list.Add(list2);
			}
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			int count = list.Count;
			int num = ((count > 0) ? list[0].Count : 0);
			int num2 = ((num > 0) ? list[0][0].Count : 0);
			T[,,] array = new T[count, num, num2];
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list[i].Count; j++)
				{
					for (int k = 0; k < list[i][j].Count; k++)
					{
						array[i, j, k] = list[i][j][k];
					}
				}
			}
			return array;
		}
	}
}
