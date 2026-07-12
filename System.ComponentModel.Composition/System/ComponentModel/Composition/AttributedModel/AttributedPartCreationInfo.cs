using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Diagnostics;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.AttributedModel
{
	internal class AttributedPartCreationInfo : IReflectionPartCreationInfo, ICompositionElement
	{
		public AttributedPartCreationInfo(Type type, PartCreationPolicyAttribute partCreationPolicy, bool ignoreConstructorImports, ICompositionElement origin)
		{
			Assumes.NotNull<Type>(type);
			this._type = type;
			this._ignoreConstructorImports = ignoreConstructorImports;
			this._partCreationPolicy = partCreationPolicy;
			this._origin = origin;
		}

		public Type GetPartType()
		{
			return this._type;
		}

		public Lazy<Type> GetLazyPartType()
		{
			return new Lazy<Type>(new Func<Type>(this.GetPartType), LazyThreadSafetyMode.PublicationOnly);
		}

		public ConstructorInfo GetConstructor()
		{
			if (this._constructor == null && !this._ignoreConstructorImports)
			{
				this._constructor = AttributedPartCreationInfo.SelectPartConstructor(this._type);
			}
			return this._constructor;
		}

		public IDictionary<string, object> GetMetadata()
		{
			return this._type.GetPartMetadataForType(this.CreationPolicy);
		}

		public IEnumerable<ExportDefinition> GetExports()
		{
			this.DiscoverExportsAndImports();
			return this._exports;
		}

		public IEnumerable<ImportDefinition> GetImports()
		{
			this.DiscoverExportsAndImports();
			return this._imports;
		}

		public bool IsDisposalRequired
		{
			get
			{
				return typeof(IDisposable).IsAssignableFrom(this.GetPartType());
			}
		}

		public bool IsPartDiscoverable()
		{
			if (this._type.IsAttributeDefined<PartNotDiscoverableAttribute>())
			{
				CompositionTrace.DefinitionMarkedWithPartNotDiscoverableAttribute(this._type);
				return false;
			}
			if (!this.HasExports())
			{
				CompositionTrace.DefinitionContainsNoExports(this._type);
				return false;
			}
			return this.AllExportsHaveMatchingArity();
		}

		private bool HasExports()
		{
			return this.GetExportMembers(this._type).Any<MemberInfo>() || this.GetInheritedExports(this._type).Any<Type>();
		}

		private bool AllExportsHaveMatchingArity()
		{
			bool flag = true;
			if (this._type.ContainsGenericParameters)
			{
				int pureGenericArity = this._type.GetPureGenericArity();
				foreach (MemberInfo memberInfo in this.GetExportMembers(this._type).Concat<MemberInfo>(this.GetInheritedExports(this._type)))
				{
					if (memberInfo.MemberType == MemberTypes.Method && ((MethodInfo)memberInfo).ContainsGenericParameters)
					{
						flag = false;
						CompositionTrace.DefinitionMismatchedExportArity(this._type, memberInfo);
					}
					else if (memberInfo.GetDefaultTypeFromMember().GetPureGenericArity() != pureGenericArity)
					{
						flag = false;
						CompositionTrace.DefinitionMismatchedExportArity(this._type, memberInfo);
					}
				}
			}
			return flag;
		}

		string ICompositionElement.DisplayName
		{
			get
			{
				return this.GetDisplayName();
			}
		}

		ICompositionElement ICompositionElement.Origin
		{
			get
			{
				return this._origin;
			}
		}

		public override string ToString()
		{
			return this.GetDisplayName();
		}

		private string GetDisplayName()
		{
			return this.GetPartType().GetDisplayName();
		}

		private CreationPolicy CreationPolicy
		{
			get
			{
				if (this._partCreationPolicy == null)
				{
					this._partCreationPolicy = this._type.GetFirstAttribute<PartCreationPolicyAttribute>() ?? PartCreationPolicyAttribute.Default;
				}
				if (this._partCreationPolicy.CreationPolicy == CreationPolicy.NewScope)
				{
					throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.InvalidPartCreationPolicyOnPart, this._partCreationPolicy.CreationPolicy), this._origin);
				}
				return this._partCreationPolicy.CreationPolicy;
			}
		}

		private static ConstructorInfo SelectPartConstructor(Type type)
		{
			Assumes.NotNull<Type>(type);
			if (type.IsAbstract)
			{
				return null;
			}
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			ConstructorInfo[] constructors = type.GetConstructors(bindingFlags);
			if (constructors.Length == 0)
			{
				return null;
			}
			if (constructors.Length == 1 && constructors[0].GetParameters().Length == 0)
			{
				return constructors[0];
			}
			ConstructorInfo constructorInfo = null;
			ConstructorInfo constructorInfo2 = null;
			foreach (ConstructorInfo constructorInfo3 in constructors)
			{
				if (constructorInfo3.IsAttributeDefined<ImportingConstructorAttribute>())
				{
					if (constructorInfo != null)
					{
						return null;
					}
					constructorInfo = constructorInfo3;
				}
				else if (constructorInfo2 == null && constructorInfo3.GetParameters().Length == 0)
				{
					constructorInfo2 = constructorInfo3;
				}
			}
			return constructorInfo ?? constructorInfo2;
		}

		private void DiscoverExportsAndImports()
		{
			if (this._exports != null && this._imports != null)
			{
				return;
			}
			this._exports = this.GetExportDefinitions();
			this._imports = this.GetImportDefinitions();
		}

		private IEnumerable<ExportDefinition> GetExportDefinitions()
		{
			List<ExportDefinition> list = new List<ExportDefinition>();
			this._contractNamesOnNonInterfaces = new HashSet<string>();
			foreach (MemberInfo memberInfo in this.GetExportMembers(this._type))
			{
				foreach (ExportAttribute exportAttribute in memberInfo.GetAttributes<ExportAttribute>())
				{
					AttributedExportDefinition attributedExportDefinition = this.CreateExportDefinition(memberInfo, exportAttribute);
					if (exportAttribute.GetType() == CompositionServices.InheritedExportAttributeType)
					{
						if (!this._contractNamesOnNonInterfaces.Contains(attributedExportDefinition.ContractName))
						{
							list.Add(new ReflectionMemberExportDefinition(memberInfo.ToLazyMember(), attributedExportDefinition, this));
							this._contractNamesOnNonInterfaces.Add(attributedExportDefinition.ContractName);
						}
					}
					else
					{
						list.Add(new ReflectionMemberExportDefinition(memberInfo.ToLazyMember(), attributedExportDefinition, this));
					}
				}
			}
			foreach (Type type in this.GetInheritedExports(this._type))
			{
				foreach (InheritedExportAttribute inheritedExportAttribute in type.GetAttributes<InheritedExportAttribute>())
				{
					AttributedExportDefinition attributedExportDefinition2 = this.CreateExportDefinition(type, inheritedExportAttribute);
					if (!this._contractNamesOnNonInterfaces.Contains(attributedExportDefinition2.ContractName))
					{
						list.Add(new ReflectionMemberExportDefinition(type.ToLazyMember(), attributedExportDefinition2, this));
						if (!type.IsInterface)
						{
							this._contractNamesOnNonInterfaces.Add(attributedExportDefinition2.ContractName);
						}
					}
				}
			}
			this._contractNamesOnNonInterfaces = null;
			return list;
		}

		private AttributedExportDefinition CreateExportDefinition(MemberInfo member, ExportAttribute exportAttribute)
		{
			string text = null;
			Type type = null;
			member.GetContractInfoFromExport(exportAttribute, out type, out text);
			return new AttributedExportDefinition(this, member, exportAttribute, type, text);
		}

		private IEnumerable<MemberInfo> GetExportMembers(Type type)
		{
			BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			if (type.IsAbstract)
			{
				flags &= ~BindingFlags.Instance;
			}
			else if (AttributedPartCreationInfo.IsExport(type))
			{
				yield return type;
			}
			foreach (FieldInfo fieldInfo in type.GetFields(flags))
			{
				if (AttributedPartCreationInfo.IsExport(fieldInfo))
				{
					yield return fieldInfo;
				}
			}
			FieldInfo[] array = null;
			foreach (PropertyInfo propertyInfo in type.GetProperties(flags))
			{
				if (AttributedPartCreationInfo.IsExport(propertyInfo))
				{
					yield return propertyInfo;
				}
			}
			PropertyInfo[] array2 = null;
			foreach (MethodInfo methodInfo in type.GetMethods(flags))
			{
				if (AttributedPartCreationInfo.IsExport(methodInfo))
				{
					yield return methodInfo;
				}
			}
			MethodInfo[] array3 = null;
			yield break;
		}

		private IEnumerable<Type> GetInheritedExports(Type type)
		{
			if (type.IsAbstract)
			{
				yield break;
			}
			Type currentType = type.BaseType;
			if (currentType == null)
			{
				yield break;
			}
			while (currentType != null && currentType.UnderlyingSystemType != CompositionServices.ObjectType)
			{
				if (AttributedPartCreationInfo.IsInheritedExport(currentType))
				{
					yield return currentType;
				}
				currentType = currentType.BaseType;
			}
			foreach (Type type2 in type.GetInterfaces())
			{
				if (AttributedPartCreationInfo.IsInheritedExport(type2))
				{
					yield return type2;
				}
			}
			Type[] array = null;
			yield break;
		}

		private static bool IsExport(ICustomAttributeProvider attributeProvider)
		{
			return attributeProvider.IsAttributeDefined<ExportAttribute>(false);
		}

		private static bool IsInheritedExport(ICustomAttributeProvider attributedProvider)
		{
			return attributedProvider.IsAttributeDefined<InheritedExportAttribute>(false);
		}

		private IEnumerable<ImportDefinition> GetImportDefinitions()
		{
			List<ImportDefinition> list = new List<ImportDefinition>();
			foreach (MemberInfo memberInfo in this.GetImportMembers(this._type))
			{
				ReflectionMemberImportDefinition reflectionMemberImportDefinition = AttributedModelDiscovery.CreateMemberImportDefinition(memberInfo, this);
				list.Add(reflectionMemberImportDefinition);
			}
			ConstructorInfo constructor = this.GetConstructor();
			if (constructor != null)
			{
				ParameterInfo[] parameters = constructor.GetParameters();
				for (int i = 0; i < parameters.Length; i++)
				{
					ReflectionParameterImportDefinition reflectionParameterImportDefinition = AttributedModelDiscovery.CreateParameterImportDefinition(parameters[i], this);
					list.Add(reflectionParameterImportDefinition);
				}
			}
			return list;
		}

		private IEnumerable<MemberInfo> GetImportMembers(Type type)
		{
			if (type.IsAbstract)
			{
				yield break;
			}
			foreach (MemberInfo memberInfo in this.GetDeclaredOnlyImportMembers(type))
			{
				yield return memberInfo;
			}
			IEnumerator<MemberInfo> enumerator = null;
			if (type.BaseType != null)
			{
				Type baseType = type.BaseType;
				while (baseType != null && baseType.UnderlyingSystemType != CompositionServices.ObjectType)
				{
					foreach (MemberInfo memberInfo2 in this.GetDeclaredOnlyImportMembers(baseType))
					{
						yield return memberInfo2;
					}
					enumerator = null;
					baseType = baseType.BaseType;
				}
				baseType = null;
			}
			yield break;
			yield break;
		}

		private IEnumerable<MemberInfo> GetDeclaredOnlyImportMembers(Type type)
		{
			BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			foreach (FieldInfo fieldInfo in type.GetFields(flags))
			{
				if (AttributedPartCreationInfo.IsImport(fieldInfo))
				{
					yield return fieldInfo;
				}
			}
			FieldInfo[] array = null;
			foreach (PropertyInfo propertyInfo in type.GetProperties(flags))
			{
				if (AttributedPartCreationInfo.IsImport(propertyInfo))
				{
					yield return propertyInfo;
				}
			}
			PropertyInfo[] array2 = null;
			yield break;
		}

		private static bool IsImport(ICustomAttributeProvider attributeProvider)
		{
			return attributeProvider.IsAttributeDefined<IAttributedImport>(false);
		}

		private readonly Type _type;

		private readonly bool _ignoreConstructorImports;

		private readonly ICompositionElement _origin;

		private PartCreationPolicyAttribute _partCreationPolicy;

		private ConstructorInfo _constructor;

		private IEnumerable<ExportDefinition> _exports;

		private IEnumerable<ImportDefinition> _imports;

		private HashSet<string> _contractNamesOnNonInterfaces;
	}
}
