using System;

namespace System.Xml.Schema
{
	internal class XsdSimpleValue
	{
		public XsdSimpleValue(XmlSchemaSimpleType st, object value)
		{
			this.xmlType = st;
			this.typedValue = value;
		}

		public XmlSchemaSimpleType XmlType
		{
			get
			{
				return this.xmlType;
			}
		}

		public object TypedValue
		{
			get
			{
				return this.typedValue;
			}
		}

		private XmlSchemaSimpleType xmlType;

		private object typedValue;
	}
}
