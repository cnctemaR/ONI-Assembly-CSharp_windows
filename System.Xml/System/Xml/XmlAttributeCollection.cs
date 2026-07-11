using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Mono.Xml;

namespace System.Xml
{
	public sealed class XmlAttributeCollection : XmlNamedNodeMap, IEnumerable, ICollection
	{
		internal XmlAttributeCollection(XmlNode parent)
			: base(parent)
		{
			this.ownerElement = parent as XmlElement;
			this.ownerDocument = parent.OwnerDocument;
			if (this.ownerElement == null)
			{
				throw new XmlException("invalid construction for XmlAttributeCollection.");
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			array.CopyTo(base.Nodes.ToArray(typeof(XmlAttribute)), index);
		}

		private bool IsReadOnly
		{
			get
			{
				return this.ownerElement.IsReadOnly;
			}
		}

		[IndexerName("ItemOf")]
		public XmlAttribute this[string name]
		{
			get
			{
				return (XmlAttribute)this.GetNamedItem(name);
			}
		}

		[IndexerName("ItemOf")]
		public XmlAttribute this[int i]
		{
			get
			{
				return (XmlAttribute)base.Nodes[i];
			}
		}

		[IndexerName("ItemOf")]
		public XmlAttribute this[string localName, string namespaceURI]
		{
			get
			{
				return (XmlAttribute)this.GetNamedItem(localName, namespaceURI);
			}
		}

		public XmlAttribute Append(XmlAttribute node)
		{
			this.SetNamedItem(node);
			return node;
		}

		public void CopyTo(XmlAttribute[] array, int index)
		{
			for (int i = 0; i < this.Count; i++)
			{
				array[index + i] = base.Nodes[i] as XmlAttribute;
			}
		}

		public XmlAttribute InsertAfter(XmlAttribute newNode, XmlAttribute refNode)
		{
			if (refNode != null)
			{
				for (int i = 0; i < this.Count; i++)
				{
					if (refNode == base.Nodes[i])
					{
						return this.InsertBefore(newNode, (this.Count != i + 1) ? this[i + 1] : null);
					}
				}
				throw new ArgumentException("refNode not found in this collection.");
			}
			if (this.Count == 0)
			{
				return this.InsertBefore(newNode, null);
			}
			return this.InsertBefore(newNode, this[0]);
		}

		public XmlAttribute InsertBefore(XmlAttribute newNode, XmlAttribute refNode)
		{
			if (newNode.OwnerDocument != this.ownerDocument)
			{
				throw new ArgumentException("different document created this newNode.");
			}
			this.ownerDocument.onNodeInserting(newNode, null);
			int num = this.Count;
			if (refNode != null)
			{
				for (int i = 0; i < this.Count; i++)
				{
					XmlNode xmlNode = base.Nodes[i] as XmlNode;
					if (xmlNode == refNode)
					{
						num = i;
						break;
					}
				}
				if (num == this.Count)
				{
					throw new ArgumentException("refNode not found in this collection.");
				}
			}
			base.SetNamedItem(newNode, num, false);
			this.ownerDocument.onNodeInserted(newNode, null);
			return newNode;
		}

		public XmlAttribute Prepend(XmlAttribute node)
		{
			return this.InsertAfter(node, null);
		}

		public XmlAttribute Remove(XmlAttribute node)
		{
			if (this.IsReadOnly)
			{
				throw new ArgumentException("This attribute collection is read-only.");
			}
			if (node == null)
			{
				throw new ArgumentException("Specified node is null.");
			}
			if (node.OwnerDocument != this.ownerDocument)
			{
				throw new ArgumentException("Specified node is in a different document.");
			}
			if (node.OwnerElement != this.ownerElement)
			{
				throw new ArgumentException("The specified attribute is not contained in the element.");
			}
			XmlAttribute xmlAttribute = null;
			for (int i = 0; i < this.Count; i++)
			{
				XmlAttribute xmlAttribute2 = (XmlAttribute)base.Nodes[i];
				if (xmlAttribute2 == node)
				{
					xmlAttribute = xmlAttribute2;
					break;
				}
			}
			if (xmlAttribute != null)
			{
				this.ownerDocument.onNodeRemoving(node, this.ownerElement);
				base.RemoveNamedItem(xmlAttribute.LocalName, xmlAttribute.NamespaceURI);
				this.RemoveIdenticalAttribute(xmlAttribute);
				this.ownerDocument.onNodeRemoved(node, this.ownerElement);
			}
			DTDAttributeDefinition attributeDefinition = xmlAttribute.GetAttributeDefinition();
			if (attributeDefinition != null && attributeDefinition.DefaultValue != null)
			{
				XmlAttribute xmlAttribute3 = this.ownerDocument.CreateAttribute(xmlAttribute.Prefix, xmlAttribute.LocalName, xmlAttribute.NamespaceURI, true, false);
				xmlAttribute3.Value = attributeDefinition.DefaultValue;
				xmlAttribute3.SetDefault();
				this.SetNamedItem(xmlAttribute3);
			}
			xmlAttribute.AttributeOwnerElement = null;
			return xmlAttribute;
		}

		public void RemoveAll()
		{
			int i = 0;
			while (i < this.Count)
			{
				XmlAttribute xmlAttribute = this[i];
				if (!xmlAttribute.Specified)
				{
					i++;
				}
				this.Remove(xmlAttribute);
			}
		}

		public XmlAttribute RemoveAt(int i)
		{
			if (this.Count <= i)
			{
				return null;
			}
			return this.Remove((XmlAttribute)base.Nodes[i]);
		}

		public override XmlNode SetNamedItem(XmlNode node)
		{
			if (this.IsReadOnly)
			{
				throw new ArgumentException("this AttributeCollection is read only.");
			}
			XmlAttribute xmlAttribute = node as XmlAttribute;
			if (xmlAttribute.OwnerElement == this.ownerElement)
			{
				return node;
			}
			if (xmlAttribute.OwnerElement != null)
			{
				throw new ArgumentException("This attribute is already set to another element.");
			}
			this.ownerElement.OwnerDocument.onNodeInserting(node, this.ownerElement);
			xmlAttribute.AttributeOwnerElement = this.ownerElement;
			XmlNode xmlNode = base.SetNamedItem(node, -1, false);
			this.AdjustIdenticalAttributes(node as XmlAttribute, (xmlNode != node) ? xmlNode : null);
			this.ownerElement.OwnerDocument.onNodeInserted(node, this.ownerElement);
			return xmlNode as XmlAttribute;
		}

		internal void AddIdenticalAttribute()
		{
			this.SetIdenticalAttribute(false);
		}

		internal void RemoveIdenticalAttribute()
		{
			this.SetIdenticalAttribute(true);
		}

		private void SetIdenticalAttribute(bool remove)
		{
			if (this.ownerElement == null)
			{
				return;
			}
			XmlDocumentType documentType = this.ownerDocument.DocumentType;
			if (documentType == null || documentType.DTD == null)
			{
				return;
			}
			DTDElementDeclaration dtdelementDeclaration = documentType.DTD.ElementDecls[this.ownerElement.Name];
			for (int i = 0; i < this.Count; i++)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)base.Nodes[i];
				DTDAttributeDefinition dtdattributeDefinition = ((dtdelementDeclaration != null) ? dtdelementDeclaration.Attributes[xmlAttribute.Name] : null);
				if (dtdattributeDefinition != null && dtdattributeDefinition.Datatype.TokenizedType == XmlTokenizedType.ID)
				{
					if (remove)
					{
						if (this.ownerDocument.GetIdenticalAttribute(xmlAttribute.Value) != null)
						{
							this.ownerDocument.RemoveIdenticalAttribute(xmlAttribute.Value);
							return;
						}
					}
					else
					{
						if (this.ownerDocument.GetIdenticalAttribute(xmlAttribute.Value) != null)
						{
							throw new XmlException(string.Format("ID value {0} already exists in this document.", xmlAttribute.Value));
						}
						this.ownerDocument.AddIdenticalAttribute(xmlAttribute);
						return;
					}
				}
			}
		}

		private void AdjustIdenticalAttributes(XmlAttribute node, XmlNode existing)
		{
			if (this.ownerElement == null)
			{
				return;
			}
			if (existing != null)
			{
				this.RemoveIdenticalAttribute(existing);
			}
			XmlDocumentType documentType = node.OwnerDocument.DocumentType;
			if (documentType == null || documentType.DTD == null)
			{
				return;
			}
			DTDAttListDeclaration dtdattListDeclaration = documentType.DTD.AttListDecls[this.ownerElement.Name];
			DTDAttributeDefinition dtdattributeDefinition = ((dtdattListDeclaration != null) ? dtdattListDeclaration.Get(node.Name) : null);
			if (dtdattributeDefinition == null || dtdattributeDefinition.Datatype.TokenizedType != XmlTokenizedType.ID)
			{
				return;
			}
			this.ownerDocument.AddIdenticalAttribute(node);
		}

		private XmlNode RemoveIdenticalAttribute(XmlNode existing)
		{
			if (this.ownerElement == null)
			{
				return existing;
			}
			if (existing != null && this.ownerDocument.GetIdenticalAttribute(existing.Value) != null)
			{
				this.ownerDocument.RemoveIdenticalAttribute(existing.Value);
			}
			return existing;
		}

		private XmlElement ownerElement;

		private XmlDocument ownerDocument;
	}
}
