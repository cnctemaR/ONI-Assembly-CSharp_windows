using System;

namespace System.Configuration
{
	public static class ProtectedConfiguration
	{
		public static string DefaultProvider
		{
			get
			{
				return ProtectedConfiguration.Section.DefaultProvider;
			}
		}

		public static ProtectedConfigurationProviderCollection Providers
		{
			get
			{
				return ProtectedConfiguration.Section.GetAllProviders();
			}
		}

		internal static ProtectedConfigurationSection Section
		{
			get
			{
				return (ProtectedConfigurationSection)ConfigurationManager.GetSection("configProtectedData");
			}
		}

		internal static ProtectedConfigurationProvider GetProvider(string name, bool throwOnError)
		{
			ProtectedConfigurationProvider protectedConfigurationProvider = ProtectedConfiguration.Providers[name];
			if (protectedConfigurationProvider == null && throwOnError)
			{
				throw new Exception(string.Format("The protection provider '{0}' was not found.", name));
			}
			return protectedConfigurationProvider;
		}

		public const string DataProtectionProviderName = "DataProtectionConfigurationProvider";

		public const string ProtectedDataSectionName = "configProtectedData";

		public const string RsaProviderName = "RsaProtectedConfigurationProvider";
	}
}
