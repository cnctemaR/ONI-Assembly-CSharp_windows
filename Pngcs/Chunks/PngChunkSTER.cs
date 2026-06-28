using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkSTER : PngChunkSingle
	{
		public byte Mode { get; set; }

		public PngChunkSTER(ImageInfo info)
			: base("sTER", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = base.createEmptyChunk(1, true);
			chunkRaw.Data[0] = this.Mode;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 1)
			{
				throw new PngjException("bad chunk length " + chunk);
			}
			this.Mode = chunk.Data[0];
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkSTER pngChunkSTER = (PngChunkSTER)other;
			this.Mode = pngChunkSTER.Mode;
		}

		public const string ID = "sTER";
	}
}
