using System;
using System.Runtime.CompilerServices;
using VYaml.Annotations;

namespace VYaml.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class KeyNameMutator
	{
		public static string Mutate(string s, NamingConvention namingConvention)
		{
			string text;
			switch (namingConvention)
			{
			case NamingConvention.LowerCamelCase:
				text = KeyNameMutator.ToLowerCamelCase(s);
				break;
			case NamingConvention.UpperCamelCase:
				text = s;
				break;
			case NamingConvention.SnakeCase:
				text = KeyNameMutator.ToSnakeCase(s, '_');
				break;
			case NamingConvention.KebabCase:
				text = KeyNameMutator.ToSnakeCase(s, '-');
				break;
			default:
				throw new ArgumentOutOfRangeException("namingConvention", namingConvention, null);
			}
			return text;
		}

		public unsafe static string ToLowerCamelCase(string s)
		{
			ReadOnlySpan<char> readOnlySpan = s.AsSpan();
			if (readOnlySpan.Length <= 0 || (readOnlySpan.Length <= 1 && char.IsLower((char)(*readOnlySpan[0]))))
			{
				return s;
			}
			int length = readOnlySpan.Length;
			Span<char> span;
			ref ReadOnlySpan<char> ptr;
			checked
			{
				span = new Span<char>(stackalloc byte[unchecked((UIntPtr)length) * 2], length);
				*span[0] = char.ToLowerInvariant((char)(*readOnlySpan[0]));
				ptr = ref readOnlySpan;
			}
			ReadOnlySpan<char> readOnlySpan2 = ptr.Slice(1, ptr.Length - 1);
			ref Span<char> ptr2 = ref span;
			readOnlySpan2.CopyTo(ptr2.Slice(1, ptr2.Length - 1));
			return span.ToString();
		}

		public unsafe static string ToSnakeCase(string s, char separator = '_')
		{
			ReadOnlySpan<char> readOnlySpan = s.AsSpan();
			if (readOnlySpan.Length <= 0)
			{
				return s;
			}
			int i = readOnlySpan.Length * 2;
			Span<char> span;
			int num;
			ReadOnlySpan<char> readOnlySpan2;
			checked
			{
				span = new Span<char>(stackalloc byte[unchecked((UIntPtr)i) * 2], i);
				num = 0;
				readOnlySpan2 = readOnlySpan;
			}
			for (i = 0; i < readOnlySpan2.Length; i++)
			{
				char c = (char)(*readOnlySpan2[i]);
				if (char.IsUpper(c))
				{
					if (num == 0 || char.IsUpper((char)(*readOnlySpan[num - 1])))
					{
						*span[num++] = char.ToLowerInvariant(c);
					}
					else
					{
						*span[num++] = separator;
						if (span.Length <= num)
						{
							span = new char[span.Length * 2];
						}
						*span[num++] = char.ToLowerInvariant(c);
					}
				}
				else
				{
					*span[num++] = c;
				}
			}
			return span.Slice(0, num).ToString();
		}
	}
}
