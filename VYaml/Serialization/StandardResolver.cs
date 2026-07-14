using System;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	public class StandardResolver : IYamlFormatterResolver
	{
		[NullableContext(2)]
		[return: Nullable(new byte[] { 2, 1 })]
		public IYamlFormatter<T> GetFormatter<T>()
		{
			return StandardResolver.FormatterCache<T>.Formatter;
		}

		[Nullable(1)]
		public static readonly StandardResolver Instance = new StandardResolver();

		[Nullable(1)]
		public static readonly IYamlFormatterResolver[] DefaultResolvers = new IYamlFormatterResolver[]
		{
			BuiltinResolver.Instance,
			GeneratedResolver.Instance
		};

		private static class FormatterCache<[Nullable(2)] T>
		{
			static FormatterCache()
			{
				if (typeof(T) == typeof(object))
				{
					StandardResolver.FormatterCache<T>.Formatter = PrimitiveObjectResolver.Instance.GetFormatter<T>();
					return;
				}
				IYamlFormatterResolver[] defaultResolvers = StandardResolver.DefaultResolvers;
				for (int i = 0; i < defaultResolvers.Length; i++)
				{
					IYamlFormatter<T> formatter = defaultResolvers[i].GetFormatter<T>();
					if (formatter != null)
					{
						StandardResolver.FormatterCache<T>.Formatter = formatter;
						return;
					}
				}
			}

			[Nullable(new byte[] { 2, 1 })]
			public static readonly IYamlFormatter<T> Formatter;
		}
	}
}
