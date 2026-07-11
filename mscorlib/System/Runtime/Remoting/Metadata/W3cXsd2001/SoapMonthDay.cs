using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapMonthDay : ISoapXsd
	{
		public SoapMonthDay()
		{
		}

		public SoapMonthDay(DateTime value)
		{
			this._value = value;
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
				return "gMonthDay";
			}
		}

		public string GetXsdType()
		{
			return SoapMonthDay.XsdType;
		}

		public static SoapMonthDay Parse(string value)
		{
			return new SoapMonthDay(DateTime.ParseExact(value, SoapMonthDay._datetimeFormats, null, DateTimeStyles.None));
		}

		public override string ToString()
		{
			return this._value.ToString("--MM-dd", CultureInfo.InvariantCulture);
		}

		private static readonly string[] _datetimeFormats = new string[] { "--MM-dd", "--MM-ddzzz" };

		private DateTime _value;
	}
}
