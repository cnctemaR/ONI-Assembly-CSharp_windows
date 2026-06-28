using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Ionic.Zip
{
	internal class WinZipAesCipherStream : Stream
	{
		internal WinZipAesCipherStream(Stream s, WinZipAesCrypto cryptoParams, long length, CryptoMode mode)
			: this(s, cryptoParams, mode)
		{
			this._length = length;
		}

		internal WinZipAesCipherStream(Stream s, WinZipAesCrypto cryptoParams, CryptoMode mode)
		{
			this._params = cryptoParams;
			this._s = s;
			this._mode = mode;
			this._nonce = 1;
			if (this._params == null)
			{
				throw new BadPasswordException("Supply a password to use AES encryption.");
			}
			int num = this._params.KeyBytes.Length * 8;
			if (num != 256 && num != 128 && num != 192)
			{
				throw new ArgumentOutOfRangeException("keysize", "size of key must be 128, 192, or 256");
			}
			this._mac = new HMACSHA1(this._params.MacIv);
			this._aesCipher = new RijndaelManaged();
			this._aesCipher.BlockSize = 128;
			this._aesCipher.KeySize = num;
			this._aesCipher.Mode = CipherMode.ECB;
			this._aesCipher.Padding = PaddingMode.None;
			byte[] array = new byte[16];
			this._xform = this._aesCipher.CreateEncryptor(this._params.KeyBytes, array);
			if (this._mode == CryptoMode.Encrypt)
			{
				this._iobuf = new byte[2048];
				this._PendingWriteBlock = new byte[16];
			}
		}

		private void XorInPlace(byte[] buffer, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				buffer[offset + i] = this.counterOut[i] ^ buffer[offset + i];
			}
		}

		private void WriteTransformOneBlock(byte[] buffer, int offset)
		{
			int nonce = this._nonce;
			this._nonce = nonce + 1;
			Array.Copy(BitConverter.GetBytes(nonce), 0, this.counter, 0, 4);
			this._xform.TransformBlock(this.counter, 0, 16, this.counterOut, 0);
			this.XorInPlace(buffer, offset, 16);
			this._mac.TransformBlock(buffer, offset, 16, null, 0);
		}

		private void WriteTransformBlocks(byte[] buffer, int offset, int count)
		{
			int num = offset;
			int num2 = count + offset;
			while (num < buffer.Length && num < num2)
			{
				this.WriteTransformOneBlock(buffer, num);
				num += 16;
			}
		}

		private void WriteTransformFinalBlock()
		{
			if (this._pendingCount == 0)
			{
				throw new InvalidOperationException("No bytes available.");
			}
			if (this._finalBlock)
			{
				throw new InvalidOperationException("The final block has already been transformed.");
			}
			int nonce = this._nonce;
			this._nonce = nonce + 1;
			Array.Copy(BitConverter.GetBytes(nonce), 0, this.counter, 0, 4);
			this.counterOut = this._xform.TransformFinalBlock(this.counter, 0, 16);
			this.XorInPlace(this._PendingWriteBlock, 0, this._pendingCount);
			this._mac.TransformFinalBlock(this._PendingWriteBlock, 0, this._pendingCount);
			this._finalBlock = true;
		}

		private int ReadTransformOneBlock(byte[] buffer, int offset, int last)
		{
			if (this._finalBlock)
			{
				throw new NotSupportedException();
			}
			int num = last - offset;
			int num2 = ((num > 16) ? 16 : num);
			int nonce = this._nonce;
			this._nonce = nonce + 1;
			Array.Copy(BitConverter.GetBytes(nonce), 0, this.counter, 0, 4);
			if (num2 == num && this._length > 0L && this._totalBytesXferred + (long)last == this._length)
			{
				this._mac.TransformFinalBlock(buffer, offset, num2);
				this.counterOut = this._xform.TransformFinalBlock(this.counter, 0, 16);
				this._finalBlock = true;
			}
			else
			{
				this._mac.TransformBlock(buffer, offset, num2, null, 0);
				this._xform.TransformBlock(this.counter, 0, 16, this.counterOut, 0);
			}
			this.XorInPlace(buffer, offset, num2);
			return num2;
		}

		private void ReadTransformBlocks(byte[] buffer, int offset, int count)
		{
			int num = offset;
			int num2 = count + offset;
			while (num < buffer.Length && num < num2)
			{
				int num3 = this.ReadTransformOneBlock(buffer, num, num2);
				num += num3;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this._mode == CryptoMode.Encrypt)
			{
				throw new NotSupportedException();
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must not be less than zero.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must not be less than zero.");
			}
			if (buffer.Length < offset + count)
			{
				throw new ArgumentException("The buffer is too small");
			}
			int num = count;
			if (this._totalBytesXferred >= this._length)
			{
				return 0;
			}
			long num2 = this._length - this._totalBytesXferred;
			if (num2 < (long)count)
			{
				num = (int)num2;
			}
			int num3 = this._s.Read(buffer, offset, num);
			this.ReadTransformBlocks(buffer, offset, num);
			this._totalBytesXferred += (long)num3;
			return num3;
		}

		public byte[] FinalAuthentication
		{
			get
			{
				if (!this._finalBlock)
				{
					if (this._totalBytesXferred != 0L)
					{
						throw new BadStateException("The final hash has not been computed.");
					}
					byte[] array = new byte[0];
					this._mac.ComputeHash(array);
				}
				byte[] array2 = new byte[10];
				Array.Copy(this._mac.Hash, 0, array2, 0, 10);
				return array2;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this._finalBlock)
			{
				throw new InvalidOperationException("The final block has already been transformed.");
			}
			if (this._mode == CryptoMode.Decrypt)
			{
				throw new NotSupportedException();
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must not be less than zero.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must not be less than zero.");
			}
			if (buffer.Length < offset + count)
			{
				throw new ArgumentException("The offset and count are too large");
			}
			if (count == 0)
			{
				return;
			}
			if (count + this._pendingCount <= 16)
			{
				Buffer.BlockCopy(buffer, offset, this._PendingWriteBlock, this._pendingCount, count);
				this._pendingCount += count;
				return;
			}
			int num = count;
			int num2 = offset;
			if (this._pendingCount != 0)
			{
				int num3 = 16 - this._pendingCount;
				if (num3 > 0)
				{
					Buffer.BlockCopy(buffer, offset, this._PendingWriteBlock, this._pendingCount, num3);
					num -= num3;
					num2 += num3;
				}
				this.WriteTransformOneBlock(this._PendingWriteBlock, 0);
				this._s.Write(this._PendingWriteBlock, 0, 16);
				this._totalBytesXferred += 16L;
				this._pendingCount = 0;
			}
			int num4 = (num - 1) / 16;
			this._pendingCount = num - num4 * 16;
			Buffer.BlockCopy(buffer, num2 + num - this._pendingCount, this._PendingWriteBlock, 0, this._pendingCount);
			num -= this._pendingCount;
			this._totalBytesXferred += (long)num;
			if (num4 > 0)
			{
				do
				{
					int num5 = this._iobuf.Length;
					if (num5 > num)
					{
						num5 = num;
					}
					Buffer.BlockCopy(buffer, num2, this._iobuf, 0, num5);
					this.WriteTransformBlocks(this._iobuf, 0, num5);
					this._s.Write(this._iobuf, 0, num5);
					num -= num5;
					num2 += num5;
				}
				while (num > 0);
			}
		}

		public override void Close()
		{
			if (this._pendingCount > 0)
			{
				this.WriteTransformFinalBlock();
				this._s.Write(this._PendingWriteBlock, 0, this._pendingCount);
				this._totalBytesXferred += (long)this._pendingCount;
				this._pendingCount = 0;
			}
			this._s.Close();
		}

		public override bool CanRead
		{
			get
			{
				return this._mode == CryptoMode.Decrypt;
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
				return this._mode == CryptoMode.Encrypt;
			}
		}

		public override void Flush()
		{
			this._s.Flush();
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
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		[Conditional("Trace")]
		private void TraceOutput(string format, params object[] varParams)
		{
			object outputLock = this._outputLock;
			lock (outputLock)
			{
				int hashCode = Thread.CurrentThread.GetHashCode();
				Console.ForegroundColor = hashCode % 8 + ConsoleColor.DarkGray;
				Console.Write("{0:000} WZACS ", hashCode);
				Console.WriteLine(format, varParams);
				Console.ResetColor();
			}
		}

		private WinZipAesCrypto _params;

		private Stream _s;

		private CryptoMode _mode;

		private int _nonce;

		private bool _finalBlock;

		internal HMACSHA1 _mac;

		internal RijndaelManaged _aesCipher;

		internal ICryptoTransform _xform;

		private const int BLOCK_SIZE_IN_BYTES = 16;

		private byte[] counter = new byte[16];

		private byte[] counterOut = new byte[16];

		private long _length;

		private long _totalBytesXferred;

		private byte[] _PendingWriteBlock;

		private int _pendingCount;

		private byte[] _iobuf;

		private object _outputLock = new object();
	}
}
