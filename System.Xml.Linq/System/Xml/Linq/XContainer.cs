using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System.Xml.Linq
{
	public abstract class XContainer : XNode
	{
		internal XContainer()
		{
		}

		internal XContainer(XContainer other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other.content is string)
			{
				this.content = other.content;
				return;
			}
			XNode xnode = (XNode)other.content;
			if (xnode != null)
			{
				do
				{
					xnode = xnode.next;
					this.AppendNodeSkipNotify(xnode.CloneNode());
				}
				while (xnode != other.content);
			}
		}

		public XNode FirstNode
		{
			get
			{
				XNode lastNode = this.LastNode;
				if (lastNode == null)
				{
					return null;
				}
				return lastNode.next;
			}
		}

		public XNode LastNode
		{
			get
			{
				if (this.content == null)
				{
					return null;
				}
				XNode xnode = this.content as XNode;
				if (xnode != null)
				{
					return xnode;
				}
				string text = this.content as string;
				if (text != null)
				{
					if (text.Length == 0)
					{
						return null;
					}
					XText xtext = new XText(text);
					xtext.parent = this;
					xtext.next = xtext;
					Interlocked.CompareExchange<object>(ref this.content, xtext, text);
				}
				return (XNode)this.content;
			}
		}

		public void Add(object content)
		{
			if (base.SkipNotify())
			{
				this.AddContentSkipNotify(content);
				return;
			}
			if (content == null)
			{
				return;
			}
			XNode xnode = content as XNode;
			if (xnode != null)
			{
				this.AddNode(xnode);
				return;
			}
			string text = content as string;
			if (text != null)
			{
				this.AddString(text);
				return;
			}
			XAttribute xattribute = content as XAttribute;
			if (xattribute != null)
			{
				this.AddAttribute(xattribute);
				return;
			}
			XStreamingElement xstreamingElement = content as XStreamingElement;
			if (xstreamingElement != null)
			{
				this.AddNode(new XElement(xstreamingElement));
				return;
			}
			object[] array = content as object[];
			if (array != null)
			{
				foreach (object obj in array)
				{
					this.Add(obj);
				}
				return;
			}
			IEnumerable enumerable = content as IEnumerable;
			if (enumerable != null)
			{
				foreach (object obj2 in enumerable)
				{
					this.Add(obj2);
				}
				return;
			}
			this.AddString(XContainer.GetStringValue(content));
		}

		public void Add(params object[] content)
		{
			this.Add(content);
		}

		public void AddFirst(object content)
		{
			new Inserter(this, null).Add(content);
		}

		public void AddFirst(params object[] content)
		{
			this.AddFirst(content);
		}

		public XmlWriter CreateWriter()
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.ConformanceLevel = ((this is XDocument) ? ConformanceLevel.Document : ConformanceLevel.Fragment);
			return XmlWriter.Create(new XNodeBuilder(this), xmlWriterSettings);
		}

		public IEnumerable<XNode> DescendantNodes()
		{
			return this.GetDescendantNodes(false);
		}

		public IEnumerable<XElement> Descendants()
		{
			return this.GetDescendants(null, false);
		}

		public IEnumerable<XElement> Descendants(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return this.GetDescendants(name, false);
		}

		public XElement Element(XName name)
		{
			XNode xnode = this.content as XNode;
			if (xnode != null)
			{
				XElement xelement;
				for (;;)
				{
					xnode = xnode.next;
					xelement = xnode as XElement;
					if (xelement != null && xelement.name == name)
					{
						break;
					}
					if (xnode == this.content)
					{
						goto IL_0039;
					}
				}
				return xelement;
			}
			IL_0039:
			return null;
		}

		public IEnumerable<XElement> Elements()
		{
			return this.GetElements(null);
		}

		public IEnumerable<XElement> Elements(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return this.GetElements(name);
		}

		public IEnumerable<XNode> Nodes()
		{
			XNode i = this.LastNode;
			if (i != null)
			{
				do
				{
					i = i.next;
					yield return i;
				}
				while (i.parent == this && i != this.content);
			}
			yield break;
		}

		public void RemoveNodes()
		{
			if (base.SkipNotify())
			{
				this.RemoveNodesSkipNotify();
				return;
			}
			while (this.content != null)
			{
				string text = this.content as string;
				if (text != null)
				{
					if (text.Length > 0)
					{
						this.ConvertTextToNode();
					}
					else if (this is XElement)
					{
						base.NotifyChanging(this, XObjectChangeEventArgs.Value);
						if (text != this.content)
						{
							throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
						}
						this.content = null;
						base.NotifyChanged(this, XObjectChangeEventArgs.Value);
					}
					else
					{
						this.content = null;
					}
				}
				XNode xnode = this.content as XNode;
				if (xnode != null)
				{
					XNode next = xnode.next;
					base.NotifyChanging(next, XObjectChangeEventArgs.Remove);
					if (xnode != this.content || next != xnode.next)
					{
						throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
					}
					if (next != xnode)
					{
						xnode.next = next.next;
					}
					else
					{
						this.content = null;
					}
					next.parent = null;
					next.next = null;
					base.NotifyChanged(next, XObjectChangeEventArgs.Remove);
				}
			}
		}

		public void ReplaceNodes(object content)
		{
			content = XContainer.GetContentSnapshot(content);
			this.RemoveNodes();
			this.Add(content);
		}

		public void ReplaceNodes(params object[] content)
		{
			this.ReplaceNodes(content);
		}

		internal virtual void AddAttribute(XAttribute a)
		{
		}

		internal virtual void AddAttributeSkipNotify(XAttribute a)
		{
		}

		internal void AddContentSkipNotify(object content)
		{
			if (content == null)
			{
				return;
			}
			XNode xnode = content as XNode;
			if (xnode != null)
			{
				this.AddNodeSkipNotify(xnode);
				return;
			}
			string text = content as string;
			if (text != null)
			{
				this.AddStringSkipNotify(text);
				return;
			}
			XAttribute xattribute = content as XAttribute;
			if (xattribute != null)
			{
				this.AddAttributeSkipNotify(xattribute);
				return;
			}
			XStreamingElement xstreamingElement = content as XStreamingElement;
			if (xstreamingElement != null)
			{
				this.AddNodeSkipNotify(new XElement(xstreamingElement));
				return;
			}
			object[] array = content as object[];
			if (array != null)
			{
				foreach (object obj in array)
				{
					this.AddContentSkipNotify(obj);
				}
				return;
			}
			IEnumerable enumerable = content as IEnumerable;
			if (enumerable != null)
			{
				foreach (object obj2 in enumerable)
				{
					this.AddContentSkipNotify(obj2);
				}
				return;
			}
			this.AddStringSkipNotify(XContainer.GetStringValue(content));
		}

		internal void AddNode(XNode n)
		{
			this.ValidateNode(n, this);
			if (n.parent != null)
			{
				n = n.CloneNode();
			}
			else
			{
				XNode xnode = this;
				while (xnode.parent != null)
				{
					xnode = xnode.parent;
				}
				if (n == xnode)
				{
					n = n.CloneNode();
				}
			}
			this.ConvertTextToNode();
			this.AppendNode(n);
		}

		internal void AddNodeSkipNotify(XNode n)
		{
			this.ValidateNode(n, this);
			if (n.parent != null)
			{
				n = n.CloneNode();
			}
			else
			{
				XNode xnode = this;
				while (xnode.parent != null)
				{
					xnode = xnode.parent;
				}
				if (n == xnode)
				{
					n = n.CloneNode();
				}
			}
			this.ConvertTextToNode();
			this.AppendNodeSkipNotify(n);
		}

		internal void AddString(string s)
		{
			this.ValidateString(s);
			if (this.content != null)
			{
				if (s.Length > 0)
				{
					this.ConvertTextToNode();
					XText xtext = this.content as XText;
					if (xtext != null && !(xtext is XCData))
					{
						XText xtext2 = xtext;
						xtext2.Value += s;
						return;
					}
					this.AppendNode(new XText(s));
				}
				return;
			}
			if (s.Length > 0)
			{
				this.AppendNode(new XText(s));
				return;
			}
			if (!(this is XElement))
			{
				this.content = s;
				return;
			}
			base.NotifyChanging(this, XObjectChangeEventArgs.Value);
			if (this.content != null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
			}
			this.content = s;
			base.NotifyChanged(this, XObjectChangeEventArgs.Value);
		}

		internal void AddStringSkipNotify(string s)
		{
			this.ValidateString(s);
			if (this.content == null)
			{
				this.content = s;
				return;
			}
			if (s.Length > 0)
			{
				if (this.content is string)
				{
					this.content = (string)this.content + s;
					return;
				}
				XText xtext = this.content as XText;
				if (xtext != null && !(xtext is XCData))
				{
					XText xtext2 = xtext;
					xtext2.text += s;
					return;
				}
				this.AppendNodeSkipNotify(new XText(s));
			}
		}

		internal void AppendNode(XNode n)
		{
			bool flag = base.NotifyChanging(n, XObjectChangeEventArgs.Add);
			if (n.parent != null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
			}
			this.AppendNodeSkipNotify(n);
			if (flag)
			{
				base.NotifyChanged(n, XObjectChangeEventArgs.Add);
			}
		}

		internal void AppendNodeSkipNotify(XNode n)
		{
			n.parent = this;
			if (this.content == null || this.content is string)
			{
				n.next = n;
			}
			else
			{
				XNode xnode = (XNode)this.content;
				n.next = xnode.next;
				xnode.next = n;
			}
			this.content = n;
		}

		internal override void AppendText(StringBuilder sb)
		{
			string text = this.content as string;
			if (text != null)
			{
				sb.Append(text);
				return;
			}
			XNode xnode = (XNode)this.content;
			if (xnode != null)
			{
				do
				{
					xnode = xnode.next;
					xnode.AppendText(sb);
				}
				while (xnode != this.content);
			}
		}

		private string GetTextOnly()
		{
			if (this.content == null)
			{
				return null;
			}
			string text = this.content as string;
			if (text == null)
			{
				XNode xnode = (XNode)this.content;
				for (;;)
				{
					xnode = xnode.next;
					if (xnode.NodeType != XmlNodeType.Text)
					{
						break;
					}
					text += ((XText)xnode).Value;
					if (xnode == this.content)
					{
						return text;
					}
				}
				return null;
			}
			return text;
		}

		private string CollectText(ref XNode n)
		{
			string text = "";
			while (n != null && n.NodeType == XmlNodeType.Text)
			{
				text += ((XText)n).Value;
				n = ((n != this.content) ? n.next : null);
			}
			return text;
		}

		internal bool ContentsEqual(XContainer e)
		{
			if (this.content == e.content)
			{
				return true;
			}
			string textOnly = this.GetTextOnly();
			if (textOnly != null)
			{
				return textOnly == e.GetTextOnly();
			}
			XNode xnode = this.content as XNode;
			XNode xnode2 = e.content as XNode;
			if (xnode != null && xnode2 != null)
			{
				xnode = xnode.next;
				xnode2 = xnode2.next;
				while (!(this.CollectText(ref xnode) != e.CollectText(ref xnode2)))
				{
					if (xnode == null && xnode2 == null)
					{
						return true;
					}
					if (xnode == null || xnode2 == null || !xnode.DeepEquals(xnode2))
					{
						break;
					}
					xnode = ((xnode != this.content) ? xnode.next : null);
					xnode2 = ((xnode2 != e.content) ? xnode2.next : null);
				}
			}
			return false;
		}

		internal int ContentsHashCode()
		{
			string textOnly = this.GetTextOnly();
			if (textOnly != null)
			{
				return textOnly.GetHashCode();
			}
			int num = 0;
			XNode xnode = this.content as XNode;
			if (xnode != null)
			{
				do
				{
					xnode = xnode.next;
					string text = this.CollectText(ref xnode);
					if (text.Length > 0)
					{
						num ^= text.GetHashCode();
					}
					if (xnode == null)
					{
						break;
					}
					num ^= xnode.GetDeepHashCode();
				}
				while (xnode != this.content);
			}
			return num;
		}

		internal void ConvertTextToNode()
		{
			string text = this.content as string;
			if (text != null && text.Length > 0)
			{
				XText xtext = new XText(text);
				xtext.parent = this;
				xtext.next = xtext;
				this.content = xtext;
			}
		}

		internal static string GetDateTimeString(DateTime value)
		{
			return XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind);
		}

		internal IEnumerable<XNode> GetDescendantNodes(bool self)
		{
			if (self)
			{
				yield return this;
			}
			XNode i = this;
			for (;;)
			{
				XContainer xcontainer = i as XContainer;
				XNode firstNode;
				if (xcontainer != null && (firstNode = xcontainer.FirstNode) != null)
				{
					i = firstNode;
				}
				else
				{
					while (i != null && i != this && i == i.parent.content)
					{
						i = i.parent;
					}
					if (i == null || i == this)
					{
						break;
					}
					i = i.next;
				}
				yield return i;
			}
			yield break;
		}

		internal IEnumerable<XElement> GetDescendants(XName name, bool self)
		{
			if (self)
			{
				XElement xelement = (XElement)this;
				if (name == null || xelement.name == name)
				{
					yield return xelement;
				}
			}
			XNode i = this;
			XContainer xcontainer = this;
			for (;;)
			{
				if (xcontainer != null && xcontainer.content is XNode)
				{
					i = ((XNode)xcontainer.content).next;
				}
				else
				{
					while (i != this && i == i.parent.content)
					{
						i = i.parent;
					}
					if (i == this)
					{
						break;
					}
					i = i.next;
				}
				XElement e = i as XElement;
				if (e != null && (name == null || e.name == name))
				{
					yield return e;
				}
				xcontainer = e;
				e = null;
			}
			yield break;
		}

		private IEnumerable<XElement> GetElements(XName name)
		{
			XNode i = this.content as XNode;
			if (i != null)
			{
				do
				{
					i = i.next;
					XElement xelement = i as XElement;
					if (xelement != null && (name == null || xelement.name == name))
					{
						yield return xelement;
					}
				}
				while (i.parent == this && i != this.content);
			}
			yield break;
		}

		internal static string GetStringValue(object value)
		{
			string text;
			if (value is string)
			{
				text = (string)value;
			}
			else if (value is double)
			{
				text = XmlConvert.ToString((double)value);
			}
			else if (value is float)
			{
				text = XmlConvert.ToString((float)value);
			}
			else if (value is decimal)
			{
				text = XmlConvert.ToString((decimal)value);
			}
			else if (value is bool)
			{
				text = XmlConvert.ToString((bool)value);
			}
			else if (value is DateTime)
			{
				text = XContainer.GetDateTimeString((DateTime)value);
			}
			else if (value is DateTimeOffset)
			{
				text = XmlConvert.ToString((DateTimeOffset)value);
			}
			else if (value is TimeSpan)
			{
				text = XmlConvert.ToString((TimeSpan)value);
			}
			else
			{
				if (value is XObject)
				{
					throw new ArgumentException(Res.GetString("Argument_XObjectValue"));
				}
				text = value.ToString();
			}
			if (text == null)
			{
				throw new ArgumentException(Res.GetString("Argument_ConvertToString"));
			}
			return text;
		}

		internal void ReadContentFrom(XmlReader r)
		{
			if (r.ReadState != ReadState.Interactive)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedInteractive"));
			}
			XContainer xcontainer = this;
			NamespaceCache namespaceCache = default(NamespaceCache);
			NamespaceCache namespaceCache2 = default(NamespaceCache);
			for (;;)
			{
				switch (r.NodeType)
				{
				case XmlNodeType.Element:
				{
					XElement xelement = new XElement(namespaceCache.Get(r.NamespaceURI).GetName(r.LocalName));
					if (r.MoveToFirstAttribute())
					{
						do
						{
							xelement.AppendAttributeSkipNotify(new XAttribute(namespaceCache2.Get((r.Prefix.Length == 0) ? string.Empty : r.NamespaceURI).GetName(r.LocalName), r.Value));
						}
						while (r.MoveToNextAttribute());
						r.MoveToElement();
					}
					xcontainer.AddNodeSkipNotify(xelement);
					if (!r.IsEmptyElement)
					{
						xcontainer = xelement;
						goto IL_0201;
					}
					goto IL_0201;
				}
				case XmlNodeType.Text:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					xcontainer.AddStringSkipNotify(r.Value);
					goto IL_0201;
				case XmlNodeType.CDATA:
					xcontainer.AddNodeSkipNotify(new XCData(r.Value));
					goto IL_0201;
				case XmlNodeType.EntityReference:
					if (!r.CanResolveEntity)
					{
						goto Block_8;
					}
					r.ResolveEntity();
					goto IL_0201;
				case XmlNodeType.ProcessingInstruction:
					xcontainer.AddNodeSkipNotify(new XProcessingInstruction(r.Name, r.Value));
					goto IL_0201;
				case XmlNodeType.Comment:
					xcontainer.AddNodeSkipNotify(new XComment(r.Value));
					goto IL_0201;
				case XmlNodeType.DocumentType:
					xcontainer.AddNodeSkipNotify(new XDocumentType(r.LocalName, r.GetAttribute("PUBLIC"), r.GetAttribute("SYSTEM"), r.Value, r.DtdInfo));
					goto IL_0201;
				case XmlNodeType.EndElement:
					if (xcontainer.content == null)
					{
						xcontainer.content = string.Empty;
					}
					if (xcontainer == this)
					{
						return;
					}
					xcontainer = xcontainer.parent;
					goto IL_0201;
				case XmlNodeType.EndEntity:
					goto IL_0201;
				}
				break;
				IL_0201:
				if (!r.Read())
				{
					return;
				}
			}
			goto IL_01DD;
			Block_8:
			throw new InvalidOperationException(Res.GetString("InvalidOperation_UnresolvedEntityReference"));
			IL_01DD:
			throw new InvalidOperationException(Res.GetString("InvalidOperation_UnexpectedNodeType", new object[] { r.NodeType }));
		}

		internal void ReadContentFrom(XmlReader r, LoadOptions o)
		{
			if ((o & (LoadOptions.SetBaseUri | LoadOptions.SetLineInfo)) == LoadOptions.None)
			{
				this.ReadContentFrom(r);
				return;
			}
			if (r.ReadState != ReadState.Interactive)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedInteractive"));
			}
			XContainer xcontainer = this;
			XNode xnode = null;
			NamespaceCache namespaceCache = default(NamespaceCache);
			NamespaceCache namespaceCache2 = default(NamespaceCache);
			string text = (((o & LoadOptions.SetBaseUri) != LoadOptions.None) ? r.BaseURI : null);
			IXmlLineInfo xmlLineInfo = (((o & LoadOptions.SetLineInfo) != LoadOptions.None) ? (r as IXmlLineInfo) : null);
			for (;;)
			{
				string baseURI = r.BaseURI;
				switch (r.NodeType)
				{
				case XmlNodeType.Element:
				{
					XElement xelement = new XElement(namespaceCache.Get(r.NamespaceURI).GetName(r.LocalName));
					if (text != null && text != baseURI)
					{
						xelement.SetBaseUri(baseURI);
					}
					if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
					{
						xelement.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
					}
					if (r.MoveToFirstAttribute())
					{
						do
						{
							XAttribute xattribute = new XAttribute(namespaceCache2.Get((r.Prefix.Length == 0) ? string.Empty : r.NamespaceURI).GetName(r.LocalName), r.Value);
							if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
							{
								xattribute.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
							}
							xelement.AppendAttributeSkipNotify(xattribute);
						}
						while (r.MoveToNextAttribute());
						r.MoveToElement();
					}
					xcontainer.AddNodeSkipNotify(xelement);
					if (r.IsEmptyElement)
					{
						goto IL_0305;
					}
					xcontainer = xelement;
					if (text != null)
					{
						text = baseURI;
						goto IL_0305;
					}
					goto IL_0305;
				}
				case XmlNodeType.Text:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					if ((text != null && text != baseURI) || (xmlLineInfo != null && xmlLineInfo.HasLineInfo()))
					{
						xnode = new XText(r.Value);
						goto IL_0305;
					}
					xcontainer.AddStringSkipNotify(r.Value);
					goto IL_0305;
				case XmlNodeType.CDATA:
					xnode = new XCData(r.Value);
					goto IL_0305;
				case XmlNodeType.EntityReference:
					if (!r.CanResolveEntity)
					{
						goto Block_25;
					}
					r.ResolveEntity();
					goto IL_0305;
				case XmlNodeType.ProcessingInstruction:
					xnode = new XProcessingInstruction(r.Name, r.Value);
					goto IL_0305;
				case XmlNodeType.Comment:
					xnode = new XComment(r.Value);
					goto IL_0305;
				case XmlNodeType.DocumentType:
					xnode = new XDocumentType(r.LocalName, r.GetAttribute("PUBLIC"), r.GetAttribute("SYSTEM"), r.Value, r.DtdInfo);
					goto IL_0305;
				case XmlNodeType.EndElement:
				{
					if (xcontainer.content == null)
					{
						xcontainer.content = string.Empty;
					}
					XElement xelement2 = xcontainer as XElement;
					if (xelement2 != null && xmlLineInfo != null && xmlLineInfo.HasLineInfo())
					{
						xelement2.SetEndElementLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
					}
					if (xcontainer == this)
					{
						return;
					}
					if (text != null && xcontainer.HasBaseUri)
					{
						text = xcontainer.parent.BaseUri;
					}
					xcontainer = xcontainer.parent;
					goto IL_0305;
				}
				case XmlNodeType.EndEntity:
					goto IL_0305;
				}
				break;
				IL_0305:
				if (xnode != null)
				{
					if (text != null && text != baseURI)
					{
						xnode.SetBaseUri(baseURI);
					}
					if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
					{
						xnode.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
					}
					xcontainer.AddNodeSkipNotify(xnode);
					xnode = null;
				}
				if (!r.Read())
				{
					return;
				}
			}
			goto IL_02E1;
			Block_25:
			throw new InvalidOperationException(Res.GetString("InvalidOperation_UnresolvedEntityReference"));
			IL_02E1:
			throw new InvalidOperationException(Res.GetString("InvalidOperation_UnexpectedNodeType", new object[] { r.NodeType }));
		}

		internal void RemoveNode(XNode n)
		{
			bool flag = base.NotifyChanging(n, XObjectChangeEventArgs.Remove);
			if (n.parent != this)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
			}
			XNode xnode = (XNode)this.content;
			while (xnode.next != n)
			{
				xnode = xnode.next;
			}
			if (xnode == n)
			{
				this.content = null;
			}
			else
			{
				if (this.content == n)
				{
					this.content = xnode;
				}
				xnode.next = n.next;
			}
			n.parent = null;
			n.next = null;
			if (flag)
			{
				base.NotifyChanged(n, XObjectChangeEventArgs.Remove);
			}
		}

		private void RemoveNodesSkipNotify()
		{
			XNode xnode = this.content as XNode;
			if (xnode != null)
			{
				do
				{
					XNode next = xnode.next;
					xnode.parent = null;
					xnode.next = null;
					xnode = next;
				}
				while (xnode != this.content);
			}
			this.content = null;
		}

		internal virtual void ValidateNode(XNode node, XNode previous)
		{
		}

		internal virtual void ValidateString(string s)
		{
		}

		internal void WriteContentTo(XmlWriter writer)
		{
			if (this.content != null)
			{
				if (this.content is string)
				{
					if (this is XDocument)
					{
						writer.WriteWhitespace((string)this.content);
						return;
					}
					writer.WriteString((string)this.content);
					return;
				}
				else
				{
					XNode xnode = (XNode)this.content;
					do
					{
						xnode = xnode.next;
						xnode.WriteTo(writer);
					}
					while (xnode != this.content);
				}
			}
		}

		private static void AddContentToList(List<object> list, object content)
		{
			IEnumerable enumerable = ((content is string) ? null : (content as IEnumerable));
			if (enumerable == null)
			{
				list.Add(content);
				return;
			}
			foreach (object obj in enumerable)
			{
				if (obj != null)
				{
					XContainer.AddContentToList(list, obj);
				}
			}
		}

		internal static object GetContentSnapshot(object content)
		{
			if (content is string || !(content is IEnumerable))
			{
				return content;
			}
			List<object> list = new List<object>();
			XContainer.AddContentToList(list, content);
			return list;
		}

		internal object content;
	}
}
