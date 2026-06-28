using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkPHYS : PngChunkSingle
	{
		public long PixelsxUnitX { get; set; }

		public long PixelsxUnitY { get; set; }

		public int Units { get; set; }

		public PngChunkPHYS(ImageInfo info)
			: base("pHYs", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = base.createEmptyChunk(9, true);
			PngHelperInternal.WriteInt4tobytes((int)this.PixelsxUnitX, chunkRaw.Data, 0);
			PngHelperInternal.WriteInt4tobytes((int)this.PixelsxUnitY, chunkRaw.Data, 4);
			chunkRaw.Data[8] = (byte)this.Units;
			return chunkRaw;
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkPHYS pngChunkPHYS = (PngChunkPHYS)other;
			this.PixelsxUnitX = pngChunkPHYS.PixelsxUnitX;
			this.PixelsxUnitY = pngChunkPHYS.PixelsxUnitY;
			this.Units = pngChunkPHYS.Units;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 9)
			{
				throw new PngjException("bad chunk length " + chunk);
			}
			this.PixelsxUnitX = (long)PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0);
			if (this.PixelsxUnitX < 0L)
			{
				this.PixelsxUnitX += 4294967296L;
			}
			this.PixelsxUnitY = (long)PngHelperInternal.ReadInt4fromBytes(chunk.Data, 4);
			if (this.PixelsxUnitY < 0L)
			{
				this.PixelsxUnitY += 4294967296L;
			}
			this.Units = PngHelperInternal.ReadInt1fromByte(chunk.Data, 8);
		}

		public double GetAsDpi()
		{
			if (this.Units != 1 || this.PixelsxUnitX != this.PixelsxUnitY)
			{
				return -1.0;
			}
			return (double)this.PixelsxUnitX * 0.0254;
		}

		public double[] GetAsDpi2()
		{
			if (this.Units != 1)
			{
				return new double[] { -1.0, -1.0 };
			}
			return new double[]
			{
				(double)this.PixelsxUnitX * 0.0254,
				(double)this.PixelsxUnitY * 0.0254
			};
		}

		public void SetAsDpi(double dpi)
		{
			this.Units = 1;
			this.PixelsxUnitX = (long)(dpi / 0.0254 + 0.5);
			this.PixelsxUnitY = this.PixelsxUnitX;
		}

		public void SetAsDpi2(double dpix, double dpiy)
		{
			this.Units = 1;
			this.PixelsxUnitX = (long)(dpix / 0.0254 + 0.5);
			this.PixelsxUnitY = (long)(dpiy / 0.0254 + 0.5);
		}

		public const string ID = "pHYs";
	}
}
