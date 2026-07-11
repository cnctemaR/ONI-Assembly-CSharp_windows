using System;
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
