using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using KSerialization;
using Newtonsoft.Json;
using UnityEngine;

[SerializationConfig(global::KSerialization.MemberSerialization.OptIn)]
public class SaveGame : KMonoBehaviour, ISaveLoadableJson
{
	public string BaseName
	{
		get
		{
			return this.baseName;
		}
	}

	protected override void OnPrefabInit()
	{
		SaveGame.Instance = this;
		ColonyRationMonitor.Instance instance = new ColonyRationMonitor.Instance(this);
		instance.StartSM();
		RedAlertManager.Instance instance2 = new RedAlertManager.Instance(this);
		instance2.StartSM();
	}

	[OnSerializing]
	private void OnSerialize()
	{
		this.speed = SpeedControlScreen.Instance.GetSpeed();
	}

	[OnDeserializing]
	private void OnDeserialize()
	{
		this.baseName = SaveLoader.Instance.SaveHeader.baseName;
	}

	public int GetSpeed()
	{
		return this.speed;
	}

	public byte[] GetSaveHeader(bool isAutoSave, out SaveGame.Header header)
	{
		string text;
		if (isAutoSave)
		{
			text = JsonConvert.SerializeObject(new SaveGame.HeaderData(GameClock.Instance.GetDay(), Components.LiveMinionIdentities.Count, this.baseName, true, SaveLoader.GetActiveSaveFilePath()));
		}
		else
		{
			text = JsonConvert.SerializeObject(new SaveGame.HeaderData(GameClock.Instance.GetDay(), Components.LiveMinionIdentities.Count, this.baseName));
		}
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		header = default(SaveGame.Header);
		header.buildVersion = 208689U;
		header.headerSize = bytes.Length;
		return bytes;
	}

	public static SaveGame.Header GetHeader(BinaryReader br)
	{
		return new SaveGame.Header
		{
			buildVersion = br.ReadUInt32(),
			headerSize = br.ReadInt32(),
			headerVersion = br.ReadUInt32()
		};
	}

	public static SaveGame.HeaderData GetHeader(IReader br, out SaveGame.Header header)
	{
		header = default(SaveGame.Header);
		header.buildVersion = br.ReadUInt32();
		header.headerSize = br.ReadInt32();
		header.headerVersion = br.ReadUInt32();
		byte[] array = br.ReadBytes(header.headerSize);
		return SaveGame.GetHeaderData(array);
	}

	public static SaveGame.HeaderData GetHeaderData(byte[] data)
	{
		return JsonConvert.DeserializeObject<SaveGame.HeaderData>(Encoding.UTF8.GetString(data));
	}

	public void SetBaseName(string newBaseName)
	{
		if (string.IsNullOrEmpty(newBaseName))
		{
			Debug.LogWarning("Cannot give the base an empty name");
			return;
		}
		this.baseName = newBaseName;
	}

	protected override void OnSpawn()
	{
		ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
		Game.Instance.Trigger(-1917495436, null);
	}

	[Serialize]
	private int speed;

	private string baseName;

	public static SaveGame Instance;

	public struct Header
	{
		public uint buildVersion;

		public int headerSize;

		public uint headerVersion;
	}

	public struct HeaderData
	{
		public HeaderData(int numberOfCycles, int numberOfDuplicants, string baseName, bool isAutoSave, string originalSaveName)
		{
			this.numberOfCycles = numberOfCycles;
			this.numberOfDuplicants = numberOfDuplicants;
			this.baseName = baseName;
			this.isAutoSave = isAutoSave;
			this.originalSaveName = originalSaveName;
		}

		public HeaderData(int numberOfCycles, int numberOfDuplicants, string baseName)
		{
			this.numberOfCycles = numberOfCycles;
			this.numberOfDuplicants = numberOfDuplicants;
			this.baseName = baseName;
			this.isAutoSave = false;
			this.originalSaveName = string.Empty;
		}

		public int numberOfCycles;

		public int numberOfDuplicants;

		public string baseName;

		public bool isAutoSave;

		public string originalSaveName;
	}
}
