using System;

namespace Hjg.Pngcs.Chunks
{
	internal class ChunkPredicateId : ChunkPredicate
	{
		public ChunkPredicateId(string id)
		{
			this.id = id;
		}

		public bool Matches(PngChunk c)
		{
			return c.Id.Equals(this.id);
		}

		private readonly string id;
	}
}
