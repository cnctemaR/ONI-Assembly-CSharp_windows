using System;
using System.Globalization;
using System.Text;

namespace System.Net.NetworkInformation
{
	public class PhysicalAddress
	{
		public PhysicalAddress(byte[] address)
		{
			this.bytes = address;
		}

		internal static PhysicalAddress ParseEthernet(string address)
		{
			if (address == null)
			{
				return PhysicalAddress.None;
			}
			string[] array = address.Split(new char[] { ':' });
			byte[] array2 = new byte[array.Length];
			int num = 0;
			foreach (string text in array)
			{
				array2[num++] = byte.Parse(text, NumberStyles.HexNumber);
			}
			return new PhysicalAddress(array2);
		}

		public static PhysicalAddress Parse(string address)
		{
			if (address == null)
			{
				return PhysicalAddress.None;
			}
			if (address == string.Empty)
			{
				throw new FormatException("An invalid physical address was specified.");
			}
			string[] array = address.Split(new char[] { '-' });
			if (array.Length == 1)
			{
				if (address.Length != 12)
				{
					throw new FormatException("An invalid physical address was specified.");
				}
				array = new string[6];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = address.Substring(i * 2, 2);
				}
			}
			if (array.Length == 6)
			{
				foreach (string text in array)
				{
					if (text.Length > 2)
					{
						throw new FormatException("An invalid physical address was specified.");
					}
					if (text.Length < 2)
					{
						throw new IndexOutOfRangeException("An invalid physical address was specified.");
					}
				}
				byte[] array3 = new byte[6];
				for (int k = 0; k < 6; k++)
				{
					byte b = (byte)(PhysicalAddress.GetValue(array[k][0]) << 4);
					b += PhysicalAddress.GetValue(array[k][1]);
					array3[k] = b;
				}
				return new PhysicalAddress(array3);
			}
			throw new FormatException("An invalid physical address was specified.");
		}

		private static byte GetValue(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return (byte)(c - '0');
			}
			if (c >= 'a' && c <= 'f')
			{
				return (byte)(c - 'a' + '\n');
			}
			if (c >= 'A' && c <= 'F')
			{
				return (byte)(c - 'A' + '\n');
			}
			throw new FormatException("Invalid physical address.");
		}

		public override bool Equals(object comparand)
		{
			PhysicalAddress physicalAddress = comparand as PhysicalAddress;
			if (physicalAddress == null)
			{
				return false;
			}
			if (this.bytes.Length != physicalAddress.bytes.Length)
			{
				return false;
			}
			for (int i = 0; i < this.bytes.Length; i++)
			{
				if (this.bytes[i] != physicalAddress.bytes[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((int)this.bytes[5] << 8) ^ (int)this.bytes[4] ^ ((int)this.bytes[3] << 24) ^ ((int)this.bytes[2] << 16) ^ ((int)this.bytes[1] << 8) ^ (int)this.bytes[0];
		}

		public byte[] GetAddressBytes()
		{
			return this.bytes;
		}

		public override string ToString()
		{
			if (this.bytes == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in this.bytes)
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			return stringBuilder.ToString();
		}

		private const int numberOfBytes = 6;

		public static readonly PhysicalAddress None = new PhysicalAddress(new byte[0]);

		private byte[] bytes;
	}
}
