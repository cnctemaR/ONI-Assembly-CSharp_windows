using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Text
{
	internal struct SurrogateChar
	{
		public SurrogateChar(int ch)
		{
			if (ch < 65536 || ch > 1114111)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Surrogate char '0x{0}' not valid. Surrogate chars range from 0x10000 to 0x10FFFF.", new object[] { ch.ToString("X", CultureInfo.InvariantCulture) }), "ch"));
			}
			this.lowChar = (char)(((ch - 65536) & 1023) + 56320);
			this.highChar = (char)(((ch - 65536 >> 10) & 1023) + 55296);
		}

		public SurrogateChar(char lowChar, char highChar)
		{
			if (lowChar < '\udc00' || lowChar > '\udfff')
			{
				string text = "Low surrogate char '0x{0}' not valid. Low surrogate chars range from 0xDC00 to 0xDFFF.";
				object[] array = new object[1];
				int num = 0;
				int num2 = (int)lowChar;
				array[num] = num2.ToString("X", CultureInfo.InvariantCulture);
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString(text, array), "lowChar"));
			}
			if (highChar < '\ud800' || highChar > '\udbff')
			{
				string text2 = "High surrogate char '0x{0}' not valid. High surrogate chars range from 0xD800 to 0xDBFF.";
				object[] array2 = new object[1];
				int num3 = 0;
				int num2 = (int)highChar;
				array2[num3] = num2.ToString("X", CultureInfo.InvariantCulture);
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString(text2, array2), "highChar"));
			}
			this.lowChar = lowChar;
			this.highChar = highChar;
		}

		public char LowChar
		{
			get
			{
				return this.lowChar;
			}
		}

		public char HighChar
		{
			get
			{
				return this.highChar;
			}
		}

		public int Char
		{
			get
			{
				return (int)(this.lowChar - '\udc00') | ((int)((int)(this.highChar - '\ud800') << 10) + 65536);
			}
		}

		private char lowChar;

		private char highChar;

		public const int MinValue = 65536;

		public const int MaxValue = 1114111;

		private const char surHighMin = '\ud800';

		private const char surHighMax = '\udbff';

		private const char surLowMin = '\udc00';

		private const char surLowMax = '\udfff';
	}
}
