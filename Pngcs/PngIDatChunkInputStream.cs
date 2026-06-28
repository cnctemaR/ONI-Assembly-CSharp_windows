using System;
using System.Collections.Generic;
using System.IO;
using Hjg.Pngcs.Chunks;
using Hjg.Pngcs.Zlib;

namespace Hjg.Pngcs
{
	internal class PngIDatChunkInputStream : Stream
	{
		public override void Write(byte[] buffer, int offset, int count)
		{
		}

		public override void SetLength(long value)
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return -1L;
		}

		public override void Flush()
		{
		}

		public override long Position { get; set; }

		public override long Length
		{
			get
			{
				return 0L;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
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
				return false;
			}
		}

		public PngIDatChunkInputStream(Stream iStream, int lenFirstChunk, long offset_0)
		{
			this.idLastChunk = new byte[4];
			this.toReadThisChunk = 0;
			this.ended = false;
			this.foundChunksInfo = new List<PngIDatChunkInputStream.IdatChunkInfo>();
			this.offset = offset_0;
			this.checkCrc = true;
			this.inputStream = iStream;
			this.crcEngine = new CRC32();
			this.lenLastChunk = lenFirstChunk;
			this.toReadThisChunk = lenFirstChunk;
			Array.Copy(ChunkHelper.b_IDAT, 0, this.idLastChunk, 0, 4);
			this.crcEngine.Update(this.idLastChunk, 0, 4);
			this.foundChunksInfo.Add(new PngIDatChunkInputStream.IdatChunkInfo(this.lenLastChunk, offset_0 - 8L));
			if (this.lenLastChunk == 0)
			{
				this.EndChunkGoForNext();
			}
		}

		public override void Close()
		{
			base.Close();
		}

		private void EndChunkGoForNext()
		{
			for (;;)
			{
				int num = PngHelperInternal.ReadInt4(this.inputStream);
				this.offset += 4L;
				if (this.checkCrc)
				{
					int value = (int)this.crcEngine.GetValue();
					if (this.lenLastChunk > 0 && num != value)
					{
						break;
					}
					this.crcEngine.Reset();
				}
				this.lenLastChunk = PngHelperInternal.ReadInt4(this.inputStream);
				if (this.lenLastChunk < 0)
				{
					goto Block_3;
				}
				this.toReadThisChunk = this.lenLastChunk;
				PngHelperInternal.ReadBytes(this.inputStream, this.idLastChunk, 0, 4);
				this.offset += 8L;
				this.ended = !PngCsUtils.arraysEqual4(this.idLastChunk, ChunkHelper.b_IDAT);
				if (!this.ended)
				{
					this.foundChunksInfo.Add(new PngIDatChunkInputStream.IdatChunkInfo(this.lenLastChunk, this.offset - 8L));
					if (this.checkCrc)
					{
						this.crcEngine.Update(this.idLastChunk, 0, 4);
					}
				}
				if (this.lenLastChunk != 0 || this.ended)
				{
					return;
				}
			}
			throw new PngjBadCrcException("error reading idat; offset: " + this.offset);
			Block_3:
			throw new PngjInputException("invalid len for chunk: " + this.lenLastChunk);
		}

		public void ForceChunkEnd()
		{
			if (!this.ended)
			{
				byte[] array = new byte[this.toReadThisChunk];
				PngHelperInternal.ReadBytes(this.inputStream, array, 0, this.toReadThisChunk);
				if (this.checkCrc)
				{
					this.crcEngine.Update(array, 0, this.toReadThisChunk);
				}
				this.EndChunkGoForNext();
			}
		}

		public override int Read(byte[] b, int off, int len_0)
		{
			if (this.ended)
			{
				return -1;
			}
			if (this.toReadThisChunk == 0)
			{
				throw new Exception("this should not happen");
			}
			int num = this.inputStream.Read(b, off, (len_0 >= this.toReadThisChunk) ? this.toReadThisChunk : len_0);
			if (num == -1)
			{
				num = -2;
			}
			if (num > 0)
			{
				if (this.checkCrc)
				{
					this.crcEngine.Update(b, off, num);
				}
				this.offset += (long)num;
				this.toReadThisChunk -= num;
			}
			if (num >= 0 && this.toReadThisChunk == 0)
			{
				this.EndChunkGoForNext();
			}
			return num;
		}

		public int Read(byte[] b)
		{
			return this.Read(b, 0, b.Length);
		}

		public override int ReadByte()
		{
			byte[] array = new byte[1];
			int num = this.Read(array, 0, 1);
			if (num >= 0)
			{
				return (int)array[0];
			}
			return -1;
		}

		public int GetLenLastChunk()
		{
			return this.lenLastChunk;
		}

		public byte[] GetIdLastChunk()
		{
			return this.idLastChunk;
		}

		public long GetOffset()
		{
			return this.offset;
		}

		public bool IsEnded()
		{
			return this.ended;
		}

		internal void DisableCrcCheck()
		{
			this.checkCrc = false;
		}

		private readonly Stream inputStream;

		private readonly CRC32 crcEngine;

		private bool checkCrc;

		private int lenLastChunk;

		private byte[] idLastChunk;

		private int toReadThisChunk;

		private bool ended;

		private long offset;

		public IList<PngIDatChunkInputStream.IdatChunkInfo> foundChunksInfo;

		public class IdatChunkInfo
		{
			public IdatChunkInfo(int len_0, long offset_1)
			{
				this.len = len_0;
				this.offset = offset_1;
			}

			public readonly int len;

			public readonly long offset;
		}
	}
}
