using System;
using System.IO;

namespace Microsoft.SqlServer.Server
{
	internal sealed class DummyStream : Stream
	{
		private void DontDoIt()
		{
			throw new Exception(SR.GetString("Internal Error"));
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Position
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = value;
			}
		}

		public override long Length
		{
			get
			{
				return this._size;
			}
		}

		public override void SetLength(long value)
		{
			this._size = value;
		}

		public override long Seek(long value, SeekOrigin loc)
		{
			this.DontDoIt();
			return -1L;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.DontDoIt();
			return -1;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this._size += (long)count;
		}

		private long _size;
	}
}
