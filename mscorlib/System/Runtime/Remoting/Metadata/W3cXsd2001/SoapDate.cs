using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapDate : ISoapXsd
	{
		public SoapDate()
		{
		}

		public SoapDate(DateTime value)
		{
			this._value = value;
		}

		public SoapDate(DateTime value, int sign)
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
				return "date";
			}
		}

		public string GetXsdType()
		{
			return SoapDate.XsdType;
		}

		public static SoapDate Parse(string value)
		{
			DateTime dateTime = DateTime.ParseExact(value, SoapDate._datetimeFormats, null, DateTimeStyles.None);
			SoapDate soapDate = new SoapDate(dateTime);
			if (value.StartsWith("-"))
			{
				soapDate.Sign = -1;
			}
			else
			{
				soapDate.Sign = 0;
			}
			return soapDate;
		}

		public override string ToString()
		{
			if (this._sign >= 0)
			{
				return this._value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			}
			return this._value.ToString("'-'yyyy-MM-dd", CultureInfo.InvariantCulture);
		}

		private static readonly string[] _datetimeFormats = new string[] { "yyyy-MM-dd", "'+'yyyy-MM-dd", "'-'yyyy-MM-dd", "yyyy-MM-ddzzz", "'+'yyyy-MM-ddzzz", "'-'yyyy-MM-ddzzz" };

		private int _sign;

		private DateTime _value;
	}
}
