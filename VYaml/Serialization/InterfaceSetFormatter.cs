using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1, 1, 1, 1, 1 })]
	public class InterfaceSetFormatter<[Nullable(2)] T> : CollectionFormatterBase<T, HashSet<T>, ISet<T>>
	{
		protected override HashSet<T> Create(YamlSerializerOptions options)
		{
			return new HashSet<T>();
		}

		protected override void Add(HashSet<T> collection, T value, YamlSerializerOptions options)
		{
			collection.Add(value);
		}

		protected override ISet<T> Complete(HashSet<T> intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
