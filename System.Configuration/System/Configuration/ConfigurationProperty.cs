using System;
using System.ComponentModel;
using Unity;

namespace System.Configuration
{
	public sealed class ConfigurationProperty
	{
		public ConfigurationProperty(string name, Type type)
			: this(name, type, ConfigurationProperty.NoDefaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(), ConfigurationPropertyOptions.None, null)
		{
		}

		public ConfigurationProperty(string name, Type type, object defaultValue)
			: this(name, type, defaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(), ConfigurationPropertyOptions.None, null)
		{
		}

		public ConfigurationProperty(string name, Type type, object defaultValue, ConfigurationPropertyOptions options)
			: this(name, type, defaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(), options, null)
		{
		}

		public ConfigurationProperty(string name, Type type, object defaultValue, TypeConverter typeConverter, ConfigurationValidatorBase validator, ConfigurationPropertyOptions options)
			: this(name, type, defaultValue, typeConverter, validator, options, null)
		{
		}

		public ConfigurationProperty(string name, Type type, object defaultValue, TypeConverter typeConverter, ConfigurationValidatorBase validator, ConfigurationPropertyOptions options, string description)
		{
			this.name = name;
			this.converter = ((typeConverter != null) ? typeConverter : TypeDescriptor.GetConverter(type));
			if (defaultValue != null)
			{
				if (defaultValue == ConfigurationProperty.NoDefaultValue)
				{
					TypeCode typeCode = Type.GetTypeCode(type);
					if (typeCode != TypeCode.Object)
					{
						if (typeCode != TypeCode.String)
						{
							defaultValue = Activator.CreateInstance(type);
						}
						else
						{
							defaultValue = string.Empty;
						}
					}
					else
					{
						defaultValue = null;
					}
				}
				else if (!type.IsAssignableFrom(defaultValue.GetType()))
				{
					if (!this.converter.CanConvertFrom(defaultValue.GetType()))
					{
						throw new ConfigurationErrorsException(string.Format("The default value for property '{0}' has a different type than the one of the property itself: expected {1} but was {2}", name, type, defaultValue.GetType()));
					}
					defaultValue = this.converter.ConvertFrom(defaultValue);
				}
			}
			this.default_value = defaultValue;
			this.flags = options;
			this.type = type;
			this.validation = ((validator != null) ? validator : new DefaultValidator());
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
				return (this.flags & ConfigurationPropertyOptions.IsKey) > ConfigurationPropertyOptions.None;
			}
		}

		public bool IsRequired
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsRequired) > ConfigurationPropertyOptions.None;
			}
		}

		public bool IsDefaultCollection
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsDefaultCollection) > ConfigurationPropertyOptions.None;
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

		public bool IsAssemblyStringTransformationRequired
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		public bool IsTypeStringTransformationRequired
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		public bool IsVersionCheckRequired
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
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
