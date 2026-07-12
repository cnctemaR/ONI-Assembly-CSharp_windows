using System;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	internal class XNodeBuilder : XmlWriter
	{
		public XNodeBuilder(XContainer container)
		{
			this._root = container;
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return new XmlWriterSettings
				{
					ConformanceLevel = ConformanceLevel.Auto
				};
			}
		}

		public override WriteState WriteState
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		public override void Close()
		{
			this._root.Add(this._content);
		}

		public override void Flush()
		{
		}

		public override string LookupPrefix(string namespaceName)
		{
			throw new NotSupportedException();
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException("This XmlWriter does not support base64 encoded data.");
		}

		public override void WriteCData(string text)
		{
			this.AddNode(new XCData(text));
		}

		public override void WriteCharEntity(char ch)
		{
			this.AddString(new string(ch, 1));
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.AddString(new string(buffer, index, count));
		}

		public override void WriteComment(string text)
		{
			this.AddNode(new XComment(text));
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.AddNode(new XDocumentType(name, pubid, sysid, subset));
		}

		public override void WriteEndAttribute()
		{
			XAttribute xattribute = new XAttribute(this._attrName, this._attrValue);
			this._attrName = null;
			this._attrValue = null;
			if (this._parent != null)
			{
				this._parent.Add(xattribute);
				return;
			}
			this.Add(xattribute);
		}

		public override void WriteEndDocument()
		{
		}

		public override void WriteEndElement()
		{
			this._parent = ((XElement)this._parent).parent;
		}

		public override void WriteEntityRef(string name)
		{
			if (name == "amp")
			{
				this.AddString("&");
				return;
			}
			if (name == "apos")
			{
				this.AddString("'");
				return;
			}
			if (name == "gt")
			{
				this.AddString(">");
				return;
			}
			if (name == "lt")
			{
				this.AddString("<");
				return;
			}
			if (!(name == "quot"))
			{
				throw new NotSupportedException("This XmlWriter does not support entity references.");
			}
			this.AddString("\"");
		}

		public override void WriteFullEndElement()
		{
			XElement xelement = (XElement)this._parent;
			if (xelement.IsEmpty)
			{
				xelement.Add(string.Empty);
			}
			this._parent = xelement.parent;
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			if (name == "xml")
			{
				return;
			}
			this.AddNode(new XProcessingInstruction(name, text));
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.AddString(new string(buffer, index, count));
		}

		public override void WriteRaw(string data)
		{
			this.AddString(data);
		}

		public override void WriteStartAttribute(string prefix, string localName, string namespaceName)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			this._attrName = XNamespace.Get((prefix.Length == 0) ? string.Empty : namespaceName).GetName(localName);
			this._attrValue = string.Empty;
		}

		public override void WriteStartDocument()
		{
		}

		public override void WriteStartDocument(bool standalone)
		{
		}

		public override void WriteStartElement(string prefix, string localName, string namespaceName)
		{
			this.AddNode(new XElement(XNamespace.Get(namespaceName).GetName(localName)));
		}

		public override void WriteString(string text)
		{
			this.AddString(text);
		}

		public override void WriteSurrogateCharEntity(char lowCh, char highCh)
		{
			this.AddString(new string(new char[] { highCh, lowCh }));
		}

		public override void WriteValue(DateTimeOffset value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		public override void WriteWhitespace(string ws)
		{
			this.AddString(ws);
		}

		private void Add(object o)
		{
			if (this._content == null)
			{
				this._content = new List<object>();
			}
			this._content.Add(o);
		}

		private void AddNode(XNode n)
		{
			if (this._parent != null)
			{
				this._parent.Add(n);
			}
			else
			{
				this.Add(n);
			}
			XContainer xcontainer = n as XContainer;
			if (xcontainer != null)
			{
				this._parent = xcontainer;
			}
		}

		private void AddString(string s)
		{
			if (s == null)
			{
				return;
			}
			if (this._attrValue != null)
			{
				this._attrValue += s;
				return;
			}
			if (this._parent != null)
			{
				this._parent.Add(s);
				return;
			}
			this.Add(s);
		}

		private List<object> _content;

		private XContainer _parent;

		private XName _attrName;

		private string _attrValue;

		private XContainer _root;
	}
}
