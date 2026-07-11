using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.TypeInspectors;

namespace YamlDotNet.Serialization
{
	public abstract class BuilderSkeleton<TBuilder> where TBuilder : BuilderSkeleton<TBuilder>
	{
		internal BuilderSkeleton()
		{
			this.overrides = new YamlAttributeOverrides();
			this.typeConverterFactories = new LazyComponentRegistrationList<Nothing, IYamlTypeConverter>();
			this.typeConverterFactories.Add(typeof(GuidConverter), (Nothing _) => new GuidConverter(false));
			this.typeInspectorFactories = new LazyComponentRegistrationList<ITypeInspector, ITypeInspector>();
		}

		protected abstract TBuilder Self { get; }

		internal ITypeInspector BuildTypeInspector()
		{
			return this.typeInspectorFactories.BuildComponentChain(new ReadablePropertiesTypeInspector(this.typeResolver));
		}

		public TBuilder WithNamingConvention(INamingConvention namingConvention)
		{
			if (namingConvention == null)
			{
				throw new ArgumentNullException("namingConvention");
			}
			this.namingConvention = namingConvention;
			return this.Self;
		}

		public TBuilder WithTypeResolver(ITypeResolver typeResolver)
		{
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			this.typeResolver = typeResolver;
			return this.Self;
		}

		public TBuilder WithAttributeOverride<TClass>(Expression<Func<TClass, object>> propertyAccessor, Attribute attribute)
		{
			this.overrides.Add<TClass>(propertyAccessor, attribute);
			return this.Self;
		}

		public TBuilder WithAttributeOverride(Type type, string member, Attribute attribute)
		{
			this.overrides.Add(type, member, attribute);
			return this.Self;
		}

		public TBuilder WithTypeConverter(IYamlTypeConverter typeConverter)
		{
			return this.WithTypeConverter(typeConverter, delegate(IRegistrationLocationSelectionSyntax<IYamlTypeConverter> w)
			{
				w.OnTop();
			});
		}

		public TBuilder WithTypeConverter(IYamlTypeConverter typeConverter, Action<IRegistrationLocationSelectionSyntax<IYamlTypeConverter>> where)
		{
			if (typeConverter == null)
			{
				throw new ArgumentNullException("typeConverter");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(this.typeConverterFactories.CreateRegistrationLocationSelector(typeConverter.GetType(), (Nothing _) => typeConverter));
			return this.Self;
		}

		public TBuilder WithTypeConverter<TYamlTypeConverter>(WrapperFactory<IYamlTypeConverter, IYamlTypeConverter> typeConverterFactory, Action<ITrackingRegistrationLocationSelectionSyntax<IYamlTypeConverter>> where) where TYamlTypeConverter : IYamlTypeConverter
		{
			if (typeConverterFactory == null)
			{
				throw new ArgumentNullException("typeConverterFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(this.typeConverterFactories.CreateTrackingRegistrationLocationSelector(typeof(TYamlTypeConverter), (IYamlTypeConverter wrapped, Nothing _) => typeConverterFactory(wrapped)));
			return this.Self;
		}

		public TBuilder WithoutTypeConverter<TYamlTypeConverter>() where TYamlTypeConverter : IYamlTypeConverter
		{
			return this.WithoutTypeConverter(typeof(TYamlTypeConverter));
		}

		public TBuilder WithoutTypeConverter(Type converterType)
		{
			if (converterType == null)
			{
				throw new ArgumentNullException("converterType");
			}
			this.typeConverterFactories.Remove(converterType);
			return this.Self;
		}

		public TBuilder WithTypeInspector<TTypeInspector>(Func<ITypeInspector, TTypeInspector> typeInspectorFactory) where TTypeInspector : ITypeInspector
		{
			return this.WithTypeInspector<TTypeInspector>(typeInspectorFactory, delegate(IRegistrationLocationSelectionSyntax<ITypeInspector> w)
			{
				w.OnTop();
			});
		}

		public TBuilder WithTypeInspector<TTypeInspector>(Func<ITypeInspector, TTypeInspector> typeInspectorFactory, Action<IRegistrationLocationSelectionSyntax<ITypeInspector>> where) where TTypeInspector : ITypeInspector
		{
			if (typeInspectorFactory == null)
			{
				throw new ArgumentNullException("typeInspectorFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(this.typeInspectorFactories.CreateRegistrationLocationSelector(typeof(TTypeInspector), (ITypeInspector inner) => typeInspectorFactory(inner)));
			return this.Self;
		}

		public TBuilder WithTypeInspector<TTypeInspector>(WrapperFactory<ITypeInspector, ITypeInspector, TTypeInspector> typeInspectorFactory, Action<ITrackingRegistrationLocationSelectionSyntax<ITypeInspector>> where) where TTypeInspector : ITypeInspector
		{
			if (typeInspectorFactory == null)
			{
				throw new ArgumentNullException("typeInspectorFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(this.typeInspectorFactories.CreateTrackingRegistrationLocationSelector(typeof(TTypeInspector), (ITypeInspector wrapped, ITypeInspector inner) => typeInspectorFactory(wrapped, inner)));
			return this.Self;
		}

		public TBuilder WithoutTypeInspector<TTypeInspector>() where TTypeInspector : ITypeInspector
		{
			return this.WithoutTypeInspector(typeof(ITypeInspector));
		}

		public TBuilder WithoutTypeInspector(Type inspectorType)
		{
			if (inspectorType == null)
			{
				throw new ArgumentNullException("inspectorType");
			}
			this.typeInspectorFactories.Remove(inspectorType);
			return this.Self;
		}

		protected IEnumerable<IYamlTypeConverter> BuildTypeConverters()
		{
			return this.typeConverterFactories.BuildComponentList<IYamlTypeConverter>();
		}

		internal INamingConvention namingConvention;

		internal ITypeResolver typeResolver;

		internal readonly YamlAttributeOverrides overrides;

		internal readonly LazyComponentRegistrationList<Nothing, IYamlTypeConverter> typeConverterFactories;

		internal readonly LazyComponentRegistrationList<ITypeInspector, ITypeInspector> typeInspectorFactories;
	}
}
