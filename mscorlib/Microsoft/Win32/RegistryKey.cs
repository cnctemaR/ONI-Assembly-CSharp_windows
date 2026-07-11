using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Win32
{
	[ComVisible(true)]
	public sealed class RegistryKey : MarshalByRefObject, IDisposable
	{
		static RegistryKey()
		{
			if (Path.DirectorySeparatorChar == '\\')
			{
				RegistryKey.RegistryApi = new Win32RegistryApi();
				return;
			}
			RegistryKey.RegistryApi = new UnixRegistryApi();
		}

		internal RegistryKey(RegistryHive hiveId)
			: this(hiveId, new IntPtr((int)hiveId), false)
		{
		}

		internal RegistryKey(RegistryHive hiveId, IntPtr keyHandle, bool remoteRoot)
		{
			this.hive = hiveId;
			this.handle = keyHandle;
			this.qname = RegistryKey.GetHiveName(hiveId);
			this.isRemoteRoot = remoteRoot;
			this.isWritable = true;
		}

		internal RegistryKey(object data, string keyName, bool writable)
		{
			this.handle = data;
			this.qname = keyName;
			this.isWritable = writable;
		}

		internal static bool IsEquals(RegistryKey a, RegistryKey b)
		{
			return a.hive == b.hive && a.handle == b.handle && a.qname == b.qname && a.isRemoteRoot == b.isRemoteRoot && a.isWritable == b.isWritable;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.Close();
		}

		public string Name
		{
			get
			{
				return this.qname;
			}
		}

		public void Flush()
		{
			RegistryKey.RegistryApi.Flush(this);
		}

		public void Close()
		{
			this.Flush();
			if (!this.isRemoteRoot && this.IsRoot)
			{
				return;
			}
			RegistryKey.RegistryApi.Close(this);
			this.handle = null;
			this.safe_handle = null;
		}

		public int SubKeyCount
		{
			get
			{
				this.AssertKeyStillValid();
				return RegistryKey.RegistryApi.SubKeyCount(this);
			}
		}

		public int ValueCount
		{
			get
			{
				this.AssertKeyStillValid();
				return RegistryKey.RegistryApi.ValueCount(this);
			}
		}

		[ComVisible(false)]
		[MonoTODO("Not implemented in Unix")]
		public SafeRegistryHandle Handle
		{
			get
			{
				this.AssertKeyStillValid();
				if (this.safe_handle == null)
				{
					IntPtr intPtr = RegistryKey.RegistryApi.GetHandle(this);
					this.safe_handle = new SafeRegistryHandle(intPtr, true);
				}
				return this.safe_handle;
			}
		}

		[ComVisible(false)]
		[MonoLimitation("View is ignored in Mono.")]
		public RegistryView View
		{
			get
			{
				return RegistryView.Default;
			}
		}

		public void SetValue(string name, object value)
		{
			this.AssertKeyStillValid();
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (name != null)
			{
				this.AssertKeyNameLength(name);
			}
			if (!this.IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			RegistryKey.RegistryApi.SetValue(this, name, value);
		}

		[ComVisible(false)]
		public void SetValue(string name, object value, RegistryValueKind valueKind)
		{
			this.AssertKeyStillValid();
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (name != null)
			{
				this.AssertKeyNameLength(name);
			}
			if (!this.IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			RegistryKey.RegistryApi.SetValue(this, name, value, valueKind);
		}

		public RegistryKey OpenSubKey(string name)
		{
			return this.OpenSubKey(name, false);
		}

		public RegistryKey OpenSubKey(string name, bool writable)
		{
			this.AssertKeyStillValid();
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.AssertKeyNameLength(name);
			return RegistryKey.RegistryApi.OpenSubKey(this, name, writable);
		}

		public object GetValue(string name)
		{
			return this.GetValue(name, null);
		}

		public object GetValue(string name, object defaultValue)
		{
			this.AssertKeyStillValid();
			return RegistryKey.RegistryApi.GetValue(this, name, defaultValue, RegistryValueOptions.None);
		}

		[ComVisible(false)]
		public object GetValue(string name, object defaultValue, RegistryValueOptions options)
		{
			this.AssertKeyStillValid();
			return RegistryKey.RegistryApi.GetValue(this, name, defaultValue, options);
		}

		[ComVisible(false)]
		public RegistryValueKind GetValueKind(string name)
		{
			return RegistryKey.RegistryApi.GetValueKind(this, name);
		}

		public RegistryKey CreateSubKey(string subkey)
		{
			this.AssertKeyStillValid();
			this.AssertKeyNameNotNull(subkey);
			this.AssertKeyNameLength(subkey);
			if (!this.IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			return RegistryKey.RegistryApi.CreateSubKey(this, subkey);
		}

		[MonoLimitation("permissionCheck is ignored in Mono")]
		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck)
		{
			return this.CreateSubKey(subkey);
		}

		[MonoLimitation("permissionCheck and registrySecurity are ignored in Mono")]
		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity)
		{
			return this.CreateSubKey(subkey);
		}

		[ComVisible(false)]
		[MonoLimitation("permissionCheck is ignored in Mono")]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions options)
		{
			this.AssertKeyStillValid();
			this.AssertKeyNameNotNull(subkey);
			this.AssertKeyNameLength(subkey);
			if (!this.IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			return RegistryKey.RegistryApi.CreateSubKey(this, subkey, options);
		}

		[ComVisible(false)]
		[MonoLimitation("permissionCheck and registrySecurity are ignored in Mono")]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions, RegistrySecurity registrySecurity)
		{
			return this.CreateSubKey(subkey, permissionCheck, registryOptions);
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, bool writable)
		{
			return this.CreateSubKey(subkey, writable ? RegistryKeyPermissionCheck.ReadWriteSubTree : RegistryKeyPermissionCheck.ReadSubTree);
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, bool writable, RegistryOptions options)
		{
			return this.CreateSubKey(subkey, writable ? RegistryKeyPermissionCheck.ReadWriteSubTree : RegistryKeyPermissionCheck.ReadSubTree, options);
		}

		public void DeleteSubKey(string subkey)
		{
			this.DeleteSubKey(subkey, true);
		}

		public void DeleteSubKey(string subkey, bool throwOnMissingSubKey)
		{
			this.AssertKeyStillValid();
			this.AssertKeyNameNotNull(subkey);
			this.AssertKeyNameLength(subkey);
			if (!this.IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			RegistryKey registryKey = this.OpenSubKey(subkey);
			if (registryKey == null)
			{
				if (throwOnMissingSubKey)
				{
					throw new ArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
				}
				return;
			}
			else
			{
				if (registryKey.SubKeyCount > 0)
				{
					throw new InvalidOperationException("Registry key has subkeys and recursive removes are not supported by this method.");
				}
				registryKey.Close();
				RegistryKey.RegistryApi.DeleteKey(this, subkey, throwOnMissingSubKey);
				return;
			}
		}

		public void DeleteSubKeyTree(string subkey)
		{
			this.DeleteSubKeyTree(subkey, true);
		}

		public void DeleteSubKeyTree(string subkey, bool throwOnMissingSubKey)
		{
			this.AssertKeyStillValid();
			this.AssertKeyNameNotNull(subkey);
			this.AssertKeyNameLength(subkey);
			RegistryKey registryKey = this.OpenSubKey(subkey, true);
			if (registryKey != null)
			{
				registryKey.DeleteChildKeysAndValues();
				registryKey.Close();
				this.DeleteSubKey(subkey, false);
				return;
			}
			if (!throwOnMissingSubKey)
			{
				return;
			}
			throw new ArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
		}

		public void DeleteValue(string name)
		{
			this.DeleteValue(name, true);
		}

		public void DeleteValue(string name, bool throwOnMissingValue)
		{
			this.AssertKeyStillValid();
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (!this.IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			RegistryKey.RegistryApi.DeleteValue(this, name, throwOnMissingValue);
		}

		public RegistrySecurity GetAccessControl()
		{
			return this.GetAccessControl(AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		public RegistrySecurity GetAccessControl(AccessControlSections includeSections)
		{
			return new RegistrySecurity(this.Name, includeSections);
		}

		public string[] GetSubKeyNames()
		{
			this.AssertKeyStillValid();
			return RegistryKey.RegistryApi.GetSubKeyNames(this);
		}

		public string[] GetValueNames()
		{
			this.AssertKeyStillValid();
			return RegistryKey.RegistryApi.GetValueNames(this);
		}

		[ComVisible(false)]
		[MonoTODO("Not implemented on unix")]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static RegistryKey FromHandle(SafeRegistryHandle handle)
		{
			if (handle == null)
			{
				throw new ArgumentNullException("handle");
			}
			return RegistryKey.RegistryApi.FromHandle(handle);
		}

		[ComVisible(false)]
		[MonoTODO("Not implemented on unix")]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static RegistryKey FromHandle(SafeRegistryHandle handle, RegistryView view)
		{
			return RegistryKey.FromHandle(handle);
		}

		[MonoTODO("Not implemented on unix")]
		public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			return RegistryKey.RegistryApi.OpenRemoteBaseKey(hKey, machineName);
		}

		[ComVisible(false)]
		[MonoTODO("Not implemented on unix")]
		public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName, RegistryView view)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			return RegistryKey.RegistryApi.OpenRemoteBaseKey(hKey, machineName);
		}

		[ComVisible(false)]
		[MonoLimitation("View is ignored in Mono")]
		public static RegistryKey OpenBaseKey(RegistryHive hKey, RegistryView view)
		{
			switch (hKey)
			{
			case RegistryHive.ClassesRoot:
				return Registry.ClassesRoot;
			case RegistryHive.CurrentUser:
				return Registry.CurrentUser;
			case RegistryHive.LocalMachine:
				return Registry.LocalMachine;
			case RegistryHive.Users:
				return Registry.Users;
			case RegistryHive.PerformanceData:
				return Registry.PerformanceData;
			case RegistryHive.CurrentConfig:
				return Registry.CurrentConfig;
			case RegistryHive.DynData:
				return Registry.DynData;
			default:
				throw new ArgumentException("hKey");
			}
		}

		[ComVisible(false)]
		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck)
		{
			return this.OpenSubKey(name, permissionCheck == RegistryKeyPermissionCheck.ReadWriteSubTree);
		}

		[ComVisible(false)]
		[MonoLimitation("rights are ignored in Mono")]
		public RegistryKey OpenSubKey(string name, RegistryRights rights)
		{
			return this.OpenSubKey(name);
		}

		[MonoLimitation("rights are ignored in Mono")]
		[ComVisible(false)]
		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights)
		{
			return this.OpenSubKey(name, permissionCheck == RegistryKeyPermissionCheck.ReadWriteSubTree);
		}

		public void SetAccessControl(RegistrySecurity registrySecurity)
		{
			if (registrySecurity == null)
			{
				throw new ArgumentNullException("registrySecurity");
			}
			registrySecurity.PersistModifications(this.Name);
		}

		public override string ToString()
		{
			this.AssertKeyStillValid();
			return RegistryKey.RegistryApi.ToString(this);
		}

		internal bool IsRoot
		{
			get
			{
				return this.hive != null;
			}
		}

		private bool IsWritable
		{
			get
			{
				return this.isWritable;
			}
		}

		internal RegistryHive Hive
		{
			get
			{
				if (!this.IsRoot)
				{
					throw new NotSupportedException();
				}
				return (RegistryHive)this.hive;
			}
		}

		internal object InternalHandle
		{
			get
			{
				return this.handle;
			}
		}

		private void AssertKeyStillValid()
		{
			if (this.handle == null)
			{
				throw new ObjectDisposedException("Microsoft.Win32.RegistryKey");
			}
		}

		private void AssertKeyNameNotNull(string subKeyName)
		{
			if (subKeyName == null)
			{
				throw new ArgumentNullException("name");
			}
		}

		private void AssertKeyNameLength(string name)
		{
			if (name.Length > 255)
			{
				throw new ArgumentException("Name of registry key cannot be greater than 255 characters");
			}
		}

		private void DeleteChildKeysAndValues()
		{
			if (this.IsRoot)
			{
				return;
			}
			foreach (string text in this.GetSubKeyNames())
			{
				RegistryKey registryKey = this.OpenSubKey(text, true);
				registryKey.DeleteChildKeysAndValues();
				registryKey.Close();
				this.DeleteSubKey(text, false);
			}
			foreach (string text2 in this.GetValueNames())
			{
				this.DeleteValue(text2, false);
			}
		}

		internal static string DecodeString(byte[] data)
		{
			string text = Encoding.Unicode.GetString(data);
			if (text.IndexOf('\0') != -1)
			{
				text = text.TrimEnd(new char[1]);
			}
			return text;
		}

		internal static IOException CreateMarkedForDeletionException()
		{
			throw new IOException("Illegal operation attempted on a registry key that has been marked for deletion.");
		}

		private static string GetHiveName(RegistryHive hive)
		{
			switch (hive)
			{
			case RegistryHive.ClassesRoot:
				return "HKEY_CLASSES_ROOT";
			case RegistryHive.CurrentUser:
				return "HKEY_CURRENT_USER";
			case RegistryHive.LocalMachine:
				return "HKEY_LOCAL_MACHINE";
			case RegistryHive.Users:
				return "HKEY_USERS";
			case RegistryHive.PerformanceData:
				return "HKEY_PERFORMANCE_DATA";
			case RegistryHive.CurrentConfig:
				return "HKEY_CURRENT_CONFIG";
			case RegistryHive.DynData:
				return "HKEY_DYN_DATA";
			default:
				throw new NotImplementedException(string.Format("Registry hive '{0}' is not implemented.", hive.ToString()));
			}
		}

		private object handle;

		private SafeRegistryHandle safe_handle;

		private object hive;

		private readonly string qname;

		private readonly bool isRemoteRoot;

		private readonly bool isWritable;

		private static readonly IRegistryApi RegistryApi;
	}
}
