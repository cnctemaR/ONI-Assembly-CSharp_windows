using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Runtime.Serialization
{
	public static class DataContractSerializerExtensions
	{
		public static ISerializationSurrogateProvider GetSerializationSurrogateProvider(this DataContractSerializer serializer)
		{
			DataContractSerializerExtensions.SurrogateProviderAdapter surrogateProviderAdapter = serializer.DataContractSurrogate as DataContractSerializerExtensions.SurrogateProviderAdapter;
			if (surrogateProviderAdapter != null)
			{
				return surrogateProviderAdapter.Provider;
			}
			return null;
		}

		public static void SetSerializationSurrogateProvider(this DataContractSerializer serializer, ISerializationSurrogateProvider provider)
		{
			IDataContractSurrogate dataContractSurrogate = ((provider != null) ? new DataContractSerializerExtensions.SurrogateProviderAdapter(provider) : null);
			typeof(DataContractSerializer).GetField("dataContractSurrogate", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(serializer, dataContractSurrogate);
		}

		private class SurrogateProviderAdapter : IDataContractSurrogate
		{
			public SurrogateProviderAdapter(ISerializationSurrogateProvider provider)
			{
				this._provider = provider;
			}

			public ISerializationSurrogateProvider Provider
			{
				get
				{
					return this._provider;
				}
			}

			public object GetCustomDataToExport(Type clrType, Type dataContractType)
			{
				throw NotImplemented.ByDesign;
			}

			public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
			{
				throw NotImplemented.ByDesign;
			}

			public Type GetDataContractType(Type type)
			{
				return this._provider.GetSurrogateType(type);
			}

			public object GetDeserializedObject(object obj, Type targetType)
			{
				return this._provider.GetDeserializedObject(obj, targetType);
			}

			public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
			{
				throw NotImplemented.ByDesign;
			}

			public object GetObjectToSerialize(object obj, Type targetType)
			{
				return this._provider.GetObjectToSerialize(obj, targetType);
			}

			public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
			{
				throw NotImplemented.ByDesign;
			}

			public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
			{
				throw NotImplemented.ByDesign;
			}

			private ISerializationSurrogateProvider _provider;
		}
	}
}
