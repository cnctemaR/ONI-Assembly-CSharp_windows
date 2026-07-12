using System;

namespace System.Resources
{
	internal class ICONDIRENTRY
	{
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"ICONDIRENTRY (",
				this.bWidth.ToString(),
				"x",
				this.bHeight.ToString(),
				" ",
				this.wBitCount.ToString(),
				" bpp)"
			});
		}

		public byte bWidth;

		public byte bHeight;

		public byte bColorCount;

		public byte bReserved;

		public short wPlanes;

		public short wBitCount;

		public int dwBytesInRes;

		public int dwImageOffset;

		public byte[] image;
	}
}
