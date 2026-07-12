using System;
using Database;

public class ArtableInfo : IBlueprintInfo, IBlueprintDlcInfo
{
	public string id { get; set; }

	public string name { get; set; }

	public string desc { get; set; }

	public PermitRarity rarity { get; set; }

	public string animFile { get; set; }

	public string[] dlcIds { get; set; }

	public ArtableInfo(string id, string name, string desc, PermitRarity rarity, string animFile, string anim, int decor_value, bool cheer_on_complete, string status_id, string prefabId, string symbolname = "")
	{
		this.id = id;
		this.name = name;
		this.desc = desc;
		this.rarity = rarity;
		this.animFile = animFile;
		this.anim = anim;
		this.decor_value = decor_value;
		this.cheer_on_complete = cheer_on_complete;
		this.status_id = status_id;
		this.prefabId = prefabId;
		this.symbolname = symbolname;
		this.dlcIds = DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public string anim;

	public int decor_value;

	public bool cheer_on_complete;

	public string status_id;

	public string prefabId;

	public string symbolname;
}
