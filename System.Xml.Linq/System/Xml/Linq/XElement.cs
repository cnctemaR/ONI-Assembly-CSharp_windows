using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using MS.Internal.Xml.Linq.ComponentModel;

namespace System.Xml.Linq
{
	[XmlSchemaProvider(null, IsAny = true)]
	[TypeDescriptionProvider(typeof(XTypeDescriptionProvider<XElement>))]
	public class XElement : XContainer, IXmlSerializable
	{
		public static IEnumerable<XElement> EmptySequence
		{
			get
			{
				if (XElement.emptySequence == null)
				{
					XElement.emptySequence = new XElement[0];
				}
				return XElement.emptySequence;
			}
		}

		public XElement(XName name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
		}

		public XElement(XName name, object content)
			: this(name)
		{
			base.AddContentSkipNotify(content);
		}

		public XElement(XName name, params object[] content)
			: this(name, content)
		{
		}

		public XElement(XElement other)
			: base(other)
		{
			this.name = other.name;
			XAttribute next = other.lastAttr;
			if (next != null)
			{
				do
				{
					next = next.next;
					this.AppendAttributeSkipNotify(new XAttribute(next));
				}
				while (next != other.lastAttr);
			}
		}

		public XElement(XStreamingElement other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.name = other.name;
			base.AddContentSkipNotify(other.content);
		}

		internal XElement()
			: this("default")
		{
		}

		internal XElement(XmlReader r)
			: this(r, LoadOptions.None)
		{
		}

		internal XElement(XmlReader r, LoadOptions o)
		{
			this.ReadElementFrom(r, o);
		}

		private static object ConvertForAssignment(object value)
		{
			XmlNode xmlNode = value as XmlNode;
			if (xmlNode == null)
			{
				return value;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.ImportNode(xmlNode, true));
			return XElement.Parse(xmlDocument.InnerXml);
		}

		public XAttribute FirstAttribute
		{
			get
			{
				if (this.lastAttr == null)
				{
					return null;
				}
				return this.lastAttr.next;
			}
		}

		public bool HasAttributes
		{
			get
			{
				return this.lastAttr != null;
			}
		}

		public bool HasElements
		{
			get
			{
				XNode xnode = this.content as XNode;
				if (xnode != null)
				{
					while (!(xnode is XElement))
					{
						xnode = xnode.next;
						if (xnode == this.content)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.content == null;
			}
		}

		public XAttribute LastAttribute
		{
			get
			{
				return this.lastAttr;
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
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Name);
				this.name = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Name);
				}
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
				if (this.content == null)
				{
					return string.Empty;
				}
				string text = this.content as string;
				if (text != null)
				{
					return text;
				}
				StringBuilder stringBuilder = new StringBuilder();
				this.AppendText(stringBuilder);
				return stringBuilder.ToString();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.RemoveNodes();
				base.Add(value);
			}
		}

		public IEnumerable<XElement> AncestorsAndSelf()
		{
			return base.GetAncestors(null, true);
		}

		public IEnumerable<XElement> AncestorsAndSelf(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return base.GetAncestors(name, true);
		}

		public XAttribute Attribute(XName name)
		{
			XAttribute next = this.lastAttr;
			if (next != null)
			{
				for (;;)
				{
					next = next.next;
					if (next.name == name)
					{
						break;
					}
					if (next == this.lastAttr)
					{
						goto IL_002A;
					}
				}
				return next;
			}
			IL_002A:
			return null;
		}

		public IEnumerable<XAttribute> Attributes()
		{
			return this.GetAttributes(null);
		}

		public IEnumerable<XAttribute> Attributes(XName name)
		{
			if (!(name != null))
			{
				return XAttribute.EmptySequence;
			}
			return this.GetAttributes(name);
		}

		public IEnumerable<XNode> DescendantNodesAndSelf()
		{
			return base.GetDescendantNodes(true);
		}

		public IEnumerable<XElement> DescendantsAndSelf()
		{
			return base.GetDescendants(null, true);
		}

		public IEnumerable<XElement> DescendantsAndSelf(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return base.GetDescendants(name, true);
		}

		public XNamespace GetDefaultNamespace()
		{
			string namespaceOfPrefixInScope = this.GetNamespaceOfPrefixInScope("xmlns", null);
			if (namespaceOfPrefixInScope == null)
			{
				return XNamespace.None;
			}
			return XNamespace.Get(namespaceOfPrefixInScope);
		}

		public XNamespace GetNamespaceOfPrefix(string prefix)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (prefix.Length == 0)
			{
				throw new ArgumentException(Res.GetString("Argument_InvalidPrefix", new object[] { prefix }));
			}
			if (prefix == "xmlns")
			{
				return XNamespace.Xmlns;
			}
			string namespaceOfPrefixInScope = this.GetNamespaceOfPrefixInScope(prefix, null);
			if (namespaceOfPrefixInScope != null)
			{
				return XNamespace.Get(namespaceOfPrefixInScope);
			}
			if (prefix == "xml")
			{
				return XNamespace.Xml;
			}
			return null;
		}

		public string GetPrefixOfNamespace(XNamespace ns)
		{
			if (ns == null)
			{
				throw new ArgumentNullException("ns");
			}
			string namespaceName = ns.NamespaceName;
			bool flag = false;
			XElement xelement = this;
			XAttribute next;
			for (;;)
			{
				next = xelement.lastAttr;
				if (next != null)
				{
					bool flag2 = false;
					do
					{
						next = next.next;
						if (next.IsNamespaceDeclaration)
						{
							if (next.Value == namespaceName && next.Name.NamespaceName.Length != 0 && (!flag || this.GetNamespaceOfPrefixInScope(next.Name.LocalName, xelement) == null))
							{
								goto IL_0072;
							}
							flag2 = true;
						}
					}
					while (next != xelement.lastAttr);
					flag = flag || flag2;
				}
				xelement = xelement.parent as XElement;
				if (xelement == null)
				{
					goto Block_8;
				}
			}
			IL_0072:
			return next.Name.LocalName;
			Block_8:
			if (namespaceName == "http://www.w3.org/XML/1998/namespace")
			{
				if (!flag || this.GetNamespaceOfPrefixInScope("xml", null) == null)
				{
					return "xml";
				}
			}
			else if (namespaceName == "http://www.w3.org/2000/xmlns/")
			{
				return "xmlns";
			}
			return null;
		}

		public static XElement Load(string uri)
		{
			return XElement.Load(uri, LoadOptions.None);
		}

		public static XElement Load(string uri, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XElement xelement;
			using (XmlReader xmlReader = XmlReader.Create(uri, xmlReaderSettings))
			{
				xelement = XElement.Load(xmlReader, options);
			}
			return xelement;
		}

		public static XElement Load(Stream stream)
		{
			return XElement.Load(stream, LoadOptions.None);
		}

		public static XElement Load(Stream stream, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XElement xelement;
			using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
			{
				xelement = XElement.Load(xmlReader, options);
			}
			return xelement;
		}

		public static XElement Load(TextReader textReader)
		{
			return XElement.Load(textReader, LoadOptions.None);
		}

		public static XElement Load(TextReader textReader, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XElement xelement;
			using (XmlReader xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
			{
				xelement = XElement.Load(xmlReader, options);
			}
			return xelement;
		}

		public static XElement Load(XmlReader reader)
		{
			return XElement.Load(reader, LoadOptions.None);
		}

		public static XElement Load(XmlReader reader, LoadOptions options)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.MoveToContent() != XmlNodeType.Element)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedNodeType", new object[]
				{
					XmlNodeType.Element,
					reader.NodeType
				}));
			}
			XElement xelement = new XElement(reader, options);
			reader.MoveToContent();
			if (!reader.EOF)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedEndOfFile"));
			}
			return xelement;
		}

		public static XElement Parse(string text)
		{
			return XElement.Parse(text, LoadOptions.None);
		}

		public static XElement Parse(string text, LoadOptions options)
		{
			XElement xelement;
			using (StringReader stringReader = new StringReader(text))
			{
				XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
				using (XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings))
				{
					xelement = XElement.Load(xmlReader, options);
				}
			}
			return xelement;
		}

		public void RemoveAll()
		{
			this.RemoveAttributes();
			base.RemoveNodes();
		}

		public void RemoveAttributes()
		{
			if (base.SkipNotify())
			{
				this.RemoveAttributesSkipNotify();
				return;
			}
			while (this.lastAttr != null)
			{
				XAttribute next = this.lastAttr.next;
				base.NotifyChanging(next, XObjectChangeEventArgs.Remove);
				if (this.lastAttr == null || next != this.lastAttr.next)
				{
					throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
				}
				if (next != this.lastAttr)
				{
					this.lastAttr.next = next.next;
				}
				else
				{
					this.lastAttr = null;
				}
				next.parent = null;
				next.next = null;
				base.NotifyChanged(next, XObjectChangeEventArgs.Remove);
			}
		}

		public void ReplaceAll(object content)
		{
			content = XContainer.GetContentSnapshot(content);
			this.RemoveAll();
			base.Add(content);
		}

		public void ReplaceAll(params object[] content)
		{
			this.ReplaceAll(content);
		}

		public void ReplaceAttributes(object content)
		{
			content = XContainer.GetContentSnapshot(content);
			this.RemoveAttributes();
			base.Add(content);
		}

		public void ReplaceAttributes(params object[] content)
		{
			this.ReplaceAttributes(content);
		}

		public void Save(string fileName)
		{
			this.Save(fileName, base.GetSaveOptionsFromAnnotations());
		}

		public void Save(string fileName, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(Stream stream)
		{
			this.Save(stream, base.GetSaveOptionsFromAnnotations());
		}

		public void Save(Stream stream, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(TextWriter textWriter)
		{
			this.Save(textWriter, base.GetSaveOptionsFromAnnotations());
		}

		public void Save(TextWriter textWriter, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteStartDocument();
			this.WriteTo(writer);
			writer.WriteEndDocument();
		}

		public void SetAttributeValue(XName name, object value)
		{
			XAttribute xattribute = this.Attribute(name);
			if (value == null)
			{
				if (xattribute != null)
				{
					this.RemoveAttribute(xattribute);
					return;
				}
			}
			else
			{
				if (xattribute != null)
				{
					xattribute.Value = XContainer.GetStringValue(value);
					return;
				}
				this.AppendAttribute(new XAttribute(name, value));
			}
		}

		public void SetElementValue(XName name, object value)
		{
			XElement xelement = base.Element(name);
			if (value == null)
			{
				if (xelement != null)
				{
					base.RemoveNode(xelement);
					return;
				}
			}
			else
			{
				if (xelement != null)
				{
					xelement.Value = XContainer.GetStringValue(value);
					return;
				}
				base.AddNode(new XElement(name, XContainer.GetStringValue(value)));
			}
		}

		public void SetValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Value = XContainer.GetStringValue(value);
		}

		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			new ElementWriter(writer).WriteElement(this);
		}

		[CLSCompliant(false)]
		public static explicit operator string(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return element.Value;
		}

		[CLSCompliant(false)]
		public static explicit operator bool(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToBoolean(element.Value.ToLower(CultureInfo.InvariantCulture));
		}

		[CLSCompliant(false)]
		public static explicit operator bool?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new bool?(XmlConvert.ToBoolean(element.Value.ToLower(CultureInfo.InvariantCulture)));
		}

		[CLSCompliant(false)]
		public static explicit operator int(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToInt32(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator int?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new int?(XmlConvert.ToInt32(element.Value));
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
			return new uint?(XmlConvert.ToUInt32(element.Value));
		}

		[CLSCompliant(false)]
		public static explicit operator long(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToInt64(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator long?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new long?(XmlConvert.ToInt64(element.Value));
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
			return new ulong?(XmlConvert.ToUInt64(element.Value));
		}

		[CLSCompliant(false)]
		public static explicit operator float(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToSingle(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator float?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new float?(XmlConvert.ToSingle(element.Value));
		}

		[CLSCompliant(false)]
		public static explicit operator double(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToDouble(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator double?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new double?(XmlConvert.ToDouble(element.Value));
		}

		[CLSCompliant(false)]
		public static explicit operator decimal(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToDecimal(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator decimal?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new decimal?(XmlConvert.ToDecimal(element.Value));
		}

		[CLSCompliant(false)]
		public static explicit operator DateTime(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return DateTime.Parse(element.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		}

		[CLSCompliant(false)]
		public static explicit operator DateTime?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new DateTime?(DateTime.Parse(element.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
		}

		[CLSCompliant(false)]
		public static explicit operator DateTimeOffset(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToDateTimeOffset(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator DateTimeOffset?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new DateTimeOffset?(XmlConvert.ToDateTimeOffset(element.Value));
		}

		[CLSCompliant(false)]
		public static explicit operator TimeSpan(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToTimeSpan(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator TimeSpan?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new TimeSpan?(XmlConvert.ToTimeSpan(element.Value));
		}

		[CLSCompliant(false)]
		public static explicit operator Guid(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToGuid(element.Value);
		}

		[CLSCompliant(false)]
		public static explicit operator Guid?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new Guid?(XmlConvert.ToGuid(element.Value));
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this.parent != null || this.annotations != null || this.content != null || this.lastAttr != null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_DeserializeInstance"));
			}
			if (reader.MoveToContent() != XmlNodeType.Element)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedNodeType", new object[]
				{
					XmlNodeType.Element,
					reader.NodeType
				}));
			}
			this.ReadElementFrom(reader, LoadOptions.None);
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			this.WriteTo(writer);
		}

		internal override void AddAttribute(XAttribute a)
		{
			if (this.Attribute(a.Name) != null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_DuplicateAttribute"));
			}
			if (a.parent != null)
			{
				a = new XAttribute(a);
			}
			this.AppendAttribute(a);
		}

		internal override void AddAttributeSkipNotify(XAttribute a)
		{
			if (this.Attribute(a.Name) != null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_DuplicateAttribute"));
			}
			if (a.parent != null)
			{
				a = new XAttribute(a);
			}
			this.AppendAttributeSkipNotify(a);
		}

		internal void AppendAttribute(XAttribute a)
		{
			bool flag = base.NotifyChanging(a, XObjectChangeEventArgs.Add);
			if (a.parent != null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
			}
			this.AppendAttributeSkipNotify(a);
			if (flag)
			{
				base.NotifyChanged(a, XObjectChangeEventArgs.Add);
			}
		}

		internal void AppendAttributeSkipNotify(XAttribute a)
		{
			a.parent = this;
			if (this.lastAttr == null)
			{
				a.next = a;
			}
			else
			{
				a.next = this.lastAttr.next;
				this.lastAttr.next = a;
			}
			this.lastAttr = a;
		}

		private bool AttributesEqual(XElement e)
		{
			XAttribute next = this.lastAttr;
			XAttribute next2 = e.lastAttr;
			if (next != null && next2 != null)
			{
				for (;;)
				{
					next = next.next;
					next2 = next2.next;
					if (next.name != next2.name || next.value != next2.value)
					{
						break;
					}
					if (next == this.lastAttr)
					{
						goto Block_3;
					}
				}
				return false;
				Block_3:
				return next2 == e.lastAttr;
			}
			return next == null && next2 == null;
		}

		internal override XNode CloneNode()
		{
			return new XElement(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XElement xelement = node as XElement;
			return xelement != null && this.name == xelement.name && base.ContentsEqual(xelement) && this.AttributesEqual(xelement);
		}

		private IEnumerable<XAttribute> GetAttributes(XName name)
		{
			XAttribute a = this.lastAttr;
			if (a != null)
			{
				do
				{
					a = a.next;
					if (name == null || a.name == name)
					{
						yield return a;
					}
				}
				while (a.parent == this && a != this.lastAttr);
			}
			yield break;
		}

		private string GetNamespaceOfPrefixInScope(string prefix, XElement outOfScope)
		{
			for (XElement xelement = this; xelement != outOfScope; xelement = xelement.parent as XElement)
			{
				XAttribute next = xelement.lastAttr;
				if (next != null)
				{
					for (;;)
					{
						next = next.next;
						if (next.IsNamespaceDeclaration && next.Name.LocalName == prefix)
						{
							break;
						}
						if (next == xelement.lastAttr)
						{
							goto IL_0040;
						}
					}
					return next.Value;
				}
				IL_0040:;
			}
			return null;
		}

		internal override int GetDeepHashCode()
		{
			int num = this.name.GetHashCode();
			num ^= base.ContentsHashCode();
			XAttribute next = this.lastAttr;
			if (next != null)
			{
				do
				{
					next = next.next;
					num ^= next.GetDeepHashCode();
				}
				while (next != this.lastAttr);
			}
			return num;
		}

		private void ReadElementFrom(XmlReader r, LoadOptions o)
		{
			if (r.ReadState != ReadState.Interactive)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedInteractive"));
			}
			this.name = XNamespace.Get(r.NamespaceURI).GetName(r.LocalName);
			if ((o & LoadOptions.SetBaseUri) != LoadOptions.None)
			{
				string baseURI = r.BaseURI;
				if (baseURI != null && baseURI.Length != 0)
				{
					base.SetBaseUri(baseURI);
				}
			}
			IXmlLineInfo xmlLineInfo = null;
			if ((o & LoadOptions.SetLineInfo) != LoadOptions.None)
			{
				xmlLineInfo = r as IXmlLineInfo;
				if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
				{
					base.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
				}
			}
			if (r.MoveToFirstAttribute())
			{
				do
				{
					XAttribute xattribute = new XAttribute(XNamespace.Get((r.Prefix.Length == 0) ? string.Empty : r.NamespaceURI).GetName(r.LocalName), r.Value);
					if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
					{
						xattribute.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
					}
					this.AppendAttributeSkipNotify(xattribute);
				}
				while (r.MoveToNextAttribute());
				r.MoveToElement();
			}
			if (!r.IsEmptyElement)
			{
				r.Read();
				base.ReadContentFrom(r, o);
			}
			r.Read();
		}

		internal void RemoveAttribute(XAttribute a)
		{
			bool flag = base.NotifyChanging(a, XObjectChangeEventArgs.Remove);
			if (a.parent != this)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
			}
			XAttribute xattribute = this.lastAttr;
			XAttribute next;
			while ((next = xattribute.next) != a)
			{
				xattribute = next;
			}
			if (xattribute == a)
			{
				this.lastAttr = null;
			}
			else
			{
				if (this.lastAttr == a)
				{
					this.lastAttr = xattribute;
				}
				xattribute.next = a.next;
			}
			a.parent = null;
			a.next = null;
			if (flag)
			{
				base.NotifyChanged(a, XObjectChangeEventArgs.Remove);
			}
		}

		private void RemoveAttributesSkipNotify()
		{
			if (this.lastAttr != null)
			{
				XAttribute xattribute = this.lastAttr;
				do
				{
					XAttribute next = xattribute.next;
					xattribute.parent = null;
					xattribute.next = null;
					xattribute = next;
				}
				while (xattribute != this.lastAttr);
				this.lastAttr = null;
			}
		}

		internal void SetEndElementLineInfo(int lineNumber, int linePosition)
		{
			base.AddAnnotation(new LineInfoEndElementAnnotation(lineNumber, linePosition));
		}

		internal override void ValidateNode(XNode node, XNode previous)
		{
			if (node is XDocument)
			{
				throw new ArgumentException(Res.GetString("Argument_AddNode", new object[] { XmlNodeType.Document }));
			}
			if (node is XDocumentType)
			{
				throw new ArgumentException(Res.GetString("Argument_AddNode", new object[] { XmlNodeType.DocumentType }));
			}
		}

		private static IEnumerable<XElement> emptySequence;

		internal XName name;

		internal XAttribute lastAttr;
	}
}
