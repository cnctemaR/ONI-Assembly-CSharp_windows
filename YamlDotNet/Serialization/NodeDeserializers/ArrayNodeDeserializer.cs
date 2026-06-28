using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ArrayNodeDeserializer : INodeDeserializer
	{
		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			if (!expectedType.IsArray)
			{
				value = false;
				return false;
			}
			value = ArrayNodeDeserializer._deserializeHelper.Invoke(new Type[] { expectedType.GetElementType() }, new object[] { reader, expectedType, nestedObjectDeserializer });
			return true;
		}

		private static TItem[] DeserializeHelper<TItem>(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer)
		{
			List<TItem> list = new List<TItem>();
			GenericCollectionNodeDeserializer.DeserializeHelper<TItem>(reader, expectedType, nestedObjectDeserializer, list);
			return list.ToArray();
		}

		private static readonly GenericStaticMethod _deserializeHelper = new GenericStaticMethod(() => ArrayNodeDeserializer.DeserializeHelper<object>(null, null, null));
	}
}
