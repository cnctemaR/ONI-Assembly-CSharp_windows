using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	internal class XmlSchemaSubstitutionGroupV1Compat : XmlSchemaSubstitutionGroup
	{
		[XmlIgnore]
		internal XmlSchemaChoice Choice
		{
			get
			{
				return this.choice;
			}
		}

		private XmlSchemaChoice choice = new XmlSchemaChoice();
	}
}
