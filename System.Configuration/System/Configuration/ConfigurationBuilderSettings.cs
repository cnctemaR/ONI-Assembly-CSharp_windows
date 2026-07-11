using System;
using Unity;

namespace System.Configuration
{
	public class ConfigurationBuilderSettings : ConfigurationElement
	{
		public ConfigurationBuilderSettings()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public ProviderSettingsCollection Builders
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}
	}
}
