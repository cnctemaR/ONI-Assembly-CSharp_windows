using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapNmtokens : ISoapXsd
	{
		public SoapNmtokens()
		{
		}

		public SoapNmtokens(string value)
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
				return "NMTOKENS";
			}
		}

		public string GetXsdType()
		{
			return SoapNmtokens.XsdType;
		}

		public static SoapNmtokens Parse(string value)
		{
			return new SoapNmtokens(value);
		}

		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
