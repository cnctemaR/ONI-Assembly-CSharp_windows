using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapToken : ISoapXsd
	{
		public SoapToken()
		{
		}

		public SoapToken(string value)
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
				return "token";
			}
		}

		public string GetXsdType()
		{
			return SoapToken.XsdType;
		}

		public static SoapToken Parse(string value)
		{
			return new SoapToken(value);
		}

		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
