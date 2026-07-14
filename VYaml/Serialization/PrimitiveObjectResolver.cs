using System;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	public class PrimitiveObjectResolver : IYamlFormatterResolver
	{
		[NullableContext(1)]
		public IYamlFormatter<T> GetFormatter<[Nullable(2)] T>()
		{
			return PrimitiveObjectResolver.FormatterCache<T>.Formatter;
		}

		[Nullable(1)]
		public static readonly PrimitiveObjectResolver Instance = new PrimitiveObjectResolver();

		private static class FormatterCache<[Nullable(2)] T>
		{
			[Nullable(1)]
			public static readonly IYamlFormatter<T> Formatter = (IYamlFormatter<T>)PrimitiveObjectFormatter.Instance;
		}
	}
}
