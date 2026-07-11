using System;
using System.Text;

namespace Mono.Net.Dns
{
	internal static class DnsUtil
	{
		public static bool IsValidDnsName(string name)
		{
			if (name == null)
			{
				return false;
			}
			int length = name.Length;
			if (length > 255)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				if (name[i] == '.')
				{
					if (i == 0 && length > 1)
					{
						return false;
					}
					if (i > 0 && num == 0)
					{
						return false;
					}
					num = 0;
				}
				else
				{
					num++;
					if (num > 63)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static int GetEncodedLength(string name)
		{
			if (!DnsUtil.IsValidDnsName(name))
			{
				return -1;
			}
			if (name == string.Empty)
			{
				return 1;
			}
			int length = name.Length;
			if (name[length - 1] == '.')
			{
				return length + 1;
			}
			return length + 2;
		}

		public static int GetNameLength(byte[] buffer)
		{
			return DnsUtil.GetNameLength(buffer, 0);
		}

		public static int GetNameLength(byte[] buffer, int offset)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			int i = 0;
			while (i < 256)
			{
				int num = (int)buffer[offset++];
				if (num == 0)
				{
					if (i <= 0)
					{
						return 0;
					}
					return i - 1;
				}
				else
				{
					int num2 = num & 192;
					if (num2 == 192)
					{
						num = ((num2 & 63) << 8) + (int)buffer[offset++];
						offset = num;
					}
					else
					{
						if (num2 >= 64)
						{
							return -2;
						}
						i += num + 1;
						offset += num;
					}
				}
			}
			return -1;
		}

		public static string ReadName(byte[] buffer, ref int offset)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			StringBuilder stringBuilder = new StringBuilder(32);
			bool flag = true;
			int num = offset;
			while (stringBuilder.Length < 256)
			{
				int num2 = (int)buffer[num++];
				if (flag)
				{
					offset++;
				}
				if (num2 == 0)
				{
					if (stringBuilder.Length > 0)
					{
						StringBuilder stringBuilder2 = stringBuilder;
						int length = stringBuilder2.Length;
						stringBuilder2.Length = length - 1;
					}
					return stringBuilder.ToString();
				}
				int num3 = num2 & 192;
				if (num3 == 192)
				{
					num2 = ((num3 & 63) << 8) + (int)buffer[num];
					if (flag)
					{
						offset++;
					}
					flag = false;
					num = num2;
				}
				else
				{
					if (num2 >= 64)
					{
						return null;
					}
					for (int i = 0; i < num2; i++)
					{
						stringBuilder.Append((char)buffer[num + i]);
					}
					stringBuilder.Append('.');
					num += num2;
					if (flag)
					{
						offset += num2;
					}
				}
			}
			return null;
		}
	}
}
