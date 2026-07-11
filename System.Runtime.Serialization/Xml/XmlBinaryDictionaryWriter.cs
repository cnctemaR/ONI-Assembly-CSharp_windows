using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Xml
{
	internal class XmlBinaryDictionaryWriter : XmlDictionaryWriter
	{
		public XmlBinaryDictionaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session, bool ownsStream)
		{
			if (dictionary == null)
			{
				dictionary = new XmlDictionary();
			}
			if (session == null)
			{
				session = new XmlBinaryWriterSession();
			}
			this.original = new XmlBinaryDictionaryWriter.MyBinaryWriter(stream);
			this.writer = this.original;
			this.buffer_writer = new XmlBinaryDictionaryWriter.MyBinaryWriter(this.buffer);
			this.dict_ext = dictionary;
			this.session = session;
			this.owns_stream = ownsStream;
			this.AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");
			this.AddNamespace("xml", "http://www.w3.org/2000/xmlns/");
			this.ns_index = 2;
		}

		public override WriteState WriteState
		{
			get
			{
				return this.state;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.xml_lang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.xml_space;
			}
		}

		private void AddMissingElementXmlns()
		{
			for (int i = this.ns_index; i < this.namespaces.Count; i++)
			{
				KeyValuePair<string, object> keyValuePair = this.namespaces[i];
				string key = keyValuePair.Key;
				string text = keyValuePair.Value as string;
				XmlDictionaryString xmlDictionaryString = keyValuePair.Value as XmlDictionaryString;
				if (text != null)
				{
					if (key.Length > 0)
					{
						this.writer.Write(9);
						this.writer.Write(key);
					}
					else
					{
						this.writer.Write(8);
					}
					this.writer.Write(text);
				}
				else
				{
					if (key.Length > 0)
					{
						this.writer.Write(11);
						this.writer.Write(key);
					}
					else
					{
						this.writer.Write(10);
					}
					this.WriteDictionaryIndex(xmlDictionaryString);
				}
			}
			this.ns_index = this.namespaces.Count;
		}

		private void CheckState()
		{
			if (this.state == WriteState.Closed)
			{
				throw new InvalidOperationException("The Writer is closed.");
			}
		}

		private void ProcessStateForContent()
		{
			this.CheckState();
			if (this.state == WriteState.Element)
			{
				this.CloseStartElement();
			}
			this.ProcessPendingBuffer(false, false);
			if (this.state != WriteState.Attribute)
			{
				this.writer = this.buffer_writer;
			}
		}

		private void ProcessTypedValue()
		{
			this.ProcessStateForContent();
			if (this.state == WriteState.Attribute)
			{
				if (this.attr_typed_value)
				{
					throw new InvalidOperationException(string.Format("A typed value for the attribute '{0}' in namespace '{1}' was already written", this.current_attr_name, this.current_attr_ns));
				}
				this.attr_typed_value = true;
			}
		}

		private void ProcessPendingBuffer(bool last, bool endElement)
		{
			if (this.buffer.Position > 0L)
			{
				byte[] array = this.buffer.GetBuffer();
				if (endElement)
				{
					byte[] array2 = array;
					int num = 0;
					array2[num] += 1;
				}
				this.original.Write(array, 0, (int)this.buffer.Position);
				this.buffer.SetLength(0L);
			}
			if (last)
			{
				this.writer = this.original;
			}
		}

		public override void Close()
		{
			this.CloseOpenAttributeAndElements();
			if (this.owns_stream)
			{
				this.writer.Close();
			}
			else if (this.state != WriteState.Closed)
			{
				this.writer.Flush();
			}
			this.state = WriteState.Closed;
		}

		private void CloseOpenAttributeAndElements()
		{
			this.CloseStartElement();
			while (this.element_count > 0)
			{
				this.WriteEndElement();
			}
		}

		private void CloseStartElement()
		{
			if (!this.open_start_element)
			{
				return;
			}
			if (this.state == WriteState.Attribute)
			{
				this.WriteEndAttribute();
			}
			this.AddMissingElementXmlns();
			this.state = WriteState.Content;
			this.open_start_element = false;
		}

		public override void Flush()
		{
			this.writer.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			if (ns == null || ns == string.Empty)
			{
				throw new ArgumentException("The Namespace cannot be empty.");
			}
			return this.namespaces.LastOrDefault<KeyValuePair<string, object>>((KeyValuePair<string, object> i) => i.Value.ToString() == ns).Key;
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			if (count < 0)
			{
				throw new IndexOutOfRangeException("Negative count");
			}
			this.ProcessStateForContent();
			if (count < 256)
			{
				this.writer.Write(158);
				this.writer.Write((byte)count);
				this.writer.Write(buffer, index, count);
			}
			else if (count < 65536)
			{
				this.writer.Write(158);
				this.writer.Write((ushort)count);
				this.writer.Write(buffer, index, count);
			}
			else
			{
				this.writer.Write(162);
				this.writer.Write(count);
				this.writer.Write(buffer, index, count);
			}
		}

		public override void WriteCData(string text)
		{
			if (text.IndexOf("]]>") >= 0)
			{
				throw new ArgumentException("CDATA section cannot contain text \"]]>\".");
			}
			this.ProcessStateForContent();
			this.WriteTextBinary(text);
		}

		public override void WriteCharEntity(char ch)
		{
			this.WriteChars(new char[] { ch }, 0, 1);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.ProcessStateForContent();
			int byteCount = Encoding.UTF8.GetByteCount(buffer, index, count);
			if (byteCount == 0)
			{
				this.writer.Write(168);
			}
			else if (count == 1 && buffer[0] == '0')
			{
				this.writer.Write(128);
			}
			else if (count == 1 && buffer[0] == '1')
			{
				this.writer.Write(130);
			}
			else if (byteCount < 256)
			{
				this.writer.Write(152);
				this.writer.Write((byte)byteCount);
				this.writer.Write(buffer, index, count);
			}
			else if (byteCount < 65536)
			{
				this.writer.Write(154);
				this.writer.Write((ushort)byteCount);
				this.writer.Write(buffer, index, count);
			}
			else
			{
				this.writer.Write(156);
				this.writer.Write(byteCount);
				this.writer.Write(buffer, index, count);
			}
		}

		public override void WriteComment(string text)
		{
			if (text.EndsWith("-"))
			{
				throw new ArgumentException("An XML comment cannot contain \"--\" inside.");
			}
			if (text.IndexOf("--") > 0)
			{
				throw new ArgumentException("An XML comment cannot end with \"-\".");
			}
			this.ProcessStateForContent();
			if (this.state == WriteState.Attribute)
			{
				throw new InvalidOperationException("Comment node is not allowed inside an attribute");
			}
			this.writer.Write(2);
			this.writer.Write(text);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			throw new NotSupportedException("This XmlWriter implementation does not support document type.");
		}

		public override void WriteEndAttribute()
		{
			if (this.state != WriteState.Attribute)
			{
				throw new InvalidOperationException("Token EndAttribute in state Start would result in an invalid XML document.");
			}
			this.CheckState();
			if (this.attr_value == null)
			{
				this.attr_value = string.Empty;
			}
			switch (this.save_target)
			{
			case XmlBinaryDictionaryWriter.SaveTarget.Namespaces:
				if (this.current_attr_name.ToString().Length > 0 && this.attr_value.Length == 0)
				{
					throw new ArgumentException("Cannot use prefix with an empty namespace.");
				}
				this.AddNamespaceChecked(this.current_attr_name.ToString(), this.attr_value);
				goto IL_0160;
			case XmlBinaryDictionaryWriter.SaveTarget.XmlLang:
				this.xml_lang = this.attr_value;
				break;
			case XmlBinaryDictionaryWriter.SaveTarget.XmlSpace:
			{
				string text = this.attr_value;
				if (text != null)
				{
					if (XmlBinaryDictionaryWriter.<>f__switch$map3 == null)
					{
						XmlBinaryDictionaryWriter.<>f__switch$map3 = new Dictionary<string, int>(2)
						{
							{ "preserve", 0 },
							{ "default", 1 }
						};
					}
					int num;
					if (XmlBinaryDictionaryWriter.<>f__switch$map3.TryGetValue(text, out num))
					{
						if (num != 0)
						{
							if (num != 1)
							{
								goto IL_00DC;
							}
							this.xml_space = XmlSpace.Default;
						}
						else
						{
							this.xml_space = XmlSpace.Preserve;
						}
						break;
					}
				}
				IL_00DC:
				throw new ArgumentException(string.Format("Invalid xml:space value: '{0}'", this.attr_value));
			}
			}
			if (!this.attr_typed_value)
			{
				this.WriteTextBinary(this.attr_value);
			}
			IL_0160:
			if (this.current_attr_prefix.Length > 0 && this.save_target != XmlBinaryDictionaryWriter.SaveTarget.Namespaces)
			{
				this.AddNamespaceChecked(this.current_attr_prefix, this.current_attr_ns);
			}
			this.state = WriteState.Element;
			this.current_attr_prefix = null;
			this.current_attr_name = null;
			this.current_attr_ns = null;
			this.attr_value = null;
			this.attr_typed_value = false;
		}

		public override void WriteEndDocument()
		{
			this.CloseOpenAttributeAndElements();
			WriteState writeState = this.state;
			if (writeState == WriteState.Start)
			{
				throw new InvalidOperationException("Document has not started.");
			}
			if (writeState != WriteState.Prolog)
			{
				this.state = WriteState.Start;
				return;
			}
			throw new ArgumentException("This document does not have a root element.");
		}

		private bool SupportsCombinedEndElementSupport(byte operation)
		{
			return operation != 2;
		}

		public override void WriteEndElement()
		{
			if (this.element_count-- == 0)
			{
				throw new InvalidOperationException("There was no XML start tag open.");
			}
			if (this.state == WriteState.Attribute)
			{
				this.WriteEndAttribute();
			}
			bool flag = this.buffer.Position == 0L || !this.SupportsCombinedEndElementSupport(this.buffer.GetBuffer()[0]);
			this.ProcessPendingBuffer(true, !flag);
			this.CheckState();
			this.AddMissingElementXmlns();
			if (flag)
			{
				this.writer.Write(1);
			}
			this.element_ns = this.element_ns_stack.Pop();
			this.xml_lang = this.xml_lang_stack.Pop();
			this.xml_space = this.xml_space_stack.Pop();
			int count = this.namespaces.Count;
			this.ns_index = this.ns_index_stack.Pop();
			this.namespaces.RemoveRange(this.ns_index, count - this.ns_index);
			this.open_start_element = false;
			base.Depth--;
		}

		public override void WriteEntityRef(string name)
		{
			throw new NotSupportedException("This XmlWriter implementation does not support entity references.");
		}

		public override void WriteFullEndElement()
		{
			this.WriteEndElement();
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			if (name != "xml")
			{
				throw new ArgumentException("Processing instructions are not supported. ('xml' is allowed for XmlDeclaration; this is because of design problem of ECMA XmlWriter)");
			}
		}

		public override void WriteQualifiedName(XmlDictionaryString local, XmlDictionaryString ns)
		{
			string text = this.namespaces.LastOrDefault<KeyValuePair<string, object>>((KeyValuePair<string, object> i) => i.Value.ToString() == ns.ToString()).Key;
			bool flag = text != null;
			if (text == null)
			{
				text = this.LookupPrefix(ns.Value);
			}
			if (text == null)
			{
				throw new ArgumentException(string.Format("Namespace URI '{0}' is not bound to any of the prefixes", ns));
			}
			this.ProcessTypedValue();
			if (flag && text.Length == 1)
			{
				this.writer.Write(188);
				this.writer.Write((byte)(text[0] - 'a'));
				this.WriteDictionaryIndex(local);
			}
			else
			{
				this.WriteString(text);
				this.WriteString(":");
				this.WriteString(local);
			}
		}

		public override void WriteRaw(string data)
		{
			this.WriteString(data);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.WriteChars(buffer, index, count);
		}

		private void CheckStateForAttribute()
		{
			this.CheckState();
			if (this.state != WriteState.Element)
			{
				throw new InvalidOperationException("Token StartAttribute in state " + this.WriteState + " would result in an invalid XML document.");
			}
		}

		private string CreateNewPrefix()
		{
			return this.CreateNewPrefix(string.Empty);
		}

		private string CreateNewPrefix(string p)
		{
			XmlBinaryDictionaryWriter.<CreateNewPrefix>c__AnonStorey2 <CreateNewPrefix>c__AnonStorey = new XmlBinaryDictionaryWriter.<CreateNewPrefix>c__AnonStorey2();
			<CreateNewPrefix>c__AnonStorey.p = p;
			char c;
			for (c = 'a'; c <= 'z'; c += '\u0001')
			{
				if (!this.namespaces.Any<KeyValuePair<string, object>>((KeyValuePair<string, object> iter) => iter.Key == <CreateNewPrefix>c__AnonStorey.p + c))
				{
					return <CreateNewPrefix>c__AnonStorey.p + c;
				}
			}
			for (char c2 = 'a'; c2 <= 'z'; c2 += '\u0001')
			{
				string text = this.CreateNewPrefix(c2.ToString());
				if (text != null)
				{
					return text;
				}
			}
			throw new InvalidOperationException("too many prefix population");
		}

		private bool CollectionContains(ICollection col, string value)
		{
			foreach (object obj in col)
			{
				string text = (string)obj;
				if (text == value)
				{
					return true;
				}
			}
			return false;
		}

		private void ProcessStartAttributeCommon(ref string prefix, string localName, string ns, object nameObj, object nsObj)
		{
			string text;
			if (prefix.Length == 0 && ns.Length > 0)
			{
				prefix = this.LookupPrefix(ns);
				if (string.IsNullOrEmpty(prefix))
				{
					prefix = this.CreateNewPrefix();
				}
			}
			else if (prefix.Length > 0 && ns.Length == 0)
			{
				text = prefix;
				if (text != null)
				{
					if (XmlBinaryDictionaryWriter.<>f__switch$map4 == null)
					{
						XmlBinaryDictionaryWriter.<>f__switch$map4 = new Dictionary<string, int>(2)
						{
							{ "xml", 0 },
							{ "xmlns", 1 }
						};
					}
					int num;
					if (XmlBinaryDictionaryWriter.<>f__switch$map4.TryGetValue(text, out num))
					{
						if (num == 0)
						{
							ns = (nsObj = "http://www.w3.org/XML/1998/namespace");
							goto IL_00D7;
						}
						if (num == 1)
						{
							ns = (nsObj = "http://www.w3.org/2000/xmlns/");
							goto IL_00D7;
						}
					}
				}
				throw new ArgumentException("Cannot use prefix with an empty namespace.");
			}
			IL_00D7:
			if (prefix == "xmlns" && ns != "http://www.w3.org/2000/xmlns/")
			{
				throw new ArgumentException(string.Format("The 'xmlns' attribute is bound to the reserved namespace '{0}'", "http://www.w3.org/2000/xmlns/"));
			}
			this.CheckStateForAttribute();
			this.state = WriteState.Attribute;
			this.save_target = XmlBinaryDictionaryWriter.SaveTarget.None;
			text = prefix;
			if (text != null)
			{
				if (XmlBinaryDictionaryWriter.<>f__switch$map6 == null)
				{
					XmlBinaryDictionaryWriter.<>f__switch$map6 = new Dictionary<string, int>(2)
					{
						{ "xml", 0 },
						{ "xmlns", 1 }
					};
				}
				int num;
				if (XmlBinaryDictionaryWriter.<>f__switch$map6.TryGetValue(text, out num))
				{
					if (num != 0)
					{
						if (num == 1)
						{
							this.save_target = XmlBinaryDictionaryWriter.SaveTarget.Namespaces;
						}
					}
					else
					{
						ns = "http://www.w3.org/XML/1998/namespace";
						if (localName != null)
						{
							if (XmlBinaryDictionaryWriter.<>f__switch$map5 == null)
							{
								XmlBinaryDictionaryWriter.<>f__switch$map5 = new Dictionary<string, int>(2)
								{
									{ "lang", 0 },
									{ "space", 1 }
								};
							}
							int num2;
							if (XmlBinaryDictionaryWriter.<>f__switch$map5.TryGetValue(localName, out num2))
							{
								if (num2 != 0)
								{
									if (num2 == 1)
									{
										this.save_target = XmlBinaryDictionaryWriter.SaveTarget.XmlSpace;
									}
								}
								else
								{
									this.save_target = XmlBinaryDictionaryWriter.SaveTarget.XmlLang;
								}
							}
						}
					}
				}
			}
			this.current_attr_prefix = prefix;
			this.current_attr_name = nameObj;
			this.current_attr_ns = nsObj;
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			if (ns == null)
			{
				ns = string.Empty;
			}
			if (localName == "xmlns" && prefix.Length == 0)
			{
				prefix = "xmlns";
				localName = string.Empty;
			}
			this.ProcessStartAttributeCommon(ref prefix, localName, ns, localName, ns);
			if (this.save_target == XmlBinaryDictionaryWriter.SaveTarget.Namespaces)
			{
				return;
			}
			byte b = ((prefix.Length != 1 || 'a' > prefix[0] || prefix[0] > 'z') ? ((prefix.Length != 0) ? 5 : 4) : ((byte)(prefix[0] - 'a' + '&')));
			if (38 <= b && b <= 63)
			{
				this.writer.Write(b);
				this.writer.Write(localName);
			}
			else
			{
				this.writer.Write(b);
				if (prefix.Length > 0)
				{
					this.writer.Write(prefix);
				}
				this.writer.Write(localName);
			}
		}

		public override void WriteStartDocument()
		{
			this.WriteStartDocument(false);
		}

		public override void WriteStartDocument(bool standalone)
		{
			if (this.state != WriteState.Start)
			{
				throw new InvalidOperationException("WriteStartDocument should be the first call.");
			}
			this.CheckState();
			this.state = WriteState.Prolog;
		}

		private void PrepareStartElement()
		{
			this.ProcessPendingBuffer(true, false);
			this.CheckState();
			this.CloseStartElement();
			base.Depth++;
			this.element_ns_stack.Push(this.element_ns);
			this.xml_lang_stack.Push(this.xml_lang);
			this.xml_space_stack.Push(this.xml_space);
			this.ns_index_stack.Push(this.ns_index);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.PrepareStartElement();
			if (prefix != null && prefix != string.Empty && (ns == null || ns == string.Empty))
			{
				throw new ArgumentException("Cannot use a prefix with an empty namespace.");
			}
			if (ns == null)
			{
				ns = string.Empty;
			}
			if (ns == string.Empty)
			{
				prefix = string.Empty;
			}
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			byte b = ((prefix.Length != 1 || 'a' > prefix[0] || prefix[0] > 'z') ? ((prefix.Length != 0) ? 65 : 64) : ((byte)(prefix[0] - 'a' + '^')));
			if (94 <= b && b <= 119)
			{
				this.writer.Write(b);
				this.writer.Write(localName);
			}
			else
			{
				this.writer.Write(b);
				if (prefix.Length > 0)
				{
					this.writer.Write(prefix);
				}
				this.writer.Write(localName);
			}
			this.OpenElement(prefix, ns);
		}

		private void OpenElement(string prefix, object nsobj)
		{
			string text = nsobj.ToString();
			this.state = WriteState.Element;
			this.open_start_element = true;
			this.element_prefix = prefix;
			this.element_count++;
			this.element_ns = nsobj.ToString();
			if (this.element_ns != string.Empty && this.LookupPrefix(this.element_ns) != prefix)
			{
				this.AddNamespace(prefix, nsobj);
			}
		}

		private void AddNamespace(string prefix, object nsobj)
		{
			this.namespaces.Add(new KeyValuePair<string, object>(prefix, nsobj));
		}

		private void CheckIfTextAllowed()
		{
			WriteState writeState = this.state;
			if (writeState != WriteState.Start && writeState != WriteState.Prolog)
			{
				return;
			}
			throw new InvalidOperationException("Token content in state Prolog would result in an invalid XML document.");
		}

		public override void WriteString(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			this.CheckIfTextAllowed();
			if (text == null)
			{
				text = string.Empty;
			}
			this.ProcessStateForContent();
			if (this.state == WriteState.Attribute)
			{
				this.attr_value += text;
			}
			else
			{
				this.WriteTextBinary(text);
			}
		}

		public override void WriteString(XmlDictionaryString text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			this.CheckIfTextAllowed();
			if (text == null)
			{
				text = XmlDictionaryString.Empty;
			}
			this.ProcessStateForContent();
			if (this.state == WriteState.Attribute)
			{
				this.attr_value += text.Value;
			}
			else if (text.Equals(XmlDictionary.Empty))
			{
				this.writer.Write(168);
			}
			else
			{
				this.writer.Write(170);
				this.WriteDictionaryIndex(text);
			}
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.WriteChars(new char[] { highChar, lowChar }, 0, 2);
		}

		public override void WriteWhitespace(string ws)
		{
			foreach (char c in ws)
			{
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
					break;
				default:
					if (c != ' ')
					{
						throw new ArgumentException("Invalid Whitespace");
					}
					break;
				}
			}
			this.ProcessStateForContent();
			this.WriteTextBinary(ws);
		}

		public override void WriteXmlnsAttribute(string prefix, string namespaceUri)
		{
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			if (string.IsNullOrEmpty(prefix))
			{
				prefix = this.CreateNewPrefix();
			}
			this.CheckStateForAttribute();
			this.AddNamespaceChecked(prefix, namespaceUri);
			this.state = WriteState.Element;
		}

		private void AddNamespaceChecked(string prefix, object ns)
		{
			string text = ns.ToString();
			if (text != null)
			{
				if (XmlBinaryDictionaryWriter.<>f__switch$map7 == null)
				{
					XmlBinaryDictionaryWriter.<>f__switch$map7 = new Dictionary<string, int>(2)
					{
						{ "http://www.w3.org/2000/xmlns/", 0 },
						{ "http://www.w3.org/XML/1998/namespace", 0 }
					};
				}
				int num;
				if (XmlBinaryDictionaryWriter.<>f__switch$map7.TryGetValue(text, out num))
				{
					if (num == 0)
					{
						return;
					}
				}
			}
			if (prefix == null)
			{
				throw new InvalidOperationException();
			}
			KeyValuePair<string, object> keyValuePair = this.namespaces.LastOrDefault<KeyValuePair<string, object>>((KeyValuePair<string, object> i) => i.Key == prefix);
			if (keyValuePair.Key != null)
			{
				if (keyValuePair.Value.ToString() != ns.ToString())
				{
					if (this.namespaces.LastIndexOf(keyValuePair) >= this.ns_index)
					{
						throw new ArgumentException(string.Format("The prefix '{0}' is already mapped to another namespace URI '{1}' in this element scope and cannot be mapped to '{2}'", prefix ?? "(null)", keyValuePair.Value ?? "(null)", ns.ToString()));
					}
					this.AddNamespace(prefix, ns);
				}
			}
			else
			{
				this.AddNamespace(prefix, ns);
			}
		}

		private void WriteDictionaryIndex(XmlDictionaryString ds)
		{
			bool flag = false;
			int key = ds.Key;
			if (ds.Dictionary != this.dict_ext)
			{
				flag = true;
				XmlDictionaryString xmlDictionaryString;
				if (this.dict_int.TryLookup(ds.Value, out xmlDictionaryString))
				{
					ds = xmlDictionaryString;
				}
				if (!this.session.TryLookup(ds, out key))
				{
					this.session.TryAdd(this.dict_int.Add(ds.Value), out key);
				}
			}
			if (key >= 128)
			{
				this.writer.Write((byte)(128 + (key % 128 << 1) + ((!flag) ? 0 : 1)));
				this.writer.Write((byte)((byte)(key / 128) << 1));
			}
			else
			{
				this.writer.Write((byte)((key % 128 << 1) + ((!flag) ? 0 : 1)));
			}
		}

		public override void WriteStartElement(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.PrepareStartElement();
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			byte b = ((prefix.Length != 1 || 'a' > prefix[0] || prefix[0] > 'z') ? ((prefix.Length != 0) ? 67 : 66) : ((byte)(prefix[0] - 'a' + 'D')));
			if (68 <= b && b <= 93)
			{
				this.writer.Write(b);
				this.WriteDictionaryIndex(localName);
			}
			else
			{
				this.writer.Write(b);
				if (prefix.Length > 0)
				{
					this.writer.Write(prefix);
				}
				this.WriteDictionaryIndex(localName);
			}
			this.OpenElement(prefix, namespaceUri);
		}

		public override void WriteStartAttribute(string prefix, XmlDictionaryString localName, XmlDictionaryString ns)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			if (ns == null)
			{
				ns = XmlDictionaryString.Empty;
			}
			if (localName.Value == "xmlns" && prefix.Length == 0)
			{
				prefix = "xmlns";
				localName = XmlDictionaryString.Empty;
			}
			this.ProcessStartAttributeCommon(ref prefix, localName.Value, ns.Value, localName, ns);
			if (this.save_target == XmlBinaryDictionaryWriter.SaveTarget.Namespaces)
			{
				return;
			}
			if (prefix.Length == 1 && 'a' <= prefix[0] && prefix[0] <= 'z')
			{
				this.writer.Write((byte)(prefix[0] - 'a' + '\f'));
				this.WriteDictionaryIndex(localName);
			}
			else
			{
				byte b = ((ns.Value.Length != 0) ? 7 : 6);
				this.writer.Write(b);
				if (prefix.Length > 0)
				{
					this.writer.Write(prefix);
				}
				this.WriteDictionaryIndex(localName);
			}
		}

		public override void WriteXmlnsAttribute(string prefix, XmlDictionaryString namespaceUri)
		{
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			if (string.IsNullOrEmpty(prefix))
			{
				prefix = this.CreateNewPrefix();
			}
			this.CheckStateForAttribute();
			this.AddNamespaceChecked(prefix, namespaceUri);
			this.state = WriteState.Element;
		}

		public override void WriteValue(bool value)
		{
			this.ProcessTypedValue();
			this.writer.Write((!value) ? 132 : 134);
		}

		public override void WriteValue(int value)
		{
			this.WriteValue((long)value);
		}

		public override void WriteValue(long value)
		{
			this.ProcessTypedValue();
			if (value == 0L)
			{
				this.writer.Write(128);
			}
			else if (value == 1L)
			{
				this.writer.Write(130);
			}
			else if (value < 0L || value > (long)((ulong)(-1)))
			{
				this.writer.Write(142);
				for (int i = 0; i < 8; i++)
				{
					this.writer.Write((byte)(value & 255L));
					value >>= 8;
				}
			}
			else if (value <= 255L)
			{
				this.writer.Write(136);
				this.writer.Write((byte)value);
			}
			else if (value <= 32767L)
			{
				this.writer.Write(138);
				this.writer.Write((byte)(value & 255L));
				this.writer.Write((byte)(value >> 8));
			}
			else if (value <= 2147483647L)
			{
				this.writer.Write(140);
				for (int j = 0; j < 4; j++)
				{
					this.writer.Write((byte)(value & 255L));
					value >>= 8;
				}
			}
		}

		public override void WriteValue(float value)
		{
			this.ProcessTypedValue();
			this.writer.Write(144);
			this.WriteValueContent(value);
		}

		private void WriteValueContent(float value)
		{
			this.writer.Write(value);
		}

		public override void WriteValue(double value)
		{
			this.ProcessTypedValue();
			this.writer.Write(146);
			this.WriteValueContent(value);
		}

		private void WriteValueContent(double value)
		{
			this.writer.Write(value);
		}

		public override void WriteValue(decimal value)
		{
			this.ProcessTypedValue();
			this.writer.Write(148);
			this.WriteValueContent(value);
		}

		private void WriteValueContent(decimal value)
		{
			int[] bits = decimal.GetBits(value);
			this.writer.Write(bits[3]);
			this.writer.Write(bits[2]);
			this.writer.Write(bits[0]);
			this.writer.Write(bits[1]);
		}

		public override void WriteValue(DateTime value)
		{
			this.ProcessTypedValue();
			this.writer.Write(150);
			this.WriteValueContent(value);
		}

		private void WriteValueContent(DateTime value)
		{
			this.writer.Write(value.Ticks);
		}

		public override void WriteValue(Guid value)
		{
			this.ProcessTypedValue();
			this.writer.Write(176);
			this.WriteValueContent(value);
		}

		private void WriteValueContent(Guid value)
		{
			byte[] array = value.ToByteArray();
			this.writer.Write(array, 0, array.Length);
		}

		public override void WriteValue(UniqueId value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Guid guid;
			if (value.TryGetGuid(out guid))
			{
				this.ProcessTypedValue();
				this.writer.Write(172);
				byte[] array = guid.ToByteArray();
				this.writer.Write(array, 0, array.Length);
			}
			else
			{
				this.WriteValue(value.ToString());
			}
		}

		public override void WriteValue(TimeSpan value)
		{
			this.ProcessTypedValue();
			this.writer.Write(174);
			this.WriteValueContent(value);
		}

		private void WriteValueContent(TimeSpan value)
		{
			this.WriteBigEndian(value.Ticks, 8);
		}

		private void WriteBigEndian(long value, int digits)
		{
			long num = 0L;
			for (int i = 0; i < digits; i++)
			{
				num = (num << 8) + (value & 255L);
				value >>= 8;
			}
			for (int j = 0; j < digits; j++)
			{
				this.writer.Write((byte)(num & 255L));
				num >>= 8;
			}
		}

		private void WriteTextBinary(string text)
		{
			if (text.Length == 0)
			{
				this.writer.Write(168);
			}
			else
			{
				char[] array = text.ToCharArray();
				this.WriteChars(array, 0, array.Length);
			}
		}

		private void WriteValueContent(bool value)
		{
			this.writer.Write((!value) ? 0 : 1);
		}

		private void WriteValueContent(short value)
		{
			this.writer.Write(value);
		}

		private void WriteValueContent(int value)
		{
			this.writer.Write(value);
		}

		private void WriteValueContent(long value)
		{
			this.writer.Write(value);
		}

		private void CheckWriteArrayArguments(Array array, int offset, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset is negative");
			}
			if (offset > array.Length)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the length of the destination array");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length is negative");
			}
			if (length > array.Length - offset)
			{
				throw new ArgumentOutOfRangeException("length + offset exceeds the length of the destination array");
			}
		}

		private void CheckDictionaryStringArgs(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, bool[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(bool[] array, int offset, int length)
		{
			this.writer.Write(181);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(DateTime[] array, int offset, int length)
		{
			this.writer.Write(151);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, decimal[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(decimal[] array, int offset, int length)
		{
			this.writer.Write(149);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, double[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(double[] array, int offset, int length)
		{
			this.writer.Write(147);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, Guid[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(Guid[] array, int offset, int length)
		{
			this.writer.Write(177);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, short[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(short[] array, int offset, int length)
		{
			this.writer.Write(139);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, int[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(int[] array, int offset, int length)
		{
			this.writer.Write(141);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, long[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(long[] array, int offset, int length)
		{
			this.writer.Write(143);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, float[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(float[] array, int offset, int length)
		{
			this.writer.Write(145);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			this.writer.Write(3);
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
			this.WriteArrayRemaining(array, offset, length);
		}

		private void WriteArrayRemaining(TimeSpan[] array, int offset, int length)
		{
			this.writer.Write(175);
			this.writer.WriteFlexibleInt(length);
			for (int i = offset; i < offset + length; i++)
			{
				this.WriteValueContent(array[i]);
			}
		}

		private const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

		private const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";

		private XmlBinaryDictionaryWriter.MyBinaryWriter original;

		private XmlBinaryDictionaryWriter.MyBinaryWriter writer;

		private XmlBinaryDictionaryWriter.MyBinaryWriter buffer_writer;

		private IXmlDictionary dict_ext;

		private XmlDictionary dict_int = new XmlDictionary();

		private XmlBinaryWriterSession session;

		private bool owns_stream;

		private Encoding utf8Enc = new UTF8Encoding();

		private MemoryStream buffer = new MemoryStream();

		private WriteState state;

		private bool open_start_element;

		private List<KeyValuePair<string, object>> namespaces = new List<KeyValuePair<string, object>>();

		private string xml_lang;

		private XmlSpace xml_space;

		private int ns_index;

		private Stack<int> ns_index_stack = new Stack<int>();

		private Stack<string> xml_lang_stack = new Stack<string>();

		private Stack<XmlSpace> xml_space_stack = new Stack<XmlSpace>();

		private Stack<string> element_ns_stack = new Stack<string>();

		private string element_ns = string.Empty;

		private int element_count;

		private string element_prefix;

		private string attr_value;

		private string current_attr_prefix;

		private object current_attr_name;

		private object current_attr_ns;

		private bool attr_typed_value;

		private XmlBinaryDictionaryWriter.SaveTarget save_target;

		private class MyBinaryWriter : BinaryWriter
		{
			public MyBinaryWriter(Stream s)
				: base(s)
			{
			}

			public void WriteFlexibleInt(int value)
			{
				base.Write7BitEncodedInt(value);
			}
		}

		private enum SaveTarget
		{
			None,
			Namespaces,
			XmlLang,
			XmlSpace
		}
	}
}
