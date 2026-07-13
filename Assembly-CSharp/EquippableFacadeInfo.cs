using System;
using Database;

public class EquippableFacadeInfo : IBlueprintInfo, IHasDlcRestrictions
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

	public EquippableFacadeInfo(string id, string name, string desc, PermitRarity rarity, string defID, string buildOverride, string animFile, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
	{
		this.id = id;
		this.name = name;
		this.desc = desc;
		this.rarity_ = rarity;
		this.defID = defID;
		this.buildOverride = buildOverride;
		this.animFile = animFile;
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

	public string buildOverride;

	public string defID;

	public string[] requiredDlcIds;

	public string[] forbiddenDlcIds;
}
