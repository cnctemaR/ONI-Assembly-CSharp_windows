using System;
using System.Globalization;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Win32
{
	internal class UnixRegistryApi : IRegistryApi
	{
		private static string ToUnix(string keyname)
		{
			if (keyname.IndexOf('\\') != -1)
			{
				keyname = keyname.Replace('\\', '/');
			}
			return keyname.ToLower();
		}

		private static bool IsWellKnownKey(string parentKeyName, string keyname)
		{
			return (parentKeyName == Registry.CurrentUser.Name || parentKeyName == Registry.LocalMachine.Name) && string.Compare("software", keyname, true, CultureInfo.InvariantCulture) == 0;
		}

		public RegistryKey CreateSubKey(RegistryKey rkey, string keyname)
		{
			return this.CreateSubKey(rkey, keyname, true);
		}

		public RegistryKey CreateSubKey(RegistryKey rkey, string keyname, RegistryOptions options)
		{
			return this.CreateSubKey(rkey, keyname, true, options == RegistryOptions.Volatile);
		}

		public RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName)
		{
			throw new NotImplementedException();
		}

		public RegistryKey OpenSubKey(RegistryKey rkey, string keyname, bool writable)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				return null;
			}
			RegistryKey registryKey = keyHandler.Probe(rkey, UnixRegistryApi.ToUnix(keyname), writable);
			if (registryKey == null && UnixRegistryApi.IsWellKnownKey(rkey.Name, keyname))
			{
				registryKey = this.CreateSubKey(rkey, keyname, writable);
			}
			return registryKey;
		}

		public RegistryKey FromHandle(SafeRegistryHandle handle)
		{
			throw new NotImplementedException();
		}

		public void Flush(RegistryKey rkey)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, false);
			if (keyHandler == null)
			{
				return;
			}
			keyHandler.Flush();
		}

		public void Close(RegistryKey rkey)
		{
			KeyHandler.Drop(rkey);
		}

		public object GetValue(RegistryKey rkey, string name, object default_value, RegistryValueOptions options)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				return default_value;
			}
			if (keyHandler.ValueExists(name))
			{
				return keyHandler.GetValue(name, options);
			}
			return default_value;
		}

		public void SetValue(RegistryKey rkey, string name, object value)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				throw RegistryKey.CreateMarkedForDeletionException();
			}
			keyHandler.SetValue(name, value);
		}

		public void SetValue(RegistryKey rkey, string name, object value, RegistryValueKind valueKind)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				throw RegistryKey.CreateMarkedForDeletionException();
			}
			keyHandler.SetValue(name, value, valueKind);
		}

		public int SubKeyCount(RegistryKey rkey)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				throw RegistryKey.CreateMarkedForDeletionException();
			}
			return keyHandler.GetSubKeyCount();
		}

		public int ValueCount(RegistryKey rkey)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				throw RegistryKey.CreateMarkedForDeletionException();
			}
			return keyHandler.ValueCount;
		}

		public void DeleteValue(RegistryKey rkey, string name, bool throw_if_missing)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				return;
			}
			if (throw_if_missing && !keyHandler.ValueExists(name))
			{
				throw new ArgumentException("the given value does not exist");
			}
			keyHandler.RemoveValue(name);
		}

		public void DeleteKey(RegistryKey rkey, string keyname, bool throw_if_missing)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				if (!throw_if_missing)
				{
					return;
				}
				throw new ArgumentException("the given value does not exist");
			}
			else
			{
				if (!KeyHandler.Delete(Path.Combine(keyHandler.Dir, UnixRegistryApi.ToUnix(keyname))) && throw_if_missing)
				{
					throw new ArgumentException("the given value does not exist");
				}
				return;
			}
		}

		public string[] GetSubKeyNames(RegistryKey rkey)
		{
			return KeyHandler.Lookup(rkey, true).GetSubKeyNames();
		}

		public string[] GetValueNames(RegistryKey rkey)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				throw RegistryKey.CreateMarkedForDeletionException();
			}
			return keyHandler.GetValueNames();
		}

		public string ToString(RegistryKey rkey)
		{
			return rkey.Name;
		}

		private RegistryKey CreateSubKey(RegistryKey rkey, string keyname, bool writable)
		{
			return this.CreateSubKey(rkey, keyname, writable, false);
		}

		private RegistryKey CreateSubKey(RegistryKey rkey, string keyname, bool writable, bool is_volatile)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler == null)
			{
				throw RegistryKey.CreateMarkedForDeletionException();
			}
			if (KeyHandler.VolatileKeyExists(keyHandler.Dir) && !is_volatile)
			{
				throw new IOException("Cannot create a non volatile subkey under a volatile key.");
			}
			return keyHandler.Ensure(rkey, UnixRegistryApi.ToUnix(keyname), writable, is_volatile);
		}

		public RegistryValueKind GetValueKind(RegistryKey rkey, string name)
		{
			KeyHandler keyHandler = KeyHandler.Lookup(rkey, true);
			if (keyHandler != null)
			{
				return keyHandler.GetValueKind(name);
			}
			return RegistryValueKind.Unknown;
		}

		public IntPtr GetHandle(RegistryKey key)
		{
			throw new NotImplementedException();
		}
	}
}
