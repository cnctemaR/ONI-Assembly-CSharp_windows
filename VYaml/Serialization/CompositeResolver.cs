using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class CompositeResolver : IYamlFormatterResolver
	{
		public static CompositeResolver Create(IEnumerable<IYamlFormatter> formatters, IEnumerable<IYamlFormatterResolver> resolvers)
		{
			return new CompositeResolver(formatters.ToList<IYamlFormatter>(), resolvers.ToList<IYamlFormatterResolver>());
		}

		public static CompositeResolver Create(IEnumerable<IYamlFormatter> formatters)
		{
			return new CompositeResolver(formatters.ToList<IYamlFormatter>(), null);
		}

		public static CompositeResolver Create(IEnumerable<IYamlFormatterResolver> resolvers)
		{
			return new CompositeResolver(null, resolvers.ToList<IYamlFormatterResolver>());
		}

		private CompositeResolver([Nullable(new byte[] { 2, 1 })] List<IYamlFormatter> formatters = null, [Nullable(new byte[] { 2, 1 })] List<IYamlFormatterResolver> resolvers = null)
		{
			this.formatters = formatters ?? new List<IYamlFormatter>();
			this.resolvers = resolvers ?? new List<IYamlFormatterResolver>();
		}

		[NullableContext(2)]
		[return: Nullable(new byte[] { 2, 1 })]
		public IYamlFormatter<T> GetFormatter<T>()
		{
			IYamlFormatter yamlFormatter;
			if (!this.formattersCache.TryGetValue(typeof(T), out yamlFormatter))
			{
				object obj = this.gate;
				lock (obj)
				{
					foreach (IYamlFormatter yamlFormatter2 in this.formatters)
					{
						if (yamlFormatter2 is IYamlFormatter<T>)
						{
							yamlFormatter = yamlFormatter2;
							goto IL_00B4;
						}
					}
					foreach (IYamlFormatterResolver yamlFormatterResolver in this.resolvers)
					{
						IYamlFormatter<T> formatter = yamlFormatterResolver.GetFormatter<T>();
						if (formatter != null)
						{
							yamlFormatter = formatter;
							break;
						}
					}
				}
				IL_00B4:
				this.formattersCache.TryAdd(typeof(T), yamlFormatter);
			}
			return yamlFormatter as IYamlFormatter<T>;
		}

		public void AddFormatter(IYamlFormatter formatter)
		{
			object obj = this.gate;
			lock (obj)
			{
				this.formatters.Add(formatter);
			}
		}

		public void AddResolver(IYamlFormatterResolver resolver)
		{
			object obj = this.gate;
			lock (obj)
			{
				this.resolvers.Add(resolver);
			}
		}

		private readonly ConcurrentDictionary<Type, IYamlFormatter> formattersCache = new ConcurrentDictionary<Type, IYamlFormatter>();

		private readonly List<IYamlFormatter> formatters;

		private readonly List<IYamlFormatterResolver> resolvers;

		private readonly object gate = new object();
	}
}
