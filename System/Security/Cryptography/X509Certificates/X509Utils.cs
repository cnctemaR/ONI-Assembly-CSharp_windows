using System;

namespace System.Security.Cryptography.X509Certificates
{
	internal class X509Utils
	{
		private X509Utils()
		{
		}

		internal static string FindOidInfo(uint keyType, string keyValue, OidGroup oidGroup)
		{
			if (keyValue == null)
			{
				throw new ArgumentNullException("keyValue");
			}
			if (keyValue.Length == 0)
			{
				return null;
			}
			if (keyType == 1U)
			{
				return CAPI.CryptFindOIDInfoNameFromKey(keyValue, oidGroup);
			}
			if (keyType != 2U)
			{
				throw new NotImplementedException(keyType.ToString());
			}
			return CAPI.CryptFindOIDInfoKeyFromName(keyValue, oidGroup);
		}

		internal static string FindOidInfoWithFallback(uint key, string value, OidGroup group)
		{
			string text = X509Utils.FindOidInfo(key, value, group);
			if (text == null && group != OidGroup.All)
			{
				text = X509Utils.FindOidInfo(key, value, OidGroup.All);
			}
			return text;
		}
	}
}
