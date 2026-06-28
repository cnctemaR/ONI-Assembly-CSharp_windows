using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.Win32
{
	[ComVisible(true)]
	public sealed class RegistryKey : MarshalByRefObject, IDisposable
	{
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

		static RegistryKey()
		{
			if (Path.DirectorySeparatorChar == '\\')
			{
				RegistryKey.RegistryApi = new Win32RegistryApi();
			}
			else
			{
				RegistryKey.RegistryApi = new UnixRegistryApi();
			}
		}

		void IDisposable.Dispose()
		{
			GC.SuppressFinalize(this);
			this.Close();
		}

		~RegistryKey()
		{
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
			throw new NotImplementedException();
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

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck)
		{
			throw new NotImplementedException();
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity)
		{
			throw new NotImplementedException();
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
			this.AssertKeyStillValid();
			this.AssertKeyNameNotNull(subkey);
			this.AssertKeyNameLength(subkey);
			RegistryKey registryKey = this.OpenSubKey(subkey, true);
			if (registryKey == null)
			{
				throw new ArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
			}
			registryKey.DeleteChildKeysAndValues();
			registryKey.Close();
			this.DeleteSubKey(subkey, false);
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
			throw new NotImplementedException();
		}

		public RegistrySecurity GetAccessControl(AccessControlSections includeSections)
		{
			throw new NotImplementedException();
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
		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck)
		{
			throw new NotImplementedException();
		}

		[ComVisible(false)]
		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights)
		{
			throw new NotImplementedException();
		}

		public void SetAccessControl(RegistrySecurity registrySecurity)
		{
			throw new NotImplementedException();
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
				return (RegistryHive)((int)this.hive);
			}
		}

		internal object Handle
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
			string[] subKeyNames = this.GetSubKeyNames();
			foreach (string text in subKeyNames)
			{
				RegistryKey registryKey = this.OpenSubKey(text, true);
				registryKey.DeleteChildKeysAndValues();
				registryKey.Close();
				this.DeleteSubKey(text, false);
			}
			string[] valueNames = this.GetValueNames();
			foreach (string text2 in valueNames)
			{
				this.DeleteValue(text2, false);
			}
		}

		internal static string DecodeString(byte[] data)
		{
			string text = Encoding.Unicode.GetString(data);
			int num = text.IndexOf('\0');
			if (num != -1)
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
			switch (hive + -2147483648)
			{
			case (RegistryHive)0:
				return "HKEY_CLASSES_ROOT";
			case (RegistryHive)1:
				return "HKEY_CURRENT_USER";
			case (RegistryHive)2:
				return "HKEY_LOCAL_MACHINE";
			case (RegistryHive)3:
				return "HKEY_USERS";
			case (RegistryHive)4:
				return "HKEY_PERFORMANCE_DATA";
			case (RegistryHive)5:
				return "HKEY_CURRENT_CONFIG";
			case (RegistryHive)6:
				return "HKEY_DYN_DATA";
			default:
				throw new NotImplementedException(string.Format("Registry hive '{0}' is not implemented.", hive.ToString()));
			}
		}

		private object handle;

		private object hive;

		private readonly string qname;

		private readonly bool isRemoteRoot;

		private readonly bool isWritable;

		private static readonly IRegistryApi RegistryApi;
	}
}
