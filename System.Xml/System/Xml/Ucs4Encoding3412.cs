using System;

namespace System.Xml
{
	internal class Ucs4Encoding3412 : Ucs4Encoding
	{
		public Ucs4Encoding3412()
		{
			this.ucs4Decoder = new Ucs4Decoder3412();
		}

		public override string EncodingName
		{
			get
			{
				return "ucs-4 (order 3412)";
			}
		}

		public override byte[] GetPreamble()
		{
			byte[] array = new byte[4];
			array[0] = 254;
			array[1] = byte.MaxValue;
			return array;
		}
	}
}
