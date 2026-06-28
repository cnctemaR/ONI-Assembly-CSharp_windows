using System;
using System.Collections.Generic;

namespace Hjg.Pngcs.Chunks
{
	public class PngMetadata
	{
		internal PngMetadata(ChunksList chunks)
		{
			this.chunkList = chunks;
			if (chunks is ChunksListForWrite)
			{
				this.ReadOnly = false;
				return;
			}
			this.ReadOnly = true;
		}

		public void QueueChunk(PngChunk chunk, bool lazyOverwrite)
		{
			ChunksListForWrite chunkListW = this.getChunkListW();
			if (this.ReadOnly)
			{
				throw new PngjException("cannot set chunk : readonly metadata");
			}
			if (lazyOverwrite)
			{
				ChunkHelper.TrimList(chunkListW.GetQueuedChunks(), new ChunkPredicateEquiv(chunk));
			}
			chunkListW.Queue(chunk);
		}

		public void QueueChunk(PngChunk chunk)
		{
			this.QueueChunk(chunk, true);
		}

		private ChunksListForWrite getChunkListW()
		{
			return (ChunksListForWrite)this.chunkList;
		}

		public double[] GetDpi()
		{
			PngChunk byId = this.chunkList.GetById1("pHYs", true);
			if (byId == null)
			{
				return new double[] { -1.0, -1.0 };
			}
			return ((PngChunkPHYS)byId).GetAsDpi2();
		}

		public void SetDpi(double dpix, double dpiy)
		{
			PngChunkPHYS pngChunkPHYS = new PngChunkPHYS(this.chunkList.imageInfo);
			pngChunkPHYS.SetAsDpi2(dpix, dpiy);
			this.QueueChunk(pngChunkPHYS);
		}

		public void SetDpi(double dpi)
		{
			this.SetDpi(dpi, dpi);
		}

		public PngChunkTIME SetTimeNow(int nsecs)
		{
			PngChunkTIME pngChunkTIME = new PngChunkTIME(this.chunkList.imageInfo);
			pngChunkTIME.SetNow(nsecs);
			this.QueueChunk(pngChunkTIME);
			return pngChunkTIME;
		}

		public PngChunkTIME SetTimeNow()
		{
			return this.SetTimeNow(0);
		}

		public PngChunkTIME SetTimeYMDHMS(int year, int mon, int day, int hour, int min, int sec)
		{
			PngChunkTIME pngChunkTIME = new PngChunkTIME(this.chunkList.imageInfo);
			pngChunkTIME.SetYMDHMS(year, mon, day, hour, min, sec);
			this.QueueChunk(pngChunkTIME, true);
			return pngChunkTIME;
		}

		public PngChunkTIME GetTime()
		{
			return (PngChunkTIME)this.chunkList.GetById1("tIME");
		}

		public string GetTimeAsString()
		{
			PngChunkTIME time = this.GetTime();
			if (time != null)
			{
				return time.GetAsString();
			}
			return "";
		}

		public PngChunkTextVar SetText(string key, string val, bool useLatin1, bool compress)
		{
			if (compress && !useLatin1)
			{
				throw new PngjException("cannot compress non latin text");
			}
			PngChunkTextVar pngChunkTextVar;
			if (useLatin1)
			{
				if (compress)
				{
					pngChunkTextVar = new PngChunkZTXT(this.chunkList.imageInfo);
				}
				else
				{
					pngChunkTextVar = new PngChunkTEXT(this.chunkList.imageInfo);
				}
			}
			else
			{
				pngChunkTextVar = new PngChunkITXT(this.chunkList.imageInfo);
				((PngChunkITXT)pngChunkTextVar).SetLangtag(key);
			}
			pngChunkTextVar.SetKeyVal(key, val);
			this.QueueChunk(pngChunkTextVar, true);
			return pngChunkTextVar;
		}

		public PngChunkTextVar SetText(string key, string val)
		{
			return this.SetText(key, val, false, false);
		}

		public List<PngChunkTextVar> GetTxtsForKey(string key)
		{
			List<PngChunkTextVar> list = new List<PngChunkTextVar>();
			foreach (PngChunk pngChunk in this.chunkList.GetById("tEXt", key))
			{
				list.Add((PngChunkTextVar)pngChunk);
			}
			foreach (PngChunk pngChunk2 in this.chunkList.GetById("zTXt", key))
			{
				list.Add((PngChunkTextVar)pngChunk2);
			}
			foreach (PngChunk pngChunk3 in this.chunkList.GetById("iTXt", key))
			{
				list.Add((PngChunkTextVar)pngChunk3);
			}
			return list;
		}

		public string GetTxtForKey(string key)
		{
			string text = "";
			List<PngChunkTextVar> txtsForKey = this.GetTxtsForKey(key);
			if (txtsForKey.Count == 0)
			{
				return text;
			}
			foreach (PngChunkTextVar pngChunkTextVar in txtsForKey)
			{
				text = text + pngChunkTextVar.GetVal() + "\n";
			}
			return text.Trim();
		}

		public PngChunkPLTE GetPLTE()
		{
			return (PngChunkPLTE)this.chunkList.GetById1("PLTE");
		}

		public PngChunkPLTE CreatePLTEChunk()
		{
			PngChunkPLTE pngChunkPLTE = new PngChunkPLTE(this.chunkList.imageInfo);
			this.QueueChunk(pngChunkPLTE);
			return pngChunkPLTE;
		}

		public PngChunkTRNS GetTRNS()
		{
			return (PngChunkTRNS)this.chunkList.GetById1("tRNS");
		}

		public PngChunkTRNS CreateTRNSChunk()
		{
			PngChunkTRNS pngChunkTRNS = new PngChunkTRNS(this.chunkList.imageInfo);
			this.QueueChunk(pngChunkTRNS);
			return pngChunkTRNS;
		}

		private readonly ChunksList chunkList;

		private readonly bool ReadOnly;
	}
}
