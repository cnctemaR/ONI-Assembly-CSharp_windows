using System;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;

namespace System.Data.SqlTypes
{
	internal sealed class StreamOnSqlChars : SqlStreamChars
	{
		internal StreamOnSqlChars(SqlChars s)
		{
			this._sqlchars = s;
			this._lPosition = 0L;
		}

		public override bool IsNull
		{
			get
			{
				return this._sqlchars == null || this._sqlchars.IsNull;
			}
		}

		public override long Length
		{
			get
			{
				this.CheckIfStreamClosed("get_Length");
				return this._sqlchars.Length;
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
				if (value < 0L || value > this._sqlchars.Length)
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
				if (offset < 0L || offset > this._sqlchars.Length)
				{
					throw ADP.ArgumentOutOfRange("offset");
				}
				this._lPosition = offset;
				break;
			case SeekOrigin.Current:
			{
				long num = this._lPosition + offset;
				if (num < 0L || num > this._sqlchars.Length)
				{
					throw ADP.ArgumentOutOfRange("offset");
				}
				this._lPosition = num;
				break;
			}
			case SeekOrigin.End:
			{
				long num = this._sqlchars.Length + offset;
				if (num < 0L || num > this._sqlchars.Length)
				{
					throw ADP.ArgumentOutOfRange("offset");
				}
				this._lPosition = num;
				break;
			}
			default:
				throw ADP.ArgumentOutOfRange("offset");
			}
			return this._lPosition;
		}

		public override int Read(char[] buffer, int offset, int count)
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
			int num = (int)this._sqlchars.Read(this._lPosition, buffer, offset, count);
			this._lPosition += (long)num;
			return num;
		}

		public override void Write(char[] buffer, int offset, int count)
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
			this._sqlchars.Write(this._lPosition, buffer, offset, count);
			this._lPosition += (long)count;
		}

		public override void SetLength(long value)
		{
			this.CheckIfStreamClosed("SetLength");
			this._sqlchars.SetLength(value);
			if (this._lPosition > value)
			{
				this._lPosition = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			this._sqlchars = null;
		}

		private bool FClosed()
		{
			return this._sqlchars == null;
		}

		private void CheckIfStreamClosed([CallerMemberName] string methodname = "")
		{
			if (this.FClosed())
			{
				throw ADP.StreamClosed(methodname);
			}
		}

		private SqlChars _sqlchars;

		private long _lPosition;
	}
}
