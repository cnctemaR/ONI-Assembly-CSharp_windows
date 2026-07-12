using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.Data.SqlClient.SNI
{
	internal sealed class LocalDB
	{
		private LocalDB()
		{
		}

		internal static string GetLocalDBConnectionString(string localDbInstance)
		{
			if (!LocalDB.Instance.LoadUserInstanceDll())
			{
				return null;
			}
			return LocalDB.Instance.GetConnectionString(localDbInstance);
		}

		internal static IntPtr GetProcAddress(string functionName)
		{
			if (!LocalDB.Instance.LoadUserInstanceDll())
			{
				return IntPtr.Zero;
			}
			return global::Interop.Kernel32.GetProcAddress(LocalDB.Instance._sqlUserInstanceLibraryHandle, functionName);
		}

		private string GetConnectionString(string localDbInstance)
		{
			StringBuilder stringBuilder = new StringBuilder(261);
			int capacity = stringBuilder.Capacity;
			this.localDBStartInstanceFunc(localDbInstance, 0, stringBuilder, ref capacity);
			return stringBuilder.ToString();
		}

		internal static uint MapLocalDBErrorStateToCode(LocalDB.LocalDBErrorState errorState)
		{
			switch (errorState)
			{
			case LocalDB.LocalDBErrorState.NO_INSTALLATION:
				return 52U;
			case LocalDB.LocalDBErrorState.INVALID_CONFIG:
				return 53U;
			case LocalDB.LocalDBErrorState.NO_SQLUSERINSTANCEDLL_PATH:
				return 54U;
			case LocalDB.LocalDBErrorState.INVALID_SQLUSERINSTANCEDLL_PATH:
				return 55U;
			case LocalDB.LocalDBErrorState.NONE:
				return 0U;
			default:
				return 53U;
			}
		}

		private bool LoadUserInstanceDll()
		{
			if (this._sqlUserInstanceLibraryHandle != null)
			{
				return true;
			}
			bool flag2;
			lock (this)
			{
				if (this._sqlUserInstanceLibraryHandle != null)
				{
					flag2 = true;
				}
				else
				{
					LocalDB.LocalDBErrorState localDBErrorState;
					string userInstanceDllPath = this.GetUserInstanceDllPath(out localDBErrorState);
					if (userInstanceDllPath == null)
					{
						SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0U, LocalDB.MapLocalDBErrorStateToCode(localDBErrorState), string.Empty);
						flag2 = false;
					}
					else if (string.IsNullOrWhiteSpace(userInstanceDllPath))
					{
						SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0U, 55U, string.Empty);
						flag2 = false;
					}
					else
					{
						SafeLibraryHandle safeLibraryHandle = global::Interop.Kernel32.LoadLibraryExW(userInstanceDllPath.Trim(), IntPtr.Zero, 0U);
						if (safeLibraryHandle.IsInvalid)
						{
							SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0U, 56U, string.Empty);
							safeLibraryHandle.Dispose();
							flag2 = false;
						}
						else
						{
							this._startInstanceHandle = global::Interop.Kernel32.GetProcAddress(safeLibraryHandle, "LocalDBStartInstance");
							if (this._startInstanceHandle == IntPtr.Zero)
							{
								SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0U, 57U, string.Empty);
								safeLibraryHandle.Dispose();
								flag2 = false;
							}
							else
							{
								this.localDBStartInstanceFunc = (LocalDB.LocalDBStartInstance)Marshal.GetDelegateForFunctionPointer(this._startInstanceHandle, typeof(LocalDB.LocalDBStartInstance));
								if (this.localDBStartInstanceFunc == null)
								{
									SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0U, 57U, string.Empty);
									safeLibraryHandle.Dispose();
									this._startInstanceHandle = IntPtr.Zero;
									flag2 = false;
								}
								else
								{
									this._sqlUserInstanceLibraryHandle = safeLibraryHandle;
									flag2 = true;
								}
							}
						}
					}
				}
			}
			return flag2;
		}

		private string GetUserInstanceDllPath(out LocalDB.LocalDBErrorState errorState)
		{
			string text;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SQL Server Local DB\\Installed Versions\\"))
			{
				if (registryKey == null)
				{
					errorState = LocalDB.LocalDBErrorState.NO_INSTALLATION;
					text = null;
				}
				else
				{
					Version version = new Version();
					Version version2 = version;
					string[] subKeyNames = registryKey.GetSubKeyNames();
					for (int i = 0; i < subKeyNames.Length; i++)
					{
						Version version3;
						if (!Version.TryParse(subKeyNames[i], out version3))
						{
							errorState = LocalDB.LocalDBErrorState.INVALID_CONFIG;
							return null;
						}
						if (version2.CompareTo(version3) < 0)
						{
							version2 = version3;
						}
					}
					if (version2.Equals(version))
					{
						errorState = LocalDB.LocalDBErrorState.INVALID_CONFIG;
						text = null;
					}
					else
					{
						using (RegistryKey registryKey2 = registryKey.OpenSubKey(version2.ToString()))
						{
							object value = registryKey2.GetValue("InstanceAPIPath");
							if (value == null)
							{
								errorState = LocalDB.LocalDBErrorState.NO_SQLUSERINSTANCEDLL_PATH;
								text = null;
							}
							else if (registryKey2.GetValueKind("InstanceAPIPath") != RegistryValueKind.String)
							{
								errorState = LocalDB.LocalDBErrorState.INVALID_SQLUSERINSTANCEDLL_PATH;
								text = null;
							}
							else
							{
								string text2 = (string)value;
								errorState = LocalDB.LocalDBErrorState.NONE;
								text = text2;
							}
						}
					}
				}
			}
			return text;
		}

		private static readonly LocalDB Instance = new LocalDB();

		private const string LocalDBInstalledVersionRegistryKey = "SOFTWARE\\Microsoft\\Microsoft SQL Server Local DB\\Installed Versions\\";

		private const string InstanceAPIPathValueName = "InstanceAPIPath";

		private const string ProcLocalDBStartInstance = "LocalDBStartInstance";

		private const int MAX_LOCAL_DB_CONNECTION_STRING_SIZE = 260;

		private IntPtr _startInstanceHandle = IntPtr.Zero;

		private LocalDB.LocalDBStartInstance localDBStartInstanceFunc;

		private volatile SafeLibraryHandle _sqlUserInstanceLibraryHandle;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate int LocalDBStartInstance([MarshalAs(UnmanagedType.LPWStr)] [In] string localDBInstanceName, [In] int flags, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder sqlConnectionDataSource, [In] [Out] ref int bufferLength);

		internal enum LocalDBErrorState
		{
			NO_INSTALLATION,
			INVALID_CONFIG,
			NO_SQLUSERINSTANCEDLL_PATH,
			INVALID_SQLUSERINSTANCEDLL_PATH,
			NONE
		}
	}
}
