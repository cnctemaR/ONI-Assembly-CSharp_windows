using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapNcName : ISoapXsd
	{
		public SoapNcName()
		{
		}

		public SoapNcName(string value)
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
				return "NCName";
			}
		}

		public string GetXsdType()
		{
			return SoapNcName.XsdType;
		}

		public static SoapNcName Parse(string value)
		{
			return new SoapNcName(value);
		}

		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
