using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1, 1, 1, 1, 1 })]
	public class BlockingCollectionFormatter<[Nullable(2)] T> : CollectionFormatterBase<T, BlockingCollection<T>, BlockingCollection<T>>
	{
		protected override BlockingCollection<T> Create(YamlSerializerOptions options)
		{
			return new BlockingCollection<T>();
		}

		protected override void Add(BlockingCollection<T> collection, T value, YamlSerializerOptions options)
		{
			collection.Add(value);
		}

		protected override BlockingCollection<T> Complete(BlockingCollection<T> intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
