using System;

namespace Hjg.Pngcs.Chunks
{
	public abstract class PngChunkSingle : PngChunk
	{
		public PngChunkSingle(string id, ImageInfo imgInfo)
			: base(id, imgInfo)
		{
		}

		public sealed override bool AllowsMultiple()
		{
			return false;
		}

		public override int GetHashCode()
		{
			int num = 31;
			int num2 = 1;
			return num * num2 + ((this.Id == null) ? 0 : this.Id.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			return obj is PngChunkSingle && this.Id != null && this.Id.Equals(((PngChunkSingle)obj).Id);
		}
	}
}
