using System;
using System.Runtime.InteropServices;
using Internal.Cryptography;

namespace System.Security.Cryptography
{
	public static class ProtectedData
	{
		public static byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope)
		{
			if (userData == null)
			{
				throw new ArgumentNullException("userData");
			}
			return ProtectedData.ProtectOrUnprotect(userData, optionalEntropy, scope, true);
		}

		public static byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			return ProtectedData.ProtectOrUnprotect(encryptedData, optionalEntropy, scope, false);
		}

		private unsafe static byte[] ProtectOrUnprotect(byte[] inputData, byte[] optionalEntropy, DataProtectionScope scope, bool protect)
		{
			byte[] array;
			byte* ptr;
			if ((array = ((inputData.Length == 0) ? ProtectedData.s_nonEmpty : inputData)) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			byte* ptr2;
			if (optionalEntropy == null || optionalEntropy.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &optionalEntropy[0];
			}
			global::Interop.Crypt32.DATA_BLOB data_BLOB = new global::Interop.Crypt32.DATA_BLOB((IntPtr)((void*)ptr), (uint)inputData.Length);
			global::Interop.Crypt32.DATA_BLOB data_BLOB2 = default(global::Interop.Crypt32.DATA_BLOB);
			if (optionalEntropy != null)
			{
				data_BLOB2 = new global::Interop.Crypt32.DATA_BLOB((IntPtr)((void*)ptr2), (uint)optionalEntropy.Length);
			}
			global::Interop.Crypt32.CryptProtectDataFlags cryptProtectDataFlags = global::Interop.Crypt32.CryptProtectDataFlags.CRYPTPROTECT_UI_FORBIDDEN;
			if (scope == DataProtectionScope.LocalMachine)
			{
				cryptProtectDataFlags |= global::Interop.Crypt32.CryptProtectDataFlags.CRYPTPROTECT_LOCAL_MACHINE;
			}
			global::Interop.Crypt32.DATA_BLOB data_BLOB3 = default(global::Interop.Crypt32.DATA_BLOB);
			byte[] array3;
			try
			{
				if (!(protect ? global::Interop.Crypt32.CryptProtectData(ref data_BLOB, null, ref data_BLOB2, IntPtr.Zero, IntPtr.Zero, cryptProtectDataFlags, out data_BLOB3) : global::Interop.Crypt32.CryptUnprotectData(ref data_BLOB, IntPtr.Zero, ref data_BLOB2, IntPtr.Zero, IntPtr.Zero, cryptProtectDataFlags, out data_BLOB3)))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (protect && ProtectedData.ErrorMayBeCausedByUnloadedProfile(lastWin32Error))
					{
						throw new CryptographicException("The data protection operation was unsuccessful. This may have been caused by not having the user profile loaded for the current thread's user context, which may be the case when the thread is impersonating.");
					}
					throw lastWin32Error.ToCryptographicException();
				}
				else
				{
					if (data_BLOB3.pbData == IntPtr.Zero)
					{
						throw new OutOfMemoryException();
					}
					int cbData = (int)data_BLOB3.cbData;
					byte[] array2 = new byte[cbData];
					Marshal.Copy(data_BLOB3.pbData, array2, 0, cbData);
					array3 = array2;
				}
			}
			finally
			{
				if (data_BLOB3.pbData != IntPtr.Zero)
				{
					int cbData2 = (int)data_BLOB3.cbData;
					byte* ptr3 = (byte*)(void*)data_BLOB3.pbData;
					for (int i = 0; i < cbData2; i++)
					{
						ptr3[i] = 0;
					}
					Marshal.FreeHGlobal(data_BLOB3.pbData);
				}
			}
			return array3;
		}

		private static bool ErrorMayBeCausedByUnloadedProfile(int errorCode)
		{
			return errorCode == -2147024894 || errorCode == 2;
		}

		private static readonly byte[] s_nonEmpty = new byte[1];
	}
}
