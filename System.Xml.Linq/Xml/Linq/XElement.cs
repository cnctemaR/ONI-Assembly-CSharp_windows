using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Xml.Linq
{
	[XmlSchemaProvider(null, IsAny = true)]
	public class XElement : XContainer, IXmlSerializable
	{
		public XElement(XName name, object value)
		{
			this.name = name;
			base.Add(value);
		}

		public XElement(XElement source)
		{
			this.name = source.name;
			base.Add(source.Attributes());
			base.Add(source.Nodes());
		}

		public XElement(XName name)
		{
			this.name = name;
		}

		public XElement(XName name, params object[] contents)
		{
			this.name = name;
			base.Add(contents);
		}

		public XElement(XStreamingElement source)
		{
			this.name = source.Name;
			base.Add(source.Contents);
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			this.Save(writer);
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			base.ReadContentFrom(reader, LoadOptions.None);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		public static IEnumerable<XElement> EmptySequence
		{
			get
			{
				return XElement.emptySequence;
			}
		}

		public XAttribute FirstAttribute
		{
			get
			{
				return this.attr_first;
			}
			internal set
			{
				this.attr_first = value;
			}
		}

		public XAttribute LastAttribute
		{
			get
			{
				return this.attr_last;
			}
			internal set
			{
				this.attr_last = value;
			}
		}

		public bool HasAttributes
		{
			get
			{
				return this.attr_first != null;
			}
		}

		public bool HasElements
		{
			get
			{
				foreach (object obj in base.Nodes())
				{
					if (obj is XElement)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return !base.Nodes().GetEnumerator().MoveNext() && this.explicit_is_empty;
			}
			internal set
			{
				this.explicit_is_empty = value;
			}
		}

		public XName Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.name == null)
				{
					throw new ArgumentNullException("value");
				}
				this.name = value;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Element;
			}
		}

		public string Value
		{
			get
			{
				StringBuilder stringBuilder = null;
				foreach (XNode xnode in base.Nodes())
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					if (xnode is XText)
					{
						stringBuilder.Append(((XText)xnode).Value);
					}
					else if (xnode is XElement)
					{
						stringBuilder.Append(((XElement)xnode).Value);
					}
				}
				return (stringBuilder != null) ? stringBuilder.ToString() : string.Empty;
			}
			set
			{
				base.RemoveNodes();
				base.Add(value);
			}
		}

		private IEnumerable<XElement> GetAncestorList(XName name, bool getMeIn)
		{
			List<XElement> list = new List<XElement>();
			if (getMeIn)
			{
				list.Add(this);
			}
			for (XElement xelement = base.Parent; xelement != null; xelement = xelement.Parent)
			{
				if (name == null || xelement.Name == name)
				{
					list.Add(xelement);
				}
			}
			return list;
		}

		public XAttribute Attribute(XName name)
		{
			foreach (XAttribute xattribute in this.Attributes())
			{
				if (xattribute.Name == name)
				{
					return xattribute;
				}
			}
			return null;
		}

		public IEnumerable<XAttribute> Attributes()
		{
			XAttribute next;
			for (XAttribute a = this.attr_first; a != null; a = next)
			{
				next = a.NextAttribute;
				yield return a;
			}
			yield break;
		}

		public IEnumerable<XAttribute> Attributes(XName name)
		{
			foreach (XAttribute a in this.Attributes())
			{
				if (a.Name == name)
				{
					yield return a;
				}
			}
			yield break;
		}

		private static void DefineDefaultSettings(XmlReaderSettings settings, LoadOptions options)
		{
			settings.ProhibitDtd = false;
			settings.IgnoreWhitespace = (options & LoadOptions.PreserveWhitespace) == LoadOptions.None;
		}

		private static XmlReaderSettings CreateDefaultSettings(LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			XElement.DefineDefaultSettings(xmlReaderSettings, options);
			return xmlReaderSettings;
		}

		public static XElement Load(string uri)
		{
			return XElement.Load(uri, LoadOptions.None);
		}

		public static XElement Load(string uri, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XElement.CreateDefaultSettings(options);
			XElement xelement;
			using (XmlReader xmlReader = XmlReader.Create(uri, xmlReaderSettings))
			{
				xelement = XElement.LoadCore(xmlReader, options);
			}
			return xelement;
		}

		public static XElement Load(TextReader tr)
		{
			return XElement.Load(tr, LoadOptions.None);
		}

		public static XElement Load(TextReader tr, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XElement.CreateDefaultSettings(options);
			XElement xelement;
			using (XmlReader xmlReader = XmlReader.Create(tr, xmlReaderSettings))
			{
				xelement = XElement.LoadCore(xmlReader, options);
			}
			return xelement;
		}

		public static XElement Load(XmlReader reader)
		{
			return XElement.Load(reader, LoadOptions.None);
		}

		public static XElement Load(XmlReader reader, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = ((reader.Settings == null) ? new XmlReaderSettings() : reader.Settings.Clone());
			XElement.DefineDefaultSettings(xmlReaderSettings, options);
			XElement xelement;
			using (XmlReader xmlReader = XmlReader.Create(reader, xmlReaderSettings))
			{
				xelement = XElement.LoadCore(xmlReader, options);
			}
			return xelement;
		}

		internal static XElement LoadCore(XmlReader r, LoadOptions options)
		{
			r.MoveToContent();
			if (r.NodeType != XmlNodeType.Element)
			{
				throw new InvalidOperationException("The XmlReader must be positioned at an element");
			}
			XName xname = XName.Get(r.LocalName, r.NamespaceURI);
			XElement xelement = new XElement(xname);
			xelement.FillLineInfoAndBaseUri(r, options);
			if (r.MoveToFirstAttribute())
			{
				do
				{
					if (r.LocalName == "xmlns" && r.NamespaceURI == XNamespace.Xmlns.NamespaceName)
					{
						xelement.SetAttributeValue(XNamespace.None.GetName("xmlns"), r.Value);
					}
					else
					{
						xelement.SetAttributeValue(XName.Get(r.LocalName, r.NamespaceURI), r.Value);
					}
					xelement.LastAttribute.FillLineInfoAndBaseUri(r, options);
				}
				while (r.MoveToNextAttribute());
				r.MoveToElement();
			}
			if (!r.IsEmptyElement)
			{
				r.Read();
				xelement.ReadContentFrom(r, options);
				r.ReadEndElement();
				xelement.explicit_is_empty = false;
			}
			else
			{
				xelement.explicit_is_empty = true;
				r.Read();
			}
			return xelement;
		}

		public static XElement Parse(string s)
		{
			return XElement.Parse(s, LoadOptions.None);
		}

		public static XElement Parse(string s, LoadOptions options)
		{
			return XElement.Load(new StringReader(s), options);
		}

		public void RemoveAll()
		{
			this.RemoveAttributes();
			base.RemoveNodes();
		}

		public void RemoveAttributes()
		{
			while (this.attr_first != null)
			{
				this.attr_last.Remove();
			}
		}

		public void Save(string filename)
		{
			this.Save(filename, SaveOptions.None);
		}

		public void Save(string filename, SaveOptions options)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(filename, new XmlWriterSettings
			{
				Indent = (options != SaveOptions.DisableFormatting)
			}))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(TextWriter tw)
		{
			this.Save(tw, SaveOptions.None);
		}

		public void Save(TextWriter tw, SaveOptions options)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(tw, new XmlWriterSettings
			{
				Indent = (options != SaveOptions.DisableFormatting)
			}))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(XmlWriter w)
		{
			this.WriteTo(w);
		}

		public IEnumerable<XElement> AncestorsAndSelf()
		{
			return this.GetAncestorList(null, true);
		}

		public IEnumerable<XElement> AncestorsAndSelf(XName name)
		{
			return this.GetAncestorList(name, true);
		}

		public IEnumerable<XElement> DescendantsAndSelf()
		{
			List<XElement> list = new List<XElement>();
			list.Add(this);
			list.AddRange(base.Descendants());
			return list;
		}

		public IEnumerable<XElement> DescendantsAndSelf(XName name)
		{
			List<XElement> list = new List<XElement>();
			if (name == this.name)
			{
				list.Add(this);
			}
			list.AddRange(base.Descendants(name));
			return list;
		}

		public IEnumerable<XNode> DescendantNodesAndSelf()
		{
			yield return this;
			foreach (XNode node in base.DescendantNodes())
			{
				yield return node;
			}
			yield break;
		}

		public void SetAttributeValue(XName name, object value)
		{
			XAttribute xattribute = this.Attribute(name);
			if (value == null)
			{
				if (xattribute != null)
				{
					xattribute.Remove();
				}
			}
			else if (xattribute == null)
			{
				this.SetAttributeObject(new XAttribute(name, value));
			}
			else
			{
				xattribute.Value = XUtil.ToString(value);
			}
		}

		private void SetAttributeObject(XAttribute a)
		{
			a = (XAttribute)XUtil.GetDetachedObject(a);
			a.SetOwner(this);
			if (this.attr_first == null)
			{
				this.attr_first = a;
				this.attr_last = a;
			}
			else
			{
				this.attr_last.NextAttribute = a;
				a.PreviousAttribute = this.attr_last;
				this.attr_last = a;
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			string text = ((this.name.NamespaceName.Length <= 0) ? string.Empty : w.LookupPrefix(this.name.Namespace.NamespaceName));
			foreach (XAttribute xattribute in this.Attributes())
			{
				if (xattribute.IsNamespaceDeclaration && xattribute.Value == this.name.Namespace.NamespaceName)
				{
					if (xattribute.Name.Namespace == XNamespace.Xmlns)
					{
						text = xattribute.Name.LocalName;
					}
					break;
				}
			}
			w.WriteStartElement(text, this.name.LocalName, this.name.Namespace.NamespaceName);
			foreach (XAttribute xattribute2 in this.Attributes())
			{
				if (xattribute2.IsNamespaceDeclaration)
				{
					if (xattribute2.Name.Namespace == XNamespace.Xmlns)
					{
						w.WriteAttributeString("xmlns", xattribute2.Name.LocalName, XNamespace.Xmlns.NamespaceName, xattribute2.Value);
					}
					else
					{
						w.WriteAttributeString("xmlns", xattribute2.Value);
					}
				}
				else
				{
					w.WriteAttributeString(xattribute2.Name.LocalName, xattribute2.Name.Namespace.NamespaceName, xattribute2.Value);
				}
			}
			foreach (XNode xnode in base.Nodes())
			{
				xnode.WriteTo(w);
			}
			if (this.explicit_is_empty)
			{
				w.WriteEndElement();
			}
			else
			{
				w.WriteFullEndElement();
			}
		}

		public XNamespace GetDefaultNamespace()
		{
			for (XElement xelement = this; xelement != null; xelement = xelement.Parent)
			{
				foreach (XAttribute xattribute in xelement.Attributes())
				{
					if (xattribute.IsNamespaceDeclaration && xattribute.Name.Namespace == XNamespace.None)
					{
						return XNamespace.Get(xattribute.Value);
					}
				}
			}
			return XNamespace.None;
		}

		public XNamespace GetNamespaceOfPrefix(string prefix)
		{
			for (XElement xelement = this; xelement != null; xelement = xelement.Parent)
			{
				foreach (XAttribute xattribute in xelement.Attributes())
				{
					if (xattribute.IsNamespaceDeclaration && ((prefix.Length == 0 && xattribute.Name.LocalName == "xmlns") || xattribute.Name.LocalName == prefix))
					{
						return XNamespace.Get(xattribute.Value);
					}
				}
			}
			return XNamespace.None;
		}

		public string GetPrefixOfNamespace(XNamespace ns)
		{
			foreach (string text in this.GetPrefixOfNamespaceCore(ns))
			{
				if (this.GetNamespaceOfPrefix(text) == ns)
				{
					return text;
				}
			}
			return null;
		}

		private IEnumerable<string> GetPrefixOfNamespaceCore(XNamespace ns)
		{
			for (XElement el = this; el != null; el = el.Parent)
			{
				foreach (XAttribute a in el.Attributes())
				{
					if (a.IsNamespaceDeclaration && a.Value == ns.NamespaceName)
					{
						yield return (!(a.Name.Namespace == XNamespace.None)) ? a.Name.LocalName : string.Empty;
					}
				}
			}
			yield break;
		}

		public void ReplaceAll(object item)
		{
			base.RemoveNodes();
			base.Add(item);
		}

		public void ReplaceAll(params object[] items)
		{
			base.RemoveNodes();
			base.Add(items);
		}

		public void ReplaceAttributes(object item)
		{
			this.RemoveAttributes();
			base.Add(item);
		}

		public void ReplaceAttributes(params object[] items)
		{
			this.RemoveAttributes();
			base.Add(items);
		}

		public void SetElementValue(XName name, object value)
		{
			XElement xelement = new XElement(name, value);
			base.RemoveNodes();
			base.Add(xelement);
		}

		public void SetValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value is XAttribute || value is XDocument || value is XDeclaration || value is XDocumentType)
			{
				throw new ArgumentException(string.Format("Node type {0} is not allowed as element value", value.GetType()));
			}
			base.RemoveNodes();
			foreach (object obj in XUtil.ExpandArray(value))
			{
				base.Add(obj);
			}
		}

		internal override bool OnAddingObject(object o, bool rejectAttribute, XNode refNode, bool addFirst)
		{
			if (o is XDocument || o is XDocumentType || o is XDeclaration || (rejectAttribute && o is XAttribute))
			{
				throw new ArgumentException(string.Format("A node of type {0} cannot be added as a content", o.GetType()));
			}
			XAttribute xattribute = o as XAttribute;
			if (xattribute != null)
			{
				foreach (XAttribute xattribute2 in this.Attributes())
				{
					if (xattribute.Name == xattribute2.Name)
					{
						throw new InvalidOperationException(string.Format("Duplicate attribute: {0}", xattribute.Name));
					}
				}
				this.SetAttributeObject(xattribute);
				return true;
			}
			if (o is string && refNode is XText)
			{
				XText xtext = (XText)refNode;
				xtext.Value += o as string;
				return true;
			}
			return false;
		}

		public static explicit operator bool(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XUtil.ConvertToBoolean(element.Value);
		}

		public static explicit operator bool?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new bool?(XUtil.ConvertToBoolean(element.Value)) : null;
		}

		public static explicit operator DateTime(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XUtil.ToDateTime(element.Value);
		}

		public static explicit operator DateTime?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new DateTime?(XUtil.ToDateTime(element.Value)) : null;
		}

		public static explicit operator DateTimeOffset(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToDateTimeOffset(element.Value);
		}

		public static explicit operator DateTimeOffset?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new DateTimeOffset?(XmlConvert.ToDateTimeOffset(element.Value)) : null;
		}

		public static explicit operator decimal(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToDecimal(element.Value);
		}

		public static explicit operator decimal?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new decimal?(XmlConvert.ToDecimal(element.Value)) : null;
		}

		public static explicit operator double(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToDouble(element.Value);
		}

		public static explicit operator double?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new double?(XmlConvert.ToDouble(element.Value)) : null;
		}

		public static explicit operator float(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToSingle(element.Value);
		}

		public static explicit operator float?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new float?(XmlConvert.ToSingle(element.Value)) : null;
		}

		public static explicit operator Guid(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToGuid(element.Value);
		}

		public static explicit operator Guid?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new Guid?(XmlConvert.ToGuid(element.Value)) : null;
		}

		public static explicit operator int(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToInt32(element.Value);
		}

		public static explicit operator int?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new int?(XmlConvert.ToInt32(element.Value)) : null;
		}

		public static explicit operator long(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToInt64(element.Value);
		}

		public static explicit operator long?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new long?(XmlConvert.ToInt64(element.Value)) : null;
		}

		[CLSCompliant(false)]
		public static explicit operator uint(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToUInt32(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator uint?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new uint?(XmlConvert.ToUInt32(element.Value)) : null;
		}

		[CLSCompliant(false)]
		public static explicit operator ulong(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToUInt64(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator ulong?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new ulong?(XmlConvert.ToUInt64(element.Value)) : null;
		}

		public static explicit operator TimeSpan(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToTimeSpan(element.Value);
		}

		public static explicit operator TimeSpan?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return (element.Value != null) ? new TimeSpan?(XmlConvert.ToTimeSpan(element.Value)) : null;
		}

		public static explicit operator string(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return element.Value;
		}

		private static IEnumerable<XElement> emptySequence = new List<XElement>();

		private XName name;

		private XAttribute attr_first;

		private XAttribute attr_last;

		private bool explicit_is_empty = true;
	}
}
