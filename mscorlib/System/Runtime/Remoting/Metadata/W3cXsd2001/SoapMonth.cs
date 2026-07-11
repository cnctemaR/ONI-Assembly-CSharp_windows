using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapMonth : ISoapXsd
	{
		public SoapMonth()
		{
		}

		public SoapMonth(DateTime value)
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
				return "gMonth";
			}
		}

		public string GetXsdType()
		{
			return SoapMonth.XsdType;
		}

		public static SoapMonth Parse(string value)
		{
			return new SoapMonth(DateTime.ParseExact(value, SoapMonth._datetimeFormats, null, DateTimeStyles.None));
		}

		public override string ToString()
		{
			return this._value.ToString("--MM--", CultureInfo.InvariantCulture);
		}

		private static readonly string[] _datetimeFormats = new string[] { "--MM--", "--MM--zzz" };

		private DateTime _value;
	}
}
