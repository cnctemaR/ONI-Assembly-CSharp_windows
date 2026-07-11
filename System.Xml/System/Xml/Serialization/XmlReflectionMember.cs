using System;

namespace System.Xml.Serialization
{
	public class XmlReflectionMember
	{
		public Type MemberType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public XmlAttributes XmlAttributes
		{
			get
			{
				return this.xmlAttributes;
			}
			set
			{
				this.xmlAttributes = value;
			}
		}

		public SoapAttributes SoapAttributes
		{
			get
			{
				return this.soapAttributes;
			}
			set
			{
				this.soapAttributes = value;
			}
		}

		public string MemberName
		{
			get
			{
				if (this.memberName != null)
				{
					return this.memberName;
				}
				return string.Empty;
			}
			set
			{
				this.memberName = value;
			}
		}

		public bool IsReturnValue
		{
			get
			{
				return this.isReturnValue;
			}
			set
			{
				this.isReturnValue = value;
			}
		}

		public bool OverrideIsNullable
		{
			get
			{
				return this.overrideIsNullable;
			}
			set
			{
				this.overrideIsNullable = value;
			}
		}

		private string memberName;

		private Type type;

		private XmlAttributes xmlAttributes = new XmlAttributes();

		private SoapAttributes soapAttributes = new SoapAttributes();

		private bool isReturnValue;

		private bool overrideIsNullable;
	}
}
