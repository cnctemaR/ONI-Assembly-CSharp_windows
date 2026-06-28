using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkUNKNOWN : PngChunkMultiple
	{
		public PngChunkUNKNOWN(string id, ImageInfo info)
			: base(id, info)
		{
		}

		private PngChunkUNKNOWN(PngChunkUNKNOWN c, ImageInfo info)
			: base(c.Id, info)
		{
			Array.Copy(c.data, 0, this.data, 0, c.data.Length);
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.NONE;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = base.createEmptyChunk(this.data.Length, false);
			chunkRaw.Data = this.data;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			this.data = c.Data;
		}

		public byte[] GetData()
		{
			return this.data;
		}

		public void SetData(byte[] data_0)
		{
			this.data = data_0;
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkUNKNOWN pngChunkUNKNOWN = (PngChunkUNKNOWN)other;
			this.data = pngChunkUNKNOWN.data;
		}

		private byte[] data;
	}
}
