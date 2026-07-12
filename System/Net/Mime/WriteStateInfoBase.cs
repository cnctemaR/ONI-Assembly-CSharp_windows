using System;

namespace System.Net.Mime
{
	internal class WriteStateInfoBase
	{
		internal WriteStateInfoBase()
		{
			this._header = Array.Empty<byte>();
			this._footer = Array.Empty<byte>();
			this._maxLineLength = 70;
			this._buffer = new byte[1024];
			this._currentLineLength = 0;
			this._currentBufferUsed = 0;
		}

		internal WriteStateInfoBase(int bufferSize, byte[] header, byte[] footer, int maxLineLength)
			: this(bufferSize, header, footer, maxLineLength, 0)
		{
		}

		internal WriteStateInfoBase(int bufferSize, byte[] header, byte[] footer, int maxLineLength, int mimeHeaderLength)
		{
			this._buffer = new byte[bufferSize];
			this._header = header;
			this._footer = footer;
			this._maxLineLength = maxLineLength;
			this._currentLineLength = mimeHeaderLength;
			this._currentBufferUsed = 0;
		}

		internal int FooterLength
		{
			get
			{
				return this._footer.Length;
			}
		}

		internal byte[] Footer
		{
			get
			{
				return this._footer;
			}
		}

		internal byte[] Header
		{
			get
			{
				return this._header;
			}
		}

		internal byte[] Buffer
		{
			get
			{
				return this._buffer;
			}
		}

		internal int Length
		{
			get
			{
				return this._currentBufferUsed;
			}
		}

		internal int CurrentLineLength
		{
			get
			{
				return this._currentLineLength;
			}
		}

		private void EnsureSpaceInBuffer(int moreBytes)
		{
			int num = this.Buffer.Length;
			while (this._currentBufferUsed + moreBytes >= num)
			{
				num *= 2;
			}
			if (num > this.Buffer.Length)
			{
				byte[] array = new byte[num];
				this._buffer.CopyTo(array, 0);
				this._buffer = array;
			}
		}

		internal void Append(byte aByte)
		{
			this.EnsureSpaceInBuffer(1);
			byte[] buffer = this.Buffer;
			int currentBufferUsed = this._currentBufferUsed;
			this._currentBufferUsed = currentBufferUsed + 1;
			buffer[currentBufferUsed] = aByte;
			this._currentLineLength++;
		}

		internal void Append(params byte[] bytes)
		{
			this.EnsureSpaceInBuffer(bytes.Length);
			bytes.CopyTo(this._buffer, this.Length);
			this._currentLineLength += bytes.Length;
			this._currentBufferUsed += bytes.Length;
		}

		internal void AppendCRLF(bool includeSpace)
		{
			this.AppendFooter();
			this.Append(new byte[] { 13, 10 });
			this._currentLineLength = 0;
			if (includeSpace)
			{
				this.Append(32);
			}
			this.AppendHeader();
		}

		internal void AppendHeader()
		{
			if (this.Header != null && this.Header.Length != 0)
			{
				this.Append(this.Header);
			}
		}

		internal void AppendFooter()
		{
			if (this.Footer != null && this.Footer.Length != 0)
			{
				this.Append(this.Footer);
			}
		}

		internal int MaxLineLength
		{
			get
			{
				return this._maxLineLength;
			}
		}

		internal void Reset()
		{
			this._currentBufferUsed = 0;
			this._currentLineLength = 0;
		}

		internal void BufferFlushed()
		{
			this._currentBufferUsed = 0;
		}

		protected readonly byte[] _header;

		protected readonly byte[] _footer;

		protected readonly int _maxLineLength;

		protected byte[] _buffer;

		protected int _currentLineLength;

		protected int _currentBufferUsed;

		protected const int DefaultBufferSize = 1024;
	}
}
