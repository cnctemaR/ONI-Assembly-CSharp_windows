using System;
using Database;

public class EquippableFacadeInfo : IBlueprintInfo, IBlueprintDlcInfo
{
	public string id { get; set; }

	public string name { get; set; }

	public string desc { get; set; }

	public PermitRarity rarity { get; set; }

	public string animFile { get; set; }

	public string[] dlcIds { get; set; }

	public EquippableFacadeInfo(string id, string name, string desc, PermitRarity rarity, string defID, string buildOverride, string animFile)
	{
		this.id = id;
		this.name = name;
		this.desc = desc;
		this.rarity = rarity;
		this.defID = defID;
		this.buildOverride = buildOverride;
		this.animFile = animFile;
		this.dlcIds = DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public string buildOverride;

	public string defID;
}
