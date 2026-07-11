using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Serialization.TypeInspectors;

namespace YamlDotNet.Serialization
{
	public sealed class YamlAttributeOverridesInspector : TypeInspectorSkeleton
	{
		public YamlAttributeOverridesInspector(ITypeInspector innerTypeDescriptor, YamlAttributeOverrides overrides)
		{
			this.innerTypeDescriptor = innerTypeDescriptor;
			this.overrides = overrides;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			if (this.overrides == null)
			{
				return this.innerTypeDescriptor.GetProperties(type, container);
			}
			return from p in this.innerTypeDescriptor.GetProperties(type, container)
				select new YamlAttributeOverridesInspector.OverridePropertyDescriptor(p, this.overrides, type);
		}

		private readonly ITypeInspector innerTypeDescriptor;

		private readonly YamlAttributeOverrides overrides;

		public sealed class OverridePropertyDescriptor : IPropertyDescriptor
		{
			public OverridePropertyDescriptor(IPropertyDescriptor baseDescriptor, YamlAttributeOverrides overrides, Type classType)
			{
				this.baseDescriptor = baseDescriptor;
				this.overrides = overrides;
				this.classType = classType;
			}

			public string Name
			{
				get
				{
					return this.baseDescriptor.Name;
				}
			}

			public bool CanWrite
			{
				get
				{
					return this.baseDescriptor.CanWrite;
				}
			}

			public Type Type
			{
				get
				{
					return this.baseDescriptor.Type;
				}
			}

			public Type TypeOverride
			{
				get
				{
					return this.baseDescriptor.TypeOverride;
				}
				set
				{
					this.baseDescriptor.TypeOverride = value;
				}
			}

			public int Order
			{
				get
				{
					return this.baseDescriptor.Order;
				}
				set
				{
					this.baseDescriptor.Order = value;
				}
			}

			public ScalarStyle ScalarStyle
			{
				get
				{
					return this.baseDescriptor.ScalarStyle;
				}
				set
				{
					this.baseDescriptor.ScalarStyle = value;
				}
			}

			public void Write(object target, object value)
			{
				this.baseDescriptor.Write(target, value);
			}

			public T GetCustomAttribute<T>() where T : Attribute
			{
				T attribute = this.overrides.GetAttribute<T>(this.classType, this.Name);
				T t;
				if ((t = attribute) == null)
				{
					t = this.baseDescriptor.GetCustomAttribute<T>();
				}
				return t;
			}

			public IObjectDescriptor Read(object target)
			{
				return this.baseDescriptor.Read(target);
			}

			private readonly IPropertyDescriptor baseDescriptor;

			private readonly YamlAttributeOverrides overrides;

			private readonly Type classType;
		}
	}
}
