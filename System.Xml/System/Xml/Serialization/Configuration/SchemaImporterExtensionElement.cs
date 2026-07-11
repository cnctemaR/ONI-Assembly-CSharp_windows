using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;

namespace System.Xml.Serialization.Configuration
{
	public sealed class SchemaImporterExtensionElement : ConfigurationElement
	{
		public SchemaImporterExtensionElement()
		{
			this.properties.Add(this.name);
			this.properties.Add(this.type);
		}

		public SchemaImporterExtensionElement(string name, string type)
			: this()
		{
			this.Name = name;
			base[this.type] = new SchemaImporterExtensionElement.TypeAndName(type);
		}

		public SchemaImporterExtensionElement(string name, Type type)
			: this()
		{
			this.Name = name;
			this.Type = type;
		}

		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return (string)base[this.name];
			}
			set
			{
				base[this.name] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		[TypeConverter(typeof(SchemaImporterExtensionElement.TypeTypeConverter))]
		[ConfigurationProperty("type", IsRequired = true, IsKey = false)]
		public Type Type
		{
			get
			{
				return ((SchemaImporterExtensionElement.TypeAndName)base[this.type]).type;
			}
			set
			{
				base[this.type] = new SchemaImporterExtensionElement.TypeAndName(value);
			}
		}

		internal string Key
		{
			get
			{
				return this.Name;
			}
		}

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsKey);

		private readonly ConfigurationProperty type = new ConfigurationProperty("type", typeof(Type), null, new SchemaImporterExtensionElement.TypeTypeConverter(), null, ConfigurationPropertyOptions.IsRequired);

		private class TypeAndName
		{
			public TypeAndName(string name)
			{
				this.type = Type.GetType(name, true, true);
				this.name = name;
			}

			public TypeAndName(Type type)
			{
				this.type = type;
			}

			public override int GetHashCode()
			{
				return this.type.GetHashCode();
			}

			public override bool Equals(object comparand)
			{
				return this.type.Equals(((SchemaImporterExtensionElement.TypeAndName)comparand).type);
			}

			public readonly Type type;

			public readonly string name;
		}

		private class TypeTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value is string)
				{
					return new SchemaImporterExtensionElement.TypeAndName((string)value);
				}
				return base.ConvertFrom(context, culture, value);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (!(destinationType == typeof(string)))
				{
					return base.ConvertTo(context, culture, value, destinationType);
				}
				SchemaImporterExtensionElement.TypeAndName typeAndName = (SchemaImporterExtensionElement.TypeAndName)value;
				if (typeAndName.name != null)
				{
					return typeAndName.name;
				}
				return typeAndName.type.AssemblyQualifiedName;
			}
		}
	}
}
