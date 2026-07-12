using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonFormatWriterInterpreter
	{
		public JsonFormatWriterInterpreter(ClassDataContract classContract)
		{
			this.classContract = classContract;
		}

		public JsonFormatWriterInterpreter(CollectionDataContract collectionContract)
		{
			this.collectionContract = collectionContract;
		}

		private ClassDataContract classDataContract
		{
			get
			{
				return (ClassDataContract)this.dataContract;
			}
		}

		private CollectionDataContract collectionDataContract
		{
			get
			{
				return (CollectionDataContract)this.dataContract;
			}
		}

		public void WriteToJson(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, ClassDataContract dataContract, XmlDictionaryString[] memberNames)
		{
			this.writer = xmlWriter;
			this.obj = obj;
			this.context = context;
			this.dataContract = dataContract;
			this.memberNames = memberNames;
			this.InitArgs(this.classContract.UnderlyingType);
			if (this.classContract.IsReadOnlyContract)
			{
				DataContract.ThrowInvalidDataContractException(this.classContract.SerializationExceptionMessage, null);
			}
			this.WriteClass(this.classContract);
		}

		public void WriteCollectionToJson(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, CollectionDataContract dataContract)
		{
			this.writer = xmlWriter;
			this.obj = obj;
			this.context = context;
			this.dataContract = dataContract;
			this.InitArgs(this.collectionContract.UnderlyingType);
			if (this.collectionContract.IsReadOnlyContract)
			{
				DataContract.ThrowInvalidDataContractException(this.collectionContract.SerializationExceptionMessage, null);
			}
			this.WriteCollection(this.collectionContract);
		}

		private void InitArgs(Type objType)
		{
			if (objType == Globals.TypeOfDateTimeOffsetAdapter)
			{
				this.objLocal = DateTimeOffsetAdapter.GetDateTimeOffsetAdapter((DateTimeOffset)this.obj);
				return;
			}
			this.objLocal = CodeInterpreter.ConvertValue(this.obj, typeof(object), objType);
		}

		private void InvokeOnSerializing(ClassDataContract classContract, object objSerialized, XmlObjectSerializerWriteContext context)
		{
			if (classContract.BaseContract != null)
			{
				this.InvokeOnSerializing(classContract.BaseContract, objSerialized, context);
			}
			if (classContract.OnSerializing != null)
			{
				classContract.OnSerializing.Invoke(objSerialized, new object[] { context.GetStreamingContext() });
			}
		}

		private void InvokeOnSerialized(ClassDataContract classContract, object objSerialized, XmlObjectSerializerWriteContext context)
		{
			if (classContract.BaseContract != null)
			{
				this.InvokeOnSerialized(classContract.BaseContract, objSerialized, context);
			}
			if (classContract.OnSerialized != null)
			{
				classContract.OnSerialized.Invoke(objSerialized, new object[] { context.GetStreamingContext() });
			}
		}

		private void WriteClass(ClassDataContract classContract)
		{
			this.InvokeOnSerializing(classContract, this.objLocal, this.context);
			if (classContract.IsISerializable)
			{
				this.context.WriteJsonISerializable(this.writer, (ISerializable)this.objLocal);
			}
			else if (classContract.HasExtensionData)
			{
				ExtensionDataObject extensionData = ((IExtensibleDataObject)this.objLocal).ExtensionData;
				this.context.WriteExtensionData(this.writer, extensionData, -1);
				this.WriteMembers(classContract, extensionData, classContract);
			}
			else
			{
				this.WriteMembers(classContract, null, classContract);
			}
			this.InvokeOnSerialized(classContract, this.objLocal, this.context);
		}

		private void WriteCollection(CollectionDataContract collectionContract)
		{
			XmlDictionaryString collectionItemName = this.context.CollectionItemName;
			if (collectionContract.Kind == CollectionKind.Array)
			{
				Type itemType = collectionContract.ItemType;
				if (this.objLocal.GetType().GetElementType() != itemType)
				{
					throw new InvalidCastException(string.Format("Cannot cast array of {0} to array of {1}", this.objLocal.GetType().GetElementType(), itemType));
				}
				this.context.IncrementArrayCount(this.writer, (Array)this.objLocal);
				if (!this.TryWritePrimitiveArray(collectionContract.UnderlyingType, itemType, () => this.objLocal, collectionItemName))
				{
					this.WriteArrayAttribute();
					Array array = (Array)this.objLocal;
					int[] array2 = new int[1];
					for (int i = 0; i < array.Length; i++)
					{
						if (!this.TryWritePrimitive(itemType, null, null, new int?(i), collectionItemName, 0))
						{
							this.WriteStartElement(collectionItemName, 0);
							array2[0] = i;
							object value = array.GetValue(array2);
							this.WriteValue(itemType, value);
							this.WriteEndElement();
						}
					}
					return;
				}
			}
			else
			{
				if (!collectionContract.UnderlyingType.IsAssignableFrom(this.objLocal.GetType()))
				{
					throw new InvalidCastException(string.Format("Cannot cast {0} to {1}", this.objLocal.GetType(), collectionContract.UnderlyingType));
				}
				MethodInfo methodInfo = null;
				switch (collectionContract.Kind)
				{
				case CollectionKind.GenericDictionary:
					methodInfo = XmlFormatGeneratorStatics.IncrementCollectionCountGenericMethod.MakeGenericMethod(new Type[] { Globals.TypeOfKeyValuePair.MakeGenericType(collectionContract.ItemType.GetGenericArguments()) });
					break;
				case CollectionKind.Dictionary:
				case CollectionKind.List:
				case CollectionKind.Collection:
					methodInfo = XmlFormatGeneratorStatics.IncrementCollectionCountMethod;
					break;
				case CollectionKind.GenericList:
				case CollectionKind.GenericCollection:
					methodInfo = XmlFormatGeneratorStatics.IncrementCollectionCountGenericMethod.MakeGenericMethod(new Type[] { collectionContract.ItemType });
					break;
				}
				if (methodInfo != null)
				{
					methodInfo.Invoke(this.context, new object[] { this.writer, this.objLocal });
				}
				bool flag = false;
				bool flag2 = false;
				Type[] array3 = null;
				Type type;
				if (collectionContract.Kind == CollectionKind.GenericDictionary)
				{
					flag2 = true;
					array3 = collectionContract.ItemType.GetGenericArguments();
					type = Globals.TypeOfGenericDictionaryEnumerator.MakeGenericType(array3);
				}
				else if (collectionContract.Kind == CollectionKind.Dictionary)
				{
					flag = true;
					array3 = new Type[]
					{
						Globals.TypeOfObject,
						Globals.TypeOfObject
					};
					type = Globals.TypeOfDictionaryEnumerator;
				}
				else
				{
					type = collectionContract.GetEnumeratorMethod.ReturnType;
				}
				MethodInfo methodInfo2 = type.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Public, null, Globals.EmptyTypeArray, null);
				MethodInfo methodInfo3 = type.GetMethod("get_Current", BindingFlags.Instance | BindingFlags.Public, null, Globals.EmptyTypeArray, null);
				if (methodInfo2 == null || methodInfo3 == null)
				{
					if (type.IsInterface)
					{
						if (methodInfo2 == null)
						{
							methodInfo2 = JsonFormatGeneratorStatics.MoveNextMethod;
						}
						if (methodInfo3 == null)
						{
							methodInfo3 = JsonFormatGeneratorStatics.GetCurrentMethod;
						}
					}
					else
					{
						Type type2 = Globals.TypeOfIEnumerator;
						CollectionKind kind = collectionContract.Kind;
						if (kind == CollectionKind.GenericDictionary || kind == CollectionKind.GenericCollection || kind == CollectionKind.GenericEnumerable)
						{
							foreach (Type type3 in type.GetInterfaces())
							{
								if (type3.IsGenericType && type3.GetGenericTypeDefinition() == Globals.TypeOfIEnumeratorGeneric && type3.GetGenericArguments()[0] == collectionContract.ItemType)
								{
									type2 = type3;
									break;
								}
							}
						}
						if (methodInfo2 == null)
						{
							methodInfo2 = CollectionDataContract.GetTargetMethodWithName("MoveNext", type, type2);
						}
						if (methodInfo3 == null)
						{
							methodInfo3 = CollectionDataContract.GetTargetMethodWithName("get_Current", type, type2);
						}
					}
				}
				Type returnType = methodInfo3.ReturnType;
				object currentValue = null;
				IEnumerator enumerator = (IEnumerator)collectionContract.GetEnumeratorMethod.Invoke(this.objLocal, new object[0]);
				if (flag)
				{
					enumerator = (IEnumerator)type.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { Globals.TypeOfIDictionaryEnumerator }, null).Invoke(new object[] { enumerator });
				}
				else if (flag2)
				{
					Type type4 = Globals.TypeOfIEnumeratorGeneric.MakeGenericType(new Type[] { Globals.TypeOfKeyValuePair.MakeGenericType(array3) });
					type.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { type4 }, null);
					enumerator = (IEnumerator)Activator.CreateInstance(type, new object[] { enumerator });
				}
				bool flag3 = flag || flag2;
				bool flag4 = flag3 && this.context.UseSimpleDictionaryFormat;
				PropertyInfo propertyInfo = null;
				PropertyInfo propertyInfo2 = null;
				if (flag3)
				{
					Type type5 = Globals.TypeOfKeyValue.MakeGenericType(array3);
					propertyInfo = type5.GetProperty("Key");
					propertyInfo2 = type5.GetProperty("Value");
				}
				if (flag4)
				{
					this.WriteObjectAttribute();
					object[] array4 = new object[0];
					while ((bool)methodInfo2.Invoke(enumerator, array4))
					{
						currentValue = methodInfo3.Invoke(enumerator, array4);
						object member = CodeInterpreter.GetMember(propertyInfo, currentValue);
						object member2 = CodeInterpreter.GetMember(propertyInfo2, currentValue);
						this.WriteStartElement(member, 0);
						this.WriteValue(propertyInfo2.PropertyType, member2);
						this.WriteEndElement();
					}
					return;
				}
				this.WriteArrayAttribute();
				object[] array5 = new object[0];
				Func<object> <>9__1;
				while (enumerator != null && enumerator.MoveNext())
				{
					currentValue = methodInfo3.Invoke(enumerator, array5);
					if (methodInfo == null)
					{
						XmlFormatGeneratorStatics.IncrementItemCountMethod.Invoke(this.context, new object[] { 1 });
					}
					Type type6 = returnType;
					Func<object> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = () => currentValue);
					}
					if (!this.TryWritePrimitive(type6, func, null, null, collectionItemName, 0))
					{
						this.WriteStartElement(collectionItemName, 0);
						if (flag2 || flag)
						{
							DataContractJsonSerializer.WriteJsonValue(JsonDataContract.GetJsonDataContract(XmlObjectSerializerWriteContextComplexJson.GetRevisedItemContract(this.collectionDataContract.ItemContract)), this.writer, currentValue, this.context, currentValue.GetType().TypeHandle);
						}
						else
						{
							this.WriteValue(returnType, currentValue);
						}
						this.WriteEndElement();
					}
				}
			}
		}

		private int WriteMembers(ClassDataContract classContract, ExtensionDataObject extensionData, ClassDataContract derivedMostClassContract)
		{
			int num = ((classContract.BaseContract == null) ? 0 : this.WriteMembers(classContract.BaseContract, extensionData, derivedMostClassContract));
			this.context.IncrementItemCount(classContract.Members.Count);
			int i = 0;
			while (i < classContract.Members.Count)
			{
				DataMember dataMember = classContract.Members[i];
				Type memberType = dataMember.MemberType;
				object memberValue = null;
				if (dataMember.IsGetOnlyCollection)
				{
					this.context.StoreIsGetOnlyCollection();
				}
				bool flag = true;
				bool flag2 = false;
				if (!dataMember.EmitDefaultValue)
				{
					flag2 = true;
					memberValue = this.LoadMemberValue(dataMember);
					flag = !this.IsDefaultValue(memberType, memberValue);
				}
				if (flag)
				{
					bool flag3 = DataContractJsonSerializer.CheckIfXmlNameRequiresMapping(classContract.MemberNames[i]);
					if (flag3 || !this.TryWritePrimitive(memberType, flag2 ? (() => memberValue) : null, dataMember.MemberInfo, null, null, i + this.childElementIndex))
					{
						if (flag3)
						{
							XmlObjectSerializerWriteContextComplexJson.WriteJsonNameWithMapping(this.writer, this.memberNames, i + this.childElementIndex);
						}
						else
						{
							this.WriteStartElement(null, i + this.childElementIndex);
						}
						if (memberValue == null)
						{
							memberValue = this.LoadMemberValue(dataMember);
						}
						this.WriteValue(memberType, memberValue);
						this.WriteEndElement();
					}
					if (classContract.HasExtensionData)
					{
						this.context.WriteExtensionData(this.writer, extensionData, num);
					}
				}
				else if (!dataMember.EmitDefaultValue && dataMember.IsRequired)
				{
					XmlObjectSerializerWriteContext.ThrowRequiredMemberMustBeEmitted(dataMember.Name, classContract.UnderlyingType);
				}
				i++;
				num++;
			}
			this.typeIndex++;
			this.childElementIndex += classContract.Members.Count;
			return num;
		}

		internal bool IsDefaultValue(Type type, object value)
		{
			object defaultValue = this.GetDefaultValue(type);
			if (defaultValue != null)
			{
				return defaultValue.Equals(value);
			}
			return value == null;
		}

		internal object GetDefaultValue(Type type)
		{
			if (type.IsValueType)
			{
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
					return false;
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
					return 0;
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return 0L;
				case TypeCode.Single:
					return 0f;
				case TypeCode.Double:
					return 0.0;
				case TypeCode.Decimal:
					return 0m;
				case TypeCode.DateTime:
					return default(DateTime);
				}
			}
			return null;
		}

		private void WriteStartElement(object nameLocal, int nameIndex)
		{
			object obj = nameLocal ?? this.memberNames[nameIndex];
			if (nameLocal != null && nameLocal is string)
			{
				this.writer.WriteStartElement((string)obj, null);
				return;
			}
			if (obj is XmlDictionaryString)
			{
				this.writer.WriteStartElement((XmlDictionaryString)obj, null);
				return;
			}
			this.writer.WriteStartElement(obj.ToString(), null);
		}

		private void WriteEndElement()
		{
			this.writer.WriteEndElement();
		}

		private void WriteArrayAttribute()
		{
			this.writer.WriteAttributeString(null, "type", string.Empty, "array");
		}

		private void WriteObjectAttribute()
		{
			this.writer.WriteAttributeString(null, "type", null, "object");
		}

		private void WriteValue(Type memberType, object memberValue)
		{
			if (memberType.IsPointer)
			{
				Pointer pointer = (Pointer)JsonFormatGeneratorStatics.BoxPointer.Invoke(null, new object[] { memberValue, memberType });
			}
			bool flag = memberType.IsGenericType && memberType.GetGenericTypeDefinition() == Globals.TypeOfNullable;
			if (memberType.IsValueType && !flag)
			{
				PrimitiveDataContract primitiveDataContract = PrimitiveDataContract.GetPrimitiveDataContract(memberType);
				if (primitiveDataContract != null)
				{
					primitiveDataContract.XmlFormatContentWriterMethod.Invoke(this.writer, new object[] { memberValue });
					return;
				}
				this.InternalSerialize(XmlFormatGeneratorStatics.InternalSerializeMethod, () => memberValue, memberType, false);
				return;
			}
			else
			{
				bool flag2;
				if (flag)
				{
					memberValue = this.UnwrapNullableObject(() => memberValue, ref memberType, out flag2);
				}
				else
				{
					flag2 = memberValue == null;
				}
				if (flag2)
				{
					XmlFormatGeneratorStatics.WriteNullMethod.Invoke(this.context, new object[]
					{
						this.writer,
						memberType,
						DataContract.IsTypeSerializable(memberType)
					});
					return;
				}
				PrimitiveDataContract primitiveDataContract2 = PrimitiveDataContract.GetPrimitiveDataContract(memberType);
				if (primitiveDataContract2 != null && primitiveDataContract2.UnderlyingType != Globals.TypeOfObject)
				{
					if (flag)
					{
						primitiveDataContract2.XmlFormatContentWriterMethod.Invoke(this.writer, new object[] { memberValue });
						return;
					}
					primitiveDataContract2.XmlFormatContentWriterMethod.Invoke(this.context, new object[] { this.writer, memberValue });
					return;
				}
				else
				{
					bool flag3 = false;
					if (memberType == Globals.TypeOfObject || memberType == Globals.TypeOfValueType || ((IList)Globals.TypeOfNullable.GetInterfaces()).Contains(memberType))
					{
						object obj = CodeInterpreter.ConvertValue(memberValue, memberType.GetType(), Globals.TypeOfObject);
						memberValue = obj;
						flag3 = memberValue == null;
					}
					if (flag3)
					{
						XmlFormatGeneratorStatics.WriteNullMethod.Invoke(this.context, new object[]
						{
							this.writer,
							memberType,
							DataContract.IsTypeSerializable(memberType)
						});
						return;
					}
					this.InternalSerialize(flag ? XmlFormatGeneratorStatics.InternalSerializeMethod : XmlFormatGeneratorStatics.InternalSerializeReferenceMethod, () => memberValue, memberType, false);
					return;
				}
			}
		}

		private void InternalSerialize(MethodInfo methodInfo, Func<object> memberValue, Type memberType, bool writeXsiType)
		{
			object obj = memberValue();
			bool flag = Type.GetTypeHandle(obj).Equals(CodeInterpreter.ConvertValue(obj, memberType, Globals.TypeOfObject));
			try
			{
				methodInfo.Invoke(this.context, new object[]
				{
					this.writer,
					(memberValue != null) ? obj : null,
					flag,
					writeXsiType,
					DataContract.GetId(memberType.TypeHandle),
					memberType.TypeHandle
				});
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

		private object UnwrapNullableObject(Func<object> memberValue, ref Type memberType, out bool isNull)
		{
			object obj = memberValue();
			isNull = false;
			while (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == Globals.TypeOfNullable)
			{
				Type type = memberType.GetGenericArguments()[0];
				if ((bool)XmlFormatGeneratorStatics.GetHasValueMethod.MakeGenericMethod(new Type[] { type }).Invoke(null, new object[] { obj }))
				{
					obj = XmlFormatGeneratorStatics.GetNullableValueMethod.MakeGenericMethod(new Type[] { type }).Invoke(null, new object[] { obj });
				}
				else
				{
					isNull = true;
					obj = XmlFormatGeneratorStatics.GetDefaultValueMethod.MakeGenericMethod(new Type[] { memberType }).Invoke(null, new object[0]);
				}
				memberType = type;
			}
			return obj;
		}

		private bool TryWritePrimitive(Type type, Func<object> value, MemberInfo memberInfo, int? arrayItemIndex, XmlDictionaryString name, int nameIndex)
		{
			PrimitiveDataContract primitiveDataContract = PrimitiveDataContract.GetPrimitiveDataContract(type);
			if (primitiveDataContract == null || primitiveDataContract.UnderlyingType == Globals.TypeOfObject)
			{
				return false;
			}
			List<object> list = new List<object>();
			object obj;
			if (type.IsValueType)
			{
				obj = this.writer;
			}
			else
			{
				obj = this.context;
				list.Add(this.writer);
			}
			if (value != null)
			{
				list.Add(value());
			}
			else if (memberInfo != null)
			{
				list.Add(CodeInterpreter.GetMember(memberInfo, this.objLocal));
			}
			else
			{
				list.Add(((Array)this.objLocal).GetValue(new int[] { arrayItemIndex.Value }));
			}
			if (name != null)
			{
				list.Add(name);
			}
			else
			{
				list.Add(this.memberNames[nameIndex]);
			}
			list.Add(null);
			primitiveDataContract.XmlFormatWriterMethod.Invoke(obj, list.ToArray());
			return true;
		}

		private bool TryWritePrimitiveArray(Type type, Type itemType, Func<object> value, XmlDictionaryString itemName)
		{
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
					text = "WriteJsonInt32Array";
					break;
				case TypeCode.Int64:
					text = "WriteJsonInt64Array";
					break;
				case TypeCode.Single:
					text = "WriteJsonSingleArray";
					break;
				case TypeCode.Double:
					text = "WriteJsonDoubleArray";
					break;
				case TypeCode.Decimal:
					text = "WriteJsonDecimalArray";
					break;
				case TypeCode.DateTime:
					text = "WriteJsonDateTimeArray";
					break;
				}
			}
			else
			{
				text = "WriteJsonBooleanArray";
			}
			if (text != null)
			{
				this.WriteArrayAttribute();
				MethodBase method = typeof(JsonWriterDelegator).GetMethod(text, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
				{
					type,
					typeof(XmlDictionaryString),
					typeof(XmlDictionaryString)
				}, null);
				object obj = this.writer;
				object[] array = new object[3];
				array[0] = value();
				array[1] = itemName;
				method.Invoke(obj, array);
				return true;
			}
			return false;
		}

		private object LoadMemberValue(DataMember member)
		{
			return CodeInterpreter.GetMember(member.MemberInfo, this.objLocal);
		}

		private ClassDataContract classContract;

		private CollectionDataContract collectionContract;

		private XmlWriterDelegator writer;

		private object obj;

		private XmlObjectSerializerWriteContextComplexJson context;

		private DataContract dataContract;

		private object objLocal;

		private XmlDictionaryString[] memberNames;

		private int typeIndex = 1;

		private int childElementIndex;
	}
}
