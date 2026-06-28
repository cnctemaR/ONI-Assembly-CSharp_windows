using System;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkZTXT : PngChunkTextVar
	{
		public PngChunkZTXT(ImageInfo info)
			: base("zTXt", info)
		{
		}

		public override ChunkRaw CreateRawChunk()
		{
			if (this.key.Length == 0)
			{
				throw new PngjException("Text chunk key must be non empty");
			}
			MemoryStream memoryStream = new MemoryStream();
			ChunkHelper.WriteBytesToStream(memoryStream, ChunkHelper.ToBytes(this.key));
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			byte[] array = ChunkHelper.compressBytes(ChunkHelper.ToBytes(this.val), true);
			ChunkHelper.WriteBytesToStream(memoryStream, array);
			byte[] array2 = memoryStream.ToArray();
			ChunkRaw chunkRaw = base.createEmptyChunk(array2.Length, false);
			chunkRaw.Data = array2;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			int num = -1;
			for (int i = 0; i < c.Data.Length; i++)
			{
				if (c.Data[i] == 0)
				{
					num = i;
					break;
				}
			}
			if (num < 0 || num > c.Data.Length - 2)
			{
				throw new PngjException("bad zTXt chunk: no separator found");
			}
			this.key = ChunkHelper.ToString(c.Data, 0, num);
			int num2 = (int)c.Data[num + 1];
			if (num2 != 0)
			{
				throw new PngjException("bad zTXt chunk: unknown compression method");
			}
			byte[] array = ChunkHelper.compressBytes(c.Data, num + 2, c.Data.Length - num - 2, false);
			this.val = ChunkHelper.ToString(array);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkZTXT pngChunkZTXT = (PngChunkZTXT)other;
			this.key = pngChunkZTXT.key;
			this.val = pngChunkZTXT.val;
		}

		public const string ID = "zTXt";
	}
}
