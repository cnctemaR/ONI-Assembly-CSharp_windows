using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkPLTE : PngChunkSingle
	{
		public PngChunkPLTE(ImageInfo info)
			: base("PLTE", info)
		{
			this.nentries = 0;
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk()
		{
			int num = 3 * this.nentries;
			int[] array = new int[3];
			ChunkRaw chunkRaw = base.createEmptyChunk(num, true);
			int i = 0;
			int num2 = 0;
			while (i < this.nentries)
			{
				this.GetEntryRgb(i, array);
				chunkRaw.Data[num2++] = (byte)array[0];
				chunkRaw.Data[num2++] = (byte)array[1];
				chunkRaw.Data[num2++] = (byte)array[2];
				i++;
			}
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			this.SetNentries(chunk.Length / 3);
			int i = 0;
			int num = 0;
			while (i < this.nentries)
			{
				this.SetEntry(i, (int)(chunk.Data[num++] & byte.MaxValue), (int)(chunk.Data[num++] & byte.MaxValue), (int)(chunk.Data[num++] & byte.MaxValue));
				i++;
			}
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkPLTE pngChunkPLTE = (PngChunkPLTE)other;
			this.SetNentries(pngChunkPLTE.GetNentries());
			Array.Copy(pngChunkPLTE.entries, 0, this.entries, 0, this.nentries);
		}

		public void SetNentries(int nentries)
		{
			this.nentries = nentries;
			if (nentries < 1 || nentries > 256)
			{
				throw new PngjException("invalid pallette - nentries=" + nentries);
			}
			if (this.entries == null || this.entries.Length != nentries)
			{
				this.entries = new int[nentries];
			}
		}

		public int GetNentries()
		{
			return this.nentries;
		}

		public void SetEntry(int n, int r, int g, int b)
		{
			this.entries[n] = (r << 16) | (g << 8) | b;
		}

		public int GetEntry(int n)
		{
			return this.entries[n];
		}

		public void GetEntryRgb(int index, int[] rgb, int offset)
		{
			int num = this.entries[index];
			rgb[offset] = (num & 16711680) >> 16;
			rgb[offset + 1] = (num & 65280) >> 8;
			rgb[offset + 2] = num & 255;
		}

		public void GetEntryRgb(int n, int[] rgb)
		{
			this.GetEntryRgb(n, rgb, 0);
		}

		public int MinBitDepth()
		{
			if (this.nentries <= 2)
			{
				return 1;
			}
			if (this.nentries <= 4)
			{
				return 2;
			}
			if (this.nentries <= 16)
			{
				return 4;
			}
			return 8;
		}

		public const string ID = "PLTE";

		private int nentries;

		private int[] entries;
	}
}
