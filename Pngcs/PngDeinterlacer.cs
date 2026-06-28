using System;

namespace Hjg.Pngcs
{
	internal class PngDeinterlacer
	{
		internal PngDeinterlacer(ImageInfo iminfo)
		{
			this.imi = iminfo;
			this.pass = 0;
			if (this.imi.Packed)
			{
				this.packedValsPerPixel = 8 / this.imi.BitDepth;
				this.packedShift = this.imi.BitDepth;
				if (this.imi.BitDepth == 1)
				{
					this.packedMask = 128;
				}
				else if (this.imi.BitDepth == 2)
				{
					this.packedMask = 192;
				}
				else
				{
					this.packedMask = 240;
				}
			}
			else
			{
				this.packedMask = (this.packedShift = (this.packedValsPerPixel = 1));
			}
			this.setPass(1);
			this.setRow(0);
		}

		internal void setRow(int n)
		{
			this.currRowSubimg = n;
			this.currRowReal = n * this.dY + this.oY;
			if (this.currRowReal < 0 || this.currRowReal >= this.imi.Rows)
			{
				throw new PngjExceptionInternal("bad row - this should not happen");
			}
		}

		internal void setPass(int p)
		{
			if (this.pass == p)
			{
				return;
			}
			this.pass = p;
			switch (this.pass)
			{
			case 1:
				this.dY = (this.dX = 8);
				this.oX = (this.oY = 0);
				break;
			case 2:
				this.dY = (this.dX = 8);
				this.oX = 4;
				this.oY = 0;
				break;
			case 3:
				this.dX = 4;
				this.dY = 8;
				this.oX = 0;
				this.oY = 4;
				break;
			case 4:
				this.dX = (this.dY = 4);
				this.oX = 2;
				this.oY = 0;
				break;
			case 5:
				this.dX = 2;
				this.dY = 4;
				this.oX = 0;
				this.oY = 2;
				break;
			case 6:
				this.dX = (this.dY = 2);
				this.oX = 1;
				this.oY = 0;
				break;
			case 7:
				this.dX = 1;
				this.dY = 2;
				this.oX = 0;
				this.oY = 1;
				break;
			default:
				throw new PngjExceptionInternal("bad interlace pass" + this.pass);
			}
			this.rows = (this.imi.Rows - this.oY) / this.dY + 1;
			if ((this.rows - 1) * this.dY + this.oY >= this.imi.Rows)
			{
				this.rows--;
			}
			this.cols = (this.imi.Cols - this.oX) / this.dX + 1;
			if ((this.cols - 1) * this.dX + this.oX >= this.imi.Cols)
			{
				this.cols--;
			}
			if (this.cols == 0)
			{
				this.rows = 0;
			}
			this.dXsamples = this.dX * this.imi.Channels;
			this.oXsamples = this.oX * this.imi.Channels;
		}

		internal void deinterlaceInt(int[] src, int[] dst, bool readInPackedFormat)
		{
			if (!this.imi.Packed || !readInPackedFormat)
			{
				int i = 0;
				int num = this.oXsamples;
				while (i < this.cols * this.imi.Channels)
				{
					for (int j = 0; j < this.imi.Channels; j++)
					{
						dst[num + j] = src[i + j];
					}
					i += this.imi.Channels;
					num += this.dXsamples;
				}
				return;
			}
			this.deinterlaceIntPacked(src, dst);
		}

		private void deinterlaceIntPacked(int[] src, int[] dst)
		{
			int num = this.packedMask;
			int num2 = -1;
			int i = 0;
			int num3 = this.oX;
			while (i < this.cols)
			{
				int num4 = i / this.packedValsPerPixel;
				num2++;
				if (num2 >= this.packedValsPerPixel)
				{
					num2 = 0;
				}
				num >>= this.packedShift;
				if (num2 == 0)
				{
					num = this.packedMask;
				}
				int num5 = num3 / this.packedValsPerPixel;
				int num6 = num3 % this.packedValsPerPixel;
				int num7 = src[num4] & num;
				int num8 = num6 - num2;
				if (num8 > 0)
				{
					num7 >>= num8 * this.packedShift;
				}
				else if (num8 < 0)
				{
					num7 <<= -num8 * this.packedShift;
				}
				dst[num5] |= num7;
				i++;
				num3 += this.dX;
			}
		}

		internal void deinterlaceByte(byte[] src, byte[] dst, bool readInPackedFormat)
		{
			if (!this.imi.Packed || !readInPackedFormat)
			{
				int i = 0;
				int num = this.oXsamples;
				while (i < this.cols * this.imi.Channels)
				{
					for (int j = 0; j < this.imi.Channels; j++)
					{
						dst[num + j] = src[i + j];
					}
					i += this.imi.Channels;
					num += this.dXsamples;
				}
				return;
			}
			this.deinterlacePackedByte(src, dst);
		}

		private void deinterlacePackedByte(byte[] src, byte[] dst)
		{
			int num = this.packedMask;
			int num2 = -1;
			int i = 0;
			int num3 = this.oX;
			while (i < this.cols)
			{
				int num4 = i / this.packedValsPerPixel;
				num2++;
				if (num2 >= this.packedValsPerPixel)
				{
					num2 = 0;
				}
				num >>= this.packedShift;
				if (num2 == 0)
				{
					num = this.packedMask;
				}
				int num5 = num3 / this.packedValsPerPixel;
				int num6 = num3 % this.packedValsPerPixel;
				int num7 = (int)src[num4] & num;
				int num8 = num6 - num2;
				if (num8 > 0)
				{
					num7 >>= num8 * this.packedShift;
				}
				else if (num8 < 0)
				{
					num7 <<= -num8 * this.packedShift;
				}
				int num9 = num5;
				dst[num9] |= (byte)num7;
				i++;
				num3 += this.dX;
			}
		}

		internal bool isAtLastRow()
		{
			return this.pass == 7 && this.currRowSubimg == this.rows - 1;
		}

		internal int getCurrRowSubimg()
		{
			return this.currRowSubimg;
		}

		internal int getCurrRowReal()
		{
			return this.currRowReal;
		}

		internal int getPass()
		{
			return this.pass;
		}

		internal int getRows()
		{
			return this.rows;
		}

		internal int getCols()
		{
			return this.cols;
		}

		internal int getPixelsToRead()
		{
			return this.getCols();
		}

		internal int[][] getImageInt()
		{
			return this.imageInt;
		}

		internal void setImageInt(int[][] imageInt)
		{
			this.imageInt = imageInt;
		}

		internal byte[][] getImageByte()
		{
			return this.imageByte;
		}

		internal void setImageByte(byte[][] imageByte)
		{
			this.imageByte = imageByte;
		}

		private readonly ImageInfo imi;

		private int pass;

		private int rows;

		private int cols;

		private int dY;

		private int dX;

		private int oY;

		private int oX;

		private int oXsamples;

		private int dXsamples;

		private int currRowSubimg = -1;

		private int currRowReal = -1;

		private readonly int packedValsPerPixel;

		private readonly int packedMask;

		private readonly int packedShift;

		private int[][] imageInt;

		private byte[][] imageByte;
	}
}
