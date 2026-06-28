using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkOFFS : PngChunkSingle
	{
		public PngChunkOFFS(ImageInfo info)
			: base("oFFs", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = base.createEmptyChunk(9, true);
			PngHelperInternal.WriteInt4tobytes((int)this.posX, chunkRaw.Data, 0);
			PngHelperInternal.WriteInt4tobytes((int)this.posY, chunkRaw.Data, 4);
			chunkRaw.Data[8] = (byte)this.units;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 9)
			{
				throw new PngjException("bad chunk length " + chunk);
			}
			this.posX = (long)PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0);
			if (this.posX < 0L)
			{
				this.posX += 4294967296L;
			}
			this.posY = (long)PngHelperInternal.ReadInt4fromBytes(chunk.Data, 4);
			if (this.posY < 0L)
			{
				this.posY += 4294967296L;
			}
			this.units = PngHelperInternal.ReadInt1fromByte(chunk.Data, 8);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkOFFS pngChunkOFFS = (PngChunkOFFS)other;
			this.posX = pngChunkOFFS.posX;
			this.posY = pngChunkOFFS.posY;
			this.units = pngChunkOFFS.units;
		}

		public int GetUnits()
		{
			return this.units;
		}

		public void SetUnits(int units)
		{
			this.units = units;
		}

		public long GetPosX()
		{
			return this.posX;
		}

		public void SetPosX(long posX)
		{
			this.posX = posX;
		}

		public long GetPosY()
		{
			return this.posY;
		}

		public void SetPosY(long posY)
		{
			this.posY = posY;
		}

		public const string ID = "oFFs";

		private long posX;

		private long posY;

		private int units;
	}
}
