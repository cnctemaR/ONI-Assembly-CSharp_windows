using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;
using Klei;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using KSerialization;
using Newtonsoft.Json;
using ProcGenGame;
using STRINGS;
using UnityEngine;

public class SaveLoader : KMonoBehaviour
{
	public bool loadedFromSave { get; private set; }

	public static void DestroyInstance()
	{
		SaveLoader.Instance = null;
	}

	public static SaveLoader Instance { get; private set; }

	public global::System.Action OnWorldGenComplete { get; set; }

	public SaveGame.Header LoadedHeader { get; private set; }

	public SaveGame.GameInfo GameInfo { get; private set; }

	protected override void OnPrefabInit()
	{
		SaveLoader.Instance = this;
		this.saveManager = base.GetComponent<SaveManager>();
	}

	private void MoveCorruptFile(string filename)
	{
		try
		{
		}
		catch
		{
			File.Replace(filename, filename + "_", filename + "_.bak", true);
		}
	}

	protected override void OnSpawn()
	{
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		if (WorldGen.CanLoad(activeSaveFilePath))
		{
			Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			SimMessages.CreateDiseaseTable();
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
			bool flag = WorldGen.CanLoad(WorldGen.SIM_SAVE_FILENAME);
			if (!flag || !this.LoadFromWorldGen())
			{
				DebugUtil.LogWarningArgs(new object[] { "Couldn't start new game with current world gen, moving file" });
				if (flag)
				{
					KMonoBehaviour.isLoadingScene = true;
					this.MoveCorruptFile(WorldGen.SIM_SAVE_FILENAME);
				}
				App.LoadScene("frontend");
			}
		}
	}

	public static byte[] CompressContents(byte[] uncompressed)
	{
		return SaveLoader.CompressContents(uncompressed, uncompressed.Length);
	}

	public static byte[] CompressContents(byte[] uncompressed, int length)
	{
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream(length))
		{
			using (ZlibStream zlibStream = new ZlibStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestSpeed))
			{
				zlibStream.Write(uncompressed, 0, length);
				zlibStream.Flush();
			}
			memoryStream.Flush();
			array = memoryStream.ToArray();
		}
		return array;
	}

	private byte[] FloatToBytes(float[] floats)
	{
		byte[] array = new byte[floats.Length * 4];
		Buffer.BlockCopy(floats, 0, array, 0, array.Length);
		return array;
	}

	public static byte[] DecompressContents(byte[] compressed)
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
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				Sim.Save(binaryWriter);
			}
			if (this.zipStreams)
			{
				saveFileRoot.streamed["SimBZ"] = SaveLoader.CompressContents(memoryStream.ToArray());
			}
			else
			{
				saveFileRoot.streamed["Sim"] = memoryStream.ToArray();
			}
		}
		if (this.zipStreams)
		{
			saveFileRoot.streamed["GridVisibleBZ"] = SaveLoader.CompressContents(Grid.Visible);
			saveFileRoot.streamed["GridSpawnableBZ"] = SaveLoader.CompressContents(Grid.Spawnable);
			saveFileRoot.streamed["GridDamageBZ"] = SaveLoader.CompressContents(this.FloatToBytes(Grid.Damage));
		}
		else
		{
			saveFileRoot.streamed["GridVisible"] = Grid.Visible;
			saveFileRoot.streamed["GridSpawnable"] = Grid.Spawnable;
			saveFileRoot.streamed["GridDamage"] = this.FloatToBytes(Grid.Damage);
		}
		Global.Instance.modManager.SendMetricsEvent();
		saveFileRoot.active_mods = new List<Label>();
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if (mod.enabled)
			{
				saveFileRoot.active_mods.Add(mod.label);
			}
		}
		string text = ((Game.worldID == null) ? CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id : Game.worldID);
		saveFileRoot.worldID = text;
		using (MemoryStream memoryStream2 = new MemoryStream())
		{
			using (BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2))
			{
				Camera.main.transform.parent.GetComponent<CameraController>().Save(binaryWriter2);
			}
			saveFileRoot.streamed["Camera"] = memoryStream2.ToArray();
		}
		return saveFileRoot;
	}

	private void Save(BinaryWriter writer)
	{
		writer.WriteKleiString("world");
		SaveFileRoot saveFileRoot = this.PrepSaveFile();
		Serializer.Serialize(saveFileRoot, writer);
		Game.SaveSettings(writer);
		this.saveManager.Save(writer);
		Game.Instance.Save(writer);
	}

	private bool Load(IReader reader)
	{
		string text = reader.ReadKleiString();
		global::Debug.Assert(text == "world");
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
					version = modInfo.lastModifiedTime,
					distribution_platform = Label.DistributionPlatform.Steam,
					title = modInfo.description
				});
			}
			saveFileRoot.requiredMods.Clear();
		}
		global::KMod.Manager modManager = Global.Instance.modManager;
		modManager.Load(Content.LayerableFiles);
		if (!modManager.MatchFootprint(saveFileRoot.active_mods, Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation))
		{
			DebugUtil.LogWarningArgs(new object[] { "Mod footprint of save file doesn't match current mod configuration" });
		}
		Global.Instance.modManager.SendMetricsEvent();
		string text2 = saveFileRoot.worldID;
		if (text2 == null)
		{
			try
			{
				text2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id;
			}
			catch
			{
				text2 = "worlds/Default";
			}
		}
		Game.worldID = text2;
		this.worldGen = new WorldGen(text2);
		Game.LoadSettings(deserializer);
		GridSettings.Reset(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells);
		Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		SimMessages.CreateDiseaseTable();
		byte[] array = saveFileRoot.streamed["Sim"];
		FastReader fastReader = new FastReader(array);
		if (Sim.Load(fastReader) != 0)
		{
			DebugUtil.LogWarningArgs(new object[] { "\n--- Error loading save ---\nSimDLL found bad data\n" });
			Sim.Shutdown();
			return false;
		}
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
		FastReader fastReader2 = new FastReader(saveFileRoot.streamed["Camera"]);
		CameraSaveData.Load(fastReader2);
		return true;
	}

	public static string GetSavePrefix()
	{
		string text = Util.RootFolder();
		return Path.Combine(text, "save_files/");
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

	public static string GetAutoSavePrefix()
	{
		string text = Path.Combine(SaveLoader.GetSavePrefixAndCreateFolder(), "auto_save/");
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

	public static string GetAutosaveFilePath()
	{
		return SaveLoader.GetAutoSavePrefix() + "AutoSave Cycle 1.sav";
	}

	public static string GetActiveSaveFolder()
	{
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		if (!string.IsNullOrEmpty(activeSaveFilePath))
		{
			return Path.GetDirectoryName(activeSaveFilePath);
		}
		return null;
	}

	public static List<string> GetSaveFiles(string save_dir)
	{
		List<string> list = new List<string>();
		try
		{
			if (!global::System.IO.Directory.Exists(save_dir))
			{
				global::System.IO.Directory.CreateDirectory(save_dir);
			}
			string[] files = global::System.IO.Directory.GetFiles(save_dir, "*.sav", SearchOption.AllDirectories);
			List<SaveLoader.SaveFileEntry> list2 = new List<SaveLoader.SaveFileEntry>();
			foreach (string text in files)
			{
				try
				{
					global::System.DateTime lastWriteTime = File.GetLastWriteTime(text);
					SaveLoader.SaveFileEntry saveFileEntry = new SaveLoader.SaveFileEntry
					{
						path = text,
						timeStamp = lastWriteTime
					};
					list2.Add(saveFileEntry);
				}
				catch (Exception ex)
				{
					global::Debug.LogWarning("Problem reading file: " + text + "\n" + ex.ToString());
				}
			}
			list2.Sort((SaveLoader.SaveFileEntry x, SaveLoader.SaveFileEntry y) => y.timeStamp.CompareTo(x.timeStamp));
			foreach (SaveLoader.SaveFileEntry saveFileEntry2 in list2)
			{
				list.Add(saveFileEntry2.path);
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
			GameObject gameObject = ((!(FrontEndManager.Instance == null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas);
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(text2, null, null, null, null, null, null, null, null);
		}
		return list;
	}

	public static List<string> GetAllFiles()
	{
		return SaveLoader.GetSaveFiles(SaveLoader.GetSavePrefixAndCreateFolder());
	}

	public static string GetLatestSaveFile()
	{
		List<string> allFiles = SaveLoader.GetAllFiles();
		if (allFiles.Count == 0)
		{
			return null;
		}
		return allFiles[0];
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
		this.Save(text, false, true);
	}

	public string Save(string filename, bool isAutoSave = false, bool updateSavePointer = true)
	{
		global::KSerialization.Manager.Clear();
		this.ReportSaveMetrics(isAutoSave);
		if (isAutoSave && !GenericGameSettings.instance.keepAllAutosaves)
		{
			List<string> saveFiles = SaveLoader.GetSaveFiles(Path.GetDirectoryName(filename));
			for (int i = saveFiles.Count - 1; i >= 9; i--)
			{
				string text = saveFiles[i];
				try
				{
					global::Debug.Log("Deleting old autosave: " + text);
					File.Delete(text);
				}
				catch (Exception ex)
				{
					global::Debug.LogWarning("Problem deleting autosave: " + text + "\n" + ex.ToString());
				}
				if (GenericGameSettings.instance.takeSaveScreenshots)
				{
					string text2 = Path.ChangeExtension(text, ".png");
					try
					{
						if (File.Exists(text2))
						{
							File.Delete(text2);
						}
					}
					catch (Exception ex2)
					{
						global::Debug.LogWarning("Problem deleting autosave screenshot: " + text2 + "\n" + ex2.ToString());
					}
				}
			}
		}
		byte[] array = null;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				this.Save(binaryWriter);
				if (this.compressSaveData)
				{
					array = SaveLoader.CompressContents(memoryStream.GetBuffer(), (int)memoryStream.Length);
				}
				else
				{
					array = memoryStream.ToArray();
				}
			}
		}
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
				binaryWriter2.Write(array);
				Stats.Print();
			}
		}
		catch (Exception ex3)
		{
			if (ex3 is UnauthorizedAccessException)
			{
				DebugUtil.LogArgs(new object[] { "UnauthorizedAccessException for " + filename });
				ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
				confirmDialogScreen.PopupConfirmDialog(string.Format(UI.CRASHSCREEN.SAVEFAILED, "Unauthorized Access Exception"), null, null, null, null, null, null, null, null);
				return SaveLoader.GetActiveSaveFilePath();
			}
			if (ex3 is IOException)
			{
				DebugUtil.LogArgs(new object[] { "IOException (probably out of disk space) for " + filename });
				ConfirmDialogScreen confirmDialogScreen2 = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
				confirmDialogScreen2.PopupConfirmDialog(string.Format(UI.CRASHSCREEN.SAVEFAILED, "IOException. You may not have enough free space!"), null, null, null, null, null, null, null, null);
				return SaveLoader.GetActiveSaveFilePath();
			}
			throw ex3;
		}
		if (updateSavePointer)
		{
			SaveLoader.SetActiveSaveFilePath(filename);
		}
		if (GenericGameSettings.instance.takeSaveScreenshots)
		{
			string text3 = Path.ChangeExtension(filename, ".png");
			ScreenCapture.CaptureScreenshot(text3, 1);
		}
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
		SaveGame.GameInfo gameInfo;
		using (BinaryReader binaryReader = new BinaryReader(File.Open(filename, FileMode.Open)))
		{
			header = SaveGame.GetHeader(binaryReader);
			byte[] array = binaryReader.ReadBytes(header.headerSize);
			gameInfo = SaveGame.GetGameInfo(array);
		}
		return gameInfo;
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
			this.GameInfo = SaveGame.GetHeader(reader, out header);
			this.LoadedHeader = header;
			DebugUtil.LogArgs(new object[] { string.Format("Loading save file: {4}\n headerVersion:{0}, buildVersion:{1}, headerSize:{2}, IsCompressed:{3}", new object[] { header.headerVersion, header.buildVersion, header.headerSize, header.IsCompressed, filename }) });
			DebugUtil.LogArgs(new object[] { string.Format("GameInfo: numberOfCycles:{0}, numberOfDuplicants:{1}, baseName:{2}, isAutoSave:{3}, originalSaveName:{4}, saveVersion:{5}.{6}", new object[]
			{
				this.GameInfo.numberOfCycles,
				this.GameInfo.numberOfDuplicants,
				this.GameInfo.baseName,
				this.GameInfo.isAutoSave,
				this.GameInfo.originalSaveName,
				this.GameInfo.saveMajorVersion,
				this.GameInfo.saveMinorVersion
			}) });
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
				IReader reader2 = new FastReader(array3);
				this.Load(reader2);
			}
			else
			{
				this.Load(reader);
			}
			if (this.GameInfo.isAutoSave && !string.IsNullOrEmpty(this.GameInfo.originalSaveName))
			{
				SaveLoader.SetActiveSaveFilePath(this.GameInfo.originalSaveName);
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
			string.Concat(new object[]
			{
				"[",
				this.worldDetailSave.globalWorldSeed,
				"/",
				this.worldDetailSave.globalWorldLayoutSeed,
				"/",
				this.worldDetailSave.globalTerrainSeed,
				"/",
				this.worldDetailSave.globalNoiseSeed,
				"]"
			})
		});
		GC.Collect();
		return true;
	}

	public bool LoadFromWorldGen()
	{
		DebugUtil.LogArgs(new object[] { "Attempting to start a new game with current world gen" });
		WorldGen.LoadSettings();
		string text;
		try
		{
			text = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id;
		}
		catch
		{
			text = "worlds/Default";
		}
		this.worldGen = new WorldGen(text);
		SimSaveFileStructure simSaveFileStructure = this.worldGen.LoadWorldGenSim();
		if (simSaveFileStructure == null)
		{
			global::Debug.LogError("Attempt failed");
			return false;
		}
		this.worldDetailSave = simSaveFileStructure.worldDetail;
		if (this.worldDetailSave == null)
		{
			global::Debug.LogError("Detail is null");
		}
		GridSettings.Reset(simSaveFileStructure.WidthInCells, simSaveFileStructure.HeightInCells);
		Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		SimMessages.CreateDiseaseTable();
		try
		{
			FastReader fastReader = new FastReader(simSaveFileStructure.Sim);
			if (Sim.Load(fastReader) != 0)
			{
				DebugUtil.LogWarningArgs(new object[] { "\n--- Error loading save ---\nSimDLL found bad data\n" });
				Sim.Shutdown();
				return false;
			}
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning("--- Error loading Sim FROM NEW WORLDGEN ---" + ex.Message + "\n" + ex.StackTrace);
			Sim.Shutdown();
			return false;
		}
		global::Debug.Log("Attempt success");
		SceneInitializer.Instance.PostLoadPrefabs();
		SceneInitializer.Instance.NewSaveGamePrefab();
		this.worldGen.ReplayGenerate(new WorldGen.ResetFunction(this.Reset));
		this.OnWorldGenComplete.Signal();
		ThreadedHttps<KleiMetrics>.Instance.StartNewGame();
		return true;
	}

	public GameSpawnData cachedGSD { get; private set; }

	public WorldDetailSave worldDetailSave { get; private set; }

	public void SetWorldDetail(WorldDetailSave worldDetail)
	{
		this.worldDetailSave = worldDetail;
	}

	private void Reset(GameSpawnData gsd)
	{
		this.cachedGSD = gsd;
	}

	private void ReportSaveMetrics(bool is_auto_save)
	{
		if (ThreadedHttps<KleiMetrics>.Instance == null || !ThreadedHttps<KleiMetrics>.Instance.enabled || this.saveManager == null)
		{
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary[GameClock.NewCycleKey] = GameClock.Instance.GetCycle() + 1;
		dictionary["WasDebugEverUsed"] = Game.Instance.debugWasUsed;
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
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(dictionary);
	}

	private List<SaveLoader.MinionMetricsData> GetMinionMetrics()
	{
		List<SaveLoader.MinionMetricsData> list = new List<SaveLoader.MinionMetricsData>();
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			if (!(minionIdentity == null))
			{
				Modifiers component = minionIdentity.gameObject.GetComponent<Modifiers>();
				Amounts amounts = component.amounts;
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
				MinionResume component2 = minionIdentity.gameObject.GetComponent<MinionResume>();
				float totalExperienceGained = component2.TotalExperienceGained;
				List<string> list3 = new List<string>();
				foreach (KeyValuePair<string, bool> keyValuePair in component2.MasteryBySkillID)
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
		Dictionary<Tag, float> accessibleAmounts = WorldInventory.Instance.GetAccessibleAmounts();
		List<SaveLoader.WorldInventoryMetricsData> list = new List<SaveLoader.WorldInventoryMetricsData>(accessibleAmounts.Count);
		foreach (KeyValuePair<Tag, float> keyValuePair in accessibleAmounts)
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

	public bool saveAsText;

	public bool zipStreams;

	public const string MAINMENU_LEVELNAME = "launchscene";

	public const string FRONTEND_LEVELNAME = "frontend";

	public const string BACKEND_LEVELNAME = "backend";

	public const string SAVE_EXTENSION = ".sav";

	public const int MAX_AUTOSAVE_FILES = 10;

	[NonSerialized]
	public SaveManager saveManager;

	private const string CorruptFileSuffix = "_";

	private bool mustRestartOnFail;

	public WorldGen worldGen;

	public const string METRIC_SAVED_PREFAB_KEY = "SavedPrefabs";

	public const string METRIC_IS_AUTO_SAVE_KEY = "IsAutoSave";

	public const string METRIC_WAS_DEBUG_EVER_USED = "WasDebugEverUsed";

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
