using System;
using System.IO;
using System.IO.Pipes;

namespace System.Data.SqlClient.SNI
{
	internal sealed class SslOverTdsStream : Stream
	{
		public SslOverTdsStream(Stream stream)
		{
			this._stream = stream;
			this._encapsulate = true;
		}

		public void FinishHandshake()
		{
			this._encapsulate = false;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int i = 0;
			byte[] array = new byte[(count < 8) ? 8 : count];
			if (this._encapsulate)
			{
				if (this._packetBytes == 0)
				{
					while (i < 8)
					{
						i += this._stream.Read(array, i, 8 - i);
					}
					this._packetBytes = ((int)array[2] << 8) | (int)array[3];
					this._packetBytes -= 8;
				}
				if (count > this._packetBytes)
				{
					count = this._packetBytes;
				}
			}
			i = this._stream.Read(array, 0, count);
			if (this._encapsulate)
			{
				this._packetBytes -= i;
			}
			Buffer.BlockCopy(array, 0, buffer, offset, i);
			return i;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			int num = offset;
			while (count > 0)
			{
				int num2;
				if (this._encapsulate)
				{
					if (count > 4088)
					{
						num2 = 4088;
					}
					else
					{
						num2 = count;
					}
					count -= num2;
					byte[] array = new byte[8 + num2];
					array[0] = 18;
					array[1] = ((count > 0) ? 0 : 1);
					array[2] = (byte)((num2 + 8) / 256);
					array[3] = (byte)((num2 + 8) % 256);
					array[4] = 0;
					array[5] = 0;
					array[6] = 0;
					array[7] = 0;
					for (int i = 8; i < array.Length; i++)
					{
						array[i] = buffer[num + (i - 8)];
					}
					this._stream.Write(array, 0, array.Length);
				}
				else
				{
					num2 = count;
					count = 0;
					this._stream.Write(buffer, num, num2);
				}
				this._stream.Flush();
				num += num2;
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			if (!(this._stream is PipeStream))
			{
				this._stream.Flush();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override bool CanRead
		{
			get
			{
				return this._stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._stream.CanWrite;
			}
		}

		public override bool CanSeek
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
				throw new NotSupportedException();
			}
		}

		private readonly Stream _stream;

		private int _packetBytes;

		private bool _encapsulate;

		private const int PACKET_SIZE_WITHOUT_HEADER = 4088;

		private const int PRELOGIN_PACKET_TYPE = 18;
	}
}
