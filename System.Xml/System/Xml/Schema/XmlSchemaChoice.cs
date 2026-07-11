using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaChoice : XmlSchemaGroupBase
	{
		[XmlElement("group", typeof(XmlSchemaGroupRef))]
		[XmlElement("element", typeof(XmlSchemaElement))]
		[XmlElement("any", typeof(XmlSchemaAny))]
		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		public override XmlSchemaObjectCollection Items
		{
			get
			{
				return this.items;
			}
		}

		internal override bool IsEmpty
		{
			get
			{
				return base.IsEmpty;
			}
		}

		internal override void SetItems(XmlSchemaObjectCollection newItems)
		{
			this.items = newItems;
		}

		private XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();
	}
}
