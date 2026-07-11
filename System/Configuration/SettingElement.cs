using System;

namespace System.Configuration
{
	public sealed class SettingElement : ConfigurationElement
	{
		public SettingElement()
		{
		}

		public SettingElement(string name, SettingsSerializeAs serializeAs)
		{
			this.Name = name;
			this.SerializeAs = serializeAs;
		}

		static SettingElement()
		{
			SettingElement.properties.Add(SettingElement.name_prop);
			SettingElement.properties.Add(SettingElement.serialize_as_prop);
			SettingElement.properties.Add(SettingElement.value_prop);
		}

		[ConfigurationProperty("name", DefaultValue = "", Options = ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey)]
		public string Name
		{
			get
			{
				return (string)base[SettingElement.name_prop];
			}
			set
			{
				base[SettingElement.name_prop] = value;
			}
		}

		[ConfigurationProperty("value", DefaultValue = null, Options = ConfigurationPropertyOptions.IsRequired)]
		public SettingValueElement Value
		{
			get
			{
				return (SettingValueElement)base[SettingElement.value_prop];
			}
			set
			{
				base[SettingElement.value_prop] = value;
			}
		}

		[ConfigurationProperty("serializeAs", DefaultValue = SettingsSerializeAs.String, Options = ConfigurationPropertyOptions.IsRequired)]
		public SettingsSerializeAs SerializeAs
		{
			get
			{
				return (SettingsSerializeAs)((base[SettingElement.serialize_as_prop] == null) ? 0 : ((int)base[SettingElement.serialize_as_prop]));
			}
			set
			{
				base[SettingElement.serialize_as_prop] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SettingElement.properties;
			}
		}

		public override bool Equals(object o)
		{
			SettingElement settingElement = o as SettingElement;
			return settingElement != null && (settingElement.SerializeAs == this.SerializeAs && settingElement.Value == this.Value) && settingElement.Name == this.Name;
		}

		public override int GetHashCode()
		{
			int num = (int)(this.SerializeAs ^ (SettingsSerializeAs)127);
			if (this.Name != null)
			{
				num += this.Name.GetHashCode() ^ 127;
			}
			if (this.Value != null)
			{
				num += this.Value.GetHashCode();
			}
			return num;
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty name_prop = new ConfigurationProperty("name", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

		private static ConfigurationProperty serialize_as_prop = new ConfigurationProperty("serializeAs", typeof(SettingsSerializeAs), null, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty value_prop = new ConfigurationProperty("value", typeof(SettingValueElement), null, ConfigurationPropertyOptions.IsRequired);
	}
}
