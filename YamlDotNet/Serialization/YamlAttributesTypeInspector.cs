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
					YamlAliasAttribute customAttribute = p.GetCustomAttribute<YamlAliasAttribute>();
					if (customAttribute != null)
					{
						propertyDescriptor.Name = customAttribute.Alias;
					}
					YamlMemberAttribute customAttribute2 = p.GetCustomAttribute<YamlMemberAttribute>();
					if (customAttribute2 != null)
					{
						if (customAttribute2.SerializeAs != null)
						{
							propertyDescriptor.TypeOverride = customAttribute2.SerializeAs;
						}
						propertyDescriptor.Order = customAttribute2.Order;
						propertyDescriptor.ScalarStyle = customAttribute2.ScalarStyle;
						if (customAttribute2.Alias != null)
						{
							if (customAttribute != null)
							{
								throw new InvalidOperationException("Mixing YamlAlias(...) with YamlMember(Alias = ...) is an error. The YamlAlias attribute is obsolete and should be removed.");
							}
							propertyDescriptor.Name = customAttribute2.Alias;
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
