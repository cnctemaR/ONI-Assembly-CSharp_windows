using System;

namespace Mono.Net.Security
{
	internal class BufferOffsetSize2 : BufferOffsetSize
	{
		public BufferOffsetSize2(int size)
			: base(new byte[size], 0, 0)
		{
			this.InitialSize = size;
		}

		public void Reset()
		{
			this.Offset = (this.Size = 0);
			this.TotalBytes = 0;
			if (this.Buffer.Length <= this.InitialSize)
			{
				Array.Clear(this.Buffer, 0, this.Buffer.Length);
			}
			else
			{
				this.Buffer = new byte[this.InitialSize];
			}
			this.Complete = false;
		}

		public void MakeRoom(int size)
		{
			if (base.Remaining >= size)
			{
				return;
			}
			int num = size - base.Remaining;
			if (this.Offset == 0 && this.Size == 0)
			{
				this.Buffer = new byte[size];
				return;
			}
			byte[] array = new byte[this.Buffer.Length + num];
			this.Buffer.CopyTo(array, 0);
			this.Buffer = array;
		}

		public void AppendData(byte[] buffer, int offset, int size)
		{
			this.MakeRoom(size);
			global::System.Buffer.BlockCopy(buffer, offset, this.Buffer, base.EndOffset, size);
			this.Size += size;
		}

		public readonly int InitialSize;
	}
}
