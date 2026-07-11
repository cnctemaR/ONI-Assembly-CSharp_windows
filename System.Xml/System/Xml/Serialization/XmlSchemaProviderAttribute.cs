using System;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class XmlSchemaProviderAttribute : Attribute
	{
		public XmlSchemaProviderAttribute(string methodName)
		{
			this.methodName = methodName;
		}

		public string MethodName
		{
			get
			{
				return this.methodName;
			}
		}

		public bool IsAny
		{
			get
			{
				return this.any;
			}
			set
			{
				this.any = value;
			}
		}

		private string methodName;

		private bool any;
	}
}
