using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data
{
	internal static class LocalDBAPI
	{
		internal static void ReleaseDLLHandles()
		{
			LocalDBAPI.s_userInstanceDLLHandle = IntPtr.Zero;
			LocalDBAPI.s_localDBFormatMessage = null;
		}

		private static LocalDBAPI.LocalDBFormatMessageDelegate LocalDBFormatMessage
		{
			get
			{
				if (LocalDBAPI.s_localDBFormatMessage == null)
				{
					object obj = LocalDBAPI.s_dllLock;
					lock (obj)
					{
						if (LocalDBAPI.s_localDBFormatMessage == null)
						{
							IntPtr intPtr = LocalDBAPI.LoadProcAddress();
							if (intPtr == IntPtr.Zero)
							{
								Marshal.GetLastWin32Error();
								throw LocalDBAPI.CreateLocalDBException("Invalid SQLUserInstance.dll found at the location specified in the registry. Verify that the Local Database Runtime feature of SQL Server Express is properly installed.", null, 0, 0);
							}
							LocalDBAPI.s_localDBFormatMessage = Marshal.GetDelegateForFunctionPointer<LocalDBAPI.LocalDBFormatMessageDelegate>(intPtr);
						}
					}
				}
				return LocalDBAPI.s_localDBFormatMessage;
			}
		}

		internal static string GetLocalDBMessage(int hrCode)
		{
			string text;
			try
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				uint num = (uint)stringBuilder.Capacity;
				int num2 = LocalDBAPI.LocalDBFormatMessage(hrCode, 1U, (uint)CultureInfo.CurrentCulture.LCID, stringBuilder, ref num);
				if (num2 >= 0)
				{
					text = stringBuilder.ToString();
				}
				else
				{
					stringBuilder = new StringBuilder(1024);
					num = (uint)stringBuilder.Capacity;
					num2 = LocalDBAPI.LocalDBFormatMessage(hrCode, 1U, 0U, stringBuilder, ref num);
					if (num2 >= 0)
					{
						text = stringBuilder.ToString();
					}
					else
					{
						text = string.Format(CultureInfo.CurrentCulture, "{0} (0x{1:X}).", "Cannot obtain Local Database Runtime error message", num2);
					}
				}
			}
			catch (SqlException ex)
			{
				text = string.Format(CultureInfo.CurrentCulture, "{0} ({1}).", "Cannot obtain Local Database Runtime error message", ex.Message);
			}
			return text;
		}

		private static SqlException CreateLocalDBException(string errorMessage, string instance = null, int localDbError = 0, int sniError = 0)
		{
			SqlErrorCollection sqlErrorCollection = new SqlErrorCollection();
			int num = ((localDbError == 0) ? sniError : localDbError);
			if (sniError != 0)
			{
				string snierrorMessage = SQL.GetSNIErrorMessage(sniError);
				errorMessage = string.Format(null, "{0} (error: {1} - {2})", errorMessage, sniError, snierrorMessage);
			}
			sqlErrorCollection.Add(new SqlError(num, 0, 20, instance, errorMessage, null, 0, null));
			if (localDbError != 0)
			{
				sqlErrorCollection.Add(new SqlError(num, 0, 20, instance, LocalDBAPI.GetLocalDBMessage(localDbError), null, 0, null));
			}
			SqlException ex = SqlException.CreateException(sqlErrorCollection, null);
			ex._doNotReconnect = true;
			return ex;
		}

		private static IntPtr LoadProcAddress()
		{
			return SafeNativeMethods.GetProcAddress(LocalDBAPI.UserInstanceDLLHandle, "LocalDBFormatMessage");
		}

		private static IntPtr UserInstanceDLLHandle
		{
			get
			{
				if (LocalDBAPI.s_userInstanceDLLHandle == IntPtr.Zero)
				{
					object obj = LocalDBAPI.s_dllLock;
					lock (obj)
					{
						if (LocalDBAPI.s_userInstanceDLLHandle == IntPtr.Zero)
						{
							SNINativeMethodWrapper.SNIQueryInfo(SNINativeMethodWrapper.QTypes.SNI_QUERY_LOCALDB_HMODULE, ref LocalDBAPI.s_userInstanceDLLHandle);
							if (LocalDBAPI.s_userInstanceDLLHandle == IntPtr.Zero)
							{
								SNINativeMethodWrapper.SNI_Error sni_Error;
								SNINativeMethodWrapper.SNIGetLastError(out sni_Error);
								throw LocalDBAPI.CreateLocalDBException(SR.GetString("LocalDB_FailedGetDLLHandle"), null, 0, (int)sni_Error.sniError);
							}
						}
					}
				}
				return LocalDBAPI.s_userInstanceDLLHandle;
			}
		}

		internal static string GetLocalDbInstanceNameFromServerName(string serverName)
		{
			if (serverName == null)
			{
				return null;
			}
			serverName = serverName.TrimStart();
			if (!serverName.StartsWith("(localdb)\\", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			string text = serverName.Substring("(localdb)\\".Length).Trim();
			if (text.Length == 0)
			{
				return null;
			}
			return text;
		}

		private static LocalDBAPI.LocalDBFormatMessageDelegate s_localDBFormatMessage = null;

		private static IntPtr s_userInstanceDLLHandle = IntPtr.Zero;

		private static readonly object s_dllLock = new object();

		private const uint const_LOCALDB_TRUNCATE_ERR_MESSAGE = 1U;

		private const int const_ErrorMessageBufferSize = 1024;

		private const string const_localDbPrefix = "(localdb)\\";

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int LocalDBFormatMessageDelegate(int hrLocalDB, uint dwFlags, uint dwLanguageId, StringBuilder buffer, ref uint buflen);
	}
}
