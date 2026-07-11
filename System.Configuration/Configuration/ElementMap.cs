using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Configuration
{
	internal class ElementMap
	{
		public static ElementMap GetMap(Type t)
		{
			ElementMap elementMap = ElementMap.elementMaps[t] as ElementMap;
			if (elementMap != null)
			{
				return elementMap;
			}
			elementMap = new ElementMap(t);
			ElementMap.elementMaps[t] = elementMap;
			return elementMap;
		}

		public ElementMap(Type t)
		{
			this.properties = new ConfigurationPropertyCollection();
			this.collectionAttribute = Attribute.GetCustomAttribute(t, typeof(ConfigurationCollectionAttribute)) as ConfigurationCollectionAttribute;
			foreach (PropertyInfo propertyInfo in t.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				ConfigurationPropertyAttribute configurationPropertyAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(ConfigurationPropertyAttribute)) as ConfigurationPropertyAttribute;
				if (configurationPropertyAttribute != null)
				{
					string text = ((configurationPropertyAttribute.Name != null) ? configurationPropertyAttribute.Name : propertyInfo.Name);
					ConfigurationValidatorAttribute configurationValidatorAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(ConfigurationValidatorAttribute)) as ConfigurationValidatorAttribute;
					ConfigurationValidatorBase configurationValidatorBase = ((configurationValidatorAttribute != null) ? configurationValidatorAttribute.ValidatorInstance : null);
					TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(TypeConverterAttribute));
					TypeConverter typeConverter = ((typeConverterAttribute != null) ? ((TypeConverter)Activator.CreateInstance(Type.GetType(typeConverterAttribute.ConverterTypeName), true)) : null);
					ConfigurationProperty configurationProperty = new ConfigurationProperty(text, propertyInfo.PropertyType, configurationPropertyAttribute.DefaultValue, typeConverter, configurationValidatorBase, configurationPropertyAttribute.Options);
					configurationProperty.CollectionAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(ConfigurationCollectionAttribute)) as ConfigurationCollectionAttribute;
					this.properties.Add(configurationProperty);
				}
			}
		}

		public ConfigurationCollectionAttribute CollectionAttribute
		{
			get
			{
				return this.collectionAttribute;
			}
		}

		public bool HasProperties
		{
			get
			{
				return this.properties.Count > 0;
			}
		}

		public ConfigurationPropertyCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		private static readonly Hashtable elementMaps = Hashtable.Synchronized(new Hashtable());

		private readonly ConfigurationPropertyCollection properties;

		private readonly ConfigurationCollectionAttribute collectionAttribute;
	}
}
