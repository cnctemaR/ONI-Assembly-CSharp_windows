using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkTRNS : PngChunkSingle
	{
		public PngChunkTRNS(ImageInfo info)
			: base("tRNS", info)
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
				chunkRaw = base.createEmptyChunk(this.paletteAlpha.Length, true);
				for (int i = 0; i < chunkRaw.Length; i++)
				{
					chunkRaw.Data[i] = (byte)this.paletteAlpha[i];
				}
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
				int num = c.Data.Length;
				this.paletteAlpha = new int[num];
				for (int i = 0; i < num; i++)
				{
					this.paletteAlpha[i] = (int)(c.Data[i] & byte.MaxValue);
				}
				return;
			}
			this.red = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
			this.green = PngHelperInternal.ReadInt2fromBytes(c.Data, 2);
			this.blue = PngHelperInternal.ReadInt2fromBytes(c.Data, 4);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkTRNS pngChunkTRNS = (PngChunkTRNS)other;
			this.gray = pngChunkTRNS.gray;
			this.red = pngChunkTRNS.red;
			this.green = pngChunkTRNS.red;
			this.blue = pngChunkTRNS.red;
			if (pngChunkTRNS.paletteAlpha != null)
			{
				this.paletteAlpha = new int[pngChunkTRNS.paletteAlpha.Length];
				Array.Copy(pngChunkTRNS.paletteAlpha, 0, this.paletteAlpha, 0, this.paletteAlpha.Length);
			}
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

		public void SetGray(int g)
		{
			if (!this.ImgInfo.Greyscale)
			{
				throw new PngjException("only grayscale images support this");
			}
			this.gray = g;
		}

		public int GetGray()
		{
			if (!this.ImgInfo.Greyscale)
			{
				throw new PngjException("only grayscale images support this");
			}
			return this.gray;
		}

		public void SetPalletteAlpha(int[] palAlpha)
		{
			if (!this.ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images support this");
			}
			this.paletteAlpha = palAlpha;
		}

		public void setIndexEntryAsTransparent(int palAlphaIndex)
		{
			if (!this.ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images support this");
			}
			this.paletteAlpha = new int[] { palAlphaIndex + 1 };
			for (int i = 0; i < palAlphaIndex; i++)
			{
				this.paletteAlpha[i] = 255;
			}
			this.paletteAlpha[palAlphaIndex] = 0;
		}

		public int[] GetPalletteAlpha()
		{
			if (!this.ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images support this");
			}
			return this.paletteAlpha;
		}

		public const string ID = "tRNS";

		private int gray;

		private int red;

		private int green;

		private int blue;

		private int[] paletteAlpha;
	}
}
