using System;
using System.IO;

namespace Hjg.Pngcs.Zlib
{
	public abstract class AZlibOutputStream : Stream
	{
		public AZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
		{
			this.rawStream = st;
			this.leaveOpen = leaveOpen;
			this.strategy = strat;
			this.compressLevel = compressLevel;
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

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
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

		protected int compressLevel;

		protected EDeflateCompressStrategy strategy;
	}
}
