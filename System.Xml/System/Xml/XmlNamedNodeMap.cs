using System;
using System.Collections;
using Mono.Xml;

namespace System.Xml
{
	public class XmlNamedNodeMap : IEnumerable
	{
		internal XmlNamedNodeMap(XmlNode parent)
		{
			this.parent = parent;
		}

		private ArrayList NodeList
		{
			get
			{
				if (this.nodeList == null)
				{
					this.nodeList = new ArrayList();
				}
				return this.nodeList;
			}
		}

		public virtual int Count
		{
			get
			{
				return (this.nodeList != null) ? this.nodeList.Count : 0;
			}
		}

		public virtual IEnumerator GetEnumerator()
		{
			if (this.nodeList == null)
			{
				return XmlNamedNodeMap.emptyEnumerator;
			}
			return this.nodeList.GetEnumerator();
		}

		public virtual XmlNode GetNamedItem(string name)
		{
			if (this.nodeList == null)
			{
				return null;
			}
			for (int i = 0; i < this.nodeList.Count; i++)
			{
				XmlNode xmlNode = (XmlNode)this.nodeList[i];
				if (xmlNode.Name == name)
				{
					return xmlNode;
				}
			}
			return null;
		}

		public virtual XmlNode GetNamedItem(string localName, string namespaceURI)
		{
			if (this.nodeList == null)
			{
				return null;
			}
			for (int i = 0; i < this.nodeList.Count; i++)
			{
				XmlNode xmlNode = (XmlNode)this.nodeList[i];
				if (xmlNode.LocalName == localName && xmlNode.NamespaceURI == namespaceURI)
				{
					return xmlNode;
				}
			}
			return null;
		}

		public virtual XmlNode Item(int index)
		{
			if (this.nodeList == null || index < 0 || index >= this.nodeList.Count)
			{
				return null;
			}
			return (XmlNode)this.nodeList[index];
		}

		public virtual XmlNode RemoveNamedItem(string name)
		{
			if (this.nodeList == null)
			{
				return null;
			}
			int i = 0;
			while (i < this.nodeList.Count)
			{
				XmlNode xmlNode = (XmlNode)this.nodeList[i];
				if (xmlNode.Name == name)
				{
					if (xmlNode.IsReadOnly)
					{
						throw new InvalidOperationException("Cannot remove. This node is read only: " + name);
					}
					this.nodeList.Remove(xmlNode);
					XmlAttribute xmlAttribute = xmlNode as XmlAttribute;
					if (xmlAttribute != null)
					{
						DTDAttributeDefinition attributeDefinition = xmlAttribute.GetAttributeDefinition();
						if (attributeDefinition != null && attributeDefinition.DefaultValue != null)
						{
							XmlAttribute xmlAttribute2 = xmlAttribute.OwnerDocument.CreateAttribute(xmlAttribute.Prefix, xmlAttribute.LocalName, xmlAttribute.NamespaceURI, true, false);
							xmlAttribute2.Value = attributeDefinition.DefaultValue;
							xmlAttribute2.SetDefault();
							xmlAttribute.OwnerElement.SetAttributeNode(xmlAttribute2);
						}
					}
					return xmlNode;
				}
				else
				{
					i++;
				}
			}
			return null;
		}

		public virtual XmlNode RemoveNamedItem(string localName, string namespaceURI)
		{
			if (this.nodeList == null)
			{
				return null;
			}
			for (int i = 0; i < this.nodeList.Count; i++)
			{
				XmlNode xmlNode = (XmlNode)this.nodeList[i];
				if (xmlNode.LocalName == localName && xmlNode.NamespaceURI == namespaceURI)
				{
					this.nodeList.Remove(xmlNode);
					return xmlNode;
				}
			}
			return null;
		}

		public virtual XmlNode SetNamedItem(XmlNode node)
		{
			return this.SetNamedItem(node, -1, true);
		}

		internal XmlNode SetNamedItem(XmlNode node, bool raiseEvent)
		{
			return this.SetNamedItem(node, -1, raiseEvent);
		}

		internal XmlNode SetNamedItem(XmlNode node, int pos, bool raiseEvent)
		{
			if (this.readOnly || node.OwnerDocument != this.parent.OwnerDocument)
			{
				throw new ArgumentException("Cannot add to NodeMap.");
			}
			if (raiseEvent)
			{
				this.parent.OwnerDocument.onNodeInserting(node, this.parent);
			}
			XmlNode xmlNode2;
			try
			{
				for (int i = 0; i < this.NodeList.Count; i++)
				{
					XmlNode xmlNode = (XmlNode)this.nodeList[i];
					if (xmlNode.LocalName == node.LocalName && xmlNode.NamespaceURI == node.NamespaceURI)
					{
						this.nodeList.Remove(xmlNode);
						if (pos < 0)
						{
							this.nodeList.Add(node);
						}
						else
						{
							this.nodeList.Insert(pos, node);
						}
						return xmlNode;
					}
				}
				if (pos < 0)
				{
					this.nodeList.Add(node);
				}
				else
				{
					this.nodeList.Insert(pos, node);
				}
				xmlNode2 = node;
			}
			finally
			{
				if (raiseEvent)
				{
					this.parent.OwnerDocument.onNodeInserted(node, this.parent);
				}
			}
			return xmlNode2;
		}

		internal ArrayList Nodes
		{
			get
			{
				return this.NodeList;
			}
		}

		private static readonly IEnumerator emptyEnumerator = new XmlNode[0].GetEnumerator();

		private XmlNode parent;

		private ArrayList nodeList;

		private bool readOnly;
	}
}
