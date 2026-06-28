using System;

namespace Hjg.Pngcs
{
	public class ImageLine
	{
		public ImageInfo ImgInfo { get; private set; }

		public int[] Scanline { get; private set; }

		public byte[] ScanlineB { get; private set; }

		public int Rown { get; set; }

		public int ElementsPerRow { get; private set; }

		public int maxSampleVal { get; private set; }

		public ImageLine.ESampleType SampleType { get; private set; }

		public bool SamplesUnpacked { get; private set; }

		public FilterType FilterUsed { get; set; }

		public ImageLine(ImageInfo imgInfo)
			: this(imgInfo, ImageLine.ESampleType.INT, false)
		{
		}

		public ImageLine(ImageInfo imgInfo, ImageLine.ESampleType stype)
			: this(imgInfo, stype, false)
		{
		}

		public ImageLine(ImageInfo imgInfo, ImageLine.ESampleType stype, bool unpackedMode)
			: this(imgInfo, stype, unpackedMode, null, null)
		{
		}

		internal ImageLine(ImageInfo imgInfo, ImageLine.ESampleType stype, bool unpackedMode, int[] sci, byte[] scb)
		{
			this.ImgInfo = imgInfo;
			this.channels = imgInfo.Channels;
			this.bitDepth = imgInfo.BitDepth;
			this.FilterUsed = FilterType.FILTER_UNKNOWN;
			this.SampleType = stype;
			this.SamplesUnpacked = unpackedMode || !imgInfo.Packed;
			this.ElementsPerRow = (this.SamplesUnpacked ? imgInfo.SamplesPerRow : imgInfo.SamplesPerRowPacked);
			if (stype == ImageLine.ESampleType.INT)
			{
				this.Scanline = ((sci != null) ? sci : new int[this.ElementsPerRow]);
				this.ScanlineB = null;
				this.maxSampleVal = ((this.bitDepth == 16) ? 65535 : ImageLine.GetMaskForPackedFormatsLs(this.bitDepth));
			}
			else
			{
				if (stype != ImageLine.ESampleType.BYTE)
				{
					throw new PngjExceptionInternal("bad ImageLine initialization");
				}
				this.ScanlineB = ((scb != null) ? scb : new byte[this.ElementsPerRow]);
				this.Scanline = null;
				this.maxSampleVal = ((this.bitDepth == 16) ? 255 : ImageLine.GetMaskForPackedFormatsLs(this.bitDepth));
			}
			this.Rown = -1;
		}

		internal static void unpackInplaceInt(ImageInfo iminfo, int[] src, int[] dst, bool Scale)
		{
			int num = iminfo.BitDepth;
			if (num >= 8)
			{
				return;
			}
			int maskForPackedFormatsLs = ImageLine.GetMaskForPackedFormatsLs(num);
			int num2 = 8 - num;
			int num3 = 8 * iminfo.SamplesPerRowPacked - num * iminfo.SamplesPerRow;
			int num4;
			int num5;
			if (num3 != 8)
			{
				num4 = maskForPackedFormatsLs << num3;
				num5 = num3;
			}
			else
			{
				num4 = maskForPackedFormatsLs;
				num5 = 0;
			}
			int i = iminfo.SamplesPerRow - 1;
			int num6 = iminfo.SamplesPerRowPacked - 1;
			while (i >= 0)
			{
				int num7 = (src[num6] & num4) >> num5;
				if (Scale)
				{
					num7 <<= num2;
				}
				dst[i] = num7;
				num4 <<= num;
				num5 += num;
				if (num5 == 8)
				{
					num4 = maskForPackedFormatsLs;
					num5 = 0;
					num6--;
				}
				i--;
			}
		}

		internal static void packInplaceInt(ImageInfo iminfo, int[] src, int[] dst, bool scaled)
		{
			int num = iminfo.BitDepth;
			if (num >= 8)
			{
				return;
			}
			int maskForPackedFormatsLs = ImageLine.GetMaskForPackedFormatsLs(num);
			int num2 = 8 - num;
			int num3 = 8 - num;
			int num4 = 8 - num;
			int num5 = src[0];
			dst[0] = 0;
			if (scaled)
			{
				num5 >>= num2;
			}
			num5 = (num5 & maskForPackedFormatsLs) << num4;
			int num6 = 0;
			for (int i = 0; i < iminfo.SamplesPerRow; i++)
			{
				int num7 = src[i];
				if (scaled)
				{
					num7 >>= num2;
				}
				dst[num6] |= (num7 & maskForPackedFormatsLs) << num4;
				num4 -= num;
				if (num4 < 0)
				{
					num4 = num3;
					num6++;
					dst[num6] = 0;
				}
			}
			dst[0] |= num5;
		}

		internal static void unpackInplaceByte(ImageInfo iminfo, byte[] src, byte[] dst, bool scale)
		{
			int num = iminfo.BitDepth;
			if (num >= 8)
			{
				return;
			}
			int maskForPackedFormatsLs = ImageLine.GetMaskForPackedFormatsLs(num);
			int num2 = 8 - num;
			int num3 = 8 * iminfo.SamplesPerRowPacked - num * iminfo.SamplesPerRow;
			int num4;
			int num5;
			if (num3 != 8)
			{
				num4 = maskForPackedFormatsLs << num3;
				num5 = num3;
			}
			else
			{
				num4 = maskForPackedFormatsLs;
				num5 = 0;
			}
			int i = iminfo.SamplesPerRow - 1;
			int num6 = iminfo.SamplesPerRowPacked - 1;
			while (i >= 0)
			{
				int num7 = ((int)src[num6] & num4) >> num5;
				if (scale)
				{
					num7 <<= num2;
				}
				dst[i] = (byte)num7;
				num4 <<= num;
				num5 += num;
				if (num5 == 8)
				{
					num4 = maskForPackedFormatsLs;
					num5 = 0;
					num6--;
				}
				i--;
			}
		}

		internal static void packInplaceByte(ImageInfo iminfo, byte[] src, byte[] dst, bool scaled)
		{
			int num = iminfo.BitDepth;
			if (num >= 8)
			{
				return;
			}
			byte b = (byte)ImageLine.GetMaskForPackedFormatsLs(num);
			byte b2 = (byte)(8 - num);
			byte b3 = (byte)(8 - num);
			int num2 = 8 - num;
			byte b4 = src[0];
			dst[0] = 0;
			if (scaled)
			{
				b4 = (byte)(b4 >> (int)b2);
			}
			b4 = (byte)((b4 & b) << num2);
			int num3 = 0;
			for (int i = 0; i < iminfo.SamplesPerRow; i++)
			{
				byte b5 = src[i];
				if (scaled)
				{
					b5 = (byte)(b5 >> (int)b2);
				}
				int num4 = num3;
				dst[num4] |= (byte)((b5 & b) << num2);
				num2 -= num;
				if (num2 < 0)
				{
					num2 = (int)b3;
					num3++;
					dst[num3] = 0;
				}
			}
			int num5 = 0;
			dst[num5] |= b4;
		}

		internal void SetScanLine(int[] b)
		{
			Array.Copy(b, 0, this.Scanline, 0, this.Scanline.Length);
		}

		internal int[] GetScanLineCopy(int[] b)
		{
			if (b == null || b.Length < this.Scanline.Length)
			{
				b = new int[this.Scanline.Length];
			}
			Array.Copy(this.Scanline, 0, b, 0, this.Scanline.Length);
			return b;
		}

		public ImageLine unpackToNewImageLine()
		{
			ImageLine imageLine = new ImageLine(this.ImgInfo, this.SampleType, true);
			if (this.SampleType == ImageLine.ESampleType.INT)
			{
				ImageLine.unpackInplaceInt(this.ImgInfo, this.Scanline, imageLine.Scanline, false);
			}
			else
			{
				ImageLine.unpackInplaceByte(this.ImgInfo, this.ScanlineB, imageLine.ScanlineB, false);
			}
			return imageLine;
		}

		public ImageLine packToNewImageLine()
		{
			ImageLine imageLine = new ImageLine(this.ImgInfo, this.SampleType, false);
			if (this.SampleType == ImageLine.ESampleType.INT)
			{
				ImageLine.packInplaceInt(this.ImgInfo, this.Scanline, imageLine.Scanline, false);
			}
			else
			{
				ImageLine.packInplaceByte(this.ImgInfo, this.ScanlineB, imageLine.ScanlineB, false);
			}
			return imageLine;
		}

		public int[] GetScanlineInt()
		{
			return this.Scanline;
		}

		public byte[] GetScanlineByte()
		{
			return this.ScanlineB;
		}

		public bool IsInt()
		{
			return this.SampleType == ImageLine.ESampleType.INT;
		}

		public bool IsByte()
		{
			return this.SampleType == ImageLine.ESampleType.BYTE;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"row=",
				this.Rown,
				" cols=",
				this.ImgInfo.Cols,
				" bpc=",
				this.ImgInfo.BitDepth,
				" size=",
				this.Scanline.Length
			});
		}

		internal static int GetMaskForPackedFormats(int bitDepth)
		{
			if (bitDepth == 4)
			{
				return 240;
			}
			if (bitDepth == 2)
			{
				return 192;
			}
			if (bitDepth == 1)
			{
				return 128;
			}
			return 255;
		}

		internal static int GetMaskForPackedFormatsLs(int bitDepth)
		{
			if (bitDepth == 4)
			{
				return 15;
			}
			if (bitDepth == 2)
			{
				return 3;
			}
			if (bitDepth == 1)
			{
				return 1;
			}
			return 255;
		}

		internal readonly int channels;

		internal readonly int bitDepth;

		public enum ESampleType
		{
			INT,
			BYTE
		}
	}
}
