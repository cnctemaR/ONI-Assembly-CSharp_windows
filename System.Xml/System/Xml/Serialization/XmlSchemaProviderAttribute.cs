using System;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class XmlSchemaProviderAttribute : Attribute
	{
		public XmlSchemaProviderAttribute(string methodName)
		{
			this._methodName = methodName;
		}

		public string MethodName
		{
			get
			{
				return this._methodName;
			}
		}

		public bool IsAny
		{
			get
			{
				return this._isAny;
			}
			set
			{
				this._isAny = value;
			}
		}

		private string _methodName;

		private bool _isAny;
	}
}
