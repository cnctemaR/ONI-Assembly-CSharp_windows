using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1, 1, 1, 1, 1 })]
	public class InterfaceEnumerableFormatter<[Nullable(2)] T> : CollectionFormatterBase<T, List<T>, IEnumerable<T>>
	{
		protected override List<T> Create(YamlSerializerOptions options)
		{
			return new List<T>();
		}

		protected override void Add(List<T> collection, T value, YamlSerializerOptions options)
		{
			collection.Add(value);
		}

		protected override IEnumerable<T> Complete(List<T> intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
