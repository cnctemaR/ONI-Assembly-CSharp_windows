using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Helpers;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class CollectionNodeDeserializer : INodeDeserializer
	{
		public CollectionNodeDeserializer(IObjectFactory objectFactory)
		{
			this._objectFactory = objectFactory;
		}

		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			bool flag = true;
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(expectedType, typeof(ICollection<>));
			Type type;
			IList list;
			if (implementedGenericInterface != null)
			{
				Type[] genericArguments = implementedGenericInterface.GetGenericArguments();
				type = genericArguments[0];
				value = this._objectFactory.Create(expectedType);
				list = value as IList;
				if (list == null)
				{
					Type implementedGenericInterface2 = ReflectionUtility.GetImplementedGenericInterface(expectedType, typeof(IList<>));
					flag = implementedGenericInterface2 != null;
					list = new GenericCollectionToNonGenericAdapter(value, implementedGenericInterface, implementedGenericInterface2);
				}
			}
			else
			{
				if (!typeof(IList).IsAssignableFrom(expectedType))
				{
					value = null;
					return false;
				}
				type = typeof(object);
				value = this._objectFactory.Create(expectedType);
				list = (IList)value;
			}
			CollectionNodeDeserializer.DeserializeHelper(type, parser, nestedObjectDeserializer, list, flag);
			return true;
		}

		internal static void DeserializeHelper(Type tItem, IParser parser, Func<IParser, Type, object> nestedObjectDeserializer, IList result, bool canUpdate)
		{
			parser.Expect<SequenceStart>();
			while (!parser.Accept<SequenceEnd>())
			{
				ParsingEvent parsingEvent = parser.Current;
				object obj = nestedObjectDeserializer(parser, tItem);
				IValuePromise valuePromise = obj as IValuePromise;
				if (valuePromise == null)
				{
					result.Add(TypeConverter.ChangeType(obj, tItem));
				}
				else
				{
					if (!canUpdate)
					{
						throw new ForwardAnchorNotSupportedException(parsingEvent.Start, parsingEvent.End, "Forward alias references are not allowed because this type does not implement IList<>");
					}
					int index = result.Add((!tItem.IsValueType()) ? null : Activator.CreateInstance(tItem));
					valuePromise.ValueAvailable += delegate(object v)
					{
						result[index] = TypeConverter.ChangeType(v, tItem);
					};
				}
			}
			parser.Expect<SequenceEnd>();
		}

		private readonly IObjectFactory _objectFactory;
	}
}
