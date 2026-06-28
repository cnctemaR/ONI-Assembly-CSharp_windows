using System;
using System.IO;
using System.Text;

namespace FileHelpers
{
	[Serializable]
	internal sealed class InternalStreamReader : TextReader
	{
		internal InternalStreamReader()
		{
		}

		public InternalStreamReader(string path)
			: this(path, Encoding.UTF8)
		{
		}

		public InternalStreamReader(string path, Encoding encoding)
			: this(path, encoding, true, 1024)
		{
		}

		public InternalStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
		{
			if (path == null || encoding == null)
			{
				throw new ArgumentNullException((path == null) ? "path" : "encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Empty path", "path");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "bufferSize must be positive");
			}
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan);
			this.Init(fileStream, encoding, detectEncodingFromByteOrderMarks, bufferSize);
		}

		public override void Close()
		{
			this.Dispose(true);
		}

		private void CompressBuffer(int n)
		{
			for (int i = 0; i < this.mByteLen - n; i++)
			{
				this.mByteBuffer[i] = this.mByteBuffer[i + n];
			}
			this.mByteLen -= n;
		}

		private void DetectEncoding()
		{
			if (this.mByteLen >= 2)
			{
				this.mDetectEncoding = false;
				bool flag = false;
				if (this.mByteBuffer[0] == 254 && this.mByteBuffer[1] == 255)
				{
					this.mEncoding = new UnicodeEncoding(true, true);
					this.CompressBuffer(2);
					flag = true;
				}
				else if (this.mByteBuffer[0] == 255 && this.mByteBuffer[1] == 254)
				{
					if (this.mByteLen >= 4 && this.mByteBuffer[2] == 0 && this.mByteBuffer[3] == 0)
					{
						this.mEncoding = new UTF32Encoding(false, true);
						this.CompressBuffer(4);
					}
					else
					{
						this.mEncoding = new UnicodeEncoding(false, true);
						this.CompressBuffer(2);
					}
					flag = true;
				}
				else if (this.mByteLen >= 3 && this.mByteBuffer[0] == 239 && this.mByteBuffer[1] == 187 && this.mByteBuffer[2] == 191)
				{
					this.mEncoding = Encoding.UTF8;
					this.CompressBuffer(3);
					flag = true;
				}
				else if (this.mByteLen >= 4 && this.mByteBuffer[0] == 0 && this.mByteBuffer[1] == 0 && this.mByteBuffer[2] == 254 && this.mByteBuffer[3] == 255)
				{
					this.mEncoding = new UTF32Encoding(true, true);
					flag = true;
				}
				else if (this.mByteLen == 2)
				{
					this.mDetectEncoding = true;
				}
				if (flag)
				{
					this.mDecoder = this.mEncoding.GetDecoder();
					this.mMaxCharsPerBuffer = this.mEncoding.GetMaxCharCount(this.mByteBuffer.Length);
					this.mCharBuffer = new char[this.mMaxCharsPerBuffer];
				}
			}
		}

		public void DiscardBufferedData()
		{
			this.mByteLen = 0;
			this.mCharLen = 0;
			this.mCharPos = 0;
			this.mDecoder = this.mEncoding.GetDecoder();
			this.mIsBlocked = false;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this.Closable && disposing && this.mStream != null)
				{
					this.mStream.Close();
				}
			}
			finally
			{
				if (this.Closable && this.mStream != null)
				{
					this.mStream = null;
					this.mEncoding = null;
					this.mDecoder = null;
					this.mByteBuffer = null;
					this.mCharBuffer = null;
					this.mCharPos = 0;
					this.mCharLen = 0;
					base.Dispose(disposing);
				}
			}
		}

		private void Init(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
		{
			this.mStream = stream;
			this.mEncoding = encoding;
			this.mDecoder = encoding.GetDecoder();
			if (bufferSize < 128)
			{
				bufferSize = 128;
			}
			this.mByteBuffer = new byte[bufferSize];
			this.mMaxCharsPerBuffer = encoding.GetMaxCharCount(bufferSize);
			this.mCharBuffer = new char[this.mMaxCharsPerBuffer];
			this.mByteLen = 0;
			this.mBytePos = 0;
			this.mDetectEncoding = detectEncodingFromByteOrderMarks;
			this.mPreamble = encoding.GetPreamble();
			this.mCheckPreamble = this.mPreamble.Length > 0;
			this.mIsBlocked = false;
			this.mClosable = true;
		}

		private bool IsPreamble()
		{
			if (this.mCheckPreamble)
			{
				int num = ((this.mByteLen >= this.mPreamble.Length) ? (this.mPreamble.Length - this.mBytePos) : (this.mByteLen - this.mBytePos));
				int i = 0;
				while (i < num)
				{
					if (this.mByteBuffer[this.mBytePos] != this.mPreamble[this.mBytePos])
					{
						this.mBytePos = 0;
						this.mCheckPreamble = false;
						break;
					}
					i++;
					this.mBytePos++;
				}
				if (this.mCheckPreamble && this.mBytePos == this.mPreamble.Length)
				{
					this.CompressBuffer(this.mPreamble.Length);
					this.mBytePos = 0;
					this.mCheckPreamble = false;
					this.mDetectEncoding = false;
				}
			}
			return this.mCheckPreamble;
		}

		public override int Peek()
		{
			if (this.mStream == null)
			{
				throw new ObjectDisposedException(null, "The reader is closed");
			}
			if (this.mCharPos != this.mCharLen || (!this.mIsBlocked && this.ReadBuffer() != 0))
			{
				return (int)this.mCharBuffer[this.mCharPos];
			}
			return -1;
		}

		public override int Read()
		{
			if (this.mStream == null)
			{
				throw new ObjectDisposedException(null, "The reader is closed");
			}
			if (this.mCharPos == this.mCharLen && this.ReadBuffer() == 0)
			{
				return -1;
			}
			int num = (int)this.mCharBuffer[this.mCharPos];
			this.mCharPos++;
			return num;
		}

		public long Position
		{
			get
			{
				return this.mStream.Position + (long)this.mCharPos - (long)this.mCharLen;
			}
		}

		private int ReadBuffer()
		{
			this.mCharLen = 0;
			this.mCharPos = 0;
			if (!this.mCheckPreamble)
			{
				this.mByteLen = 0;
			}
			for (;;)
			{
				if (this.mCheckPreamble)
				{
					int num = this.mStream.Read(this.mByteBuffer, this.mBytePos, this.mByteBuffer.Length - this.mBytePos);
					if (num == 0)
					{
						break;
					}
					this.mByteLen += num;
				}
				else
				{
					this.mByteLen = this.mStream.Read(this.mByteBuffer, 0, this.mByteBuffer.Length);
					if (this.mByteLen == 0)
					{
						goto Block_5;
					}
				}
				this.mIsBlocked = this.mByteLen < this.mByteBuffer.Length;
				if (!this.IsPreamble())
				{
					if (this.mDetectEncoding && this.mByteLen >= 2)
					{
						this.DetectEncoding();
					}
					this.mCharLen += this.mDecoder.GetChars(this.mByteBuffer, 0, this.mByteLen, this.mCharBuffer, this.mCharLen);
				}
				if (this.mCharLen != 0)
				{
					goto Block_9;
				}
			}
			if (this.mByteLen > 0)
			{
				this.mCharLen += this.mDecoder.GetChars(this.mByteBuffer, 0, this.mByteLen, this.mCharBuffer, this.mCharLen);
			}
			return this.mCharLen;
			Block_5:
			return this.mCharLen;
			Block_9:
			return this.mCharLen;
		}

		public override string ReadLine()
		{
			if (this.mStream == null)
			{
				throw new ObjectDisposedException(null, "The reader is closed");
			}
			if (this.mCharPos == this.mCharLen && this.ReadBuffer() == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = null;
			int num;
			char c;
			for (;;)
			{
				num = this.mCharPos;
				do
				{
					c = this.mCharBuffer[num];
					char c2 = c;
					if (c2 == '\n' || c2 == '\r')
					{
						goto IL_0050;
					}
					num++;
				}
				while (num < this.mCharLen);
				num = this.mCharLen - this.mCharPos;
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(num + 80);
				}
				stringBuilder.Append(this.mCharBuffer, this.mCharPos, num);
				if (this.ReadBuffer() <= 0)
				{
					goto Block_11;
				}
			}
			IL_0050:
			string text;
			if (stringBuilder != null)
			{
				stringBuilder.Append(this.mCharBuffer, this.mCharPos, num - this.mCharPos);
				text = stringBuilder.ToString();
			}
			else
			{
				text = new string(this.mCharBuffer, this.mCharPos, num - this.mCharPos);
			}
			this.mCharPos = num + 1;
			if (c == '\r' && (this.mCharPos < this.mCharLen || this.ReadBuffer() > 0) && this.mCharBuffer[this.mCharPos] == '\n')
			{
				this.mCharPos++;
			}
			return text;
			Block_11:
			return stringBuilder.ToString();
		}

		internal bool Closable
		{
			get
			{
				return this.mClosable;
			}
		}

		public Encoding CurrentEncoding
		{
			get
			{
				return this.mEncoding;
			}
		}

		public Stream BaseStream
		{
			get
			{
				return this.mStream;
			}
		}

		public bool EndOfStream
		{
			get
			{
				if (this.mStream == null)
				{
					throw new ObjectDisposedException(null, "The reader is closed");
				}
				return this.mCharPos >= this.mCharLen && this.ReadBuffer() == 0;
			}
		}

		private const int DefaultBufferSize = 1024;

		private const int DefaultFileStreamBufferSize = 4096;

		private const int MinBufferSize = 128;

		private bool mCheckPreamble;

		private bool mClosable;

		private bool mDetectEncoding;

		private bool mIsBlocked;

		private int mMaxCharsPerBuffer;

		private byte[] mPreamble;

		private byte[] mByteBuffer;

		private int mByteLen;

		private int mBytePos;

		private char[] mCharBuffer;

		private int mCharLen;

		private int mCharPos;

		private Decoder mDecoder;

		private Encoding mEncoding;

		private Stream mStream;
	}
}
