using System;
using System.Text;

namespace System.Net
{
	internal struct IPv6AddressFormatter
	{
		public IPv6AddressFormatter(ushort[] addr, long scopeId)
		{
			this.address = addr;
			this.scopeId = scopeId;
		}

		private static ushort SwapUShort(ushort number)
		{
			return (ushort)(((number >> 8) & 255) + (((int)number << 8) & 65280));
		}

		private uint AsIPv4Int()
		{
			return (uint)(((int)IPv6AddressFormatter.SwapUShort(this.address[7]) << 16) + (int)IPv6AddressFormatter.SwapUShort(this.address[6]));
		}

		private bool IsIPv4Compatible()
		{
			for (int i = 0; i < 6; i++)
			{
				if (this.address[i] != 0)
				{
					return false;
				}
			}
			return this.address[6] != 0 && this.AsIPv4Int() > 1U;
		}

		private bool IsIPv4Mapped()
		{
			for (int i = 0; i < 5; i++)
			{
				if (this.address[i] != 0)
				{
					return false;
				}
			}
			return this.address[6] != 0 && this.address[5] == ushort.MaxValue;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.IsIPv4Compatible() || this.IsIPv4Mapped())
			{
				stringBuilder.Append("::");
				if (this.IsIPv4Mapped())
				{
					stringBuilder.Append("ffff:");
				}
				stringBuilder.Append(new IPAddress((long)((ulong)this.AsIPv4Int())).ToString());
				return stringBuilder.ToString();
			}
			int num = -1;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < 8; i++)
			{
				if (this.address[i] != 0)
				{
					if (num3 > num2 && num3 > 1)
					{
						num2 = num3;
						num = i - num3;
					}
					num3 = 0;
				}
				else
				{
					num3++;
				}
			}
			if (num3 > num2 && num3 > 1)
			{
				num2 = num3;
				num = 8 - num3;
			}
			if (num == 0)
			{
				stringBuilder.Append(":");
			}
			for (int j = 0; j < 8; j++)
			{
				if (j == num)
				{
					stringBuilder.Append(":");
					j += num2 - 1;
				}
				else
				{
					stringBuilder.AppendFormat("{0:x}", this.address[j]);
					if (j < 7)
					{
						stringBuilder.Append(':');
					}
				}
			}
			if (this.scopeId != 0L)
			{
				stringBuilder.Append('%').Append(this.scopeId);
			}
			return stringBuilder.ToString();
		}

		private ushort[] address;

		private long scopeId;
	}
}
