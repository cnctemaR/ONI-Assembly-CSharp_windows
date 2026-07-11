using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapHexBinary : ISoapXsd
	{
		public SoapHexBinary()
		{
		}

		public SoapHexBinary(byte[] value)
		{
			this._value = value;
		}

		public byte[] Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public static string XsdType
		{
			get
			{
				return "hexBinary";
			}
		}

		public string GetXsdType()
		{
			return SoapHexBinary.XsdType;
		}

		public static SoapHexBinary Parse(string value)
		{
			return new SoapHexBinary(SoapHexBinary.FromBinHexString(value));
		}

		internal static byte[] FromBinHexString(string value)
		{
			char[] array = value.ToCharArray();
			byte[] array2 = new byte[array.Length / 2 + array.Length % 2];
			int num = array.Length;
			if (num % 2 != 0)
			{
				throw SoapHexBinary.CreateInvalidValueException(value);
			}
			int num2 = 0;
			for (int i = 0; i < num - 1; i += 2)
			{
				array2[num2] = SoapHexBinary.FromHex(array[i], value);
				byte[] array3 = array2;
				int num3 = num2;
				array3[num3] = (byte)(array3[num3] << 4);
				byte[] array4 = array2;
				int num4 = num2;
				array4[num4] += SoapHexBinary.FromHex(array[i + 1], value);
				num2++;
			}
			return array2;
		}

		private static byte FromHex(char hexDigit, string value)
		{
			byte b;
			try
			{
				b = byte.Parse(hexDigit.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw SoapHexBinary.CreateInvalidValueException(value);
			}
			return b;
		}

		private static Exception CreateInvalidValueException(string value)
		{
			return new RemotingException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for xsd:{1}.", value, SoapHexBinary.XsdType));
		}

		public override string ToString()
		{
			this.sb.Length = 0;
			foreach (byte b in this._value)
			{
				this.sb.Append(b.ToString("X2"));
			}
			return this.sb.ToString();
		}

		private byte[] _value;

		private StringBuilder sb = new StringBuilder();
	}
}
