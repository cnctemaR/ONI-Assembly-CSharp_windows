using System;
using Mono.Cecil.PE;

namespace Mono.Cecil.Metadata
{
	internal sealed class DataBuffer : ByteBuffer
	{
		public DataBuffer()
			: base(0)
		{
		}

		private void Align(int align)
		{
			align--;
			base.WriteBytes(((this.position + align) & ~align) - this.position);
		}

		public uint AddData(byte[] data, int align)
		{
			if (this.buffer_align < align)
			{
				this.buffer_align = align;
			}
			this.Align(align);
			uint position = (uint)this.position;
			base.WriteBytes(data);
			return position;
		}

		public int BufferAlign
		{
			get
			{
				return this.buffer_align;
			}
		}

		private int buffer_align = 4;
	}
}
