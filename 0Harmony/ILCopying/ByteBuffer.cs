using System;

namespace Harmony.ILCopying
{
	public class ByteBuffer
	{
		public ByteBuffer(byte[] buffer)
		{
			this.buffer = buffer;
		}

		public byte ReadByte()
		{
			this.CheckCanRead(1);
			byte[] array = this.buffer;
			int num = this.position;
			this.position = num + 1;
			return array[num];
		}

		public byte[] ReadBytes(int length)
		{
			this.CheckCanRead(length);
			byte[] array = new byte[length];
			Buffer.BlockCopy(this.buffer, this.position, array, 0, length);
			this.position += length;
			return array;
		}

		public short ReadInt16()
		{
			this.CheckCanRead(2);
			short num = (short)((int)this.buffer[this.position] | ((int)this.buffer[this.position + 1] << 8));
			this.position += 2;
			return num;
		}

		public int ReadInt32()
		{
			this.CheckCanRead(4);
			int num = (int)this.buffer[this.position] | ((int)this.buffer[this.position + 1] << 8) | ((int)this.buffer[this.position + 2] << 16) | ((int)this.buffer[this.position + 3] << 24);
			this.position += 4;
			return num;
		}

		public long ReadInt64()
		{
			this.CheckCanRead(8);
			uint num = (uint)((int)this.buffer[this.position] | ((int)this.buffer[this.position + 1] << 8) | ((int)this.buffer[this.position + 2] << 16) | ((int)this.buffer[this.position + 3] << 24));
			uint num2 = (uint)((int)this.buffer[this.position + 4] | ((int)this.buffer[this.position + 5] << 8) | ((int)this.buffer[this.position + 6] << 16) | ((int)this.buffer[this.position + 7] << 24));
			long num3 = (long)(((ulong)num2 << 32) | (ulong)num);
			this.position += 8;
			return num3;
		}

		public float ReadSingle()
		{
			bool flag = !BitConverter.IsLittleEndian;
			float num;
			if (flag)
			{
				byte[] array = this.ReadBytes(4);
				Array.Reverse(array);
				num = BitConverter.ToSingle(array, 0);
			}
			else
			{
				this.CheckCanRead(4);
				float num2 = BitConverter.ToSingle(this.buffer, this.position);
				this.position += 4;
				num = num2;
			}
			return num;
		}

		public double ReadDouble()
		{
			bool flag = !BitConverter.IsLittleEndian;
			double num;
			if (flag)
			{
				byte[] array = this.ReadBytes(8);
				Array.Reverse(array);
				num = BitConverter.ToDouble(array, 0);
			}
			else
			{
				this.CheckCanRead(8);
				double num2 = BitConverter.ToDouble(this.buffer, this.position);
				this.position += 8;
				num = num2;
			}
			return num;
		}

		private void CheckCanRead(int count)
		{
			bool flag = this.position + count > this.buffer.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		public byte[] buffer;

		public int position;
	}
}
