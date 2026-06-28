using System;
using System.Configuration.Provider;

namespace System.Configuration
{
	public class SettingsProviderCollection : ProviderCollection
	{
		public override void Add(ProviderBase provider)
		{
			if (!(provider is SettingsProvider))
			{
				throw new ArgumentException("SettingsProvider is expected");
			}
			if (string.IsNullOrEmpty(provider.Name))
			{
				throw new ArgumentException("Provider name cannot be null or empty");
			}
			base.Add(provider);
		}

		public SettingsProvider this[string name]
		{
			get
			{
				return (SettingsProvider)base[name];
			}
		}
	}
}
