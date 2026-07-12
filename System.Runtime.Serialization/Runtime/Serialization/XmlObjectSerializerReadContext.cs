using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Diagnostics;
using System.Runtime.Serialization.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace System.Runtime.Serialization
{
	internal class XmlObjectSerializerReadContext : XmlObjectSerializerContext
	{
		private HybridObjectCache DeserializedObjects
		{
			get
			{
				if (this.deserializedObjects == null)
				{
					this.deserializedObjects = new HybridObjectCache();
				}
				return this.deserializedObjects;
			}
		}

		private XmlDocument Document
		{
			get
			{
				if (this.xmlDocument == null)
				{
					this.xmlDocument = new XmlDocument();
				}
				return this.xmlDocument;
			}
		}

		internal override bool IsGetOnlyCollection
		{
			get
			{
				return this.isGetOnlyCollection;
			}
			set
			{
				this.isGetOnlyCollection = value;
			}
		}

		internal object GetCollectionMember()
		{
			return this.getOnlyCollectionValue;
		}

		internal void StoreCollectionMemberInfo(object collectionMember)
		{
			this.getOnlyCollectionValue = collectionMember;
			this.isGetOnlyCollection = true;
		}

		internal static void ThrowNullValueReturnedForGetOnlyCollectionException(Type type)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("The get-only collection of type '{0}' returned a null value.  The input stream contains collection items which cannot be added if the instance is null.  Consider initializing the collection either in the constructor of the the object or in the getter.", new object[] { DataContract.GetClrTypeFullName(type) })));
		}

		internal static void ThrowArrayExceededSizeException(int arraySize, Type type)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Array length '{0}' provided by the get-only collection of type '{1}' is less than the number of array elements found in the input stream.  Consider increasing the length of the array.", new object[]
			{
				arraySize,
				DataContract.GetClrTypeFullName(type)
			})));
		}

		internal static XmlObjectSerializerReadContext CreateContext(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver dataContractResolver)
		{
			if (!serializer.PreserveObjectReferences && serializer.DataContractSurrogate == null)
			{
				return new XmlObjectSerializerReadContext(serializer, rootTypeDataContract, dataContractResolver);
			}
			return new XmlObjectSerializerReadContextComplex(serializer, rootTypeDataContract, dataContractResolver);
		}

		internal static XmlObjectSerializerReadContext CreateContext(NetDataContractSerializer serializer)
		{
			return new XmlObjectSerializerReadContextComplex(serializer);
		}

		internal XmlObjectSerializerReadContext(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject)
			: base(serializer, maxItemsInObjectGraph, streamingContext, ignoreExtensionDataObject)
		{
		}

		internal XmlObjectSerializerReadContext(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver dataContractResolver)
			: base(serializer, rootTypeDataContract, dataContractResolver)
		{
			this.attributes = new Attributes();
		}

		protected XmlObjectSerializerReadContext(NetDataContractSerializer serializer)
			: base(serializer)
		{
			this.attributes = new Attributes();
		}

		public virtual object InternalDeserialize(XmlReaderDelegator xmlReader, int id, RuntimeTypeHandle declaredTypeHandle, string name, string ns)
		{
			DataContract dataContract = this.GetDataContract(id, declaredTypeHandle);
			return this.InternalDeserialize(xmlReader, name, ns, Type.GetTypeFromHandle(declaredTypeHandle), ref dataContract);
		}

		internal virtual object InternalDeserialize(XmlReaderDelegator xmlReader, Type declaredType, string name, string ns)
		{
			DataContract dataContract = base.GetDataContract(declaredType);
			return this.InternalDeserialize(xmlReader, name, ns, declaredType, ref dataContract);
		}

		internal virtual object InternalDeserialize(XmlReaderDelegator xmlReader, Type declaredType, DataContract dataContract, string name, string ns)
		{
			if (dataContract == null)
			{
				base.GetDataContract(declaredType);
			}
			return this.InternalDeserialize(xmlReader, name, ns, declaredType, ref dataContract);
		}

		protected bool TryHandleNullOrRef(XmlReaderDelegator reader, Type declaredType, string name, string ns, ref object retObj)
		{
			this.ReadAttributes(reader);
			if (this.attributes.Ref != Globals.NewObjectId)
			{
				if (this.isGetOnlyCollection)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("On type '{1}', attribute '{0}' points to get-only collection, which is not supported.", new object[]
					{
						this.attributes.Ref,
						DataContract.GetClrTypeFullName(declaredType)
					})));
				}
				retObj = this.GetExistingObject(this.attributes.Ref, declaredType, name, ns);
				reader.Skip();
				return true;
			}
			else
			{
				if (this.attributes.XsiNil)
				{
					reader.Skip();
					return true;
				}
				return false;
			}
		}

		protected object InternalDeserialize(XmlReaderDelegator reader, string name, string ns, Type declaredType, ref DataContract dataContract)
		{
			object obj = null;
			if (this.TryHandleNullOrRef(reader, dataContract.UnderlyingType, name, ns, ref obj))
			{
				return obj;
			}
			bool flag = false;
			if (dataContract.KnownDataContracts != null)
			{
				this.scopedKnownTypes.Push(dataContract.KnownDataContracts);
				flag = true;
			}
			if (this.attributes.XsiTypeName != null)
			{
				dataContract = base.ResolveDataContractFromKnownTypes(this.attributes.XsiTypeName, this.attributes.XsiTypeNamespace, dataContract, declaredType);
				if (dataContract == null)
				{
					if (base.DataContractResolver == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(reader, global::System.Runtime.Serialization.SR.GetString("Element '{2}:{3}' contains data of the '{0}:{1}' data contract. The deserializer has no knowledge of any type that maps to this contract. Add the type corresponding to '{1}' to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding it to the list of known types passed to DataContractSerializer.", new object[]
						{
							this.attributes.XsiTypeNamespace,
							this.attributes.XsiTypeName,
							reader.NamespaceURI,
							reader.LocalName
						}))));
					}
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(reader, global::System.Runtime.Serialization.SR.GetString("Element '{2}:{3}' contains data from a type that maps to the name '{0}:{1}'. The deserializer has no knowledge of any type that maps to this name. Consider changing the implementation of the ResolveName method on your DataContractResolver to return a non-null value for name '{1}' and namespace '{0}'.", new object[]
					{
						this.attributes.XsiTypeNamespace,
						this.attributes.XsiTypeName,
						reader.NamespaceURI,
						reader.LocalName
					}))));
				}
				else
				{
					flag = this.ReplaceScopedKnownTypesTop(dataContract.KnownDataContracts, flag);
				}
			}
			if (dataContract.IsISerializable && this.attributes.FactoryTypeName != null)
			{
				DataContract dataContract2 = base.ResolveDataContractFromKnownTypes(this.attributes.FactoryTypeName, this.attributes.FactoryTypeNamespace, dataContract, declaredType);
				if (dataContract2 != null)
				{
					if (!dataContract2.IsISerializable)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("For data contract '{1}', factory type '{0}' is not ISerializable.", new object[]
						{
							DataContract.GetClrTypeFullName(dataContract2.UnderlyingType),
							DataContract.GetClrTypeFullName(dataContract.UnderlyingType)
						})));
					}
					dataContract = dataContract2;
					flag = this.ReplaceScopedKnownTypesTop(dataContract.KnownDataContracts, flag);
				}
				else if (DiagnosticUtility.ShouldTraceWarning)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>(2);
					dictionary["FactoryType"] = this.attributes.FactoryTypeNamespace + ":" + this.attributes.FactoryTypeName;
					dictionary["ISerializableType"] = dataContract.StableName.Namespace + ":" + dataContract.StableName.Name;
					TraceUtility.Trace(TraceEventType.Warning, 196625, global::System.Runtime.Serialization.SR.GetString("Factory type not found"), new DictionaryTraceRecord(dictionary));
				}
			}
			if (flag)
			{
				object obj2 = this.ReadDataContractValue(dataContract, reader);
				this.scopedKnownTypes.Pop();
				return obj2;
			}
			return this.ReadDataContractValue(dataContract, reader);
		}

		private bool ReplaceScopedKnownTypesTop(Dictionary<XmlQualifiedName, DataContract> knownDataContracts, bool knownTypesAddedInCurrentScope)
		{
			if (knownTypesAddedInCurrentScope)
			{
				this.scopedKnownTypes.Pop();
				knownTypesAddedInCurrentScope = false;
			}
			if (knownDataContracts != null)
			{
				this.scopedKnownTypes.Push(knownDataContracts);
				knownTypesAddedInCurrentScope = true;
			}
			return knownTypesAddedInCurrentScope;
		}

		public static bool MoveToNextElement(XmlReaderDelegator xmlReader)
		{
			return xmlReader.MoveToContent() != XmlNodeType.EndElement;
		}

		public int GetMemberIndex(XmlReaderDelegator xmlReader, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces, int memberIndex, ExtensionDataObject extensionData)
		{
			for (int i = memberIndex + 1; i < memberNames.Length; i++)
			{
				if (xmlReader.IsStartElement(memberNames[i], memberNamespaces[i]))
				{
					return i;
				}
			}
			this.HandleMemberNotFound(xmlReader, extensionData, memberIndex);
			return memberNames.Length;
		}

		public int GetMemberIndexWithRequiredMembers(XmlReaderDelegator xmlReader, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces, int memberIndex, int requiredIndex, ExtensionDataObject extensionData)
		{
			for (int i = memberIndex + 1; i < memberNames.Length; i++)
			{
				if (xmlReader.IsStartElement(memberNames[i], memberNamespaces[i]))
				{
					if (requiredIndex < i)
					{
						XmlObjectSerializerReadContext.ThrowRequiredMemberMissingException(xmlReader, memberIndex, requiredIndex, memberNames);
					}
					return i;
				}
			}
			this.HandleMemberNotFound(xmlReader, extensionData, memberIndex);
			return memberNames.Length;
		}

		public static void ThrowRequiredMemberMissingException(XmlReaderDelegator xmlReader, int memberIndex, int requiredIndex, XmlDictionaryString[] memberNames)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (requiredIndex == memberNames.Length)
			{
				requiredIndex--;
			}
			for (int i = memberIndex + 1; i <= requiredIndex; i++)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(" | ");
				}
				stringBuilder.Append(memberNames[i].Value);
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(xmlReader, global::System.Runtime.Serialization.SR.GetString("'{0}' '{1}' from namespace '{2}' is not expected. Expecting element '{3}'.", new object[]
			{
				xmlReader.NodeType,
				xmlReader.LocalName,
				xmlReader.NamespaceURI,
				stringBuilder.ToString()
			}))));
		}

		protected void HandleMemberNotFound(XmlReaderDelegator xmlReader, ExtensionDataObject extensionData, int memberIndex)
		{
			xmlReader.MoveToContent();
			if (xmlReader.NodeType != XmlNodeType.Element)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
			}
			if (base.IgnoreExtensionDataObject || extensionData == null)
			{
				this.SkipUnknownElement(xmlReader);
				return;
			}
			this.HandleUnknownElement(xmlReader, extensionData, memberIndex);
		}

		internal void HandleUnknownElement(XmlReaderDelegator xmlReader, ExtensionDataObject extensionData, int memberIndex)
		{
			if (extensionData.Members == null)
			{
				extensionData.Members = new List<ExtensionDataMember>();
			}
			extensionData.Members.Add(this.ReadExtensionDataMember(xmlReader, memberIndex));
		}

		public void SkipUnknownElement(XmlReaderDelegator xmlReader)
		{
			this.ReadAttributes(xmlReader);
			if (DiagnosticUtility.ShouldTraceVerbose)
			{
				TraceUtility.Trace(TraceEventType.Verbose, 196615, global::System.Runtime.Serialization.SR.GetString("Element ignored"), new StringTraceRecord("Element", xmlReader.NamespaceURI + ":" + xmlReader.LocalName));
			}
			xmlReader.Skip();
		}

		public string ReadIfNullOrRef(XmlReaderDelegator xmlReader, Type memberType, bool isMemberTypeSerializable)
		{
			if (this.attributes.Ref != Globals.NewObjectId)
			{
				this.CheckIfTypeSerializable(memberType, isMemberTypeSerializable);
				xmlReader.Skip();
				return this.attributes.Ref;
			}
			if (this.attributes.XsiNil)
			{
				this.CheckIfTypeSerializable(memberType, isMemberTypeSerializable);
				xmlReader.Skip();
				return null;
			}
			return Globals.NewObjectId;
		}

		internal virtual void ReadAttributes(XmlReaderDelegator xmlReader)
		{
			if (this.attributes == null)
			{
				this.attributes = new Attributes();
			}
			this.attributes.Read(xmlReader);
		}

		public void ResetAttributes()
		{
			if (this.attributes != null)
			{
				this.attributes.Reset();
			}
		}

		public string GetObjectId()
		{
			return this.attributes.Id;
		}

		internal virtual int GetArraySize()
		{
			return -1;
		}

		public void AddNewObject(object obj)
		{
			this.AddNewObjectWithId(this.attributes.Id, obj);
		}

		public void AddNewObjectWithId(string id, object obj)
		{
			if (id != Globals.NewObjectId)
			{
				this.DeserializedObjects.Add(id, obj);
			}
			if (this.extensionDataReader != null)
			{
				this.extensionDataReader.UnderlyingExtensionDataReader.SetDeserializedValue(obj);
			}
		}

		public void ReplaceDeserializedObject(string id, object oldObj, object newObj)
		{
			if (oldObj == newObj)
			{
				return;
			}
			if (id != Globals.NewObjectId)
			{
				if (this.DeserializedObjects.IsObjectReferenced(id))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Factory object contains a reference to self. Old object is '{0}', new object is '{1}'.", new object[]
					{
						DataContract.GetClrTypeFullName(oldObj.GetType()),
						DataContract.GetClrTypeFullName(newObj.GetType()),
						id
					})));
				}
				this.DeserializedObjects.Remove(id);
				this.DeserializedObjects.Add(id, newObj);
			}
			if (this.extensionDataReader != null)
			{
				this.extensionDataReader.UnderlyingExtensionDataReader.SetDeserializedValue(newObj);
			}
		}

		public object GetExistingObject(string id, Type type, string name, string ns)
		{
			object obj = this.DeserializedObjects.GetObject(id);
			if (obj == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Deserialized object with reference id '{0}' not found in stream.", new object[] { id })));
			}
			if (obj is IDataNode)
			{
				IDataNode dataNode = (IDataNode)obj;
				obj = ((dataNode.Value != null && dataNode.IsFinalValue) ? dataNode.Value : this.DeserializeFromExtensionData(dataNode, type, name, ns));
			}
			return obj;
		}

		private object GetExistingObjectOrExtensionData(string id)
		{
			object @object = this.DeserializedObjects.GetObject(id);
			if (@object == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Deserialized object with reference id '{0}' not found in stream.", new object[] { id })));
			}
			return @object;
		}

		public object GetRealObject(IObjectReference obj, string id)
		{
			object realObject = SurrogateDataContract.GetRealObject(obj, base.GetStreamingContext());
			if (realObject == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("On the surrogate data contract for '{0}', GetRealObject method returned null.", new object[] { DataContract.GetClrTypeFullName(obj.GetType()) })));
			}
			this.ReplaceDeserializedObject(id, obj, realObject);
			return realObject;
		}

		private object DeserializeFromExtensionData(IDataNode dataNode, Type type, string name, string ns)
		{
			ExtensionDataReader extensionDataReader;
			if (this.extensionDataReader == null)
			{
				extensionDataReader = new ExtensionDataReader(this);
				this.extensionDataReader = this.CreateReaderDelegatorForReader(extensionDataReader);
			}
			else
			{
				extensionDataReader = this.extensionDataReader.UnderlyingExtensionDataReader;
			}
			extensionDataReader.SetDataNode(dataNode, name, ns);
			object obj = this.InternalDeserialize(this.extensionDataReader, type, name, ns);
			dataNode.Clear();
			extensionDataReader.Reset();
			return obj;
		}

		public static void Read(XmlReaderDelegator xmlReader)
		{
			if (!xmlReader.Read())
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Unexpected end of file.")));
			}
		}

		internal static void ParseQualifiedName(string qname, XmlReaderDelegator xmlReader, out string name, out string ns, out string prefix)
		{
			int num = qname.IndexOf(':');
			prefix = "";
			if (num >= 0)
			{
				prefix = qname.Substring(0, num);
			}
			name = qname.Substring(num + 1);
			ns = xmlReader.LookupNamespace(prefix);
		}

		public static T[] EnsureArraySize<T>(T[] array, int index)
		{
			if (array.Length <= index)
			{
				if (index == 2147483647)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("The maximum array length ({0}) has been exceeded while reading XML data for array of type '{1}'.", new object[]
					{
						int.MaxValue,
						DataContract.GetClrTypeFullName(typeof(T))
					})));
				}
				T[] array2 = new T[(index < 1073741823) ? (index * 2) : int.MaxValue];
				Array.Copy(array, 0, array2, 0, array.Length);
				array = array2;
			}
			return array;
		}

		public static T[] TrimArraySize<T>(T[] array, int size)
		{
			if (size != array.Length)
			{
				T[] array2 = new T[size];
				Array.Copy(array, 0, array2, 0, size);
				array = array2;
			}
			return array;
		}

		public void CheckEndOfArray(XmlReaderDelegator xmlReader, int arraySize, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
		{
			if (xmlReader.NodeType == XmlNodeType.EndElement)
			{
				return;
			}
			while (xmlReader.IsStartElement())
			{
				if (xmlReader.IsStartElement(itemName, itemNamespace))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Array length '{0}' provided by Size attribute is not equal to the number of array elements '{1}' from namespace '{2}' found.", new object[] { arraySize, itemName.Value, itemNamespace.Value })));
				}
				this.SkipUnknownElement(xmlReader);
			}
			if (xmlReader.NodeType != XmlNodeType.EndElement)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.EndElement, xmlReader));
			}
		}

		internal object ReadIXmlSerializable(XmlReaderDelegator xmlReader, XmlDataContract xmlDataContract, bool isMemberType)
		{
			if (this.xmlSerializableReader == null)
			{
				this.xmlSerializableReader = new XmlSerializableReader();
			}
			return XmlObjectSerializerReadContext.ReadIXmlSerializable(this.xmlSerializableReader, xmlReader, xmlDataContract, isMemberType);
		}

		internal static object ReadRootIXmlSerializable(XmlReaderDelegator xmlReader, XmlDataContract xmlDataContract, bool isMemberType)
		{
			return XmlObjectSerializerReadContext.ReadIXmlSerializable(new XmlSerializableReader(), xmlReader, xmlDataContract, isMemberType);
		}

		internal static object ReadIXmlSerializable(XmlSerializableReader xmlSerializableReader, XmlReaderDelegator xmlReader, XmlDataContract xmlDataContract, bool isMemberType)
		{
			xmlSerializableReader.BeginRead(xmlReader);
			if (isMemberType && !xmlDataContract.HasRoot)
			{
				xmlReader.Read();
				xmlReader.MoveToContent();
			}
			object obj;
			if (xmlDataContract.UnderlyingType == Globals.TypeOfXmlElement)
			{
				if (!xmlReader.IsStartElement())
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
				}
				obj = (XmlElement)new XmlDocument().ReadNode(xmlSerializableReader);
			}
			else if (xmlDataContract.UnderlyingType == Globals.TypeOfXmlNodeArray)
			{
				obj = XmlSerializableServices.ReadNodes(xmlSerializableReader);
			}
			else
			{
				IXmlSerializable xmlSerializable = xmlDataContract.CreateXmlSerializableDelegate();
				xmlSerializable.ReadXml(xmlSerializableReader);
				obj = xmlSerializable;
			}
			xmlSerializableReader.EndRead();
			return obj;
		}

		public SerializationInfo ReadSerializationInfo(XmlReaderDelegator xmlReader, Type type)
		{
			SerializationInfo serializationInfo = new SerializationInfo(type, XmlObjectSerializer.FormatterConverter);
			XmlNodeType xmlNodeType;
			while ((xmlNodeType = xmlReader.MoveToContent()) != XmlNodeType.EndElement)
			{
				if (xmlNodeType != XmlNodeType.Element)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
				}
				if (xmlReader.NamespaceURI.Length != 0)
				{
					this.SkipUnknownElement(xmlReader);
				}
				else
				{
					string text = XmlConvert.DecodeName(xmlReader.LocalName);
					base.IncrementItemCount(1);
					this.ReadAttributes(xmlReader);
					object obj;
					if (this.attributes.Ref != Globals.NewObjectId)
					{
						xmlReader.Skip();
						obj = this.GetExistingObject(this.attributes.Ref, null, text, string.Empty);
					}
					else if (this.attributes.XsiNil)
					{
						xmlReader.Skip();
						obj = null;
					}
					else
					{
						obj = this.InternalDeserialize(xmlReader, Globals.TypeOfObject, text, string.Empty);
					}
					serializationInfo.AddValue(text, obj);
				}
			}
			return serializationInfo;
		}

		protected virtual DataContract ResolveDataContractFromTypeName()
		{
			if (this.attributes.XsiTypeName != null)
			{
				return base.ResolveDataContractFromKnownTypes(this.attributes.XsiTypeName, this.attributes.XsiTypeNamespace, null, null);
			}
			return null;
		}

		private ExtensionDataMember ReadExtensionDataMember(XmlReaderDelegator xmlReader, int memberIndex)
		{
			ExtensionDataMember extensionDataMember = new ExtensionDataMember();
			extensionDataMember.Name = xmlReader.LocalName;
			extensionDataMember.Namespace = xmlReader.NamespaceURI;
			extensionDataMember.MemberIndex = memberIndex;
			if (xmlReader.UnderlyingExtensionDataReader != null)
			{
				extensionDataMember.Value = xmlReader.UnderlyingExtensionDataReader.GetCurrentNode();
			}
			else
			{
				extensionDataMember.Value = this.ReadExtensionDataValue(xmlReader);
			}
			return extensionDataMember;
		}

		public IDataNode ReadExtensionDataValue(XmlReaderDelegator xmlReader)
		{
			this.ReadAttributes(xmlReader);
			base.IncrementItemCount(1);
			IDataNode dataNode = null;
			if (this.attributes.Ref != Globals.NewObjectId)
			{
				xmlReader.Skip();
				object existingObjectOrExtensionData = this.GetExistingObjectOrExtensionData(this.attributes.Ref);
				IDataNode dataNode3;
				if (!(existingObjectOrExtensionData is IDataNode))
				{
					IDataNode dataNode2 = new DataNode<object>(existingObjectOrExtensionData);
					dataNode3 = dataNode2;
				}
				else
				{
					dataNode3 = (IDataNode)existingObjectOrExtensionData;
				}
				dataNode = dataNode3;
				dataNode.Id = this.attributes.Ref;
			}
			else if (this.attributes.XsiNil)
			{
				xmlReader.Skip();
				dataNode = null;
			}
			else
			{
				string text = null;
				string text2 = null;
				if (this.attributes.XsiTypeName != null)
				{
					text = this.attributes.XsiTypeName;
					text2 = this.attributes.XsiTypeNamespace;
				}
				if (this.IsReadingCollectionExtensionData(xmlReader))
				{
					XmlObjectSerializerReadContext.Read(xmlReader);
					dataNode = this.ReadUnknownCollectionData(xmlReader, text, text2);
				}
				else if (this.attributes.FactoryTypeName != null)
				{
					XmlObjectSerializerReadContext.Read(xmlReader);
					dataNode = this.ReadUnknownISerializableData(xmlReader, text, text2);
				}
				else if (this.IsReadingClassExtensionData(xmlReader))
				{
					XmlObjectSerializerReadContext.Read(xmlReader);
					dataNode = this.ReadUnknownClassData(xmlReader, text, text2);
				}
				else
				{
					DataContract dataContract = this.ResolveDataContractFromTypeName();
					if (dataContract == null)
					{
						dataNode = this.ReadExtensionDataValue(xmlReader, text, text2);
					}
					else if (dataContract is XmlDataContract)
					{
						dataNode = this.ReadUnknownXmlData(xmlReader, text, text2);
					}
					else if (dataContract.IsISerializable)
					{
						XmlObjectSerializerReadContext.Read(xmlReader);
						dataNode = this.ReadUnknownISerializableData(xmlReader, text, text2);
					}
					else if (dataContract is PrimitiveDataContract)
					{
						if (this.attributes.Id == Globals.NewObjectId)
						{
							XmlObjectSerializerReadContext.Read(xmlReader);
							xmlReader.MoveToContent();
							dataNode = this.ReadUnknownPrimitiveData(xmlReader, dataContract.UnderlyingType, text, text2);
							xmlReader.ReadEndElement();
						}
						else
						{
							dataNode = new DataNode<object>(xmlReader.ReadElementContentAsAnyType(dataContract.UnderlyingType));
							this.InitializeExtensionDataNode(dataNode, text, text2);
						}
					}
					else if (dataContract is EnumDataContract)
					{
						dataNode = new DataNode<object>(((EnumDataContract)dataContract).ReadEnumValue(xmlReader));
						this.InitializeExtensionDataNode(dataNode, text, text2);
					}
					else if (dataContract is ClassDataContract)
					{
						XmlObjectSerializerReadContext.Read(xmlReader);
						dataNode = this.ReadUnknownClassData(xmlReader, text, text2);
					}
					else if (dataContract is CollectionDataContract)
					{
						XmlObjectSerializerReadContext.Read(xmlReader);
						dataNode = this.ReadUnknownCollectionData(xmlReader, text, text2);
					}
				}
			}
			return dataNode;
		}

		protected virtual void StartReadExtensionDataValue(XmlReaderDelegator xmlReader)
		{
		}

		private IDataNode ReadExtensionDataValue(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
		{
			this.StartReadExtensionDataValue(xmlReader);
			if (this.attributes.UnrecognizedAttributesFound)
			{
				return this.ReadUnknownXmlData(xmlReader, dataContractName, dataContractNamespace);
			}
			IDictionary<string, string> namespacesInScope = xmlReader.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml);
			XmlObjectSerializerReadContext.Read(xmlReader);
			xmlReader.MoveToContent();
			XmlNodeType nodeType = xmlReader.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType == XmlNodeType.Text)
				{
					return this.ReadPrimitiveExtensionDataValue(xmlReader, dataContractName, dataContractNamespace);
				}
				if (nodeType != XmlNodeType.EndElement)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
				}
				IDataNode dataNode = this.ReadUnknownPrimitiveData(xmlReader, Globals.TypeOfObject, dataContractName, dataContractNamespace);
				xmlReader.ReadEndElement();
				dataNode.IsFinalValue = false;
				return dataNode;
			}
			else
			{
				if (xmlReader.NamespaceURI.StartsWith("http://schemas.datacontract.org/2004/07/", StringComparison.Ordinal))
				{
					return this.ReadUnknownClassData(xmlReader, dataContractName, dataContractNamespace);
				}
				return this.ReadAndResolveUnknownXmlData(xmlReader, namespacesInScope, dataContractName, dataContractNamespace);
			}
		}

		protected virtual IDataNode ReadPrimitiveExtensionDataValue(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
		{
			Type valueType = xmlReader.ValueType;
			if (valueType == Globals.TypeOfString)
			{
				IDataNode dataNode = new DataNode<object>(xmlReader.ReadContentAsString());
				this.InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);
				dataNode.IsFinalValue = false;
				xmlReader.ReadEndElement();
				return dataNode;
			}
			IDataNode dataNode2 = this.ReadUnknownPrimitiveData(xmlReader, valueType, dataContractName, dataContractNamespace);
			xmlReader.ReadEndElement();
			return dataNode2;
		}

		protected void InitializeExtensionDataNode(IDataNode dataNode, string dataContractName, string dataContractNamespace)
		{
			dataNode.DataContractName = dataContractName;
			dataNode.DataContractNamespace = dataContractNamespace;
			dataNode.ClrAssemblyName = this.attributes.ClrAssembly;
			dataNode.ClrTypeName = this.attributes.ClrType;
			this.AddNewObject(dataNode);
			dataNode.Id = this.attributes.Id;
		}

		private IDataNode ReadUnknownPrimitiveData(XmlReaderDelegator xmlReader, Type type, string dataContractName, string dataContractNamespace)
		{
			IDataNode dataNode = xmlReader.ReadExtensionData(type);
			this.InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);
			return dataNode;
		}

		private ClassDataNode ReadUnknownClassData(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
		{
			ClassDataNode classDataNode = new ClassDataNode();
			this.InitializeExtensionDataNode(classDataNode, dataContractName, dataContractNamespace);
			int num = 0;
			XmlNodeType xmlNodeType;
			while ((xmlNodeType = xmlReader.MoveToContent()) != XmlNodeType.EndElement)
			{
				if (xmlNodeType != XmlNodeType.Element)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
				}
				if (classDataNode.Members == null)
				{
					classDataNode.Members = new List<ExtensionDataMember>();
				}
				classDataNode.Members.Add(this.ReadExtensionDataMember(xmlReader, num++));
			}
			xmlReader.ReadEndElement();
			return classDataNode;
		}

		private CollectionDataNode ReadUnknownCollectionData(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
		{
			CollectionDataNode collectionDataNode = new CollectionDataNode();
			this.InitializeExtensionDataNode(collectionDataNode, dataContractName, dataContractNamespace);
			int arraySZSize = this.attributes.ArraySZSize;
			XmlNodeType xmlNodeType;
			while ((xmlNodeType = xmlReader.MoveToContent()) != XmlNodeType.EndElement)
			{
				if (xmlNodeType != XmlNodeType.Element)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
				}
				if (collectionDataNode.ItemName == null)
				{
					collectionDataNode.ItemName = xmlReader.LocalName;
					collectionDataNode.ItemNamespace = xmlReader.NamespaceURI;
				}
				if (xmlReader.IsStartElement(collectionDataNode.ItemName, collectionDataNode.ItemNamespace))
				{
					if (collectionDataNode.Items == null)
					{
						collectionDataNode.Items = new List<IDataNode>();
					}
					collectionDataNode.Items.Add(this.ReadExtensionDataValue(xmlReader));
				}
				else
				{
					this.SkipUnknownElement(xmlReader);
				}
			}
			xmlReader.ReadEndElement();
			if (arraySZSize != -1)
			{
				collectionDataNode.Size = arraySZSize;
				if (collectionDataNode.Items == null)
				{
					if (collectionDataNode.Size > 0)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Array size attribute is incorrect; must be between {0} and {1}.", new object[] { arraySZSize, 0 })));
					}
				}
				else if (collectionDataNode.Size != collectionDataNode.Items.Count)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Array size attribute is incorrect; must be between {0} and {1}.", new object[]
					{
						arraySZSize,
						collectionDataNode.Items.Count
					})));
				}
			}
			else if (collectionDataNode.Items != null)
			{
				collectionDataNode.Size = collectionDataNode.Items.Count;
			}
			else
			{
				collectionDataNode.Size = 0;
			}
			return collectionDataNode;
		}

		private ISerializableDataNode ReadUnknownISerializableData(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
		{
			ISerializableDataNode serializableDataNode = new ISerializableDataNode();
			this.InitializeExtensionDataNode(serializableDataNode, dataContractName, dataContractNamespace);
			serializableDataNode.FactoryTypeName = this.attributes.FactoryTypeName;
			serializableDataNode.FactoryTypeNamespace = this.attributes.FactoryTypeNamespace;
			XmlNodeType xmlNodeType;
			while ((xmlNodeType = xmlReader.MoveToContent()) != XmlNodeType.EndElement)
			{
				if (xmlNodeType != XmlNodeType.Element)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
				}
				if (xmlReader.NamespaceURI.Length != 0)
				{
					this.SkipUnknownElement(xmlReader);
				}
				else
				{
					ISerializableDataMember serializableDataMember = new ISerializableDataMember();
					serializableDataMember.Name = xmlReader.LocalName;
					serializableDataMember.Value = this.ReadExtensionDataValue(xmlReader);
					if (serializableDataNode.Members == null)
					{
						serializableDataNode.Members = new List<ISerializableDataMember>();
					}
					serializableDataNode.Members.Add(serializableDataMember);
				}
			}
			xmlReader.ReadEndElement();
			return serializableDataNode;
		}

		private IDataNode ReadUnknownXmlData(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
		{
			XmlDataNode xmlDataNode = new XmlDataNode();
			this.InitializeExtensionDataNode(xmlDataNode, dataContractName, dataContractNamespace);
			xmlDataNode.OwnerDocument = this.Document;
			if (xmlReader.NodeType == XmlNodeType.EndElement)
			{
				return xmlDataNode;
			}
			IList<XmlAttribute> list = null;
			IList<XmlNode> list2 = null;
			if (xmlReader.MoveToContent() != XmlNodeType.Text)
			{
				while (xmlReader.MoveToNextAttribute())
				{
					string namespaceURI = xmlReader.NamespaceURI;
					if (namespaceURI != "http://schemas.microsoft.com/2003/10/Serialization/" && namespaceURI != "http://www.w3.org/2001/XMLSchema-instance")
					{
						if (list == null)
						{
							list = new List<XmlAttribute>();
						}
						list.Add((XmlAttribute)this.Document.ReadNode(xmlReader.UnderlyingReader));
					}
				}
				XmlObjectSerializerReadContext.Read(xmlReader);
			}
			while (xmlReader.MoveToContent() != XmlNodeType.EndElement)
			{
				if (xmlReader.EOF)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Unexpected end of file.")));
				}
				if (list2 == null)
				{
					list2 = new List<XmlNode>();
				}
				list2.Add(this.Document.ReadNode(xmlReader.UnderlyingReader));
			}
			xmlReader.ReadEndElement();
			xmlDataNode.XmlAttributes = list;
			xmlDataNode.XmlChildNodes = list2;
			return xmlDataNode;
		}

		private IDataNode ReadAndResolveUnknownXmlData(XmlReaderDelegator xmlReader, IDictionary<string, string> namespaces, string dataContractName, string dataContractNamespace)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			string text = null;
			string text2 = null;
			IList<XmlNode> list = new List<XmlNode>();
			IList<XmlAttribute> list2 = null;
			if (namespaces == null)
			{
				goto IL_0194;
			}
			list2 = new List<XmlAttribute>();
			using (IEnumerator<KeyValuePair<string, string>> enumerator = namespaces.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> keyValuePair = enumerator.Current;
					list2.Add(this.AddNamespaceDeclaration(keyValuePair.Key, keyValuePair.Value));
				}
				goto IL_0194;
			}
			IL_006A:
			XmlNodeType nodeType;
			if (nodeType == XmlNodeType.Element)
			{
				string namespaceURI = xmlReader.NamespaceURI;
				string localName = xmlReader.LocalName;
				if (flag)
				{
					flag = namespaceURI.Length == 0;
				}
				if (flag2)
				{
					if (text2 == null)
					{
						text2 = localName;
						text = namespaceURI;
					}
					else
					{
						flag2 = string.CompareOrdinal(text2, localName) == 0 && string.CompareOrdinal(text, namespaceURI) == 0;
					}
				}
			}
			else
			{
				if (xmlReader.EOF)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Unexpected end of file.")));
				}
				if (this.IsContentNode(xmlReader.NodeType))
				{
					flag = (flag3 = (flag2 = false));
				}
			}
			if (this.attributesInXmlData == null)
			{
				this.attributesInXmlData = new Attributes();
			}
			this.attributesInXmlData.Read(xmlReader);
			XmlNode xmlNode = this.Document.ReadNode(xmlReader.UnderlyingReader);
			list.Add(xmlNode);
			if (namespaces == null)
			{
				if (this.attributesInXmlData.XsiTypeName != null)
				{
					xmlNode.Attributes.Append(this.AddNamespaceDeclaration(this.attributesInXmlData.XsiTypePrefix, this.attributesInXmlData.XsiTypeNamespace));
				}
				if (this.attributesInXmlData.FactoryTypeName != null)
				{
					xmlNode.Attributes.Append(this.AddNamespaceDeclaration(this.attributesInXmlData.FactoryTypePrefix, this.attributesInXmlData.FactoryTypeNamespace));
				}
			}
			IL_0194:
			if ((nodeType = xmlReader.NodeType) != XmlNodeType.EndElement)
			{
				goto IL_006A;
			}
			xmlReader.ReadEndElement();
			if (text2 != null && flag2)
			{
				return this.ReadUnknownCollectionData(this.CreateReaderOverChildNodes(list2, list), dataContractName, dataContractNamespace);
			}
			if (flag)
			{
				return this.ReadUnknownISerializableData(this.CreateReaderOverChildNodes(list2, list), dataContractName, dataContractNamespace);
			}
			if (flag3)
			{
				return this.ReadUnknownClassData(this.CreateReaderOverChildNodes(list2, list), dataContractName, dataContractNamespace);
			}
			XmlDataNode xmlDataNode = new XmlDataNode();
			this.InitializeExtensionDataNode(xmlDataNode, dataContractName, dataContractNamespace);
			xmlDataNode.OwnerDocument = this.Document;
			xmlDataNode.XmlChildNodes = list;
			xmlDataNode.XmlAttributes = list2;
			return xmlDataNode;
		}

		private bool IsContentNode(XmlNodeType nodeType)
		{
			switch (nodeType)
			{
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
			case XmlNodeType.DocumentType:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				return false;
			}
			return true;
		}

		internal XmlReaderDelegator CreateReaderOverChildNodes(IList<XmlAttribute> xmlAttributes, IList<XmlNode> xmlChildNodes)
		{
			XmlNode xmlNode = XmlObjectSerializerReadContext.CreateWrapperXmlElement(this.Document, xmlAttributes, xmlChildNodes, null, null, null);
			XmlReaderDelegator xmlReaderDelegator = this.CreateReaderDelegatorForReader(new XmlNodeReader(xmlNode));
			xmlReaderDelegator.MoveToContent();
			XmlObjectSerializerReadContext.Read(xmlReaderDelegator);
			return xmlReaderDelegator;
		}

		internal static XmlNode CreateWrapperXmlElement(XmlDocument document, IList<XmlAttribute> xmlAttributes, IList<XmlNode> xmlChildNodes, string prefix, string localName, string ns)
		{
			localName = localName ?? "wrapper";
			ns = ns ?? string.Empty;
			XmlNode xmlNode = document.CreateElement(prefix, localName, ns);
			if (xmlAttributes != null)
			{
				for (int i = 0; i < xmlAttributes.Count; i++)
				{
					xmlNode.Attributes.Append(xmlAttributes[i]);
				}
			}
			if (xmlChildNodes != null)
			{
				for (int j = 0; j < xmlChildNodes.Count; j++)
				{
					xmlNode.AppendChild(xmlChildNodes[j]);
				}
			}
			return xmlNode;
		}

		private XmlAttribute AddNamespaceDeclaration(string prefix, string ns)
		{
			XmlAttribute xmlAttribute = ((prefix == null || prefix.Length == 0) ? this.Document.CreateAttribute(null, "xmlns", "http://www.w3.org/2000/xmlns/") : this.Document.CreateAttribute("xmlns", prefix, "http://www.w3.org/2000/xmlns/"));
			xmlAttribute.Value = ns;
			return xmlAttribute;
		}

		public static Exception CreateUnexpectedStateException(XmlNodeType expectedState, XmlReaderDelegator xmlReader)
		{
			return XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(global::System.Runtime.Serialization.SR.GetString("Expecting state '{0}'.", new object[] { expectedState }), xmlReader);
		}

		protected virtual object ReadDataContractValue(DataContract dataContract, XmlReaderDelegator reader)
		{
			return dataContract.ReadXmlValue(reader, this);
		}

		protected virtual XmlReaderDelegator CreateReaderDelegatorForReader(XmlReader xmlReader)
		{
			return new XmlReaderDelegator(xmlReader);
		}

		protected virtual bool IsReadingCollectionExtensionData(XmlReaderDelegator xmlReader)
		{
			return this.attributes.ArraySZSize != -1;
		}

		protected virtual bool IsReadingClassExtensionData(XmlReaderDelegator xmlReader)
		{
			return false;
		}

		internal Attributes attributes;

		private HybridObjectCache deserializedObjects;

		private XmlSerializableReader xmlSerializableReader;

		private XmlDocument xmlDocument;

		private Attributes attributesInXmlData;

		private XmlReaderDelegator extensionDataReader;

		private object getOnlyCollectionValue;

		private bool isGetOnlyCollection;
	}
}
