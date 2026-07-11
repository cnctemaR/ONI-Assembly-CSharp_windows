using System;

namespace System.Net.Sockets
{
	public class SendPacketsElement
	{
		public SendPacketsElement(byte[] buffer)
			: this(buffer, 0, (buffer == null) ? 0 : buffer.Length)
		{
		}

		public SendPacketsElement(byte[] buffer, int offset, int count)
			: this(buffer, offset, count, false)
		{
		}

		public SendPacketsElement(byte[] buffer, int offset, int count, bool endOfPacket)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = buffer.Length;
			if (offset < 0 || offset >= num)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || offset + count >= num)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.Buffer = buffer;
			this.Offset = offset;
			this.Count = count;
			this.EndOfPacket = endOfPacket;
			this.FilePath = null;
		}

		public SendPacketsElement(string filepath)
			: this(filepath, 0, 0, false)
		{
		}

		public SendPacketsElement(string filepath, int offset, int count)
			: this(filepath, offset, count, false)
		{
		}

		public SendPacketsElement(string filepath, int offset, int count, bool endOfPacket)
		{
			if (filepath == null)
			{
				throw new ArgumentNullException("filepath");
			}
			this.Buffer = null;
			this.Offset = offset;
			this.Count = count;
			this.EndOfPacket = endOfPacket;
			this.FilePath = filepath;
		}

		public byte[] Buffer { get; private set; }

		public int Count { get; private set; }

		public bool EndOfPacket { get; private set; }

		public string FilePath { get; private set; }

		public int Offset { get; private set; }
	}
}
