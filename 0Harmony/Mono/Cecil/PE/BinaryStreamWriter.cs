using System;
using System.IO;

namespace Mono.Cecil.PE
{
	internal class BinaryStreamWriter : BinaryWriter
	{
		public int Position
		{
			get
			{
				return (int)this.BaseStream.Position;
			}
			set
			{
				this.BaseStream.Position = (long)value;
			}
		}

		public BinaryStreamWriter(Stream stream)
			: base(stream)
		{
		}

		public void WriteByte(byte value)
		{
			this.Write(value);
		}

		public void WriteUInt16(ushort value)
		{
			this.Write(value);
		}

		public void WriteInt16(short value)
		{
			this.Write(value);
		}

		public void WriteUInt32(uint value)
		{
			this.Write(value);
		}

		public void WriteInt32(int value)
		{
			this.Write(value);
		}

		public void WriteUInt64(ulong value)
		{
			this.Write(value);
		}

		public void WriteBytes(byte[] bytes)
		{
			this.Write(bytes);
		}

		public void WriteDataDirectory(DataDirectory directory)
		{
			this.Write(directory.VirtualAddress);
			this.Write(directory.Size);
		}

		public void WriteBuffer(ByteBuffer buffer)
		{
			this.Write(buffer.buffer, 0, buffer.length);
		}

		protected void Advance(int bytes)
		{
			this.BaseStream.Seek((long)bytes, SeekOrigin.Current);
		}

		public void Align(int align)
		{
			align--;
			int position = this.Position;
			int num = ((position + align) & ~align) - position;
			for (int i = 0; i < num; i++)
			{
				this.WriteByte(0);
			}
		}
	}
}
