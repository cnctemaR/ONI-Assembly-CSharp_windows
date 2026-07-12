using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	public sealed class ProtectedMemory
	{
		private ProtectedMemory()
		{
		}

		[MonoTODO("only supported on Windows 2000 SP3 and later")]
		public static void Protect(byte[] userData, MemoryProtectionScope scope)
		{
			if (userData == null)
			{
				throw new ArgumentNullException("userData");
			}
			ProtectedMemory.Check(userData.Length, scope);
			try
			{
				uint num = (uint)userData.Length;
				ProtectedMemory.MemoryProtectionImplementation memoryProtectionImplementation = ProtectedMemory.impl;
				if (memoryProtectionImplementation != ProtectedMemory.MemoryProtectionImplementation.Win32RtlEncryptMemory)
				{
					if (memoryProtectionImplementation != ProtectedMemory.MemoryProtectionImplementation.Win32CryptoProtect)
					{
						throw new PlatformNotSupportedException();
					}
					if (!ProtectedMemory.CryptProtectMemory(userData, num, (uint)scope))
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
				}
				else
				{
					int num2 = ProtectedMemory.RtlEncryptMemory(userData, num, (uint)scope);
					if (num2 < 0)
					{
						throw new CryptographicException(Locale.GetText("Error. NTSTATUS = {0}.", new object[] { num2 }));
					}
				}
			}
			catch
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Unsupported;
				throw new PlatformNotSupportedException();
			}
		}

		[MonoTODO("only supported on Windows 2000 SP3 and later")]
		public static void Unprotect(byte[] encryptedData, MemoryProtectionScope scope)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			ProtectedMemory.Check(encryptedData.Length, scope);
			try
			{
				uint num = (uint)encryptedData.Length;
				ProtectedMemory.MemoryProtectionImplementation memoryProtectionImplementation = ProtectedMemory.impl;
				if (memoryProtectionImplementation != ProtectedMemory.MemoryProtectionImplementation.Win32RtlEncryptMemory)
				{
					if (memoryProtectionImplementation != ProtectedMemory.MemoryProtectionImplementation.Win32CryptoProtect)
					{
						throw new PlatformNotSupportedException();
					}
					if (!ProtectedMemory.CryptUnprotectMemory(encryptedData, num, (uint)scope))
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
				}
				else
				{
					int num2 = ProtectedMemory.RtlDecryptMemory(encryptedData, num, (uint)scope);
					if (num2 < 0)
					{
						throw new CryptographicException(Locale.GetText("Error. NTSTATUS = {0}.", new object[] { num2 }));
					}
				}
			}
			catch
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Unsupported;
				throw new PlatformNotSupportedException();
			}
		}

		private static void Detect()
		{
			OperatingSystem osversion = Environment.OSVersion;
			if (osversion.Platform != PlatformID.Win32NT)
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Unsupported;
				return;
			}
			Version version = osversion.Version;
			if (version.Major < 5)
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Unsupported;
				return;
			}
			if (version.Major != 5)
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Win32CryptoProtect;
				return;
			}
			if (version.Minor < 2)
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Win32RtlEncryptMemory;
				return;
			}
			ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Win32CryptoProtect;
		}

		private static void Check(int size, MemoryProtectionScope scope)
		{
			if (size % 16 != 0)
			{
				throw new CryptographicException(Locale.GetText("Not a multiple of {0} bytes.", new object[] { 16 }));
			}
			if (scope < MemoryProtectionScope.SameProcess || scope > MemoryProtectionScope.SameLogon)
			{
				throw new ArgumentException(Locale.GetText("Invalid enum value for '{0}'.", new object[] { "MemoryProtectionScope" }), "scope");
			}
			ProtectedMemory.MemoryProtectionImplementation memoryProtectionImplementation = ProtectedMemory.impl;
			if (memoryProtectionImplementation == ProtectedMemory.MemoryProtectionImplementation.Unsupported)
			{
				throw new PlatformNotSupportedException();
			}
			if (memoryProtectionImplementation == ProtectedMemory.MemoryProtectionImplementation.Unknown)
			{
				ProtectedMemory.Detect();
				return;
			}
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "SystemFunction040", SetLastError = true)]
		private static extern int RtlEncryptMemory(byte[] pData, uint cbData, uint dwFlags);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "SystemFunction041", SetLastError = true)]
		private static extern int RtlDecryptMemory(byte[] pData, uint cbData, uint dwFlags);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("crypt32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptProtectMemory(byte[] pData, uint cbData, uint dwFlags);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("crypt32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptUnprotectMemory(byte[] pData, uint cbData, uint dwFlags);

		private const int BlockSize = 16;

		private static ProtectedMemory.MemoryProtectionImplementation impl;

		private enum MemoryProtectionImplementation
		{
			Unknown,
			Win32RtlEncryptMemory,
			Win32CryptoProtect,
			Unsupported = -2147483648
		}
	}
}
