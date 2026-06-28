using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ObjectNodeDeserializer : INodeDeserializer
	{
		public ObjectNodeDeserializer(IObjectFactory objectFactory, ITypeInspector typeDescriptor, bool ignoreUnmatched)
		{
			this._objectFactory = objectFactory;
			this._typeDescriptor = typeDescriptor;
			this._ignoreUnmatched = ignoreUnmatched;
		}

		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			if (reader.Allow<MappingStart>() == null)
			{
				value = null;
				return false;
			}
			value = this._objectFactory.Create(expectedType);
			while (!reader.Accept<MappingEnd>())
			{
				Scalar scalar = reader.Expect<Scalar>();
				IPropertyDescriptor property = this._typeDescriptor.GetProperty(expectedType, null, scalar.Value, this._ignoreUnmatched);
				if (property == null)
				{
					reader.SkipThisAndNestedEvents();
				}
				else
				{
					object obj = nestedObjectDeserializer(reader, property.Type);
					IValuePromise valuePromise = obj as IValuePromise;
					if (valuePromise == null)
					{
						object obj2 = TypeConverter.ChangeType(obj, property.Type);
						property.Write(value, obj2);
					}
					else
					{
						object valueRef = value;
						valuePromise.ValueAvailable += delegate(object v)
						{
							object obj3 = TypeConverter.ChangeType(v, property.Type);
							property.Write(valueRef, obj3);
						};
					}
				}
			}
			reader.Expect<MappingEnd>();
			return true;
		}

		private readonly IObjectFactory _objectFactory;

		private readonly ITypeInspector _typeDescriptor;

		private readonly bool _ignoreUnmatched;
	}
}
