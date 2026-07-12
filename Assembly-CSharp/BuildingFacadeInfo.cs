using System;
using System.Collections.Generic;
using Database;

public class BuildingFacadeInfo : IBlueprintInfo, IBlueprintDlcInfo
{
	public string id { get; set; }

	public string name { get; set; }

	public string desc { get; set; }

	public PermitRarity rarity { get; set; }

	public string animFile { get; set; }

	public string[] dlcIds { get; set; }

	public BuildingFacadeInfo(string id, string name, string desc, PermitRarity rarity, string prefabId, string animFile, string[] dlcIds, Dictionary<string, string> workables = null)
	{
		this.id = id;
		this.name = name;
		this.desc = desc;
		this.rarity = rarity;
		this.prefabId = prefabId;
		this.animFile = animFile;
		this.workables = workables;
		this.dlcIds = dlcIds;
	}

	public string prefabId;

	public Dictionary<string, string> workables;
}
