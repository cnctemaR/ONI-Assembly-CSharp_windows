using System;
using Unity;

namespace System.Configuration
{
	public sealed class UriSection : ConfigurationSection
	{
		static UriSection()
		{
			UriSection.properties.Add(UriSection.idn_prop);
			UriSection.properties.Add(UriSection.iriParsing_prop);
		}

		[ConfigurationProperty("idn")]
		public IdnElement Idn
		{
			get
			{
				return (IdnElement)base[UriSection.idn_prop];
			}
		}

		[ConfigurationProperty("iriParsing")]
		public IriParsingElement IriParsing
		{
			get
			{
				return (IriParsingElement)base[UriSection.iriParsing_prop];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return UriSection.properties;
			}
		}

		public SchemeSettingElementCollection SchemeSettings
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty idn_prop = new ConfigurationProperty("idn", typeof(IdnElement), null);

		private static ConfigurationProperty iriParsing_prop = new ConfigurationProperty("iriParsing", typeof(IriParsingElement), null);
	}
}
