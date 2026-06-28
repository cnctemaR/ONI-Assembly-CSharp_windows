using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkHIST : PngChunkSingle
	{
		public PngChunkHIST(ImageInfo info)
			: base(PngChunkHIST.ID, info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			if (!this.ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images accept a HIST chunk");
			}
			ChunkRaw chunkRaw = base.createEmptyChunk(this.hist.Length * 2, true);
			for (int i = 0; i < this.hist.Length; i++)
			{
				PngHelperInternal.WriteInt2tobytes(this.hist[i], chunkRaw.Data, i * 2);
			}
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (!this.ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images accept a HIST chunk");
			}
			int num = c.Data.Length / 2;
			this.hist = new int[num];
			for (int i = 0; i < this.hist.Length; i++)
			{
				this.hist[i] = PngHelperInternal.ReadInt2fromBytes(c.Data, i * 2);
			}
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkHIST pngChunkHIST = (PngChunkHIST)other;
			this.hist = new int[pngChunkHIST.hist.Length];
			Array.Copy(pngChunkHIST.hist, 0, this.hist, 0, pngChunkHIST.hist.Length);
		}

		public int[] GetHist()
		{
			return this.hist;
		}

		public void SetHist(int[] hist)
		{
			this.hist = hist;
		}

		public static readonly string ID = "hIST";

		private int[] hist = new int[0];
	}
}
