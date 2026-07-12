using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal static class JsonFormatGeneratorStatics
	{
		public static MethodInfo BoxPointer
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.boxPointer == null)
				{
					JsonFormatGeneratorStatics.boxPointer = typeof(Pointer).GetMethod("Box");
				}
				return JsonFormatGeneratorStatics.boxPointer;
			}
		}

		public static PropertyInfo CollectionItemNameProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.collectionItemNameProperty == null)
				{
					JsonFormatGeneratorStatics.collectionItemNameProperty = typeof(XmlObjectSerializerWriteContextComplexJson).GetProperty("CollectionItemName", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.collectionItemNameProperty;
			}
		}

		public static ConstructorInfo ExtensionDataObjectCtor
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.extensionDataObjectCtor == null)
				{
					JsonFormatGeneratorStatics.extensionDataObjectCtor = typeof(ExtensionDataObject).GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
				}
				return JsonFormatGeneratorStatics.extensionDataObjectCtor;
			}
		}

		public static PropertyInfo ExtensionDataProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.extensionDataProperty == null)
				{
					JsonFormatGeneratorStatics.extensionDataProperty = typeof(IExtensibleDataObject).GetProperty("ExtensionData");
				}
				return JsonFormatGeneratorStatics.extensionDataProperty;
			}
		}

		public static MethodInfo GetCurrentMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.ienumeratorGetCurrentMethod == null)
				{
					JsonFormatGeneratorStatics.ienumeratorGetCurrentMethod = typeof(IEnumerator).GetProperty("Current").GetGetMethod();
				}
				return JsonFormatGeneratorStatics.ienumeratorGetCurrentMethod;
			}
		}

		public static MethodInfo GetItemContractMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.getItemContractMethod == null)
				{
					JsonFormatGeneratorStatics.getItemContractMethod = typeof(CollectionDataContract).GetProperty("ItemContract", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true);
				}
				return JsonFormatGeneratorStatics.getItemContractMethod;
			}
		}

		public static MethodInfo GetJsonDataContractMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.getJsonDataContractMethod == null)
				{
					JsonFormatGeneratorStatics.getJsonDataContractMethod = typeof(JsonDataContract).GetMethod("GetJsonDataContract", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.getJsonDataContractMethod;
			}
		}

		public static MethodInfo GetJsonMemberIndexMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.getJsonMemberIndexMethod == null)
				{
					JsonFormatGeneratorStatics.getJsonMemberIndexMethod = typeof(XmlObjectSerializerReadContextComplexJson).GetMethod("GetJsonMemberIndex", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.getJsonMemberIndexMethod;
			}
		}

		public static MethodInfo GetRevisedItemContractMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.getRevisedItemContractMethod == null)
				{
					JsonFormatGeneratorStatics.getRevisedItemContractMethod = typeof(XmlObjectSerializerWriteContextComplexJson).GetMethod("GetRevisedItemContract", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.getRevisedItemContractMethod;
			}
		}

		public static MethodInfo GetUninitializedObjectMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.getUninitializedObjectMethod == null)
				{
					JsonFormatGeneratorStatics.getUninitializedObjectMethod = typeof(XmlFormatReaderGenerator).GetMethod("UnsafeGetUninitializedObject", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null);
				}
				return JsonFormatGeneratorStatics.getUninitializedObjectMethod;
			}
		}

		public static MethodInfo IsStartElementMethod0
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.isStartElementMethod0 == null)
				{
					JsonFormatGeneratorStatics.isStartElementMethod0 = typeof(XmlReaderDelegator).GetMethod("IsStartElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
				}
				return JsonFormatGeneratorStatics.isStartElementMethod0;
			}
		}

		public static MethodInfo IsStartElementMethod2
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.isStartElementMethod2 == null)
				{
					JsonFormatGeneratorStatics.isStartElementMethod2 = typeof(XmlReaderDelegator).GetMethod("IsStartElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(XmlDictionaryString),
						typeof(XmlDictionaryString)
					}, null);
				}
				return JsonFormatGeneratorStatics.isStartElementMethod2;
			}
		}

		public static PropertyInfo LocalNameProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.localNameProperty == null)
				{
					JsonFormatGeneratorStatics.localNameProperty = typeof(XmlReaderDelegator).GetProperty("LocalName", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.localNameProperty;
			}
		}

		public static PropertyInfo NamespaceProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.namespaceProperty == null)
				{
					JsonFormatGeneratorStatics.namespaceProperty = typeof(XmlReaderDelegator).GetProperty("NamespaceProperty", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.namespaceProperty;
			}
		}

		public static MethodInfo MoveNextMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.ienumeratorMoveNextMethod == null)
				{
					JsonFormatGeneratorStatics.ienumeratorMoveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
				}
				return JsonFormatGeneratorStatics.ienumeratorMoveNextMethod;
			}
		}

		public static MethodInfo MoveToContentMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.moveToContentMethod == null)
				{
					JsonFormatGeneratorStatics.moveToContentMethod = typeof(XmlReaderDelegator).GetMethod("MoveToContent", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.moveToContentMethod;
			}
		}

		public static PropertyInfo NodeTypeProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.nodeTypeProperty == null)
				{
					JsonFormatGeneratorStatics.nodeTypeProperty = typeof(XmlReaderDelegator).GetProperty("NodeType", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.nodeTypeProperty;
			}
		}

		public static MethodInfo OnDeserializationMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.onDeserializationMethod == null)
				{
					JsonFormatGeneratorStatics.onDeserializationMethod = typeof(IDeserializationCallback).GetMethod("OnDeserialization");
				}
				return JsonFormatGeneratorStatics.onDeserializationMethod;
			}
		}

		public static MethodInfo ReadJsonValueMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.readJsonValueMethod == null)
				{
					JsonFormatGeneratorStatics.readJsonValueMethod = typeof(DataContractJsonSerializer).GetMethod("ReadJsonValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.readJsonValueMethod;
			}
		}

		public static ConstructorInfo SerializationExceptionCtor
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.serializationExceptionCtor == null)
				{
					JsonFormatGeneratorStatics.serializationExceptionCtor = typeof(SerializationException).GetConstructor(new Type[] { typeof(string) });
				}
				return JsonFormatGeneratorStatics.serializationExceptionCtor;
			}
		}

		public static Type[] SerInfoCtorArgs
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.serInfoCtorArgs == null)
				{
					JsonFormatGeneratorStatics.serInfoCtorArgs = new Type[]
					{
						typeof(SerializationInfo),
						typeof(StreamingContext)
					};
				}
				return JsonFormatGeneratorStatics.serInfoCtorArgs;
			}
		}

		public static MethodInfo ThrowDuplicateMemberExceptionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.throwDuplicateMemberExceptionMethod == null)
				{
					JsonFormatGeneratorStatics.throwDuplicateMemberExceptionMethod = typeof(XmlObjectSerializerReadContextComplexJson).GetMethod("ThrowDuplicateMemberException", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.throwDuplicateMemberExceptionMethod;
			}
		}

		public static MethodInfo ThrowMissingRequiredMembersMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.throwMissingRequiredMembersMethod == null)
				{
					JsonFormatGeneratorStatics.throwMissingRequiredMembersMethod = typeof(XmlObjectSerializerReadContextComplexJson).GetMethod("ThrowMissingRequiredMembers", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.throwMissingRequiredMembersMethod;
			}
		}

		public static PropertyInfo TypeHandleProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.typeHandleProperty == null)
				{
					JsonFormatGeneratorStatics.typeHandleProperty = typeof(Type).GetProperty("TypeHandle");
				}
				return JsonFormatGeneratorStatics.typeHandleProperty;
			}
		}

		public static MethodInfo UnboxPointer
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.unboxPointer == null)
				{
					JsonFormatGeneratorStatics.unboxPointer = typeof(Pointer).GetMethod("Unbox");
				}
				return JsonFormatGeneratorStatics.unboxPointer;
			}
		}

		public static PropertyInfo UseSimpleDictionaryFormatReadProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.useSimpleDictionaryFormatReadProperty == null)
				{
					JsonFormatGeneratorStatics.useSimpleDictionaryFormatReadProperty = typeof(XmlObjectSerializerReadContextComplexJson).GetProperty("UseSimpleDictionaryFormat", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.useSimpleDictionaryFormatReadProperty;
			}
		}

		public static PropertyInfo UseSimpleDictionaryFormatWriteProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.useSimpleDictionaryFormatWriteProperty == null)
				{
					JsonFormatGeneratorStatics.useSimpleDictionaryFormatWriteProperty = typeof(XmlObjectSerializerWriteContextComplexJson).GetProperty("UseSimpleDictionaryFormat", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.useSimpleDictionaryFormatWriteProperty;
			}
		}

		public static MethodInfo WriteAttributeStringMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.writeAttributeStringMethod == null)
				{
					JsonFormatGeneratorStatics.writeAttributeStringMethod = typeof(XmlWriterDelegator).GetMethod("WriteAttributeString", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(string),
						typeof(string),
						typeof(string),
						typeof(string)
					}, null);
				}
				return JsonFormatGeneratorStatics.writeAttributeStringMethod;
			}
		}

		public static MethodInfo WriteEndElementMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.writeEndElementMethod == null)
				{
					JsonFormatGeneratorStatics.writeEndElementMethod = typeof(XmlWriterDelegator).GetMethod("WriteEndElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
				}
				return JsonFormatGeneratorStatics.writeEndElementMethod;
			}
		}

		public static MethodInfo WriteJsonISerializableMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.writeJsonISerializableMethod == null)
				{
					JsonFormatGeneratorStatics.writeJsonISerializableMethod = typeof(XmlObjectSerializerWriteContextComplexJson).GetMethod("WriteJsonISerializable", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.writeJsonISerializableMethod;
			}
		}

		public static MethodInfo WriteJsonNameWithMappingMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.writeJsonNameWithMappingMethod == null)
				{
					JsonFormatGeneratorStatics.writeJsonNameWithMappingMethod = typeof(XmlObjectSerializerWriteContextComplexJson).GetMethod("WriteJsonNameWithMapping", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.writeJsonNameWithMappingMethod;
			}
		}

		public static MethodInfo WriteJsonValueMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.writeJsonValueMethod == null)
				{
					JsonFormatGeneratorStatics.writeJsonValueMethod = typeof(DataContractJsonSerializer).GetMethod("WriteJsonValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return JsonFormatGeneratorStatics.writeJsonValueMethod;
			}
		}

		public static MethodInfo WriteStartElementMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.writeStartElementMethod == null)
				{
					JsonFormatGeneratorStatics.writeStartElementMethod = typeof(XmlWriterDelegator).GetMethod("WriteStartElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(XmlDictionaryString),
						typeof(XmlDictionaryString)
					}, null);
				}
				return JsonFormatGeneratorStatics.writeStartElementMethod;
			}
		}

		public static MethodInfo WriteStartElementStringMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.writeStartElementStringMethod == null)
				{
					JsonFormatGeneratorStatics.writeStartElementStringMethod = typeof(XmlWriterDelegator).GetMethod("WriteStartElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(string),
						typeof(string)
					}, null);
				}
				return JsonFormatGeneratorStatics.writeStartElementStringMethod;
			}
		}

		public static MethodInfo ParseEnumMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.parseEnumMethod == null)
				{
					JsonFormatGeneratorStatics.parseEnumMethod = typeof(Enum).GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[]
					{
						typeof(Type),
						typeof(string)
					}, null);
				}
				return JsonFormatGeneratorStatics.parseEnumMethod;
			}
		}

		public static MethodInfo GetJsonMemberNameMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonFormatGeneratorStatics.getJsonMemberNameMethod == null)
				{
					JsonFormatGeneratorStatics.getJsonMemberNameMethod = typeof(XmlObjectSerializerReadContextComplexJson).GetMethod("GetJsonMemberName", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(XmlReaderDelegator) }, null);
				}
				return JsonFormatGeneratorStatics.getJsonMemberNameMethod;
			}
		}

		[SecurityCritical]
		private static MethodInfo boxPointer;

		[SecurityCritical]
		private static PropertyInfo collectionItemNameProperty;

		[SecurityCritical]
		private static ConstructorInfo extensionDataObjectCtor;

		[SecurityCritical]
		private static PropertyInfo extensionDataProperty;

		[SecurityCritical]
		private static MethodInfo getItemContractMethod;

		[SecurityCritical]
		private static MethodInfo getJsonDataContractMethod;

		[SecurityCritical]
		private static MethodInfo getJsonMemberIndexMethod;

		[SecurityCritical]
		private static MethodInfo getRevisedItemContractMethod;

		[SecurityCritical]
		private static MethodInfo getUninitializedObjectMethod;

		[SecurityCritical]
		private static MethodInfo ienumeratorGetCurrentMethod;

		[SecurityCritical]
		private static MethodInfo ienumeratorMoveNextMethod;

		[SecurityCritical]
		private static MethodInfo isStartElementMethod0;

		[SecurityCritical]
		private static MethodInfo isStartElementMethod2;

		[SecurityCritical]
		private static PropertyInfo localNameProperty;

		[SecurityCritical]
		private static PropertyInfo namespaceProperty;

		[SecurityCritical]
		private static MethodInfo moveToContentMethod;

		[SecurityCritical]
		private static PropertyInfo nodeTypeProperty;

		[SecurityCritical]
		private static MethodInfo onDeserializationMethod;

		[SecurityCritical]
		private static MethodInfo readJsonValueMethod;

		[SecurityCritical]
		private static ConstructorInfo serializationExceptionCtor;

		[SecurityCritical]
		private static Type[] serInfoCtorArgs;

		[SecurityCritical]
		private static MethodInfo throwDuplicateMemberExceptionMethod;

		[SecurityCritical]
		private static MethodInfo throwMissingRequiredMembersMethod;

		[SecurityCritical]
		private static PropertyInfo typeHandleProperty;

		[SecurityCritical]
		private static MethodInfo unboxPointer;

		[SecurityCritical]
		private static PropertyInfo useSimpleDictionaryFormatReadProperty;

		[SecurityCritical]
		private static PropertyInfo useSimpleDictionaryFormatWriteProperty;

		[SecurityCritical]
		private static MethodInfo writeAttributeStringMethod;

		[SecurityCritical]
		private static MethodInfo writeEndElementMethod;

		[SecurityCritical]
		private static MethodInfo writeJsonISerializableMethod;

		[SecurityCritical]
		private static MethodInfo writeJsonNameWithMappingMethod;

		[SecurityCritical]
		private static MethodInfo writeJsonValueMethod;

		[SecurityCritical]
		private static MethodInfo writeStartElementMethod;

		[SecurityCritical]
		private static MethodInfo writeStartElementStringMethod;

		[SecurityCritical]
		private static MethodInfo parseEnumMethod;

		[SecurityCritical]
		private static MethodInfo getJsonMemberNameMethod;
	}
}
