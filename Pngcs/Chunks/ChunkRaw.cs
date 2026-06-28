using System;
using System.IO;
using Hjg.Pngcs.Zlib;

namespace Hjg.Pngcs.Chunks
{
	public class ChunkRaw
	{
		internal ChunkRaw(int length, byte[] idbytes, bool alloc)
		{
			this.IdBytes = new byte[4];
			this.Data = null;
			this.crcval = 0;
			this.Length = length;
			Array.Copy(idbytes, 0, this.IdBytes, 0, 4);
			if (alloc)
			{
				this.AllocData();
			}
		}

		private int ComputeCrc()
		{
			CRC32 crc = PngHelperInternal.GetCRC();
			crc.Reset();
			crc.Update(this.IdBytes, 0, 4);
			if (this.Length > 0)
			{
				crc.Update(this.Data, 0, this.Length);
			}
			return (int)crc.GetValue();
		}

		internal void WriteChunk(Stream os)
		{
			if (this.IdBytes.Length != 4)
			{
				throw new PngjOutputException("bad chunkid [" + ChunkHelper.ToString(this.IdBytes) + "]");
			}
			this.crcval = this.ComputeCrc();
			PngHelperInternal.WriteInt4(os, this.Length);
			PngHelperInternal.WriteBytes(os, this.IdBytes);
			if (this.Length > 0)
			{
				PngHelperInternal.WriteBytes(os, this.Data, 0, this.Length);
			}
			PngHelperInternal.WriteInt4(os, this.crcval);
		}

		internal int ReadChunkData(Stream stream, bool checkCrc)
		{
			PngHelperInternal.ReadBytes(stream, this.Data, 0, this.Length);
			this.crcval = PngHelperInternal.ReadInt4(stream);
			if (checkCrc)
			{
				int num = this.ComputeCrc();
				if (num != this.crcval)
				{
					throw new PngjBadCrcException(string.Concat(new object[]
					{
						"crc invalid for chunk ",
						this.ToString(),
						" calc=",
						num,
						" read=",
						this.crcval
					}));
				}
			}
			return this.Length + 4;
		}

		internal MemoryStream GetAsByteStream()
		{
			return new MemoryStream(this.Data);
		}

		private void AllocData()
		{
			if (this.Data == null || this.Data.Length < this.Length)
			{
				this.Data = new byte[this.Length];
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"chunkid=",
				ChunkHelper.ToString(this.IdBytes),
				" len=",
				this.Length
			});
		}

		public readonly int Length;

		public readonly byte[] IdBytes;

		public byte[] Data;

		private int crcval;
	}
}
