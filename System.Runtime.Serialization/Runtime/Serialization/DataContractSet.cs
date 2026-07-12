using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class DataContractSet
	{
		internal DataContractSet(IDataContractSurrogate dataContractSurrogate)
			: this(dataContractSurrogate, null, null)
		{
		}

		internal DataContractSet(IDataContractSurrogate dataContractSurrogate, ICollection<Type> referencedTypes, ICollection<Type> referencedCollectionTypes)
		{
			this.dataContractSurrogate = dataContractSurrogate;
			this.referencedTypes = referencedTypes;
			this.referencedCollectionTypes = referencedCollectionTypes;
		}

		internal DataContractSet(DataContractSet dataContractSet)
		{
			if (dataContractSet == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("dataContractSet"));
			}
			this.dataContractSurrogate = dataContractSet.dataContractSurrogate;
			this.referencedTypes = dataContractSet.referencedTypes;
			this.referencedCollectionTypes = dataContractSet.referencedCollectionTypes;
			foreach (KeyValuePair<XmlQualifiedName, DataContract> keyValuePair in dataContractSet)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
			if (dataContractSet.processedContracts != null)
			{
				foreach (KeyValuePair<DataContract, object> keyValuePair2 in dataContractSet.processedContracts)
				{
					this.ProcessedContracts.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
			}
		}

		private Dictionary<XmlQualifiedName, DataContract> Contracts
		{
			get
			{
				if (this.contracts == null)
				{
					this.contracts = new Dictionary<XmlQualifiedName, DataContract>();
				}
				return this.contracts;
			}
		}

		private Dictionary<DataContract, object> ProcessedContracts
		{
			get
			{
				if (this.processedContracts == null)
				{
					this.processedContracts = new Dictionary<DataContract, object>();
				}
				return this.processedContracts;
			}
		}

		private Hashtable SurrogateDataTable
		{
			get
			{
				if (this.surrogateDataTable == null)
				{
					this.surrogateDataTable = new Hashtable();
				}
				return this.surrogateDataTable;
			}
		}

		internal Dictionary<XmlQualifiedName, DataContract> KnownTypesForObject
		{
			get
			{
				return this.knownTypesForObject;
			}
			set
			{
				this.knownTypesForObject = value;
			}
		}

		internal void Add(Type type)
		{
			DataContract dataContract = this.GetDataContract(type);
			DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
			this.Add(dataContract);
		}

		internal static void EnsureTypeNotGeneric(Type type)
		{
			if (type.ContainsGenericParameters)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Generic type '{0}' is not exportable.", new object[] { type })));
			}
		}

		private void Add(DataContract dataContract)
		{
			this.Add(dataContract.StableName, dataContract);
		}

		public void Add(XmlQualifiedName name, DataContract dataContract)
		{
			if (dataContract.IsBuiltInDataContract)
			{
				return;
			}
			this.InternalAdd(name, dataContract);
		}

		internal void InternalAdd(XmlQualifiedName name, DataContract dataContract)
		{
			DataContract dataContract2 = null;
			if (this.Contracts.TryGetValue(name, out dataContract2))
			{
				if (!dataContract2.Equals(dataContract))
				{
					if (dataContract.UnderlyingType == null || dataContract2.UnderlyingType == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Duplicate contract in data contract set was found, for '{0}' in '{1}' namespace.", new object[]
						{
							dataContract.StableName.Name,
							dataContract.StableName.Namespace
						})));
					}
					bool flag = DataContract.GetClrTypeFullName(dataContract.UnderlyingType) == DataContract.GetClrTypeFullName(dataContract2.UnderlyingType);
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Duplicate type contract in data contract set. Type name '{0}', for data contract '{1}' in '{2}' namespace.", new object[]
					{
						flag ? dataContract.UnderlyingType.AssemblyQualifiedName : DataContract.GetClrTypeFullName(dataContract.UnderlyingType),
						flag ? dataContract2.UnderlyingType.AssemblyQualifiedName : DataContract.GetClrTypeFullName(dataContract2.UnderlyingType),
						dataContract.StableName.Name,
						dataContract.StableName.Namespace
					})));
				}
			}
			else
			{
				this.Contracts.Add(name, dataContract);
				if (dataContract is ClassDataContract)
				{
					this.AddClassDataContract((ClassDataContract)dataContract);
					return;
				}
				if (dataContract is CollectionDataContract)
				{
					this.AddCollectionDataContract((CollectionDataContract)dataContract);
					return;
				}
				if (dataContract is XmlDataContract)
				{
					this.AddXmlDataContract((XmlDataContract)dataContract);
				}
			}
		}

		private void AddClassDataContract(ClassDataContract classDataContract)
		{
			if (classDataContract.BaseContract != null)
			{
				this.Add(classDataContract.BaseContract.StableName, classDataContract.BaseContract);
			}
			if (!classDataContract.IsISerializable && classDataContract.Members != null)
			{
				for (int i = 0; i < classDataContract.Members.Count; i++)
				{
					DataMember dataMember = classDataContract.Members[i];
					DataContract memberTypeDataContract = this.GetMemberTypeDataContract(dataMember);
					if (this.dataContractSurrogate != null && dataMember.MemberInfo != null)
					{
						object customDataToExport = DataContractSurrogateCaller.GetCustomDataToExport(this.dataContractSurrogate, dataMember.MemberInfo, memberTypeDataContract.UnderlyingType);
						if (customDataToExport != null)
						{
							this.SurrogateDataTable.Add(dataMember, customDataToExport);
						}
					}
					this.Add(memberTypeDataContract.StableName, memberTypeDataContract);
				}
			}
			this.AddKnownDataContracts(classDataContract.KnownDataContracts);
		}

		private void AddCollectionDataContract(CollectionDataContract collectionDataContract)
		{
			if (collectionDataContract.IsDictionary)
			{
				ClassDataContract classDataContract = collectionDataContract.ItemContract as ClassDataContract;
				this.AddClassDataContract(classDataContract);
			}
			else
			{
				DataContract itemTypeDataContract = this.GetItemTypeDataContract(collectionDataContract);
				if (itemTypeDataContract != null)
				{
					this.Add(itemTypeDataContract.StableName, itemTypeDataContract);
				}
			}
			this.AddKnownDataContracts(collectionDataContract.KnownDataContracts);
		}

		private void AddXmlDataContract(XmlDataContract xmlDataContract)
		{
			this.AddKnownDataContracts(xmlDataContract.KnownDataContracts);
		}

		private void AddKnownDataContracts(Dictionary<XmlQualifiedName, DataContract> knownDataContracts)
		{
			if (knownDataContracts != null)
			{
				foreach (DataContract dataContract in knownDataContracts.Values)
				{
					this.Add(dataContract);
				}
			}
		}

		internal XmlQualifiedName GetStableName(Type clrType)
		{
			if (this.dataContractSurrogate != null)
			{
				return DataContract.GetStableName(DataContractSurrogateCaller.GetDataContractType(this.dataContractSurrogate, clrType));
			}
			return DataContract.GetStableName(clrType);
		}

		internal DataContract GetDataContract(Type clrType)
		{
			if (this.dataContractSurrogate == null)
			{
				return DataContract.GetDataContract(clrType);
			}
			DataContract dataContract = DataContract.GetBuiltInDataContract(clrType);
			if (dataContract != null)
			{
				return dataContract;
			}
			Type dataContractType = DataContractSurrogateCaller.GetDataContractType(this.dataContractSurrogate, clrType);
			dataContract = DataContract.GetDataContract(dataContractType);
			if (!this.SurrogateDataTable.Contains(dataContract))
			{
				object customDataToExport = DataContractSurrogateCaller.GetCustomDataToExport(this.dataContractSurrogate, clrType, dataContractType);
				if (customDataToExport != null)
				{
					this.SurrogateDataTable.Add(dataContract, customDataToExport);
				}
			}
			return dataContract;
		}

		internal DataContract GetMemberTypeDataContract(DataMember dataMember)
		{
			if (!(dataMember.MemberInfo != null))
			{
				return dataMember.MemberTypeContract;
			}
			Type memberType = dataMember.MemberType;
			if (!dataMember.IsGetOnlyCollection)
			{
				return this.GetDataContract(memberType);
			}
			if (this.dataContractSurrogate != null && DataContractSurrogateCaller.GetDataContractType(this.dataContractSurrogate, memberType) != memberType)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Surrogates with get-only collections are not supported. Type '{1}' contains '{2}' which is of '{0}' type.", new object[]
				{
					DataContract.GetClrTypeFullName(memberType),
					DataContract.GetClrTypeFullName(dataMember.MemberInfo.DeclaringType),
					dataMember.MemberInfo.Name
				})));
			}
			return DataContract.GetGetOnlyCollectionDataContract(DataContract.GetId(memberType.TypeHandle), memberType.TypeHandle, memberType, SerializationMode.SharedContract);
		}

		internal DataContract GetItemTypeDataContract(CollectionDataContract collectionContract)
		{
			if (collectionContract.ItemType != null)
			{
				return this.GetDataContract(collectionContract.ItemType);
			}
			return collectionContract.ItemContract;
		}

		internal object GetSurrogateData(object key)
		{
			return this.SurrogateDataTable[key];
		}

		internal void SetSurrogateData(object key, object surrogateData)
		{
			this.SurrogateDataTable[key] = surrogateData;
		}

		public DataContract this[XmlQualifiedName key]
		{
			get
			{
				DataContract builtInDataContract = DataContract.GetBuiltInDataContract(key.Name, key.Namespace);
				if (builtInDataContract == null)
				{
					this.Contracts.TryGetValue(key, out builtInDataContract);
				}
				return builtInDataContract;
			}
		}

		public IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return this.dataContractSurrogate;
			}
		}

		public bool Remove(XmlQualifiedName key)
		{
			return DataContract.GetBuiltInDataContract(key.Name, key.Namespace) == null && this.Contracts.Remove(key);
		}

		public IEnumerator<KeyValuePair<XmlQualifiedName, DataContract>> GetEnumerator()
		{
			return this.Contracts.GetEnumerator();
		}

		internal bool IsContractProcessed(DataContract dataContract)
		{
			return this.ProcessedContracts.ContainsKey(dataContract);
		}

		internal void SetContractProcessed(DataContract dataContract)
		{
			this.ProcessedContracts.Add(dataContract, dataContract);
		}

		internal ContractCodeDomInfo GetContractCodeDomInfo(DataContract dataContract)
		{
			object obj;
			if (this.ProcessedContracts.TryGetValue(dataContract, out obj))
			{
				return (ContractCodeDomInfo)obj;
			}
			return null;
		}

		internal void SetContractCodeDomInfo(DataContract dataContract, ContractCodeDomInfo info)
		{
			this.ProcessedContracts.Add(dataContract, info);
		}

		private Dictionary<XmlQualifiedName, object> GetReferencedTypes()
		{
			if (this.referencedTypesDictionary == null)
			{
				this.referencedTypesDictionary = new Dictionary<XmlQualifiedName, object>();
				this.referencedTypesDictionary.Add(DataContract.GetStableName(Globals.TypeOfNullable), Globals.TypeOfNullable);
				if (this.referencedTypes != null)
				{
					foreach (Type type in this.referencedTypes)
					{
						if (type == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Referenced types cannot contain null.")));
						}
						this.AddReferencedType(this.referencedTypesDictionary, type);
					}
				}
			}
			return this.referencedTypesDictionary;
		}

		private Dictionary<XmlQualifiedName, object> GetReferencedCollectionTypes()
		{
			if (this.referencedCollectionTypesDictionary == null)
			{
				this.referencedCollectionTypesDictionary = new Dictionary<XmlQualifiedName, object>();
				if (this.referencedCollectionTypes != null)
				{
					foreach (Type type in this.referencedCollectionTypes)
					{
						if (type == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Referenced collection types cannot contain null.")));
						}
						this.AddReferencedType(this.referencedCollectionTypesDictionary, type);
					}
				}
				XmlQualifiedName stableName = DataContract.GetStableName(Globals.TypeOfDictionaryGeneric);
				if (!this.referencedCollectionTypesDictionary.ContainsKey(stableName) && this.GetReferencedTypes().ContainsKey(stableName))
				{
					this.AddReferencedType(this.referencedCollectionTypesDictionary, Globals.TypeOfDictionaryGeneric);
				}
			}
			return this.referencedCollectionTypesDictionary;
		}

		private void AddReferencedType(Dictionary<XmlQualifiedName, object> referencedTypes, Type type)
		{
			if (DataContractSet.IsTypeReferenceable(type))
			{
				XmlQualifiedName stableName;
				try
				{
					stableName = this.GetStableName(type);
				}
				catch (InvalidDataContractException)
				{
					return;
				}
				catch (InvalidOperationException)
				{
					return;
				}
				object obj;
				if (referencedTypes.TryGetValue(stableName, out obj))
				{
					Type type2 = obj as Type;
					if (type2 != null)
					{
						if (type2 != type)
						{
							referencedTypes.Remove(stableName);
							referencedTypes.Add(stableName, new List<Type> { type2, type });
							return;
						}
					}
					else
					{
						List<Type> list = (List<Type>)obj;
						if (!list.Contains(type))
						{
							list.Add(type);
							return;
						}
					}
				}
				else
				{
					referencedTypes.Add(stableName, type);
				}
			}
		}

		internal bool TryGetReferencedType(XmlQualifiedName stableName, DataContract dataContract, out Type type)
		{
			return this.TryGetReferencedType(stableName, dataContract, false, out type);
		}

		internal bool TryGetReferencedCollectionType(XmlQualifiedName stableName, DataContract dataContract, out Type type)
		{
			return this.TryGetReferencedType(stableName, dataContract, true, out type);
		}

		private bool TryGetReferencedType(XmlQualifiedName stableName, DataContract dataContract, bool useReferencedCollectionTypes, out Type type)
		{
			object obj;
			if (!(useReferencedCollectionTypes ? this.GetReferencedCollectionTypes() : this.GetReferencedTypes()).TryGetValue(stableName, out obj))
			{
				type = null;
				return false;
			}
			type = obj as Type;
			if (type != null)
			{
				return true;
			}
			List<Type> list = (List<Type>)obj;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				Type type2 = list[i];
				if (!flag)
				{
					flag = type2.IsGenericTypeDefinition;
				}
				stringBuilder.AppendFormat("{0}\"{1}\" ", Environment.NewLine, type2.AssemblyQualifiedName);
				if (dataContract != null)
				{
					DataContract dataContract2 = this.GetDataContract(type2);
					stringBuilder.Append(global::System.Runtime.Serialization.SR.GetString((dataContract2 != null && dataContract2.Equals(dataContract)) ? "Reference type matches." : "Reference type does not match."));
				}
			}
			if (flag)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString(useReferencedCollectionTypes ? "Ambiguous collection types were referenced: {0}" : "Ambiguous types were referenced: {0}", new object[] { stringBuilder.ToString() })));
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString(useReferencedCollectionTypes ? "In '{0}' element in '{1}' namespace, ambiguous collection types were referenced: {2}" : "In '{0}' element in '{1}' namespace, ambiguous types were referenced: {2}", new object[]
			{
				XmlConvert.DecodeName(stableName.Name),
				stableName.Namespace,
				stringBuilder.ToString()
			})));
		}

		private static bool IsTypeReferenceable(Type type)
		{
			try
			{
				Type type2;
				return type.IsSerializable || type.IsDefined(Globals.TypeOfDataContractAttribute, false) || (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type) && !type.IsGenericTypeDefinition) || CollectionDataContract.IsCollection(type, out type2) || ClassDataContract.IsNonAttributedTypeValidForSerialization(type);
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
			}
			return false;
		}

		private Dictionary<XmlQualifiedName, DataContract> contracts;

		private Dictionary<DataContract, object> processedContracts;

		private IDataContractSurrogate dataContractSurrogate;

		private Hashtable surrogateDataTable;

		private Dictionary<XmlQualifiedName, DataContract> knownTypesForObject;

		private ICollection<Type> referencedTypes;

		private ICollection<Type> referencedCollectionTypes;

		private Dictionary<XmlQualifiedName, object> referencedTypesDictionary;

		private Dictionary<XmlQualifiedName, object> referencedCollectionTypesDictionary;
	}
}
