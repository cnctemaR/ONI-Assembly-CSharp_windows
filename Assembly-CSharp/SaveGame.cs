using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using KSerialization;
using Newtonsoft.Json;
using ProcGen;
using ProcGenGame;
using STRINGS;
using UnityEngine;

[SerializationConfig(global::KSerialization.MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SaveGame")]
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
		new ColonyRationMonitor.Instance(this).StartSM();
		new VignetteManager.Instance(this).StartSM();
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
		header.buildVersion = 399090U;
		header.headerSize = bytes.Length;
		header.headerVersion = 1U;
		header.compression = (isCompressed ? 1 : 0);
		return bytes;
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
		SaveGame.GameInfo gameInfo = SaveGame.GetGameInfo(br.ReadBytes(header.headerSize));
		if (gameInfo.IsVersionOlderThan(7, 14) && gameInfo.worldTraits != null)
		{
			string[] worldTraits = gameInfo.worldTraits;
			for (int i = 0; i < worldTraits.Length; i++)
			{
				worldTraits[i] = worldTraits[i].Replace('\\', '/');
			}
		}
		return gameInfo;
	}

	public static SaveGame.GameInfo GetGameInfo(byte[] data)
	{
		return JsonConvert.DeserializeObject<SaveGame.GameInfo>(Encoding.UTF8.GetString(data));
	}

	public void SetBaseName(string newBaseName)
	{
		if (string.IsNullOrEmpty(newBaseName))
		{
			global::Debug.LogWarning("Cannot give the base an empty name");
			return;
		}
		this.baseName = newBaseName;
	}

	protected override void OnSpawn()
	{
		ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
		Game.Instance.Trigger(-1917495436, null);
	}

	public List<global::Tuple<string, ScriptableObject>> GetColonyToolTip()
	{
		List<global::Tuple<string, ScriptableObject>> list = new List<global::Tuple<string, ScriptableObject>>();
		list.Add(new global::Tuple<string, ScriptableObject>(this.baseName, ToolTipScreen.Instance.defaultTooltipHeaderStyle));
		if (GameClock.Instance != null)
		{
			list.Add(new global::Tuple<string, ScriptableObject>(" ", null));
			list.Add(new global::Tuple<string, ScriptableObject>(string.Format(UI.ASTEROIDCLOCK.CYCLES_OLD, GameUtil.GetCurrentCycle()), ToolTipScreen.Instance.defaultTooltipHeaderStyle));
			list.Add(new global::Tuple<string, ScriptableObject>(string.Format(UI.ASTEROIDCLOCK.TIME_PLAYED, (GameClock.Instance.GetTimePlayedInSeconds() / 3600f).ToString("0.00")), ToolTipScreen.Instance.defaultTooltipBodyStyle));
		}
		global::ProcGen.World worldData = SettingsCache.worlds.GetWorldData(SaveLoader.Instance.GameInfo.worldID);
		list.Add(new global::Tuple<string, ScriptableObject>(" ", null));
		list.Add(new global::Tuple<string, ScriptableObject>(Strings.Get(worldData.name), ToolTipScreen.Instance.defaultTooltipHeaderStyle));
		if (SaveLoader.Instance.GameInfo.worldTraits != null)
		{
			string[] worldTraits = SaveLoader.Instance.GameInfo.worldTraits;
			for (int i = 0; i < worldTraits.Length; i++)
			{
				WorldTrait cachedTrait = SettingsCache.GetCachedTrait(worldTraits[i], false);
				if (cachedTrait != null)
				{
					list.Add(new global::Tuple<string, ScriptableObject>(Strings.Get(cachedTrait.name), ToolTipScreen.Instance.defaultTooltipBodyStyle));
				}
				else
				{
					list.Add(new global::Tuple<string, ScriptableObject>(WORLD_TRAITS.MISSING_TRAIT, ToolTipScreen.Instance.defaultTooltipBodyStyle));
				}
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
				return this.compression != 0;
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
			this.saveMinorVersion = 16;
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
