using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal sealed class CollectionDataContract : DataContract
	{
		[SecuritySafeCritical]
		internal CollectionDataContract(CollectionKind kind)
			: base(new CollectionDataContract.CollectionDataContractCriticalHelper(kind))
		{
			this.InitCollectionDataContract(this);
		}

		[SecuritySafeCritical]
		internal CollectionDataContract(Type type)
			: base(new CollectionDataContract.CollectionDataContractCriticalHelper(type))
		{
			this.InitCollectionDataContract(this);
		}

		[SecuritySafeCritical]
		internal CollectionDataContract(Type type, DataContract itemContract)
			: base(new CollectionDataContract.CollectionDataContractCriticalHelper(type, itemContract))
		{
			this.InitCollectionDataContract(this);
		}

		[SecuritySafeCritical]
		private CollectionDataContract(Type type, CollectionKind kind, Type itemType, MethodInfo getEnumeratorMethod, string serializationExceptionMessage, string deserializationExceptionMessage)
			: base(new CollectionDataContract.CollectionDataContractCriticalHelper(type, kind, itemType, getEnumeratorMethod, serializationExceptionMessage, deserializationExceptionMessage))
		{
			this.InitCollectionDataContract(this.GetSharedTypeContract(type));
		}

		[SecuritySafeCritical]
		private CollectionDataContract(Type type, CollectionKind kind, Type itemType, MethodInfo getEnumeratorMethod, MethodInfo addMethod, ConstructorInfo constructor)
			: base(new CollectionDataContract.CollectionDataContractCriticalHelper(type, kind, itemType, getEnumeratorMethod, addMethod, constructor))
		{
			this.InitCollectionDataContract(this.GetSharedTypeContract(type));
		}

		[SecuritySafeCritical]
		private CollectionDataContract(Type type, CollectionKind kind, Type itemType, MethodInfo getEnumeratorMethod, MethodInfo addMethod, ConstructorInfo constructor, bool isConstructorCheckRequired)
			: base(new CollectionDataContract.CollectionDataContractCriticalHelper(type, kind, itemType, getEnumeratorMethod, addMethod, constructor, isConstructorCheckRequired))
		{
			this.InitCollectionDataContract(this.GetSharedTypeContract(type));
		}

		[SecuritySafeCritical]
		private CollectionDataContract(Type type, string invalidCollectionInSharedContractMessage)
			: base(new CollectionDataContract.CollectionDataContractCriticalHelper(type, invalidCollectionInSharedContractMessage))
		{
			this.InitCollectionDataContract(this.GetSharedTypeContract(type));
		}

		[SecurityCritical]
		private void InitCollectionDataContract(DataContract sharedTypeContract)
		{
			this.helper = base.Helper as CollectionDataContract.CollectionDataContractCriticalHelper;
			this.collectionItemName = this.helper.CollectionItemName;
			if (this.helper.Kind == CollectionKind.Dictionary || this.helper.Kind == CollectionKind.GenericDictionary)
			{
				this.itemContract = this.helper.ItemContract;
			}
			this.helper.SharedTypeContract = sharedTypeContract;
		}

		private void InitSharedTypeContract()
		{
		}

		private static Type[] KnownInterfaces
		{
			[SecuritySafeCritical]
			get
			{
				return CollectionDataContract.CollectionDataContractCriticalHelper.KnownInterfaces;
			}
		}

		internal CollectionKind Kind
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.Kind;
			}
		}

		internal Type ItemType
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.ItemType;
			}
		}

		public DataContract ItemContract
		{
			[SecuritySafeCritical]
			get
			{
				return this.itemContract ?? this.helper.ItemContract;
			}
			[SecurityCritical]
			set
			{
				this.itemContract = value;
				this.helper.ItemContract = value;
			}
		}

		internal DataContract SharedTypeContract
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.SharedTypeContract;
			}
		}

		internal string ItemName
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.ItemName;
			}
			[SecurityCritical]
			set
			{
				this.helper.ItemName = value;
			}
		}

		public XmlDictionaryString CollectionItemName
		{
			[SecuritySafeCritical]
			get
			{
				return this.collectionItemName;
			}
		}

		internal string KeyName
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.KeyName;
			}
			[SecurityCritical]
			set
			{
				this.helper.KeyName = value;
			}
		}

		internal string ValueName
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.ValueName;
			}
			[SecurityCritical]
			set
			{
				this.helper.ValueName = value;
			}
		}

		internal bool IsDictionary
		{
			get
			{
				return this.KeyName != null;
			}
		}

		public XmlDictionaryString ChildElementNamespace
		{
			[SecuritySafeCritical]
			get
			{
				if (this.childElementNamespace == null)
				{
					lock (this)
					{
						if (this.childElementNamespace == null)
						{
							if (this.helper.ChildElementNamespace == null && !this.IsDictionary)
							{
								XmlDictionaryString childNamespaceToDeclare = ClassDataContract.GetChildNamespaceToDeclare(this, this.ItemType, new XmlDictionary());
								Thread.MemoryBarrier();
								this.helper.ChildElementNamespace = childNamespaceToDeclare;
							}
							this.childElementNamespace = this.helper.ChildElementNamespace;
						}
					}
				}
				return this.childElementNamespace;
			}
		}

		internal bool IsItemTypeNullable
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsItemTypeNullable;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsItemTypeNullable = value;
			}
		}

		internal bool IsConstructorCheckRequired
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsConstructorCheckRequired;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsConstructorCheckRequired = value;
			}
		}

		internal MethodInfo GetEnumeratorMethod
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.GetEnumeratorMethod;
			}
		}

		internal MethodInfo AddMethod
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.AddMethod;
			}
		}

		internal ConstructorInfo Constructor
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.Constructor;
			}
		}

		internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.KnownDataContracts;
			}
			[SecurityCritical]
			set
			{
				this.helper.KnownDataContracts = value;
			}
		}

		internal string InvalidCollectionInSharedContractMessage
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.InvalidCollectionInSharedContractMessage;
			}
		}

		internal string SerializationExceptionMessage
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.SerializationExceptionMessage;
			}
		}

		internal string DeserializationExceptionMessage
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.DeserializationExceptionMessage;
			}
		}

		internal bool IsReadOnlyContract
		{
			get
			{
				return this.DeserializationExceptionMessage != null;
			}
		}

		private bool ItemNameSetExplicit
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.ItemNameSetExplicit;
			}
		}

		internal XmlFormatCollectionWriterDelegate XmlFormatWriterDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (this.helper.XmlFormatWriterDelegate == null)
				{
					lock (this)
					{
						if (this.helper.XmlFormatWriterDelegate == null)
						{
							XmlFormatCollectionWriterDelegate xmlFormatCollectionWriterDelegate = new XmlFormatWriterGenerator().GenerateCollectionWriter(this);
							Thread.MemoryBarrier();
							this.helper.XmlFormatWriterDelegate = xmlFormatCollectionWriterDelegate;
						}
					}
				}
				return this.helper.XmlFormatWriterDelegate;
			}
		}

		internal XmlFormatCollectionReaderDelegate XmlFormatReaderDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (this.helper.XmlFormatReaderDelegate == null)
				{
					lock (this)
					{
						if (this.helper.XmlFormatReaderDelegate == null)
						{
							if (this.IsReadOnlyContract)
							{
								DataContract.ThrowInvalidDataContractException(this.helper.DeserializationExceptionMessage, null);
							}
							XmlFormatCollectionReaderDelegate xmlFormatCollectionReaderDelegate = new XmlFormatReaderGenerator().GenerateCollectionReader(this);
							Thread.MemoryBarrier();
							this.helper.XmlFormatReaderDelegate = xmlFormatCollectionReaderDelegate;
						}
					}
				}
				return this.helper.XmlFormatReaderDelegate;
			}
		}

		internal XmlFormatGetOnlyCollectionReaderDelegate XmlFormatGetOnlyCollectionReaderDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (this.helper.XmlFormatGetOnlyCollectionReaderDelegate == null)
				{
					lock (this)
					{
						if (this.helper.XmlFormatGetOnlyCollectionReaderDelegate == null)
						{
							if (base.UnderlyingType.IsInterface && (this.Kind == CollectionKind.Enumerable || this.Kind == CollectionKind.Collection || this.Kind == CollectionKind.GenericEnumerable))
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("On type '{0}', get-only collection must have an Add method.", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) })));
							}
							if (this.IsReadOnlyContract)
							{
								DataContract.ThrowInvalidDataContractException(this.helper.DeserializationExceptionMessage, null);
							}
							XmlFormatGetOnlyCollectionReaderDelegate xmlFormatGetOnlyCollectionReaderDelegate = new XmlFormatReaderGenerator().GenerateGetOnlyCollectionReader(this);
							Thread.MemoryBarrier();
							this.helper.XmlFormatGetOnlyCollectionReaderDelegate = xmlFormatGetOnlyCollectionReaderDelegate;
						}
					}
				}
				return this.helper.XmlFormatGetOnlyCollectionReaderDelegate;
			}
		}

		private DataContract GetSharedTypeContract(Type type)
		{
			if (type.IsDefined(Globals.TypeOfCollectionDataContractAttribute, false))
			{
				return this;
			}
			if (type.IsSerializable || type.IsDefined(Globals.TypeOfDataContractAttribute, false))
			{
				return new ClassDataContract(type);
			}
			return null;
		}

		internal static bool IsCollectionInterface(Type type)
		{
			if (type.IsGenericType)
			{
				type = type.GetGenericTypeDefinition();
			}
			return ((ICollection<Type>)CollectionDataContract.KnownInterfaces).Contains(type);
		}

		internal static bool IsCollection(Type type)
		{
			Type type2;
			return CollectionDataContract.IsCollection(type, out type2);
		}

		internal static bool IsCollection(Type type, out Type itemType)
		{
			return CollectionDataContract.IsCollectionHelper(type, out itemType, true, false);
		}

		internal static bool IsCollection(Type type, bool constructorRequired, bool skipIfReadOnlyContract)
		{
			Type type2;
			return CollectionDataContract.IsCollectionHelper(type, out type2, constructorRequired, skipIfReadOnlyContract);
		}

		private static bool IsCollectionHelper(Type type, out Type itemType, bool constructorRequired, bool skipIfReadOnlyContract = false)
		{
			if (type.IsArray && DataContract.GetBuiltInDataContract(type) == null)
			{
				itemType = type.GetElementType();
				return true;
			}
			DataContract dataContract;
			return CollectionDataContract.IsCollectionOrTryCreate(type, false, out dataContract, out itemType, constructorRequired, skipIfReadOnlyContract);
		}

		internal static bool TryCreate(Type type, out DataContract dataContract)
		{
			Type type2;
			return CollectionDataContract.IsCollectionOrTryCreate(type, true, out dataContract, out type2, true, false);
		}

		internal static bool TryCreateGetOnlyCollectionDataContract(Type type, out DataContract dataContract)
		{
			if (type.IsArray)
			{
				dataContract = new CollectionDataContract(type);
				return true;
			}
			Type type2;
			return CollectionDataContract.IsCollectionOrTryCreate(type, true, out dataContract, out type2, false, false);
		}

		internal static MethodInfo GetTargetMethodWithName(string name, Type type, Type interfaceType)
		{
			InterfaceMapping interfaceMap = type.GetInterfaceMap(interfaceType);
			for (int i = 0; i < interfaceMap.TargetMethods.Length; i++)
			{
				if (interfaceMap.InterfaceMethods[i].Name == name)
				{
					return interfaceMap.InterfaceMethods[i];
				}
			}
			return null;
		}

		private static bool IsArraySegment(Type t)
		{
			return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ArraySegment<>);
		}

		private static bool IsCollectionOrTryCreate(Type type, bool tryCreate, out DataContract dataContract, out Type itemType, bool constructorRequired, bool skipIfReadOnlyContract = false)
		{
			dataContract = null;
			itemType = Globals.TypeOfObject;
			if (DataContract.GetBuiltInDataContract(type) != null)
			{
				return CollectionDataContract.HandleIfInvalidCollection(type, tryCreate, false, false, "{0} is a built-in type and cannot be a collection.", null, ref dataContract);
			}
			bool flag = CollectionDataContract.IsCollectionDataContract(type);
			bool flag2 = false;
			string text = null;
			string text2 = null;
			Type baseType = type.BaseType;
			bool flag3 = baseType != null && baseType != Globals.TypeOfObject && baseType != Globals.TypeOfValueType && baseType != Globals.TypeOfUri && CollectionDataContract.IsCollection(baseType) && !type.IsSerializable;
			if (type.IsDefined(Globals.TypeOfDataContractAttribute, false))
			{
				return CollectionDataContract.HandleIfInvalidCollection(type, tryCreate, flag, flag3, "{0} has DataContractAttribute attribute.", null, ref dataContract);
			}
			if (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type) || CollectionDataContract.IsArraySegment(type))
			{
				return false;
			}
			if (!Globals.TypeOfIEnumerable.IsAssignableFrom(type))
			{
				return CollectionDataContract.HandleIfInvalidCollection(type, tryCreate, flag, flag3, "{0} does not implement IEnumerable interface.", null, ref dataContract);
			}
			if (type.IsInterface)
			{
				Type type2 = (type.IsGenericType ? type.GetGenericTypeDefinition() : type);
				Type[] knownInterfaces = CollectionDataContract.KnownInterfaces;
				for (int i = 0; i < knownInterfaces.Length; i++)
				{
					if (knownInterfaces[i] == type2)
					{
						MethodInfo methodInfo = null;
						MethodInfo methodInfo2;
						if (type.IsGenericType)
						{
							Type[] genericArguments = type.GetGenericArguments();
							if (type2 == Globals.TypeOfIDictionaryGeneric)
							{
								itemType = Globals.TypeOfKeyValue.MakeGenericType(genericArguments);
								methodInfo = type.GetMethod("Add");
								methodInfo2 = Globals.TypeOfIEnumerableGeneric.MakeGenericType(new Type[] { Globals.TypeOfKeyValuePair.MakeGenericType(genericArguments) }).GetMethod("GetEnumerator");
							}
							else
							{
								itemType = genericArguments[0];
								if (type2 == Globals.TypeOfICollectionGeneric || type2 == Globals.TypeOfIListGeneric)
								{
									methodInfo = Globals.TypeOfICollectionGeneric.MakeGenericType(new Type[] { itemType }).GetMethod("Add");
								}
								methodInfo2 = Globals.TypeOfIEnumerableGeneric.MakeGenericType(new Type[] { itemType }).GetMethod("GetEnumerator");
							}
						}
						else
						{
							if (type2 == Globals.TypeOfIDictionary)
							{
								itemType = typeof(KeyValue<object, object>);
								methodInfo = type.GetMethod("Add");
							}
							else
							{
								itemType = Globals.TypeOfObject;
								if (type2 == Globals.TypeOfIList)
								{
									methodInfo = Globals.TypeOfIList.GetMethod("Add");
								}
							}
							methodInfo2 = Globals.TypeOfIEnumerable.GetMethod("GetEnumerator");
						}
						if (tryCreate)
						{
							dataContract = new CollectionDataContract(type, (CollectionKind)(i + 1), itemType, methodInfo2, methodInfo, null);
						}
						return true;
					}
				}
			}
			ConstructorInfo constructorInfo = null;
			if (!type.IsValueType)
			{
				constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null);
				if (constructorInfo == null && constructorRequired)
				{
					if (type.IsSerializable)
					{
						return CollectionDataContract.HandleIfInvalidCollection(type, tryCreate, flag, flag3, "{0} does not have a default constructor.", null, ref dataContract);
					}
					flag2 = true;
					CollectionDataContract.GetReadOnlyCollectionExceptionMessages(type, flag, "{0} does not have a default constructor.", null, out text, out text2);
				}
			}
			Type type3 = null;
			CollectionKind collectionKind = CollectionKind.None;
			bool flag4 = false;
			foreach (Type type4 in type.GetInterfaces())
			{
				Type type5 = (type4.IsGenericType ? type4.GetGenericTypeDefinition() : type4);
				Type[] knownInterfaces2 = CollectionDataContract.KnownInterfaces;
				int k = 0;
				while (k < knownInterfaces2.Length)
				{
					if (knownInterfaces2[k] == type5)
					{
						CollectionKind collectionKind2 = (CollectionKind)(k + 1);
						if (collectionKind == CollectionKind.None || collectionKind2 < collectionKind)
						{
							collectionKind = collectionKind2;
							type3 = type4;
							flag4 = false;
							break;
						}
						if ((collectionKind & collectionKind2) == collectionKind2)
						{
							flag4 = true;
							break;
						}
						break;
					}
					else
					{
						k++;
					}
				}
			}
			if (collectionKind == CollectionKind.None)
			{
				return CollectionDataContract.HandleIfInvalidCollection(type, tryCreate, flag, flag3, "{0} does not implement IEnumerable interface.", null, ref dataContract);
			}
			if (collectionKind == CollectionKind.Enumerable || collectionKind == CollectionKind.Collection || collectionKind == CollectionKind.GenericEnumerable)
			{
				if (flag4)
				{
					type3 = Globals.TypeOfIEnumerable;
				}
				itemType = (type3.IsGenericType ? type3.GetGenericArguments()[0] : Globals.TypeOfObject);
				MethodInfo methodInfo;
				MethodInfo methodInfo2;
				CollectionDataContract.GetCollectionMethods(type, type3, new Type[] { itemType }, false, out methodInfo2, out methodInfo);
				if (methodInfo == null)
				{
					if (type.IsSerializable || skipIfReadOnlyContract)
					{
						return CollectionDataContract.HandleIfInvalidCollection(type, tryCreate, flag, flag3 && !skipIfReadOnlyContract, "{0} does not have a valid Add method with parameter of type '{1}'.", DataContract.GetClrTypeFullName(itemType), ref dataContract);
					}
					flag2 = true;
					CollectionDataContract.GetReadOnlyCollectionExceptionMessages(type, flag, "{0} does not have a valid Add method with parameter of type '{1}'.", DataContract.GetClrTypeFullName(itemType), out text, out text2);
				}
				if (tryCreate)
				{
					dataContract = (flag2 ? new CollectionDataContract(type, collectionKind, itemType, methodInfo2, text, text2) : new CollectionDataContract(type, collectionKind, itemType, methodInfo2, methodInfo, constructorInfo, !constructorRequired));
				}
			}
			else
			{
				if (flag4)
				{
					return CollectionDataContract.HandleIfInvalidCollection(type, tryCreate, flag, flag3, "{0} has multiple definitions of interface '{1}'.", CollectionDataContract.KnownInterfaces[(int)(collectionKind - CollectionKind.GenericDictionary)].Name, ref dataContract);
				}
				Type[] array = null;
				switch (collectionKind)
				{
				case CollectionKind.GenericDictionary:
					array = type3.GetGenericArguments();
					itemType = ((type3.IsGenericTypeDefinition || (array[0].IsGenericParameter && array[1].IsGenericParameter)) ? Globals.TypeOfKeyValue : Globals.TypeOfKeyValue.MakeGenericType(array));
					break;
				case CollectionKind.Dictionary:
					array = new Type[]
					{
						Globals.TypeOfObject,
						Globals.TypeOfObject
					};
					itemType = Globals.TypeOfKeyValue.MakeGenericType(array);
					break;
				case CollectionKind.GenericList:
				case CollectionKind.GenericCollection:
					array = type3.GetGenericArguments();
					itemType = array[0];
					break;
				case CollectionKind.List:
					itemType = Globals.TypeOfObject;
					array = new Type[] { itemType };
					break;
				}
				if (tryCreate)
				{
					MethodInfo methodInfo;
					MethodInfo methodInfo2;
					CollectionDataContract.GetCollectionMethods(type, type3, array, true, out methodInfo2, out methodInfo);
					dataContract = (flag2 ? new CollectionDataContract(type, collectionKind, itemType, methodInfo2, text, text2) : new CollectionDataContract(type, collectionKind, itemType, methodInfo2, methodInfo, constructorInfo, !constructorRequired));
				}
			}
			return !flag2 || !skipIfReadOnlyContract;
		}

		internal static bool IsCollectionDataContract(Type type)
		{
			return type.IsDefined(Globals.TypeOfCollectionDataContractAttribute, false);
		}

		private static bool HandleIfInvalidCollection(Type type, bool tryCreate, bool hasCollectionDataContract, bool createContractWithException, string message, string param, ref DataContract dataContract)
		{
			if (hasCollectionDataContract)
			{
				if (tryCreate)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(CollectionDataContract.GetInvalidCollectionMessage(message, global::System.Runtime.Serialization.SR.GetString("Type '{0}' with CollectionDataContractAttribute attribute is an invalid collection type since it", new object[] { DataContract.GetClrTypeFullName(type) }), param)));
				}
				return true;
			}
			else
			{
				if (createContractWithException)
				{
					if (tryCreate)
					{
						dataContract = new CollectionDataContract(type, CollectionDataContract.GetInvalidCollectionMessage(message, global::System.Runtime.Serialization.SR.GetString("Type '{0}' is an invalid collection type since it", new object[] { DataContract.GetClrTypeFullName(type) }), param));
					}
					return true;
				}
				return false;
			}
		}

		private static void GetReadOnlyCollectionExceptionMessages(Type type, bool hasCollectionDataContract, string message, string param, out string serializationExceptionMessage, out string deserializationExceptionMessage)
		{
			serializationExceptionMessage = CollectionDataContract.GetInvalidCollectionMessage(message, global::System.Runtime.Serialization.SR.GetString(hasCollectionDataContract ? "Type '{0}' with CollectionDataContractAttribute attribute is an invalid collection type since it" : "Type '{0}' is an invalid collection type since it", new object[] { DataContract.GetClrTypeFullName(type) }), param);
			deserializationExceptionMessage = CollectionDataContract.GetInvalidCollectionMessage(message, global::System.Runtime.Serialization.SR.GetString("Error on deserializing read-only collection: {0}", new object[] { DataContract.GetClrTypeFullName(type) }), param);
		}

		private static string GetInvalidCollectionMessage(string message, string nestedMessage, string param)
		{
			if (param != null)
			{
				return global::System.Runtime.Serialization.SR.GetString(message, new object[] { nestedMessage, param });
			}
			return global::System.Runtime.Serialization.SR.GetString(message, new object[] { nestedMessage });
		}

		private static void FindCollectionMethodsOnInterface(Type type, Type interfaceType, ref MethodInfo addMethod, ref MethodInfo getEnumeratorMethod)
		{
			InterfaceMapping interfaceMap = type.GetInterfaceMap(interfaceType);
			for (int i = 0; i < interfaceMap.TargetMethods.Length; i++)
			{
				if (interfaceMap.InterfaceMethods[i].Name == "Add")
				{
					addMethod = interfaceMap.InterfaceMethods[i];
				}
				else if (interfaceMap.InterfaceMethods[i].Name == "GetEnumerator")
				{
					getEnumeratorMethod = interfaceMap.InterfaceMethods[i];
				}
			}
		}

		private static void GetCollectionMethods(Type type, Type interfaceType, Type[] addMethodTypeArray, bool addMethodOnInterface, out MethodInfo getEnumeratorMethod, out MethodInfo addMethod)
		{
			MethodInfo methodInfo;
			getEnumeratorMethod = (methodInfo = null);
			addMethod = methodInfo;
			if (addMethodOnInterface)
			{
				addMethod = type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, null, addMethodTypeArray, null);
				if (addMethod == null || addMethod.GetParameters()[0].ParameterType != addMethodTypeArray[0])
				{
					CollectionDataContract.FindCollectionMethodsOnInterface(type, interfaceType, ref addMethod, ref getEnumeratorMethod);
					if (addMethod == null)
					{
						foreach (Type type2 in interfaceType.GetInterfaces())
						{
							if (CollectionDataContract.IsKnownInterface(type2))
							{
								CollectionDataContract.FindCollectionMethodsOnInterface(type, type2, ref addMethod, ref getEnumeratorMethod);
								if (addMethod == null)
								{
									break;
								}
							}
						}
					}
				}
			}
			else
			{
				addMethod = type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, addMethodTypeArray, null);
			}
			if (getEnumeratorMethod == null)
			{
				getEnumeratorMethod = type.GetMethod("GetEnumerator", BindingFlags.Instance | BindingFlags.Public, null, Globals.EmptyTypeArray, null);
				if (getEnumeratorMethod == null || !Globals.TypeOfIEnumerator.IsAssignableFrom(getEnumeratorMethod.ReturnType))
				{
					Type type3 = interfaceType.GetInterface("System.Collections.Generic.IEnumerable*");
					if (type3 == null)
					{
						type3 = Globals.TypeOfIEnumerable;
					}
					getEnumeratorMethod = CollectionDataContract.GetTargetMethodWithName("GetEnumerator", type, type3);
				}
			}
		}

		private static bool IsKnownInterface(Type type)
		{
			Type type2 = (type.IsGenericType ? type.GetGenericTypeDefinition() : type);
			foreach (Type type3 in CollectionDataContract.KnownInterfaces)
			{
				if (type2 == type3)
				{
					return true;
				}
			}
			return false;
		}

		[SecuritySafeCritical]
		internal override DataContract BindGenericParameters(DataContract[] paramContracts, Dictionary<DataContract, DataContract> boundContracts)
		{
			DataContract dataContract;
			if (boundContracts.TryGetValue(this, out dataContract))
			{
				return dataContract;
			}
			CollectionDataContract collectionDataContract = new CollectionDataContract(this.Kind);
			boundContracts.Add(this, collectionDataContract);
			collectionDataContract.ItemContract = this.ItemContract.BindGenericParameters(paramContracts, boundContracts);
			collectionDataContract.IsItemTypeNullable = !collectionDataContract.ItemContract.IsValueType;
			collectionDataContract.ItemName = (this.ItemNameSetExplicit ? this.ItemName : collectionDataContract.ItemContract.StableName.Name);
			collectionDataContract.KeyName = this.KeyName;
			collectionDataContract.ValueName = this.ValueName;
			collectionDataContract.StableName = DataContract.CreateQualifiedName(DataContract.ExpandGenericParameters(XmlConvert.DecodeName(base.StableName.Name), new GenericNameProvider(DataContract.GetClrTypeFullName(base.UnderlyingType), paramContracts)), CollectionDataContract.IsCollectionDataContract(base.UnderlyingType) ? base.StableName.Namespace : DataContract.GetCollectionNamespace(collectionDataContract.ItemContract.StableName.Namespace));
			return collectionDataContract;
		}

		internal override DataContract GetValidContract(SerializationMode mode)
		{
			if (mode == SerializationMode.SharedType)
			{
				if (this.SharedTypeContract == null)
				{
					DataContract.ThrowTypeNotSerializable(base.UnderlyingType);
				}
				return this.SharedTypeContract;
			}
			this.ThrowIfInvalid();
			return this;
		}

		private void ThrowIfInvalid()
		{
			if (this.InvalidCollectionInSharedContractMessage != null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(this.InvalidCollectionInSharedContractMessage));
			}
		}

		internal override DataContract GetValidContract()
		{
			if (this.IsConstructorCheckRequired)
			{
				this.CheckConstructor();
			}
			return this;
		}

		[SecuritySafeCritical]
		private void CheckConstructor()
		{
			if (this.Constructor == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("{0} does not have a default constructor.", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) })));
			}
			this.IsConstructorCheckRequired = false;
		}

		internal override bool IsValidContract(SerializationMode mode)
		{
			if (mode == SerializationMode.SharedType)
			{
				return this.SharedTypeContract != null;
			}
			return this.InvalidCollectionInSharedContractMessage == null;
		}

		internal override bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
		{
			if (base.IsEqualOrChecked(other, checkedContracts))
			{
				return true;
			}
			if (base.Equals(other, checkedContracts))
			{
				CollectionDataContract collectionDataContract = other as CollectionDataContract;
				if (collectionDataContract != null)
				{
					bool flag = this.ItemContract != null && !this.ItemContract.IsValueType;
					bool flag2 = collectionDataContract.ItemContract != null && !collectionDataContract.ItemContract.IsValueType;
					return this.ItemName == collectionDataContract.ItemName && (this.IsItemTypeNullable || flag) == (collectionDataContract.IsItemTypeNullable || flag2) && this.ItemContract.Equals(collectionDataContract.ItemContract, checkedContracts);
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
		{
			context.IsGetOnlyCollection = false;
			this.XmlFormatWriterDelegate(xmlWriter, obj, context, this);
		}

		public override object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
		{
			xmlReader.Read();
			object obj = null;
			if (context.IsGetOnlyCollection)
			{
				context.IsGetOnlyCollection = false;
				this.XmlFormatGetOnlyCollectionReaderDelegate(xmlReader, context, this.CollectionItemName, this.Namespace, this);
			}
			else
			{
				obj = this.XmlFormatReaderDelegate(xmlReader, context, this.CollectionItemName, this.Namespace, this);
			}
			xmlReader.ReadEndElement();
			return obj;
		}

		[SecurityCritical]
		private XmlDictionaryString collectionItemName;

		[SecurityCritical]
		private XmlDictionaryString childElementNamespace;

		[SecurityCritical]
		private DataContract itemContract;

		[SecurityCritical]
		private CollectionDataContract.CollectionDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class CollectionDataContractCriticalHelper : DataContract.DataContractCriticalHelper
		{
			internal static Type[] KnownInterfaces
			{
				get
				{
					if (CollectionDataContract.CollectionDataContractCriticalHelper._knownInterfaces == null)
					{
						CollectionDataContract.CollectionDataContractCriticalHelper._knownInterfaces = new Type[]
						{
							Globals.TypeOfIDictionaryGeneric,
							Globals.TypeOfIDictionary,
							Globals.TypeOfIListGeneric,
							Globals.TypeOfICollectionGeneric,
							Globals.TypeOfIList,
							Globals.TypeOfIEnumerableGeneric,
							Globals.TypeOfICollection,
							Globals.TypeOfIEnumerable
						};
					}
					return CollectionDataContract.CollectionDataContractCriticalHelper._knownInterfaces;
				}
			}

			private void Init(CollectionKind kind, Type itemType, CollectionDataContractAttribute collectionContractAttribute)
			{
				this.kind = kind;
				if (itemType != null)
				{
					this.itemType = itemType;
					this.isItemTypeNullable = DataContract.IsTypeNullable(itemType);
					bool flag = kind == CollectionKind.Dictionary || kind == CollectionKind.GenericDictionary;
					string text = null;
					string text2 = null;
					string text3 = null;
					if (collectionContractAttribute != null)
					{
						if (collectionContractAttribute.IsItemNameSetExplicitly)
						{
							if (collectionContractAttribute.ItemName == null || collectionContractAttribute.ItemName.Length == 0)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot have CollectionDataContractAttribute attribute ItemName set to null or empty string.", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) })));
							}
							text = DataContract.EncodeLocalName(collectionContractAttribute.ItemName);
							this.itemNameSetExplicit = true;
						}
						if (collectionContractAttribute.IsKeyNameSetExplicitly)
						{
							if (collectionContractAttribute.KeyName == null || collectionContractAttribute.KeyName.Length == 0)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot have CollectionDataContractAttribute attribute KeyName set to null or empty string.", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) })));
							}
							if (!flag)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("The collection data contract type '{0}' specifies '{1}' for the KeyName property. This is not allowed since the type is not IDictionary. Remove the setting for the KeyName property.", new object[]
								{
									DataContract.GetClrTypeFullName(base.UnderlyingType),
									collectionContractAttribute.KeyName
								})));
							}
							text2 = DataContract.EncodeLocalName(collectionContractAttribute.KeyName);
						}
						if (collectionContractAttribute.IsValueNameSetExplicitly)
						{
							if (collectionContractAttribute.ValueName == null || collectionContractAttribute.ValueName.Length == 0)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot have CollectionDataContractAttribute attribute ValueName set to null or empty string.", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) })));
							}
							if (!flag)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("The collection data contract type '{0}' specifies '{1}' for the ValueName property. This is not allowed since the type is not IDictionary. Remove the setting for the ValueName property.", new object[]
								{
									DataContract.GetClrTypeFullName(base.UnderlyingType),
									collectionContractAttribute.ValueName
								})));
							}
							text3 = DataContract.EncodeLocalName(collectionContractAttribute.ValueName);
						}
					}
					XmlDictionary xmlDictionary = (flag ? new XmlDictionary(5) : new XmlDictionary(3));
					base.Name = xmlDictionary.Add(base.StableName.Name);
					base.Namespace = xmlDictionary.Add(base.StableName.Namespace);
					this.itemName = text ?? DataContract.GetStableName(DataContract.UnwrapNullableType(itemType)).Name;
					this.collectionItemName = xmlDictionary.Add(this.itemName);
					if (flag)
					{
						this.keyName = text2 ?? "Key";
						this.valueName = text3 ?? "Value";
					}
				}
				if (collectionContractAttribute != null)
				{
					base.IsReference = collectionContractAttribute.IsReference;
				}
			}

			internal CollectionDataContractCriticalHelper(CollectionKind kind)
			{
				this.Init(kind, null, null);
			}

			internal CollectionDataContractCriticalHelper(Type type)
				: base(type)
			{
				if (type == Globals.TypeOfArray)
				{
					type = Globals.TypeOfObjectArray;
				}
				if (type.GetArrayRank() > 1)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Multi-dimensional arrays are not supported.")));
				}
				base.StableName = DataContract.GetStableName(type);
				this.Init(CollectionKind.Array, type.GetElementType(), null);
			}

			internal CollectionDataContractCriticalHelper(Type type, DataContract itemContract)
				: base(type)
			{
				if (type.GetArrayRank() > 1)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Multi-dimensional arrays are not supported.")));
				}
				base.StableName = DataContract.CreateQualifiedName("ArrayOf" + itemContract.StableName.Name, itemContract.StableName.Namespace);
				this.itemContract = itemContract;
				this.Init(CollectionKind.Array, type.GetElementType(), null);
			}

			internal CollectionDataContractCriticalHelper(Type type, CollectionKind kind, Type itemType, MethodInfo getEnumeratorMethod, string serializationExceptionMessage, string deserializationExceptionMessage)
				: base(type)
			{
				if (getEnumeratorMethod == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Collection type '{0}' does not have a valid GetEnumerator method.", new object[] { DataContract.GetClrTypeFullName(type) })));
				}
				if (itemType == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Collection type '{0}' must have a non-null item type.", new object[] { DataContract.GetClrTypeFullName(type) })));
				}
				CollectionDataContractAttribute collectionDataContractAttribute;
				base.StableName = DataContract.GetCollectionStableName(type, itemType, out collectionDataContractAttribute);
				this.Init(kind, itemType, collectionDataContractAttribute);
				this.getEnumeratorMethod = getEnumeratorMethod;
				this.serializationExceptionMessage = serializationExceptionMessage;
				this.deserializationExceptionMessage = deserializationExceptionMessage;
			}

			internal CollectionDataContractCriticalHelper(Type type, CollectionKind kind, Type itemType, MethodInfo getEnumeratorMethod, MethodInfo addMethod, ConstructorInfo constructor)
				: this(type, kind, itemType, getEnumeratorMethod, null, null)
			{
				if (addMethod == null && !type.IsInterface)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Collection type '{0}' does not have a valid Add method.", new object[] { DataContract.GetClrTypeFullName(type) })));
				}
				this.addMethod = addMethod;
				this.constructor = constructor;
			}

			internal CollectionDataContractCriticalHelper(Type type, CollectionKind kind, Type itemType, MethodInfo getEnumeratorMethod, MethodInfo addMethod, ConstructorInfo constructor, bool isConstructorCheckRequired)
				: this(type, kind, itemType, getEnumeratorMethod, addMethod, constructor)
			{
				this.isConstructorCheckRequired = isConstructorCheckRequired;
			}

			internal CollectionDataContractCriticalHelper(Type type, string invalidCollectionInSharedContractMessage)
				: base(type)
			{
				this.Init(CollectionKind.Collection, null, null);
				this.invalidCollectionInSharedContractMessage = invalidCollectionInSharedContractMessage;
			}

			internal CollectionKind Kind
			{
				get
				{
					return this.kind;
				}
			}

			internal Type ItemType
			{
				get
				{
					return this.itemType;
				}
			}

			internal DataContract ItemContract
			{
				get
				{
					if (this.itemContract == null && base.UnderlyingType != null)
					{
						if (this.IsDictionary)
						{
							if (string.CompareOrdinal(this.KeyName, this.ValueName) == 0)
							{
								DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("The collection data contract type '{0}' specifies the same value '{1}' for both the KeyName and the ValueName properties. This is not allowed. Consider changing either the KeyName or the ValueName property.", new object[]
								{
									DataContract.GetClrTypeFullName(base.UnderlyingType),
									this.KeyName
								}), base.UnderlyingType);
							}
							this.itemContract = ClassDataContract.CreateClassDataContractForKeyValue(this.ItemType, base.Namespace, new string[] { this.KeyName, this.ValueName });
							DataContract.GetDataContract(this.ItemType);
						}
						else
						{
							this.itemContract = DataContract.GetDataContract(this.ItemType);
						}
					}
					return this.itemContract;
				}
				set
				{
					this.itemContract = value;
				}
			}

			internal DataContract SharedTypeContract
			{
				get
				{
					return this.sharedTypeContract;
				}
				set
				{
					this.sharedTypeContract = value;
				}
			}

			internal string ItemName
			{
				get
				{
					return this.itemName;
				}
				set
				{
					this.itemName = value;
				}
			}

			internal bool IsConstructorCheckRequired
			{
				get
				{
					return this.isConstructorCheckRequired;
				}
				set
				{
					this.isConstructorCheckRequired = value;
				}
			}

			public XmlDictionaryString CollectionItemName
			{
				get
				{
					return this.collectionItemName;
				}
			}

			internal string KeyName
			{
				get
				{
					return this.keyName;
				}
				set
				{
					this.keyName = value;
				}
			}

			internal string ValueName
			{
				get
				{
					return this.valueName;
				}
				set
				{
					this.valueName = value;
				}
			}

			internal bool IsDictionary
			{
				get
				{
					return this.KeyName != null;
				}
			}

			public string SerializationExceptionMessage
			{
				get
				{
					return this.serializationExceptionMessage;
				}
			}

			public string DeserializationExceptionMessage
			{
				get
				{
					return this.deserializationExceptionMessage;
				}
			}

			public XmlDictionaryString ChildElementNamespace
			{
				get
				{
					return this.childElementNamespace;
				}
				set
				{
					this.childElementNamespace = value;
				}
			}

			internal bool IsItemTypeNullable
			{
				get
				{
					return this.isItemTypeNullable;
				}
				set
				{
					this.isItemTypeNullable = value;
				}
			}

			internal MethodInfo GetEnumeratorMethod
			{
				get
				{
					return this.getEnumeratorMethod;
				}
			}

			internal MethodInfo AddMethod
			{
				get
				{
					return this.addMethod;
				}
			}

			internal ConstructorInfo Constructor
			{
				get
				{
					return this.constructor;
				}
			}

			internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
			{
				get
				{
					if (!this.isKnownTypeAttributeChecked && base.UnderlyingType != null)
					{
						lock (this)
						{
							if (!this.isKnownTypeAttributeChecked)
							{
								this.knownDataContracts = DataContract.ImportKnownTypeAttributes(base.UnderlyingType);
								Thread.MemoryBarrier();
								this.isKnownTypeAttributeChecked = true;
							}
						}
					}
					return this.knownDataContracts;
				}
				set
				{
					this.knownDataContracts = value;
				}
			}

			internal string InvalidCollectionInSharedContractMessage
			{
				get
				{
					return this.invalidCollectionInSharedContractMessage;
				}
			}

			internal bool ItemNameSetExplicit
			{
				get
				{
					return this.itemNameSetExplicit;
				}
			}

			internal XmlFormatCollectionWriterDelegate XmlFormatWriterDelegate
			{
				get
				{
					return this.xmlFormatWriterDelegate;
				}
				set
				{
					this.xmlFormatWriterDelegate = value;
				}
			}

			internal XmlFormatCollectionReaderDelegate XmlFormatReaderDelegate
			{
				get
				{
					return this.xmlFormatReaderDelegate;
				}
				set
				{
					this.xmlFormatReaderDelegate = value;
				}
			}

			internal XmlFormatGetOnlyCollectionReaderDelegate XmlFormatGetOnlyCollectionReaderDelegate
			{
				get
				{
					return this.xmlFormatGetOnlyCollectionReaderDelegate;
				}
				set
				{
					this.xmlFormatGetOnlyCollectionReaderDelegate = value;
				}
			}

			private static Type[] _knownInterfaces;

			private Type itemType;

			private bool isItemTypeNullable;

			private CollectionKind kind;

			private readonly MethodInfo getEnumeratorMethod;

			private readonly MethodInfo addMethod;

			private readonly ConstructorInfo constructor;

			private readonly string serializationExceptionMessage;

			private readonly string deserializationExceptionMessage;

			private DataContract itemContract;

			private DataContract sharedTypeContract;

			private Dictionary<XmlQualifiedName, DataContract> knownDataContracts;

			private bool isKnownTypeAttributeChecked;

			private string itemName;

			private bool itemNameSetExplicit;

			private XmlDictionaryString collectionItemName;

			private string keyName;

			private string valueName;

			private XmlDictionaryString childElementNamespace;

			private string invalidCollectionInSharedContractMessage;

			private XmlFormatCollectionReaderDelegate xmlFormatReaderDelegate;

			private XmlFormatGetOnlyCollectionReaderDelegate xmlFormatGetOnlyCollectionReaderDelegate;

			private XmlFormatCollectionWriterDelegate xmlFormatWriterDelegate;

			private bool isConstructorCheckRequired;
		}

		public class DictionaryEnumerator : IEnumerator<KeyValue<object, object>>, IDisposable, IEnumerator
		{
			public DictionaryEnumerator(IDictionaryEnumerator enumerator)
			{
				this.enumerator = enumerator;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return this.enumerator.MoveNext();
			}

			public KeyValue<object, object> Current
			{
				get
				{
					return new KeyValue<object, object>(this.enumerator.Key, this.enumerator.Value);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Reset()
			{
				this.enumerator.Reset();
			}

			private IDictionaryEnumerator enumerator;
		}

		public class GenericDictionaryEnumerator<K, V> : IEnumerator<KeyValue<K, V>>, IDisposable, IEnumerator
		{
			public GenericDictionaryEnumerator(IEnumerator<KeyValuePair<K, V>> enumerator)
			{
				this.enumerator = enumerator;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return this.enumerator.MoveNext();
			}

			public KeyValue<K, V> Current
			{
				get
				{
					KeyValuePair<K, V> keyValuePair = this.enumerator.Current;
					return new KeyValue<K, V>(keyValuePair.Key, keyValuePair.Value);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Reset()
			{
				this.enumerator.Reset();
			}

			private IEnumerator<KeyValuePair<K, V>> enumerator;
		}
	}
}
