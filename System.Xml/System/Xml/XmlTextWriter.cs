using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Xml
{
	public class XmlTextWriter : XmlWriter
	{
		public XmlTextWriter(string filename, Encoding encoding)
			: this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None), encoding)
		{
		}

		public XmlTextWriter(Stream stream, Encoding encoding)
			: this(new StreamWriter(stream, (encoding != null) ? encoding : XmlTextWriter.unmarked_utf8encoding))
		{
			this.ignore_encoding = encoding == null;
			this.Initialize(this.writer);
			this.allow_doc_fragment = true;
		}

		public XmlTextWriter(TextWriter writer)
		{
			this.close_output_stream = true;
			this.namespaces = true;
			this.newline_handling = NewLineHandling.None;
			this.elements = new XmlTextWriter.XmlNodeInfo[10];
			this.new_local_namespaces = new Stack();
			this.explicit_nsdecls = new ArrayList();
			this.indent_count = 2;
			this.indent_char = ' ';
			this.indent_string = "  ";
			this.quote_char = '"';
			base..ctor();
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.ignore_encoding = writer.Encoding == null;
			this.Initialize(writer);
			this.allow_doc_fragment = true;
		}

		internal XmlTextWriter(TextWriter writer, XmlWriterSettings settings, bool closeOutput)
		{
			this.close_output_stream = true;
			this.namespaces = true;
			this.newline_handling = NewLineHandling.None;
			this.elements = new XmlTextWriter.XmlNodeInfo[10];
			this.new_local_namespaces = new Stack();
			this.explicit_nsdecls = new ArrayList();
			this.indent_count = 2;
			this.indent_char = ' ';
			this.indent_string = "  ";
			this.quote_char = '"';
			base..ctor();
			this.v2 = true;
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			this.Initialize(writer);
			this.close_output_stream = closeOutput;
			this.allow_doc_fragment = settings.ConformanceLevel != ConformanceLevel.Document;
			switch (settings.ConformanceLevel)
			{
			case ConformanceLevel.Auto:
				this.xmldecl_state = ((!settings.OmitXmlDeclaration) ? XmlTextWriter.XmlDeclState.Allow : XmlTextWriter.XmlDeclState.Ignore);
				break;
			case ConformanceLevel.Fragment:
				this.xmldecl_state = XmlTextWriter.XmlDeclState.Prohibit;
				break;
			case ConformanceLevel.Document:
				this.xmldecl_state = ((!settings.OmitXmlDeclaration) ? XmlTextWriter.XmlDeclState.Auto : XmlTextWriter.XmlDeclState.Ignore);
				break;
			}
			if (settings.Indent)
			{
				this.Formatting = Formatting.Indented;
			}
			this.indent_string = ((settings.IndentChars != null) ? settings.IndentChars : string.Empty);
			if (settings.NewLineChars != null)
			{
				this.newline = settings.NewLineChars;
			}
			this.indent_attributes = settings.NewLineOnAttributes;
			this.check_character_validity = settings.CheckCharacters;
			this.newline_handling = settings.NewLineHandling;
			this.namespace_handling = settings.NamespaceHandling;
		}

		private void Initialize(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			XmlNameTable xmlNameTable = new NameTable();
			this.writer = writer;
			if (writer is StreamWriter)
			{
				this.base_stream = ((StreamWriter)writer).BaseStream;
			}
			this.source = writer;
			this.nsmanager = new XmlNamespaceManager(xmlNameTable);
			this.newline = writer.NewLine;
			char[] array;
			if (this.newline_handling != NewLineHandling.None)
			{
				RuntimeHelpers.InitializeArray(array = new char[5], fieldof(<PrivateImplementationDetails>.$$field-40).FieldHandle);
			}
			else
			{
				char[] array2 = new char[3];
				array2[0] = '&';
				array2[1] = '<';
				array = array2;
				array2[2] = '>';
			}
			XmlTextWriter.escaped_text_chars = array;
			XmlTextWriter.escaped_attr_chars = new char[] { '"', '&', '<', '>', '\r', '\n' };
		}

		public Formatting Formatting
		{
			get
			{
				return (!this.indent) ? Formatting.None : Formatting.Indented;
			}
			set
			{
				this.indent = value == Formatting.Indented;
			}
		}

		public int Indentation
		{
			get
			{
				return this.indent_count;
			}
			set
			{
				if (value < 0)
				{
					throw this.ArgumentError("Indentation must be non-negative integer.");
				}
				this.indent_count = value;
				this.indent_string = ((value != 0) ? new string(this.indent_char, this.indent_count) : string.Empty);
			}
		}

		public char IndentChar
		{
			get
			{
				return this.indent_char;
			}
			set
			{
				this.indent_char = value;
				this.indent_string = new string(this.indent_char, this.indent_count);
			}
		}

		public char QuoteChar
		{
			get
			{
				return this.quote_char;
			}
			set
			{
				if (this.state == WriteState.Attribute)
				{
					throw this.InvalidOperation("QuoteChar must not be changed inside attribute value.");
				}
				if (value != '\'' && value != '"')
				{
					throw this.ArgumentError("Only ' and \" are allowed as an attribute quote character.");
				}
				this.quote_char = value;
				XmlTextWriter.escaped_attr_chars[0] = this.quote_char;
			}
		}

		public override string XmlLang
		{
			get
			{
				return (this.open_count != 0) ? this.elements[this.open_count - 1].XmlLang : null;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return (this.open_count != 0) ? this.elements[this.open_count - 1].XmlSpace : XmlSpace.None;
			}
		}

		public override WriteState WriteState
		{
			get
			{
				return this.state;
			}
		}

		public override string LookupPrefix(string namespaceUri)
		{
			if (namespaceUri == null || namespaceUri == string.Empty)
			{
				throw this.ArgumentError("The Namespace cannot be empty.");
			}
			if (namespaceUri == this.nsmanager.DefaultNamespace)
			{
				return string.Empty;
			}
			return this.nsmanager.LookupPrefixExclusive(namespaceUri, false);
		}

		public Stream BaseStream
		{
			get
			{
				return this.base_stream;
			}
		}

		public override void Close()
		{
			if (this.state != WriteState.Error)
			{
				if (this.state == WriteState.Attribute)
				{
					this.WriteEndAttribute();
				}
				while (this.open_count > 0)
				{
					this.WriteEndElement();
				}
			}
			if (this.close_output_stream)
			{
				this.writer.Close();
			}
			else
			{
				this.writer.Flush();
			}
			this.state = WriteState.Closed;
		}

		public override void Flush()
		{
			this.writer.Flush();
		}

		public bool Namespaces
		{
			get
			{
				return this.namespaces;
			}
			set
			{
				if (this.state != WriteState.Start)
				{
					throw this.InvalidOperation("This property must be set before writing output.");
				}
				this.namespaces = value;
			}
		}

		public override void WriteStartDocument()
		{
			this.WriteStartDocumentCore(false, false);
			this.is_document_entity = true;
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.WriteStartDocumentCore(true, standalone);
			this.is_document_entity = true;
		}

		private void WriteStartDocumentCore(bool outputStd, bool standalone)
		{
			if (this.state != WriteState.Start)
			{
				throw this.StateError("XmlDeclaration");
			}
			switch (this.xmldecl_state)
			{
			case XmlTextWriter.XmlDeclState.Ignore:
				return;
			case XmlTextWriter.XmlDeclState.Prohibit:
				throw this.InvalidOperation("WriteStartDocument cannot be called when ConformanceLevel is Fragment.");
			}
			this.state = WriteState.Prolog;
			this.writer.Write("<?xml version=");
			this.writer.Write(this.quote_char);
			this.writer.Write("1.0");
			this.writer.Write(this.quote_char);
			if (!this.ignore_encoding)
			{
				this.writer.Write(" encoding=");
				this.writer.Write(this.quote_char);
				this.writer.Write(this.writer.Encoding.WebName);
				this.writer.Write(this.quote_char);
			}
			if (outputStd)
			{
				this.writer.Write(" standalone=");
				this.writer.Write(this.quote_char);
				this.writer.Write((!standalone) ? "no" : "yes");
				this.writer.Write(this.quote_char);
			}
			this.writer.Write("?>");
			this.xmldecl_state = XmlTextWriter.XmlDeclState.Ignore;
		}

		public override void WriteEndDocument()
		{
			WriteState writeState = this.state;
			if (writeState != WriteState.Closed && writeState != WriteState.Error && writeState != WriteState.Start)
			{
				if (this.state == WriteState.Attribute)
				{
					this.WriteEndAttribute();
				}
				while (this.open_count > 0)
				{
					this.WriteEndElement();
				}
				this.state = WriteState.Start;
				this.is_document_entity = false;
				return;
			}
			throw this.StateError("EndDocument");
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			if (name == null)
			{
				throw this.ArgumentError("name");
			}
			if (!XmlChar.IsName(name))
			{
				throw this.ArgumentError("name");
			}
			if (this.node_state != XmlNodeType.None)
			{
				throw this.StateError("DocType");
			}
			this.node_state = XmlNodeType.DocumentType;
			if (this.xmldecl_state == XmlTextWriter.XmlDeclState.Auto)
			{
				this.OutputAutoStartDocument();
			}
			this.WriteIndent();
			this.writer.Write("<!DOCTYPE ");
			this.writer.Write(name);
			if (pubid != null)
			{
				this.writer.Write(" PUBLIC ");
				this.writer.Write(this.quote_char);
				this.writer.Write(pubid);
				this.writer.Write(this.quote_char);
				this.writer.Write(' ');
				this.writer.Write(this.quote_char);
				if (sysid != null)
				{
					this.writer.Write(sysid);
				}
				this.writer.Write(this.quote_char);
			}
			else if (sysid != null)
			{
				this.writer.Write(" SYSTEM ");
				this.writer.Write(this.quote_char);
				this.writer.Write(sysid);
				this.writer.Write(this.quote_char);
			}
			if (subset != null)
			{
				this.writer.Write("[");
				this.writer.Write(subset);
				this.writer.Write("]");
			}
			this.writer.Write('>');
			this.state = WriteState.Prolog;
		}

		public override void WriteStartElement(string prefix, string localName, string namespaceUri)
		{
			if (this.state == WriteState.Error || this.state == WriteState.Closed)
			{
				throw this.StateError("StartTag");
			}
			this.node_state = XmlNodeType.Element;
			bool flag = prefix == null;
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			if (!this.namespaces && namespaceUri != null && namespaceUri.Length > 0)
			{
				throw this.ArgumentError("Namespace is disabled in this XmlTextWriter.");
			}
			if (!this.namespaces && prefix.Length > 0)
			{
				throw this.ArgumentError("Namespace prefix is disabled in this XmlTextWriter.");
			}
			if (prefix.Length > 0 && namespaceUri == null)
			{
				namespaceUri = this.nsmanager.LookupNamespace(prefix, false);
				if (namespaceUri == null || namespaceUri.Length == 0)
				{
					throw this.ArgumentError("Namespace URI must not be null when prefix is not an empty string.");
				}
			}
			if (this.namespaces && prefix != null && prefix.Length == 3 && namespaceUri != "http://www.w3.org/XML/1998/namespace" && (prefix[0] == 'x' || prefix[0] == 'X') && (prefix[1] == 'm' || prefix[1] == 'M') && (prefix[2] == 'l' || prefix[2] == 'L'))
			{
				throw new ArgumentException("A prefix cannot be equivalent to \"xml\" in case-insensitive match.");
			}
			if (this.xmldecl_state == XmlTextWriter.XmlDeclState.Auto)
			{
				this.OutputAutoStartDocument();
			}
			if (this.state == WriteState.Element)
			{
				this.CloseStartElement();
			}
			if (this.open_count > 0)
			{
				this.elements[this.open_count - 1].HasElements = true;
			}
			this.nsmanager.PushScope();
			if (this.namespaces && namespaceUri != null)
			{
				if (flag && namespaceUri.Length > 0)
				{
					prefix = this.LookupPrefix(namespaceUri);
				}
				if (prefix == null || namespaceUri.Length == 0)
				{
					prefix = string.Empty;
				}
			}
			this.WriteIndent();
			this.writer.Write("<");
			if (prefix.Length > 0)
			{
				this.writer.Write(prefix);
				this.writer.Write(':');
			}
			this.writer.Write(localName);
			if (this.elements.Length == this.open_count)
			{
				XmlTextWriter.XmlNodeInfo[] array = new XmlTextWriter.XmlNodeInfo[this.open_count << 1];
				Array.Copy(this.elements, array, this.open_count);
				this.elements = array;
			}
			if (this.elements[this.open_count] == null)
			{
				this.elements[this.open_count] = new XmlTextWriter.XmlNodeInfo();
			}
			XmlTextWriter.XmlNodeInfo xmlNodeInfo = this.elements[this.open_count];
			xmlNodeInfo.Prefix = prefix;
			xmlNodeInfo.LocalName = localName;
			xmlNodeInfo.NS = namespaceUri;
			xmlNodeInfo.HasSimple = false;
			xmlNodeInfo.HasElements = false;
			xmlNodeInfo.XmlLang = this.XmlLang;
			xmlNodeInfo.XmlSpace = this.XmlSpace;
			this.open_count++;
			if (this.namespaces && namespaceUri != null)
			{
				string text = this.nsmanager.LookupNamespace(prefix, false);
				if (text != namespaceUri)
				{
					this.nsmanager.AddNamespace(prefix, namespaceUri);
					this.new_local_namespaces.Push(prefix);
				}
			}
			this.state = WriteState.Element;
		}

		private void CloseStartElement()
		{
			this.CloseStartElementCore();
			if (this.state == WriteState.Element)
			{
				this.writer.Write('>');
			}
			this.state = WriteState.Content;
		}

		private void CloseStartElementCore()
		{
			if (this.state == WriteState.Attribute)
			{
				this.WriteEndAttribute();
			}
			if (this.new_local_namespaces.Count == 0)
			{
				if (this.explicit_nsdecls.Count > 0)
				{
					this.explicit_nsdecls.Clear();
				}
				return;
			}
			int count = this.explicit_nsdecls.Count;
			while (this.new_local_namespaces.Count > 0)
			{
				string text = (string)this.new_local_namespaces.Pop();
				bool flag = false;
				for (int i = 0; i < this.explicit_nsdecls.Count; i++)
				{
					if ((string)this.explicit_nsdecls[i] == text)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.explicit_nsdecls.Add(text);
				}
			}
			for (int j = count; j < this.explicit_nsdecls.Count; j++)
			{
				string text2 = (string)this.explicit_nsdecls[j];
				string text3 = this.nsmanager.LookupNamespace(text2, false);
				if (text3 != null)
				{
					if (text2.Length > 0)
					{
						this.writer.Write(" xmlns:");
						this.writer.Write(text2);
					}
					else
					{
						this.writer.Write(" xmlns");
					}
					this.writer.Write('=');
					this.writer.Write(this.quote_char);
					this.WriteEscapedString(text3, true);
					this.writer.Write(this.quote_char);
				}
			}
			this.explicit_nsdecls.Clear();
		}

		public override void WriteEndElement()
		{
			this.WriteEndElementCore(false);
		}

		public override void WriteFullEndElement()
		{
			this.WriteEndElementCore(true);
		}

		private void WriteEndElementCore(bool full)
		{
			if (this.state == WriteState.Error || this.state == WriteState.Closed)
			{
				throw this.StateError("EndElement");
			}
			if (this.open_count == 0)
			{
				throw this.InvalidOperation("There is no more open element.");
			}
			this.CloseStartElementCore();
			this.nsmanager.PopScope();
			if (this.state == WriteState.Element)
			{
				if (full)
				{
					this.writer.Write('>');
				}
				else
				{
					this.writer.Write(" />");
				}
			}
			if (full || this.state == WriteState.Content)
			{
				this.WriteIndentEndElement();
			}
			XmlTextWriter.XmlNodeInfo xmlNodeInfo = this.elements[--this.open_count];
			if (full || this.state == WriteState.Content)
			{
				this.writer.Write("</");
				if (xmlNodeInfo.Prefix.Length > 0)
				{
					this.writer.Write(xmlNodeInfo.Prefix);
					this.writer.Write(':');
				}
				this.writer.Write(xmlNodeInfo.LocalName);
				this.writer.Write('>');
			}
			this.state = WriteState.Content;
			if (this.open_count == 0)
			{
				this.node_state = XmlNodeType.EndElement;
			}
		}

		public override void WriteStartAttribute(string prefix, string localName, string namespaceUri)
		{
			if (this.state == WriteState.Attribute)
			{
				this.WriteEndAttribute();
			}
			if (this.state != WriteState.Element && this.state != WriteState.Start)
			{
				throw this.StateError("Attribute");
			}
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			bool flag;
			if (namespaceUri == "http://www.w3.org/2000/xmlns/")
			{
				flag = true;
				if (prefix.Length == 0 && localName != "xmlns")
				{
					prefix = "xmlns";
				}
			}
			else
			{
				flag = prefix == "xmlns" || (localName == "xmlns" && prefix.Length == 0);
			}
			if (this.namespaces)
			{
				if (prefix == "xml")
				{
					namespaceUri = "http://www.w3.org/XML/1998/namespace";
				}
				else if (namespaceUri == null)
				{
					if (flag)
					{
						namespaceUri = "http://www.w3.org/2000/xmlns/";
					}
					else
					{
						namespaceUri = string.Empty;
					}
				}
				if (flag && namespaceUri != "http://www.w3.org/2000/xmlns/")
				{
					throw this.ArgumentError(string.Format("The 'xmlns' attribute is bound to the reserved namespace '{0}'", "http://www.w3.org/2000/xmlns/"));
				}
				if (prefix.Length > 0 && namespaceUri.Length == 0)
				{
					namespaceUri = this.nsmanager.LookupNamespace(prefix, false);
					if (namespaceUri == null || namespaceUri.Length == 0)
					{
						throw this.ArgumentError("Namespace URI must not be null when prefix is not an empty string.");
					}
				}
				if (!flag && namespaceUri.Length > 0)
				{
					prefix = this.DetermineAttributePrefix(prefix, localName, namespaceUri);
				}
			}
			if (this.indent_attributes)
			{
				this.WriteIndentAttribute();
			}
			else if (this.state != WriteState.Start)
			{
				this.writer.Write(' ');
			}
			if (prefix.Length > 0)
			{
				this.writer.Write(prefix);
				this.writer.Write(':');
			}
			this.writer.Write(localName);
			this.writer.Write('=');
			this.writer.Write(this.quote_char);
			if (flag || prefix == "xml")
			{
				if (this.preserver == null)
				{
					this.preserver = new StringWriter();
				}
				else
				{
					this.preserver.GetStringBuilder().Length = 0;
				}
				this.writer = this.preserver;
				if (!flag)
				{
					this.is_preserved_xmlns = false;
					this.preserved_name = localName;
				}
				else
				{
					this.is_preserved_xmlns = true;
					this.preserved_name = ((!(localName == "xmlns")) ? localName : string.Empty);
				}
			}
			this.state = WriteState.Attribute;
		}

		private string DetermineAttributePrefix(string prefix, string local, string ns)
		{
			bool flag = false;
			if (prefix.Length == 0)
			{
				prefix = this.LookupPrefix(ns);
				if (prefix != null && prefix.Length > 0)
				{
					return prefix;
				}
				flag = true;
			}
			else
			{
				prefix = this.nsmanager.NameTable.Add(prefix);
				string text = this.nsmanager.LookupNamespace(prefix, true);
				if (text == ns)
				{
					return prefix;
				}
				if (text != null)
				{
					this.nsmanager.RemoveNamespace(prefix, text);
					if (this.nsmanager.LookupNamespace(prefix, true) != text)
					{
						flag = true;
						this.nsmanager.AddNamespace(prefix, text);
					}
				}
			}
			if (flag)
			{
				prefix = this.MockupPrefix(ns, true);
			}
			this.new_local_namespaces.Push(prefix);
			this.nsmanager.AddNamespace(prefix, ns);
			return prefix;
		}

		private string MockupPrefix(string ns, bool skipLookup)
		{
			string text = ((!skipLookup) ? this.LookupPrefix(ns) : null);
			if (text != null && text.Length > 0)
			{
				return text;
			}
			int num = 1;
			for (;;)
			{
				text = XmlTextWriter.StringUtil.Format("d{0}p{1}", new object[] { this.open_count, num });
				if (!this.new_local_namespaces.Contains(text))
				{
					if (this.nsmanager.LookupNamespace(this.nsmanager.NameTable.Get(text)) == null)
					{
						break;
					}
				}
				num++;
			}
			this.nsmanager.AddNamespace(text, ns);
			this.new_local_namespaces.Push(text);
			return text;
		}

		public override void WriteEndAttribute()
		{
			if (this.state != WriteState.Attribute)
			{
				throw this.StateError("End of attribute");
			}
			if (this.writer == this.preserver)
			{
				this.writer = this.source;
				string text = this.preserver.ToString();
				if (this.is_preserved_xmlns)
				{
					if (this.preserved_name.Length > 0 && text.Length == 0)
					{
						throw this.ArgumentError("Non-empty prefix must be mapped to non-empty namespace URI.");
					}
					string text2 = this.nsmanager.LookupNamespace(this.preserved_name, false);
					if ((this.namespace_handling & NamespaceHandling.OmitDuplicates) == NamespaceHandling.Default || text2 != text)
					{
						this.explicit_nsdecls.Add(this.preserved_name);
					}
					if (this.open_count > 0)
					{
						if (this.v2 && this.elements[this.open_count - 1].Prefix == this.preserved_name && this.elements[this.open_count - 1].NS != text)
						{
							throw new XmlException(string.Format("Cannot redefine the namespace for prefix '{0}' used at current element", this.preserved_name));
						}
						if (!(this.elements[this.open_count - 1].NS == string.Empty) || !(this.elements[this.open_count - 1].Prefix == this.preserved_name))
						{
							if (text2 != text)
							{
								this.nsmanager.AddNamespace(this.preserved_name, text);
							}
						}
					}
				}
				else
				{
					string text3 = this.preserved_name;
					if (text3 != null)
					{
						if (XmlTextWriter.<>f__switch$map3B == null)
						{
							XmlTextWriter.<>f__switch$map3B = new Dictionary<string, int>(2)
							{
								{ "lang", 0 },
								{ "space", 1 }
							};
						}
						int num;
						if (XmlTextWriter.<>f__switch$map3B.TryGetValue(text3, out num))
						{
							if (num != 0)
							{
								if (num == 1)
								{
									string text4 = text;
									if (text4 != null)
									{
										if (XmlTextWriter.<>f__switch$map3A == null)
										{
											XmlTextWriter.<>f__switch$map3A = new Dictionary<string, int>(2)
											{
												{ "default", 0 },
												{ "preserve", 1 }
											};
										}
										int num2;
										if (XmlTextWriter.<>f__switch$map3A.TryGetValue(text4, out num2))
										{
											if (num2 != 0)
											{
												if (num2 != 1)
												{
													goto IL_02C5;
												}
												if (this.open_count > 0)
												{
													this.elements[this.open_count - 1].XmlSpace = XmlSpace.Preserve;
												}
											}
											else if (this.open_count > 0)
											{
												this.elements[this.open_count - 1].XmlSpace = XmlSpace.Default;
											}
											goto IL_02D6;
										}
									}
									IL_02C5:
									throw this.ArgumentError("Invalid value for xml:space.");
								}
							}
							else if (this.open_count > 0)
							{
								this.elements[this.open_count - 1].XmlLang = text;
							}
						}
					}
				}
				IL_02D6:
				this.writer.Write(text);
			}
			this.writer.Write(this.quote_char);
			this.state = WriteState.Element;
		}

		public override void WriteComment(string text)
		{
			if (text == null)
			{
				throw this.ArgumentError("text");
			}
			if (text.Length > 0 && text[text.Length - 1] == '-')
			{
				throw this.ArgumentError("An input string to WriteComment method must not end with '-'. Escape it with '&#2D;'.");
			}
			if (XmlTextWriter.StringUtil.IndexOf(text, "--") > 0)
			{
				throw this.ArgumentError("An XML comment cannot end with \"-\".");
			}
			if (this.state == WriteState.Attribute || this.state == WriteState.Element)
			{
				this.CloseStartElement();
			}
			this.WriteIndent();
			this.ShiftStateTopLevel("Comment", false, false, false);
			this.writer.Write("<!--");
			this.writer.Write(text);
			this.writer.Write("-->");
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			if (name == null)
			{
				throw this.ArgumentError("name");
			}
			if (text == null)
			{
				throw this.ArgumentError("text");
			}
			this.WriteIndent();
			if (!XmlChar.IsName(name))
			{
				throw this.ArgumentError("A processing instruction name must be a valid XML name.");
			}
			if (XmlTextWriter.StringUtil.IndexOf(text, "?>") > 0)
			{
				throw this.ArgumentError("Processing instruction cannot contain \"?>\" as its value.");
			}
			this.ShiftStateTopLevel("ProcessingInstruction", false, name == "xml", false);
			this.writer.Write("<?");
			this.writer.Write(name);
			this.writer.Write(' ');
			this.writer.Write(text);
			this.writer.Write("?>");
			if (this.state == WriteState.Start)
			{
				this.state = WriteState.Prolog;
			}
		}

		public override void WriteWhitespace(string text)
		{
			if (text == null)
			{
				throw this.ArgumentError("text");
			}
			if (text.Length == 0 || XmlChar.IndexOfNonWhitespace(text) >= 0)
			{
				throw this.ArgumentError("WriteWhitespace method accepts only whitespaces.");
			}
			this.ShiftStateTopLevel("Whitespace", true, false, true);
			this.writer.Write(text);
		}

		public override void WriteCData(string text)
		{
			if (text == null)
			{
				text = string.Empty;
			}
			this.ShiftStateContent("CData", false);
			if (XmlTextWriter.StringUtil.IndexOf(text, "]]>") >= 0)
			{
				throw this.ArgumentError("CDATA section must not contain ']]>'.");
			}
			this.writer.Write("<![CDATA[");
			this.WriteCheckedString(text);
			this.writer.Write("]]>");
		}

		public override void WriteString(string text)
		{
			if (text == null || (text.Length == 0 && !this.v2))
			{
				return;
			}
			this.ShiftStateContent("Text", true);
			this.WriteEscapedString(text, this.state == WriteState.Attribute);
		}

		public override void WriteRaw(string raw)
		{
			if (raw == null)
			{
				return;
			}
			this.ShiftStateTopLevel("Raw string", true, true, true);
			this.writer.Write(raw);
		}

		public override void WriteCharEntity(char ch)
		{
			this.WriteCharacterEntity(ch, '\0', false);
		}

		public override void WriteSurrogateCharEntity(char low, char high)
		{
			this.WriteCharacterEntity(low, high, true);
		}

		private void WriteCharacterEntity(char ch, char high, bool surrogate)
		{
			if (surrogate && ('\ud800' > high || high > '\udc00' || '\udc00' > ch || ch > '\udfff'))
			{
				throw this.ArgumentError(string.Format("Invalid surrogate pair was found. Low: &#x{0:X}; High: &#x{0:X};", (int)ch, (int)high));
			}
			if (this.check_character_validity && XmlChar.IsInvalid((int)ch))
			{
				throw this.ArgumentError(string.Format("Invalid character &#x{0:X};", (int)ch));
			}
			this.ShiftStateContent("Character", true);
			int num = ((!surrogate) ? ((int)ch) : ((int)((high - '\ud800') * 'Ѐ' + ch - '\udc00') + 65536));
			this.writer.Write("&#x");
			this.writer.Write(num.ToString("X", CultureInfo.InvariantCulture));
			this.writer.Write(';');
		}

		public override void WriteEntityRef(string name)
		{
			if (name == null)
			{
				throw this.ArgumentError("name");
			}
			if (!XmlChar.IsName(name))
			{
				throw this.ArgumentError("Argument name must be a valid XML name.");
			}
			this.ShiftStateContent("Entity reference", true);
			this.writer.Write('&');
			this.writer.Write(name);
			this.writer.Write(';');
		}

		public override void WriteName(string name)
		{
			if (name == null)
			{
				throw this.ArgumentError("name");
			}
			if (!XmlChar.IsName(name))
			{
				throw this.ArgumentError("Not a valid name string.");
			}
			this.WriteString(name);
		}

		public override void WriteNmToken(string nmtoken)
		{
			if (nmtoken == null)
			{
				throw this.ArgumentError("nmtoken");
			}
			if (!XmlChar.IsNmToken(nmtoken))
			{
				throw this.ArgumentError("Not a valid NMTOKEN string.");
			}
			this.WriteString(nmtoken);
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			if (localName == null)
			{
				throw this.ArgumentError("localName");
			}
			if (ns == null)
			{
				ns = string.Empty;
			}
			if (ns == "http://www.w3.org/2000/xmlns/")
			{
				throw this.ArgumentError("Prefix 'xmlns' is reserved and cannot be overriden.");
			}
			if (!XmlChar.IsNCName(localName))
			{
				throw this.ArgumentError("localName must be a valid NCName.");
			}
			this.ShiftStateContent("QName", true);
			string text = ((ns.Length <= 0) ? string.Empty : this.LookupPrefix(ns));
			if (text == null)
			{
				if (this.state != WriteState.Attribute)
				{
					throw this.ArgumentError(string.Format("Namespace '{0}' is not declared.", ns));
				}
				text = this.MockupPrefix(ns, false);
			}
			if (text != string.Empty)
			{
				this.writer.Write(text);
				this.writer.Write(":");
			}
			this.writer.Write(localName);
		}

		private void CheckChunkRange(Array buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || buffer.Length < index)
			{
				throw this.ArgumentOutOfRangeError("index");
			}
			if (count < 0 || buffer.Length < index + count)
			{
				throw this.ArgumentOutOfRangeError("count");
			}
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.CheckChunkRange(buffer, index, count);
			this.WriteString(Convert.ToBase64String(buffer, index, count));
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			this.CheckChunkRange(buffer, index, count);
			this.ShiftStateContent("BinHex", true);
			XmlConvert.WriteBinHex(buffer, index, count, this.writer);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.CheckChunkRange(buffer, index, count);
			this.ShiftStateContent("Chars", true);
			this.WriteEscapedBuffer(buffer, index, count, this.state == WriteState.Attribute);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.CheckChunkRange(buffer, index, count);
			this.ShiftStateContent("Raw text", false);
			this.writer.Write(buffer, index, count);
		}

		private void WriteIndent()
		{
			this.WriteIndentCore(0, false);
		}

		private void WriteIndentEndElement()
		{
			this.WriteIndentCore(-1, false);
		}

		private void WriteIndentAttribute()
		{
			if (!this.WriteIndentCore(0, true))
			{
				this.writer.Write(' ');
			}
		}

		private bool WriteIndentCore(int nestFix, bool attribute)
		{
			if (!this.indent)
			{
				return false;
			}
			for (int i = this.open_count - 1; i >= 0; i--)
			{
				if (!attribute && this.elements[i].HasSimple)
				{
					return false;
				}
			}
			if (this.state != WriteState.Start)
			{
				this.writer.Write(this.newline);
			}
			for (int j = 0; j < this.open_count + nestFix; j++)
			{
				this.writer.Write(this.indent_string);
			}
			return true;
		}

		private void OutputAutoStartDocument()
		{
			if (this.state != WriteState.Start)
			{
				return;
			}
			this.WriteStartDocumentCore(false, false);
		}

		private void ShiftStateTopLevel(string occured, bool allowAttribute, bool dontCheckXmlDecl, bool isCharacter)
		{
			switch (this.state)
			{
			case WriteState.Start:
				if (isCharacter)
				{
					this.CheckMixedContentState();
				}
				if (this.xmldecl_state == XmlTextWriter.XmlDeclState.Auto && !dontCheckXmlDecl)
				{
					this.OutputAutoStartDocument();
				}
				this.state = WriteState.Prolog;
				return;
			case WriteState.Prolog:
				return;
			case WriteState.Element:
				if (isCharacter)
				{
					this.CheckMixedContentState();
				}
				this.CloseStartElement();
				return;
			case WriteState.Attribute:
				if (allowAttribute)
				{
					return;
				}
				break;
			case WriteState.Content:
				if (isCharacter)
				{
					this.CheckMixedContentState();
				}
				return;
			case WriteState.Closed:
			case WriteState.Error:
				break;
			default:
				return;
			}
			throw this.StateError(occured);
		}

		private void CheckMixedContentState()
		{
			if (this.open_count > 0)
			{
				this.elements[this.open_count - 1].HasSimple = true;
			}
		}

		private void ShiftStateContent(string occured, bool allowAttribute)
		{
			switch (this.state)
			{
			case WriteState.Start:
			case WriteState.Prolog:
				if (this.allow_doc_fragment && !this.is_document_entity)
				{
					if (this.xmldecl_state == XmlTextWriter.XmlDeclState.Auto)
					{
						this.OutputAutoStartDocument();
					}
					this.CheckMixedContentState();
					this.state = WriteState.Content;
					return;
				}
				break;
			case WriteState.Element:
				this.CloseStartElement();
				this.CheckMixedContentState();
				return;
			case WriteState.Attribute:
				if (allowAttribute)
				{
					return;
				}
				break;
			case WriteState.Content:
				this.CheckMixedContentState();
				return;
			case WriteState.Closed:
			case WriteState.Error:
				break;
			default:
				return;
			}
			throw this.StateError(occured);
		}

		private void WriteEscapedString(string text, bool isAttribute)
		{
			char[] array = ((!isAttribute) ? XmlTextWriter.escaped_text_chars : XmlTextWriter.escaped_attr_chars);
			int num = text.IndexOfAny(array);
			if (num >= 0)
			{
				char[] array2 = text.ToCharArray();
				this.WriteCheckedBuffer(array2, 0, num);
				this.WriteEscapedBuffer(array2, num, array2.Length - num, isAttribute);
			}
			else
			{
				this.WriteCheckedString(text);
			}
		}

		private void WriteCheckedString(string s)
		{
			int num = XmlChar.IndexOfInvalid(s, true);
			if (num >= 0)
			{
				char[] array = s.ToCharArray();
				this.writer.Write(array, 0, num);
				this.WriteCheckedBuffer(array, num, array.Length - num);
			}
			else
			{
				this.writer.Write(s);
			}
		}

		private void WriteCheckedBuffer(char[] text, int idx, int length)
		{
			int num = idx;
			int num2 = idx + length;
			while ((idx = XmlChar.IndexOfInvalid(text, num, length, true)) >= 0)
			{
				if (this.check_character_validity)
				{
					throw this.ArgumentError(string.Format("Input contains invalid character at {0} : &#x{1:X};", idx, (int)text[idx]));
				}
				if (num < idx)
				{
					this.writer.Write(text, num, idx - num);
				}
				this.writer.Write("&#x");
				TextWriter textWriter = this.writer;
				int num3 = (int)text[idx];
				textWriter.Write(num3.ToString("X", CultureInfo.InvariantCulture));
				this.writer.Write(';');
				length -= idx - num + 1;
				num = idx + 1;
			}
			if (num < num2)
			{
				this.writer.Write(text, num, num2 - num);
			}
		}

		private void WriteEscapedBuffer(char[] text, int index, int length, bool isAttribute)
		{
			int num = index;
			int num2 = index + length;
			int i = num;
			while (i < num2)
			{
				char c = text[i];
				switch (c)
				{
				case '"':
				case '\'':
					if (isAttribute && text[i] == this.quote_char)
					{
						goto IL_006A;
					}
					break;
				default:
				{
					switch (c)
					{
					case '\n':
						break;
					default:
						switch (c)
						{
						case '<':
						case '>':
							goto IL_006A;
						default:
							goto IL_022D;
						}
						break;
					case '\r':
						if (i + 1 < num2 && text[i] == '\n')
						{
							i++;
						}
						break;
					}
					if (num < i)
					{
						this.WriteCheckedBuffer(text, num, i - num);
					}
					if (isAttribute)
					{
						this.writer.Write((text[i] != '\r') ? "&#xA;" : "&#xD;");
						goto IL_0229;
					}
					NewLineHandling newLineHandling = this.newline_handling;
					if (newLineHandling != NewLineHandling.Replace)
					{
						if (newLineHandling != NewLineHandling.Entitize)
						{
							this.writer.Write(text[i]);
						}
						else
						{
							this.writer.Write((text[i] != '\r') ? "&#xA;" : "&#xD;");
						}
					}
					else
					{
						this.writer.Write(this.newline);
					}
					goto IL_0229;
				}
				case '&':
					goto IL_006A;
				}
				IL_022D:
				i++;
				continue;
				IL_0229:
				num = i + 1;
				goto IL_022D;
				IL_006A:
				if (num < i)
				{
					this.WriteCheckedBuffer(text, num, i - num);
				}
				this.writer.Write('&');
				char c2 = text[i];
				switch (c2)
				{
				case '"':
					this.writer.Write("quot;");
					break;
				default:
					switch (c2)
					{
					case '<':
						this.writer.Write("lt;");
						break;
					case '>':
						this.writer.Write("gt;");
						break;
					}
					break;
				case '&':
					this.writer.Write("amp;");
					break;
				case '\'':
					this.writer.Write("apos;");
					break;
				}
				goto IL_0229;
			}
			if (num < num2)
			{
				this.WriteCheckedBuffer(text, num, num2 - num);
			}
		}

		private Exception ArgumentOutOfRangeError(string name)
		{
			this.state = WriteState.Error;
			return new ArgumentOutOfRangeException(name);
		}

		private Exception ArgumentError(string msg)
		{
			this.state = WriteState.Error;
			return new ArgumentException(msg);
		}

		private Exception InvalidOperation(string msg)
		{
			this.state = WriteState.Error;
			return new InvalidOperationException(msg);
		}

		private Exception StateError(string occured)
		{
			return this.InvalidOperation(string.Format("This XmlWriter does not accept {0} at this state {1}.", occured, this.state));
		}

		private const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

		private const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";

		private static readonly Encoding unmarked_utf8encoding = new UTF8Encoding(false, false);

		private static char[] escaped_text_chars;

		private static char[] escaped_attr_chars;

		private Stream base_stream;

		private TextWriter source;

		private TextWriter writer;

		private StringWriter preserver;

		private string preserved_name;

		private bool is_preserved_xmlns;

		private bool allow_doc_fragment;

		private bool close_output_stream;

		private bool ignore_encoding;

		private bool namespaces;

		private XmlTextWriter.XmlDeclState xmldecl_state;

		private bool check_character_validity;

		private NewLineHandling newline_handling;

		private bool is_document_entity;

		private WriteState state;

		private XmlNodeType node_state;

		private XmlNamespaceManager nsmanager;

		private int open_count;

		private XmlTextWriter.XmlNodeInfo[] elements;

		private Stack new_local_namespaces;

		private ArrayList explicit_nsdecls;

		private NamespaceHandling namespace_handling;

		private bool indent;

		private int indent_count;

		private char indent_char;

		private string indent_string;

		private string newline;

		private bool indent_attributes;

		private char quote_char;

		private bool v2;

		private class XmlNodeInfo
		{
			public string Prefix;

			public string LocalName;

			public string NS;

			public bool HasSimple;

			public bool HasElements;

			public string XmlLang;

			public XmlSpace XmlSpace;
		}

		internal class StringUtil
		{
			public static int IndexOf(string src, string target)
			{
				return XmlTextWriter.StringUtil.cmp.IndexOf(src, target);
			}

			public static int Compare(string s1, string s2)
			{
				return XmlTextWriter.StringUtil.cmp.Compare(s1, s2);
			}

			public static string Format(string format, params object[] args)
			{
				return string.Format(XmlTextWriter.StringUtil.cul, format, args);
			}

			private static CultureInfo cul = CultureInfo.InvariantCulture;

			private static CompareInfo cmp = CultureInfo.InvariantCulture.CompareInfo;
		}

		private enum XmlDeclState
		{
			Allow,
			Ignore,
			Auto,
			Prohibit
		}
	}
}
