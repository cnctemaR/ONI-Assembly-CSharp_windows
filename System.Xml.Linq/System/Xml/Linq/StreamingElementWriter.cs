using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	internal struct StreamingElementWriter
	{
		public StreamingElementWriter(XmlWriter w)
		{
			this._writer = w;
			this._element = null;
			this._attributes = new List<XAttribute>();
			this._resolver = default(NamespaceResolver);
		}

		private void FlushElement()
		{
			if (this._element != null)
			{
				this.PushElement();
				XNamespace xnamespace = this._element.Name.Namespace;
				this._writer.WriteStartElement(this.GetPrefixOfNamespace(xnamespace, true), this._element.Name.LocalName, xnamespace.NamespaceName);
				foreach (XAttribute xattribute in this._attributes)
				{
					xnamespace = xattribute.Name.Namespace;
					string localName = xattribute.Name.LocalName;
					string namespaceName = xnamespace.NamespaceName;
					this._writer.WriteAttributeString(this.GetPrefixOfNamespace(xnamespace, false), localName, (namespaceName.Length == 0 && localName == "xmlns") ? "http://www.w3.org/2000/xmlns/" : namespaceName, xattribute.Value);
				}
				this._element = null;
				this._attributes.Clear();
			}
		}

		private string GetPrefixOfNamespace(XNamespace ns, bool allowDefaultNamespace)
		{
			string namespaceName = ns.NamespaceName;
			if (namespaceName.Length == 0)
			{
				return string.Empty;
			}
			string prefixOfNamespace = this._resolver.GetPrefixOfNamespace(ns, allowDefaultNamespace);
			if (prefixOfNamespace != null)
			{
				return prefixOfNamespace;
			}
			if (namespaceName == "http://www.w3.org/XML/1998/namespace")
			{
				return "xml";
			}
			if (namespaceName == "http://www.w3.org/2000/xmlns/")
			{
				return "xmlns";
			}
			return null;
		}

		private void PushElement()
		{
			this._resolver.PushScope();
			foreach (XAttribute xattribute in this._attributes)
			{
				if (xattribute.IsNamespaceDeclaration)
				{
					this._resolver.Add((xattribute.Name.NamespaceName.Length == 0) ? string.Empty : xattribute.Name.LocalName, XNamespace.Get(xattribute.Value));
				}
			}
		}

		private void Write(object content)
		{
			if (content == null)
			{
				return;
			}
			XNode xnode = content as XNode;
			if (xnode != null)
			{
				this.WriteNode(xnode);
				return;
			}
			string text = content as string;
			if (text != null)
			{
				this.WriteString(text);
				return;
			}
			XAttribute xattribute = content as XAttribute;
			if (xattribute != null)
			{
				this.WriteAttribute(xattribute);
				return;
			}
			XStreamingElement xstreamingElement = content as XStreamingElement;
			if (xstreamingElement != null)
			{
				this.WriteStreamingElement(xstreamingElement);
				return;
			}
			object[] array = content as object[];
			if (array != null)
			{
				foreach (object obj in array)
				{
					this.Write(obj);
				}
				return;
			}
			IEnumerable enumerable = content as IEnumerable;
			if (enumerable != null)
			{
				foreach (object obj2 in enumerable)
				{
					this.Write(obj2);
				}
				return;
			}
			this.WriteString(XContainer.GetStringValue(content));
		}

		private void WriteAttribute(XAttribute a)
		{
			if (this._element == null)
			{
				throw new InvalidOperationException("An attribute cannot be written after content.");
			}
			this._attributes.Add(a);
		}

		private void WriteNode(XNode n)
		{
			this.FlushElement();
			n.WriteTo(this._writer);
		}

		internal void WriteStreamingElement(XStreamingElement e)
		{
			this.FlushElement();
			this._element = e;
			this.Write(e.content);
			this.FlushElement();
			this._writer.WriteEndElement();
			this._resolver.PopScope();
		}

		private void WriteString(string s)
		{
			this.FlushElement();
			this._writer.WriteString(s);
		}

		private XmlWriter _writer;

		private XStreamingElement _element;

		private List<XAttribute> _attributes;

		private NamespaceResolver _resolver;
	}
}
