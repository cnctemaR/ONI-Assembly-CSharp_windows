using System;

namespace System.Xml
{
	internal class XmlChar
	{
		public static bool IsWhitespace(int ch)
		{
			return ch == 32 || ch == 9 || ch == 13 || ch == 10;
		}

		public static bool IsWhitespace(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!XmlChar.IsWhitespace((int)str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static int IndexOfNonWhitespace(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!XmlChar.IsWhitespace((int)str[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool IsFirstNameChar(int ch)
		{
			return (ch >= 97 && ch <= 122) || (ch >= 65 && ch <= 90) || (ch <= 65535 && ((ulong)XmlChar.nameBitmap[((int)XmlChar.firstNamePages[ch >> 8] << 3) + ((ch & 255) >> 5)] & (ulong)(1L << (ch & 31 & 31))) != 0UL);
		}

		public static bool IsValid(int ch)
		{
			return !XmlChar.IsInvalid(ch);
		}

		public static bool IsInvalid(int ch)
		{
			switch (ch)
			{
			case 9:
			case 10:
			case 13:
				return false;
			}
			return ch < 32 || (ch >= 55296 && (ch < 57344 || (ch >= 65534 && (ch < 65536 || ch >= 1114112))));
		}

		public static int IndexOfInvalid(string s, bool allowSurrogate)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (XmlChar.IsInvalid((int)s[i]))
				{
					if (!allowSurrogate || i + 1 == s.Length || s[i] < '\ud800' || s[i] >= '\udc00' || s[i + 1] < '\udc00' || s[i + 1] >= '\ue000')
					{
						return i;
					}
					i++;
				}
			}
			return -1;
		}

		public static int IndexOfInvalid(char[] s, int start, int length, bool allowSurrogate)
		{
			int num = start + length;
			if (s.Length < num)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			for (int i = start; i < num; i++)
			{
				if (XmlChar.IsInvalid((int)s[i]))
				{
					if (!allowSurrogate || i + 1 == num || s[i] < '\ud800' || s[i] >= '\udc00' || s[i + 1] < '\udc00' || s[i + 1] >= '\ue000')
					{
						return i;
					}
					i++;
				}
			}
			return -1;
		}

		public static bool IsNameChar(int ch)
		{
			return (ch >= 97 && ch <= 122) || (ch >= 65 && ch <= 90) || (ch <= 65535 && ((ulong)XmlChar.nameBitmap[((int)XmlChar.namePages[ch >> 8] << 3) + ((ch & 255) >> 5)] & (ulong)(1L << (ch & 31 & 31))) != 0UL);
		}

		public static bool IsNCNameChar(int ch)
		{
			bool flag = false;
			if (ch >= 0 && ch <= 65535 && ch != 58)
			{
				flag = ((ulong)XmlChar.nameBitmap[((int)XmlChar.namePages[ch >> 8] << 3) + ((ch & 255) >> 5)] & (ulong)(1L << (ch & 31 & 31))) != 0UL;
			}
			return flag;
		}

		public static bool IsName(string str)
		{
			if (str.Length == 0)
			{
				return false;
			}
			if (!XmlChar.IsFirstNameChar((int)str[0]))
			{
				return false;
			}
			for (int i = 1; i < str.Length; i++)
			{
				if (!XmlChar.IsNameChar((int)str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNCName(string str)
		{
			if (str.Length == 0)
			{
				return false;
			}
			if (!XmlChar.IsFirstNameChar((int)str[0]))
			{
				return false;
			}
			for (int i = 0; i < str.Length; i++)
			{
				if (!XmlChar.IsNCNameChar((int)str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNmToken(string str)
		{
			if (str.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < str.Length; i++)
			{
				if (!XmlChar.IsNameChar((int)str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsPubidChar(int ch)
		{
			return (XmlChar.IsWhitespace(ch) && ch != 9) | (97 <= ch && ch <= 122) | (65 <= ch && ch <= 90) | (48 <= ch && ch <= 57) | ("-'()+,./:=?;!*#@$_%".IndexOf((char)ch) >= 0);
		}

		public static bool IsPubid(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!XmlChar.IsPubidChar((int)str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsValidIANAEncoding(string ianaEncoding)
		{
			if (ianaEncoding != null)
			{
				int length = ianaEncoding.Length;
				if (length > 0)
				{
					char c = ianaEncoding[0];
					if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
					{
						for (int i = 1; i < length; i++)
						{
							c = ianaEncoding[i];
							if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && (c < '0' || c > '9') && c != '.' && c != '_' && c != '-')
							{
								return false;
							}
						}
						return true;
					}
				}
			}
			return false;
		}

		public static int GetPredefinedEntity(string name)
		{
			switch (name)
			{
			case "amp":
				return 38;
			case "lt":
				return 60;
			case "gt":
				return 62;
			case "quot":
				return 34;
			case "apos":
				return 39;
			}
			return -1;
		}

		public static readonly char[] WhitespaceChars = new char[] { ' ', '\n', '\t', '\r' };

		private static readonly byte[] firstNamePages = new byte[]
		{
			2, 3, 4, 5, 6, 7, 8, 0, 0, 9,
			10, 11, 12, 13, 14, 15, 16, 17, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			18, 19, 0, 20, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 21, 22,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 23,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 24, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0
		};

		private static readonly byte[] namePages = new byte[]
		{
			25, 3, 26, 27, 28, 29, 30, 0, 0, 31,
			32, 33, 34, 35, 36, 37, 16, 17, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			18, 19, 38, 20, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 39, 22,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 23,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 24, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0
		};

		private static readonly uint[] nameBitmap = new uint[]
		{
			0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, uint.MaxValue, uint.MaxValue,
			uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, 0U, 67108864U, 2281701374U, 134217726U,
			0U, 0U, 4286578687U, 4286578687U, uint.MaxValue, 2146697215U, 4294966782U, 2147483647U, uint.MaxValue, uint.MaxValue,
			4294959119U, 4231135231U, 16777215U, 0U, 4294901760U, uint.MaxValue, uint.MaxValue, 4160750079U, 3U, 0U,
			0U, 0U, 0U, 0U, 4294956864U, 4294967291U, 1417641983U, 1048573U, 4294959102U, uint.MaxValue,
			3758030847U, uint.MaxValue, 4294901763U, uint.MaxValue, 4294908319U, 54513663U, 0U, 4294836224U, 41943039U, 4294967294U,
			127U, 0U, 4294901760U, 460799U, 0U, 134217726U, 2046U, 4294836224U, uint.MaxValue, 2097151999U,
			3112959U, 96U, 4294967264U, 603979775U, 4278190080U, 3U, 4294549472U, 63307263U, 2952790016U, 196611U,
			4294543328U, 57540095U, 1577058304U, 1835008U, 4294684640U, 602799615U, 0U, 1U, 4294549472U, 600702463U,
			2952790016U, 3U, 3594373088U, 62899992U, 0U, 0U, 4294828000U, 66059775U, 0U, 3U,
			4294828000U, 66059775U, 1073741824U, 3U, 4294828000U, 67108351U, 0U, 3U, 0U, 0U,
			0U, 0U, 4294967294U, 884735U, 63U, 0U, 4277151126U, 537750702U, 31U, 0U,
			0U, 0U, 4294967039U, 1023U, 0U, 0U, 0U, 0U, 0U, 0U,
			0U, 0U, 0U, uint.MaxValue, 4294901823U, 8388607U, 514797U, 1342177280U, 2184269825U, 2908843U,
			1073741824U, 4118857984U, 7U, 33622016U, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, 268435455U, uint.MaxValue,
			uint.MaxValue, 67108863U, 1061158911U, uint.MaxValue, 2868854591U, 1073741823U, uint.MaxValue, 1608515583U, 265232348U, 534519807U,
			0U, 19520U, 0U, 0U, 7U, 0U, 0U, 0U, 128U, 1022U,
			4294967294U, uint.MaxValue, 2097151U, 4294967294U, uint.MaxValue, 134217727U, 4294967264U, 8191U, 0U, 0U,
			0U, 0U, 0U, 0U, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, 63U,
			0U, 0U, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, 15U, 0U, 0U,
			0U, 134176768U, 2281701374U, 134217726U, 0U, 8388608U, 4286578687U, 4286578687U, 16777215U, 0U,
			4294901760U, uint.MaxValue, uint.MaxValue, 4160750079U, 196611U, 0U, uint.MaxValue, uint.MaxValue, 63U, 3U,
			4294956992U, 4294967291U, 1417641983U, 1048573U, 4294959102U, uint.MaxValue, 3758030847U, uint.MaxValue, 4294901883U, uint.MaxValue,
			4294908319U, 54513663U, 0U, 4294836224U, 41943039U, 4294967294U, 4294836351U, 3154116603U, 4294901782U, 460799U,
			0U, 134217726U, 524287U, 4294902783U, uint.MaxValue, 2097151999U, 4293885951U, 67059199U, 4294967278U, 4093640703U,
			4280172543U, 65487U, 4294549486U, 3552968191U, 2961193375U, 262095U, 4294543332U, 3547201023U, 1577073031U, 2097088U,
			4294684654U, 4092460543U, 15295U, 65473U, 4294549486U, 4090363391U, 2965387663U, 65475U, 3594373100U, 3284125464U,
			8404423U, 65408U, 4294828014U, 3287285247U, 6307295U, 65475U, 4294828012U, 3287285247U, 1080049119U, 65475U,
			4294828012U, 3288333823U, 8404431U, 65475U, 0U, 0U, 0U, 0U, 4294967294U, 134184959U,
			67076095U, 0U, 4277151126U, 1006595246U, 67059551U, 0U, 50331648U, 3265266687U, 4294967039U, 4294837247U,
			4273934303U, 50216959U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U,
			536805376U, 2U, 160U, 4128766U, 4294967294U, uint.MaxValue, 1713373183U, 4294967294U, uint.MaxValue, 2013265919U
		};
	}
}
