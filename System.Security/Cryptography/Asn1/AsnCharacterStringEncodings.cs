using System;
using System.Text;

namespace System.Security.Cryptography.Asn1
{
	internal static class AsnCharacterStringEncodings
	{
		internal static Encoding GetEncoding(UniversalTagNumber encodingType)
		{
			if (encodingType <= UniversalTagNumber.PrintableString)
			{
				if (encodingType == UniversalTagNumber.UTF8String)
				{
					return AsnCharacterStringEncodings.s_utf8Encoding;
				}
				if (encodingType == UniversalTagNumber.PrintableString)
				{
					return AsnCharacterStringEncodings.s_printableStringEncoding;
				}
			}
			else
			{
				if (encodingType == UniversalTagNumber.IA5String)
				{
					return AsnCharacterStringEncodings.s_ia5Encoding;
				}
				if (encodingType == UniversalTagNumber.VisibleString)
				{
					return AsnCharacterStringEncodings.s_visibleStringEncoding;
				}
				if (encodingType == UniversalTagNumber.BMPString)
				{
					return AsnCharacterStringEncodings.s_bmpEncoding;
				}
			}
			throw new ArgumentOutOfRangeException("encodingType", encodingType, null);
		}

		private static readonly Encoding s_utf8Encoding = new UTF8Encoding(false, true);

		private static readonly Encoding s_bmpEncoding = new BMPEncoding();

		private static readonly Encoding s_ia5Encoding = new IA5Encoding();

		private static readonly Encoding s_visibleStringEncoding = new VisibleStringEncoding();

		private static readonly Encoding s_printableStringEncoding = new PrintableStringEncoding();
	}
}
