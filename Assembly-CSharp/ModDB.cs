using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Harmony;
using Ionic.Zip;
using Klei;
using Newtonsoft.Json;
using STRINGS;
using UnityEngine;

internal class ModDB
{
	public ModDB(string filename, string mods_root)
	{
		this.Load(filename);
		this.Start(mods_root);
	}

	public ICollection<ModInfo> GetMods()
	{
		return this.mods;
	}

	public List<ModInfo> Mods { get; set; }

	private void Load(string moddb_filename)
	{
		try
		{
			if (File.Exists(moddb_filename))
			{
				string text = File.ReadAllText(moddb_filename);
				ModInfo[] array = JsonConvert.DeserializeObject<ModInfo[]>(text);
				this.mods.Clear();
				this.mods.AddRange(array);
			}
		}
		catch
		{
			this.mods = new List<ModInfo>();
			ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas);
			confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.MODS.DB_CORRUPT, moddb_filename), null, null, null, null, null, null, null, null);
			global::UnityEngine.Object.DontDestroyOnLoad(confirmDialogScreen.gameObject);
		}
	}

	public bool Save(string moddb_filename)
	{
		moddb_filename = Path.GetFullPath(moddb_filename);
		string directoryName = Path.GetDirectoryName(moddb_filename);
		if (!FileUtil.CreateDirectory(directoryName))
		{
			return false;
		}
		using (FileStream fileStream = FileUtil.Create(moddb_filename))
		{
			if (fileStream == null)
			{
				return false;
			}
			using (StreamWriter streamWriter = new StreamWriter(fileStream))
			{
				string text = JsonConvert.SerializeObject(this.mods, Formatting.Indented);
				streamWriter.Write(text);
			}
		}
		return true;
	}

	public void Start(string mods_root)
	{
		List<ModInfo> list = new List<ModInfo>();
		for (int i = 0; i < this.mods.Count; i++)
		{
			ModInfo modInfo = this.mods[i];
			bool flag = true;
			if (modInfo.markedForDelete)
			{
				string modDir = this.GetModDir(mods_root, modInfo);
				FileUtil.DeleteDirectory(modDir);
				if (!File.Exists(modDir))
				{
					flag = false;
				}
			}
			if (modInfo.markedForUpdate)
			{
				string modDir2 = this.GetModDir(mods_root, modInfo);
				if (this.InstallCodeMod(modInfo, modDir2))
				{
					modInfo.markedForUpdate = false;
					this.mods[i] = modInfo;
				}
			}
			if (flag)
			{
				list.Add(modInfo);
			}
		}
		this.mods = list;
	}

	public void LoadMods(ModManager mgr, string mods_root)
	{
		if (this.StartModLoader())
		{
			return;
		}
		Console.Out.WriteLine("Failed to use ModLoader. Using built in mod system instead.");
		List<ModDB.ModAssemblyInfo> list = new List<ModDB.ModAssemblyInfo>();
		HarmonyInstance harmonyInstance = HarmonyInstance.Create(string.Format("OxygenNotIncluded_v{0}.{1}", 0, 1));
		for (int i = 0; i < this.mods.Count; i++)
		{
			ModInfo modInfo = this.mods[i];
			if (modInfo.enabled)
			{
				string modDir = this.GetModDir(mods_root, modInfo);
				if (Directory.Exists(modDir))
				{
					string[] files = Directory.GetFiles(modDir, "*.dll");
					string[] array = files;
					int j = 0;
					while (j < array.Length)
					{
						string text = array[j];
						try
						{
							string fullPath = Path.GetFullPath(text);
							Output.Log(new object[] { string.Format("Loading MOD: {0}, {1}, {2}", modInfo.assetID, (modInfo.description != null) ? modInfo.description : "no desc", fullPath) });
							Assembly assembly = Assembly.LoadFrom(fullPath);
							if (assembly == null)
							{
								goto IL_01B7;
							}
							if (harmonyInstance != null)
							{
								harmonyInstance.PatchAll(assembly);
							}
							list.Add(new ModDB.ModAssemblyInfo
							{
								assembly = assembly,
								infoIdx = i,
								modDir = modDir
							});
						}
						catch (Exception ex)
						{
							modInfo.enabled = false;
							this.mods[i] = modInfo;
							ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas);
							confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.MODS.FAILED_TO_LOAD, modInfo.assetID, modInfo.description, ex.ToString()), null, null, null, null, null, null, null, null);
							global::UnityEngine.Object.DontDestroyOnLoad(confirmDialogScreen.gameObject);
						}
						break;
						IL_01B7:
						j++;
						continue;
						break;
					}
				}
				else
				{
					Output.Log(new object[] { "Disabling mod because its directory does not exist: " + modDir });
					modInfo.enabled = false;
					this.mods[i] = modInfo;
				}
			}
		}
		ModDB.MethodInfoQueryData[] array2 = new ModDB.MethodInfoQueryData[]
		{
			new ModDB.MethodInfoQueryData("OnLoad", new Type[0]),
			new ModDB.MethodInfoQueryData("OnLoad", new Type[] { typeof(string) })
		};
		foreach (ModDB.ModAssemblyInfo modAssemblyInfo in list)
		{
			foreach (Type type in modAssemblyInfo.assembly.GetTypes())
			{
				if (type != null)
				{
					try
					{
						foreach (ModDB.MethodInfoQueryData methodInfoQueryData in array2)
						{
							MethodInfo method = type.GetMethod(methodInfoQueryData.methodName, methodInfoQueryData.parameterTypes);
							if (method != null)
							{
								method.Invoke(null, new object[] { modAssemblyInfo.modDir });
								break;
							}
						}
					}
					catch (Exception ex2)
					{
						ModInfo modInfo2 = this.mods[modAssemblyInfo.infoIdx];
						modInfo2.enabled = false;
						this.mods[modAssemblyInfo.infoIdx] = modInfo2;
						ConfirmDialogScreen confirmDialogScreen2 = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas);
						confirmDialogScreen2.PopupConfirmDialog(string.Format(UI.FRONTEND.MODS.FAILED_TO_LOAD, modInfo2.assetID, modInfo2.description, ex2.ToString()), null, null, null, null, null, null, null, null);
						global::UnityEngine.Object.DontDestroyOnLoad(confirmDialogScreen2.gameObject);
					}
				}
			}
		}
	}

	private bool StartModLoader()
	{
		bool flag = false;
		string text = Path.Combine(Application.dataPath, "Managed/ModLoader.dll");
		if (File.Exists(text))
		{
			try
			{
				Assembly assembly = Assembly.LoadFile(text);
				if (assembly != null)
				{
					Type type = assembly.GetType("ModLoader.ModLoader");
					if (type != null)
					{
						MethodInfo method = type.GetMethod("Start");
						if (method != null)
						{
							method.Invoke(null, null);
							Console.Out.WriteLine("Started ModLoader.dll");
							flag = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.Out.WriteLine(ex.ToString());
			}
		}
		return flag;
	}

	public bool IsInstalled(ModInfo info)
	{
		return this.mods.Contains(info);
	}

	public void Install(string mods_root, ModInfo mod_info)
	{
		bool flag = false;
		string modDir = this.GetModDir(mods_root, mod_info);
		int num = this.mods.IndexOf(mod_info);
		if (num >= 0)
		{
			flag = true;
			ModInfo modInfo = this.mods[num];
			modInfo.markedForDelete = false;
			bool flag2 = true;
			if (Directory.Exists(modDir))
			{
				global::System.DateTime lastWriteTime = Directory.GetLastWriteTime(modDir);
				global::System.DateTime dateTime = ModDB.UnixTimeStampToDateTime(mod_info.lastModifiedTime);
				if (lastWriteTime > dateTime)
				{
					flag2 = false;
				}
			}
			modInfo.markedForUpdate = flag2;
			if (modInfo.markedForUpdate)
			{
				Output.Log(new object[] { string.Format("Mod marked for update: {0}", mod_info.assetID) });
			}
			this.mods[num] = modInfo;
		}
		if (!flag)
		{
			ModInfo.ModType type = mod_info.type;
			if (type == ModInfo.ModType.Mod)
			{
				if (this.InstallCodeMod(mod_info, modDir))
				{
					this.mods.Add(mod_info);
				}
			}
		}
	}

	public bool Uninstall(string mods_root, ModInfo mod_info)
	{
		int num = this.mods.IndexOf(mod_info);
		if (num < 0)
		{
			return false;
		}
		ModInfo modInfo = this.mods[num];
		modInfo.enabled = false;
		modInfo.markedForDelete = true;
		this.mods[num] = modInfo;
		return true;
	}

	public bool IsEnabled(ModInfo mod_info)
	{
		int num = this.mods.IndexOf(mod_info);
		return num >= 0 && this.mods[num].enabled;
	}

	public bool Enable(ModInfo mod_info)
	{
		int num = this.mods.IndexOf(mod_info);
		if (num < 0)
		{
			return false;
		}
		ModInfo modInfo = this.mods[num];
		modInfo.enabled = true;
		this.mods[num] = modInfo;
		return true;
	}

	public bool Disable(ModInfo mod_info)
	{
		int num = this.mods.IndexOf(mod_info);
		if (num < 0)
		{
			return false;
		}
		ModInfo modInfo = this.mods[num];
		modInfo.enabled = false;
		this.mods[num] = modInfo;
		return true;
	}

	public string GetModDir(string root, ModInfo info)
	{
		return Path.Combine(root, info.assetID);
	}

	public void Reorder(int a_idx, int b_idx)
	{
		if (a_idx >= 0 && b_idx >= 0)
		{
			ModInfo modInfo = this.mods[a_idx];
			this.mods[a_idx] = this.mods[b_idx];
			this.mods[b_idx] = modInfo;
		}
	}

	private bool InstallCodeMod(ModInfo info, string dest_path)
	{
		Output.Log(new object[] { string.Format("Installing Mod: {0}", info.assetID) });
		bool flag = false;
		ModInfo.Source source = info.source;
		if (source == ModInfo.Source.Steam)
		{
			if (Directory.Exists(dest_path))
			{
				Directory.Delete(dest_path, true);
			}
			if (FileUtil.CreateDirectory(dest_path))
			{
				string assetPath = info.assetPath;
				if (File.Exists(assetPath))
				{
					Output.Log(new object[] { string.Format("Extracting {0} to {1}", assetPath, dest_path) });
					using (ZipFile zipFile = ZipFile.Read(assetPath))
					{
						zipFile.ExtractAll(dest_path, ExtractExistingFileAction.OverwriteSilently);
						flag = true;
					}
				}
			}
		}
		return flag;
	}

	public static global::System.DateTime UnixTimeStampToDateTime(double unixTimeStamp)
	{
		global::System.DateTime dateTime = new global::System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
		return dateTime;
	}

	[JsonProperty]
	private List<ModInfo> mods = new List<ModInfo>();

	private const int MAJOR_VERSION = 0;

	private const int MINOR_VERSION = 1;

	private struct ModAssemblyInfo
	{
		public Assembly assembly;

		public int infoIdx;

		public string modDir;
	}

	private struct MethodInfoQueryData
	{
		public MethodInfoQueryData(string method_name, Type[] parameter_types)
		{
			this.methodName = method_name;
			this.parameterTypes = parameter_types;
		}

		public string methodName;

		public Type[] parameterTypes;
	}
}
