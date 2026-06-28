using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkSBIT : PngChunkSingle
	{
		public int Graysb { get; set; }

		public int Alphasb { get; set; }

		public int Redsb { get; set; }

		public int Greensb { get; set; }

		public int Bluesb { get; set; }

		public PngChunkSBIT(ImageInfo info)
			: base("sBIT", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (c.Length != this.GetLen())
			{
				throw new PngjException("bad chunk length " + c);
			}
			if (this.ImgInfo.Greyscale)
			{
				this.Graysb = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
				if (this.ImgInfo.Alpha)
				{
					this.Alphasb = PngHelperInternal.ReadInt1fromByte(c.Data, 1);
					return;
				}
			}
			else
			{
				this.Redsb = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
				this.Greensb = PngHelperInternal.ReadInt1fromByte(c.Data, 1);
				this.Bluesb = PngHelperInternal.ReadInt1fromByte(c.Data, 2);
				if (this.ImgInfo.Alpha)
				{
					this.Alphasb = PngHelperInternal.ReadInt1fromByte(c.Data, 3);
				}
			}
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = base.createEmptyChunk(this.GetLen(), true);
			if (this.ImgInfo.Greyscale)
			{
				chunkRaw.Data[0] = (byte)this.Graysb;
				if (this.ImgInfo.Alpha)
				{
					chunkRaw.Data[1] = (byte)this.Alphasb;
				}
			}
			else
			{
				chunkRaw.Data[0] = (byte)this.Redsb;
				chunkRaw.Data[1] = (byte)this.Greensb;
				chunkRaw.Data[2] = (byte)this.Bluesb;
				if (this.ImgInfo.Alpha)
				{
					chunkRaw.Data[3] = (byte)this.Alphasb;
				}
			}
			return chunkRaw;
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkSBIT pngChunkSBIT = (PngChunkSBIT)other;
			this.Graysb = pngChunkSBIT.Graysb;
			this.Redsb = pngChunkSBIT.Redsb;
			this.Greensb = pngChunkSBIT.Greensb;
			this.Bluesb = pngChunkSBIT.Bluesb;
			this.Alphasb = pngChunkSBIT.Alphasb;
		}

		private int GetLen()
		{
			int num = (this.ImgInfo.Greyscale ? 1 : 3);
			if (this.ImgInfo.Alpha)
			{
				num++;
			}
			return num;
		}

		public const string ID = "sBIT";
	}
}
