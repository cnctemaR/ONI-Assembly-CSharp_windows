using System;

namespace System.Net.Sockets
{
	public class SendPacketsElement
	{
		private SendPacketsElement()
		{
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
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.Initialize(filepath, null, offset, count, endOfPacket);
		}

		public SendPacketsElement(byte[] buffer)
			: this(buffer, 0, (buffer != null) ? buffer.Length : 0, false)
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
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.Initialize(null, buffer, offset, count, endOfPacket);
		}

		private void Initialize(string filePath, byte[] buffer, int offset, int count, bool endOfPacket)
		{
			this.m_FilePath = filePath;
			this.m_Buffer = buffer;
			this.m_Offset = offset;
			this.m_Count = count;
			this.m_endOfPacket = endOfPacket;
		}

		public string FilePath
		{
			get
			{
				return this.m_FilePath;
			}
		}

		public byte[] Buffer
		{
			get
			{
				return this.m_Buffer;
			}
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public int Offset
		{
			get
			{
				return this.m_Offset;
			}
		}

		public bool EndOfPacket
		{
			get
			{
				return this.m_endOfPacket;
			}
		}

		internal string m_FilePath;

		internal byte[] m_Buffer;

		internal int m_Offset;

		internal int m_Count;

		private bool m_endOfPacket;
	}
}
