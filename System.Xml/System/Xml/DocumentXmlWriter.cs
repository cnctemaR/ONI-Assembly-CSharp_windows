using System;
using System.Collections.Generic;

namespace System.Xml
{
	internal sealed class DocumentXmlWriter : XmlRawWriter, IXmlNamespaceResolver
	{
		public DocumentXmlWriter(DocumentXmlWriterType type, XmlNode start, XmlDocument document)
		{
			this.type = type;
			this.start = start;
			this.document = document;
			this.state = this.StartState();
			this.fragment = new List<XmlNode>();
			this.settings = new XmlWriterSettings();
			this.settings.ReadOnly = false;
			this.settings.CheckCharacters = false;
			this.settings.CloseOutput = false;
			this.settings.ConformanceLevel = ((this.state == DocumentXmlWriter.State.Prolog) ? ConformanceLevel.Document : ConformanceLevel.Fragment);
			this.settings.ReadOnly = true;
		}

		public XmlNamespaceManager NamespaceManager
		{
			set
			{
				this.namespaceManager = value;
			}
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return this.settings;
			}
		}

		internal void SetSettings(XmlWriterSettings value)
		{
			this.settings = value;
		}

		public DocumentXPathNavigator Navigator
		{
			set
			{
				this.navigator = value;
			}
		}

		public XmlNode EndNode
		{
			set
			{
				this.end = value;
			}
		}

		internal override void WriteXmlDeclaration(XmlStandalone standalone)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteXmlDeclaration);
			if (standalone != XmlStandalone.Omit)
			{
				XmlNode xmlNode = this.document.CreateXmlDeclaration("1.0", string.Empty, (standalone == XmlStandalone.Yes) ? "yes" : "no");
				this.AddChild(xmlNode, this.write);
			}
		}

		internal override void WriteXmlDeclaration(string xmldecl)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteXmlDeclaration);
			string text;
			string text2;
			string text3;
			XmlLoader.ParseXmlDeclarationValue(xmldecl, out text, out text2, out text3);
			XmlNode xmlNode = this.document.CreateXmlDeclaration(text, text2, text3);
			this.AddChild(xmlNode, this.write);
		}

		public override void WriteStartDocument()
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteStartDocument);
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteStartDocument);
		}

		public override void WriteEndDocument()
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteEndDocument);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteDocType);
			XmlNode xmlNode = this.document.CreateDocumentType(name, pubid, sysid, subset);
			this.AddChild(xmlNode, this.write);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteStartElement);
			XmlNode xmlNode = this.document.CreateElement(prefix, localName, ns);
			this.AddChild(xmlNode, this.write);
			this.write = xmlNode;
		}

		public override void WriteEndElement()
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteEndElement);
			if (this.write == null)
			{
				throw new InvalidOperationException();
			}
			this.write = this.write.ParentNode;
		}

		internal override void WriteEndElement(string prefix, string localName, string ns)
		{
			this.WriteEndElement();
		}

		public override void WriteFullEndElement()
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteFullEndElement);
			XmlElement xmlElement = this.write as XmlElement;
			if (xmlElement == null)
			{
				throw new InvalidOperationException();
			}
			xmlElement.IsEmpty = false;
			this.write = xmlElement.ParentNode;
		}

		internal override void WriteFullEndElement(string prefix, string localName, string ns)
		{
			this.WriteFullEndElement();
		}

		internal override void StartElementContent()
		{
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteStartAttribute);
			XmlAttribute xmlAttribute = this.document.CreateAttribute(prefix, localName, ns);
			this.AddAttribute(xmlAttribute, this.write);
			this.write = xmlAttribute;
		}

		public override void WriteEndAttribute()
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteEndAttribute);
			XmlAttribute xmlAttribute = this.write as XmlAttribute;
			if (xmlAttribute == null)
			{
				throw new InvalidOperationException();
			}
			if (!xmlAttribute.HasChildNodes)
			{
				XmlNode xmlNode = this.document.CreateTextNode(string.Empty);
				this.AddChild(xmlNode, xmlAttribute);
			}
			this.write = xmlAttribute.OwnerElement;
		}

		internal override void WriteNamespaceDeclaration(string prefix, string ns)
		{
			this.WriteStartNamespaceDeclaration(prefix);
			this.WriteString(ns);
			this.WriteEndNamespaceDeclaration();
		}

		internal override bool SupportsNamespaceDeclarationInChunks
		{
			get
			{
				return true;
			}
		}

		internal override void WriteStartNamespaceDeclaration(string prefix)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteStartNamespaceDeclaration);
			XmlAttribute xmlAttribute;
			if (prefix.Length == 0)
			{
				xmlAttribute = this.document.CreateAttribute(prefix, this.document.strXmlns, this.document.strReservedXmlns);
			}
			else
			{
				xmlAttribute = this.document.CreateAttribute(this.document.strXmlns, prefix, this.document.strReservedXmlns);
			}
			this.AddAttribute(xmlAttribute, this.write);
			this.write = xmlAttribute;
		}

		internal override void WriteEndNamespaceDeclaration()
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteEndNamespaceDeclaration);
			XmlAttribute xmlAttribute = this.write as XmlAttribute;
			if (xmlAttribute == null)
			{
				throw new InvalidOperationException();
			}
			if (!xmlAttribute.HasChildNodes)
			{
				XmlNode xmlNode = this.document.CreateTextNode(string.Empty);
				this.AddChild(xmlNode, xmlAttribute);
			}
			this.write = xmlAttribute.OwnerElement;
		}

		public override void WriteCData(string text)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteCData);
			XmlConvert.VerifyCharData(text, ExceptionType.ArgumentException);
			XmlNode xmlNode = this.document.CreateCDataSection(text);
			this.AddChild(xmlNode, this.write);
		}

		public override void WriteComment(string text)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteComment);
			XmlConvert.VerifyCharData(text, ExceptionType.ArgumentException);
			XmlNode xmlNode = this.document.CreateComment(text);
			this.AddChild(xmlNode, this.write);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteProcessingInstruction);
			XmlConvert.VerifyCharData(text, ExceptionType.ArgumentException);
			XmlNode xmlNode = this.document.CreateProcessingInstruction(name, text);
			this.AddChild(xmlNode, this.write);
		}

		public override void WriteEntityRef(string name)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteEntityRef);
			XmlNode xmlNode = this.document.CreateEntityReference(name);
			this.AddChild(xmlNode, this.write);
		}

		public override void WriteCharEntity(char ch)
		{
			this.WriteString(new string(ch, 1));
		}

		public override void WriteWhitespace(string text)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteWhitespace);
			XmlConvert.VerifyCharData(text, ExceptionType.ArgumentException);
			if (this.document.PreserveWhitespace)
			{
				XmlNode xmlNode = this.document.CreateWhitespace(text);
				this.AddChild(xmlNode, this.write);
			}
		}

		public override void WriteString(string text)
		{
			this.VerifyState(DocumentXmlWriter.Method.WriteString);
			XmlConvert.VerifyCharData(text, ExceptionType.ArgumentException);
			XmlNode xmlNode = this.document.CreateTextNode(text);
			this.AddChild(xmlNode, this.write);
		}

		public override void WriteSurrogateCharEntity(char lowCh, char highCh)
		{
			this.WriteString(new string(new char[] { highCh, lowCh }));
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.WriteString(new string(buffer, index, count));
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.WriteString(new string(buffer, index, count));
		}

		public override void WriteRaw(string data)
		{
			this.WriteString(data);
		}

		public override void Close()
		{
		}

		internal override void Close(WriteState currentState)
		{
			if (currentState == WriteState.Error)
			{
				return;
			}
			try
			{
				switch (this.type)
				{
				case DocumentXmlWriterType.InsertSiblingAfter:
				{
					XmlNode xmlNode = this.start.ParentNode;
					if (xmlNode == null)
					{
						throw new InvalidOperationException(Res.GetString("The current position of the navigator is missing a valid parent."));
					}
					for (int i = this.fragment.Count - 1; i >= 0; i--)
					{
						xmlNode.InsertAfter(this.fragment[i], this.start);
					}
					break;
				}
				case DocumentXmlWriterType.InsertSiblingBefore:
				{
					XmlNode xmlNode = this.start.ParentNode;
					if (xmlNode == null)
					{
						throw new InvalidOperationException(Res.GetString("The current position of the navigator is missing a valid parent."));
					}
					for (int j = 0; j < this.fragment.Count; j++)
					{
						xmlNode.InsertBefore(this.fragment[j], this.start);
					}
					break;
				}
				case DocumentXmlWriterType.PrependChild:
				{
					for (int k = this.fragment.Count - 1; k >= 0; k--)
					{
						this.start.PrependChild(this.fragment[k]);
					}
					break;
				}
				case DocumentXmlWriterType.AppendChild:
				{
					for (int l = 0; l < this.fragment.Count; l++)
					{
						this.start.AppendChild(this.fragment[l]);
					}
					break;
				}
				case DocumentXmlWriterType.AppendAttribute:
					this.CloseWithAppendAttribute();
					break;
				case DocumentXmlWriterType.ReplaceToFollowingSibling:
					if (this.fragment.Count == 0)
					{
						throw new InvalidOperationException(Res.GetString("No content generated as the result of the operation."));
					}
					this.CloseWithReplaceToFollowingSibling();
					break;
				}
			}
			finally
			{
				this.fragment.Clear();
			}
		}

		private void CloseWithAppendAttribute()
		{
			XmlAttributeCollection attributes = (this.start as XmlElement).Attributes;
			for (int i = 0; i < this.fragment.Count; i++)
			{
				XmlAttribute xmlAttribute = this.fragment[i] as XmlAttribute;
				int num = attributes.FindNodeOffsetNS(xmlAttribute);
				if (num != -1 && ((XmlAttribute)attributes.nodes[num]).Specified)
				{
					throw new XmlException("'{0}' is a duplicate attribute name.", (xmlAttribute.Prefix.Length == 0) ? xmlAttribute.LocalName : (xmlAttribute.Prefix + ":" + xmlAttribute.LocalName));
				}
			}
			for (int j = 0; j < this.fragment.Count; j++)
			{
				XmlAttribute xmlAttribute2 = this.fragment[j] as XmlAttribute;
				attributes.Append(xmlAttribute2);
			}
		}

		private void CloseWithReplaceToFollowingSibling()
		{
			XmlNode parentNode = this.start.ParentNode;
			if (parentNode == null)
			{
				throw new InvalidOperationException(Res.GetString("The current position of the navigator is missing a valid parent."));
			}
			if (this.start != this.end)
			{
				if (!DocumentXPathNavigator.IsFollowingSibling(this.start, this.end))
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
				}
				if (this.start.IsReadOnly)
				{
					throw new InvalidOperationException(Res.GetString("This node is read-only. It cannot be modified."));
				}
				DocumentXPathNavigator.DeleteToFollowingSibling(this.start.NextSibling, this.end);
			}
			XmlNode xmlNode = this.fragment[0];
			parentNode.ReplaceChild(xmlNode, this.start);
			for (int i = this.fragment.Count - 1; i >= 1; i--)
			{
				parentNode.InsertAfter(this.fragment[i], xmlNode);
			}
			this.navigator.ResetPosition(xmlNode);
		}

		public override void Flush()
		{
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.namespaceManager.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			return this.namespaceManager.LookupNamespace(prefix);
		}

		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			return this.namespaceManager.LookupPrefix(namespaceName);
		}

		private void AddAttribute(XmlAttribute attr, XmlNode parent)
		{
			if (parent == null)
			{
				this.fragment.Add(attr);
				return;
			}
			XmlElement xmlElement = parent as XmlElement;
			if (xmlElement == null)
			{
				throw new InvalidOperationException();
			}
			xmlElement.Attributes.Append(attr);
		}

		private void AddChild(XmlNode node, XmlNode parent)
		{
			if (parent == null)
			{
				this.fragment.Add(node);
				return;
			}
			parent.AppendChild(node);
		}

		private DocumentXmlWriter.State StartState()
		{
			XmlNodeType xmlNodeType = XmlNodeType.None;
			switch (this.type)
			{
			case DocumentXmlWriterType.InsertSiblingAfter:
			case DocumentXmlWriterType.InsertSiblingBefore:
			{
				XmlNode parentNode = this.start.ParentNode;
				if (parentNode != null)
				{
					xmlNodeType = parentNode.NodeType;
				}
				if (xmlNodeType == XmlNodeType.Document)
				{
					return DocumentXmlWriter.State.Prolog;
				}
				if (xmlNodeType == XmlNodeType.DocumentFragment)
				{
					return DocumentXmlWriter.State.Fragment;
				}
				break;
			}
			case DocumentXmlWriterType.PrependChild:
			case DocumentXmlWriterType.AppendChild:
				xmlNodeType = this.start.NodeType;
				if (xmlNodeType == XmlNodeType.Document)
				{
					return DocumentXmlWriter.State.Prolog;
				}
				if (xmlNodeType == XmlNodeType.DocumentFragment)
				{
					return DocumentXmlWriter.State.Fragment;
				}
				break;
			case DocumentXmlWriterType.AppendAttribute:
				return DocumentXmlWriter.State.Attribute;
			}
			return DocumentXmlWriter.State.Content;
		}

		private void VerifyState(DocumentXmlWriter.Method method)
		{
			this.state = DocumentXmlWriter.changeState[(int)(method * DocumentXmlWriter.Method.WriteEndElement + (int)this.state)];
			if (this.state == DocumentXmlWriter.State.Error)
			{
				throw new InvalidOperationException(Res.GetString("The Writer is closed or in error state."));
			}
		}

		private DocumentXmlWriterType type;

		private XmlNode start;

		private XmlDocument document;

		private XmlNamespaceManager namespaceManager;

		private DocumentXmlWriter.State state;

		private XmlNode write;

		private List<XmlNode> fragment;

		private XmlWriterSettings settings;

		private DocumentXPathNavigator navigator;

		private XmlNode end;

		private static DocumentXmlWriter.State[] changeState = new DocumentXmlWriter.State[]
		{
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Prolog,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Prolog,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Prolog,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Prolog,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Prolog,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Error,
			DocumentXmlWriter.State.Content,
			DocumentXmlWriter.State.Content
		};

		private enum State
		{
			Error,
			Attribute,
			Prolog,
			Fragment,
			Content,
			Last
		}

		private enum Method
		{
			WriteXmlDeclaration,
			WriteStartDocument,
			WriteEndDocument,
			WriteDocType,
			WriteStartElement,
			WriteEndElement,
			WriteFullEndElement,
			WriteStartAttribute,
			WriteEndAttribute,
			WriteStartNamespaceDeclaration,
			WriteEndNamespaceDeclaration,
			WriteCData,
			WriteComment,
			WriteProcessingInstruction,
			WriteEntityRef,
			WriteWhitespace,
			WriteString
		}
	}
}
