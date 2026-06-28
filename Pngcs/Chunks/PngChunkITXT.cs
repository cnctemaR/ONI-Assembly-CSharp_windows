using System;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkITXT : PngChunkTextVar
	{
		public PngChunkITXT(ImageInfo info)
			: base("iTXt", info)
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
			memoryStream.WriteByte(this.compressed ? 1 : 0);
			memoryStream.WriteByte(0);
			ChunkHelper.WriteBytesToStream(memoryStream, ChunkHelper.ToBytes(this.langTag));
			memoryStream.WriteByte(0);
			ChunkHelper.WriteBytesToStream(memoryStream, ChunkHelper.ToBytesUTF8(this.translatedTag));
			memoryStream.WriteByte(0);
			byte[] array = ChunkHelper.ToBytesUTF8(this.val);
			if (this.compressed)
			{
				array = ChunkHelper.compressBytes(array, true);
			}
			ChunkHelper.WriteBytesToStream(memoryStream, array);
			byte[] array2 = memoryStream.ToArray();
			ChunkRaw chunkRaw = base.createEmptyChunk(array2.Length, false);
			chunkRaw.Data = array2;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			int num = 0;
			int[] array = new int[3];
			for (int i = 0; i < c.Data.Length; i++)
			{
				if (c.Data[i] == 0)
				{
					array[num] = i;
					num++;
					if (num == 1)
					{
						i += 2;
					}
					if (num == 3)
					{
						break;
					}
				}
			}
			if (num != 3)
			{
				throw new PngjException("Bad formed PngChunkITXT chunk");
			}
			this.key = ChunkHelper.ToString(c.Data, 0, array[0]);
			int num2 = array[0] + 1;
			this.compressed = c.Data[num2] != 0;
			num2++;
			if (this.compressed && c.Data[num2] != 0)
			{
				throw new PngjException("Bad formed PngChunkITXT chunk - bad compression method ");
			}
			this.langTag = ChunkHelper.ToString(c.Data, num2, array[1] - num2);
			this.translatedTag = ChunkHelper.ToStringUTF8(c.Data, array[1] + 1, array[2] - array[1] - 1);
			num2 = array[2] + 1;
			if (this.compressed)
			{
				byte[] array2 = ChunkHelper.compressBytes(c.Data, num2, c.Data.Length - num2, false);
				this.val = ChunkHelper.ToStringUTF8(array2);
				return;
			}
			this.val = ChunkHelper.ToStringUTF8(c.Data, num2, c.Data.Length - num2);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkITXT pngChunkITXT = (PngChunkITXT)other;
			this.key = pngChunkITXT.key;
			this.val = pngChunkITXT.val;
			this.compressed = pngChunkITXT.compressed;
			this.langTag = pngChunkITXT.langTag;
			this.translatedTag = pngChunkITXT.translatedTag;
		}

		public bool IsCompressed()
		{
			return this.compressed;
		}

		public void SetCompressed(bool compressed)
		{
			this.compressed = compressed;
		}

		public string GetLangtag()
		{
			return this.langTag;
		}

		public void SetLangtag(string langtag)
		{
			this.langTag = langtag;
		}

		public string GetTranslatedTag()
		{
			return this.translatedTag;
		}

		public void SetTranslatedTag(string translatedTag)
		{
			this.translatedTag = translatedTag;
		}

		public const string ID = "iTXt";

		private bool compressed;

		private string langTag = "";

		private string translatedTag = "";
	}
}
