using System;
using System.Collections;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class NonGenericDictionaryNodeDeserializer : INodeDeserializer
	{
		public NonGenericDictionaryNodeDeserializer(IObjectFactory objectFactory)
		{
			this._objectFactory = objectFactory;
		}

		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			if (!typeof(IDictionary).IsAssignableFrom(expectedType))
			{
				value = false;
				return false;
			}
			reader.Expect<MappingStart>();
			IDictionary dictionary = (IDictionary)this._objectFactory.Create(expectedType);
			while (!reader.Accept<MappingEnd>())
			{
				object key = nestedObjectDeserializer(reader, typeof(object));
				IValuePromise valuePromise = key as IValuePromise;
				object keyValue = nestedObjectDeserializer(reader, typeof(object));
				IValuePromise valuePromise2 = keyValue as IValuePromise;
				if (valuePromise == null)
				{
					if (valuePromise2 == null)
					{
						dictionary.Add(key, keyValue);
					}
					else
					{
						valuePromise2.ValueAvailable += delegate(object v)
						{
							dictionary.Add(key, v);
						};
					}
				}
				else if (valuePromise2 == null)
				{
					valuePromise.ValueAvailable += delegate(object v)
					{
						dictionary.Add(v, keyValue);
					};
				}
				else
				{
					bool hasFirstPart = false;
					valuePromise.ValueAvailable += delegate(object v)
					{
						if (hasFirstPart)
						{
							dictionary.Add(v, keyValue);
							return;
						}
						key = v;
						hasFirstPart = true;
					};
					valuePromise2.ValueAvailable += delegate(object v)
					{
						if (hasFirstPart)
						{
							dictionary.Add(key, v);
							return;
						}
						keyValue = v;
						hasFirstPart = true;
					};
				}
			}
			value = dictionary;
			reader.Expect<MappingEnd>();
			return true;
		}

		private readonly IObjectFactory _objectFactory;
	}
}
