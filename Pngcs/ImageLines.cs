using System;

namespace Hjg.Pngcs
{
	public class ImageLines
	{
		public ImageInfo ImgInfo { get; private set; }

		public ImageLine.ESampleType sampleType { get; private set; }

		public bool SamplesUnpacked { get; private set; }

		public int RowOffset { get; private set; }

		public int Nrows { get; private set; }

		public int RowStep { get; private set; }

		public int[][] Scanlines { get; private set; }

		public byte[][] ScanlinesB { get; private set; }

		public ImageLines(ImageInfo ImgInfo, ImageLine.ESampleType sampleType, bool unpackedMode, int rowOffset, int nRows, int rowStep)
		{
			this.ImgInfo = ImgInfo;
			this.channels = ImgInfo.Channels;
			this.bitDepth = ImgInfo.BitDepth;
			this.sampleType = sampleType;
			this.SamplesUnpacked = unpackedMode || !ImgInfo.Packed;
			this.RowOffset = rowOffset;
			this.Nrows = nRows;
			this.RowStep = rowStep;
			this.elementsPerRow = (unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked);
			if (sampleType == ImageLine.ESampleType.INT)
			{
				this.Scanlines = new int[nRows][];
				for (int i = 0; i < nRows; i++)
				{
					this.Scanlines[i] = new int[this.elementsPerRow];
				}
				this.ScanlinesB = null;
				return;
			}
			if (sampleType == ImageLine.ESampleType.BYTE)
			{
				this.ScanlinesB = new byte[nRows][];
				for (int j = 0; j < nRows; j++)
				{
					this.ScanlinesB[j] = new byte[this.elementsPerRow];
				}
				this.Scanlines = null;
				return;
			}
			throw new PngjExceptionInternal("bad ImageLine initialization");
		}

		public int ImageRowToMatrixRow(int imrow)
		{
			int num = (imrow - this.RowOffset) / this.RowStep;
			if (num < 0)
			{
				return 0;
			}
			if (num >= this.Nrows)
			{
				return this.Nrows - 1;
			}
			return num;
		}

		public int ImageRowToMatrixRowStrict(int imrow)
		{
			imrow -= this.RowOffset;
			int num = ((imrow >= 0 && imrow % this.RowStep == 0) ? (imrow / this.RowStep) : (-1));
			if (num >= this.Nrows)
			{
				return -1;
			}
			return num;
		}

		public int MatrixRowToImageRow(int mrow)
		{
			return mrow * this.RowStep + this.RowOffset;
		}

		public ImageLine GetImageLineAtMatrixRow(int mrow)
		{
			if (mrow < 0 || mrow > this.Nrows)
			{
				throw new PngjException(string.Concat(new object[] { "Bad row ", mrow, ". Should be positive and less than ", this.Nrows }));
			}
			ImageLine imageLine = ((this.sampleType == ImageLine.ESampleType.INT) ? new ImageLine(this.ImgInfo, this.sampleType, this.SamplesUnpacked, this.Scanlines[mrow], null) : new ImageLine(this.ImgInfo, this.sampleType, this.SamplesUnpacked, null, this.ScanlinesB[mrow]));
			imageLine.Rown = this.MatrixRowToImageRow(mrow);
			return imageLine;
		}

		internal readonly int channels;

		internal readonly int bitDepth;

		internal readonly int elementsPerRow;
	}
}
