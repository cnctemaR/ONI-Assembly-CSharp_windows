using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public struct ModInfo
{
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
