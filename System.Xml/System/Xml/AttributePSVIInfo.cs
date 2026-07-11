using System;
using System.Xml.Schema;

namespace System.Xml
{
	internal class AttributePSVIInfo
	{
		internal AttributePSVIInfo()
		{
			this.attributeSchemaInfo = new XmlSchemaInfo();
		}

		internal void Reset()
		{
			this.typedAttributeValue = null;
			this.localName = string.Empty;
			this.namespaceUri = string.Empty;
			this.attributeSchemaInfo.Clear();
		}

		internal string localName;

		internal string namespaceUri;

		internal object typedAttributeValue;

		internal XmlSchemaInfo attributeSchemaInfo;
	}
}
