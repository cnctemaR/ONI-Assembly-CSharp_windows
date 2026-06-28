using System;

namespace Hjg.Pngcs
{
	public class ImageInfo
	{
		public ImageInfo(int cols, int rows, int bitdepth, bool alpha)
			: this(cols, rows, bitdepth, alpha, false, false)
		{
		}

		public ImageInfo(int cols, int rows, int bitdepth, bool alpha, bool grayscale, bool palette)
		{
			this.Cols = cols;
			this.Rows = rows;
			this.Alpha = alpha;
			this.Indexed = palette;
			this.Greyscale = grayscale;
			if (this.Greyscale && palette)
			{
				throw new PngjException("palette and greyscale are exclusive");
			}
			this.Channels = ((grayscale || palette) ? (alpha ? 2 : 1) : (alpha ? 4 : 3));
			this.BitDepth = bitdepth;
			this.Packed = bitdepth < 8;
			this.BitspPixel = this.Channels * this.BitDepth;
			this.BytesPixel = (this.BitspPixel + 7) / 8;
			this.BytesPerRow = (this.BitspPixel * cols + 7) / 8;
			this.SamplesPerRow = this.Channels * this.Cols;
			this.SamplesPerRowPacked = (this.Packed ? this.BytesPerRow : this.SamplesPerRow);
			int bitDepth = this.BitDepth;
			switch (bitDepth)
			{
			case 1:
			case 2:
			case 4:
				if (!this.Indexed && !this.Greyscale)
				{
					throw new PngjException("only indexed or grayscale can have bitdepth=" + this.BitDepth);
				}
				goto IL_016B;
			case 3:
				break;
			default:
				if (bitDepth == 8)
				{
					goto IL_016B;
				}
				if (bitDepth == 16)
				{
					if (this.Indexed)
					{
						throw new PngjException("indexed can't have bitdepth=" + this.BitDepth);
					}
					goto IL_016B;
				}
				break;
			}
			throw new PngjException("invalid bitdepth=" + this.BitDepth);
			IL_016B:
			if (cols < 1 || cols > 400000)
			{
				throw new PngjException("invalid cols=" + cols + " ???");
			}
			if (rows < 1 || rows > 400000)
			{
				throw new PngjException("invalid rows=" + rows + " ???");
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"ImageInfo [cols=", this.Cols, ", rows=", this.Rows, ", bitDepth=", this.BitDepth, ", channels=", this.Channels, ", bitspPixel=", this.BitspPixel,
				", bytesPixel=", this.BytesPixel, ", bytesPerRow=", this.BytesPerRow, ", samplesPerRow=", this.SamplesPerRow, ", samplesPerRowP=", this.SamplesPerRowPacked, ", alpha=", this.Alpha,
				", greyscale=", this.Greyscale, ", indexed=", this.Indexed, ", packed=", this.Packed, "]"
			});
		}

		public override int GetHashCode()
		{
			int num = 31;
			int num2 = 1;
			num2 = num * num2 + (this.Alpha ? 1231 : 1237);
			num2 = num * num2 + this.BitDepth;
			num2 = num * num2 + this.Channels;
			num2 = num * num2 + this.Cols;
			num2 = num * num2 + (this.Greyscale ? 1231 : 1237);
			num2 = num * num2 + (this.Indexed ? 1231 : 1237);
			return num * num2 + this.Rows;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (base.GetType() != obj.GetType())
			{
				return false;
			}
			ImageInfo imageInfo = (ImageInfo)obj;
			return this.Alpha == imageInfo.Alpha && this.BitDepth == imageInfo.BitDepth && this.Channels == imageInfo.Channels && this.Cols == imageInfo.Cols && this.Greyscale == imageInfo.Greyscale && this.Indexed == imageInfo.Indexed && this.Rows == imageInfo.Rows;
		}

		private const int MAX_COLS_ROWS_VAL = 400000;

		public readonly int Cols;

		public readonly int Rows;

		public readonly int BitDepth;

		public readonly int Channels;

		public readonly int BitspPixel;

		public readonly int BytesPixel;

		public readonly int BytesPerRow;

		public readonly int SamplesPerRow;

		public readonly int SamplesPerRowPacked;

		public readonly bool Alpha;

		public readonly bool Greyscale;

		public readonly bool Indexed;

		public readonly bool Packed;
	}
}
