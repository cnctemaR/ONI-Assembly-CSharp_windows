using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapLanguage : ISoapXsd
	{
		public SoapLanguage()
		{
		}

		public SoapLanguage(string value)
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
				return "language";
			}
		}

		public string GetXsdType()
		{
			return SoapLanguage.XsdType;
		}

		public static SoapLanguage Parse(string value)
		{
			return new SoapLanguage(value);
		}

		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
