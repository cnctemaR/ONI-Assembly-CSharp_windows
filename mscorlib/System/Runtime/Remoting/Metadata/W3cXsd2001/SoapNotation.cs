using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapNotation : ISoapXsd
	{
		public SoapNotation()
		{
		}

		public SoapNotation(string value)
		{
			this._value = value;
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
				return "NOTATION";
			}
		}

		public string GetXsdType()
		{
			return SoapNotation.XsdType;
		}

		public static SoapNotation Parse(string value)
		{
			return new SoapNotation(value);
		}

		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
