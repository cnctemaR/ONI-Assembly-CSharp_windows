using System;
using System.Collections.Generic;
using System.Diagnostics;
using Database;

[DebuggerDisplay("{id} - {name}")]
public class BuildingFacadeInfo : IBlueprintInfo, IHasDlcRestrictions
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

	public BuildingFacadeInfo(string id, string name, string desc, PermitRarity rarity, string prefabId, string animFile, Dictionary<string, string> workables = null, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null, Dictionary<string, string> data = null)
	{
		this.id = id;
		this.name = name;
		this.desc = desc;
		this.rarity_ = rarity;
		this.prefabId = prefabId;
		this.animFile = animFile;
		this.workables = workables;
		this.requiredDlcIds = requiredDlcIds;
		this.forbiddenDlcIds = forbiddenDlcIds;
		this.data = data;
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

	public string prefabId;

	public Dictionary<string, string> workables;

	public Dictionary<string, string> data;

	public string[] requiredDlcIds;

	public string[] forbiddenDlcIds;
}
