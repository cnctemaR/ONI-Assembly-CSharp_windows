using System;
using System.Collections;
using Unity;

namespace System.Xml
{
	public class XmlNamedNodeMap : IEnumerable
	{
		internal XmlNamedNodeMap(XmlNode parent)
		{
			this.parent = parent;
		}

		public virtual XmlNode GetNamedItem(string name)
		{
			int num = this.FindNodeOffset(name);
			if (num >= 0)
			{
				return (XmlNode)this.nodes[num];
			}
			return null;
		}

		public virtual XmlNode SetNamedItem(XmlNode node)
		{
			if (node == null)
			{
				return null;
			}
			int num = this.FindNodeOffset(node.LocalName, node.NamespaceURI);
			if (num == -1)
			{
				this.AddNode(node);
				return null;
			}
			return this.ReplaceNodeAt(num, node);
		}

		public virtual XmlNode RemoveNamedItem(string name)
		{
			int num = this.FindNodeOffset(name);
			if (num >= 0)
			{
				return this.RemoveNodeAt(num);
			}
			return null;
		}

		public virtual int Count
		{
			get
			{
				return this.nodes.Count;
			}
		}

		public virtual XmlNode Item(int index)
		{
			if (index < 0 || index >= this.nodes.Count)
			{
				return null;
			}
			XmlNode xmlNode;
			try
			{
				xmlNode = (XmlNode)this.nodes[index];
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new IndexOutOfRangeException(Res.GetString("The index being passed in is out of range."));
			}
			return xmlNode;
		}

		public virtual XmlNode GetNamedItem(string localName, string namespaceURI)
		{
			int num = this.FindNodeOffset(localName, namespaceURI);
			if (num >= 0)
			{
				return (XmlNode)this.nodes[num];
			}
			return null;
		}

		public virtual XmlNode RemoveNamedItem(string localName, string namespaceURI)
		{
			int num = this.FindNodeOffset(localName, namespaceURI);
			if (num >= 0)
			{
				return this.RemoveNodeAt(num);
			}
			return null;
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.nodes.GetEnumerator();
		}

		internal int FindNodeOffset(string name)
		{
			int count = this.Count;
			for (int i = 0; i < count; i++)
			{
				XmlNode xmlNode = (XmlNode)this.nodes[i];
				if (name == xmlNode.Name)
				{
					return i;
				}
			}
			return -1;
		}

		internal int FindNodeOffset(string localName, string namespaceURI)
		{
			int count = this.Count;
			for (int i = 0; i < count; i++)
			{
				XmlNode xmlNode = (XmlNode)this.nodes[i];
				if (xmlNode.LocalName == localName && xmlNode.NamespaceURI == namespaceURI)
				{
					return i;
				}
			}
			return -1;
		}

		internal virtual XmlNode AddNode(XmlNode node)
		{
			XmlNode xmlNode;
			if (node.NodeType == XmlNodeType.Attribute)
			{
				xmlNode = ((XmlAttribute)node).OwnerElement;
			}
			else
			{
				xmlNode = node.ParentNode;
			}
			string value = node.Value;
			XmlNodeChangedEventArgs eventArgs = this.parent.GetEventArgs(node, xmlNode, this.parent, value, value, XmlNodeChangedAction.Insert);
			if (eventArgs != null)
			{
				this.parent.BeforeEvent(eventArgs);
			}
			this.nodes.Add(node);
			node.SetParent(this.parent);
			if (eventArgs != null)
			{
				this.parent.AfterEvent(eventArgs);
			}
			return node;
		}

		internal virtual XmlNode AddNodeForLoad(XmlNode node, XmlDocument doc)
		{
			XmlNodeChangedEventArgs insertEventArgsForLoad = doc.GetInsertEventArgsForLoad(node, this.parent);
			if (insertEventArgsForLoad != null)
			{
				doc.BeforeEvent(insertEventArgsForLoad);
			}
			this.nodes.Add(node);
			node.SetParent(this.parent);
			if (insertEventArgsForLoad != null)
			{
				doc.AfterEvent(insertEventArgsForLoad);
			}
			return node;
		}

		internal virtual XmlNode RemoveNodeAt(int i)
		{
			XmlNode xmlNode = (XmlNode)this.nodes[i];
			string value = xmlNode.Value;
			XmlNodeChangedEventArgs eventArgs = this.parent.GetEventArgs(xmlNode, this.parent, null, value, value, XmlNodeChangedAction.Remove);
			if (eventArgs != null)
			{
				this.parent.BeforeEvent(eventArgs);
			}
			this.nodes.RemoveAt(i);
			xmlNode.SetParent(null);
			if (eventArgs != null)
			{
				this.parent.AfterEvent(eventArgs);
			}
			return xmlNode;
		}

		internal XmlNode ReplaceNodeAt(int i, XmlNode node)
		{
			XmlNode xmlNode = this.RemoveNodeAt(i);
			this.InsertNodeAt(i, node);
			return xmlNode;
		}

		internal virtual XmlNode InsertNodeAt(int i, XmlNode node)
		{
			XmlNode xmlNode;
			if (node.NodeType == XmlNodeType.Attribute)
			{
				xmlNode = ((XmlAttribute)node).OwnerElement;
			}
			else
			{
				xmlNode = node.ParentNode;
			}
			string value = node.Value;
			XmlNodeChangedEventArgs eventArgs = this.parent.GetEventArgs(node, xmlNode, this.parent, value, value, XmlNodeChangedAction.Insert);
			if (eventArgs != null)
			{
				this.parent.BeforeEvent(eventArgs);
			}
			this.nodes.Insert(i, node);
			node.SetParent(this.parent);
			if (eventArgs != null)
			{
				this.parent.AfterEvent(eventArgs);
			}
			return node;
		}

		internal XmlNamedNodeMap()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal XmlNode parent;

		internal XmlNamedNodeMap.SmallXmlNodeList nodes;

		internal struct SmallXmlNodeList
		{
			public int Count
			{
				get
				{
					if (this.field == null)
					{
						return 0;
					}
					ArrayList arrayList = this.field as ArrayList;
					if (arrayList != null)
					{
						return arrayList.Count;
					}
					return 1;
				}
			}

			public object this[int index]
			{
				get
				{
					if (this.field == null)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					ArrayList arrayList = this.field as ArrayList;
					if (arrayList != null)
					{
						return arrayList[index];
					}
					if (index != 0)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return this.field;
				}
			}

			public void Add(object value)
			{
				if (this.field == null)
				{
					if (value == null)
					{
						this.field = new ArrayList { null };
						return;
					}
					this.field = value;
					return;
				}
				else
				{
					ArrayList arrayList = this.field as ArrayList;
					if (arrayList != null)
					{
						arrayList.Add(value);
						return;
					}
					this.field = new ArrayList { this.field, value };
					return;
				}
			}

			public void RemoveAt(int index)
			{
				if (this.field == null)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				ArrayList arrayList = this.field as ArrayList;
				if (arrayList != null)
				{
					arrayList.RemoveAt(index);
					return;
				}
				if (index != 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this.field = null;
			}

			public void Insert(int index, object value)
			{
				if (this.field == null)
				{
					if (index != 0)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					this.Add(value);
					return;
				}
				else
				{
					ArrayList arrayList = this.field as ArrayList;
					if (arrayList != null)
					{
						arrayList.Insert(index, value);
						return;
					}
					if (index == 0)
					{
						this.field = new ArrayList { value, this.field };
						return;
					}
					if (index == 1)
					{
						this.field = new ArrayList { this.field, value };
						return;
					}
					throw new ArgumentOutOfRangeException("index");
				}
			}

			public IEnumerator GetEnumerator()
			{
				if (this.field == null)
				{
					return XmlDocument.EmptyEnumerator;
				}
				ArrayList arrayList = this.field as ArrayList;
				if (arrayList != null)
				{
					return arrayList.GetEnumerator();
				}
				return new XmlNamedNodeMap.SmallXmlNodeList.SingleObjectEnumerator(this.field);
			}

			private object field;

			private class SingleObjectEnumerator : IEnumerator
			{
				public SingleObjectEnumerator(object value)
				{
					this.loneValue = value;
				}

				public object Current
				{
					get
					{
						if (this.position != 0)
						{
							throw new InvalidOperationException();
						}
						return this.loneValue;
					}
				}

				public bool MoveNext()
				{
					if (this.position < 0)
					{
						this.position = 0;
						return true;
					}
					this.position = 1;
					return false;
				}

				public void Reset()
				{
					this.position = -1;
				}

				private object loneValue;

				private int position = -1;
			}
		}
	}
}
