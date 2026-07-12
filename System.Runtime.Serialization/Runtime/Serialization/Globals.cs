using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Runtime.Serialization
{
	internal static class Globals
	{
		internal static XmlQualifiedName IdQualifiedName
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.idQualifiedName == null)
				{
					Globals.idQualifiedName = new XmlQualifiedName("Id", "http://schemas.microsoft.com/2003/10/Serialization/");
				}
				return Globals.idQualifiedName;
			}
		}

		internal static XmlQualifiedName RefQualifiedName
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.refQualifiedName == null)
				{
					Globals.refQualifiedName = new XmlQualifiedName("Ref", "http://schemas.microsoft.com/2003/10/Serialization/");
				}
				return Globals.refQualifiedName;
			}
		}

		internal static Type TypeOfObject
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfObject == null)
				{
					Globals.typeOfObject = typeof(object);
				}
				return Globals.typeOfObject;
			}
		}

		internal static Type TypeOfValueType
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfValueType == null)
				{
					Globals.typeOfValueType = typeof(ValueType);
				}
				return Globals.typeOfValueType;
			}
		}

		internal static Type TypeOfArray
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfArray == null)
				{
					Globals.typeOfArray = typeof(Array);
				}
				return Globals.typeOfArray;
			}
		}

		internal static Type TypeOfString
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfString == null)
				{
					Globals.typeOfString = typeof(string);
				}
				return Globals.typeOfString;
			}
		}

		internal static Type TypeOfInt
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfInt == null)
				{
					Globals.typeOfInt = typeof(int);
				}
				return Globals.typeOfInt;
			}
		}

		internal static Type TypeOfULong
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfULong == null)
				{
					Globals.typeOfULong = typeof(ulong);
				}
				return Globals.typeOfULong;
			}
		}

		internal static Type TypeOfVoid
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfVoid == null)
				{
					Globals.typeOfVoid = typeof(void);
				}
				return Globals.typeOfVoid;
			}
		}

		internal static Type TypeOfByteArray
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfByteArray == null)
				{
					Globals.typeOfByteArray = typeof(byte[]);
				}
				return Globals.typeOfByteArray;
			}
		}

		internal static Type TypeOfTimeSpan
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfTimeSpan == null)
				{
					Globals.typeOfTimeSpan = typeof(TimeSpan);
				}
				return Globals.typeOfTimeSpan;
			}
		}

		internal static Type TypeOfGuid
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfGuid == null)
				{
					Globals.typeOfGuid = typeof(Guid);
				}
				return Globals.typeOfGuid;
			}
		}

		internal static Type TypeOfDateTimeOffset
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfDateTimeOffset == null)
				{
					Globals.typeOfDateTimeOffset = typeof(DateTimeOffset);
				}
				return Globals.typeOfDateTimeOffset;
			}
		}

		internal static Type TypeOfDateTimeOffsetAdapter
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfDateTimeOffsetAdapter == null)
				{
					Globals.typeOfDateTimeOffsetAdapter = typeof(DateTimeOffsetAdapter);
				}
				return Globals.typeOfDateTimeOffsetAdapter;
			}
		}

		internal static Type TypeOfUri
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfUri == null)
				{
					Globals.typeOfUri = typeof(Uri);
				}
				return Globals.typeOfUri;
			}
		}

		internal static Type TypeOfTypeEnumerable
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfTypeEnumerable == null)
				{
					Globals.typeOfTypeEnumerable = typeof(IEnumerable<Type>);
				}
				return Globals.typeOfTypeEnumerable;
			}
		}

		internal static Type TypeOfStreamingContext
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfStreamingContext == null)
				{
					Globals.typeOfStreamingContext = typeof(StreamingContext);
				}
				return Globals.typeOfStreamingContext;
			}
		}

		internal static Type TypeOfISerializable
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfISerializable == null)
				{
					Globals.typeOfISerializable = typeof(ISerializable);
				}
				return Globals.typeOfISerializable;
			}
		}

		internal static Type TypeOfIDeserializationCallback
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIDeserializationCallback == null)
				{
					Globals.typeOfIDeserializationCallback = typeof(IDeserializationCallback);
				}
				return Globals.typeOfIDeserializationCallback;
			}
		}

		internal static Type TypeOfIObjectReference
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIObjectReference == null)
				{
					Globals.typeOfIObjectReference = typeof(IObjectReference);
				}
				return Globals.typeOfIObjectReference;
			}
		}

		internal static Type TypeOfXmlFormatClassWriterDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlFormatClassWriterDelegate == null)
				{
					Globals.typeOfXmlFormatClassWriterDelegate = typeof(XmlFormatClassWriterDelegate);
				}
				return Globals.typeOfXmlFormatClassWriterDelegate;
			}
		}

		internal static Type TypeOfXmlFormatCollectionWriterDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlFormatCollectionWriterDelegate == null)
				{
					Globals.typeOfXmlFormatCollectionWriterDelegate = typeof(XmlFormatCollectionWriterDelegate);
				}
				return Globals.typeOfXmlFormatCollectionWriterDelegate;
			}
		}

		internal static Type TypeOfXmlFormatClassReaderDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlFormatClassReaderDelegate == null)
				{
					Globals.typeOfXmlFormatClassReaderDelegate = typeof(XmlFormatClassReaderDelegate);
				}
				return Globals.typeOfXmlFormatClassReaderDelegate;
			}
		}

		internal static Type TypeOfXmlFormatCollectionReaderDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlFormatCollectionReaderDelegate == null)
				{
					Globals.typeOfXmlFormatCollectionReaderDelegate = typeof(XmlFormatCollectionReaderDelegate);
				}
				return Globals.typeOfXmlFormatCollectionReaderDelegate;
			}
		}

		internal static Type TypeOfXmlFormatGetOnlyCollectionReaderDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlFormatGetOnlyCollectionReaderDelegate == null)
				{
					Globals.typeOfXmlFormatGetOnlyCollectionReaderDelegate = typeof(XmlFormatGetOnlyCollectionReaderDelegate);
				}
				return Globals.typeOfXmlFormatGetOnlyCollectionReaderDelegate;
			}
		}

		internal static Type TypeOfKnownTypeAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfKnownTypeAttribute == null)
				{
					Globals.typeOfKnownTypeAttribute = typeof(KnownTypeAttribute);
				}
				return Globals.typeOfKnownTypeAttribute;
			}
		}

		internal static Type TypeOfDataContractAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfDataContractAttribute == null)
				{
					Globals.typeOfDataContractAttribute = typeof(DataContractAttribute);
				}
				return Globals.typeOfDataContractAttribute;
			}
		}

		internal static Type TypeOfContractNamespaceAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfContractNamespaceAttribute == null)
				{
					Globals.typeOfContractNamespaceAttribute = typeof(ContractNamespaceAttribute);
				}
				return Globals.typeOfContractNamespaceAttribute;
			}
		}

		internal static Type TypeOfDataMemberAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfDataMemberAttribute == null)
				{
					Globals.typeOfDataMemberAttribute = typeof(DataMemberAttribute);
				}
				return Globals.typeOfDataMemberAttribute;
			}
		}

		internal static Type TypeOfEnumMemberAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfEnumMemberAttribute == null)
				{
					Globals.typeOfEnumMemberAttribute = typeof(EnumMemberAttribute);
				}
				return Globals.typeOfEnumMemberAttribute;
			}
		}

		internal static Type TypeOfCollectionDataContractAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfCollectionDataContractAttribute == null)
				{
					Globals.typeOfCollectionDataContractAttribute = typeof(CollectionDataContractAttribute);
				}
				return Globals.typeOfCollectionDataContractAttribute;
			}
		}

		internal static Type TypeOfOptionalFieldAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfOptionalFieldAttribute == null)
				{
					Globals.typeOfOptionalFieldAttribute = typeof(OptionalFieldAttribute);
				}
				return Globals.typeOfOptionalFieldAttribute;
			}
		}

		internal static Type TypeOfObjectArray
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfObjectArray == null)
				{
					Globals.typeOfObjectArray = typeof(object[]);
				}
				return Globals.typeOfObjectArray;
			}
		}

		internal static Type TypeOfOnSerializingAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfOnSerializingAttribute == null)
				{
					Globals.typeOfOnSerializingAttribute = typeof(OnSerializingAttribute);
				}
				return Globals.typeOfOnSerializingAttribute;
			}
		}

		internal static Type TypeOfOnSerializedAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfOnSerializedAttribute == null)
				{
					Globals.typeOfOnSerializedAttribute = typeof(OnSerializedAttribute);
				}
				return Globals.typeOfOnSerializedAttribute;
			}
		}

		internal static Type TypeOfOnDeserializingAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfOnDeserializingAttribute == null)
				{
					Globals.typeOfOnDeserializingAttribute = typeof(OnDeserializingAttribute);
				}
				return Globals.typeOfOnDeserializingAttribute;
			}
		}

		internal static Type TypeOfOnDeserializedAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfOnDeserializedAttribute == null)
				{
					Globals.typeOfOnDeserializedAttribute = typeof(OnDeserializedAttribute);
				}
				return Globals.typeOfOnDeserializedAttribute;
			}
		}

		internal static Type TypeOfFlagsAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfFlagsAttribute == null)
				{
					Globals.typeOfFlagsAttribute = typeof(FlagsAttribute);
				}
				return Globals.typeOfFlagsAttribute;
			}
		}

		internal static Type TypeOfSerializableAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfSerializableAttribute == null)
				{
					Globals.typeOfSerializableAttribute = typeof(SerializableAttribute);
				}
				return Globals.typeOfSerializableAttribute;
			}
		}

		internal static Type TypeOfNonSerializedAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfNonSerializedAttribute == null)
				{
					Globals.typeOfNonSerializedAttribute = typeof(NonSerializedAttribute);
				}
				return Globals.typeOfNonSerializedAttribute;
			}
		}

		internal static Type TypeOfSerializationInfo
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfSerializationInfo == null)
				{
					Globals.typeOfSerializationInfo = typeof(SerializationInfo);
				}
				return Globals.typeOfSerializationInfo;
			}
		}

		internal static Type TypeOfSerializationInfoEnumerator
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfSerializationInfoEnumerator == null)
				{
					Globals.typeOfSerializationInfoEnumerator = typeof(SerializationInfoEnumerator);
				}
				return Globals.typeOfSerializationInfoEnumerator;
			}
		}

		internal static Type TypeOfSerializationEntry
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfSerializationEntry == null)
				{
					Globals.typeOfSerializationEntry = typeof(SerializationEntry);
				}
				return Globals.typeOfSerializationEntry;
			}
		}

		internal static Type TypeOfIXmlSerializable
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIXmlSerializable == null)
				{
					Globals.typeOfIXmlSerializable = typeof(IXmlSerializable);
				}
				return Globals.typeOfIXmlSerializable;
			}
		}

		internal static Type TypeOfXmlSchemaProviderAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlSchemaProviderAttribute == null)
				{
					Globals.typeOfXmlSchemaProviderAttribute = typeof(XmlSchemaProviderAttribute);
				}
				return Globals.typeOfXmlSchemaProviderAttribute;
			}
		}

		internal static Type TypeOfXmlRootAttribute
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlRootAttribute == null)
				{
					Globals.typeOfXmlRootAttribute = typeof(XmlRootAttribute);
				}
				return Globals.typeOfXmlRootAttribute;
			}
		}

		internal static Type TypeOfXmlQualifiedName
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlQualifiedName == null)
				{
					Globals.typeOfXmlQualifiedName = typeof(XmlQualifiedName);
				}
				return Globals.typeOfXmlQualifiedName;
			}
		}

		internal static Type TypeOfXmlSchemaType
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlSchemaType == null)
				{
					Globals.typeOfXmlSchemaType = typeof(XmlSchemaType);
				}
				return Globals.typeOfXmlSchemaType;
			}
		}

		internal static Type TypeOfXmlSerializableServices
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlSerializableServices == null)
				{
					Globals.typeOfXmlSerializableServices = typeof(XmlSerializableServices);
				}
				return Globals.typeOfXmlSerializableServices;
			}
		}

		internal static Type TypeOfXmlNodeArray
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlNodeArray == null)
				{
					Globals.typeOfXmlNodeArray = typeof(XmlNode[]);
				}
				return Globals.typeOfXmlNodeArray;
			}
		}

		internal static Type TypeOfXmlSchemaSet
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlSchemaSet == null)
				{
					Globals.typeOfXmlSchemaSet = typeof(XmlSchemaSet);
				}
				return Globals.typeOfXmlSchemaSet;
			}
		}

		internal static object[] EmptyObjectArray
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.emptyObjectArray == null)
				{
					Globals.emptyObjectArray = new object[0];
				}
				return Globals.emptyObjectArray;
			}
		}

		internal static Type[] EmptyTypeArray
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.emptyTypeArray == null)
				{
					Globals.emptyTypeArray = new Type[0];
				}
				return Globals.emptyTypeArray;
			}
		}

		internal static Type TypeOfIPropertyChange
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIPropertyChange == null)
				{
					Globals.typeOfIPropertyChange = typeof(INotifyPropertyChanged);
				}
				return Globals.typeOfIPropertyChange;
			}
		}

		internal static Type TypeOfIExtensibleDataObject
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIExtensibleDataObject == null)
				{
					Globals.typeOfIExtensibleDataObject = typeof(IExtensibleDataObject);
				}
				return Globals.typeOfIExtensibleDataObject;
			}
		}

		internal static Type TypeOfExtensionDataObject
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfExtensionDataObject == null)
				{
					Globals.typeOfExtensionDataObject = typeof(ExtensionDataObject);
				}
				return Globals.typeOfExtensionDataObject;
			}
		}

		internal static Type TypeOfISerializableDataNode
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfISerializableDataNode == null)
				{
					Globals.typeOfISerializableDataNode = typeof(ISerializableDataNode);
				}
				return Globals.typeOfISerializableDataNode;
			}
		}

		internal static Type TypeOfClassDataNode
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfClassDataNode == null)
				{
					Globals.typeOfClassDataNode = typeof(ClassDataNode);
				}
				return Globals.typeOfClassDataNode;
			}
		}

		internal static Type TypeOfCollectionDataNode
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfCollectionDataNode == null)
				{
					Globals.typeOfCollectionDataNode = typeof(CollectionDataNode);
				}
				return Globals.typeOfCollectionDataNode;
			}
		}

		internal static Type TypeOfXmlDataNode
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlDataNode == null)
				{
					Globals.typeOfXmlDataNode = typeof(XmlDataNode);
				}
				return Globals.typeOfXmlDataNode;
			}
		}

		internal static Type TypeOfNullable
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfNullable == null)
				{
					Globals.typeOfNullable = typeof(Nullable<>);
				}
				return Globals.typeOfNullable;
			}
		}

		internal static Type TypeOfReflectionPointer
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfReflectionPointer == null)
				{
					Globals.typeOfReflectionPointer = typeof(Pointer);
				}
				return Globals.typeOfReflectionPointer;
			}
		}

		internal static Type TypeOfIDictionaryGeneric
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIDictionaryGeneric == null)
				{
					Globals.typeOfIDictionaryGeneric = typeof(IDictionary<, >);
				}
				return Globals.typeOfIDictionaryGeneric;
			}
		}

		internal static Type TypeOfIDictionary
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIDictionary == null)
				{
					Globals.typeOfIDictionary = typeof(IDictionary);
				}
				return Globals.typeOfIDictionary;
			}
		}

		internal static Type TypeOfIListGeneric
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIListGeneric == null)
				{
					Globals.typeOfIListGeneric = typeof(IList<>);
				}
				return Globals.typeOfIListGeneric;
			}
		}

		internal static Type TypeOfIList
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIList == null)
				{
					Globals.typeOfIList = typeof(IList);
				}
				return Globals.typeOfIList;
			}
		}

		internal static Type TypeOfICollectionGeneric
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfICollectionGeneric == null)
				{
					Globals.typeOfICollectionGeneric = typeof(ICollection<>);
				}
				return Globals.typeOfICollectionGeneric;
			}
		}

		internal static Type TypeOfICollection
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfICollection == null)
				{
					Globals.typeOfICollection = typeof(ICollection);
				}
				return Globals.typeOfICollection;
			}
		}

		internal static Type TypeOfIEnumerableGeneric
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIEnumerableGeneric == null)
				{
					Globals.typeOfIEnumerableGeneric = typeof(IEnumerable<>);
				}
				return Globals.typeOfIEnumerableGeneric;
			}
		}

		internal static Type TypeOfIEnumerable
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIEnumerable == null)
				{
					Globals.typeOfIEnumerable = typeof(IEnumerable);
				}
				return Globals.typeOfIEnumerable;
			}
		}

		internal static Type TypeOfIEnumeratorGeneric
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIEnumeratorGeneric == null)
				{
					Globals.typeOfIEnumeratorGeneric = typeof(IEnumerator<>);
				}
				return Globals.typeOfIEnumeratorGeneric;
			}
		}

		internal static Type TypeOfIEnumerator
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIEnumerator == null)
				{
					Globals.typeOfIEnumerator = typeof(IEnumerator);
				}
				return Globals.typeOfIEnumerator;
			}
		}

		internal static Type TypeOfKeyValuePair
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfKeyValuePair == null)
				{
					Globals.typeOfKeyValuePair = typeof(KeyValuePair<, >);
				}
				return Globals.typeOfKeyValuePair;
			}
		}

		internal static Type TypeOfKeyValue
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfKeyValue == null)
				{
					Globals.typeOfKeyValue = typeof(KeyValue<, >);
				}
				return Globals.typeOfKeyValue;
			}
		}

		internal static Type TypeOfIDictionaryEnumerator
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfIDictionaryEnumerator == null)
				{
					Globals.typeOfIDictionaryEnumerator = typeof(IDictionaryEnumerator);
				}
				return Globals.typeOfIDictionaryEnumerator;
			}
		}

		internal static Type TypeOfDictionaryEnumerator
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfDictionaryEnumerator == null)
				{
					Globals.typeOfDictionaryEnumerator = typeof(CollectionDataContract.DictionaryEnumerator);
				}
				return Globals.typeOfDictionaryEnumerator;
			}
		}

		internal static Type TypeOfGenericDictionaryEnumerator
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfGenericDictionaryEnumerator == null)
				{
					Globals.typeOfGenericDictionaryEnumerator = typeof(CollectionDataContract.GenericDictionaryEnumerator<, >);
				}
				return Globals.typeOfGenericDictionaryEnumerator;
			}
		}

		internal static Type TypeOfDictionaryGeneric
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfDictionaryGeneric == null)
				{
					Globals.typeOfDictionaryGeneric = typeof(Dictionary<, >);
				}
				return Globals.typeOfDictionaryGeneric;
			}
		}

		internal static Type TypeOfHashtable
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfHashtable == null)
				{
					Globals.typeOfHashtable = typeof(Hashtable);
				}
				return Globals.typeOfHashtable;
			}
		}

		internal static Type TypeOfListGeneric
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfListGeneric == null)
				{
					Globals.typeOfListGeneric = typeof(List<>);
				}
				return Globals.typeOfListGeneric;
			}
		}

		internal static Type TypeOfXmlElement
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfXmlElement == null)
				{
					Globals.typeOfXmlElement = typeof(XmlElement);
				}
				return Globals.typeOfXmlElement;
			}
		}

		internal static Type TypeOfDBNull
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.typeOfDBNull == null)
				{
					Globals.typeOfDBNull = typeof(DBNull);
				}
				return Globals.typeOfDBNull;
			}
		}

		internal static Uri DataContractXsdBaseNamespaceUri
		{
			[SecuritySafeCritical]
			get
			{
				if (Globals.dataContractXsdBaseNamespaceUri == null)
				{
					Globals.dataContractXsdBaseNamespaceUri = new Uri("http://schemas.datacontract.org/2004/07/");
				}
				return Globals.dataContractXsdBaseNamespaceUri;
			}
		}

		internal const BindingFlags ScanAllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		[SecurityCritical]
		private static XmlQualifiedName idQualifiedName;

		[SecurityCritical]
		private static XmlQualifiedName refQualifiedName;

		[SecurityCritical]
		private static Type typeOfObject;

		[SecurityCritical]
		private static Type typeOfValueType;

		[SecurityCritical]
		private static Type typeOfArray;

		[SecurityCritical]
		private static Type typeOfString;

		[SecurityCritical]
		private static Type typeOfInt;

		[SecurityCritical]
		private static Type typeOfULong;

		[SecurityCritical]
		private static Type typeOfVoid;

		[SecurityCritical]
		private static Type typeOfByteArray;

		[SecurityCritical]
		private static Type typeOfTimeSpan;

		[SecurityCritical]
		private static Type typeOfGuid;

		[SecurityCritical]
		private static Type typeOfDateTimeOffset;

		[SecurityCritical]
		private static Type typeOfDateTimeOffsetAdapter;

		[SecurityCritical]
		private static Type typeOfUri;

		[SecurityCritical]
		private static Type typeOfTypeEnumerable;

		[SecurityCritical]
		private static Type typeOfStreamingContext;

		[SecurityCritical]
		private static Type typeOfISerializable;

		[SecurityCritical]
		private static Type typeOfIDeserializationCallback;

		[SecurityCritical]
		private static Type typeOfIObjectReference;

		[SecurityCritical]
		private static Type typeOfXmlFormatClassWriterDelegate;

		[SecurityCritical]
		private static Type typeOfXmlFormatCollectionWriterDelegate;

		[SecurityCritical]
		private static Type typeOfXmlFormatClassReaderDelegate;

		[SecurityCritical]
		private static Type typeOfXmlFormatCollectionReaderDelegate;

		[SecurityCritical]
		private static Type typeOfXmlFormatGetOnlyCollectionReaderDelegate;

		[SecurityCritical]
		private static Type typeOfKnownTypeAttribute;

		[SecurityCritical]
		private static Type typeOfDataContractAttribute;

		[SecurityCritical]
		private static Type typeOfContractNamespaceAttribute;

		[SecurityCritical]
		private static Type typeOfDataMemberAttribute;

		[SecurityCritical]
		private static Type typeOfEnumMemberAttribute;

		[SecurityCritical]
		private static Type typeOfCollectionDataContractAttribute;

		[SecurityCritical]
		private static Type typeOfOptionalFieldAttribute;

		[SecurityCritical]
		private static Type typeOfObjectArray;

		[SecurityCritical]
		private static Type typeOfOnSerializingAttribute;

		[SecurityCritical]
		private static Type typeOfOnSerializedAttribute;

		[SecurityCritical]
		private static Type typeOfOnDeserializingAttribute;

		[SecurityCritical]
		private static Type typeOfOnDeserializedAttribute;

		[SecurityCritical]
		private static Type typeOfFlagsAttribute;

		[SecurityCritical]
		private static Type typeOfSerializableAttribute;

		[SecurityCritical]
		private static Type typeOfNonSerializedAttribute;

		[SecurityCritical]
		private static Type typeOfSerializationInfo;

		[SecurityCritical]
		private static Type typeOfSerializationInfoEnumerator;

		[SecurityCritical]
		private static Type typeOfSerializationEntry;

		[SecurityCritical]
		private static Type typeOfIXmlSerializable;

		[SecurityCritical]
		private static Type typeOfXmlSchemaProviderAttribute;

		[SecurityCritical]
		private static Type typeOfXmlRootAttribute;

		[SecurityCritical]
		private static Type typeOfXmlQualifiedName;

		[SecurityCritical]
		private static Type typeOfXmlSchemaType;

		[SecurityCritical]
		private static Type typeOfXmlSerializableServices;

		[SecurityCritical]
		private static Type typeOfXmlNodeArray;

		[SecurityCritical]
		private static Type typeOfXmlSchemaSet;

		[SecurityCritical]
		private static object[] emptyObjectArray;

		[SecurityCritical]
		private static Type[] emptyTypeArray;

		[SecurityCritical]
		private static Type typeOfIPropertyChange;

		[SecurityCritical]
		private static Type typeOfIExtensibleDataObject;

		[SecurityCritical]
		private static Type typeOfExtensionDataObject;

		[SecurityCritical]
		private static Type typeOfISerializableDataNode;

		[SecurityCritical]
		private static Type typeOfClassDataNode;

		[SecurityCritical]
		private static Type typeOfCollectionDataNode;

		[SecurityCritical]
		private static Type typeOfXmlDataNode;

		[SecurityCritical]
		private static Type typeOfNullable;

		[SecurityCritical]
		private static Type typeOfReflectionPointer;

		[SecurityCritical]
		private static Type typeOfIDictionaryGeneric;

		[SecurityCritical]
		private static Type typeOfIDictionary;

		[SecurityCritical]
		private static Type typeOfIListGeneric;

		[SecurityCritical]
		private static Type typeOfIList;

		[SecurityCritical]
		private static Type typeOfICollectionGeneric;

		[SecurityCritical]
		private static Type typeOfICollection;

		[SecurityCritical]
		private static Type typeOfIEnumerableGeneric;

		[SecurityCritical]
		private static Type typeOfIEnumerable;

		[SecurityCritical]
		private static Type typeOfIEnumeratorGeneric;

		[SecurityCritical]
		private static Type typeOfIEnumerator;

		[SecurityCritical]
		private static Type typeOfKeyValuePair;

		[SecurityCritical]
		private static Type typeOfKeyValue;

		[SecurityCritical]
		private static Type typeOfIDictionaryEnumerator;

		[SecurityCritical]
		private static Type typeOfDictionaryEnumerator;

		[SecurityCritical]
		private static Type typeOfGenericDictionaryEnumerator;

		[SecurityCritical]
		private static Type typeOfDictionaryGeneric;

		[SecurityCritical]
		private static Type typeOfHashtable;

		[SecurityCritical]
		private static Type typeOfListGeneric;

		[SecurityCritical]
		private static Type typeOfXmlElement;

		[SecurityCritical]
		private static Type typeOfDBNull;

		[SecurityCritical]
		private static Uri dataContractXsdBaseNamespaceUri;

		public const bool DefaultIsRequired = false;

		public const bool DefaultEmitDefaultValue = true;

		public const int DefaultOrder = 0;

		public const bool DefaultIsReference = false;

		public static readonly string NewObjectId = string.Empty;

		public const string SimpleSRSInternalsVisiblePattern = "^[\\s]*System\\.Runtime\\.Serialization[\\s]*$";

		public const string FullSRSInternalsVisiblePattern = "^[\\s]*System\\.Runtime\\.Serialization[\\s]*,[\\s]*PublicKey[\\s]*=[\\s]*(?i:00000000000000000400000000000000)[\\s]*$";

		public const string NullObjectId = null;

		public const string Space = " ";

		public const string OpenBracket = "[";

		public const string CloseBracket = "]";

		public const string Comma = ",";

		public const string XsiPrefix = "i";

		public const string XsdPrefix = "x";

		public const string SerPrefix = "z";

		public const string SerPrefixForSchema = "ser";

		public const string ElementPrefix = "q";

		public const string DataContractXsdBaseNamespace = "http://schemas.datacontract.org/2004/07/";

		public const string DataContractXmlNamespace = "http://schemas.datacontract.org/2004/07/System.Xml";

		public const string SchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

		public const string SchemaNamespace = "http://www.w3.org/2001/XMLSchema";

		public const string XsiNilLocalName = "nil";

		public const string XsiTypeLocalName = "type";

		public const string TnsPrefix = "tns";

		public const string OccursUnbounded = "unbounded";

		public const string AnyTypeLocalName = "anyType";

		public const string StringLocalName = "string";

		public const string IntLocalName = "int";

		public const string True = "true";

		public const string False = "false";

		public const string ArrayPrefix = "ArrayOf";

		public const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";

		public const string XmlnsPrefix = "xmlns";

		public const string SchemaLocalName = "schema";

		public const string CollectionsNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

		public const string DefaultClrNamespace = "GeneratedNamespace";

		public const string DefaultTypeName = "GeneratedType";

		public const string DefaultGeneratedMember = "GeneratedMember";

		public const string DefaultFieldSuffix = "Field";

		public const string DefaultPropertySuffix = "Property";

		public const string DefaultMemberSuffix = "Member";

		public const string NameProperty = "Name";

		public const string NamespaceProperty = "Namespace";

		public const string OrderProperty = "Order";

		public const string IsReferenceProperty = "IsReference";

		public const string IsRequiredProperty = "IsRequired";

		public const string EmitDefaultValueProperty = "EmitDefaultValue";

		public const string ClrNamespaceProperty = "ClrNamespace";

		public const string ItemNameProperty = "ItemName";

		public const string KeyNameProperty = "KeyName";

		public const string ValueNameProperty = "ValueName";

		public const string SerializationInfoPropertyName = "SerializationInfo";

		public const string SerializationInfoFieldName = "info";

		public const string NodeArrayPropertyName = "Nodes";

		public const string NodeArrayFieldName = "nodesField";

		public const string ExportSchemaMethod = "ExportSchema";

		public const string IsAnyProperty = "IsAny";

		public const string ContextFieldName = "context";

		public const string GetObjectDataMethodName = "GetObjectData";

		public const string GetEnumeratorMethodName = "GetEnumerator";

		public const string MoveNextMethodName = "MoveNext";

		public const string AddValueMethodName = "AddValue";

		public const string CurrentPropertyName = "Current";

		public const string ValueProperty = "Value";

		public const string EnumeratorFieldName = "enumerator";

		public const string SerializationEntryFieldName = "entry";

		public const string ExtensionDataSetMethod = "set_ExtensionData";

		public const string ExtensionDataSetExplicitMethod = "System.Runtime.Serialization.IExtensibleDataObject.set_ExtensionData";

		public const string ExtensionDataObjectPropertyName = "ExtensionData";

		public const string ExtensionDataObjectFieldName = "extensionDataField";

		public const string AddMethodName = "Add";

		public const string ParseMethodName = "Parse";

		public const string GetCurrentMethodName = "get_Current";

		public const string SerializationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";

		public const string ClrTypeLocalName = "Type";

		public const string ClrAssemblyLocalName = "Assembly";

		public const string IsValueTypeLocalName = "IsValueType";

		public const string EnumerationValueLocalName = "EnumerationValue";

		public const string SurrogateDataLocalName = "Surrogate";

		public const string GenericTypeLocalName = "GenericType";

		public const string GenericParameterLocalName = "GenericParameter";

		public const string GenericNameAttribute = "Name";

		public const string GenericNamespaceAttribute = "Namespace";

		public const string GenericParameterNestedLevelAttribute = "NestedLevel";

		public const string IsDictionaryLocalName = "IsDictionary";

		public const string ActualTypeLocalName = "ActualType";

		public const string ActualTypeNameAttribute = "Name";

		public const string ActualTypeNamespaceAttribute = "Namespace";

		public const string DefaultValueLocalName = "DefaultValue";

		public const string EmitDefaultValueAttribute = "EmitDefaultValue";

		public const string ISerializableFactoryTypeLocalName = "FactoryType";

		public const string IdLocalName = "Id";

		public const string RefLocalName = "Ref";

		public const string ArraySizeLocalName = "Size";

		public const string KeyLocalName = "Key";

		public const string ValueLocalName = "Value";

		public const string MscorlibAssemblyName = "0";

		public const string MscorlibAssemblySimpleName = "mscorlib";

		public const string MscorlibFileName = "mscorlib.dll";

		public const string SerializationSchema = "<?xml version='1.0' encoding='utf-8'?>\n<xs:schema elementFormDefault='qualified' attributeFormDefault='qualified' xmlns:tns='http://schemas.microsoft.com/2003/10/Serialization/' targetNamespace='http://schemas.microsoft.com/2003/10/Serialization/' xmlns:xs='http://www.w3.org/2001/XMLSchema'>\n  <xs:element name='anyType' nillable='true' type='xs:anyType' />\n  <xs:element name='anyURI' nillable='true' type='xs:anyURI' />\n  <xs:element name='base64Binary' nillable='true' type='xs:base64Binary' />\n  <xs:element name='boolean' nillable='true' type='xs:boolean' />\n  <xs:element name='byte' nillable='true' type='xs:byte' />\n  <xs:element name='dateTime' nillable='true' type='xs:dateTime' />\n  <xs:element name='decimal' nillable='true' type='xs:decimal' />\n  <xs:element name='double' nillable='true' type='xs:double' />\n  <xs:element name='float' nillable='true' type='xs:float' />\n  <xs:element name='int' nillable='true' type='xs:int' />\n  <xs:element name='long' nillable='true' type='xs:long' />\n  <xs:element name='QName' nillable='true' type='xs:QName' />\n  <xs:element name='short' nillable='true' type='xs:short' />\n  <xs:element name='string' nillable='true' type='xs:string' />\n  <xs:element name='unsignedByte' nillable='true' type='xs:unsignedByte' />\n  <xs:element name='unsignedInt' nillable='true' type='xs:unsignedInt' />\n  <xs:element name='unsignedLong' nillable='true' type='xs:unsignedLong' />\n  <xs:element name='unsignedShort' nillable='true' type='xs:unsignedShort' />\n  <xs:element name='char' nillable='true' type='tns:char' />\n  <xs:simpleType name='char'>\n    <xs:restriction base='xs:int'/>\n  </xs:simpleType>  \n  <xs:element name='duration' nillable='true' type='tns:duration' />\n  <xs:simpleType name='duration'>\n    <xs:restriction base='xs:duration'>\n      <xs:pattern value='\\-?P(\\d*D)?(T(\\d*H)?(\\d*M)?(\\d*(\\.\\d*)?S)?)?' />\n      <xs:minInclusive value='-P10675199DT2H48M5.4775808S' />\n      <xs:maxInclusive value='P10675199DT2H48M5.4775807S' />\n    </xs:restriction>\n  </xs:simpleType>\n  <xs:element name='guid' nillable='true' type='tns:guid' />\n  <xs:simpleType name='guid'>\n    <xs:restriction base='xs:string'>\n      <xs:pattern value='[\\da-fA-F]{8}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{12}' />\n    </xs:restriction>\n  </xs:simpleType>\n  <xs:attribute name='FactoryType' type='xs:QName' />\n  <xs:attribute name='Id' type='xs:ID' />\n  <xs:attribute name='Ref' type='xs:IDREF' />\n</xs:schema>\n";
	}
}
