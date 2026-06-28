using System;

namespace Hjg.Pngcs.Chunks
{
	public interface ChunkPredicate
	{
		bool Matches(PngChunk chunk);
	}
}
