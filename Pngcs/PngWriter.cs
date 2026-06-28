using System;
using System.Collections.Generic;
using System.IO;
using Hjg.Pngcs.Chunks;
using Hjg.Pngcs.Zlib;

namespace Hjg.Pngcs
{
	public class PngWriter
	{
		public EDeflateCompressStrategy CompressionStrategy { get; set; }

		public int CompLevel { get; set; }

		public bool ShouldCloseStream { get; set; }

		public int IdatMaxSize { get; set; }

		public int CurrentChunkGroup { get; private set; }

		public PngWriter(Stream outputStream, ImageInfo imgInfo)
			: this(outputStream, imgInfo, "[NO FILENAME AVAILABLE]")
		{
		}

		public PngWriter(Stream outputStream, ImageInfo imgInfo, string filename)
		{
			this.filename = ((filename == null) ? "" : filename);
			this.outputStream = outputStream;
			this.ImgInfo = imgInfo;
			this.CompLevel = 6;
			this.ShouldCloseStream = true;
			this.IdatMaxSize = 0;
			this.CompressionStrategy = EDeflateCompressStrategy.Filtered;
			this.rowb = new byte[imgInfo.BytesPerRow + 1];
			this.rowbprev = new byte[this.rowb.Length];
			this.rowbfilter = new byte[this.rowb.Length];
			this.chunksList = new ChunksListForWrite(this.ImgInfo);
			this.metadata = new PngMetadata(this.chunksList);
			this.filterStrat = new FilterWriteStrategy(this.ImgInfo, FilterType.FILTER_DEFAULT);
			this.unpackedMode = false;
			this.needsPack = this.unpackedMode && imgInfo.Packed;
		}

		private void init()
		{
			this.datStream = new PngIDatChunkOutputStream(this.outputStream, this.IdatMaxSize);
			this.datStreamDeflated = ZlibStreamFactory.createZlibOutputStream(this.datStream, this.CompLevel, this.CompressionStrategy, true);
			this.WriteSignatureAndIHDR();
			this.WriteFirstChunks();
		}

		private void reportResultsForFilter(int rown, FilterType type, bool tentative)
		{
			for (int i = 0; i < this.histox.Length; i++)
			{
				this.histox[i] = 0;
			}
			int num = 0;
			for (int j = 1; j <= this.ImgInfo.BytesPerRow; j++)
			{
				int num2 = (int)this.rowbfilter[j];
				if (num2 < 0)
				{
					num -= num2;
				}
				else
				{
					num += num2;
				}
				this.histox[num2 & 255]++;
			}
			this.filterStrat.fillResultsForFilter(rown, type, (double)num, this.histox, tentative);
		}

		private void WriteEndChunk()
		{
			PngChunkIEND pngChunkIEND = new PngChunkIEND(this.ImgInfo);
			pngChunkIEND.CreateRawChunk().WriteChunk(this.outputStream);
		}

		private void WriteFirstChunks()
		{
			this.CurrentChunkGroup = 1;
			int num = this.chunksList.writeChunks(this.outputStream, this.CurrentChunkGroup);
			this.CurrentChunkGroup = 2;
			num = this.chunksList.writeChunks(this.outputStream, this.CurrentChunkGroup);
			if (num > 0 && this.ImgInfo.Greyscale)
			{
				throw new PngjOutputException("cannot write palette for this format");
			}
			if (num == 0 && this.ImgInfo.Indexed)
			{
				throw new PngjOutputException("missing palette");
			}
			this.CurrentChunkGroup = 3;
			num = this.chunksList.writeChunks(this.outputStream, this.CurrentChunkGroup);
			this.CurrentChunkGroup = 4;
		}

		private void WriteLastChunks()
		{
			this.CurrentChunkGroup = 5;
			this.chunksList.writeChunks(this.outputStream, this.CurrentChunkGroup);
			List<PngChunk> queuedChunks = this.chunksList.GetQueuedChunks();
			if (queuedChunks.Count > 0)
			{
				throw new PngjOutputException(queuedChunks.Count + " chunks were not written! Eg: " + queuedChunks[0].ToString());
			}
			this.CurrentChunkGroup = 6;
		}

		private void WriteSignatureAndIHDR()
		{
			this.CurrentChunkGroup = 0;
			PngHelperInternal.WriteBytes(this.outputStream, PngHelperInternal.PNG_ID_SIGNATURE);
			PngChunkIHDR pngChunkIHDR = new PngChunkIHDR(this.ImgInfo);
			pngChunkIHDR.Cols = this.ImgInfo.Cols;
			pngChunkIHDR.Rows = this.ImgInfo.Rows;
			pngChunkIHDR.Bitspc = this.ImgInfo.BitDepth;
			int num = 0;
			if (this.ImgInfo.Alpha)
			{
				num += 4;
			}
			if (this.ImgInfo.Indexed)
			{
				num++;
			}
			if (!this.ImgInfo.Greyscale)
			{
				num += 2;
			}
			pngChunkIHDR.Colormodel = num;
			pngChunkIHDR.Compmeth = 0;
			pngChunkIHDR.Filmeth = 0;
			pngChunkIHDR.Interlaced = 0;
			pngChunkIHDR.CreateRawChunk().WriteChunk(this.outputStream);
		}

		protected void encodeRowFromByte(byte[] row)
		{
			if (row.Length == this.ImgInfo.SamplesPerRowPacked && !this.needsPack)
			{
				int num = 1;
				if (this.ImgInfo.BitDepth <= 8)
				{
					foreach (byte b in row)
					{
						this.rowb[num++] = b;
					}
					return;
				}
				foreach (byte b2 in row)
				{
					this.rowb[num] = b2;
					num += 2;
				}
				return;
			}
			else
			{
				if (row.Length >= this.ImgInfo.SamplesPerRow && this.needsPack)
				{
					ImageLine.packInplaceByte(this.ImgInfo, row, row, false);
				}
				if (this.ImgInfo.BitDepth <= 8)
				{
					int k = 0;
					int num2 = 1;
					while (k < this.ImgInfo.SamplesPerRowPacked)
					{
						this.rowb[num2++] = row[k];
						k++;
					}
					return;
				}
				int l = 0;
				int num3 = 1;
				while (l < this.ImgInfo.SamplesPerRowPacked)
				{
					this.rowb[num3++] = row[l];
					this.rowb[num3++] = 0;
					l++;
				}
				return;
			}
		}

		protected void encodeRowFromInt(int[] row)
		{
			if (row.Length == this.ImgInfo.SamplesPerRowPacked && !this.needsPack)
			{
				int num = 1;
				if (this.ImgInfo.BitDepth <= 8)
				{
					foreach (int num2 in row)
					{
						this.rowb[num++] = (byte)num2;
					}
					return;
				}
				foreach (int num3 in row)
				{
					this.rowb[num++] = (byte)(num3 >> 8);
					this.rowb[num++] = (byte)num3;
				}
				return;
			}
			else
			{
				if (row.Length >= this.ImgInfo.SamplesPerRow && this.needsPack)
				{
					ImageLine.packInplaceInt(this.ImgInfo, row, row, false);
				}
				if (this.ImgInfo.BitDepth <= 8)
				{
					int k = 0;
					int num4 = 1;
					while (k < this.ImgInfo.SamplesPerRowPacked)
					{
						this.rowb[num4++] = (byte)row[k];
						k++;
					}
					return;
				}
				int l = 0;
				int num5 = 1;
				while (l < this.ImgInfo.SamplesPerRowPacked)
				{
					this.rowb[num5++] = (byte)(row[l] >> 8);
					this.rowb[num5++] = (byte)row[l];
					l++;
				}
				return;
			}
		}

		private void FilterRow(int rown)
		{
			if (this.filterStrat.shouldTestAll(rown))
			{
				this.FilterRowNone();
				this.reportResultsForFilter(rown, FilterType.FILTER_NONE, true);
				this.FilterRowSub();
				this.reportResultsForFilter(rown, FilterType.FILTER_SUB, true);
				this.FilterRowUp();
				this.reportResultsForFilter(rown, FilterType.FILTER_UP, true);
				this.FilterRowAverage();
				this.reportResultsForFilter(rown, FilterType.FILTER_AVERAGE, true);
				this.FilterRowPaeth();
				this.reportResultsForFilter(rown, FilterType.FILTER_PAETH, true);
			}
			FilterType filterType = this.filterStrat.gimmeFilterType(rown, true);
			this.rowbfilter[0] = (byte)filterType;
			switch (filterType)
			{
			case FilterType.FILTER_NONE:
				this.FilterRowNone();
				break;
			case FilterType.FILTER_SUB:
				this.FilterRowSub();
				break;
			case FilterType.FILTER_UP:
				this.FilterRowUp();
				break;
			case FilterType.FILTER_AVERAGE:
				this.FilterRowAverage();
				break;
			case FilterType.FILTER_PAETH:
				this.FilterRowPaeth();
				break;
			default:
				throw new PngjOutputException("Filter type " + filterType + " not implemented");
			}
			this.reportResultsForFilter(rown, filterType, false);
		}

		private void prepareEncodeRow(int rown)
		{
			if (this.datStream == null)
			{
				this.init();
			}
			this.rowNum++;
			if (rown >= 0 && this.rowNum != rown)
			{
				throw new PngjOutputException(string.Concat(new object[] { "rows must be written in order: expected:", this.rowNum, " passed:", rown }));
			}
			byte[] array = this.rowb;
			this.rowb = this.rowbprev;
			this.rowbprev = array;
		}

		private void filterAndSend(int rown)
		{
			this.FilterRow(rown);
			this.datStreamDeflated.Write(this.rowbfilter, 0, this.ImgInfo.BytesPerRow + 1);
		}

		private void FilterRowAverage()
		{
			int bytesPerRow = this.ImgInfo.BytesPerRow;
			int num = 1 - this.ImgInfo.BytesPixel;
			int i = 1;
			while (i <= bytesPerRow)
			{
				this.rowbfilter[i] = this.rowb[i] - (this.rowbprev[i] + ((num > 0) ? this.rowb[num] : 0)) / 2;
				i++;
				num++;
			}
		}

		private void FilterRowNone()
		{
			for (int i = 1; i <= this.ImgInfo.BytesPerRow; i++)
			{
				this.rowbfilter[i] = this.rowb[i];
			}
		}

		private void FilterRowPaeth()
		{
			int bytesPerRow = this.ImgInfo.BytesPerRow;
			int num = 1 - this.ImgInfo.BytesPixel;
			int i = 1;
			while (i <= bytesPerRow)
			{
				this.rowbfilter[i] = (byte)((int)this.rowb[i] - PngHelperInternal.FilterPaethPredictor((int)((num > 0) ? this.rowb[num] : 0), (int)this.rowbprev[i], (int)((num > 0) ? this.rowbprev[num] : 0)));
				i++;
				num++;
			}
		}

		private void FilterRowSub()
		{
			int i;
			for (i = 1; i <= this.ImgInfo.BytesPixel; i++)
			{
				this.rowbfilter[i] = this.rowb[i];
			}
			int num = 1;
			i = this.ImgInfo.BytesPixel + 1;
			while (i <= this.ImgInfo.BytesPerRow)
			{
				this.rowbfilter[i] = this.rowb[i] - this.rowb[num];
				i++;
				num++;
			}
		}

		private void FilterRowUp()
		{
			for (int i = 1; i <= this.ImgInfo.BytesPerRow; i++)
			{
				this.rowbfilter[i] = this.rowb[i] - this.rowbprev[i];
			}
		}

		private long SumRowbfilter()
		{
			long num = 0L;
			for (int i = 1; i <= this.ImgInfo.BytesPerRow; i++)
			{
				if (this.rowbfilter[i] < 0)
				{
					num -= (long)((ulong)this.rowbfilter[i]);
				}
				else
				{
					num += (long)((ulong)this.rowbfilter[i]);
				}
			}
			return num;
		}

		private void CopyChunks(PngReader reader, int copy_mask, bool onlyAfterIdat)
		{
			bool flag = this.CurrentChunkGroup >= 4;
			if (onlyAfterIdat && reader.CurrentChunkGroup < 6)
			{
				throw new PngjException("tried to copy last chunks but reader has not ended");
			}
			foreach (PngChunk pngChunk in reader.GetChunksList().GetChunks())
			{
				int chunkGroup = pngChunk.ChunkGroup;
				if (chunkGroup >= 4 || !flag)
				{
					bool flag2 = false;
					if (pngChunk.Crit)
					{
						if (pngChunk.Id.Equals("PLTE"))
						{
							if (this.ImgInfo.Indexed && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_PALETTE))
							{
								flag2 = true;
							}
							if (!this.ImgInfo.Greyscale && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALL))
							{
								flag2 = true;
							}
						}
					}
					else
					{
						bool flag3 = pngChunk is PngChunkTextVar;
						bool safe = pngChunk.Safe;
						if (ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALL))
						{
							flag2 = true;
						}
						if (safe && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALL_SAFE))
						{
							flag2 = true;
						}
						if (pngChunk.Id.Equals("tRNS") && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_TRANSPARENCY))
						{
							flag2 = true;
						}
						if (pngChunk.Id.Equals("pHYs") && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_PHYS))
						{
							flag2 = true;
						}
						if (flag3 && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_TEXTUAL))
						{
							flag2 = true;
						}
						if (ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALMOSTALL) && !ChunkHelper.IsUnknown(pngChunk) && !flag3 && !pngChunk.Id.Equals("hIST") && !pngChunk.Id.Equals("tIME"))
						{
							flag2 = true;
						}
						if (pngChunk is PngChunkSkipped)
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						this.chunksList.Queue(PngChunk.CloneChunk<PngChunk>(pngChunk, this.ImgInfo));
					}
				}
			}
		}

		public void CopyChunksFirst(PngReader reader, int copy_mask)
		{
			this.CopyChunks(reader, copy_mask, false);
		}

		public void CopyChunksLast(PngReader reader, int copy_mask)
		{
			this.CopyChunks(reader, copy_mask, true);
		}

		public double ComputeCompressionRatio()
		{
			if (this.CurrentChunkGroup < 6)
			{
				throw new PngjException("must be called after End()");
			}
			double num = (double)this.datStream.GetCountFlushed();
			double num2 = (double)((this.ImgInfo.BytesPerRow + 1) * this.ImgInfo.Rows);
			return num / num2;
		}

		public void End()
		{
			if (this.rowNum != this.ImgInfo.Rows - 1)
			{
				throw new PngjOutputException("all rows have not been written");
			}
			try
			{
				this.datStreamDeflated.Close();
				this.datStream.Close();
				this.WriteLastChunks();
				this.WriteEndChunk();
				if (this.ShouldCloseStream)
				{
					this.outputStream.Close();
				}
			}
			catch (IOException ex)
			{
				throw new PngjOutputException(ex);
			}
		}

		public string GetFilename()
		{
			return this.filename;
		}

		public void WriteRow(ImageLine imgline, int rownumber)
		{
			this.SetUseUnPackedMode(imgline.SamplesUnpacked);
			if (imgline.SampleType == ImageLine.ESampleType.INT)
			{
				this.WriteRowInt(imgline.Scanline, rownumber);
				return;
			}
			this.WriteRowByte(imgline.ScanlineB, rownumber);
		}

		public void WriteRow(int[] newrow)
		{
			this.WriteRow(newrow, -1);
		}

		public void WriteRow(int[] newrow, int rown)
		{
			this.WriteRowInt(newrow, rown);
		}

		public void WriteRowInt(int[] newrow, int rown)
		{
			this.prepareEncodeRow(rown);
			this.encodeRowFromInt(newrow);
			this.filterAndSend(rown);
		}

		public void WriteRowByte(byte[] newrow, int rown)
		{
			this.prepareEncodeRow(rown);
			this.encodeRowFromByte(newrow);
			this.filterAndSend(rown);
		}

		public void WriteRowsInt(int[][] image)
		{
			for (int i = 0; i < this.ImgInfo.Rows; i++)
			{
				this.WriteRowInt(image[i], i);
			}
		}

		public void WriteRowsByte(byte[][] image)
		{
			for (int i = 0; i < this.ImgInfo.Rows; i++)
			{
				this.WriteRowByte(image[i], i);
			}
		}

		public PngMetadata GetMetadata()
		{
			return this.metadata;
		}

		public ChunksListForWrite GetChunksList()
		{
			return this.chunksList;
		}

		public void SetFilterType(FilterType filterType)
		{
			this.filterStrat = new FilterWriteStrategy(this.ImgInfo, filterType);
		}

		public bool IsUnpackedMode()
		{
			return this.unpackedMode;
		}

		public void SetUseUnPackedMode(bool useUnpackedMode)
		{
			this.unpackedMode = useUnpackedMode;
			this.needsPack = this.unpackedMode && this.ImgInfo.Packed;
		}

		public readonly ImageInfo ImgInfo;

		protected readonly string filename;

		private FilterWriteStrategy filterStrat;

		private readonly PngMetadata metadata;

		private readonly ChunksListForWrite chunksList;

		protected byte[] rowb;

		protected byte[] rowbprev;

		protected byte[] rowbfilter;

		private int rowNum = -1;

		private readonly Stream outputStream;

		private PngIDatChunkOutputStream datStream;

		private AZlibOutputStream datStreamDeflated;

		private int[] histox = new int[256];

		private bool unpackedMode;

		private bool needsPack;
	}
}
