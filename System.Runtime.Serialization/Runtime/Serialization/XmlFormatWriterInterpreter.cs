using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class XmlFormatWriterInterpreter
	{
		public XmlFormatWriterInterpreter(ClassDataContract classContract)
		{
			this.classContract = classContract;
		}

		public XmlFormatWriterInterpreter(CollectionDataContract collectionContract)
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

		public void WriteToXml(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context, ClassDataContract dataContract)
		{
			this.writer = xmlWriter;
			this.obj = obj;
			this.ctx = context;
			this.dataContract = dataContract;
			this.InitArgs(this.classContract.UnderlyingType);
			if (this.classContract.IsReadOnlyContract)
			{
				DataContract.ThrowInvalidDataContractException(this.classContract.SerializationExceptionMessage, null);
			}
			this.WriteClass(this.classContract);
		}

		public void WriteCollectionToXml(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context, CollectionDataContract collectionContract)
		{
			this.writer = xmlWriter;
			this.obj = obj;
			this.ctx = context;
			this.dataContract = collectionContract;
			this.InitArgs(collectionContract.UnderlyingType);
			if (collectionContract.IsReadOnlyContract)
			{
				DataContract.ThrowInvalidDataContractException(collectionContract.SerializationExceptionMessage, null);
			}
			this.WriteCollection(collectionContract);
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

		private void InvokeOnSerializing(ClassDataContract classContract, object objSerialized, XmlObjectSerializerWriteContext ctx)
		{
			if (classContract.BaseContract != null)
			{
				this.InvokeOnSerializing(classContract.BaseContract, objSerialized, ctx);
			}
			if (classContract.OnSerializing != null)
			{
				classContract.OnSerializing.Invoke(objSerialized, new object[] { ctx.GetStreamingContext() });
			}
		}

		private void InvokeOnSerialized(ClassDataContract classContract, object objSerialized, XmlObjectSerializerWriteContext ctx)
		{
			if (classContract.BaseContract != null)
			{
				this.InvokeOnSerialized(classContract.BaseContract, objSerialized, ctx);
			}
			if (classContract.OnSerialized != null)
			{
				classContract.OnSerialized.Invoke(objSerialized, new object[] { ctx.GetStreamingContext() });
			}
		}

		private void WriteClass(ClassDataContract classContract)
		{
			this.InvokeOnSerializing(classContract, this.objLocal, this.ctx);
			if (classContract.IsISerializable)
			{
				this.ctx.WriteISerializable(this.writer, (ISerializable)this.objLocal);
			}
			else
			{
				if (classContract.ContractNamespaces.Length > 1)
				{
					this.contractNamespaces = this.classDataContract.ContractNamespaces;
				}
				this.memberNames = this.classDataContract.MemberNames;
				for (int i = 0; i < classContract.ChildElementNamespaces.Length; i++)
				{
					if (classContract.ChildElementNamespaces[i] != null)
					{
						this.childElementNamespaces = this.classDataContract.ChildElementNamespaces;
					}
				}
				if (classContract.HasExtensionData)
				{
					ExtensionDataObject extensionData = ((IExtensibleDataObject)this.objLocal).ExtensionData;
					this.ctx.WriteExtensionData(this.writer, extensionData, -1);
					this.WriteMembers(classContract, extensionData, classContract);
				}
				else
				{
					this.WriteMembers(classContract, null, classContract);
				}
			}
			this.InvokeOnSerialized(classContract, this.objLocal, this.ctx);
		}

		private void WriteCollection(CollectionDataContract collectionContract)
		{
			XmlDictionaryString @namespace = this.dataContract.Namespace;
			XmlDictionaryString collectionItemName = this.collectionDataContract.CollectionItemName;
			if (collectionContract.ChildElementNamespace != null)
			{
				this.writer.WriteNamespaceDecl(this.collectionDataContract.ChildElementNamespace);
			}
			if (collectionContract.Kind == CollectionKind.Array)
			{
				Type itemType = collectionContract.ItemType;
				if (this.objLocal.GetType().GetElementType() != itemType)
				{
					throw new InvalidCastException(string.Format("Cannot cast array of {0} to array of {1}", this.objLocal.GetType().GetElementType(), itemType));
				}
				this.ctx.IncrementArrayCount(this.writer, (Array)this.objLocal);
				if (!this.TryWritePrimitiveArray(collectionContract.UnderlyingType, itemType, () => this.objLocal, collectionItemName, @namespace))
				{
					Array array = (Array)this.objLocal;
					int[] array2 = new int[1];
					for (int i = 0; i < array.Length; i++)
					{
						if (!this.TryWritePrimitive(itemType, null, null, new int?(i), @namespace, collectionItemName, 0))
						{
							this.WriteStartElement(itemType, collectionContract.Namespace, @namespace, collectionItemName, 0);
							array2[0] = i;
							object value = array.GetValue(array2);
							this.WriteValue(itemType, value, false);
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
					methodInfo.Invoke(this.ctx, new object[] { this.writer, this.objLocal });
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
							methodInfo2 = XmlFormatGeneratorStatics.MoveNextMethod;
						}
						if (methodInfo3 == null)
						{
							methodInfo3 = XmlFormatGeneratorStatics.GetCurrentMethod;
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
					enumerator = new CollectionDataContract.DictionaryEnumerator((IDictionaryEnumerator)enumerator);
				}
				else if (flag2)
				{
					Type type4 = Globals.TypeOfIEnumeratorGeneric.MakeGenericType(new Type[] { Globals.TypeOfKeyValuePair.MakeGenericType(array3) });
					type.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { type4 }, null);
					enumerator = (IEnumerator)Activator.CreateInstance(type, new object[] { enumerator });
				}
				object[] array4 = new object[0];
				while (enumerator != null && enumerator.MoveNext())
				{
					currentValue = methodInfo3.Invoke(enumerator, array4);
					if (methodInfo == null)
					{
						XmlFormatGeneratorStatics.IncrementItemCountMethod.Invoke(this.ctx, new object[] { 1 });
					}
					if (!this.TryWritePrimitive(returnType, () => currentValue, null, null, @namespace, collectionItemName, 0))
					{
						this.WriteStartElement(returnType, collectionContract.Namespace, @namespace, collectionItemName, 0);
						if (flag2 || flag)
						{
							this.collectionDataContract.ItemContract.WriteXmlValue(this.writer, currentValue, this.ctx);
						}
						else
						{
							this.WriteValue(returnType, currentValue, false);
						}
						this.WriteEndElement();
					}
				}
			}
		}

		private int WriteMembers(ClassDataContract classContract, ExtensionDataObject extensionData, ClassDataContract derivedMostClassContract)
		{
			int num = ((classContract.BaseContract == null) ? 0 : this.WriteMembers(classContract.BaseContract, extensionData, derivedMostClassContract));
			XmlDictionaryString xmlDictionaryString = ((this.contractNamespaces == null) ? this.dataContract.Namespace : this.contractNamespaces[this.typeIndex - 1]);
			this.ctx.IncrementItemCount(classContract.Members.Count);
			int i = 0;
			while (i < classContract.Members.Count)
			{
				DataMember dataMember = classContract.Members[i];
				Type memberType = dataMember.MemberType;
				object memberValue = null;
				if (dataMember.IsGetOnlyCollection)
				{
					this.ctx.StoreIsGetOnlyCollection();
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
					bool flag3 = this.CheckIfMemberHasConflict(dataMember, classContract, derivedMostClassContract);
					if (flag3 || !this.TryWritePrimitive(memberType, flag2 ? (() => memberValue) : null, dataMember.MemberInfo, null, xmlDictionaryString, null, i + this.childElementIndex))
					{
						this.WriteStartElement(memberType, classContract.Namespace, xmlDictionaryString, null, i + this.childElementIndex);
						if (classContract.ChildElementNamespaces[i + this.childElementIndex] != null)
						{
							this.writer.WriteNamespaceDecl(this.childElementNamespaces[i + this.childElementIndex]);
						}
						if (memberValue == null)
						{
							memberValue = this.LoadMemberValue(dataMember);
						}
						this.WriteValue(memberType, memberValue, flag3);
						this.WriteEndElement();
					}
					if (classContract.HasExtensionData)
					{
						this.ctx.WriteExtensionData(this.writer, extensionData, num);
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

		private bool CheckIfMemberHasConflict(DataMember member, ClassDataContract classContract, ClassDataContract derivedMostClassContract)
		{
			if (this.CheckIfConflictingMembersHaveDifferentTypes(member))
			{
				return true;
			}
			string name = member.Name;
			string @namespace = classContract.StableName.Namespace;
			ClassDataContract classDataContract = derivedMostClassContract;
			while (classDataContract != null && classDataContract != classContract)
			{
				if (@namespace == classDataContract.StableName.Namespace)
				{
					List<DataMember> members = classDataContract.Members;
					for (int i = 0; i < members.Count; i++)
					{
						if (name == members[i].Name)
						{
							return this.CheckIfConflictingMembersHaveDifferentTypes(members[i]);
						}
					}
				}
				classDataContract = classDataContract.BaseContract;
			}
			return false;
		}

		private bool CheckIfConflictingMembersHaveDifferentTypes(DataMember member)
		{
			while (member.ConflictingMember != null)
			{
				if (member.MemberType != member.ConflictingMember.MemberType)
				{
					return true;
				}
				member = member.ConflictingMember;
			}
			return false;
		}

		private bool NeedsPrefix(Type type, XmlDictionaryString ns)
		{
			return type == Globals.TypeOfXmlQualifiedName && (ns != null && ns.Value != null) && ns.Value.Length > 0;
		}

		private void WriteStartElement(Type type, XmlDictionaryString ns, XmlDictionaryString namespaceLocal, XmlDictionaryString nameLocal, int nameIndex)
		{
			bool flag = this.NeedsPrefix(type, ns);
			nameLocal = nameLocal ?? this.memberNames[nameIndex];
			if (flag)
			{
				this.writer.WriteStartElement("q", nameLocal, namespaceLocal);
				return;
			}
			this.writer.WriteStartElement(nameLocal, namespaceLocal);
		}

		private void WriteEndElement()
		{
			this.writer.WriteEndElement();
		}

		private void WriteValue(Type memberType, object memberValue, bool writeXsiType)
		{
			if (memberType.IsPointer)
			{
				Pointer pointer = (Pointer)XmlFormatGeneratorStatics.BoxPointer.Invoke(null, new object[] { memberValue, memberType });
			}
			bool flag = memberType.IsGenericType && memberType.GetGenericTypeDefinition() == Globals.TypeOfNullable;
			if (memberType.IsValueType && !flag)
			{
				PrimitiveDataContract primitiveDataContract = PrimitiveDataContract.GetPrimitiveDataContract(memberType);
				if (primitiveDataContract != null && !writeXsiType)
				{
					primitiveDataContract.XmlFormatContentWriterMethod.Invoke(this.writer, new object[] { memberValue });
					return;
				}
				bool flag2 = Type.GetTypeHandle(memberValue).Equals(CodeInterpreter.ConvertValue(memberValue, memberType, Globals.TypeOfObject));
				this.ctx.InternalSerialize(this.writer, memberValue, flag2, writeXsiType, DataContract.GetId(memberType.TypeHandle), memberType.TypeHandle);
				return;
			}
			else
			{
				bool flag3;
				if (flag)
				{
					memberValue = this.UnwrapNullableObject(() => memberValue, ref memberType, out flag3);
				}
				else
				{
					flag3 = memberValue == null;
				}
				if (flag3)
				{
					XmlFormatGeneratorStatics.WriteNullMethod.Invoke(this.ctx, new object[]
					{
						this.writer,
						memberType,
						DataContract.IsTypeSerializable(memberType)
					});
					return;
				}
				PrimitiveDataContract primitiveDataContract2 = PrimitiveDataContract.GetPrimitiveDataContract(memberType);
				if (primitiveDataContract2 != null && primitiveDataContract2.UnderlyingType != Globals.TypeOfObject && !writeXsiType)
				{
					if (flag)
					{
						primitiveDataContract2.XmlFormatContentWriterMethod.Invoke(this.writer, new object[] { memberValue });
						return;
					}
					primitiveDataContract2.XmlFormatContentWriterMethod.Invoke(this.ctx, new object[] { this.writer, memberValue });
					return;
				}
				else
				{
					bool flag4 = false;
					if (memberType == Globals.TypeOfObject || memberType == Globals.TypeOfValueType || ((IList)Globals.TypeOfNullable.GetInterfaces()).Contains(memberType))
					{
						object obj = CodeInterpreter.ConvertValue(memberValue, memberType.GetType(), Globals.TypeOfObject);
						memberValue = obj;
						flag4 = memberValue == null;
					}
					if (flag4)
					{
						XmlFormatGeneratorStatics.WriteNullMethod.Invoke(this.ctx, new object[]
						{
							this.writer,
							memberType,
							DataContract.IsTypeSerializable(memberType)
						});
						return;
					}
					RuntimeTypeHandle typeHandle = Type.GetTypeHandle(memberValue);
					bool flag5 = typeHandle.Equals(CodeInterpreter.ConvertValue(memberValue, memberType, Globals.TypeOfObject));
					if (flag)
					{
						this.ctx.InternalSerialize(this.writer, memberValue, flag5, writeXsiType, DataContract.GetId(memberType.TypeHandle), memberType.TypeHandle);
						return;
					}
					if (memberType == Globals.TypeOfObject)
					{
						DataContract dataContract = DataContract.GetDataContract(memberValue.GetType());
						this.writer.WriteAttributeQualifiedName("i", DictionaryGlobals.XsiTypeLocalName, DictionaryGlobals.SchemaInstanceNamespace, dataContract.Name, dataContract.Namespace);
						this.ctx.InternalSerializeReference(this.writer, memberValue, false, false, -1, typeHandle);
						return;
					}
					this.ctx.InternalSerializeReference(this.writer, memberValue, flag5, writeXsiType, DataContract.GetId(memberType.TypeHandle), memberType.TypeHandle);
					return;
				}
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

		private bool TryWritePrimitive(Type type, Func<object> value, MemberInfo memberInfo, int? arrayItemIndex, XmlDictionaryString ns, XmlDictionaryString name, int nameIndex)
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
				obj = this.ctx;
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
			list.Add(ns);
			primitiveDataContract.XmlFormatWriterMethod.Invoke(obj, list.ToArray());
			return true;
		}

		private bool TryWritePrimitiveArray(Type type, Type itemType, Func<object> value, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
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
					text = "WriteInt32Array";
					break;
				case TypeCode.Int64:
					text = "WriteInt64Array";
					break;
				case TypeCode.Single:
					text = "WriteSingleArray";
					break;
				case TypeCode.Double:
					text = "WriteDoubleArray";
					break;
				case TypeCode.Decimal:
					text = "WriteDecimalArray";
					break;
				case TypeCode.DateTime:
					text = "WriteDateTimeArray";
					break;
				}
			}
			else
			{
				text = "WriteBooleanArray";
			}
			if (text != null)
			{
				typeof(XmlWriterDelegator).GetMethod(text, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
				{
					type,
					typeof(XmlDictionaryString),
					typeof(XmlDictionaryString)
				}, null).Invoke(this.writer, new object[]
				{
					value(),
					itemName,
					itemNamespace
				});
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

		private XmlObjectSerializerWriteContext ctx;

		private DataContract dataContract;

		private object objLocal;

		private XmlDictionaryString[] contractNamespaces;

		private XmlDictionaryString[] memberNames;

		private XmlDictionaryString[] childElementNamespaces;

		private int typeIndex = 1;

		private int childElementIndex;
	}
}
