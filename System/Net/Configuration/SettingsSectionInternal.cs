using System;
using System.Configuration;
using System.Net.Security;
using System.Net.Sockets;

namespace System.Net.Configuration
{
	internal sealed class SettingsSectionInternal
	{
		internal static SettingsSectionInternal Section
		{
			get
			{
				return SettingsSectionInternal.instance;
			}
		}

		internal bool UseNagleAlgorithm { get; set; }

		internal bool Expect100Continue { get; set; }

		internal bool CheckCertificateName { get; private set; }

		internal int DnsRefreshTimeout { get; set; }

		internal bool EnableDnsRoundRobin { get; set; }

		internal bool CheckCertificateRevocationList { get; set; }

		internal EncryptionPolicy EncryptionPolicy { get; private set; }

		internal bool Ipv6Enabled
		{
			get
			{
				try
				{
					SettingsSection settingsSection = (SettingsSection)ConfigurationManager.GetSection("system.net/settings");
					if (settingsSection != null)
					{
						return settingsSection.Ipv6.Enabled;
					}
				}
				catch
				{
				}
				return true;
			}
		}

		private static readonly SettingsSectionInternal instance = new SettingsSectionInternal();

		internal UnicodeEncodingConformance WebUtilityUnicodeEncodingConformance;

		internal UnicodeDecodingConformance WebUtilityUnicodeDecodingConformance;

		internal readonly bool HttpListenerUnescapeRequestUrl = true;

		internal readonly IPProtectionLevel IPProtectionLevel = IPProtectionLevel.Unspecified;
	}
}
