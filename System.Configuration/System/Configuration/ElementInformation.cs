using System;
using System.Collections;
using Unity;

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

		[MonoTODO("Support multiple levels of inheritance")]
		public bool IsPresent
		{
			get
			{
				return this.owner.IsElementPresent;
			}
		}

		public int LineNumber
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return 0;
				}
				return this.propertyInfo.LineNumber;
			}
		}

		public string Source
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return null;
				}
				return this.propertyInfo.Source;
			}
		}

		public Type Type
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return this.owner.GetType();
				}
				return this.propertyInfo.Type;
			}
		}

		public ConfigurationValidatorBase Validator
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return new DefaultValidator();
				}
				return this.propertyInfo.Validator;
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

		internal ElementInformation()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly PropertyInformation propertyInfo;

		private readonly ConfigurationElement owner;

		private readonly PropertyInformationCollection properties;
	}
}
