using System;
using System.Data;
using System.Threading;

namespace System.Xml
{
	internal sealed class XmlBoundElement : XmlElement
	{
		internal XmlBoundElement(string prefix, string localName, string namespaceURI, XmlDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
			this._state = ElementState.None;
		}

		public override XmlAttributeCollection Attributes
		{
			get
			{
				this.AutoFoliate();
				return base.Attributes;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				return this.Attributes.Count > 0;
			}
		}

		public override XmlNode FirstChild
		{
			get
			{
				this.AutoFoliate();
				return base.FirstChild;
			}
		}

		internal XmlNode SafeFirstChild
		{
			get
			{
				return base.FirstChild;
			}
		}

		public override XmlNode LastChild
		{
			get
			{
				this.AutoFoliate();
				return base.LastChild;
			}
		}

		public override XmlNode PreviousSibling
		{
			get
			{
				XmlNode previousSibling = base.PreviousSibling;
				if (previousSibling == null)
				{
					XmlBoundElement xmlBoundElement = this.ParentNode as XmlBoundElement;
					if (xmlBoundElement != null)
					{
						xmlBoundElement.AutoFoliate();
						return base.PreviousSibling;
					}
				}
				return previousSibling;
			}
		}

		internal XmlNode SafePreviousSibling
		{
			get
			{
				return base.PreviousSibling;
			}
		}

		public override XmlNode NextSibling
		{
			get
			{
				XmlNode nextSibling = base.NextSibling;
				if (nextSibling == null)
				{
					XmlBoundElement xmlBoundElement = this.ParentNode as XmlBoundElement;
					if (xmlBoundElement != null)
					{
						xmlBoundElement.AutoFoliate();
						return base.NextSibling;
					}
				}
				return nextSibling;
			}
		}

		internal XmlNode SafeNextSibling
		{
			get
			{
				return base.NextSibling;
			}
		}

		public override bool HasChildNodes
		{
			get
			{
				this.AutoFoliate();
				return base.HasChildNodes;
			}
		}

		public override XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
		{
			this.AutoFoliate();
			return base.InsertBefore(newChild, refChild);
		}

		public override XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
		{
			this.AutoFoliate();
			return base.InsertAfter(newChild, refChild);
		}

		public override XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
		{
			this.AutoFoliate();
			return base.ReplaceChild(newChild, oldChild);
		}

		public override XmlNode AppendChild(XmlNode newChild)
		{
			this.AutoFoliate();
			return base.AppendChild(newChild);
		}

		internal void RemoveAllChildren()
		{
			XmlNode nextSibling;
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = nextSibling)
			{
				nextSibling = xmlNode.NextSibling;
				this.RemoveChild(xmlNode);
			}
		}

		public override string InnerXml
		{
			get
			{
				return base.InnerXml;
			}
			set
			{
				this.RemoveAllChildren();
				XmlDataDocument xmlDataDocument = (XmlDataDocument)this.OwnerDocument;
				bool ignoreXmlEvents = xmlDataDocument.IgnoreXmlEvents;
				bool ignoreDataSetEvents = xmlDataDocument.IgnoreDataSetEvents;
				xmlDataDocument.IgnoreXmlEvents = true;
				xmlDataDocument.IgnoreDataSetEvents = true;
				base.InnerXml = value;
				xmlDataDocument.SyncTree(this);
				xmlDataDocument.IgnoreDataSetEvents = ignoreDataSetEvents;
				xmlDataDocument.IgnoreXmlEvents = ignoreXmlEvents;
			}
		}

		internal DataRow Row
		{
			get
			{
				return this._row;
			}
			set
			{
				this._row = value;
			}
		}

		internal bool IsFoliated
		{
			get
			{
				while (this._state == ElementState.Foliating || this._state == ElementState.Defoliating)
				{
					Thread.Sleep(0);
				}
				return this._state != ElementState.Defoliated;
			}
		}

		internal ElementState ElementState
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
			}
		}

		internal void Foliate(ElementState newState)
		{
			XmlDataDocument xmlDataDocument = (XmlDataDocument)this.OwnerDocument;
			if (xmlDataDocument != null)
			{
				xmlDataDocument.Foliate(this, newState);
			}
		}

		private void AutoFoliate()
		{
			XmlDataDocument xmlDataDocument = (XmlDataDocument)this.OwnerDocument;
			if (xmlDataDocument != null)
			{
				xmlDataDocument.Foliate(this, xmlDataDocument.AutoFoliationState);
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlDataDocument xmlDataDocument = (XmlDataDocument)this.OwnerDocument;
			ElementState autoFoliationState = xmlDataDocument.AutoFoliationState;
			xmlDataDocument.AutoFoliationState = ElementState.WeakFoliation;
			XmlElement xmlElement;
			try
			{
				this.Foliate(ElementState.WeakFoliation);
				xmlElement = (XmlElement)base.CloneNode(deep);
			}
			finally
			{
				xmlDataDocument.AutoFoliationState = autoFoliationState;
			}
			return xmlElement;
		}

		public override void WriteContentTo(XmlWriter w)
		{
			DataPointer dataPointer = new DataPointer((XmlDataDocument)this.OwnerDocument, this);
			try
			{
				dataPointer.AddPointer();
				XmlBoundElement.WriteBoundElementContentTo(dataPointer, w);
			}
			finally
			{
				dataPointer.SetNoLongerUse();
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			DataPointer dataPointer = new DataPointer((XmlDataDocument)this.OwnerDocument, this);
			try
			{
				dataPointer.AddPointer();
				this.WriteRootBoundElementTo(dataPointer, w);
			}
			finally
			{
				dataPointer.SetNoLongerUse();
			}
		}

		private void WriteRootBoundElementTo(DataPointer dp, XmlWriter w)
		{
			XmlDataDocument xmlDataDocument = (XmlDataDocument)this.OwnerDocument;
			w.WriteStartElement(dp.Prefix, dp.LocalName, dp.NamespaceURI);
			int attributeCount = dp.AttributeCount;
			bool flag = false;
			if (attributeCount > 0)
			{
				for (int i = 0; i < attributeCount; i++)
				{
					dp.MoveToAttribute(i);
					if (dp.Prefix == "xmlns" && dp.LocalName == "xsi")
					{
						flag = true;
					}
					XmlBoundElement.WriteTo(dp, w);
					dp.MoveToOwnerElement();
				}
			}
			if (!flag && xmlDataDocument._bLoadFromDataSet && xmlDataDocument._bHasXSINIL)
			{
				w.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");
			}
			XmlBoundElement.WriteBoundElementContentTo(dp, w);
			if (dp.IsEmptyElement)
			{
				w.WriteEndElement();
				return;
			}
			w.WriteFullEndElement();
		}

		private static void WriteBoundElementTo(DataPointer dp, XmlWriter w)
		{
			w.WriteStartElement(dp.Prefix, dp.LocalName, dp.NamespaceURI);
			int attributeCount = dp.AttributeCount;
			if (attributeCount > 0)
			{
				for (int i = 0; i < attributeCount; i++)
				{
					dp.MoveToAttribute(i);
					XmlBoundElement.WriteTo(dp, w);
					dp.MoveToOwnerElement();
				}
			}
			XmlBoundElement.WriteBoundElementContentTo(dp, w);
			if (dp.IsEmptyElement)
			{
				w.WriteEndElement();
				return;
			}
			w.WriteFullEndElement();
		}

		private static void WriteBoundElementContentTo(DataPointer dp, XmlWriter w)
		{
			if (!dp.IsEmptyElement && dp.MoveToFirstChild())
			{
				do
				{
					XmlBoundElement.WriteTo(dp, w);
				}
				while (dp.MoveToNextSibling());
				dp.MoveToParent();
			}
		}

		private static void WriteTo(DataPointer dp, XmlWriter w)
		{
			switch (dp.NodeType)
			{
			case XmlNodeType.Element:
				XmlBoundElement.WriteBoundElementTo(dp, w);
				return;
			case XmlNodeType.Attribute:
				if (!dp.IsDefault)
				{
					w.WriteStartAttribute(dp.Prefix, dp.LocalName, dp.NamespaceURI);
					if (dp.MoveToFirstChild())
					{
						do
						{
							XmlBoundElement.WriteTo(dp, w);
						}
						while (dp.MoveToNextSibling());
						dp.MoveToParent();
					}
					w.WriteEndAttribute();
					return;
				}
				break;
			case XmlNodeType.Text:
				w.WriteString(dp.Value);
				return;
			default:
				if (dp.GetNode() != null)
				{
					dp.GetNode().WriteTo(w);
				}
				break;
			}
		}

		public override XmlNodeList GetElementsByTagName(string name)
		{
			XmlNodeList elementsByTagName = base.GetElementsByTagName(name);
			int count = elementsByTagName.Count;
			return elementsByTagName;
		}

		private DataRow _row;

		private ElementState _state;
	}
}
