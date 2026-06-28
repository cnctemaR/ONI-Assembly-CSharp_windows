using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class GenericDictionaryNodeDeserializer : INodeDeserializer
	{
		public GenericDictionaryNodeDeserializer(IObjectFactory objectFactory)
		{
			this._objectFactory = objectFactory;
		}

		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(expectedType, typeof(IDictionary<, >));
			if (implementedGenericInterface == null)
			{
				value = false;
				return false;
			}
			reader.Expect<MappingStart>();
			value = this._objectFactory.Create(expectedType);
			GenericDictionaryNodeDeserializer.deserializeHelperMethod.Invoke(implementedGenericInterface.GetGenericArguments(), new object[] { reader, expectedType, nestedObjectDeserializer, value });
			reader.Expect<MappingEnd>();
			return true;
		}

		private static void DeserializeHelper<TKey, TValue>(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, IDictionary<TKey, TValue> result)
		{
			while (!reader.Accept<MappingEnd>())
			{
				object key = nestedObjectDeserializer(reader, typeof(TKey));
				IValuePromise valuePromise = key as IValuePromise;
				object value = nestedObjectDeserializer(reader, typeof(TValue));
				IValuePromise valuePromise2 = value as IValuePromise;
				if (valuePromise == null)
				{
					if (valuePromise2 == null)
					{
						result[(TKey)((object)key)] = (TValue)((object)value);
					}
					else
					{
						valuePromise2.ValueAvailable += delegate(object v)
						{
							result[(TKey)((object)key)] = (TValue)((object)v);
						};
					}
				}
				else if (valuePromise2 == null)
				{
					valuePromise.ValueAvailable += delegate(object v)
					{
						result[(TKey)((object)v)] = (TValue)((object)value);
					};
				}
				else
				{
					bool hasFirstPart = false;
					valuePromise.ValueAvailable += delegate(object v)
					{
						if (hasFirstPart)
						{
							result[(TKey)((object)v)] = (TValue)((object)value);
							return;
						}
						key = v;
						hasFirstPart = true;
					};
					valuePromise2.ValueAvailable += delegate(object v)
					{
						if (hasFirstPart)
						{
							result[(TKey)((object)key)] = (TValue)((object)v);
							return;
						}
						value = v;
						hasFirstPart = true;
					};
				}
			}
		}

		private readonly IObjectFactory _objectFactory;

		private static readonly GenericStaticMethod deserializeHelperMethod = new GenericStaticMethod(() => GenericDictionaryNodeDeserializer.DeserializeHelper<object, object>(null, null, null, null));
	}
}
