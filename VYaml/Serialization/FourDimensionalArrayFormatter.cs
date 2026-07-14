using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public sealed class FourDimensionalArrayFormatter<[Nullable(2)] T> : IYamlFormatter<T[,,,]>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[] { 2, 1 })] T[,,,] value, YamlSerializationContext context)
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
						emitter.BeginSequence(SequenceStyle.Block);
						for (int l = 0; l < value.GetLength(3); l++)
						{
							formatterWithVerify.Serialize(ref emitter, value[i, j, k, l], context);
						}
						emitter.EndSequence();
					}
					emitter.EndSequence();
				}
				emitter.EndSequence();
			}
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1 })]
		public T[,,,] Deserialize(ref YamlParser parser, YamlDeserializationContext options)
		{
			if (parser.IsNullScalar())
			{
				return null;
			}
			IYamlFormatter<T> formatterWithVerify = options.Resolver.GetFormatterWithVerify<T>();
			List<List<List<List<T>>>> list = new List<List<List<List<T>>>>();
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			while (parser.CurrentEventType != ParseEventType.SequenceEnd)
			{
				List<List<List<T>>> list2 = new List<List<List<T>>>();
				parser.ReadWithVerify(ParseEventType.SequenceStart);
				while (parser.CurrentEventType != ParseEventType.SequenceEnd)
				{
					List<List<T>> list3 = new List<List<T>>();
					parser.ReadWithVerify(ParseEventType.SequenceStart);
					while (parser.CurrentEventType != ParseEventType.SequenceEnd)
					{
						List<T> list4 = new List<T>();
						parser.ReadWithVerify(ParseEventType.SequenceStart);
						while (parser.CurrentEventType != ParseEventType.SequenceEnd)
						{
							list4.Add(options.DeserializeWithAlias<T>(formatterWithVerify, ref parser));
						}
						parser.ReadWithVerify(ParseEventType.SequenceEnd);
						list3.Add(list4);
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
			int num3 = ((num2 > 0) ? list[0][0][0].Count : 0);
			T[,,,] array = new T[count, num, num2, num3];
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list[i].Count; j++)
				{
					for (int k = 0; k < list[i][j].Count; k++)
					{
						for (int l = 0; l < list[i][j][k].Count; l++)
						{
							array[i, j, k, l] = list[i][j][k][l];
						}
					}
				}
			}
			return array;
		}
	}
}
