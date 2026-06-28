using System;

namespace System.Xml.Serialization
{
	public class SoapSchemaMember
	{
		public string MemberName
		{
			get
			{
				if (this.memberName == null)
				{
					return string.Empty;
				}
				return this.memberName;
			}
			set
			{
				this.memberName = value;
			}
		}

		public XmlQualifiedName MemberType
		{
			get
			{
				return this.memberType;
			}
			set
			{
				this.memberType = value;
			}
		}

		private string memberName;

		private XmlQualifiedName memberType = XmlQualifiedName.Empty;
	}
}
