using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapBase64Binary : ISoapXsd
	{
		public SoapBase64Binary()
		{
		}

		public SoapBase64Binary(byte[] value)
		{
			this._value = value;
		}

		public byte[] Value
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
				return "base64Binary";
			}
		}

		public string GetXsdType()
		{
			return SoapBase64Binary.XsdType;
		}

		public static SoapBase64Binary Parse(string value)
		{
			return new SoapBase64Binary(Convert.FromBase64String(value));
		}

		public override string ToString()
		{
			return Convert.ToBase64String(this._value);
		}

		private byte[] _value;
	}
}
