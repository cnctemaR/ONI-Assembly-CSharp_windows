using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	internal static class CompositionServices
	{
		internal static Type GetDefaultTypeFromMember(this MemberInfo member)
		{
			Assumes.NotNull<MemberInfo>(member);
			MemberTypes memberType = member.MemberType;
			if (memberType <= MemberTypes.Property)
			{
				if (memberType != MemberTypes.Field)
				{
					if (memberType == MemberTypes.Property)
					{
						return ((PropertyInfo)member).PropertyType;
					}
				}
			}
			else if (memberType == MemberTypes.TypeInfo || memberType == MemberTypes.NestedType)
			{
				return (Type)member;
			}
			Assumes.IsTrue(member.MemberType == MemberTypes.Field);
			return ((FieldInfo)member).FieldType;
		}

		internal static Type AdjustSpecifiedTypeIdentityType(this Type specifiedContractType, MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Method)
			{
				return specifiedContractType;
			}
			return specifiedContractType.AdjustSpecifiedTypeIdentityType(member.GetDefaultTypeFromMember());
		}

		internal static Type AdjustSpecifiedTypeIdentityType(this Type specifiedContractType, Type memberType)
		{
			Assumes.NotNull<Type>(specifiedContractType);
			if (memberType != null && memberType.IsGenericType && specifiedContractType.IsGenericType)
			{
				if (specifiedContractType.ContainsGenericParameters && !memberType.ContainsGenericParameters)
				{
					Type[] genericArguments = memberType.GetGenericArguments();
					Type[] genericArguments2 = specifiedContractType.GetGenericArguments();
					if (genericArguments.Length == genericArguments2.Length)
					{
						return specifiedContractType.MakeGenericType(genericArguments);
					}
				}
				else if (specifiedContractType.ContainsGenericParameters && memberType.ContainsGenericParameters)
				{
					IList<Type> pureGenericParameters = memberType.GetPureGenericParameters();
					if (specifiedContractType.GetPureGenericArity() == pureGenericParameters.Count)
					{
						return specifiedContractType.GetGenericTypeDefinition().MakeGenericType(pureGenericParameters.ToArray<Type>());
					}
				}
			}
			return specifiedContractType;
		}

		private static string AdjustTypeIdentity(string originalTypeIdentity, Type typeIdentityType)
		{
			return GenericServices.GetGenericName(originalTypeIdentity, GenericServices.GetGenericParametersOrder(typeIdentityType), typeIdentityType.GetPureGenericArity());
		}

		internal static void GetContractInfoFromExport(this MemberInfo member, ExportAttribute export, out Type typeIdentityType, out string contractName)
		{
			typeIdentityType = member.GetTypeIdentityTypeFromExport(export);
			if (!string.IsNullOrEmpty(export.ContractName))
			{
				contractName = export.ContractName;
				return;
			}
			contractName = member.GetTypeIdentityFromExport(typeIdentityType);
		}

		internal static string GetTypeIdentityFromExport(this MemberInfo member, Type typeIdentityType)
		{
			if (typeIdentityType != null)
			{
				string text = AttributedModelServices.GetTypeIdentity(typeIdentityType);
				if (typeIdentityType.ContainsGenericParameters)
				{
					text = CompositionServices.AdjustTypeIdentity(text, typeIdentityType);
				}
				return text;
			}
			MethodInfo methodInfo = member as MethodInfo;
			Assumes.NotNull<MethodInfo>(methodInfo);
			return AttributedModelServices.GetTypeIdentity(methodInfo);
		}

		private static Type GetTypeIdentityTypeFromExport(this MemberInfo member, ExportAttribute export)
		{
			if (export.ContractType != null)
			{
				return export.ContractType.AdjustSpecifiedTypeIdentityType(member);
			}
			if (member.MemberType == MemberTypes.Method)
			{
				return null;
			}
			return member.GetDefaultTypeFromMember();
		}

		internal static bool IsContractNameSameAsTypeIdentity(this ExportAttribute export)
		{
			return string.IsNullOrEmpty(export.ContractName);
		}

		internal static Type GetContractTypeFromImport(this IAttributedImport import, ImportType importType)
		{
			if (import.ContractType != null)
			{
				return import.ContractType.AdjustSpecifiedTypeIdentityType(importType.ContractType);
			}
			return importType.ContractType;
		}

		internal static string GetContractNameFromImport(this IAttributedImport import, ImportType importType)
		{
			if (!string.IsNullOrEmpty(import.ContractName))
			{
				return import.ContractName;
			}
			return AttributedModelServices.GetContractName(import.GetContractTypeFromImport(importType));
		}

		internal static string GetTypeIdentityFromImport(this IAttributedImport import, ImportType importType)
		{
			Type contractTypeFromImport = import.GetContractTypeFromImport(importType);
			if (contractTypeFromImport == CompositionServices.ObjectType)
			{
				return null;
			}
			return AttributedModelServices.GetTypeIdentity(contractTypeFromImport);
		}

		internal static IDictionary<string, object> GetPartMetadataForType(this Type type, CreationPolicy creationPolicy)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>(StringComparers.MetadataKeyNames);
			if (creationPolicy != CreationPolicy.Any)
			{
				dictionary.Add("System.ComponentModel.Composition.CreationPolicy", creationPolicy);
			}
			foreach (PartMetadataAttribute partMetadataAttribute in type.GetAttributes<PartMetadataAttribute>())
			{
				if (!CompositionServices.reservedMetadataNames.Contains(partMetadataAttribute.Name, StringComparers.MetadataKeyNames) && !dictionary.ContainsKey(partMetadataAttribute.Name))
				{
					dictionary.Add(partMetadataAttribute.Name, partMetadataAttribute.Value);
				}
			}
			if (type.ContainsGenericParameters)
			{
				dictionary.Add("System.ComponentModel.Composition.IsGenericPart", true);
				Type[] genericArguments = type.GetGenericArguments();
				dictionary.Add("System.ComponentModel.Composition.GenericPartArity", genericArguments.Length);
				bool flag = false;
				object[] array = new object[genericArguments.Length];
				GenericParameterAttributes[] array2 = new GenericParameterAttributes[genericArguments.Length];
				for (int j = 0; j < genericArguments.Length; j++)
				{
					Type type2 = genericArguments[j];
					Type[] array3 = type2.GetGenericParameterConstraints();
					if (array3.Length == 0)
					{
						array3 = null;
					}
					GenericParameterAttributes genericParameterAttributes = type2.GenericParameterAttributes;
					if (array3 != null || genericParameterAttributes != GenericParameterAttributes.None)
					{
						array[j] = array3;
						array2[j] = genericParameterAttributes;
						flag = true;
					}
				}
				if (flag)
				{
					dictionary.Add("System.ComponentModel.Composition.GenericParameterConstraints", array);
					dictionary.Add("System.ComponentModel.Composition.GenericParameterAttributes", array2);
				}
			}
			if (dictionary.Count == 0)
			{
				return MetadataServices.EmptyMetadata;
			}
			return dictionary;
		}

		internal static void TryExportMetadataForMember(this MemberInfo member, out IDictionary<string, object> dictionary)
		{
			dictionary = new Dictionary<string, object>();
			foreach (Attribute attribute in member.GetAttributes<Attribute>())
			{
				ExportMetadataAttribute exportMetadataAttribute = attribute as ExportMetadataAttribute;
				if (exportMetadataAttribute != null)
				{
					if (CompositionServices.reservedMetadataNames.Contains(exportMetadataAttribute.Name, StringComparers.MetadataKeyNames))
					{
						throw ExceptionBuilder.CreateDiscoveryException(Strings.Discovery_ReservedMetadataNameUsed, new string[]
						{
							member.GetDisplayName(),
							exportMetadataAttribute.Name
						});
					}
					if (!dictionary.TryContributeMetadataValue(exportMetadataAttribute.Name, exportMetadataAttribute.Value, null, exportMetadataAttribute.IsMultiple))
					{
						throw ExceptionBuilder.CreateDiscoveryException(Strings.Discovery_DuplicateMetadataNameValues, new string[]
						{
							member.GetDisplayName(),
							exportMetadataAttribute.Name
						});
					}
				}
				else
				{
					Type type = attribute.GetType();
					if (type != CompositionServices.ExportAttributeType && type.IsAttributeDefined<MetadataAttributeAttribute>(true))
					{
						bool flag = false;
						AttributeUsageAttribute firstAttribute = type.GetFirstAttribute<AttributeUsageAttribute>(true);
						if (firstAttribute != null)
						{
							flag = firstAttribute.AllowMultiple;
						}
						foreach (PropertyInfo propertyInfo in type.GetProperties())
						{
							if (!(propertyInfo.DeclaringType == CompositionServices.ExportAttributeType) && !(propertyInfo.DeclaringType == CompositionServices.AttributeType))
							{
								if (CompositionServices.reservedMetadataNames.Contains(propertyInfo.Name, StringComparers.MetadataKeyNames))
								{
									throw ExceptionBuilder.CreateDiscoveryException(Strings.Discovery_ReservedMetadataNameUsed, new string[]
									{
										member.GetDisplayName(),
										exportMetadataAttribute.Name
									});
								}
								object value = propertyInfo.GetValue(attribute, null);
								if (value != null && !CompositionServices.IsValidAttributeType(value.GetType()))
								{
									throw ExceptionBuilder.CreateDiscoveryException(Strings.Discovery_MetadataContainsValueWithInvalidType, new string[]
									{
										propertyInfo.GetDisplayName(),
										value.GetType().GetDisplayName()
									});
								}
								if (!dictionary.TryContributeMetadataValue(propertyInfo.Name, value, propertyInfo.PropertyType, flag))
								{
									throw ExceptionBuilder.CreateDiscoveryException(Strings.Discovery_DuplicateMetadataNameValues, new string[]
									{
										member.GetDisplayName(),
										propertyInfo.Name
									});
								}
							}
						}
					}
				}
			}
			foreach (string text in dictionary.Keys.ToArray<string>())
			{
				CompositionServices.MetadataList metadataList = dictionary[text] as CompositionServices.MetadataList;
				if (metadataList != null)
				{
					dictionary[text] = metadataList.ToArray();
				}
			}
		}

		private static bool TryContributeMetadataValue(this IDictionary<string, object> dictionary, string name, object value, Type valueType, bool allowsMultiple)
		{
			object obj;
			if (!dictionary.TryGetValue(name, out obj))
			{
				if (allowsMultiple)
				{
					CompositionServices.MetadataList metadataList = new CompositionServices.MetadataList();
					metadataList.Add(value, valueType);
					value = metadataList;
				}
				dictionary.Add(name, value);
			}
			else
			{
				CompositionServices.MetadataList metadataList2 = obj as CompositionServices.MetadataList;
				if (!allowsMultiple || metadataList2 == null)
				{
					dictionary.Remove(name);
					return false;
				}
				metadataList2.Add(value, valueType);
			}
			return true;
		}

		internal static IEnumerable<KeyValuePair<string, Type>> GetRequiredMetadata(Type metadataViewType)
		{
			if (metadataViewType == null || ExportServices.IsDefaultMetadataViewType(metadataViewType) || ExportServices.IsDictionaryConstructorViewType(metadataViewType) || !metadataViewType.IsInterface)
			{
				return Enumerable.Empty<KeyValuePair<string, Type>>();
			}
			return from property in (from property in metadataViewType.GetAllProperties()
					where property.GetFirstAttribute<DefaultValueAttribute>() == null
					select property).ToList<PropertyInfo>()
				select new KeyValuePair<string, Type>(property.Name, property.PropertyType);
		}

		internal static IDictionary<string, object> GetImportMetadata(ImportType importType, IAttributedImport attributedImport)
		{
			return CompositionServices.GetImportMetadata(importType.ContractType, attributedImport);
		}

		internal static IDictionary<string, object> GetImportMetadata(Type type, IAttributedImport attributedImport)
		{
			Dictionary<string, object> dictionary = null;
			if (type.IsGenericType)
			{
				dictionary = new Dictionary<string, object>();
				if (type.ContainsGenericParameters)
				{
					dictionary["System.ComponentModel.Composition.GenericImportParametersOrderMetadataName"] = GenericServices.GetGenericParametersOrder(type);
				}
				else
				{
					dictionary["System.ComponentModel.Composition.GenericContractName"] = ContractNameServices.GetTypeIdentity(type.GetGenericTypeDefinition());
					dictionary["System.ComponentModel.Composition.GenericParameters"] = type.GetGenericArguments();
				}
			}
			if (attributedImport != null && attributedImport.Source != ImportSource.Any)
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, object>();
				}
				dictionary["System.ComponentModel.Composition.ImportSource"] = attributedImport.Source;
			}
			if (dictionary != null)
			{
				return dictionary.AsReadOnly();
			}
			return MetadataServices.EmptyMetadata;
		}

		internal static object GetExportedValueFromComposedPart(ImportEngine engine, ComposablePart part, ExportDefinition definition)
		{
			if (engine != null)
			{
				try
				{
					engine.SatisfyImports(part);
				}
				catch (CompositionException ex)
				{
					throw ExceptionBuilder.CreateCannotGetExportedValue(part, definition, ex);
				}
			}
			object exportedValue;
			try
			{
				exportedValue = part.GetExportedValue(definition);
			}
			catch (ComposablePartException ex2)
			{
				throw ExceptionBuilder.CreateCannotGetExportedValue(part, definition, ex2);
			}
			return exportedValue;
		}

		internal static bool IsRecomposable(this ComposablePart part)
		{
			return part.ImportDefinitions.Any<ImportDefinition>((ImportDefinition import) => import.IsRecomposable);
		}

		internal static CompositionResult TryInvoke(Action action)
		{
			CompositionResult compositionResult;
			try
			{
				action();
				compositionResult = CompositionResult.SucceededResult;
			}
			catch (CompositionException ex)
			{
				compositionResult = new CompositionResult(ex.Errors);
			}
			return compositionResult;
		}

		internal static CompositionResult TryFire<TEventArgs>(EventHandler<TEventArgs> _delegate, object sender, TEventArgs e) where TEventArgs : EventArgs
		{
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			foreach (EventHandler<TEventArgs> eventHandler in _delegate.GetInvocationList())
			{
				try
				{
					eventHandler(sender, e);
				}
				catch (CompositionException ex)
				{
					compositionResult = compositionResult.MergeErrors(ex.Errors);
				}
			}
			return compositionResult;
		}

		internal static CreationPolicy GetRequiredCreationPolicy(this ImportDefinition definition)
		{
			ContractBasedImportDefinition contractBasedImportDefinition = definition as ContractBasedImportDefinition;
			if (contractBasedImportDefinition != null)
			{
				return contractBasedImportDefinition.RequiredCreationPolicy;
			}
			return CreationPolicy.Any;
		}

		internal static bool IsAtMostOne(this ImportCardinality cardinality)
		{
			return cardinality == ImportCardinality.ZeroOrOne || cardinality == ImportCardinality.ExactlyOne;
		}

		private static bool IsValidAttributeType(Type type)
		{
			return CompositionServices.IsValidAttributeType(type, true);
		}

		private static bool IsValidAttributeType(Type type, bool arrayAllowed)
		{
			Assumes.NotNull<Type>(type);
			return type.IsPrimitive || type == typeof(string) || (type.IsEnum && type.IsVisible) || typeof(Type).IsAssignableFrom(type) || (arrayAllowed && type.IsArray && type.GetArrayRank() == 1 && CompositionServices.IsValidAttributeType(type.GetElementType(), false));
		}

		internal static readonly Type InheritedExportAttributeType = typeof(InheritedExportAttribute);

		internal static readonly Type ExportAttributeType = typeof(ExportAttribute);

		internal static readonly Type AttributeType = typeof(Attribute);

		internal static readonly Type ObjectType = typeof(object);

		private static readonly string[] reservedMetadataNames = new string[] { "System.ComponentModel.Composition.CreationPolicy" };

		private class MetadataList
		{
			public void Add(object item, Type itemType)
			{
				this._containsNulls |= item == null;
				if (itemType == CompositionServices.MetadataList.ObjectType)
				{
					itemType = null;
				}
				if (itemType == null && item != null)
				{
					itemType = item.GetType();
				}
				if (item is Type)
				{
					itemType = CompositionServices.MetadataList.TypeType;
				}
				if (itemType != null)
				{
					this.InferArrayType(itemType);
				}
				this._innerList.Add(item);
			}

			private void InferArrayType(Type itemType)
			{
				Assumes.NotNull<Type>(itemType);
				if (this._arrayType == null)
				{
					this._arrayType = itemType;
					return;
				}
				if (this._arrayType != itemType)
				{
					this._arrayType = CompositionServices.MetadataList.ObjectType;
				}
			}

			public Array ToArray()
			{
				if (this._arrayType == null)
				{
					this._arrayType = CompositionServices.MetadataList.ObjectType;
				}
				else if (this._containsNulls && this._arrayType.IsValueType)
				{
					this._arrayType = CompositionServices.MetadataList.ObjectType;
				}
				Array array = Array.CreateInstance(this._arrayType, this._innerList.Count);
				for (int i = 0; i < array.Length; i++)
				{
					array.SetValue(this._innerList[i], i);
				}
				return array;
			}

			private Type _arrayType;

			private bool _containsNulls;

			private static readonly Type ObjectType = typeof(object);

			private static readonly Type TypeType = typeof(Type);

			private Collection<object> _innerList = new Collection<object>();
		}
	}
}
