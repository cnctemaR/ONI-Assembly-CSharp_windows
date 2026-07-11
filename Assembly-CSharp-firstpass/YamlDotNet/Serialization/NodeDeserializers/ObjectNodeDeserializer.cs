using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ObjectNodeDeserializer : INodeDeserializer
	{
		public ObjectNodeDeserializer(IObjectFactory objectFactory, ITypeInspector typeDescriptor, bool ignoreUnmatched, Action<string> unmatchedLogFn = null)
		{
			this._objectFactory = objectFactory;
			this._typeDescriptor = typeDescriptor;
			this._ignoreUnmatched = ignoreUnmatched;
			this._unmatchedLogFn = unmatchedLogFn;
		}

		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (parser.Allow<MappingStart>() == null)
			{
				value = null;
				return false;
			}
			value = this._objectFactory.Create(expectedType);
			while (!parser.Accept<MappingEnd>())
			{
				Scalar scalar = parser.Expect<Scalar>();
				IPropertyDescriptor property = this._typeDescriptor.GetProperty(expectedType, null, scalar.Value, this._ignoreUnmatched);
				if (property == null)
				{
					if (this._unmatchedLogFn != null)
					{
						this._unmatchedLogFn(string.Format("Found a property '{0}' on a type '{1}', but that type doesn't have that property!", scalar.Value, expectedType.FullName));
					}
					parser.SkipThisAndNestedEvents();
				}
				else
				{
					object obj = nestedObjectDeserializer(parser, property.Type);
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
			parser.Expect<MappingEnd>();
			return true;
		}

		private readonly IObjectFactory _objectFactory;

		private readonly ITypeInspector _typeDescriptor;

		private readonly bool _ignoreUnmatched;

		private readonly Action<string> _unmatchedLogFn;
	}
}
