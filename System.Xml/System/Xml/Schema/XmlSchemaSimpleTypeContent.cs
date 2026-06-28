using System;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaSimpleTypeContent : XmlSchemaAnnotated
	{
		internal object ActualBaseSchemaType
		{
			get
			{
				return this.OwnerType.BaseSchemaType;
			}
		}

		internal virtual string Normalize(string s, XmlNameTable nt, XmlNamespaceManager nsmgr)
		{
			return s;
		}

		internal XmlSchemaSimpleType OwnerType;
	}
}
