using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class TypeConverterNodeDeserializer : INodeDeserializer
	{
		public TypeConverterNodeDeserializer(IEnumerable<IYamlTypeConverter> converters)
		{
			if (converters == null)
			{
				throw new ArgumentNullException("converters");
			}
			this.converters = converters;
		}

		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			IYamlTypeConverter yamlTypeConverter = this.converters.FirstOrDefault<IYamlTypeConverter>((IYamlTypeConverter c) => c.Accepts(expectedType));
			if (yamlTypeConverter == null)
			{
				value = null;
				return false;
			}
			value = yamlTypeConverter.ReadYaml(parser, expectedType);
			return true;
		}

		private readonly IEnumerable<IYamlTypeConverter> converters;
	}
}
