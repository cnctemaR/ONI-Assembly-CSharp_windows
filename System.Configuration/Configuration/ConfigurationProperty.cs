using System;
using System.ComponentModel;

namespace System.Configuration
{
	public sealed class ConfigurationProperty
	{
		public ConfigurationProperty(string name, Type type)
			: this(name, type, ConfigurationProperty.NoDefaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(), ConfigurationPropertyOptions.None, null)
		{
		}

		public ConfigurationProperty(string name, Type type, object default_value)
			: this(name, type, default_value, TypeDescriptor.GetConverter(type), new DefaultValidator(), ConfigurationPropertyOptions.None, null)
		{
		}

		public ConfigurationProperty(string name, Type type, object default_value, ConfigurationPropertyOptions flags)
			: this(name, type, default_value, TypeDescriptor.GetConverter(type), new DefaultValidator(), flags, null)
		{
		}

		public ConfigurationProperty(string name, Type type, object default_value, TypeConverter converter, ConfigurationValidatorBase validation, ConfigurationPropertyOptions flags)
			: this(name, type, default_value, converter, validation, flags, null)
		{
		}

		public ConfigurationProperty(string name, Type type, object default_value, TypeConverter converter, ConfigurationValidatorBase validation, ConfigurationPropertyOptions flags, string description)
		{
			this.name = name;
			this.converter = ((converter == null) ? TypeDescriptor.GetConverter(type) : converter);
			if (default_value != null)
			{
				if (default_value == ConfigurationProperty.NoDefaultValue)
				{
					TypeCode typeCode = Type.GetTypeCode(type);
					if (typeCode != TypeCode.Object)
					{
						if (typeCode != TypeCode.String)
						{
							default_value = Activator.CreateInstance(type);
						}
						else
						{
							default_value = string.Empty;
						}
					}
					else
					{
						default_value = null;
					}
				}
				else if (!type.IsAssignableFrom(default_value.GetType()))
				{
					if (!this.converter.CanConvertFrom(default_value.GetType()))
					{
						throw new ConfigurationErrorsException(string.Format("The default value for property '{0}' has a different type than the one of the property itself: expected {1} but was {2}", name, type, default_value.GetType()));
					}
					default_value = this.converter.ConvertFrom(default_value);
				}
			}
			this.default_value = default_value;
			this.flags = flags;
			this.type = type;
			this.validation = ((validation == null) ? new DefaultValidator() : validation);
			this.description = description;
		}

		public TypeConverter Converter
		{
			get
			{
				return this.converter;
			}
		}

		public object DefaultValue
		{
			get
			{
				return this.default_value;
			}
		}

		public bool IsKey
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsKey) != ConfigurationPropertyOptions.None;
			}
		}

		public bool IsRequired
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsRequired) != ConfigurationPropertyOptions.None;
			}
		}

		public bool IsDefaultCollection
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsDefaultCollection) != ConfigurationPropertyOptions.None;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		public ConfigurationValidatorBase Validator
		{
			get
			{
				return this.validation;
			}
		}

		internal object ConvertFromString(string value)
		{
			if (this.converter != null)
			{
				return this.converter.ConvertFromInvariantString(value);
			}
			throw new NotImplementedException();
		}

		internal string ConvertToString(object value)
		{
			if (this.converter != null)
			{
				return this.converter.ConvertToInvariantString(value);
			}
			throw new NotImplementedException();
		}

		internal bool IsElement
		{
			get
			{
				return typeof(ConfigurationElement).IsAssignableFrom(this.type);
			}
		}

		internal ConfigurationCollectionAttribute CollectionAttribute
		{
			get
			{
				return this.collectionAttribute;
			}
			set
			{
				this.collectionAttribute = value;
			}
		}

		internal void Validate(object value)
		{
			if (this.validation != null)
			{
				this.validation.Validate(value);
			}
		}

		internal static readonly object NoDefaultValue = new object();

		private string name;

		private Type type;

		private object default_value;

		private TypeConverter converter;

		private ConfigurationValidatorBase validation;

		private ConfigurationPropertyOptions flags;

		private string description;

		private ConfigurationCollectionAttribute collectionAttribute;
	}
}
