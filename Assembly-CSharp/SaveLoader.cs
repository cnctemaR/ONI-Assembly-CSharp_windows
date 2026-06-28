using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Delaunay.Geo;
using Ionic.Zlib;
using Klei;
using KSerialization;
using UnityEngine;

public class SaveLoader : KMonoBehaviour
{
	public static SaveLoader Instance { get; private set; }

	public global::System.Action OnWorldGenComplete { get; set; }

	public SaveGame.HeaderData SaveHeader { get; private set; }

	protected override void OnPrefabInit()
	{
		SaveLoader.Instance = this;
		this.saveManager = base.GetComponent<SaveManager>();
	}

	private void MoveCurruptFile(string filename)
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
		WorldGen.LoadSettings();
		this.CheckForLoad();
	}

	public void CheckForLoad()
	{
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		if (WorldGen.CanLoad(activeSaveFilePath))
		{
			Sim.SIM_Initialize(null);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			this.loadedFromSave = this.Load(activeSaveFilePath);
			this.saveFileCurrupt = !this.loadedFromSave;
			if (!this.loadedFromSave)
			{
				SaveLoader.SetActiveSaveFilePath(null);
				if (this.mustRestartOnFail)
				{
					this.MoveCurruptFile(activeSaveFilePath);
					Sim.Shutdown();
					App.LoadScene("frontend");
					return;
				}
			}
		}
		if (!this.loadedFromSave)
		{
			Sim.Shutdown();
			if (activeSaveFilePath != null && activeSaveFilePath != string.Empty)
			{
				Output.Log(new object[] { "Couldn't load [" + activeSaveFilePath + "]" });
			}
			if (this.saveFileCurrupt)
			{
				this.MoveCurruptFile(activeSaveFilePath);
			}
			bool flag = WorldGen.CanLoad(WorldGen.SIM_SAVE_FILENAME);
			if (!flag || !this.LoadFromWorldGen())
			{
				Output.LogWarning(new object[] { "Couldn't start new game with current world gen, moving file" });
				if (flag)
				{
					KMonoBehaviour.isLoadingScene = true;
					this.MoveCurruptFile(WorldGen.SIM_SAVE_FILENAME);
				}
				App.LoadScene("frontend");
			}
			return;
		}
	}

	public static byte[] CompressContents(byte[] uncompressed)
	{
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream(uncompressed.Length))
		{
			using (ZlibStream zlibStream = new ZlibStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestSpeed))
			{
				zlibStream.Write(uncompressed, 0, uncompressed.Length);
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
			saveFileRoot.streamed["GridDamageBZ"] = SaveLoader.CompressContents(this.FloatToBytes(Grid.Damage));
		}
		else
		{
			saveFileRoot.streamed["GridVisible"] = Grid.Visible;
			saveFileRoot.streamed["GridDamage"] = this.FloatToBytes(Grid.Damage);
		}
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

	public static string GetSavePrefix()
	{
		string text = Util.RootFolder();
		string text2 = Path.Combine(text, "save_files/");
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		return text2;
	}

	public static string GetAutoSavePrefix()
	{
		string text = Path.Combine(SaveLoader.GetSavePrefix(), "auto_save/");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	public static void SetActiveSaveFilePath(string path)
	{
		PlayerPrefs.SetString("SaveFilenameKey/", path);
	}

	public static string GetActiveSaveFilePath()
	{
		return PlayerPrefs.GetString("SaveFilenameKey/");
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
		if (!Directory.Exists(save_dir))
		{
			Directory.CreateDirectory(save_dir);
		}
		List<string> list = new List<string>(Directory.GetFiles(save_dir));
		List<string> list2 = new List<string>();
		foreach (string text in list)
		{
			if (Path.GetExtension(text) == ".sav")
			{
				try
				{
					File.GetLastWriteTime(text);
					list2.Add(text);
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Problem reading file: " + text);
				}
			}
		}
		return list2;
	}

	public static int GetSaveFileCount()
	{
		return SaveLoader.GetAllFiles().Count;
	}

	public static List<string> GetAllFiles()
	{
		List<string> list = SaveLoader.GetSaveFiles(SaveLoader.GetSavePrefix()).ToList<string>();
		list.AddRange(SaveLoader.GetSaveFiles(SaveLoader.GetAutoSavePrefix()));
		return list.OrderByDescending<string, global::System.DateTime>((string file) => File.GetLastWriteTime(file)).ToList<string>();
	}

	public static string GetLatestSaveFile()
	{
		List<string> allFiles = SaveLoader.GetAllFiles();
		if (allFiles.Count == 0)
		{
			return null;
		}
		return SaveLoader.GetAllFiles()[0];
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
		Manager.Clear();
		if (isAutoSave)
		{
			List<string> list = SaveLoader.GetSaveFiles(SaveLoader.GetAutoSavePrefix());
			list = list.OrderBy<string, global::System.DateTime>((string file) => File.GetLastWriteTime(file)).ToList<string>();
			while (list.Count >= 10)
			{
				File.Delete(list[0]);
				list.RemoveAt(0);
			}
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				using (BinaryWriter binaryWriter2 = new BinaryWriter(File.Open(filename, FileMode.Create)))
				{
					SaveGame.Header header;
					byte[] saveHeader = SaveGame.Instance.GetSaveHeader(isAutoSave, out header);
					binaryWriter2.Write(header.buildVersion);
					binaryWriter2.Write(header.headerSize);
					binaryWriter2.Write(header.headerVersion);
					binaryWriter2.Write(saveHeader);
					this.Save(binaryWriter);
					Manager.SerializeDirectory(binaryWriter2);
					memoryStream.WriteTo(binaryWriter2.BaseStream);
					Stats.Print();
					Manager.Clear();
				}
			}
		}
		if (updateSavePointer)
		{
			SaveLoader.SetActiveSaveFilePath(filename);
		}
		Output.Log(new object[]
		{
			"Saved to",
			"[" + filename + "]"
		});
		return filename;
	}

	public static SaveGame.HeaderData LoadHeader(string filename, out SaveGame.Header header)
	{
		SaveGame.HeaderData headerData;
		using (BinaryReader binaryReader = new BinaryReader(File.Open(filename, FileMode.Open)))
		{
			header = SaveGame.GetHeader(binaryReader);
			byte[] array = binaryReader.ReadBytes(header.headerSize);
			headerData = SaveGame.GetHeaderData(array);
		}
		return headerData;
	}

	public bool Load(string filename)
	{
		SaveLoader.SetActiveSaveFilePath(filename);
		try
		{
			byte[] array = File.ReadAllBytes(filename);
			IReader reader = new FastReader(array);
			SaveGame.Header header;
			this.SaveHeader = SaveGame.GetHeader(reader, out header);
			Manager.assemblies = new Assembly[]
			{
				typeof(WorldGen).Assembly,
				typeof(Polygon).Assembly,
				typeof(Vector2).Assembly
			};
			Manager.DeserializeDirectory(reader);
			string text = reader.ReadKleiString();
			Debug.Assert(text == "world");
			Deserializer deserializer = new Deserializer(reader);
			SaveFileRoot saveFileRoot = new SaveFileRoot();
			deserializer.Deserialize(saveFileRoot);
			Game.LoadSettings(deserializer);
			GridSettings.Reset(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells);
			Sim.SIM_Initialize(null);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			byte[] array2 = saveFileRoot.streamed["Sim"];
			FastReader fastReader = new FastReader(array2);
			if (Sim.Load(fastReader) != 0)
			{
				Output.LogWarning(new object[] { "\n--- Error loading save ---\nSimDLL found bad data\n" });
				Sim.Shutdown();
				return false;
			}
			if (PlayerPrefs.HasKey("TemperatureUnit"))
			{
				GameUtil.temperatureUnit = (GameUtil.TemperatureUnit)PlayerPrefs.GetInt("TemperatureUnit");
			}
			if (PlayerPrefs.HasKey("MassUnit"))
			{
				GameUtil.massUnit = (GameUtil.MassUnit)PlayerPrefs.GetInt("MassUnit");
			}
			SceneInitializer.Instance.PostLoadPrefabs();
			this.mustRestartOnFail = true;
			if (!this.saveManager.Load(reader))
			{
				Sim.Shutdown();
				Output.LogWarning(new object[] { "\n--- Error loading save ---\n" });
				SaveLoader.SetActiveSaveFilePath(null);
				return false;
			}
			Grid.Visible = saveFileRoot.streamed["GridVisible"];
			Grid.Damage = this.BytesToFloat(saveFileRoot.streamed["GridDamage"]);
			Game.Instance.Load(deserializer);
			FastReader fastReader2 = new FastReader(saveFileRoot.streamed["Camera"]);
			CameraSaveData.Load(fastReader2);
			if (this.SaveHeader.isAutoSave && !string.IsNullOrEmpty(this.SaveHeader.originalSaveName))
			{
				SaveLoader.SetActiveSaveFilePath(this.SaveHeader.originalSaveName);
			}
		}
		catch (Exception ex)
		{
			Output.LogWarning(new object[] { "\n--- Error loading save ---\n" + ex.Message + "\n" + ex.StackTrace });
			Sim.Shutdown();
			SaveLoader.SetActiveSaveFilePath(null);
			return false;
		}
		Stats.Print();
		Output.Log(new object[]
		{
			"Loaded",
			"[" + filename + "]"
		});
		return true;
	}

	public bool LoadFromWorldGen()
	{
		Output.Log(new object[] { "Attempting to start a new game with current world gen" });
		SimSaveFileStructure simSaveFileStructure = WorldGen.LoadWorldGenSim();
		if (simSaveFileStructure == null)
		{
			Debug.LogError("Attempt failed");
			return false;
		}
		this.worldDetailSave = simSaveFileStructure.worldDetail;
		if (this.worldDetailSave == null)
		{
			Debug.LogError("Detail is null");
		}
		GridSettings.Reset(simSaveFileStructure.WidthInCells, simSaveFileStructure.HeightInCells);
		Sim.SIM_Initialize(null);
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		try
		{
			FastReader fastReader = new FastReader(simSaveFileStructure.Sim);
			if (Sim.Load(fastReader) != 0)
			{
				Output.LogWarning(new object[] { "\n--- Error loading save ---\nSimDLL found bad data\n" });
				Sim.Shutdown();
				return false;
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("--- Error loading Sim FROM NEW WORLDGEN ---" + ex.Message + "\n" + ex.StackTrace);
			Sim.Shutdown();
			return false;
		}
		Debug.Log("Attempt success");
		SceneInitializer.Instance.PostLoadPrefabs();
		SceneInitializer.Instance.NewSaveGamePrefab();
		WorldGen.ReplayGenerate(new WorldGen.ResetFunction(this.Reset));
		this.OnWorldGenComplete.Signal();
		UpdateManager.instance.enabled = true;
		UpdateManager.instance.SkipNextUpdate();
		ThreadedHttps<KleiMetrics>.Instance.StartNewGame();
		return true;
	}

	public WorldGen.GameSpawnData cachedGSD { get; private set; }

	public WorldDetailSave worldDetailSave { get; private set; }

	public void SetWorldDetail(WorldDetailSave worldDetail)
	{
		this.worldDetailSave = worldDetail;
	}

	private void Reset(WorldGen.GameSpawnData gsd)
	{
		this.cachedGSD = gsd;
	}

	public const string MAINMENU_LEVELNAME = "launchscene";

	public const string FRONTEND_LEVELNAME = "frontend";

	public const string BACKEND_LEVELNAME = "backend";

	public const string SAVE_EXTENSION = ".sav";

	public const int MAX_AUTOSAVE_FILES = 10;

	private const string CorruptFileSuffix = "_";

	[MyCmpGet]
	private GridSettings gridSettings;

	private bool loadedFromSave;

	private bool saveFileCurrupt;

	public bool saveAsText;

	public bool zipStreams;

	[NonSerialized]
	public SaveManager saveManager;

	public TextAsset simElementsSolidsFile;

	public TextAsset simElementsLiquidsFile;

	public TextAsset simElementsGasesFile;

	[SerializeField]
	private TextAsset elementAudio;

	public TextAsset worldGenSettingsFile;

	private bool mustRestartOnFail;

	public class FlowUtilityNetworkInstance
	{
		public int id = -1;

		public SimHashes containedElement = SimHashes.Vacuum;

		public float containedMass;

		public float containedTemperature;
	}

	[SerializationConfig(MemberSerialization.OptOut)]
	public class FlowUtilityNetworkSaver : ISaveLoadableJson
	{
		public FlowUtilityNetworkSaver()
		{
			this.gas = new List<SaveLoader.FlowUtilityNetworkInstance>();
			this.liquid = new List<SaveLoader.FlowUtilityNetworkInstance>();
		}

		public List<SaveLoader.FlowUtilityNetworkInstance> gas;

		public List<SaveLoader.FlowUtilityNetworkInstance> liquid;
	}
}
