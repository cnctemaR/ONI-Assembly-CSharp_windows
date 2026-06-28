using System;
using System.Configuration.Provider;

namespace System.Configuration
{
	public abstract class SettingsProvider : ProviderBase
	{
		public abstract SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection);

		public abstract void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection);

		public abstract string ApplicationName { get; set; }
	}
}
