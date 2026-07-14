using System;

namespace VYaml.Serialization
{
	internal class NotationCaseMutator : INamingConventionMutator
	{
		public NotationCaseMutator(char separator)
		{
			this.separator = separator;
		}

		public unsafe bool TryMutate(ReadOnlySpan<char> source, Span<char> destination, out int written)
		{
			if (source.Length <= 0)
			{
				written = 0;
				return true;
			}
			int num = 0;
			for (int i = 0; i < source.Length; i++)
			{
				if (num >= destination.Length - 1)
				{
					written = 0;
					return false;
				}
				char c = (char)(*source[i]);
				if (char.IsUpper(c))
				{
					bool flag = i > 0;
					if (flag)
					{
						char c2 = (char)(*source[i - 1]);
						bool flag2 = c2 == '-' || c2 == '_';
						flag = !flag2;
					}
					if (flag)
					{
						*destination[num++] = this.separator;
					}
					if (num >= destination.Length - 1)
					{
						written = 0;
						return false;
					}
					*destination[num++] = char.ToLowerInvariant(c);
				}
				else
				{
					bool flag = c == '-' || c == '_';
					if (flag)
					{
						*destination[num++] = this.separator;
					}
					else
					{
						*destination[num++] = c;
					}
				}
			}
			written = num;
			return true;
		}

		public unsafe bool TryMutate(ReadOnlySpan<byte> source, Span<byte> destination, out int written)
		{
			if (source.Length <= 0)
			{
				written = 0;
				return true;
			}
			byte b = (byte)this.separator;
			int num = 0;
			for (int i = 0; i < source.Length; i++)
			{
				if (num >= destination.Length - 1)
				{
					written = 0;
					return false;
				}
				byte b2 = *source[i];
				if (NamingConventionMutator.IsUpper(b2))
				{
					bool flag = i > 0;
					if (flag)
					{
						byte b3 = *source[i - 1];
						bool flag2 = b3 == 45 || b3 == 95;
						flag = !flag2;
					}
					if (flag)
					{
						*destination[num++] = b;
					}
					if (num >= destination.Length - 1)
					{
						written = 0;
						return false;
					}
					*destination[num++] = NamingConventionMutator.ToLower(b2);
				}
				else
				{
					bool flag = b2 == 45 || b2 == 95;
					if (flag)
					{
						*destination[num++] = b;
					}
					else
					{
						*destination[num++] = b2;
					}
				}
			}
			written = num;
			return true;
		}

		private readonly char separator;
	}
}
