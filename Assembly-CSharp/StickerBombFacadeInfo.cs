using System;
using Database;

public class StickerBombFacadeInfo : IBlueprintInfo, IBlueprintDlcInfo
{
	public string id { get; set; }

	public string name { get; set; }

	public string desc { get; set; }

	public PermitRarity rarity { get; set; }

	public string animFile { get; set; }

	public string[] dlcIds { get; set; }

	public StickerBombFacadeInfo(string id, string name, string desc, PermitRarity rarity, string animFile, string sticker)
	{
		this.id = id;
		this.name = name;
		this.desc = desc;
		this.rarity = rarity;
		this.animFile = animFile;
		this.sticker = sticker;
		this.dlcIds = DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public string sticker;
}
