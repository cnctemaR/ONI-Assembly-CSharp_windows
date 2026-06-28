using System;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkIHDR : PngChunkSingle
	{
		public int Cols { get; set; }

		public int Rows { get; set; }

		public int Bitspc { get; set; }

		public int Colormodel { get; set; }

		public int Compmeth { get; set; }

		public int Filmeth { get; set; }

		public int Interlaced { get; set; }

		public PngChunkIHDR(ImageInfo info)
			: base("IHDR", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = new ChunkRaw(13, ChunkHelper.b_IHDR, true);
			int num = 0;
			PngHelperInternal.WriteInt4tobytes(this.Cols, chunkRaw.Data, num);
			num += 4;
			PngHelperInternal.WriteInt4tobytes(this.Rows, chunkRaw.Data, num);
			num += 4;
			chunkRaw.Data[num++] = (byte)this.Bitspc;
			chunkRaw.Data[num++] = (byte)this.Colormodel;
			chunkRaw.Data[num++] = (byte)this.Compmeth;
			chunkRaw.Data[num++] = (byte)this.Filmeth;
			chunkRaw.Data[num++] = (byte)this.Interlaced;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (c.Length != 13)
			{
				throw new PngjException("Bad IDHR len " + c.Length);
			}
			MemoryStream asByteStream = c.GetAsByteStream();
			this.Cols = PngHelperInternal.ReadInt4(asByteStream);
			this.Rows = PngHelperInternal.ReadInt4(asByteStream);
			this.Bitspc = PngHelperInternal.ReadByte(asByteStream);
			this.Colormodel = PngHelperInternal.ReadByte(asByteStream);
			this.Compmeth = PngHelperInternal.ReadByte(asByteStream);
			this.Filmeth = PngHelperInternal.ReadByte(asByteStream);
			this.Interlaced = PngHelperInternal.ReadByte(asByteStream);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkIHDR pngChunkIHDR = (PngChunkIHDR)other;
			this.Cols = pngChunkIHDR.Cols;
			this.Rows = pngChunkIHDR.Rows;
			this.Bitspc = pngChunkIHDR.Bitspc;
			this.Colormodel = pngChunkIHDR.Colormodel;
			this.Compmeth = pngChunkIHDR.Compmeth;
			this.Filmeth = pngChunkIHDR.Filmeth;
			this.Interlaced = pngChunkIHDR.Interlaced;
		}

		public const string ID = "IHDR";
	}
}
