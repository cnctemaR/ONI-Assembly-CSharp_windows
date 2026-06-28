using System;
using System.IO;
using Hjg.Pngcs.Chunks;

namespace Hjg.Pngcs
{
	internal class PngIDatChunkOutputStream : ProgressiveOutputStream
	{
		public PngIDatChunkOutputStream(Stream outputStream_0)
			: this(outputStream_0, 32768)
		{
		}

		public PngIDatChunkOutputStream(Stream outputStream_0, int size)
			: base((size > 8) ? size : 32768)
		{
			this.outputStream = outputStream_0;
		}

		protected override void FlushBuffer(byte[] b, int len)
		{
			new ChunkRaw(len, ChunkHelper.b_IDAT, false)
			{
				Data = b
			}.WriteChunk(this.outputStream);
		}

		public override void Close()
		{
			this.Flush();
		}

		private const int SIZE_DEFAULT = 32768;

		private readonly Stream outputStream;
	}
}
