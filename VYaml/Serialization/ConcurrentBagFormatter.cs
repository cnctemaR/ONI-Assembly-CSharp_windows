using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1, 1, 1, 1, 1 })]
	public class ConcurrentBagFormatter<[Nullable(2)] T> : CollectionFormatterBase<T, ConcurrentBag<T>, ConcurrentBag<T>>
	{
		protected override ConcurrentBag<T> Create(YamlSerializerOptions options)
		{
			return new ConcurrentBag<T>();
		}

		protected override void Add(ConcurrentBag<T> collection, T value, YamlSerializerOptions options)
		{
			collection.Add(value);
		}

		protected override ConcurrentBag<T> Complete(ConcurrentBag<T> intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
