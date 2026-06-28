using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkCHRM : PngChunkSingle
	{
		public PngChunkCHRM(ImageInfo info)
			: base("cHRM", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = base.createEmptyChunk(32, true);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.whitex), chunkRaw.Data, 0);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.whitey), chunkRaw.Data, 4);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.redx), chunkRaw.Data, 8);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.redy), chunkRaw.Data, 12);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.greenx), chunkRaw.Data, 16);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.greeny), chunkRaw.Data, 20);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.bluex), chunkRaw.Data, 24);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.bluey), chunkRaw.Data, 28);
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (c.Length != 32)
			{
				throw new PngjException("bad chunk " + c);
			}
			this.whitex = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 0));
			this.whitey = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 4));
			this.redx = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 8));
			this.redy = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 12));
			this.greenx = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 16));
			this.greeny = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 20));
			this.bluex = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 24));
			this.bluey = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 28));
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkCHRM pngChunkCHRM = (PngChunkCHRM)other;
			this.whitex = pngChunkCHRM.whitex;
			this.whitey = pngChunkCHRM.whitex;
			this.redx = pngChunkCHRM.redx;
			this.redy = pngChunkCHRM.redy;
			this.greenx = pngChunkCHRM.greenx;
			this.greeny = pngChunkCHRM.greeny;
			this.bluex = pngChunkCHRM.bluex;
			this.bluey = pngChunkCHRM.bluey;
		}

		public void SetChromaticities(double whitex, double whitey, double redx, double redy, double greenx, double greeny, double bluex, double bluey)
		{
			this.whitex = whitex;
			this.redx = redx;
			this.greenx = greenx;
			this.bluex = bluex;
			this.whitey = whitey;
			this.redy = redy;
			this.greeny = greeny;
			this.bluey = bluey;
		}

		public double[] GetChromaticities()
		{
			return new double[] { this.whitex, this.whitey, this.redx, this.redy, this.greenx, this.greeny, this.bluex, this.bluey };
		}

		public const string ID = "cHRM";

		private double whitex;

		private double whitey;

		private double redx;

		private double redy;

		private double greenx;

		private double greeny;

		private double bluex;

		private double bluey;
	}
}
