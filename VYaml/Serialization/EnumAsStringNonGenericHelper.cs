using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using VYaml.Annotations;
using VYaml.Emitter;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class EnumAsStringNonGenericHelper
	{
		[return: Nullable(2)]
		public static string GetAliasStringValue(Type type, object value)
		{
			return EnumAsStringNonGenericHelper.AliasStringValues.GetOrAdd<Type>(value, EnumAsStringNonGenericHelper.AliasStringValueFactory, type);
		}

		public static NamingConvention? GetNamingConventionByType(Type type)
		{
			return EnumAsStringNonGenericHelper.NamingConventionsByType.GetOrAdd(type, EnumAsStringNonGenericHelper.NamingConventionFactory);
		}

		public unsafe static void Serialize(ref Utf8YamlEmitter emitter, Type type, object value, YamlSerializationContext context)
		{
			string aliasStringValue = EnumAsStringNonGenericHelper.GetAliasStringValue(type, value);
			if (aliasStringValue != null)
			{
				emitter.WriteString(aliasStringValue, ScalarStyle.Any);
				return;
			}
			string name = Enum.GetName(type, value);
			INamingConventionMutator namingConventionMutator = NamingConventionMutator.Of(EnumAsStringNonGenericHelper.GetNamingConventionByType(type) ?? context.Options.NamingConvention);
			int num = name.Length * 2;
			checked
			{
				Span<char> span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
				int num2;
				while (!namingConventionMutator.TryMutate(name.AsSpan(), span, out num2))
				{
					num = unchecked(span.Length * 2);
					span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
				}
				emitter.WriteString(span.Slice(0, num2).ToString(), ScalarStyle.Any);
			}
		}

		private static NamingConvention? AnalyzeNamingConventionByType(Type type)
		{
			YamlObjectAttribute customAttribute = type.GetCustomAttribute<YamlObjectAttribute>();
			if (customAttribute == null)
			{
				return null;
			}
			return new NamingConvention?(customAttribute.NamingConvention);
		}

		[return: Nullable(2)]
		private static string AnalyzeAliasStringValue(object value, Type type)
		{
			string name = Enum.GetName(type, value);
			object[] customAttributes = type.GetField(name).GetCustomAttributes(true);
			EnumMemberAttribute enumMemberAttribute = customAttributes.OfType<EnumMemberAttribute>().FirstOrDefault<EnumMemberAttribute>();
			if (enumMemberAttribute != null)
			{
				string value2 = enumMemberAttribute.Value;
				if (value2 != null)
				{
					return value2;
				}
			}
			DataMemberAttribute dataMemberAttribute = customAttributes.OfType<DataMemberAttribute>().FirstOrDefault<DataMemberAttribute>();
			if (dataMemberAttribute != null)
			{
				string name2 = dataMemberAttribute.Name;
				if (name2 != null)
				{
					return name2;
				}
			}
			return null;
		}

		private static readonly ConcurrentDictionary<object, string> AliasStringValues = new ConcurrentDictionary<object, string>();

		private static readonly ConcurrentDictionary<Type, NamingConvention?> NamingConventionsByType = new ConcurrentDictionary<Type, NamingConvention?>();

		[Nullable(new byte[] { 1, 1, 1, 2 })]
		private static readonly Func<object, Type, string> AliasStringValueFactory = new Func<object, Type, string>(EnumAsStringNonGenericHelper.AnalyzeAliasStringValue);

		private static readonly Func<Type, NamingConvention?> NamingConventionFactory = new Func<Type, NamingConvention?>(EnumAsStringNonGenericHelper.AnalyzeNamingConventionByType);
	}
}
