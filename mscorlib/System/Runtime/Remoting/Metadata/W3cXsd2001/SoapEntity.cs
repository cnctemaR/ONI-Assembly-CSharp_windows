using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapEntity : ISoapXsd
	{
		public SoapEntity()
		{
		}

		public SoapEntity(string value)
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
				return "ENTITY";
			}
		}

		public string GetXsdType()
		{
			return SoapEntity.XsdType;
		}

		public static SoapEntity Parse(string value)
		{
			return new SoapEntity(value);
		}

		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
