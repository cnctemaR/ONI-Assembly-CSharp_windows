using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	internal class NativeDapiProtection
	{
		[SuppressUnmanagedCodeSecurity]
		[DllImport("crypt32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptProtectData(ref NativeDapiProtection.DATA_BLOB pDataIn, string szDataDescr, ref NativeDapiProtection.DATA_BLOB pOptionalEntropy, IntPtr pvReserved, ref NativeDapiProtection.CRYPTPROTECT_PROMPTSTRUCT pPromptStruct, uint dwFlags, ref NativeDapiProtection.DATA_BLOB pDataOut);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("crypt32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptUnprotectData(ref NativeDapiProtection.DATA_BLOB pDataIn, string szDataDescr, ref NativeDapiProtection.DATA_BLOB pOptionalEntropy, IntPtr pvReserved, ref NativeDapiProtection.CRYPTPROTECT_PROMPTSTRUCT pPromptStruct, uint dwFlags, ref NativeDapiProtection.DATA_BLOB pDataOut);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "RtlZeroMemory")]
		private static extern void ZeroMemory(IntPtr dest, int size);

		public static byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope)
		{
			byte[] array = null;
			int num = 0;
			NativeDapiProtection.DATA_BLOB data_BLOB = default(NativeDapiProtection.DATA_BLOB);
			NativeDapiProtection.DATA_BLOB data_BLOB2 = default(NativeDapiProtection.DATA_BLOB);
			NativeDapiProtection.DATA_BLOB data_BLOB3 = default(NativeDapiProtection.DATA_BLOB);
			try
			{
				NativeDapiProtection.CRYPTPROTECT_PROMPTSTRUCT cryptprotect_PROMPTSTRUCT = new NativeDapiProtection.CRYPTPROTECT_PROMPTSTRUCT(0U);
				data_BLOB.Alloc(userData);
				data_BLOB2.Alloc(optionalEntropy);
				uint num2 = 1U;
				if (scope == DataProtectionScope.LocalMachine)
				{
					num2 |= 4U;
				}
				if (NativeDapiProtection.CryptProtectData(ref data_BLOB, string.Empty, ref data_BLOB2, IntPtr.Zero, ref cryptprotect_PROMPTSTRUCT, num2, ref data_BLOB3))
				{
					array = data_BLOB3.ToBytes();
				}
				else
				{
					num = Marshal.GetLastWin32Error();
				}
			}
			catch (Exception ex)
			{
				throw new CryptographicException(Locale.GetText("Error protecting data."), ex);
			}
			finally
			{
				data_BLOB3.Free();
				data_BLOB.Free();
				data_BLOB2.Free();
			}
			if (array == null || num != 0)
			{
				throw new CryptographicException(num);
			}
			return array;
		}

		public static byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope)
		{
			byte[] array = null;
			int num = 0;
			NativeDapiProtection.DATA_BLOB data_BLOB = default(NativeDapiProtection.DATA_BLOB);
			NativeDapiProtection.DATA_BLOB data_BLOB2 = default(NativeDapiProtection.DATA_BLOB);
			NativeDapiProtection.DATA_BLOB data_BLOB3 = default(NativeDapiProtection.DATA_BLOB);
			try
			{
				NativeDapiProtection.CRYPTPROTECT_PROMPTSTRUCT cryptprotect_PROMPTSTRUCT = new NativeDapiProtection.CRYPTPROTECT_PROMPTSTRUCT(0U);
				data_BLOB.Alloc(encryptedData);
				data_BLOB2.Alloc(optionalEntropy);
				uint num2 = 1U;
				if (scope == DataProtectionScope.LocalMachine)
				{
					num2 |= 4U;
				}
				if (NativeDapiProtection.CryptUnprotectData(ref data_BLOB, null, ref data_BLOB2, IntPtr.Zero, ref cryptprotect_PROMPTSTRUCT, num2, ref data_BLOB3))
				{
					array = data_BLOB3.ToBytes();
				}
				else
				{
					num = Marshal.GetLastWin32Error();
				}
			}
			catch (Exception ex)
			{
				throw new CryptographicException(Locale.GetText("Error protecting data."), ex);
			}
			finally
			{
				data_BLOB.Free();
				data_BLOB3.Free();
				data_BLOB2.Free();
			}
			if (array == null || num != 0)
			{
				throw new CryptographicException(num);
			}
			return array;
		}

		private const uint CRYPTPROTECT_UI_FORBIDDEN = 1U;

		private const uint CRYPTPROTECT_LOCAL_MACHINE = 4U;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct DATA_BLOB
		{
			public void Alloc(int size)
			{
				if (size > 0)
				{
					this.pbData = Marshal.AllocHGlobal(size);
					this.cbData = size;
				}
			}

			public void Alloc(byte[] managedMemory)
			{
				if (managedMemory != null)
				{
					int num = managedMemory.Length;
					this.pbData = Marshal.AllocHGlobal(num);
					this.cbData = num;
					Marshal.Copy(managedMemory, 0, this.pbData, this.cbData);
				}
			}

			public void Free()
			{
				if (this.pbData != IntPtr.Zero)
				{
					NativeDapiProtection.ZeroMemory(this.pbData, this.cbData);
					Marshal.FreeHGlobal(this.pbData);
					this.pbData = IntPtr.Zero;
					this.cbData = 0;
				}
			}

			public byte[] ToBytes()
			{
				if (this.cbData <= 0)
				{
					return new byte[0];
				}
				byte[] array = new byte[this.cbData];
				Marshal.Copy(this.pbData, array, 0, this.cbData);
				return array;
			}

			private int cbData;

			private IntPtr pbData;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct CRYPTPROTECT_PROMPTSTRUCT
		{
			public CRYPTPROTECT_PROMPTSTRUCT(uint flags)
			{
				this.cbSize = Marshal.SizeOf(typeof(NativeDapiProtection.CRYPTPROTECT_PROMPTSTRUCT));
				this.dwPromptFlags = flags;
				this.hwndApp = IntPtr.Zero;
				this.szPrompt = null;
			}

			private int cbSize;

			private uint dwPromptFlags;

			private IntPtr hwndApp;

			private string szPrompt;
		}
	}
}
