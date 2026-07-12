using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity;

namespace System.Data.SqlTypes
{
	public sealed class SqlFileStream : Stream
	{
		public SqlFileStream(string path, byte[] transactionContext, FileAccess access)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public SqlFileStream(string path, byte[] transactionContext, FileAccess access, FileOptions options, long allocationSize)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public override bool CanRead
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		public override bool CanSeek
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		public override bool CanWrite
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		public override long Length
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
		}

		public string Name
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public override long Position
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public byte[] TransactionContext
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public override void Flush()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return 0;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return 0L;
		}

		public override void SetLength(long value)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
