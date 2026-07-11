using System;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	public sealed class ProtectedData
	{
		private ProtectedData()
		{
		}

		public static byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope)
		{
			if (userData == null)
			{
				throw new ArgumentNullException("userData");
			}
			ProtectedData.Check(scope);
			ProtectedData.DataProtectionImplementation dataProtectionImplementation = ProtectedData.impl;
			if (dataProtectionImplementation != ProtectedData.DataProtectionImplementation.Win32CryptoProtect)
			{
				if (dataProtectionImplementation != ProtectedData.DataProtectionImplementation.ManagedProtection)
				{
					goto IL_008D;
				}
				try
				{
					return ManagedProtection.Protect(userData, optionalEntropy, scope);
				}
				catch (Exception ex)
				{
					string text = Locale.GetText("Data protection failed.");
					throw new CryptographicException(text, ex);
				}
			}
			try
			{
				return NativeDapiProtection.Protect(userData, optionalEntropy, scope);
			}
			catch (Exception ex2)
			{
				string text2 = Locale.GetText("Data protection failed.");
				throw new CryptographicException(text2, ex2);
			}
			IL_008D:
			throw new PlatformNotSupportedException();
		}

		public static byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			ProtectedData.Check(scope);
			ProtectedData.DataProtectionImplementation dataProtectionImplementation = ProtectedData.impl;
			if (dataProtectionImplementation != ProtectedData.DataProtectionImplementation.Win32CryptoProtect)
			{
				if (dataProtectionImplementation != ProtectedData.DataProtectionImplementation.ManagedProtection)
				{
					goto IL_008D;
				}
				try
				{
					return ManagedProtection.Unprotect(encryptedData, optionalEntropy, scope);
				}
				catch (Exception ex)
				{
					string text = Locale.GetText("Data unprotection failed.");
					throw new CryptographicException(text, ex);
				}
			}
			try
			{
				return NativeDapiProtection.Unprotect(encryptedData, optionalEntropy, scope);
			}
			catch (Exception ex2)
			{
				string text2 = Locale.GetText("Data unprotection failed.");
				throw new CryptographicException(text2, ex2);
			}
			IL_008D:
			throw new PlatformNotSupportedException();
		}

		private static void Detect()
		{
			OperatingSystem osversion = Environment.OSVersion;
			switch (osversion.Platform)
			{
			case PlatformID.Win32NT:
			{
				Version version = osversion.Version;
				if (version.Major < 5)
				{
					ProtectedData.impl = ProtectedData.DataProtectionImplementation.Unsupported;
				}
				else
				{
					ProtectedData.impl = ProtectedData.DataProtectionImplementation.Win32CryptoProtect;
				}
				return;
			}
			case PlatformID.Unix:
				ProtectedData.impl = ProtectedData.DataProtectionImplementation.ManagedProtection;
				return;
			}
			ProtectedData.impl = ProtectedData.DataProtectionImplementation.Unsupported;
		}

		private static void Check(DataProtectionScope scope)
		{
			if (scope < DataProtectionScope.CurrentUser || scope > DataProtectionScope.LocalMachine)
			{
				string text = Locale.GetText("Invalid enum value '{0}' for '{1}'.", new object[] { scope, "DataProtectionScope" });
				throw new ArgumentException(text, "scope");
			}
			ProtectedData.DataProtectionImplementation dataProtectionImplementation = ProtectedData.impl;
			if (dataProtectionImplementation != ProtectedData.DataProtectionImplementation.Unknown)
			{
				if (dataProtectionImplementation == ProtectedData.DataProtectionImplementation.Unsupported)
				{
					throw new PlatformNotSupportedException();
				}
			}
			else
			{
				ProtectedData.Detect();
			}
		}

		private static ProtectedData.DataProtectionImplementation impl;

		private enum DataProtectionImplementation
		{
			Unknown,
			Win32CryptoProtect,
			ManagedProtection,
			Unsupported = -2147483648
		}
	}
}
