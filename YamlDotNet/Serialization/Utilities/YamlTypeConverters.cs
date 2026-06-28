using System;
using System.Collections.Generic;
using YamlDotNet.Serialization.Converters;

namespace YamlDotNet.Serialization.Utilities
{
	internal static class YamlTypeConverters
	{
		public static IEnumerable<IYamlTypeConverter> BuiltInConverters
		{
			get
			{
				return YamlTypeConverters._builtInTypeConverters;
			}
		}

		private static readonly IEnumerable<IYamlTypeConverter> _builtInTypeConverters = new IYamlTypeConverter[]
		{
			new GuidConverter()
		};
	}
}
