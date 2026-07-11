using System;

namespace System.Xml
{
	internal class Ucs4Encoding2143 : Ucs4Encoding
	{
		public Ucs4Encoding2143()
		{
			this.ucs4Decoder = new Ucs4Decoder2143();
		}

		public override string EncodingName
		{
			get
			{
				return "ucs-4 (order 2143)";
			}
		}

		public override byte[] GetPreamble()
		{
			return new byte[] { 0, 0, byte.MaxValue, 254 };
		}
	}
}
