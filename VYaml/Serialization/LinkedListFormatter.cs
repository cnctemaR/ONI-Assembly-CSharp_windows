using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1, 1, 1, 1, 1 })]
	public class LinkedListFormatter<[Nullable(2)] T> : CollectionFormatterBase<T, LinkedList<T>, LinkedList<T>>
	{
		protected override LinkedList<T> Create(YamlSerializerOptions options)
		{
			return new LinkedList<T>();
		}

		protected override void Add(LinkedList<T> collection, T value, YamlSerializerOptions options)
		{
			collection.AddLast(value);
		}

		protected override LinkedList<T> Complete(LinkedList<T> intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
