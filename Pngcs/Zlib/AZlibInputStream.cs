using System;
using System.IO;

namespace Hjg.Pngcs.Zlib
{
	public abstract class AZlibInputStream : Stream
	{
		public AZlibInputStream(Stream st, bool leaveOpen)
		{
			this.rawStream = st;
			this.leaveOpen = leaveOpen;
		}

		public override bool CanRead
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

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override bool CanTimeout
		{
			get
			{
				return false;
			}
		}

		public abstract string getImplementationId();

		protected readonly Stream rawStream;

		protected readonly bool leaveOpen;
	}
}
