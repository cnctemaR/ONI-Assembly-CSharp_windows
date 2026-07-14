using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using VYaml.Annotations;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class EnumAsStringFormatter<[Nullable(0)] T> : IYamlFormatter<T>, IYamlFormatter where T : Enum
	{
		unsafe static EnumAsStringFormatter()
		{
			Type type = typeof(T);
			EnumAsStringFormatter<T>.NamingConventionByType = EnumAsStringNonGenericHelper.GetNamingConventionByType(type);
			IEnumerable<FieldInfo> fields = type.GetFields();
			Func<FieldInfo, bool> <>9__0;
			Func<FieldInfo, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (FieldInfo x) => x.FieldType == type);
			}
			checked
			{
				foreach (FieldInfo fieldInfo in fields.Where<FieldInfo>(func))
				{
					object value = fieldInfo.GetValue(null);
					string aliasStringValue = EnumAsStringNonGenericHelper.GetAliasStringValue(type, value);
					if (aliasStringValue != null)
					{
						EnumAsStringFormatter<T>.StringValues.Add((T)((object)value), new ValueTuple<string, bool>(aliasStringValue, true));
						EnumAsStringFormatter<T>.Values.Add(aliasStringValue, (T)((object)value));
					}
					else
					{
						INamingConventionMutator namingConventionMutator = NamingConventionMutator.Of(EnumAsStringFormatter<T>.NamingConventionByType.GetValueOrDefault());
						string name = Enum.GetName(type, value);
						int num = name.Length;
						Span<char> span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
						int num2;
						while (!namingConventionMutator.TryMutate(name.AsSpan(), span, out num2))
						{
							num = unchecked(span.Length * 2);
							span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
						}
						string text = span.Slice(0, num2).ToString();
						EnumAsStringFormatter<T>.StringValues.Add((T)((object)value), new ValueTuple<string, bool>(text, false));
						EnumAsStringFormatter<T>.Values.Add(text, (T)((object)value));
					}
				}
			}
		}

		public unsafe void Serialize(ref Utf8YamlEmitter emitter, T value, YamlSerializationContext context)
		{
			ValueTuple<string, bool> valueTuple;
			if (!EnumAsStringFormatter<T>.StringValues.TryGetValue(value, out valueTuple))
			{
				YamlSerializerException.ThrowInvalidType<T>(value.ToString());
				return;
			}
			ValueTuple<string, bool> valueTuple2 = valueTuple;
			string item = valueTuple2.Item1;
			if (valueTuple2.Item2 || context.Options.NamingConvention == EnumAsStringFormatter<T>.NamingConventionByType.GetValueOrDefault())
			{
				emitter.WriteString(item, ScalarStyle.Any);
				return;
			}
			INamingConventionMutator namingConventionMutator = NamingConventionMutator.Of(EnumAsStringFormatter<T>.NamingConventionByType ?? context.Options.NamingConvention);
			int num = item.Length;
			checked
			{
				Span<char> span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
				int num2;
				while (!namingConventionMutator.TryMutate(item.AsSpan(), span, out num2))
				{
					num = unchecked(span.Length * 2);
					span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
				}
				fixed (char* pinnableReference = span.GetPinnableReference())
				{
					char* ptr = pinnableReference;
					emitter.WriteString(ptr, num2, ScalarStyle.Any);
				}
			}
		}

		public unsafe T Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			string text = parser.ReadScalarAsString();
			if (text == null)
			{
				YamlSerializerException.ThrowInvalidType<T>("null");
				return default(T);
			}
			T t;
			if (EnumAsStringFormatter<T>.Values.TryGetValue(text, out t))
			{
				return t;
			}
			INamingConventionMutator namingConventionMutator = NamingConventionMutator.Of(EnumAsStringFormatter<T>.NamingConventionByType.GetValueOrDefault());
			int num = text.Length;
			checked
			{
				Span<char> span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
				int num2;
				while (!namingConventionMutator.TryMutate(text.AsSpan(), span, out num2))
				{
					num = unchecked(span.Length * 2);
					span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
				}
				string text2 = span.Slice(0, num2).ToString();
				parser.Read();
				if (EnumAsStringFormatter<T>.Values.TryGetValue(text2, out t))
				{
					return t;
				}
				YamlSerializerException.ThrowInvalidType<T>(text2);
				return default(T);
			}
		}

		internal static readonly NamingConvention? NamingConventionByType;

		[TupleElementNames(new string[] { "Value", "Alias" })]
		[Nullable(new byte[] { 1, 1, 0, 1 })]
		private static readonly Dictionary<T, ValueTuple<string, bool>> StringValues = new Dictionary<T, ValueTuple<string, bool>>();

		private static readonly Dictionary<string, T> Values = new Dictionary<string, T>();
	}
}
