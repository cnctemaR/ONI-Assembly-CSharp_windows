using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zlib;
using Klei;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using KSerialization;
using Newtonsoft.Json;
using ProcGen;
using ProcGenGame;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SaveLoader")]
public class SaveLoader : KMonoBehaviour
{
	public bool loadedFromSave { get; private set; }

	public static void DestroyInstance()
	{
		SaveLoader.Instance = null;
	}

	public static SaveLoader Instance { get; private set; }

	public Action<Cluster> OnWorldGenComplete { get; set; }

	public Cluster ClusterLayout
	{
		get
		{
			return this.m_clusterLayout;
		}
	}

	public SaveGame.GameInfo GameInfo { get; private set; }

	protected override void OnPrefabInit()
	{
		SaveLoader.Instance = this;
		this.saveManager = base.GetComponent<SaveManager>();
	}

	private void MoveCorruptFile(string filename)
	{
	}

	protected override void OnSpawn()
	{
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		if (WorldGen.CanLoad(activeSaveFilePath))
		{
			Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			SimMessages.CreateDiseaseTable(Db.Get().Diseases);
			this.loadedFromSave = true;
			this.loadedFromSave = this.Load(activeSaveFilePath);
			this.saveFileCorrupt = !this.loadedFromSave;
			if (!this.loadedFromSave)
			{
				SaveLoader.SetActiveSaveFilePath(null);
				if (this.mustRestartOnFail)
				{
					this.MoveCorruptFile(activeSaveFilePath);
					Sim.Shutdown();
					App.LoadScene("frontend");
					return;
				}
			}
		}
		if (!this.loadedFromSave)
		{
			Sim.Shutdown();
			if (!string.IsNullOrEmpty(activeSaveFilePath))
			{
				DebugUtil.LogArgs(new object[] { "Couldn't load [" + activeSaveFilePath + "]" });
			}
			if (this.saveFileCorrupt)
			{
				this.MoveCorruptFile(activeSaveFilePath);
			}
			int num = 0;
			bool flag = WorldGen.CanLoad(WorldGen.GetSIMSaveFilename(num));
			if (!flag || !this.LoadFromWorldGen())
			{
				DebugUtil.LogWarningArgs(new object[] { "Couldn't start new game with current world gen, moving file" });
				if (flag)
				{
					KMonoBehaviour.isLoadingScene = true;
					while (FileSystem.FileExists(WorldGen.GetSIMSaveFilename(num)))
					{
						this.MoveCorruptFile(WorldGen.GetSIMSaveFilename(num));
						num++;
					}
				}
				App.LoadScene("frontend");
			}
		}
	}

	private static void CompressContents(BinaryWriter fileWriter, byte[] uncompressed, int length)
	{
		using (ZlibStream zlibStream = new ZlibStream(fileWriter.BaseStream, CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed))
		{
			zlibStream.Write(uncompressed, 0, length);
			zlibStream.Flush();
		}
	}

	private byte[] FloatToBytes(float[] floats)
	{
		byte[] array = new byte[floats.Length * 4];
		Buffer.BlockCopy(floats, 0, array, 0, array.Length);
		return array;
	}

	private static byte[] DecompressContents(byte[] compressed)
	{
		return ZlibStream.UncompressBuffer(compressed);
	}

	private float[] BytesToFloat(byte[] bytes)
	{
		float[] array = new float[bytes.Length / 4];
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		return array;
	}

	private SaveFileRoot PrepSaveFile()
	{
		SaveFileRoot saveFileRoot = new SaveFileRoot();
		saveFileRoot.WidthInCells = Grid.WidthInCells;
		saveFileRoot.HeightInCells = Grid.HeightInCells;
		saveFileRoot.streamed["GridVisible"] = Grid.Visible;
		saveFileRoot.streamed["GridSpawnable"] = Grid.Spawnable;
		saveFileRoot.streamed["GridDamage"] = this.FloatToBytes(Grid.Damage);
		Global.Instance.modManager.SendMetricsEvent();
		saveFileRoot.active_mods = new List<Label>();
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if (mod.IsEnabledForActiveDlc())
			{
				saveFileRoot.active_mods.Add(mod.label);
			}
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				Camera.main.transform.parent.GetComponent<CameraController>().Save(binaryWriter);
			}
			saveFileRoot.streamed["Camera"] = memoryStream.ToArray();
		}
		return saveFileRoot;
	}

	private void Save(BinaryWriter writer)
	{
		writer.WriteKleiString("world");
		Serializer.Serialize(this.PrepSaveFile(), writer);
		Game.SaveSettings(writer);
		Sim.Save(writer, 0, 0);
		this.saveManager.Save(writer);
		Game.Instance.Save(writer);
	}

	private bool Load(IReader reader)
	{
		global::Debug.Assert(reader.ReadKleiString() == "world");
		Deserializer deserializer = new Deserializer(reader);
		SaveFileRoot saveFileRoot = new SaveFileRoot();
		deserializer.Deserialize(saveFileRoot);
		if ((this.GameInfo.saveMajorVersion == 7 || this.GameInfo.saveMinorVersion < 8) && saveFileRoot.requiredMods != null)
		{
			saveFileRoot.active_mods = new List<Label>();
			foreach (ModInfo modInfo in saveFileRoot.requiredMods)
			{
				saveFileRoot.active_mods.Add(new Label
				{
					id = modInfo.assetID,
					version = (long)modInfo.lastModifiedTime,
					distribution_platform = Label.DistributionPlatform.Steam,
					title = modInfo.description
				});
			}
			saveFileRoot.requiredMods.Clear();
		}
		global::KMod.Manager modManager = Global.Instance.modManager;
		modManager.Load(Content.LayerableFiles);
		if (!modManager.MatchFootprint(saveFileRoot.active_mods, Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation))
		{
			DebugUtil.LogWarningArgs(new object[] { "Mod footprint of save file doesn't match current mod configuration" });
		}
		string text = string.Format("Mod Footprint ({0}):", saveFileRoot.active_mods.Count);
		foreach (Label label in saveFileRoot.active_mods)
		{
			text = text + "\n  - " + label.title;
		}
		global::Debug.Log(text);
		this.LogActiveMods();
		Global.Instance.modManager.SendMetricsEvent();
		WorldGen.LoadSettings(false);
		CustomGameSettings.Instance.LoadClusters();
		if (this.GameInfo.clusterId == null)
		{
			SaveGame.GameInfo gameInfo = this.GameInfo;
			if (!string.IsNullOrEmpty(saveFileRoot.clusterID))
			{
				gameInfo.clusterId = saveFileRoot.clusterID;
			}
			else
			{
				try
				{
					gameInfo.clusterId = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout).id;
				}
				catch
				{
					gameInfo.clusterId = WorldGenSettings.ClusterDefaultName;
					CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.ClusterLayout, gameInfo.clusterId);
				}
			}
			this.GameInfo = gameInfo;
		}
		Game.clusterId = this.GameInfo.clusterId;
		Game.LoadSettings(deserializer);
		GridSettings.Reset(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells);
		if (Application.isPlaying)
		{
			Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
		}
		Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		Sim.AllocateCells(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells, false);
		SimMessages.CreateDiseaseTable(Db.Get().Diseases);
		Sim.HandleMessage(SimMessageHashes.ClearUnoccupiedCells, 0, null);
		IReader reader2;
		if (saveFileRoot.streamed.ContainsKey("Sim"))
		{
			reader2 = new FastReader(saveFileRoot.streamed["Sim"]);
		}
		else
		{
			reader2 = reader;
		}
		if (Sim.LoadWorld(reader2) != 0)
		{
			DebugUtil.LogWarningArgs(new object[] { "\n--- Error loading save ---\nSimDLL found bad data\n" });
			Sim.Shutdown();
			return false;
		}
		Sim.Start();
		SceneInitializer.Instance.PostLoadPrefabs();
		this.mustRestartOnFail = true;
		if (!this.saveManager.Load(reader))
		{
			Sim.Shutdown();
			DebugUtil.LogWarningArgs(new object[] { "\n--- Error loading save ---\n" });
			SaveLoader.SetActiveSaveFilePath(null);
			return false;
		}
		Grid.Visible = saveFileRoot.streamed["GridVisible"];
		if (saveFileRoot.streamed.ContainsKey("GridSpawnable"))
		{
			Grid.Spawnable = saveFileRoot.streamed["GridSpawnable"];
		}
		Grid.Damage = this.BytesToFloat(saveFileRoot.streamed["GridDamage"]);
		Game.Instance.Load(deserializer);
		CameraSaveData.Load(new FastReader(saveFileRoot.streamed["Camera"]));
		ClusterManager.Instance.InitializeWorldGrid();
		SimMessages.DefineWorldOffsets(ClusterManager.Instance.WorldContainers.Select<WorldContainer, SimMessages.WorldOffsetData>((WorldContainer container) => new SimMessages.WorldOffsetData
		{
			worldOffsetX = container.WorldOffset.x,
			worldOffsetY = container.WorldOffset.y,
			worldSizeX = container.WorldSize.x,
			worldSizeY = container.WorldSize.y
		}).ToList<SimMessages.WorldOffsetData>());
		return true;
	}

	private void LogActiveMods()
	{
		string text = string.Format("Active Mods ({0}):", Global.Instance.modManager.mods.Count);
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if (mod.IsEnabledForActiveDlc())
			{
				text = text + "\n  - " + mod.title;
			}
		}
		global::Debug.Log(text);
	}

	public static string GetSavePrefix()
	{
		return global::System.IO.Path.Combine(global::Util.RootFolder(), string.Format("{0}{1}", "save_files", global::System.IO.Path.DirectorySeparatorChar));
	}

	public static string GetCloudSavePrefix()
	{
		string text = global::System.IO.Path.Combine(global::Util.RootFolder(), string.Format("{0}{1}", "cloud_save_files", global::System.IO.Path.DirectorySeparatorChar));
		string userID = SaveLoader.GetUserID();
		if (string.IsNullOrEmpty(userID))
		{
			return null;
		}
		text = global::System.IO.Path.Combine(text, userID);
		if (!global::System.IO.Directory.Exists(text))
		{
			global::System.IO.Directory.CreateDirectory(text);
		}
		return text;
	}

	public static string GetSavePrefixAndCreateFolder()
	{
		string savePrefix = SaveLoader.GetSavePrefix();
		if (!global::System.IO.Directory.Exists(savePrefix))
		{
			global::System.IO.Directory.CreateDirectory(savePrefix);
		}
		return savePrefix;
	}

	public static string GetUserID()
	{
		DistributionPlatform.User localUser = DistributionPlatform.Inst.LocalUser;
		if (localUser == null)
		{
			return null;
		}
		return localUser.Id.ToString();
	}

	public static string GetNextUsableSavePath(string filename)
	{
		int num = 0;
		string text = global::System.IO.Path.ChangeExtension(filename, null);
		while (File.Exists(filename))
		{
			filename = SaveScreen.GetValidSaveFilename(string.Format("{0} ({1})", text, num));
			num++;
		}
		return filename;
	}

	public static string GetOriginalSaveFileName(string filename)
	{
		if (!filename.Contains("/") && !filename.Contains("\\"))
		{
			return filename;
		}
		filename.Replace('\\', '/');
		return global::System.IO.Path.GetFileName(filename);
	}

	public static bool IsSaveAuto(string filename)
	{
		filename = filename.Replace('\\', '/');
		return filename.Contains("/auto_save/");
	}

	public static bool IsSaveLocal(string filename)
	{
		filename = filename.Replace('\\', '/');
		return filename.Contains("/save_files/");
	}

	public static bool IsSaveCloud(string filename)
	{
		filename = filename.Replace('\\', '/');
		return filename.Contains("/cloud_save_files/");
	}

	public static string GetAutoSavePrefix()
	{
		string text = global::System.IO.Path.Combine(SaveLoader.GetSavePrefixAndCreateFolder(), string.Format("{0}{1}", "auto_save", global::System.IO.Path.DirectorySeparatorChar));
		if (!global::System.IO.Directory.Exists(text))
		{
			global::System.IO.Directory.CreateDirectory(text);
		}
		return text;
	}

	public static void SetActiveSaveFilePath(string path)
	{
		KPlayerPrefs.SetString("SaveFilenameKey/", path);
	}

	public static string GetActiveSaveFilePath()
	{
		return KPlayerPrefs.GetString("SaveFilenameKey/");
	}

	public static string GetActiveAutoSavePath()
	{
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		if (activeSaveFilePath == null)
		{
			return SaveLoader.GetAutoSavePrefix();
		}
		return global::System.IO.Path.Combine(global::System.IO.Path.GetDirectoryName(activeSaveFilePath), "auto_save");
	}

	public static string GetAutosaveFilePath()
	{
		return SaveLoader.GetAutoSavePrefix() + "AutoSave Cycle 1.sav";
	}

	public static string GetActiveSaveColonyFolder()
	{
		string text = SaveLoader.GetActiveSaveFolder();
		if (text == null)
		{
			text = global::System.IO.Path.Combine(SaveLoader.GetSavePrefix(), SaveLoader.Instance.GameInfo.baseName);
		}
		return text;
	}

	public static string GetActiveSaveFolder()
	{
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		if (!string.IsNullOrEmpty(activeSaveFilePath))
		{
			return global::System.IO.Path.GetDirectoryName(activeSaveFilePath);
		}
		return null;
	}

	public static List<SaveLoader.SaveFileEntry> GetSaveFiles(string save_dir, bool sort, SearchOption search = SearchOption.AllDirectories)
	{
		List<SaveLoader.SaveFileEntry> list = new List<SaveLoader.SaveFileEntry>();
		if (string.IsNullOrEmpty(save_dir))
		{
			return list;
		}
		try
		{
			if (!global::System.IO.Directory.Exists(save_dir))
			{
				global::System.IO.Directory.CreateDirectory(save_dir);
			}
			foreach (string text in global::System.IO.Directory.GetFiles(save_dir, "*.sav", search))
			{
				try
				{
					global::System.DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(text);
					SaveLoader.SaveFileEntry saveFileEntry = new SaveLoader.SaveFileEntry
					{
						path = text,
						timeStamp = lastWriteTimeUtc
					};
					list.Add(saveFileEntry);
				}
				catch (Exception ex)
				{
					global::Debug.LogWarning("Problem reading file: " + text + "\n" + ex.ToString());
				}
			}
			if (sort)
			{
				list.Sort((SaveLoader.SaveFileEntry x, SaveLoader.SaveFileEntry y) => y.timeStamp.CompareTo(x.timeStamp));
			}
		}
		catch (Exception ex2)
		{
			string text2 = null;
			if (ex2 is UnauthorizedAccessException)
			{
				text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, save_dir);
			}
			else if (ex2 is IOException)
			{
				text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, save_dir);
			}
			if (text2 == null)
			{
				throw ex2;
			}
			GameObject gameObject = ((FrontEndManager.Instance == null) ? GameScreenManager.Instance.ssOverlayCanvas : FrontEndManager.Instance.gameObject);
			global::Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(text2, null, null, null, null, null, null, null, null);
		}
		return list;
	}

	public static List<SaveLoader.SaveFileEntry> GetAllFiles(bool sort, SaveLoader.SaveType type = SaveLoader.SaveType.both)
	{
		switch (type)
		{
		case SaveLoader.SaveType.local:
			return SaveLoader.GetSaveFiles(SaveLoader.GetSavePrefixAndCreateFolder(), sort, SearchOption.AllDirectories);
		case SaveLoader.SaveType.cloud:
			return SaveLoader.GetSaveFiles(SaveLoader.GetCloudSavePrefix(), sort, SearchOption.AllDirectories);
		case SaveLoader.SaveType.both:
		{
			List<SaveLoader.SaveFileEntry> saveFiles = SaveLoader.GetSaveFiles(SaveLoader.GetSavePrefixAndCreateFolder(), false, SearchOption.AllDirectories);
			List<SaveLoader.SaveFileEntry> saveFiles2 = SaveLoader.GetSaveFiles(SaveLoader.GetCloudSavePrefix(), false, SearchOption.AllDirectories);
			saveFiles.AddRange(saveFiles2);
			if (sort)
			{
				saveFiles.Sort((SaveLoader.SaveFileEntry x, SaveLoader.SaveFileEntry y) => y.timeStamp.CompareTo(x.timeStamp));
			}
			return saveFiles;
		}
		default:
			return new List<SaveLoader.SaveFileEntry>();
		}
	}

	public static List<SaveLoader.SaveFileEntry> GetAllColonyFiles(bool sort, SearchOption search = SearchOption.TopDirectoryOnly)
	{
		return SaveLoader.GetSaveFiles(SaveLoader.GetActiveSaveColonyFolder(), sort, search);
	}

	public static bool GetCloudSavesDefault()
	{
		return !(SaveLoader.GetCloudSavesDefaultPref() == "Disabled");
	}

	public static string GetCloudSavesDefaultPref()
	{
		string text = KPlayerPrefs.GetString("SavesDefaultToCloud", "Enabled");
		if (text != "Enabled" && text != "Disabled")
		{
			text = "Enabled";
		}
		return text;
	}

	public static void SetCloudSavesDefault(bool value)
	{
		SaveLoader.SetCloudSavesDefaultPref(value ? "Enabled" : "Disabled");
	}

	public static void SetCloudSavesDefaultPref(string pref)
	{
		if (pref != "Enabled" && pref != "Disabled")
		{
			global::Debug.LogWarning("Ignoring cloud saves default pref `" + pref + "` as it's not valid, expected `Enabled` or `Disabled`");
			return;
		}
		KPlayerPrefs.SetString("SavesDefaultToCloud", pref);
	}

	public static bool GetCloudSavesAvailable()
	{
		return !string.IsNullOrEmpty(SaveLoader.GetUserID()) && SaveLoader.GetCloudSavePrefix() != null;
	}

	public static string GetLatestSaveFile()
	{
		List<SaveLoader.SaveFileEntry> allFiles = SaveLoader.GetAllFiles(true, SaveLoader.SaveType.both);
		if (allFiles.Count == 0)
		{
			return null;
		}
		return allFiles[0].path;
	}

	public static string GetLatestSaveForCurrentDLC()
	{
		List<SaveLoader.SaveFileEntry> allFiles = SaveLoader.GetAllFiles(true, SaveLoader.SaveType.both);
		for (int i = 0; i < allFiles.Count; i++)
		{
			global::Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(allFiles[i].path);
			if (fileInfo != null)
			{
				SaveGame.Header first = fileInfo.first;
				SaveGame.GameInfo second = fileInfo.second;
				if (second.saveMajorVersion >= 7 && DlcManager.GetHighestActiveDlcId() == second.dlcId)
				{
					return allFiles[i].path;
				}
			}
		}
		return null;
	}

	public void InitialSave()
	{
		string text = SaveLoader.GetActiveSaveFilePath();
		if (string.IsNullOrEmpty(text))
		{
			text = SaveLoader.GetAutosaveFilePath();
		}
		else if (!text.Contains(".sav"))
		{
			text += ".sav";
		}
		this.LogActiveMods();
		this.Save(text, false, true);
	}

	public string Save(string filename, bool isAutoSave = false, bool updateSavePointer = true)
	{
		global::KSerialization.Manager.Clear();
		string directoryName = global::System.IO.Path.GetDirectoryName(filename);
		try
		{
			if (directoryName != null && !global::System.IO.Directory.Exists(directoryName))
			{
				global::System.IO.Directory.CreateDirectory(directoryName);
			}
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning("Problem creating save folder for " + filename + "!\n" + ex.ToString());
		}
		this.ReportSaveMetrics(isAutoSave);
		RetireColonyUtility.SaveColonySummaryData();
		if (isAutoSave && !GenericGameSettings.instance.keepAllAutosaves)
		{
			List<SaveLoader.SaveFileEntry> saveFiles = SaveLoader.GetSaveFiles(SaveLoader.GetActiveAutoSavePath(), true, SearchOption.AllDirectories);
			List<string> list = new List<string>();
			foreach (SaveLoader.SaveFileEntry saveFileEntry in saveFiles)
			{
				global::Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(saveFileEntry.path);
				if (fileInfo != null && SaveGame.GetSaveUniqueID(fileInfo.second) == SaveLoader.Instance.GameInfo.colonyGuid.ToString())
				{
					list.Add(saveFileEntry.path);
				}
			}
			for (int i = list.Count - 1; i >= 9; i--)
			{
				string text = list[i];
				try
				{
					global::Debug.Log("Deleting old autosave: " + text);
					File.Delete(text);
				}
				catch (Exception ex2)
				{
					global::Debug.LogWarning("Problem deleting autosave: " + text + "\n" + ex2.ToString());
				}
				string text2 = global::System.IO.Path.ChangeExtension(text, ".png");
				try
				{
					if (File.Exists(text2))
					{
						File.Delete(text2);
					}
				}
				catch (Exception ex3)
				{
					global::Debug.LogWarning("Problem deleting autosave screenshot: " + text2 + "\n" + ex3.ToString());
				}
			}
		}
		using (MemoryStream memoryStream = new MemoryStream((int)((float)this.lastUncompressedSize * 1.1f)))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				this.Save(binaryWriter);
				this.lastUncompressedSize = (int)memoryStream.Length;
				try
				{
					using (BinaryWriter binaryWriter2 = new BinaryWriter(File.Open(filename, FileMode.Create)))
					{
						SaveGame.Header header;
						byte[] saveHeader = SaveGame.Instance.GetSaveHeader(isAutoSave, this.compressSaveData, out header);
						binaryWriter2.Write(header.buildVersion);
						binaryWriter2.Write(header.headerSize);
						binaryWriter2.Write(header.headerVersion);
						binaryWriter2.Write(header.compression);
						binaryWriter2.Write(saveHeader);
						global::KSerialization.Manager.SerializeDirectory(binaryWriter2);
						if (this.compressSaveData)
						{
							SaveLoader.CompressContents(binaryWriter2, memoryStream.GetBuffer(), (int)memoryStream.Length);
						}
						else
						{
							binaryWriter2.Write(memoryStream.ToArray());
						}
						KCrashReporter.MOST_RECENT_SAVEFILE = filename;
						Stats.Print();
					}
				}
				catch (Exception ex4)
				{
					if (ex4 is UnauthorizedAccessException)
					{
						DebugUtil.LogArgs(new object[] { "UnauthorizedAccessException for " + filename });
						((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay)).PopupConfirmDialog(string.Format(UI.CRASHSCREEN.SAVEFAILED, "Unauthorized Access Exception"), null, null, null, null, null, null, null, null);
						return SaveLoader.GetActiveSaveFilePath();
					}
					if (ex4 is IOException)
					{
						DebugUtil.LogArgs(new object[] { "IOException (probably out of disk space) for " + filename });
						((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay)).PopupConfirmDialog(string.Format(UI.CRASHSCREEN.SAVEFAILED, "IOException. You may not have enough free space!"), null, null, null, null, null, null, null, null);
						return SaveLoader.GetActiveSaveFilePath();
					}
					throw ex4;
				}
			}
		}
		if (updateSavePointer)
		{
			SaveLoader.SetActiveSaveFilePath(filename);
		}
		Game.Instance.timelapser.SaveColonyPreview(filename);
		DebugUtil.LogArgs(new object[]
		{
			"Saved to",
			"[" + filename + "]"
		});
		GC.Collect();
		return filename;
	}

	public static SaveGame.GameInfo LoadHeader(string filename, out SaveGame.Header header)
	{
		byte[] array = new byte[512];
		SaveGame.GameInfo header2;
		using (FileStream fileStream = File.OpenRead(filename))
		{
			fileStream.Read(array, 0, 512);
			header2 = SaveGame.GetHeader(new FastReader(array), out header, filename);
		}
		return header2;
	}

	public bool Load(string filename)
	{
		SaveLoader.SetActiveSaveFilePath(filename);
		try
		{
			global::KSerialization.Manager.Clear();
			byte[] array = File.ReadAllBytes(filename);
			IReader reader = new FastReader(array);
			SaveGame.Header header;
			this.GameInfo = SaveGame.GetHeader(reader, out header, filename);
			DebugUtil.LogArgs(new object[] { string.Format("Loading save file: {4}\n headerVersion:{0}, buildVersion:{1}, headerSize:{2}, IsCompressed:{3}", new object[] { header.headerVersion, header.buildVersion, header.headerSize, header.IsCompressed, filename }) });
			DebugUtil.LogArgs(new object[] { string.Format("GameInfo loaded from save header:\n  numberOfCycles:{0},\n  numberOfDuplicants:{1},\n  baseName:{2},\n  isAutoSave:{3},\n  originalSaveName:{4},\n  clusterId:{5},\n  worldTraits:{6},\n  colonyGuid:{7},\n  saveVersion:{8}.{9}", new object[]
			{
				this.GameInfo.numberOfCycles,
				this.GameInfo.numberOfDuplicants,
				this.GameInfo.baseName,
				this.GameInfo.isAutoSave,
				this.GameInfo.originalSaveName,
				this.GameInfo.clusterId,
				(this.GameInfo.worldTraits != null && this.GameInfo.worldTraits.Length != 0) ? string.Join(", ", this.GameInfo.worldTraits) : "<i>none</i>",
				this.GameInfo.colonyGuid,
				this.GameInfo.saveMajorVersion,
				this.GameInfo.saveMinorVersion
			}) });
			string originalSaveName = this.GameInfo.originalSaveName;
			if (originalSaveName.Contains("/") || originalSaveName.Contains("\\"))
			{
				string originalSaveFileName = SaveLoader.GetOriginalSaveFileName(originalSaveName);
				SaveGame.GameInfo gameInfo = this.GameInfo;
				gameInfo.originalSaveName = originalSaveFileName;
				this.GameInfo = gameInfo;
				global::Debug.Log(string.Concat(new string[]
				{
					"Migration / Save originalSaveName updated from: `",
					originalSaveName,
					"` => `",
					this.GameInfo.originalSaveName,
					"`"
				}));
			}
			if (this.GameInfo.saveMajorVersion == 7 && this.GameInfo.saveMinorVersion < 4)
			{
				Helper.SetTypeInfoMask((SerializationTypeInfo)191);
			}
			global::KSerialization.Manager.DeserializeDirectory(reader);
			if (header.IsCompressed)
			{
				int num = array.Length - reader.Position;
				byte[] array2 = new byte[num];
				Array.Copy(array, reader.Position, array2, 0, num);
				byte[] array3 = SaveLoader.DecompressContents(array2);
				this.lastUncompressedSize = array3.Length;
				IReader reader2 = new FastReader(array3);
				this.Load(reader2);
			}
			else
			{
				this.lastUncompressedSize = array.Length;
				this.Load(reader);
			}
			KCrashReporter.MOST_RECENT_SAVEFILE = filename;
			if (this.GameInfo.isAutoSave && !string.IsNullOrEmpty(this.GameInfo.originalSaveName))
			{
				string originalSaveFileName2 = SaveLoader.GetOriginalSaveFileName(this.GameInfo.originalSaveName);
				string text;
				if (SaveLoader.IsSaveCloud(filename))
				{
					string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
					if (cloudSavePrefix != null)
					{
						text = global::System.IO.Path.Combine(cloudSavePrefix, this.GameInfo.baseName, originalSaveFileName2);
					}
					else
					{
						text = global::System.IO.Path.Combine(global::System.IO.Path.GetDirectoryName(filename).Replace("auto_save", ""), this.GameInfo.baseName, originalSaveFileName2);
					}
				}
				else
				{
					text = global::System.IO.Path.Combine(SaveLoader.GetSavePrefix(), this.GameInfo.baseName, originalSaveFileName2);
				}
				if (text != null)
				{
					SaveLoader.SetActiveSaveFilePath(text);
				}
			}
		}
		catch (Exception ex)
		{
			DebugUtil.LogWarningArgs(new object[] { "\n--- Error loading save ---\n" + ex.Message + "\n" + ex.StackTrace });
			Sim.Shutdown();
			SaveLoader.SetActiveSaveFilePath(null);
			return false;
		}
		Stats.Print();
		DebugUtil.LogArgs(new object[]
		{
			"Loaded",
			"[" + filename + "]"
		});
		DebugUtil.LogArgs(new object[]
		{
			"World Seeds",
			string.Concat(new string[]
			{
				"[",
				this.clusterDetailSave.globalWorldSeed.ToString(),
				"/",
				this.clusterDetailSave.globalWorldLayoutSeed.ToString(),
				"/",
				this.clusterDetailSave.globalTerrainSeed.ToString(),
				"/",
				this.clusterDetailSave.globalNoiseSeed.ToString(),
				"]"
			})
		});
		GC.Collect();
		return true;
	}

	public bool LoadFromWorldGen()
	{
		DebugUtil.LogArgs(new object[] { "Attempting to start a new game with current world gen" });
		WorldGen.LoadSettings(false);
		this.m_clusterLayout = Cluster.Load();
		ListPool<SimSaveFileStructure, SaveLoader>.PooledList pooledList = ListPool<SimSaveFileStructure, SaveLoader>.Allocate();
		this.m_clusterLayout.LoadClusterLayoutSim(pooledList);
		SaveGame.GameInfo gameInfo = this.GameInfo;
		gameInfo.clusterId = this.m_clusterLayout.Id;
		gameInfo.colonyGuid = Guid.NewGuid();
		this.GameInfo = gameInfo;
		if (pooledList.Count != this.m_clusterLayout.worlds.Count)
		{
			global::Debug.LogError("Attempt failed. Failed to load all worlds.");
			pooledList.Recycle();
			return false;
		}
		GridSettings.Reset(this.m_clusterLayout.size.x, this.m_clusterLayout.size.y);
		if (Application.isPlaying)
		{
			Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
		}
		this.clusterDetailSave = new WorldDetailSave();
		foreach (SimSaveFileStructure simSaveFileStructure in pooledList)
		{
			this.clusterDetailSave.globalNoiseSeed = simSaveFileStructure.worldDetail.globalNoiseSeed;
			this.clusterDetailSave.globalTerrainSeed = simSaveFileStructure.worldDetail.globalTerrainSeed;
			this.clusterDetailSave.globalWorldLayoutSeed = simSaveFileStructure.worldDetail.globalWorldLayoutSeed;
			this.clusterDetailSave.globalWorldSeed = simSaveFileStructure.worldDetail.globalWorldSeed;
			Vector2 vector = Grid.CellToPos2D(Grid.PosToCell(new Vector2I(simSaveFileStructure.x, simSaveFileStructure.y)));
			foreach (WorldDetailSave.OverworldCell overworldCell in simSaveFileStructure.worldDetail.overworldCells)
			{
				for (int num = 0; num != overworldCell.poly.Vertices.Count; num++)
				{
					List<Vector2> vertices = overworldCell.poly.Vertices;
					int num2 = num;
					vertices[num2] += vector;
				}
				overworldCell.poly.RefreshBounds();
			}
			this.clusterDetailSave.overworldCells.AddRange(simSaveFileStructure.worldDetail.overworldCells);
		}
		Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		Sim.AllocateCells(this.m_clusterLayout.size.x, this.m_clusterLayout.size.y, false);
		SimMessages.DefineWorldOffsets(this.m_clusterLayout.worlds.Select<WorldGen, SimMessages.WorldOffsetData>((WorldGen world) => new SimMessages.WorldOffsetData
		{
			worldOffsetX = world.WorldOffset.x,
			worldOffsetY = world.WorldOffset.y,
			worldSizeX = world.WorldSize.x,
			worldSizeY = world.WorldSize.y
		}).ToList<SimMessages.WorldOffsetData>());
		SimMessages.CreateDiseaseTable(Db.Get().Diseases);
		Sim.HandleMessage(SimMessageHashes.ClearUnoccupiedCells, 0, null);
		try
		{
			foreach (SimSaveFileStructure simSaveFileStructure2 in pooledList)
			{
				FastReader fastReader = new FastReader(simSaveFileStructure2.Sim);
				if (Sim.Load(fastReader) != 0)
				{
					DebugUtil.LogWarningArgs(new object[] { "\n--- Error loading save ---\nSimDLL found bad data\n" });
					Sim.Shutdown();
					pooledList.Recycle();
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning("--- Error loading Sim FROM NEW WORLDGEN ---" + ex.Message + "\n" + ex.StackTrace);
			Sim.Shutdown();
			pooledList.Recycle();
			return false;
		}
		global::Debug.Log("Attempt success");
		Sim.Start();
		SceneInitializer.Instance.PostLoadPrefabs();
		SceneInitializer.Instance.NewSaveGamePrefab();
		this.cachedGSD = this.m_clusterLayout.currentWorld.SpawnData;
		this.OnWorldGenComplete.Signal(this.m_clusterLayout);
		OniMetrics.LogEvent(OniMetrics.Event.NewSave, "NewGame", true);
		StoryManager.Instance.InitialSaveSetup();
		ThreadedHttps<KleiMetrics>.Instance.IncrementGameCount();
		OniMetrics.SendEvent(OniMetrics.Event.NewSave, "New Save");
		pooledList.Recycle();
		return true;
	}

	public GameSpawnData cachedGSD { get; private set; }

	public WorldDetailSave clusterDetailSave { get; private set; }

	public void SetWorldDetail(WorldDetailSave worldDetail)
	{
		this.clusterDetailSave = worldDetail;
	}

	private void ReportSaveMetrics(bool is_auto_save)
	{
		if (ThreadedHttps<KleiMetrics>.Instance == null || !ThreadedHttps<KleiMetrics>.Instance.enabled || this.saveManager == null)
		{
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary[GameClock.NewCycleKey] = GameClock.Instance.GetCycle() + 1;
		dictionary["IsAutoSave"] = is_auto_save;
		dictionary["SavedPrefabs"] = this.GetSavedPrefabMetrics();
		dictionary["ResourcesAccessible"] = this.GetWorldInventoryMetrics();
		dictionary["MinionMetrics"] = this.GetMinionMetrics();
		if (is_auto_save)
		{
			dictionary["DailyReport"] = this.GetDailyReportMetrics();
			dictionary["PerformanceMeasurements"] = this.GetPerformanceMeasurements();
			dictionary["AverageFrameTime"] = this.GetFrameTime();
		}
		dictionary["CustomGameSettings"] = CustomGameSettings.Instance.GetSettingsForMetrics();
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(dictionary, "ReportSaveMetrics");
	}

	private List<SaveLoader.MinionMetricsData> GetMinionMetrics()
	{
		List<SaveLoader.MinionMetricsData> list = new List<SaveLoader.MinionMetricsData>();
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			if (!(minionIdentity == null))
			{
				Amounts amounts = minionIdentity.gameObject.GetComponent<Modifiers>().amounts;
				List<SaveLoader.MinionAttrFloatData> list2 = new List<SaveLoader.MinionAttrFloatData>(amounts.Count);
				foreach (AmountInstance amountInstance in amounts)
				{
					float value = amountInstance.value;
					if (!float.IsNaN(value) && !float.IsInfinity(value))
					{
						list2.Add(new SaveLoader.MinionAttrFloatData
						{
							Name = amountInstance.modifier.Id,
							Value = amountInstance.value
						});
					}
				}
				MinionResume component = minionIdentity.gameObject.GetComponent<MinionResume>();
				float totalExperienceGained = component.TotalExperienceGained;
				List<string> list3 = new List<string>();
				foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
				{
					if (keyValuePair.Value)
					{
						list3.Add(keyValuePair.Key);
					}
				}
				list.Add(new SaveLoader.MinionMetricsData
				{
					Name = minionIdentity.name,
					Modifiers = list2,
					TotalExperienceGained = totalExperienceGained,
					Skills = list3
				});
			}
		}
		return list;
	}

	private List<SaveLoader.SavedPrefabMetricsData> GetSavedPrefabMetrics()
	{
		Dictionary<Tag, List<SaveLoadRoot>> lists = this.saveManager.GetLists();
		List<SaveLoader.SavedPrefabMetricsData> list = new List<SaveLoader.SavedPrefabMetricsData>(lists.Count);
		foreach (KeyValuePair<Tag, List<SaveLoadRoot>> keyValuePair in lists)
		{
			Tag key = keyValuePair.Key;
			List<SaveLoadRoot> value = keyValuePair.Value;
			if (value.Count > 0)
			{
				list.Add(new SaveLoader.SavedPrefabMetricsData
				{
					PrefabName = key.ToString(),
					Count = value.Count
				});
			}
		}
		return list;
	}

	private List<SaveLoader.WorldInventoryMetricsData> GetWorldInventoryMetrics()
	{
		Dictionary<Tag, float> allWorldsAccessibleAmounts = ClusterManager.Instance.GetAllWorldsAccessibleAmounts();
		List<SaveLoader.WorldInventoryMetricsData> list = new List<SaveLoader.WorldInventoryMetricsData>(allWorldsAccessibleAmounts.Count);
		foreach (KeyValuePair<Tag, float> keyValuePair in allWorldsAccessibleAmounts)
		{
			float value = keyValuePair.Value;
			if (!float.IsInfinity(value) && !float.IsNaN(value))
			{
				list.Add(new SaveLoader.WorldInventoryMetricsData
				{
					Name = keyValuePair.Key.ToString(),
					Amount = value
				});
			}
		}
		return list;
	}

	private List<SaveLoader.DailyReportMetricsData> GetDailyReportMetrics()
	{
		List<SaveLoader.DailyReportMetricsData> list = new List<SaveLoader.DailyReportMetricsData>();
		int cycle = GameClock.Instance.GetCycle();
		ReportManager.DailyReport dailyReport = ReportManager.Instance.FindReport(cycle);
		if (dailyReport != null)
		{
			foreach (ReportManager.ReportEntry reportEntry in dailyReport.reportEntries)
			{
				SaveLoader.DailyReportMetricsData dailyReportMetricsData = default(SaveLoader.DailyReportMetricsData);
				dailyReportMetricsData.Name = reportEntry.reportType.ToString();
				if (!float.IsInfinity(reportEntry.Net) && !float.IsNaN(reportEntry.Net))
				{
					dailyReportMetricsData.Net = new float?(reportEntry.Net);
				}
				if (SaveLoader.force_infinity)
				{
					dailyReportMetricsData.Net = null;
				}
				if (!float.IsInfinity(reportEntry.Positive) && !float.IsNaN(reportEntry.Positive))
				{
					dailyReportMetricsData.Positive = new float?(reportEntry.Positive);
				}
				if (!float.IsInfinity(reportEntry.Negative) && !float.IsNaN(reportEntry.Negative))
				{
					dailyReportMetricsData.Negative = new float?(reportEntry.Negative);
				}
				list.Add(dailyReportMetricsData);
			}
			list.Add(new SaveLoader.DailyReportMetricsData
			{
				Name = "MinionCount",
				Net = new float?((float)Components.LiveMinionIdentities.Count),
				Positive = new float?(0f),
				Negative = new float?(0f)
			});
		}
		return list;
	}

	private List<SaveLoader.PerformanceMeasurement> GetPerformanceMeasurements()
	{
		List<SaveLoader.PerformanceMeasurement> list = new List<SaveLoader.PerformanceMeasurement>();
		if (Global.Instance != null)
		{
			PerformanceMonitor component = Global.Instance.GetComponent<PerformanceMonitor>();
			list.Add(new SaveLoader.PerformanceMeasurement
			{
				name = "FramesAbove30",
				value = component.NumFramesAbove30
			});
			list.Add(new SaveLoader.PerformanceMeasurement
			{
				name = "FramesBelow30",
				value = component.NumFramesBelow30
			});
			component.Reset();
		}
		return list;
	}

	private float GetFrameTime()
	{
		PerformanceMonitor component = Global.Instance.GetComponent<PerformanceMonitor>();
		DebugUtil.LogArgs(new object[]
		{
			"Average frame time:",
			1f / component.FPS
		});
		return 1f / component.FPS;
	}

	[MyCmpGet]
	private GridSettings gridSettings;

	private bool saveFileCorrupt;

	private bool compressSaveData = true;

	private int lastUncompressedSize;

	public bool saveAsText;

	public const string MAINMENU_LEVELNAME = "launchscene";

	public const string FRONTEND_LEVELNAME = "frontend";

	public const string BACKEND_LEVELNAME = "backend";

	public const string SAVE_EXTENSION = ".sav";

	public const string AUTOSAVE_FOLDER = "auto_save";

	public const string CLOUDSAVE_FOLDER = "cloud_save_files";

	public const string SAVE_FOLDER = "save_files";

	public const int MAX_AUTOSAVE_FILES = 10;

	[NonSerialized]
	public SaveManager saveManager;

	private Cluster m_clusterLayout;

	private const string CorruptFileSuffix = "_";

	private const float SAVE_BUFFER_HEAD_ROOM = 0.1f;

	private bool mustRestartOnFail;

	public const string METRIC_SAVED_PREFAB_KEY = "SavedPrefabs";

	public const string METRIC_IS_AUTO_SAVE_KEY = "IsAutoSave";

	public const string METRIC_WAS_DEBUG_EVER_USED = "WasDebugEverUsed";

	public const string METRIC_IS_SANDBOX_ENABLED = "IsSandboxEnabled";

	public const string METRIC_RESOURCES_ACCESSIBLE_KEY = "ResourcesAccessible";

	public const string METRIC_DAILY_REPORT_KEY = "DailyReport";

	public const string METRIC_MINION_METRICS_KEY = "MinionMetrics";

	public const string METRIC_CUSTOM_GAME_SETTINGS = "CustomGameSettings";

	public const string METRIC_PERFORMANCE_MEASUREMENTS = "PerformanceMeasurements";

	public const string METRIC_FRAME_TIME = "AverageFrameTime";

	private static bool force_infinity;

	public class FlowUtilityNetworkInstance
	{
		public int id = -1;

		public SimHashes containedElement = SimHashes.Vacuum;

		public float containedMass;

		public float containedTemperature;
	}

	[SerializationConfig(global::KSerialization.MemberSerialization.OptOut)]
	public class FlowUtilityNetworkSaver : ISaveLoadable
	{
		public FlowUtilityNetworkSaver()
		{
			this.gas = new List<SaveLoader.FlowUtilityNetworkInstance>();
			this.liquid = new List<SaveLoader.FlowUtilityNetworkInstance>();
		}

		public List<SaveLoader.FlowUtilityNetworkInstance> gas;

		public List<SaveLoader.FlowUtilityNetworkInstance> liquid;
	}

	public struct SaveFileEntry
	{
		public string path;

		public global::System.DateTime timeStamp;
	}

	public enum SaveType
	{
		local,
		cloud,
		both
	}

	private struct MinionAttrFloatData
	{
		public string Name;

		public float Value;
	}

	private struct MinionMetricsData
	{
		public string Name;

		public List<SaveLoader.MinionAttrFloatData> Modifiers;

		public float TotalExperienceGained;

		public List<string> Skills;
	}

	private struct SavedPrefabMetricsData
	{
		public string PrefabName;

		public int Count;
	}

	private struct WorldInventoryMetricsData
	{
		public string Name;

		public float Amount;
	}

	private struct DailyReportMetricsData
	{
		public string Name;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Net;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Positive;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Negative;
	}

	private struct PerformanceMeasurement
	{
		public string name;

		public float value;
	}
}
