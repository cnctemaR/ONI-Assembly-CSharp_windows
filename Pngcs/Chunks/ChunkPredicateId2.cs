using System;

namespace Hjg.Pngcs.Chunks
{
	internal class ChunkPredicateId2 : ChunkPredicate
	{
		public ChunkPredicateId2(string id, string inner)
		{
			this.id = id;
			this.innerid = inner;
		}

		public bool Matches(PngChunk c)
		{
			return c.Id.Equals(this.id) && (!(c is PngChunkTextVar) || ((PngChunkTextVar)c).GetKey().Equals(this.innerid)) && (!(c is PngChunkSPLT) || ((PngChunkSPLT)c).PalName.Equals(this.innerid));
		}

		private readonly string id;

		private readonly string innerid;
	}
}
