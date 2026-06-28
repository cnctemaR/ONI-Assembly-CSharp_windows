using System;

namespace Hjg.Pngcs.Chunks
{
	internal class ChunkPredicateEquiv : ChunkPredicate
	{
		public ChunkPredicateEquiv(PngChunk chunk)
		{
			this.chunk = chunk;
		}

		public bool Matches(PngChunk c)
		{
			return ChunkHelper.Equivalent(c, this.chunk);
		}

		private readonly PngChunk chunk;
	}
}
