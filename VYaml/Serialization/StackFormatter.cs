using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1, 1, 1, 1, 1 })]
	public class StackFormatter<[Nullable(2)] T> : CollectionFormatterBase<T, List<T>, Stack<T>>
	{
		protected override List<T> Create(YamlSerializerOptions options)
		{
			return new List<T>();
		}

		protected override void Add(List<T> collection, T value, YamlSerializerOptions options)
		{
			collection.Add(value);
		}

		protected override Stack<T> Complete(List<T> intermediateCollection)
		{
			Stack<T> stack = new Stack<T>();
			for (int i = intermediateCollection.Count - 1; i >= 0; i--)
			{
				stack.Push(intermediateCollection[i]);
			}
			return stack;
		}
	}
}
