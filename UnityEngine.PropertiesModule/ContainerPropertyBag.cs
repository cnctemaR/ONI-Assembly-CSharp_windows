using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public abstract class ContainerPropertyBag<TContainer> : PropertyBag<TContainer>, INamedProperties<TContainer>
	{
		static ContainerPropertyBag()
		{
			bool flag = !TypeTraits.IsContainer(typeof(TContainer));
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Failed to create a property bag for Type=[{0}]. The type is not a valid container type.", typeof(TContainer)));
			}
		}

		protected void AddProperty<TValue>(Property<TContainer, TValue> property)
		{
			this.m_PropertiesList.Add(property);
			this.m_PropertiesHash.Add(property.Name, property);
		}

		public override PropertyCollection<TContainer> GetProperties()
		{
			return new PropertyCollection<TContainer>(this.m_PropertiesList);
		}

		public override PropertyCollection<TContainer> GetProperties(ref TContainer container)
		{
			return new PropertyCollection<TContainer>(this.m_PropertiesList);
		}

		public bool TryGetProperty(ref TContainer container, string name, out IProperty<TContainer> property)
		{
			return this.m_PropertiesHash.TryGetValue(name, out property);
		}

		private readonly List<IProperty<TContainer>> m_PropertiesList = new List<IProperty<TContainer>>();

		private readonly Dictionary<string, IProperty<TContainer>> m_PropertiesHash = new Dictionary<string, IProperty<TContainer>>();
	}
}
