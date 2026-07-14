using System;
using System.Runtime.CompilerServices;
using VYaml.Annotations;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public static class NamingConventionMutator
	{
		private static byte[] GetThreadStaticBufferUtf8(int sizeHint)
		{
			if (NamingConventionMutator.ThreadStaticBufferUtf8 == null || NamingConventionMutator.ThreadStaticBufferUtf8.Length < sizeHint)
			{
				NamingConventionMutator.ThreadStaticBufferUtf8 = new byte[Math.Max(64, sizeHint)];
			}
			return NamingConventionMutator.ThreadStaticBufferUtf8;
		}

		private static char[] GetThreadStaticBuffer(int sizeHint)
		{
			if (NamingConventionMutator.ThreadStaticBuffer == null || NamingConventionMutator.ThreadStaticBuffer.Length < sizeHint)
			{
				NamingConventionMutator.ThreadStaticBuffer = new char[Math.Max(64, sizeHint)];
			}
			return NamingConventionMutator.ThreadStaticBuffer;
		}

		[NullableContext(0)]
		public static void MutateToThreadStaticBuffer(ReadOnlySpan<char> source, NamingConvention convention, [Nullable(1)] out char[] threadStaticBuffer, out int written)
		{
			INamingConventionMutator namingConventionMutator = NamingConventionMutator.Of(convention);
			threadStaticBuffer = NamingConventionMutator.GetThreadStaticBuffer(source.Length * 2);
			while (!namingConventionMutator.TryMutate(source, threadStaticBuffer, out written))
			{
				threadStaticBuffer = NamingConventionMutator.GetThreadStaticBuffer(threadStaticBuffer.Length * 2);
			}
		}

		[NullableContext(0)]
		public static void MutateToThreadStaticBufferUtf8(ReadOnlySpan<byte> sourceUtf8, NamingConvention convention, [Nullable(1)] out byte[] threadStaticBuffer, out int written)
		{
			INamingConventionMutator namingConventionMutator = NamingConventionMutator.Of(convention);
			threadStaticBuffer = NamingConventionMutator.GetThreadStaticBufferUtf8(sourceUtf8.Length * 2);
			while (!namingConventionMutator.TryMutate(sourceUtf8, threadStaticBuffer, out written))
			{
				threadStaticBuffer = NamingConventionMutator.GetThreadStaticBufferUtf8(threadStaticBuffer.Length * 2);
			}
		}

		public unsafe static string Mutate(string source, NamingConvention namingConvention)
		{
			INamingConventionMutator namingConventionMutator = NamingConventionMutator.Of(namingConvention);
			int num = source.Length * 2;
			checked
			{
				Span<char> span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
				int num2;
				while (!namingConventionMutator.TryMutate(source.AsSpan(), span, out num2))
				{
					num = unchecked(span.Length * 2);
					span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
				}
				return span.ToString();
			}
		}

		public static INamingConventionMutator Of(NamingConvention namingConvention)
		{
			INamingConventionMutator namingConventionMutator;
			switch (namingConvention)
			{
			case NamingConvention.LowerCamelCase:
				namingConventionMutator = NamingConventionMutator.LowerCamelCase;
				break;
			case NamingConvention.UpperCamelCase:
				namingConventionMutator = NamingConventionMutator.UpperCamelCase;
				break;
			case NamingConvention.SnakeCase:
				namingConventionMutator = NamingConventionMutator.SnakeCase;
				break;
			case NamingConvention.KebabCase:
				namingConventionMutator = NamingConventionMutator.KebabCase;
				break;
			default:
				throw new ArgumentOutOfRangeException("namingConvention", namingConvention, null);
			}
			return namingConventionMutator;
		}

		internal static bool IsUpper(byte ch)
		{
			return ch >= 65 && ch <= 90;
		}

		internal static bool IsLower(byte ch)
		{
			return ch >= 97 && ch <= 122;
		}

		internal static byte ToUpper(byte ch)
		{
			if (ch >= 97 && ch <= 122)
			{
				return ch - 32;
			}
			return ch;
		}

		internal static byte ToLower(byte ch)
		{
			if (ch >= 65 && ch <= 90)
			{
				return ch + 32;
			}
			return ch;
		}

		public static readonly INamingConventionMutator UpperCamelCase = new UpperCamelCaseMutator();

		public static readonly INamingConventionMutator LowerCamelCase = new LowerCamelCaseMutator();

		public static readonly INamingConventionMutator SnakeCase = new NotationCaseMutator('_');

		public static readonly INamingConventionMutator KebabCase = new NotationCaseMutator('-');

		[Nullable(2)]
		[ThreadStatic]
		private static char[] ThreadStaticBuffer;

		[Nullable(2)]
		[ThreadStatic]
		private static byte[] ThreadStaticBufferUtf8;
	}
}
