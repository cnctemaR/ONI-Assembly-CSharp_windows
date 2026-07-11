using System;

namespace System.Xml
{
	internal class Ucs4Encoding1234 : Ucs4Encoding
	{
		public Ucs4Encoding1234()
		{
			this.ucs4Decoder = new Ucs4Decoder1234();
		}

		public override string EncodingName
		{
			get
			{
				return "ucs-4 (Bigendian)";
			}
		}

		public override byte[] GetPreamble()
		{
			return new byte[] { 0, 0, 254, byte.MaxValue };
		}
	}
}
