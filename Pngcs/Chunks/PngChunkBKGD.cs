using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkBKGD : PngChunkSingle
	{
		public PngChunkBKGD(ImageInfo info)
			: base("bKGD", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw;
			if (this.ImgInfo.Greyscale)
			{
				chunkRaw = base.createEmptyChunk(2, true);
				PngHelperInternal.WriteInt2tobytes(this.gray, chunkRaw.Data, 0);
			}
			else if (this.ImgInfo.Indexed)
			{
				chunkRaw = base.createEmptyChunk(1, true);
				chunkRaw.Data[0] = (byte)this.paletteIndex;
			}
			else
			{
				chunkRaw = base.createEmptyChunk(6, true);
				PngHelperInternal.WriteInt2tobytes(this.red, chunkRaw.Data, 0);
				PngHelperInternal.WriteInt2tobytes(this.green, chunkRaw.Data, 0);
				PngHelperInternal.WriteInt2tobytes(this.blue, chunkRaw.Data, 0);
			}
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (this.ImgInfo.Greyscale)
			{
				this.gray = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
				return;
			}
			if (this.ImgInfo.Indexed)
			{
				this.paletteIndex = (int)(c.Data[0] & byte.MaxValue);
				return;
			}
			this.red = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
			this.green = PngHelperInternal.ReadInt2fromBytes(c.Data, 2);
			this.blue = PngHelperInternal.ReadInt2fromBytes(c.Data, 4);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkBKGD pngChunkBKGD = (PngChunkBKGD)other;
			this.gray = pngChunkBKGD.gray;
			this.red = pngChunkBKGD.red;
			this.green = pngChunkBKGD.red;
			this.blue = pngChunkBKGD.red;
			this.paletteIndex = pngChunkBKGD.paletteIndex;
		}

		public void SetGray(int gray)
		{
			if (!this.ImgInfo.Greyscale)
			{
				throw new PngjException("only gray images support this");
			}
			this.gray = gray;
		}

		public int GetGray()
		{
			if (!this.ImgInfo.Greyscale)
			{
				throw new PngjException("only gray images support this");
			}
			return this.gray;
		}

		public void SetPaletteIndex(int index)
		{
			if (!this.ImgInfo.Indexed)
			{
				throw new PngjException("only indexed (pallete) images support this");
			}
			this.paletteIndex = index;
		}

		public int GetPaletteIndex()
		{
			if (!this.ImgInfo.Indexed)
			{
				throw new PngjException("only indexed (pallete) images support this");
			}
			return this.paletteIndex;
		}

		public void SetRGB(int r, int g, int b)
		{
			if (this.ImgInfo.Greyscale || this.ImgInfo.Indexed)
			{
				throw new PngjException("only rgb or rgba images support this");
			}
			this.red = r;
			this.green = g;
			this.blue = b;
		}

		public int[] GetRGB()
		{
			if (this.ImgInfo.Greyscale || this.ImgInfo.Indexed)
			{
				throw new PngjException("only rgb or rgba images support this");
			}
			return new int[] { this.red, this.green, this.blue };
		}

		public const string ID = "bKGD";

		private int gray;

		private int red;

		private int green;

		private int blue;

		private int paletteIndex;
	}
}
