using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization.TypeInspectors;

namespace YamlDotNet.Serialization
{
	public sealed class YamlAttributesTypeInspector : TypeInspectorSkeleton
	{
		public YamlAttributesTypeInspector(ITypeInspector innerTypeDescriptor)
		{
			this.innerTypeDescriptor = innerTypeDescriptor;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			return from p in (from p in this.innerTypeDescriptor.GetProperties(type, container)
					where p.GetCustomAttribute<YamlIgnoreAttribute>() == null
					select p).Select<IPropertyDescriptor, IPropertyDescriptor>(delegate(IPropertyDescriptor p)
				{
					PropertyDescriptor propertyDescriptor = new PropertyDescriptor(p);
					YamlMemberAttribute customAttribute = p.GetCustomAttribute<YamlMemberAttribute>();
					if (customAttribute != null)
					{
						if (customAttribute.SerializeAs != null)
						{
							propertyDescriptor.TypeOverride = customAttribute.SerializeAs;
						}
						propertyDescriptor.Order = customAttribute.Order;
						propertyDescriptor.ScalarStyle = customAttribute.ScalarStyle;
						if (customAttribute.Alias != null)
						{
							propertyDescriptor.Name = customAttribute.Alias;
						}
					}
					return propertyDescriptor;
				})
				orderby p.Order
				select p;
		}

		private readonly ITypeInspector innerTypeDescriptor;
	}
}
