using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkGAMA : PngChunkSingle
	{
		public PngChunkGAMA(ImageInfo info)
			: base("gAMA", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = base.createEmptyChunk(4, true);
			int num = (int)(this.gamma * 100000.0 + 0.5);
			PngHelperInternal.WriteInt4tobytes(num, chunkRaw.Data, 0);
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 4)
			{
				throw new PngjException("bad chunk " + chunk);
			}
			int num = PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0);
			this.gamma = (double)num / 100000.0;
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			this.gamma = ((PngChunkGAMA)other).gamma;
		}

		public double GetGamma()
		{
			return this.gamma;
		}

		public void SetGamma(double gamma)
		{
			this.gamma = gamma;
		}

		public const string ID = "gAMA";

		private double gamma;
	}
}
