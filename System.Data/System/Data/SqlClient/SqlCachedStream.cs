using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;

namespace System.Data.SqlClient
{
	internal sealed class SqlCachedStream : Stream
	{
		internal SqlCachedStream(SqlCachedBuffer sqlBuf)
		{
			this._cachedBytes = sqlBuf.CachedBytes;
		}

		public override bool CanRead
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
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return this.TotalLength;
			}
		}

		public override long Position
		{
			get
			{
				long num = 0L;
				if (this._currentArrayIndex > 0)
				{
					for (int i = 0; i < this._currentArrayIndex; i++)
					{
						num += (long)this._cachedBytes[i].Length;
					}
				}
				return num + (long)this._currentPosition;
			}
			set
			{
				if (this._cachedBytes == null)
				{
					throw ADP.StreamClosed("set_Position");
				}
				this.SetInternalPosition(value, "set_Position");
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._cachedBytes != null)
				{
					this._cachedBytes.Clear();
				}
				this._cachedBytes = null;
				this._currentPosition = 0;
				this._currentArrayIndex = 0;
				this._totalLength = 0L;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override void Flush()
		{
			throw ADP.NotSupported();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			if (this._cachedBytes == null)
			{
				throw ADP.StreamClosed("Read");
			}
			if (buffer == null)
			{
				throw ADP.ArgumentNull("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw ADP.ArgumentOutOfRange(string.Empty, (offset < 0) ? "offset" : "count");
			}
			if (buffer.Length - offset < count)
			{
				throw ADP.ArgumentOutOfRange("count");
			}
			if (this._cachedBytes.Count <= this._currentArrayIndex)
			{
				return 0;
			}
			while (count > 0)
			{
				if (this._cachedBytes[this._currentArrayIndex].Length <= this._currentPosition)
				{
					this._currentArrayIndex++;
					if (this._cachedBytes.Count <= this._currentArrayIndex)
					{
						break;
					}
					this._currentPosition = 0;
				}
				int num2 = this._cachedBytes[this._currentArrayIndex].Length - this._currentPosition;
				if (num2 > count)
				{
					num2 = count;
				}
				Buffer.BlockCopy(this._cachedBytes[this._currentArrayIndex], this._currentPosition, buffer, offset, num2);
				this._currentPosition += num2;
				count -= num2;
				offset += num2;
				num += num2;
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long num = 0L;
			if (this._cachedBytes == null)
			{
				throw ADP.StreamClosed("Seek");
			}
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.SetInternalPosition(offset, "offset");
				break;
			case SeekOrigin.Current:
				num = offset + this.Position;
				this.SetInternalPosition(num, "offset");
				break;
			case SeekOrigin.End:
				num = this.TotalLength + offset;
				this.SetInternalPosition(num, "offset");
				break;
			default:
				throw ADP.InvalidSeekOrigin("offset");
			}
			return num;
		}

		public override void SetLength(long value)
		{
			throw ADP.NotSupported();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw ADP.NotSupported();
		}

		private void SetInternalPosition(long lPos, string argumentName)
		{
			long num = lPos;
			if (num < 0L)
			{
				throw new ArgumentOutOfRangeException(argumentName);
			}
			for (int i = 0; i < this._cachedBytes.Count; i++)
			{
				if (num <= (long)this._cachedBytes[i].Length)
				{
					this._currentArrayIndex = i;
					this._currentPosition = (int)num;
					return;
				}
				num -= (long)this._cachedBytes[i].Length;
			}
			if (num > 0L)
			{
				throw new ArgumentOutOfRangeException(argumentName);
			}
		}

		private long TotalLength
		{
			get
			{
				if (this._totalLength == 0L && this._cachedBytes != null)
				{
					long num = 0L;
					for (int i = 0; i < this._cachedBytes.Count; i++)
					{
						num += (long)this._cachedBytes[i].Length;
					}
					this._totalLength = num;
				}
				return this._totalLength;
			}
		}

		private int _currentPosition;

		private int _currentArrayIndex;

		private List<byte[]> _cachedBytes;

		private long _totalLength;
	}
}
