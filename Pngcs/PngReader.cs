using System;
using System.Collections.Generic;
using System.IO;
using Hjg.Pngcs.Chunks;
using Hjg.Pngcs.Zlib;

namespace Hjg.Pngcs
{
	public class PngReader
	{
		public ImageInfo ImgInfo { get; private set; }

		public ChunkLoadBehaviour ChunkLoadBehaviour { get; set; }

		public bool ShouldCloseStream { get; set; }

		public long MaxBytesMetadata { get; set; }

		public long MaxTotalBytesRead { get; set; }

		public int SkipChunkMaxSize { get; set; }

		public string[] SkipChunkIds { get; set; }

		public int CurrentChunkGroup { get; private set; }

		public PngReader(Stream inputStream)
			: this(inputStream, "[NO FILENAME AVAILABLE]")
		{
		}

		public PngReader(Stream inputStream, string filename)
		{
			this.filename = ((filename == null) ? "" : filename);
			this.inputStream = inputStream;
			this.chunksList = new ChunksList(null);
			this.metadata = new PngMetadata(this.chunksList);
			this.offset = 0L;
			this.CurrentChunkGroup = -1;
			this.ShouldCloseStream = true;
			this.MaxBytesMetadata = 5242880L;
			this.MaxTotalBytesRead = 209715200L;
			this.SkipChunkMaxSize = 2097152;
			this.SkipChunkIds = new string[] { "fdAT" };
			this.ChunkLoadBehaviour = ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS;
			byte[] array = new byte[8];
			PngHelperInternal.ReadBytes(inputStream, array, 0, array.Length);
			this.offset += (long)array.Length;
			if (!PngCsUtils.arraysEqual(array, PngHelperInternal.PNG_ID_SIGNATURE))
			{
				throw new PngjInputException("Bad PNG signature");
			}
			this.CurrentChunkGroup = 0;
			int num = PngHelperInternal.ReadInt4(inputStream);
			this.offset += 4L;
			if (num != 13)
			{
				throw new Exception("IDHR chunk len != 13 ?? " + num);
			}
			byte[] array2 = new byte[4];
			PngHelperInternal.ReadBytes(inputStream, array2, 0, 4);
			if (!PngCsUtils.arraysEqual4(array2, ChunkHelper.b_IHDR))
			{
				throw new PngjInputException("IHDR not found as first chunk??? [" + ChunkHelper.ToString(array2) + "]");
			}
			this.offset += 4L;
			PngChunkIHDR pngChunkIHDR = (PngChunkIHDR)this.ReadChunk(array2, num, false);
			bool flag = (pngChunkIHDR.Colormodel & 4) != 0;
			bool flag2 = (pngChunkIHDR.Colormodel & 1) != 0;
			bool flag3 = pngChunkIHDR.Colormodel == 0 || pngChunkIHDR.Colormodel == 4;
			this.ImgInfo = new ImageInfo(pngChunkIHDR.Cols, pngChunkIHDR.Rows, pngChunkIHDR.Bitspc, flag, flag3, flag2);
			this.rowb = new byte[this.ImgInfo.BytesPerRow + 1];
			this.rowbprev = new byte[this.rowb.Length];
			this.rowbfilter = new byte[this.rowb.Length];
			this.interlaced = pngChunkIHDR.Interlaced == 1;
			this.deinterlacer = (this.interlaced ? new PngDeinterlacer(this.ImgInfo) : null);
			if (pngChunkIHDR.Filmeth != 0 || pngChunkIHDR.Compmeth != 0 || (pngChunkIHDR.Interlaced & 65534) != 0)
			{
				throw new PngjInputException("compmethod or filtermethod or interlaced unrecognized");
			}
			if (pngChunkIHDR.Colormodel < 0 || pngChunkIHDR.Colormodel > 6 || pngChunkIHDR.Colormodel == 1 || pngChunkIHDR.Colormodel == 5)
			{
				throw new PngjInputException("Invalid colormodel " + pngChunkIHDR.Colormodel);
			}
			if (pngChunkIHDR.Bitspc != 1 && pngChunkIHDR.Bitspc != 2 && pngChunkIHDR.Bitspc != 4 && pngChunkIHDR.Bitspc != 8 && pngChunkIHDR.Bitspc != 16)
			{
				throw new PngjInputException("Invalid bit depth " + pngChunkIHDR.Bitspc);
			}
		}

		private bool FirstChunksNotYetRead()
		{
			return this.CurrentChunkGroup < 1;
		}

		private void ReadLastAndClose()
		{
			if (this.CurrentChunkGroup < 5)
			{
				try
				{
					this.idatIstream.Close();
				}
				catch (Exception)
				{
				}
				this.ReadLastChunks();
			}
			this.Close();
		}

		private void Close()
		{
			if (this.CurrentChunkGroup < 6)
			{
				try
				{
					this.idatIstream.Close();
				}
				catch (Exception)
				{
				}
				this.CurrentChunkGroup = 6;
			}
			if (this.ShouldCloseStream)
			{
				this.inputStream.Close();
			}
		}

		private void UnfilterRow(int nbytes)
		{
			int num = (int)this.rowbfilter[0];
			switch (num)
			{
			case 0:
				this.UnfilterRowNone(nbytes);
				break;
			case 1:
				this.UnfilterRowSub(nbytes);
				break;
			case 2:
				this.UnfilterRowUp(nbytes);
				break;
			case 3:
				this.UnfilterRowAverage(nbytes);
				break;
			case 4:
				this.UnfilterRowPaeth(nbytes);
				break;
			default:
				throw new PngjInputException("Filter type " + num + " not implemented");
			}
			if (this.crctest != null)
			{
				this.crctest.Update(this.rowb, 1, nbytes);
			}
		}

		private void UnfilterRowAverage(int nbytes)
		{
			int num = 1 - this.ImgInfo.BytesPixel;
			int i = 1;
			while (i <= nbytes)
			{
				int num2 = (int)((num > 0) ? this.rowb[num] : 0);
				this.rowb[i] = (byte)((int)this.rowbfilter[i] + (num2 + (int)(this.rowbprev[i] & byte.MaxValue)) / 2);
				i++;
				num++;
			}
		}

		private void UnfilterRowNone(int nbytes)
		{
			for (int i = 1; i <= nbytes; i++)
			{
				this.rowb[i] = this.rowbfilter[i];
			}
		}

		private void UnfilterRowPaeth(int nbytes)
		{
			int num = 1 - this.ImgInfo.BytesPixel;
			int i = 1;
			while (i <= nbytes)
			{
				int num2 = (int)((num > 0) ? this.rowb[num] : 0);
				int num3 = (int)((num > 0) ? this.rowbprev[num] : 0);
				this.rowb[i] = (byte)((int)this.rowbfilter[i] + PngHelperInternal.FilterPaethPredictor(num2, (int)this.rowbprev[i], num3));
				i++;
				num++;
			}
		}

		private void UnfilterRowSub(int nbytes)
		{
			int i;
			for (i = 1; i <= this.ImgInfo.BytesPixel; i++)
			{
				this.rowb[i] = this.rowbfilter[i];
			}
			int num = 1;
			i = this.ImgInfo.BytesPixel + 1;
			while (i <= nbytes)
			{
				this.rowb[i] = this.rowbfilter[i] + this.rowb[num];
				i++;
				num++;
			}
		}

		private void UnfilterRowUp(int nbytes)
		{
			for (int i = 1; i <= nbytes; i++)
			{
				this.rowb[i] = this.rowbfilter[i] + this.rowbprev[i];
			}
		}

		private void ReadFirstChunks()
		{
			if (!this.FirstChunksNotYetRead())
			{
				return;
			}
			int num = 0;
			bool flag = false;
			byte[] array = new byte[4];
			this.CurrentChunkGroup = 1;
			while (!flag)
			{
				num = PngHelperInternal.ReadInt4(this.inputStream);
				this.offset += 4L;
				if (num < 0)
				{
					break;
				}
				PngHelperInternal.ReadBytes(this.inputStream, array, 0, 4);
				this.offset += 4L;
				if (PngCsUtils.arraysEqual4(array, ChunkHelper.b_IDAT))
				{
					flag = true;
					this.CurrentChunkGroup = 4;
					this.chunksList.AppendReadChunk(new PngChunkIDAT(this.ImgInfo, num, this.offset - 8L), this.CurrentChunkGroup);
					break;
				}
				if (PngCsUtils.arraysEqual4(array, ChunkHelper.b_IEND))
				{
					throw new PngjInputException("END chunk found before image data (IDAT) at offset=" + this.offset);
				}
				string text = ChunkHelper.ToString(array);
				if (text.Equals("PLTE"))
				{
					this.CurrentChunkGroup = 2;
				}
				this.ReadChunk(array, num, false);
				if (text.Equals("PLTE"))
				{
					this.CurrentChunkGroup = 3;
				}
			}
			int num2 = (flag ? num : (-1));
			if (num2 < 0)
			{
				throw new PngjInputException("first idat chunk not found!");
			}
			this.iIdatCstream = new PngIDatChunkInputStream(this.inputStream, num2, this.offset);
			this.idatIstream = ZlibStreamFactory.createZlibInputStream(this.iIdatCstream, true);
			if (!this.crcEnabled)
			{
				this.iIdatCstream.DisableCrcCheck();
			}
		}

		private void ReadLastChunks()
		{
			this.CurrentChunkGroup = 5;
			if (!this.iIdatCstream.IsEnded())
			{
				this.iIdatCstream.ForceChunkEnd();
			}
			int num = this.iIdatCstream.GetLenLastChunk();
			byte[] idLastChunk = this.iIdatCstream.GetIdLastChunk();
			bool flag = false;
			bool flag2 = true;
			while (!flag)
			{
				bool flag3 = false;
				if (!flag2)
				{
					num = PngHelperInternal.ReadInt4(this.inputStream);
					this.offset += 4L;
					if (num < 0)
					{
						throw new PngjInputException("bad len " + num);
					}
					PngHelperInternal.ReadBytes(this.inputStream, idLastChunk, 0, 4);
					this.offset += 4L;
				}
				flag2 = false;
				if (PngCsUtils.arraysEqual4(idLastChunk, ChunkHelper.b_IDAT))
				{
					flag3 = true;
				}
				else if (PngCsUtils.arraysEqual4(idLastChunk, ChunkHelper.b_IEND))
				{
					this.CurrentChunkGroup = 6;
					flag = true;
				}
				this.ReadChunk(idLastChunk, num, flag3);
			}
			if (!flag)
			{
				throw new PngjInputException("end chunk not found - offset=" + this.offset);
			}
		}

		private PngChunk ReadChunk(byte[] chunkid, int clen, bool skipforced)
		{
			if (clen < 0)
			{
				throw new PngjInputException("invalid chunk lenght: " + clen);
			}
			if (this.skipChunkIdsSet == null && this.CurrentChunkGroup > 0)
			{
				this.skipChunkIdsSet = new Dictionary<string, int>();
				if (this.SkipChunkIds != null)
				{
					foreach (string text in this.SkipChunkIds)
					{
						this.skipChunkIdsSet.Add(text, 1);
					}
				}
			}
			string text2 = ChunkHelper.ToString(chunkid);
			bool flag = ChunkHelper.IsCritical(text2);
			bool flag2 = skipforced;
			if (this.MaxTotalBytesRead > 0L && (long)clen + this.offset > this.MaxTotalBytesRead)
			{
				throw new PngjInputException(string.Concat(new object[] { "Maximum total bytes to read exceeeded: ", this.MaxTotalBytesRead, " offset:", this.offset, " clen=", clen }));
			}
			if (this.CurrentChunkGroup > 0 && !ChunkHelper.IsCritical(text2))
			{
				flag2 = flag2 || (this.SkipChunkMaxSize > 0 && clen >= this.SkipChunkMaxSize) || this.skipChunkIdsSet.ContainsKey(text2) || (this.MaxBytesMetadata > 0L && (long)clen > this.MaxBytesMetadata - (long)this.bytesChunksLoaded) || !ChunkHelper.ShouldLoad(text2, this.ChunkLoadBehaviour);
			}
			PngChunk pngChunk;
			if (flag2)
			{
				PngHelperInternal.SkipBytes(this.inputStream, clen);
				PngHelperInternal.ReadInt4(this.inputStream);
				pngChunk = new PngChunkSkipped(text2, this.ImgInfo, clen);
			}
			else
			{
				ChunkRaw chunkRaw = new ChunkRaw(clen, chunkid, true);
				chunkRaw.ReadChunkData(this.inputStream, this.crcEnabled || flag);
				pngChunk = PngChunk.Factory(chunkRaw, this.ImgInfo);
				if (!pngChunk.Crit)
				{
					this.bytesChunksLoaded += chunkRaw.Length;
				}
			}
			pngChunk.Offset = this.offset - 8L;
			this.chunksList.AppendReadChunk(pngChunk, this.CurrentChunkGroup);
			this.offset += (long)clen + 4L;
			return pngChunk;
		}

		internal void logWarn(string warn)
		{
			Console.Error.WriteLine(warn);
		}

		public ChunksList GetChunksList()
		{
			if (this.FirstChunksNotYetRead())
			{
				this.ReadFirstChunks();
			}
			return this.chunksList;
		}

		public PngMetadata GetMetadata()
		{
			if (this.FirstChunksNotYetRead())
			{
				this.ReadFirstChunks();
			}
			return this.metadata;
		}

		public ImageLine ReadRow(int nrow)
		{
			if (this.imgLine != null && this.imgLine.SampleType == ImageLine.ESampleType.BYTE)
			{
				return this.ReadRowByte(nrow);
			}
			return this.ReadRowInt(nrow);
		}

		public ImageLine ReadRowInt(int nrow)
		{
			if (this.imgLine == null)
			{
				this.imgLine = new ImageLine(this.ImgInfo, ImageLine.ESampleType.INT, this.unpackedMode);
			}
			if (this.imgLine.Rown == nrow)
			{
				return this.imgLine;
			}
			this.ReadRowInt(this.imgLine.Scanline, nrow);
			this.imgLine.FilterUsed = (FilterType)this.rowbfilter[0];
			this.imgLine.Rown = nrow;
			return this.imgLine;
		}

		public ImageLine ReadRowByte(int nrow)
		{
			if (this.imgLine == null)
			{
				this.imgLine = new ImageLine(this.ImgInfo, ImageLine.ESampleType.BYTE, this.unpackedMode);
			}
			if (this.imgLine.Rown == nrow)
			{
				return this.imgLine;
			}
			this.ReadRowByte(this.imgLine.ScanlineB, nrow);
			this.imgLine.FilterUsed = (FilterType)this.rowbfilter[0];
			this.imgLine.Rown = nrow;
			return this.imgLine;
		}

		public int[] ReadRow(int[] buffer, int nrow)
		{
			return this.ReadRowInt(buffer, nrow);
		}

		public int[] ReadRowInt(int[] buffer, int nrow)
		{
			if (buffer == null)
			{
				buffer = new int[this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked];
			}
			if (!this.interlaced)
			{
				if (nrow <= this.rowNum)
				{
					throw new PngjInputException("rows must be read in increasing order: " + nrow);
				}
				int num = 0;
				while (this.rowNum < nrow)
				{
					num = this.ReadRowRaw(this.rowNum + 1);
				}
				this.decodeLastReadRowToInt(buffer, num);
			}
			else
			{
				if (this.deinterlacer.getImageInt() == null)
				{
					this.deinterlacer.setImageInt(this.ReadRowsInt().Scanlines);
				}
				Array.Copy(this.deinterlacer.getImageInt()[nrow], 0, buffer, 0, this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked);
			}
			return buffer;
		}

		public byte[] ReadRowByte(byte[] buffer, int nrow)
		{
			if (buffer == null)
			{
				buffer = new byte[this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked];
			}
			if (!this.interlaced)
			{
				if (nrow <= this.rowNum)
				{
					throw new PngjInputException("rows must be read in increasing order: " + nrow);
				}
				int num = 0;
				while (this.rowNum < nrow)
				{
					num = this.ReadRowRaw(this.rowNum + 1);
				}
				this.decodeLastReadRowToByte(buffer, num);
			}
			else
			{
				if (this.deinterlacer.getImageByte() == null)
				{
					this.deinterlacer.setImageByte(this.ReadRowsByte().ScanlinesB);
				}
				Array.Copy(this.deinterlacer.getImageByte()[nrow], 0, buffer, 0, this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked);
			}
			return buffer;
		}

		[Obsolete("GetRow is deprecated,  use ReadRow/ReadRowInt/ReadRowByte instead.")]
		public ImageLine GetRow(int nrow)
		{
			return this.ReadRow(nrow);
		}

		private void decodeLastReadRowToInt(int[] buffer, int bytesRead)
		{
			if (this.ImgInfo.BitDepth <= 8)
			{
				int i = 0;
				int num = 1;
				while (i < bytesRead)
				{
					buffer[i] = (int)this.rowb[num++];
					i++;
				}
			}
			else
			{
				int num2 = 0;
				int j = 1;
				while (j < bytesRead)
				{
					buffer[num2] = ((int)this.rowb[j++] << 8) + (int)this.rowb[j++];
					num2++;
				}
			}
			if (this.ImgInfo.Packed && this.unpackedMode)
			{
				ImageLine.unpackInplaceInt(this.ImgInfo, buffer, buffer, false);
			}
		}

		private void decodeLastReadRowToByte(byte[] buffer, int bytesRead)
		{
			if (this.ImgInfo.BitDepth <= 8)
			{
				Array.Copy(this.rowb, 1, buffer, 0, bytesRead);
			}
			else
			{
				int num = 0;
				for (int i = 1; i < bytesRead; i += 2)
				{
					buffer[num] = this.rowb[i];
					num++;
				}
			}
			if (this.ImgInfo.Packed && this.unpackedMode)
			{
				ImageLine.unpackInplaceByte(this.ImgInfo, buffer, buffer, false);
			}
		}

		public ImageLines ReadRowsInt(int rowOffset, int nRows, int rowStep)
		{
			if (nRows < 0)
			{
				nRows = (this.ImgInfo.Rows - rowOffset) / rowStep;
			}
			if (rowStep < 1 || rowOffset < 0 || nRows * rowStep + rowOffset > this.ImgInfo.Rows)
			{
				throw new PngjInputException("bad args");
			}
			ImageLines imageLines = new ImageLines(this.ImgInfo, ImageLine.ESampleType.INT, this.unpackedMode, rowOffset, nRows, rowStep);
			if (!this.interlaced)
			{
				for (int i = 0; i < this.ImgInfo.Rows; i++)
				{
					int num = this.ReadRowRaw(i);
					int num2 = imageLines.ImageRowToMatrixRowStrict(i);
					if (num2 >= 0)
					{
						this.decodeLastReadRowToInt(imageLines.Scanlines[num2], num);
					}
				}
			}
			else
			{
				int[] array = new int[this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked];
				for (int j = 1; j <= 7; j++)
				{
					this.deinterlacer.setPass(j);
					for (int k = 0; k < this.deinterlacer.getRows(); k++)
					{
						int num3 = this.ReadRowRaw(k);
						int currRowReal = this.deinterlacer.getCurrRowReal();
						int num4 = imageLines.ImageRowToMatrixRowStrict(currRowReal);
						if (num4 >= 0)
						{
							this.decodeLastReadRowToInt(array, num3);
							this.deinterlacer.deinterlaceInt(array, imageLines.Scanlines[num4], !this.unpackedMode);
						}
					}
				}
			}
			this.End();
			return imageLines;
		}

		public ImageLines ReadRowsInt()
		{
			return this.ReadRowsInt(0, this.ImgInfo.Rows, 1);
		}

		public ImageLines ReadRowsByte(int rowOffset, int nRows, int rowStep)
		{
			if (nRows < 0)
			{
				nRows = (this.ImgInfo.Rows - rowOffset) / rowStep;
			}
			if (rowStep < 1 || rowOffset < 0 || nRows * rowStep + rowOffset > this.ImgInfo.Rows)
			{
				throw new PngjInputException("bad args");
			}
			ImageLines imageLines = new ImageLines(this.ImgInfo, ImageLine.ESampleType.BYTE, this.unpackedMode, rowOffset, nRows, rowStep);
			if (!this.interlaced)
			{
				for (int i = 0; i < this.ImgInfo.Rows; i++)
				{
					int num = this.ReadRowRaw(i);
					int num2 = imageLines.ImageRowToMatrixRowStrict(i);
					if (num2 >= 0)
					{
						this.decodeLastReadRowToByte(imageLines.ScanlinesB[num2], num);
					}
				}
			}
			else
			{
				byte[] array = new byte[this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked];
				for (int j = 1; j <= 7; j++)
				{
					this.deinterlacer.setPass(j);
					for (int k = 0; k < this.deinterlacer.getRows(); k++)
					{
						int num3 = this.ReadRowRaw(k);
						int currRowReal = this.deinterlacer.getCurrRowReal();
						int num4 = imageLines.ImageRowToMatrixRowStrict(currRowReal);
						if (num4 >= 0)
						{
							this.decodeLastReadRowToByte(array, num3);
							this.deinterlacer.deinterlaceByte(array, imageLines.ScanlinesB[num4], !this.unpackedMode);
						}
					}
				}
			}
			this.End();
			return imageLines;
		}

		public ImageLines ReadRowsByte()
		{
			return this.ReadRowsByte(0, this.ImgInfo.Rows, 1);
		}

		private int ReadRowRaw(int nrow)
		{
			if (nrow == 0 && this.FirstChunksNotYetRead())
			{
				this.ReadFirstChunks();
			}
			if (nrow == 0 && this.interlaced)
			{
				Array.Clear(this.rowb, 0, this.rowb.Length);
			}
			int num = this.ImgInfo.BytesPerRow;
			if (this.interlaced)
			{
				if (nrow < 0 || nrow > this.deinterlacer.getRows() || (nrow != 0 && nrow != this.deinterlacer.getCurrRowSubimg() + 1))
				{
					throw new PngjInputException("invalid row in interlaced mode: " + nrow);
				}
				this.deinterlacer.setRow(nrow);
				num = (this.ImgInfo.BitspPixel * this.deinterlacer.getPixelsToRead() + 7) / 8;
				if (num < 1)
				{
					throw new PngjExceptionInternal("wtf??");
				}
			}
			else if (nrow < 0 || nrow >= this.ImgInfo.Rows || nrow != this.rowNum + 1)
			{
				throw new PngjInputException("invalid row: " + nrow);
			}
			this.rowNum = nrow;
			byte[] array = this.rowb;
			this.rowb = this.rowbprev;
			this.rowbprev = array;
			PngHelperInternal.ReadBytes(this.idatIstream, this.rowbfilter, 0, num + 1);
			this.offset = this.iIdatCstream.GetOffset();
			if (this.offset < 0L)
			{
				throw new PngjExceptionInternal("bad offset ??" + this.offset);
			}
			if (this.MaxTotalBytesRead > 0L && this.offset >= this.MaxTotalBytesRead)
			{
				throw new PngjInputException(string.Concat(new object[] { "Reading IDAT: Maximum total bytes to read exceeeded: ", this.MaxTotalBytesRead, " offset:", this.offset }));
			}
			this.rowb[0] = 0;
			this.UnfilterRow(num);
			this.rowb[0] = this.rowbfilter[0];
			if ((this.rowNum == this.ImgInfo.Rows - 1 && !this.interlaced) || (this.interlaced && this.deinterlacer.isAtLastRow()))
			{
				this.ReadLastAndClose();
			}
			return num;
		}

		public void ReadSkippingAllRows()
		{
			if (this.FirstChunksNotYetRead())
			{
				this.ReadFirstChunks();
			}
			this.iIdatCstream.DisableCrcCheck();
			try
			{
				int num;
				do
				{
					num = this.iIdatCstream.Read(this.rowbfilter, 0, this.rowbfilter.Length);
				}
				while (num >= 0);
			}
			catch (IOException ex)
			{
				throw new PngjInputException("error in raw read of IDAT", ex);
			}
			this.offset = this.iIdatCstream.GetOffset();
			if (this.offset < 0L)
			{
				throw new PngjExceptionInternal("bad offset ??" + this.offset);
			}
			if (this.MaxTotalBytesRead > 0L && this.offset >= this.MaxTotalBytesRead)
			{
				throw new PngjInputException(string.Concat(new object[] { "Reading IDAT: Maximum total bytes to read exceeeded: ", this.MaxTotalBytesRead, " offset:", this.offset }));
			}
			this.ReadLastAndClose();
		}

		public override string ToString()
		{
			return "filename=" + this.filename + " " + this.ImgInfo.ToString();
		}

		public void End()
		{
			if (this.CurrentChunkGroup < 6)
			{
				this.Close();
			}
		}

		public bool IsInterlaced()
		{
			return this.interlaced;
		}

		public void SetUnpackedMode(bool unPackedMode)
		{
			this.unpackedMode = unPackedMode;
		}

		public bool IsUnpackedMode()
		{
			return this.unpackedMode;
		}

		public void SetCrcCheckDisabled()
		{
			this.crcEnabled = false;
		}

		internal long GetCrctestVal()
		{
			return (long)((ulong)this.crctest.GetValue());
		}

		internal void InitCrctest()
		{
			this.crctest = new Adler32();
		}

		protected readonly string filename;

		private Dictionary<string, int> skipChunkIdsSet;

		private readonly PngMetadata metadata;

		private readonly ChunksList chunksList;

		protected ImageLine imgLine;

		protected byte[] rowb;

		protected byte[] rowbprev;

		protected byte[] rowbfilter;

		public readonly bool interlaced;

		private readonly PngDeinterlacer deinterlacer;

		private bool crcEnabled = true;

		private bool unpackedMode;

		protected int rowNum = -1;

		private long offset;

		private int bytesChunksLoaded;

		private readonly Stream inputStream;

		internal AZlibInputStream idatIstream;

		internal PngIDatChunkInputStream iIdatCstream;

		protected Adler32 crctest;
	}
}
