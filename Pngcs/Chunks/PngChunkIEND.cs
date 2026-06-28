using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkIEND : PngChunkSingle
	{
		public PngChunkIEND(ImageInfo info)
			: base("IEND", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk()
		{
			return new ChunkRaw(0, ChunkHelper.b_IEND, false);
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
		}

		public override void CloneDataFromRead(PngChunk other)
		{
		}

		public const string ID = "IEND";
	}
}
