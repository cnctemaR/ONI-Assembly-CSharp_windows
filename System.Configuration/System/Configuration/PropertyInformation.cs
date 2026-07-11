using System;
using System.ComponentModel;
using Unity;

namespace System.Configuration
{
	public sealed class PropertyInformation
	{
		internal PropertyInformation(ConfigurationElement owner, ConfigurationProperty property)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			this.owner = owner;
			this.property = property;
		}

		public TypeConverter Converter
		{
			get
			{
				return this.property.Converter;
			}
		}

		public object DefaultValue
		{
			get
			{
				return this.property.DefaultValue;
			}
		}

		public string Description
		{
			get
			{
				return this.property.Description;
			}
		}

		public bool IsKey
		{
			get
			{
				return this.property.IsKey;
			}
		}

		[MonoTODO]
		public bool IsLocked
		{
			get
			{
				return this.isLocked;
			}
			internal set
			{
				this.isLocked = value;
			}
		}

		public bool IsModified
		{
			get
			{
				return this.isModified;
			}
			internal set
			{
				this.isModified = value;
			}
		}

		public bool IsRequired
		{
			get
			{
				return this.property.IsRequired;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
			internal set
			{
				this.lineNumber = value;
			}
		}

		public string Name
		{
			get
			{
				return this.property.Name;
			}
		}

		public string Source
		{
			get
			{
				return this.source;
			}
			internal set
			{
				this.source = value;
			}
		}

		public Type Type
		{
			get
			{
				return this.property.Type;
			}
		}

		public ConfigurationValidatorBase Validator
		{
			get
			{
				return this.property.Validator;
			}
		}

		public object Value
		{
			get
			{
				if (this.origin == PropertyValueOrigin.Default)
				{
					if (!this.property.IsElement)
					{
						return this.DefaultValue;
					}
					ConfigurationElement configurationElement = (ConfigurationElement)Activator.CreateInstance(this.Type, true);
					configurationElement.InitFromProperty(this);
					if (this.owner != null && this.owner.IsReadOnly())
					{
						configurationElement.SetReadOnly();
					}
					this.val = configurationElement;
					this.origin = PropertyValueOrigin.Inherited;
				}
				return this.val;
			}
			set
			{
				this.val = value;
				this.isModified = true;
				this.origin = PropertyValueOrigin.SetHere;
			}
		}

		internal void Reset(PropertyInformation parentProperty)
		{
			if (parentProperty == null)
			{
				this.origin = PropertyValueOrigin.Default;
				return;
			}
			if (this.property.IsElement)
			{
				((ConfigurationElement)this.Value).Reset((ConfigurationElement)parentProperty.Value);
				return;
			}
			this.val = parentProperty.Value;
			this.origin = PropertyValueOrigin.Inherited;
		}

		internal bool IsElement
		{
			get
			{
				return this.property.IsElement;
			}
		}

		public PropertyValueOrigin ValueOrigin
		{
			get
			{
				return this.origin;
			}
		}

		internal string GetStringValue()
		{
			return this.property.ConvertToString(this.Value);
		}

		internal void SetStringValue(string value)
		{
			this.val = this.property.ConvertFromString(value);
			if (!object.Equals(this.val, this.DefaultValue))
			{
				this.origin = PropertyValueOrigin.SetHere;
			}
		}

		internal ConfigurationProperty Property
		{
			get
			{
				return this.property;
			}
		}

		internal PropertyInformation()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private bool isLocked;

		private bool isModified;

		private int lineNumber;

		private string source;

		private object val;

		private PropertyValueOrigin origin;

		private readonly ConfigurationElement owner;

		private readonly ConfigurationProperty property;
	}
}
