using System;

namespace Hjg.Pngcs.Chunks
{
	public enum ChunkLoadBehaviour
	{
		LOAD_CHUNK_NEVER,
		LOAD_CHUNK_KNOWN,
		LOAD_CHUNK_IF_SAFE,
		LOAD_CHUNK_ALWAYS
	}
}
