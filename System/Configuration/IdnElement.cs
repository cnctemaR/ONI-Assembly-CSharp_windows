using System;

namespace System.Configuration
{
	public sealed class IdnElement : ConfigurationElement
	{
		static IdnElement()
		{
			IdnElement.properties.Add(IdnElement.enabled_prop);
		}

		[ConfigurationProperty("enabled", DefaultValue = global::System.UriIdnScope.None, Options = ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey)]
		public global::System.UriIdnScope Enabled
		{
			get
			{
				return (global::System.UriIdnScope)((int)base[IdnElement.enabled_prop]);
			}
			set
			{
				base[IdnElement.enabled_prop] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return IdnElement.properties;
			}
		}

		public override bool Equals(object o)
		{
			IdnElement idnElement = o as IdnElement;
			return idnElement != null && idnElement.Enabled == this.Enabled;
		}

		public override int GetHashCode()
		{
			return (int)(this.Enabled ^ (global::System.UriIdnScope)127);
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty enabled_prop = new ConfigurationProperty("enabled", typeof(global::System.UriIdnScope), global::System.UriIdnScope.None, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
	}
}
