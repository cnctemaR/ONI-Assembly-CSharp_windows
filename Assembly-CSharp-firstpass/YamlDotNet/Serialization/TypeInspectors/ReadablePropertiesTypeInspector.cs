using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.TypeInspectors
{
	public sealed class ReadablePropertiesTypeInspector : TypeInspectorSkeleton
	{
		public ReadablePropertiesTypeInspector(ITypeResolver typeResolver)
		{
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			this._typeResolver = typeResolver;
		}

		private static bool IsValidProperty(PropertyInfo property)
		{
			return property.CanRead && property.GetGetMethod().GetParameters().Length == 0;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			return from p in type.GetPublicProperties().Where<PropertyInfo>(new Func<PropertyInfo, bool>(ReadablePropertiesTypeInspector.IsValidProperty))
				select new ReadablePropertiesTypeInspector.ReflectionPropertyDescriptor(p, this._typeResolver);
		}

		private readonly ITypeResolver _typeResolver;

		private sealed class ReflectionPropertyDescriptor : IPropertyDescriptor
		{
			public ReflectionPropertyDescriptor(PropertyInfo propertyInfo, ITypeResolver typeResolver)
			{
				this._propertyInfo = propertyInfo;
				this._typeResolver = typeResolver;
				this.ScalarStyle = ScalarStyle.Any;
			}

			public string Name
			{
				get
				{
					return this._propertyInfo.Name;
				}
			}

			public Type Type
			{
				get
				{
					return this._propertyInfo.PropertyType;
				}
			}

			public Type TypeOverride { get; set; }

			public int Order { get; set; }

			public bool CanWrite
			{
				get
				{
					return this._propertyInfo.CanWrite;
				}
			}

			public ScalarStyle ScalarStyle { get; set; }

			public void Write(object target, object value)
			{
				this._propertyInfo.SetValue(target, value, null);
			}

			public T GetCustomAttribute<T>() where T : Attribute
			{
				object[] customAttributes = this._propertyInfo.GetCustomAttributes(typeof(T), true);
				return (T)((object)customAttributes.FirstOrDefault<object>());
			}

			public IObjectDescriptor Read(object target)
			{
				object obj = this._propertyInfo.ReadValue(target);
				Type type = this.TypeOverride ?? this._typeResolver.Resolve(this.Type, obj);
				return new ObjectDescriptor(obj, type, this.Type, this.ScalarStyle);
			}

			private readonly PropertyInfo _propertyInfo;

			private readonly ITypeResolver _typeResolver;
		}
	}
}
