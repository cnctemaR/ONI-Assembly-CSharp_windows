using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapIdrefs : ISoapXsd
	{
		public SoapIdrefs()
		{
		}

		public SoapIdrefs(string value)
		{
			this._value = SoapHelper.Normalize(value);
		}

		public string Value
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
				return "IDREFS";
			}
		}

		public string GetXsdType()
		{
			return SoapIdrefs.XsdType;
		}

		public static SoapIdrefs Parse(string value)
		{
			return new SoapIdrefs(value);
		}

		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
