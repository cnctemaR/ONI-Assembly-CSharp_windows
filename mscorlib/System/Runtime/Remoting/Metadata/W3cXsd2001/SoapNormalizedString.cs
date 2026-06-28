using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapNormalizedString : ISoapXsd
	{
		public SoapNormalizedString()
		{
		}

		public SoapNormalizedString(string value)
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
				return "normalizedString";
			}
		}

		public string GetXsdType()
		{
			return SoapNormalizedString.XsdType;
		}

		public static SoapNormalizedString Parse(string value)
		{
			return new SoapNormalizedString(value);
		}

		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
