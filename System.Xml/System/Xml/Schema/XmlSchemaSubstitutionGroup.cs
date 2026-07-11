using System;
using System.Collections;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	internal class XmlSchemaSubstitutionGroup : XmlSchemaObject
	{
		[XmlIgnore]
		internal ArrayList Members
		{
			get
			{
				return this.membersList;
			}
		}

		[XmlIgnore]
		internal XmlQualifiedName Examplar
		{
			get
			{
				return this.examplar;
			}
			set
			{
				this.examplar = value;
			}
		}

		private ArrayList membersList = new ArrayList();

		private XmlQualifiedName examplar = XmlQualifiedName.Empty;
	}
}
