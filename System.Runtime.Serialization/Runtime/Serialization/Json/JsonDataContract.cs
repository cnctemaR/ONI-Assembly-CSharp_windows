using System;
using System.Collections.Generic;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonDataContract
	{
		[SecuritySafeCritical]
		protected JsonDataContract(DataContract traditionalDataContract)
		{
			this.helper = new JsonDataContract.JsonDataContractCriticalHelper(traditionalDataContract);
		}

		[SecuritySafeCritical]
		protected JsonDataContract(JsonDataContract.JsonDataContractCriticalHelper helper)
		{
			this.helper = helper;
		}

		internal virtual string TypeName
		{
			get
			{
				return null;
			}
		}

		protected JsonDataContract.JsonDataContractCriticalHelper Helper
		{
			[SecurityCritical]
			get
			{
				return this.helper;
			}
		}

		protected DataContract TraditionalDataContract
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.TraditionalDataContract;
			}
		}

		private Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.KnownDataContracts;
			}
		}

		[SecuritySafeCritical]
		public static JsonDataContract GetJsonDataContract(DataContract traditionalDataContract)
		{
			return JsonDataContract.JsonDataContractCriticalHelper.GetJsonDataContract(traditionalDataContract);
		}

		public object ReadJsonValue(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
		{
			this.PushKnownDataContracts(context);
			object obj = this.ReadJsonValueCore(jsonReader, context);
			this.PopKnownDataContracts(context);
			return obj;
		}

		public virtual object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
		{
			return this.TraditionalDataContract.ReadXmlValue(jsonReader, context);
		}

		public void WriteJsonValue(XmlWriterDelegator jsonWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
		{
			this.PushKnownDataContracts(context);
			this.WriteJsonValueCore(jsonWriter, obj, context, declaredTypeHandle);
			this.PopKnownDataContracts(context);
		}

		public virtual void WriteJsonValueCore(XmlWriterDelegator jsonWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
		{
			this.TraditionalDataContract.WriteXmlValue(jsonWriter, obj, context);
		}

		protected static object HandleReadValue(object obj, XmlObjectSerializerReadContext context)
		{
			context.AddNewObject(obj);
			return obj;
		}

		protected static bool TryReadNullAtTopLevel(XmlReaderDelegator reader)
		{
			if (!reader.MoveToAttribute("type") || !(reader.Value == "null"))
			{
				reader.MoveToElement();
				return false;
			}
			reader.Skip();
			reader.MoveToElement();
			return true;
		}

		protected void PopKnownDataContracts(XmlObjectSerializerContext context)
		{
			if (this.KnownDataContracts != null)
			{
				context.scopedKnownTypes.Pop();
			}
		}

		protected void PushKnownDataContracts(XmlObjectSerializerContext context)
		{
			if (this.KnownDataContracts != null)
			{
				context.scopedKnownTypes.Push(this.KnownDataContracts);
			}
		}

		[SecurityCritical]
		private JsonDataContract.JsonDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		internal class JsonDataContractCriticalHelper
		{
			internal JsonDataContractCriticalHelper(DataContract traditionalDataContract)
			{
				this.traditionalDataContract = traditionalDataContract;
				this.AddCollectionItemContractsToKnownDataContracts();
				this.typeName = (string.IsNullOrEmpty(traditionalDataContract.Namespace.Value) ? traditionalDataContract.Name.Value : (traditionalDataContract.Name.Value + ":" + XmlObjectSerializerWriteContextComplexJson.TruncateDefaultDataContractNamespace(traditionalDataContract.Namespace.Value)));
			}

			internal Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
			{
				get
				{
					return this.knownDataContracts;
				}
			}

			internal DataContract TraditionalDataContract
			{
				get
				{
					return this.traditionalDataContract;
				}
			}

			internal virtual string TypeName
			{
				get
				{
					return this.typeName;
				}
			}

			public static JsonDataContract GetJsonDataContract(DataContract traditionalDataContract)
			{
				int id = JsonDataContract.JsonDataContractCriticalHelper.GetId(traditionalDataContract.UnderlyingType.TypeHandle);
				JsonDataContract jsonDataContract = JsonDataContract.JsonDataContractCriticalHelper.dataContractCache[id];
				if (jsonDataContract == null)
				{
					jsonDataContract = JsonDataContract.JsonDataContractCriticalHelper.CreateJsonDataContract(id, traditionalDataContract);
					JsonDataContract.JsonDataContractCriticalHelper.dataContractCache[id] = jsonDataContract;
				}
				return jsonDataContract;
			}

			internal static int GetId(RuntimeTypeHandle typeHandle)
			{
				object obj = JsonDataContract.JsonDataContractCriticalHelper.cacheLock;
				int value;
				lock (obj)
				{
					JsonDataContract.JsonDataContractCriticalHelper.typeHandleRef.Value = typeHandle;
					IntRef intRef;
					if (!JsonDataContract.JsonDataContractCriticalHelper.typeToIDCache.TryGetValue(JsonDataContract.JsonDataContractCriticalHelper.typeHandleRef, out intRef))
					{
						int num = JsonDataContract.JsonDataContractCriticalHelper.dataContractID++;
						if (num >= JsonDataContract.JsonDataContractCriticalHelper.dataContractCache.Length)
						{
							int num2 = ((num < 1073741823) ? (num * 2) : int.MaxValue);
							if (num2 <= num)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("An internal error has occurred. DataContract cache overflow.")));
							}
							Array.Resize<JsonDataContract>(ref JsonDataContract.JsonDataContractCriticalHelper.dataContractCache, num2);
						}
						intRef = new IntRef(num);
						try
						{
							JsonDataContract.JsonDataContractCriticalHelper.typeToIDCache.Add(new TypeHandleRef(typeHandle), intRef);
						}
						catch (Exception ex)
						{
							if (Fx.IsFatal(ex))
							{
								throw;
							}
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
						}
					}
					value = intRef.Value;
				}
				return value;
			}

			private static JsonDataContract CreateJsonDataContract(int id, DataContract traditionalDataContract)
			{
				object obj = JsonDataContract.JsonDataContractCriticalHelper.createDataContractLock;
				JsonDataContract jsonDataContract2;
				lock (obj)
				{
					JsonDataContract jsonDataContract = JsonDataContract.JsonDataContractCriticalHelper.dataContractCache[id];
					if (jsonDataContract == null)
					{
						Type type = traditionalDataContract.GetType();
						if (type == typeof(ObjectDataContract))
						{
							jsonDataContract = new JsonObjectDataContract(traditionalDataContract);
						}
						else if (type == typeof(StringDataContract))
						{
							jsonDataContract = new JsonStringDataContract((StringDataContract)traditionalDataContract);
						}
						else if (type == typeof(UriDataContract))
						{
							jsonDataContract = new JsonUriDataContract((UriDataContract)traditionalDataContract);
						}
						else if (type == typeof(QNameDataContract))
						{
							jsonDataContract = new JsonQNameDataContract((QNameDataContract)traditionalDataContract);
						}
						else if (type == typeof(ByteArrayDataContract))
						{
							jsonDataContract = new JsonByteArrayDataContract((ByteArrayDataContract)traditionalDataContract);
						}
						else if (traditionalDataContract.IsPrimitive || traditionalDataContract.UnderlyingType == Globals.TypeOfXmlQualifiedName)
						{
							jsonDataContract = new JsonDataContract(traditionalDataContract);
						}
						else if (type == typeof(ClassDataContract))
						{
							jsonDataContract = new JsonClassDataContract((ClassDataContract)traditionalDataContract);
						}
						else if (type == typeof(EnumDataContract))
						{
							jsonDataContract = new JsonEnumDataContract((EnumDataContract)traditionalDataContract);
						}
						else if (type == typeof(GenericParameterDataContract) || type == typeof(SpecialTypeDataContract))
						{
							jsonDataContract = new JsonDataContract(traditionalDataContract);
						}
						else if (type == typeof(CollectionDataContract))
						{
							jsonDataContract = new JsonCollectionDataContract((CollectionDataContract)traditionalDataContract);
						}
						else
						{
							if (!(type == typeof(XmlDataContract)))
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("traditionalDataContract", global::System.Runtime.Serialization.SR.GetString("Type '{0}' is not suppotred by DataContractJsonSerializer.", new object[] { traditionalDataContract.UnderlyingType }));
							}
							jsonDataContract = new JsonXmlDataContract((XmlDataContract)traditionalDataContract);
						}
					}
					jsonDataContract2 = jsonDataContract;
				}
				return jsonDataContract2;
			}

			private void AddCollectionItemContractsToKnownDataContracts()
			{
				if (this.traditionalDataContract.KnownDataContracts != null)
				{
					foreach (KeyValuePair<XmlQualifiedName, DataContract> keyValuePair in this.traditionalDataContract.KnownDataContracts)
					{
						if (keyValuePair != null)
						{
							DataContract itemContract;
							for (CollectionDataContract collectionDataContract = keyValuePair.Value as CollectionDataContract; collectionDataContract != null; collectionDataContract = itemContract as CollectionDataContract)
							{
								itemContract = collectionDataContract.ItemContract;
								if (this.knownDataContracts == null)
								{
									this.knownDataContracts = new Dictionary<XmlQualifiedName, DataContract>();
								}
								if (!this.knownDataContracts.ContainsKey(itemContract.StableName))
								{
									this.knownDataContracts.Add(itemContract.StableName, itemContract);
								}
								if (collectionDataContract.ItemType.IsGenericType && collectionDataContract.ItemType.GetGenericTypeDefinition() == typeof(KeyValue<, >))
								{
									DataContract dataContract = DataContract.GetDataContract(Globals.TypeOfKeyValuePair.MakeGenericType(collectionDataContract.ItemType.GetGenericArguments()));
									if (!this.knownDataContracts.ContainsKey(dataContract.StableName))
									{
										this.knownDataContracts.Add(dataContract.StableName, dataContract);
									}
								}
								if (!(itemContract is CollectionDataContract))
								{
									break;
								}
							}
						}
					}
				}
			}

			private static object cacheLock = new object();

			private static object createDataContractLock = new object();

			private static JsonDataContract[] dataContractCache = new JsonDataContract[32];

			private static int dataContractID = 0;

			private static TypeHandleRef typeHandleRef = new TypeHandleRef();

			private static Dictionary<TypeHandleRef, IntRef> typeToIDCache = new Dictionary<TypeHandleRef, IntRef>(new TypeHandleRefEqualityComparer());

			private Dictionary<XmlQualifiedName, DataContract> knownDataContracts;

			private DataContract traditionalDataContract;

			private string typeName;
		}
	}
}
