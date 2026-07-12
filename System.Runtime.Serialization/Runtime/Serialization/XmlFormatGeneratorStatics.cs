using System;
using System.Collections;
using System.Reflection;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal static class XmlFormatGeneratorStatics
	{
		internal static MethodInfo WriteStartElementMethod2
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.writeStartElementMethod2 == null)
				{
					XmlFormatGeneratorStatics.writeStartElementMethod2 = typeof(XmlWriterDelegator).GetMethod("WriteStartElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(XmlDictionaryString),
						typeof(XmlDictionaryString)
					}, null);
				}
				return XmlFormatGeneratorStatics.writeStartElementMethod2;
			}
		}

		internal static MethodInfo WriteStartElementMethod3
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.writeStartElementMethod3 == null)
				{
					XmlFormatGeneratorStatics.writeStartElementMethod3 = typeof(XmlWriterDelegator).GetMethod("WriteStartElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(string),
						typeof(XmlDictionaryString),
						typeof(XmlDictionaryString)
					}, null);
				}
				return XmlFormatGeneratorStatics.writeStartElementMethod3;
			}
		}

		internal static MethodInfo WriteEndElementMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.writeEndElementMethod == null)
				{
					XmlFormatGeneratorStatics.writeEndElementMethod = typeof(XmlWriterDelegator).GetMethod("WriteEndElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
				}
				return XmlFormatGeneratorStatics.writeEndElementMethod;
			}
		}

		internal static MethodInfo WriteNamespaceDeclMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.writeNamespaceDeclMethod == null)
				{
					XmlFormatGeneratorStatics.writeNamespaceDeclMethod = typeof(XmlWriterDelegator).GetMethod("WriteNamespaceDecl", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(XmlDictionaryString) }, null);
				}
				return XmlFormatGeneratorStatics.writeNamespaceDeclMethod;
			}
		}

		internal static PropertyInfo ExtensionDataProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.extensionDataProperty == null)
				{
					XmlFormatGeneratorStatics.extensionDataProperty = typeof(IExtensibleDataObject).GetProperty("ExtensionData");
				}
				return XmlFormatGeneratorStatics.extensionDataProperty;
			}
		}

		internal static MethodInfo BoxPointer
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.boxPointer == null)
				{
					XmlFormatGeneratorStatics.boxPointer = typeof(Pointer).GetMethod("Box");
				}
				return XmlFormatGeneratorStatics.boxPointer;
			}
		}

		internal static ConstructorInfo DictionaryEnumeratorCtor
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.dictionaryEnumeratorCtor == null)
				{
					XmlFormatGeneratorStatics.dictionaryEnumeratorCtor = Globals.TypeOfDictionaryEnumerator.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { Globals.TypeOfIDictionaryEnumerator }, null);
				}
				return XmlFormatGeneratorStatics.dictionaryEnumeratorCtor;
			}
		}

		internal static MethodInfo MoveNextMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.ienumeratorMoveNextMethod == null)
				{
					XmlFormatGeneratorStatics.ienumeratorMoveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
				}
				return XmlFormatGeneratorStatics.ienumeratorMoveNextMethod;
			}
		}

		internal static MethodInfo GetCurrentMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.ienumeratorGetCurrentMethod == null)
				{
					XmlFormatGeneratorStatics.ienumeratorGetCurrentMethod = typeof(IEnumerator).GetProperty("Current").GetGetMethod();
				}
				return XmlFormatGeneratorStatics.ienumeratorGetCurrentMethod;
			}
		}

		internal static MethodInfo GetItemContractMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getItemContractMethod == null)
				{
					XmlFormatGeneratorStatics.getItemContractMethod = typeof(CollectionDataContract).GetProperty("ItemContract", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true);
				}
				return XmlFormatGeneratorStatics.getItemContractMethod;
			}
		}

		internal static MethodInfo IsStartElementMethod2
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.isStartElementMethod2 == null)
				{
					XmlFormatGeneratorStatics.isStartElementMethod2 = typeof(XmlReaderDelegator).GetMethod("IsStartElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(XmlDictionaryString),
						typeof(XmlDictionaryString)
					}, null);
				}
				return XmlFormatGeneratorStatics.isStartElementMethod2;
			}
		}

		internal static MethodInfo IsStartElementMethod0
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.isStartElementMethod0 == null)
				{
					XmlFormatGeneratorStatics.isStartElementMethod0 = typeof(XmlReaderDelegator).GetMethod("IsStartElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
				}
				return XmlFormatGeneratorStatics.isStartElementMethod0;
			}
		}

		internal static MethodInfo GetUninitializedObjectMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getUninitializedObjectMethod == null)
				{
					XmlFormatGeneratorStatics.getUninitializedObjectMethod = typeof(XmlFormatReaderGenerator).GetMethod("UnsafeGetUninitializedObject", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null);
				}
				return XmlFormatGeneratorStatics.getUninitializedObjectMethod;
			}
		}

		internal static MethodInfo OnDeserializationMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.onDeserializationMethod == null)
				{
					XmlFormatGeneratorStatics.onDeserializationMethod = typeof(IDeserializationCallback).GetMethod("OnDeserialization");
				}
				return XmlFormatGeneratorStatics.onDeserializationMethod;
			}
		}

		internal static MethodInfo UnboxPointer
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.unboxPointer == null)
				{
					XmlFormatGeneratorStatics.unboxPointer = typeof(Pointer).GetMethod("Unbox");
				}
				return XmlFormatGeneratorStatics.unboxPointer;
			}
		}

		internal static PropertyInfo NodeTypeProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.nodeTypeProperty == null)
				{
					XmlFormatGeneratorStatics.nodeTypeProperty = typeof(XmlReaderDelegator).GetProperty("NodeType", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.nodeTypeProperty;
			}
		}

		internal static ConstructorInfo SerializationExceptionCtor
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.serializationExceptionCtor == null)
				{
					XmlFormatGeneratorStatics.serializationExceptionCtor = typeof(SerializationException).GetConstructor(new Type[] { typeof(string) });
				}
				return XmlFormatGeneratorStatics.serializationExceptionCtor;
			}
		}

		internal static ConstructorInfo ExtensionDataObjectCtor
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.extensionDataObjectCtor == null)
				{
					XmlFormatGeneratorStatics.extensionDataObjectCtor = typeof(ExtensionDataObject).GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
				}
				return XmlFormatGeneratorStatics.extensionDataObjectCtor;
			}
		}

		internal static ConstructorInfo HashtableCtor
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.hashtableCtor == null)
				{
					XmlFormatGeneratorStatics.hashtableCtor = Globals.TypeOfHashtable.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null);
				}
				return XmlFormatGeneratorStatics.hashtableCtor;
			}
		}

		internal static MethodInfo GetStreamingContextMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getStreamingContextMethod == null)
				{
					XmlFormatGeneratorStatics.getStreamingContextMethod = typeof(XmlObjectSerializerContext).GetMethod("GetStreamingContext", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getStreamingContextMethod;
			}
		}

		internal static MethodInfo GetCollectionMemberMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getCollectionMemberMethod == null)
				{
					XmlFormatGeneratorStatics.getCollectionMemberMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetCollectionMember", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getCollectionMemberMethod;
			}
		}

		internal static MethodInfo StoreCollectionMemberInfoMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.storeCollectionMemberInfoMethod == null)
				{
					XmlFormatGeneratorStatics.storeCollectionMemberInfoMethod = typeof(XmlObjectSerializerReadContext).GetMethod("StoreCollectionMemberInfo", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(object) }, null);
				}
				return XmlFormatGeneratorStatics.storeCollectionMemberInfoMethod;
			}
		}

		internal static MethodInfo StoreIsGetOnlyCollectionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.storeIsGetOnlyCollectionMethod == null)
				{
					XmlFormatGeneratorStatics.storeIsGetOnlyCollectionMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("StoreIsGetOnlyCollection", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.storeIsGetOnlyCollectionMethod;
			}
		}

		internal static MethodInfo ThrowNullValueReturnedForGetOnlyCollectionExceptionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.throwNullValueReturnedForGetOnlyCollectionExceptionMethod == null)
				{
					XmlFormatGeneratorStatics.throwNullValueReturnedForGetOnlyCollectionExceptionMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ThrowNullValueReturnedForGetOnlyCollectionException", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.throwNullValueReturnedForGetOnlyCollectionExceptionMethod;
			}
		}

		internal static MethodInfo ThrowArrayExceededSizeExceptionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.throwArrayExceededSizeExceptionMethod == null)
				{
					XmlFormatGeneratorStatics.throwArrayExceededSizeExceptionMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ThrowArrayExceededSizeException", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.throwArrayExceededSizeExceptionMethod;
			}
		}

		internal static MethodInfo IncrementItemCountMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.incrementItemCountMethod == null)
				{
					XmlFormatGeneratorStatics.incrementItemCountMethod = typeof(XmlObjectSerializerContext).GetMethod("IncrementItemCount", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.incrementItemCountMethod;
			}
		}

		internal static MethodInfo DemandSerializationFormatterPermissionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.demandSerializationFormatterPermissionMethod == null)
				{
					XmlFormatGeneratorStatics.demandSerializationFormatterPermissionMethod = typeof(XmlObjectSerializerContext).GetMethod("DemandSerializationFormatterPermission", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.demandSerializationFormatterPermissionMethod;
			}
		}

		internal static MethodInfo DemandMemberAccessPermissionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.demandMemberAccessPermissionMethod == null)
				{
					XmlFormatGeneratorStatics.demandMemberAccessPermissionMethod = typeof(XmlObjectSerializerContext).GetMethod("DemandMemberAccessPermission", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.demandMemberAccessPermissionMethod;
			}
		}

		internal static MethodInfo InternalDeserializeMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.internalDeserializeMethod == null)
				{
					XmlFormatGeneratorStatics.internalDeserializeMethod = typeof(XmlObjectSerializerReadContext).GetMethod("InternalDeserialize", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(XmlReaderDelegator),
						typeof(int),
						typeof(RuntimeTypeHandle),
						typeof(string),
						typeof(string)
					}, null);
				}
				return XmlFormatGeneratorStatics.internalDeserializeMethod;
			}
		}

		internal static MethodInfo MoveToNextElementMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.moveToNextElementMethod == null)
				{
					XmlFormatGeneratorStatics.moveToNextElementMethod = typeof(XmlObjectSerializerReadContext).GetMethod("MoveToNextElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.moveToNextElementMethod;
			}
		}

		internal static MethodInfo GetMemberIndexMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getMemberIndexMethod == null)
				{
					XmlFormatGeneratorStatics.getMemberIndexMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetMemberIndex", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getMemberIndexMethod;
			}
		}

		internal static MethodInfo GetMemberIndexWithRequiredMembersMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getMemberIndexWithRequiredMembersMethod == null)
				{
					XmlFormatGeneratorStatics.getMemberIndexWithRequiredMembersMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetMemberIndexWithRequiredMembers", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getMemberIndexWithRequiredMembersMethod;
			}
		}

		internal static MethodInfo ThrowRequiredMemberMissingExceptionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.throwRequiredMemberMissingExceptionMethod == null)
				{
					XmlFormatGeneratorStatics.throwRequiredMemberMissingExceptionMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ThrowRequiredMemberMissingException", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.throwRequiredMemberMissingExceptionMethod;
			}
		}

		internal static MethodInfo SkipUnknownElementMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.skipUnknownElementMethod == null)
				{
					XmlFormatGeneratorStatics.skipUnknownElementMethod = typeof(XmlObjectSerializerReadContext).GetMethod("SkipUnknownElement", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.skipUnknownElementMethod;
			}
		}

		internal static MethodInfo ReadIfNullOrRefMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.readIfNullOrRefMethod == null)
				{
					XmlFormatGeneratorStatics.readIfNullOrRefMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ReadIfNullOrRef", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(XmlReaderDelegator),
						typeof(Type),
						typeof(bool)
					}, null);
				}
				return XmlFormatGeneratorStatics.readIfNullOrRefMethod;
			}
		}

		internal static MethodInfo ReadAttributesMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.readAttributesMethod == null)
				{
					XmlFormatGeneratorStatics.readAttributesMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ReadAttributes", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.readAttributesMethod;
			}
		}

		internal static MethodInfo ResetAttributesMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.resetAttributesMethod == null)
				{
					XmlFormatGeneratorStatics.resetAttributesMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ResetAttributes", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.resetAttributesMethod;
			}
		}

		internal static MethodInfo GetObjectIdMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getObjectIdMethod == null)
				{
					XmlFormatGeneratorStatics.getObjectIdMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetObjectId", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getObjectIdMethod;
			}
		}

		internal static MethodInfo GetArraySizeMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getArraySizeMethod == null)
				{
					XmlFormatGeneratorStatics.getArraySizeMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetArraySize", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getArraySizeMethod;
			}
		}

		internal static MethodInfo AddNewObjectMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.addNewObjectMethod == null)
				{
					XmlFormatGeneratorStatics.addNewObjectMethod = typeof(XmlObjectSerializerReadContext).GetMethod("AddNewObject", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.addNewObjectMethod;
			}
		}

		internal static MethodInfo AddNewObjectWithIdMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.addNewObjectWithIdMethod == null)
				{
					XmlFormatGeneratorStatics.addNewObjectWithIdMethod = typeof(XmlObjectSerializerReadContext).GetMethod("AddNewObjectWithId", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.addNewObjectWithIdMethod;
			}
		}

		internal static MethodInfo ReplaceDeserializedObjectMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.replaceDeserializedObjectMethod == null)
				{
					XmlFormatGeneratorStatics.replaceDeserializedObjectMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ReplaceDeserializedObject", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.replaceDeserializedObjectMethod;
			}
		}

		internal static MethodInfo GetExistingObjectMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getExistingObjectMethod == null)
				{
					XmlFormatGeneratorStatics.getExistingObjectMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetExistingObject", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getExistingObjectMethod;
			}
		}

		internal static MethodInfo GetRealObjectMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getRealObjectMethod == null)
				{
					XmlFormatGeneratorStatics.getRealObjectMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetRealObject", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getRealObjectMethod;
			}
		}

		internal static MethodInfo ReadMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.readMethod == null)
				{
					XmlFormatGeneratorStatics.readMethod = typeof(XmlObjectSerializerReadContext).GetMethod("Read", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.readMethod;
			}
		}

		internal static MethodInfo EnsureArraySizeMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.ensureArraySizeMethod == null)
				{
					XmlFormatGeneratorStatics.ensureArraySizeMethod = typeof(XmlObjectSerializerReadContext).GetMethod("EnsureArraySize", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.ensureArraySizeMethod;
			}
		}

		internal static MethodInfo TrimArraySizeMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.trimArraySizeMethod == null)
				{
					XmlFormatGeneratorStatics.trimArraySizeMethod = typeof(XmlObjectSerializerReadContext).GetMethod("TrimArraySize", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.trimArraySizeMethod;
			}
		}

		internal static MethodInfo CheckEndOfArrayMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.checkEndOfArrayMethod == null)
				{
					XmlFormatGeneratorStatics.checkEndOfArrayMethod = typeof(XmlObjectSerializerReadContext).GetMethod("CheckEndOfArray", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.checkEndOfArrayMethod;
			}
		}

		internal static MethodInfo GetArrayLengthMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getArrayLengthMethod == null)
				{
					XmlFormatGeneratorStatics.getArrayLengthMethod = Globals.TypeOfArray.GetProperty("Length").GetGetMethod();
				}
				return XmlFormatGeneratorStatics.getArrayLengthMethod;
			}
		}

		internal static MethodInfo ReadSerializationInfoMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.readSerializationInfoMethod == null)
				{
					XmlFormatGeneratorStatics.readSerializationInfoMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ReadSerializationInfo", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.readSerializationInfoMethod;
			}
		}

		internal static MethodInfo CreateUnexpectedStateExceptionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.createUnexpectedStateExceptionMethod == null)
				{
					XmlFormatGeneratorStatics.createUnexpectedStateExceptionMethod = typeof(XmlObjectSerializerReadContext).GetMethod("CreateUnexpectedStateException", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(XmlNodeType),
						typeof(XmlReaderDelegator)
					}, null);
				}
				return XmlFormatGeneratorStatics.createUnexpectedStateExceptionMethod;
			}
		}

		internal static MethodInfo InternalSerializeReferenceMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.internalSerializeReferenceMethod == null)
				{
					XmlFormatGeneratorStatics.internalSerializeReferenceMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("InternalSerializeReference", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.internalSerializeReferenceMethod;
			}
		}

		internal static MethodInfo InternalSerializeMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.internalSerializeMethod == null)
				{
					XmlFormatGeneratorStatics.internalSerializeMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("InternalSerialize", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.internalSerializeMethod;
			}
		}

		internal static MethodInfo WriteNullMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.writeNullMethod == null)
				{
					XmlFormatGeneratorStatics.writeNullMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("WriteNull", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(XmlWriterDelegator),
						typeof(Type),
						typeof(bool)
					}, null);
				}
				return XmlFormatGeneratorStatics.writeNullMethod;
			}
		}

		internal static MethodInfo IncrementArrayCountMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.incrementArrayCountMethod == null)
				{
					XmlFormatGeneratorStatics.incrementArrayCountMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("IncrementArrayCount", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.incrementArrayCountMethod;
			}
		}

		internal static MethodInfo IncrementCollectionCountMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.incrementCollectionCountMethod == null)
				{
					XmlFormatGeneratorStatics.incrementCollectionCountMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("IncrementCollectionCount", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(XmlWriterDelegator),
						typeof(ICollection)
					}, null);
				}
				return XmlFormatGeneratorStatics.incrementCollectionCountMethod;
			}
		}

		internal static MethodInfo IncrementCollectionCountGenericMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.incrementCollectionCountGenericMethod == null)
				{
					XmlFormatGeneratorStatics.incrementCollectionCountGenericMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("IncrementCollectionCountGeneric", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.incrementCollectionCountGenericMethod;
			}
		}

		internal static MethodInfo GetDefaultValueMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getDefaultValueMethod == null)
				{
					XmlFormatGeneratorStatics.getDefaultValueMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("GetDefaultValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getDefaultValueMethod;
			}
		}

		internal static MethodInfo GetNullableValueMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getNullableValueMethod == null)
				{
					XmlFormatGeneratorStatics.getNullableValueMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("GetNullableValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getNullableValueMethod;
			}
		}

		internal static MethodInfo ThrowRequiredMemberMustBeEmittedMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.throwRequiredMemberMustBeEmittedMethod == null)
				{
					XmlFormatGeneratorStatics.throwRequiredMemberMustBeEmittedMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("ThrowRequiredMemberMustBeEmitted", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.throwRequiredMemberMustBeEmittedMethod;
			}
		}

		internal static MethodInfo GetHasValueMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getHasValueMethod == null)
				{
					XmlFormatGeneratorStatics.getHasValueMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("GetHasValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getHasValueMethod;
			}
		}

		internal static MethodInfo WriteISerializableMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.writeISerializableMethod == null)
				{
					XmlFormatGeneratorStatics.writeISerializableMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("WriteISerializable", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.writeISerializableMethod;
			}
		}

		internal static MethodInfo WriteExtensionDataMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.writeExtensionDataMethod == null)
				{
					XmlFormatGeneratorStatics.writeExtensionDataMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("WriteExtensionData", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.writeExtensionDataMethod;
			}
		}

		internal static MethodInfo WriteXmlValueMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.writeXmlValueMethod == null)
				{
					XmlFormatGeneratorStatics.writeXmlValueMethod = typeof(DataContract).GetMethod("WriteXmlValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.writeXmlValueMethod;
			}
		}

		internal static MethodInfo ReadXmlValueMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.readXmlValueMethod == null)
				{
					XmlFormatGeneratorStatics.readXmlValueMethod = typeof(DataContract).GetMethod("ReadXmlValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.readXmlValueMethod;
			}
		}

		internal static MethodInfo ThrowTypeNotSerializableMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.throwTypeNotSerializableMethod == null)
				{
					XmlFormatGeneratorStatics.throwTypeNotSerializableMethod = typeof(DataContract).GetMethod("ThrowTypeNotSerializable", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.throwTypeNotSerializableMethod;
			}
		}

		internal static PropertyInfo NamespaceProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.namespaceProperty == null)
				{
					XmlFormatGeneratorStatics.namespaceProperty = typeof(DataContract).GetProperty("Namespace", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.namespaceProperty;
			}
		}

		internal static FieldInfo ContractNamespacesField
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.contractNamespacesField == null)
				{
					XmlFormatGeneratorStatics.contractNamespacesField = typeof(ClassDataContract).GetField("ContractNamespaces", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.contractNamespacesField;
			}
		}

		internal static FieldInfo MemberNamesField
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.memberNamesField == null)
				{
					XmlFormatGeneratorStatics.memberNamesField = typeof(ClassDataContract).GetField("MemberNames", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.memberNamesField;
			}
		}

		internal static MethodInfo ExtensionDataSetExplicitMethodInfo
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.extensionDataSetExplicitMethodInfo == null)
				{
					XmlFormatGeneratorStatics.extensionDataSetExplicitMethodInfo = typeof(IExtensibleDataObject).GetMethod("set_ExtensionData");
				}
				return XmlFormatGeneratorStatics.extensionDataSetExplicitMethodInfo;
			}
		}

		internal static PropertyInfo ChildElementNamespacesProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.childElementNamespacesProperty == null)
				{
					XmlFormatGeneratorStatics.childElementNamespacesProperty = typeof(ClassDataContract).GetProperty("ChildElementNamespaces", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.childElementNamespacesProperty;
			}
		}

		internal static PropertyInfo CollectionItemNameProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.collectionItemNameProperty == null)
				{
					XmlFormatGeneratorStatics.collectionItemNameProperty = typeof(CollectionDataContract).GetProperty("CollectionItemName", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.collectionItemNameProperty;
			}
		}

		internal static PropertyInfo ChildElementNamespaceProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.childElementNamespaceProperty == null)
				{
					XmlFormatGeneratorStatics.childElementNamespaceProperty = typeof(CollectionDataContract).GetProperty("ChildElementNamespace", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.childElementNamespaceProperty;
			}
		}

		internal static MethodInfo GetDateTimeOffsetMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getDateTimeOffsetMethod == null)
				{
					XmlFormatGeneratorStatics.getDateTimeOffsetMethod = typeof(DateTimeOffsetAdapter).GetMethod("GetDateTimeOffset", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getDateTimeOffsetMethod;
			}
		}

		internal static MethodInfo GetDateTimeOffsetAdapterMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.getDateTimeOffsetAdapterMethod == null)
				{
					XmlFormatGeneratorStatics.getDateTimeOffsetAdapterMethod = typeof(DateTimeOffsetAdapter).GetMethod("GetDateTimeOffsetAdapter", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.getDateTimeOffsetAdapterMethod;
			}
		}

		internal static MethodInfo TraceInstructionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.traceInstructionMethod == null)
				{
					XmlFormatGeneratorStatics.traceInstructionMethod = typeof(SerializationTrace).GetMethod("TraceInstruction", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.traceInstructionMethod;
			}
		}

		internal static MethodInfo ThrowInvalidDataContractExceptionMethod
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.throwInvalidDataContractExceptionMethod == null)
				{
					XmlFormatGeneratorStatics.throwInvalidDataContractExceptionMethod = typeof(DataContract).GetMethod("ThrowInvalidDataContractException", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(string),
						typeof(Type)
					}, null);
				}
				return XmlFormatGeneratorStatics.throwInvalidDataContractExceptionMethod;
			}
		}

		internal static PropertyInfo SerializeReadOnlyTypesProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.serializeReadOnlyTypesProperty == null)
				{
					XmlFormatGeneratorStatics.serializeReadOnlyTypesProperty = typeof(XmlObjectSerializerWriteContext).GetProperty("SerializeReadOnlyTypes", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.serializeReadOnlyTypesProperty;
			}
		}

		internal static PropertyInfo ClassSerializationExceptionMessageProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.classSerializationExceptionMessageProperty == null)
				{
					XmlFormatGeneratorStatics.classSerializationExceptionMessageProperty = typeof(ClassDataContract).GetProperty("SerializationExceptionMessage", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.classSerializationExceptionMessageProperty;
			}
		}

		internal static PropertyInfo CollectionSerializationExceptionMessageProperty
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlFormatGeneratorStatics.collectionSerializationExceptionMessageProperty == null)
				{
					XmlFormatGeneratorStatics.collectionSerializationExceptionMessageProperty = typeof(CollectionDataContract).GetProperty("SerializationExceptionMessage", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlFormatGeneratorStatics.collectionSerializationExceptionMessageProperty;
			}
		}

		[SecurityCritical]
		private static MethodInfo writeStartElementMethod2;

		[SecurityCritical]
		private static MethodInfo writeStartElementMethod3;

		[SecurityCritical]
		private static MethodInfo writeEndElementMethod;

		[SecurityCritical]
		private static MethodInfo writeNamespaceDeclMethod;

		[SecurityCritical]
		private static PropertyInfo extensionDataProperty;

		[SecurityCritical]
		private static MethodInfo boxPointer;

		[SecurityCritical]
		private static ConstructorInfo dictionaryEnumeratorCtor;

		[SecurityCritical]
		private static MethodInfo ienumeratorMoveNextMethod;

		[SecurityCritical]
		private static MethodInfo ienumeratorGetCurrentMethod;

		[SecurityCritical]
		private static MethodInfo getItemContractMethod;

		[SecurityCritical]
		private static MethodInfo isStartElementMethod2;

		[SecurityCritical]
		private static MethodInfo isStartElementMethod0;

		[SecurityCritical]
		private static MethodInfo getUninitializedObjectMethod;

		[SecurityCritical]
		private static MethodInfo onDeserializationMethod;

		[SecurityCritical]
		private static MethodInfo unboxPointer;

		[SecurityCritical]
		private static PropertyInfo nodeTypeProperty;

		[SecurityCritical]
		private static ConstructorInfo serializationExceptionCtor;

		[SecurityCritical]
		private static ConstructorInfo extensionDataObjectCtor;

		[SecurityCritical]
		private static ConstructorInfo hashtableCtor;

		[SecurityCritical]
		private static MethodInfo getStreamingContextMethod;

		[SecurityCritical]
		private static MethodInfo getCollectionMemberMethod;

		[SecurityCritical]
		private static MethodInfo storeCollectionMemberInfoMethod;

		[SecurityCritical]
		private static MethodInfo storeIsGetOnlyCollectionMethod;

		[SecurityCritical]
		private static MethodInfo throwNullValueReturnedForGetOnlyCollectionExceptionMethod;

		private static MethodInfo throwArrayExceededSizeExceptionMethod;

		[SecurityCritical]
		private static MethodInfo incrementItemCountMethod;

		[SecurityCritical]
		private static MethodInfo demandSerializationFormatterPermissionMethod;

		[SecurityCritical]
		private static MethodInfo demandMemberAccessPermissionMethod;

		[SecurityCritical]
		private static MethodInfo internalDeserializeMethod;

		[SecurityCritical]
		private static MethodInfo moveToNextElementMethod;

		[SecurityCritical]
		private static MethodInfo getMemberIndexMethod;

		[SecurityCritical]
		private static MethodInfo getMemberIndexWithRequiredMembersMethod;

		[SecurityCritical]
		private static MethodInfo throwRequiredMemberMissingExceptionMethod;

		[SecurityCritical]
		private static MethodInfo skipUnknownElementMethod;

		[SecurityCritical]
		private static MethodInfo readIfNullOrRefMethod;

		[SecurityCritical]
		private static MethodInfo readAttributesMethod;

		[SecurityCritical]
		private static MethodInfo resetAttributesMethod;

		[SecurityCritical]
		private static MethodInfo getObjectIdMethod;

		[SecurityCritical]
		private static MethodInfo getArraySizeMethod;

		[SecurityCritical]
		private static MethodInfo addNewObjectMethod;

		[SecurityCritical]
		private static MethodInfo addNewObjectWithIdMethod;

		[SecurityCritical]
		private static MethodInfo replaceDeserializedObjectMethod;

		[SecurityCritical]
		private static MethodInfo getExistingObjectMethod;

		[SecurityCritical]
		private static MethodInfo getRealObjectMethod;

		[SecurityCritical]
		private static MethodInfo readMethod;

		[SecurityCritical]
		private static MethodInfo ensureArraySizeMethod;

		[SecurityCritical]
		private static MethodInfo trimArraySizeMethod;

		[SecurityCritical]
		private static MethodInfo checkEndOfArrayMethod;

		[SecurityCritical]
		private static MethodInfo getArrayLengthMethod;

		[SecurityCritical]
		private static MethodInfo readSerializationInfoMethod;

		[SecurityCritical]
		private static MethodInfo createUnexpectedStateExceptionMethod;

		[SecurityCritical]
		private static MethodInfo internalSerializeReferenceMethod;

		[SecurityCritical]
		private static MethodInfo internalSerializeMethod;

		[SecurityCritical]
		private static MethodInfo writeNullMethod;

		[SecurityCritical]
		private static MethodInfo incrementArrayCountMethod;

		[SecurityCritical]
		private static MethodInfo incrementCollectionCountMethod;

		[SecurityCritical]
		private static MethodInfo incrementCollectionCountGenericMethod;

		[SecurityCritical]
		private static MethodInfo getDefaultValueMethod;

		[SecurityCritical]
		private static MethodInfo getNullableValueMethod;

		[SecurityCritical]
		private static MethodInfo throwRequiredMemberMustBeEmittedMethod;

		[SecurityCritical]
		private static MethodInfo getHasValueMethod;

		[SecurityCritical]
		private static MethodInfo writeISerializableMethod;

		[SecurityCritical]
		private static MethodInfo writeExtensionDataMethod;

		[SecurityCritical]
		private static MethodInfo writeXmlValueMethod;

		[SecurityCritical]
		private static MethodInfo readXmlValueMethod;

		[SecurityCritical]
		private static MethodInfo throwTypeNotSerializableMethod;

		[SecurityCritical]
		private static PropertyInfo namespaceProperty;

		[SecurityCritical]
		private static FieldInfo contractNamespacesField;

		[SecurityCritical]
		private static FieldInfo memberNamesField;

		[SecurityCritical]
		private static MethodInfo extensionDataSetExplicitMethodInfo;

		[SecurityCritical]
		private static PropertyInfo childElementNamespacesProperty;

		[SecurityCritical]
		private static PropertyInfo collectionItemNameProperty;

		[SecurityCritical]
		private static PropertyInfo childElementNamespaceProperty;

		[SecurityCritical]
		private static MethodInfo getDateTimeOffsetMethod;

		[SecurityCritical]
		private static MethodInfo getDateTimeOffsetAdapterMethod;

		[SecurityCritical]
		private static MethodInfo traceInstructionMethod;

		[SecurityCritical]
		private static MethodInfo throwInvalidDataContractExceptionMethod;

		[SecurityCritical]
		private static PropertyInfo serializeReadOnlyTypesProperty;

		[SecurityCritical]
		private static PropertyInfo classSerializationExceptionMessageProperty;

		[SecurityCritical]
		private static PropertyInfo collectionSerializationExceptionMessageProperty;
	}
}
