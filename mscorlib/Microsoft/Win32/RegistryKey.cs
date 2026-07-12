using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Unity;

namespace Microsoft.Win32
{
	public sealed class RegistryKey : MarshalByRefObject, IDisposable
	{
		private void ClosePerfDataKey()
		{
			Interop.Advapi32.RegCloseKey(RegistryKey.HKEY_PERFORMANCE_DATA);
		}

		private void FlushCore()
		{
			if (this._hkey != null && this.IsDirty())
			{
				Interop.Advapi32.RegFlushKey(this._hkey);
			}
		}

		private RegistryKey CreateSubKeyInternalCore(string subkey, RegistryKeyPermissionCheck permissionCheck, object registrySecurityObj, RegistryOptions registryOptions)
		{
			Interop.Kernel32.SECURITY_ATTRIBUTES security_ATTRIBUTES = default(Interop.Kernel32.SECURITY_ATTRIBUTES);
			int num = 0;
			SafeRegistryHandle safeRegistryHandle = null;
			int num2 = Interop.Advapi32.RegCreateKeyEx(this._hkey, subkey, 0, null, (int)registryOptions, RegistryKey.GetRegistryKeyAccess(permissionCheck != RegistryKeyPermissionCheck.ReadSubTree) | (int)this._regView, ref security_ATTRIBUTES, out safeRegistryHandle, out num);
			if (num2 == 0 && !safeRegistryHandle.IsInvalid)
			{
				RegistryKey registryKey = new RegistryKey(safeRegistryHandle, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree, false, this._remoteKey, false, this._regView);
				registryKey._checkMode = permissionCheck;
				if (subkey.Length == 0)
				{
					registryKey._keyName = this._keyName;
				}
				else
				{
					registryKey._keyName = this._keyName + "\\" + subkey;
				}
				return registryKey;
			}
			if (num2 != 0)
			{
				this.Win32Error(num2, this._keyName + "\\" + subkey);
			}
			return null;
		}

		private void DeleteSubKeyCore(string subkey, bool throwOnMissingSubKey)
		{
			int num = Interop.Advapi32.RegDeleteKeyEx(this._hkey, subkey, (int)this._regView, 0);
			if (num != 0)
			{
				if (num == 2)
				{
					if (throwOnMissingSubKey)
					{
						ThrowHelper.ThrowArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
						return;
					}
				}
				else
				{
					this.Win32Error(num, null);
				}
			}
		}

		private void DeleteSubKeyTreeCore(string subkey)
		{
			int num = Interop.Advapi32.RegDeleteKeyEx(this._hkey, subkey, (int)this._regView, 0);
			if (num != 0)
			{
				this.Win32Error(num, null);
			}
		}

		private void DeleteValueCore(string name, bool throwOnMissingValue)
		{
			int num = Interop.Advapi32.RegDeleteValue(this._hkey, name);
			if (num == 2 || num == 206)
			{
				if (throwOnMissingValue)
				{
					ThrowHelper.ThrowArgumentException("No value exists with that name.");
					return;
				}
			}
		}

		private static RegistryKey OpenBaseKeyCore(RegistryHive hKeyHive, RegistryView view)
		{
			IntPtr intPtr = (IntPtr)((int)hKeyHive);
			int num = (int)intPtr & 268435455;
			bool flag = intPtr == RegistryKey.HKEY_PERFORMANCE_DATA;
			return new RegistryKey(new SafeRegistryHandle(intPtr, flag), true, true, false, flag, view)
			{
				_checkMode = RegistryKeyPermissionCheck.Default,
				_keyName = RegistryKey.s_hkeyNames[num]
			};
		}

		private static RegistryKey OpenRemoteBaseKeyCore(RegistryHive hKey, string machineName, RegistryView view)
		{
			int num = (int)(hKey & (RegistryHive)268435455);
			if (num < 0 || num >= RegistryKey.s_hkeyNames.Length || ((long)hKey & (long)((ulong)(-16))) != (long)((ulong)(-2147483648)))
			{
				throw new ArgumentException("Registry HKEY was out of the legal range.");
			}
			SafeRegistryHandle safeRegistryHandle = null;
			int num2 = Interop.Advapi32.RegConnectRegistry(machineName, new SafeRegistryHandle(new IntPtr((int)hKey), false), out safeRegistryHandle);
			if (num2 == 1114)
			{
				throw new ArgumentException("One machine may not have remote administration enabled, or both machines may not be running the remote registry service.");
			}
			if (num2 != 0)
			{
				RegistryKey.Win32ErrorStatic(num2, null);
			}
			if (safeRegistryHandle.IsInvalid)
			{
				throw new ArgumentException(SR.Format("No remote connection to '{0}' while trying to read the registry.", machineName));
			}
			return new RegistryKey(safeRegistryHandle, true, false, true, (IntPtr)((int)hKey) == RegistryKey.HKEY_PERFORMANCE_DATA, view)
			{
				_checkMode = RegistryKeyPermissionCheck.Default,
				_keyName = RegistryKey.s_hkeyNames[num]
			};
		}

		private RegistryKey InternalOpenSubKeyCore(string name, RegistryKeyPermissionCheck permissionCheck, int rights, bool throwOnPermissionFailure)
		{
			SafeRegistryHandle safeRegistryHandle = null;
			int num = Interop.Advapi32.RegOpenKeyEx(this._hkey, name, 0, rights | (int)this._regView, out safeRegistryHandle);
			if (num == 0 && !safeRegistryHandle.IsInvalid)
			{
				return new RegistryKey(safeRegistryHandle, permissionCheck == RegistryKeyPermissionCheck.ReadWriteSubTree, false, this._remoteKey, false, this._regView)
				{
					_keyName = this._keyName + "\\" + name,
					_checkMode = permissionCheck
				};
			}
			if (throwOnPermissionFailure && (num == 5 || num == 1346))
			{
				ThrowHelper.ThrowSecurityException("Requested registry access is not allowed.");
			}
			return null;
		}

		private RegistryKey InternalOpenSubKeyCore(string name, bool writable, bool throwOnPermissionFailure)
		{
			SafeRegistryHandle safeRegistryHandle = null;
			int num = Interop.Advapi32.RegOpenKeyEx(this._hkey, name, 0, RegistryKey.GetRegistryKeyAccess(writable) | (int)this._regView, out safeRegistryHandle);
			if (num == 0 && !safeRegistryHandle.IsInvalid)
			{
				return new RegistryKey(safeRegistryHandle, writable, false, this._remoteKey, false, this._regView)
				{
					_checkMode = this.GetSubKeyPermissionCheck(writable),
					_keyName = this._keyName + "\\" + name
				};
			}
			if (throwOnPermissionFailure && (num == 5 || num == 1346))
			{
				ThrowHelper.ThrowSecurityException("Requested registry access is not allowed.");
			}
			return null;
		}

		internal RegistryKey InternalOpenSubKeyWithoutSecurityChecksCore(string name, bool writable)
		{
			SafeRegistryHandle safeRegistryHandle = null;
			if (Interop.Advapi32.RegOpenKeyEx(this._hkey, name, 0, RegistryKey.GetRegistryKeyAccess(writable) | (int)this._regView, out safeRegistryHandle) == 0 && !safeRegistryHandle.IsInvalid)
			{
				return new RegistryKey(safeRegistryHandle, writable, false, this._remoteKey, false, this._regView)
				{
					_keyName = this._keyName + "\\" + name
				};
			}
			return null;
		}

		private SafeRegistryHandle SystemKeyHandle
		{
			get
			{
				int num = 6;
				IntPtr intPtr = (IntPtr)0;
				string keyName = this._keyName;
				if (!(keyName == "HKEY_CLASSES_ROOT"))
				{
					if (!(keyName == "HKEY_CURRENT_USER"))
					{
						if (!(keyName == "HKEY_LOCAL_MACHINE"))
						{
							if (!(keyName == "HKEY_USERS"))
							{
								if (!(keyName == "HKEY_PERFORMANCE_DATA"))
								{
									if (!(keyName == "HKEY_CURRENT_CONFIG"))
									{
										this.Win32Error(num, null);
									}
									else
									{
										intPtr = RegistryKey.HKEY_CURRENT_CONFIG;
									}
								}
								else
								{
									intPtr = RegistryKey.HKEY_PERFORMANCE_DATA;
								}
							}
							else
							{
								intPtr = RegistryKey.HKEY_USERS;
							}
						}
						else
						{
							intPtr = RegistryKey.HKEY_LOCAL_MACHINE;
						}
					}
					else
					{
						intPtr = RegistryKey.HKEY_CURRENT_USER;
					}
				}
				else
				{
					intPtr = RegistryKey.HKEY_CLASSES_ROOT;
				}
				SafeRegistryHandle safeRegistryHandle;
				num = Interop.Advapi32.RegOpenKeyEx(intPtr, null, 0, RegistryKey.GetRegistryKeyAccess(this.IsWritable()) | (int)this._regView, out safeRegistryHandle);
				if (num == 0 && !safeRegistryHandle.IsInvalid)
				{
					return safeRegistryHandle;
				}
				this.Win32Error(num, null);
				throw new IOException(Interop.Kernel32.GetMessage(num), num);
			}
		}

		private int InternalSubKeyCountCore()
		{
			int num = 0;
			int num2 = 0;
			int num3 = Interop.Advapi32.RegQueryInfoKey(this._hkey, null, null, IntPtr.Zero, ref num, null, null, ref num2, null, null, null, null);
			if (num3 != 0)
			{
				this.Win32Error(num3, null);
			}
			return num;
		}

		private string[] InternalGetSubKeyNamesCore(int subkeys)
		{
			List<string> list = new List<string>(subkeys);
			char[] array = ArrayPool<char>.Shared.Rent(256);
			try
			{
				int num = array.Length;
				int num2;
				while ((num2 = Interop.Advapi32.RegEnumKeyEx(this._hkey, list.Count, array, ref num, null, null, null, null)) != 259)
				{
					if (num2 == 0)
					{
						list.Add(new string(array, 0, num));
						num = array.Length;
					}
					else
					{
						this.Win32Error(num2, null);
					}
				}
			}
			finally
			{
				ArrayPool<char>.Shared.Return(array, false);
			}
			return list.ToArray();
		}

		private int InternalValueCountCore()
		{
			int num = 0;
			int num2 = 0;
			int num3 = Interop.Advapi32.RegQueryInfoKey(this._hkey, null, null, IntPtr.Zero, ref num2, null, null, ref num, null, null, null, null);
			if (num3 != 0)
			{
				this.Win32Error(num3, null);
			}
			return num;
		}

		private unsafe string[] GetValueNamesCore(int values)
		{
			List<string> list = new List<string>(values);
			char[] array = ArrayPool<char>.Shared.Rent(100);
			try
			{
				int num = array.Length;
				int num2;
				while ((num2 = Interop.Advapi32.RegEnumValue(this._hkey, list.Count, array, ref num, IntPtr.Zero, null, null, null)) != 259)
				{
					if (num2 != 0)
					{
						if (num2 != 234)
						{
							this.Win32Error(num2, null);
						}
						else
						{
							if (this.IsPerfDataKey())
							{
								try
								{
									fixed (char* ptr = &array[0])
									{
										char* ptr2 = ptr;
										list.Add(new string(ptr2));
										goto IL_0092;
									}
								}
								finally
								{
									char* ptr = null;
								}
							}
							char[] array2 = array;
							int num3 = array2.Length;
							array = null;
							ArrayPool<char>.Shared.Return(array2, false);
							array = ArrayPool<char>.Shared.Rent(checked(num3 * 2));
						}
					}
					else
					{
						list.Add(new string(array, 0, num));
					}
					IL_0092:
					num = array.Length;
				}
			}
			finally
			{
				if (array != null)
				{
					ArrayPool<char>.Shared.Return(array, false);
				}
			}
			return list.ToArray();
		}

		private object InternalGetValueCore(string name, object defaultValue, bool doNotExpand)
		{
			object obj = defaultValue;
			int num = 0;
			int num2 = 0;
			int num3 = Interop.Advapi32.RegQueryValueEx(this._hkey, name, null, ref num, null, ref num2);
			if (num3 != 0)
			{
				if (this.IsPerfDataKey())
				{
					int num4 = 65000;
					int num5 = num4;
					byte[] array = new byte[num4];
					int num6;
					while (234 == (num6 = Interop.Advapi32.RegQueryValueEx(this._hkey, name, null, ref num, array, ref num5)))
					{
						if (num4 == 2147483647)
						{
							this.Win32Error(num6, name);
						}
						else if (num4 > 1073741823)
						{
							num4 = int.MaxValue;
						}
						else
						{
							num4 *= 2;
						}
						num5 = num4;
						array = new byte[num4];
					}
					if (num6 != 0)
					{
						this.Win32Error(num6, name);
					}
					return array;
				}
				if (num3 != 234)
				{
					return obj;
				}
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			switch (num)
			{
			case 0:
			case 3:
			case 5:
				break;
			case 1:
			{
				char[] array2;
				checked
				{
					if (num2 % 2 == 1)
					{
						try
						{
							num2++;
						}
						catch (OverflowException ex)
						{
							throw new IOException("RegistryKey.GetValue does not allow a String that has a length greater than Int32.MaxValue.", ex);
						}
					}
					array2 = new char[num2 / 2];
					num3 = Interop.Advapi32.RegQueryValueEx(this._hkey, name, null, ref num, array2, ref num2);
				}
				if (array2.Length != 0 && array2[array2.Length - 1] == '\0')
				{
					return new string(array2, 0, array2.Length - 1);
				}
				return new string(array2);
			}
			case 2:
			{
				char[] array3;
				checked
				{
					if (num2 % 2 == 1)
					{
						try
						{
							num2++;
						}
						catch (OverflowException ex2)
						{
							throw new IOException("RegistryKey.GetValue does not allow a String that has a length greater than Int32.MaxValue.", ex2);
						}
					}
					array3 = new char[num2 / 2];
					num3 = Interop.Advapi32.RegQueryValueEx(this._hkey, name, null, ref num, array3, ref num2);
				}
				if (array3.Length != 0 && array3[array3.Length - 1] == '\0')
				{
					obj = new string(array3, 0, array3.Length - 1);
				}
				else
				{
					obj = new string(array3);
				}
				if (!doNotExpand)
				{
					return Environment.ExpandEnvironmentVariables((string)obj);
				}
				return obj;
			}
			case 4:
				if (num2 <= 4)
				{
					int num7 = 0;
					num3 = Interop.Advapi32.RegQueryValueEx(this._hkey, name, null, ref num, ref num7, ref num2);
					return num7;
				}
				goto IL_0118;
			case 6:
			case 8:
			case 9:
			case 10:
				return obj;
			case 7:
			{
				char[] array4;
				checked
				{
					if (num2 % 2 == 1)
					{
						try
						{
							num2++;
						}
						catch (OverflowException ex3)
						{
							throw new IOException("RegistryKey.GetValue does not allow a String that has a length greater than Int32.MaxValue.", ex3);
						}
					}
					array4 = new char[num2 / 2];
					num3 = Interop.Advapi32.RegQueryValueEx(this._hkey, name, null, ref num, array4, ref num2);
				}
				if (array4.Length != 0 && array4[array4.Length - 1] != '\0')
				{
					Array.Resize<char>(ref array4, array4.Length + 1);
				}
				string[] array5 = Array.Empty<string>();
				int num8 = 0;
				int num9 = 0;
				int num10 = array4.Length;
				while (num3 == 0 && num9 < num10)
				{
					int num11 = num9;
					while (num11 < num10 && array4[num11] != '\0')
					{
						num11++;
					}
					string text = null;
					if (num11 < num10)
					{
						if (num11 - num9 > 0)
						{
							text = new string(array4, num9, num11 - num9);
						}
						else if (num11 != num10 - 1)
						{
							text = string.Empty;
						}
					}
					else
					{
						text = new string(array4, num9, num10 - num9);
					}
					num9 = num11 + 1;
					if (text != null)
					{
						if (array5.Length == num8)
						{
							Array.Resize<string>(ref array5, (num8 > 0) ? (num8 * 2) : 4);
						}
						array5[num8++] = text;
					}
				}
				Array.Resize<string>(ref array5, num8);
				return array5;
			}
			case 11:
				goto IL_0118;
			default:
				return obj;
			}
			IL_00F2:
			byte[] array6 = new byte[num2];
			num3 = Interop.Advapi32.RegQueryValueEx(this._hkey, name, null, ref num, array6, ref num2);
			return array6;
			IL_0118:
			if (num2 > 8)
			{
				goto IL_00F2;
			}
			long num12 = 0L;
			num3 = Interop.Advapi32.RegQueryValueEx(this._hkey, name, null, ref num, ref num12, ref num2);
			obj = num12;
			return obj;
		}

		private RegistryValueKind GetValueKindCore(string name)
		{
			int num = 0;
			int num2 = 0;
			int num3 = Interop.Advapi32.RegQueryValueEx(this._hkey, name, null, ref num, null, ref num2);
			if (num3 != 0)
			{
				this.Win32Error(num3, null);
			}
			if (num == 0)
			{
				return RegistryValueKind.None;
			}
			if (Enum.IsDefined(typeof(RegistryValueKind), num))
			{
				return (RegistryValueKind)num;
			}
			return RegistryValueKind.Unknown;
		}

		private void SetValueCore(string name, object value, RegistryValueKind valueKind)
		{
			int num = 0;
			try
			{
				switch (valueKind)
				{
				case RegistryValueKind.None:
				case RegistryValueKind.Binary:
				{
					byte[] array = (byte[])value;
					num = Interop.Advapi32.RegSetValueEx(this._hkey, name, 0, (valueKind == RegistryValueKind.None) ? RegistryValueKind.Unknown : RegistryValueKind.Binary, array, array.Length);
					break;
				}
				case RegistryValueKind.String:
				case RegistryValueKind.ExpandString:
				{
					string text = value.ToString();
					num = Interop.Advapi32.RegSetValueEx(this._hkey, name, 0, valueKind, text, checked(text.Length * 2 + 2));
					break;
				}
				case RegistryValueKind.DWord:
				{
					int num2 = Convert.ToInt32(value, CultureInfo.InvariantCulture);
					num = Interop.Advapi32.RegSetValueEx(this._hkey, name, 0, RegistryValueKind.DWord, ref num2, 4);
					break;
				}
				case RegistryValueKind.MultiString:
				{
					string[] array2 = (string[])((string[])value).Clone();
					int num3 = 1;
					for (int i = 0; i < array2.Length; i++)
					{
						if (array2[i] == null)
						{
							ThrowHelper.ThrowArgumentException("RegistryKey.SetValue does not allow a String[] that contains a null String reference.");
						}
						checked
						{
							num3 += array2[i].Length + 1;
						}
					}
					int num4 = checked(num3 * 2);
					char[] array3 = new char[num3];
					int num5 = 0;
					for (int j = 0; j < array2.Length; j++)
					{
						int length = array2[j].Length;
						array2[j].CopyTo(0, array3, num5, length);
						num5 += length + 1;
					}
					num = Interop.Advapi32.RegSetValueEx(this._hkey, name, 0, RegistryValueKind.MultiString, array3, num4);
					break;
				}
				case RegistryValueKind.QWord:
				{
					long num6 = Convert.ToInt64(value, CultureInfo.InvariantCulture);
					num = Interop.Advapi32.RegSetValueEx(this._hkey, name, 0, RegistryValueKind.QWord, ref num6, 8);
					break;
				}
				}
			}
			catch (Exception ex) when (ex is OverflowException || ex is InvalidOperationException || ex is FormatException || ex is InvalidCastException)
			{
				ThrowHelper.ThrowArgumentException("The type of the value object did not match the specified RegistryValueKind or the object could not be properly converted.");
			}
			if (num == 0)
			{
				this.SetDirty();
				return;
			}
			this.Win32Error(num, null);
		}

		private void Win32Error(int errorCode, string str)
		{
			switch (errorCode)
			{
			case 2:
				throw new IOException("The specified registry key does not exist.", errorCode);
			case 5:
				throw (str != null) ? new UnauthorizedAccessException(SR.Format("Access to the registry key '{0}' is denied.", str)) : new UnauthorizedAccessException();
			case 6:
				if (!this.IsPerfDataKey())
				{
					this._hkey.SetHandleAsInvalid();
					this._hkey = null;
				}
				break;
			}
			throw new IOException(Interop.Kernel32.GetMessage(errorCode), errorCode);
		}

		private static void Win32ErrorStatic(int errorCode, string str)
		{
			if (errorCode == 5)
			{
				throw (str != null) ? new UnauthorizedAccessException(SR.Format("Access to the registry key '{0}' is denied.", str)) : new UnauthorizedAccessException();
			}
			throw new IOException(Interop.Kernel32.GetMessage(errorCode), errorCode);
		}

		private static int GetRegistryKeyAccess(bool isWritable)
		{
			int num;
			if (!isWritable)
			{
				num = 131097;
			}
			else
			{
				num = 131103;
			}
			return num;
		}

		private static int GetRegistryKeyAccess(RegistryKeyPermissionCheck mode)
		{
			int num = 0;
			if (mode > RegistryKeyPermissionCheck.ReadSubTree)
			{
				if (mode == RegistryKeyPermissionCheck.ReadWriteSubTree)
				{
					num = 131103;
				}
			}
			else
			{
				num = 131097;
			}
			return num;
		}

		private RegistryKey(SafeRegistryHandle hkey, bool writable, RegistryView view)
			: this(hkey, writable, false, false, false, view)
		{
		}

		private RegistryKey(SafeRegistryHandle hkey, bool writable, bool systemkey, bool remoteKey, bool isPerfData, RegistryView view)
		{
			RegistryKey.ValidateKeyView(view);
			this._hkey = hkey;
			this._keyName = "";
			this._remoteKey = remoteKey;
			this._regView = view;
			if (systemkey)
			{
				this._state |= RegistryKey.StateFlags.SystemKey;
			}
			if (writable)
			{
				this._state |= RegistryKey.StateFlags.WriteAccess;
			}
			if (isPerfData)
			{
				this._state |= RegistryKey.StateFlags.PerfData;
			}
		}

		public void Flush()
		{
			this.FlushCore();
		}

		public void Close()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (this._hkey != null)
			{
				if (!this.IsSystemKey())
				{
					try
					{
						this._hkey.Dispose();
						return;
					}
					catch (IOException)
					{
						return;
					}
					finally
					{
						this._hkey = null;
					}
				}
				if (this.IsPerfDataKey())
				{
					this.ClosePerfDataKey();
				}
			}
		}

		public RegistryKey CreateSubKey(string subkey)
		{
			return this.CreateSubKey(subkey, this._checkMode);
		}

		public RegistryKey CreateSubKey(string subkey, bool writable)
		{
			return this.CreateSubKeyInternal(subkey, writable ? RegistryKeyPermissionCheck.ReadWriteSubTree : RegistryKeyPermissionCheck.ReadSubTree, null, RegistryOptions.None);
		}

		public RegistryKey CreateSubKey(string subkey, bool writable, RegistryOptions options)
		{
			return this.CreateSubKeyInternal(subkey, writable ? RegistryKeyPermissionCheck.ReadWriteSubTree : RegistryKeyPermissionCheck.ReadSubTree, null, options);
		}

		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck)
		{
			return this.CreateSubKeyInternal(subkey, permissionCheck, null, RegistryOptions.None);
		}

		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions)
		{
			return this.CreateSubKeyInternal(subkey, permissionCheck, null, registryOptions);
		}

		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions, RegistrySecurity registrySecurity)
		{
			return this.CreateSubKeyInternal(subkey, permissionCheck, registrySecurity, registryOptions);
		}

		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity)
		{
			return this.CreateSubKeyInternal(subkey, permissionCheck, registrySecurity, RegistryOptions.None);
		}

		private RegistryKey CreateSubKeyInternal(string subkey, RegistryKeyPermissionCheck permissionCheck, object registrySecurityObj, RegistryOptions registryOptions)
		{
			RegistryKey.ValidateKeyOptions(registryOptions);
			RegistryKey.ValidateKeyName(subkey);
			RegistryKey.ValidateKeyMode(permissionCheck);
			this.EnsureWriteable();
			subkey = RegistryKey.FixupName(subkey);
			if (!this._remoteKey)
			{
				RegistryKey registryKey = this.InternalOpenSubKeyWithoutSecurityChecks(subkey, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree);
				if (registryKey != null)
				{
					registryKey._checkMode = permissionCheck;
					return registryKey;
				}
			}
			return this.CreateSubKeyInternalCore(subkey, permissionCheck, registrySecurityObj, registryOptions);
		}

		public void DeleteSubKey(string subkey)
		{
			this.DeleteSubKey(subkey, true);
		}

		public void DeleteSubKey(string subkey, bool throwOnMissingSubKey)
		{
			RegistryKey.ValidateKeyName(subkey);
			this.EnsureWriteable();
			subkey = RegistryKey.FixupName(subkey);
			RegistryKey registryKey = this.InternalOpenSubKeyWithoutSecurityChecks(subkey, false);
			if (registryKey != null)
			{
				using (registryKey)
				{
					if (registryKey.InternalSubKeyCount() > 0)
					{
						ThrowHelper.ThrowInvalidOperationException("Registry key has subkeys and recursive removes are not supported by this method.");
					}
				}
				this.DeleteSubKeyCore(subkey, throwOnMissingSubKey);
				return;
			}
			if (throwOnMissingSubKey)
			{
				ThrowHelper.ThrowArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
			}
		}

		public void DeleteSubKeyTree(string subkey)
		{
			this.DeleteSubKeyTree(subkey, true);
		}

		public void DeleteSubKeyTree(string subkey, bool throwOnMissingSubKey)
		{
			RegistryKey.ValidateKeyName(subkey);
			if (subkey.Length == 0 && this.IsSystemKey())
			{
				ThrowHelper.ThrowArgumentException("Cannot delete a registry hive's subtree.");
			}
			this.EnsureWriteable();
			subkey = RegistryKey.FixupName(subkey);
			RegistryKey registryKey = this.InternalOpenSubKeyWithoutSecurityChecks(subkey, true);
			if (registryKey != null)
			{
				using (registryKey)
				{
					if (registryKey.InternalSubKeyCount() > 0)
					{
						string[] array = registryKey.InternalGetSubKeyNames();
						for (int i = 0; i < array.Length; i++)
						{
							registryKey.DeleteSubKeyTreeInternal(array[i]);
						}
					}
				}
				this.DeleteSubKeyTreeCore(subkey);
				return;
			}
			if (throwOnMissingSubKey)
			{
				ThrowHelper.ThrowArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
			}
		}

		private void DeleteSubKeyTreeInternal(string subkey)
		{
			RegistryKey registryKey = this.InternalOpenSubKeyWithoutSecurityChecks(subkey, true);
			if (registryKey != null)
			{
				using (registryKey)
				{
					if (registryKey.InternalSubKeyCount() > 0)
					{
						string[] array = registryKey.InternalGetSubKeyNames();
						for (int i = 0; i < array.Length; i++)
						{
							registryKey.DeleteSubKeyTreeInternal(array[i]);
						}
					}
				}
				this.DeleteSubKeyTreeCore(subkey);
				return;
			}
			ThrowHelper.ThrowArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
		}

		public void DeleteValue(string name)
		{
			this.DeleteValue(name, true);
		}

		public void DeleteValue(string name, bool throwOnMissingValue)
		{
			this.EnsureWriteable();
			this.DeleteValueCore(name, throwOnMissingValue);
		}

		public static RegistryKey OpenBaseKey(RegistryHive hKey, RegistryView view)
		{
			RegistryKey.ValidateKeyView(view);
			return RegistryKey.OpenBaseKeyCore(hKey, view);
		}

		public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName)
		{
			return RegistryKey.OpenRemoteBaseKey(hKey, machineName, RegistryView.Default);
		}

		public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName, RegistryView view)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			RegistryKey.ValidateKeyView(view);
			return RegistryKey.OpenRemoteBaseKeyCore(hKey, machineName, view);
		}

		public RegistryKey OpenSubKey(string name)
		{
			return this.OpenSubKey(name, false);
		}

		public RegistryKey OpenSubKey(string name, bool writable)
		{
			RegistryKey.ValidateKeyName(name);
			this.EnsureNotDisposed();
			name = RegistryKey.FixupName(name);
			return this.InternalOpenSubKeyCore(name, writable, true);
		}

		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck)
		{
			RegistryKey.ValidateKeyMode(permissionCheck);
			return this.InternalOpenSubKey(name, permissionCheck, RegistryKey.GetRegistryKeyAccess(permissionCheck));
		}

		public RegistryKey OpenSubKey(string name, RegistryRights rights)
		{
			return this.InternalOpenSubKey(name, this._checkMode, (int)rights);
		}

		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights)
		{
			return this.InternalOpenSubKey(name, permissionCheck, (int)rights);
		}

		private RegistryKey InternalOpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, int rights)
		{
			RegistryKey.ValidateKeyName(name);
			RegistryKey.ValidateKeyMode(permissionCheck);
			RegistryKey.ValidateKeyRights(rights);
			this.EnsureNotDisposed();
			name = RegistryKey.FixupName(name);
			return this.InternalOpenSubKeyCore(name, permissionCheck, rights, true);
		}

		internal RegistryKey InternalOpenSubKeyWithoutSecurityChecks(string name, bool writable)
		{
			RegistryKey.ValidateKeyName(name);
			this.EnsureNotDisposed();
			return this.InternalOpenSubKeyWithoutSecurityChecksCore(name, writable);
		}

		public RegistrySecurity GetAccessControl()
		{
			return this.GetAccessControl(AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		public RegistrySecurity GetAccessControl(AccessControlSections includeSections)
		{
			this.EnsureNotDisposed();
			return new RegistrySecurity(this.Handle, this.Name, includeSections);
		}

		public void SetAccessControl(RegistrySecurity registrySecurity)
		{
			this.EnsureWriteable();
			if (registrySecurity == null)
			{
				throw new ArgumentNullException("registrySecurity");
			}
			registrySecurity.Persist(this.Handle, this.Name);
		}

		public int SubKeyCount
		{
			get
			{
				return this.InternalSubKeyCount();
			}
		}

		public RegistryView View
		{
			get
			{
				this.EnsureNotDisposed();
				return this._regView;
			}
		}

		public SafeRegistryHandle Handle
		{
			get
			{
				this.EnsureNotDisposed();
				if (!this.IsSystemKey())
				{
					return this._hkey;
				}
				return this.SystemKeyHandle;
			}
		}

		public static RegistryKey FromHandle(SafeRegistryHandle handle)
		{
			return RegistryKey.FromHandle(handle, RegistryView.Default);
		}

		public static RegistryKey FromHandle(SafeRegistryHandle handle, RegistryView view)
		{
			if (handle == null)
			{
				throw new ArgumentNullException("handle");
			}
			RegistryKey.ValidateKeyView(view);
			return new RegistryKey(handle, true, view);
		}

		private int InternalSubKeyCount()
		{
			this.EnsureNotDisposed();
			return this.InternalSubKeyCountCore();
		}

		public string[] GetSubKeyNames()
		{
			return this.InternalGetSubKeyNames();
		}

		private string[] InternalGetSubKeyNames()
		{
			this.EnsureNotDisposed();
			int num = this.InternalSubKeyCount();
			if (num <= 0)
			{
				return Array.Empty<string>();
			}
			return this.InternalGetSubKeyNamesCore(num);
		}

		public int ValueCount
		{
			get
			{
				return this.InternalValueCount();
			}
		}

		private int InternalValueCount()
		{
			this.EnsureNotDisposed();
			return this.InternalValueCountCore();
		}

		public string[] GetValueNames()
		{
			this.EnsureNotDisposed();
			int num = this.InternalValueCount();
			if (num <= 0)
			{
				return Array.Empty<string>();
			}
			return this.GetValueNamesCore(num);
		}

		public object GetValue(string name)
		{
			return this.InternalGetValue(name, null, false, true);
		}

		public object GetValue(string name, object defaultValue)
		{
			return this.InternalGetValue(name, defaultValue, false, true);
		}

		public object GetValue(string name, object defaultValue, RegistryValueOptions options)
		{
			if (options < RegistryValueOptions.None || options > RegistryValueOptions.DoNotExpandEnvironmentNames)
			{
				throw new ArgumentException(SR.Format("Illegal enum value: {0}.", (int)options), "options");
			}
			bool flag = options == RegistryValueOptions.DoNotExpandEnvironmentNames;
			return this.InternalGetValue(name, defaultValue, flag, true);
		}

		private object InternalGetValue(string name, object defaultValue, bool doNotExpand, bool checkSecurity)
		{
			if (checkSecurity)
			{
				this.EnsureNotDisposed();
			}
			return this.InternalGetValueCore(name, defaultValue, doNotExpand);
		}

		public RegistryValueKind GetValueKind(string name)
		{
			this.EnsureNotDisposed();
			return this.GetValueKindCore(name);
		}

		public string Name
		{
			get
			{
				this.EnsureNotDisposed();
				return this._keyName;
			}
		}

		public void SetValue(string name, object value)
		{
			this.SetValue(name, value, RegistryValueKind.Unknown);
		}

		public void SetValue(string name, object value, RegistryValueKind valueKind)
		{
			if (value == null)
			{
				ThrowHelper.ThrowArgumentNullException("value");
			}
			if (name != null && name.Length > 16383)
			{
				throw new ArgumentException("Registry value names should not be greater than 16,383 characters.", "name");
			}
			if (!Enum.IsDefined(typeof(RegistryValueKind), valueKind))
			{
				throw new ArgumentException("The specified RegistryValueKind is an invalid value.", "valueKind");
			}
			this.EnsureWriteable();
			if (valueKind == RegistryValueKind.Unknown)
			{
				valueKind = this.CalculateValueKind(value);
			}
			this.SetValueCore(name, value, valueKind);
		}

		private RegistryValueKind CalculateValueKind(object value)
		{
			if (value is int)
			{
				return RegistryValueKind.DWord;
			}
			if (!(value is Array))
			{
				return RegistryValueKind.String;
			}
			if (value is byte[])
			{
				return RegistryValueKind.Binary;
			}
			if (value is string[])
			{
				return RegistryValueKind.MultiString;
			}
			throw new ArgumentException(SR.Format("RegistryKey.SetValue does not support arrays of type '{0}'. Only Byte[] and String[] are supported.", value.GetType().Name));
		}

		public override string ToString()
		{
			this.EnsureNotDisposed();
			return this._keyName;
		}

		private static string FixupName(string name)
		{
			if (name.IndexOf('\\') == -1)
			{
				return name;
			}
			StringBuilder stringBuilder = new StringBuilder(name);
			RegistryKey.FixupPath(stringBuilder);
			int num = stringBuilder.Length - 1;
			if (num >= 0 && stringBuilder[num] == '\\')
			{
				stringBuilder.Length = num;
			}
			return stringBuilder.ToString();
		}

		private static void FixupPath(StringBuilder path)
		{
			int length = path.Length;
			bool flag = false;
			char maxValue = char.MaxValue;
			for (int i = 1; i < length - 1; i++)
			{
				if (path[i] == '\\')
				{
					i++;
					while (i < length && path[i] == '\\')
					{
						path[i] = maxValue;
						i++;
						flag = true;
					}
				}
			}
			if (flag)
			{
				int i = 0;
				int num = 0;
				while (i < length)
				{
					if (path[i] == maxValue)
					{
						i++;
					}
					else
					{
						path[num] = path[i];
						i++;
						num++;
					}
				}
				path.Length += num - i;
			}
		}

		private void EnsureNotDisposed()
		{
			if (this._hkey == null)
			{
				ThrowHelper.ThrowObjectDisposedException(this._keyName, "Cannot access a closed registry key.");
			}
		}

		private void EnsureWriteable()
		{
			this.EnsureNotDisposed();
			if (!this.IsWritable())
			{
				ThrowHelper.ThrowUnauthorizedAccessException("Cannot write to the registry key.");
			}
		}

		private RegistryKeyPermissionCheck GetSubKeyPermissionCheck(bool subkeyWritable)
		{
			if (this._checkMode == RegistryKeyPermissionCheck.Default)
			{
				return this._checkMode;
			}
			if (subkeyWritable)
			{
				return RegistryKeyPermissionCheck.ReadWriteSubTree;
			}
			return RegistryKeyPermissionCheck.ReadSubTree;
		}

		private static void ValidateKeyName(string name)
		{
			if (name == null)
			{
				ThrowHelper.ThrowArgumentNullException("name");
			}
			int num = name.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
			int num2 = 0;
			while (num != -1)
			{
				if (num - num2 > 255)
				{
					ThrowHelper.ThrowArgumentException("Registry key names should not be greater than 255 characters.", "name");
				}
				num2 = num + 1;
				num = name.IndexOf("\\", num2, StringComparison.OrdinalIgnoreCase);
			}
			if (name.Length - num2 > 255)
			{
				ThrowHelper.ThrowArgumentException("Registry key names should not be greater than 255 characters.", "name");
			}
		}

		private static void ValidateKeyMode(RegistryKeyPermissionCheck mode)
		{
			if (mode < RegistryKeyPermissionCheck.Default || mode > RegistryKeyPermissionCheck.ReadWriteSubTree)
			{
				ThrowHelper.ThrowArgumentException("The specified RegistryKeyPermissionCheck value is invalid.", "mode");
			}
		}

		private static void ValidateKeyOptions(RegistryOptions options)
		{
			if (options < RegistryOptions.None || options > RegistryOptions.Volatile)
			{
				ThrowHelper.ThrowArgumentException("The specified RegistryOptions value is invalid.", "options");
			}
		}

		private static void ValidateKeyView(RegistryView view)
		{
			if (view != RegistryView.Default && view != RegistryView.Registry32 && view != RegistryView.Registry64)
			{
				ThrowHelper.ThrowArgumentException("The specified RegistryView value is invalid.", "view");
			}
		}

		private static void ValidateKeyRights(int rights)
		{
			if ((rights & -983104) != 0)
			{
				ThrowHelper.ThrowSecurityException("Requested registry access is not allowed.");
			}
		}

		private bool IsDirty()
		{
			return (this._state & RegistryKey.StateFlags.Dirty) > (RegistryKey.StateFlags)0;
		}

		private bool IsSystemKey()
		{
			return (this._state & RegistryKey.StateFlags.SystemKey) > (RegistryKey.StateFlags)0;
		}

		private bool IsWritable()
		{
			return (this._state & RegistryKey.StateFlags.WriteAccess) > (RegistryKey.StateFlags)0;
		}

		private bool IsPerfDataKey()
		{
			return (this._state & RegistryKey.StateFlags.PerfData) > (RegistryKey.StateFlags)0;
		}

		private void SetDirty()
		{
			this._state |= RegistryKey.StateFlags.Dirty;
		}

		internal RegistryKey()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(int.MinValue);

		internal static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);

		internal static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);

		internal static readonly IntPtr HKEY_USERS = new IntPtr(-2147483645);

		internal static readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(-2147483644);

		internal static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);

		internal static readonly IntPtr HKEY_DYN_DATA = new IntPtr(-2147483642);

		private static readonly string[] s_hkeyNames = new string[] { "HKEY_CLASSES_ROOT", "HKEY_CURRENT_USER", "HKEY_LOCAL_MACHINE", "HKEY_USERS", "HKEY_PERFORMANCE_DATA", "HKEY_CURRENT_CONFIG", "HKEY_DYN_DATA" };

		private const int MaxKeyLength = 255;

		private const int MaxValueLength = 16383;

		private volatile SafeRegistryHandle _hkey;

		private volatile string _keyName;

		private volatile bool _remoteKey;

		private volatile RegistryKey.StateFlags _state;

		private volatile RegistryKeyPermissionCheck _checkMode;

		private volatile RegistryView _regView;

		[Flags]
		private enum StateFlags
		{
			Dirty = 1,
			SystemKey = 2,
			WriteAccess = 4,
			PerfData = 8
		}
	}
}
