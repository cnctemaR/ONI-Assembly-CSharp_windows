using System;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ImportType
	{
		public ImportType(Type type, ImportCardinality cardinality)
		{
			Assumes.NotNull<Type>(type);
			this._type = type;
			this._contractType = type;
			if (cardinality == ImportCardinality.ZeroOrMore)
			{
				this._isAssignableCollectionType = ImportType.IsTypeAssignableCollectionType(type);
				this._contractType = this.CheckForCollection(type);
			}
			this._isOpenGeneric = type.ContainsGenericParameters;
			this._contractType = this.CheckForLazyAndPartCreator(this._contractType);
		}

		public bool IsAssignableCollectionType
		{
			get
			{
				return this._isAssignableCollectionType;
			}
		}

		public Type ElementType { get; private set; }

		public Type ActualType
		{
			get
			{
				return this._type;
			}
		}

		public bool IsPartCreator { get; private set; }

		public Type ContractType
		{
			get
			{
				return this._contractType;
			}
		}

		public Func<Export, object> CastExport
		{
			get
			{
				Assumes.IsTrue(!this._isOpenGeneric);
				return this._castSingleValue;
			}
		}

		public Type MetadataViewType { get; private set; }

		private Type CheckForCollection(Type type)
		{
			this.ElementType = CollectionServices.GetEnumerableElementType(type);
			if (this.ElementType != null)
			{
				return this.ElementType;
			}
			return type;
		}

		private static bool IsGenericDescendentOf(Type type, Type baseGenericTypeDefinition)
		{
			return !(type == typeof(object)) && !(type == null) && ((type.IsGenericType && type.GetGenericTypeDefinition() == baseGenericTypeDefinition) || ImportType.IsGenericDescendentOf(type.BaseType, baseGenericTypeDefinition));
		}

		public static bool IsDescendentOf(Type type, Type baseType)
		{
			Assumes.NotNull<Type>(type);
			Assumes.NotNull<Type>(baseType);
			if (!baseType.IsGenericTypeDefinition)
			{
				return baseType.IsAssignableFrom(type);
			}
			return ImportType.IsGenericDescendentOf(type, baseType.GetGenericTypeDefinition());
		}

		private Type CheckForLazyAndPartCreator(Type type)
		{
			if (type.IsGenericType)
			{
				Type underlyingSystemType = type.GetGenericTypeDefinition().UnderlyingSystemType;
				Type[] genericArguments = type.GetGenericArguments();
				if (underlyingSystemType == ImportType.LazyOfTType)
				{
					if (!this._isOpenGeneric)
					{
						this._castSingleValue = ExportServices.CreateStronglyTypedLazyFactory(genericArguments[0].UnderlyingSystemType, null);
					}
					return genericArguments[0];
				}
				if (underlyingSystemType == ImportType.LazyOfTMType)
				{
					this.MetadataViewType = genericArguments[1];
					if (!this._isOpenGeneric)
					{
						this._castSingleValue = ExportServices.CreateStronglyTypedLazyFactory(genericArguments[0].UnderlyingSystemType, genericArguments[1].UnderlyingSystemType);
					}
					return genericArguments[0];
				}
				if (underlyingSystemType != null && ImportType.IsDescendentOf(underlyingSystemType, ImportType.ExportFactoryOfTType))
				{
					this.IsPartCreator = true;
					if (genericArguments.Length == 1)
					{
						if (!this._isOpenGeneric)
						{
							this._castSingleValue = new ExportFactoryCreator(underlyingSystemType).CreateStronglyTypedExportFactoryFactory(genericArguments[0].UnderlyingSystemType, null);
						}
					}
					else
					{
						if (genericArguments.Length != 2)
						{
							throw ExceptionBuilder.ExportFactory_TooManyGenericParameters(underlyingSystemType.FullName);
						}
						if (!this._isOpenGeneric)
						{
							this._castSingleValue = new ExportFactoryCreator(underlyingSystemType).CreateStronglyTypedExportFactoryFactory(genericArguments[0].UnderlyingSystemType, genericArguments[1].UnderlyingSystemType);
						}
						this.MetadataViewType = genericArguments[1];
					}
					return genericArguments[0];
				}
			}
			return type;
		}

		private static bool IsTypeAssignableCollectionType(Type type)
		{
			return type.IsArray || CollectionServices.IsEnumerableOfT(type);
		}

		private static readonly Type LazyOfTType = typeof(Lazy<>);

		private static readonly Type LazyOfTMType = typeof(Lazy<, >);

		private static readonly Type ExportFactoryOfTType = typeof(ExportFactory<>);

		private static readonly Type ExportFactoryOfTMType = typeof(ExportFactory<, >);

		private readonly Type _type;

		private readonly bool _isAssignableCollectionType;

		private readonly Type _contractType;

		private Func<Export, object> _castSingleValue;

		private bool _isOpenGeneric;
	}
}
