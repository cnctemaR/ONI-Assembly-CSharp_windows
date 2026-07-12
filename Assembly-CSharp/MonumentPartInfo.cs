using System;
using Database;

public class MonumentPartInfo : IBlueprintInfo, IBlueprintDlcInfo
{
	public string id { get; set; }

	public string name { get; set; }

	public string desc { get; set; }

	public PermitRarity rarity { get; set; }

	public string animFile { get; set; }

	public string[] dlcIds { get; set; }

	public MonumentPartInfo(string id, string name, string desc, PermitRarity rarity, string animFilename, string state, string symbolName, MonumentPartResource.Part part, string[] dlcIds)
	{
		this.id = id;
		this.name = name;
		this.desc = desc;
		this.rarity = rarity;
		this.animFile = animFilename;
		this.state = state;
		this.symbolName = symbolName;
		this.part = part;
		this.dlcIds = dlcIds;
	}

	public string state;

	public string symbolName;

	public MonumentPartResource.Part part;
}
