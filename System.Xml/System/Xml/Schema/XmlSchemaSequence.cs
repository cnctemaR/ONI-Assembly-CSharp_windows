using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSequence : XmlSchemaGroupBase
	{
		[XmlElement("element", typeof(XmlSchemaElement))]
		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("any", typeof(XmlSchemaAny))]
		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		[XmlElement("group", typeof(XmlSchemaGroupRef))]
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
				return base.IsEmpty || this.items.Count == 0;
			}
		}

		internal override void SetItems(XmlSchemaObjectCollection newItems)
		{
			this.items = newItems;
		}

		private XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();
	}
}
