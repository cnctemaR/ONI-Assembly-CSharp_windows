using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using VYaml.Annotations;

namespace VYaml.Serialization
{
	public class GeneratedResolver : IYamlFormatterResolver
	{
		[NullableContext(2)]
		private static bool TryInvokeRegisterYamlFormatter<T>()
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.GetCustomAttribute<YamlObjectAttribute>() == null)
			{
				return false;
			}
			if (typeFromHandle.IsInterface)
			{
				Type nestedType = typeFromHandle.GetNestedType(typeFromHandle.Name + "GeneratedFormatter");
				if (nestedType == null)
				{
					return false;
				}
				GeneratedResolver.Register<T>((IYamlFormatter<T>)Activator.CreateInstance(nestedType));
				return true;
			}
			else
			{
				MethodInfo method = typeFromHandle.GetMethod("__RegisterVYamlFormatter", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method == null)
				{
					return false;
				}
				method.Invoke(null, null);
				return true;
			}
		}

		[NullableContext(1)]
		[Preserve]
		public static void Register<[Nullable(2)] T>(IYamlFormatter<T> formatter)
		{
			GeneratedResolver.Check<T>.Registered = true;
			GeneratedResolver.Cache<T>.Formatter = formatter;
		}

		[NullableContext(2)]
		[return: Nullable(new byte[] { 2, 1 })]
		public IYamlFormatter<T> GetFormatter<T>()
		{
			return GeneratedResolver.Cache<T>.Formatter;
		}

		[Nullable(1)]
		public static readonly GeneratedResolver Instance = new GeneratedResolver();

		private static class Check<[Nullable(2)] T>
		{
			internal static bool Registered;
		}

		private static class Cache<[Nullable(2)] T>
		{
			static Cache()
			{
				if (GeneratedResolver.Check<T>.Registered)
				{
					return;
				}
				GeneratedResolver.TryInvokeRegisterYamlFormatter<T>();
			}

			[Nullable(new byte[] { 2, 1 })]
			internal static IYamlFormatter<T> Formatter;
		}
	}
}
