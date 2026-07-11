using System;

namespace System.Xml
{
	internal class Ucs4Encoding4321 : Ucs4Encoding
	{
		public Ucs4Encoding4321()
		{
			this.ucs4Decoder = new Ucs4Decoder4321();
		}

		public override string EncodingName
		{
			get
			{
				return "ucs-4";
			}
		}

		public override byte[] GetPreamble()
		{
			byte[] array = new byte[4];
			array[0] = byte.MaxValue;
			array[1] = 254;
			return array;
		}
	}
}
