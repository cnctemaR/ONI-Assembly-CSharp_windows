using System;
using System.Text;

namespace Mono.Net.Dns
{
	internal class DnsHeader
	{
		public DnsHeader(byte[] bytes)
			: this(bytes, 0)
		{
		}

		public DnsHeader(byte[] bytes, int offset)
			: this(new ArraySegment<byte>(bytes, offset, 12))
		{
		}

		public DnsHeader(ArraySegment<byte> segment)
		{
			if (segment.Count != 12)
			{
				throw new ArgumentException("Count must be 12", "segment");
			}
			this.bytes = segment;
		}

		public void Clear()
		{
			for (int i = 0; i < 12; i++)
			{
				this.bytes.Array[i + this.bytes.Offset] = 0;
			}
		}

		public ushort ID
		{
			get
			{
				return (ushort)((int)this.bytes.Array[this.bytes.Offset] * 256 + (int)this.bytes.Array[this.bytes.Offset + 1]);
			}
			set
			{
				this.bytes.Array[this.bytes.Offset] = (byte)((value & 65280) >> 8);
				this.bytes.Array[this.bytes.Offset + 1] = (byte)(value & 255);
			}
		}

		public bool IsQuery
		{
			get
			{
				return (this.bytes.Array[2 + this.bytes.Offset] & 128) > 0;
			}
			set
			{
				if (!value)
				{
					byte[] array = this.bytes.Array;
					int num = 2 + this.bytes.Offset;
					array[num] |= 128;
					return;
				}
				byte[] array2 = this.bytes.Array;
				int num2 = 2 + this.bytes.Offset;
				array2[num2] &= 127;
			}
		}

		public DnsOpCode OpCode
		{
			get
			{
				return (DnsOpCode)((this.bytes.Array[2 + this.bytes.Offset] & 120) >> 3);
			}
			set
			{
				if (!Enum.IsDefined(typeof(DnsOpCode), value))
				{
					throw new ArgumentOutOfRangeException("value", "Invalid DnsOpCode value");
				}
				int num = (int)((int)value << 3);
				int num2 = (int)(this.bytes.Array[2 + this.bytes.Offset] & 135);
				num |= num2;
				this.bytes.Array[2 + this.bytes.Offset] = (byte)num;
			}
		}

		public bool AuthoritativeAnswer
		{
			get
			{
				return (this.bytes.Array[2 + this.bytes.Offset] & 4) > 0;
			}
			set
			{
				if (value)
				{
					byte[] array = this.bytes.Array;
					int num = 2 + this.bytes.Offset;
					array[num] |= 4;
					return;
				}
				byte[] array2 = this.bytes.Array;
				int num2 = 2 + this.bytes.Offset;
				array2[num2] &= 251;
			}
		}

		public bool Truncation
		{
			get
			{
				return (this.bytes.Array[2 + this.bytes.Offset] & 2) > 0;
			}
			set
			{
				if (value)
				{
					byte[] array = this.bytes.Array;
					int num = 2 + this.bytes.Offset;
					array[num] |= 2;
					return;
				}
				byte[] array2 = this.bytes.Array;
				int num2 = 2 + this.bytes.Offset;
				array2[num2] &= 253;
			}
		}

		public bool RecursionDesired
		{
			get
			{
				return (this.bytes.Array[2 + this.bytes.Offset] & 1) > 0;
			}
			set
			{
				if (value)
				{
					byte[] array = this.bytes.Array;
					int num = 2 + this.bytes.Offset;
					array[num] |= 1;
					return;
				}
				byte[] array2 = this.bytes.Array;
				int num2 = 2 + this.bytes.Offset;
				array2[num2] &= 254;
			}
		}

		public bool RecursionAvailable
		{
			get
			{
				return (this.bytes.Array[3 + this.bytes.Offset] & 128) > 0;
			}
			set
			{
				if (value)
				{
					byte[] array = this.bytes.Array;
					int num = 3 + this.bytes.Offset;
					array[num] |= 128;
					return;
				}
				byte[] array2 = this.bytes.Array;
				int num2 = 3 + this.bytes.Offset;
				array2[num2] &= 127;
			}
		}

		public int ZReserved
		{
			get
			{
				return (this.bytes.Array[3 + this.bytes.Offset] & 112) >> 4;
			}
			set
			{
				if (value < 0 || value > 7)
				{
					throw new ArgumentOutOfRangeException("value", "Must be between 0 and 7");
				}
				byte[] array = this.bytes.Array;
				int num = 3 + this.bytes.Offset;
				array[num] &= 143;
				byte[] array2 = this.bytes.Array;
				int num2 = 3 + this.bytes.Offset;
				array2[num2] |= (byte)((value << 4) & 112);
			}
		}

		public DnsRCode RCode
		{
			get
			{
				return (DnsRCode)(this.bytes.Array[3 + this.bytes.Offset] & 15);
			}
			set
			{
				if (value < DnsRCode.NoError || value > (DnsRCode)15)
				{
					throw new ArgumentOutOfRangeException("value", "Must be between 0 and 15");
				}
				byte[] array = this.bytes.Array;
				int num = 3 + this.bytes.Offset;
				array[num] &= 15;
				byte[] array2 = this.bytes.Array;
				int num2 = 3 + this.bytes.Offset;
				array2[num2] |= (byte)value;
			}
		}

		private static ushort GetUInt16(byte[] bytes, int offset)
		{
			return (ushort)((int)bytes[offset] * 256 + (int)bytes[offset + 1]);
		}

		private static void SetUInt16(byte[] bytes, int offset, ushort val)
		{
			bytes[offset] = (byte)((val & 65280) >> 8);
			bytes[offset + 1] = (byte)(val & 255);
		}

		public ushort QuestionCount
		{
			get
			{
				return DnsHeader.GetUInt16(this.bytes.Array, 4);
			}
			set
			{
				DnsHeader.SetUInt16(this.bytes.Array, 4, value);
			}
		}

		public ushort AnswerCount
		{
			get
			{
				return DnsHeader.GetUInt16(this.bytes.Array, 6);
			}
			set
			{
				DnsHeader.SetUInt16(this.bytes.Array, 6, value);
			}
		}

		public ushort AuthorityCount
		{
			get
			{
				return DnsHeader.GetUInt16(this.bytes.Array, 8);
			}
			set
			{
				DnsHeader.SetUInt16(this.bytes.Array, 8, value);
			}
		}

		public ushort AdditionalCount
		{
			get
			{
				return DnsHeader.GetUInt16(this.bytes.Array, 10);
			}
			set
			{
				DnsHeader.SetUInt16(this.bytes.Array, 10, value);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("ID: {0} QR: {1} Opcode: {2} AA: {3} TC: {4} RD: {5} RA: {6} \r\nRCode: {7} ", new object[] { this.ID, this.IsQuery, this.OpCode, this.AuthoritativeAnswer, this.Truncation, this.RecursionDesired, this.RecursionAvailable, this.RCode });
			stringBuilder.AppendFormat("Q: {0} A: {1} NS: {2} AR: {3}\r\n", new object[] { this.QuestionCount, this.AnswerCount, this.AuthorityCount, this.AdditionalCount });
			return stringBuilder.ToString();
		}

		public const int DnsHeaderLength = 12;

		private ArraySegment<byte> bytes;
	}
}
