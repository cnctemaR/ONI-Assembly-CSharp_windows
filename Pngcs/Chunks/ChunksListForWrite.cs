using System;
using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
	public class ChunksListForWrite : ChunksList
	{
		internal ChunksListForWrite(ImageInfo info)
			: base(info)
		{
			this.queuedChunks = new List<PngChunk>();
			this.alreadyWrittenKeys = new Dictionary<string, int>();
		}

		public List<PngChunk> GetQueuedById(string id)
		{
			return this.GetQueuedById(id, null);
		}

		public List<PngChunk> GetQueuedById(string id, string innerid)
		{
			return ChunksList.GetXById(this.queuedChunks, id, innerid);
		}

		public PngChunk GetQueuedById1(string id, string innerid, bool failIfMultiple)
		{
			List<PngChunk> queuedById = this.GetQueuedById(id, innerid);
			if (queuedById.Count == 0)
			{
				return null;
			}
			if (queuedById.Count > 1 && (failIfMultiple || !queuedById[0].AllowsMultiple()))
			{
				throw new PngjException("unexpected multiple chunks id=" + id);
			}
			return queuedById[queuedById.Count - 1];
		}

		public PngChunk GetQueuedById1(string id, bool failIfMultiple)
		{
			return this.GetQueuedById1(id, null, failIfMultiple);
		}

		public PngChunk GetQueuedById1(string id)
		{
			return this.GetQueuedById1(id, false);
		}

		public bool RemoveChunk(PngChunk c)
		{
			return this.queuedChunks.Remove(c);
		}

		public bool Queue(PngChunk chunk)
		{
			this.queuedChunks.Add(chunk);
			return true;
		}

		private static bool shouldWrite(PngChunk c, int currentGroup)
		{
			if (currentGroup == 2)
			{
				return c.Id.Equals("PLTE");
			}
			if (currentGroup % 2 == 0)
			{
				throw new PngjOutputException("bad chunk group?");
			}
			int num2;
			int num;
			if (c.mustGoBeforePLTE())
			{
				num = (num2 = 1);
			}
			else if (c.mustGoBeforeIDAT())
			{
				num = 3;
				num2 = (c.mustGoAfterPLTE() ? 3 : 1);
			}
			else
			{
				num = 5;
				num2 = 1;
			}
			int num3 = num;
			if (c.Priority)
			{
				num3 = num2;
			}
			if (ChunkHelper.IsUnknown(c) && c.ChunkGroup > 0)
			{
				num3 = c.ChunkGroup;
			}
			return currentGroup == num3 || (currentGroup > num3 && currentGroup <= num);
		}

		internal int writeChunks(Stream os, int currentGroup)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.queuedChunks.Count; i++)
			{
				PngChunk pngChunk = this.queuedChunks[i];
				if (ChunksListForWrite.shouldWrite(pngChunk, currentGroup))
				{
					if (ChunkHelper.IsCritical(pngChunk.Id) && !pngChunk.Id.Equals("PLTE"))
					{
						throw new PngjOutputException("bad chunk queued: " + pngChunk);
					}
					if (this.alreadyWrittenKeys.ContainsKey(pngChunk.Id) && !pngChunk.AllowsMultiple())
					{
						throw new PngjOutputException("duplicated chunk does not allow multiple: " + pngChunk);
					}
					pngChunk.write(os);
					this.chunks.Add(pngChunk);
					this.alreadyWrittenKeys[pngChunk.Id] = (this.alreadyWrittenKeys.ContainsKey(pngChunk.Id) ? (this.alreadyWrittenKeys[pngChunk.Id] + 1) : 1);
					list.Add(i);
					pngChunk.ChunkGroup = currentGroup;
				}
			}
			for (int j = list.Count - 1; j >= 0; j--)
			{
				this.queuedChunks.RemoveAt(list[j]);
			}
			return list.Count;
		}

		internal List<PngChunk> GetQueuedChunks()
		{
			return this.queuedChunks;
		}

		private List<PngChunk> queuedChunks;

		private Dictionary<string, int> alreadyWrittenKeys;
	}
}
