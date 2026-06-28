using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapEntities : ISoapXsd
	{
		public SoapEntities()
		{
		}

		public SoapEntities(string value)
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
				return "ENTITIES";
			}
		}

		public string GetXsdType()
		{
			return SoapEntities.XsdType;
		}

		public static SoapEntities Parse(string value)
		{
			return new SoapEntities(value);
		}

		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
