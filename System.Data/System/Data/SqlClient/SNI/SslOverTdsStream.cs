using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

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
			return this.ReadInternal(buffer, offset, count, CancellationToken.None, false).GetAwaiter().GetResult();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.WriteInternal(buffer, offset, count, CancellationToken.None, false).Wait();
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
		{
			return this.WriteInternal(buffer, offset, count, token, true);
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
		{
			return this.ReadInternal(buffer, offset, count, token, true);
		}

		private async Task<int> ReadInternal(byte[] buffer, int offset, int count, CancellationToken token, bool async)
		{
			int i = 0;
			byte[] packetData = new byte[(count < 8) ? 8 : count];
			int num2;
			if (this._encapsulate)
			{
				if (this._packetBytes == 0)
				{
					while (i < 8)
					{
						int num = i;
						if (async)
						{
							num2 = await this._stream.ReadAsync(packetData, i, 8 - i, token).ConfigureAwait(false);
						}
						else
						{
							num2 = this._stream.Read(packetData, i, 8 - i);
						}
						i = num + num2;
					}
					this._packetBytes = ((int)packetData[2] << 8) | (int)packetData[3];
					this._packetBytes -= 8;
				}
				if (count > this._packetBytes)
				{
					count = this._packetBytes;
				}
			}
			if (async)
			{
				num2 = await this._stream.ReadAsync(packetData, 0, count, token).ConfigureAwait(false);
			}
			else
			{
				num2 = this._stream.Read(packetData, 0, count);
			}
			i = num2;
			if (this._encapsulate)
			{
				this._packetBytes -= i;
			}
			Buffer.BlockCopy(packetData, 0, buffer, offset, i);
			return i;
		}

		private async Task WriteInternal(byte[] buffer, int offset, int count, CancellationToken token, bool async)
		{
			int currentCount = 0;
			int currentOffset = offset;
			while (count > 0)
			{
				if (this._encapsulate)
				{
					if (count > 4088)
					{
						currentCount = 4088;
					}
					else
					{
						currentCount = count;
					}
					count -= currentCount;
					byte[] array = new byte[8 + currentCount];
					array[0] = 18;
					array[1] = ((count > 0) ? 0 : 1);
					array[2] = (byte)((currentCount + 8) / 256);
					array[3] = (byte)((currentCount + 8) % 256);
					array[4] = 0;
					array[5] = 0;
					array[6] = 0;
					array[7] = 0;
					for (int i = 8; i < array.Length; i++)
					{
						array[i] = buffer[currentOffset + (i - 8)];
					}
					if (async)
					{
						await this._stream.WriteAsync(array, 0, array.Length, token).ConfigureAwait(false);
					}
					else
					{
						this._stream.Write(array, 0, array.Length);
					}
				}
				else
				{
					currentCount = count;
					count = 0;
					if (async)
					{
						await this._stream.WriteAsync(buffer, currentOffset, currentCount, token).ConfigureAwait(false);
					}
					else
					{
						this._stream.Write(buffer, currentOffset, currentCount);
					}
				}
				if (async)
				{
					await this._stream.FlushAsync().ConfigureAwait(false);
				}
				else
				{
					this._stream.Flush();
				}
				currentOffset += currentCount;
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
