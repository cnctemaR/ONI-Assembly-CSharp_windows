using System;
using System.IO;
using System.Text;

namespace Ionic.Zlib
{
	public class GZipStream : Stream
	{
		public string Comment
		{
			get
			{
				return this._Comment;
			}
			set
			{
				if (this._disposed)
				{
					throw new ObjectDisposedException("GZipStream");
				}
				this._Comment = value;
			}
		}

		public string FileName
		{
			get
			{
				return this._FileName;
			}
			set
			{
				if (this._disposed)
				{
					throw new ObjectDisposedException("GZipStream");
				}
				this._FileName = value;
				if (this._FileName == null)
				{
					return;
				}
				if (this._FileName.IndexOf("/") != -1)
				{
					this._FileName = this._FileName.Replace("/", "\\");
				}
				if (this._FileName.EndsWith("\\"))
				{
					throw new Exception("Illegal filename");
				}
				if (this._FileName.IndexOf("\\") != -1)
				{
					this._FileName = Path.GetFileName(this._FileName);
				}
			}
		}

		public int Crc32
		{
			get
			{
				return this._Crc32;
			}
		}

		public GZipStream(Stream stream, CompressionMode mode)
			: this(stream, mode, CompressionLevel.Default, false)
		{
		}

		public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level)
			: this(stream, mode, level, false)
		{
		}

		public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
			: this(stream, mode, CompressionLevel.Default, leaveOpen)
		{
		}

		public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
		{
			this._baseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.GZIP, leaveOpen);
		}

		public virtual FlushType FlushMode
		{
			get
			{
				return this._baseStream._flushMode;
			}
			set
			{
				if (this._disposed)
				{
					throw new ObjectDisposedException("GZipStream");
				}
				this._baseStream._flushMode = value;
			}
		}

		public int BufferSize
		{
			get
			{
				return this._baseStream._bufferSize;
			}
			set
			{
				if (this._disposed)
				{
					throw new ObjectDisposedException("GZipStream");
				}
				if (this._baseStream._workingBuffer != null)
				{
					throw new ZlibException("The working buffer is already set.");
				}
				if (value < 1024)
				{
					throw new ZlibException(string.Format("Don't be silly. {0} bytes?? Use a bigger buffer, at least {1}.", value, 1024));
				}
				this._baseStream._bufferSize = value;
			}
		}

		public virtual long TotalIn
		{
			get
			{
				return this._baseStream._z.TotalBytesIn;
			}
		}

		public virtual long TotalOut
		{
			get
			{
				return this._baseStream._z.TotalBytesOut;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this._disposed)
				{
					if (disposing && this._baseStream != null)
					{
						this._baseStream.Close();
						this._Crc32 = this._baseStream.Crc32;
					}
					this._disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override bool CanRead
		{
			get
			{
				if (this._disposed)
				{
					throw new ObjectDisposedException("GZipStream");
				}
				return this._baseStream._stream.CanRead;
			}
		}

		public override bool CanSeek
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
				if (this._disposed)
				{
					throw new ObjectDisposedException("GZipStream");
				}
				return this._baseStream._stream.CanWrite;
			}
		}

		public override void Flush()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			this._baseStream.Flush();
		}

		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long Position
		{
			get
			{
				if (this._baseStream._streamMode == ZlibBaseStream.StreamMode.Writer)
				{
					return this._baseStream._z.TotalBytesOut + (long)this._headerByteCount;
				}
				if (this._baseStream._streamMode == ZlibBaseStream.StreamMode.Reader)
				{
					return this._baseStream._z.TotalBytesIn + (long)this._baseStream._gzipHeaderByteCount;
				}
				return 0L;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			int num = this._baseStream.Read(buffer, offset, count);
			if (!this._firstReadDone)
			{
				this._firstReadDone = true;
				this.FileName = this._baseStream._GzipFileName;
				this.Comment = this._baseStream._GzipComment;
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			if (this._baseStream._streamMode == ZlibBaseStream.StreamMode.Undefined)
			{
				if (!this._baseStream._wantCompress)
				{
					throw new InvalidOperationException();
				}
				this._headerByteCount = this.EmitHeader();
			}
			this._baseStream.Write(buffer, offset, count);
		}

		private int EmitHeader()
		{
			byte[] array = ((this.Comment == null) ? null : GZipStream.iso8859dash1.GetBytes(this.Comment));
			byte[] array2 = ((this.FileName == null) ? null : GZipStream.iso8859dash1.GetBytes(this.FileName));
			int num = ((this.Comment == null) ? 0 : (array.Length + 1));
			int num2 = ((this.FileName == null) ? 0 : (array2.Length + 1));
			byte[] array3 = new byte[10 + num + num2];
			int num3 = 0;
			array3[num3++] = 31;
			array3[num3++] = 139;
			array3[num3++] = 8;
			byte b = 0;
			if (this.Comment != null)
			{
				b ^= 16;
			}
			if (this.FileName != null)
			{
				b ^= 8;
			}
			array3[num3++] = b;
			if (this.LastModified == null)
			{
				this.LastModified = new DateTime?(DateTime.Now);
			}
			Array.Copy(BitConverter.GetBytes((int)(this.LastModified.Value - GZipStream._unixEpoch).TotalSeconds), 0, array3, num3, 4);
			num3 += 4;
			array3[num3++] = 0;
			array3[num3++] = byte.MaxValue;
			if (num2 != 0)
			{
				Array.Copy(array2, 0, array3, num3, num2 - 1);
				num3 += num2 - 1;
				array3[num3++] = 0;
			}
			if (num != 0)
			{
				Array.Copy(array, 0, array3, num3, num - 1);
				num3 += num - 1;
				array3[num3++] = 0;
			}
			this._baseStream._stream.Write(array3, 0, array3.Length);
			return array3.Length;
		}

		public static byte[] CompressString(string s)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Stream stream = new GZipStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestCompression);
				ZlibBaseStream.CompressString(s, stream);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public static byte[] CompressBuffer(byte[] b)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Stream stream = new GZipStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestCompression);
				ZlibBaseStream.CompressBuffer(b, stream);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public static string UncompressString(byte[] compressed)
		{
			string text;
			using (MemoryStream memoryStream = new MemoryStream(compressed))
			{
				Stream stream = new GZipStream(memoryStream, CompressionMode.Decompress);
				text = ZlibBaseStream.UncompressString(compressed, stream);
			}
			return text;
		}

		public static byte[] UncompressBuffer(byte[] compressed)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream(compressed))
			{
				Stream stream = new GZipStream(memoryStream, CompressionMode.Decompress);
				array = ZlibBaseStream.UncompressBuffer(compressed, stream);
			}
			return array;
		}

		public DateTime? LastModified;

		private int _headerByteCount;

		internal ZlibBaseStream _baseStream;

		private bool _disposed;

		private bool _firstReadDone;

		private string _FileName;

		private string _Comment;

		private int _Crc32;

		internal static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		internal static readonly Encoding iso8859dash1 = Encoding.GetEncoding("iso-8859-1");
	}
}
