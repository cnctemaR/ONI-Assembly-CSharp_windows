using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Win32
{
	internal class Win32RegistryApi : IRegistryApi
	{
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegCreateKeyEx(IntPtr keyBase, string keyName, int reserved, IntPtr lpClass, int options, int access, IntPtr securityAttrs, out IntPtr keyHandle, out int disposition);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegCloseKey(IntPtr keyHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegConnectRegistry(string machineName, IntPtr hKey, out IntPtr keyHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegFlushKey(IntPtr keyHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegOpenKeyEx(IntPtr keyBase, string keyName, IntPtr reserved, int access, out IntPtr keyHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegDeleteKey(IntPtr keyHandle, string valueName);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegDeleteValue(IntPtr keyHandle, string valueName);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegEnumKeyExW")]
		internal unsafe static extern int RegEnumKeyEx(IntPtr keyHandle, int dwIndex, char* lpName, ref int lpcbName, int[] lpReserved, [Out] StringBuilder lpClass, int[] lpcbClass, long[] lpftLastWriteTime);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		internal unsafe static extern int RegEnumValue(IntPtr hKey, int dwIndex, char* lpValueName, ref int lpcbValueName, IntPtr lpReserved_MustBeZero, int[] lpType, byte[] lpData, int[] lpcbData);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, string data, int rawDataLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, byte[] rawData, int rawDataLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, ref int data, int rawDataLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, ref long data, int rawDataLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, IntPtr zero, ref int dataSize);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, [Out] byte[] data, ref int dataSize);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, ref int data, ref int dataSize);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, ref long data, ref int dataSize);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryInfoKeyW")]
		internal static extern int RegQueryInfoKey(IntPtr hKey, [Out] StringBuilder lpClass, int[] lpcbClass, IntPtr lpReserved_MustBeZero, ref int lpcSubKeys, int[] lpcbMaxSubKeyLen, int[] lpcbMaxClassLen, ref int lpcValues, int[] lpcbMaxValueNameLen, int[] lpcbMaxValueLen, int[] lpcbSecurityDescriptor, int[] lpftLastWriteTime);

		public IntPtr GetHandle(RegistryKey key)
		{
			return (IntPtr)key.InternalHandle;
		}

		private static bool IsHandleValid(RegistryKey key)
		{
			return key.InternalHandle != null;
		}

		public RegistryValueKind GetValueKind(RegistryKey rkey, string name)
		{
			RegistryValueKind registryValueKind = RegistryValueKind.Unknown;
			int num = 0;
			int num2 = Win32RegistryApi.RegQueryValueEx(this.GetHandle(rkey), name, IntPtr.Zero, ref registryValueKind, IntPtr.Zero, ref num);
			if (num2 == 2 || num2 == 1018)
			{
				return RegistryValueKind.Unknown;
			}
			return registryValueKind;
		}

		public object GetValue(RegistryKey rkey, string name, object defaultValue, RegistryValueOptions options)
		{
			RegistryValueKind registryValueKind = RegistryValueKind.Unknown;
			int num = 0;
			IntPtr handle = this.GetHandle(rkey);
			int num2 = Win32RegistryApi.RegQueryValueEx(handle, name, IntPtr.Zero, ref registryValueKind, IntPtr.Zero, ref num);
			if (num2 == 2 || num2 == 1018)
			{
				return defaultValue;
			}
			if (num2 != 234 && num2 != 0)
			{
				this.GenerateException(num2);
			}
			object obj;
			if (registryValueKind == RegistryValueKind.String)
			{
				byte[] array;
				num2 = this.GetBinaryValue(rkey, name, registryValueKind, out array, num);
				obj = RegistryKey.DecodeString(array);
			}
			else if (registryValueKind == RegistryValueKind.ExpandString)
			{
				byte[] array2;
				num2 = this.GetBinaryValue(rkey, name, registryValueKind, out array2, num);
				obj = RegistryKey.DecodeString(array2);
				if ((options & RegistryValueOptions.DoNotExpandEnvironmentNames) == RegistryValueOptions.None)
				{
					obj = Environment.ExpandEnvironmentVariables((string)obj);
				}
			}
			else if (registryValueKind == RegistryValueKind.DWord)
			{
				int num3 = 0;
				num2 = Win32RegistryApi.RegQueryValueEx(handle, name, IntPtr.Zero, ref registryValueKind, ref num3, ref num);
				obj = num3;
			}
			else if (registryValueKind == RegistryValueKind.QWord)
			{
				long num4 = 0L;
				num2 = Win32RegistryApi.RegQueryValueEx(handle, name, IntPtr.Zero, ref registryValueKind, ref num4, ref num);
				obj = num4;
			}
			else if (registryValueKind == RegistryValueKind.Binary)
			{
				byte[] array3;
				num2 = this.GetBinaryValue(rkey, name, registryValueKind, out array3, num);
				obj = array3;
			}
			else
			{
				if (registryValueKind != RegistryValueKind.MultiString)
				{
					throw new SystemException();
				}
				obj = null;
				byte[] array4;
				num2 = this.GetBinaryValue(rkey, name, registryValueKind, out array4, num);
				if (num2 == 0)
				{
					obj = RegistryKey.DecodeString(array4).Split(new char[1]);
				}
			}
			if (num2 != 0)
			{
				this.GenerateException(num2);
			}
			return obj;
		}

		public void SetValue(RegistryKey rkey, string name, object value, RegistryValueKind valueKind)
		{
			Type type = value.GetType();
			IntPtr handle = this.GetHandle(rkey);
			switch (valueKind)
			{
			case RegistryValueKind.String:
			case RegistryValueKind.ExpandString:
				if (type == typeof(string))
				{
					string text = string.Format("{0}{1}", value, '\0');
					this.CheckResult(Win32RegistryApi.RegSetValueEx(handle, name, IntPtr.Zero, valueKind, text, text.Length * this.NativeBytesPerCharacter));
					return;
				}
				goto IL_01B7;
			case RegistryValueKind.Binary:
				goto IL_009C;
			case RegistryValueKind.DWord:
				break;
			case (RegistryValueKind)5:
			case (RegistryValueKind)6:
			case (RegistryValueKind)8:
			case (RegistryValueKind)9:
			case (RegistryValueKind)10:
				goto IL_01A4;
			case RegistryValueKind.MultiString:
				if (type == typeof(string[]))
				{
					string[] array = (string[])value;
					StringBuilder stringBuilder = new StringBuilder();
					foreach (string text2 in array)
					{
						stringBuilder.Append(text2);
						stringBuilder.Append('\0');
					}
					stringBuilder.Append('\0');
					byte[] bytes = Encoding.Unicode.GetBytes(stringBuilder.ToString());
					this.CheckResult(Win32RegistryApi.RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.MultiString, bytes, bytes.Length));
					return;
				}
				goto IL_01B7;
			case RegistryValueKind.QWord:
				try
				{
					long num = Convert.ToInt64(value);
					this.CheckResult(Win32RegistryApi.RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.QWord, ref num, 8));
					return;
				}
				catch (OverflowException)
				{
					goto IL_01B7;
				}
				break;
			default:
				goto IL_01A4;
			}
			try
			{
				int num2 = Convert.ToInt32(value);
				this.CheckResult(Win32RegistryApi.RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.DWord, ref num2, 4));
				return;
			}
			catch (OverflowException)
			{
				goto IL_01B7;
			}
			IL_009C:
			if (type == typeof(byte[]))
			{
				byte[] array3 = (byte[])value;
				this.CheckResult(Win32RegistryApi.RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.Binary, array3, array3.Length));
				return;
			}
			goto IL_01B7;
			IL_01A4:
			if (type.IsArray)
			{
				throw new ArgumentException("Only string and byte arrays can written as registry values");
			}
			IL_01B7:
			throw new ArgumentException("Type does not match the valueKind");
		}

		public void SetValue(RegistryKey rkey, string name, object value)
		{
			Type type = value.GetType();
			IntPtr handle = this.GetHandle(rkey);
			int num2;
			if (type == typeof(int))
			{
				int num = (int)value;
				num2 = Win32RegistryApi.RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.DWord, ref num, 4);
			}
			else if (type == typeof(byte[]))
			{
				byte[] array = (byte[])value;
				num2 = Win32RegistryApi.RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.Binary, array, array.Length);
			}
			else if (type == typeof(string[]))
			{
				string[] array2 = (string[])value;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in array2)
				{
					stringBuilder.Append(text);
					stringBuilder.Append('\0');
				}
				stringBuilder.Append('\0');
				byte[] bytes = Encoding.Unicode.GetBytes(stringBuilder.ToString());
				num2 = Win32RegistryApi.RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.MultiString, bytes, bytes.Length);
			}
			else
			{
				if (type.IsArray)
				{
					throw new ArgumentException("Only string and byte arrays can written as registry values");
				}
				string text2 = string.Format("{0}{1}", value, '\0');
				num2 = Win32RegistryApi.RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.String, text2, text2.Length * this.NativeBytesPerCharacter);
			}
			if (num2 != 0)
			{
				this.GenerateException(num2);
			}
		}

		private int GetBinaryValue(RegistryKey rkey, string name, RegistryValueKind type, out byte[] data, int size)
		{
			byte[] array = new byte[size];
			int num = Win32RegistryApi.RegQueryValueEx(this.GetHandle(rkey), name, IntPtr.Zero, ref type, array, ref size);
			data = array;
			return num;
		}

		public int SubKeyCount(RegistryKey rkey)
		{
			int num = 0;
			int num2 = 0;
			int num3 = Win32RegistryApi.RegQueryInfoKey(this.GetHandle(rkey), null, null, IntPtr.Zero, ref num, null, null, ref num2, null, null, null, null);
			if (num3 != 0)
			{
				this.GenerateException(num3);
			}
			return num;
		}

		public int ValueCount(RegistryKey rkey)
		{
			int num = 0;
			int num2 = 0;
			int num3 = Win32RegistryApi.RegQueryInfoKey(this.GetHandle(rkey), null, null, IntPtr.Zero, ref num2, null, null, ref num, null, null, null, null);
			if (num3 != 0)
			{
				this.GenerateException(num3);
			}
			return num;
		}

		public RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName)
		{
			IntPtr intPtr = new IntPtr((int)hKey);
			IntPtr intPtr2;
			int num = Win32RegistryApi.RegConnectRegistry(machineName, intPtr, out intPtr2);
			if (num != 0)
			{
				this.GenerateException(num);
			}
			return new RegistryKey(hKey, intPtr2, true);
		}

		public RegistryKey OpenSubKey(RegistryKey rkey, string keyName, bool writable)
		{
			int num = 131097;
			if (writable)
			{
				num |= 131078;
			}
			IntPtr intPtr;
			int num2 = Win32RegistryApi.RegOpenKeyEx(this.GetHandle(rkey), keyName, IntPtr.Zero, num, out intPtr);
			if (num2 == 2 || num2 == 1018)
			{
				return null;
			}
			if (num2 != 0)
			{
				this.GenerateException(num2);
			}
			return new RegistryKey(intPtr, Win32RegistryApi.CombineName(rkey, keyName), writable);
		}

		public void Flush(RegistryKey rkey)
		{
			if (!Win32RegistryApi.IsHandleValid(rkey))
			{
				return;
			}
			Win32RegistryApi.RegFlushKey(this.GetHandle(rkey));
		}

		public void Close(RegistryKey rkey)
		{
			if (!Win32RegistryApi.IsHandleValid(rkey))
			{
				return;
			}
			SafeRegistryHandle handle = rkey.Handle;
			if (handle != null)
			{
				handle.Close();
				return;
			}
			Win32RegistryApi.RegCloseKey(this.GetHandle(rkey));
		}

		public RegistryKey FromHandle(SafeRegistryHandle handle)
		{
			return new RegistryKey(handle.DangerousGetHandle(), string.Empty, true);
		}

		public RegistryKey CreateSubKey(RegistryKey rkey, string keyName)
		{
			IntPtr intPtr;
			int num2;
			int num = Win32RegistryApi.RegCreateKeyEx(this.GetHandle(rkey), keyName, 0, IntPtr.Zero, 0, 131103, IntPtr.Zero, out intPtr, out num2);
			if (num != 0)
			{
				this.GenerateException(num);
			}
			return new RegistryKey(intPtr, Win32RegistryApi.CombineName(rkey, keyName), true);
		}

		public RegistryKey CreateSubKey(RegistryKey rkey, string keyName, RegistryOptions options)
		{
			IntPtr intPtr;
			int num2;
			int num = Win32RegistryApi.RegCreateKeyEx(this.GetHandle(rkey), keyName, 0, IntPtr.Zero, (options == RegistryOptions.Volatile) ? 1 : 0, 131103, IntPtr.Zero, out intPtr, out num2);
			if (num != 0)
			{
				this.GenerateException(num);
			}
			return new RegistryKey(intPtr, Win32RegistryApi.CombineName(rkey, keyName), true);
		}

		public void DeleteKey(RegistryKey rkey, string keyName, bool shouldThrowWhenKeyMissing)
		{
			int num = Win32RegistryApi.RegDeleteKey(this.GetHandle(rkey), keyName);
			if (num != 2)
			{
				if (num != 0)
				{
					this.GenerateException(num);
				}
				return;
			}
			if (shouldThrowWhenKeyMissing)
			{
				throw new ArgumentException("key " + keyName);
			}
		}

		public void DeleteValue(RegistryKey rkey, string value, bool shouldThrowWhenKeyMissing)
		{
			int num = Win32RegistryApi.RegDeleteValue(this.GetHandle(rkey), value);
			if (num == 1018)
			{
				return;
			}
			if (num != 2)
			{
				if (num != 0)
				{
					this.GenerateException(num);
				}
				return;
			}
			if (shouldThrowWhenKeyMissing)
			{
				throw new ArgumentException("value " + value);
			}
		}

		public unsafe string[] GetSubKeyNames(RegistryKey rkey)
		{
			int num = this.SubKeyCount(rkey);
			string[] array = new string[num];
			if (num > 0)
			{
				IntPtr handle = this.GetHandle(rkey);
				char[] array2 = new char[256];
				fixed (char* ptr = &array2[0])
				{
					char* ptr2 = ptr;
					for (int i = 0; i < num; i++)
					{
						int num2 = array2.Length;
						int num3 = Win32RegistryApi.RegEnumKeyEx(handle, i, ptr2, ref num2, null, null, null, null);
						if (num3 != 0)
						{
							this.GenerateException(num3);
						}
						array[i] = new string(ptr2);
					}
				}
			}
			return array;
		}

		public unsafe string[] GetValueNames(RegistryKey rkey)
		{
			int num = this.ValueCount(rkey);
			string[] array = new string[num];
			if (num > 0)
			{
				IntPtr handle = this.GetHandle(rkey);
				char[] array2 = new char[16384];
				fixed (char* ptr = &array2[0])
				{
					char* ptr2 = ptr;
					for (int i = 0; i < num; i++)
					{
						int num2 = array2.Length;
						int num3 = Win32RegistryApi.RegEnumValue(handle, i, ptr2, ref num2, IntPtr.Zero, null, null, null);
						if (num3 != 0 && num3 != 234)
						{
							this.GenerateException(num3);
						}
						array[i] = new string(ptr2);
					}
				}
			}
			return array;
		}

		private void CheckResult(int result)
		{
			if (result != 0)
			{
				this.GenerateException(result);
			}
		}

		private void GenerateException(int errorCode)
		{
			if (errorCode <= 53)
			{
				switch (errorCode)
				{
				case 2:
					break;
				case 3:
				case 4:
					goto IL_0072;
				case 5:
					throw new SecurityException();
				case 6:
					throw new IOException("Invalid handle.");
				default:
					if (errorCode != 53)
					{
						goto IL_0072;
					}
					throw new IOException("The network path was not found.");
				}
			}
			else if (errorCode != 87)
			{
				if (errorCode == 1018)
				{
					throw RegistryKey.CreateMarkedForDeletionException();
				}
				if (errorCode != 1021)
				{
					goto IL_0072;
				}
				throw new IOException("Cannot create a stable subkey under a volatile parent key.");
			}
			throw new ArgumentException();
			IL_0072:
			throw new SystemException();
		}

		public string ToString(RegistryKey rkey)
		{
			return rkey.Name;
		}

		internal static string CombineName(RegistryKey rkey, string localName)
		{
			return rkey.Name + "\\" + localName;
		}

		private const int OpenRegKeyRead = 131097;

		private const int OpenRegKeyWrite = 131078;

		private const int Int32ByteSize = 4;

		private const int Int64ByteSize = 8;

		private readonly int NativeBytesPerCharacter = Marshal.SystemDefaultCharSize;

		private const int RegOptionsNonVolatile = 0;

		private const int RegOptionsVolatile = 1;

		private const int MaxKeyLength = 255;

		private const int MaxValueLength = 16383;
	}
}
