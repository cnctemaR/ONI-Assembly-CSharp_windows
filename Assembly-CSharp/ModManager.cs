using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using Steamworks;
using UnityEngine;

public class ModManager
{
	public ModManager()
	{
		string modRootDir = ModManager.GetModRootDir();
		this.db = new ModDB(ModManager.GetDBFilename(), modRootDir);
		this.Save();
	}

	public ICollection<ModInfo> ActiveMods
	{
		get
		{
			return this.activeMods;
		}
	}

	public ICollection<ModInfo> GetInstalledMods()
	{
		return this.db.GetMods();
	}

	private static string GetModRootDir()
	{
		string text = Util.RootFolder();
		return Path.Combine(text, "mods/");
	}

	private static string GetDBFilename()
	{
		return Path.Combine(ModManager.GetModRootDir(), "moddb.json");
	}

	public void Start()
	{
		string modRootDir = ModManager.GetModRootDir();
		this.db.LoadMods(this, modRootDir);
		this.Save();
	}

	public void Save()
	{
		this.db.Save(ModManager.GetDBFilename());
	}

	public void Shutdown()
	{
		foreach (ModInfo modInfo in this.activeMods)
		{
			this.Unmount(modInfo);
		}
		this.activeMods.Clear();
	}

	public bool IsModEnabled(ModInfo info)
	{
		return this.db.IsEnabled(info);
	}

	public void EnableMod(ModInfo info)
	{
		this.db.Enable(info);
		this.Save();
	}

	public void DisableMod(ModInfo info)
	{
		this.db.Disable(info);
		this.Save();
	}

	public bool IsInstalled(ModInfo info)
	{
		return this.db.IsInstalled(info);
	}

	public void UninstallMod(ModInfo info)
	{
		this.db.Uninstall(ModManager.GetModRootDir(), info);
	}

	public void ActivateWorldGenMod(ModInfo info)
	{
		this.DeactivateWorldGenMod();
		this.ActivateMod(info);
		this.worldGenMod = info;
	}

	public void Reorder(int a_idx, int b_idx)
	{
		this.db.Reorder(a_idx, b_idx);
		this.Save();
	}

	public void DeactivateWorldGenMod()
	{
		if (this.worldGenMod.type == ModInfo.ModType.WorldGen)
		{
			this.DeactiveMod(this.worldGenMod);
		}
	}

	public bool ActivateMod(ModInfo info)
	{
		foreach (ModInfo modInfo in this.activeMods)
		{
			if (modInfo.assetID == info.assetID && modInfo.source == info.source)
			{
				return true;
			}
		}
		bool flag = this.Mount(info);
		if (flag)
		{
			this.activeMods.Add(info);
		}
		return flag;
	}

	public void DeactiveMod(ModInfo info)
	{
		int num = this.activeMods.FindIndex((ModInfo active) => active.assetID == info.assetID && active.source == info.source);
		if (num > 0)
		{
			this.Unmount(this.activeMods[num]);
			this.activeMods.RemoveAt(num);
		}
	}

	private bool Mount(ModInfo info)
	{
		bool flag = false;
		ModInfo.ModType type = info.type;
		if (type != ModInfo.ModType.WorldGen)
		{
			if (type == ModInfo.ModType.Mod)
			{
				ModInfo.Source source = info.source;
				if (source == ModInfo.Source.Local || source == ModInfo.Source.Steam)
				{
					flag = this.MountDirectory(info);
				}
			}
		}
		else
		{
			ModInfo.Source source2 = info.source;
			if (source2 != ModInfo.Source.Local)
			{
				if (source2 == ModInfo.Source.Steam)
				{
					flag = this.MountSteamMod(info);
				}
			}
			else
			{
				flag = this.MountLocalMod(info);
			}
		}
		return flag;
	}

	private bool MountLocalMod(ModInfo info)
	{
		return this.MountZipFile(info.assetID, info.assetID, info.assetPath);
	}

	private bool MountDirectory(ModInfo info)
	{
		string modDir = this.db.GetModDir(ModManager.GetModRootDir(), info);
		string fullPath = Path.GetFullPath(Application.dataPath);
		PrefixFileSystem prefixFileSystem = new PrefixFileSystem(info.assetID, modDir, fullPath);
		Global.Instance.layeredFileSystem.AddFileSystem(prefixFileSystem);
		return true;
	}

	private bool UnmountDirectory(ModInfo info)
	{
		string modDir = this.db.GetModDir(ModManager.GetModRootDir(), info);
		string fullPath = Path.GetFullPath(Application.dataPath);
		PrefixFileSystem prefixFileSystem = new PrefixFileSystem(info.assetID, modDir, fullPath);
		Global.Instance.layeredFileSystem.AddFileSystem(prefixFileSystem);
		return true;
	}

	private bool MountZipFile(string filename, string mountpoint, string id)
	{
		bool flag = false;
		if (File.Exists(filename))
		{
			FileStream fileStream = File.OpenRead(filename);
			ZipFileSystem zipFileSystem = new ZipFileSystem(id, fileStream, mountpoint);
			Global.Instance.layeredFileSystem.AddFileSystem(zipFileSystem);
			flag = true;
		}
		return flag;
	}

	private bool MountSteamMod(ModInfo info)
	{
		ulong num = ulong.Parse(info.assetID);
		PublishedFileId_t publishedFileId_t = new PublishedFileId_t(num);
		ulong num2;
		string text;
		uint num3;
		SteamUGC.GetItemInstallInfo(publishedFileId_t, out num2, out text, 1024U, out num3);
		return this.MountZipFile(text, info.assetPath, info.assetID);
	}

	private void Unmount(ModInfo info)
	{
		foreach (IFileSystem fileSystem in Global.Instance.layeredFileSystem.GetFileSystems())
		{
			if (info.assetID == fileSystem.GetID())
			{
				Global.Instance.layeredFileSystem.RemoveFileSystem(fileSystem);
				break;
			}
		}
	}

	public void RegisterUGCEventHandlers(SteamUGCService event_src)
	{
		ModManager.UGCEventHandler ugceventHandler = new ModManager.UGCEventHandler(this);
		event_src.ugcEventHandlers.Add(ugceventHandler);
	}

	public void UpdateSteamCodeModSubscriptions()
	{
		if (SteamUGCService.Instance == null)
		{
			return;
		}
		List<SteamUGCService.Subscribed> subscribed = SteamUGCService.Instance.GetSubscribed("mod");
		using (IEnumerator<ModInfo> enumerator = this.db.GetMods().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ModInfo installed = enumerator.Current;
				if (installed.source == ModInfo.Source.Steam)
				{
					int num = subscribed.FindIndex((SteamUGCService.Subscribed i) => i.fileId.m_PublishedFileId.ToString() == installed.assetID);
					if (num < 0)
					{
						this.db.Uninstall(ModManager.GetModRootDir(), installed);
					}
				}
			}
		}
		foreach (SteamUGCService.Subscribed subscribed2 in subscribed)
		{
			ulong num2;
			string text;
			uint num3;
			SteamUGC.GetItemInstallInfo(subscribed2.fileId, out num2, out text, 1024U, out num3);
			string text2 = subscribed2.fileId.m_PublishedFileId.ToString();
			ModInfo modInfo = new ModInfo(ModInfo.Source.Steam, ModInfo.ModType.Mod, text2, subscribed2.description, text, subscribed2.lastUpdateTime);
			this.db.Install(ModManager.GetModRootDir(), modInfo);
		}
		this.Save();
	}

	private List<ModInfo> activeMods = new List<ModInfo>();

	private ModInfo worldGenMod;

	private ModDB db;

	private const int MAJOR_VERSION = 0;

	private const int MINOR_VERSION = 1;

	private class UGCEventHandler : SteamUGCService.IUGCEventHandler
	{
		public UGCEventHandler(ModManager mgr)
		{
			this.mgr = mgr;
		}

		public void OnUGCItemInstalled(ItemInstalled_t pCallback)
		{
		}

		public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
		{
		}

		public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
		{
		}

		public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
		{
		}

		public void OnUGCRefresh()
		{
			this.mgr.UpdateSteamCodeModSubscriptions();
		}

		private ModManager mgr;
	}
}
