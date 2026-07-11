using System;
using System.Configuration;

namespace System.Xml.Serialization.Configuration
{
	public sealed class XmlSerializerSection : ConfigurationSection
	{
		public XmlSerializerSection()
		{
			this.properties.Add(this.checkDeserializeAdvances);
			this.properties.Add(this.tempFilesLocation);
			this.properties.Add(this.useLegacySerializerGeneration);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		[ConfigurationProperty("checkDeserializeAdvances", DefaultValue = false)]
		public bool CheckDeserializeAdvances
		{
			get
			{
				return (bool)base[this.checkDeserializeAdvances];
			}
			set
			{
				base[this.checkDeserializeAdvances] = value;
			}
		}

		[ConfigurationProperty("tempFilesLocation", DefaultValue = null)]
		public string TempFilesLocation
		{
			get
			{
				return (string)base[this.tempFilesLocation];
			}
			set
			{
				base[this.tempFilesLocation] = value;
			}
		}

		[ConfigurationProperty("useLegacySerializerGeneration", DefaultValue = false)]
		public bool UseLegacySerializerGeneration
		{
			get
			{
				return (bool)base[this.useLegacySerializerGeneration];
			}
			set
			{
				base[this.useLegacySerializerGeneration] = value;
			}
		}

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private readonly ConfigurationProperty checkDeserializeAdvances = new ConfigurationProperty("checkDeserializeAdvances", typeof(bool), false, ConfigurationPropertyOptions.None);

		private readonly ConfigurationProperty tempFilesLocation = new ConfigurationProperty("tempFilesLocation", typeof(string), null, null, new RootedPathValidator(), ConfigurationPropertyOptions.None);

		private readonly ConfigurationProperty useLegacySerializerGeneration = new ConfigurationProperty("useLegacySerializerGeneration", typeof(bool), false, ConfigurationPropertyOptions.None);
	}
}
