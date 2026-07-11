using System;
using System.Xml.Schema;

namespace System.Xml.XPath
{
	internal class XPathNavigatorReaderWithSI : XPathNavigatorReader, IXmlSchemaInfo
	{
		internal XPathNavigatorReaderWithSI(XPathNavigator navToRead, IXmlLineInfo xli, IXmlSchemaInfo xsi)
			: base(navToRead, xli, xsi)
		{
		}

		public virtual XmlSchemaValidity Validity
		{
			get
			{
				if (!base.IsReading)
				{
					return XmlSchemaValidity.NotKnown;
				}
				return this.schemaInfo.Validity;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return base.IsReading && this.schemaInfo.IsDefault;
			}
		}

		public virtual bool IsNil
		{
			get
			{
				return base.IsReading && this.schemaInfo.IsNil;
			}
		}

		public virtual XmlSchemaSimpleType MemberType
		{
			get
			{
				if (!base.IsReading)
				{
					return null;
				}
				return this.schemaInfo.MemberType;
			}
		}

		public virtual XmlSchemaType SchemaType
		{
			get
			{
				if (!base.IsReading)
				{
					return null;
				}
				return this.schemaInfo.SchemaType;
			}
		}

		public virtual XmlSchemaElement SchemaElement
		{
			get
			{
				if (!base.IsReading)
				{
					return null;
				}
				return this.schemaInfo.SchemaElement;
			}
		}

		public virtual XmlSchemaAttribute SchemaAttribute
		{
			get
			{
				if (!base.IsReading)
				{
					return null;
				}
				return this.schemaInfo.SchemaAttribute;
			}
		}
	}
}
