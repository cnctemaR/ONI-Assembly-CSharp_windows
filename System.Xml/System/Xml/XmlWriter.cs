using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml.XPath;

namespace System.Xml
{
	public abstract class XmlWriter : IDisposable
	{
		void IDisposable.Dispose()
		{
			this.Dispose(false);
		}

		public virtual XmlWriterSettings Settings
		{
			get
			{
				if (this.settings == null)
				{
					this.settings = new XmlWriterSettings();
				}
				return this.settings;
			}
		}

		public abstract WriteState WriteState { get; }

		public virtual string XmlLang
		{
			get
			{
				return null;
			}
		}

		public virtual XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		public abstract void Close();

		public static XmlWriter Create(Stream stream)
		{
			return XmlWriter.Create(stream, null);
		}

		public static XmlWriter Create(string file)
		{
			return XmlWriter.Create(file, null);
		}

		public static XmlWriter Create(TextWriter writer)
		{
			return XmlWriter.Create(writer, null);
		}

		public static XmlWriter Create(XmlWriter writer)
		{
			return XmlWriter.Create(writer, null);
		}

		public static XmlWriter Create(StringBuilder builder)
		{
			return XmlWriter.Create(builder, null);
		}

		public static XmlWriter Create(Stream stream, XmlWriterSettings settings)
		{
			Encoding encoding = ((settings == null) ? Encoding.UTF8 : settings.Encoding);
			return XmlWriter.Create(new StreamWriter(stream, encoding), settings);
		}

		public static XmlWriter Create(string file, XmlWriterSettings settings)
		{
			Encoding encoding = ((settings == null) ? Encoding.UTF8 : settings.Encoding);
			return XmlWriter.CreateTextWriter(new StreamWriter(file, false, encoding), settings, true);
		}

		public static XmlWriter Create(StringBuilder builder, XmlWriterSettings settings)
		{
			return XmlWriter.Create(new StringWriter(builder), settings);
		}

		public static XmlWriter Create(TextWriter writer, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return XmlWriter.CreateTextWriter(writer, settings, settings.CloseOutput);
		}

		public static XmlWriter Create(XmlWriter writer, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			writer.settings = settings;
			return writer;
		}

		private static XmlWriter CreateTextWriter(TextWriter writer, XmlWriterSettings settings, bool closeOutput)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			XmlTextWriter xmlTextWriter = new XmlTextWriter(writer, settings, closeOutput);
			return XmlWriter.Create(xmlTextWriter, settings);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.Close();
		}

		public abstract void Flush();

		public abstract string LookupPrefix(string ns);

		private void WriteAttribute(XmlReader reader, bool defattr)
		{
			if (!defattr && reader.IsDefault)
			{
				return;
			}
			this.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
			while (reader.ReadAttributeValue())
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Text:
					this.WriteString(reader.Value);
					break;
				case XmlNodeType.EntityReference:
					this.WriteEntityRef(reader.Name);
					break;
				}
			}
			this.WriteEndAttribute();
		}

		public virtual void WriteAttributes(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentException("null XmlReader specified.", "reader");
			}
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					if (nodeType != XmlNodeType.XmlDeclaration)
					{
						throw new XmlException("NodeType is not one of Element, Attribute, nor XmlDeclaration.");
					}
					this.WriteAttributeString("version", reader["version"]);
					if (reader["encoding"] != null)
					{
						this.WriteAttributeString("encoding", reader["encoding"]);
					}
					if (reader["standalone"] != null)
					{
						this.WriteAttributeString("standalone", reader["standalone"]);
					}
					return;
				}
			}
			else if (!reader.MoveToFirstAttribute())
			{
				return;
			}
			do
			{
				this.WriteAttribute(reader, defattr);
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
		}

		public void WriteAttributeString(string localName, string value)
		{
			this.WriteAttributeString(string.Empty, localName, null, value);
		}

		public void WriteAttributeString(string localName, string ns, string value)
		{
			this.WriteAttributeString(string.Empty, localName, ns, value);
		}

		public void WriteAttributeString(string prefix, string localName, string ns, string value)
		{
			this.WriteStartAttribute(prefix, localName, ns);
			if (value != null && value.Length > 0)
			{
				this.WriteString(value);
			}
			this.WriteEndAttribute();
		}

		public abstract void WriteBase64(byte[] buffer, int index, int count);

		public virtual void WriteBinHex(byte[] buffer, int index, int count)
		{
			StringWriter stringWriter = new StringWriter();
			XmlConvert.WriteBinHex(buffer, index, count, stringWriter);
			this.WriteString(stringWriter.ToString());
		}

		public abstract void WriteCData(string text);

		public abstract void WriteCharEntity(char ch);

		public abstract void WriteChars(char[] buffer, int index, int count);

		public abstract void WriteComment(string text);

		public abstract void WriteDocType(string name, string pubid, string sysid, string subset);

		public void WriteElementString(string localName, string value)
		{
			this.WriteStartElement(localName);
			if (value != null && value.Length > 0)
			{
				this.WriteString(value);
			}
			this.WriteEndElement();
		}

		public void WriteElementString(string localName, string ns, string value)
		{
			this.WriteStartElement(localName, ns);
			if (value != null && value.Length > 0)
			{
				this.WriteString(value);
			}
			this.WriteEndElement();
		}

		public void WriteElementString(string prefix, string localName, string ns, string value)
		{
			this.WriteStartElement(prefix, localName, ns);
			if (value != null && value.Length > 0)
			{
				this.WriteString(value);
			}
			this.WriteEndElement();
		}

		public abstract void WriteEndAttribute();

		public abstract void WriteEndDocument();

		public abstract void WriteEndElement();

		public abstract void WriteEntityRef(string name);

		public abstract void WriteFullEndElement();

		public virtual void WriteName(string name)
		{
			this.WriteNameInternal(name);
		}

		public virtual void WriteNmToken(string name)
		{
			this.WriteNmTokenInternal(name);
		}

		public virtual void WriteQualifiedName(string localName, string ns)
		{
			this.WriteQualifiedNameInternal(localName, ns);
		}

		internal void WriteNameInternal(string name)
		{
			ConformanceLevel conformanceLevel = this.Settings.ConformanceLevel;
			if (conformanceLevel == ConformanceLevel.Fragment || conformanceLevel == ConformanceLevel.Document)
			{
				XmlConvert.VerifyName(name);
			}
			this.WriteString(name);
		}

		internal virtual void WriteNmTokenInternal(string name)
		{
			bool flag = true;
			ConformanceLevel conformanceLevel = this.Settings.ConformanceLevel;
			if (conformanceLevel == ConformanceLevel.Fragment || conformanceLevel == ConformanceLevel.Document)
			{
				flag = XmlChar.IsNmToken(name);
			}
			if (!flag)
			{
				throw new ArgumentException("Argument name is not a valid NMTOKEN.");
			}
			this.WriteString(name);
		}

		internal void WriteQualifiedNameInternal(string localName, string ns)
		{
			if (localName == null || localName == string.Empty)
			{
				throw new ArgumentException();
			}
			if (ns == null)
			{
				ns = string.Empty;
			}
			ConformanceLevel conformanceLevel = this.Settings.ConformanceLevel;
			if (conformanceLevel == ConformanceLevel.Fragment || conformanceLevel == ConformanceLevel.Document)
			{
				XmlConvert.VerifyNCName(localName);
			}
			string text = ((ns.Length <= 0) ? string.Empty : this.LookupPrefix(ns));
			if (text == null)
			{
				throw new ArgumentException(string.Format("Namespace '{0}' is not declared.", ns));
			}
			if (text != string.Empty)
			{
				this.WriteString(text);
				this.WriteString(":");
				this.WriteString(localName);
			}
			else
			{
				this.WriteString(localName);
			}
		}

		public virtual void WriteNode(XPathNavigator navigator, bool defattr)
		{
			if (navigator == null)
			{
				throw new ArgumentNullException("navigator");
			}
			switch (navigator.NodeType)
			{
			case XPathNodeType.Root:
				if (navigator.MoveToFirstChild())
				{
					do
					{
						this.WriteNode(navigator, defattr);
					}
					while (navigator.MoveToNext());
					navigator.MoveToParent();
				}
				break;
			case XPathNodeType.Element:
				this.WriteStartElement(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
				if (navigator.MoveToFirstNamespace(XPathNamespaceScope.Local))
				{
					do
					{
						if (defattr || navigator.SchemaInfo == null || navigator.SchemaInfo.IsDefault)
						{
							this.WriteAttributeString(navigator.Prefix, (!(navigator.LocalName == string.Empty)) ? navigator.LocalName : "xmlns", "http://www.w3.org/2000/xmlns/", navigator.Value);
						}
					}
					while (navigator.MoveToNextNamespace(XPathNamespaceScope.Local));
					navigator.MoveToParent();
				}
				if (navigator.MoveToFirstAttribute())
				{
					do
					{
						if (defattr || navigator.SchemaInfo == null || navigator.SchemaInfo.IsDefault)
						{
							this.WriteAttributeString(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI, navigator.Value);
						}
					}
					while (navigator.MoveToNextAttribute());
					navigator.MoveToParent();
				}
				if (navigator.MoveToFirstChild())
				{
					do
					{
						this.WriteNode(navigator, defattr);
					}
					while (navigator.MoveToNext());
					navigator.MoveToParent();
				}
				if (navigator.IsEmptyElement)
				{
					this.WriteEndElement();
				}
				else
				{
					this.WriteFullEndElement();
				}
				break;
			case XPathNodeType.Attribute:
				break;
			case XPathNodeType.Namespace:
				break;
			case XPathNodeType.Text:
				this.WriteString(navigator.Value);
				break;
			case XPathNodeType.SignificantWhitespace:
				this.WriteWhitespace(navigator.Value);
				break;
			case XPathNodeType.Whitespace:
				this.WriteWhitespace(navigator.Value);
				break;
			case XPathNodeType.ProcessingInstruction:
				this.WriteProcessingInstruction(navigator.Name, navigator.Value);
				break;
			case XPathNodeType.Comment:
				this.WriteComment(navigator.Value);
				break;
			default:
				throw new NotSupportedException();
			}
		}

		public virtual void WriteNode(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentException();
			}
			if (reader.ReadState == ReadState.Initial)
			{
				reader.Read();
				do
				{
					this.WriteNode(reader, defattr);
				}
				while (!reader.EOF);
				return;
			}
			switch (reader.NodeType)
			{
			case XmlNodeType.None:
				goto IL_0218;
			case XmlNodeType.Element:
				this.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
				if (reader.HasAttributes)
				{
					for (int i = 0; i < reader.AttributeCount; i++)
					{
						reader.MoveToAttribute(i);
						this.WriteAttribute(reader, defattr);
					}
					reader.MoveToElement();
				}
				if (reader.IsEmptyElement)
				{
					this.WriteEndElement();
				}
				else
				{
					int depth = reader.Depth;
					reader.Read();
					if (reader.NodeType != XmlNodeType.EndElement)
					{
						do
						{
							this.WriteNode(reader, defattr);
						}
						while (depth < reader.Depth);
					}
					this.WriteFullEndElement();
				}
				goto IL_0218;
			case XmlNodeType.Attribute:
				return;
			case XmlNodeType.Text:
				this.WriteString(reader.Value);
				goto IL_0218;
			case XmlNodeType.CDATA:
				this.WriteCData(reader.Value);
				goto IL_0218;
			case XmlNodeType.EntityReference:
				this.WriteEntityRef(reader.Name);
				goto IL_0218;
			case XmlNodeType.Entity:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Notation:
				goto IL_01E0;
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.XmlDeclaration:
				this.WriteProcessingInstruction(reader.Name, reader.Value);
				goto IL_0218;
			case XmlNodeType.Comment:
				this.WriteComment(reader.Value);
				goto IL_0218;
			case XmlNodeType.DocumentType:
				this.WriteDocType(reader.Name, reader["PUBLIC"], reader["SYSTEM"], reader.Value);
				goto IL_0218;
			case XmlNodeType.Whitespace:
				break;
			case XmlNodeType.SignificantWhitespace:
				break;
			case XmlNodeType.EndElement:
				this.WriteFullEndElement();
				goto IL_0218;
			case XmlNodeType.EndEntity:
				goto IL_0218;
			default:
				goto IL_01E0;
			}
			this.WriteWhitespace(reader.Value);
			goto IL_0218;
			IL_01E0:
			throw new XmlException(string.Concat(new object[] { "Unexpected node ", reader.Name, " of type ", reader.NodeType }));
			IL_0218:
			reader.Read();
		}

		public abstract void WriteProcessingInstruction(string name, string text);

		public abstract void WriteRaw(string data);

		public abstract void WriteRaw(char[] buffer, int index, int count);

		public void WriteStartAttribute(string localName)
		{
			this.WriteStartAttribute(null, localName, null);
		}

		public void WriteStartAttribute(string localName, string ns)
		{
			this.WriteStartAttribute(null, localName, ns);
		}

		public abstract void WriteStartAttribute(string prefix, string localName, string ns);

		public abstract void WriteStartDocument();

		public abstract void WriteStartDocument(bool standalone);

		public void WriteStartElement(string localName)
		{
			this.WriteStartElement(null, localName, null);
		}

		public void WriteStartElement(string localName, string ns)
		{
			this.WriteStartElement(null, localName, ns);
		}

		public abstract void WriteStartElement(string prefix, string localName, string ns);

		public abstract void WriteString(string text);

		public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);

		public abstract void WriteWhitespace(string ws);

		public virtual void WriteValue(bool value)
		{
			this.WriteString(XQueryConvert.BooleanToString(value));
		}

		public virtual void WriteValue(DateTime value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		public virtual void WriteValue(decimal value)
		{
			this.WriteString(XQueryConvert.DecimalToString(value));
		}

		public virtual void WriteValue(double value)
		{
			this.WriteString(XQueryConvert.DoubleToString(value));
		}

		public virtual void WriteValue(int value)
		{
			this.WriteString(XQueryConvert.IntToString(value));
		}

		public virtual void WriteValue(long value)
		{
			this.WriteString(XQueryConvert.IntegerToString(value));
		}

		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value is string)
			{
				this.WriteString((string)value);
			}
			else if (value is bool)
			{
				this.WriteValue((bool)value);
			}
			else if (value is byte)
			{
				this.WriteValue((int)value);
			}
			else if (value is byte[])
			{
				this.WriteBase64((byte[])value, 0, ((byte[])value).Length);
			}
			else if (value is char[])
			{
				this.WriteChars((char[])value, 0, ((char[])value).Length);
			}
			else if (value is DateTime)
			{
				this.WriteValue((DateTime)value);
			}
			else if (value is decimal)
			{
				this.WriteValue((decimal)value);
			}
			else if (value is double)
			{
				this.WriteValue((double)value);
			}
			else if (value is short)
			{
				this.WriteValue((int)value);
			}
			else if (value is int)
			{
				this.WriteValue((int)value);
			}
			else if (value is long)
			{
				this.WriteValue((long)value);
			}
			else if (value is float)
			{
				this.WriteValue((float)value);
			}
			else if (value is TimeSpan)
			{
				this.WriteString(XmlConvert.ToString((TimeSpan)value));
			}
			else if (value is XmlQualifiedName)
			{
				XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)value;
				if (!xmlQualifiedName.Equals(XmlQualifiedName.Empty))
				{
					if (xmlQualifiedName.Namespace.Length > 0 && this.LookupPrefix(xmlQualifiedName.Namespace) == null)
					{
						throw new InvalidCastException(string.Format("The QName '{0}' cannot be written. No corresponding prefix is declared", xmlQualifiedName));
					}
					this.WriteQualifiedName(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
				}
				else
				{
					this.WriteString(string.Empty);
				}
			}
			else
			{
				if (!(value is IEnumerable))
				{
					throw new InvalidCastException(string.Format("Type '{0}' cannot be cast to string", value.GetType()));
				}
				bool flag = false;
				foreach (object obj in ((IEnumerable)value))
				{
					if (flag)
					{
						this.WriteString(" ");
					}
					else
					{
						flag = true;
					}
					this.WriteValue(obj);
				}
			}
		}

		public virtual void WriteValue(float value)
		{
			this.WriteString(XQueryConvert.FloatToString(value));
		}

		public virtual void WriteValue(string value)
		{
			this.WriteString(value);
		}

		private XmlWriterSettings settings;
	}
}
