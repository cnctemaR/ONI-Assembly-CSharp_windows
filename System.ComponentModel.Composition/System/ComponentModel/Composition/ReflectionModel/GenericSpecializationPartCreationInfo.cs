using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class GenericSpecializationPartCreationInfo : IReflectionPartCreationInfo, ICompositionElement
	{
		public GenericSpecializationPartCreationInfo(IReflectionPartCreationInfo originalPartCreationInfo, ReflectionComposablePartDefinition originalPart, Type[] specialization)
		{
			GenericSpecializationPartCreationInfo <>4__this = this;
			Assumes.NotNull<IReflectionPartCreationInfo>(originalPartCreationInfo);
			Assumes.NotNull<Type[]>(specialization);
			Assumes.NotNull<ReflectionComposablePartDefinition>(originalPart);
			this._originalPartCreationInfo = originalPartCreationInfo;
			this._originalPart = originalPart;
			this._specialization = specialization;
			this._specializationIdentities = new string[this._specialization.Length];
			for (int i = 0; i < this._specialization.Length; i++)
			{
				this._specializationIdentities[i] = AttributedModelServices.GetTypeIdentity(this._specialization[i]);
			}
			this._lazyPartType = new Lazy<Type>(() => <>4__this._originalPartCreationInfo.GetPartType().MakeGenericType(specialization), LazyThreadSafetyMode.PublicationOnly);
		}

		public ReflectionComposablePartDefinition OriginalPart
		{
			get
			{
				return this._originalPart;
			}
		}

		public Type GetPartType()
		{
			return this._lazyPartType.Value;
		}

		public Lazy<Type> GetLazyPartType()
		{
			return this._lazyPartType;
		}

		public ConstructorInfo GetConstructor()
		{
			if (this._constructor == null)
			{
				ConstructorInfo constructor = this._originalPartCreationInfo.GetConstructor();
				ConstructorInfo constructorInfo = null;
				if (constructor != null)
				{
					foreach (ConstructorInfo constructorInfo2 in this.GetPartType().GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (constructorInfo2.MetadataToken == constructor.MetadataToken)
						{
							constructorInfo = constructorInfo2;
							break;
						}
					}
				}
				Thread.MemoryBarrier();
				object @lock = this._lock;
				lock (@lock)
				{
					if (this._constructor == null)
					{
						this._constructor = constructorInfo;
					}
				}
			}
			return this._constructor;
		}

		public IDictionary<string, object> GetMetadata()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(this._originalPartCreationInfo.GetMetadata(), StringComparers.MetadataKeyNames);
			dictionary.Remove("System.ComponentModel.Composition.IsGenericPart");
			dictionary.Remove("System.ComponentModel.Composition.GenericPartArity");
			dictionary.Remove("System.ComponentModel.Composition.GenericParameterConstraints");
			dictionary.Remove("System.ComponentModel.Composition.GenericParameterAttributes");
			return dictionary;
		}

		private MemberInfo[] GetAccessors(LazyMemberInfo originalLazyMember)
		{
			this.BuildTables();
			Assumes.NotNull<Dictionary<LazyMemberInfo, MemberInfo[]>>(this._membersTable);
			return this._membersTable[originalLazyMember];
		}

		private ParameterInfo GetParameter(Lazy<ParameterInfo> originalParameter)
		{
			this.BuildTables();
			Assumes.NotNull<Dictionary<Lazy<ParameterInfo>, ParameterInfo>>(this._parametersTable);
			return this._parametersTable[originalParameter];
		}

		private void BuildTables()
		{
			if (this._membersTable != null)
			{
				return;
			}
			this.PopulateImportsAndExports();
			List<LazyMemberInfo> list = null;
			List<Lazy<ParameterInfo>> list2 = null;
			object obj = this._lock;
			lock (obj)
			{
				if (this._membersTable == null)
				{
					list = this._members;
					list2 = this._parameters;
					Assumes.NotNull<List<LazyMemberInfo>>(list);
				}
			}
			Dictionary<LazyMemberInfo, MemberInfo[]> dictionary = this.BuildMembersTable(list);
			Dictionary<Lazy<ParameterInfo>, ParameterInfo> dictionary2 = this.BuildParametersTable(list2);
			obj = this._lock;
			lock (obj)
			{
				if (this._membersTable == null)
				{
					this._membersTable = dictionary;
					this._parametersTable = dictionary2;
					Thread.MemoryBarrier();
					this._parameters = null;
					this._members = null;
				}
			}
		}

		private Dictionary<LazyMemberInfo, MemberInfo[]> BuildMembersTable(List<LazyMemberInfo> members)
		{
			Assumes.NotNull<List<LazyMemberInfo>>(members);
			Dictionary<LazyMemberInfo, MemberInfo[]> dictionary = new Dictionary<LazyMemberInfo, MemberInfo[]>();
			Dictionary<int, MemberInfo> dictionary2 = new Dictionary<int, MemberInfo>();
			Type partType = this.GetPartType();
			dictionary2[partType.MetadataToken] = partType;
			foreach (MethodInfo methodInfo in partType.GetAllMethods())
			{
				dictionary2[methodInfo.MetadataToken] = methodInfo;
			}
			foreach (FieldInfo fieldInfo in partType.GetAllFields())
			{
				dictionary2[fieldInfo.MetadataToken] = fieldInfo;
			}
			foreach (LazyMemberInfo lazyMemberInfo in members)
			{
				MemberInfo[] accessors = lazyMemberInfo.GetAccessors();
				MemberInfo[] array = new MemberInfo[accessors.Length];
				for (int i = 0; i < accessors.Length; i++)
				{
					array[i] = ((accessors[i] != null) ? dictionary2[accessors[i].MetadataToken] : null);
				}
				dictionary[lazyMemberInfo] = array;
			}
			return dictionary;
		}

		private Dictionary<Lazy<ParameterInfo>, ParameterInfo> BuildParametersTable(List<Lazy<ParameterInfo>> parameters)
		{
			if (parameters != null)
			{
				Dictionary<Lazy<ParameterInfo>, ParameterInfo> dictionary = new Dictionary<Lazy<ParameterInfo>, ParameterInfo>();
				ParameterInfo[] parameters2 = this.GetConstructor().GetParameters();
				foreach (Lazy<ParameterInfo> lazy in parameters)
				{
					dictionary[lazy] = parameters2[lazy.Value.Position];
				}
				return dictionary;
			}
			return null;
		}

		private List<ImportDefinition> PopulateImports(List<LazyMemberInfo> members, List<Lazy<ParameterInfo>> parameters)
		{
			List<ImportDefinition> list = new List<ImportDefinition>();
			foreach (ImportDefinition importDefinition in this._originalPartCreationInfo.GetImports())
			{
				ReflectionImportDefinition reflectionImportDefinition = importDefinition as ReflectionImportDefinition;
				if (reflectionImportDefinition != null)
				{
					list.Add(this.TranslateImport(reflectionImportDefinition, members, parameters));
				}
			}
			return list;
		}

		private ImportDefinition TranslateImport(ReflectionImportDefinition reflectionImport, List<LazyMemberInfo> members, List<Lazy<ParameterInfo>> parameters)
		{
			bool flag = false;
			ContractBasedImportDefinition contractBasedImportDefinition = reflectionImport;
			IPartCreatorImportDefinition partCreatorImportDefinition = reflectionImport as IPartCreatorImportDefinition;
			if (partCreatorImportDefinition != null)
			{
				contractBasedImportDefinition = partCreatorImportDefinition.ProductImportDefinition;
				flag = true;
			}
			string text = this.Translate(contractBasedImportDefinition.ContractName);
			string text2 = this.Translate(contractBasedImportDefinition.RequiredTypeIdentity);
			IDictionary<string, object> dictionary = this.TranslateImportMetadata(contractBasedImportDefinition);
			ReflectionMemberImportDefinition reflectionMemberImportDefinition = reflectionImport as ReflectionMemberImportDefinition;
			ImportDefinition importDefinition;
			if (reflectionMemberImportDefinition != null)
			{
				LazyMemberInfo lazyMember = reflectionMemberImportDefinition.ImportingLazyMember;
				LazyMemberInfo lazyMemberInfo = new LazyMemberInfo(lazyMember.MemberType, () => this.GetAccessors(lazyMember));
				if (flag)
				{
					importDefinition = new PartCreatorMemberImportDefinition(lazyMemberInfo, ((ICompositionElement)reflectionMemberImportDefinition).Origin, new ContractBasedImportDefinition(text, text2, contractBasedImportDefinition.RequiredMetadata, contractBasedImportDefinition.Cardinality, contractBasedImportDefinition.IsRecomposable, false, CreationPolicy.NonShared, dictionary));
				}
				else
				{
					importDefinition = new ReflectionMemberImportDefinition(lazyMemberInfo, text, text2, contractBasedImportDefinition.RequiredMetadata, contractBasedImportDefinition.Cardinality, contractBasedImportDefinition.IsRecomposable, false, contractBasedImportDefinition.RequiredCreationPolicy, dictionary, ((ICompositionElement)reflectionMemberImportDefinition).Origin);
				}
				members.Add(lazyMember);
			}
			else
			{
				ReflectionParameterImportDefinition reflectionParameterImportDefinition = reflectionImport as ReflectionParameterImportDefinition;
				Assumes.NotNull<ReflectionParameterImportDefinition>(reflectionParameterImportDefinition);
				Lazy<ParameterInfo> lazyParameter = reflectionParameterImportDefinition.ImportingLazyParameter;
				Lazy<ParameterInfo> lazy = new Lazy<ParameterInfo>(() => this.GetParameter(lazyParameter));
				if (flag)
				{
					importDefinition = new PartCreatorParameterImportDefinition(lazy, ((ICompositionElement)reflectionParameterImportDefinition).Origin, new ContractBasedImportDefinition(text, text2, contractBasedImportDefinition.RequiredMetadata, contractBasedImportDefinition.Cardinality, false, true, CreationPolicy.NonShared, dictionary));
				}
				else
				{
					importDefinition = new ReflectionParameterImportDefinition(lazy, text, text2, contractBasedImportDefinition.RequiredMetadata, contractBasedImportDefinition.Cardinality, contractBasedImportDefinition.RequiredCreationPolicy, dictionary, ((ICompositionElement)reflectionParameterImportDefinition).Origin);
				}
				parameters.Add(lazyParameter);
			}
			return importDefinition;
		}

		private List<ExportDefinition> PopulateExports(List<LazyMemberInfo> members)
		{
			List<ExportDefinition> list = new List<ExportDefinition>();
			foreach (ExportDefinition exportDefinition in this._originalPartCreationInfo.GetExports())
			{
				ReflectionMemberExportDefinition reflectionMemberExportDefinition = exportDefinition as ReflectionMemberExportDefinition;
				if (reflectionMemberExportDefinition != null)
				{
					list.Add(this.TranslateExpot(reflectionMemberExportDefinition, members));
				}
			}
			return list;
		}

		public ExportDefinition TranslateExpot(ReflectionMemberExportDefinition reflectionExport, List<LazyMemberInfo> members)
		{
			LazyMemberInfo exportingLazyMember = reflectionExport.ExportingLazyMember;
			LazyMemberInfo capturedLazyMember = exportingLazyMember;
			ReflectionMemberExportDefinition capturedReflectionExport = reflectionExport;
			string text = this.Translate(reflectionExport.ContractName, reflectionExport.Metadata.GetValue<int[]>("System.ComponentModel.Composition.GenericExportParametersOrderMetadataName"));
			LazyMemberInfo lazyMemberInfo = new LazyMemberInfo(capturedLazyMember.MemberType, () => this.GetAccessors(capturedLazyMember));
			Lazy<IDictionary<string, object>> lazy = new Lazy<IDictionary<string, object>>(() => this.TranslateExportMetadata(capturedReflectionExport));
			ExportDefinition exportDefinition = new ReflectionMemberExportDefinition(lazyMemberInfo, new LazyExportDefinition(text, lazy), ((ICompositionElement)reflectionExport).Origin);
			members.Add(capturedLazyMember);
			return exportDefinition;
		}

		private string Translate(string originalValue, int[] genericParametersOrder)
		{
			if (genericParametersOrder != null)
			{
				string[] array = GenericServices.Reorder<string>(this._specializationIdentities, genericParametersOrder);
				IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
				object[] array2 = array;
				return string.Format(invariantCulture, originalValue, array2);
			}
			return this.Translate(originalValue);
		}

		private string Translate(string originalValue)
		{
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			object[] specializationIdentities = this._specializationIdentities;
			return string.Format(invariantCulture, originalValue, specializationIdentities);
		}

		private IDictionary<string, object> TranslateImportMetadata(ContractBasedImportDefinition originalImport)
		{
			int[] value = originalImport.Metadata.GetValue<int[]>("System.ComponentModel.Composition.GenericImportParametersOrderMetadataName");
			if (value != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>(originalImport.Metadata, StringComparers.MetadataKeyNames);
				dictionary["System.ComponentModel.Composition.GenericContractName"] = GenericServices.GetGenericName(originalImport.ContractName, value, this._specialization.Length);
				dictionary["System.ComponentModel.Composition.GenericParameters"] = GenericServices.Reorder<Type>(this._specialization, value);
				dictionary.Remove("System.ComponentModel.Composition.GenericImportParametersOrderMetadataName");
				return dictionary.AsReadOnly();
			}
			return originalImport.Metadata;
		}

		private IDictionary<string, object> TranslateExportMetadata(ReflectionMemberExportDefinition originalExport)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(originalExport.Metadata, StringComparers.MetadataKeyNames);
			string value = originalExport.Metadata.GetValue<string>("ExportTypeIdentity");
			if (!string.IsNullOrEmpty(value))
			{
				dictionary["ExportTypeIdentity"] = this.Translate(value, originalExport.Metadata.GetValue<int[]>("System.ComponentModel.Composition.GenericExportParametersOrderMetadataName"));
			}
			dictionary.Remove("System.ComponentModel.Composition.GenericExportParametersOrderMetadataName");
			return dictionary;
		}

		private void PopulateImportsAndExports()
		{
			if (this._exports == null || this._imports == null)
			{
				List<LazyMemberInfo> list = new List<LazyMemberInfo>();
				List<Lazy<ParameterInfo>> list2 = new List<Lazy<ParameterInfo>>();
				List<ExportDefinition> list3 = this.PopulateExports(list);
				List<ImportDefinition> list4 = this.PopulateImports(list, list2);
				Thread.MemoryBarrier();
				object @lock = this._lock;
				lock (@lock)
				{
					if (this._exports == null || this._imports == null)
					{
						this._members = list;
						if (list2.Count > 0)
						{
							this._parameters = list2;
						}
						this._exports = list3;
						this._imports = list4;
					}
				}
			}
		}

		public IEnumerable<ExportDefinition> GetExports()
		{
			this.PopulateImportsAndExports();
			return this._exports;
		}

		public IEnumerable<ImportDefinition> GetImports()
		{
			this.PopulateImportsAndExports();
			return this._imports;
		}

		public bool IsDisposalRequired
		{
			get
			{
				return this._originalPartCreationInfo.IsDisposalRequired;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.Translate(this._originalPartCreationInfo.DisplayName);
			}
		}

		public ICompositionElement Origin
		{
			get
			{
				return this._originalPartCreationInfo.Origin;
			}
		}

		public override bool Equals(object obj)
		{
			GenericSpecializationPartCreationInfo genericSpecializationPartCreationInfo = obj as GenericSpecializationPartCreationInfo;
			return genericSpecializationPartCreationInfo != null && this._originalPartCreationInfo.Equals(genericSpecializationPartCreationInfo._originalPartCreationInfo) && this._specialization.IsArrayEqual<Type>(genericSpecializationPartCreationInfo._specialization);
		}

		public override int GetHashCode()
		{
			return this._originalPartCreationInfo.GetHashCode();
		}

		public static bool CanSpecialize(IDictionary<string, object> partMetadata, Type[] specialization)
		{
			int value = partMetadata.GetValue<int>("System.ComponentModel.Composition.GenericPartArity");
			if (value != specialization.Length)
			{
				return false;
			}
			object[] value2 = partMetadata.GetValue<object[]>("System.ComponentModel.Composition.GenericParameterConstraints");
			GenericParameterAttributes[] value3 = partMetadata.GetValue<GenericParameterAttributes[]>("System.ComponentModel.Composition.GenericParameterAttributes");
			if (value2 == null && value3 == null)
			{
				return true;
			}
			if (value2 != null && value2.Length != value)
			{
				return false;
			}
			if (value3 != null && value3.Length != value)
			{
				return false;
			}
			for (int i = 0; i < value; i++)
			{
				if (!GenericServices.CanSpecialize(specialization[i], (value2[i] as Type[]).CreateTypeSpecializations(specialization), value3[i]))
				{
					return false;
				}
			}
			return true;
		}

		private readonly IReflectionPartCreationInfo _originalPartCreationInfo;

		private readonly ReflectionComposablePartDefinition _originalPart;

		private readonly Type[] _specialization;

		private readonly string[] _specializationIdentities;

		private IEnumerable<ExportDefinition> _exports;

		private IEnumerable<ImportDefinition> _imports;

		private readonly Lazy<Type> _lazyPartType;

		private List<LazyMemberInfo> _members;

		private List<Lazy<ParameterInfo>> _parameters;

		private Dictionary<LazyMemberInfo, MemberInfo[]> _membersTable;

		private Dictionary<Lazy<ParameterInfo>, ParameterInfo> _parametersTable;

		private ConstructorInfo _constructor;

		private object _lock = new object();
	}
}
