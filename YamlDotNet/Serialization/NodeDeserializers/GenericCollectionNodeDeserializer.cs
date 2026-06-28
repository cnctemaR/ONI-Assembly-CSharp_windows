using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class GenericCollectionNodeDeserializer : INodeDeserializer
	{
		public GenericCollectionNodeDeserializer(IObjectFactory objectFactory)
		{
			this._objectFactory = objectFactory;
		}

		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(expectedType, typeof(ICollection<>));
			if (implementedGenericInterface == null)
			{
				value = false;
				return false;
			}
			value = this._objectFactory.Create(expectedType);
			GenericCollectionNodeDeserializer._deserializeHelper.Invoke(implementedGenericInterface.GetGenericArguments(), new object[] { reader, expectedType, nestedObjectDeserializer, value });
			return true;
		}

		internal static void DeserializeHelper<TItem>(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, ICollection<TItem> result)
		{
			IList<TItem> list = result as IList<TItem>;
			reader.Expect<SequenceStart>();
			while (!reader.Accept<SequenceEnd>())
			{
				ParsingEvent parsingEvent = reader.Parser.Current;
				object obj = nestedObjectDeserializer(reader, typeof(TItem));
				IValuePromise valuePromise = obj as IValuePromise;
				if (valuePromise == null)
				{
					result.Add(TypeConverter.ChangeType<TItem>(obj));
				}
				else
				{
					if (list == null)
					{
						throw new ForwardAnchorNotSupportedException(parsingEvent.Start, parsingEvent.End, "Forward alias references are not allowed because this type does not implement IList<>");
					}
					int index = list.Count;
					result.Add(default(TItem));
					valuePromise.ValueAvailable += delegate(object v)
					{
						list[index] = TypeConverter.ChangeType<TItem>(v);
					};
				}
			}
			reader.Expect<SequenceEnd>();
		}

		private readonly IObjectFactory _objectFactory;

		private static readonly GenericStaticMethod _deserializeHelper = new GenericStaticMethod(() => GenericCollectionNodeDeserializer.DeserializeHelper<object>(null, null, null, null));
	}
}
