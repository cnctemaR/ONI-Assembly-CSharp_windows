using System;

namespace System.Xml.Schema
{
	internal sealed class XmlSchemaCollectionNode
	{
		internal string NamespaceURI
		{
			get
			{
				return this.namespaceUri;
			}
			set
			{
				this.namespaceUri = value;
			}
		}

		internal SchemaInfo SchemaInfo
		{
			get
			{
				return this.schemaInfo;
			}
			set
			{
				this.schemaInfo = value;
			}
		}

		internal XmlSchema Schema
		{
			get
			{
				return this.schema;
			}
			set
			{
				this.schema = value;
			}
		}

		private string namespaceUri;

		private SchemaInfo schemaInfo;

		private XmlSchema schema;
	}
}
