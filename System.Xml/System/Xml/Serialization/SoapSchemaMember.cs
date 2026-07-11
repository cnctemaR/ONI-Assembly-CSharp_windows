using System;

namespace System.Xml.Serialization
{
	public class SoapSchemaMember
	{
		public XmlQualifiedName MemberType
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

		private string memberName;

		private XmlQualifiedName type = XmlQualifiedName.Empty;
	}
}
