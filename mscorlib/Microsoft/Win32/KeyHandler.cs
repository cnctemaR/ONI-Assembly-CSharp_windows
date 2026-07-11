using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;

namespace Microsoft.Win32
{
	internal class KeyHandler
	{
		static KeyHandler()
		{
			KeyHandler.CleanVolatileKeys();
		}

		private KeyHandler(RegistryKey rkey, string basedir)
			: this(rkey, basedir, false)
		{
		}

		private KeyHandler(RegistryKey rkey, string basedir, bool is_volatile)
		{
			string volatileDir = KeyHandler.GetVolatileDir(basedir);
			string text = basedir;
			if (Directory.Exists(basedir))
			{
				is_volatile = false;
			}
			else if (Directory.Exists(volatileDir))
			{
				text = volatileDir;
				is_volatile = true;
			}
			else if (is_volatile)
			{
				text = volatileDir;
			}
			if (!Directory.Exists(text))
			{
				try
				{
					Directory.CreateDirectory(text);
				}
				catch (UnauthorizedAccessException ex)
				{
					throw new SecurityException("No access to the given key", ex);
				}
			}
			this.Dir = basedir;
			this.ActualDir = text;
			this.IsVolatile = is_volatile;
			this.file = Path.Combine(this.ActualDir, "values.xml");
			this.Load();
		}

		public void Load()
		{
			this.values = new Hashtable();
			if (!File.Exists(this.file))
			{
				return;
			}
			try
			{
				using (FileStream fileStream = File.OpenRead(this.file))
				{
					string text = new StreamReader(fileStream).ReadToEnd();
					if (text.Length != 0)
					{
						SecurityElement securityElement = SecurityElement.FromString(text);
						if (securityElement.Tag == "values" && securityElement.Children != null)
						{
							foreach (object obj in securityElement.Children)
							{
								SecurityElement securityElement2 = (SecurityElement)obj;
								if (securityElement2.Tag == "value")
								{
									this.LoadKey(securityElement2);
								}
							}
						}
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
				this.values.Clear();
				throw new SecurityException("No access to the given key");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("While loading registry key at {0}: {1}", this.file, ex);
				this.values.Clear();
			}
		}

		private void LoadKey(SecurityElement se)
		{
			Hashtable attributes = se.Attributes;
			try
			{
				string text = (string)attributes["name"];
				if (text != null)
				{
					string text2 = (string)attributes["type"];
					if (text2 != null)
					{
						if (!(text2 == "int"))
						{
							if (!(text2 == "bytearray"))
							{
								if (!(text2 == "string"))
								{
									if (!(text2 == "expand"))
									{
										if (!(text2 == "qword"))
										{
											if (text2 == "string-array")
											{
												List<string> list = new List<string>();
												if (se.Children != null)
												{
													foreach (object obj in se.Children)
													{
														SecurityElement securityElement = (SecurityElement)obj;
														list.Add(securityElement.Text);
													}
												}
												this.values[text] = list.ToArray();
											}
										}
										else
										{
											this.values[text] = long.Parse(se.Text);
										}
									}
									else
									{
										this.values[text] = new ExpandString(se.Text);
									}
								}
								else
								{
									this.values[text] = ((se.Text == null) ? string.Empty : se.Text);
								}
							}
							else
							{
								this.values[text] = Convert.FromBase64String(se.Text);
							}
						}
						else
						{
							this.values[text] = int.Parse(se.Text);
						}
					}
				}
			}
			catch
			{
			}
		}

		public RegistryKey Ensure(RegistryKey rkey, string extra, bool writable)
		{
			return this.Ensure(rkey, extra, writable, false);
		}

		public RegistryKey Ensure(RegistryKey rkey, string extra, bool writable, bool is_volatile)
		{
			Type typeFromHandle = typeof(KeyHandler);
			RegistryKey registryKey2;
			lock (typeFromHandle)
			{
				string text = Path.Combine(this.Dir, extra);
				KeyHandler keyHandler = (KeyHandler)KeyHandler.dir_to_handler[text];
				if (keyHandler == null)
				{
					keyHandler = new KeyHandler(rkey, text, is_volatile);
				}
				RegistryKey registryKey = new RegistryKey(keyHandler, KeyHandler.CombineName(rkey, extra), writable);
				KeyHandler.key_to_handler[registryKey] = keyHandler;
				KeyHandler.dir_to_handler[text] = keyHandler;
				registryKey2 = registryKey;
			}
			return registryKey2;
		}

		public RegistryKey Probe(RegistryKey rkey, string extra, bool writable)
		{
			RegistryKey registryKey = null;
			Type typeFromHandle = typeof(KeyHandler);
			RegistryKey registryKey2;
			lock (typeFromHandle)
			{
				string text = Path.Combine(this.Dir, extra);
				KeyHandler keyHandler = (KeyHandler)KeyHandler.dir_to_handler[text];
				if (keyHandler != null)
				{
					registryKey = new RegistryKey(keyHandler, KeyHandler.CombineName(rkey, extra), writable);
					KeyHandler.key_to_handler[registryKey] = keyHandler;
				}
				else if (Directory.Exists(text) || KeyHandler.VolatileKeyExists(text))
				{
					keyHandler = new KeyHandler(rkey, text);
					registryKey = new RegistryKey(keyHandler, KeyHandler.CombineName(rkey, extra), writable);
					KeyHandler.dir_to_handler[text] = keyHandler;
					KeyHandler.key_to_handler[registryKey] = keyHandler;
				}
				registryKey2 = registryKey;
			}
			return registryKey2;
		}

		private static string CombineName(RegistryKey rkey, string extra)
		{
			if (extra.IndexOf('/') != -1)
			{
				extra = extra.Replace('/', '\\');
			}
			return rkey.Name + "\\" + extra;
		}

		private static long GetSystemBootTime()
		{
			if (!File.Exists("/proc/stat"))
			{
				return -1L;
			}
			string text = null;
			try
			{
				using (StreamReader streamReader = new StreamReader("/proc/stat", Encoding.ASCII))
				{
					string text2;
					while ((text2 = streamReader.ReadLine()) != null)
					{
						if (text2.StartsWith("btime"))
						{
							text = text2;
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("While reading system info {0}", ex);
			}
			if (text == null)
			{
				return -1L;
			}
			int num = text.IndexOf(' ');
			long num2;
			if (!long.TryParse(text.Substring(num, text.Length - num), out num2))
			{
				return -1L;
			}
			return num2;
		}

		private static long GetRegisteredBootTime(string path)
		{
			if (!File.Exists(path))
			{
				return -1L;
			}
			string text = null;
			try
			{
				using (StreamReader streamReader = new StreamReader(path, Encoding.ASCII))
				{
					text = streamReader.ReadLine();
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("While reading registry data at {0}: {1}", path, ex);
			}
			if (text == null)
			{
				return -1L;
			}
			long num;
			if (!long.TryParse(text, out num))
			{
				return -1L;
			}
			return num;
		}

		private static void SaveRegisteredBootTime(string path, long btime)
		{
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.ASCII))
				{
					streamWriter.WriteLine(btime.ToString());
				}
			}
			catch (Exception)
			{
			}
		}

		private static void CleanVolatileKeys()
		{
			long systemBootTime = KeyHandler.GetSystemBootTime();
			foreach (string text in new string[]
			{
				KeyHandler.UserStore,
				KeyHandler.MachineStore
			})
			{
				if (Directory.Exists(text))
				{
					string text2 = Path.Combine(text, "last-btime");
					string text3 = Path.Combine(text, "volatile-keys");
					if (Directory.Exists(text3))
					{
						long registeredBootTime = KeyHandler.GetRegisteredBootTime(text2);
						if (systemBootTime < 0L || registeredBootTime < 0L || registeredBootTime != systemBootTime)
						{
							Directory.Delete(text3, true);
						}
					}
					KeyHandler.SaveRegisteredBootTime(text2, systemBootTime);
				}
			}
		}

		public static bool VolatileKeyExists(string dir)
		{
			Type typeFromHandle = typeof(KeyHandler);
			lock (typeFromHandle)
			{
				KeyHandler keyHandler = (KeyHandler)KeyHandler.dir_to_handler[dir];
				if (keyHandler != null)
				{
					return keyHandler.IsVolatile;
				}
			}
			return !Directory.Exists(dir) && Directory.Exists(KeyHandler.GetVolatileDir(dir));
		}

		public static string GetVolatileDir(string dir)
		{
			string rootFromDir = KeyHandler.GetRootFromDir(dir);
			return dir.Replace(rootFromDir, Path.Combine(rootFromDir, "volatile-keys"));
		}

		public static KeyHandler Lookup(RegistryKey rkey, bool createNonExisting)
		{
			Type typeFromHandle = typeof(KeyHandler);
			KeyHandler keyHandler2;
			lock (typeFromHandle)
			{
				KeyHandler keyHandler = (KeyHandler)KeyHandler.key_to_handler[rkey];
				if (keyHandler != null)
				{
					keyHandler2 = keyHandler;
				}
				else if (!rkey.IsRoot || !createNonExisting)
				{
					keyHandler2 = null;
				}
				else
				{
					RegistryHive hive = rkey.Hive;
					switch (hive)
					{
					case RegistryHive.ClassesRoot:
					case RegistryHive.LocalMachine:
					case RegistryHive.Users:
					case RegistryHive.PerformanceData:
					case RegistryHive.CurrentConfig:
					case RegistryHive.DynData:
					{
						string text = Path.Combine(KeyHandler.MachineStore, hive.ToString());
						keyHandler = new KeyHandler(rkey, text);
						KeyHandler.dir_to_handler[text] = keyHandler;
						break;
					}
					case RegistryHive.CurrentUser:
					{
						string text2 = Path.Combine(KeyHandler.UserStore, hive.ToString());
						keyHandler = new KeyHandler(rkey, text2);
						KeyHandler.dir_to_handler[text2] = keyHandler;
						break;
					}
					default:
						throw new Exception("Unknown RegistryHive");
					}
					KeyHandler.key_to_handler[rkey] = keyHandler;
					keyHandler2 = keyHandler;
				}
			}
			return keyHandler2;
		}

		private static string GetRootFromDir(string dir)
		{
			if (dir.IndexOf(KeyHandler.UserStore) > -1)
			{
				return KeyHandler.UserStore;
			}
			if (dir.IndexOf(KeyHandler.MachineStore) > -1)
			{
				return KeyHandler.MachineStore;
			}
			throw new Exception("Could not get root for dir " + dir);
		}

		public static void Drop(RegistryKey rkey)
		{
			Type typeFromHandle = typeof(KeyHandler);
			lock (typeFromHandle)
			{
				KeyHandler keyHandler = (KeyHandler)KeyHandler.key_to_handler[rkey];
				if (keyHandler != null)
				{
					KeyHandler.key_to_handler.Remove(rkey);
					int num = 0;
					foreach (object obj in KeyHandler.key_to_handler)
					{
						if (((DictionaryEntry)obj).Value == keyHandler)
						{
							num++;
						}
					}
					if (num == 0)
					{
						KeyHandler.dir_to_handler.Remove(keyHandler.Dir);
					}
				}
			}
		}

		public static void Drop(string dir)
		{
			Type typeFromHandle = typeof(KeyHandler);
			lock (typeFromHandle)
			{
				KeyHandler keyHandler = (KeyHandler)KeyHandler.dir_to_handler[dir];
				if (keyHandler != null)
				{
					KeyHandler.dir_to_handler.Remove(dir);
					ArrayList arrayList = new ArrayList();
					foreach (object obj in KeyHandler.key_to_handler)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						if (dictionaryEntry.Value == keyHandler)
						{
							arrayList.Add(dictionaryEntry.Key);
						}
					}
					foreach (object obj2 in arrayList)
					{
						KeyHandler.key_to_handler.Remove(obj2);
					}
				}
			}
		}

		public static bool Delete(string dir)
		{
			if (!Directory.Exists(dir))
			{
				string volatileDir = KeyHandler.GetVolatileDir(dir);
				if (!Directory.Exists(volatileDir))
				{
					return false;
				}
				dir = volatileDir;
			}
			Directory.Delete(dir, true);
			KeyHandler.Drop(dir);
			return true;
		}

		public RegistryValueKind GetValueKind(string name)
		{
			if (name == null)
			{
				return RegistryValueKind.Unknown;
			}
			Hashtable hashtable = this.values;
			object obj;
			lock (hashtable)
			{
				obj = this.values[name];
			}
			if (obj == null)
			{
				return RegistryValueKind.Unknown;
			}
			if (obj is int)
			{
				return RegistryValueKind.DWord;
			}
			if (obj is string[])
			{
				return RegistryValueKind.MultiString;
			}
			if (obj is long)
			{
				return RegistryValueKind.QWord;
			}
			if (obj is byte[])
			{
				return RegistryValueKind.Binary;
			}
			if (obj is string)
			{
				return RegistryValueKind.String;
			}
			if (obj is ExpandString)
			{
				return RegistryValueKind.ExpandString;
			}
			return RegistryValueKind.Unknown;
		}

		public object GetValue(string name, RegistryValueOptions options)
		{
			if (this.IsMarkedForDeletion)
			{
				return null;
			}
			if (name == null)
			{
				name = string.Empty;
			}
			Hashtable hashtable = this.values;
			object obj;
			lock (hashtable)
			{
				obj = this.values[name];
			}
			ExpandString expandString = obj as ExpandString;
			if (expandString == null)
			{
				return obj;
			}
			if ((options & RegistryValueOptions.DoNotExpandEnvironmentNames) == RegistryValueOptions.None)
			{
				return expandString.Expand();
			}
			return expandString.ToString();
		}

		public void SetValue(string name, object value)
		{
			this.AssertNotMarkedForDeletion();
			if (name == null)
			{
				name = string.Empty;
			}
			Hashtable hashtable = this.values;
			lock (hashtable)
			{
				if (value is int || value is string || value is byte[] || value is string[])
				{
					this.values[name] = value;
				}
				else
				{
					this.values[name] = value.ToString();
				}
			}
			this.SetDirty();
		}

		public string[] GetValueNames()
		{
			this.AssertNotMarkedForDeletion();
			Hashtable hashtable = this.values;
			string[] array2;
			lock (hashtable)
			{
				ICollection keys = this.values.Keys;
				string[] array = new string[keys.Count];
				keys.CopyTo(array, 0);
				array2 = array;
			}
			return array2;
		}

		public int GetSubKeyCount()
		{
			return this.GetSubKeyNames().Length;
		}

		public string[] GetSubKeyNames()
		{
			DirectoryInfo[] directories = new DirectoryInfo(this.ActualDir).GetDirectories();
			string[] array;
			if (this.IsVolatile || !Directory.Exists(KeyHandler.GetVolatileDir(this.Dir)))
			{
				array = new string[directories.Length];
				for (int i = 0; i < directories.Length; i++)
				{
					DirectoryInfo directoryInfo = directories[i];
					array[i] = directoryInfo.Name;
				}
				return array;
			}
			DirectoryInfo[] directories2 = new DirectoryInfo(KeyHandler.GetVolatileDir(this.Dir)).GetDirectories();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				dictionary[directoryInfo2.Name] = directoryInfo2.Name;
			}
			foreach (DirectoryInfo directoryInfo3 in directories2)
			{
				dictionary[directoryInfo3.Name] = directoryInfo3.Name;
			}
			array = new string[dictionary.Count];
			int num = 0;
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				array[num++] = keyValuePair.Value;
			}
			return array;
		}

		public void SetValue(string name, object value, RegistryValueKind valueKind)
		{
			this.SetDirty();
			if (name == null)
			{
				name = string.Empty;
			}
			Hashtable hashtable = this.values;
			lock (hashtable)
			{
				switch (valueKind)
				{
				case RegistryValueKind.String:
					if (value is string)
					{
						this.values[name] = value;
						return;
					}
					goto IL_0116;
				case RegistryValueKind.ExpandString:
					if (value is string)
					{
						this.values[name] = new ExpandString((string)value);
						return;
					}
					goto IL_0116;
				case RegistryValueKind.Binary:
					if (value is byte[])
					{
						this.values[name] = value;
						return;
					}
					goto IL_0116;
				case RegistryValueKind.DWord:
					try
					{
						this.values[name] = Convert.ToInt32(value);
						return;
					}
					catch (OverflowException)
					{
						goto IL_0122;
					}
					break;
				case (RegistryValueKind)5:
				case (RegistryValueKind)6:
				case (RegistryValueKind)8:
				case (RegistryValueKind)9:
				case (RegistryValueKind)10:
					goto IL_0106;
				case RegistryValueKind.MultiString:
					break;
				case RegistryValueKind.QWord:
					try
					{
						this.values[name] = Convert.ToInt64(value);
						return;
					}
					catch (OverflowException)
					{
						goto IL_0122;
					}
					goto IL_0106;
				default:
					goto IL_0106;
				}
				if (value is string[])
				{
					this.values[name] = value;
					return;
				}
				goto IL_0116;
				IL_0106:
				throw new ArgumentException("unknown value", "valueKind");
				IL_0116:;
			}
			IL_0122:
			throw new ArgumentException("Value could not be converted to specified type", "valueKind");
		}

		private void SetDirty()
		{
			Type typeFromHandle = typeof(KeyHandler);
			lock (typeFromHandle)
			{
				if (!this.dirty)
				{
					this.dirty = true;
					new Timer(new TimerCallback(this.DirtyTimeout), null, 3000, -1);
				}
			}
		}

		public void DirtyTimeout(object state)
		{
			this.Flush();
		}

		public void Flush()
		{
			Type typeFromHandle = typeof(KeyHandler);
			lock (typeFromHandle)
			{
				if (this.dirty)
				{
					this.Save();
					this.dirty = false;
				}
			}
		}

		public bool ValueExists(string name)
		{
			if (name == null)
			{
				name = string.Empty;
			}
			Hashtable hashtable = this.values;
			bool flag2;
			lock (hashtable)
			{
				flag2 = this.values.Contains(name);
			}
			return flag2;
		}

		public int ValueCount
		{
			get
			{
				Hashtable hashtable = this.values;
				int count;
				lock (hashtable)
				{
					count = this.values.Keys.Count;
				}
				return count;
			}
		}

		public bool IsMarkedForDeletion
		{
			get
			{
				return !KeyHandler.dir_to_handler.Contains(this.Dir);
			}
		}

		public void RemoveValue(string name)
		{
			this.AssertNotMarkedForDeletion();
			Hashtable hashtable = this.values;
			lock (hashtable)
			{
				this.values.Remove(name);
			}
			this.SetDirty();
		}

		~KeyHandler()
		{
			this.Flush();
		}

		private void Save()
		{
			if (this.IsMarkedForDeletion)
			{
				return;
			}
			SecurityElement securityElement = new SecurityElement("values");
			Hashtable hashtable = this.values;
			lock (hashtable)
			{
				if (!File.Exists(this.file) && this.values.Count == 0)
				{
					return;
				}
				foreach (object obj in this.values)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					object value = dictionaryEntry.Value;
					SecurityElement securityElement2 = new SecurityElement("value");
					securityElement2.AddAttribute("name", SecurityElement.Escape((string)dictionaryEntry.Key));
					if (value is string)
					{
						securityElement2.AddAttribute("type", "string");
						securityElement2.Text = SecurityElement.Escape((string)value);
					}
					else if (value is int)
					{
						securityElement2.AddAttribute("type", "int");
						securityElement2.Text = value.ToString();
					}
					else if (value is long)
					{
						securityElement2.AddAttribute("type", "qword");
						securityElement2.Text = value.ToString();
					}
					else if (value is byte[])
					{
						securityElement2.AddAttribute("type", "bytearray");
						securityElement2.Text = Convert.ToBase64String((byte[])value);
					}
					else if (value is ExpandString)
					{
						securityElement2.AddAttribute("type", "expand");
						securityElement2.Text = SecurityElement.Escape(value.ToString());
					}
					else if (value is string[])
					{
						securityElement2.AddAttribute("type", "string-array");
						foreach (string text in (string[])value)
						{
							securityElement2.AddChild(new SecurityElement("string")
							{
								Text = SecurityElement.Escape(text)
							});
						}
					}
					securityElement.AddChild(securityElement2);
				}
			}
			using (FileStream fileStream = File.Create(this.file))
			{
				StreamWriter streamWriter = new StreamWriter(fileStream);
				streamWriter.Write(securityElement.ToString());
				streamWriter.Flush();
			}
		}

		private void AssertNotMarkedForDeletion()
		{
			if (this.IsMarkedForDeletion)
			{
				throw RegistryKey.CreateMarkedForDeletionException();
			}
		}

		private static string UserStore
		{
			get
			{
				if (KeyHandler.user_store == null)
				{
					KeyHandler.user_store = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ".mono/registry");
				}
				return KeyHandler.user_store;
			}
		}

		private static string MachineStore
		{
			get
			{
				if (KeyHandler.machine_store == null)
				{
					KeyHandler.machine_store = Environment.GetEnvironmentVariable("MONO_REGISTRY_PATH");
					if (KeyHandler.machine_store == null)
					{
						string machineConfigPath = Environment.GetMachineConfigPath();
						int num = machineConfigPath.IndexOf("machine.config");
						KeyHandler.machine_store = Path.Combine(Path.Combine(machineConfigPath.Substring(0, num - 1), ".."), "registry");
					}
				}
				return KeyHandler.machine_store;
			}
		}

		private static Hashtable key_to_handler = new Hashtable(new RegistryKeyComparer());

		private static Hashtable dir_to_handler = new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer());

		private const string VolatileDirectoryName = "volatile-keys";

		public string Dir;

		private string ActualDir;

		public bool IsVolatile;

		private Hashtable values;

		private string file;

		private bool dirty;

		private static string user_store;

		private static string machine_store;
	}
}
