using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class YamlConvertibleNodeDeserializer : INodeDeserializer
	{
		public YamlConvertibleNodeDeserializer(IObjectFactory objectFactory)
		{
			this.objectFactory = objectFactory;
		}

		public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (typeof(IYamlConvertible).IsAssignableFrom(expectedType))
			{
				IYamlConvertible yamlConvertible = (IYamlConvertible)this.objectFactory.Create(expectedType);
				yamlConvertible.Read(parser, expectedType, (Type type) => nestedObjectDeserializer(parser, type));
				value = yamlConvertible;
				return true;
			}
			value = null;
			return false;
		}

		private readonly IObjectFactory objectFactory;
	}
}
