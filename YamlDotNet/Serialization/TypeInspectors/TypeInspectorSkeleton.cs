using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace YamlDotNet.Serialization.TypeInspectors
{
	public abstract class TypeInspectorSkeleton : ITypeInspector
	{
		public abstract IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container);

		public IPropertyDescriptor GetProperty(Type type, object container, string name, bool ignoreUnmatched)
		{
			IEnumerable<IPropertyDescriptor> enumerable = from p in this.GetProperties(type, container)
				where p.Name == name
				select p;
			IPropertyDescriptor propertyDescriptor;
			using (IEnumerator<IPropertyDescriptor> enumerator = enumerable.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					if (!ignoreUnmatched)
					{
						throw new SerializationException(string.Format(CultureInfo.InvariantCulture, "Property '{0}' not found on type '{1}'.", new object[] { name, type.FullName }));
					}
					propertyDescriptor = null;
				}
				else
				{
					IPropertyDescriptor propertyDescriptor2 = enumerator.Current;
					if (enumerator.MoveNext())
					{
						IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
						string text = "Multiple properties with the name/alias '{0}' already exists on type '{1}', maybe you're misusing YamlAlias or maybe you are using the wrong naming convention? The matching properties are: {2}";
						object[] array = new object[3];
						array[0] = name;
						array[1] = type.FullName;
						array[2] = string.Join(", ", enumerable.Select<IPropertyDescriptor, string>((IPropertyDescriptor p) => p.Name).ToArray<string>());
						throw new SerializationException(string.Format(invariantCulture, text, array));
					}
					propertyDescriptor = propertyDescriptor2;
				}
			}
			return propertyDescriptor;
		}
	}
}
