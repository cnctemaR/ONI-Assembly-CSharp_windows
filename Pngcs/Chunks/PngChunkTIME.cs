using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkTIME : PngChunkSingle
	{
		public PngChunkTIME(ImageInfo info)
			: base("tIME", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.NONE;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = base.createEmptyChunk(7, true);
			PngHelperInternal.WriteInt2tobytes(this.year, chunkRaw.Data, 0);
			chunkRaw.Data[2] = (byte)this.mon;
			chunkRaw.Data[3] = (byte)this.day;
			chunkRaw.Data[4] = (byte)this.hour;
			chunkRaw.Data[5] = (byte)this.min;
			chunkRaw.Data[6] = (byte)this.sec;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 7)
			{
				throw new PngjException("bad chunk " + chunk);
			}
			this.year = PngHelperInternal.ReadInt2fromBytes(chunk.Data, 0);
			this.mon = PngHelperInternal.ReadInt1fromByte(chunk.Data, 2);
			this.day = PngHelperInternal.ReadInt1fromByte(chunk.Data, 3);
			this.hour = PngHelperInternal.ReadInt1fromByte(chunk.Data, 4);
			this.min = PngHelperInternal.ReadInt1fromByte(chunk.Data, 5);
			this.sec = PngHelperInternal.ReadInt1fromByte(chunk.Data, 6);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkTIME pngChunkTIME = (PngChunkTIME)other;
			this.year = pngChunkTIME.year;
			this.mon = pngChunkTIME.mon;
			this.day = pngChunkTIME.day;
			this.hour = pngChunkTIME.hour;
			this.min = pngChunkTIME.min;
			this.sec = pngChunkTIME.sec;
		}

		public void SetNow(int secsAgo)
		{
			DateTime now = DateTime.Now;
			this.year = now.Year;
			this.mon = now.Month;
			this.day = now.Day;
			this.hour = now.Hour;
			this.min = now.Minute;
			this.sec = now.Second;
		}

		internal void SetYMDHMS(int yearx, int monx, int dayx, int hourx, int minx, int secx)
		{
			this.year = yearx;
			this.mon = monx;
			this.day = dayx;
			this.hour = hourx;
			this.min = minx;
			this.sec = secx;
		}

		public int[] GetYMDHMS()
		{
			return new int[] { this.year, this.mon, this.day, this.hour, this.min, this.sec };
		}

		public string GetAsString()
		{
			return string.Format("%04d/%02d/%02d %02d:%02d:%02d", new object[] { this.year, this.mon, this.day, this.hour, this.min, this.sec });
		}

		public const string ID = "tIME";

		private int year;

		private int mon;

		private int day;

		private int hour;

		private int min;

		private int sec;
	}
}
