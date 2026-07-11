using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonFormatReaderInterpreter
	{
		public JsonFormatReaderInterpreter(ClassDataContract classContract)
		{
			this.classContract = classContract;
		}

		public JsonFormatReaderInterpreter(CollectionDataContract collectionContract, bool isGetOnly)
		{
			this.collectionContract = collectionContract;
			this.is_get_only_collection = isGetOnly;
		}

		public object ReadFromJson(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContextComplexJson context, XmlDictionaryString emptyDictionaryString, XmlDictionaryString[] memberNames)
		{
			this.xmlReader = xmlReader;
			this.context = context;
			this.emptyDictionaryString = emptyDictionaryString;
			this.memberNames = memberNames;
			this.CreateObject(this.classContract);
			context.AddNewObject(this.objectLocal);
			this.InvokeOnDeserializing(this.classContract);
			if (this.classContract.IsISerializable)
			{
				this.ReadISerializable(this.classContract);
			}
			else
			{
				this.ReadClass(this.classContract);
			}
			if (Globals.TypeOfIDeserializationCallback.IsAssignableFrom(this.classContract.UnderlyingType))
			{
				((IDeserializationCallback)this.objectLocal).OnDeserialization(null);
			}
			this.InvokeOnDeserialized(this.classContract);
			if (!this.InvokeFactoryMethod(this.classContract) && this.classContract.UnderlyingType == Globals.TypeOfDateTimeOffsetAdapter)
			{
				this.objectLocal = DateTimeOffsetAdapter.GetDateTimeOffset((DateTimeOffsetAdapter)this.objectLocal);
			}
			return this.objectLocal;
		}

		public object ReadCollectionFromJson(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContextComplexJson context, XmlDictionaryString emptyDictionaryString, XmlDictionaryString itemName, CollectionDataContract collectionContract)
		{
			this.xmlReader = xmlReader;
			this.context = context;
			this.emptyDictionaryString = emptyDictionaryString;
			this.itemName = itemName;
			this.collectionContract = collectionContract;
			this.ReadCollection(collectionContract);
			return this.objectLocal;
		}

		public void ReadGetOnlyCollectionFromJson(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContextComplexJson context, XmlDictionaryString emptyDictionaryString, XmlDictionaryString itemName, CollectionDataContract collectionContract)
		{
			this.xmlReader = xmlReader;
			this.context = context;
			this.emptyDictionaryString = emptyDictionaryString;
			this.itemName = itemName;
			this.collectionContract = collectionContract;
			this.ReadGetOnlyCollection(collectionContract);
		}

		private void CreateObject(ClassDataContract classContract)
		{
			Type type = (this.objectType = classContract.UnderlyingType);
			if (type.IsValueType && !classContract.IsNonAttributedType)
			{
				type = Globals.TypeOfValueType;
			}
			if (classContract.UnderlyingType == Globals.TypeOfDBNull)
			{
				this.objectLocal = DBNull.Value;
				return;
			}
			if (!classContract.IsNonAttributedType)
			{
				this.objectLocal = CodeInterpreter.ConvertValue(XmlFormatReaderGenerator.UnsafeGetUninitializedObject(DataContract.GetIdForInitialization(classContract)), Globals.TypeOfObject, type);
				return;
			}
			if (type.IsValueType)
			{
				this.objectLocal = FormatterServices.GetUninitializedObject(type);
				return;
			}
			this.objectLocal = classContract.GetNonAttributedTypeConstructor().Invoke(new object[0]);
		}

		private void InvokeOnDeserializing(ClassDataContract classContract)
		{
			if (classContract.BaseContract != null)
			{
				this.InvokeOnDeserializing(classContract.BaseContract);
			}
			if (classContract.OnDeserializing != null)
			{
				classContract.OnDeserializing.Invoke(this.objectLocal, new object[] { this.context.GetStreamingContext() });
			}
		}

		private void InvokeOnDeserialized(ClassDataContract classContract)
		{
			if (classContract.BaseContract != null)
			{
				this.InvokeOnDeserialized(classContract.BaseContract);
			}
			if (classContract.OnDeserialized != null)
			{
				classContract.OnDeserialized.Invoke(this.objectLocal, new object[] { this.context.GetStreamingContext() });
			}
		}

		private bool HasFactoryMethod(ClassDataContract classContract)
		{
			return Globals.TypeOfIObjectReference.IsAssignableFrom(classContract.UnderlyingType);
		}

		private bool InvokeFactoryMethod(ClassDataContract classContract)
		{
			if (this.HasFactoryMethod(classContract))
			{
				this.objectLocal = CodeInterpreter.ConvertValue(this.context.GetRealObject((IObjectReference)this.objectLocal, Globals.NewObjectId), Globals.TypeOfObject, classContract.UnderlyingType);
				return true;
			}
			return false;
		}

		private void ReadISerializable(ClassDataContract classContract)
		{
			ConstructorInfo constructor = classContract.UnderlyingType.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, JsonFormatGeneratorStatics.SerInfoCtorArgs, null);
			if (constructor == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Constructor that takes SerializationInfo and StreamingContext is not found for '{0}'.", new object[] { DataContract.GetClrTypeFullName(classContract.UnderlyingType) })));
			}
			this.context.ReadSerializationInfo(this.xmlReader, classContract.UnderlyingType);
			constructor.Invoke(this.objectLocal, new object[] { this.context.GetStreamingContext() });
		}

		private void ReadClass(ClassDataContract classContract)
		{
			if (classContract.HasExtensionData)
			{
				ExtensionDataObject extensionDataObject = new ExtensionDataObject();
				this.ReadMembers(classContract, extensionDataObject);
				for (ClassDataContract classDataContract = classContract; classDataContract != null; classDataContract = classDataContract.BaseContract)
				{
					MethodInfo extensionDataSetMethod = classDataContract.ExtensionDataSetMethod;
					if (extensionDataSetMethod != null)
					{
						extensionDataSetMethod.Invoke(this.objectLocal, new object[] { extensionDataObject });
					}
				}
				return;
			}
			this.ReadMembers(classContract, null);
		}

		private void ReadMembers(ClassDataContract classContract, ExtensionDataObject extensionData)
		{
			int num = classContract.MemberNames.Length;
			this.context.IncrementItemCount(num);
			int num2 = -1;
			BitFlagsGenerator bitFlagsGenerator = new BitFlagsGenerator(num);
			byte[] array = new byte[bitFlagsGenerator.GetLocalCount()];
			this.SetRequiredElements(classContract, array);
			this.SetExpectedElements(bitFlagsGenerator, 0);
			while (XmlObjectSerializerReadContext.MoveToNextElement(this.xmlReader))
			{
				int jsonMemberIndex = this.context.GetJsonMemberIndex(this.xmlReader, this.memberNames, num2, extensionData);
				if (num > 0)
				{
					this.ReadMembers(jsonMemberIndex, classContract, bitFlagsGenerator, ref num2);
				}
			}
			if (!this.CheckRequiredElements(bitFlagsGenerator, array))
			{
				XmlObjectSerializerReadContextComplexJson.ThrowMissingRequiredMembers(this.objectLocal, this.memberNames, bitFlagsGenerator.LoadArray(), array);
			}
		}

		private int ReadMembers(int index, ClassDataContract classContract, BitFlagsGenerator expectedElements, ref int memberIndex)
		{
			int num = ((classContract.BaseContract == null) ? 0 : this.ReadMembers(index, classContract.BaseContract, expectedElements, ref memberIndex));
			if (num <= index && index < num + classContract.Members.Count)
			{
				DataMember dataMember = classContract.Members[index - num];
				Type memberType = dataMember.MemberType;
				memberIndex = num;
				if (!expectedElements.Load(index))
				{
					XmlObjectSerializerReadContextComplexJson.ThrowDuplicateMemberException(this.objectLocal, this.memberNames, memberIndex);
				}
				if (dataMember.IsGetOnlyCollection)
				{
					object member = CodeInterpreter.GetMember(dataMember.MemberInfo, this.objectLocal);
					this.context.StoreCollectionMemberInfo(member);
					this.ReadValue(memberType, dataMember.Name);
				}
				else
				{
					object obj = this.ReadValue(memberType, dataMember.Name);
					CodeInterpreter.SetMember(dataMember.MemberInfo, this.objectLocal, obj);
				}
				memberIndex = index;
				this.ResetExpectedElements(expectedElements, index);
			}
			return num + classContract.Members.Count;
		}

		private bool CheckRequiredElements(BitFlagsGenerator expectedElements, byte[] requiredElements)
		{
			for (int i = 0; i < requiredElements.Length; i++)
			{
				if ((expectedElements.GetLocal(i) & requiredElements[i]) != 0)
				{
					return false;
				}
			}
			return true;
		}

		private int SetRequiredElements(ClassDataContract contract, byte[] requiredElements)
		{
			int num = ((contract.BaseContract == null) ? 0 : this.SetRequiredElements(contract.BaseContract, requiredElements));
			List<DataMember> members = contract.Members;
			int i = 0;
			while (i < members.Count)
			{
				if (members[i].IsRequired)
				{
					BitFlagsGenerator.SetBit(requiredElements, num);
				}
				i++;
				num++;
			}
			return num;
		}

		private void SetExpectedElements(BitFlagsGenerator expectedElements, int startIndex)
		{
			int bitCount = expectedElements.GetBitCount();
			for (int i = startIndex; i < bitCount; i++)
			{
				expectedElements.Store(i, true);
			}
		}

		private void ResetExpectedElements(BitFlagsGenerator expectedElements, int index)
		{
			expectedElements.Store(index, false);
		}

		private object ReadValue(Type type, string name)
		{
			Type type2 = type;
			bool flag = false;
			int num = 0;
			while (type.IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable)
			{
				num++;
				type = type.GetGenericArguments()[0];
			}
			PrimitiveDataContract primitiveDataContract = PrimitiveDataContract.GetPrimitiveDataContract(type);
			object obj;
			if ((primitiveDataContract != null && primitiveDataContract.UnderlyingType != Globals.TypeOfObject) || num != 0 || type.IsValueType)
			{
				this.context.ReadAttributes(this.xmlReader);
				string text = this.context.ReadIfNullOrRef(this.xmlReader, type, DataContract.IsTypeSerializable(type));
				if (text == null)
				{
					if (num != 0)
					{
						obj = Activator.CreateInstance(type2);
					}
					else
					{
						if (type.IsValueType)
						{
							throw new SerializationException(global::System.Runtime.Serialization.SR.GetString("ValueType '{0}' cannot be null.", new object[] { DataContract.GetClrTypeFullName(type) }));
						}
						obj = null;
					}
				}
				else if (text == string.Empty)
				{
					text = this.context.GetObjectId();
					if (type.IsValueType && !string.IsNullOrEmpty(text))
					{
						throw new SerializationException(global::System.Runtime.Serialization.SR.GetString("ValueType '{0}' cannot have id.", new object[] { DataContract.GetClrTypeFullName(type) }));
					}
					if (num != 0)
					{
						flag = true;
					}
					if (primitiveDataContract != null && primitiveDataContract.UnderlyingType != Globals.TypeOfObject)
					{
						obj = primitiveDataContract.XmlFormatReaderMethod.Invoke(this.xmlReader, new object[0]);
						if (!type.IsValueType)
						{
							this.context.AddNewObject(obj);
						}
					}
					else
					{
						obj = this.InternalDeserialize(type, name);
					}
				}
				else
				{
					if (type.IsValueType)
					{
						throw new SerializationException(global::System.Runtime.Serialization.SR.GetString("ValueType '{0}' cannot have ref to another object.", new object[] { DataContract.GetClrTypeFullName(type) }));
					}
					obj = CodeInterpreter.ConvertValue(this.context.GetExistingObject(text, type, name, string.Empty), Globals.TypeOfObject, type);
				}
				if (flag && text != null)
				{
					obj = this.WrapNullableObject(type, obj, type2, num);
				}
			}
			else
			{
				obj = this.InternalDeserialize(type, name);
			}
			return obj;
		}

		private object InternalDeserialize(Type type, string name)
		{
			Type type2 = (type.IsPointer ? Globals.TypeOfReflectionPointer : type);
			object obj = this.context.InternalDeserialize(this.xmlReader, DataContract.GetId(type2.TypeHandle), type2.TypeHandle, name, string.Empty);
			if (type.IsPointer)
			{
				return JsonFormatGeneratorStatics.UnboxPointer.Invoke(null, new object[] { obj });
			}
			return CodeInterpreter.ConvertValue(obj, Globals.TypeOfObject, type);
		}

		private object WrapNullableObject(Type innerType, object innerValue, Type outerType, int nullables)
		{
			object obj = innerValue;
			for (int i = 1; i < nullables; i++)
			{
				Type type = Globals.TypeOfNullable.MakeGenericType(new Type[] { innerType });
				obj = Activator.CreateInstance(type, new object[] { obj });
				innerType = type;
			}
			return Activator.CreateInstance(outerType, new object[] { obj });
		}

		private void ReadCollection(CollectionDataContract collectionContract)
		{
			Type type = collectionContract.UnderlyingType;
			Type itemType = collectionContract.ItemType;
			bool flag = collectionContract.Kind == CollectionKind.Array;
			ConstructorInfo constructorInfo = collectionContract.Constructor;
			if (type.IsInterface)
			{
				switch (collectionContract.Kind)
				{
				case CollectionKind.GenericDictionary:
					type = Globals.TypeOfDictionaryGeneric.MakeGenericType(itemType.GetGenericArguments());
					constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null);
					break;
				case CollectionKind.Dictionary:
					type = Globals.TypeOfHashtable;
					constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null);
					break;
				case CollectionKind.GenericList:
				case CollectionKind.GenericCollection:
				case CollectionKind.List:
				case CollectionKind.GenericEnumerable:
				case CollectionKind.Collection:
				case CollectionKind.Enumerable:
					type = itemType.MakeArrayType();
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (type.IsValueType)
				{
					this.objectLocal = FormatterServices.GetUninitializedObject(type);
				}
				else
				{
					this.objectLocal = constructorInfo.Invoke(new object[0]);
					this.context.AddNewObject(this.objectLocal);
				}
			}
			if ((collectionContract.Kind == CollectionKind.Dictionary || collectionContract.Kind == CollectionKind.GenericDictionary) & this.context.UseSimpleDictionaryFormat)
			{
				this.ReadSimpleDictionary(collectionContract, itemType);
				return;
			}
			string objectId = this.context.GetObjectId();
			bool flag2 = false;
			bool flag3 = false;
			if (flag && this.TryReadPrimitiveArray(itemType, out flag3))
			{
				flag2 = true;
			}
			if (!flag2)
			{
				object obj = null;
				if (flag)
				{
					obj = Array.CreateInstance(itemType, 32);
				}
				int i;
				for (i = 0; i < 2147483647; i++)
				{
					if (this.IsStartElement(this.itemName, this.emptyDictionaryString))
					{
						this.context.IncrementItemCount(1);
						object obj2 = this.ReadCollectionItem(collectionContract, itemType);
						if (flag)
						{
							obj = XmlFormatGeneratorStatics.EnsureArraySizeMethod.MakeGenericMethod(new Type[] { itemType }).Invoke(null, new object[] { obj, i });
							((Array)obj).SetValue(obj2, i);
						}
						else
						{
							this.StoreCollectionValue(this.objectLocal, itemType, obj2, collectionContract);
						}
					}
					else
					{
						if (this.IsEndElement())
						{
							break;
						}
						this.HandleUnexpectedItemInCollection(ref i);
					}
				}
				if (flag)
				{
					MethodInfo methodInfo = XmlFormatGeneratorStatics.TrimArraySizeMethod.MakeGenericMethod(new Type[] { itemType });
					this.objectLocal = methodInfo.Invoke(null, new object[] { obj, i });
					this.context.AddNewObjectWithId(objectId, this.objectLocal);
					return;
				}
			}
			else
			{
				this.context.AddNewObjectWithId(objectId, this.objectLocal);
			}
		}

		private void ReadSimpleDictionary(CollectionDataContract collectionContract, Type keyValueType)
		{
			Type[] genericArguments = keyValueType.GetGenericArguments();
			Type type = genericArguments[0];
			Type type2 = genericArguments[1];
			int num = 0;
			while (type.IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable)
			{
				num++;
				type = type.GetGenericArguments()[0];
			}
			DataContract memberTypeContract = ((ClassDataContract)collectionContract.ItemContract).Members[0].MemberTypeContract;
			JsonFormatReaderInterpreter.KeyParseMode keyParseMode = JsonFormatReaderInterpreter.KeyParseMode.Fail;
			if (type == Globals.TypeOfString || type == Globals.TypeOfObject)
			{
				keyParseMode = JsonFormatReaderInterpreter.KeyParseMode.AsString;
			}
			else if (type.IsEnum)
			{
				keyParseMode = JsonFormatReaderInterpreter.KeyParseMode.UsingParseEnum;
			}
			else if (memberTypeContract.ParseMethod != null)
			{
				keyParseMode = JsonFormatReaderInterpreter.KeyParseMode.UsingCustomParse;
			}
			if (keyParseMode == JsonFormatReaderInterpreter.KeyParseMode.Fail)
			{
				this.ThrowSerializationException(global::System.Runtime.Serialization.SR.GetString("Key type '{1}' for collection type '{0}' cannot be parsed in simple dictionary.", new object[]
				{
					DataContract.GetClrTypeFullName(collectionContract.UnderlyingType),
					DataContract.GetClrTypeFullName(type)
				}), Array.Empty<object>());
				return;
			}
			XmlNodeType xmlNodeType;
			while ((xmlNodeType = this.xmlReader.MoveToContent()) != XmlNodeType.EndElement)
			{
				if (xmlNodeType != XmlNodeType.Element)
				{
					this.ThrowUnexpectedStateException(XmlNodeType.Element);
				}
				this.context.IncrementItemCount(1);
				string jsonMemberName = XmlObjectSerializerReadContextComplexJson.GetJsonMemberName(this.xmlReader);
				object obj = null;
				if (keyParseMode == JsonFormatReaderInterpreter.KeyParseMode.UsingParseEnum)
				{
					obj = Enum.Parse(type, jsonMemberName);
				}
				else if (keyParseMode == JsonFormatReaderInterpreter.KeyParseMode.UsingCustomParse)
				{
					obj = memberTypeContract.ParseMethod.Invoke(null, new object[] { jsonMemberName });
				}
				if (num > 0)
				{
					obj = this.WrapNullableObject(type, obj, type2, num);
				}
				object obj2 = this.ReadValue(type2, string.Empty);
				collectionContract.AddMethod.Invoke(this.objectLocal, new object[] { obj, obj2 });
			}
		}

		private void ReadGetOnlyCollection(CollectionDataContract collectionContract)
		{
			Type underlyingType = collectionContract.UnderlyingType;
			Type itemType = collectionContract.ItemType;
			bool flag = collectionContract.Kind == CollectionKind.Array;
			int num = 0;
			this.objectLocal = this.context.GetCollectionMember();
			if ((collectionContract.Kind != CollectionKind.Dictionary && collectionContract.Kind != CollectionKind.GenericDictionary) || !this.context.UseSimpleDictionaryFormat)
			{
				if (this.IsStartElement(this.itemName, this.emptyDictionaryString))
				{
					if (this.objectLocal == null)
					{
						XmlObjectSerializerReadContext.ThrowNullValueReturnedForGetOnlyCollectionException(underlyingType);
						return;
					}
					num = 0;
					if (flag)
					{
						num = ((Array)this.objectLocal).Length;
					}
					int i = 0;
					while (i < 2147483647)
					{
						if (this.IsStartElement(this.itemName, this.emptyDictionaryString))
						{
							this.context.IncrementItemCount(1);
							object obj = this.ReadCollectionItem(collectionContract, itemType);
							if (flag)
							{
								if (num == i)
								{
									XmlObjectSerializerReadContext.ThrowArrayExceededSizeException(num, underlyingType);
								}
								else
								{
									((Array)this.objectLocal).SetValue(obj, i);
								}
							}
							else
							{
								this.StoreCollectionValue(this.objectLocal, itemType, obj, collectionContract);
							}
						}
						else
						{
							if (this.IsEndElement())
							{
								break;
							}
							this.HandleUnexpectedItemInCollection(ref i);
						}
					}
					this.context.CheckEndOfArray(this.xmlReader, num, this.itemName, this.emptyDictionaryString);
				}
				return;
			}
			if (this.objectLocal == null)
			{
				XmlObjectSerializerReadContext.ThrowNullValueReturnedForGetOnlyCollectionException(underlyingType);
				return;
			}
			this.ReadSimpleDictionary(collectionContract, itemType);
			this.context.CheckEndOfArray(this.xmlReader, num, this.itemName, this.emptyDictionaryString);
		}

		private bool TryReadPrimitiveArray(Type itemType, out bool readResult)
		{
			readResult = false;
			if (PrimitiveDataContract.GetPrimitiveDataContract(itemType) == null)
			{
				return false;
			}
			string text = null;
			TypeCode typeCode = Type.GetTypeCode(itemType);
			if (typeCode != TypeCode.Boolean)
			{
				switch (typeCode)
				{
				case TypeCode.Int32:
					text = "TryReadInt32Array";
					break;
				case TypeCode.Int64:
					text = "TryReadInt64Array";
					break;
				case TypeCode.Single:
					text = "TryReadSingleArray";
					break;
				case TypeCode.Double:
					text = "TryReadDoubleArray";
					break;
				case TypeCode.Decimal:
					text = "TryReadDecimalArray";
					break;
				case TypeCode.DateTime:
					text = "TryReadJsonDateTimeArray";
					break;
				}
			}
			else
			{
				text = "TryReadBooleanArray";
			}
			if (text != null)
			{
				MethodInfo method = typeof(JsonReaderDelegator).GetMethod(text, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				object[] array = new object[] { this.context, this.itemName, this.emptyDictionaryString, -1, this.objectLocal };
				readResult = (bool)method.Invoke((JsonReaderDelegator)this.xmlReader, array);
				this.objectLocal = array.Last<object>();
				return true;
			}
			return false;
		}

		private object ReadCollectionItem(CollectionDataContract collectionContract, Type itemType)
		{
			if (collectionContract.Kind == CollectionKind.Dictionary || collectionContract.Kind == CollectionKind.GenericDictionary)
			{
				this.context.ResetAttributes();
				return CodeInterpreter.ConvertValue(DataContractJsonSerializer.ReadJsonValue(XmlObjectSerializerWriteContextComplexJson.GetRevisedItemContract(collectionContract.ItemContract), this.xmlReader, this.context), Globals.TypeOfObject, itemType);
			}
			return this.ReadValue(itemType, "item");
		}

		private void StoreCollectionValue(object collection, Type valueType, object value, CollectionDataContract collectionContract)
		{
			if (collectionContract.Kind == CollectionKind.GenericDictionary || collectionContract.Kind == CollectionKind.Dictionary)
			{
				ClassDataContract classDataContract = DataContract.GetDataContract(valueType) as ClassDataContract;
				DataMember dataMember = classDataContract.Members[0];
				DataMember dataMember2 = classDataContract.Members[1];
				object member = CodeInterpreter.GetMember(dataMember.MemberInfo, value);
				object member2 = CodeInterpreter.GetMember(dataMember2.MemberInfo, value);
				try
				{
					collectionContract.AddMethod.Invoke(collection, new object[] { member, member2 });
					return;
				}
				catch (TargetInvocationException ex)
				{
					if (ex.InnerException != null)
					{
						throw ex.InnerException;
					}
					throw;
				}
			}
			collectionContract.AddMethod.Invoke(collection, new object[] { value });
		}

		private void HandleUnexpectedItemInCollection(ref int iterator)
		{
			if (this.IsStartElement())
			{
				this.context.SkipUnknownElement(this.xmlReader);
				iterator--;
				return;
			}
			throw XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, this.xmlReader);
		}

		private bool IsStartElement(XmlDictionaryString name, XmlDictionaryString ns)
		{
			return this.xmlReader.IsStartElement(name, ns);
		}

		private bool IsStartElement()
		{
			return this.xmlReader.IsStartElement();
		}

		private bool IsEndElement()
		{
			return this.xmlReader.NodeType == XmlNodeType.EndElement;
		}

		private void ThrowUnexpectedStateException(XmlNodeType expectedState)
		{
			throw XmlObjectSerializerReadContext.CreateUnexpectedStateException(expectedState, this.xmlReader);
		}

		private void ThrowSerializationException(string msg, params object[] values)
		{
			if (values != null && values.Length != 0)
			{
				msg = string.Format(msg, values);
			}
			throw new SerializationException(msg);
		}

		private bool is_get_only_collection;

		private ClassDataContract classContract;

		private CollectionDataContract collectionContract;

		private object objectLocal;

		private Type objectType;

		private XmlReaderDelegator xmlReader;

		private XmlObjectSerializerReadContextComplexJson context;

		private XmlDictionaryString[] memberNames;

		private XmlDictionaryString emptyDictionaryString;

		private XmlDictionaryString itemName;

		private XmlDictionaryString itemNamespace;

		private enum KeyParseMode
		{
			Fail,
			AsString,
			UsingParseEnum,
			UsingCustomParse
		}
	}
}
