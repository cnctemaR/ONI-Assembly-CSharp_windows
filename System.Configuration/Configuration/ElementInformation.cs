using System;
using System.Collections;

namespace System.Configuration
{
	public sealed class ElementInformation
	{
		internal ElementInformation(ConfigurationElement owner, PropertyInformation propertyInfo)
		{
			this.propertyInfo = propertyInfo;
			this.owner = owner;
			this.properties = new PropertyInformationCollection();
			foreach (object obj in owner.Properties)
			{
				ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
				this.properties.Add(new PropertyInformation(owner, configurationProperty));
			}
		}

		[MonoTODO]
		public ICollection Errors
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsCollection
		{
			get
			{
				return this.owner is ConfigurationElementCollection;
			}
		}

		public bool IsLocked
		{
			get
			{
				return this.propertyInfo != null && this.propertyInfo.IsLocked;
			}
		}

		[MonoTODO]
		public bool IsPresent
		{
			get
			{
				return this.propertyInfo != null;
			}
		}

		public int LineNumber
		{
			get
			{
				return (this.propertyInfo == null) ? 0 : this.propertyInfo.LineNumber;
			}
		}

		public string Source
		{
			get
			{
				return (this.propertyInfo == null) ? null : this.propertyInfo.Source;
			}
		}

		public Type Type
		{
			get
			{
				return (this.propertyInfo == null) ? this.owner.GetType() : this.propertyInfo.Type;
			}
		}

		public ConfigurationValidatorBase Validator
		{
			get
			{
				return (this.propertyInfo == null) ? new DefaultValidator() : this.propertyInfo.Validator;
			}
		}

		public PropertyInformationCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		internal void Reset(ElementInformation parentInfo)
		{
			foreach (object obj in this.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				PropertyInformation propertyInformation2 = parentInfo.Properties[propertyInformation.Name];
				propertyInformation.Reset(propertyInformation2);
			}
		}

		private readonly PropertyInformation propertyInfo;

		private readonly ConfigurationElement owner;

		private readonly PropertyInformationCollection properties;
	}
}
