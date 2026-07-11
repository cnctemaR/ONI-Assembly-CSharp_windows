using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using KSerialization;
using Newtonsoft.Json;
using ProcGen;
using ProcGenGame;
using STRINGS;
using UnityEngine;

[SerializationConfig(global::KSerialization.MemberSerialization.OptIn)]
public class SaveGame : KMonoBehaviour, ISaveLoadable
{
	public int AutoSaveCycleInterval
	{
		get
		{
			return this.autoSaveCycleInterval;
		}
		set
		{
			this.autoSaveCycleInterval = value;
		}
	}

	public Vector2I TimelapseResolution
	{
		get
		{
			return this.timelapseResolution;
		}
		set
		{
			this.timelapseResolution = value;
		}
	}

	public string BaseName
	{
		get
		{
			return this.baseName;
		}
	}

	public static void DestroyInstance()
	{
		SaveGame.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		SaveGame.Instance = this;
		ColonyRationMonitor.Instance instance = new ColonyRationMonitor.Instance(this);
		instance.StartSM();
		VignetteManager.Instance instance2 = new VignetteManager.Instance(this);
		instance2.StartSM();
		this.entombedItemManager = base.gameObject.AddComponent<EntombedItemManager>();
		this.worldGen = SaveLoader.Instance.worldGen;
		this.worldGenSpawner = base.gameObject.AddComponent<WorldGenSpawner>();
	}

	[OnSerializing]
	private void OnSerialize()
	{
		this.speed = SpeedControlScreen.Instance.GetSpeed();
	}

	[OnDeserializing]
	private void OnDeserialize()
	{
		this.baseName = SaveLoader.Instance.GameInfo.baseName;
	}

	public int GetSpeed()
	{
		return this.speed;
	}

	public byte[] GetSaveHeader(bool isAutoSave, bool isCompressed, out SaveGame.Header header)
	{
		string text = JsonConvert.SerializeObject(new SaveGame.GameInfo(GameClock.Instance.GetCycle(), Components.LiveMinionIdentities.Count, this.baseName, isAutoSave, SaveLoader.GetActiveSaveFilePath(), SaveLoader.Instance.GameInfo.worldID, SaveLoader.Instance.GameInfo.worldTraits, this.sandboxEnabled));
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		header = default(SaveGame.Header);
		header.buildVersion = 371951U;
		header.headerSize = bytes.Length;
		header.headerVersion = 1U;
		header.compression = ((!isCompressed) ? 0 : 1);
		return bytes;
	}

	public static SaveGame.Header GetHeader(BinaryReader br)
	{
		SaveGame.Header header = default(SaveGame.Header);
		header.buildVersion = br.ReadUInt32();
		header.headerSize = br.ReadInt32();
		header.headerVersion = br.ReadUInt32();
		if (1U <= header.headerVersion)
		{
			header.compression = br.ReadInt32();
		}
		return header;
	}

	public static SaveGame.GameInfo GetHeader(IReader br, out SaveGame.Header header)
	{
		header = default(SaveGame.Header);
		header.buildVersion = br.ReadUInt32();
		header.headerSize = br.ReadInt32();
		header.headerVersion = br.ReadUInt32();
		if (1U <= header.headerVersion)
		{
			header.compression = br.ReadInt32();
		}
		byte[] array = br.ReadBytes(header.headerSize);
		return SaveGame.GetGameInfo(array);
	}

	public static SaveGame.GameInfo GetGameInfo(byte[] data)
	{
		return JsonConvert.DeserializeObject<SaveGame.GameInfo>(Encoding.UTF8.GetString(data));
	}

	public void SetBaseName(string newBaseName)
	{
		if (string.IsNullOrEmpty(newBaseName))
		{
			global::UnityEngine.Debug.LogWarning("Cannot give the base an empty name");
			return;
		}
		this.baseName = newBaseName;
	}

	protected override void OnSpawn()
	{
		ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
		Game.Instance.Trigger(-1917495436, null);
	}

	public List<Tuple<string, ScriptableObject>> GetColonyToolTip()
	{
		List<Tuple<string, ScriptableObject>> list = new List<Tuple<string, ScriptableObject>>();
		list.Add(new Tuple<string, ScriptableObject>(this.baseName, ToolTipScreen.Instance.defaultTooltipHeaderStyle));
		if (GameClock.Instance != null)
		{
			list.Add(new Tuple<string, ScriptableObject>(" ", null));
			list.Add(new Tuple<string, ScriptableObject>(string.Format(UI.ASTEROIDCLOCK.CYCLES_OLD, GameUtil.GetCurrentCycle()), ToolTipScreen.Instance.defaultTooltipHeaderStyle));
			list.Add(new Tuple<string, ScriptableObject>(string.Format(UI.ASTEROIDCLOCK.TIME_PLAYED, (GameClock.Instance.GetTimePlayedInSeconds() / 3600f).ToString("0.00")), ToolTipScreen.Instance.defaultTooltipBodyStyle));
		}
		global::ProcGen.World worldData = SettingsCache.worlds.GetWorldData(SaveLoader.Instance.GameInfo.worldID);
		list.Add(new Tuple<string, ScriptableObject>(" ", null));
		list.Add(new Tuple<string, ScriptableObject>(Strings.Get(worldData.name), ToolTipScreen.Instance.defaultTooltipHeaderStyle));
		if (SaveLoader.Instance.GameInfo.worldTraits != null)
		{
			foreach (string text in SaveLoader.Instance.GameInfo.worldTraits)
			{
				WorldTrait cachedTrait = SettingsCache.GetCachedTrait(text);
				list.Add(new Tuple<string, ScriptableObject>(Strings.Get(cachedTrait.name), ToolTipScreen.Instance.defaultTooltipBodyStyle));
			}
		}
		return list;
	}

	[Serialize]
	private int speed;

	[Serialize]
	public List<Tag> expandedResourceTags = new List<Tag>();

	[Serialize]
	public int minGermCountForDisinfect = 10000;

	[Serialize]
	public bool enableAutoDisinfect = true;

	[Serialize]
	public bool sandboxEnabled;

	[Serialize]
	private int autoSaveCycleInterval = 1;

	[Serialize]
	private Vector2I timelapseResolution = new Vector2I(512, 768);

	private string baseName;

	public static SaveGame Instance;

	public EntombedItemManager entombedItemManager;

	public WorldGenSpawner worldGenSpawner;

	[MyCmpReq]
	public MaterialSelectorSerializer materialSelectorSerializer;

	public WorldGen worldGen;

	public struct Header
	{
		public bool IsCompressed
		{
			get
			{
				return 0 != this.compression;
			}
		}

		public uint buildVersion;

		public int headerSize;

		public uint headerVersion;

		public int compression;
	}

	public struct GameInfo
	{
		public GameInfo(int numberOfCycles, int numberOfDuplicants, string baseName, bool isAutoSave, string originalSaveName, string worldID, string[] worldTraits, bool sandboxEnabled = false)
		{
			this.numberOfCycles = numberOfCycles;
			this.numberOfDuplicants = numberOfDuplicants;
			this.baseName = baseName;
			this.isAutoSave = isAutoSave;
			this.originalSaveName = originalSaveName;
			this.worldID = worldID;
			this.worldTraits = worldTraits;
			this.sandboxEnabled = sandboxEnabled;
			this.saveMajorVersion = 7;
			this.saveMinorVersion = 12;
		}

		public bool IsVersionOlderThan(int major, int minor)
		{
			return this.saveMajorVersion < major || (this.saveMajorVersion == major && this.saveMinorVersion < minor);
		}

		public bool IsVersionExactly(int major, int minor)
		{
			return this.saveMajorVersion == major && this.saveMinorVersion == minor;
		}

		public int numberOfCycles;

		public int numberOfDuplicants;

		public string baseName;

		public bool isAutoSave;

		public string originalSaveName;

		public int saveMajorVersion;

		public int saveMinorVersion;

		public string worldID;

		public string[] worldTraits;

		public bool sandboxEnabled;
	}
}
