using System;
using System.Xml.Schema;

namespace System.Xml
{
	internal class XmlName : IXmlSchemaInfo
	{
		public static XmlName Create(string prefix, string localName, string ns, int hashCode, XmlDocument ownerDoc, XmlName next, IXmlSchemaInfo schemaInfo)
		{
			if (schemaInfo == null)
			{
				return new XmlName(prefix, localName, ns, hashCode, ownerDoc, next);
			}
			return new XmlNameEx(prefix, localName, ns, hashCode, ownerDoc, next, schemaInfo);
		}

		internal XmlName(string prefix, string localName, string ns, int hashCode, XmlDocument ownerDoc, XmlName next)
		{
			this.prefix = prefix;
			this.localName = localName;
			this.ns = ns;
			this.name = null;
			this.hashCode = hashCode;
			this.ownerDoc = ownerDoc;
			this.next = next;
		}

		public string LocalName
		{
			get
			{
				return this.localName;
			}
		}

		public string NamespaceURI
		{
			get
			{
				return this.ns;
			}
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		public int HashCode
		{
			get
			{
				return this.hashCode;
			}
		}

		public XmlDocument OwnerDocument
		{
			get
			{
				return this.ownerDoc;
			}
		}

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					if (this.prefix.Length > 0)
					{
						if (this.localName.Length > 0)
						{
							string text = this.prefix + ":" + this.localName;
							XmlNameTable nameTable = this.ownerDoc.NameTable;
							lock (nameTable)
							{
								if (this.name == null)
								{
									this.name = this.ownerDoc.NameTable.Add(text);
								}
								goto IL_0099;
							}
						}
						this.name = this.prefix;
					}
					else
					{
						this.name = this.localName;
					}
				}
				IL_0099:
				return this.name;
			}
		}

		public virtual XmlSchemaValidity Validity
		{
			get
			{
				return XmlSchemaValidity.NotKnown;
			}
		}

		public virtual bool IsDefault
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNil
		{
			get
			{
				return false;
			}
		}

		public virtual XmlSchemaSimpleType MemberType
		{
			get
			{
				return null;
			}
		}

		public virtual XmlSchemaType SchemaType
		{
			get
			{
				return null;
			}
		}

		public virtual XmlSchemaElement SchemaElement
		{
			get
			{
				return null;
			}
		}

		public virtual XmlSchemaAttribute SchemaAttribute
		{
			get
			{
				return null;
			}
		}

		public virtual bool Equals(IXmlSchemaInfo schemaInfo)
		{
			return schemaInfo == null;
		}

		public static int GetHashCode(string name)
		{
			int num = 0;
			if (name != null)
			{
				for (int i = name.Length - 1; i >= 0; i--)
				{
					char c = name[i];
					if (c == ':')
					{
						break;
					}
					num += (num << 7) ^ (int)c;
				}
				num -= num >> 17;
				num -= num >> 11;
				num -= num >> 5;
			}
			return num;
		}

		private string prefix;

		private string localName;

		private string ns;

		private string name;

		private int hashCode;

		internal XmlDocument ownerDoc;

		internal XmlName next;
	}
}
