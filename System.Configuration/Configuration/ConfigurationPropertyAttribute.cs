using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ConfigurationPropertyAttribute : Attribute
	{
		public ConfigurationPropertyAttribute(string name)
		{
			this.name = name;
		}

		public bool IsKey
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsKey) != ConfigurationPropertyOptions.None;
			}
			set
			{
				if (value)
				{
					this.flags |= ConfigurationPropertyOptions.IsKey;
				}
				else
				{
					this.flags &= ~ConfigurationPropertyOptions.IsKey;
				}
			}
		}

		public bool IsDefaultCollection
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsDefaultCollection) != ConfigurationPropertyOptions.None;
			}
			set
			{
				if (value)
				{
					this.flags |= ConfigurationPropertyOptions.IsDefaultCollection;
				}
				else
				{
					this.flags &= ~ConfigurationPropertyOptions.IsDefaultCollection;
				}
			}
		}

		public object DefaultValue
		{
			get
			{
				return this.default_value;
			}
			set
			{
				this.default_value = value;
			}
		}

		public ConfigurationPropertyOptions Options
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool IsRequired
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsRequired) != ConfigurationPropertyOptions.None;
			}
			set
			{
				if (value)
				{
					this.flags |= ConfigurationPropertyOptions.IsRequired;
				}
				else
				{
					this.flags &= ~ConfigurationPropertyOptions.IsRequired;
				}
			}
		}

		private string name;

		private object default_value = ConfigurationProperty.NoDefaultValue;

		private ConfigurationPropertyOptions flags;
	}
}
