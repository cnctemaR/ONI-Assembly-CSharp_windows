using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public struct ModInfo : IEquatable<ModInfo>
{
	public ModInfo(ModInfo.Source source, ModInfo.ModType type, string asset_id, string description, string asset_path, ulong last_modified_time = 0UL)
	{
		this.source = source;
		this.type = type;
		this.assetID = asset_id;
		this.description = description;
		this.assetPath = asset_path;
		this.enabled = false;
		this.markedForDelete = false;
		this.markedForUpdate = false;
		this.lastModifiedTime = last_modified_time;
	}

	public bool Equals(ModInfo other)
	{
		return this.source == other.source && this.assetID == other.assetID;
	}

	public override int GetHashCode()
	{
		return this.source.GetHashCode() ^ this.assetID.GetHashCode();
	}

	public override bool Equals(object other)
	{
		if (!(other is ModInfo))
		{
			return false;
		}
		ModInfo modInfo = (ModInfo)other;
		return this.Equals(modInfo);
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public ModInfo.Source source;

	[JsonConverter(typeof(StringEnumConverter))]
	public ModInfo.ModType type;

	public string assetID;

	public string assetPath;

	public bool enabled;

	public bool markedForDelete;

	public bool markedForUpdate;

	public string description;

	public ulong lastModifiedTime;

	public enum Source
	{
		Local,
		Steam,
		Rail
	}

	public enum ModType
	{
		WorldGen,
		Scenario,
		Mod
	}
}
