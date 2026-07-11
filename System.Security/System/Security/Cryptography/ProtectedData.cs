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
					goto IL_005E;
				}
				try
				{
					return ManagedProtection.Protect(userData, optionalEntropy, scope);
				}
				catch (Exception ex)
				{
					throw new CryptographicException(Locale.GetText("Data protection failed."), ex);
				}
			}
			try
			{
				return NativeDapiProtection.Protect(userData, optionalEntropy, scope);
			}
			catch (Exception ex2)
			{
				throw new CryptographicException(Locale.GetText("Data protection failed."), ex2);
			}
			IL_005E:
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
					goto IL_005E;
				}
				try
				{
					return ManagedProtection.Unprotect(encryptedData, optionalEntropy, scope);
				}
				catch (Exception ex)
				{
					throw new CryptographicException(Locale.GetText("Data unprotection failed."), ex);
				}
			}
			try
			{
				return NativeDapiProtection.Unprotect(encryptedData, optionalEntropy, scope);
			}
			catch (Exception ex2)
			{
				throw new CryptographicException(Locale.GetText("Data unprotection failed."), ex2);
			}
			IL_005E:
			throw new PlatformNotSupportedException();
		}

		private static void Detect()
		{
			OperatingSystem osversion = Environment.OSVersion;
			PlatformID platform = osversion.Platform;
			if (platform != PlatformID.Win32NT)
			{
				if (platform != PlatformID.Unix)
				{
					ProtectedData.impl = ProtectedData.DataProtectionImplementation.Unsupported;
					return;
				}
				ProtectedData.impl = ProtectedData.DataProtectionImplementation.ManagedProtection;
				return;
			}
			else
			{
				if (osversion.Version.Major < 5)
				{
					ProtectedData.impl = ProtectedData.DataProtectionImplementation.Unsupported;
					return;
				}
				ProtectedData.impl = ProtectedData.DataProtectionImplementation.Win32CryptoProtect;
				return;
			}
		}

		private static void Check(DataProtectionScope scope)
		{
			if (scope < DataProtectionScope.CurrentUser || scope > DataProtectionScope.LocalMachine)
			{
				throw new ArgumentException(Locale.GetText("Invalid enum value '{0}' for '{1}'.", new object[] { scope, "DataProtectionScope" }), "scope");
			}
			ProtectedData.DataProtectionImplementation dataProtectionImplementation = ProtectedData.impl;
			if (dataProtectionImplementation == ProtectedData.DataProtectionImplementation.Unsupported)
			{
				throw new PlatformNotSupportedException();
			}
			if (dataProtectionImplementation == ProtectedData.DataProtectionImplementation.Unknown)
			{
				ProtectedData.Detect();
				return;
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
