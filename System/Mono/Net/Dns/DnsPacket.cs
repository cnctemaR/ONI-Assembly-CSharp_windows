using System;

namespace Mono.Net.Dns
{
	internal abstract class DnsPacket
	{
		protected DnsPacket()
		{
		}

		protected DnsPacket(int length)
			: this(new byte[length], length)
		{
		}

		protected DnsPacket(byte[] buffer, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException("length", "Must be greater than zero.");
			}
			this.packet = buffer;
			this.position = length;
			this.header = new DnsHeader(new ArraySegment<byte>(this.packet, 0, 12));
		}

		public byte[] Packet
		{
			get
			{
				return this.packet;
			}
		}

		public int Length
		{
			get
			{
				return this.position;
			}
		}

		public DnsHeader Header
		{
			get
			{
				return this.header;
			}
		}

		protected void WriteUInt16(ushort v)
		{
			byte[] array = this.packet;
			int num = this.position;
			this.position = num + 1;
			array[num] = (byte)((v & 65280) >> 8);
			byte[] array2 = this.packet;
			num = this.position;
			this.position = num + 1;
			array2[num] = (byte)(v & 255);
		}

		protected void WriteStringBytes(string str, int offset, int count)
		{
			int num = offset;
			int i = 0;
			while (i < count)
			{
				byte[] array = this.packet;
				int num2 = this.position;
				this.position = num2 + 1;
				array[num2] = (byte)str[num];
				i++;
				num++;
			}
		}

		protected void WriteLabel(string str, int offset, int count)
		{
			byte[] array = this.packet;
			int num = this.position;
			this.position = num + 1;
			array[num] = (byte)count;
			this.WriteStringBytes(str, offset, count);
		}

		protected void WriteDnsName(string name)
		{
			if (!DnsUtil.IsValidDnsName(name))
			{
				throw new ArgumentException("Invalid DNS name");
			}
			if (!string.IsNullOrEmpty(name))
			{
				int length = name.Length;
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < length; i++)
				{
					if (name[i] != '.')
					{
						num2++;
					}
					else
					{
						if (i == 0)
						{
							break;
						}
						this.WriteLabel(name, num, num2);
						num += num2 + 1;
						num2 = 0;
					}
				}
				if (num2 > 0)
				{
					this.WriteLabel(name, num, num2);
				}
			}
			byte[] array = this.packet;
			int num3 = this.position;
			this.position = num3 + 1;
			array[num3] = 0;
		}

		protected internal string ReadName(ref int offset)
		{
			return DnsUtil.ReadName(this.packet, ref offset);
		}

		protected internal static string ReadName(byte[] buffer, ref int offset)
		{
			return DnsUtil.ReadName(buffer, ref offset);
		}

		protected internal ushort ReadUInt16(ref int offset)
		{
			byte[] array = this.packet;
			int num = offset;
			offset = num + 1;
			ushort num2 = array[num] << 8;
			byte[] array2 = this.packet;
			num = offset;
			offset = num + 1;
			return num2 + array2[num];
		}

		protected internal int ReadInt32(ref int offset)
		{
			byte[] array = this.packet;
			int num = offset;
			offset = num + 1;
			int num2 = array[num] << 24;
			byte[] array2 = this.packet;
			num = offset;
			offset = num + 1;
			int num3 = num2 + (array2[num] << 16);
			byte[] array3 = this.packet;
			num = offset;
			offset = num + 1;
			int num4 = num3 + (array3[num] << 8);
			byte[] array4 = this.packet;
			num = offset;
			offset = num + 1;
			return num4 + array4[num];
		}

		protected byte[] packet;

		protected int position;

		protected DnsHeader header;
	}
}
