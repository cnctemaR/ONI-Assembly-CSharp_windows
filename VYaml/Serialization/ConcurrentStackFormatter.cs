using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1, 1, 1, 1, 1 })]
	public class ConcurrentStackFormatter<[Nullable(2)] T> : CollectionFormatterBase<T, List<T>, ConcurrentStack<T>>
	{
		protected override List<T> Create(YamlSerializerOptions options)
		{
			return new List<T>();
		}

		protected override void Add(List<T> collection, T value, YamlSerializerOptions options)
		{
			collection.Add(value);
		}

		protected override ConcurrentStack<T> Complete(List<T> intermediateCollection)
		{
			ConcurrentStack<T> concurrentStack = new ConcurrentStack<T>();
			for (int i = intermediateCollection.Count - 1; i >= 0; i--)
			{
				concurrentStack.Push(intermediateCollection[i]);
			}
			return concurrentStack;
		}
	}
}
