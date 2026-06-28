using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml.Serialization;

namespace System.Diagnostics
{
	public abstract class Switch
	{
		protected Switch(string displayName, string description)
		{
			this.name = displayName;
			this.description = description;
		}

		protected Switch(string displayName, string description, string defaultSwitchValue)
			: this(displayName, description)
		{
			this.defaultSwitchValue = defaultSwitchValue;
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.name;
			}
		}

		protected int SwitchSetting
		{
			get
			{
				if (!this.initialized)
				{
					this.initialized = true;
					this.GetConfigFileSetting();
					this.OnSwitchSettingChanged();
				}
				return this.switchSetting;
			}
			set
			{
				if (this.switchSetting != value)
				{
					this.switchSetting = value;
					this.OnSwitchSettingChanged();
				}
				this.initialized = true;
			}
		}

		[XmlIgnore]
		public global::System.Collections.Specialized.StringDictionary Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		protected string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
				try
				{
					this.OnValueChanged();
				}
				catch (Exception ex)
				{
					string text = string.Format("The config value for Switch '{0}' was invalid.", this.DisplayName);
					throw new ConfigurationErrorsException(text, ex);
				}
			}
		}

		protected internal virtual string[] GetSupportedAttributes()
		{
			return null;
		}

		protected virtual void OnValueChanged()
		{
		}

		private void GetConfigFileSetting()
		{
			IDictionary dictionary = (IDictionary)DiagnosticsConfiguration.Settings["switches"];
			if (dictionary != null && dictionary.Contains(this.name))
			{
				this.Value = dictionary[this.name] as string;
				return;
			}
			if (this.defaultSwitchValue != null)
			{
				this.value = this.defaultSwitchValue;
				this.OnValueChanged();
			}
		}

		protected virtual void OnSwitchSettingChanged()
		{
		}

		private string name;

		private string description;

		private int switchSetting;

		private string value;

		private string defaultSwitchValue;

		private bool initialized;

		private global::System.Collections.Specialized.StringDictionary attributes = new global::System.Collections.Specialized.StringDictionary();
	}
}
