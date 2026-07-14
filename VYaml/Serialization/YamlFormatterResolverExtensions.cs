using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public static class YamlFormatterResolverExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IYamlFormatter<T> GetFormatterWithVerify<[Nullable(2)] T>(this IYamlFormatterResolver resolver)
		{
			IYamlFormatter<T> formatter;
			try
			{
				formatter = resolver.GetFormatter<T>();
			}
			catch (TypeInitializationException ex)
			{
				ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
				return null;
			}
			if (formatter != null)
			{
				return formatter;
			}
			YamlFormatterResolverExtensions.Throw(typeof(T), resolver);
			return null;
		}

		private static void Throw(Type t, IYamlFormatterResolver resolver)
		{
			throw new YamlSerializerException(t.FullName + string.Format("{0} is not registered in resolver: {1}", t, resolver.GetType()));
		}

		private static readonly Dictionary<Type, Func<IYamlFormatterResolver, IYamlFormatter>> FormatterGetters = new Dictionary<Type, Func<IYamlFormatterResolver, IYamlFormatter>>();
	}
}
