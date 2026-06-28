using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkIDAT : PngChunkMultiple
	{
		public PngChunkIDAT(ImageInfo i, int len, long offset)
			: base("IDAT", i)
		{
			base.Length = len;
			base.Offset = offset;
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk()
		{
			return null;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
		}

		public override void CloneDataFromRead(PngChunk other)
		{
		}

		public const string ID = "IDAT";
	}
}
