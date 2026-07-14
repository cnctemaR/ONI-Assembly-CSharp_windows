using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	public class KleiResolver : IYamlFormatterResolver
	{
		[NullableContext(2)]
		[return: Nullable(new byte[] { 2, 1 })]
		public IYamlFormatter<T> GetFormatter<T>()
		{
			return KleiResolver.FormatterCache<T>.Formatter;
		}

		[Nullable(1)]
		public static readonly KleiResolver Instance = new KleiResolver();

		[Nullable(1)]
		public static readonly Dictionary<Type, IYamlFormatter> FormatterMap = new Dictionary<Type, IYamlFormatter>
		{
			{
				typeof(Vector2f),
				Vector2fFormatter.Instance
			},
			{
				typeof(SimHashes),
				SimHashesFormatter.Instance
			},
			{
				typeof(Tag),
				TagFormatter.Instance
			},
			{
				typeof(Element.State),
				ElementStateFormatter.Instance
			}
		};

		private static class FormatterCache<[Nullable(2)] T>
		{
			static FormatterCache()
			{
				IYamlFormatter yamlFormatter;
				if (KleiResolver.FormatterMap.TryGetValue(typeof(T), out yamlFormatter))
				{
					IYamlFormatter<T> yamlFormatter2 = yamlFormatter as IYamlFormatter<T>;
					if (yamlFormatter2 != null)
					{
						KleiResolver.FormatterCache<T>.Formatter = yamlFormatter2;
						return;
					}
				}
				KleiResolver.FormatterCache<T>.Formatter = null;
			}

			[Nullable(new byte[] { 2, 1 })]
			public static readonly IYamlFormatter<T> Formatter;
		}
	}
}
