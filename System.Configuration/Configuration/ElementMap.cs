using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Configuration
{
	internal class ElementMap
	{
		public ElementMap(Type t)
		{
			this.properties = new ConfigurationPropertyCollection();
			this.collectionAttribute = Attribute.GetCustomAttribute(t, typeof(ConfigurationCollectionAttribute)) as ConfigurationCollectionAttribute;
			PropertyInfo[] array = t.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in array)
			{
				ConfigurationPropertyAttribute configurationPropertyAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(ConfigurationPropertyAttribute)) as ConfigurationPropertyAttribute;
				if (configurationPropertyAttribute != null)
				{
					string text = ((configurationPropertyAttribute.Name == null) ? propertyInfo.Name : configurationPropertyAttribute.Name);
					ConfigurationValidatorAttribute configurationValidatorAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(ConfigurationValidatorAttribute)) as ConfigurationValidatorAttribute;
					ConfigurationValidatorBase configurationValidatorBase = ((configurationValidatorAttribute == null) ? null : configurationValidatorAttribute.ValidatorInstance);
					TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(TypeConverterAttribute));
					TypeConverter typeConverter = ((typeConverterAttribute == null) ? null : ((TypeConverter)Activator.CreateInstance(Type.GetType(typeConverterAttribute.ConverterTypeName), true)));
					ConfigurationProperty configurationProperty = new ConfigurationProperty(text, propertyInfo.PropertyType, configurationPropertyAttribute.DefaultValue, typeConverter, configurationValidatorBase, configurationPropertyAttribute.Options);
					configurationProperty.CollectionAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(ConfigurationCollectionAttribute)) as ConfigurationCollectionAttribute;
					this.properties.Add(configurationProperty);
				}
			}
		}

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
