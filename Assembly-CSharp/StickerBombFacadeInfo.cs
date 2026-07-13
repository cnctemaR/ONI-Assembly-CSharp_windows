using System;
using Database;

public class StickerBombFacadeInfo : IBlueprintInfo, IHasDlcRestrictions
{
	public string id { get; set; }

	public string name { get; set; }

	public string desc { get; set; }

	public PermitRarity rarity
	{
		get
		{
			return this.rarity_;
		}
	}

	public string animFile { get; set; }

	public StickerBombFacadeInfo(string id, string name, string desc, PermitRarity rarity, string animFile, string sticker, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
	{
		this.id = id;
		this.name = name;
		this.desc = desc;
		this.rarity_ = rarity;
		this.animFile = animFile;
		this.sticker = sticker;
		this.requiredDlcIds = requiredDlcIds;
		this.forbiddenDlcIds = forbiddenDlcIds;
	}

	public string[] GetRequiredDlcIds()
	{
		return this.requiredDlcIds;
	}

	public string[] GetForbiddenDlcIds()
	{
		return this.forbiddenDlcIds;
	}

	private readonly PermitRarity rarity_;

	public string sticker;

	public string[] requiredDlcIds;

	public string[] forbiddenDlcIds;
}
