using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Xml.Serialization
{
	public abstract class XmlSerializationWriter : XmlSerializationGeneratedCode
	{
		protected XmlSerializationWriter()
		{
			this.qnameCount = 0;
			this.serializedObjects = new Hashtable();
		}

		internal void Initialize(XmlWriter writer, XmlSerializerNamespaces nss)
		{
			this.writer = writer;
			if (nss != null)
			{
				this.namespaces = new ArrayList();
				foreach (XmlQualifiedName xmlQualifiedName in nss.ToArray())
				{
					if (xmlQualifiedName.Name != string.Empty && xmlQualifiedName.Namespace != string.Empty)
					{
						this.namespaces.Add(xmlQualifiedName);
					}
				}
			}
		}

		protected ArrayList Namespaces
		{
			get
			{
				return this.namespaces;
			}
			set
			{
				this.namespaces = value;
			}
		}

		protected XmlWriter Writer
		{
			get
			{
				return this.writer;
			}
			set
			{
				this.writer = value;
			}
		}

		protected void AddWriteCallback(Type type, string typeName, string typeNs, XmlSerializationWriteCallback callback)
		{
			XmlSerializationWriter.WriteCallbackInfo writeCallbackInfo = new XmlSerializationWriter.WriteCallbackInfo();
			writeCallbackInfo.Type = type;
			writeCallbackInfo.TypeName = typeName;
			writeCallbackInfo.TypeNs = typeNs;
			writeCallbackInfo.Callback = callback;
			if (this.callbacks == null)
			{
				this.callbacks = new Hashtable();
			}
			this.callbacks.Add(type, writeCallbackInfo);
		}

		protected Exception CreateChoiceIdentifierValueException(string value, string identifier, string name, string ns)
		{
			string text = string.Format("Value '{0}' of the choice identifier '{1}' does not match element '{2}' from namespace '{3}'.", new object[] { value, identifier, name, ns });
			return new InvalidOperationException(text);
		}

		protected Exception CreateInvalidChoiceIdentifierValueException(string type, string identifier)
		{
			string text = string.Format("Invalid or missing choice identifier '{0}' of type '{1}'.", identifier, type);
			return new InvalidOperationException(text);
		}

		protected Exception CreateMismatchChoiceException(string value, string elementName, string enumValue)
		{
			string text = string.Format("Value of {0} mismatches the type of {1}, you need to set it to {2}.", elementName, value, enumValue);
			return new InvalidOperationException(text);
		}

		protected Exception CreateUnknownAnyElementException(string name, string ns)
		{
			string text = string.Format("The XML element named '{0}' from namespace '{1}' was not expected. The XML element name and namespace must match those provided via XmlAnyElementAttribute(s).", name, ns);
			return new InvalidOperationException(text);
		}

		protected Exception CreateUnknownTypeException(object o)
		{
			return this.CreateUnknownTypeException(o.GetType());
		}

		protected Exception CreateUnknownTypeException(Type type)
		{
			string text = string.Format("The type {0} may not be used in this context.", type);
			return new InvalidOperationException(text);
		}

		protected static byte[] FromByteArrayBase64(byte[] value)
		{
			return value;
		}

		protected static string FromByteArrayHex(byte[] value)
		{
			return XmlCustomFormatter.FromByteArrayHex(value);
		}

		protected static string FromChar(char value)
		{
			return XmlCustomFormatter.FromChar(value);
		}

		protected static string FromDate(DateTime value)
		{
			return XmlCustomFormatter.FromDate(value);
		}

		protected static string FromDateTime(DateTime value)
		{
			return XmlCustomFormatter.FromDateTime(value);
		}

		protected static string FromEnum(long value, string[] values, long[] ids)
		{
			return XmlCustomFormatter.FromEnum(value, values, ids);
		}

		protected static string FromTime(DateTime value)
		{
			return XmlCustomFormatter.FromTime(value);
		}

		protected static string FromXmlName(string name)
		{
			return XmlCustomFormatter.FromXmlName(name);
		}

		protected static string FromXmlNCName(string ncName)
		{
			return XmlCustomFormatter.FromXmlNCName(ncName);
		}

		protected static string FromXmlNmToken(string nmToken)
		{
			return XmlCustomFormatter.FromXmlNmToken(nmToken);
		}

		protected static string FromXmlNmTokens(string nmTokens)
		{
			return XmlCustomFormatter.FromXmlNmTokens(nmTokens);
		}

		protected string FromXmlQualifiedName(XmlQualifiedName xmlQualifiedName)
		{
			if (xmlQualifiedName == null || xmlQualifiedName == XmlQualifiedName.Empty)
			{
				return null;
			}
			return this.GetQualifiedName(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
		}

		private string GetId(object o, bool addToReferencesList)
		{
			if (this.idGenerator == null)
			{
				this.idGenerator = new ObjectIDGenerator();
			}
			bool flag;
			long id = this.idGenerator.GetId(o, out flag);
			return string.Format(CultureInfo.InvariantCulture, "id{0}", new object[] { id });
		}

		private bool AlreadyQueued(object ob)
		{
			if (this.idGenerator == null)
			{
				return false;
			}
			bool flag;
			this.idGenerator.HasId(ob, out flag);
			return !flag;
		}

		private string GetNamespacePrefix(string ns)
		{
			string text = this.Writer.LookupPrefix(ns);
			if (text == null)
			{
				text = string.Format(CultureInfo.InvariantCulture, "q{0}", new object[] { ++this.qnameCount });
				this.WriteAttribute("xmlns", text, null, ns);
			}
			return text;
		}

		private string GetQualifiedName(string name, string ns)
		{
			if (ns == string.Empty)
			{
				return name;
			}
			string namespacePrefix = this.GetNamespacePrefix(ns);
			if (namespacePrefix == string.Empty)
			{
				return name;
			}
			return string.Format("{0}:{1}", namespacePrefix, name);
		}

		protected abstract void InitCallbacks();

		protected void TopLevelElement()
		{
			this.topLevelElement = true;
		}

		protected void WriteAttribute(string localName, byte[] value)
		{
			this.WriteAttribute(localName, string.Empty, value);
		}

		protected void WriteAttribute(string localName, string value)
		{
			this.WriteAttribute(string.Empty, localName, string.Empty, value);
		}

		protected void WriteAttribute(string localName, string ns, byte[] value)
		{
			if (value == null)
			{
				return;
			}
			this.Writer.WriteStartAttribute(localName, ns);
			this.WriteValue(value);
			this.Writer.WriteEndAttribute();
		}

		protected void WriteAttribute(string localName, string ns, string value)
		{
			this.WriteAttribute(null, localName, ns, value);
		}

		protected void WriteAttribute(string prefix, string localName, string ns, string value)
		{
			if (value == null)
			{
				return;
			}
			this.Writer.WriteAttributeString(prefix, localName, ns, value);
		}

		private void WriteXmlNode(XmlNode node)
		{
			if (node is XmlDocument)
			{
				node = ((XmlDocument)node).DocumentElement;
			}
			node.WriteTo(this.Writer);
		}

		protected void WriteElementEncoded(XmlNode node, string name, string ns, bool isNullable, bool any)
		{
			if (name != string.Empty)
			{
				if (node == null)
				{
					if (isNullable)
					{
						this.WriteNullTagEncoded(name, ns);
					}
				}
				else
				{
					this.Writer.WriteStartElement(name, ns);
					this.WriteXmlNode(node);
					this.Writer.WriteEndElement();
				}
			}
			else
			{
				this.WriteXmlNode(node);
			}
		}

		protected void WriteElementLiteral(XmlNode node, string name, string ns, bool isNullable, bool any)
		{
			if (name != string.Empty)
			{
				if (node == null)
				{
					if (isNullable)
					{
						this.WriteNullTagLiteral(name, ns);
					}
				}
				else
				{
					this.Writer.WriteStartElement(name, ns);
					this.WriteXmlNode(node);
					this.Writer.WriteEndElement();
				}
			}
			else
			{
				this.WriteXmlNode(node);
			}
		}

		protected void WriteElementQualifiedName(string localName, XmlQualifiedName value)
		{
			this.WriteElementQualifiedName(localName, string.Empty, value, null);
		}

		protected void WriteElementQualifiedName(string localName, string ns, XmlQualifiedName value)
		{
			this.WriteElementQualifiedName(localName, ns, value, null);
		}

		protected void WriteElementQualifiedName(string localName, XmlQualifiedName value, XmlQualifiedName xsiType)
		{
			this.WriteElementQualifiedName(localName, string.Empty, value, xsiType);
		}

		protected void WriteElementQualifiedName(string localName, string ns, XmlQualifiedName value, XmlQualifiedName xsiType)
		{
			localName = XmlCustomFormatter.FromXmlNCName(localName);
			this.WriteStartElement(localName, ns);
			if (xsiType != null)
			{
				this.WriteXsiType(xsiType.Name, xsiType.Namespace);
			}
			this.Writer.WriteString(this.FromXmlQualifiedName(value));
			this.WriteEndElement();
		}

		protected void WriteElementString(string localName, string value)
		{
			this.WriteElementString(localName, string.Empty, value, null);
		}

		protected void WriteElementString(string localName, string ns, string value)
		{
			this.WriteElementString(localName, ns, value, null);
		}

		protected void WriteElementString(string localName, string value, XmlQualifiedName xsiType)
		{
			this.WriteElementString(localName, string.Empty, value, xsiType);
		}

		protected void WriteElementString(string localName, string ns, string value, XmlQualifiedName xsiType)
		{
			if (value == null)
			{
				return;
			}
			if (xsiType != null)
			{
				localName = XmlCustomFormatter.FromXmlNCName(localName);
				this.WriteStartElement(localName, ns);
				this.WriteXsiType(xsiType.Name, xsiType.Namespace);
				this.Writer.WriteString(value);
				this.WriteEndElement();
			}
			else
			{
				this.Writer.WriteElementString(localName, ns, value);
			}
		}

		protected void WriteElementStringRaw(string localName, byte[] value)
		{
			this.WriteElementStringRaw(localName, string.Empty, value, null);
		}

		protected void WriteElementStringRaw(string localName, string value)
		{
			this.WriteElementStringRaw(localName, string.Empty, value, null);
		}

		protected void WriteElementStringRaw(string localName, byte[] value, XmlQualifiedName xsiType)
		{
			this.WriteElementStringRaw(localName, string.Empty, value, xsiType);
		}

		protected void WriteElementStringRaw(string localName, string ns, byte[] value)
		{
			this.WriteElementStringRaw(localName, ns, value, null);
		}

		protected void WriteElementStringRaw(string localName, string ns, string value)
		{
			this.WriteElementStringRaw(localName, ns, value, null);
		}

		protected void WriteElementStringRaw(string localName, string value, XmlQualifiedName xsiType)
		{
			this.WriteElementStringRaw(localName, string.Empty, value, null);
		}

		protected void WriteElementStringRaw(string localName, string ns, byte[] value, XmlQualifiedName xsiType)
		{
			if (value == null)
			{
				return;
			}
			this.WriteStartElement(localName, ns);
			if (xsiType != null)
			{
				this.WriteXsiType(xsiType.Name, xsiType.Namespace);
			}
			if (value.Length > 0)
			{
				this.Writer.WriteBase64(value, 0, value.Length);
			}
			this.WriteEndElement();
		}

		protected void WriteElementStringRaw(string localName, string ns, string value, XmlQualifiedName xsiType)
		{
			localName = XmlCustomFormatter.FromXmlNCName(localName);
			this.WriteStartElement(localName, ns);
			if (xsiType != null)
			{
				this.WriteXsiType(xsiType.Name, xsiType.Namespace);
			}
			this.Writer.WriteRaw(value);
			this.WriteEndElement();
		}

		protected void WriteEmptyTag(string name)
		{
			this.WriteEmptyTag(name, string.Empty);
		}

		protected void WriteEmptyTag(string name, string ns)
		{
			name = XmlCustomFormatter.FromXmlName(name);
			this.WriteStartElement(name, ns);
			this.WriteEndElement();
		}

		protected void WriteEndElement()
		{
			this.WriteEndElement(null);
		}

		protected void WriteEndElement(object o)
		{
			if (o != null)
			{
				this.serializedObjects.Remove(o);
			}
			this.Writer.WriteEndElement();
		}

		protected void WriteId(object o)
		{
			this.WriteAttribute("id", this.GetId(o, true));
		}

		protected void WriteNamespaceDeclarations(XmlSerializerNamespaces ns)
		{
			if (ns == null)
			{
				return;
			}
			ICollection values = ns.Namespaces.Values;
			foreach (object obj in values)
			{
				XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)obj;
				if (xmlQualifiedName.Namespace != string.Empty && this.Writer.LookupPrefix(xmlQualifiedName.Namespace) != xmlQualifiedName.Name)
				{
					this.WriteAttribute("xmlns", xmlQualifiedName.Name, "http://www.w3.org/2000/xmlns/", xmlQualifiedName.Namespace);
				}
			}
		}

		protected void WriteNullableQualifiedNameEncoded(string name, string ns, XmlQualifiedName value, XmlQualifiedName xsiType)
		{
			if (value != null)
			{
				this.WriteElementQualifiedName(name, ns, value, xsiType);
			}
			else
			{
				this.WriteNullTagEncoded(name, ns);
			}
		}

		protected void WriteNullableQualifiedNameLiteral(string name, string ns, XmlQualifiedName value)
		{
			if (value != null)
			{
				this.WriteElementQualifiedName(name, ns, value);
			}
			else
			{
				this.WriteNullTagLiteral(name, ns);
			}
		}

		protected void WriteNullableStringEncoded(string name, string ns, string value, XmlQualifiedName xsiType)
		{
			if (value != null)
			{
				this.WriteElementString(name, ns, value, xsiType);
			}
			else
			{
				this.WriteNullTagEncoded(name, ns);
			}
		}

		protected void WriteNullableStringEncodedRaw(string name, string ns, byte[] value, XmlQualifiedName xsiType)
		{
			if (value == null)
			{
				this.WriteNullTagEncoded(name, ns);
			}
			else
			{
				this.WriteElementStringRaw(name, ns, value, xsiType);
			}
		}

		protected void WriteNullableStringEncodedRaw(string name, string ns, string value, XmlQualifiedName xsiType)
		{
			if (value == null)
			{
				this.WriteNullTagEncoded(name, ns);
			}
			else
			{
				this.WriteElementStringRaw(name, ns, value, xsiType);
			}
		}

		protected void WriteNullableStringLiteral(string name, string ns, string value)
		{
			if (value != null)
			{
				this.WriteElementString(name, ns, value, null);
			}
			else
			{
				this.WriteNullTagLiteral(name, ns);
			}
		}

		protected void WriteNullableStringLiteralRaw(string name, string ns, byte[] value)
		{
			if (value == null)
			{
				this.WriteNullTagLiteral(name, ns);
			}
			else
			{
				this.WriteElementStringRaw(name, ns, value);
			}
		}

		protected void WriteNullableStringLiteralRaw(string name, string ns, string value)
		{
			if (value == null)
			{
				this.WriteNullTagLiteral(name, ns);
			}
			else
			{
				this.WriteElementStringRaw(name, ns, value);
			}
		}

		protected void WriteNullTagEncoded(string name)
		{
			this.WriteNullTagEncoded(name, string.Empty);
		}

		protected void WriteNullTagEncoded(string name, string ns)
		{
			this.Writer.WriteStartElement(name, ns);
			this.Writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			this.Writer.WriteEndElement();
		}

		protected void WriteNullTagLiteral(string name)
		{
			this.WriteNullTagLiteral(name, string.Empty);
		}

		protected void WriteNullTagLiteral(string name, string ns)
		{
			this.WriteStartElement(name, ns);
			this.Writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			this.WriteEndElement();
		}

		protected void WritePotentiallyReferencingElement(string n, string ns, object o)
		{
			this.WritePotentiallyReferencingElement(n, ns, o, null, false, false);
		}

		protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType)
		{
			this.WritePotentiallyReferencingElement(n, ns, o, ambientType, false, false);
		}

		protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType, bool suppressReference)
		{
			this.WritePotentiallyReferencingElement(n, ns, o, ambientType, suppressReference, false);
		}

		protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType, bool suppressReference, bool isNullable)
		{
			if (o == null)
			{
				if (isNullable)
				{
					this.WriteNullTagEncoded(n, ns);
				}
				return;
			}
			this.WriteStartElement(n, ns, true);
			this.CheckReferenceQueue();
			if (this.callbacks != null && this.callbacks.ContainsKey(o.GetType()))
			{
				XmlSerializationWriter.WriteCallbackInfo writeCallbackInfo = (XmlSerializationWriter.WriteCallbackInfo)this.callbacks[o.GetType()];
				if (o.GetType().IsEnum)
				{
					writeCallbackInfo.Callback(o);
				}
				else if (suppressReference)
				{
					this.Writer.WriteAttributeString("id", this.GetId(o, false));
					if (ambientType != o.GetType())
					{
						this.WriteXsiType(writeCallbackInfo.TypeName, writeCallbackInfo.TypeNs);
					}
					writeCallbackInfo.Callback(o);
				}
				else
				{
					if (!this.AlreadyQueued(o))
					{
						this.referencedElements.Enqueue(o);
					}
					this.Writer.WriteAttributeString("href", "#" + this.GetId(o, true));
				}
			}
			else
			{
				TypeData typeData = TypeTranslator.GetTypeData(o.GetType());
				if (typeData.SchemaType == SchemaTypes.Primitive)
				{
					this.WriteXsiType(typeData.XmlType, "http://www.w3.org/2001/XMLSchema");
					this.Writer.WriteString(XmlCustomFormatter.ToXmlString(typeData, o));
				}
				else
				{
					if (!this.IsPrimitiveArray(typeData))
					{
						throw new InvalidOperationException("Invalid type: " + o.GetType().FullName);
					}
					if (!this.AlreadyQueued(o))
					{
						this.referencedElements.Enqueue(o);
					}
					this.Writer.WriteAttributeString("href", "#" + this.GetId(o, true));
				}
			}
			this.WriteEndElement();
		}

		protected void WriteReferencedElements()
		{
			if (this.referencedElements == null)
			{
				return;
			}
			if (this.callbacks == null)
			{
				return;
			}
			while (this.referencedElements.Count > 0)
			{
				object obj = this.referencedElements.Dequeue();
				TypeData typeData = TypeTranslator.GetTypeData(obj.GetType());
				XmlSerializationWriter.WriteCallbackInfo writeCallbackInfo = (XmlSerializationWriter.WriteCallbackInfo)this.callbacks[obj.GetType()];
				if (writeCallbackInfo != null)
				{
					this.WriteStartElement(writeCallbackInfo.TypeName, writeCallbackInfo.TypeNs, true);
					this.Writer.WriteAttributeString("id", this.GetId(obj, false));
					if (typeData.SchemaType != SchemaTypes.Array)
					{
						this.WriteXsiType(writeCallbackInfo.TypeName, writeCallbackInfo.TypeNs);
					}
					writeCallbackInfo.Callback(obj);
					this.WriteEndElement();
				}
				else if (this.IsPrimitiveArray(typeData))
				{
					this.WriteArray(obj, typeData);
				}
			}
		}

		private bool IsPrimitiveArray(TypeData td)
		{
			return td.SchemaType == SchemaTypes.Array && (td.ListItemTypeData.SchemaType == SchemaTypes.Primitive || td.ListItemType == typeof(object) || this.IsPrimitiveArray(td.ListItemTypeData));
		}

		private void WriteArray(object o, TypeData td)
		{
			TypeData typeData = td;
			int num = -1;
			string text;
			do
			{
				typeData = typeData.ListItemTypeData;
				text = typeData.XmlType;
				num++;
			}
			while (typeData.SchemaType == SchemaTypes.Array);
			while (num-- > 0)
			{
				text += "[]";
			}
			this.WriteStartElement("Array", "http://schemas.xmlsoap.org/soap/encoding/", true);
			this.Writer.WriteAttributeString("id", this.GetId(o, false));
			if (td.SchemaType == SchemaTypes.Array)
			{
				Array array = (Array)o;
				int length = array.Length;
				this.Writer.WriteAttributeString("arrayType", "http://schemas.xmlsoap.org/soap/encoding/", this.GetQualifiedName(text, "http://www.w3.org/2001/XMLSchema") + "[" + length.ToString() + "]");
				for (int i = 0; i < length; i++)
				{
					this.WritePotentiallyReferencingElement("Item", string.Empty, array.GetValue(i), td.ListItemType, false, true);
				}
			}
			this.WriteEndElement();
		}

		protected void WriteReferencingElement(string n, string ns, object o)
		{
			this.WriteReferencingElement(n, ns, o, false);
		}

		protected void WriteReferencingElement(string n, string ns, object o, bool isNullable)
		{
			if (o == null)
			{
				if (isNullable)
				{
					this.WriteNullTagEncoded(n, ns);
				}
				return;
			}
			this.CheckReferenceQueue();
			if (!this.AlreadyQueued(o))
			{
				this.referencedElements.Enqueue(o);
			}
			this.Writer.WriteStartElement(n, ns);
			this.Writer.WriteAttributeString("href", "#" + this.GetId(o, true));
			this.Writer.WriteEndElement();
		}

		private void CheckReferenceQueue()
		{
			if (this.referencedElements == null)
			{
				this.referencedElements = new Queue();
				this.InitCallbacks();
			}
		}

		[MonoTODO]
		protected void WriteRpcResult(string name, string ns)
		{
			throw new NotImplementedException();
		}

		protected void WriteSerializable(IXmlSerializable serializable, string name, string ns, bool isNullable)
		{
			this.WriteSerializable(serializable, name, ns, isNullable, true);
		}

		protected void WriteSerializable(IXmlSerializable serializable, string name, string ns, bool isNullable, bool wrapped)
		{
			if (serializable == null)
			{
				if (isNullable && wrapped)
				{
					this.WriteNullTagLiteral(name, ns);
				}
				return;
			}
			if (wrapped)
			{
				this.Writer.WriteStartElement(name, ns);
			}
			serializable.WriteXml(this.Writer);
			if (wrapped)
			{
				this.Writer.WriteEndElement();
			}
		}

		protected void WriteStartDocument()
		{
			if (this.Writer.WriteState == WriteState.Start)
			{
				this.Writer.WriteStartDocument();
			}
		}

		protected void WriteStartElement(string name)
		{
			this.WriteStartElement(name, string.Empty, null, false);
		}

		protected void WriteStartElement(string name, string ns)
		{
			this.WriteStartElement(name, ns, null, false);
		}

		protected void WriteStartElement(string name, string ns, bool writePrefixed)
		{
			this.WriteStartElement(name, ns, null, writePrefixed);
		}

		protected void WriteStartElement(string name, string ns, object o)
		{
			this.WriteStartElement(name, ns, o, false);
		}

		protected void WriteStartElement(string name, string ns, object o, bool writePrefixed)
		{
			this.WriteStartElement(name, ns, o, writePrefixed, this.namespaces);
		}

		protected void WriteStartElement(string name, string ns, object o, bool writePrefixed, XmlSerializerNamespaces xmlns)
		{
			if (xmlns == null)
			{
				throw new ArgumentNullException("xmlns");
			}
			this.WriteStartElement(name, ns, o, writePrefixed, xmlns.ToArray());
		}

		private void WriteStartElement(string name, string ns, object o, bool writePrefixed, ICollection namespaces)
		{
			if (o != null)
			{
				if (this.serializedObjects.Contains(o))
				{
					throw new InvalidOperationException("A circular reference was detected while serializing an object of type " + o.GetType().Name);
				}
				this.serializedObjects[o] = o;
			}
			string text = null;
			if (this.topLevelElement && ns != null && ns.Length != 0)
			{
				foreach (object obj in namespaces)
				{
					XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)obj;
					if (xmlQualifiedName.Namespace == ns)
					{
						text = xmlQualifiedName.Name;
						writePrefixed = true;
						break;
					}
				}
			}
			if (writePrefixed && ns != string.Empty)
			{
				name = XmlCustomFormatter.FromXmlName(name);
				if (text == null)
				{
					text = this.Writer.LookupPrefix(ns);
				}
				if (text == null || text.Length == 0)
				{
					text = "q" + ++this.qnameCount;
				}
				this.Writer.WriteStartElement(text, name, ns);
			}
			else
			{
				this.Writer.WriteStartElement(name, ns);
			}
			if (this.topLevelElement)
			{
				if (namespaces != null)
				{
					foreach (object obj2 in namespaces)
					{
						XmlQualifiedName xmlQualifiedName2 = (XmlQualifiedName)obj2;
						string text2 = this.Writer.LookupPrefix(xmlQualifiedName2.Namespace);
						if (text2 == null || text2.Length == 0)
						{
							this.WriteAttribute("xmlns", xmlQualifiedName2.Name, "http://www.w3.org/2000/xmlns/", xmlQualifiedName2.Namespace);
						}
					}
				}
				this.topLevelElement = false;
			}
		}

		protected void WriteTypedPrimitive(string name, string ns, object o, bool xsiType)
		{
			TypeData typeData = TypeTranslator.GetTypeData(o.GetType());
			if (typeData.SchemaType != SchemaTypes.Primitive)
			{
				throw new InvalidOperationException(string.Format("The type of the argument object '{0}' is not primitive.", typeData.FullTypeName));
			}
			if (name == null)
			{
				ns = ((!typeData.IsXsdType) ? "http://microsoft.com/wsdl/types/" : "http://www.w3.org/2001/XMLSchema");
				name = typeData.XmlType;
			}
			else
			{
				name = XmlCustomFormatter.FromXmlName(name);
			}
			this.Writer.WriteStartElement(name, ns);
			string text;
			if (o is XmlQualifiedName)
			{
				text = this.FromXmlQualifiedName((XmlQualifiedName)o);
			}
			else
			{
				text = XmlCustomFormatter.ToXmlString(typeData, o);
			}
			if (xsiType)
			{
				if (typeData.SchemaType != SchemaTypes.Primitive)
				{
					throw new InvalidOperationException(string.Format("The type {0} was not expected. Use the XmlInclude or SoapInclude attribute to specify types that are not known statically.", o.GetType().FullName));
				}
				this.WriteXsiType(typeData.XmlType, (!typeData.IsXsdType) ? "http://microsoft.com/wsdl/types/" : "http://www.w3.org/2001/XMLSchema");
			}
			this.WriteValue(text);
			this.Writer.WriteEndElement();
		}

		protected void WriteValue(byte[] value)
		{
			this.Writer.WriteBase64(value, 0, value.Length);
		}

		protected void WriteValue(string value)
		{
			if (value != null)
			{
				this.Writer.WriteString(value);
			}
		}

		protected void WriteXmlAttribute(XmlNode node)
		{
			this.WriteXmlAttribute(node, null);
		}

		protected void WriteXmlAttribute(XmlNode node, object container)
		{
			XmlAttribute xmlAttribute = node as XmlAttribute;
			if (xmlAttribute == null)
			{
				throw new InvalidOperationException("The node must be either type XmlAttribute or a derived type.");
			}
			if (xmlAttribute.NamespaceURI == "http://schemas.xmlsoap.org/wsdl/" && xmlAttribute.LocalName == "arrayType")
			{
				string text;
				string text2;
				string text3;
				TypeTranslator.ParseArrayType(xmlAttribute.Value, out text, out text2, out text3);
				string qualifiedName = this.GetQualifiedName(text + text3, text2);
				this.WriteAttribute(xmlAttribute.Prefix, xmlAttribute.LocalName, xmlAttribute.NamespaceURI, qualifiedName);
				return;
			}
			this.WriteAttribute(xmlAttribute.Prefix, xmlAttribute.LocalName, xmlAttribute.NamespaceURI, xmlAttribute.Value);
		}

		protected void WriteXsiType(string name, string ns)
		{
			if (ns != null && ns != string.Empty)
			{
				this.WriteAttribute("type", "http://www.w3.org/2001/XMLSchema-instance", this.GetQualifiedName(name, ns));
			}
			else
			{
				this.WriteAttribute("type", "http://www.w3.org/2001/XMLSchema-instance", name);
			}
		}

		protected Exception CreateInvalidAnyTypeException(object o)
		{
			if (o == null)
			{
				return new InvalidOperationException("null is invalid as anyType in XmlSerializer");
			}
			return this.CreateInvalidAnyTypeException(o.GetType());
		}

		protected Exception CreateInvalidAnyTypeException(Type t)
		{
			return new InvalidOperationException(string.Format("An object of type '{0}' is invalid as anyType in XmlSerializer", t));
		}

		protected Exception CreateInvalidEnumValueException(object value, string typeName)
		{
			return new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "'{0}' is not a valid value for {1}.", new object[] { value, typeName }));
		}

		protected static string FromEnum(long value, string[] values, long[] ids, string typeName)
		{
			return XmlCustomFormatter.FromEnum(value, values, ids, typeName);
		}

		[MonoTODO]
		protected string FromXmlQualifiedName(XmlQualifiedName xmlQualifiedName, bool ignoreEmpty)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected static Assembly ResolveDynamicAssembly(string assemblyFullName)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected bool EscapeName
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

		private const string xmlNamespace = "http://www.w3.org/2000/xmlns/";

		private const string unexpectedTypeError = "The type {0} was not expected. Use the XmlInclude or SoapInclude attribute to specify types that are not known statically.";

		private ObjectIDGenerator idGenerator;

		private int qnameCount;

		private bool topLevelElement;

		private ArrayList namespaces;

		private XmlWriter writer;

		private Queue referencedElements;

		private Hashtable callbacks;

		private Hashtable serializedObjects;

		private class WriteCallbackInfo
		{
			public Type Type;

			public string TypeName;

			public string TypeNs;

			public XmlSerializationWriteCallback Callback;
		}
	}
}
