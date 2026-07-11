using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapYearMonth : ISoapXsd
	{
		public SoapYearMonth()
		{
		}

		public SoapYearMonth(DateTime value)
		{
			this._value = value;
		}

		public SoapYearMonth(DateTime value, int sign)
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
				return "gYearMonth";
			}
		}

		public string GetXsdType()
		{
			return SoapYearMonth.XsdType;
		}

		public static SoapYearMonth Parse(string value)
		{
			SoapYearMonth soapYearMonth = new SoapYearMonth(DateTime.ParseExact(value, SoapYearMonth._datetimeFormats, null, DateTimeStyles.None));
			if (value.StartsWith("-"))
			{
				soapYearMonth.Sign = -1;
			}
			else
			{
				soapYearMonth.Sign = 0;
			}
			return soapYearMonth;
		}

		public override string ToString()
		{
			if (this._sign >= 0)
			{
				return this._value.ToString("yyyy-MM", CultureInfo.InvariantCulture);
			}
			return this._value.ToString("'-'yyyy-MM", CultureInfo.InvariantCulture);
		}

		private static readonly string[] _datetimeFormats = new string[] { "yyyy-MM", "'+'yyyy-MM", "'-'yyyy-MM", "yyyy-MMzzz", "'+'yyyy-MMzzz", "'-'yyyy-MMzzz" };

		private int _sign;

		private DateTime _value;
	}
}
