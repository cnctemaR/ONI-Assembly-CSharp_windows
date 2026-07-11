using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleTypeUnion : XmlSchemaSimpleTypeContent
	{
		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		public XmlSchemaObjectCollection BaseTypes
		{
			get
			{
				return this.baseTypes;
			}
		}

		[XmlAttribute("memberTypes")]
		public XmlQualifiedName[] MemberTypes
		{
			get
			{
				return this.memberTypes;
			}
			set
			{
				this.memberTypes = value;
			}
		}

		[XmlIgnore]
		public XmlSchemaSimpleType[] BaseMemberTypes
		{
			get
			{
				return this.baseMemberTypes;
			}
		}

		internal void SetBaseMemberTypes(XmlSchemaSimpleType[] baseMemberTypes)
		{
			this.baseMemberTypes = baseMemberTypes;
		}

		internal override XmlSchemaObject Clone()
		{
			if (this.memberTypes != null && this.memberTypes.Length != 0)
			{
				XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = (XmlSchemaSimpleTypeUnion)base.MemberwiseClone();
				XmlQualifiedName[] array = new XmlQualifiedName[this.memberTypes.Length];
				for (int i = 0; i < this.memberTypes.Length; i++)
				{
					array[i] = this.memberTypes[i].Clone();
				}
				xmlSchemaSimpleTypeUnion.MemberTypes = array;
				return xmlSchemaSimpleTypeUnion;
			}
			return this;
		}

		private XmlSchemaObjectCollection baseTypes = new XmlSchemaObjectCollection();

		private XmlQualifiedName[] memberTypes;

		private XmlSchemaSimpleType[] baseMemberTypes;
	}
}
