using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Xml
{
	internal class XmlBinaryDictionaryReader : XmlDictionaryReader, IXmlNamespaceResolver
	{
		public XmlBinaryDictionaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quota, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			this.source = new XmlBinaryDictionaryReader.StreamSource(new MemoryStream(buffer, offset, count));
			this.Initialize(dictionary, quota, session, onClose);
		}

		public XmlBinaryDictionaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quota, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			this.source = new XmlBinaryDictionaryReader.StreamSource(stream);
			this.Initialize(dictionary, quota, session, onClose);
		}

		private void Initialize(IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			if (quotas == null)
			{
				throw new ArgumentNullException("quotas");
			}
			if (dictionary == null)
			{
				dictionary = new XmlDictionary();
			}
			this.dictionary = dictionary;
			this.quota = quotas;
			if (session == null)
			{
				session = new XmlBinaryReaderSession();
			}
			this.session = session;
			this.on_close = onClose;
			NameTable nameTable = new NameTable();
			this.context = new XmlParserContext(nameTable, new XmlNamespaceManager(nameTable), null, XmlSpace.None);
			this.current = (this.node = new XmlBinaryDictionaryReader.NodeInfo());
			this.current.Reset();
			this.node_stack.Add(this.node);
		}

		public override int AttributeCount
		{
			get
			{
				return this.attr_count;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.context.BaseURI;
			}
		}

		public override int Depth
		{
			get
			{
				return (this.current != this.node) ? ((this.NodeType != XmlNodeType.Attribute) ? (this.depth + 2) : (this.depth + 1)) : this.depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.state == ReadState.EndOfFile || this.state == ReadState.Error;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.Value.Length > 0;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return false;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.current.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				return (this.current_attr < 0) ? this.current.Prefix : this.attributes[this.current_attr].Prefix;
			}
		}

		public override string LocalName
		{
			get
			{
				return (this.current_attr < 0) ? this.current.LocalName : this.attributes[this.current_attr].LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return (this.current_attr < 0) ? this.current.Name : this.attributes[this.current_attr].Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return (this.current_attr < 0) ? this.current.NS : this.attributes[this.current_attr].NS;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.context.NameTable;
			}
		}

		public override XmlDictionaryReaderQuotas Quotas
		{
			get
			{
				return this.quota;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return this.state;
			}
		}

		public override string Value
		{
			get
			{
				return this.current.Value;
			}
		}

		public override void Close()
		{
			if (this.on_close != null)
			{
				this.on_close(this);
			}
		}

		public override string GetAttribute(int i)
		{
			if (i >= this.attr_count)
			{
				throw new ArgumentOutOfRangeException(string.Format("Specified attribute index is {0} and should be less than {1}", i, this.attr_count));
			}
			return this.attributes[i].Value;
		}

		public override string GetAttribute(string name)
		{
			for (int i = 0; i < this.attr_count; i++)
			{
				if (this.attributes[i].Name == name)
				{
					return this.attributes[i].Value;
				}
			}
			return null;
		}

		public override string GetAttribute(string localName, string ns)
		{
			for (int i = 0; i < this.attr_count; i++)
			{
				if (this.attributes[i].LocalName == localName && this.attributes[i].NS == ns)
				{
					return this.attributes[i].Value;
				}
			}
			return null;
		}

		public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.context.NamespaceManager.GetNamespacesInScope(scope);
		}

		public string LookupPrefix(string ns)
		{
			return this.context.NamespaceManager.LookupPrefix(this.NameTable.Get(ns));
		}

		public override string LookupNamespace(string prefix)
		{
			return this.context.NamespaceManager.LookupNamespace(this.NameTable.Get(prefix));
		}

		public override bool IsArray(out Type type)
		{
			if (this.array_state == XmlNodeType.Element)
			{
				type = this.GetArrayType((int)this.array_item_type);
				return true;
			}
			type = null;
			return false;
		}

		public override bool MoveToElement()
		{
			bool flag = this.current_attr >= 0;
			this.current_attr = -1;
			this.current = this.node;
			return flag;
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.attr_count == 0)
			{
				return false;
			}
			this.current_attr = 0;
			this.current = this.attributes[this.current_attr];
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (++this.current_attr < this.attr_count)
			{
				this.current = this.attributes[this.current_attr];
				return true;
			}
			this.current_attr--;
			return false;
		}

		public override void MoveToAttribute(int i)
		{
			if (i >= this.attr_count)
			{
				throw new ArgumentOutOfRangeException(string.Format("Specified attribute index is {0} and should be less than {1}", i, this.attr_count));
			}
			this.current_attr = i;
			this.current = this.attributes[i];
		}

		public override bool MoveToAttribute(string name)
		{
			for (int i = 0; i < this.attributes.Count; i++)
			{
				if (this.attributes[i].Name == name)
				{
					this.MoveToAttribute(i);
					return true;
				}
			}
			return false;
		}

		public override bool MoveToAttribute(string localName, string ns)
		{
			for (int i = 0; i < this.attributes.Count; i++)
			{
				if (this.attributes[i].LocalName == localName && this.attributes[i].NS == ns)
				{
					this.MoveToAttribute(i);
					return true;
				}
			}
			return false;
		}

		public override bool ReadAttributeValue()
		{
			if (this.current_attr < 0)
			{
				return false;
			}
			int valueIndex = this.attributes[this.current_attr].ValueIndex;
			int num = ((this.current_attr + 1 != this.attr_count) ? this.attributes[this.current_attr + 1].ValueIndex : this.attr_value_count);
			if (valueIndex == num)
			{
				return false;
			}
			if (!this.current.IsAttributeValue)
			{
				this.current = this.attr_values[valueIndex];
				return true;
			}
			return false;
		}

		public override bool Read()
		{
			switch (this.state)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return false;
			default:
			{
				this.state = ReadState.Interactive;
				this.MoveToElement();
				this.attr_count = 0;
				this.attr_value_count = 0;
				this.ns_slot = 0;
				if (this.node.NodeType == XmlNodeType.Element)
				{
					if (this.node_stack.Count <= ++this.depth)
					{
						if (this.depth == this.quota.MaxDepth)
						{
							throw new XmlException(string.Format("Binary XML stream quota exceeded. Depth must be less than {0}", this.quota.MaxDepth));
						}
						this.node = new XmlBinaryDictionaryReader.NodeInfo();
						this.node_stack.Add(this.node);
					}
					else
					{
						this.node = this.node_stack[this.depth];
						this.node.Reset();
					}
				}
				this.current = this.node;
				if (this.is_next_end_element)
				{
					this.is_next_end_element = false;
					this.node.Reset();
					this.ProcessEndElement();
					return true;
				}
				XmlNodeType xmlNodeType = this.array_state;
				switch (xmlNodeType)
				{
				case XmlNodeType.Element:
					this.ReadArrayItem();
					return true;
				default:
				{
					if (xmlNodeType == XmlNodeType.EndElement)
					{
						if (--this.array_item_remaining != 0)
						{
							this.ShiftToArrayItemElement();
							return true;
						}
						this.array_state = XmlNodeType.None;
					}
					this.node.Reset();
					int num = ((this.next < 0) ? this.source.ReadByte() : this.next);
					this.next = -1;
					if (num < 0)
					{
						this.state = ReadState.EndOfFile;
						this.current.Reset();
						return false;
					}
					this.is_next_end_element = num > 128 && (num & 1) == 1;
					num -= ((!this.is_next_end_element) ? 0 : 1);
					int num2 = num;
					switch (num2)
					{
					case 64:
					case 65:
					case 66:
					case 67:
						break;
					default:
						switch (num2)
						{
						case 1:
							this.ProcessEndElement();
							return true;
						case 2:
							this.node.Value = this.ReadUTF8();
							this.node.ValueType = 2;
							this.node.NodeType = XmlNodeType.Comment;
							return true;
						case 3:
							num = (int)this.ReadByteOrError();
							this.ReadElementBinary((int)((byte)num));
							num = (int)this.ReadByteOrError();
							if (num != 1)
							{
								throw new XmlException(string.Format("EndElement is expected after element in an array. The actual byte was {0:X} in hexadecimal", num));
							}
							num = (int)(this.ReadByteOrError() - 1);
							this.VerifyValidArrayItemType(num);
							if (num < 0)
							{
								throw new XmlException("The stream has ended where the array item type is expected");
							}
							this.array_item_type = (byte)num;
							this.array_item_remaining = this.ReadVariantSize();
							if (this.array_item_remaining > this.quota.MaxArrayLength)
							{
								throw new Exception(string.Format("Binary xml stream exceeded max array length quota. Items are {0} and should be less than quota.MaxArrayLength", this.quota.MaxArrayLength));
							}
							this.array_state = XmlNodeType.Element;
							return true;
						default:
							if ((68 > num || num > 93) && (94 > num || num > 119))
							{
								this.ReadTextOrValue((byte)num, this.node, false);
								return true;
							}
							break;
						}
						break;
					}
					this.ReadElementBinary((int)((byte)num));
					return true;
				}
				case XmlNodeType.Text:
					this.ShiftToArrayItemEndElement();
					return true;
				}
				break;
			}
			}
		}

		private void ReadArrayItem()
		{
			this.ReadTextOrValue(this.array_item_type, this.node, false);
			this.array_state = XmlNodeType.Text;
		}

		private void ShiftToArrayItemEndElement()
		{
			this.ProcessEndElement();
			this.array_state = XmlNodeType.EndElement;
		}

		private void ShiftToArrayItemElement()
		{
			this.node.NodeType = XmlNodeType.Element;
			this.context.NamespaceManager.PushScope();
			this.array_state = XmlNodeType.Element;
		}

		private void VerifyValidArrayItemType(int ident)
		{
			if (this.GetArrayType(ident) == null)
			{
				throw new XmlException(string.Format("Unexpected array item type {0:X} in hexadecimal", ident));
			}
		}

		private Type GetArrayType(int ident)
		{
			switch (ident)
			{
			case 138:
				return typeof(short);
			default:
				switch (ident)
				{
				case 174:
					return typeof(TimeSpan);
				default:
					if (ident != 180)
					{
						return null;
					}
					return typeof(bool);
				case 176:
					return typeof(Guid);
				}
				break;
			case 140:
				return typeof(int);
			case 142:
				return typeof(long);
			case 144:
				return typeof(float);
			case 146:
				return typeof(double);
			case 148:
				return typeof(decimal);
			case 150:
				return typeof(DateTime);
			}
		}

		private void ProcessEndElement()
		{
			if (this.depth == 0)
			{
				throw new XmlException("Unexpected end of element while there is no element started.");
			}
			this.current = (this.node = this.node_stack[--this.depth]);
			this.node.NodeType = XmlNodeType.EndElement;
			this.context.NamespaceManager.PopScope();
		}

		private void ReadElementBinary(int ident)
		{
			this.node.NodeType = XmlNodeType.Element;
			this.node.Prefix = string.Empty;
			this.context.NamespaceManager.PushScope();
			switch (ident)
			{
			case 64:
				break;
			case 65:
				this.node.Prefix = this.ReadUTF8();
				this.node.NSSlot = this.ns_slot++;
				break;
			case 66:
				goto IL_0096;
			case 67:
				this.node.Prefix = this.ReadUTF8();
				this.node.NSSlot = this.ns_slot++;
				goto IL_0096;
			default:
				if (68 <= ident && ident <= 93)
				{
					this.node.Prefix = ((char)(ident - 68 + 97)).ToString();
					this.node.DictLocalName = this.ReadDictName();
				}
				else
				{
					if (94 > ident || ident > 119)
					{
						throw new XmlException(string.Format("Invalid element node type {0:X02} in hexadecimal", ident));
					}
					this.node.Prefix = ((char)(ident - 94 + 97)).ToString();
					this.node.LocalName = this.ReadUTF8();
				}
				goto IL_017F;
			}
			this.node.LocalName = this.ReadUTF8();
			goto IL_017F;
			IL_0096:
			this.node.DictLocalName = this.ReadDictName();
			IL_017F:
			bool flag = true;
			do
			{
				ident = (int)this.ReadByteOrError();
				switch (ident)
				{
				case 4:
				case 5:
				case 6:
				case 7:
					this.ReadAttribute((byte)ident);
					break;
				case 8:
				case 9:
				case 10:
				case 11:
					this.ReadNamespace((byte)ident);
					break;
				default:
					if ((38 <= ident && ident <= 63) || (12 <= ident && ident <= 37))
					{
						this.ReadAttribute((byte)ident);
					}
					else
					{
						this.next = ident;
						flag = false;
					}
					break;
				}
			}
			while (flag);
			this.node.NS = this.context.NamespaceManager.LookupNamespace(this.node.Prefix) ?? string.Empty;
			foreach (XmlBinaryDictionaryReader.AttrNodeInfo attrNodeInfo in this.attributes)
			{
				if (attrNodeInfo.Prefix.Length > 0)
				{
					attrNodeInfo.NS = this.context.NamespaceManager.LookupNamespace(attrNodeInfo.Prefix);
				}
			}
			this.ns_store.Clear();
			this.ns_dict_store.Clear();
		}

		private void ReadAttribute(byte ident)
		{
			if (this.attributes.Count == this.attr_count)
			{
				this.attributes.Add(new XmlBinaryDictionaryReader.AttrNodeInfo(this));
			}
			XmlBinaryDictionaryReader.AttrNodeInfo attrNodeInfo = this.attributes[this.attr_count++];
			attrNodeInfo.Reset();
			attrNodeInfo.Position = this.source.Position;
			switch (ident)
			{
			case 4:
				break;
			case 5:
				attrNodeInfo.Prefix = this.ReadUTF8();
				attrNodeInfo.NSSlot = this.ns_slot++;
				break;
			case 6:
				goto IL_00B3;
			case 7:
				attrNodeInfo.Prefix = this.ReadUTF8();
				attrNodeInfo.NSSlot = this.ns_slot++;
				goto IL_00B3;
			default:
				if (38 <= ident && ident <= 63)
				{
					attrNodeInfo.Prefix = ((char)(97 + ident - 38)).ToString();
					attrNodeInfo.LocalName = this.ReadUTF8();
					goto IL_0171;
				}
				if (12 <= ident && ident <= 37)
				{
					attrNodeInfo.Prefix = ((char)(97 + ident - 12)).ToString();
					attrNodeInfo.DictLocalName = this.ReadDictName();
					goto IL_0171;
				}
				throw new XmlException(string.Format("Unexpected attribute node type: 0x{0:X02}", ident));
			}
			attrNodeInfo.LocalName = this.ReadUTF8();
			goto IL_0171;
			IL_00B3:
			attrNodeInfo.DictLocalName = this.ReadDictName();
			IL_0171:
			this.ReadAttributeValueBinary(attrNodeInfo);
		}

		private void ReadNamespace(byte ident)
		{
			if (this.attributes.Count == this.attr_count)
			{
				this.attributes.Add(new XmlBinaryDictionaryReader.AttrNodeInfo(this));
			}
			XmlBinaryDictionaryReader.AttrNodeInfo attrNodeInfo = this.attributes[this.attr_count++];
			attrNodeInfo.Reset();
			attrNodeInfo.Position = this.source.Position;
			string text = null;
			string text2 = null;
			switch (ident)
			{
			case 8:
				text = string.Empty;
				text2 = this.ReadUTF8();
				break;
			case 9:
				text = this.ReadUTF8();
				text2 = this.ReadUTF8();
				break;
			case 10:
			{
				text = string.Empty;
				XmlDictionaryString xmlDictionaryString = this.ReadDictName();
				this.ns_dict_store.Add(this.ns_store.Count, xmlDictionaryString);
				text2 = xmlDictionaryString.Value;
				break;
			}
			case 11:
			{
				text = this.ReadUTF8();
				XmlDictionaryString xmlDictionaryString = this.ReadDictName();
				this.ns_dict_store.Add(this.ns_store.Count, xmlDictionaryString);
				text2 = xmlDictionaryString.Value;
				break;
			}
			}
			attrNodeInfo.Prefix = ((text.Length <= 0) ? string.Empty : "xmlns");
			attrNodeInfo.LocalName = ((text.Length <= 0) ? "xmlns" : text);
			attrNodeInfo.NS = "http://www.w3.org/2000/xmlns/";
			attrNodeInfo.ValueIndex = this.attr_value_count;
			if (this.attr_value_count == this.attr_values.Count)
			{
				this.attr_values.Add(new XmlBinaryDictionaryReader.NodeInfo(true));
			}
			XmlBinaryDictionaryReader.NodeInfo nodeInfo = this.attr_values[this.attr_value_count++];
			nodeInfo.Reset();
			nodeInfo.Value = text2;
			nodeInfo.ValueType = 152;
			nodeInfo.NodeType = XmlNodeType.Text;
			this.ns_store.Add(new XmlQualifiedName(text, text2));
			this.context.NamespaceManager.AddNamespace(text, text2);
		}

		private void ReadAttributeValueBinary(XmlBinaryDictionaryReader.AttrNodeInfo a)
		{
			a.ValueIndex = this.attr_value_count;
			if (this.attr_value_count == this.attr_values.Count)
			{
				this.attr_values.Add(new XmlBinaryDictionaryReader.NodeInfo(true));
			}
			XmlBinaryDictionaryReader.NodeInfo nodeInfo = this.attr_values[this.attr_value_count++];
			nodeInfo.Reset();
			int num = (int)this.ReadByteOrError();
			bool flag = num > 128 && (num & 1) == 1;
			num -= ((!flag) ? 0 : 1);
			this.ReadTextOrValue((byte)num, nodeInfo, true);
		}

		private bool ReadTextOrValue(byte ident, XmlBinaryDictionaryReader.NodeInfo node, bool canSkip)
		{
			node.Value = null;
			node.ValueType = ident;
			node.NodeType = XmlNodeType.Text;
			int num;
			switch (ident)
			{
			case 128:
				node.TypedValue = 0;
				return true;
			default:
				switch (ident)
				{
				case 168:
					node.Value = string.Empty;
					node.NodeType = XmlNodeType.Text;
					return true;
				default:
					switch (ident)
					{
					case 182:
					case 184:
					case 186:
						goto IL_0398;
					}
					if (!canSkip)
					{
						throw new ArgumentException(string.Format("Unexpected binary XML data at position {1}: {0:X}", (int)(ident + ((!this.is_next_end_element) ? 0 : 1)), this.source.Position));
					}
					this.next = (int)ident;
					return false;
				case 170:
					node.DictValue = this.ReadDictName();
					node.NodeType = XmlNodeType.Text;
					return true;
				case 172:
				{
					byte[] array = new byte[16];
					this.source.Reader.Read(array, 0, array.Length);
					node.TypedValue = new UniqueId(new Guid(array));
					return true;
				}
				case 174:
					node.TypedValue = new TimeSpan(this.source.Reader.ReadInt64());
					return true;
				case 176:
				{
					byte[] array = new byte[16];
					this.source.Reader.Read(array, 0, array.Length);
					node.TypedValue = new Guid(array);
					return true;
				}
				}
				break;
			case 130:
				node.TypedValue = 1;
				return true;
			case 132:
				node.TypedValue = false;
				return true;
			case 134:
				node.TypedValue = true;
				return true;
			case 136:
				node.TypedValue = this.ReadByteOrError();
				return true;
			case 138:
				node.TypedValue = this.source.Reader.ReadInt16();
				return true;
			case 140:
				node.TypedValue = this.source.Reader.ReadInt32();
				return true;
			case 142:
				node.TypedValue = this.source.Reader.ReadInt64();
				return true;
			case 144:
				node.TypedValue = this.source.Reader.ReadSingle();
				return true;
			case 146:
				node.TypedValue = this.source.Reader.ReadDouble();
				return true;
			case 148:
			{
				int[] array2 = new int[]
				{
					0,
					0,
					0,
					this.source.Reader.ReadInt32()
				};
				array2[2] = this.source.Reader.ReadInt32();
				array2[0] = this.source.Reader.ReadInt32();
				array2[1] = this.source.Reader.ReadInt32();
				node.TypedValue = new decimal(array2);
				return true;
			}
			case 150:
				node.TypedValue = new DateTime(this.source.Reader.ReadInt64());
				return true;
			case 152:
			case 154:
			case 156:
				break;
			case 158:
			case 160:
			case 162:
			{
				num = ((ident != 158) ? ((ident != 160) ? this.source.Reader.ReadInt32() : ((int)this.source.Reader.ReadUInt16())) : ((int)this.source.Reader.ReadByte()));
				byte[] array3 = this.Alloc(num);
				this.source.Reader.Read(array3, 0, array3.Length);
				node.TypedValue = array3;
				return true;
			}
			}
			IL_0398:
			Encoding encoding = ((ident > 156) ? Encoding.Unicode : Encoding.UTF8);
			num = ((ident != 152 && ident != 182) ? ((ident != 154 && ident != 184) ? this.source.Reader.ReadInt32() : ((int)this.source.Reader.ReadUInt16())) : ((int)this.source.Reader.ReadByte()));
			byte[] array4 = this.Alloc(num);
			this.source.Reader.Read(array4, 0, num);
			node.Value = encoding.GetString(array4, 0, num);
			node.NodeType = XmlNodeType.Text;
			return true;
		}

		private byte[] Alloc(int size)
		{
			if (size > this.quota.MaxStringContentLength || size < 0)
			{
				throw new XmlException(string.Format("Text content buffer exceeds the quota limitation at {2}. {0} bytes and should be less than {1} bytes", size, this.quota.MaxStringContentLength, this.source.Position));
			}
			return new byte[size];
		}

		private int ReadVariantSize()
		{
			int num = 0;
			int num2 = 0;
			byte b;
			do
			{
				b = this.ReadByteOrError();
				num += (int)(b & 127) << num2;
				num2 += 7;
			}
			while (b >= 128);
			return num;
		}

		private string ReadUTF8()
		{
			int num = this.ReadVariantSize();
			if (num == 0)
			{
				return string.Empty;
			}
			if (this.tmp_buffer.Length < num)
			{
				int num2 = this.tmp_buffer.Length * 2;
				this.tmp_buffer = this.Alloc((num >= num2) ? num : num2);
			}
			num = this.source.Read(this.tmp_buffer, 0, num);
			return this.utf8enc.GetString(this.tmp_buffer, 0, num);
		}

		private XmlDictionaryString ReadDictName()
		{
			int num = this.ReadVariantSize();
			XmlDictionaryString xmlDictionaryString;
			if ((num & 1) == 1)
			{
				if (this.session.TryLookup(num >> 1, out xmlDictionaryString))
				{
					return xmlDictionaryString;
				}
			}
			else if (this.dictionary.TryLookup(num >> 1, out xmlDictionaryString))
			{
				return xmlDictionaryString;
			}
			throw new XmlException(string.Format("Input XML binary stream is invalid. No matching XML dictionary string entry at {0}. Binary stream position at {1}", num, this.source.Position));
		}

		private byte ReadByteOrError()
		{
			if (this.next >= 0)
			{
				byte b = (byte)this.next;
				this.next = -1;
				return b;
			}
			int num = this.source.ReadByte();
			if (num < 0)
			{
				throw new XmlException(string.Format("Unexpected end of binary stream. Position is at {0}", this.source.Position));
			}
			return (byte)num;
		}

		public override void ResolveEntity()
		{
			throw new NotSupportedException("this XmlReader does not support ResolveEntity.");
		}

		public override bool TryGetBase64ContentLength(out int length)
		{
			length = 0;
			switch (this.current.ValueType)
			{
			case 158:
			case 160:
			case 162:
				length = ((byte[])this.current.TypedValue).Length;
				return true;
			}
			return false;
		}

		public override string ReadContentAsString()
		{
			string text = string.Empty;
			for (;;)
			{
				XmlNodeType nodeType = this.NodeType;
				switch (nodeType)
				{
				case XmlNodeType.Element:
					return text;
				default:
					if (nodeType == XmlNodeType.EndElement)
					{
						return text;
					}
					break;
				case XmlNodeType.Text:
					text += this.Value;
					break;
				}
				if (!this.Read())
				{
					return text;
				}
			}
			return text;
		}

		public override int ReadContentAsInt()
		{
			int intValue = this.GetIntValue();
			this.Read();
			return intValue;
		}

		private int GetIntValue()
		{
			byte valueType = this.node.ValueType;
			switch (valueType)
			{
			case 136:
				return (int)((byte)this.current.TypedValue);
			default:
				switch (valueType)
				{
				case 128:
					return 0;
				case 130:
					return 1;
				}
				throw new InvalidOperationException(string.Format("Current content is not an integer. (Internal value type:{0:X02})", (int)this.node.ValueType));
			case 138:
				return (int)((short)this.current.TypedValue);
			case 140:
				return (int)this.current.TypedValue;
			}
		}

		public override long ReadContentAsLong()
		{
			if (this.node.ValueType == 142)
			{
				long num = (long)this.current.TypedValue;
				this.Read();
				return num;
			}
			return (long)this.ReadContentAsInt();
		}

		public override float ReadContentAsFloat()
		{
			if (this.node.ValueType != 144)
			{
				throw new InvalidOperationException("Current content is not a single");
			}
			float num = (float)this.current.TypedValue;
			this.Read();
			return num;
		}

		public override double ReadContentAsDouble()
		{
			if (this.node.ValueType != 146)
			{
				throw new InvalidOperationException("Current content is not a double");
			}
			double num = (double)this.current.TypedValue;
			this.Read();
			return num;
		}

		private bool IsBase64Node(byte b)
		{
			switch (b)
			{
			case 158:
			case 160:
			case 162:
				return true;
			}
			return false;
		}

		public override byte[] ReadContentAsBase64()
		{
			byte[] array = null;
			if (!this.IsBase64Node(this.node.ValueType))
			{
				throw new InvalidOperationException("Current content is not base64");
			}
			while (this.NodeType == XmlNodeType.Text && this.IsBase64Node(this.node.ValueType))
			{
				if (array == null)
				{
					array = (byte[])this.node.TypedValue;
				}
				else
				{
					byte[] array2 = (byte[])this.node.TypedValue;
					byte[] array3 = this.Alloc(array.Length + array2.Length);
					Array.Copy(array, array3, array.Length);
					Array.Copy(array2, 0, array3, array.Length, array2.Length);
					array = array3;
				}
				this.Read();
			}
			return array;
		}

		public override Guid ReadContentAsGuid()
		{
			if (this.node.ValueType != 176)
			{
				throw new InvalidOperationException("Current content is not a Guid");
			}
			Guid guid = (Guid)this.node.TypedValue;
			this.Read();
			return guid;
		}

		public override UniqueId ReadContentAsUniqueId()
		{
			byte valueType = this.node.ValueType;
			UniqueId uniqueId;
			switch (valueType)
			{
			case 152:
			case 154:
			case 156:
				break;
			default:
				switch (valueType)
				{
				case 182:
				case 184:
				case 186:
					break;
				default:
					if (valueType != 172)
					{
						throw new InvalidOperationException("Current content is not a UniqueId");
					}
					uniqueId = (UniqueId)this.node.TypedValue;
					this.Read();
					return uniqueId;
				}
				break;
			}
			uniqueId = new UniqueId(this.node.Value);
			this.Read();
			return uniqueId;
		}

		private XmlBinaryDictionaryReader.ISource source;

		private IXmlDictionary dictionary;

		private XmlDictionaryReaderQuotas quota;

		private XmlBinaryReaderSession session;

		private OnXmlDictionaryReaderClose on_close;

		private XmlParserContext context;

		private ReadState state;

		private XmlBinaryDictionaryReader.NodeInfo node;

		private XmlBinaryDictionaryReader.NodeInfo current;

		private List<XmlBinaryDictionaryReader.AttrNodeInfo> attributes = new List<XmlBinaryDictionaryReader.AttrNodeInfo>();

		private List<XmlBinaryDictionaryReader.NodeInfo> attr_values = new List<XmlBinaryDictionaryReader.NodeInfo>();

		private List<XmlBinaryDictionaryReader.NodeInfo> node_stack = new List<XmlBinaryDictionaryReader.NodeInfo>();

		private List<XmlQualifiedName> ns_store = new List<XmlQualifiedName>();

		private Dictionary<int, XmlDictionaryString> ns_dict_store = new Dictionary<int, XmlDictionaryString>();

		private int attr_count;

		private int attr_value_count;

		private int current_attr = -1;

		private int depth;

		private int ns_slot;

		private int next = -1;

		private bool is_next_end_element;

		private byte[] tmp_buffer = new byte[128];

		private UTF8Encoding utf8enc = new UTF8Encoding();

		private int array_item_remaining;

		private byte array_item_type;

		private XmlNodeType array_state;

		internal interface ISource
		{
			int Position { get; }

			int ReadByte();

			int Read(byte[] data, int offset, int count);

			BinaryReader Reader { get; }
		}

		internal class StreamSource : XmlBinaryDictionaryReader.ISource
		{
			public StreamSource(Stream stream)
			{
				this.reader = new BinaryReader(stream);
			}

			public int Position
			{
				get
				{
					return (int)this.reader.BaseStream.Position;
				}
			}

			public BinaryReader Reader
			{
				get
				{
					return this.reader;
				}
			}

			public int ReadByte()
			{
				if (this.reader.PeekChar() < 0)
				{
					return -1;
				}
				return (int)this.reader.ReadByte();
			}

			public int Read(byte[] data, int offset, int count)
			{
				return this.reader.Read(data, offset, count);
			}

			private BinaryReader reader;
		}

		private class NodeInfo
		{
			public NodeInfo()
			{
			}

			public NodeInfo(bool isAttr)
			{
				this.IsAttributeValue = isAttr;
			}

			public string LocalName
			{
				get
				{
					return (this.DictLocalName == null) ? this.local_name : this.DictLocalName.Value;
				}
				set
				{
					this.DictLocalName = null;
					this.local_name = value;
				}
			}

			public string NS
			{
				get
				{
					return (this.DictNS == null) ? this.ns : this.DictNS.Value;
				}
				set
				{
					this.DictNS = null;
					this.ns = value;
				}
			}

			public string Name
			{
				get
				{
					if (this.name.Length == 0)
					{
						this.name = ((this.Prefix.Length <= 0) ? this.LocalName : (this.Prefix + ":" + this.LocalName));
					}
					return this.name;
				}
			}

			public virtual string Value
			{
				get
				{
					byte valueType = this.ValueType;
					switch (valueType)
					{
					case 128:
						return "0";
					default:
						switch (valueType)
						{
						case 150:
							return XmlConvert.ToString((DateTime)this.TypedValue, XmlDateTimeSerializationMode.RoundtripKind);
						default:
							switch (valueType)
							{
							case 168:
								break;
							default:
								switch (valueType)
								{
								case 182:
								case 184:
								case 186:
									break;
								default:
									switch (valueType)
									{
									case 0:
									case 2:
										goto IL_0106;
									}
									throw new NotImplementedException(string.Concat(new object[] { "ValueType ", this.ValueType, " on node ", this.NodeType }));
								}
								break;
							case 170:
								return this.DictValue.Value;
							case 172:
								return this.TypedValue.ToString();
							case 174:
								return XmlConvert.ToString((TimeSpan)this.TypedValue);
							case 176:
								return XmlConvert.ToString((Guid)this.TypedValue);
							}
							break;
						case 152:
						case 154:
						case 156:
							break;
						case 158:
						case 160:
						case 162:
							return Convert.ToBase64String((byte[])this.TypedValue);
						}
						IL_0106:
						return this.value;
					case 130:
						return "1";
					case 132:
						return "false";
					case 134:
						return "true";
					case 136:
						return XmlConvert.ToString((byte)this.TypedValue);
					case 138:
						return XmlConvert.ToString((short)this.TypedValue);
					case 140:
						return XmlConvert.ToString((int)this.TypedValue);
					case 142:
						return XmlConvert.ToString((long)this.TypedValue);
					case 144:
						return XmlConvert.ToString((float)this.TypedValue);
					case 146:
						return XmlConvert.ToString((double)this.TypedValue);
					}
				}
				set
				{
					this.value = value;
				}
			}

			public virtual void Reset()
			{
				this.Position = 0;
				this.DictLocalName = (this.DictNS = null);
				string text = string.Empty;
				this.Value = text;
				text = (this.Prefix = text);
				this.NS = text;
				this.LocalName = text;
				this.NodeType = XmlNodeType.None;
				this.TypedValue = null;
				this.ValueType = 0;
				this.NSSlot = -1;
			}

			public bool IsAttributeValue;

			public int Position;

			public string Prefix;

			public XmlDictionaryString DictLocalName;

			public XmlDictionaryString DictNS;

			public XmlDictionaryString DictValue;

			public XmlNodeType NodeType;

			public object TypedValue;

			public byte ValueType;

			public int NSSlot;

			private string name = string.Empty;

			private string local_name = string.Empty;

			private string ns = string.Empty;

			private string value;
		}

		private class AttrNodeInfo : XmlBinaryDictionaryReader.NodeInfo
		{
			public AttrNodeInfo(XmlBinaryDictionaryReader owner)
			{
				this.owner = owner;
			}

			public override void Reset()
			{
				base.Reset();
				this.ValueIndex = -1;
				this.NodeType = XmlNodeType.Attribute;
			}

			public override string Value
			{
				get
				{
					return this.owner.attr_values[this.ValueIndex].Value;
				}
			}

			private XmlBinaryDictionaryReader owner;

			public int ValueIndex;
		}
	}
}
