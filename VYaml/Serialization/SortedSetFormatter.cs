using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1, 1, 1, 1, 1 })]
	public class SortedSetFormatter<[Nullable(2)] T> : CollectionFormatterBase<T, SortedSet<T>, SortedSet<T>>
	{
		protected override SortedSet<T> Create(YamlSerializerOptions options)
		{
			return new SortedSet<T>();
		}

		protected override void Add(SortedSet<T> collection, T value, YamlSerializerOptions options)
		{
			collection.Add(value);
		}

		protected override SortedSet<T> Complete(SortedSet<T> intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
