using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapQName : ISoapXsd
	{
		public SoapQName()
		{
		}

		public SoapQName(string value)
		{
			this._name = value;
		}

		public SoapQName(string key, string name)
		{
			this._key = key;
			this._name = name;
		}

		public SoapQName(string key, string name, string namespaceValue)
		{
			this._key = key;
			this._name = name;
			this._namespace = namespaceValue;
		}

		public string Key
		{
			get
			{
				return this._key;
			}
			set
			{
				this._key = value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this._namespace;
			}
			set
			{
				this._namespace = value;
			}
		}

		public static string XsdType
		{
			get
			{
				return "QName";
			}
		}

		public string GetXsdType()
		{
			return SoapQName.XsdType;
		}

		public static SoapQName Parse(string value)
		{
			SoapQName soapQName = new SoapQName();
			int num = value.IndexOf(':');
			if (num != -1)
			{
				soapQName.Key = value.Substring(0, num);
				soapQName.Name = value.Substring(num + 1);
			}
			else
			{
				soapQName.Name = value;
			}
			return soapQName;
		}

		public override string ToString()
		{
			if (this._key == null || this._key == string.Empty)
			{
				return this._name;
			}
			return this._key + ":" + this._name;
		}

		private string _name;

		private string _key;

		private string _namespace;
	}
}
