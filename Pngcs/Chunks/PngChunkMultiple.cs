using System;

namespace Hjg.Pngcs.Chunks
{
	public abstract class PngChunkMultiple : PngChunk
	{
		internal PngChunkMultiple(string id, ImageInfo imgInfo)
			: base(id, imgInfo)
		{
		}

		public sealed override bool AllowsMultiple()
		{
			return true;
		}
	}
}
