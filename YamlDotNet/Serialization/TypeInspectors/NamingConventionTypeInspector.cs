using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization.TypeInspectors
{
	public sealed class NamingConventionTypeInspector : TypeInspectorSkeleton
	{
		public NamingConventionTypeInspector(ITypeInspector innerTypeDescriptor, INamingConvention namingConvention)
		{
			if (innerTypeDescriptor == null)
			{
				throw new ArgumentNullException("innerTypeDescriptor");
			}
			this.innerTypeDescriptor = innerTypeDescriptor;
			if (namingConvention == null)
			{
				throw new ArgumentNullException("namingConvention");
			}
			this.namingConvention = namingConvention;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			return from p in this.innerTypeDescriptor.GetProperties(type, container)
				select new PropertyDescriptor(p)
				{
					Name = this.namingConvention.Apply(p.Name)
				};
		}

		private readonly ITypeInspector innerTypeDescriptor;

		private readonly INamingConvention namingConvention;
	}
}
