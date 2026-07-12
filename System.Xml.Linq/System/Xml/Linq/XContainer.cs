using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
							throw new InvalidOperationException("This operation was corrupted by external code.");
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
						throw new InvalidOperationException("This operation was corrupted by external code.");
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
				throw new InvalidOperationException("This operation was corrupted by external code.");
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
				string text = this.content as string;
				if (text != null)
				{
					this.content = text + s;
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
				throw new InvalidOperationException("This operation was corrupted by external code.");
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
			if (!string.IsNullOrEmpty(text))
			{
				XText xtext = new XText(text);
				xtext.parent = this;
				xtext.next = xtext;
				this.content = xtext;
			}
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
			string text = value as string;
			if (text != null)
			{
				return text;
			}
			if (value is double)
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
				text = XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.RoundtripKind);
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
					throw new ArgumentException("An XObject cannot be used as a value.");
				}
				text = value.ToString();
			}
			if (text == null)
			{
				throw new ArgumentException("The argument cannot be converted to a string.");
			}
			return text;
		}

		internal void ReadContentFrom(XmlReader r)
		{
			if (r.ReadState != ReadState.Interactive)
			{
				throw new InvalidOperationException("The XmlReader state should be Interactive.");
			}
			XContainer.ContentReader contentReader = new XContainer.ContentReader(this);
			while (contentReader.ReadContentFrom(this, r) && r.Read())
			{
			}
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
				throw new InvalidOperationException("The XmlReader state should be Interactive.");
			}
			XContainer.ContentReader contentReader = new XContainer.ContentReader(this, r, o);
			while (contentReader.ReadContentFrom(this, r, o) && r.Read())
			{
			}
		}

		internal async Task ReadContentFromAsync(XmlReader r, CancellationToken cancellationToken)
		{
			if (r.ReadState != ReadState.Interactive)
			{
				throw new InvalidOperationException("The XmlReader state should be Interactive.");
			}
			XContainer.ContentReader cr = new XContainer.ContentReader(this);
			bool flag;
			do
			{
				cancellationToken.ThrowIfCancellationRequested();
				flag = cr.ReadContentFrom(this, r);
				if (flag)
				{
					flag = await r.ReadAsync().ConfigureAwait(false);
				}
			}
			while (flag);
		}

		internal async Task ReadContentFromAsync(XmlReader r, LoadOptions o, CancellationToken cancellationToken)
		{
			if ((o & (LoadOptions.SetBaseUri | LoadOptions.SetLineInfo)) == LoadOptions.None)
			{
				await this.ReadContentFromAsync(r, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				if (r.ReadState != ReadState.Interactive)
				{
					throw new InvalidOperationException("The XmlReader state should be Interactive.");
				}
				XContainer.ContentReader cr = new XContainer.ContentReader(this, r, o);
				bool flag;
				do
				{
					cancellationToken.ThrowIfCancellationRequested();
					flag = cr.ReadContentFrom(this, r, o);
					if (flag)
					{
						flag = await r.ReadAsync().ConfigureAwait(false);
					}
				}
				while (flag);
			}
		}

		internal void RemoveNode(XNode n)
		{
			bool flag = base.NotifyChanging(n, XObjectChangeEventArgs.Remove);
			if (n.parent != this)
			{
				throw new InvalidOperationException("This operation was corrupted by external code.");
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
				string text = this.content as string;
				if (text != null)
				{
					if (this is XDocument)
					{
						writer.WriteWhitespace(text);
						return;
					}
					writer.WriteString(text);
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

		internal async Task WriteContentToAsync(XmlWriter writer, CancellationToken cancellationToken)
		{
			if (this.content != null)
			{
				string text = this.content as string;
				if (text != null)
				{
					cancellationToken.ThrowIfCancellationRequested();
					Task task;
					if (this is XDocument)
					{
						task = writer.WriteWhitespaceAsync(text);
					}
					else
					{
						task = writer.WriteStringAsync(text);
					}
					await task.ConfigureAwait(false);
				}
				else
				{
					XNode i = (XNode)this.content;
					do
					{
						i = i.next;
						await i.WriteToAsync(writer, cancellationToken).ConfigureAwait(false);
					}
					while (i != this.content);
					i = null;
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

		private sealed class ContentReader
		{
			public ContentReader(XContainer rootContainer)
			{
				this._currentContainer = rootContainer;
			}

			public ContentReader(XContainer rootContainer, XmlReader r, LoadOptions o)
			{
				this._currentContainer = rootContainer;
				this._baseUri = (((o & LoadOptions.SetBaseUri) != LoadOptions.None) ? r.BaseURI : null);
				this._lineInfo = (((o & LoadOptions.SetLineInfo) != LoadOptions.None) ? (r as IXmlLineInfo) : null);
			}

			public bool ReadContentFrom(XContainer rootContainer, XmlReader r)
			{
				switch (r.NodeType)
				{
				case XmlNodeType.Element:
				{
					NamespaceCache namespaceCache = this._eCache;
					XElement xelement = new XElement(namespaceCache.Get(r.NamespaceURI).GetName(r.LocalName));
					if (r.MoveToFirstAttribute())
					{
						do
						{
							XElement xelement2 = xelement;
							namespaceCache = this._aCache;
							xelement2.AppendAttributeSkipNotify(new XAttribute(namespaceCache.Get((r.Prefix.Length == 0) ? string.Empty : r.NamespaceURI).GetName(r.LocalName), r.Value));
						}
						while (r.MoveToNextAttribute());
						r.MoveToElement();
					}
					this._currentContainer.AddNodeSkipNotify(xelement);
					if (!r.IsEmptyElement)
					{
						this._currentContainer = xelement;
						return true;
					}
					return true;
				}
				case XmlNodeType.Text:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					this._currentContainer.AddStringSkipNotify(r.Value);
					return true;
				case XmlNodeType.CDATA:
					this._currentContainer.AddNodeSkipNotify(new XCData(r.Value));
					return true;
				case XmlNodeType.EntityReference:
					if (!r.CanResolveEntity)
					{
						throw new InvalidOperationException("The XmlReader cannot resolve entity references.");
					}
					r.ResolveEntity();
					return true;
				case XmlNodeType.ProcessingInstruction:
					this._currentContainer.AddNodeSkipNotify(new XProcessingInstruction(r.Name, r.Value));
					return true;
				case XmlNodeType.Comment:
					this._currentContainer.AddNodeSkipNotify(new XComment(r.Value));
					return true;
				case XmlNodeType.DocumentType:
					this._currentContainer.AddNodeSkipNotify(new XDocumentType(r.LocalName, r.GetAttribute("PUBLIC"), r.GetAttribute("SYSTEM"), r.Value));
					return true;
				case XmlNodeType.EndElement:
					if (this._currentContainer.content == null)
					{
						this._currentContainer.content = string.Empty;
					}
					if (this._currentContainer == rootContainer)
					{
						return false;
					}
					this._currentContainer = this._currentContainer.parent;
					return true;
				case XmlNodeType.EndEntity:
					return true;
				}
				throw new InvalidOperationException(global::SR.Format("The XmlReader should not be on a node of type {0}.", r.NodeType));
			}

			public bool ReadContentFrom(XContainer rootContainer, XmlReader r, LoadOptions o)
			{
				XNode xnode = null;
				string baseURI = r.BaseURI;
				switch (r.NodeType)
				{
				case XmlNodeType.Element:
				{
					NamespaceCache namespaceCache = this._eCache;
					XElement xelement = new XElement(namespaceCache.Get(r.NamespaceURI).GetName(r.LocalName));
					if (this._baseUri != null && this._baseUri != baseURI)
					{
						xelement.SetBaseUri(baseURI);
					}
					if (this._lineInfo != null && this._lineInfo.HasLineInfo())
					{
						xelement.SetLineInfo(this._lineInfo.LineNumber, this._lineInfo.LinePosition);
					}
					if (r.MoveToFirstAttribute())
					{
						do
						{
							namespaceCache = this._aCache;
							XAttribute xattribute = new XAttribute(namespaceCache.Get((r.Prefix.Length == 0) ? string.Empty : r.NamespaceURI).GetName(r.LocalName), r.Value);
							if (this._lineInfo != null && this._lineInfo.HasLineInfo())
							{
								xattribute.SetLineInfo(this._lineInfo.LineNumber, this._lineInfo.LinePosition);
							}
							xelement.AppendAttributeSkipNotify(xattribute);
						}
						while (r.MoveToNextAttribute());
						r.MoveToElement();
					}
					this._currentContainer.AddNodeSkipNotify(xelement);
					if (r.IsEmptyElement)
					{
						goto IL_032F;
					}
					this._currentContainer = xelement;
					if (this._baseUri != null)
					{
						this._baseUri = baseURI;
						goto IL_032F;
					}
					goto IL_032F;
				}
				case XmlNodeType.Text:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					if ((this._baseUri != null && this._baseUri != baseURI) || (this._lineInfo != null && this._lineInfo.HasLineInfo()))
					{
						xnode = new XText(r.Value);
						goto IL_032F;
					}
					this._currentContainer.AddStringSkipNotify(r.Value);
					goto IL_032F;
				case XmlNodeType.CDATA:
					xnode = new XCData(r.Value);
					goto IL_032F;
				case XmlNodeType.EntityReference:
					if (!r.CanResolveEntity)
					{
						throw new InvalidOperationException("The XmlReader cannot resolve entity references.");
					}
					r.ResolveEntity();
					goto IL_032F;
				case XmlNodeType.ProcessingInstruction:
					xnode = new XProcessingInstruction(r.Name, r.Value);
					goto IL_032F;
				case XmlNodeType.Comment:
					xnode = new XComment(r.Value);
					goto IL_032F;
				case XmlNodeType.DocumentType:
					xnode = new XDocumentType(r.LocalName, r.GetAttribute("PUBLIC"), r.GetAttribute("SYSTEM"), r.Value);
					goto IL_032F;
				case XmlNodeType.EndElement:
				{
					if (this._currentContainer.content == null)
					{
						this._currentContainer.content = string.Empty;
					}
					XElement xelement2 = this._currentContainer as XElement;
					if (xelement2 != null && this._lineInfo != null && this._lineInfo.HasLineInfo())
					{
						xelement2.SetEndElementLineInfo(this._lineInfo.LineNumber, this._lineInfo.LinePosition);
					}
					if (this._currentContainer == rootContainer)
					{
						return false;
					}
					if (this._baseUri != null && this._currentContainer.HasBaseUri)
					{
						this._baseUri = this._currentContainer.parent.BaseUri;
					}
					this._currentContainer = this._currentContainer.parent;
					goto IL_032F;
				}
				case XmlNodeType.EndEntity:
					goto IL_032F;
				}
				throw new InvalidOperationException(global::SR.Format("The XmlReader should not be on a node of type {0}.", r.NodeType));
				IL_032F:
				if (xnode != null)
				{
					if (this._baseUri != null && this._baseUri != baseURI)
					{
						xnode.SetBaseUri(baseURI);
					}
					if (this._lineInfo != null && this._lineInfo.HasLineInfo())
					{
						xnode.SetLineInfo(this._lineInfo.LineNumber, this._lineInfo.LinePosition);
					}
					this._currentContainer.AddNodeSkipNotify(xnode);
				}
				return true;
			}

			private readonly NamespaceCache _eCache;

			private readonly NamespaceCache _aCache;

			private readonly IXmlLineInfo _lineInfo;

			private XContainer _currentContainer;

			private string _baseUri;
		}
	}
}
