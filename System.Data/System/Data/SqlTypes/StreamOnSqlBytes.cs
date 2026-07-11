using System;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;

namespace System.Data.SqlTypes
{
	internal sealed class StreamOnSqlBytes : Stream
	{
		internal StreamOnSqlBytes(SqlBytes sb)
		{
			this._sb = sb;
			this._lPosition = 0L;
		}

		public override bool CanRead
		{
			get
			{
				return this._sb != null && !this._sb.IsNull;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this._sb != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._sb != null && (!this._sb.IsNull || this._sb._rgbBuf != null);
			}
		}

		public override long Length
		{
			get
			{
				this.CheckIfStreamClosed("get_Length");
				return this._sb.Length;
			}
		}

		public override long Position
		{
			get
			{
				this.CheckIfStreamClosed("get_Position");
				return this._lPosition;
			}
			set
			{
				this.CheckIfStreamClosed("set_Position");
				if (value < 0L || value > this._sb.Length)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._lPosition = value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckIfStreamClosed("Seek");
			switch (origin)
			{
			case SeekOrigin.Begin:
				if (offset < 0L || offset > this._sb.Length)
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				this._lPosition = offset;
				break;
			case SeekOrigin.Current:
			{
				long num = this._lPosition + offset;
				if (num < 0L || num > this._sb.Length)
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				this._lPosition = num;
				break;
			}
			case SeekOrigin.End:
			{
				long num = this._sb.Length + offset;
				if (num < 0L || num > this._sb.Length)
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				this._lPosition = num;
				break;
			}
			default:
				throw ADP.InvalidSeekOrigin("offset");
			}
			return this._lPosition;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckIfStreamClosed("Read");
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
			int num = (int)this._sb.Read(this._lPosition, buffer, offset, count);
			this._lPosition += (long)num;
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckIfStreamClosed("Write");
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
			this._sb.Write(this._lPosition, buffer, offset, count);
			this._lPosition += (long)count;
		}

		public override int ReadByte()
		{
			this.CheckIfStreamClosed("ReadByte");
			if (this._lPosition >= this._sb.Length)
			{
				return -1;
			}
			int num = (int)this._sb[this._lPosition];
			this._lPosition += 1L;
			return num;
		}

		public override void WriteByte(byte value)
		{
			this.CheckIfStreamClosed("WriteByte");
			this._sb[this._lPosition] = value;
			this._lPosition += 1L;
		}

		public override void SetLength(long value)
		{
			this.CheckIfStreamClosed("SetLength");
			this._sb.SetLength(value);
			if (this._lPosition > value)
			{
				this._lPosition = value;
			}
		}

		public override void Flush()
		{
			if (this._sb.FStream())
			{
				this._sb._stream.Flush();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				this._sb = null;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private bool FClosed()
		{
			return this._sb == null;
		}

		private void CheckIfStreamClosed([CallerMemberName] string methodname = "")
		{
			if (this.FClosed())
			{
				throw ADP.StreamClosed(methodname);
			}
		}

		private SqlBytes _sb;

		private long _lPosition;
	}
}
