using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapDay : ISoapXsd
	{
		public SoapDay()
		{
		}

		public SoapDay(DateTime value)
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
				return "gDay";
			}
		}

		public string GetXsdType()
		{
			return SoapDay.XsdType;
		}

		public static SoapDay Parse(string value)
		{
			DateTime dateTime = DateTime.ParseExact(value, SoapDay._datetimeFormats, null, DateTimeStyles.None);
			return new SoapDay(dateTime);
		}

		public override string ToString()
		{
			return this._value.ToString("---dd", CultureInfo.InvariantCulture);
		}

		private static readonly string[] _datetimeFormats = new string[] { "---dd", "---ddzzz" };

		private DateTime _value;
	}
}
