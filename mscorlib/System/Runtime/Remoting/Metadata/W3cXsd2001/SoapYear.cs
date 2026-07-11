using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapYear : ISoapXsd
	{
		public SoapYear()
		{
		}

		public SoapYear(DateTime value)
		{
			this._value = value;
		}

		public SoapYear(DateTime value, int sign)
		{
			this._value = value;
			this._sign = sign;
		}

		public int Sign
		{
			get
			{
				return this._sign;
			}
			set
			{
				this._sign = value;
			}
		}

		public DateTime Value
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
				return "gYear";
			}
		}

		public string GetXsdType()
		{
			return SoapYear.XsdType;
		}

		public static SoapYear Parse(string value)
		{
			DateTime dateTime = DateTime.ParseExact(value, SoapYear._datetimeFormats, null, DateTimeStyles.None);
			SoapYear soapYear = new SoapYear(dateTime);
			if (value.StartsWith("-"))
			{
				soapYear.Sign = -1;
			}
			else
			{
				soapYear.Sign = 0;
			}
			return soapYear;
		}

		public override string ToString()
		{
			if (this._sign >= 0)
			{
				return this._value.ToString("yyyy", CultureInfo.InvariantCulture);
			}
			return this._value.ToString("'-'yyyy", CultureInfo.InvariantCulture);
		}

		private static readonly string[] _datetimeFormats = new string[] { "yyyy", "'+'yyyy", "'-'yyyy", "yyyyzzz", "'+'yyyyzzz", "'-'yyyyzzz" };

		private int _sign;

		private DateTime _value;
	}
}
