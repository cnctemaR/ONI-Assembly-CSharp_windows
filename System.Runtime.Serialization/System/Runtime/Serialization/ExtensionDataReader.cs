using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class ExtensionDataReader : XmlReader
	{
		[SecuritySafeCritical]
		static ExtensionDataReader()
		{
			ExtensionDataReader.AddPrefix("i", "http://www.w3.org/2001/XMLSchema-instance");
			ExtensionDataReader.AddPrefix("z", "http://schemas.microsoft.com/2003/10/Serialization/");
			ExtensionDataReader.AddPrefix(string.Empty, string.Empty);
		}

		internal ExtensionDataReader(XmlObjectSerializerReadContext context)
		{
			this.attributeIndex = -1;
			this.context = context;
		}

		internal void SetDeserializedValue(object obj)
		{
			IDataNode dataNode = ((this.deserializedDataNodes == null || this.deserializedDataNodes.Count == 0) ? null : this.deserializedDataNodes.Dequeue());
			if (dataNode != null && !(obj is IDataNode))
			{
				dataNode.Value = obj;
				dataNode.IsFinalValue = true;
			}
		}

		internal IDataNode GetCurrentNode()
		{
			IDataNode dataNode = this.element.dataNode;
			this.Skip();
			return dataNode;
		}

		internal void SetDataNode(IDataNode dataNode, string name, string ns)
		{
			this.SetNextElement(dataNode, name, ns, null);
			this.element = this.nextElement;
			this.nextElement = null;
			this.SetElement();
		}

		internal void Reset()
		{
			this.localName = null;
			this.ns = null;
			this.prefix = null;
			this.value = null;
			this.attributeCount = 0;
			this.attributeIndex = -1;
			this.depth = 0;
			this.element = null;
			this.nextElement = null;
			this.elements = null;
			this.deserializedDataNodes = null;
		}

		private bool IsXmlDataNode
		{
			get
			{
				return this.internalNodeType == ExtensionDataReader.ExtensionDataNodeType.Xml;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.nodeType;
				}
				return this.xmlNodeReader.NodeType;
			}
		}

		public override string LocalName
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.localName;
				}
				return this.xmlNodeReader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.ns;
				}
				return this.xmlNodeReader.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.prefix;
				}
				return this.xmlNodeReader.Prefix;
			}
		}

		public override string Value
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.value;
				}
				return this.xmlNodeReader.Value;
			}
		}

		public override int Depth
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.depth;
				}
				return this.xmlNodeReader.Depth;
			}
		}

		public override int AttributeCount
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.attributeCount;
				}
				return this.xmlNodeReader.AttributeCount;
			}
		}

		public override bool EOF
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.readState == ReadState.EndOfFile;
				}
				return this.xmlNodeReader.EOF;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.readState;
				}
				return this.xmlNodeReader.ReadState;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.IsXmlDataNode && this.xmlNodeReader.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return base.IsDefault;
				}
				return this.xmlNodeReader.IsDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return base.QuoteChar;
				}
				return this.xmlNodeReader.QuoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return base.XmlSpace;
				}
				return this.xmlNodeReader.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return base.XmlLang;
				}
				return this.xmlNodeReader.XmlLang;
			}
		}

		public override string this[int i]
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.GetAttribute(i);
				}
				return this.xmlNodeReader[i];
			}
		}

		public override string this[string name]
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.GetAttribute(name);
				}
				return this.xmlNodeReader[name];
			}
		}

		public override string this[string name, string namespaceURI]
		{
			get
			{
				if (!this.IsXmlDataNode)
				{
					return this.GetAttribute(name, namespaceURI);
				}
				return this.xmlNodeReader[name, namespaceURI];
			}
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.IsXmlDataNode)
			{
				return this.xmlNodeReader.MoveToFirstAttribute();
			}
			if (this.attributeCount == 0)
			{
				return false;
			}
			this.MoveToAttribute(0);
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (this.IsXmlDataNode)
			{
				return this.xmlNodeReader.MoveToNextAttribute();
			}
			if (this.attributeIndex + 1 >= this.attributeCount)
			{
				return false;
			}
			this.MoveToAttribute(this.attributeIndex + 1);
			return true;
		}

		public override void MoveToAttribute(int index)
		{
			if (this.IsXmlDataNode)
			{
				this.xmlNodeReader.MoveToAttribute(index);
				return;
			}
			if (index < 0 || index >= this.attributeCount)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid XML while deserializing extension data.")));
			}
			this.nodeType = XmlNodeType.Attribute;
			AttributeData attributeData = this.element.attributes[index];
			this.localName = attributeData.localName;
			this.ns = attributeData.ns;
			this.prefix = attributeData.prefix;
			this.value = attributeData.value;
			this.attributeIndex = index;
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			if (this.IsXmlDataNode)
			{
				return this.xmlNodeReader.GetAttribute(name, namespaceURI);
			}
			for (int i = 0; i < this.element.attributeCount; i++)
			{
				AttributeData attributeData = this.element.attributes[i];
				if (attributeData.localName == name && attributeData.ns == namespaceURI)
				{
					return attributeData.value;
				}
			}
			return null;
		}

		public override bool MoveToAttribute(string name, string namespaceURI)
		{
			if (this.IsXmlDataNode)
			{
				return this.xmlNodeReader.MoveToAttribute(name, this.ns);
			}
			for (int i = 0; i < this.element.attributeCount; i++)
			{
				AttributeData attributeData = this.element.attributes[i];
				if (attributeData.localName == name && attributeData.ns == namespaceURI)
				{
					this.MoveToAttribute(i);
					return true;
				}
			}
			return false;
		}

		public override bool MoveToElement()
		{
			if (this.IsXmlDataNode)
			{
				return this.xmlNodeReader.MoveToElement();
			}
			if (this.nodeType != XmlNodeType.Attribute)
			{
				return false;
			}
			this.SetElement();
			return true;
		}

		private void SetElement()
		{
			this.nodeType = XmlNodeType.Element;
			this.localName = this.element.localName;
			this.ns = this.element.ns;
			this.prefix = this.element.prefix;
			this.value = string.Empty;
			this.attributeCount = this.element.attributeCount;
			this.attributeIndex = -1;
		}

		[SecuritySafeCritical]
		public override string LookupNamespace(string prefix)
		{
			if (this.IsXmlDataNode)
			{
				return this.xmlNodeReader.LookupNamespace(prefix);
			}
			string text;
			if (!ExtensionDataReader.prefixToNsTable.TryGetValue(prefix, out text))
			{
				return null;
			}
			return text;
		}

		public override void Skip()
		{
			if (this.IsXmlDataNode)
			{
				this.xmlNodeReader.Skip();
				return;
			}
			if (this.ReadState != ReadState.Interactive)
			{
				return;
			}
			this.MoveToElement();
			if (this.IsElementNode(this.internalNodeType))
			{
				int num = 1;
				while (num != 0)
				{
					if (!this.Read())
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid XML while deserializing extension data.")));
					}
					if (this.IsElementNode(this.internalNodeType))
					{
						num++;
					}
					else if (this.internalNodeType == ExtensionDataReader.ExtensionDataNodeType.EndElement)
					{
						this.ReadEndElement();
						num--;
					}
				}
				return;
			}
			this.Read();
		}

		private bool IsElementNode(ExtensionDataReader.ExtensionDataNodeType nodeType)
		{
			return nodeType == ExtensionDataReader.ExtensionDataNodeType.Element || nodeType == ExtensionDataReader.ExtensionDataNodeType.ReferencedElement || nodeType == ExtensionDataReader.ExtensionDataNodeType.NullElement;
		}

		public override void Close()
		{
			if (this.IsXmlDataNode)
			{
				this.xmlNodeReader.Close();
				return;
			}
			this.Reset();
			this.readState = ReadState.Closed;
		}

		public override bool Read()
		{
			if (this.nodeType == XmlNodeType.Attribute && this.MoveToNextAttribute())
			{
				return true;
			}
			this.MoveNext(this.element.dataNode);
			switch (this.internalNodeType)
			{
			case ExtensionDataReader.ExtensionDataNodeType.None:
				if (this.depth != 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid XML while deserializing extension data.")));
				}
				this.nodeType = XmlNodeType.None;
				this.prefix = string.Empty;
				this.ns = string.Empty;
				this.localName = string.Empty;
				this.value = string.Empty;
				this.attributeCount = 0;
				this.readState = ReadState.EndOfFile;
				return false;
			case ExtensionDataReader.ExtensionDataNodeType.Element:
			case ExtensionDataReader.ExtensionDataNodeType.ReferencedElement:
			case ExtensionDataReader.ExtensionDataNodeType.NullElement:
				this.PushElement();
				this.SetElement();
				break;
			case ExtensionDataReader.ExtensionDataNodeType.EndElement:
				this.nodeType = XmlNodeType.EndElement;
				this.prefix = string.Empty;
				this.ns = string.Empty;
				this.localName = string.Empty;
				this.value = string.Empty;
				this.attributeCount = 0;
				this.attributeIndex = -1;
				this.PopElement();
				break;
			case ExtensionDataReader.ExtensionDataNodeType.Text:
				this.nodeType = XmlNodeType.Text;
				this.prefix = string.Empty;
				this.ns = string.Empty;
				this.localName = string.Empty;
				this.attributeCount = 0;
				this.attributeIndex = -1;
				break;
			case ExtensionDataReader.ExtensionDataNodeType.Xml:
				break;
			default:
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("Invalid state in extension data reader.")));
			}
			this.readState = ReadState.Interactive;
			return true;
		}

		public override string Name
		{
			get
			{
				if (this.IsXmlDataNode)
				{
					return this.xmlNodeReader.Name;
				}
				return string.Empty;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.IsXmlDataNode && this.xmlNodeReader.HasValue;
			}
		}

		public override string BaseURI
		{
			get
			{
				if (this.IsXmlDataNode)
				{
					return this.xmlNodeReader.BaseURI;
				}
				return string.Empty;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				if (this.IsXmlDataNode)
				{
					return this.xmlNodeReader.NameTable;
				}
				return null;
			}
		}

		public override string GetAttribute(string name)
		{
			if (this.IsXmlDataNode)
			{
				return this.xmlNodeReader.GetAttribute(name);
			}
			return null;
		}

		public override string GetAttribute(int i)
		{
			if (this.IsXmlDataNode)
			{
				return this.xmlNodeReader.GetAttribute(i);
			}
			return null;
		}

		public override bool MoveToAttribute(string name)
		{
			return this.IsXmlDataNode && this.xmlNodeReader.MoveToAttribute(name);
		}

		public override void ResolveEntity()
		{
			if (this.IsXmlDataNode)
			{
				this.xmlNodeReader.ResolveEntity();
			}
		}

		public override bool ReadAttributeValue()
		{
			return this.IsXmlDataNode && this.xmlNodeReader.ReadAttributeValue();
		}

		private void MoveNext(IDataNode dataNode)
		{
			ExtensionDataReader.ExtensionDataNodeType extensionDataNodeType = this.internalNodeType;
			if (extensionDataNodeType == ExtensionDataReader.ExtensionDataNodeType.Text || extensionDataNodeType - ExtensionDataReader.ExtensionDataNodeType.ReferencedElement <= 1)
			{
				this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.EndElement;
				return;
			}
			Type dataType = dataNode.DataType;
			if (dataType == Globals.TypeOfClassDataNode)
			{
				this.MoveNextInClass((ClassDataNode)dataNode);
				return;
			}
			if (dataType == Globals.TypeOfCollectionDataNode)
			{
				this.MoveNextInCollection((CollectionDataNode)dataNode);
				return;
			}
			if (dataType == Globals.TypeOfISerializableDataNode)
			{
				this.MoveNextInISerializable((ISerializableDataNode)dataNode);
				return;
			}
			if (dataType == Globals.TypeOfXmlDataNode)
			{
				this.MoveNextInXml((XmlDataNode)dataNode);
				return;
			}
			if (dataNode.Value != null)
			{
				this.MoveToDeserializedObject(dataNode);
				return;
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("Invalid state in extension data reader.")));
		}

		private void SetNextElement(IDataNode node, string name, string ns, string prefix)
		{
			this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.Element;
			this.nextElement = this.GetNextElement();
			this.nextElement.localName = name;
			this.nextElement.ns = ns;
			this.nextElement.prefix = prefix;
			if (node == null)
			{
				this.nextElement.attributeCount = 0;
				this.nextElement.AddAttribute("i", "http://www.w3.org/2001/XMLSchema-instance", "nil", "true");
				this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.NullElement;
				return;
			}
			if (!this.CheckIfNodeHandled(node))
			{
				this.AddDeserializedDataNode(node);
				node.GetData(this.nextElement);
				if (node is XmlDataNode)
				{
					this.MoveNextInXml((XmlDataNode)node);
				}
			}
		}

		private void AddDeserializedDataNode(IDataNode node)
		{
			if (node.Id != Globals.NewObjectId && (node.Value == null || !node.IsFinalValue))
			{
				if (this.deserializedDataNodes == null)
				{
					this.deserializedDataNodes = new Queue<IDataNode>();
				}
				this.deserializedDataNodes.Enqueue(node);
			}
		}

		private bool CheckIfNodeHandled(IDataNode node)
		{
			bool flag = false;
			if (node.Id != Globals.NewObjectId)
			{
				flag = this.cache[node] != null;
				if (flag)
				{
					if (this.nextElement == null)
					{
						this.nextElement = this.GetNextElement();
					}
					this.nextElement.attributeCount = 0;
					this.nextElement.AddAttribute("z", "http://schemas.microsoft.com/2003/10/Serialization/", "Ref", node.Id.ToString(NumberFormatInfo.InvariantInfo));
					this.nextElement.AddAttribute("i", "http://www.w3.org/2001/XMLSchema-instance", "nil", "true");
					this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.ReferencedElement;
				}
				else
				{
					this.cache.Add(node, node);
				}
			}
			return flag;
		}

		private void MoveNextInClass(ClassDataNode dataNode)
		{
			if (dataNode.Members != null && this.element.childElementIndex < dataNode.Members.Count)
			{
				if (this.element.childElementIndex == 0)
				{
					this.context.IncrementItemCount(-dataNode.Members.Count);
				}
				IList<ExtensionDataMember> members = dataNode.Members;
				ElementData elementData = this.element;
				int childElementIndex = elementData.childElementIndex;
				elementData.childElementIndex = childElementIndex + 1;
				ExtensionDataMember extensionDataMember = members[childElementIndex];
				this.SetNextElement(extensionDataMember.Value, extensionDataMember.Name, extensionDataMember.Namespace, ExtensionDataReader.GetPrefix(extensionDataMember.Namespace));
				return;
			}
			this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.EndElement;
			this.element.childElementIndex = 0;
		}

		private void MoveNextInCollection(CollectionDataNode dataNode)
		{
			if (dataNode.Items != null && this.element.childElementIndex < dataNode.Items.Count)
			{
				if (this.element.childElementIndex == 0)
				{
					this.context.IncrementItemCount(-dataNode.Items.Count);
				}
				IList<IDataNode> items = dataNode.Items;
				ElementData elementData = this.element;
				int childElementIndex = elementData.childElementIndex;
				elementData.childElementIndex = childElementIndex + 1;
				IDataNode dataNode2 = items[childElementIndex];
				this.SetNextElement(dataNode2, dataNode.ItemName, dataNode.ItemNamespace, ExtensionDataReader.GetPrefix(dataNode.ItemNamespace));
				return;
			}
			this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.EndElement;
			this.element.childElementIndex = 0;
		}

		private void MoveNextInISerializable(ISerializableDataNode dataNode)
		{
			if (dataNode.Members != null && this.element.childElementIndex < dataNode.Members.Count)
			{
				if (this.element.childElementIndex == 0)
				{
					this.context.IncrementItemCount(-dataNode.Members.Count);
				}
				IList<ISerializableDataMember> members = dataNode.Members;
				ElementData elementData = this.element;
				int childElementIndex = elementData.childElementIndex;
				elementData.childElementIndex = childElementIndex + 1;
				ISerializableDataMember serializableDataMember = members[childElementIndex];
				this.SetNextElement(serializableDataMember.Value, serializableDataMember.Name, string.Empty, string.Empty);
				return;
			}
			this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.EndElement;
			this.element.childElementIndex = 0;
		}

		private void MoveNextInXml(XmlDataNode dataNode)
		{
			if (this.IsXmlDataNode)
			{
				this.xmlNodeReader.Read();
				if (this.xmlNodeReader.Depth == 0)
				{
					this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.EndElement;
					this.xmlNodeReader = null;
					return;
				}
			}
			else
			{
				this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.Xml;
				if (this.element == null)
				{
					this.element = this.nextElement;
				}
				else
				{
					this.PushElement();
				}
				XmlNode xmlNode = XmlObjectSerializerReadContext.CreateWrapperXmlElement(dataNode.OwnerDocument, dataNode.XmlAttributes, dataNode.XmlChildNodes, this.element.prefix, this.element.localName, this.element.ns);
				for (int i = 0; i < this.element.attributeCount; i++)
				{
					AttributeData attributeData = this.element.attributes[i];
					XmlAttribute xmlAttribute = dataNode.OwnerDocument.CreateAttribute(attributeData.prefix, attributeData.localName, attributeData.ns);
					xmlAttribute.Value = attributeData.value;
					xmlNode.Attributes.Append(xmlAttribute);
				}
				this.xmlNodeReader = new XmlNodeReader(xmlNode);
				this.xmlNodeReader.Read();
			}
		}

		private void MoveToDeserializedObject(IDataNode dataNode)
		{
			Type type = dataNode.DataType;
			bool flag = true;
			if (type == Globals.TypeOfObject)
			{
				type = dataNode.Value.GetType();
				if (type == Globals.TypeOfObject)
				{
					this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.EndElement;
					return;
				}
				flag = false;
			}
			if (this.MoveToText(type, dataNode, flag))
			{
				return;
			}
			if (dataNode.IsFinalValue)
			{
				this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.EndElement;
				return;
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid data node for '{0}' type.", new object[] { DataContract.GetClrTypeFullName(type) })));
		}

		private bool MoveToText(Type type, IDataNode dataNode, bool isTypedNode)
		{
			bool flag = true;
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<bool>)dataNode).GetValue() : ((bool)dataNode.Value));
				goto IL_03E7;
			case TypeCode.Char:
				this.value = XmlConvert.ToString((int)(isTypedNode ? ((DataNode<char>)dataNode).GetValue() : ((char)dataNode.Value)));
				goto IL_03E7;
			case TypeCode.SByte:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<sbyte>)dataNode).GetValue() : ((sbyte)dataNode.Value));
				goto IL_03E7;
			case TypeCode.Byte:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<byte>)dataNode).GetValue() : ((byte)dataNode.Value));
				goto IL_03E7;
			case TypeCode.Int16:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<short>)dataNode).GetValue() : ((short)dataNode.Value));
				goto IL_03E7;
			case TypeCode.UInt16:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<ushort>)dataNode).GetValue() : ((ushort)dataNode.Value));
				goto IL_03E7;
			case TypeCode.Int32:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<int>)dataNode).GetValue() : ((int)dataNode.Value));
				goto IL_03E7;
			case TypeCode.UInt32:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<uint>)dataNode).GetValue() : ((uint)dataNode.Value));
				goto IL_03E7;
			case TypeCode.Int64:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<long>)dataNode).GetValue() : ((long)dataNode.Value));
				goto IL_03E7;
			case TypeCode.UInt64:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<ulong>)dataNode).GetValue() : ((ulong)dataNode.Value));
				goto IL_03E7;
			case TypeCode.Single:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<float>)dataNode).GetValue() : ((float)dataNode.Value));
				goto IL_03E7;
			case TypeCode.Double:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<double>)dataNode).GetValue() : ((double)dataNode.Value));
				goto IL_03E7;
			case TypeCode.Decimal:
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<decimal>)dataNode).GetValue() : ((decimal)dataNode.Value));
				goto IL_03E7;
			case TypeCode.DateTime:
				this.value = (isTypedNode ? ((DataNode<DateTime>)dataNode).GetValue() : ((DateTime)dataNode.Value)).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK", DateTimeFormatInfo.InvariantInfo);
				goto IL_03E7;
			case TypeCode.String:
				this.value = (isTypedNode ? ((DataNode<string>)dataNode).GetValue() : ((string)dataNode.Value));
				goto IL_03E7;
			}
			if (type == Globals.TypeOfByteArray)
			{
				byte[] array = (isTypedNode ? ((DataNode<byte[]>)dataNode).GetValue() : ((byte[])dataNode.Value));
				this.value = ((array == null) ? string.Empty : Convert.ToBase64String(array));
			}
			else if (type == Globals.TypeOfTimeSpan)
			{
				this.value = XmlConvert.ToString(isTypedNode ? ((DataNode<TimeSpan>)dataNode).GetValue() : ((TimeSpan)dataNode.Value));
			}
			else if (type == Globals.TypeOfGuid)
			{
				this.value = (isTypedNode ? ((DataNode<Guid>)dataNode).GetValue() : ((Guid)dataNode.Value)).ToString();
			}
			else if (type == Globals.TypeOfUri)
			{
				Uri uri = (isTypedNode ? ((DataNode<Uri>)dataNode).GetValue() : ((Uri)dataNode.Value));
				this.value = uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
			}
			else
			{
				flag = false;
			}
			IL_03E7:
			if (flag)
			{
				this.internalNodeType = ExtensionDataReader.ExtensionDataNodeType.Text;
			}
			return flag;
		}

		private void PushElement()
		{
			this.GrowElementsIfNeeded();
			ElementData[] array = this.elements;
			int num = this.depth;
			this.depth = num + 1;
			array[num] = this.element;
			if (this.nextElement == null)
			{
				this.element = this.GetNextElement();
				return;
			}
			this.element = this.nextElement;
			this.nextElement = null;
		}

		private void PopElement()
		{
			this.prefix = this.element.prefix;
			this.localName = this.element.localName;
			this.ns = this.element.ns;
			if (this.depth == 0)
			{
				return;
			}
			this.depth--;
			if (this.elements != null)
			{
				this.element = this.elements[this.depth];
			}
		}

		private void GrowElementsIfNeeded()
		{
			if (this.elements == null)
			{
				this.elements = new ElementData[8];
				return;
			}
			if (this.elements.Length == this.depth)
			{
				ElementData[] array = new ElementData[this.elements.Length * 2];
				Array.Copy(this.elements, 0, array, 0, this.elements.Length);
				this.elements = array;
			}
		}

		private ElementData GetNextElement()
		{
			int num = this.depth + 1;
			if (this.elements != null && this.elements.Length > num && this.elements[num] != null)
			{
				return this.elements[num];
			}
			return new ElementData();
		}

		[SecuritySafeCritical]
		internal static string GetPrefix(string ns)
		{
			ns = ns ?? string.Empty;
			string text;
			if (!ExtensionDataReader.nsToPrefixTable.TryGetValue(ns, out text))
			{
				Dictionary<string, string> dictionary = ExtensionDataReader.nsToPrefixTable;
				lock (dictionary)
				{
					if (!ExtensionDataReader.nsToPrefixTable.TryGetValue(ns, out text))
					{
						text = ((ns == null || ns.Length == 0) ? string.Empty : ("p" + ExtensionDataReader.nsToPrefixTable.Count));
						ExtensionDataReader.AddPrefix(text, ns);
					}
				}
			}
			return text;
		}

		[SecuritySafeCritical]
		private static void AddPrefix(string prefix, string ns)
		{
			ExtensionDataReader.nsToPrefixTable.Add(ns, prefix);
			ExtensionDataReader.prefixToNsTable.Add(prefix, ns);
		}

		private Hashtable cache = new Hashtable();

		private ElementData[] elements;

		private ElementData element;

		private ElementData nextElement;

		private ReadState readState;

		private ExtensionDataReader.ExtensionDataNodeType internalNodeType;

		private XmlNodeType nodeType;

		private int depth;

		private string localName;

		private string ns;

		private string prefix;

		private string value;

		private int attributeCount;

		private int attributeIndex;

		private XmlNodeReader xmlNodeReader;

		private Queue<IDataNode> deserializedDataNodes;

		private XmlObjectSerializerReadContext context;

		[SecurityCritical]
		private static Dictionary<string, string> nsToPrefixTable = new Dictionary<string, string>();

		[SecurityCritical]
		private static Dictionary<string, string> prefixToNsTable = new Dictionary<string, string>();

		private enum ExtensionDataNodeType
		{
			None,
			Element,
			EndElement,
			Text,
			Xml,
			ReferencedElement,
			NullElement
		}
	}
}
