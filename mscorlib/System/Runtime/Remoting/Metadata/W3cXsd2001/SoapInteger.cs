using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapInteger : ISoapXsd
	{
		public SoapInteger()
		{
		}

		public SoapInteger(decimal value)
		{
			this._value = value;
		}

		public decimal Value
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
				return "integer";
			}
		}

		public string GetXsdType()
		{
			return SoapInteger.XsdType;
		}

		public static SoapInteger Parse(string value)
		{
			return new SoapInteger(decimal.Parse(value));
		}

		public override string ToString()
		{
			return this._value.ToString();
		}

		private decimal _value;
	}
}
