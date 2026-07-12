using System;

namespace HarmonyLib
{
	internal class ByteBuffer
	{
		internal ByteBuffer(byte[] buffer)
		{
			this.buffer = buffer;
		}

		internal byte ReadByte()
		{
			this.CheckCanRead(1);
			byte[] array = this.buffer;
			int num = this.position;
			this.position = num + 1;
			return array[num];
		}

		internal byte[] ReadBytes(int length)
		{
			this.CheckCanRead(length);
			byte[] array = new byte[length];
			Buffer.BlockCopy(this.buffer, this.position, array, 0, length);
			this.position += length;
			return array;
		}

		internal short ReadInt16()
		{
			this.CheckCanRead(2);
			short num = (short)((int)this.buffer[this.position] | ((int)this.buffer[this.position + 1] << 8));
			this.position += 2;
			return num;
		}

		internal int ReadInt32()
		{
			this.CheckCanRead(4);
			int num = (int)this.buffer[this.position] | ((int)this.buffer[this.position + 1] << 8) | ((int)this.buffer[this.position + 2] << 16) | ((int)this.buffer[this.position + 3] << 24);
			this.position += 4;
			return num;
		}

		internal long ReadInt64()
		{
			this.CheckCanRead(8);
			uint num = (uint)((int)this.buffer[this.position] | ((int)this.buffer[this.position + 1] << 8) | ((int)this.buffer[this.position + 2] << 16) | ((int)this.buffer[this.position + 3] << 24));
			long num2 = (long)(((ulong)((int)this.buffer[this.position + 4] | ((int)this.buffer[this.position + 5] << 8) | ((int)this.buffer[this.position + 6] << 16) | ((int)this.buffer[this.position + 7] << 24)) << 32) | (ulong)num);
			this.position += 8;
			return num2;
		}

		internal float ReadSingle()
		{
			if (!BitConverter.IsLittleEndian)
			{
				byte[] array = this.ReadBytes(4);
				Array.Reverse(array);
				return BitConverter.ToSingle(array, 0);
			}
			this.CheckCanRead(4);
			float num = BitConverter.ToSingle(this.buffer, this.position);
			this.position += 4;
			return num;
		}

		internal double ReadDouble()
		{
			if (!BitConverter.IsLittleEndian)
			{
				byte[] array = this.ReadBytes(8);
				Array.Reverse(array);
				return BitConverter.ToDouble(array, 0);
			}
			this.CheckCanRead(8);
			double num = BitConverter.ToDouble(this.buffer, this.position);
			this.position += 8;
			return num;
		}

		private void CheckCanRead(int count)
		{
			if (this.position + count > this.buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", string.Format("position({0}) + count({1}) > buffer.Length({2})", this.position, count, this.buffer.Length));
			}
		}

		internal byte[] buffer;

		internal int position;
	}
}
