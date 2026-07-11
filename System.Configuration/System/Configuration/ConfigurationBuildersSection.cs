using System;
using Unity;

namespace System.Configuration
{
	public sealed class ConfigurationBuildersSection : ConfigurationSection
	{
		public ConfigurationBuildersSection()
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

		public ConfigurationBuilder GetBuilderFromName(string builderName)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
