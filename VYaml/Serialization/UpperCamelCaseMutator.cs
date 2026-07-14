using System;

namespace VYaml.Serialization
{
	internal class UpperCamelCaseMutator : INamingConventionMutator
	{
		public unsafe bool TryMutate(ReadOnlySpan<byte> source, Span<byte> destination, out int written)
		{
			if (source.Length > destination.Length)
			{
				written = 0;
				return false;
			}
			int num = 0;
			*destination[num++] = NamingConventionMutator.ToUpper(*source[0]);
			for (int i = 1; i < source.Length; i++)
			{
				byte b = *source[i];
				bool flag = i > 1;
				if (flag)
				{
					bool flag2 = b == 45 || b == 95;
					flag = flag2;
				}
				if (flag)
				{
					i++;
					if (i <= source.Length - 1)
					{
						*destination[num++] = NamingConventionMutator.ToUpper(*source[i]);
					}
				}
				else
				{
					*destination[num++] = b;
				}
			}
			written = num;
			return true;
		}

		public unsafe bool TryMutate(ReadOnlySpan<char> source, Span<char> destination, out int written)
		{
			if (source.Length > destination.Length)
			{
				written = 0;
				return false;
			}
			int num = 0;
			*destination[num++] = char.ToUpperInvariant((char)(*source[0]));
			for (int i = 1; i < source.Length; i++)
			{
				char c = (char)(*source[i]);
				bool flag = i > 1;
				if (flag)
				{
					bool flag2 = c == '-' || c == '_';
					flag = flag2;
				}
				if (flag)
				{
					i++;
					if (i <= source.Length - 1)
					{
						*destination[num++] = char.ToUpperInvariant((char)(*source[i]));
					}
				}
				else
				{
					*destination[num++] = c;
				}
			}
			written = num;
			return true;
		}
	}
}
