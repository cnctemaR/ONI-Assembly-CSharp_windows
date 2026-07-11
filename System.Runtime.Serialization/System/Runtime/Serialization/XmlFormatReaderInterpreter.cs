using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class XmlFormatReaderInterpreter
	{
		public XmlFormatReaderInterpreter(ClassDataContract classContract)
		{
			this.classContract = classContract;
		}

		public XmlFormatReaderInterpreter(CollectionDataContract collectionContract, bool isGetOnly)
		{
			this.collectionContract = collectionContract;
			this.is_get_only_collection = isGetOnly;
		}

		public object ReadFromXml(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces)
		{
			this.xmlReader = xmlReader;
			this.context = context;
			this.memberNames = memberNames;
			this.memberNamespaces = memberNamespaces;
			this.CreateObject(this.classContract);
			context.AddNewObject(this.objectLocal);
			this.InvokeOnDeserializing(this.classContract);
			string text = null;
			if (this.HasFactoryMethod(this.classContract))
			{
				text = context.GetObjectId();
			}
			if (this.classContract.IsISerializable)
			{
				this.ReadISerializable(this.classContract);
			}
			else
			{
				this.ReadClass(this.classContract);
			}
			bool flag = this.InvokeFactoryMethod(this.classContract, text);
			if (Globals.TypeOfIDeserializationCallback.IsAssignableFrom(this.classContract.UnderlyingType))
			{
				((IDeserializationCallback)this.objectLocal).OnDeserialization(null);
			}
			this.InvokeOnDeserialized(this.classContract);
			if ((text == null || !flag) && this.classContract.UnderlyingType == Globals.TypeOfDateTimeOffsetAdapter)
			{
				this.objectLocal = DateTimeOffsetAdapter.GetDateTimeOffset((DateTimeOffsetAdapter)this.objectLocal);
			}
			return this.objectLocal;
		}

		public object ReadCollectionFromXml(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, CollectionDataContract collectionContract)
		{
			this.xmlReader = xmlReader;
			this.context = context;
			this.itemName = itemName;
			this.itemNamespace = itemNamespace;
			this.collectionContract = collectionContract;
			this.ReadCollection(collectionContract);
			return this.objectLocal;
		}

		public void ReadGetOnlyCollectionFromXml(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, CollectionDataContract collectionContract)
		{
			this.xmlReader = xmlReader;
			this.context = context;
			this.itemName = itemName;
			this.itemNamespace = itemNamespace;
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

		private bool InvokeFactoryMethod(ClassDataContract classContract, string objectId)
		{
			if (this.HasFactoryMethod(classContract))
			{
				this.objectLocal = CodeInterpreter.ConvertValue(this.context.GetRealObject((IObjectReference)this.objectLocal, objectId), Globals.TypeOfObject, classContract.UnderlyingType);
				return true;
			}
			return false;
		}

		private void ReadISerializable(ClassDataContract classContract)
		{
			MethodBase iserializableConstructor = classContract.GetISerializableConstructor();
			SerializationInfo serializationInfo = this.context.ReadSerializationInfo(this.xmlReader, classContract.UnderlyingType);
			iserializableConstructor.Invoke(this.objectLocal, new object[]
			{
				serializationInfo,
				this.context.GetStreamingContext()
			});
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
			int num3;
			bool[] requiredMembers = this.GetRequiredMembers(classContract, out num3);
			bool flag = num3 < num;
			int num4 = (flag ? num3 : num);
			while (XmlObjectSerializerReadContext.MoveToNextElement(this.xmlReader))
			{
				int num5;
				if (flag)
				{
					num5 = this.context.GetMemberIndexWithRequiredMembers(this.xmlReader, this.memberNames, this.memberNamespaces, num2, num4, extensionData);
				}
				else
				{
					num5 = this.context.GetMemberIndex(this.xmlReader, this.memberNames, this.memberNamespaces, num2, extensionData);
				}
				if (num > 0)
				{
					this.ReadMembers(num5, classContract, requiredMembers, ref num2, ref num4);
				}
			}
			if (flag && num4 < num)
			{
				XmlObjectSerializerReadContext.ThrowRequiredMemberMissingException(this.xmlReader, num2, num4, this.memberNames);
			}
		}

		private int ReadMembers(int index, ClassDataContract classContract, bool[] requiredMembers, ref int memberIndex, ref int requiredIndex)
		{
			int num = ((classContract.BaseContract == null) ? 0 : this.ReadMembers(index, classContract.BaseContract, requiredMembers, ref memberIndex, ref requiredIndex));
			if (num <= index && index < num + classContract.Members.Count)
			{
				DataMember dataMember = classContract.Members[index - num];
				Type memberType = dataMember.MemberType;
				if (dataMember.IsRequired)
				{
					int num2 = index + 1;
					while (num2 < requiredMembers.Length && !requiredMembers[num2])
					{
						num2++;
					}
					requiredIndex = num2;
				}
				if (dataMember.IsGetOnlyCollection)
				{
					object member = CodeInterpreter.GetMember(dataMember.MemberInfo, this.objectLocal);
					this.context.StoreCollectionMemberInfo(member);
					this.ReadValue(memberType, dataMember.Name, classContract.StableName.Namespace);
				}
				else
				{
					object obj = this.ReadValue(memberType, dataMember.Name, classContract.StableName.Namespace);
					CodeInterpreter.SetMember(dataMember.MemberInfo, this.objectLocal, obj);
				}
				memberIndex = index;
			}
			return num + classContract.Members.Count;
		}

		private bool[] GetRequiredMembers(ClassDataContract contract, out int firstRequiredMember)
		{
			int num = contract.MemberNames.Length;
			bool[] array = new bool[num];
			this.GetRequiredMembers(contract, array);
			firstRequiredMember = 0;
			while (firstRequiredMember < num && !array[firstRequiredMember])
			{
				firstRequiredMember++;
			}
			return array;
		}

		private int GetRequiredMembers(ClassDataContract contract, bool[] requiredMembers)
		{
			int num = ((contract.BaseContract == null) ? 0 : this.GetRequiredMembers(contract.BaseContract, requiredMembers));
			List<DataMember> members = contract.Members;
			int i = 0;
			while (i < members.Count)
			{
				requiredMembers[num] = members[i].IsRequired;
				i++;
				num++;
			}
			return num;
		}

		private object ReadValue(Type type, string name, string ns)
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
						obj = this.InternalDeserialize(type, name, ns);
					}
				}
				else
				{
					if (type.IsValueType)
					{
						throw new SerializationException(global::System.Runtime.Serialization.SR.GetString("ValueType '{0}' cannot have ref to another object.", new object[] { DataContract.GetClrTypeFullName(type) }));
					}
					obj = CodeInterpreter.ConvertValue(this.context.GetExistingObject(text, type, name, ns), Globals.TypeOfObject, type);
				}
				if (flag && text != null)
				{
					obj = this.WrapNullableObject(type, obj, type2, num);
				}
			}
			else
			{
				obj = this.InternalDeserialize(type, name, ns);
			}
			return obj;
		}

		private object InternalDeserialize(Type type, string name, string ns)
		{
			Type type2 = (type.IsPointer ? Globals.TypeOfReflectionPointer : type);
			object obj = this.context.InternalDeserialize(this.xmlReader, DataContract.GetId(type2.TypeHandle), type2.TypeHandle, name, ns);
			if (type.IsPointer)
			{
				return XmlFormatGeneratorStatics.UnboxPointer.Invoke(null, new object[] { obj });
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
					constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Globals.EmptyTypeArray, null);
					break;
				case CollectionKind.Dictionary:
					type = Globals.TypeOfHashtable;
					constructorInfo = XmlFormatGeneratorStatics.HashtableCtor;
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
			string text = collectionContract.ItemName;
			string @namespace = collectionContract.StableName.Namespace;
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
			int arraySize = this.context.GetArraySize();
			string objectId = this.context.GetObjectId();
			bool flag2 = false;
			bool flag3 = false;
			if (flag && this.TryReadPrimitiveArray(type, itemType, arraySize, out flag3))
			{
				flag2 = true;
			}
			if (!flag3)
			{
				if (arraySize == -1)
				{
					object obj = null;
					if (flag)
					{
						obj = Array.CreateInstance(itemType, 32);
					}
					int i;
					for (i = 0; i < 2147483647; i++)
					{
						if (this.IsStartElement(this.itemName, this.itemNamespace))
						{
							this.context.IncrementItemCount(1);
							object obj2 = this.ReadCollectionItem(collectionContract, itemType, text, @namespace);
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
					}
				}
				else
				{
					this.context.IncrementItemCount(arraySize);
					if (flag)
					{
						this.objectLocal = Array.CreateInstance(itemType, arraySize);
						this.context.AddNewObject(this.objectLocal);
					}
					for (int j = 0; j < arraySize; j++)
					{
						if (this.IsStartElement(this.itemName, this.itemNamespace))
						{
							object obj3 = this.ReadCollectionItem(collectionContract, itemType, text, @namespace);
							if (flag)
							{
								((Array)this.objectLocal).SetValue(obj3, j);
							}
							else
							{
								this.StoreCollectionValue(this.objectLocal, itemType, obj3, collectionContract);
							}
						}
						else
						{
							this.HandleUnexpectedItemInCollection(ref j);
						}
					}
					this.context.CheckEndOfArray(this.xmlReader, arraySize, this.itemName, this.itemNamespace);
				}
			}
			if (flag2)
			{
				this.context.AddNewObjectWithId(objectId, this.objectLocal);
			}
		}

		private void ReadGetOnlyCollection(CollectionDataContract collectionContract)
		{
			Type underlyingType = collectionContract.UnderlyingType;
			Type itemType = collectionContract.ItemType;
			bool flag = collectionContract.Kind == CollectionKind.Array;
			string text = collectionContract.ItemName;
			string @namespace = collectionContract.StableName.Namespace;
			this.objectLocal = this.context.GetCollectionMember();
			if (this.IsStartElement(this.itemName, this.itemNamespace))
			{
				if (this.objectLocal == null)
				{
					XmlObjectSerializerReadContext.ThrowNullValueReturnedForGetOnlyCollectionException(underlyingType);
					return;
				}
				int num = 0;
				if (flag)
				{
					num = ((Array)this.objectLocal).Length;
				}
				this.context.AddNewObject(this.objectLocal);
				int i = 0;
				while (i < 2147483647)
				{
					if (this.IsStartElement(this.itemName, this.itemNamespace))
					{
						this.context.IncrementItemCount(1);
						object obj = this.ReadCollectionItem(collectionContract, itemType, text, @namespace);
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
				this.context.CheckEndOfArray(this.xmlReader, num, this.itemName, this.itemNamespace);
			}
		}

		private bool TryReadPrimitiveArray(Type type, Type itemType, int size, out bool readResult)
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
					text = "TryReadDateTimeArray";
					break;
				}
			}
			else
			{
				text = "TryReadBooleanArray";
			}
			if (text != null)
			{
				MethodInfo method = typeof(XmlReaderDelegator).GetMethod(text, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				object[] array = new object[] { this.context, this.itemName, this.itemNamespace, size, this.objectLocal };
				readResult = (bool)method.Invoke(this.xmlReader, array);
				this.objectLocal = array.Last<object>();
				return true;
			}
			return false;
		}

		private object ReadCollectionItem(CollectionDataContract collectionContract, Type itemType, string itemName, string itemNs)
		{
			if (collectionContract.Kind == CollectionKind.Dictionary || collectionContract.Kind == CollectionKind.GenericDictionary)
			{
				this.context.ResetAttributes();
				return CodeInterpreter.ConvertValue(collectionContract.ItemContract.ReadXmlValue(this.xmlReader, this.context), Globals.TypeOfObject, itemType);
			}
			return this.ReadValue(itemType, itemName, itemNs);
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
				collectionContract.AddMethod.Invoke(collection, new object[] { member, member2 });
				return;
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

		private bool is_get_only_collection;

		private ClassDataContract classContract;

		private CollectionDataContract collectionContract;

		private object objectLocal;

		private Type objectType;

		private XmlReaderDelegator xmlReader;

		private XmlObjectSerializerReadContext context;

		private XmlDictionaryString[] memberNames;

		private XmlDictionaryString[] memberNamespaces;

		private XmlDictionaryString itemName;

		private XmlDictionaryString itemNamespace;
	}
}
