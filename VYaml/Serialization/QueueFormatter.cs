using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1, 1, 1, 1, 1 })]
	public class QueueFormatter<[Nullable(2)] T> : CollectionFormatterBase<T, Queue<T>, Queue<T>>
	{
		protected override Queue<T> Create(YamlSerializerOptions options)
		{
			return new Queue<T>();
		}

		protected override void Add(Queue<T> collection, T value, YamlSerializerOptions options)
		{
			collection.Enqueue(value);
		}

		protected override Queue<T> Complete(Queue<T> intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
