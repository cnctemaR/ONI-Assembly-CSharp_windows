using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkTEXT : PngChunkTextVar
	{
		public PngChunkTEXT(ImageInfo info)
			: base("tEXt", info)
		{
		}

		public override ChunkRaw CreateRawChunk()
		{
			if (this.key.Length == 0)
			{
				throw new PngjException("Text chunk key must be non empty");
			}
			byte[] bytes = PngHelperInternal.charsetLatin1.GetBytes(this.key);
			byte[] bytes2 = PngHelperInternal.charsetLatin1.GetBytes(this.val);
			ChunkRaw chunkRaw = base.createEmptyChunk(bytes.Length + bytes2.Length + 1, true);
			Array.Copy(bytes, 0, chunkRaw.Data, 0, bytes.Length);
			chunkRaw.Data[bytes.Length] = 0;
			Array.Copy(bytes2, 0, chunkRaw.Data, bytes.Length + 1, bytes2.Length);
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			int num = 0;
			while (num < c.Data.Length && c.Data[num] != 0)
			{
				num++;
			}
			this.key = PngHelperInternal.charsetLatin1.GetString(c.Data, 0, num);
			num++;
			this.val = ((num < c.Data.Length) ? PngHelperInternal.charsetLatin1.GetString(c.Data, num, c.Data.Length - num) : "");
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkTEXT pngChunkTEXT = (PngChunkTEXT)other;
			this.key = pngChunkTEXT.key;
			this.val = pngChunkTEXT.val;
		}

		public const string ID = "tEXt";
	}
}
