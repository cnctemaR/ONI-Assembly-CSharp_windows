using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace System.Xml.Serialization
{
	[MonoTODO]
	public abstract class XmlSerializationReader : XmlSerializationGeneratedCode
	{
		internal void Initialize(XmlReader reader, XmlSerializer eventSource)
		{
			this.w3SchemaNS = reader.NameTable.Add("http://www.w3.org/2001/XMLSchema");
			this.w3InstanceNS = reader.NameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
			this.w3InstanceNS2000 = reader.NameTable.Add("http://www.w3.org/2000/10/XMLSchema-instance");
			this.w3InstanceNS1999 = reader.NameTable.Add("http://www.w3.org/1999/XMLSchema-instance");
			this.soapNS = reader.NameTable.Add("http://schemas.xmlsoap.org/soap/encoding/");
			this.wsdlNS = reader.NameTable.Add("http://schemas.xmlsoap.org/wsdl/");
			this.nullX = reader.NameTable.Add("null");
			this.nil = reader.NameTable.Add("nil");
			this.typeX = reader.NameTable.Add("type");
			this.arrayType = reader.NameTable.Add("arrayType");
			this.reader = reader;
			this.eventSource = eventSource;
			this.arrayQName = new XmlQualifiedName("Array", this.soapNS);
			this.InitIDs();
		}

		private ArrayList EnsureArrayList(ArrayList list)
		{
			if (list == null)
			{
				list = new ArrayList();
			}
			return list;
		}

		private Hashtable EnsureHashtable(Hashtable hash)
		{
			if (hash == null)
			{
				hash = new Hashtable();
			}
			return hash;
		}

		protected XmlDocument Document
		{
			get
			{
				if (this.document == null)
				{
					this.document = new XmlDocument(this.reader.NameTable);
				}
				return this.document;
			}
		}

		protected XmlReader Reader
		{
			get
			{
				return this.reader;
			}
		}

		[MonoTODO]
		protected bool IsReturnValue
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected int ReaderCount
		{
			get
			{
				return this.readCount;
			}
		}

		protected void AddFixup(XmlSerializationReader.CollectionFixup fixup)
		{
			this.collFixups = this.EnsureHashtable(this.collFixups);
			this.collFixups[fixup.Id] = fixup;
			if (this.delayedListFixups != null && this.delayedListFixups.ContainsKey(fixup.Id))
			{
				fixup.CollectionItems = this.delayedListFixups[fixup.Id];
				this.delayedListFixups.Remove(fixup.Id);
			}
		}

		protected void AddFixup(XmlSerializationReader.Fixup fixup)
		{
			this.fixups = this.EnsureArrayList(this.fixups);
			this.fixups.Add(fixup);
		}

		private void AddFixup(XmlSerializationReader.CollectionItemFixup fixup)
		{
			this.collItemFixups = this.EnsureArrayList(this.collItemFixups);
			this.collItemFixups.Add(fixup);
		}

		protected void AddReadCallback(string name, string ns, Type type, XmlSerializationReadCallback read)
		{
			XmlSerializationReader.WriteCallbackInfo writeCallbackInfo = new XmlSerializationReader.WriteCallbackInfo();
			writeCallbackInfo.Type = type;
			writeCallbackInfo.TypeName = name;
			writeCallbackInfo.TypeNs = ns;
			writeCallbackInfo.Callback = read;
			this.typesCallbacks = this.EnsureHashtable(this.typesCallbacks);
			this.typesCallbacks.Add(new XmlQualifiedName(name, ns), writeCallbackInfo);
		}

		protected void AddTarget(string id, object o)
		{
			if (id != null)
			{
				this.targets = this.EnsureHashtable(this.targets);
				if (this.targets[id] == null)
				{
					this.targets.Add(id, o);
				}
			}
			else
			{
				if (o != null)
				{
					return;
				}
				this.noIDTargets = this.EnsureArrayList(this.noIDTargets);
				this.noIDTargets.Add(o);
			}
		}

		private string CurrentTag()
		{
			XmlNodeType nodeType = this.reader.NodeType;
			switch (nodeType)
			{
			case XmlNodeType.Element:
				return string.Format("<{0} xmlns='{1}'>", this.reader.LocalName, this.reader.NamespaceURI);
			case XmlNodeType.Attribute:
				return this.reader.Value;
			case XmlNodeType.Text:
				return "CDATA";
			default:
				if (nodeType != XmlNodeType.EndElement)
				{
					return "(unknown)";
				}
				return ">";
			case XmlNodeType.Entity:
				return "<?";
			case XmlNodeType.ProcessingInstruction:
				return "<--";
			}
		}

		protected Exception CreateCtorHasSecurityException(string typeName)
		{
			string text = string.Format("The type '{0}' cannot be serialized because its parameterless constructor is decorated with declarative security permission attributes. Consider using imperative asserts or demands in the constructor.", typeName);
			return new InvalidOperationException(text);
		}

		protected Exception CreateInaccessibleConstructorException(string typeName)
		{
			string text = string.Format("{0} cannot be serialized because it does not have a default public constructor.", typeName);
			return new InvalidOperationException(text);
		}

		protected Exception CreateAbstractTypeException(string name, string ns)
		{
			string text = string.Concat(new string[]
			{
				"The specified type is abstrace: name='",
				name,
				"' namespace='",
				ns,
				"', at ",
				this.CurrentTag()
			});
			return new InvalidOperationException(text);
		}

		protected Exception CreateInvalidCastException(Type type, object value)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Cannot assign object of type {0} to an object of type {1}.", new object[]
			{
				value.GetType(),
				type
			});
			return new InvalidCastException(text);
		}

		protected Exception CreateReadOnlyCollectionException(string name)
		{
			string text = string.Format("Could not serialize {0}. Default constructors are required for collections and enumerators.", name);
			return new InvalidOperationException(text);
		}

		protected Exception CreateUnknownConstantException(string value, Type enumType)
		{
			string text = string.Format("'{0}' is not a valid value for {1}.", value, enumType);
			return new InvalidOperationException(text);
		}

		protected Exception CreateUnknownNodeException()
		{
			string text = this.CurrentTag() + " was not expected";
			return new InvalidOperationException(text);
		}

		protected Exception CreateUnknownTypeException(XmlQualifiedName type)
		{
			string text = string.Concat(new string[]
			{
				"The specified type was not recognized: name='",
				type.Name,
				"' namespace='",
				type.Namespace,
				"', at ",
				this.CurrentTag()
			});
			return new InvalidOperationException(text);
		}

		protected void CheckReaderCount(ref int whileIterations, ref int readerCount)
		{
			whileIterations = this.whileIterationCount;
			readerCount = this.readCount;
		}

		protected Array EnsureArrayIndex(Array a, int index, Type elementType)
		{
			if (a != null && index < a.Length)
			{
				return a;
			}
			int num;
			if (a == null)
			{
				num = 32;
			}
			else
			{
				num = a.Length * 2;
			}
			Array array = Array.CreateInstance(elementType, num);
			if (a != null)
			{
				Array.Copy(a, array, index);
			}
			return array;
		}

		[MonoTODO]
		protected void FixupArrayRefs(object fixup)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected int GetArrayLength(string name, string ns)
		{
			throw new NotImplementedException();
		}

		protected bool GetNullAttr()
		{
			string text = this.reader.GetAttribute(this.nullX, this.w3InstanceNS);
			if (text == null)
			{
				text = this.reader.GetAttribute(this.nil, this.w3InstanceNS);
				if (text == null)
				{
					text = this.reader.GetAttribute(this.nullX, this.w3InstanceNS2000);
					if (text == null)
					{
						text = this.reader.GetAttribute(this.nullX, this.w3InstanceNS1999);
					}
				}
			}
			return text != null;
		}

		protected object GetTarget(string id)
		{
			if (this.targets == null)
			{
				return null;
			}
			object obj = this.targets[id];
			if (obj != null)
			{
				if (this.referencedObjects == null)
				{
					this.referencedObjects = new Hashtable();
				}
				this.referencedObjects[obj] = obj;
			}
			return obj;
		}

		private bool TargetReady(string id)
		{
			return this.targets != null && this.targets.ContainsKey(id);
		}

		protected XmlQualifiedName GetXsiType()
		{
			string text = this.Reader.GetAttribute(this.typeX, "http://www.w3.org/2001/XMLSchema-instance");
			if (text == string.Empty || text == null)
			{
				text = this.Reader.GetAttribute(this.typeX, this.w3InstanceNS1999);
				if (text == string.Empty || text == null)
				{
					text = this.Reader.GetAttribute(this.typeX, this.w3InstanceNS2000);
					if (text == string.Empty || text == null)
					{
						return null;
					}
				}
			}
			int num = text.IndexOf(":");
			if (num == -1)
			{
				return new XmlQualifiedName(text, this.Reader.NamespaceURI);
			}
			string text2 = text.Substring(0, num);
			string text3 = text.Substring(num + 1);
			return new XmlQualifiedName(text3, this.Reader.LookupNamespace(text2));
		}

		protected abstract void InitCallbacks();

		protected abstract void InitIDs();

		protected bool IsXmlnsAttribute(string name)
		{
			int length = name.Length;
			if (length < 5)
			{
				return false;
			}
			if (length == 5)
			{
				return name == "xmlns";
			}
			return name.StartsWith("xmlns:");
		}

		protected void ParseWsdlArrayType(XmlAttribute attr)
		{
			if (attr.NamespaceURI == this.wsdlNS && attr.LocalName == this.arrayType)
			{
				string text = string.Empty;
				string text2;
				string text3;
				TypeTranslator.ParseArrayType(attr.Value, out text2, out text, out text3);
				if (text != string.Empty)
				{
					text = this.Reader.LookupNamespace(text) + ":";
				}
				attr.Value = text + text2 + text3;
			}
		}

		protected XmlQualifiedName ReadElementQualifiedName()
		{
			this.readCount++;
			if (this.reader.IsEmptyElement)
			{
				this.reader.Skip();
				return this.ToXmlQualifiedName(string.Empty);
			}
			this.reader.ReadStartElement();
			XmlQualifiedName xmlQualifiedName = this.ToXmlQualifiedName(this.reader.ReadString());
			this.reader.ReadEndElement();
			return xmlQualifiedName;
		}

		protected void ReadEndElement()
		{
			this.readCount++;
			while (this.reader.NodeType == XmlNodeType.Whitespace)
			{
				this.reader.Skip();
			}
			if (this.reader.NodeType != XmlNodeType.None)
			{
				this.reader.ReadEndElement();
			}
			else
			{
				this.reader.Skip();
			}
		}

		protected bool ReadNull()
		{
			if (!this.GetNullAttr())
			{
				return false;
			}
			this.readCount++;
			if (this.reader.IsEmptyElement)
			{
				this.reader.Skip();
				return true;
			}
			this.reader.ReadStartElement();
			while (this.reader.NodeType != XmlNodeType.EndElement)
			{
				this.UnknownNode(null);
			}
			this.ReadEndElement();
			return true;
		}

		protected XmlQualifiedName ReadNullableQualifiedName()
		{
			if (this.ReadNull())
			{
				return null;
			}
			return this.ReadElementQualifiedName();
		}

		protected string ReadNullableString()
		{
			if (this.ReadNull())
			{
				return null;
			}
			this.readCount++;
			return this.reader.ReadElementString();
		}

		protected bool ReadReference(out string fixupReference)
		{
			string attribute = this.reader.GetAttribute("href");
			if (attribute == null)
			{
				fixupReference = null;
				return false;
			}
			if (attribute[0] != '#')
			{
				throw new InvalidOperationException("href not found: " + attribute);
			}
			fixupReference = attribute.Substring(1);
			this.readCount++;
			if (!this.reader.IsEmptyElement)
			{
				this.reader.ReadStartElement();
				this.ReadEndElement();
			}
			else
			{
				this.reader.Skip();
			}
			return true;
		}

		protected object ReadReferencedElement()
		{
			return this.ReadReferencedElement(this.Reader.LocalName, this.Reader.NamespaceURI);
		}

		private XmlSerializationReader.WriteCallbackInfo GetCallbackInfo(XmlQualifiedName qname)
		{
			if (this.typesCallbacks == null)
			{
				this.typesCallbacks = new Hashtable();
				this.InitCallbacks();
			}
			return (XmlSerializationReader.WriteCallbackInfo)this.typesCallbacks[qname];
		}

		protected object ReadReferencedElement(string name, string ns)
		{
			XmlQualifiedName xmlQualifiedName = this.GetXsiType();
			if (xmlQualifiedName == null)
			{
				xmlQualifiedName = new XmlQualifiedName(name, ns);
			}
			string attribute = this.Reader.GetAttribute("id");
			string attribute2 = this.Reader.GetAttribute(this.arrayType, this.soapNS);
			object obj;
			if (xmlQualifiedName == this.arrayQName || (attribute2 != null && attribute2.Length > 0))
			{
				XmlSerializationReader.CollectionFixup collectionFixup = ((this.collFixups == null) ? null : ((XmlSerializationReader.CollectionFixup)this.collFixups[attribute]));
				if (this.ReadList(out obj))
				{
					if (collectionFixup != null)
					{
						collectionFixup.Callback(collectionFixup.Collection, obj);
						this.collFixups.Remove(attribute);
						obj = collectionFixup.Collection;
					}
				}
				else if (collectionFixup != null)
				{
					collectionFixup.CollectionItems = (object[])obj;
					obj = collectionFixup.Collection;
				}
			}
			else
			{
				XmlSerializationReader.WriteCallbackInfo callbackInfo = this.GetCallbackInfo(xmlQualifiedName);
				if (callbackInfo == null)
				{
					obj = this.ReadTypedPrimitive(xmlQualifiedName, attribute != null);
				}
				else
				{
					obj = callbackInfo.Callback();
				}
			}
			this.AddTarget(attribute, obj);
			return obj;
		}

		private bool ReadList(out object resultList)
		{
			string text = this.Reader.GetAttribute(this.arrayType, this.soapNS);
			if (text == null)
			{
				text = this.Reader.GetAttribute(this.arrayType, this.wsdlNS);
			}
			XmlQualifiedName xmlQualifiedName = this.ToXmlQualifiedName(text);
			int num = xmlQualifiedName.Name.LastIndexOf('[');
			string text2 = xmlQualifiedName.Name.Substring(num);
			string text3 = xmlQualifiedName.Name.Substring(0, num);
			int num2 = int.Parse(text2.Substring(1, text2.Length - 2), CultureInfo.InvariantCulture);
			num = text3.IndexOf('[');
			if (num == -1)
			{
				num = text3.Length;
			}
			string text4 = text3.Substring(0, num);
			string text5;
			if (xmlQualifiedName.Namespace == this.w3SchemaNS)
			{
				text5 = TypeTranslator.GetPrimitiveTypeData(text4).Type.FullName + text3.Substring(num);
			}
			else
			{
				XmlSerializationReader.WriteCallbackInfo callbackInfo = this.GetCallbackInfo(new XmlQualifiedName(text4, xmlQualifiedName.Namespace));
				text5 = callbackInfo.Type.FullName + text3.Substring(num) + ", " + callbackInfo.Type.Assembly.FullName;
			}
			Array array = Array.CreateInstance(Type.GetType(text5), num2);
			bool flag = true;
			if (this.Reader.IsEmptyElement)
			{
				this.readCount++;
				this.Reader.Skip();
			}
			else
			{
				this.Reader.ReadStartElement();
				for (int i = 0; i < num2; i++)
				{
					this.whileIterationCount++;
					this.readCount++;
					this.Reader.MoveToContent();
					string text6;
					object obj = this.ReadReferencingElement(text3, xmlQualifiedName.Namespace, out text6);
					if (text6 == null)
					{
						array.SetValue(obj, i);
					}
					else
					{
						this.AddFixup(new XmlSerializationReader.CollectionItemFixup(array, i, text6));
						flag = false;
					}
				}
				this.whileIterationCount = 0;
				this.Reader.ReadEndElement();
			}
			resultList = array;
			return flag;
		}

		protected void ReadReferencedElements()
		{
			this.reader.MoveToContent();
			XmlNodeType xmlNodeType = this.reader.NodeType;
			while (xmlNodeType != XmlNodeType.EndElement && xmlNodeType != XmlNodeType.None)
			{
				this.whileIterationCount++;
				this.readCount++;
				this.ReadReferencedElement();
				this.reader.MoveToContent();
				xmlNodeType = this.reader.NodeType;
			}
			this.whileIterationCount = 0;
			if (this.delayedListFixups != null)
			{
				foreach (object obj in this.delayedListFixups)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					this.AddTarget((string)dictionaryEntry.Key, dictionaryEntry.Value);
				}
			}
			if (this.collItemFixups != null)
			{
				foreach (object obj2 in this.collItemFixups)
				{
					XmlSerializationReader.CollectionItemFixup collectionItemFixup = (XmlSerializationReader.CollectionItemFixup)obj2;
					collectionItemFixup.Collection.SetValue(this.GetTarget(collectionItemFixup.Id), collectionItemFixup.Index);
				}
			}
			if (this.collFixups != null)
			{
				ICollection values = this.collFixups.Values;
				foreach (object obj3 in values)
				{
					XmlSerializationReader.CollectionFixup collectionFixup = (XmlSerializationReader.CollectionFixup)obj3;
					collectionFixup.Callback(collectionFixup.Collection, collectionFixup.CollectionItems);
				}
			}
			if (this.fixups != null)
			{
				foreach (object obj4 in this.fixups)
				{
					XmlSerializationReader.Fixup fixup = (XmlSerializationReader.Fixup)obj4;
					fixup.Callback(fixup);
				}
			}
			if (this.targets != null)
			{
				foreach (object obj5 in this.targets)
				{
					DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj5;
					if (dictionaryEntry2.Value != null && (this.referencedObjects == null || !this.referencedObjects.Contains(dictionaryEntry2.Value)))
					{
						this.UnreferencedObject((string)dictionaryEntry2.Key, dictionaryEntry2.Value);
					}
				}
			}
		}

		protected object ReadReferencingElement(out string fixupReference)
		{
			return this.ReadReferencingElement(this.Reader.LocalName, this.Reader.NamespaceURI, false, out fixupReference);
		}

		protected object ReadReferencingElement(string name, string ns, out string fixupReference)
		{
			return this.ReadReferencingElement(name, ns, false, out fixupReference);
		}

		protected object ReadReferencingElement(string name, string ns, bool elementCanBeType, out string fixupReference)
		{
			if (this.ReadNull())
			{
				fixupReference = null;
				return null;
			}
			string text = this.Reader.GetAttribute("href");
			if (text == string.Empty || text == null)
			{
				fixupReference = null;
				XmlQualifiedName xmlQualifiedName = this.GetXsiType();
				if (xmlQualifiedName == null)
				{
					xmlQualifiedName = new XmlQualifiedName(name, ns);
				}
				string attribute = this.Reader.GetAttribute(this.arrayType, this.soapNS);
				if (xmlQualifiedName == this.arrayQName || attribute != null)
				{
					this.delayedListFixups = this.EnsureHashtable(this.delayedListFixups);
					fixupReference = "__<" + this.delayedFixupId++ + ">";
					object obj;
					this.ReadList(out obj);
					this.delayedListFixups[fixupReference] = obj;
					return null;
				}
				XmlSerializationReader.WriteCallbackInfo callbackInfo = this.GetCallbackInfo(xmlQualifiedName);
				if (callbackInfo == null)
				{
					return this.ReadTypedPrimitive(xmlQualifiedName, true);
				}
				return callbackInfo.Callback();
			}
			else
			{
				if (text.StartsWith("#"))
				{
					text = text.Substring(1);
				}
				this.readCount++;
				this.Reader.Skip();
				if (this.TargetReady(text))
				{
					fixupReference = null;
					return this.GetTarget(text);
				}
				fixupReference = text;
				return null;
			}
		}

		protected IXmlSerializable ReadSerializable(IXmlSerializable serializable)
		{
			if (this.ReadNull())
			{
				return null;
			}
			int depth = this.reader.Depth;
			this.readCount++;
			serializable.ReadXml(this.reader);
			this.Reader.MoveToContent();
			while (this.reader.Depth > depth)
			{
				this.reader.Skip();
			}
			if (this.reader.Depth == depth && this.reader.NodeType == XmlNodeType.EndElement)
			{
				this.reader.ReadEndElement();
			}
			return serializable;
		}

		protected string ReadString(string value)
		{
			this.readCount++;
			if (value == null || value == string.Empty)
			{
				return this.reader.ReadString();
			}
			return value + this.reader.ReadString();
		}

		protected object ReadTypedPrimitive(XmlQualifiedName qname)
		{
			return this.ReadTypedPrimitive(qname, false);
		}

		private object ReadTypedPrimitive(XmlQualifiedName qname, bool reportUnknown)
		{
			if (qname == null)
			{
				qname = this.GetXsiType();
			}
			TypeData typeData = TypeTranslator.FindPrimitiveTypeData(qname.Name);
			if (typeData == null || typeData.SchemaType != SchemaTypes.Primitive)
			{
				this.readCount++;
				XmlNode xmlNode = this.Document.ReadNode(this.reader);
				if (reportUnknown)
				{
					this.OnUnknownNode(xmlNode, null, null);
				}
				if (xmlNode.ChildNodes.Count == 0 && xmlNode.Attributes.Count == 0)
				{
					return new object();
				}
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement == null)
				{
					return new XmlNode[] { xmlNode };
				}
				XmlNode[] array = new XmlNode[xmlElement.Attributes.Count + xmlElement.ChildNodes.Count];
				int num = 0;
				foreach (object obj in xmlElement.Attributes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					array[num++] = xmlNode2;
				}
				foreach (object obj2 in xmlElement.ChildNodes)
				{
					XmlNode xmlNode3 = (XmlNode)obj2;
					array[num++] = xmlNode3;
				}
				return array;
			}
			else
			{
				if (typeData.Type == typeof(XmlQualifiedName))
				{
					return this.ReadNullableQualifiedName();
				}
				this.readCount++;
				return XmlCustomFormatter.FromXmlString(typeData, this.Reader.ReadElementString());
			}
		}

		protected XmlNode ReadXmlNode(bool wrapped)
		{
			this.readCount++;
			XmlNode xmlNode = this.Document.ReadNode(this.reader);
			if (wrapped)
			{
				return xmlNode.FirstChild;
			}
			return xmlNode;
		}

		protected XmlDocument ReadXmlDocument(bool wrapped)
		{
			this.readCount++;
			if (wrapped)
			{
				this.reader.ReadStartElement();
			}
			this.reader.MoveToContent();
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNode = xmlDocument.ReadNode(this.reader);
			xmlDocument.AppendChild(xmlNode);
			if (wrapped)
			{
				this.reader.ReadEndElement();
			}
			return xmlDocument;
		}

		protected void Referenced(object o)
		{
			if (o != null)
			{
				if (this.referencedObjects == null)
				{
					this.referencedObjects = new Hashtable();
				}
				this.referencedObjects[o] = o;
			}
		}

		protected Array ShrinkArray(Array a, int length, Type elementType, bool isNullable)
		{
			if (length == 0 && isNullable)
			{
				return null;
			}
			if (a == null)
			{
				return Array.CreateInstance(elementType, length);
			}
			if (a.Length == length)
			{
				return a;
			}
			Array array = Array.CreateInstance(elementType, length);
			Array.Copy(a, array, length);
			return array;
		}

		protected byte[] ToByteArrayBase64(bool isNull)
		{
			this.readCount++;
			if (isNull)
			{
				this.Reader.ReadString();
				return null;
			}
			return XmlSerializationReader.ToByteArrayBase64(this.Reader.ReadString());
		}

		protected static byte[] ToByteArrayBase64(string value)
		{
			return Convert.FromBase64String(value);
		}

		protected byte[] ToByteArrayHex(bool isNull)
		{
			this.readCount++;
			if (isNull)
			{
				this.Reader.ReadString();
				return null;
			}
			return XmlSerializationReader.ToByteArrayHex(this.Reader.ReadString());
		}

		protected static byte[] ToByteArrayHex(string value)
		{
			return XmlConvert.FromBinHexString(value);
		}

		protected static char ToChar(string value)
		{
			return XmlCustomFormatter.ToChar(value);
		}

		protected static DateTime ToDate(string value)
		{
			return XmlCustomFormatter.ToDate(value);
		}

		protected static DateTime ToDateTime(string value)
		{
			return XmlCustomFormatter.ToDateTime(value);
		}

		protected static long ToEnum(string value, Hashtable h, string typeName)
		{
			return XmlCustomFormatter.ToEnum(value, h, typeName, true);
		}

		protected static DateTime ToTime(string value)
		{
			return XmlCustomFormatter.ToTime(value);
		}

		protected static string ToXmlName(string value)
		{
			return XmlCustomFormatter.ToXmlName(value);
		}

		protected static string ToXmlNCName(string value)
		{
			return XmlCustomFormatter.ToXmlNCName(value);
		}

		protected static string ToXmlNmToken(string value)
		{
			return XmlCustomFormatter.ToXmlNmToken(value);
		}

		protected static string ToXmlNmTokens(string value)
		{
			return XmlCustomFormatter.ToXmlNmTokens(value);
		}

		protected XmlQualifiedName ToXmlQualifiedName(string value)
		{
			int num = value.LastIndexOf(':');
			string text = XmlConvert.DecodeName(value);
			string text2;
			string text3;
			if (num < 0)
			{
				text2 = this.reader.NameTable.Add(text);
				text3 = this.reader.LookupNamespace(string.Empty);
			}
			else
			{
				string text4 = value.Substring(0, num);
				text3 = this.reader.LookupNamespace(text4);
				if (text3 == null)
				{
					throw new InvalidOperationException("namespace " + text4 + " not defined");
				}
				text2 = this.reader.NameTable.Add(value.Substring(num + 1));
			}
			return new XmlQualifiedName(text2, text3);
		}

		protected void UnknownAttribute(object o, XmlAttribute attr)
		{
			this.UnknownAttribute(o, attr, null);
		}

		protected void UnknownAttribute(object o, XmlAttribute attr, string qnames)
		{
			int num;
			int num2;
			if (this.Reader is XmlTextReader)
			{
				num = ((XmlTextReader)this.Reader).LineNumber;
				num2 = ((XmlTextReader)this.Reader).LinePosition;
			}
			else
			{
				num = 0;
				num2 = 0;
			}
			XmlAttributeEventArgs e = new XmlAttributeEventArgs(attr, num, num2, o);
			e.ExpectedAttributes = qnames;
			if (this.eventSource != null)
			{
				this.eventSource.OnUnknownAttribute(e);
			}
		}

		protected void UnknownElement(object o, XmlElement elem)
		{
			this.UnknownElement(o, elem, null);
		}

		protected void UnknownElement(object o, XmlElement elem, string qnames)
		{
			int num;
			int num2;
			if (this.Reader is XmlTextReader)
			{
				num = ((XmlTextReader)this.Reader).LineNumber;
				num2 = ((XmlTextReader)this.Reader).LinePosition;
			}
			else
			{
				num = 0;
				num2 = 0;
			}
			XmlElementEventArgs e = new XmlElementEventArgs(elem, num, num2, o);
			e.ExpectedElements = qnames;
			if (this.eventSource != null)
			{
				this.eventSource.OnUnknownElement(e);
			}
		}

		protected void UnknownNode(object o)
		{
			this.UnknownNode(o, null);
		}

		protected void UnknownNode(object o, string qnames)
		{
			this.OnUnknownNode(this.ReadXmlNode(false), o, qnames);
		}

		private void OnUnknownNode(XmlNode node, object o, string qnames)
		{
			int num;
			int num2;
			if (this.Reader is XmlTextReader)
			{
				num = ((XmlTextReader)this.Reader).LineNumber;
				num2 = ((XmlTextReader)this.Reader).LinePosition;
			}
			else
			{
				num = 0;
				num2 = 0;
			}
			if (node is XmlAttribute)
			{
				this.UnknownAttribute(o, (XmlAttribute)node, qnames);
				return;
			}
			if (node is XmlElement)
			{
				this.UnknownElement(o, (XmlElement)node, qnames);
				return;
			}
			if (this.eventSource != null)
			{
				this.eventSource.OnUnknownNode(new XmlNodeEventArgs(num, num2, node.LocalName, node.Name, node.NamespaceURI, node.NodeType, o, node.Value));
			}
			if (this.Reader.ReadState == ReadState.EndOfFile)
			{
				throw new InvalidOperationException("End of document found");
			}
		}

		protected void UnreferencedObject(string id, object o)
		{
			if (this.eventSource != null)
			{
				this.eventSource.OnUnreferencedObject(new UnreferencedObjectEventArgs(o, id));
			}
		}

		[MonoTODO]
		protected bool DecodeName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		protected string CollapseWhitespace(string value)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected Exception CreateBadDerivationException(string xsdDerived, string nsDerived, string xsdBase, string nsBase, string clrDerived, string clrBase)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected Exception CreateInvalidCastException(Type type, object value, string id)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected Exception CreateMissingIXmlSerializableType(string name, string ns, string clrType)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected string ReadString(string value, bool trim)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected object ReadTypedNull(XmlQualifiedName type)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected static Assembly ResolveDynamicAssembly(string assemblyFullName)
		{
			throw new NotImplementedException();
		}

		private XmlDocument document;

		private XmlReader reader;

		private ArrayList fixups;

		private Hashtable collFixups;

		private ArrayList collItemFixups;

		private Hashtable typesCallbacks;

		private ArrayList noIDTargets;

		private Hashtable targets;

		private Hashtable delayedListFixups;

		private XmlSerializer eventSource;

		private int delayedFixupId;

		private Hashtable referencedObjects;

		private int readCount;

		private int whileIterationCount;

		private string w3SchemaNS;

		private string w3InstanceNS;

		private string w3InstanceNS2000;

		private string w3InstanceNS1999;

		private string soapNS;

		private string wsdlNS;

		private string nullX;

		private string nil;

		private string typeX;

		private string arrayType;

		private XmlQualifiedName arrayQName;

		private class WriteCallbackInfo
		{
			public Type Type;

			public string TypeName;

			public string TypeNs;

			public XmlSerializationReadCallback Callback;
		}

		protected class CollectionFixup
		{
			public CollectionFixup(object collection, XmlSerializationCollectionFixupCallback callback, string id)
			{
				this.callback = callback;
				this.collection = collection;
				this.id = id;
			}

			public XmlSerializationCollectionFixupCallback Callback
			{
				get
				{
					return this.callback;
				}
			}

			public object Collection
			{
				get
				{
					return this.collection;
				}
			}

			public object Id
			{
				get
				{
					return this.id;
				}
			}

			internal object CollectionItems
			{
				get
				{
					return this.collectionItems;
				}
				set
				{
					this.collectionItems = value;
				}
			}

			private XmlSerializationCollectionFixupCallback callback;

			private object collection;

			private object collectionItems;

			private string id;
		}

		protected class Fixup
		{
			public Fixup(object o, XmlSerializationFixupCallback callback, int count)
			{
				this.source = o;
				this.callback = callback;
				this.ids = new string[count];
			}

			public Fixup(object o, XmlSerializationFixupCallback callback, string[] ids)
			{
				this.source = o;
				this.ids = ids;
				this.callback = callback;
			}

			public XmlSerializationFixupCallback Callback
			{
				get
				{
					return this.callback;
				}
			}

			public string[] Ids
			{
				get
				{
					return this.ids;
				}
			}

			public object Source
			{
				get
				{
					return this.source;
				}
				set
				{
					this.source = value;
				}
			}

			private object source;

			private string[] ids;

			private XmlSerializationFixupCallback callback;
		}

		protected class CollectionItemFixup
		{
			public CollectionItemFixup(Array list, int index, string id)
			{
				this.list = list;
				this.index = index;
				this.id = id;
			}

			public Array Collection
			{
				get
				{
					return this.list;
				}
			}

			public int Index
			{
				get
				{
					return this.index;
				}
			}

			public string Id
			{
				get
				{
					return this.id;
				}
			}

			private Array list;

			private int index;

			private string id;
		}
	}
}
